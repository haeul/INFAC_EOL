using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO; // File.Exists
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DHSTesterXL
{
    public partial class FormProduct
    {
        // ───────────────────── ZPL 생성 (좌상단 기준, 회전 없음) ─────────────────────
        private string BuildZplFromUi(int dpi = DefaultDpi)
        {
            int printWidthDots = MmToDots(_style.LabelWmm, dpi); // Print Width
            int labelLengthDots = MmToDots(_style.LabelHmm, dpi); // Label Length

            // 텍스트 페이로드
            string brand = _style.BrandText ?? "";
            string part = _style.PartText ?? "";
            string hw = GetGridText(RowKey.HW, _style.HWText ?? "");
            string sw = GetGridText(RowKey.SW, _style.SWText ?? "");
            string lot = GetGridText(RowKey.LOT, _style.LOTText ?? "");
            string sn = GetGridText(RowKey.SN, _style.SerialText ?? "");

            // 인쇄 설정
            int darkness = Clamp(AsInt(numPrintDarkness?.Value, 0), 0, 30);
            int printQuantity = Math.Max(1, AsInt(numPrintQty?.Value, 1));
            double inchesPerSecond;
            try { inchesPerSecond = Convert.ToDouble(numPrintSpeed?.Value ?? 0m); }
            catch { inchesPerSecond = 0.0; }

            var zplBuilder = new StringBuilder();
            zplBuilder.AppendLine("~SD" + darkness);
            zplBuilder.AppendLine("^XA");
            // 글꼴 매핑
            zplBuilder.AppendLine("^CW1,E:D2CODING-VER1.TTF");

            zplBuilder.AppendLine("^PW" + printWidthDots);
            zplBuilder.AppendLine("^LL" + labelLengthDots);
            zplBuilder.AppendLine("^LH0,0");
            const double PrintHeadYOffsetMm = 0.6;  // 출력이 약간 위로 가는 오차 보정
            int labelTopOffsetDots = MmToDots(PrintHeadYOffsetMm, dpi);
            zplBuilder.AppendLine("^LT" + labelTopOffsetDots);
            zplBuilder.AppendLine("^LS0");
            if (inchesPerSecond > 0) zplBuilder.AppendLine("^PR" + ((int)Math.Round(inchesPerSecond)));
            zplBuilder.AppendLine("^PQ" + printQuantity);

            // ───────────────────── 로고 (^GFA, 좌상단 기준) ─────────────────────
            if (_style.ShowLogoPrint && !string.IsNullOrWhiteSpace(_style.LogoImagePath) && File.Exists(ResolveLogoPath(_style.LogoImagePath)))
            {
                if (_logoBitmap == null) LoadLogoBitmap();
                if (_logoBitmap != null)
                {
                    var logoRow = GetRow(RowKey.Logo);
                    double logoScaleX = ReadScaleCell(logoRow, ColumnScaleX, 1.0);
                    double logoScaleY = ReadScaleCell(logoRow, ColumnScaleY, 1.0);

                    double logoAspectRatio = _logoBitmap.Width / (double)_logoBitmap.Height;
                    int logoHeightDots = Math.Max(1, MmToDots(_style.LogoH * logoScaleY, dpi));
                    int logoWidthDots = Math.Max(1, MmToDots(_style.LogoH * logoAspectRatio * logoScaleX, dpi));

                    string logoGraphicFieldData;
                    using (var canvas = new Bitmap(logoWidthDots, logoHeightDots))
                    using (var graphics = Graphics.FromImage(canvas))
                    {
                        graphics.Clear(Color.White);
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(_logoBitmap, new Rectangle(0, 0, logoWidthDots, logoHeightDots));
                        int bytesPerRow, rowCount;
                        logoGraphicFieldData = ToZplGFA(canvas, out bytesPerRow, out rowCount);
                    }

                    int logoOriginXDots = MmToDots(_style.LogoX, dpi);
                    int logoOriginYDots = MmToDots(_style.LogoY, dpi);
                    zplBuilder.AppendLine($"^FO{logoOriginXDots},{logoOriginYDots}{logoGraphicFieldData}^FS");
                }
            }

            // ───────────────────── Brand (텍스트, 좌상단 기준) ─────────────────────
            if (_style.ShowBrandPrint && !string.IsNullOrEmpty(brand))
            {
                var brandRow = GetRow(RowKey.Brand);
                double scaleX = ReadScaleCell(brandRow, ColumnScaleX, 1.0);
                double scaleY = ReadScaleCell(brandRow, ColumnScaleY, 1.0);

                int fontHeightDots = MmToDots(_style.BrandFont * scaleY, dpi);
                int fontWidthDots = (int)Math.Round(fontHeightDots * scaleX);

                int originXDots = MmToDots(_style.BrandX, dpi);
                int originYDots = MmToDots(_style.BrandY, dpi);

                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(brand)}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{w}^FD{Escape(brand)}^FS");
            }

            // ───────────────────── Part ─────────────────────
            if (_style.ShowPartPrint && !string.IsNullOrEmpty(part))
            {
                var partRow = GetRow(RowKey.Part);
                double scaleX = ReadScaleCell(partRow, ColumnScaleX, 1.0);
                double scaleY = ReadScaleCell(partRow, ColumnScaleY, 1.0);
                int fontHeightDots = MmToDots(_style.PartFont * scaleY, dpi);
                int fontWidthDots = (int)Math.Round(fontHeightDots * scaleX);

                if (_style.PartX <= 0)
                {
                    int partY = MmToDots(_style.PartY, dpi);
                    zplBuilder.AppendLine($"^FO0,{partY}^FB{printWidthDots},1,0,C^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(part)}^FS");
                    //zplBuilder.AppendLine($"^FO0,{partY}^FB{printWidthDots},1,0,C^A1N,{h},{w}^FD{Escape(part)}^FS");
                }
                else
                {
                    int originXDots = MmToDots(_style.PartX, dpi);
                    int originYDots = MmToDots(_style.PartY, dpi);
                    zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(part)}^FS");
                    //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{w}^FD{Escape(part)}^FS");
                }
            }

            // ───────────────────── HW ─────────────────────
            if (_style.ShowHWPrint && !string.IsNullOrEmpty(hw))
            {
                var hardwareRow = GetRow(RowKey.HW);
                double scaleX = ReadScaleCell(hardwareRow, ColumnScaleX, 1.0);
                double scaleY = ReadScaleCell(hardwareRow, ColumnScaleY, 1.0);

                int fontHeightDots = MmToDots(PositiveOr(_style.HWfont, 2.6) * scaleY, dpi);
                int fontWidthDots = (int)Math.Round(fontHeightDots * scaleX);

                int originXDots = MmToDots(_style.HWx, dpi);
                int originYDots = MmToDots(_style.HWy, dpi);

                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(hw)}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{w}^FD{Escape(hw)}^FS");
            }

            // ───────────────────── SW ─────────────────────
            if (_style.ShowSWPrint && !string.IsNullOrEmpty(sw))
            {
                var softwareRow = GetRow(RowKey.SW);
                double scaleX = ReadScaleCell(softwareRow, ColumnScaleX, 1.0);
                double scaleY = ReadScaleCell(softwareRow, ColumnScaleY, 1.0);

                int fontHeightDots = MmToDots(PositiveOr(_style.SWfont, 2.6) * scaleY, dpi);
                int fontWidthDots = (int)Math.Round(fontHeightDots * scaleX);

                int originXDots = MmToDots(_style.SWx, dpi);
                int originYDots = MmToDots(_style.SWy, dpi);

                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(sw)}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{w}^FD{Escape(sw)}^FS");
            }
            // LOT
            if (_style.ShowLOTPrint && !string.IsNullOrEmpty(lot))
            {
                var lotRow = GetRow(RowKey.LOT);
                double scaleX = ReadScaleCell(lotRow, ColumnScaleX, 1.0);
                double scaleY = ReadScaleCell(lotRow, ColumnScaleY, 1.0);

                int fontHeightDots = MmToDots(PositiveOr(_style.LOTfont, 2.6) * scaleY, dpi);
                int fontWidthDots = (int)Math.Round(fontHeightDots * scaleX);

                int originXDots = MmToDots(_style.LOTx, dpi);
                int originYDots = MmToDots(_style.LOTy, dpi);

                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(lot)}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{w}^FD{Escape(lot)}^FS");
            }
            // SN
            if (_style.ShowSNPrint && !string.IsNullOrEmpty(sn))
            {
                var serialNumberRow = GetRow(RowKey.SN);
                double scaleX = ReadScaleCell(serialNumberRow, ColumnScaleX, 1.0);
                double scaleY = ReadScaleCell(serialNumberRow, ColumnScaleY, 1.0);

                int fontHeightDots = MmToDots(PositiveOr(_style.SNfont, 2.6) * scaleY, dpi);
                int fontWidthDots = (int)Math.Round(fontHeightDots * scaleX);

                int originXDots = MmToDots(_style.SNx, dpi);
                int originYDots = MmToDots(_style.SNy, dpi);

                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(sn)}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{w}^FD{Escape(sn)}^FS");
            }

            // ───────────────────── Pb (^GE 타원 + 중앙 Pb) ─────────────────────
            if (_style.ShowPbPrint)
            {
                var leadFreeRow = GetRow(RowKey.Pb);
                double scaleX = ReadScaleCell(leadFreeRow, ColumnScaleX, 1.0);
                double scaleY = ReadScaleCell(leadFreeRow, ColumnScaleY, 1.0);

                int badgeWidthDots = Math.Max(1, MmToDots(_style.BadgeDiameter * scaleX, dpi));
                int badgeHeightDots = Math.Max(1, MmToDots(_style.BadgeDiameter * scaleY, dpi));
                int originXDots = MmToDots(_style.BadgeX, dpi);
                int originYDots = MmToDots(_style.BadgeY, dpi);

                int strokeThickness = 2;
                int fontHeightDots = (int)Math.Round(Math.Min(badgeWidthDots, badgeHeightDots) * 0.45);
                int fontWidthDots = fontHeightDots;

                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^GE{badgeWidthDots},{badgeHeightDots},{strokeThickness}^FS");
                int fieldBlockWidthDots = Math.Max(badgeWidthDots, badgeHeightDots);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^FB{fieldBlockWidthDots},1,0,C^A0N,{fontHeightDots},{fontWidthDots}^FDPb^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^FB{fbWidth},1,0,C^A1N,{fh},{fw}^FDPb^FS");
            }

            // ───────────────────── Data Matrix (좌상단 기준) ─────────────────────
            if (_style.ShowDMPrint)
            {
                // 모듈(셀) 1칸의 mm → 정수 도트
                int moduleDotsPerCell = Math.Max(1, MmToDots(Math.Max(0.1, _style.DMModuleMm), dpi));

                // 좌표
                int originXDots = MmToDots(_style.DMx, dpi);
                int originYDots = MmToDots(_style.DMy, dpi);

                // DM 데이터: DM 때 쓰던 "MA," 접두어는 제거 (Data Matrix엔 모드 접두어 불필요)
                string dataMatrixPayload = BuildEtcsQrPayloadFromUi();  // ^FH\ 로 해석될 데이터

                // (옵션) 그리드의 X/Y 스케일 칸을 DM 열/행으로 활용 (정수, 10~144). 범위 밖이면 자동.
                var dataMatrixRow = GetRow(RowKey.DM);
                int columnCount = (int)Math.Round(ReadScaleCell(dataMatrixRow, ColumnScaleX, 0.0));
                int rowCount = (int)Math.Round(ReadScaleCell(dataMatrixRow, ColumnScaleY, 0.0));
                string columnCountArgument = (columnCount >= 10 && columnCount <= 144) ? columnCount.ToString() : "";
                string rowCountArgument = (rowCount >= 10 && rowCount <= 144) ? rowCount.ToString() : "";

                // ^BX: N(정방향), h=모듈 도트, s=ECC(200=ECC200), c=열, r=행
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^BXN,{moduleDotsPerCell},200,{columnCountArgument},{rowCountArgument}");
                zplBuilder.AppendLine("^FH\\^FD" + dataMatrixPayload + "^FS");   // "MA," 붙이지 않기
            }



            // Rating
            if (_style.ShowRatingPrint)
            {
                int originXDots = MmToDots(_style.RatingX, dpi);
                int originYDots = MmToDots(_style.RatingY, dpi);
                int fontHeightDots = MmToDots(PositiveOr(_style.RatingFont, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(GetGridText(RowKey.Rating, _style.RatingText))}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{h}^FD{Escape(GetGridText(RowKey.Rating, _style.RatingText))}^FS");
            }

            // FCC ID
            if (_style.ShowFCCIDPrint)
            {
                int originXDots = MmToDots(_style.FCCIDX, dpi);
                int originYDots = MmToDots(_style.FCCIDY, dpi);
                int fontHeightDots = MmToDots(PositiveOr(_style.FCCIDFont, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(GetGridText(RowKey.FCCID, _style.FCCIDText))}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{h}^FD{Escape(GetGridText(RowKey.FCCID, _style.FCCIDText))}^FS");
            }

            // IC ID
            if (_style.ShowICIDPrint)
            {
                int originXDots = MmToDots(_style.ICIDX, dpi);
                int originYDots = MmToDots(_style.ICIDY, dpi);
                int fontHeightDots = MmToDots(PositiveOr(_style.ICIDFont, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(GetGridText(RowKey.ICID, _style.ICIDText))}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{h}^FD{Escape(GetGridText(RowKey.ICID, _style.ICIDText))}^FS");
            }

            // Item1
            if (_style.ShowItem1Print)
            {
                int originXDots = MmToDots(_style.Item1X, dpi);
                int originYDots = MmToDots(_style.Item1Y, dpi);
                int fontHeightDots = MmToDots(PositiveOr(_style.Item1Font, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(GetGridText(RowKey.Item1, _style.Item1Text))}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{h}^FD{Escape(GetGridText(RowKey.Item1, _style.Item1Text))}^FS");
            }
            // Item2
            if (_style.ShowItem2Print)
            {
                int originXDots = MmToDots(_style.Item2X, dpi);
                int originYDots = MmToDots(_style.Item2Y, dpi);
                int fontHeightDots = MmToDots(PositiveOr(_style.Item2Font, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(GetGridText(RowKey.Item2, _style.Item2Text))}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{h}^FD{Escape(GetGridText(RowKey.Item2, _style.Item2Text))}^FS");
            }
            // Item3
            if (_style.ShowItem3Print)
            {
                int originXDots = MmToDots(_style.Item3X, dpi);
                int originYDots = MmToDots(_style.Item3Y, dpi);
                int fontHeightDots = MmToDots(PositiveOr(_style.Item3Font, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(GetGridText(RowKey.Item3, _style.Item3Text))}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{h}^FD{Escape(GetGridText(RowKey.Item3, _style.Item3Text))}^FS");
            }
            // Item4
            if (_style.ShowItem4Print)
            {
                int originXDots = MmToDots(_style.Item4X, dpi);
                int originYDots = MmToDots(_style.Item4Y, dpi);
                int fontHeightDots = MmToDots(PositiveOr(_style.Item4Font, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(GetGridText(RowKey.Item4, _style.Item4Text))}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{h}^FD{Escape(GetGridText(RowKey.Item4, _style.Item4Text))}^FS");
            }
            // Item5
            if (_style.ShowItem5Print)
            {
                int originXDots = MmToDots(_style.Item5X, dpi);
                int originYDots = MmToDots(_style.Item5Y, dpi);
                int fontHeightDots = MmToDots(PositiveOr(_style.Item5Font, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(GetGridText(RowKey.Item5, _style.Item5Text))}^FS");
                //zplBuilder.AppendLine($"^FO{x},{y}^A1N,{h},{h}^FD{Escape(GetGridText(RowKey.Item5, _style.Item5Text))}^FS");
            }


            zplBuilder.AppendLine("^XZ");
            return zplBuilder.ToString();
        }

        // ───────────────────── H/KMC ETCS DM 페이로드 빌더 ─────────────────────
        private string BuildEtcsQrPayloadFromUi()
        {
            // UI 값(오른쪽 큰 칸: Value) 읽기
            string vendorValue = (txtEtcsVendorValue?.Text ?? "").Trim();          // V
            string partNumberValue = (txtEtcsPartNoValue?.Text ?? "").Trim();      // P
            string serialValue = (txtEtcsSerialValue?.Text ?? "").Trim();          // S (옵션)
            string eoValue = (txtEtcsEoValue?.Text ?? "").Trim();                  // E (옵션)
            string traceValue = (txtEtcsTraceValue?.Text ?? "").Trim();            // T = YYMMDD + ... + 4M + 7자리
            string specialValue = (txtEtcsSpecialValue?.Text ?? "").Trim();        // 1A (옵션 확장)
            string initialValue = (txtEtcsInitialValue?.Text ?? "").Trim();        // M  (옵션 확장)
            string companyAreaValue = (txtEtcsCompanyAreaValue?.Text ?? "").Trim(); // C  (옵션 확장)

            // 제어코드(백슬래시-헥스 표기) - ^FH\ 가 해석함
            const string GroupSeparator = @"\1D";
            const string RecordSeparator = @"\1E";
            const string EndOfTransmission = @"\04";

            var payloadBuilder = new System.Text.StringBuilder(256);
            payloadBuilder.Append("[)>");                   // 심볼 식별자
            payloadBuilder.Append(RecordSeparator).Append("06");        // 버전

            payloadBuilder.Append(GroupSeparator).Append("V").Append(vendorValue);
            payloadBuilder.Append(GroupSeparator).Append("P").Append(partNumberValue);

            // S/E는 비어도 칸(=GroupSeparator) 자체는 유지 → 필드 밀림 방지
            payloadBuilder.Append(GroupSeparator);
            //if (!string.IsNullOrEmpty(s))
            payloadBuilder.Append("S").Append(serialValue);

            payloadBuilder.Append(GroupSeparator);
            //if (!string.IsNullOrEmpty(e))
            payloadBuilder.Append("E").Append(eoValue);

            // T (필수) - UI에서 한 칸으로 받음(yyMMdd + 4M + 7자리 등)
            payloadBuilder.Append(GroupSeparator).Append("T").Append(traceValue).Append(specialValue).Append(initialValue).Append(companyAreaValue);

            // 트레일러
            payloadBuilder.Append(GroupSeparator).Append(RecordSeparator).Append(EndOfTransmission);
            return payloadBuilder.ToString();
        }

        // \hh(16진) 시퀀스를 실제 바이트로 변환 (^FH\ 해석)
        private static byte[] DecodeZplFh(string s)
        {
            using (var ms = new MemoryStream(s.Length))
            {
                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];
                    if (c == '\\' && i + 2 < s.Length &&
                        Uri.IsHexDigit(s[i + 1]) && Uri.IsHexDigit(s[i + 2]))
                    {
                        ms.WriteByte(Convert.ToByte(s.Substring(i + 1, 2), 16));
                        i += 2;
                    }
                    else ms.WriteByte((byte)c);
                }
                return ms.ToArray();
            }
        }

        // DM(ECC200) 정방형 심볼 용량표(바이너리 기준, 일부)
        private static readonly (int bytes, int modules)[] DataMatrixCapacityBySymbol = new[]
        {
            (6,10),(10,12),(16,14),(24,16),(36,18),(44,20),
            (60,22),(72,24),(88,26),(120,32),(164,36),(224,40),(280,44)
            // 필요하면 48×48 … 144×144 추가
        };

        // ── 현재 UI 기준 DM 모듈 수(행/열) 결정: 지정값 우선, 없으면 용량표로 자동 추정
        private int GetCurrentDmModulesFromUiOrAuto()
        {
            var dataMatrixRow = GetRow(RowKey.DM);
            int columnCount = (int)Math.Round(ReadScaleCell(dataMatrixRow, ColumnScaleX, 0.0));
            int rowCount = (int)Math.Round(ReadScaleCell(dataMatrixRow, ColumnScaleY, 0.0));
            if (columnCount >= 10 && columnCount <= 144 && rowCount >= 10 && rowCount <= 144)
                return Math.Max(columnCount, rowCount); // 정방형 기준

            int payloadLengthBytes = DecodeZplFh(BuildEtcsQrPayloadFromUi()).Length;
            foreach (var capacity in DataMatrixCapacityBySymbol)
                if (payloadLengthBytes <= capacity.bytes) return capacity.modules;
            return 144; // 상한
        }

        // ───────────────────── DM(정방형) 후보 모듈 목록 ─────────────────────
        private static readonly int[] DataMatrixSquareModuleCandidates = { 10,12,14,16,18,20,22,24,26,32,36,40,44,48,52,64,72,80,88,96,104,120,132,144 };

        // 현재 데이터(ETCS) 길이로 '필요 최소 모듈 수' 계산
        private int GetMinDmModulesNeeded()
        {
            int len = DecodeZplFh(BuildEtcsQrPayloadFromUi()).Length;
            foreach (var capacity in DataMatrixCapacityBySymbol)
                if (len <= capacity.bytes) return capacity.modules;
            return 144;
        }

        // 목표 mm에 가장 근접한 (M, h, 실제 한 변 mm) 선택
        private (int moduleCount, int moduleHeightDots, double sideMmActual) AutoPickDmByTarget(double targetMm, int dpi)
        {
            double mmPerDot = 25.4 / dpi;
            int dotsTarget = Math.Max(1, (int)Math.Round(targetMm / mmPerDot));

            int minM = GetMinDmModulesNeeded();
            int bestM = minM, bestH = 1;
            double bestSide = bestM * bestH * mmPerDot;
            double bestErr = Math.Abs(bestSide - targetMm);

            foreach (int moduleCandidate in DataMatrixSquareModuleCandidates)
            {
                if (moduleCandidate < minM) continue;
                int heightCandidate = Math.Max(1, (int)Math.Round((double)dotsTarget / moduleCandidate));
                double side = moduleCandidate * heightCandidate * mmPerDot;
                double err = Math.Abs(side - targetMm);
                if (err < bestErr)
                {
                    bestErr = err; bestM = moduleCandidate; bestH = heightCandidate; bestSide = side;
                }
            }
            return (bestM, bestH, bestSide);
        }


        // Font 테스트용 헬퍼
        private string BuildZplTemplateWithAZ(int dpi = DefaultDpi)
        {
            // 1) 레이아웃 그대로 ZPL 생성
            var zpl = BuildZplFromUi(dpi);

            // 2) 헤더에 박혀 있는 임의 매핑(^CW...)은 제거 (나중에 폰트별로 다시 넣을 것)
            zpl = Regex.Replace(zpl, @"^\^CW.*\r?\n", "", RegexOptions.Multiline);

            // 3) 본문 폰트호출을 전부 Z 폰트로 통일
            //    ^A0N/^A0R/^A0I/^A0B → ^AZN/^AZR/^AZI/^AZB
            zpl = Regex.Replace(zpl, @"\^A0(?=[NRBI])", "^AZ");
            //    바꿔둔 ^A1N...도 있을 수 있으니 같이 교체
            zpl = Regex.Replace(zpl, @"\^A1(?=[NRBI])", "^AZ");

            // 4) 수량은 1장으로 고정
            zpl = Regex.Replace(zpl, @"\^PQ\d+", "^PQ1");

            return zpl;
        }
        private string BuildZplForFont(string ttfPath, int dpi = DefaultDpi)
        {
            string tpl = BuildZplTemplateWithAZ(dpi);

            int xa = tpl.IndexOf("^XA", StringComparison.Ordinal);
            if (xa >= 0)
            {
                int insertPos = xa + 3;
                string header =
                    "\n^CWZ," + ttfPath + "\n" +
                    "^FO10,10^AZN,24,24^FD" + ttfPath + "^FS\n";
                tpl = tpl.Insert(insertPos, header);
            }
            return tpl;
        }


        // 테스트할 TTF 목록
        private static readonly string[] _ttfFontsOnPrinter = new[]
        {
            "E:ROBOTOMONO-EXTRA.TTF",
            "E:ROBOTOMONO-LIGHT.TTF",
            "E:ROBOTOMONO.TTF",
            "E:ROBOTO.TTF",
            "E:LUCON.TTF",
            "E:CONSOLA.TTF",
            "E:CONSOLAB.TTF",
            "E:OCRAEXT.TTF",
            "E:D2CODING-VER1.TTF",
            "E:D2CODINGBOLD-VER.TTF",
            //"E:CG_TIMES.TTF",
            //"E:CG_TRIUMVIRATE.TTF",
            //"E:EFONT_A.TTF",
            //"E:EFONT_B.TTF",
            //"E:EFONT_C.TTF",
            //"E:M_BOLD.TTF",
            //"E:M_CG_6PT.TTF",
            //"E:M_CG_BOLD.TTF",
            //"E:REDUCED.TTF",
            //"E:STANDARD.TTF",
            //"E:HELVETICA_A.TTF",
            //"E:HELVETICA_B.TTF",
            "E:TT0003M_.TTF", // SWISS 721
            // 필요 시 추가 (R:*.TTF 있으면 여기에 "R:파일명.TTF")
        };

        // 한번에 모두 출력
        private void PrintAllTtfSamples(string printerName, int dpi = DefaultDpi)
        {
            var all = new StringBuilder(4096);

            foreach (var ttf in _ttfFontsOnPrinter)
            {
                string zpl = BuildZplForFont(ttf, dpi);
                all.AppendLine(zpl);
            }

            // 한 번에 전송
            LabelPrinter.SendRawToPrinter(printerName, all.ToString());
        }
        // Font 헬퍼
        private static string FontTTF(int h, int w) => $"^A@N,{h},{w},E:D2CODING-VER1.TTF";

        /// <summary>
        /// Bitmap → ZPL ^GFA (ASCII HEX).
        /// src는 어떤 PixelFormat이든 OK. 32bpp로 그린 뒤 임계값 이진화하여 1bpp로 패킹.
        /// </summary>
        private static string ToZplGFA(Bitmap src, out int bytesPerRow, out int rows)
        {
            using (var rgba = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(rgba))
                {
                    g.Clear(Color.White);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(src, 0, 0, rgba.Width, rgba.Height);
                }

                rows = rgba.Height;
                bytesPerRow = (rgba.Width + 7) / 8;

                var rect = new Rectangle(0, 0, rgba.Width, rgba.Height);
                var bd = rgba.LockBits(rect,
                                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                       System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try
                {
                    int stride = bd.Stride;
                    int width = rgba.Width;
                    int height = rgba.Height;

                    var rowBuf = new byte[stride];
                    var zplBuilder = new StringBuilder(rows * bytesPerRow * 2 + rows);

                    const int BrightnessThreshold = 200; // 밝기 임계값

                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(IntPtr.Add(bd.Scan0, y * stride), rowBuf, 0, stride);

                        int bitPos = 7;
                        byte packed = 0;
                        int writtenBytes = 0;

                        for (int x = 0; x < width; x++)
                        {
                            int px = x * 4;
                            byte b = rowBuf[px + 0];
                            byte gg = rowBuf[px + 1];
                            byte r = rowBuf[px + 2];

                            int lum = (r * 299 + gg * 587 + b * 114) / 1000;
                            bool isBlack = lum < BrightnessThreshold;

                            if (isBlack) packed |= (byte)(1 << bitPos);

                            bitPos--;
                            if (bitPos < 0)
                            {
                                zplBuilder.Append(packed.ToString("X2"));
                                packed = 0;
                                bitPos = 7;
                                writtenBytes++;
                            }
                        }

                        if (bitPos != 7)
                        {
                            zplBuilder.Append(packed.ToString("X2"));
                            writtenBytes++;
                        }
                        for (; writtenBytes < bytesPerRow; writtenBytes++)
                            zplBuilder.Append("00");

                        zplBuilder.Append('\n');
                    }

                    int totalBytes = bytesPerRow * rows;
                    return $"^GFA,{totalBytes},{totalBytes},{bytesPerRow},\n{zplBuilder}";
                }
                finally
                {
                    rgba.UnlockBits(bd);
                }
            }
        }

        // ───────────────────── 공통 유틸 ─────────────────────
        private static int MmToDots(double mm, int dpi) => (int)Math.Round(mm * dpi / 25.4);
        private static string Escape(string s) => s?.Replace("^", "") ?? "";
        private static double PositiveOr(double v, double fallback) => v > 0 ? v : fallback;

        private static string KeepDigits(string raw, int take)
        {
            if (string.IsNullOrEmpty(raw)) return "";
            var d = new string(raw.Where(char.IsDigit).ToArray());
            if (d.Length >= take) return d.Substring(0, take);
            return d.PadLeft(take, '0');
        }

        // DM 페이로드(그리드 데이터 모두 포함, key=value|... 형태)
        private string BuildQrPayloadFromGrid()
        {   /*
            string brand = GetGridText(RowKey.Brand, _style.BrandText ?? "");
            string part = GetGridText(RowKey.Part, _style.PartText ?? "");
            string hw = GetGridText(RowKey.HW, _style.HWText ?? "");
            string sw = GetGridText(RowKey.SW, _style.SWText ?? "");
            string lot = GetGridText(RowKey.LOT, _style.LOTText ?? "");
            string sn = GetGridText(RowKey.SN, _style.SerialText ?? "");
            string logo = Path.GetFileName(_style.LogoImagePath ?? "");

            var zplBuilder = new StringBuilder(256);
            void Add(string k, string v)
            {
                v = AsciiSafeOneLine(v ?? "");
                if (zplBuilder.Length > 0) zplBuilder.Append('|');
                zplBuilder.Append(k).Append('=').Append(v);
            }

            Add("BRAND", brand);
            Add("PART", part);
            Add("HW", hw);
            Add("SW", sw);
            Add("LOT", lot);
            Add("SN", sn);
            if (!string.IsNullOrWhiteSpace(logo)) Add("LOGO", logo);
            */
            return BuildEtcsQrPayloadFromUi();
        }

        private static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);
        private static int AsInt(object v, int fallback = 0)
        {
            try
            {
                if (v == null) return fallback;
                if (v is decimal dec) return (int)Math.Round(dec);
                if (v is int i) return i;
                if (int.TryParse(v.ToString(), out var parsed)) return parsed;
                return fallback;
            }
            catch { return fallback; }
        }

        private static string AsciiSafeOneLine(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var zplBuilder = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (ch == '\r' || ch == '\n' || ch == '\t') continue;
                zplBuilder.Append((ch >= 32 && ch <= 126) ? ch : '_');
            }
            return zplBuilder.ToString();
        }
    }
    /// <summary>
    /// UI에 의존하지 않는 순수 ZPL 빌더 (LabelStyle + LabelPayload만 사용)
    /// - ScaleX/ScaleY, DM Cols/Rows 같은 필드 의존 제거(현재 LabelStyle에 없음)
    /// </summary>
    public static class ZebraZplFacade
    {
        private static int MmToDots(double mm, int dpi) => (int)Math.Round(mm * dpi / 25.4);
        private static string Escape(string s) => (s ?? string.Empty).Replace("^", "^^");

        private static string ToZplGFA(Bitmap bmp, out int bytesPerRow, out int rows)
        {
            using (var mono = new Bitmap(bmp.Width, bmp.Height))
            using (var g = Graphics.FromImage(mono))
            {
                g.Clear(Color.White);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, mono.Width, mono.Height));

                rows = mono.Height;
                bytesPerRow = (mono.Width + 7) / 8;

                var hex = new StringBuilder(rows * bytesPerRow * 2);
                for (int y = 0; y < mono.Height; y++)
                {
                    int bit = 0, cur = 0;
                    for (int x = 0; x < mono.Width; x++)
                    {
                        var c = mono.GetPixel(x, y);
                        int v = (c.R + c.G + c.B) / 3 < 128 ? 1 : 0; // 1=검정
                        cur = (cur << 1) | v;
                        bit++;
                        if (bit == 8) { hex.Append(cur.ToString("X2")); bit = 0; cur = 0; }
                    }
                    if (bit != 0) { cur <<= (8 - bit); hex.Append(cur.ToString("X2")); }
                }

                int totalBytes = bytesPerRow * rows;
                return $"^GFA,{totalBytes},{totalBytes},{bytesPerRow},{hex}";
            }
        }

        private static string BuildEtcsDm(EtcsSettings d)
        {
            if (d == null) return null;

            // 최소 필수: Vendor, PartNo, Trace
            if (string.IsNullOrWhiteSpace(d.Vendor) ||
                string.IsNullOrWhiteSpace(d.PartNo) ||
                string.IsNullOrWhiteSpace(d.Trace))
                return null;

            const string GroupSeparator = @"\1D";
            const string RecordSeparator = @"\1E";
            const string EndOfTransmission = @"\04";

            var v = (d.Vendor ?? "").Trim();  // V
            var p = (d.PartNo ?? "").Trim();  // P
            var s = (d.Serial ?? "").Trim();  // S (opt)
            var e = (d.Eo ?? "").Trim();      // E (opt)
            var t = (d.Trace ?? "").Trim();   // T (필수)
            var a1 = (d.A1 ?? "").Trim();      // 1A (opt)
            var m = (d.M ?? "").Trim();       // M (opt)
            var c = (d.C ?? "").Trim();       // C (opt)

            var zplBuilder = new StringBuilder(256);
            zplBuilder.Append("[)>");            // 심볼 식별자
            zplBuilder.Append(RecordSeparator).Append("06"); // 버전

            zplBuilder.Append(GroupSeparator).Append("V").Append(v);
            zplBuilder.Append(GroupSeparator).Append("P").Append(p);

            // S/E는 비어도 GroupSeparator 토큰은 남겨 필드 밀림 방지
            zplBuilder.Append(GroupSeparator);
            //if (!string.IsNullOrEmpty(s))
            zplBuilder.Append("S").Append(s);

            zplBuilder.Append(GroupSeparator);
            //if (!string.IsNullOrEmpty(e))
            zplBuilder.Append("E").Append(e);

            zplBuilder.Append(GroupSeparator).Append("T").Append(t).Append(a1).Append(m).Append(c);

            zplBuilder.Append(GroupSeparator).Append(RecordSeparator).Append(EndOfTransmission);
            return zplBuilder.ToString();
        }

        public static string BuildZpl(
            LabelStyle labelStyle,
            LabelPayload payload,
            EtcsSettings etcs,
            int dpi,
            int printQuantity,
            int printDarkness,
            double printSpeedInchesPerSecond,
            Func<Bitmap> loadLogoBitmap // 없으면 null 리턴
        )
        {
            if (labelStyle == null) throw new ArgumentNullException(nameof(labelStyle));

            int printWidthDots = MmToDots(labelStyle.LabelWmm, dpi);
            int labelLengthDots = MmToDots(labelStyle.LabelHmm, dpi);

            var zplBuilder = new StringBuilder(2048);
            zplBuilder.AppendLine("~SD" + Clamp(printDarkness, 0, 30));
            zplBuilder.AppendLine("^XA");
            zplBuilder.AppendLine("^PW" + printWidthDots);
            zplBuilder.AppendLine("^LL" + labelLengthDots);
            zplBuilder.AppendLine("^LH0,0");
            const double PrintHeadYOffsetMm = 0.6;
            zplBuilder.AppendLine("^LT" + MmToDots(PrintHeadYOffsetMm, dpi));
            zplBuilder.AppendLine("^LS0");
            if (printSpeedInchesPerSecond > 0) zplBuilder.AppendLine("^PR" + (int)Math.Round(printSpeedInchesPerSecond));
            zplBuilder.AppendLine("^PQ" + Math.Max(1, printQuantity));

            // ── 로고 (^GFA) : Style에 이미지 경로만 있고 스케일 속성 없으므로, 높이만 사용
            if (labelStyle.ShowLogoPrint && !string.IsNullOrWhiteSpace(labelStyle.LogoImagePath))
            {
                try
                {
                    using (var sourceLogoBitmap = loadLogoBitmap?.Invoke())
                    {
                        if (sourceLogoBitmap != null)
                        {
                            double logoAspectRatio = sourceLogoBitmap.Width / (double)sourceLogoBitmap.Height;
                            int logoHeightDots = Math.Max(1, MmToDots(labelStyle.LogoH, dpi));
                            int logoWidthDots = Math.Max(1, (int)Math.Round(logoHeightDots * logoAspectRatio));

                            using (var canvas = new Bitmap(logoWidthDots, logoHeightDots))
                            using (var graphics = Graphics.FromImage(canvas))
                            {
                                graphics.Clear(Color.White);
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.DrawImage(sourceLogoBitmap, new Rectangle(0, 0, logoWidthDots, logoHeightDots));
                                int bytesPerRow, rowCount;
                                string logoGraphicFieldData = ToZplGFA(canvas, out bytesPerRow, out rowCount);
                                int logoOriginXDots = MmToDots(labelStyle.LogoX, dpi);
                                int logoOriginYDots = MmToDots(labelStyle.LogoY, dpi);
                                zplBuilder.AppendLine($"^FO{logoOriginXDots},{logoOriginYDots}{logoGraphicFieldData}^FS");
                            }
                        }
                    }
                }
                catch { /* 로고 실패는 무시 */ }
            }

            // ── Brand
            if (labelStyle.ShowBrandPrint && !string.IsNullOrEmpty(payload?.Company))
            {
                int fontHeightDots = MmToDots(PositiveOr(labelStyle.BrandFont, 2.6), dpi);
                int fontWidthDots = fontHeightDots; // ScaleX 없음 → 폭=높이로 처리
                int originXDots = MmToDots(labelStyle.BrandX, dpi);
                int originYDots = MmToDots(labelStyle.BrandY, dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(payload.Company)}^FS");
            }

            // ── Part (X<=0 → 중앙정렬 규칙은 유지)
            if (labelStyle.ShowPartPrint && !string.IsNullOrEmpty(payload?.PartNo))
            {
                int fontHeightDots = MmToDots(PositiveOr(labelStyle.PartFont, 2.6), dpi);
                int fontWidthDots = fontHeightDots;
                if (labelStyle.PartX <= 0)
                {
                    int originYDots = MmToDots(labelStyle.PartY, dpi);
                    zplBuilder.AppendLine($"^FO0,{originYDots}^FB{printWidthDots},1,0,C^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(payload.PartNo)}^FS");
                }
                else
                {
                    int originXDots = MmToDots(labelStyle.PartX, dpi);
                    int originYDots = MmToDots(labelStyle.PartY, dpi);
                    zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(payload.PartNo)}^FS");
                }
            }

            // ── HW/SW/LOT/SN (Scale 없음 → 폰트 높이만 사용)
            PrintText(zplBuilder, labelStyle.ShowHWPrint, labelStyle.HWx, labelStyle.HWy, labelStyle.HWfont, payload?.HW, dpi);
            PrintText(zplBuilder, labelStyle.ShowSWPrint, labelStyle.SWx, labelStyle.SWy, labelStyle.SWfont, payload?.SW, dpi);
            PrintText(zplBuilder, labelStyle.ShowLOTPrint, labelStyle.LOTx, labelStyle.LOTy, labelStyle.LOTfont, payload?.LOT, dpi);
            PrintText(zplBuilder, labelStyle.ShowSNPrint, labelStyle.SNx, labelStyle.SNy, labelStyle.SNfont, payload?.SN, dpi);

            // ── Pb 배지 : 지름만 존재 → 타원 크기 w=h로
            if (labelStyle.ShowPbPrint)
            {
                int badgeDiameterDots = Math.Max(1, MmToDots(labelStyle.BadgeDiameter, dpi));
                int originXDots = MmToDots(labelStyle.BadgeX, dpi);
                int originYDots = MmToDots(labelStyle.BadgeY, dpi);
                int strokeThickness = 2;
                int fontHeightDots = (int)Math.Round(badgeDiameterDots * 0.45);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^GE{badgeDiameterDots},{badgeDiameterDots},{strokeThickness}^FS");
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^FB{badgeDiameterDots},1,0,C^A0N,{fontHeightDots},{fontHeightDots}^FDPb^FS");
            }

            // ── Data Matrix (^BX) : Cols/Rows 속성 없음 → 기본값으로 단순 출력
            string dataMatrixPayload = string.IsNullOrWhiteSpace(payload?.DataMatrix)
                            ? BuildEtcsDm(etcs) // 2) 없으면 ETCS로 자동 생성
                            : payload.DataMatrix;

            if (labelStyle.ShowDMPrint && !string.IsNullOrWhiteSpace(dataMatrixPayload))
            {
                int moduleDotsPerCell = Math.Max(1, MmToDots(Math.Max(0.1, labelStyle.DMModuleMm), dpi));
                int originXDots = MmToDots(labelStyle.DMx, dpi);
                int originYDots = MmToDots(labelStyle.DMy, dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^BXN,{moduleDotsPerCell},200");
                zplBuilder.AppendLine("^FH\\^FD" + dataMatrixPayload + "^FS"); // ^FH\가 \1D 등 제어코드 해석
            }
            // ── Rating / FCC / IC
            if (labelStyle.ShowRatingPrint && !string.IsNullOrEmpty(labelStyle.RatingText))
            {
                int originXDots = MmToDots(labelStyle.RatingX, dpi);
                int originYDots = MmToDots(labelStyle.RatingY, dpi);
                int fontHeightDots = MmToDots(PositiveOr(labelStyle.RatingFont, 2.6), dpi);
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(labelStyle.RatingText)}^FS");
            }
            if (labelStyle.ShowFCCIDPrint && !string.IsNullOrEmpty(payload?.FCCID ?? labelStyle.FCCIDText))
            {
                int originXDots = MmToDots(labelStyle.FCCIDX, dpi);
                int originYDots = MmToDots(labelStyle.FCCIDY, dpi);
                int fontHeightDots = MmToDots(PositiveOr(labelStyle.FCCIDFont, 2.6), dpi);
                string textToPrint = string.IsNullOrEmpty(payload?.FCCID) ? labelStyle.FCCIDText : payload.FCCID;
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(textToPrint)}^FS");
            }
            if (labelStyle.ShowICIDPrint && !string.IsNullOrEmpty(payload?.ICID ?? labelStyle.ICIDText))
            {
                int originXDots = MmToDots(labelStyle.ICIDX, dpi);
                int originYDots = MmToDots(labelStyle.ICIDY, dpi);
                int fontHeightDots = MmToDots(PositiveOr(labelStyle.ICIDFont, 2.6), dpi);
                string textToPrint = string.IsNullOrEmpty(payload?.ICID) ? labelStyle.ICIDText : payload.ICID;
                zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontHeightDots}^FD{Escape(textToPrint)}^FS");
            }

            zplBuilder.AppendLine("^XZ");
            return zplBuilder.ToString();
        }

        private static void PrintText(StringBuilder zplBuilder, bool shouldPrint,
                                      double xPositionMm, double yPositionMm, double fontHeightMm,
                                      string text, int dpi)
        {
            if (!shouldPrint || string.IsNullOrWhiteSpace(text)) return;
            int fontHeightDots = MmToDots(PositiveOr(fontHeightMm, 2.6), dpi);
            int fontWidthDots = fontHeightDots;
            int originXDots = MmToDots(xPositionMm, dpi);
            int originYDots = MmToDots(yPositionMm, dpi);
            zplBuilder.AppendLine($"^FO{originXDots},{originYDots}^A0N,{fontHeightDots},{fontWidthDots}^FD{Escape(text)}^FS");
        }

        private static double PositiveOr(double value, double fallback) => (value > 0) ? value : fallback;
        private static int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;
    }
}

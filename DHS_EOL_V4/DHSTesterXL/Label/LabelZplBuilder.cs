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
        private string BuildZplFromUi(int dpi = DEFAULT_DPI)
        {
            int PW = MmToDots(_style.LabelWmm, dpi); // Print Width
            int LL = MmToDots(_style.LabelHmm, dpi); // Label Length

            // Offset (mm 단위) 읽기: Apply 안 눌러도 출력에 반영되도록
            double offsetX = 0.0;
            double offsetY = 0.0;
            try { offsetX = Convert.ToDouble(nudOffsetX?.Value ?? 0m); } catch { offsetX = 0.0; }
            try { offsetY = Convert.ToDouble(nudOffsetY?.Value ?? 0m); } catch { offsetY = 0.0; }

            // 텍스트 페이로드
            string brand = _style.BrandText ?? "";
            string part = _style.PartText ?? "";
            string hw = GetGridText(RowKey.HW, _style.HWText ?? "");
            string sw = GetGridText(RowKey.SW, _style.SWText ?? "");
            string lot = GetGridText(RowKey.LOT, _style.LOTText ?? "");
            string sn = GetGridText(RowKey.SN, _style.SerialText ?? "");

            // 인쇄 설정
            int darkness = Clamp(AsInt(numPrintDarkness?.Value, 0), 0, 30);
            int printQauantity = Math.Max(1, AsInt(numPrintQty?.Value, 1));
            double inchesPerSecond;
            try { inchesPerSecond = Convert.ToDouble(numPrintSpeed?.Value ?? 0m); }
            catch { inchesPerSecond = 0.0; }

            var sb = new StringBuilder();
            sb.AppendLine("~SD" + darkness);
            sb.AppendLine("^XA");
            // 글꼴 매핑
            //sb.AppendLine("^CW1,E:D2CODING-VER1.TTF");

            sb.AppendLine("^PW" + PW);
            sb.AppendLine("^LL" + LL);
            sb.AppendLine("^LH0,0");
            const double NUDGE_Y_MM = 0.6;  // 출력이 약간 위로 가는 오차 보정
            int lt = MmToDots(NUDGE_Y_MM, dpi);
            sb.AppendLine("^LT" + lt);
            sb.AppendLine("^LS0");
            if (inchesPerSecond > 0) sb.AppendLine("^PR" + ((int)Math.Round(inchesPerSecond)));
            sb.AppendLine("^PQ" + printQauantity);

            // ───────────────────── 로고 (^GFA, 좌상단 기준) ─────────────────────
            if (_style.ShowLogoPrint && !string.IsNullOrWhiteSpace(_style.LogoImagePath) && File.Exists(ResolveLogoPath(_style.LogoImagePath)))
            {
                if (_logoBitmap == null) LoadLogoBitmap();
                if (_logoBitmap != null)
                {
                    var rLogo = GetRow(RowKey.Logo);
                    double sx = ReadScaleCell(rLogo, COL_XSCALE, 1.0);
                    double sy = ReadScaleCell(rLogo, COL_YSCALE, 1.0);

                    double aspect = _logoBitmap.Width / (double)_logoBitmap.Height;
                    int logoH = Math.Max(1, MmToDots(_style.LogoH * sy, dpi));
                    int logoW = Math.Max(1, MmToDots(_style.LogoH * aspect * sx, dpi));

                    string logoGraphicFieldData;
                    using (var canvas = new Bitmap(logoW, logoH))
                    using (var graphics = Graphics.FromImage(canvas))
                    {
                        graphics.Clear(Color.White);
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(_logoBitmap, new Rectangle(0, 0, logoW, logoH));
                        int bytePerRow, rowCount;
                        logoGraphicFieldData = ToZplGFA(canvas, out bytePerRow, out rowCount);
                    }

                    int x = MmToDots(_style.LogoX + offsetX, dpi);
                    int y = MmToDots(_style.LogoY + offsetY, dpi);
                    sb.AppendLine($"^FO{x},{y}{logoGraphicFieldData}^FS");
                }
            }

            // ───────────────────── Brand (텍스트, 좌상단 기준) ─────────────────────
            if (_style.ShowBrandPrint && !string.IsNullOrEmpty(brand))
            {
                var r = GetRow(RowKey.Brand);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                int h = MmToDots(_style.BrandFont * sy, dpi);
                int w = (int)Math.Round(h * sx);

                int x = MmToDots(_style.BrandX + offsetX, dpi);
                int y = MmToDots(_style.BrandY + offsetY, dpi);

                sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(brand)}^FS");
            }

            // ───────────────────── Part ─────────────────────
            if (_style.ShowPartPrint && !string.IsNullOrEmpty(part))
            {
                var r = GetRow(RowKey.Part);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                int h = MmToDots(_style.PartFont * sy, dpi);
                int w = (int)Math.Round(h * sx);

                if (_style.PartX <= 0)
                {
                    // 중앙 정렬: X는 0 고정, Y에만 offset 적용
                    int partY = MmToDots(_style.PartY + offsetY, dpi);
                    sb.AppendLine($"^FO0,{partY}^FB{PW},1,0,C^A0N,{h},{w}^FD{Escape(part)}^FS");
                }
                else
                {
                    int x = MmToDots(_style.PartX + offsetX, dpi);
                    int y = MmToDots(_style.PartY + offsetY, dpi);
                    sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(part)}^FS");
                }
            }

            // ───────────────────── HW ─────────────────────
            if (_style.ShowHWPrint && !string.IsNullOrEmpty(hw))
            {
                var r = GetRow(RowKey.HW);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                int h = MmToDots(PositiveOr(_style.HWfont, 2.6) * sy, dpi);
                int w = (int)Math.Round(h * sx);

                int x = MmToDots(_style.HWx + offsetX, dpi);
                int y = MmToDots(_style.HWy + offsetY, dpi);

                sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(hw)}^FS");
            }

            // ───────────────────── SW ─────────────────────
            if (_style.ShowSWPrint && !string.IsNullOrEmpty(sw))
            {
                var r = GetRow(RowKey.SW);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                int h = MmToDots(PositiveOr(_style.SWfont, 2.6) * sy, dpi);
                int w = (int)Math.Round(h * sx);

                int x = MmToDots(_style.SWx + offsetX, dpi);
                int y = MmToDots(_style.SWy + offsetY, dpi);

                sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(sw)}^FS");
            }

            // LOT
            if (_style.ShowLOTPrint && !string.IsNullOrEmpty(lot))
            {
                var r = GetRow(RowKey.LOT);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                int h = MmToDots(PositiveOr(_style.LOTfont, 2.6) * sy, dpi);
                int w = (int)Math.Round(h * sx);

                int x = MmToDots(_style.LOTx + offsetX, dpi);
                int y = MmToDots(_style.LOTy + offsetY, dpi);

                sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(lot)}^FS");
            }

            // SN
            if (_style.ShowSNPrint && !string.IsNullOrEmpty(sn))
            {
                var r = GetRow(RowKey.SN);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                int h = MmToDots(PositiveOr(_style.SNfont, 2.6) * sy, dpi);
                int w = (int)Math.Round(h * sx);

                int x = MmToDots(_style.SNx + offsetX, dpi);
                int y = MmToDots(_style.SNy + offsetY, dpi);

                sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(sn)}^FS");
            }

            // ───────────────────── Pb (^GE 타원 + 중앙 Pb) ─────────────────────
            if (_style.ShowPbPrint)
            {
                var r = GetRow(RowKey.Pb);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                int w = Math.Max(1, MmToDots(_style.BadgeDiameter * sx, dpi));
                int h = Math.Max(1, MmToDots(_style.BadgeDiameter * sy, dpi));
                int x = MmToDots(_style.BadgeX + offsetX, dpi);
                int y = MmToDots(_style.BadgeY + offsetY, dpi);

                int stroke = 2;
                int fh = (int)Math.Round(Math.Min(w, h) * 0.45);
                int fw = fh;

                sb.AppendLine($"^FO{x},{y}^GE{w},{h},{stroke}^FS");
                int fbWidth = Math.Max(w, h);
                sb.AppendLine($"^FO{x},{y}^FB{fbWidth},1,0,C^A0N,{fh},{fw}^FDPb^FS");
            }

            // ───────────────────── Data Matrix (좌상단 기준) ─────────────────────
            if (_style.ShowDMPrint)
            {
                // ① 모듈 1칸 mm → 도트 (UI에서 온 값 그대로)
                int moduleDots = Math.Max(1, MmToDots(Math.Max(0.1, _style.DMModuleMm), dpi));

                // ② 좌표 (offset 적용)
                int x = MmToDots(_style.DMx + offsetX, dpi);
                int y = MmToDots(_style.DMy + offsetY, dpi);

                // ③ DM 데이터 (ETCS)
                string dm = BuildEtcsDmPayloadFromUi();

                // ④ 자동 출력 쪽과 동일하게: cols/rows를 강제하지 않고 프린터에 맡긴다
                sb.AppendLine($"^FO{x},{y}^BXN,{moduleDots},200");
                sb.AppendLine("^FH\\^FD" + dm + "^FS");
            }

            // Rating
            if (_style.ShowRatingPrint)
            {
                int x = MmToDots(_style.RatingX + offsetX, dpi);
                int y = MmToDots(_style.RatingY + offsetY, dpi);
                int h = MmToDots(PositiveOr(_style.RatingFont, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(GetGridText(RowKey.Rating, _style.RatingText))}^FS");
            }

            // FCC ID
            if (_style.ShowFCCIDPrint)
            {
                int x = MmToDots(_style.FCCIDX + offsetX, dpi);
                int y = MmToDots(_style.FCCIDY + offsetY, dpi);
                int h = MmToDots(PositiveOr(_style.FCCIDFont, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(GetGridText(RowKey.FCCID, _style.FCCIDText))}^FS");
            }

            // IC ID
            if (_style.ShowICIDPrint)
            {
                int x = MmToDots(_style.ICIDX + offsetX, dpi);
                int y = MmToDots(_style.ICIDY + offsetY, dpi);
                int h = MmToDots(PositiveOr(_style.ICIDFont, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(GetGridText(RowKey.ICID, _style.ICIDText))}^FS");
            }

            // Item1
            if (_style.ShowItem1Print)
            {
                int x = MmToDots(_style.Item1X + offsetX, dpi);
                int y = MmToDots(_style.Item1Y + offsetY, dpi);
                int h = MmToDots(PositiveOr(_style.Item1Font, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(GetGridText(RowKey.Item1, _style.Item1Text))}^FS");
            }
            // Item2
            if (_style.ShowItem2Print)
            {
                int x = MmToDots(_style.Item2X + offsetX, dpi);
                int y = MmToDots(_style.Item2Y + offsetY, dpi);
                int h = MmToDots(PositiveOr(_style.Item2Font, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(GetGridText(RowKey.Item2, _style.Item2Text))}^FS");
            }
            // Item3
            if (_style.ShowItem3Print)
            {
                int x = MmToDots(_style.Item3X + offsetX, dpi);
                int y = MmToDots(_style.Item3Y + offsetY, dpi);
                int h = MmToDots(PositiveOr(_style.Item3Font, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(GetGridText(RowKey.Item3, _style.Item3Text))}^FS");
            }
            // Item4
            if (_style.ShowItem4Print)
            {
                int x = MmToDots(_style.Item4X + offsetX, dpi);
                int y = MmToDots(_style.Item4Y + offsetY, dpi);
                int h = MmToDots(PositiveOr(_style.Item4Font, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(GetGridText(RowKey.Item4, _style.Item4Text))}^FS");
            }
            // Item5
            if (_style.ShowItem5Print)
            {
                int x = MmToDots(_style.Item5X + offsetX, dpi);
                int y = MmToDots(_style.Item5Y + offsetY, dpi);
                int h = MmToDots(PositiveOr(_style.Item5Font, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(GetGridText(RowKey.Item5, _style.Item5Text))}^FS");
            }

            sb.AppendLine("^XZ");
            return sb.ToString();
        }

        // ───────────────────── H/KMC ETCS DM 페이로드 빌더 ─────────────────────
        private string BuildEtcsDmPayloadFromUi()
        {
            // UI 값(오른쪽 큰 칸: Value) 읽기
            string v = (txtEtcsVendorValue?.Text ?? "").Trim();          // V
            string p = (txtEtcsPartNoValue?.Text ?? "").Trim();          // P
            string s = (txtEtcsSequenceValue?.Text ?? "").Trim();          // S (옵션)
            string e = (txtEtcsEoValue?.Text ?? "").Trim();              // E (옵션)
            string t = (txtEtcsTraceValue?.Text ?? "").Trim();           // T = YYMMDD 
            string m = (txtEtcs4MValue?.Text ?? "").Trim();         // 부품4M
            string a = (txtEtcsAValue?.Text ?? "");
            if (string.IsNullOrEmpty(m))
                m = new string('0', 4);   // M 필드: 0000으로 채움
            string Serial = (txtEtcsSerialValue?.Text ?? "").Trim();     // C  (옵션 확장)

            // 제어코드(백슬래시-헥스 표기) - ^FH\ 가 해석함
            const string GS = @"\1D";
            const string RS = @"\1E";
            const string EOT = @"\04";

            var sb = new System.Text.StringBuilder(256);
            sb.Append("[)>");               // 심볼 식별자 (스펙/현행 문자열에 맞춰 "[)>" 사용)
            sb.Append(RS).Append("06");    // 버전

            sb.Append(GS).Append("V").Append(v);
            sb.Append(GS).Append("P").Append(p);

            // S/E: 비어도 GS 슬롯은 유지해 필드 밀림 방지
            sb.Append(GS);
            //if (!string.IsNullOrEmpty(s))
            sb.Append("S").Append(s);

            sb.Append(GS);
            //if (!string.IsNullOrEmpty(e))
            sb.Append("E").Append(e);

            // T(필수)
            //sb.Append(GS).Append("T").Append(T);

            sb.Append(GS);
            sb.Append("T").Append(t);

            sb.Append(m);

            sb.Append(a);

            sb.Append(Serial);

            // 트레일러
            sb.Append(GS).Append(RS).Append(EOT);
            return sb.ToString();
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
        private static readonly (int bytes, int modules)[] DM_CAP = new[]
        {
            (6,10),(10,12),(16,14),(24,16),(36,18),(44,20),
            (60,22),(72,24),(88,26),(120,32),(164,36),(224,40),(280,44)
            // 필요하면 48×48 … 144×144 추가
        };

        // ── 현재 UI 기준 DM 모듈 수(행/열) 결정: 지정값 우선, 없으면 용량표로 자동 추정
        private int GetCurrentDmModulesFromUiOrAuto()
        {
            var r = GetRow(RowKey.DM);
            int cols = (int)Math.Round(ReadScaleCell(r, COL_XSCALE, 0.0));
            int rows = (int)Math.Round(ReadScaleCell(r, COL_YSCALE, 0.0));
            if (cols >= 10 && cols <= 144 && rows >= 10 && rows <= 144)
                return Math.Max(cols, rows); // 정방형 기준

            int len = DecodeZplFh(BuildEtcsDmPayloadFromUi()).Length;
            foreach (var t in DM_CAP) if (len <= t.bytes) return t.modules;
            return 144; // 상한
        }

        // ───────────────────── DM(정방형) 후보 모듈 목록 ─────────────────────
        private static readonly int[] DM_SQUARE = { 10, 12, 14, 16, 18, 20, 22, 24, 26, 32, 36, 40, 44, 48, 52, 64, 72, 80, 88, 96, 104, 120, 132, 144 };

        // 현재 데이터(ETCS) 길이로 '필요 최소 모듈 수' 계산
        private int GetMinDmModulesNeeded()
        {
            int len = DecodeZplFh(BuildEtcsDmPayloadFromUi()).Length;
            foreach (var t in DM_CAP)
                if (len <= t.bytes) return t.modules;
            return 144;
        }

        // 목표 mm에 가장 근접한 (M, h, 실제 한 변 mm) 선택
        private (int M, int h, double sideMmActual) AutoPickDmByTarget(double targetMm, int dpi)
        {
            double mmPerDot = 25.4 / dpi;
            int dotsTarget = Math.Max(1, (int)Math.Round(targetMm / mmPerDot));

            int minM = GetMinDmModulesNeeded();
            int bestM = minM, bestH = 1;
            double bestSide = bestM * bestH * mmPerDot;
            double bestErr = Math.Abs(bestSide - targetMm);

            foreach (int M in DM_SQUARE)
            {
                if (M < minM) continue;
                int h = Math.Max(1, (int)Math.Round((double)dotsTarget / M));
                double side = M * h * mmPerDot;
                double err = Math.Abs(side - targetMm);
                if (err < bestErr)
                {
                    bestErr = err; bestM = M; bestH = h; bestSide = side;
                }
            }
            return (bestM, bestH, bestSide);
        }


        // Font 테스트용 헬퍼
        private string BuildZplTemplateWithAZ(int dpi = DEFAULT_DPI)
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
        private string BuildZplForFont(string ttfPath, int dpi = DEFAULT_DPI)
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
        private void PrintAllTtfSamples(string printerName, int dpi = DEFAULT_DPI)
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
                    var sb = new StringBuilder(rows * bytesPerRow * 2 + rows);

                    const int THRESH = 200; // 밝기 임계값

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
                            bool isBlack = lum < THRESH;

                            if (isBlack) packed |= (byte)(1 << bitPos);

                            bitPos--;
                            if (bitPos < 0)
                            {
                                sb.Append(packed.ToString("X2"));
                                packed = 0;
                                bitPos = 7;
                                writtenBytes++;
                            }
                        }

                        if (bitPos != 7)
                        {
                            sb.Append(packed.ToString("X2"));
                            writtenBytes++;
                        }
                        for (; writtenBytes < bytesPerRow; writtenBytes++)
                            sb.Append("00");

                        sb.Append('\n');
                    }

                    int totalBytes = bytesPerRow * rows;
                    return $"^GFA,{totalBytes},{totalBytes},{bytesPerRow},\n{sb}";
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
        private string BuildDmPayloadFromGrid()
        {   /*
            string brand = GetGridText(RowKey.Brand, _style.BrandText ?? "");
            string part = GetGridText(RowKey.Part, _style.PartText ?? "");
            string hw = GetGridText(RowKey.HW, _style.HWText ?? "");
            string sw = GetGridText(RowKey.SW, _style.SWText ?? "");
            string lot = GetGridText(RowKey.LOT, _style.LOTText ?? "");
            string sn = GetGridText(RowKey.SN, _style.SerialText ?? "");
            string logo = Path.GetFileName(_style.LogoImagePath ?? "");

            var sb = new StringBuilder(256);
            void Add(string k, string v)
            {
                v = AsciiSafeOneLine(v ?? "");
                if (sb.Length > 0) sb.Append('|');
                sb.Append(k).Append('=').Append(v);
            }

            Add("BRAND", brand);
            Add("PART", part);
            Add("HW", hw);
            Add("SW", sw);
            Add("LOT", lot);
            Add("SN", sn);
            if (!string.IsNullOrWhiteSpace(logo)) Add("LOGO", logo);
            */
            return BuildEtcsDmPayloadFromUi();
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
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (ch == '\r' || ch == '\n' || ch == '\t') continue;
                sb.Append((ch >= 32 && ch <= 126) ? ch : '_');
            }
            return sb.ToString();
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

            // 최소 필수: Vendor, PartNo, Trace(= 생산일자 YYMMDD 기준)
            if (string.IsNullOrWhiteSpace(d.Vendor) ||
                string.IsNullOrWhiteSpace(d.PartNo))
                return null;

            // 제어코드(\hh → ^FH\에서 해석)
            const string GS = @"\1D";
            const string RS = @"\1E";
            const string EOT = @"\04";

            var v = (d.Vendor ?? "").Trim();   // V
            var p = (d.PartNo ?? "").Trim();   // P
            var s = (d.Sequence ?? "").Trim();   // S (opt)
            var e = (d.Eo ?? "").Trim();       // E (opt)

            // ── T(추적영역) 구성 규칙 ─────────────────────────────────────────────
            // 1) 생산일자(YYMMDD): 당일 날짜만 사용 (뒤에 붙던 기존 후미는 모두 버림)
            // 2) 부품4M: 공란 강제
            // 3) A or @: 한 글자만 허용 (A 또는 @), 그 외 값은 공란
            // 4) 추적번호: 숫자만 추출해서 7자리면 7자리, 그 외는 4자리(우리 설비 기본)로 0패딩
            string todayYYMMDD = DateTime.Now.ToString("yyMMdd");

            // A or @ (1문자만 허용)
            string a = (d.A ?? "").Trim();
            a = (a == "A" || a == "@") ? a : "A";

            // 부품4M 0000 강제
            string fourM = new string('0', 4);  // 0 4개

            // 추적번호
            string cDigits = System.Text.RegularExpressions.Regex.Replace((d.Serial ?? ""), @"\D", "");
            if (string.IsNullOrEmpty(cDigits) && GSystem.ProductSettings != null)
                cDigits = GSystem.ProductSettings.GetCurrentSerialNumber().ToString("D4"); // 기본 4자리

            string Serial; // 최종 추적번호
            if (cDigits.Length >= 7)
                Serial = cDigits.Substring(cDigits.Length - 7).PadLeft(7, '0'); // 7자리 우선
            else
                Serial = cDigits.PadLeft(4, '0'); // 기본 4자리

            // 최종 T 페이로드: [YYMMDD] + [4M=0000] + [A/@] + [추적번호]
            // ── DM 문자열 조립 ────────────────────────────────────────────────────
            var sb = new StringBuilder(256);
            sb.Append("[)>");               // 심볼 식별자 (스펙/현행 문자열에 맞춰 "[)>" 사용)
            sb.Append(RS).Append("06");    // 버전

            sb.Append(GS).Append("V").Append(v);
            sb.Append(GS).Append("P").Append(p);

            // S/E: 비어도 GS 슬롯은 유지해 필드 밀림 방지
            sb.Append(GS);
            //if (!string.IsNullOrEmpty(s))
            sb.Append("S").Append(s);

            sb.Append(GS);
            //if (!string.IsNullOrEmpty(e))
            sb.Append("E").Append(e);

            // T(필수)
            //sb.Append(GS).Append("T").Append(T);

            sb.Append(GS);
            sb.Append("T").Append(todayYYMMDD);

            sb.Append(fourM);

            sb.Append(a);

            sb.Append(Serial);

            // 트레일러
            sb.Append(GS).Append(RS).Append(EOT);
            return sb.ToString();
        }


        public static string BuildZpl(
            LabelStyle labelStyle,
            LabelPayload labelPayload,
            EtcsSettings etcs,
            int dpi,
            int qty,
            int darkness,
            double speedIps,
            Func<Bitmap> getLogoBitmap // 없으면 null 리턴
        )
        {
            if (labelStyle == null) throw new ArgumentNullException(nameof(labelStyle));

            int PW = MmToDots(labelStyle.LabelWmm, dpi);
            int LL = MmToDots(labelStyle.LabelHmm, dpi);

            var sb = new StringBuilder(2048);
            sb.AppendLine("~SD" + Clamp(darkness, 0, 30));
            sb.AppendLine("^XA");
            sb.AppendLine("^PW" + PW);
            sb.AppendLine("^LL" + LL);
            sb.AppendLine("^LH0,0");
            const double NUDGE_Y_MM = 0.6;
            sb.AppendLine("^LT" + MmToDots(NUDGE_Y_MM, dpi));
            sb.AppendLine("^LS0");
            if (speedIps > 0) sb.AppendLine("^PR" + (int)Math.Round(speedIps));
            sb.AppendLine("^PQ" + Math.Max(1, qty));

            // ── 로고 (^GFA) : Style에 이미지 경로만 있고 스케일 속성 없으므로, 높이만 사용
            if (labelStyle.ShowLogoPrint && !string.IsNullOrWhiteSpace(labelStyle.LogoImagePath))
            {
                try
                {
                    using (var bmpSrc = getLogoBitmap?.Invoke())
                    {
                        if (bmpSrc != null)
                        {
                            double aspect = bmpSrc.Width / (double)bmpSrc.Height;
                            int logoH = Math.Max(1, MmToDots(labelStyle.LogoH, dpi));
                            int logoW = Math.Max(1, (int)Math.Round(logoH * aspect));

                            using (var canvas = new Bitmap(logoW, logoH))
                            using (var g = Graphics.FromImage(canvas))
                            {
                                g.Clear(Color.White);
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.DrawImage(bmpSrc, new Rectangle(0, 0, logoW, logoH));
                                int bpr, rows;
                                string gfa = ToZplGFA(canvas, out bpr, out rows);
                                int x = MmToDots(labelStyle.LogoX, dpi);
                                int y = MmToDots(labelStyle.LogoY, dpi);
                                sb.AppendLine($"^FO{x},{y}{gfa}^FS");
                            }
                        }
                    }
                }
                catch { /* 로고 실패는 무시 */ }
            }

            // ── Brand
            if (labelStyle.ShowBrandPrint && !string.IsNullOrEmpty(labelPayload?.Company))
            {
                int h = MmToDots(PositiveOr(labelStyle.BrandFont, 2.6), dpi);
                int w = h; // ScaleX 없음 → 폭=높이로 처리
                int x = MmToDots(labelStyle.BrandX, dpi);
                int y = MmToDots(labelStyle.BrandY, dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(labelPayload.Company)}^FS");
            }

            // ── Part (X<=0 → 중앙정렬 규칙은 유지)
            if (labelStyle.ShowPartPrint && !string.IsNullOrEmpty(labelPayload?.PartNo))
            {
                int h = MmToDots(PositiveOr(labelStyle.PartFont, 2.6), dpi);
                int w = h;
                if (labelStyle.PartX <= 0)
                {
                    int y = MmToDots(labelStyle.PartY, dpi);
                    sb.AppendLine($"^FO0,{y}^FB{PW},1,0,C^A0N,{h},{w}^FD{Escape(labelPayload.PartNo)}^FS");
                }
                else
                {
                    int x = MmToDots(labelStyle.PartX, dpi);
                    int y = MmToDots(labelStyle.PartY, dpi);
                    sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(labelPayload.PartNo)}^FS");
                }
            }

            // ── HW/SW/LOT/SN (Scale 없음 → 폰트 높이만 사용)
            PrintText(sb, labelStyle.ShowHWPrint, labelStyle.HWx, labelStyle.HWy, labelStyle.HWfont, labelPayload?.HW, dpi);
            PrintText(sb, labelStyle.ShowSWPrint, labelStyle.SWx, labelStyle.SWy, labelStyle.SWfont, labelPayload?.SW, dpi);
            PrintText(sb, labelStyle.ShowLOTPrint, labelStyle.LOTx, labelStyle.LOTy, labelStyle.LOTfont, labelPayload?.LOT, dpi);
            PrintText(sb, labelStyle.ShowSNPrint, labelStyle.SNx, labelStyle.SNy, labelStyle.SNfont, labelPayload?.SN, dpi);

            // ── Pb 배지 : 지름만 존재 → 타원 크기 w=h로
            if (labelStyle.ShowPbPrint)
            {
                int d = Math.Max(1, MmToDots(labelStyle.BadgeDiameter, dpi));
                int x = MmToDots(labelStyle.BadgeX, dpi);
                int y = MmToDots(labelStyle.BadgeY, dpi);
                int stroke = 2;
                int fh = (int)Math.Round(d * 0.45);
                sb.AppendLine($"^FO{x},{y}^GE{d},{d},{stroke}^FS");
                sb.AppendLine($"^FO{x},{y}^FB{d},1,0,C^A0N,{fh},{fh}^FDPb^FS");
            }

            // ── Data Matrix (^BX) : Cols/Rows 속성 없음 → 기본값으로 단순 출력
            string dm = string.IsNullOrWhiteSpace(labelPayload?.DataMatrix)
                            ? BuildEtcsDm(etcs) // 2) 없으면 ETCS로 자동 생성
                            : labelPayload.DataMatrix;

            if (labelStyle.ShowDMPrint && !string.IsNullOrWhiteSpace(dm))
            {
                int moduleDots = Math.Max(1, MmToDots(Math.Max(0.1, labelStyle.DMModuleMm), dpi));
                int x = MmToDots(labelStyle.DMx, dpi);
                int y = MmToDots(labelStyle.DMy, dpi);
                sb.AppendLine($"^FO{x},{y}^BXN,{moduleDots},200");
                sb.AppendLine("^FH\\^FD" + dm + "^FS"); // ^FH\가 \1D 등 제어코드 해석
            }
            // ── Rating / FCC / IC
            if (labelStyle.ShowRatingPrint && !string.IsNullOrEmpty(labelStyle.RatingText))
            {
                int x = MmToDots(labelStyle.RatingX, dpi);
                int y = MmToDots(labelStyle.RatingY, dpi);
                int h = MmToDots(PositiveOr(labelStyle.RatingFont, 2.6), dpi);
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(labelStyle.RatingText)}^FS");
            }
            if (labelStyle.ShowFCCIDPrint && !string.IsNullOrEmpty(labelPayload?.FCCID ?? labelStyle.FCCIDText))
            {
                int x = MmToDots(labelStyle.FCCIDX, dpi);
                int y = MmToDots(labelStyle.FCCIDY, dpi);
                int h = MmToDots(PositiveOr(labelStyle.FCCIDFont, 2.6), dpi);
                string txt = string.IsNullOrEmpty(labelPayload?.FCCID) ? labelStyle.FCCIDText : labelPayload.FCCID;
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(txt)}^FS");
            }
            if (labelStyle.ShowICIDPrint && !string.IsNullOrEmpty(labelPayload?.ICID ?? labelStyle.ICIDText))
            {
                int x = MmToDots(labelStyle.ICIDX, dpi);
                int y = MmToDots(labelStyle.ICIDY, dpi);
                int h = MmToDots(PositiveOr(labelStyle.ICIDFont, 2.6), dpi);
                string txt = string.IsNullOrEmpty(labelPayload?.ICID) ? labelStyle.ICIDText : labelPayload.ICID;
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(txt)}^FS");
            }
            // Item1
            if (labelStyle.ShowItem1Print)
            {
                int x = MmToDots(labelStyle.Item1X, dpi);
                int y = MmToDots(labelStyle.Item1Y, dpi);
                int h = MmToDots(PositiveOr(labelStyle.Item1Font, 2.6), dpi);
                string txt = string.IsNullOrEmpty(labelPayload?.Item1) ? labelStyle.Item1Text : labelPayload.Item1;
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(txt)}^FS");
            }
            // Item2
            if (labelStyle.ShowItem2Print)
            {
                int x = MmToDots(labelStyle.Item2X, dpi);
                int y = MmToDots(labelStyle.Item2Y, dpi);
                int h = MmToDots(PositiveOr(labelStyle.Item2Font, 2.6), dpi);
                string txt = string.IsNullOrEmpty(labelPayload?.Item2) ? labelStyle.Item2Text : labelPayload.Item2;
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(txt)}^FS");
            }
            // Item3
            if (labelStyle.ShowItem3Print)
            {
                int x = MmToDots(labelStyle.Item3X, dpi);
                int y = MmToDots(labelStyle.Item3Y, dpi);
                int h = MmToDots(PositiveOr(labelStyle.Item3Font, 2.6), dpi);
                string txt = string.IsNullOrEmpty(labelPayload?.Item3) ? labelStyle.Item3Text : labelPayload.Item3;
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(txt)}^FS");
            }
            // Item4
            if (labelStyle.ShowItem4Print)
            {
                int x = MmToDots(labelStyle.Item4X, dpi);
                int y = MmToDots(labelStyle.Item4Y, dpi);
                int h = MmToDots(PositiveOr(labelStyle.Item4Font, 2.6), dpi);
                string txt = string.IsNullOrEmpty(labelPayload?.Item4) ? labelStyle.Item4Text : labelPayload.Item4;
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(txt)}^FS");
            }
            // Item5
            if (labelStyle.ShowItem5Print)
            {
                int x = MmToDots(labelStyle.Item5X, dpi);
                int y = MmToDots(labelStyle.Item5Y, dpi);
                int h = MmToDots(PositiveOr(labelStyle.Item5Font, 2.6), dpi);
                string txt = string.IsNullOrEmpty(labelPayload?.Item5) ? labelStyle.Item5Text : labelPayload.Item5;
                sb.AppendLine($"^FO{x},{y}^A0N,{h},{h}^FD{Escape(txt)}^FS");
            }


            sb.AppendLine("^XZ");
            return sb.ToString();
        }

        private static void PrintText(StringBuilder sb, bool show,
                                      double xMm, double yMm, double fontMm,
                                      string text, int dpi)
        {
            if (!show || string.IsNullOrWhiteSpace(text)) return;
            int h = MmToDots(PositiveOr(fontMm, 2.6), dpi);
            int w = h;
            int x = MmToDots(xMm, dpi);
            int y = MmToDots(yMm, dpi);
            sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(text)}^FS");
        }

        private static double PositiveOr(double v, double fallback) => (v > 0) ? v : fallback;
        private static int Clamp(int v, int min, int max) => (v < min) ? min : (v > max) ? max : v;
    }
}

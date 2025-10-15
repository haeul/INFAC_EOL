using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace DHSTesterXL
{
    public partial class FormProduct
    {
        // 프린터와 동일한 정수 도트 양자화
        static int MmToDotsInt(double mm, int dpi) => (int)Math.Round(mm * dpi / 25.4);
        static double DotsToMm(int dots, int dpi) => dots * 25.4 / (double)dpi;

        // ───────────────────── 프리뷰 ─────────────────────
        private void Preview_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.White);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle previewBounds = Preview.ClientRectangle;
            if (previewBounds.Width <= 0 || previewBounds.Height <= 0) return;

            // mm → px 스케일
            float previewScaleX = (float)(previewBounds.Width / _style.LabelWidthMm);
            float previewScaleY = (float)(previewBounds.Height / _style.LabelHeightMm);
            float millimeterToPixelScale = Math.Min(previewScaleX, previewScaleY);

            float labelWidthPixels = (float)(_style.LabelWidthMm * millimeterToPixelScale);
            float labelHeightPixels = (float)(_style.LabelHeightMm * millimeterToPixelScale);

            float labelOriginX = previewBounds.Left + (previewBounds.Width - labelWidthPixels) / 2f;
            float labelOriginY = previewBounds.Top + (previewBounds.Height - labelHeightPixels) / 2f;

            var labelOutlineBounds = new RectangleF(labelOriginX, labelOriginY, labelWidthPixels, labelHeightPixels);
            using (var backgroundBrush = new SolidBrush(Color.White))
            using (var borderPen = new Pen(Color.Silver, 1f))
            using (var roundedLabelPath = CreateRoundedRectPath(labelOutlineBounds, _style.CornerRadiusPx))
            {
                graphics.FillPath(backgroundBrush, roundedLabelPath);
                graphics.DrawPath(borderPen, roundedLabelPath);
            }

            // 좌표 변환기(mm→px)
            //float ConvertMmToPreviewX(double mm) => originX + (float)(mm * millimeterToPixelScale);
            //float ConvertMmToPreviewY(double mm) => originY + (float)(mm * millimeterToPixelScale);
            const double PrintHeadYOffsetMm = 0.6; // 출력과 동일
            float ConvertMmToPreviewX(double millimeters)
            {
                int quantizedXDots = MmToDotsInt(millimeters, DefaultDpi);
                double quantizedMillimeters = DotsToMm(quantizedXDots, DefaultDpi);
                return labelOriginX + (float)(quantizedMillimeters * millimeterToPixelScale);
            }
            float ConvertMmToPreviewY(double millimeters)
            {
                // ^LT 보정 추가
                double millimetersWithLt = millimeters + PrintHeadYOffsetMm;
                int quantizedYDots = MmToDotsInt(millimetersWithLt, DefaultDpi);
                double quantizedMillimeters = DotsToMm(quantizedYDots, DefaultDpi);
                return labelOriginY + (float)(quantizedMillimeters * millimeterToPixelScale);
            }


            // 고정 요소(로고/브랜드/품번)
            DrawLogoBrandPart(graphics, ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale);

            // HW
            if (_style.IsHardwarePreviewEnabled)
            {
                var hardwareRow = GetRow(RowKey.HW);
                double scaleX = ReadScaleCell(hardwareRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(hardwareRow, ColumnNameScaleY, 1.0);

                DrawTextTopLeft(graphics, GetGridText(RowKey.HW, _style.HardwareText),
                                _style.HardwareXMm, _style.HardwareYMm, PositiveOr(_style.HardwareFontMm, 2.6),
                                ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }

            // SW
            if (_style.IsSoftwarePreviewEnabled)
            {
                var softwareRow = GetRow(RowKey.SW);
                double scaleX = ReadScaleCell(softwareRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(softwareRow, ColumnNameScaleY, 1.0);

                DrawTextTopLeft(graphics, GetGridText(RowKey.SW, _style.SoftwareText),
                                _style.SoftwareXMm, _style.SoftwareYMm, PositiveOr(_style.SoftwareFontMm, 2.6),
                                ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }
            // LOT
            if (_style.IsLotPreviewEnabled)
            {
                var lotRow = GetRow(RowKey.LOT);
                double scaleX = ReadScaleCell(lotRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(lotRow, ColumnNameScaleY, 1.0);

                DrawTextTopLeft(graphics, GetGridText(RowKey.LOT, _style.LotText),
                                _style.LotXMm, _style.LotYMm, PositiveOr(_style.LotFontMm, 2.6),
                                ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }
            // SN
            if (_style.IsSerialPreviewEnabled)
            {
                var serialNumberRow = GetRow(RowKey.SN);
                double scaleX = ReadScaleCell(serialNumberRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(serialNumberRow, ColumnNameScaleY, 1.0);

                DrawTextTopLeft(graphics, GetGridText(RowKey.SN, _style.SerialText),
                                _style.SerialXMm, _style.SerialYMm, PositiveOr(_style.SerialFontMm, 2.6),
                                ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }

            // Pb 배지
            if (_style.IsPbPreviewEnabled)
            {
                var leadFreeRow = GetRow(RowKey.Pb);
                double scaleX = ReadScaleCell(leadFreeRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(leadFreeRow, ColumnNameScaleY, 1.0);

                DrawPbBadge(graphics, ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale,
                            _style.BadgeDiameterMm, _style.BadgeXMm, _style.BadgeYMm,
                            scaleX, scaleY);
            }

            // DM 프리뷰
            if (_style.IsDataMatrixPreviewEnabled)
            {
                var dataMatrixRow = GetRow(RowKey.DM);

                // 1) DM은 출력 시 모듈 크기가 "정수 도트"로 양자화됨 → 프리뷰도 동일 규칙 적용
                int dataMatrixModuleDots = Math.Max(1, MmToDots(Math.Max(0.1, _style.DataMatrixModuleSizeMm), DefaultDpi));
                double moduleSizeForPreviewMm = dataMatrixModuleDots * (25.4 / (double)DefaultDpi);

                // 2) 그리드 X/Y를 DM 열/행(정수)로 활용(0 또는 범위 밖이면 자동)
                int columnCount = (int)Math.Round(ReadScaleCell(dataMatrixRow, ColumnNameScaleX, 0.0));
                int rowCount = (int)Math.Round(ReadScaleCell(dataMatrixRow, ColumnNameScaleY, 0.0));
                if (columnCount < 10 || columnCount > 144) columnCount = 0;
                if (rowCount < 10 || rowCount > 144) rowCount = 0;

                // 3) 프리뷰 그리기 (모듈수 × 모듈mm)
                DrawDataMatrixPreview(
                    graphics, ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale,
                    moduleSizeForPreviewMm,   // 양자화된 모듈 mm 사용(출력과 동일)
                    _style.DataMatrixXMm, _style.DataMatrixYMm,
                    columnCount, rowCount            // DM 열/행(0=자동)
                );
            }

            // Rating
            if (_style.IsRatingPreviewEnabled)
            {
                var ratingRow = GetRow(RowKey.Rating);
                double scaleX = ReadScaleCell(ratingRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(ratingRow, ColumnNameScaleY, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Rating, _style.RatingText),
                    _style.RatingXMm, _style.RatingYMm, PositiveOr(_style.RatingFontMm, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }

            // FCC ID
            if (_style.IsFccIdPreviewEnabled)
            {
                var fccIdRow = GetRow(RowKey.FCCID);
                double scaleX = ReadScaleCell(fccIdRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(fccIdRow, ColumnNameScaleY, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.FCCID, _style.FccIdText),
                    _style.FccIdXMm, _style.FccIdYMm, PositiveOr(_style.FccIdFontMm, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }

            // IC ID
            if (_style.IsIcIdPreviewEnabled)
            {
                var icIdRow = GetRow(RowKey.ICID);
                double scaleX = ReadScaleCell(icIdRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(icIdRow, ColumnNameScaleY, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.ICID, _style.IcIdText),
                    _style.IcIdXMm, _style.IcIdYMm, PositiveOr(_style.IcIdFontMm, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }

            // Item1
            if (_style.IsItem1PreviewEnabled)
            {
                var item1Row = GetRow(RowKey.Item1);
                double scaleX = ReadScaleCell(item1Row, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(item1Row, ColumnNameScaleY, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item1, _style.Item1Text),
                    _style.Item1XMm, _style.Item1YMm, PositiveOr(_style.Item1FontMm, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }
            // Item2
            if (_style.IsItem2PreviewEnabled)
            {
                var item2Row = GetRow(RowKey.Item2);
                double scaleX = ReadScaleCell(item2Row, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(item2Row, ColumnNameScaleY, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item2, _style.Item2Text),
                    _style.Item2XMm, _style.Item2YMm, PositiveOr(_style.Item2FontMm, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }
            // Item3
            if (_style.IsItem3PreviewEnabled)
            {
                var item3Row = GetRow(RowKey.Item3);
                double scaleX = ReadScaleCell(item3Row, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(item3Row, ColumnNameScaleY, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item3, _style.Item3Text),
                    _style.Item3XMm, _style.Item3YMm, PositiveOr(_style.Item3FontMm, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }
            // Item4
            if (_style.IsItem4PreviewEnabled)
            {
                var item4Row = GetRow(RowKey.Item4);
                double scaleX = ReadScaleCell(item4Row, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(item4Row, ColumnNameScaleY, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item4, _style.Item4Text),
                    _style.Item4XMm, _style.Item4YMm, PositiveOr(_style.Item4FontMm, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }
            // Item5
            if (_style.IsItem5PreviewEnabled)
            {
                var item5Row = GetRow(RowKey.Item5);
                double scaleX = ReadScaleCell(item5Row, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(item5Row, ColumnNameScaleY, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item5, _style.Item5Text),
                    _style.Item5XMm, _style.Item5YMm, PositiveOr(_style.Item5FontMm, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }

        }

        /// <summary>로고/회사명/품번(프리뷰, 모두 좌상단 기준)</summary>
        private void DrawLogoBrandPart(Graphics graphics,
                                       Func<double, float> convertMmToPreviewX,
                                       Func<double, float> convertMmToPreviewY,
                                       float millimeterToPixelScale)
        {
            // 로고
            if (_style.IsLogoPreviewEnabled)
            {
                if (_logoBitmap == null && !string.IsNullOrWhiteSpace(_style.LogoImagePath))
                    LoadLogoBitmap();

                if (_logoBitmap != null)
                {
                    var logoRow = GetRow(RowKey.Logo);
                    double scaleX = ReadScaleCell(logoRow, ColumnNameScaleX, 1.0);
                    double scaleY = ReadScaleCell(logoRow, ColumnNameScaleY, 1.0);

                    float leftPx = convertMmToPreviewX(_style.LogoXMm);
                    float topPx = convertMmToPreviewY(_style.LogoYMm);

                    double aspect = _logoBitmap.Width / (double)_logoBitmap.Height;
                    float heightPixels = (float)(_style.LogoHeightMm * scaleY * millimeterToPixelScale);
                    float widthPixels = (float)(_style.LogoHeightMm * aspect * scaleX * millimeterToPixelScale);

                    var originalSmoothing = graphics.SmoothingMode;
                    var originalInterpolation = graphics.InterpolationMode;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    graphics.DrawImage(_logoBitmap,
                        new RectangleF(leftPx, topPx,
                                       Math.Max(1, widthPixels), Math.Max(1, heightPixels)));

                    graphics.SmoothingMode = originalSmoothing;
                    graphics.InterpolationMode = originalInterpolation;
                }
            }

            // 회사명
            if (_style.IsBrandPreviewEnabled)
            {
                var brandRow = GetRow(RowKey.Brand);
                double scaleX = ReadScaleCell(brandRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(brandRow, ColumnNameScaleY, 1.0);

                DrawTextTopLeft(graphics, _style.BrandText ?? "",
                    _style.BrandXMm, _style.BrandYMm, _style.BrandFontMm,
                    convertMmToPreviewX, convertMmToPreviewY, millimeterToPixelScale, false, scaleX, scaleY);
            }

            // 품번
            if (_style.IsPartPreviewEnabled)
            {
                var partRow = GetRow(RowKey.Part);
                double scaleX = ReadScaleCell(partRow, ColumnNameScaleX, 1.0);
                double scaleY = ReadScaleCell(partRow, ColumnNameScaleY, 1.0);
                string text = _style.PartText ?? "";

                if (_style.PartXMm > 0)
                {
                    DrawTextTopLeft(graphics, text,
                        _style.PartXMm, _style.PartYMm, _style.PartFontMm,
                        convertMmToPreviewX, convertMmToPreviewY, millimeterToPixelScale, true, scaleX, scaleY);
                }
                else
                {
                    // 중앙정렬(프리뷰) — 측정 기반
                    float fontPx = (float)(_style.PartFontMm * millimeterToPixelScale);
                    float fontPt = Math.Max(1f, fontPx * 72f / graphics.DpiY);
                    using (var font = new Font("Arial", fontPt, FontStyle.Bold))
                    {
                        var textSize = graphics.MeasureString(text, font);
                        float centerPx = convertMmToPreviewX(_style.LabelWidthMm / 2.0);
                        float leftPx = centerPx - (float)(textSize.Width * scaleX / 2.0);
                        double leftMm = (leftPx - convertMmToPreviewX(0)) / millimeterToPixelScale;

                        DrawTextTopLeft(graphics, text,
                            leftMm, _style.PartYMm, _style.PartFontMm,
                            convertMmToPreviewX, convertMmToPreviewY, millimeterToPixelScale, true, scaleX, scaleY);
                    }
                }
            }
        }

        /// <summary> Pb 배지(프리뷰, 좌상단 기준) </summary>
        private void DrawPbBadge(Graphics graphics,
                                 Func<double, float> convertMmToPreviewX,
                                 Func<double, float> convertMmToPreviewY,
                                 float millimeterToPixelScale,
                                 double diameterMm, double xMm, double yMm,
                                 double scaleX, double scaleY)
        {
            float baseDiameterPixels = (float)(diameterMm * millimeterToPixelScale);
            float leftPixels = convertMmToPreviewX(xMm);
            float topPixels = convertMmToPreviewY(yMm);
            float widthPixels = baseDiameterPixels * (float)scaleX;
            float heightPixels = baseDiameterPixels * (float)scaleY;

            var badgeBounds = new RectangleF(leftPixels, topPixels, widthPixels, heightPixels);

            using (var fill = new SolidBrush(Color.Black))
                graphics.FillEllipse(fill, badgeBounds);
            using (var pen = new Pen(Color.Black, Math.Max(1f, 1.2f * millimeterToPixelScale)))
                graphics.DrawEllipse(pen, badgeBounds);

            float fontPx = Math.Min(widthPixels, heightPixels) * 0.58f;
            float fontPointSize = fontPx * 72f / graphics.DpiY;
            using (var font = new Font("Arial", Math.Max(1f, fontPointSize), FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            using (var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.DrawString("Pb", font, textBrush, badgeBounds, textFormat);
            }
        }

        // Data Matrix 프리뷰: sidePx = 모듈수 × 모듈픽셀
        private void DrawDataMatrixPreview(
            Graphics graphics,
            Func<double, float> convertMmToPreviewX,
            Func<double, float> convertMmToPreviewY,
            float millimeterToPixelScale,
            double moduleSizeMm, double originXMillimeters, double originYMillimeters,
            double columnCountInput, double rowCountInput)
        {
            // 모듈수(행/열) 결정: 지정값(10~144)이면 사용, 아니면 데이터 길이로 대략 추정
            int columnCount = (int)Math.Round(columnCountInput);
            int rowCount = (int)Math.Round(rowCountInput);
            if (columnCount < 10 || columnCount > 144 || rowCount < 10 || rowCount > 144)
            {
                int estimatedModuleCount = EstimateDmModulesFromData(BuildEtcsQrPayloadFromUi());
                columnCount = rowCount = estimatedModuleCount;
            }

            float moduleSizePixels = (float)(moduleSizeMm * millimeterToPixelScale);
            float squareSideLengthPixels = moduleSizePixels * Math.Max(columnCount, rowCount);

            var dataMatrixBounds = new RectangleF(convertMmToPreviewX(originXMillimeters), convertMmToPreviewY(originYMillimeters), squareSideLengthPixels, squareSideLengthPixels);
            using (var pen = new Pen(Color.Black, 1f)) graphics.DrawRectangle(pen, dataMatrixBounds.X, dataMatrixBounds.Y, dataMatrixBounds.Width, dataMatrixBounds.Height);

            // 라벨
            float labelTextSizePixels = Math.Max(6f, Math.Min(squareSideLengthPixels * 0.28f, 28f));
            float labelTextPointSize = labelTextSizePixels * 72f / graphics.DpiY;

            using (var font = new Font("Arial", labelTextPointSize, FontStyle.Bold, GraphicsUnit.Point))
            using (var brush = new SolidBrush(Color.Black))
            using (var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.DrawString("DM", font, brush, dataMatrixBounds, textFormat);
            }
        }

        // 데이터 길이 기반 간이 추정 (자동 모드일 때 프리뷰만 대략 맞추기 용)
        private static int EstimateDmModulesFromData(string data)
        {
            int characterCount = string.IsNullOrEmpty(data) ? 0 : data.Length;
            if (characterCount <= 6) return 10;
            if (characterCount <= 14) return 12;
            if (characterCount <= 24) return 14;
            if (characterCount <= 36) return 16;
            if (characterCount <= 48) return 18;
            if (characterCount <= 60) return 20;
            if (characterCount <= 72) return 22;
            if (characterCount <= 88) return 24;
            return Math.Min(144, 26 + (int)Math.Ceiling((characterCount - 88) / 12.0));
        }

        private static GraphicsPath CreateRoundedRectPath(RectangleF bounds, float radius)
        {
            var graphicsPath = new GraphicsPath();
            float diameter = radius * 2f;

            graphicsPath.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            graphicsPath.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            graphicsPath.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            graphicsPath.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);

            graphicsPath.CloseFigure();
            return graphicsPath;
        }

        /// <summary>
        /// 텍스트 그리기(프리뷰, 좌상단 기준)
        /// - mmFont → px/pt 변환
        /// - 좌상단(X,Y)로 이동 후 Scale(X,Y) 적용
        /// </summary>
        private static void DrawTextTopLeft(Graphics graphics, string text,
            double mmX, double mmY, double mmFont,
            Func<double, float> convertMmToPreviewX, Func<double, float> convertMmToPreviewY, float millimeterToPixelScale,
            bool bold, double scaleXRatio, double scaleYRatio)
        {
            //float fontPx = (float)(mmFont * millimeterToPixelScale);
            //float fontPt = Math.Max(1f, fontPx * 72f / g.DpiY);

            int heightDots = Math.Max(1, MmToDotsInt(mmFont * scaleYRatio, DefaultDpi));
            int widthDots = Math.Max(1, (int)Math.Round(heightDots * scaleXRatio));
            float heightPixels = (float)(DotsToMm(heightDots, DefaultDpi) * millimeterToPixelScale);
            // GDI 폰트 포인트로 변환
            float fontPt = Math.Max(1f, heightPixels * 72f / graphics.DpiY);

            using (var font = new Font("Arial", fontPt, bold ? FontStyle.Bold : FontStyle.Regular))
            using (var brush = new SolidBrush(Color.Black))
            {
                var graphicsState = graphics.Save();
                try
                {
                    float topLeftX = convertMmToPreviewX(mmX);
                    float topLeftY = convertMmToPreviewY(mmY);

                    graphics.TranslateTransform(topLeftX, topLeftY);
                    if (Math.Abs(scaleXRatio - 1.0) > 1e-6 || Math.Abs(scaleYRatio - 1.0) > 1e-6)
                        graphics.ScaleTransform((float)Math.Max(0.01, scaleXRatio),
                                         (float)Math.Max(0.01, scaleYRatio));

                    graphics.DrawString(text, font, brush, 0f, 0f);
                }
                finally { graphics.Restore(graphicsState); }
            }
        }


        // 로고 비트맵 로드(잠금 없음)
        private void LoadLogoBitmap()
        {
            try
            {
                _logoBitmap?.Dispose();
                _logoBitmap = null;

                var logoPath = ResolveLogoPath(_style.LogoImagePath);
                if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
                {
                    using (var fileStream = new FileStream(logoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var temporaryBitmap = new Bitmap(fileStream))
                    {
                        _logoBitmap = new Bitmap(temporaryBitmap);
                    }
                }
            }
            catch
            {
                _logoBitmap?.Dispose();
                _logoBitmap = null;
            }
        }

        // 기본 폴더 기준 경로 해석
        private string ResolveLogoPath(string logoPath)
        {
            if (string.IsNullOrWhiteSpace(logoPath)) return null;
            if (Path.IsPathRooted(logoPath)) return File.Exists(logoPath) ? logoPath : null;

            var defaultDirectoryPath = Path.Combine(DefaultLogoDirectory, logoPath);
            if (File.Exists(defaultDirectoryPath)) return defaultDirectoryPath;

            var applicationImagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", logoPath);
            if (File.Exists(applicationImagesPath)) return applicationImagesPath;

            return null;
        }
    }
}

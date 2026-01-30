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
            float scaleX = (float)(previewBounds.Width / _style.LabelWmm);
            float scaleY = (float)(previewBounds.Height / _style.LabelHmm);
            float mm2px = Math.Min(scaleX, scaleY);

            float labelWpx = (float)(_style.LabelWmm * mm2px);
            float labelHpx = (float)(_style.LabelHmm * mm2px);

            float labelOriginX = previewBounds.Left + (previewBounds.Width - labelWpx) / 2f;
            float labelOriginY = previewBounds.Top + (previewBounds.Height - labelHpx) / 2f;

            // Offset
            double offsetX = 0.0;
            double offsetY = 0.0;
            try { offsetX = Convert.ToDouble(nudOffsetX?.Value ?? 0m); } catch { }
            try { offsetY = Convert.ToDouble(nudOffsetY?.Value ?? 0m); } catch { }

            var labelRect = new RectangleF(labelOriginX, labelOriginY, labelWpx, labelHpx);
            using (var bg = new SolidBrush(Color.White))
            using (var pen = new Pen(Color.Silver, 1f))
            using (var rounded = CreateRoundedRectPath(labelRect, _style.CornerRadiusPx))
            {
                graphics.FillPath(bg, rounded);
                graphics.DrawPath(pen, rounded);
            }

            // 좌표 변환기(mm→px)
            //float ConvertMmToPreviewX(double mm) => labelOriginX + (float)(mm * mm2px);
            //float ConvertMmToPreviewY(double mm) => labelOriginY + (float)(mm * mm2px);
            const double NUDGE_Y_MM = 0.6; // 출력과 동일

            float ConvertMmToPreviewX(double mm)
            {
                // offsetX(mm) 적용 후 도트 양자화
                double mmWithOffset = mm + offsetX;
                int quantizedXDots = MmToDotsInt(mmWithOffset, DEFAULT_DPI);
                double quantizedMillimeters = DotsToMm(quantizedXDots, DEFAULT_DPI);
                return labelOriginX + (float)(quantizedMillimeters * mm2px);
            }

            float ConvertMmToPreviewY(double mm)
            {
                // offsetY + ^LT 보정 모두 적용
                double mmWithOffsetAndLt = mm + offsetY + NUDGE_Y_MM;
                int quantizedYDots = MmToDotsInt(mmWithOffsetAndLt, DEFAULT_DPI);
                double quantizedMillimeters = DotsToMm(quantizedYDots, DEFAULT_DPI);
                return labelOriginY + (float)(quantizedMillimeters * mm2px);
            }

            // 고정 요소(로고/브랜드/품번)
            DrawLogoBrandPart(graphics, ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px);

            // HW
            if (_style.ShowHWPreview)
            {
                var r = GetRow(RowKey.HW);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(graphics, GetGridText(RowKey.HW, _style.HWText),
                                _style.HWx, _style.HWy, PositiveOr(_style.HWfont, 2.6),
                                ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }

            // SW
            if (_style.ShowSWPreview)
            {
                var r = GetRow(RowKey.SW);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(graphics, GetGridText(RowKey.SW, _style.SWText),
                                _style.SWx, _style.SWy, PositiveOr(_style.SWfont, 2.6),
                                ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }
            // LOT
            if (_style.ShowLOTPreview)
            {
                var r = GetRow(RowKey.LOT);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(graphics, GetGridText(RowKey.LOT, _style.LOTText),
                                _style.LOTx, _style.LOTy, PositiveOr(_style.LOTfont, 2.6),
                                ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }
            // SN
            if (_style.ShowSNPreview)
            {
                var r = GetRow(RowKey.SN);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(graphics, GetGridText(RowKey.SN, _style.SerialText),
                                _style.SNx, _style.SNy, PositiveOr(_style.SNfont, 2.6),
                                ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }

            // Pb 배지
            if (_style.ShowPbPreview)
            {
                var rPb = GetRow(RowKey.Pb);
                double sx = ReadScaleCell(rPb, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(rPb, COL_YSCALE, 1.0);

                DrawPbBadge(graphics, ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px,
                            _style.BadgeDiameter, _style.BadgeX, _style.BadgeY,
                            sx, sy);
            }

            // DM 프리뷰
            if (_style.ShowDMPreview)
            {
                var rDM = GetRow(RowKey.DM);

                // 1) DM은 출력 시 모듈 크기가 "정수 도트"로 양자화됨 → 프리뷰도 동일 규칙 적용
                int moduleDots = Math.Max(1, MmToDots(Math.Max(0.1, _style.DMModuleMm), DEFAULT_DPI));
                double moduleMmForPreview = moduleDots * (25.4 / (double)DEFAULT_DPI);

                // 2) 그리드 X/Y를 DM 열/행(정수)로 활용(0 또는 범위 밖이면 자동)
                int cols = (int)Math.Round(ReadScaleCell(rDM, COL_XSCALE, 0.0));
                int rows = (int)Math.Round(ReadScaleCell(rDM, COL_YSCALE, 0.0));
                if (cols < 10 || cols > 144) cols = 0;
                if (rows < 10 || rows > 144) rows = 0;

                // 3) 프리뷰 그리기 (모듈수 × 모듈mm)
                DrawDataMatrixPreview(
                    graphics, ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px,
                    moduleMmForPreview,   // 양자화된 모듈 mm 사용(출력과 동일)
                    _style.DMx, _style.DMy,
                    cols, rows            // DM 열/행(0=자동)
                );
            }       

            // Rating
            if (_style.ShowRatingPreview)
            {
                var r = GetRow(RowKey.Rating);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Rating, _style.RatingText),
                    _style.RatingX, _style.RatingY, PositiveOr(_style.RatingFont, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }

            // FCC ID
            if (_style.ShowFCCIDPreview)
            {
                var r = GetRow(RowKey.FCCID);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.FCCID, _style.FCCIDText),
                    _style.FCCIDX, _style.FCCIDY, PositiveOr(_style.FCCIDFont, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }

            // IC ID
            if (_style.ShowICIDPreview)
            {
                var r = GetRow(RowKey.ICID);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.ICID, _style.ICIDText),
                    _style.ICIDX, _style.ICIDY, PositiveOr(_style.ICIDFont, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }

            // Item1
            if (_style.ShowItem1Preview)
            {
                var r = GetRow(RowKey.Item1);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item1, _style.Item1Text),
                    _style.Item1X, _style.Item1Y, PositiveOr(_style.Item1Font, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }
            // Item2
            if (_style.ShowItem2Preview)
            {
                var r = GetRow(RowKey.Item2);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item2, _style.Item2Text),
                    _style.Item2X, _style.Item2Y, PositiveOr(_style.Item2Font, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }
            // Item3
            if (_style.ShowItem3Preview)
            {
                var r = GetRow(RowKey.Item3);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item3, _style.Item3Text),
                    _style.Item3X, _style.Item3Y, PositiveOr(_style.Item3Font, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }
            // Item4
            if (_style.ShowItem4Preview)
            {
                var r = GetRow(RowKey.Item4);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item4, _style.Item4Text),
                    _style.Item4X, _style.Item4Y, PositiveOr(_style.Item4Font, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }
            // Item5
            if (_style.ShowItem5Preview)
            {
                var r = GetRow(RowKey.Item5);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(graphics, GetGridText(RowKey.Item5, _style.Item5Text),
                    _style.Item5X, _style.Item5Y, PositiveOr(_style.Item5Font, 2.6),
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }

        }

        /// <summary>로고/회사명/품번(프리뷰, 모두 좌상단 기준)</summary>
        private void DrawLogoBrandPart(Graphics graphics,
                                       Func<double, float> ConvertMmToPreviewX,
                                       Func<double, float> ConvertMmToPreviewY,
                                       float mm2px)
        {
            // 로고
            if (_style.ShowLogoPreview)
            {
                if (_logoBitmap == null && !string.IsNullOrWhiteSpace(_style.LogoImagePath))
                    LoadLogoBitmap();

                if (_logoBitmap != null)
                {
                    var rLogo = GetRow(RowKey.Logo);
                    double sx = ReadScaleCell(rLogo, COL_XSCALE, 1.0);
                    double sy = ReadScaleCell(rLogo, COL_YSCALE, 1.0);

                    float leftPx = ConvertMmToPreviewX(_style.LogoX);
                    float topPx = ConvertMmToPreviewY(_style.LogoY);

                    double aspect = _logoBitmap.Width / (double)_logoBitmap.Height;
                    float hPx = (float)(_style.LogoH * sy * mm2px);
                    float wPx = (float)(_style.LogoH * aspect * sx * mm2px);

                    var originalSmoothing = graphics.SmoothingMode;
                    var originalInterpolation = graphics.InterpolationMode;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    graphics.DrawImage(_logoBitmap,
                        new RectangleF(leftPx, topPx,
                                       Math.Max(1, wPx), Math.Max(1, hPx)));

                    graphics.SmoothingMode = originalSmoothing;
                    graphics.InterpolationMode = originalInterpolation;
                }
            }

            // 회사명
            if (_style.ShowBrandPreview)
            {
                var r = GetRow(RowKey.Brand);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(graphics, _style.BrandText ?? "",
                    _style.BrandX, _style.BrandY, _style.BrandFont,
                    ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, false, sx, sy);
            }

            // 품번
            if (_style.ShowPartPreview)
            {
                var r = GetRow(RowKey.Part);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                string text = _style.PartText ?? "";

                if (_style.PartX > 0)
                {
                    DrawTextTopLeft(graphics, text,
                        _style.PartX, _style.PartY, _style.PartFont,
                        ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, true, sx, sy);
                }
                else
                {
                    // 중앙정렬(프리뷰) — 측정 기반
                    float fontPx = (float)(_style.PartFont * mm2px);
                    float fontPt = Math.Max(1f, fontPx * 72f / graphics.DpiY);
                    using (var font = new Font("Arial", fontPt, FontStyle.Bold))
                    {
                        var textSize = graphics.MeasureString(text, font);
                        float centerPx = ConvertMmToPreviewX(_style.LabelWmm / 2.0);
                        float leftPx = centerPx - (float)(textSize.Width * sx / 2.0);
                        double leftMm = (leftPx - ConvertMmToPreviewX(0)) / mm2px;

                        DrawTextTopLeft(graphics, text,
                            leftMm, _style.PartY, _style.PartFont,
                            ConvertMmToPreviewX, ConvertMmToPreviewY, mm2px, true, sx, sy);
                    }
                }
            }
        }

        /// <summary> Pb 배지(프리뷰, 좌상단 기준) </summary>
        private void DrawPbBadge(Graphics graphics,
                                 Func<double, float> ConvertMmToPreviewX, Func<double, float> ConvertMmToPreviewY, float mm2px,
                                 double diameterMm, double xMm, double yMm,
                                 double scaleX, double scaleY)
        {
            float baseDiameterPixels = (float)(diameterMm * mm2px);
            float leftPixels = ConvertMmToPreviewX(xMm);
            float topPixels = ConvertMmToPreviewY(yMm);
            float widthPixels = baseDiameterPixels * (float)scaleX;
            float heightPixels = baseDiameterPixels * (float)scaleY;

            var badgeBounds = new RectangleF(leftPixels, topPixels, widthPixels, heightPixels);

            using (var fill = new SolidBrush(Color.Black))
                graphics.FillEllipse(fill, badgeBounds);
            using (var pen = new Pen(Color.Black, Math.Max(1f, 1.2f * mm2px)))
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
            Graphics graphics, Func<double, float> ConvertMmToPreviewX, Func<double, float> ConvertMmToPreviewY, float mm2px,
            double moduleMm, double xMm, double yMm,
            double columnCountInput, double rowCountInput)
        {
            // 모듈수(행/열) 결정: 지정값(10~144)이면 사용, 아니면 데이터 길이로 대략 추정
            int cols = (int)Math.Round(columnCountInput);
            int rows = (int)Math.Round(rowCountInput);
            if (cols < 10 || cols > 144 || rows < 10 || rows > 144)
            {
                int estimatedModuleCount = EstimateDmModulesFromData(BuildEtcsDmPayloadFromUi());
                cols = rows = estimatedModuleCount;
            }

            float modulePx = (float)(moduleMm * mm2px);
            float sidePx = modulePx * Math.Max(cols, rows);

            var DmBounds = new RectangleF(ConvertMmToPreviewX(xMm), ConvertMmToPreviewY(yMm), sidePx, sidePx);
            using (var pen = new Pen(Color.Black, 1f)) graphics.DrawRectangle(pen, DmBounds.X, DmBounds.Y, DmBounds.Width, DmBounds.Height);

            // 라벨
            float textPx = Math.Max(6f, Math.Min(sidePx * 0.28f, 28f));
            float textPt = textPx * 72f / graphics.DpiY;

            using (var font = new Font("Arial", textPt, FontStyle.Bold, GraphicsUnit.Point))
            using (var brush = new SolidBrush(Color.Black))
            using (var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.DrawString("DM", font, brush, DmBounds, textFormat);
            }
        }

        // 데이터 길이 기반 간이 추정 (자동 모드일 때 프리뷰만 대략 맞추기 용)
        private static int EstimateDmModulesFromData(string s)
        {
            int len = string.IsNullOrEmpty(s) ? 0 : s.Length;
            if (len <= 6) return 10;
            if (len <= 14) return 12;
            if (len <= 24) return 14;
            if (len <= 36) return 16;
            if (len <= 48) return 18;
            if (len <= 60) return 20;
            if (len <= 72) return 22;
            if (len <= 88) return 24;
            return Math.Min(144, 26 + (int)Math.Ceiling((len - 88) / 12.0));
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
            Func<double, float> ConvertMmToPreviewX, Func<double, float> ConvertMmToPreviewY, float mm2px,
            bool bold, double scaleXRatio, double scaleYRatio)
        {
            //float fontPx = (float)(mmFont * mm2px);
            //float fontPointSize = Math.Max(1f, fontPx * 72f / graphics.DpiY);

            int hDots = Math.Max(1, MmToDotsInt(mmFont * scaleYRatio, DEFAULT_DPI));
            int wDots = Math.Max(1, (int)Math.Round(hDots * scaleXRatio));
            float hPx = (float)(DotsToMm(hDots, DEFAULT_DPI) * mm2px);
            // GDI 폰트 포인트로 변환
            float fontPt = Math.Max(1f, hPx * 72f / graphics.DpiY);

            using (var font = new Font("Arial", fontPt, bold ? FontStyle.Bold : FontStyle.Regular))
            using (var brush = new SolidBrush(Color.Black))
            {
                var graphicsState = graphics.Save();
                try
                {
                    float xTop = ConvertMmToPreviewX(mmX);
                    float yTop = ConvertMmToPreviewY(mmY);

                    graphics.TranslateTransform(xTop, yTop);
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

                var path = ResolveLogoPath(_style.LogoImagePath);
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
        private string ResolveLogoPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            if (Path.IsPathRooted(path)) return File.Exists(path) ? path : null;

            var defaultDirectoryPath = Path.Combine(DEFAULT_LOGO_DIR, path);
            if (File.Exists(defaultDirectoryPath)) return defaultDirectoryPath;

            var applicationImagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", path);
            if (File.Exists(applicationImagesPath)) return applicationImagesPath;

            return null;
        }
    }
}

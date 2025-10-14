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
            var g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle pr = Preview.ClientRectangle;
            if (pr.Width <= 0 || pr.Height <= 0) return;

            // mm → px 스케일
            float scaleX = (float)(pr.Width / _style.LabelWmm);
            float scaleY = (float)(pr.Height / _style.LabelHmm);
            float mm2px = Math.Min(scaleX, scaleY);

            float labelWpx = (float)(_style.LabelWmm * mm2px);
            float labelHpx = (float)(_style.LabelHmm * mm2px);

            float originX = pr.Left + (pr.Width - labelWpx) / 2f;
            float originY = pr.Top + (pr.Height - labelHpx) / 2f;

            var labelRect = new RectangleF(originX, originY, labelWpx, labelHpx);
            using (var bg = new SolidBrush(Color.White))
            using (var pen = new Pen(Color.Silver, 1f))
            using (var rounded = CreateRoundedRectPath(labelRect, _style.CornerRadiusPx))
            {
                g.FillPath(bg, rounded);
                g.DrawPath(pen, rounded);
            }

            // 좌표 변환기(mm→px)
            //float MMX(double mm) => originX + (float)(mm * mm2px);
            //float MMY(double mm) => originY + (float)(mm * mm2px);
            const double NUDGE_Y_MM = 0.6; // 출력과 동일
            float MMX(double mm)
            {
                int xDots = MmToDotsInt(mm, DEFAULT_DPI);
                double xMmQ = DotsToMm(xDots, DEFAULT_DPI);
                return originX + (float)(xMmQ * mm2px);
            }
            float MMY(double mm)
            {
                // ^LT 보정 추가
                double mmWithLt = mm + NUDGE_Y_MM;
                int yDots = MmToDotsInt(mmWithLt, DEFAULT_DPI);
                double yMmQ = DotsToMm(yDots, DEFAULT_DPI);
                return originY + (float)(yMmQ * mm2px);
            }


            // 고정 요소(로고/브랜드/품번)
            DrawLogoBrandPart(g, MMX, MMY, mm2px);

            // HW
            if (_style.ShowHWPreview)
            {
                var r = GetRow(RowKey.HW);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(g, GetGridText(RowKey.HW, _style.HWText),
                                _style.HWx, _style.HWy, PositiveOr(_style.HWfont, 2.6),
                                MMX, MMY, mm2px, false, sx, sy);
            }

            // SW
            if (_style.ShowSWPreview)
            {
                var r = GetRow(RowKey.SW);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(g, GetGridText(RowKey.SW, _style.SWText),
                                _style.SWx, _style.SWy, PositiveOr(_style.SWfont, 2.6),
                                MMX, MMY, mm2px, false, sx, sy);
            }
            // LOT
            if (_style.ShowLOTPreview)
            {
                var r = GetRow(RowKey.LOT);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(g, GetGridText(RowKey.LOT, _style.LOTText),
                                _style.LOTx, _style.LOTy, PositiveOr(_style.LOTfont, 2.6),
                                MMX, MMY, mm2px, false, sx, sy);
            }
            // SN
            if (_style.ShowSNPreview)
            {
                var r = GetRow(RowKey.SN);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(g, GetGridText(RowKey.SN, _style.SerialText),
                                _style.SNx, _style.SNy, PositiveOr(_style.SNfont, 2.6),
                                MMX, MMY, mm2px, false, sx, sy);
            }

            // Pb 배지
            if (_style.ShowPbPreview)
            {
                var rPb = GetRow(RowKey.Pb);
                double sx = ReadScaleCell(rPb, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(rPb, COL_YSCALE, 1.0);

                DrawPbBadge(g, MMX, MMY, mm2px,
                            _style.BadgeDiameter, _style.BadgeX, _style.BadgeY,
                            sx, sy);
            }

            // DM 프리뷰
            if (_style.ShowDMPreview)
            {
                var rQR = GetRow(RowKey.DM);

                // 1) DM은 출력 시 모듈 크기가 "정수 도트"로 양자화됨 → 프리뷰도 동일 규칙 적용
                int moduleDots = Math.Max(1, MmToDots(Math.Max(0.1, _style.DMModuleMm), DEFAULT_DPI));
                double moduleMmForPreview = moduleDots * (25.4 / (double)DEFAULT_DPI);

                // 2) 그리드 X/Y를 DM 열/행(정수)로 활용(0 또는 범위 밖이면 자동)
                int cols = (int)Math.Round(ReadScaleCell(rQR, COL_XSCALE, 0.0));
                int rows = (int)Math.Round(ReadScaleCell(rQR, COL_YSCALE, 0.0));
                if (cols < 10 || cols > 144) cols = 0;
                if (rows < 10 || rows > 144) rows = 0;

                // 3) 프리뷰 그리기 (모듈수 × 모듈mm)
                DrawDataMatrixPreview(
                    g, MMX, MMY, mm2px,
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
                DrawTextTopLeft(g, GetGridText(RowKey.Rating, _style.RatingText),
                    _style.RatingX, _style.RatingY, PositiveOr(_style.RatingFont, 2.6),
                    MMX, MMY, mm2px, false, sx, sy);
            }

            // FCC ID
            if (_style.ShowFCCIDPreview)
            {
                var r = GetRow(RowKey.FCCID);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(g, GetGridText(RowKey.FCCID, _style.FCCIDText),
                    _style.FCCIDX, _style.FCCIDY, PositiveOr(_style.FCCIDFont, 2.6),
                    MMX, MMY, mm2px, false, sx, sy);
            }

            // IC ID
            if (_style.ShowICIDPreview)
            {
                var r = GetRow(RowKey.ICID);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(g, GetGridText(RowKey.ICID, _style.ICIDText),
                    _style.ICIDX, _style.ICIDY, PositiveOr(_style.ICIDFont, 2.6),
                    MMX, MMY, mm2px, false, sx, sy);
            }

            // Item1
            if (_style.ShowItem1Preview)
            {
                var r = GetRow(RowKey.Item1);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(g, GetGridText(RowKey.Item1, _style.Item1Text),
                    _style.Item1X, _style.Item1Y, PositiveOr(_style.Item1Font, 2.6),
                    MMX, MMY, mm2px, false, sx, sy);
            }
            // Item2
            if (_style.ShowItem2Preview)
            {
                var r = GetRow(RowKey.Item1);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(g, GetGridText(RowKey.Item2, _style.Item2Text),
                    _style.Item2X, _style.Item2Y, PositiveOr(_style.Item2Font, 2.6),
                    MMX, MMY, mm2px, false, sx, sy);
            }
            // Item3
            if (_style.ShowItem3Preview)
            {
                var r = GetRow(RowKey.Item3);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(g, GetGridText(RowKey.Item3, _style.Item3Text),
                    _style.Item3X, _style.Item3Y, PositiveOr(_style.Item3Font, 2.6),
                    MMX, MMY, mm2px, false, sx, sy);
            }
            // Item4
            if (_style.ShowItem4Preview)
            {
                var r = GetRow(RowKey.Item4);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(g, GetGridText(RowKey.Item4, _style.Item4Text),
                    _style.Item4X, _style.Item4Y, PositiveOr(_style.Item4Font, 2.6),
                    MMX, MMY, mm2px, false, sx, sy);
            }
            // Item5
            if (_style.ShowItem5Preview)
            {
                var r = GetRow(RowKey.Item5);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);
                DrawTextTopLeft(g, GetGridText(RowKey.Item5, _style.Item5Text),
                    _style.Item5X, _style.Item5Y, PositiveOr(_style.Item5Font, 2.6),
                    MMX, MMY, mm2px, false, sx, sy);
            }

        }

        /// <summary>로고/회사명/품번(프리뷰, 모두 좌상단 기준)</summary>
        private void DrawLogoBrandPart(Graphics g,
                                       Func<double, float> MMX,
                                       Func<double, float> MMY,
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

                    float leftPx = MMX(_style.LogoX);
                    float topPx = MMY(_style.LogoY);

                    double aspect = _logoBitmap.Width / (double)_logoBitmap.Height;
                    float hPx = (float)(_style.LogoH * sy * mm2px);
                    float wPx = (float)(_style.LogoH * aspect * sx * mm2px);

                    var oldS = g.SmoothingMode;
                    var oldI = g.InterpolationMode;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.DrawImage(_logoBitmap,
                        new RectangleF(leftPx, topPx,
                                       Math.Max(1, wPx), Math.Max(1, hPx)));

                    g.SmoothingMode = oldS;
                    g.InterpolationMode = oldI;
                }
            }

            // 회사명
            if (_style.ShowBrandPreview)
            {
                var r = GetRow(RowKey.Brand);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                DrawTextTopLeft(g, _style.BrandText ?? "",
                    _style.BrandX, _style.BrandY, _style.BrandFont,
                    MMX, MMY, mm2px, false, sx, sy);
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
                    DrawTextTopLeft(g, text,
                        _style.PartX, _style.PartY, _style.PartFont,
                        MMX, MMY, mm2px, true, sx, sy);
                }
                else
                {
                    // 중앙정렬(프리뷰) — 측정 기반
                    float fontPx = (float)(_style.PartFont * mm2px);
                    float fontPt = Math.Max(1f, fontPx * 72f / g.DpiY);
                    using (var font = new Font("Arial", fontPt, FontStyle.Bold))
                    {
                        var sz = g.MeasureString(text, font);
                        float centerPx = MMX(_style.LabelWmm / 2.0);
                        float leftPx = centerPx - (float)(sz.Width * sx / 2.0);
                        double leftMm = (leftPx - MMX(0)) / mm2px;

                        DrawTextTopLeft(g, text,
                            leftMm, _style.PartY, _style.PartFont,
                            MMX, MMY, mm2px, true, sx, sy);
                    }
                }
            }
        }

        /// <summary> Pb 배지(프리뷰, 좌상단 기준) </summary>
        private void DrawPbBadge(Graphics g,
                                 Func<double, float> MMX, Func<double, float> MMY, float mm2px,
                                 double diameterMm, double xMm, double yMm,
                                 double scaleX, double scaleY)
        {
            float baseD = (float)(diameterMm * mm2px);
            float x = MMX(xMm);
            float y = MMY(yMm);
            float w = baseD * (float)scaleX;
            float h = baseD * (float)scaleY;

            var rect = new RectangleF(x, y, w, h);

            using (var fill = new SolidBrush(Color.Black))
                g.FillEllipse(fill, rect);
            using (var pen = new Pen(Color.Black, Math.Max(1f, 1.2f * mm2px)))
                g.DrawEllipse(pen, rect);

            float fontPx = Math.Min(w, h) * 0.58f;
            float fontPt = fontPx * 72f / g.DpiY;
            using (var font = new Font("Arial", Math.Max(1f, fontPt), FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            using (var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("Pb", font, textBrush, rect, fmt);
            }
        }

        // Data Matrix 프리뷰: sidePx = 모듈수 × 모듈픽셀
        private void DrawDataMatrixPreview(
            Graphics g, Func<double, float> MMX, Func<double, float> MMY, float mm2px,
            double moduleMm, double xMm, double yMm,
            double colsIn, double rowsIn)
        {
            // 모듈수(행/열) 결정: 지정값(10~144)이면 사용, 아니면 데이터 길이로 대략 추정
            int cols = (int)Math.Round(colsIn);
            int rows = (int)Math.Round(rowsIn);
            if (cols < 10 || cols > 144 || rows < 10 || rows > 144)
            {
                int est = EstimateDmModulesFromData(BuildEtcsQrPayloadFromUi());
                cols = rows = est;
            }

            float modulePx = (float)(moduleMm * mm2px);
            float sidePx = modulePx * Math.Max(cols, rows);

            var rect = new RectangleF(MMX(xMm), MMY(yMm), sidePx, sidePx);
            using (var pen = new Pen(Color.Black, 1f)) g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            // 라벨
            float textPx = Math.Max(6f, Math.Min(sidePx * 0.28f, 28f));
            float textPt = textPx * 72f / g.DpiY;

            using (var font = new Font("Arial", textPt, FontStyle.Bold, GraphicsUnit.Point))
            using (var brush = new SolidBrush(Color.Black))
            using (var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("DM", font, brush, rect, fmt);
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

        private static GraphicsPath CreateRoundedRectPath(RectangleF rect, float radius)
        {
            var graphicsPath = new GraphicsPath();
            float diameter = radius * 2f;

            graphicsPath.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            graphicsPath.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            graphicsPath.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            graphicsPath.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);

            graphicsPath.CloseFigure();
            return graphicsPath;
        }

        /// <summary>
        /// 텍스트 그리기(프리뷰, 좌상단 기준)
        /// - mmFont → px/pt 변환
        /// - 좌상단(X,Y)로 이동 후 Scale(X,Y) 적용
        /// </summary>
        private static void DrawTextTopLeft(Graphics g, string text,
            double mmX, double mmY, double mmFont,
            Func<double, float> MMX, Func<double, float> MMY, float mm2px,
            bool bold, double scaleXRatio, double scaleYRatio)
        {
            //float fontPx = (float)(mmFont * mm2px);
            //float fontPt = Math.Max(1f, fontPx * 72f / g.DpiY);

            int hDots = Math.Max(1, MmToDotsInt(mmFont * scaleYRatio, DEFAULT_DPI));
            int wDots = Math.Max(1, (int)Math.Round(hDots * scaleXRatio));
            float hPx = (float)(DotsToMm(hDots, DEFAULT_DPI) * mm2px);
            // GDI 폰트 포인트로 변환
            float fontPt = Math.Max(1f, hPx * 72f / g.DpiY);

            using (var font = new Font("Arial", fontPt, bold ? FontStyle.Bold : FontStyle.Regular))
            using (var brush = new SolidBrush(Color.Black))
            {
                var state = g.Save();
                try
                {
                    float xTop = MMX(mmX);
                    float yTop = MMY(mmY);

                    g.TranslateTransform(xTop, yTop);
                    if (Math.Abs(scaleXRatio - 1.0) > 1e-6 || Math.Abs(scaleYRatio - 1.0) > 1e-6)
                        g.ScaleTransform((float)Math.Max(0.01, scaleXRatio),
                                         (float)Math.Max(0.01, scaleYRatio));

                    g.DrawString(text, font, brush, 0f, 0f);
                }
                finally { g.Restore(state); }
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
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var tmp = new Bitmap(fs))
                    {
                        _logoBitmap = new Bitmap(tmp);
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

            var inDefault = Path.Combine(DEFAULT_LOGO_DIR, path);
            if (File.Exists(inDefault)) return inDefault;

            var inExeImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", path);
            if (File.Exists(inExeImages)) return inExeImages;

            return null;
        }
    }
}

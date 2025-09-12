using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO; // File.Exists

namespace DHSTesterXL
{
    public partial class FormProduct
    {
        // ───────────────────── ZPL 생성 (좌상단 기준, 회전 없음) ─────────────────────
        private string BuildZplFromUi(int dpi = DEFAULT_DPI)
        {
            int PW = MmToDots(_style.LabelWmm, dpi); // Print Width
            int LL = MmToDots(_style.LabelHmm, dpi); // Label Length

            // 텍스트 페이로드
            string brand = _style.BrandText ?? "";
            string part = _style.PartText ?? "";
            string hw = GetGridText(RowKey.HW, _style.HWText ?? "");
            string sw = GetGridText(RowKey.SW, _style.SWText ?? "");
            string lot = GetGridText(RowKey.LOT, _style.LOTText ?? "");
            string sn = GetGridText(RowKey.SN, _style.SerialText ?? "");
            //string lotLn = BuildLotDisplay(_style.LOTText, _style.SerialText); // LOT + S/N

            // 인쇄 설정
            int darkness = Clamp(AsInt(numPrintDarkness?.Value, 0), 0, 30);
            int qty = Math.Max(1, AsInt(numPrintQty?.Value, 1));
            double ips; try { ips = Convert.ToDouble(numPrintSpeed?.Value ?? 0m); } catch { ips = 0.0; }

            var sb = new StringBuilder();
            sb.AppendLine("~SD" + darkness);
            sb.AppendLine("^XA");
            sb.AppendLine("^PW" + PW);
            sb.AppendLine("^LL" + LL);
            sb.AppendLine("^LH0,0");
            const double NUDGE_Y_MM = 0.6;  // 출력이 약간 위로 가는 오차 보정
            int lt = MmToDots(NUDGE_Y_MM, dpi);
            sb.AppendLine("^LT" + lt);
            sb.AppendLine("^LS0");
            if (ips > 0) sb.AppendLine("^PR" + ((int)Math.Round(ips)));
            sb.AppendLine("^PQ" + qty);

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

                    string gfa;
                    using (var canvas = new Bitmap(logoW, logoH))
                    using (var g = Graphics.FromImage(canvas))
                    {
                        g.Clear(Color.White);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(_logoBitmap, new Rectangle(0, 0, logoW, logoH));
                        int bpr, rows;
                        gfa = ToZplGFA(canvas, out bpr, out rows);
                    }

                    int x = MmToDots(_style.LogoX, dpi);
                    int y = MmToDots(_style.LogoY, dpi);
                    sb.AppendLine($"^FO{x},{y}{gfa}^FS");
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

                int x = MmToDots(_style.BrandX, dpi);
                int y = MmToDots(_style.BrandY, dpi);

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
                    int partY = MmToDots(_style.PartY, dpi);
                    sb.AppendLine($"^FO0,{partY}^FB{PW},1,0,C^A0N,{h},{w}^FD{Escape(part)}^FS");
                }
                else
                {
                    int x = MmToDots(_style.PartX, dpi);
                    int y = MmToDots(_style.PartY, dpi);
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

                int x = MmToDots(_style.HWx, dpi);
                int y = MmToDots(_style.HWy, dpi);

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

                int x = MmToDots(_style.SWx, dpi);
                int y = MmToDots(_style.SWy, dpi);

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

                int x = MmToDots(_style.LOTx, dpi);
                int y = MmToDots(_style.LOTy, dpi);

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

                int x = MmToDots(_style.SNx, dpi);
                int y = MmToDots(_style.SNy, dpi);

                sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(sn)}^FS");
            }

            /* ───────────────────── LOT + S/N ─────────────────────
            if (_style.ShowLOTPrint || _style.ShowSNPrint)
            {
                var r = GetRow(RowKey.LOT);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                int h = MmToDots(PositiveOr(_style.LOTfont, 2.6) * sy, dpi);
                int w = (int)Math.Round(h * sx);

                int x = MmToDots(_style.LOTx, dpi);
                int y = MmToDots(_style.LOTy, dpi);

                sb.AppendLine($"^FO{x},{y}^A0N,{h},{w}^FD{Escape(lotLn)}^FS");
            }*/

            // ───────────────────── Pb (^GE 타원 + 중앙 Pb) ─────────────────────
            if (_style.ShowPbPrint)
            {
                var r = GetRow(RowKey.Pb);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                int w = Math.Max(1, MmToDots(_style.BadgeDiameter * sx, dpi));
                int h = Math.Max(1, MmToDots(_style.BadgeDiameter * sy, dpi));
                int x = MmToDots(_style.BadgeX, dpi);
                int y = MmToDots(_style.BadgeY, dpi);

                int stroke = 2;
                int fh = (int)Math.Round(Math.Min(w, h) * 0.45);
                int fw = fh;

                sb.AppendLine($"^FO{x},{y}^GE{w},{h},{stroke}^FS");
                int fbWidth = Math.Max(w, h);
                sb.AppendLine($"^FO{x},{y}^FB{fbWidth},1,0,C^A0N,{fh},{fw}^FDPb^FS");
            }

            // ───────────────────── QR (좌상단 기준) ─────────────────────
            if (_style.ShowQRPrint)
            {
                var r = GetRow(RowKey.QR);
                double sx = ReadScaleCell(r, COL_XSCALE, 1.0);
                double sy = ReadScaleCell(r, COL_YSCALE, 1.0);

                double effModuleMm = Math.Max(0.1, _style.QRModuleMm * Math.Max(0.1, (sx + sy) / 2.0));
                int mag = MmToDots(effModuleMm, dpi);
                mag = Math.Max(1, Math.Min(mag, 10));   // 1~10

                int x = MmToDots(_style.QRx, dpi);
                int y = MmToDots(_style.QRy, dpi);

                string qr = AsciiSafeOneLine(BuildQrPayloadFromGrid());
                sb.AppendLine($"^FO{x},{y}^BQN,2,{mag}^FDQA,{qr}^FS");
            }

            sb.AppendLine("^XZ");
            return sb.ToString();
        }

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

        // QR 페이로드(그리드 데이터 모두 포함, key=value|... 형태)
        private string BuildQrPayloadFromGrid()
        {
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

            return sb.ToString();
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
}

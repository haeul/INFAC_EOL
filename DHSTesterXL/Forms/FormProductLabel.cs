using DHSTesterXL;   // LabelStyle
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO; // File.Exists

namespace DHSTesterXL
{
    /// <summary>
    /// ZEBRA ZD421 라벨 프린터 테스트/설정용 (라벨 탭)
    /// - 화면 프리뷰는 mm → px 변환
    /// - 인쇄(ZPL)는 mm → dots 변환
    /// - 모든 좌표/폰트/크기 수치는 mm 기준으로 일관 관리
    /// - LabelDataGridView 그리드로 항목(Logo/Brand/Part/Pb/HW/SW/LOT/SN/QR)의 좌표/크기/데이터를 제어
    ///   (회전 기능 제거, 모든 요소 좌상단 기준)
    /// </summary>
    public partial class FormProduct : Form
    {
        private const int DEFAULT_DPI = 203;

        private LabelStyle _style = new LabelStyle();
        private bool _suppressPreview;

        private Bitmap _logoBitmap;

        // 로고 기본 폴더
        private const string DEFAULT_LOGO_DIR = @"D:\INFAC_20250827\DHSEOL_V0\DHSTesterXL\Images";
        private string _lastLogoDir = null;

        // ───────────────────── Label Grid ─────────────────────
        private const string COL_SEQ = "Seq";
        private const string COL_FIELD = "Field";
        private const string COL_DATA = "Data";
        private const string COL_X = "Xmm";
        private const string COL_Y = "Ymm";
        private const string COL_SIZE = "Fontmm";
        private const string COL_XSCALE = "Xscale";   // 가로 비율
        private const string COL_YSCALE = "Yscale";   // 세로 비율

        // 표시 체크박스(미리보기/인쇄)
        private const string COL_SHOW_PREVIEW = "미리보기";
        private const string COL_SHOW_PRINT = "인쇄";

        // 행 키(고정)
        private enum RowKey { Logo, Brand, Part, Pb, HW, SW, LOT, SN, QR }

        // ───────────────────── 초기화 ─────────────────────
        partial void Label_Init()
        {
            // 이벤트 정리 후 재연결
            this.Load -= FormProductLabel_Load;
            this.Preview.Paint -= Preview_Paint;
            this.btnPreview.Click -= btnPreview_Click;
            this.btnPrint.Click -= btnPrint_Click;
            this.btnReset.Click -= btnReset_Click;
            this.btnTest.Click -= btnTest_Click;
            LabelDataGridView.CellDoubleClick -= LabelGrid_CellDoubleClick;

            this.Load += FormProductLabel_Load;
            this.Preview.Paint += Preview_Paint;
            this.btnPreview.Click += btnPreview_Click;
            this.btnPrint.Click += btnPrint_Click;
            this.btnReset.Click += btnReset_Click;
            this.btnTest.Click += btnTest_Click;

            // 로고 셀 더블클릭 → 파일 선택
            LabelDataGridView.CellDoubleClick += LabelGrid_CellDoubleClick;

            if (cmbPrinter != null) cmbPrinter.SelectedIndexChanged += (_, __) => _isModified = true;

            SetupLabelGrid();
            UpdateGridLabel();

            // 인쇄 방향 콤보
            comboPrintDir.Items.Clear();
            comboPrintDir.Items.Add("0° (Normal)");
            comboPrintDir.Items.Add("90° (Rotated)");
            comboPrintDir.Items.Add("180° (Inverted)");
            comboPrintDir.Items.Add("270° (Bottom-up)");

            // 로고 기본 폴더
            if (string.IsNullOrWhiteSpace(_lastLogoDir))
            {
                if (!string.IsNullOrWhiteSpace(_style.LogoImagePath) && File.Exists(ResolveLogoPath(_style.LogoImagePath)))
                    _lastLogoDir = Path.GetDirectoryName(ResolveLogoPath(_style.LogoImagePath));
                else if (Directory.Exists(DEFAULT_LOGO_DIR))
                    _lastLogoDir = DEFAULT_LOGO_DIR;
                else
                    _lastLogoDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
        }

        // 테스트 인쇄
        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                string hw = "1.00";
                string sw = "2.52";
                string lot = "240";
                string sn = "1234";
                string printerName = "ZDesigner ZD421-203dpi ZPL";

                // 설정값(ProductSettings.LabelPrint.PrinterName) 우선 사용
                GSystem.PrintProductLabel(hw, sw, lot, sn, printerName);

                MessageBox.Show("라벨 테스트 인쇄 요청을 보냈습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("라벨 테스트 중 오류: " + ex.Message);
            }
        }

        // 접두사 제거
        private static string StripPrefix(string value, string prefix)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            value = value.Trim();
            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? value.Substring(prefix.Length).Trim()
                : value;
        }

        // ───────────────────── 외부 연동용 퍼블릭 API ─────────────────────
        public LabelStyle Style => _style;

        public void LoadFromProduct(ProductConfig cfg)
        {
            _suppressPreview = true;
            try
            {
                var lp = cfg?.LabelPrint ?? new LabelPrintSettings();

                if (lp.Style != null)
                    _style = lp.Style.Clone();

                string part = cfg?.ProductInfo?.PartNo ?? "";
                if (string.IsNullOrWhiteSpace(_style.PartText))
                    _style.PartText = part;

                if (cmbPrinter != null &&
                    !string.IsNullOrWhiteSpace(lp.PrinterName) &&
                    cmbPrinter.Items.Contains(lp.PrinterName))
                {
                    cmbPrinter.SelectedItem = lp.PrinterName;
                }

                if (comboPrintDir != null) comboPrintDir.SelectedIndex = 0;

                if (numLabelWidth != null) numLabelWidth.Value = (decimal)(_style.LabelWmm > 0 ? _style.LabelWmm : 60.0);
                if (numLabelHeight != null) numLabelHeight.Value = (decimal)(_style.LabelHmm > 0 ? _style.LabelHmm : 15.0);
                if (numLabelGap != null) numLabelGap.Value = 1.0m;

                if (numPrintDarkness != null) numPrintDarkness.Value = 15;
                if (numSerialFrom != null) numSerialFrom.Value = 1;
                if (numSerialTo != null) numSerialTo.Value = 1;
                if (numPrintSpeed != null) numPrintSpeed.Value = 4;
                if (numPrintQty != null) numPrintQty.Value = 1;

                UpdateGridLabel();
            }
            finally
            {
                _suppressPreview = false;
                Preview.Invalidate();
            }
        }

        public void ApplyToProduct(ProductConfig cfg)
        {
            if (cfg == null) return;

            GetGridLabelValue();

            if (cfg.ProductInfo == null) cfg.ProductInfo = new SProductInfo();
            var pn = (_style.PartText ?? "").Trim();
            if (!string.IsNullOrEmpty(pn))
                cfg.ProductInfo.PartNo = pn;

            if (cfg.LabelPrint == null) cfg.LabelPrint = new LabelPrintSettings();
            cfg.LabelPrint.PrinterName = cmbPrinter?.SelectedItem?.ToString() ?? "";
            cfg.LabelPrint.Dpi = DEFAULT_DPI;
            cfg.LabelPrint.UseQr = true;
            cfg.LabelPrint.QRMagnification = Math.Max(1, MmToDots(_style.QRModuleMm, DEFAULT_DPI)); // 참고용
            cfg.LabelPrint.Style = _style.Clone();
        }

        public string BuildZpl() { GetGridLabelValue(); return BuildZplFromUi(DEFAULT_DPI); }

        public bool PrintTo(string printerName)
        {
            GetGridLabelValue();
            string zpl = BuildZplFromUi(DEFAULT_DPI);
            return SendRawToPrinter(printerName, zpl);
        }

        // ───────────────────── 로드 ─────────────────────
        private void FormProductLabel_Load(object sender, EventArgs e)
        {
            try
            {
                cmbPrinter.Items.Clear();
                foreach (string printerName in PrinterSettings.InstalledPrinters)
                    cmbPrinter.Items.Add(printerName);
                if (cmbPrinter.Items.Count > 0) cmbPrinter.SelectedIndex = 0;

                bool empty =
                    string.IsNullOrWhiteSpace(GetGridText(RowKey.HW, "")) &&
                    string.IsNullOrWhiteSpace(GetGridText(RowKey.SW, "")) &&
                    string.IsNullOrWhiteSpace(GetGridText(RowKey.LOT, "")) &&
                    string.IsNullOrWhiteSpace(GetGridText(RowKey.SN, ""));

                if (empty) ResetDefaults();
                else
                {
                    GetGridLabelValue();
                    Preview.Invalidate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("초기화 중 오류: " + ex.Message);
            }
        }

        private void ResetDefaults()
        {
            try
            {
                _suppressPreview = true;
                _style = new LabelStyle();
                UpdateGridLabel();
            }
            finally
            {
                _suppressPreview = false;
                Preview.Invalidate();
            }
        }

        // ───────────────────── 버튼 핸들러 ─────────────────────
        private void btnPreview_Click(object sender, EventArgs e)
        {
            GetGridLabelValue();
            Preview.Invalidate();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (cmbPrinter.SelectedItem == null)
            {
                MessageBox.Show("프린터를 선택하세요.");
                return;
            }
            GetGridLabelValue();
            string zpl = BuildZplFromUi(DEFAULT_DPI);
            bool ok = SendRawToPrinter(cmbPrinter.Text, zpl);
            MessageBox.Show(ok ? "인쇄 전송 완료" : "전송 실패: 프린터 이름/드라이버(ZPL) 확인");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetDefaults();
        }
    }
}

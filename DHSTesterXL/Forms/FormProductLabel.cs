using DHSTesterXL;   // LabelStyle
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO; // File.Exists
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls.WebParts;
//using System.Windows;
using System.Windows.Forms;

namespace DHSTesterXL
{
    /// <summary>
    /// ZEBRA ZD421 라벨 프린터 테스트/설정용 (라벨 탭)
    /// - 화면 프리뷰는 mm → px 변환
    /// - 인쇄(ZPL)는 mm → dots 변환
    /// - 모든 좌표/폰트/크기 수치는 mm 기준으로 일관 관리
    /// - LabelDataGridView 그리드로 항목(Logo, Brand, Part, Pb, Rating, HW, SW, LOT, SN, FCCID, ICID, DM, Item1, Item2, Item3, Item4, Item5)의 좌표/크기/데이터를 제어
    /// </summary>
    public partial class FormProduct : Form
    {
        private const int DEFAULT_DPI = 203;

        private LabelStyle _style = new LabelStyle();
        private bool _suppressPreview;

        private Bitmap _logoBitmap;

        // 로고 기본 폴더
        private const string DEFAULT_LOGO_DIR = @"D:\INFAC_20250915\DHS_EOL_V3\DHSTesterXL\Images";
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
        private enum RowKey { Logo, Brand, Part, Pb, Rating, HW, SW, LOT, SN, FCCID, ICID, DM, Item1, Item2, Item3, Item4, Item5 }

        // ───────────────────── 외부 연동용 퍼블릭 API ─────────────────────
        public LabelStyle Style => _style;

        public void LoadFromProduct(ProductConfig cfg)
        {
            _suppressPreview = true;
            try
            {
                var lp = cfg?.LabelPrint ?? new LabelPrintSettings();

                // 1) 스타일 복원
                if (lp.Style != null)
                    _style = lp.Style.Clone();

                // 품번 텍스트 보정
                string part = cfg?.ProductInfo?.PartNo ?? "";
                if (string.IsNullOrWhiteSpace(_style.PartText))
                    _style.PartText = part;

                // 2) 프린터 콤보 복원
                if (cmbPrinter != null &&
                    !string.IsNullOrWhiteSpace(lp.PrinterName) &&
                    cmbPrinter.Items.Contains(lp.PrinterName))
                {
                    cmbPrinter.SelectedItem = lp.PrinterName;
                }

                if (comboPrintDir != null) comboPrintDir.SelectedIndex = 0;

                // 라벨 크기 UI 기본 세팅
                if (numLabelWidth != null) numLabelWidth.Value = (decimal)(_style.LabelWmm > 0 ? _style.LabelWmm : 60.0);
                if (numLabelHeight != null) numLabelHeight.Value = (decimal)(_style.LabelHmm > 0 ? _style.LabelHmm : 15.0);

                // 3) 여기: JSON의 Runtime/ETCS 값을 UI 컨트롤에 복원
                if (cfg?.LabelPrint?.Runtime != null)
                {
                    if (numPrintDarkness != null) numPrintDarkness.Value = cfg.LabelPrint.Runtime.Darkness;
                    if (numPrintSpeed != null) numPrintSpeed.Value = (decimal)cfg.LabelPrint.Runtime.SpeedIPS;
                    if (numPrintQty != null) numPrintQty.Value = cfg.LabelPrint.Runtime.Quantity;
                }
                else
                {
                    if (numPrintDarkness != null) numPrintDarkness.Value = 15;
                    if (numPrintSpeed != null) numPrintSpeed.Value = 4;
                    if (numPrintQty != null) numPrintQty.Value = 1;
                }

                if (cfg?.LabelPrint?.Etcs != null)
                {
                    if (txtEtcsVendorValue != null) txtEtcsVendorValue.Text = cfg.LabelPrint.Etcs.Vendor;
                    if (txtEtcsPartNoValue != null) txtEtcsPartNoValue.Text = cfg.LabelPrint.Etcs.PartNo;
                    if (txtEtcsSerialValue != null) txtEtcsSerialValue.Text = cfg.LabelPrint.Etcs.Serial;
                    if (txtEtcsEoValue != null) txtEtcsEoValue.Text = cfg.LabelPrint.Etcs.Eo;
                    if (txtEtcsTraceValue != null) txtEtcsTraceValue.Text = cfg.LabelPrint.Etcs.Trace;
                    if (txtEtcsSpecialValue != null) txtEtcsSpecialValue.Text = cfg.LabelPrint.Etcs.A1;
                    if (txtEtcsInitialValue != null) txtEtcsInitialValue.Text = cfg.LabelPrint.Etcs.M;
                    if (txtEtcsCompanyAreaValue != null) txtEtcsCompanyAreaValue.Text = cfg.LabelPrint.Etcs.C;
                }
                else
                {
                    // JSON에 없으면 기본값 루틴 호출
                    // SetupEtcsDefaults();
                }

                // 4) 그리드(레이아웃/텍스트)도 현재 _style로 동기화
                UpdateGridLabel();

                // 5) DM 텍스트가 그리드 DM 데이터 칸에 반영되도록 한 번 갱신
                EtcsValueChanged(this, EventArgs.Empty);
            }
            finally
            {
                _suppressPreview = false;
                Preview?.Invalidate();
            }
        }


        public void ApplyToProduct(ProductConfig cfg)
        {
            if (cfg == null) return;

            // Grid → _style(좌표/폰트/표시여부/텍스트) 반영
            GetGridLabelValue();
            if (cfg.LabelPrint == null) cfg.LabelPrint = new LabelPrintSettings();
            cfg.LabelPrint.Style = _style.Clone();

            // 인쇄 옵션 저장
            cfg.LabelPrint.PrinterName = cmbPrinter?.SelectedItem?.ToString() ?? "";
            cfg.LabelPrint.Dpi = DEFAULT_DPI;
            cfg.LabelPrint.Runtime.Darkness = AsInt(numPrintDarkness?.Value, 15);
            cfg.LabelPrint.Runtime.SpeedIPS = Convert.ToDouble(numPrintSpeed?.Value ?? 0m);
            cfg.LabelPrint.Runtime.Quantity = AsInt(numPrintQty?.Value, 1);

            // DM(ETCS) 재료 저장 (우측 입력칸들)
            cfg.LabelPrint.Etcs.Vendor = txtEtcsVendorValue.Text;
            cfg.LabelPrint.Etcs.PartNo = txtEtcsPartNoValue.Text;
            cfg.LabelPrint.Etcs.Serial = txtEtcsSerialValue.Text;
            cfg.LabelPrint.Etcs.Eo = txtEtcsEoValue.Text;
            cfg.LabelPrint.Etcs.Trace = txtEtcsTraceValue.Text;
            cfg.LabelPrint.Etcs.A1 = txtEtcsSpecialValue.Text;
            cfg.LabelPrint.Etcs.M = txtEtcsInitialValue.Text;
            cfg.LabelPrint.Etcs.C = txtEtcsCompanyAreaValue.Text;

            // 즉시 저장
            cfg.Save(cfg.GetFileName(), GSystem.SystemData.GeneralSettings.ProductFolder);
        }


        public string BuildZpl() { GetGridLabelValue(); return BuildZplFromUi(DEFAULT_DPI); }

        public bool PrintTo(string printerName)
        {
            LabelDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            LabelDataGridView.EndEdit();
            GetGridLabelValue();
            string zpl = BuildZplFromUi(DEFAULT_DPI);
            return LabelPrinter.SendRawToPrinter(printerName, zpl);
        }

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

        private void SetupEtcsDefaults()
        {
            // ── 통신코드(표시용, ReadOnly 권장) ──
            txtEtcsCommHeader.Text = "[)>";
            txtEtcsCommVersion.Text = "06";
            txtEtcsCommGs.Text = @"\1D";   // GS
            txtEtcsCommRs.Text = @"\1E";   // RS
            txtEtcsCommEot.Text = @"\04";   // EOT

            // ── 왼쪽 태그(표시용, ReadOnly 권장) ──
            txtEtcsVTag.Text = "V";
            txtEtcsPTag.Text = "P";
            txtEtcsSTag.Text = "S";
            txtEtcsETag.Text = "E";
            txtEtcsTTag.Text = "T";
            txtEtcsATag.Text = "1A";
            txtEtcsMTag.Text = "M";
            txtEtcsCTag.Text = "C";

            // ── 값 기본값(운영에 맞게 바꿔도 됨) ──
            txtEtcsVendorValue.Text = "SUR2";                    // 업체코드
            txtEtcsPartNoValue.Text = GetGridText(RowKey.Part, _style.PartText);   // 품번 연동
            txtEtcsSerialValue.Text = "";                        // 서열코드(없으면 비움)
            txtEtcsEoValue.Text = "";                        // EO번호(없으면 비움)

            // 추적정보: YYMMDD + (세부칸) + 4M + 7자리
            // 예시: 오늘날짜 + "A" + "0000001"
            string yyMMdd = DateTime.Now.ToString("yyMMdd");
            txtEtcsTraceValue.Text = yyMMdd + "A" + "0000001";

            txtEtcsSpecialValue.Text = "";   // 1A (옵션)

            // 그리드/프리뷰 동기화
            EtcsValueChanged(this, EventArgs.Empty);
        }

        private void WireUpEtcsEvents()
        {
            txtEtcsVendorValue.TextChanged += EtcsValueChanged;
            txtEtcsPartNoValue.TextChanged += EtcsValueChanged;
            txtEtcsSerialValue.TextChanged += EtcsValueChanged;
            txtEtcsEoValue.TextChanged += EtcsValueChanged;
            txtEtcsTraceValue.TextChanged += EtcsValueChanged;
            txtEtcsSpecialValue.TextChanged += EtcsValueChanged;
            txtEtcsInitialValue.TextChanged += EtcsValueChanged;
            txtEtcsCompanyAreaValue.TextChanged += EtcsValueChanged;
        }

        private void EtcsValueChanged(object sender, EventArgs e)
        {
            RefreshDmDataCell();   // 그리드 DM 데이터 칸 갱신
            Preview?.Invalidate(); // 프리뷰 즉시 반영
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
        /*
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
        */
        private void ResetDefaults()
        {
            try
            {
                _suppressPreview = true;

                // 현재 품번과 제품 폴더 결정
                string partNo = _tempProductSettings?.ProductInfo?.PartNo ?? GSystem.ProductSettings?.ProductInfo?.PartNo ?? "";
                string productFolder = GSystem.SystemData?.GeneralSettings?.ProductFolder; // 시스템 설정에 정의됨
                
                // 예외 발생
                if (string.IsNullOrWhiteSpace(GSystem.SystemData?.GeneralSettings?.ProductFolder))
                    throw new InvalidOperationException("ProductFolder가 설정되지 않았습니다.");

                // JSON 로드 → LabelPrint.Style 적용
                LabelStyle styleFromJson = null;
                if (!string.IsNullOrWhiteSpace(partNo))
                {
                    var cfg = ProductConfig.GetInstance();
                    string fileName = partNo + GSystem.JSON_EXT;

                    if (cfg.Load(fileName, productFolder))
                    {
                        var lp = cfg.LabelPrint ?? new LabelPrintSettings();
                        if (lp.Style != null)
                            styleFromJson = lp.Style.Clone(); // 폼에서 안전하게 편집하기 위해 Clone
                    }
                }

                // 로드 실패/미존재 시 기존 기본값
                _style = styleFromJson ?? new LabelStyle();

                // 그리드/프리뷰 반영
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

            // 단일 경로로 위임 (PrintTo가 커밋/빌드/전송 전부 수행)
            bool ok = PrintTo(cmbPrinter.Text);

            MessageBox.Show(ok ? "인쇄 전송 완료" : "전송 실패: 프린터 이름/드라이버(ZPL) 확인");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetDefaults();
        }

        private void btnPrintFontTest_Click(object sender, EventArgs e)
        {
            string printer = cmbPrinter?.Text;
            if (string.IsNullOrWhiteSpace(printer)) return;

            PrintAllTtfSamples(printer, DEFAULT_DPI);
        }

        // 테스트 인쇄
        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                string hwVersion = "1.00";
                string swVersion = "2571";
                string lotNumber = "LOT NO:UH2";
                string sn = "S/N:0001";
                string partNo = "82667-P8100";
                string fccId = "FCC ID:2A93T-LQ2-DHS-NFC";
                string icId = "IC ID:30083-LQ2DHSNFC";
                string company = "INFAC ELECS";

                string EtcsVendor = "SUR2";
                string EtcsPartNo = "8266703200";
                string EtcsSerial = "";
                string EtcsEo = "";
                string EtcsTrace = "250807";
                string EtcsA1 = "A000";
                string EtcsM = "0";
                string EtcsC = "001";

                var payload = new LabelPayload
                {
                    HW = hwVersion,
                    SW = swVersion,
                    LOT = lotNumber,
                    SN = sn,
                    PartNo = partNo,
                    FCCID = fccId,
                    ICID = icId,
                    Company = company,
                    DataMatrix = null // DM은 보내지 않음 → JSON Etcs로 자동 생성
                };

                var etcs = new EtcsSettings
                {
                    Vendor = EtcsVendor,
                    PartNo = EtcsPartNo,
                    Serial = EtcsSerial,
                    Eo = EtcsEo,
                    Trace = EtcsTrace,
                    A1 = EtcsA1,
                    M = EtcsM,
                    C = EtcsC
                };

                // 설정값(ProductSettings.LabelPrint.PrinterName) 우선 사용 + 신규 데이터 전달
                GSystem.PrintProductLabel(
                    payload,
                    GSystem.ProductSettings.LabelPrint.Style,
                    etcs: etcs,
                    printerName: "ZDesigner ZD421-203dpi ZPL",
                    dpi: null, darkness: null, qty: 1, speedIps: 1
                );

                MessageBox.Show("라벨 테스트 인쇄 요청을 보냈습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("라벨 테스트 중 오류: " + ex.Message);
            }
        }

    }
}

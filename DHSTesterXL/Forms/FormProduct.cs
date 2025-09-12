using DHSTesterXL.Forms;
using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace DHSTesterXL
{
    public partial class FormProduct : Form
    {
        private Point _labelCanReqID_HS_Location = new Point(5, 51);
        private Point _labelCanResID_HS_Location = new Point(5, 79);
        private Point _labelNM_ReqID_HS_Location = new Point(5, 107);
        private Point _labelNM_ResID_HS_Location = new Point(5, 135);
        private Point _checkPLightReqID_HS_Location = new Point(41, 165);
        private Point _labelPLightResID_HS_Location = new Point(5, 193);
        private Point _textCanReqID_HS_Location = new Point(175, 51);
        private Point _textCanResID_HS_Location = new Point(175, 79);
        private Point _textNM_ReqID_HS_Location = new Point(175, 107);
        private Point _textNM_ResID_HS_Location = new Point(175, 135);
        private Point _textPLightReqID_HS_Location = new Point(175, 163);
        private Point _textPLightResID_HS_Location = new Point(175, 191);

        private Point _labelCanReqID_FD_Location = new Point(5, 247);
        private Point _labelCanResID_FD_Location = new Point(5, 275);
        private Point _labelNM_ReqID_FD_Location = new Point(5, 303);
        private Point _labelNM_ResID_FD_Location = new Point(5, 331);
        private Point _checkPLightReqID_FD_Location = new Point(41, 361);
        private Point _labelPLightResID_FD_Location = new Point(5, 389);
        private Point _textCanReqID_FD_Location = new Point(175, 247);
        private Point _textCanResID_FD_Location = new Point(175, 275);
        private Point _textNM_ReqID_FD_Location = new Point(175, 303);
        private Point _textNM_ResID_FD_Location = new Point(175, 331);
        private Point _textPLightReqID_FD_Location = new Point(175, 359);
        private Point _textPLightResID_FD_Location = new Point(175, 387);

        private readonly ProductConfig _tempProductSettings = new ProductConfig();
        private bool _isModified = false;

        public string CurrentProductNo { get; set; }
        public string NewProductNo { get; set; }

        public FormProduct()
        {
            InitializeComponent();
            // 임시 ProductSettings를 사용한다.
            _tempProductSettings = GSystem.ProductSettings;

            Label_Init(); // 라벨 탭 초기화(이벤트 연결)

            textProductName.TextChanged += (_, __) => CopyProductInfoToLabelTab();
            comboProductType.SelectionChangeCommitted += (_, __) => CopyProductInfoToLabelTab();
            textTypeNo.TextChanged += (_, __) => CopyProductInfoToLabelTab();
            textCarType.TextChanged += (_, __) => CopyProductInfoToLabelTab();
            textAlcNo.TextChanged += (_, __) => CopyProductInfoToLabelTab();
        }
        partial void Label_Init();

        private void FormProduct_Load(object sender, EventArgs e)
        {
            SetupProductInfo();
            SetupCommSettings();
            SetupGridTestList();
            SetupGridXcpList();
            SetupCommTypePosition(comboCommType.SelectedItem.ToString());
            labelUartPort.Location = new Point(5, 5);
            comboUartPort.Location = new Point(175, 5);

            // btnTest 버튼 숨기기/보이기
            btnTest.Visible = false;

            // 리스트에서 실제 품번을 하나 선택해 로드한다
            if (comboProductNo.Items.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(CurrentProductNo))
                    CurrentProductNo = comboProductNo.Items[0].ToString();

                comboProductNo.SelectedItem = CurrentProductNo;
                ApplyProductNo(CurrentProductNo); // ← 파일 로드 + UI 갱신 + 라벨탭 갱신(마지막 줄)
            }
        }

        private void FormProduct_Shown(object sender, EventArgs e)
        {
            _isModified = false;
        }

        private void FormProduct_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormProduct_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void UpdateProductNoList()
        {
            List<string> productNoList = _tempProductSettings.GetProductList(GSystem.SystemData.GeneralSettings.ProductFolder);
            comboProductNo.Items.Clear();
            foreach (string productNo in productNoList)
            {
                if (string.IsNullOrWhiteSpace(productNo)) continue;
                comboProductNo.Items.Add($"{productNo}");
            }
            // 선택 없으면 첫 번째 항목 자동 선택
            if (comboProductNo.SelectedIndex < 0 && comboProductNo.Items.Count > 0)
                comboProductNo.SelectedIndex = 0;
        }

        private void CopyProductInfoToLabelTab()
        {
            // 원본 값 꺼내기
            var name = (textProductName != null) ? textProductName.Text : "";
            var type = (comboProductType != null && comboProductType.SelectedItem != null)
                        ? comboProductType.SelectedItem : null;
            var car = (textCarType != null) ? textCarType.Text : "";
            var alc = (textAlcNo != null) ? textAlcNo.Text : "";

            if (textProductName2 != null) textProductName2.Text = name;
            if (comboProductType2 != null) comboProductType2.SelectedItem = type;
            if (textCarType2 != null) textCarType2.Text = car;
            if (textAlcNo2 != null) textAlcNo2.Text = alc;

            // 품번(PartNo) 결정: Product 탭 콤보 → 설정값 → 라벨 스타일(PartText)
            string partNo = "";
            if (comboProductNo != null && comboProductNo.SelectedItem != null)
                partNo = comboProductNo.SelectedItem.ToString();
            else if (_tempProductSettings != null && _tempProductSettings.ProductInfo != null
                     && !string.IsNullOrWhiteSpace(_tempProductSettings.ProductInfo.PartNo))
                partNo = _tempProductSettings.ProductInfo.PartNo;
            else if (_style != null && !string.IsNullOrWhiteSpace(_style.PartText))
                partNo = _style.PartText;

            // 회색 박스의 품번 콤보박스(comboProductNo2)에 반영
            if (comboProductNo2 != null)
            {
                if (comboProductNo != null)
                {
                    comboProductNo2.BeginUpdate();
                    comboProductNo2.Items.Clear();
                    foreach (var it in comboProductNo.Items) comboProductNo2.Items.Add(it);
                    comboProductNo2.EndUpdate();
                }

                int idx = comboProductNo2.Items.IndexOf(partNo);
                if (idx >= 0)
                {
                    comboProductNo2.SelectedIndex = idx;
                }
                else
                {
                    if (comboProductNo2.DropDownStyle == ComboBoxStyle.DropDownList)
                    {
                        comboProductNo2.Items.Add(partNo);
                        comboProductNo2.SelectedItem = partNo;
                    }
                    else
                    {
                        comboProductNo2.Text = partNo;
                    }
                }
            }

            // ▶ 라벨 탭 상태 갱신(텍스트박스 제거했으므로 스타일에만 기록)
            _style.PartText = partNo;     // 프리뷰/인쇄 시 이 값을 사용
            _isModified = true;
            Preview?.Invalidate();
        }


        private void SetupProductInfo()
        {
            UpdateProductNoList();
            comboProductNo.SelectedItem = CurrentProductNo;

            textProductName.Text = _tempProductSettings.ProductInfo.PartName;
            comboProductType.SelectedItem = _tempProductSettings.ProductInfo.TypeName;
            textTypeNo.Text = _tempProductSettings.ProductInfo.TypeNo;
            textCarType.Text = _tempProductSettings.ProductInfo.CarType;
            textAlcNo.Text = _tempProductSettings.ProductInfo.AlcNo;

            CopyProductInfoToLabelTab();
        }

        private void UpdateProductInfo()
        {
            textProductName.Text = _tempProductSettings.ProductInfo.PartName;
            comboProductType.SelectedItem = _tempProductSettings.ProductInfo.TypeName;
            textTypeNo.Text = _tempProductSettings.ProductInfo.TypeNo;
            textCarType.Text = _tempProductSettings.ProductInfo.CarType;
            textAlcNo.Text = _tempProductSettings.ProductInfo.AlcNo;

            CopyProductInfoToLabelTab();
        }
        private void GetProductInfoValue()
        {
            _tempProductSettings.ProductInfo.PartNo = comboProductNo.SelectedItem.ToString();
            _tempProductSettings.ProductInfo.PartName = textProductName.Text;
            _tempProductSettings.ProductInfo.TypeName = comboProductType.SelectedItem.ToString();
            _tempProductSettings.ProductInfo.TypeNo = textTypeNo.Text;
            _tempProductSettings.ProductInfo.CarType = textCarType.Text;
            _tempProductSettings.ProductInfo.AlcNo = textAlcNo.Text;
        }

        private void SetupCommSettings()
        {
            comboCommType.Items.Clear();
            comboCommType.Items.AddRange(GDefines.COMM_TYPE_STR);
            comboCommType.SelectedItem = _tempProductSettings.CommSettings.CommType;

            textCanArbBitRate.Text = _tempProductSettings.CommSettings.ArbtBitRate.ToString();
            textCanArbTseg1.Text = _tempProductSettings.CommSettings.ArbtTseg1.ToString();
            textCanArbTseg2.Text = _tempProductSettings.CommSettings.ArbtTseg2.ToString();
            textCanArbSjw.Text = _tempProductSettings.CommSettings.ArbtSjw.ToString();

            textCanDatBitRate.Text = _tempProductSettings.CommSettings.DataBitRate.ToString();
            textCanDatTseg1.Text = _tempProductSettings.CommSettings.DataTseg1.ToString();
            textCanDatTseg2.Text = _tempProductSettings.CommSettings.DataTseg2.ToString();
            textCanDatSjw.Text = _tempProductSettings.CommSettings.DataSjw.ToString();

            textCanReqID.Text = _tempProductSettings.CommSettings.CanReqID;
            textCanResID.Text = _tempProductSettings.CommSettings.CanResID;

            textNM_ReqID.Text = _tempProductSettings.CommSettings.NM_ReqID;
            textNM_ResID.Text = _tempProductSettings.CommSettings.NM_ResID;

            checkPLightReqID.Checked = _tempProductSettings.CommSettings.PLightUse;
            textPLightReqID.Text = _tempProductSettings.CommSettings.PLightReqID;
            textPLightResID.Text = _tempProductSettings.CommSettings.PLightResID;
            textPLightReqID.Enabled = _tempProductSettings.CommSettings.PLightUse;
            textPLightResID.Enabled = _tempProductSettings.CommSettings.PLightUse;

            //comboUartPort.SelectedItem = _tempProductSettings.CommSettings.UartPortName;
        }
        private void UpdateCommSettings()
        {
            comboCommType.SelectedItem = _tempProductSettings.CommSettings.CommType;

            textCanArbBitRate.Text = _tempProductSettings.CommSettings.ArbtBitRate.ToString();
            textCanArbTseg1.Text = _tempProductSettings.CommSettings.ArbtTseg1.ToString();
            textCanArbTseg2.Text = _tempProductSettings.CommSettings.ArbtTseg2.ToString();
            textCanArbSjw.Text = _tempProductSettings.CommSettings.ArbtSjw.ToString();

            textCanDatBitRate.Text = _tempProductSettings.CommSettings.DataBitRate.ToString();
            textCanDatTseg1.Text = _tempProductSettings.CommSettings.DataTseg1.ToString();
            textCanDatTseg2.Text = _tempProductSettings.CommSettings.DataTseg2.ToString();
            textCanDatSjw.Text = _tempProductSettings.CommSettings.DataSjw.ToString();

            textCanReqID.Text = _tempProductSettings.CommSettings.CanReqID;
            textCanResID.Text = _tempProductSettings.CommSettings.CanResID;

            textNM_ReqID.Text = _tempProductSettings.CommSettings.NM_ReqID;
            textNM_ResID.Text = _tempProductSettings.CommSettings.NM_ResID;

            checkPLightReqID.Checked = _tempProductSettings.CommSettings.PLightUse;
            textPLightReqID.Text = _tempProductSettings.CommSettings.PLightReqID;
            textPLightResID.Text = _tempProductSettings.CommSettings.PLightResID;
            textPLightReqID.Enabled = _tempProductSettings.CommSettings.PLightUse;
            textPLightResID.Enabled = _tempProductSettings.CommSettings.PLightUse;

            //comboUartPort.SelectedItem = _tempProductSettings.CommSettings.UartPortName;
        }
        private void GetCommSettingsValue()
        {
            _tempProductSettings.CommSettings.CommType = comboCommType.SelectedItem.ToString();

            _tempProductSettings.CommSettings.ArbtBitRate = Convert.ToInt32(textCanArbBitRate.Text);

            _tempProductSettings.CommSettings.ArbtTseg1 = Convert.ToInt32(textCanArbTseg1.Text);
            _tempProductSettings.CommSettings.ArbtTseg2 = Convert.ToInt32(textCanArbTseg2.Text);
            _tempProductSettings.CommSettings.ArbtSjw = Convert.ToInt32(textCanArbSjw.Text);

            _tempProductSettings.CommSettings.DataBitRate = Convert.ToInt32(textCanDatBitRate.Text);
            _tempProductSettings.CommSettings.DataTseg1 = Convert.ToInt32(textCanDatTseg1.Text);
            _tempProductSettings.CommSettings.DataTseg2 = Convert.ToInt32(textCanDatTseg2.Text);
            _tempProductSettings.CommSettings.DataSjw = Convert.ToInt32(textCanDatSjw.Text);

            _tempProductSettings.CommSettings.CanReqID = textCanReqID.Text;
            _tempProductSettings.CommSettings.CanResID = textCanResID.Text;

            _tempProductSettings.CommSettings.NM_ReqID = textNM_ReqID.Text;
            _tempProductSettings.CommSettings.NM_ResID = textNM_ResID.Text;

            _tempProductSettings.CommSettings.PLightUse = checkPLightReqID.Checked;
            _tempProductSettings.CommSettings.PLightReqID = textPLightReqID.Text;
            _tempProductSettings.CommSettings.PLightResID = textPLightResID.Text;

            //_tempProductSettings.CommSettings.UartPortName = comboUartPort.SelectedItem.ToString();
        }

        private void SetupGridTestList()
        {
            for (int i = 0; i < (int)TestItems.Count; i++)
            {
                TestSpec testSpec = _tempProductSettings.GetTestItemSpec((TestItems)i);
                gridTestList.Rows.Add();
                gridTestList[0, i].Value = (i + 1).ToString();
                gridTestList[1, i].Value = testSpec.Use;
                if ((TestItems)i == TestItems.SerialNumber || (TestItems)i == TestItems.Manufacture)
                {
                    // Name
                    gridTestList[2, i].Value = $"{testSpec.Name} ({GDefines.TEST_ITEM_OPTION[testSpec.Option]})";
                    // Option
                    gridTestList[5, i].Value = testSpec.Option.ToString();
                }
                else
                {
                    // Name
                    gridTestList[2, i].Value = testSpec.Name;
                }
                switch (testSpec.DataType)
                {
                    case 0:
                        gridTestList[3, i].Value = testSpec.MinValue.ToString("F1");
                        gridTestList[4, i].Value = testSpec.MaxValue.ToString("F1");
                        break;
                    case 1:
                        gridTestList[3, i].Value = testSpec.MinValue.ToString();
                        gridTestList[4, i].Value = testSpec.MaxValue.ToString();
                        break;
                    case 2:
                        gridTestList[3, i].Value = testSpec.MinString;
                        gridTestList[4, i].Value = testSpec.MaxString;
                        break;
                    default: break;
                }
            }
            gridTestList.CurrentCell = null;
        }
        private void UpdateGridTestList()
        {
            for (int i = 0; i < (int)TestItems.Count; i++)
            {
                TestSpec testSpec = _tempProductSettings.GetTestItemSpec((TestItems)i);
                gridTestList[0, i].Value = (i + 1).ToString();
                gridTestList[1, i].Value = testSpec.Use;
                if ((TestItems)i == TestItems.SerialNumber || (TestItems)i == TestItems.Manufacture)
                {
                    // Name
                    gridTestList[2, i].Value = $"{testSpec.Name} ({GDefines.TEST_ITEM_OPTION[testSpec.Option]})";
                    // Option
                    gridTestList[5, i].Value = testSpec.Option.ToString();
                }
                else
                {
                    // Name
                    gridTestList[2, i].Value = testSpec.Name;
                }
                switch (testSpec.DataType)
                {
                    case 0:
                        gridTestList[3, i].Value = testSpec.MinValue.ToString("F1");
                        gridTestList[4, i].Value = testSpec.MaxValue.ToString("F1");
                        break;
                    case 1:
                        gridTestList[3, i].Value = testSpec.MinValue.ToString();
                        gridTestList[4, i].Value = testSpec.MaxValue.ToString();
                        break;
                    case 2:
                        gridTestList[3, i].Value = testSpec.MinString;
                        gridTestList[4, i].Value = testSpec.MaxString;
                        break;
                    default: break;
                }
            }
            gridTestList.CurrentCell = null;
        }
        private void GetGridTestListValue()
        {
            for (int i = 0; i < (int)TestItems.Count; i++)
            {
                TestSpec testSpec = _tempProductSettings.GetTestItemSpec((TestItems)i);
                testSpec.Use = Convert.ToBoolean(gridTestList[1, i].Value);
                //testSpec.Name = gridTestList[2, i].Value.ToString();
                if ((TestItems)i == TestItems.SerialNumber || (TestItems)i == TestItems.Manufacture)
                {
                    // Option
                    testSpec.Option = Convert.ToUInt16(gridTestList[5, i].Value.ToString());
                }
                switch (testSpec.DataType)
                {
                    case 0:
                        testSpec.MinValue = Convert.ToDouble(gridTestList[3, i].Value.ToString());
                        testSpec.MaxValue = Convert.ToDouble(gridTestList[4, i].Value.ToString());
                        break;
                    case 1:
                        testSpec.MinValue = Convert.ToDouble(gridTestList[3, i].Value.ToString());
                        testSpec.MaxValue = Convert.ToDouble(gridTestList[4, i].Value.ToString());
                        break;
                    case 2:
                        testSpec.MinString = gridTestList[3, i].Value.ToString();
                        testSpec.MaxString = gridTestList[4, i].Value.ToString();
                        break;
                    default: break;
                }
                _tempProductSettings.SetTestItemSpec((TestItems)i, testSpec);
            }
        }

        private void SetupGridXcpList()
        {
            for (int i = 0; i < (int)XCP_Items.Count; i++)
            {
                XCPSpec xcpSpec = _tempProductSettings.GetXcpItemSpec((XCP_Items)i);
                gridXcpList.Rows.Add();
                gridXcpList[0, i].Value = (i + 1).ToString();
                gridXcpList[1, i].Value = xcpSpec.Use;
                gridXcpList[2, i].Value = xcpSpec.Name;
                gridXcpList[3, i].Value = xcpSpec.Address;
            }
            gridXcpList.CurrentCell = null;

            // XCP Use
            radioXcpUse.Checked = _tempProductSettings.XCPAddress.Use;
            radioXcpNotUse.Checked = !_tempProductSettings.XCPAddress.Use;
            // XCP Req.ID
            textXcpReqID.Text = _tempProductSettings.XCPAddress.ReqID;
            // XCP Res.ID
            textXcpResID.Text = _tempProductSettings.XCPAddress.ResID;
            // XCP ECU Addr
            textXcpEcuAddr.Text = _tempProductSettings.XCPAddress.EcuAddr;
        }
        private void UpdateGridXcpList()
        {
            for (int i = 0; i < (int)XCP_Items.Count; i++)
            {
                XCPSpec xcpSpec = _tempProductSettings.GetXcpItemSpec((XCP_Items)i);
                gridXcpList[0, i].Value = (i + 1).ToString();
                gridXcpList[1, i].Value = xcpSpec.Use;
                gridXcpList[2, i].Value = xcpSpec.Name;
                gridXcpList[3, i].Value = xcpSpec.Address;
            }
            gridXcpList.CurrentCell = null;

            // XCP Use
            radioXcpUse.Checked = _tempProductSettings.XCPAddress.Use;
            radioXcpNotUse.Checked = !_tempProductSettings.XCPAddress.Use;
            // XCP Req.ID
            textXcpReqID.Text = _tempProductSettings.XCPAddress.ReqID;
            // XCP Res.ID
            textXcpResID.Text = _tempProductSettings.XCPAddress.ResID;
            // XCP ECU Addr
            textXcpEcuAddr.Text = _tempProductSettings.XCPAddress.EcuAddr;
        }
        private void GetGridXcpListValue()
        {
            for (int i = 0; i < (int)XCP_Items.Count; i++)
            {
                XCPSpec xcpSpec = _tempProductSettings.GetXcpItemSpec((XCP_Items)i);
                xcpSpec.Use = Convert.ToBoolean(gridXcpList[1, i].Value);
                xcpSpec.Name = gridXcpList[2, i].Value.ToString();
                xcpSpec.Address = gridXcpList[3, i].Value.ToString();
                _tempProductSettings.SetXcpItemSpec((XCP_Items)i, xcpSpec);
            }

            // XCP Use
            _tempProductSettings.XCPAddress.Use = radioXcpUse.Checked;
            // XCP Req.ID
            _tempProductSettings.XCPAddress.ReqID = textXcpReqID.Text;
            // XCP Res.ID
            _tempProductSettings.XCPAddress.ResID = textXcpResID.Text;
            // XCP ECU Addr
            _tempProductSettings.XCPAddress.EcuAddr = textXcpEcuAddr.Text;
        }

        private void ApplyProductNo(string productNo)
        {
            string productPath = GSystem.SystemData.GeneralSettings.ProductFolder;
            string productFile = productNo + GSystem.JSON_EXT;
            string productFilePath = Path.Combine(productPath, productFile);
            _tempProductSettings.Load(productFilePath, GSystem.SystemData.GeneralSettings.ProductFolder);
            CurrentProductNo = productNo;
            SetupProductInfo();
            UpdateCommSettings();
            UpdateGridTestList();
            UpdateGridXcpList();
            _isModified = false;

            this.LoadFromProduct(_tempProductSettings);

            UpdateGridXcpList();
            _isModified = false;

            this.LoadFromProduct(_tempProductSettings); // 라벨 탭 동기화용 기존 호출
            CopyProductInfoToLabelTab();
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            FormProductNew formProductNew = new FormProductNew();
            formProductNew.CopyMode = false;
            if (formProductNew.ShowDialog() == DialogResult.OK)
            {
                if (formProductNew.NewProductNo != CurrentProductNo)
                {
                    // 새 품번 적용
                    ApplyProductNo(formProductNew.NewProductNo);
                }
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            FormProductNew formProductNew = new FormProductNew();
            formProductNew.CopyMode = true;
            formProductNew.CurrentProductNo = CurrentProductNo;
            if (formProductNew.ShowDialog() == DialogResult.OK)
            {
                if (formProductNew.NewProductNo != CurrentProductNo)
                {
                    // 새 품번 적용
                    ApplyProductNo(formProductNew.NewProductNo);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string productNo = comboProductNo.SelectedItem.ToString();
            string caption = $"삭제";
            string message = $"[{productNo}] 품목을 삭제하시겠습니까?";
            if (MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            // 삭제
            string productPath = Application.StartupPath + "\\" + GSystem.PRODUCTS_PATH;
            string selectedNo = comboProductNo.SelectedItem.ToString();
            string selectedFile = comboProductNo.SelectedItem.ToString() + GSystem.JSON_EXT;
            string selectedFilePath = Path.Combine(productPath, selectedFile);

            try
            {
                File.Delete(selectedFilePath);

                // 품번 리스트 중 첫 번째 품번을 선택한다.
                UpdateProductNoList();
                if (comboProductNo.Items.Count > 0)
                {
                    comboProductNo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string caption = "저장";
            string message = "품목 설정을 저장하시겠습니까?";
            if (MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            SaveSettings();
        }

        private void SaveSettings()
        {
            GetProductInfoValue();
            GetCommSettingsValue();
            GetGridTestListValue();
            GetGridXcpListValue();
            // ✅ 라벨 그리드 → _style 반영(그리고 ApplyToProduct에서 라벨 섹션 복사)
            GetGridLabelValue();
            this.ApplyToProduct(_tempProductSettings); // Label 탭 정보 회수
            GSystem.ProductSettings.ProductInfo = _tempProductSettings.ProductInfo;
            GSystem.ProductSettings.CommSettings = _tempProductSettings.CommSettings;
            GSystem.ProductSettings.TestItemSpecs = _tempProductSettings.TestItemSpecs;
            GSystem.ProductSettings.XCPAddress = _tempProductSettings.XCPAddress;
            GSystem.ProductSettings.TestInfo = _tempProductSettings.TestInfo;
            GSystem.ProductSettings.LabelPrint = _tempProductSettings.LabelPrint; // 라벨 섹션 복사
            string productSettingsFileName = _tempProductSettings.ProductInfo.PartNo + GSystem.JSON_EXT;
            GSystem.ProductSettings.Save(productSettingsFileName, GSystem.SystemData.GeneralSettings.ProductFolder);
            _isModified = false;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (_isModified)
            {
                string caption = "저장 확인";
                string message = "설정값이 변경된 항목이 있습니다. 닫기 전에 저장하시겠습니까?\n[예]\t저장 후 닫기\n[아니요]\t저장하지 않고 닫기\n[취소]\t닫기를 취소하고 화면으로 복귀\t";
                DialogResult dialogResult = MessageBox.Show(this, message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Cancel) return;
                else if (dialogResult == DialogResult.Yes) SaveSettings();
            }
            NewProductNo = comboProductNo.SelectedItem.ToString();
            Close();
        }

        private void comboCommType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Enable/Disable
            //if (comboCommType.SelectedIndex == 0)
            //{
            //    // CAN
            //    SetupCommTypePosition(comboCommType.SelectedItem.ToString());
            //}
            //else if (comboCommType.SelectedIndex == 1)
            //{
            //    // CAN FD
            //    SetupCommTypePosition(comboCommType.SelectedItem.ToString());
            //}
            SetupCommTypePosition(comboCommType.SelectedItem.ToString());
        }

        private void SetupCommTypePosition(string commType)
        {
            if (commType == "CAN")
            {
                labelCanArbBitRate.Visible = true;
                labelCanDatBitRate.Visible = false;
                textCanArbBitRate.Visible = true;
                textCanDatBitRate.Visible = false;

                labelUartPort.Visible = false;
                comboUartPort.Visible = false;

                //labelCanArbTseg1  .Visible = false;
                //labelCanArbTseg2  .Visible = false;
                //labelCanArbSjw    .Visible = false;
                //labelCanDatBitRate.Visible = false;
                //labelCanDatTseg1  .Visible = false;
                //labelCanDatTseg2  .Visible = false;
                //labelCanDatSjw    .Visible = false;

                //textCanArbTseg1  .Visible = false;
                //textCanArbTseg2  .Visible = false;
                //textCanArbSjw    .Visible = false;
                //textCanDatBitRate.Visible = false;
                //textCanDatTseg1  .Visible = false;
                //textCanDatTseg2  .Visible = false;
                //textCanDatSjw    .Visible = false;

                labelCanArbBitRate.Text = "Bit Rate [bit/s]";
                //labelCanReqID.Location    = _labelCanReqID_HS_Location;
                //labelCanResID.Location    = _labelCanResID_HS_Location;
                //labelNM_ReqID.Location    = _labelNM_ReqID_HS_Location;
                //labelNM_ResID.Location    = _labelNM_ResID_HS_Location;
                //checkPLightReqID.Location = _checkPLightReqID_HS_Location;
                //labelPLightResID.Location = _labelPLightResID_HS_Location;
                //textCanReqID.Location     = _textCanReqID_HS_Location;
                //textCanResID.Location     = _textCanResID_HS_Location;
                //textNM_ReqID.Location     = _textNM_ReqID_HS_Location;
                //textNM_ResID.Location     = _textNM_ResID_HS_Location;
                //textPLightReqID.Location  = _textPLightReqID_HS_Location;
                //textPLightResID.Location  = _textPLightResID_HS_Location;
            }
            else if (commType == "CAN FD")
            {
                labelCanArbBitRate.Visible = true;
                labelCanDatBitRate.Visible = true;
                textCanArbBitRate.Visible = true;
                textCanDatBitRate.Visible = true;

                labelUartPort.Visible = false;
                comboUartPort.Visible = false;

                //labelCanArbTseg1  .Visible = true;
                //labelCanArbTseg2  .Visible = true;
                //labelCanArbSjw    .Visible = true;
                //labelCanDatBitRate.Visible = true;
                //labelCanDatTseg1  .Visible = true;
                //labelCanDatTseg2  .Visible = true;
                //labelCanDatSjw    .Visible = true;

                //textCanArbTseg1  .Visible = true;
                //textCanArbTseg2  .Visible = true;
                //textCanArbSjw    .Visible = true;
                //textCanDatBitRate.Visible = true;
                //textCanDatTseg1  .Visible = true;
                //textCanDatTseg2  .Visible = true;
                //textCanDatSjw    .Visible = true;

                labelCanArbBitRate.Text = "Arbitration Bit Rate [bit/s]";
                //labelCanReqID.Location    = _labelCanReqID_FD_Location;
                //labelCanResID.Location    = _labelCanResID_FD_Location;
                //labelNM_ReqID.Location    = _labelNM_ReqID_FD_Location;
                //labelNM_ResID.Location    = _labelNM_ResID_FD_Location;
                //checkPLightReqID.Location = _checkPLightReqID_FD_Location;
                //labelPLightResID.Location = _labelPLightResID_FD_Location;
                //textCanReqID.Location     = _textCanReqID_FD_Location;
                //textCanResID.Location     = _textCanResID_FD_Location;
                //textNM_ReqID.Location     = _textNM_ReqID_FD_Location;
                //textNM_ResID.Location     = _textNM_ResID_FD_Location;
                //textPLightReqID.Location  = _textPLightReqID_FD_Location;
                //textPLightResID.Location  = _textPLightResID_FD_Location;
            }
            else if (commType == "UART")
            {
                labelUartPort.Visible = true;
                comboUartPort.Visible = true;

                labelCanArbBitRate.Visible = false;
                labelCanDatBitRate.Visible = false;
                textCanArbBitRate.Visible = false;
                textCanDatBitRate.Visible = false;
            }
        }

        private void gridTestList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (gridTestList.CurrentCell is DataGridViewCheckBoxCell)
            {
                gridTestList.CommitEdit(DataGridViewDataErrorContexts.Commit); // 즉시 상태 반영
                if (e.RowIndex > -1)
                {
                    // 셀을 클릭했을 때
                    bool checkedState = Convert.ToBoolean(gridTestList.Rows[e.RowIndex].Cells[(int)TestListColumns.Use].Value);
                    GSystem.TraceMessage($"Click cell: Row = {e.RowIndex}, Col = {e.ColumnIndex}, Check = {checkedState}");
                }
                else
                {
                    // 헤더를 클릭했을 때, 첫 번째 행의 상태를 반영한다.
                    gridTestList.CurrentCell = null;
                    bool checkedState = Convert.ToBoolean(gridTestList.Rows[0].Cells[(int)TestListColumns.Use].Value);
                    for (int i = 0; i < gridTestList.Rows.Count; i++)
                    {
                        gridTestList.Rows[i].Cells[(int)TestListColumns.Use].Value = !checkedState;
                    }
                }
            }
        }

        private void gridTestList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gridTestList.IsCurrentCellDirty)
            {
                // 즉시 상태 반영
                //gridTestList.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void comboProductNo_DropDown(object sender, EventArgs e)
        {
            UpdateProductNoList();
        }

        private void comboProductNo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // 품번 파일을 읽은 후 화면을 업데이트 한다.
                if (_isModified)
                {
                    string caption = "저장 확인";
                    string message = "설정값이 변경된 항목이 있습니다. 품목을 변경하기 전에 저장하시겠습니까?\n[예]\t저장 후 변경\n[아니요]\t저장하지 않고 변경\n[취소]\t변경을 취소하고 화면으로 복귀\t";
                    DialogResult dialogResult = MessageBox.Show(this, message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Cancel) return;
                    else if (dialogResult == DialogResult.Yes) SaveSettings();
                }
                ApplyProductNo(comboBox.SelectedItem.ToString());
            }
        }

        private void comboProductType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _isModified = true;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            _isModified = true;
        }

        private void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView dataGrid)
            {
                if (dataGrid.Tag.ToString() == "TestList")
                {
                    if (e.ColumnIndex == 5/*Option*/)
                    {
                        if ((TestItems)e.RowIndex == TestItems.SerialNumber || (TestItems)e.RowIndex == TestItems.Manufacture)
                        {
                            TestSpec testSpec = _tempProductSettings.GetTestItemSpec((TestItems)e.RowIndex);
                            int option = Convert.ToUInt16(dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                            // Option이 0일때 WR(Write), 그 외에는 RO(Read Only)
                            if (option == 0)
                            {
                                dataGrid.Rows[e.RowIndex].Cells[2].Value = $"{testSpec.Name} ({GDefines.TEST_ITEM_OPTION[0]})"; // WR
                            }
                            else
                            {
                                dataGrid.Rows[e.RowIndex].Cells[2].Value = $"{testSpec.Name} ({GDefines.TEST_ITEM_OPTION[1]})"; // RO
                                if (option != 1)
                                    dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 1;
                            }
                        }
                    }
                }
                else
                if (dataGrid.Tag.ToString() == "XCP")
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex == 3/*Address*/)
                    {
                        try
                        {
                            if (dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                            {
                                string value = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                                uint id = Convert.ToUInt32(value, 16);
                                dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = $"0x{id:X08}";
                            }
                            else
                            {
                                dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = $"0x{0:X08}";
                            }
                        }
                        catch
                        {
                            string caption = "입력 오류";
                            string message = "16진수로 변환할 수 없는 문자가 포함되어 있습니다.\n확인하시고 다시 입력해 주세요.";
                            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            _isModified = true;
        }

        private void textID_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is System.Windows.Forms.TextBox textBox)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    try
                    {
                        uint id = Convert.ToUInt32(((System.Windows.Forms.TextBox)sender).Text, 16);
                        textBox.Text = $"0x{id:X}";
                    }
                    catch
                    {
                        string caption = "입력 오류";
                        string message = "16진수로 변환할 수 없는 문자가 포함되어 있습니다.\n확인하시고 다시 입력해 주세요.";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void textID_Leave(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.TextBox textBox)
            {
                try
                {
                    uint id = Convert.ToUInt32(((System.Windows.Forms.TextBox)sender).Text, 16);
                    textBox.Text = $"0x{id:X}";
                }
                catch
                {
                    string caption = "입력 오류";
                    string message = "16진수로 변환할 수 없는 문자가 포함되어 있습니다.\n확인하시고 다시 입력해 주세요.";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Focus();
                }
            }
        }

        private void checkPLightReqID_CheckedChanged(object sender, EventArgs e)
        {
            textPLightReqID.Enabled = checkPLightReqID.Checked;
            textPLightResID.Enabled = checkPLightReqID.Checked;
        }

        private void comboUartPort_DropDown(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // 시리얼 포트 콤보 박스가 열릴때 시스템에 등록된 시리얼 포트를 등록한다.
                string[] portList = System.IO.Ports.SerialPort.GetPortNames();
                if (portList.Length > 0)
                {
                    Array.Sort(portList);
                    comboBox.Items.Clear();
                    comboBox.Items.AddRange(portList);
                }
            }
        }

        private void comboUartPort_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                //GSystem.SystemData.DedicatedCtrlSettings.PortName = comboBox.SelectedItem.ToString();
                //GSystem.TraceMessage($"DedicatedCtrlSettings.PortName = {GSystem.SystemData.DedicatedCtrlSettings.PortName}");
            }
        }
    }
}

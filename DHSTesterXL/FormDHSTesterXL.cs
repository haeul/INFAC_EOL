using DHSTesterXL.Forms;
using GSCommon;
using MetroFramework;
using MetroFramework.Components;
using Microsoft.WindowsAPICodePack.Dialogs;
using Modbus.Device;
using Modbus.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using vxlapi_NET;
using static DHSTesterXL.GSystem;
using static System.Windows.Forms.AxHost;

// 프로그램 내 한번만 지정
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace DHSTesterXL
{
    public partial class FormDHSTesterXL : MetroFramework.Forms.MetroForm
    {
        public string CurrentProductNo { get; set; }

        public FormDHSTesterXL()
        {
            InitializeComponent();
            GSystem.Logger.Info("-------------------------------------------------------");
            GSystem.Logger.Info("                INFAC DHS Tester V1.00                 ");
            GSystem.Logger.Info("Copyright (c) 2025 by GS Co., Ltd. All rights reserved.");
            GSystem.Logger.Info("-------------------------------------------------------");

            this.StyleManager = metroStyleManager;
        }

        private void FormDHSTesterXL_Load(object sender, EventArgs e)
        {
            //metroStyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            metroStyleManager.Style = MetroFramework.MetroColorStyle.Orange;

            // 시스템데이터 로딩
            GSystem.Initialize(this);

            // 델리게이터 
            GSystem.MainFormMessageBox = MainFormMessageBox;

            // 전용 컨트롤러 연결
            if (GSystem.ConnectDedicatedCTRL())
            {
                GSystem.DedicatedCTRL.StartAsync();
            }

            // PLC 연결
            if (GSystem.SystemData.PLCSettings.AutoConnect)
            {
                GSystem.MiPLC.Connect();
            }

            ControlHelper.SetDoubleBuffered(gridTestListCh1, true);
            ControlHelper.SetDoubleBuffered(gridTestListCh2, true);

            GSystem.SetButtonForeColor(buttonPowerCh1, Color.Black, Color.DimGray);
            GSystem.SetButtonForeColor(buttonStartCh1, Color.Black, Color.DimGray);
            GSystem.SetButtonForeColor(buttonLockCh1, Color.Black, Color.DimGray);

            GSystem.SetButtonForeColor(buttonPowerCh2, Color.Black, Color.DimGray);
            GSystem.SetButtonForeColor(buttonStartCh2, Color.Black, Color.DimGray);
            GSystem.SetButtonForeColor(buttonLockCh2, Color.Black, Color.DimGray);

            // 업데이트 타이머 실행
            timerUpdate.Start();
        }

        private async void FormDHSTesterXL_Shown(object sender, EventArgs e)
        {
            // Product 폴더가 없으면 선택
            if (!Directory.Exists(GSystem.SystemData.GeneralSettings.ProductFolder))
            {
                string message = "품번 폴더가 없습니다. 품번 폴더를 선택해 주세요";
                string caption = "품번 폴더 선택";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // 폴더 선택
                CommonFileDialogResult dialogResult = CommonFileDialogResult.Ok;
                CommonOpenFileDialog dlg = new CommonOpenFileDialog();
                dlg.Title = "품번 폴더 선택";
                do
                {
                    dlg.InitialDirectory = @"./";
                    dlg.IsFolderPicker = true;
                    dialogResult = dlg.ShowDialog();
                } while (dialogResult != CommonFileDialogResult.Ok);
                GSystem.SystemData.GeneralSettings.ProductFolder = dlg.FileName;
                GSystem.SystemData.Save();
            }

            // Producto 폴더의 파일 리스트 생성
            List<string> productNoList = GSystem.ProductSettings.GetProductList(GSystem.SystemData.GeneralSettings.ProductFolder);
            if (productNoList.Count > 0)
            {
                // Product 폴더에 파일이 있는 경우
                // 이전 작업 파일이 있는지 확인
                if (GSystem.SystemData.ProductSettings.LastProductNo != "")
                {
                    string lastProductFileName = GSystem.SystemData.ProductSettings.LastProductNo + GSystem.JSON_EXT;
                    string productFilePath = Path.Combine(GSystem.SystemData.GeneralSettings.ProductFolder, lastProductFileName);

                    if (!File.Exists(productFilePath))
                    {
                        // 파일 없음...품번 선택
                        DialogResult dialogResult = DialogResult.Cancel;
                        FormSelectProduct formSelectProduct = new FormSelectProduct();
                        do
                        {
                            dialogResult = formSelectProduct.ShowDialog();
                            if (dialogResult != DialogResult.OK)
                            {
                                string message = "품번을 선택해 주세요";
                                string caption = "품번 선택";
                                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        } while (dialogResult != DialogResult.OK);
                        GSystem.SystemData.ProductSettings.LastProductNo = formSelectProduct.SelectedProductNo;
                        GSystem.SystemData.Save();
                    }
                }
                else
                {
                    // 품번 선택
                    DialogResult dialogResult = DialogResult.Cancel;
                    FormSelectProduct formSelectProduct = new FormSelectProduct();
                    do
                    {
                        dialogResult = formSelectProduct.ShowDialog();
                        if (dialogResult != DialogResult.OK)
                        {
                            string message = "품번을 선택해 주세요";
                            string caption = "품번 선택";
                            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    } while (dialogResult != DialogResult.OK);
                    GSystem.SystemData.ProductSettings.LastProductNo = formSelectProduct.SelectedProductNo;
                    GSystem.SystemData.Save();
                }
            }
            else
            {
                // Product 폴더에 파일이 없는 경우...신규 품번 생성
                DialogResult dialogResult = DialogResult.Cancel;
                FormProductNew formProductNew = new FormProductNew();
                formProductNew.CopyMode = false;
                do
                {
                    dialogResult = formProductNew.ShowDialog();
                    if (dialogResult != DialogResult.OK)
                    {
                        string message = "품번을 생성해 주세요";
                        string caption = "품번 생성";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                } while (dialogResult != DialogResult.OK);
                GSystem.SystemData.ProductSettings.LastProductNo = formProductNew.NewProductNo;
                GSystem.SystemData.Save();
            }

            // 품번 로딩
            GSystem.ProductSettings.Load(GSystem.SystemData.ProductSettings.LastProductNo + GSystem.JSON_EXT, GSystem.SystemData.GeneralSettings.ProductFolder);

            // CAN/UART 통신 방식이 달라도 동일 방법으로 사용되어야 한다.
            // UART 사양도 IDHSModel 인터페이스를 상속받아야 한다.
            if (GSystem.ProductSettings.CommSettings.CommType == "CAN")
            {
                GSystem.DHSModel = new PNFCTouch();
            }
            else if (GSystem.ProductSettings.CommSettings.CommType == "CAN FD")
            {
                GSystem.DHSModel = new PNFCTouchFD();
            }
            else if (GSystem.ProductSettings.CommSettings.CommType == "UART")
            {
                GSystem.DHSModel = new PTouchOnly();
            }

            //if (GSystem.ProductSettings.CommSettings.CommType == "CAN" || GSystem.ProductSettings.CommSettings.CommType == "CAN FD")
            //{
            //    // can, can fd 사양
            //    if (await CanXL.LoadDriver() == XLDefine.XL_Status.XL_SUCCESS)
            //    {
            //        TraceMessage($"CanXL.LoadDriver() : Success.");

            //        for (int ch = 0; ch < GSystem.ChannelCount; ch++)
            //        {
            //            if (await GSystem.DHSModel.OpenPort(ch) == XLDefine.XL_Status.XL_SUCCESS)
            //            {
            //                TraceMessage($"GSystem.DHSModel.OpenPort(CH{ch + 1}) : Success.");
            //            }
            //            else
            //            {
            //                TraceMessage($"GSystem.DHSModel.OpenPort(CH{ch + 1}) : Failed.");
            //            }
            //        }
            //    }
            //    else
            //    {
            //        TraceMessage($"CanXL.LoadDriver() : Failed.");
            //    }
            //}
            //else
            //{
            //    // uart 사양
            //}
            // 이벤트핸들러
            GSystem.DHSModel.TestItemsList = GSystem.ProductSettings.GetEnableTestItemsList();
            GSystem.DHSModel.TestStateChanged += OnTestStateChanged;
            GSystem.DHSModel.TestStepProgressChanged += OnTestStepProgressChanged;
            GSystem.DHSModel.RxsWinDataChanged += OnRxsWinDataChanged;
            GSystem.DHSModel.TouchXcpDataChanged += OnUpdateTouchXcpData;
            GSystem.DHSModel.CancelXcpDataChanged += OnUpdateCancelXcpData;
            GSystem.DHSModel.ShowFullProofMessage += OnShowFullProofMessage;

            SetupGridProductInfo();
            SetupGridTestListCh1();
            SetupGridTestListCh2();
            SetupTestCount();
            SetupConnectorCount();

            UpdateTestState(CH1, TestStates.Ready);
            UpdateTestState(CH2, TestStates.Ready);

            // 기본 사용자는 작업자
            ChangeUserMode(GSystem.AdminMode);
        }

        private void FormDHSTesterXL_FormClosing(object sender, FormClosingEventArgs e)
        {
            string caption = "프로그램 종료";
            string message = "프로그램을 종료하시겠습니까?";
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            timerUpdate.Stop();

            // PLC 연결 해제
            GSystem.MiPLC.Disconnect();

            GSystem.DedicatedCTRL.StopAsync();
            GSystem.DisconnectDedicatedCTRL();

            if (GSystem.ProductSettings.CommSettings.CommType == "CAN" || GSystem.ProductSettings.CommSettings.CommType == "CAN FD")
            {
                for (int ch = 0; ch < GSystem.ChannelCount; ch++)
                {
                    GSystem.DHSModel.ClosePort(ch);
                }
                CanXL.UnloadDriver();
            }
        }

        private void FormDHSTesterXL_FormClosed(object sender, FormClosedEventArgs e)
        {
            GSystem.Logger.Info($"-------------------------------------------------------");
            GSystem.Logger.Info($"{CanXL.AppName} V1.00 terminated.");
            GSystem.Logger.Info($"-------------------------------------------------------");
            GSystem.TraceMessage($"-------------------------------------------------------");
            GSystem.TraceMessage($"{CanXL.AppName} V1.00 terminated.");
            GSystem.TraceMessage($"-------------------------------------------------------");
        }

        private void MainFormMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (InvokeRequired)
            {
                Invoke(new MessageBoxDelegate(MainFormMessageBox), message, caption, buttons, icon);
                return;
            }
            MessageBox.Show(message, caption, buttons, icon);
        }

        private void SetupGridProductInfo()
        {
            List<string> productNoList = GSystem.ProductSettings.GetProductList(GSystem.SystemData.GeneralSettings.ProductFolder);

            DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
            cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            foreach (string productNo in productNoList)
            {
                cell.Items.Add(productNo);
            }
            cell.Value = GSystem.ProductSettings.ProductInfo.PartNo;

            gridProductInfo.Rows.Clear();
            gridProductInfo.Rows.Add();
            gridProductInfo.Rows.Add();
            gridProductInfo.Rows.Add();
            gridProductInfo.Rows.Add();
            gridProductInfo.Rows.Add();

            int rowIndex = 0;
            gridProductInfo.Rows[rowIndex++].Cells[0].Value = "품번";
            gridProductInfo.Rows[rowIndex++].Cells[0].Value = "품명";
            gridProductInfo.Rows[rowIndex++].Cells[0].Value = "구분";
            gridProductInfo.Rows[rowIndex++].Cells[0].Value = "차종";
            gridProductInfo.Rows[rowIndex++].Cells[0].Value = "ALC 번호";

            rowIndex = 0;
            gridProductInfo.Rows[rowIndex++].Cells[1] = cell;
            gridProductInfo.Rows[rowIndex++].Cells[1].Value = GSystem.ProductSettings.ProductInfo.PartName;
            gridProductInfo.Rows[rowIndex++].Cells[1].Value = GSystem.ProductSettings.ProductInfo.TypeName;
            gridProductInfo.Rows[rowIndex++].Cells[1].Value = GSystem.ProductSettings.ProductInfo.CarType;
            gridProductInfo.Rows[rowIndex++].Cells[1].Value = GSystem.ProductSettings.ProductInfo.AlcNo;

            gridProductInfo.CurrentCell = null;
        }

        private void UpdateGridProductInfo()
        {
            int rowIndex = 0;
            gridProductInfo.Rows[rowIndex++].Cells[1].Value = GSystem.ProductSettings.ProductInfo.PartName;
            gridProductInfo.Rows[rowIndex++].Cells[1].Value = GSystem.ProductSettings.ProductInfo.TypeName;
            gridProductInfo.Rows[rowIndex++].Cells[1].Value = GSystem.ProductSettings.ProductInfo.CarType;
            gridProductInfo.Rows[rowIndex++].Cells[1].Value = GSystem.ProductSettings.ProductInfo.AlcNo;
        }

        private void gridProductInfo_SelectionChanged(object sender, EventArgs e)
        {
            gridProductInfo.ClearSelection();
        }

        private void gridProductInfo_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gridProductInfo.IsCurrentCellDirty)
            {
                gridProductInfo.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void gridProductInfo_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0 && e.ColumnIndex == 1)
            {
                object selectedValue = gridProductInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (selectedValue != null)
                {
                    // 품번 파일을 읽은 후 화면을 업데이트 한다.
                    string selectProductFileName = selectedValue.ToString() + GSystem.JSON_EXT;
                    ProductConfig productSettings = ProductConfig.GetInstance();
                    productSettings.Load(selectProductFileName, GSystem.SystemData.GeneralSettings.ProductFolder);
                    GSystem.SystemData.ProductSettings.LastProductNo = selectedValue.ToString();
                    GSystem.SystemDataSave();

                    if (productSettings.CommSettings.CommType == "CAN" || productSettings.CommSettings.CommType == "CAN FD")
                    {
                        // Close
                        GSystem.DHSModel.TestStateChanged -= OnTestStateChanged;
                        GSystem.DHSModel.TestStepProgressChanged -= OnTestStepProgressChanged;
                        GSystem.DHSModel.RxsWinDataChanged -= OnRxsWinDataChanged;
                        GSystem.DHSModel.TouchXcpDataChanged -= OnUpdateTouchXcpData;
                        GSystem.DHSModel.CancelXcpDataChanged -= OnUpdateCancelXcpData;
                        GSystem.DHSModel.ShowFullProofMessage -= OnShowFullProofMessage;
                        for (int ch = 0; ch < ChannelCount; ch++)
                        {
                            GSystem.DHSModel.ClosePort(ch);
                        }
                        // 
                        if (productSettings.CommSettings.CommType == "CAN")
                        {
                            GSystem.DHSModel = new PNFCTouch();
                        }
                        else if (productSettings.CommSettings.CommType == "CAN FD")
                        {
                            GSystem.DHSModel = new PNFCTouchFD();
                        }
                        // Open
                        for (int ch = 0; ch < ChannelCount; ch++)
                        {
                            GSystem.DHSModel.OpenPort(ch);
                        }
                        GSystem.DHSModel.TestItemsList = GSystem.ProductSettings.GetEnableTestItemsList();
                        GSystem.DHSModel.TestStateChanged += OnTestStateChanged;
                        GSystem.DHSModel.TestStepProgressChanged += OnTestStepProgressChanged;
                        GSystem.DHSModel.RxsWinDataChanged += OnRxsWinDataChanged;
                        GSystem.DHSModel.TouchXcpDataChanged += OnUpdateTouchXcpData;
                        GSystem.DHSModel.CancelXcpDataChanged += OnUpdateCancelXcpData;
                        GSystem.DHSModel.ShowFullProofMessage += OnShowFullProofMessage;
                    }

                    // 
                    GSystem.DHSModel.TestItemsList = GSystem.ProductSettings.GetEnableTestItemsList();
                    SetupGridProductInfo();
                    SetupGridTestListCh1();
                    SetupGridTestListCh2();
                }
            }
        }

        private void SetupGridTestListCh1()
        {
            // TODO: 측정이 true인 항목만 표시해야 한다.
            if (GSystem.DHSModel == null) return;
            if (GSystem.DHSModel.TestItemsList == null) return;
            int rowIndex = 0;
            gridTestListCh1.Rows.Clear();
            foreach (var testItem in GSystem.DHSModel.TestItemsList)
            {
                gridTestListCh1.Rows.Add();
                gridTestListCh1[0, rowIndex].Value = (rowIndex + 1).ToString();
                gridTestListCh1[1, rowIndex].Value = testItem.Use;
                if (testItem.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SerialNumber] || testItem.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Manufacture])
                {
                    // Name
                    gridTestListCh1[2, rowIndex].Value = $"{testItem.Name} ({GDefines.TEST_ITEM_OPTION[testItem.Option]})";
                }
                else
                {
                    // Name
                    gridTestListCh1[2, rowIndex].Value = testItem.Name;
                }
                switch (testItem.DataType)
                {
                    case 0:
                        gridTestListCh1[3, rowIndex].Value = testItem.MinValue.ToString("F1");
                        gridTestListCh1[4, rowIndex].Value = testItem.MaxValue.ToString("F1");
                        break;
                    case 1:
                        gridTestListCh1[3, rowIndex].Value = testItem.MinValue.ToString();
                        gridTestListCh1[4, rowIndex].Value = testItem.MaxValue.ToString();
                        break;
                    case 2:
                        gridTestListCh1[3, rowIndex].Value = testItem.MinString;
                        gridTestListCh1[4, rowIndex].Value = testItem.MaxString;
                        break;
                    default: break;
                }
                rowIndex++;
            }
            gridTestListCh1.CurrentCell = null;
        }

        private void UpdateGridTestListCh1()
        {
            if (GSystem.DHSModel == null) return;
            if (GSystem.DHSModel.TestItemsList == null) return;
            int rowIndex = 0;
            gridTestListCh1.Rows.Clear();
            foreach (var testItem in GSystem.DHSModel.TestItemsList)
            {
                TestSpec testSpec = GSystem.ProductSettings.GetTestItemSpec((TestItems)rowIndex);
                gridTestListCh1[0, rowIndex].Value = (rowIndex + 1).ToString();
                gridTestListCh1[1, rowIndex].Value = testSpec.Use;
                if (testItem.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SerialNumber] || testItem.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Manufacture])
                {
                    // Name
                    gridTestListCh1[2, rowIndex].Value = $"{testSpec.Name} ({GDefines.TEST_ITEM_OPTION[testSpec.Option]})";
                }
                else
                {
                    // Name
                    gridTestListCh1[2, rowIndex].Value = testSpec.Name;
                }
                switch (testSpec.DataType)
                {
                    case 0:
                        gridTestListCh1[3, rowIndex].Value = testSpec.MinValue.ToString("F1");
                        gridTestListCh1[4, rowIndex].Value = testSpec.MaxValue.ToString("F1");
                        break;
                    case 1:
                        gridTestListCh1[3, rowIndex].Value = testSpec.MinValue.ToString();
                        gridTestListCh1[4, rowIndex].Value = testSpec.MaxValue.ToString();
                        break;
                    case 2:
                        gridTestListCh1[3, rowIndex].Value = testSpec.MinString;
                        gridTestListCh1[4, rowIndex].Value = testSpec.MaxString;
                        break;
                    default: break;
                }
                rowIndex++;
            }
            gridTestListCh1.CurrentCell = null;
        }

        private void GridTestListCh1_SelectionChanged(object sender, EventArgs e)
        {
            gridTestListCh1.ClearSelection();
        }

        private void SetupGridTestListCh2()
        {
            if (GSystem.DHSModel == null) return;
            if (GSystem.DHSModel.TestItemsList == null) return;
            int rowIndex = 0;
            gridTestListCh2.Rows.Clear();
            foreach (var testItem in GSystem.DHSModel.TestItemsList)
            {
                gridTestListCh2.Rows.Add();
                gridTestListCh2[0, rowIndex].Value = (rowIndex + 1).ToString();
                gridTestListCh2[1, rowIndex].Value = testItem.Use;
                if (testItem.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SerialNumber] || testItem.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Manufacture])
                {
                    // Name
                    gridTestListCh2[2, rowIndex].Value = $"{testItem.Name} ({GDefines.TEST_ITEM_OPTION[testItem.Option]})";
                }
                else
                {
                    // Name
                    gridTestListCh2[2, rowIndex].Value = testItem.Name;
                }
                switch (testItem.DataType)
                {
                    case 0:
                        gridTestListCh2[3, rowIndex].Value = testItem.MinValue.ToString("F1");
                        gridTestListCh2[4, rowIndex].Value = testItem.MaxValue.ToString("F1");
                        break;
                    case 1:
                        gridTestListCh2[3, rowIndex].Value = testItem.MinValue.ToString();
                        gridTestListCh2[4, rowIndex].Value = testItem.MaxValue.ToString();
                        break;
                    case 2:
                        gridTestListCh2[3, rowIndex].Value = testItem.MinString;
                        gridTestListCh2[4, rowIndex].Value = testItem.MaxString;
                        break;
                    default: break;
                }
                rowIndex++;
            }
            gridTestListCh2.CurrentCell = null;
        }

        private void UpdateGridTestListCh2()
        {
            if (GSystem.DHSModel == null) return;
            if (GSystem.DHSModel.TestItemsList == null) return;
            int rowIndex = 0;
            gridTestListCh2.Rows.Clear();
            foreach (var testItem in GSystem.DHSModel.TestItemsList)
            {
                TestSpec testSpec = GSystem.ProductSettings.GetTestItemSpec((TestItems)rowIndex);
                gridTestListCh2[0, rowIndex].Value = (rowIndex + 1).ToString();
                gridTestListCh2[1, rowIndex].Value = testSpec.Use;
                if (testItem.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SerialNumber] || testItem.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Manufacture])
                {
                    // Name
                    gridTestListCh2[2, rowIndex].Value = $"{testSpec.Name} ({GDefines.TEST_ITEM_OPTION[testSpec.Option]})";
                }
                else
                {
                    // Name
                    gridTestListCh2[2, rowIndex].Value = testSpec.Name;
                }
                switch (testSpec.DataType)
                {
                    case 0:
                        gridTestListCh2[3, rowIndex].Value = testSpec.MinValue.ToString("F1");
                        gridTestListCh2[4, rowIndex].Value = testSpec.MaxValue.ToString("F1");
                        break;
                    case 1:
                        gridTestListCh2[3, rowIndex].Value = testSpec.MinValue.ToString();
                        gridTestListCh2[4, rowIndex].Value = testSpec.MaxValue.ToString();
                        break;
                    case 2:
                        gridTestListCh2[3, rowIndex].Value = testSpec.MinString;
                        gridTestListCh2[4, rowIndex].Value = testSpec.MaxString;
                        break;
                    default: break;
                }
                rowIndex++;
            }
            gridTestListCh2.CurrentCell = null;
        }

        private void GridTestListCh2_SelectionChanged(object sender, EventArgs e)
        {
            gridTestListCh2.ClearSelection();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
            UpdateTestCount();
            UpdateConnectorCount();
            UpdateConnectState();

            if (GSystem.DedicatedCTRL.IsOpen)
            {
                if (statusDCtrl.ForeColor != Color.Green)
                    statusDCtrl.ForeColor = Color.Green;

                if (GSystem.DedicatedCTRL.GetLockSignalCh1())
                {
                    if (buttonLockCh1.ForeColor != Color.LimeGreen)
                    {
                        GSystem.SetButtonForeColor(buttonLockCh1, Color.LimeGreen);
                        GSystem.DedicatedCTRL.LockLampStateCh1(true);
                    }
                }
                else
                {
                    if (buttonLockCh1.ForeColor != Color.DimGray)
                    {
                        GSystem.SetButtonForeColor(buttonLockCh1, Color.DimGray);
                        GSystem.DedicatedCTRL.LockLampStateCh1(false);
                    }
                }

                if (GSystem.DedicatedCTRL.GetLockSignalCh2())
                {
                    if (buttonLockCh2.ForeColor != Color.LimeGreen)
                    {
                        GSystem.SetButtonForeColor(buttonLockCh2, Color.LimeGreen);
                        GSystem.DedicatedCTRL.LockLampStateCh2(true);
                    }
                }
                else
                {
                    if (buttonLockCh2.ForeColor != Color.DimGray)
                    {
                        GSystem.SetButtonForeColor(buttonLockCh2, Color.DimGray);
                        GSystem.DedicatedCTRL.LockLampStateCh2(false);
                    }
                }

                if (GSystem.TimerTestTime[CH1].IsStart)
                {
                    labelTimeValueCh1.Text = $"{GSystem.TimerTestTime[CH1].GetElapsedSeconds():F1} sec";
                    if (buttonStartCh1.ForeColor != Color.DodgerBlue)
                    {
                        GSystem.SetButtonForeColor(buttonStartCh1, Color.DodgerBlue);
                        GSystem.DedicatedCTRL.SetStartLampStateCh1(true);
                    }
                }
                else
                {
                    if (buttonStartCh1.ForeColor != Color.DimGray)
                    {
                        GSystem.SetButtonForeColor(buttonStartCh1, Color.DimGray);
                        GSystem.DedicatedCTRL.SetStartLampStateCh1(false);
                    }
                }

                if (GSystem.DedicatedCTRL.GetCompleteActiveCurrentCh1() || GSystem.DedicatedCTRL.GetCompleteActiveCurrentCh1())
                {
                    if (buttonPowerCh1.ForeColor != Color.OrangeRed)
                    {
                        GSystem.SetButtonForeColor(buttonPowerCh1, Color.OrangeRed);
                        GSystem.DedicatedCTRL.PowerLampStateCh1(true);
                    }
                }
                else
                {
                    if (buttonPowerCh1.ForeColor != Color.DimGray)
                    {
                        GSystem.SetButtonForeColor(buttonPowerCh1, Color.DimGray);
                        GSystem.DedicatedCTRL.PowerLampStateCh1(false);
                    }
                }

                if (GSystem.TimerTestTime[CH2].IsStart)
                {
                    labelTimeValueCh2.Text = $"{GSystem.TimerTestTime[CH2].GetElapsedSeconds():F1} sec";
                    if (buttonStartCh2.ForeColor != Color.DodgerBlue)
                    {
                        GSystem.SetButtonForeColor(buttonStartCh2, Color.DodgerBlue);
                        GSystem.DedicatedCTRL.SetStartLampStateCh2(true);
                    }
                }
                else
                {
                    if (buttonStartCh2.ForeColor != Color.DimGray)
                    {
                        GSystem.SetButtonForeColor(buttonStartCh2, Color.DimGray);
                        GSystem.DedicatedCTRL.SetStartLampStateCh2(false);
                    }
                }

                if (GSystem.DedicatedCTRL.GetCompleteActiveCurrentCh2() || GSystem.DedicatedCTRL.GetCompleteActiveCurrentCh2())
                {
                    if (buttonPowerCh2.ForeColor != Color.OrangeRed)
                    {
                        GSystem.SetButtonForeColor(buttonPowerCh2, Color.OrangeRed);
                        GSystem.DedicatedCTRL.PowerLampStateCh2(true);
                    }
                }
                else
                {
                    if (buttonPowerCh2.ForeColor != Color.DimGray)
                    {
                        GSystem.SetButtonForeColor(buttonPowerCh2, Color.DimGray);
                        GSystem.DedicatedCTRL.PowerLampStateCh2(false);
                    }
                }
            }
            else
            {
                if (statusDCtrl.ForeColor != Color.OrangeRed)
                    statusDCtrl.ForeColor = Color.OrangeRed;
                if (buttonLockCh1.ForeColor != Color.DimGray)
                    GSystem.SetButtonForeColor(buttonLockCh1, Color.DimGray);
                if (buttonPowerCh1.ForeColor != Color.DimGray)
                    GSystem.SetButtonForeColor(buttonPowerCh1, Color.DimGray);
                if (buttonStartCh1.ForeColor != Color.DimGray)
                    GSystem.SetButtonForeColor(buttonStartCh1, Color.DimGray);
                if (buttonLockCh2.ForeColor != Color.DimGray)
                    GSystem.SetButtonForeColor(buttonLockCh2, Color.DimGray);
                if (buttonPowerCh2.ForeColor != Color.DimGray)
                    GSystem.SetButtonForeColor(buttonPowerCh2, Color.DimGray);
                if (buttonStartCh2.ForeColor != Color.DimGray)
                    GSystem.SetButtonForeColor(buttonStartCh2, Color.DimGray);
            }

            labelLowCurrentValueCh1.Text = $"{GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo} uA";
            labelHighCurrentValueCh1.Text = $"{GSystem.DedicatedCTRL.Reg_03h_ch1_current_hi} mA";
            labelLowCurrentValueCh2.Text = $"{GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo} uA";
            labelHighCurrentValueCh2.Text = $"{GSystem.DedicatedCTRL.Reg_03h_ch2_current_hi} mA";
        }

        private void UpdateDateTime()
        {
            // 날짜, 시간
            labelDate.Text = DateTime.Now.ToString("yyyy년 M월 d일", System.Globalization.CultureInfo.GetCultureInfo("ko-KR"/*GSystem.SystemData.Language*/));
            labelTime.Text = DateTime.Now.ToString("T", System.Globalization.CultureInfo.GetCultureInfo("ko-KR"/*GSystem.SystemData.Language*/));
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            FormLogin formLogin = new FormLogin();
            formLogin.AdminMode = GSystem.AdminMode;
            if (formLogin.ShowDialog() == DialogResult.OK)
            {
                GSystem.AdminMode = formLogin.AdminMode;
                // 사용자 모드 변경
                ChangeUserMode(GSystem.AdminMode);
            }
        }

        private void buttonProduct_Click(object sender, EventArgs e)
        {
            FormProduct formProduct = new FormProduct();
            formProduct.CurrentProductNo = GSystem.SystemData.ProductSettings.LastProductNo;
            formProduct.ShowDialog();

            try
            {
                // 현재 파일을 수정했을 수 있기 때문에 무조건 재로딩한다.
                CurrentProductNo = formProduct.NewProductNo;
                // 품목 설정 재로딩
                GSystem.ProductSettings.Load(CurrentProductNo + GSystem.JSON_EXT, GSystem.SystemData.GeneralSettings.ProductFolder);
                GSystem.DHSModel.TestItemsList = GSystem.ProductSettings.GetEnableTestItemsList();
                SetupGridProductInfo();
                SetupGridTestListCh1();
                SetupGridTestListCh2();
                SetupTestCount();
                SetupConnectorCount();
                UpdateTestState(CH1, TestStates.Ready);
                UpdateTestState(CH2, TestStates.Ready);
                // 로딩한 품번 저장
                GSystem.SystemData.ProductSettings.LastProductNo = CurrentProductNo;
                GSystem.SystemData.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonFunction_Click(object sender, EventArgs e)
        {
            FormVFlash formVFlash = new FormVFlash();
            formVFlash.ShowDialog();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            FormSettings formSettings = new FormSettings();
            formSettings.Show();
        }

        private void buttonLog_Click(object sender, EventArgs e)
        {
            //FormLogData formLogData = new FormLogData();
            //formLogData.ShowDialog();
        }

        private void SetupConnectorCount()
        {
            // Ch1
            uint maxCount = GSystem.SystemData.ConnectorNFCTouch1Ch1.MaxCount;
            uint warnCount = GSystem.SystemData.ConnectorNFCTouch1Ch1.WarnCount;
            uint useCount = GSystem.SystemData.ConnectorNFCTouch1Ch1.UseCount;
            labelNfcTouch1SetCh1.Text = $"{maxCount:N0} ({warnCount:N0})";
            labelNfcTouch1ValueCh1.Text = useCount.ToString("N0");

            maxCount = GSystem.SystemData.ConnectorNFCTouch2Ch1.MaxCount;
            warnCount = GSystem.SystemData.ConnectorNFCTouch2Ch1.WarnCount;
            useCount = GSystem.SystemData.ConnectorNFCTouch2Ch1.UseCount;
            labelNfcTouch2SetCh1.Text = $"{maxCount:N0} ({warnCount:N0})";
            labelNfcTouch2ValueCh1.Text = useCount.ToString("N0");

            maxCount = GSystem.SystemData.ConnectorTouchOnlyCh1.MaxCount;
            warnCount = GSystem.SystemData.ConnectorTouchOnlyCh1.WarnCount;
            useCount = GSystem.SystemData.ConnectorTouchOnlyCh1.UseCount;
            labelTouchOnlySetCh1.Text = $"{maxCount:N0} ({warnCount:N0})";
            labelTouchOnlyValueCh1.Text = useCount.ToString("N0");

            // Ch2
            maxCount = GSystem.SystemData.ConnectorNFCTouch1Ch2.MaxCount;
            warnCount = GSystem.SystemData.ConnectorNFCTouch1Ch2.WarnCount;
            useCount = GSystem.SystemData.ConnectorNFCTouch1Ch2.UseCount;
            labelNfcTouch1SetCh2.Text = $"{maxCount:N0} ({warnCount:N0})";
            labelNfcTouch1ValueCh2.Text = useCount.ToString("N0");

            maxCount = GSystem.SystemData.ConnectorNFCTouch2Ch2.MaxCount;
            warnCount = GSystem.SystemData.ConnectorNFCTouch2Ch2.WarnCount;
            useCount = GSystem.SystemData.ConnectorNFCTouch2Ch2.UseCount;
            labelNfcTouch2SetCh2.Text = $"{maxCount:N0} ({warnCount:N0})";
            labelNfcTouch2ValueCh2.Text = useCount.ToString("N0");

            maxCount = GSystem.SystemData.ConnectorTouchOnlyCh2.MaxCount;
            warnCount = GSystem.SystemData.ConnectorTouchOnlyCh2.WarnCount;
            useCount = GSystem.SystemData.ConnectorTouchOnlyCh2.UseCount;
            labelTouchOnlySetCh2.Text = $"{maxCount:N0} ({warnCount:N0})";
            labelTouchOnlyValueCh2.Text = useCount.ToString("N0");
        }

        private void UpdateConnectorCount()
        {
            // Ch1
            if (labelNfcTouch1ValueCh1.Text != GSystem.SystemData.ConnectorNFCTouch1Ch1.UseCount.ToString("N0"))
                labelNfcTouch1ValueCh1.Text = GSystem.SystemData.ConnectorNFCTouch1Ch1.UseCount.ToString("N0");

            if (labelNfcTouch2ValueCh1.Text != GSystem.SystemData.ConnectorNFCTouch2Ch1.UseCount.ToString("N0"))
                labelNfcTouch2ValueCh1.Text = GSystem.SystemData.ConnectorNFCTouch2Ch1.UseCount.ToString("N0");

            if (labelTouchOnlyValueCh1.Text != GSystem.SystemData.ConnectorTouchOnlyCh1.UseCount.ToString("N0"))
                labelTouchOnlyValueCh1.Text = GSystem.SystemData.ConnectorTouchOnlyCh1.UseCount.ToString("N0");

            // Ch2
            if (labelNfcTouch1ValueCh2.Text != GSystem.SystemData.ConnectorNFCTouch1Ch2.UseCount.ToString("N0"))
                labelNfcTouch1ValueCh2.Text = GSystem.SystemData.ConnectorNFCTouch1Ch2.UseCount.ToString("N0");

            if (labelNfcTouch2ValueCh2.Text != GSystem.SystemData.ConnectorNFCTouch2Ch2.UseCount.ToString("N0"))
                labelNfcTouch2ValueCh2.Text = GSystem.SystemData.ConnectorNFCTouch2Ch2.UseCount.ToString("N0");

            if (labelTouchOnlyValueCh2.Text != GSystem.SystemData.ConnectorTouchOnlyCh2.UseCount.ToString("N0"))
                labelTouchOnlyValueCh2.Text = GSystem.SystemData.ConnectorTouchOnlyCh2.UseCount.ToString("N0");
        }

        private void UpdateConnectState()
        {
            if (ProductSettings.CommSettings.CommType == "CAN" || ProductSettings.CommSettings.CommType == "CAN FD")
            {
                // CAN 또는 CAN FD 사양일 경우 활성 시킨다.
                if (!statusCanDriver.Enabled) statusCanDriver.Enabled = true;
                if (!statusCanCh1.Enabled) statusCanCh1.Enabled = true;
                if (!statusCanCh2.Enabled) statusCanCh2.Enabled = true;

                if (CanXL.IsLoaded)
                {
                    if (statusCanDriver.ForeColor != Color.Green)
                        statusCanDriver.ForeColor = Color.Green;
                }
                else
                {
                    if (statusCanDriver.ForeColor != Color.OrangeRed)
                        statusCanDriver.ForeColor = Color.OrangeRed;
                }
                if (GSystem.DHSModel != null)
                {
                    if (GSystem.DHSModel.IsOpen(CH1))
                    {
                        if (statusCanCh1.ForeColor != Color.Green)
                            statusCanCh1.ForeColor = Color.Green;
                    }
                    else
                    {
                        if (statusCanCh1.ForeColor != Color.OrangeRed)
                            statusCanCh1.ForeColor = Color.OrangeRed;
                    }
                    if (GSystem.DHSModel.IsOpen(CH2))
                    {
                        if (statusCanCh2.ForeColor != Color.Green)
                            statusCanCh2.ForeColor = Color.Green;
                    }
                    else
                    {
                        if (statusCanCh2.ForeColor != Color.OrangeRed)
                            statusCanCh2.ForeColor = Color.OrangeRed;
                    }
                }
            }
            else
            {
                // CAN 또는 CAN FD 사양이 아닐 경우 비활성 시킨다.
                if (statusCanDriver.Enabled) statusCanDriver.Enabled = false;
                if (statusCanCh1.Enabled) statusCanCh1.Enabled = false;
                if (statusCanCh2.Enabled) statusCanCh2.Enabled = false;
            }

            if (DedicatedCTRL.IsOpen)
            {
                if (statusDCtrl.ForeColor != Color.Green)
                    statusDCtrl.ForeColor = Color.Green;
            }
            else
            {
                if (statusDCtrl.ForeColor != Color.OrangeRed)
                    statusDCtrl.ForeColor = Color.OrangeRed;
            }

            if (GSystem.MiPLC.IsConnect)
            {
                if (statusPLC.ForeColor != Color.Green)
                    statusPLC.ForeColor = Color.Green;
            }
            else
            {
                if (statusPLC.ForeColor != Color.OrangeRed)
                    statusPLC.ForeColor = Color.OrangeRed;
            }
        }

        private void SetupTestCount()
        {
            labelCountValueTotal.Text = GSystem.ProductSettings.TestInfo.TestCountTot.ToString("N0");
            labelOkValueTotal.Text = GSystem.ProductSettings.TestInfo.OkCountTot.ToString("N0");
            labelNgValueTotal.Text = GSystem.ProductSettings.TestInfo.NgCountTot.ToString("N0");
            labelRateValueTotal.Text = GSystem.ProductSettings.TestInfo.NgRateTot.ToString("F01") + " %";
            labelSerialValueTotal.Text = GSystem.ProductSettings.TestInfo.SerialNumTot.ToString("D04");

            labelCountValueCh1.Text = GSystem.ProductSettings.TestInfo.TestCountCh1.ToString("N0");
            labelOkValueCh1.Text = GSystem.ProductSettings.TestInfo.OkCountCh1.ToString("N0");
            labelNgValueCh1.Text = GSystem.ProductSettings.TestInfo.NgCountCh1.ToString("N0");
            labelRateValueCh1.Text = GSystem.ProductSettings.TestInfo.NgRateCh1.ToString("F01") + " %";
            labelSerialValueCh1.Text = GSystem.ProductSettings.TestInfo.SerialNumCh1.ToString("D04");

            labelCountValueCh2.Text = GSystem.ProductSettings.TestInfo.TestCountCh2.ToString("N0");
            labelOkValueCh2.Text = GSystem.ProductSettings.TestInfo.OkCountCh2.ToString("N0");
            labelNgValueCh2.Text = GSystem.ProductSettings.TestInfo.NgCountCh2.ToString("N0");
            labelRateValueCh2.Text = GSystem.ProductSettings.TestInfo.NgRateCh2.ToString("F01") + " %";
            labelSerialValueCh2.Text = GSystem.ProductSettings.TestInfo.SerialNumCh2.ToString("D04");
        }

        private void UpdateTestCount()
        {
            if (labelCountValueTotal.Text != GSystem.ProductSettings.TestInfo.TestCountTot.ToString("N0"))
                labelCountValueTotal.Text = GSystem.ProductSettings.TestInfo.TestCountTot.ToString("N0");
            if (labelOkValueTotal.Text != GSystem.ProductSettings.TestInfo.OkCountTot.ToString("N0"))
                labelOkValueTotal.Text = GSystem.ProductSettings.TestInfo.OkCountTot.ToString("N0");
            if (labelNgValueTotal.Text != GSystem.ProductSettings.TestInfo.NgCountTot.ToString("N0"))
                labelNgValueTotal.Text = GSystem.ProductSettings.TestInfo.NgCountTot.ToString("N0");
            if (labelRateValueTotal.Text != (GSystem.ProductSettings.TestInfo.NgRateTot.ToString("F01") + " %"))
                labelRateValueTotal.Text = GSystem.ProductSettings.TestInfo.NgRateTot.ToString("F01") + " %";
            if (labelSerialValueTotal.Text != GSystem.ProductSettings.TestInfo.SerialNumTot.ToString("D04"))
                labelSerialValueTotal.Text = GSystem.ProductSettings.TestInfo.SerialNumTot.ToString("D04");

            if (labelCountValueCh1.Text != GSystem.ProductSettings.TestInfo.TestCountCh1.ToString("N0"))
                labelCountValueCh1.Text = GSystem.ProductSettings.TestInfo.TestCountCh1.ToString("N0");
            if (labelOkValueCh1.Text != GSystem.ProductSettings.TestInfo.OkCountCh1.ToString("N0"))
                labelOkValueCh1.Text = GSystem.ProductSettings.TestInfo.OkCountCh1.ToString("N0");
            if (labelNgValueCh1.Text != GSystem.ProductSettings.TestInfo.NgCountCh1.ToString("N0"))
                labelNgValueCh1.Text = GSystem.ProductSettings.TestInfo.NgCountCh1.ToString("N0");
            if (labelRateValueCh1.Text != (GSystem.ProductSettings.TestInfo.NgRateCh1.ToString("F01") + " %"))
                labelRateValueCh1.Text = GSystem.ProductSettings.TestInfo.NgRateCh1.ToString("F01") + " %";
            if (labelSerialValueCh1.Text != GSystem.ProductSettings.TestInfo.SerialNumCh1.ToString("D04"))
                labelSerialValueCh1.Text = GSystem.ProductSettings.TestInfo.SerialNumCh1.ToString("D04");

            if (labelCountValueCh2.Text != GSystem.ProductSettings.TestInfo.TestCountCh2.ToString("N0"))
                labelCountValueCh2.Text = GSystem.ProductSettings.TestInfo.TestCountCh2.ToString("N0");
            if (labelOkValueCh2.Text != GSystem.ProductSettings.TestInfo.OkCountCh2.ToString("N0"))
                labelOkValueCh2.Text = GSystem.ProductSettings.TestInfo.OkCountCh2.ToString("N0");
            if (labelNgValueCh2.Text != GSystem.ProductSettings.TestInfo.NgCountCh2.ToString("N0"))
                labelNgValueCh2.Text = GSystem.ProductSettings.TestInfo.NgCountCh2.ToString("N0");
            if (labelRateValueCh2.Text != (GSystem.ProductSettings.TestInfo.NgRateCh2.ToString("F01") + " %"))
                labelRateValueCh2.Text = GSystem.ProductSettings.TestInfo.NgRateCh2.ToString("F01") + " %";
            if (labelSerialValueCh2.Text != GSystem.ProductSettings.TestInfo.SerialNumCh2.ToString("D04"))
                labelSerialValueCh2.Text = GSystem.ProductSettings.TestInfo.SerialNumCh2.ToString("D04");
        }

        private Task TogglePowerCh1()
        {
            if (GSystem.DedicatedCTRL.GetCompleteActiveCurrentCh1())
            {
                // ON -> OFF
                GSystem.DedicatedCTRL.SetCommandActiveCurrentCh1(false);
                GSystem.DedicatedCTRL.SetCommandTestInitCh1(true);
                while (!GSystem.DedicatedCTRL.GetCommandTestInitCh1() || !GSystem.DedicatedCTRL.GetCompleteTestInitCh1())
                {

                }
                GSystem.DedicatedCTRL.SetCommandTestInitCh1(false);
            }
            else
            {
                // OFF -> ON
                GSystem.DedicatedCTRL.SetCommandTestInitCh1(false);
                GSystem.DedicatedCTRL.SetCommandActiveCurrentCh1(true);
            }
            return Task.CompletedTask;
        }

        private Task TogglePowerCh2()
        {
            if (GSystem.DedicatedCTRL.GetCompleteActiveCurrentCh2())
            {
                // ON -> OFF
                GSystem.DedicatedCTRL.SetCommandActiveCurrentCh2(false);
                GSystem.DedicatedCTRL.SetCommandTestInitCh2(true);
                while (!GSystem.DedicatedCTRL.GetCommandTestInitCh2() || !GSystem.DedicatedCTRL.GetCompleteTestInitCh2())
                {

                }
                GSystem.DedicatedCTRL.SetCommandTestInitCh2(false);
            }
            else
            {
                // OFF -> ON
                GSystem.DedicatedCTRL.SetCommandTestInitCh2(false);
                GSystem.DedicatedCTRL.SetCommandActiveCurrentCh2(true);
            }
            return Task.CompletedTask;
        }

        private async void buttonPowerCh1_Click(object sender, EventArgs e)
        {
            await Task.Run(() => TogglePowerCh1());
        }

        private async void buttonPowerCh2_Click(object sender, EventArgs e)
        {
            await Task.Run(() => TogglePowerCh2());
        }

        private void buttonStartCh1_Click(object sender, EventArgs e)
        {
            if (GSystem.DHSModel.GetTestStep(CH1) < NFCTouchTestStep.Prepare)
            {
                string caption = "테스트 시작";
                string message = "CH.1의 테스트를 시작하시겠습니까?";
                if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                // Start...
                StartTest(CH1);
            }
            else
            {
                string caption = "테스트 중지";
                string message = "CH.1의 테스트를 중지하시겠습니까?";
                if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                // Stop
                StopTest(CH1);
            }
        }

        private void buttonStartCh2_Click(object sender, EventArgs e)
        {
            if (GSystem.DHSModel.GetTestStep(CH2) < NFCTouchTestStep.Prepare)
            {
                string caption = "테스트 시작";
                string message = "CH.2의 테스트를 시작하시겠습니까?";
                if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                // Start...
                StartTest(CH2);
            }
            else
            {
                string caption = "테스트 중지";
                string message = "CH.2의 테스트를 중지하시겠습니까?";
                if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                // Stop
                StopTest(CH2);
            }
        }

        private void buttonTestCh1_Click(object sender, EventArgs e)
        {
            FormXcpData formXcpData = new FormXcpData();
            formXcpData.Channel = CH1;
            formXcpData.ShowDialog();

            // FOR_TEST
            //GSystem.DHSModel.Send_NM((int)VCanChannels.Ch1, 0x400);
            //GSystem.DHSModel.Send_XCPConnect((int)VCanChannels.Ch1, 0x330, true);
            //GSystem.DHSModel.Send_XCPConnect((int)VCanChannels.Ch1, 0x330, true);
            //GSystem.DHSModel.Send_PLight((int)VCanChannels.Ch1, 0x11, true, true);

            //FormManualTest formManualTest = new FormManualTest();
            //formManualTest.Channel = (int)VCanChannels.Ch1;
            //formManualTest.ShowDialog();

            //GSystem.DHSModel.Send_NM(CH1, true);

            //GSystem.DHSModel.Send_PLight(CH1, true, true);

            // END_TEST
        }

        private void buttonTestCh2_Click(object sender, EventArgs e)
        {
            FormXcpData formXcpData = new FormXcpData();
            formXcpData.Channel = CH2;
            formXcpData.ShowDialog();

            // FOR_TEST
            //GSystem.DHSModel.Send_ShortUpload((int)VCanChannels.Ch1, 0x330, 0x20003560, 2, true);
            //Task.Run(async () => await GSystem.DHSModel.GetTouchAsync(0));
            //GSystem.DHSModel.Send_PLight((int)VCanChannels.Ch1, 0x11, false, true);

            //GSystem.DHSModel.ClosePort(0);

            //GSystem.DHSModel.Send_PLight(CH1, false, true);

            // END_TEST
        }

        private void StartTest(int channel)
        {
            ResetTestResult(channel);
            GSystem.DHSModel.StartTest(channel);
        }

        private void StopTest(int channel)
        {
            GSystem.DHSModel.CancelTest(channel, true);
        }

        //private void SetPerformStateCh1(PerformState state)
        //{
        //    if (InvokeRequired)
        //    {
        //        Invoke(new SetPerformStateDelegate(SetPerformStateCh1), state);
        //        return;
        //    }

        //    switch (state)
        //    {
        //        case PerformState.Ready:
        //            labelTestStatusCh1.Text = "READY";
        //            labelTestStatusCh1.ForeColor = Color.DimGray;
        //            break;
        //        case PerformState.Performing:
        //            labelTestStatusCh1.Text = "RUNNING";
        //            labelTestStatusCh1.ForeColor = Color.DodgerBlue;
        //            break;
        //        case PerformState.Perform_OK:
        //            labelTestStatusCh1.Text = "PASS";
        //            labelTestStatusCh1.ForeColor = Color.LimeGreen;
        //            break;
        //        case PerformState.Perform_NG:
        //            labelTestStatusCh1.Text = "N.G";
        //            labelTestStatusCh1.ForeColor = Color.OrangeRed;
        //            break;
        //        case PerformState.Cancel:
        //            labelTestStatusCh1.Text = "CANCEL";
        //            labelTestStatusCh1.ForeColor = Color.DimGray;
        //            ResetTestResult(CH1);
        //            break;
        //    }
        //}

        //private void SetPerformStateCh2(PerformState state)
        //{
        //    if (InvokeRequired)
        //    {
        //        Invoke(new SetPerformStateDelegate(SetPerformStateCh2), state);
        //        return;
        //    }

        //    switch (state)
        //    {
        //        case PerformState.Ready:
        //            labelTestStatusCh2.Text = "READY";
        //            labelTestStatusCh2.ForeColor = Color.DimGray;
        //            break;
        //        case PerformState.Performing:
        //            labelTestStatusCh2.Text = "RUNNING";
        //            labelTestStatusCh2.ForeColor = Color.DodgerBlue;
        //            break;
        //        case PerformState.Perform_OK:
        //            labelTestStatusCh2.Text = "PASS";
        //            labelTestStatusCh2.ForeColor = Color.LimeGreen;
        //            break;
        //        case PerformState.Perform_NG:
        //            labelTestStatusCh2.Text = "N.G";
        //            labelTestStatusCh2.ForeColor = Color.OrangeRed;
        //            ResetTestResult(CH2);
        //            break;
        //    }
        //}

        private Color GetPerformStateColor(PerformState state)
        {
            switch (state)
            {
                case PerformState.Ready: return Color.White;
                case PerformState.Performing: return Color.DodgerBlue;
                case PerformState.Perform_OK: return Color.PaleGreen;
                case PerformState.Perform_NG: return Color.OrangeRed;
                default:
                    return Color.White;
            }
        }

        private void ResetTestResult(int channel)
        {
            // 전체 초기화
            if (channel == CH1)
            {
                for (int i = 0; i < gridTestListCh1.Rows.Count; i++)
                {
                    gridTestListCh1[5, i].Value = string.Empty;
                    gridTestListCh1[5, i].Style.BackColor = GetPerformStateColor(PerformState.Ready);
                }
                if (textRxswinValueCh1.Text != string.Empty)
                {
                    textRxswinValueCh1.Text = string.Empty;
                }
            }
            else
            {
                for (int i = 0; i < gridTestListCh2.Rows.Count; i++)
                {
                    gridTestListCh2[5, i].Value = string.Empty;
                    gridTestListCh2[5, i].Style.BackColor = GetPerformStateColor(PerformState.Ready);
                }
                if (textRxswinValueCh2.Text != string.Empty)
                {
                    textRxswinValueCh2.Text = string.Empty;
                }
            }
        }

        // 테스트 실행 상테 이벤트 핸들러
        private void OnTestStateChanged(object sender, TestStateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, TestStates>(UpdateTestState), e.Channel, e.State);
            }
            else
            {
                UpdateTestState(e.Channel, e.State);
            }
        }

        private void UpdateTestState(int channel, TestStates state)
        {
            if (channel == CH1)
            {
                switch (state)
                {
                    case TestStates.Ready:
                        labelTestStatusCh1.Text = "READY";
                        labelTestStatusCh1.ForeColor = Color.DimGray;
                        GSystem.DedicatedCTRL.SetStartLampStateCh1(false);
                        break;
                    case TestStates.Running:
                        labelTestStatusCh1.Text = "RUNNING";
                        labelTestStatusCh1.ForeColor = Color.DodgerBlue;
                        GSystem.DedicatedCTRL.SetStartLampStateCh1(true);
                        break;
                    case TestStates.Pass:
                        labelTestStatusCh1.Text = "PASS";
                        labelTestStatusCh1.ForeColor = Color.LimeGreen;
                        GSystem.DedicatedCTRL.SetStartLampStateCh1(false);
                        break;
                    case TestStates.Failed:
                        labelTestStatusCh1.Text = "FAILED";
                        labelTestStatusCh1.ForeColor = Color.OrangeRed;
                        GSystem.DedicatedCTRL.SetStartLampStateCh1(false);
                        break;
                    case TestStates.Cancel:
                        labelTestStatusCh1.Text = "CANCEL";
                        labelTestStatusCh1.ForeColor = Color.LightSalmon;
                        GSystem.DedicatedCTRL.SetStartLampStateCh1(false);
                        ResetTestResult(channel);
                        break;
                }
            }
            else if (channel == CH2)
            {
                switch (state)
                {
                    case TestStates.Ready:
                        labelTestStatusCh2.Text = "READY";
                        labelTestStatusCh2.ForeColor = Color.DimGray;
                        break;
                    case TestStates.Running:
                        labelTestStatusCh2.Text = "RUNNING";
                        labelTestStatusCh2.ForeColor = Color.DodgerBlue;
                        break;
                    case TestStates.Pass:
                        labelTestStatusCh2.Text = "PASS";
                        labelTestStatusCh2.ForeColor = Color.LimeGreen;
                        break;
                    case TestStates.Failed:
                        labelTestStatusCh2.Text = "FAILED";
                        labelTestStatusCh2.ForeColor = Color.OrangeRed;
                        break;
                    case TestStates.Cancel:
                        labelTestStatusCh2.Text = "CANCEL";
                        labelTestStatusCh2.ForeColor = Color.LightSalmon;
                        ResetTestResult(channel);
                        break;
                }
            }
        }

        // 테스트 실행 상테 이벤트 핸들러
        private void OnTestStepProgressChanged(object sender, TestStateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string, string, TestStates>(UpdateTestStepProgressChanged), e.Channel, e.Name, e.Result, e.State);
            }
            else
            {
                UpdateTestStepProgressChanged(e.Channel, e.Name, e.Result, e.State);
            }
        }

        private void UpdateTestStepProgressChanged(int channel, string testName, string result, TestStates state)
        {
            if (GSystem.DHSModel.TestItemsList == null) return;

            foreach (var testItem in GSystem.DHSModel.TestItemsList)
            {
                if (testItem.Name == testName)
                {
                    if (channel == CH1)
                    {
                        for (int rowIndex = 0; rowIndex < gridTestListCh1.RowCount; rowIndex++)
                        {
                            if (gridTestListCh1[2, rowIndex].Value.ToString().IndexOf(testName) >= 0)
                            {
                                gridTestListCh1[5, rowIndex].Value = result;
                                gridTestListCh1[5, rowIndex].Style.BackColor = GetTestStepStateColor(state);
                                break;
                            }
                        }
                    }
                    if (channel == CH2)
                    {
                        for (int rowIndex = 0; rowIndex < gridTestListCh2.RowCount; rowIndex++)
                        {
                            if (gridTestListCh2[2, rowIndex].Value.ToString().IndexOf(testName) >= 0)
                            {
                                gridTestListCh2[5, rowIndex].Value = result;
                                gridTestListCh2[5, rowIndex].Style.BackColor = GetTestStepStateColor(state);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private Color GetTestStepStateColor(TestStates state)
        {
            switch (state)
            {
                case TestStates.Ready: return Color.White;
                case TestStates.Running: return Color.DodgerBlue;
                case TestStates.Pass: return Color.PaleGreen;
                case TestStates.Failed: return Color.OrangeRed;
                default:
                    return Color.White;
            }
        }

        // RXSWIN 데이터 표시 이벤트 핸들러
        private void OnRxsWinDataChanged(object sender, TestStateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string>(UpdateRxsWinDataChanged), e.Channel, e.Result);
            }
            else
            {
                UpdateRxsWinDataChanged(e.Channel, e.Result);
            }
        }
        private void UpdateRxsWinDataChanged(int channel, string rxsWinData)
        {
            if (channel == CH1)
            {
                textRxswinValueCh1.Text = rxsWinData;
            }
            else
            {
                textRxswinValueCh2.Text = rxsWinData;
            }
            if (rxsWinData != null)
            {
                if (rxsWinData.Length > 0)
                    GSystem.TraceMessage($"RXSWin size: {rxsWinData.Length}");
            }
        }

        // Lock State 표시 이벤트 핸들러
        private void OnLockStateChanged(object sender, LockStateEventArgs e)
        {
            //if (this.InvokeRequired)
            //{
            //    this.Invoke(new Action<int, int>(UpdateLockStateChanged), e.Channel, e.LockState);
            //}
            //else
            //{
            //    UpdateLockStateChanged(e.Channel, e.LockState);
            //}
        }
        private void UpdateLockStateChanged(int channel, int lockState)
        {
            if (channel == CH1)
            {
                if (lockState > 0)
                {
                    if (buttonLockCh1.ForeColor != Color.LimeGreen)
                    {
                        GSystem.SetButtonForeColor(buttonLockCh1, Color.LimeGreen);
                        GSystem.DedicatedCTRL.LockLampStateCh1(true);
                    }
                }
                else
                {
                    if (buttonLockCh1.ForeColor != Color.DimGray)
                    {
                        GSystem.SetButtonForeColor(buttonLockCh1, Color.DimGray);
                        GSystem.DedicatedCTRL.LockLampStateCh1(false);
                    }
                }
            }
            else
            {
                if (lockState > 0)
                {
                    if (buttonLockCh2.ForeColor != Color.LimeGreen)
                    {
                        GSystem.SetButtonForeColor(buttonLockCh2, Color.LimeGreen);
                        GSystem.DedicatedCTRL.LockLampStateCh2(true);
                    }
                }
                else
                {
                    if (buttonLockCh2.ForeColor != Color.DimGray)
                    {
                        GSystem.SetButtonForeColor(buttonLockCh2, Color.DimGray);
                        GSystem.DedicatedCTRL.LockLampStateCh2(false);
                    }
                }
            }
        }

        private void OnUpdateTouchXcpData(object sender, EventArgs e)
        {
            XcpDataEventArgs xcpTouchCancelArgs = e as XcpDataEventArgs;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, int, int, int, float, int, int>(UpdateTouchXcpData), 
                    xcpTouchCancelArgs.Channel, 
                    xcpTouchCancelArgs.TouchFastMutual, 
                    xcpTouchCancelArgs.TouchFastSelf, 
                    xcpTouchCancelArgs.TouchSlowSelf, 
                    xcpTouchCancelArgs.TouchComboRate, 
                    xcpTouchCancelArgs.TouchState, 
                    xcpTouchCancelArgs.IntervalTime);
            }
            else
            {
                UpdateTouchXcpData(xcpTouchCancelArgs.Channel, 
                    xcpTouchCancelArgs.TouchFastMutual, 
                    xcpTouchCancelArgs.TouchFastSelf, 
                    xcpTouchCancelArgs.TouchSlowSelf, 
                    xcpTouchCancelArgs.TouchComboRate, 
                    xcpTouchCancelArgs.TouchState, 
                    xcpTouchCancelArgs.IntervalTime);
            }
        }
        private void OnUpdateCancelXcpData(object sender, EventArgs e)
        {
            XcpDataEventArgs xcpTouchCancelArgs = e as XcpDataEventArgs;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, int, int, int, int>(UpdateCancelXcpData), xcpTouchCancelArgs.Channel, xcpTouchCancelArgs.CancelFastSelf, xcpTouchCancelArgs.CancelSlowSelf, xcpTouchCancelArgs.CancelState, xcpTouchCancelArgs.IntervalTime);
            }
            else
            {
                UpdateCancelXcpData(xcpTouchCancelArgs.Channel, xcpTouchCancelArgs.CancelFastSelf, xcpTouchCancelArgs.CancelSlowSelf, xcpTouchCancelArgs.CancelState, xcpTouchCancelArgs.IntervalTime);
            }
        }

        private void UpdateTouchXcpData(int channel, int touchFastMutual, int touchFastSelf, int touchSlowSelf, float touchComboRate, int touchState, int intervalTime)
        {
            // idle 평균
            if (!isTouchFastMutualIdleAverage[channel])
            {
                TouchFastMutualList[channel].Add(touchFastMutual);
                if (TouchFastMutualList[channel].Count > MaxAverageCount)
                {
                    TouchFastMutualList[channel].RemoveAt(0);
                    idleTouchFastMutualAvg[channel] = (int)TouchFastMutualList[channel].Average();
                    isTouchFastMutualIdleAverage[channel] = true;
                }
            }

            if (!isTouchFastSelfIdleAverage[channel])
            {
                TouchFastSelfList[channel].Add(touchFastSelf);
                if (TouchFastSelfList[channel].Count > MaxAverageCount)
                {
                    TouchFastSelfList[channel].RemoveAt(0);
                    idleTouchFastSelfAvg[channel] = (int)TouchFastSelfList[channel].Average();
                    isTouchFastSelfIdleAverage[channel] = true;
                }
            }

            // delta 계산
            deltaTouchFastMutual[channel] = touchFastMutual - idleTouchFastMutualAvg[channel];
            deltaTouchFastSelf[channel] = touchFastSelf - idleTouchFastSelfAvg[channel];

            // THD 비교
            if (!isTouchFirstExecute[channel])
            {
                if (!isTouchFastMutualComplete[channel])
                {
                    if (deltaTouchFastMutual[channel] > thdTouchFastMutual[channel])
                    {
                        if (++judgeCountTouchFastMutual[channel] > MaxJudgeCount)
                        {
                            // OK
                            isTouchFastMutualComplete[channel] = true;
                        }
                    }
                    else
                    {
                        // reset
                        judgeCountTouchFastMutual[channel] = 0;
                    }
                }
                if (!isTouchFastSelfComplete[channel])
                {
                    if (deltaTouchFastSelf[channel] > thdTouchFastSelf[channel])
                    {
                        if (++judgeCountTouchFastSelf[channel] > MaxJudgeCount)
                        {
                            // OK
                            isTouchFastSelfComplete[channel] = true;
                        }
                    }
                    else
                    {
                        // reset
                        judgeCountTouchFastSelf[channel] = 0;
                    }
                }
            }

            if (channel == CH1)
            {
                labelTouchFastMutualValueCh1.Text = touchFastMutual.ToString();
                labelTouchFastSelfValueCh1.Text = touchFastSelf.ToString();
                labelTouchStateValueCh1.Text = $"{intervalTime} ms";
            }
            else
            {
                labelTouchFastMutualValueCh2.Text = touchFastMutual.ToString();
                labelTouchFastSelfValueCh2.Text = touchFastSelf.ToString();
                labelTouchStateValueCh2.Text = $"{intervalTime} ms";
            }
        }
        private void UpdateCancelXcpData(int channel, int cancelFastSelf, int cancelSlowSelf, int cancelState, int intervalTime)
        {
            // idle 평균
            if (!isCancelFastSelfIdleAverage[channel])
            {
                CancelFastSelfList[channel].Add(cancelFastSelf);
                if (CancelFastSelfList[channel].Count > MaxAverageCount)
                {
                    CancelFastSelfList[channel].RemoveAt(0);
                    idleCancelFastSelfAvg[channel] = (int)CancelFastSelfList[channel].Average();
                    isCancelFastSelfIdleAverage[channel] = true;
                }
            }

            if (!isCancelSlowSelfIdleAverage[channel])
            {
                CancelSlowSelfList[channel].Add(cancelSlowSelf);
                if (CancelSlowSelfList[channel].Count > MaxAverageCount)
                {
                    CancelSlowSelfList[channel].RemoveAt(0);
                    idleCancelSlowSelfAvg[channel] = (int)CancelSlowSelfList[channel].Average();
                    isCancelSlowSelfIdleAverage[channel] = true;
                }
            }

            // delta 계산
            deltaCancelFastSelf[channel] = cancelFastSelf - idleCancelFastSelfAvg[channel];
            deltaCancelSlowSelf[channel] = cancelSlowSelf - idleCancelSlowSelfAvg[channel];

            // THD 비교
            if (!isCancelFirstExecute[channel])
            {
                if (!isCancelFastSelfComplete[channel])
                {
                    if (deltaCancelFastSelf[channel] > thdCancelFastSelf[channel])
                    {
                        if (++judgeCountCancelFastSelf[channel] > MaxJudgeCount)
                        {
                            // OK
                            isCancelFastSelfComplete[channel] = true;
                            //GSystem.DHSModel.CancelStepExit = true;
                        }
                    }
                    else
                    {
                        // reset
                        judgeCountCancelFastSelf[channel] = 0;
                    }
                }
                if (!isCancelSlowSelfComplete[channel])
                {
                    if (deltaCancelSlowSelf[channel] > thdCancelSlowSelf[channel])
                    {
                        if (++judgeCountCancelSlowSelf[channel] > MaxJudgeCount)
                        {
                            // OK
                            isCancelSlowSelfComplete[channel] = true;
                        }
                    }
                    else
                    {
                        // reset
                        judgeCountCancelSlowSelf[channel] = 0;
                    }
                }
            }

            if (channel == CH1)
            {
                labelCancelFastSelfValueCh1.Text = cancelFastSelf.ToString();
                labelCancelSlowSelfValueCh1.Text = cancelSlowSelf.ToString();
                labelCancelStateValueCh1.Text = $"{intervalTime} ms";
            }
            else
            {
                labelCancelFastSelfValueCh2.Text = cancelFastSelf.ToString();
                labelCancelSlowSelfValueCh2.Text = cancelSlowSelf.ToString();
                labelCancelStateValueCh2.Text = $"{intervalTime} ms";
            }
        }

        private void OnShowFullProofMessage(object sender, EventArgs e)
        {
            FullProofEventArgs eventArgs = e as FullProofEventArgs;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string, bool>(UpdateFullProofMessage), eventArgs.Channel, eventArgs.Message, eventArgs.Enabled);
            }
            else
            {
                UpdateFullProofMessage(eventArgs.Channel, eventArgs.Message, eventArgs.Enabled);
            }
        }
        private void UpdateFullProofMessage(int channel, string message, bool enabled)
        {
            if (channel == CH1)
            {
                if (enabled)
                {
                    // 메시지 표시
                    labelFullProofMessageCh1.Text = message;
                    // TODO: 색상 변경, 점멸 시작
                    labelFullProofMessageCh1.BackColor = Color.Gold;
                    timerBlinkCh1.Interval = 500;
                    timerBlinkCh1.Start();
                }
                else
                {
                    // 메시지 닫기
                    labelFullProofMessageCh1.Text = message;
                    // TODO: 색상 원복, 점멸 중지
                    labelFullProofMessageCh1.BackColor = SystemColors.Control;
                    labelFullProofMessageCh1.ForeColor = Color.Black;
                    timerBlinkCh1.Stop();
                }
            }
            else
            {
                if (enabled)
                {
                    // 메시지 표시
                    labelFullProofMessageCh2.Text = message;
                    // TODO: 색상 변경, 점멸 시작
                    labelFullProofMessageCh2.BackColor = Color.Gold;
                    timerBlinkCh2.Interval = 500;
                    timerBlinkCh2.Start();
                }
                else
                {
                    // 메시지 닫기
                    labelFullProofMessageCh2.Text = message;
                    // TODO: 색상 원복, 점멸 중지
                    labelFullProofMessageCh2.BackColor = SystemColors.Control;
                    labelFullProofMessageCh2.ForeColor = Color.Black;
                    timerBlinkCh2.Stop();
                }
            }
        }

        private void timerBlinkCh1_Tick(object sender, EventArgs e)
        {
            if (labelFullProofMessageCh1.BackColor == Color.Gray)
            {
                labelFullProofMessageCh1.BackColor = Color.Gold;
                labelFullProofMessageCh1.ForeColor = Color.Black;
            }
            else
            {
                labelFullProofMessageCh1.BackColor = Color.Gray;
                labelFullProofMessageCh1.ForeColor = Color.White;
            }
        }

        private void timerBlinkCh2_Tick(object sender, EventArgs e)
        {
            if (labelFullProofMessageCh2.BackColor == Color.Gray)
            {
                labelFullProofMessageCh2.BackColor = Color.Gold;
                labelFullProofMessageCh2.ForeColor = Color.Black;
            }
            else
            {
                labelFullProofMessageCh2.BackColor = Color.Gray;
                labelFullProofMessageCh2.ForeColor = Color.White;
            }
        }

        private void ChangeUserMode(bool adminMode)
        {
            GSystem.AdminMode = adminMode;
            if (adminMode)
            {
                // 관리자 모드
                buttonProduct.Enabled = true;
                buttonManual.Enabled = true;
                buttonFunction.Enabled = true;
                buttonSettings.Enabled = true;
                buttonLog.Enabled = true;
                labelUserMode.Text = "관리자";
                GSystem.Logger.Info($"사용자 모드 : 관리자");
            }
            else
            {
                // 작업자 모드
                buttonProduct.Enabled = false;
                buttonManual.Enabled = false;
                buttonFunction.Enabled = false;
                buttonSettings.Enabled = false;
                buttonLog.Enabled = false;
                labelUserMode.Text = "작업자";
                GSystem.Logger.Info($"사용자 모드 : 작업자");
            }
        }

        private void timerTest_Tick(object sender, EventArgs e)
        {
            GSystem.DHSModel.Send_NM(CH1);
        }

        private void buttonLockCh1_Click(object sender, EventArgs e)
        {
            if (timerTest.Enabled)
            {
                timerTest.Stop();
            }
            else
            {
                timerTest.Interval = 300;
                timerTest.Start();
            }
        }

        private void checkPLight_CheckedChanged(object sender, EventArgs e)
        {
            GSystem.DHSModel.Send_PLight(CH1, checkPLight.Checked, true);
        }
    }
}

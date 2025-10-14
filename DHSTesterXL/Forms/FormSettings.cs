using MetroFramework;
using MetroFramework.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace DHSTesterXL
{
    public partial class FormSettings : Form
    {
        private bool[] _RModuleRelayState = new bool[8];
        private Button[] _buttonRelay = new Button[8];
        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_LoadAsync(object sender, EventArgs e)
        {
            tabControl1.TabPages.RemoveAt(3);
            AssignRelayModule();
            
            SetupDedicatedCTRL();
            SetupGridDedicatedCTRL();
            //SetupRelayModule();
            SetupGridPLC();

            //if (GSystem.DedicatedCTRL.RelayModule.IsOpen)
            //{
            //    _RModuleRelayState = await GSystem.DedicatedCTRL.RelayModule.GetRelayStateAsync();
            //}

            textProductFolder.Text = GSystem.SystemData.GeneralSettings.ProductFolder;
            textVFlashFolder.Text = GSystem.SystemData.GeneralSettings.VFlashFolder;
            textDataFolderAll.Text = GSystem.SystemData.GeneralSettings.DataFolderAll;
            textDataFolderPass.Text = GSystem.SystemData.GeneralSettings.DataFolderPass;
            textDataFolderBack.Text = GSystem.SystemData.GeneralSettings.DataFolderBack;
            textDataFolderRepeat.Text = GSystem.SystemData.GeneralSettings.DataFolderRepeat;
            checkLogDelete.Checked = GSystem.SystemData.GeneralSettings.LogDelete;
            numericLogExpDate.Value = GSystem.SystemData.GeneralSettings.LogExpDate;
            numericConnChange.Value = GSystem.SystemData.ConnectorNFCTouch1Ch1.MaxCount;
            numericConnNotify.Value = GSystem.SystemData.ConnectorNFCTouch1Ch1.WarnCount;

            comboSensorModel.SelectedIndex = GSystem.DedicatedCTRL.GetRegisterValue((int)EDedicatedCTRL_Registers.Reg002_Ch1_ConnType);
        }

        private void FormSettings_Shown(object sender, EventArgs e)
        {
            timerUpdate.Start();
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerUpdate.Stop();
        }

        private void AssignRelayModule()
        {
            _buttonRelay[0] = buttonRModuleRY1;
            _buttonRelay[1] = buttonRModuleRY2;
            _buttonRelay[2] = buttonRModuleRY3;
            _buttonRelay[3] = buttonRModuleRY4;
            _buttonRelay[4] = buttonRModuleRY5;
            _buttonRelay[5] = buttonRModuleRY6;
            _buttonRelay[6] = buttonRModuleRY7;
            _buttonRelay[7] = buttonRModuleRY8;
        }

        private void ToggleRelayState(int ryIndex)
        {
            //if (GSystem.DedicatedCTRL.RelayModule.IsOpen)
            //{
            //    _RModuleRelayState[ryIndex] = !_RModuleRelayState[ryIndex];
            //    GSystem.DedicatedCTRL.RelayModule.SetRelayStateAsync(ryIndex, _RModuleRelayState[ryIndex]);
            //}
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            EnableDedicatedCTRL();
            UpdateGridDedicatedCTRL();

            EnableRelayModule();
            UpdateRelayModule();

            EnableGridPLC();
            UpdateGridPLC();
        }

        private void SetupDedicatedCTRL()
        {
            // 통신 포트
            string[] portList = System.IO.Ports.SerialPort.GetPortNames();
            if (portList.Length > 0)
            {
                Array.Sort(portList);
                comboDCtrlPortName.Items.Clear();
                comboDCtrlPortName.Items.AddRange(portList);
            }
            comboDCtrlPortName.SelectedItem = GSystem.SystemData.DedicatedCtrlSettings.PortName;

            // 통신 속도
            comboDCtrlBaudRate.Items.Clear();
            foreach (int baud in GDefines.COM_BAUDRATE)
            {
                comboDCtrlBaudRate.Items.Add(baud.ToString());
            }
            comboDCtrlBaudRate.SelectedItem = GSystem.SystemData.DedicatedCtrlSettings.BaudRate.ToString();

            // 패리티 비트
            comboDCtrlParityBit.Items.Clear();
            comboDCtrlParityBit.Items.AddRange(GDefines.COM_PARITY_BIT);
            comboDCtrlParityBit.SelectedItem = GSystem.SystemData.DedicatedCtrlSettings.ParityBit;

            // 데이터 비트
            comboDCtrlDataBit.Items.Clear();
            foreach (int dataBit in GDefines.COM_DATA_BIT)
            {
                comboDCtrlDataBit.Items.Add(dataBit.ToString());
            }
            comboDCtrlDataBit.SelectedItem = GSystem.SystemData.DedicatedCtrlSettings.DataBit.ToString();

            // 정지 비트
            comboDCtrlStopBit.Items.Clear();
            foreach (int stopBit in GDefines.COM_STOP_BIT)
            {
                comboDCtrlStopBit.Items.Add(stopBit.ToString());
            }
            comboDCtrlStopBit.SelectedItem = GSystem.SystemData.DedicatedCtrlSettings.StopBit.ToString();
        }

        private void EnableDedicatedCTRL()
        {
            if (GSystem.DedicatedCTRL != null)
            {
                if (GSystem.DedicatedCTRL.IsOpen)
                {
                    comboDCtrlPortName.Enabled = false;
                    comboDCtrlBaudRate.Enabled = false;
                    comboDCtrlParityBit.Enabled = false;
                    comboDCtrlDataBit.Enabled = false;
                    comboDCtrlStopBit.Enabled = false;
                    buttonDCtrlOpen.Enabled = false;
                }
                else
                {
                    comboDCtrlPortName.Enabled = true;
                    comboDCtrlBaudRate.Enabled = true;
                    comboDCtrlParityBit.Enabled = true;
                    comboDCtrlDataBit.Enabled = true;
                    comboDCtrlStopBit.Enabled = true;
                    buttonDCtrlOpen.Enabled = true;
                }
            }
        }

        private void SetupRelayModule()
        {
            // 통신 포트
            string[] portList = System.IO.Ports.SerialPort.GetPortNames();
            if (portList.Length > 0)
            {
                Array.Sort(portList);
                comboRModulePortName.Items.Clear();
                comboRModulePortName.Items.AddRange(portList);
            }
            comboRModulePortName.SelectedItem = GSystem.SystemData.RelayModuleSettings.PortName;

            // 통신 속도
            comboRModuleBaudRate.Items.Clear();
            foreach (int baud in GDefines.COM_BAUDRATE)
            {
                comboRModuleBaudRate.Items.Add(baud.ToString());
            }
            comboRModuleBaudRate.SelectedItem = GSystem.SystemData.RelayModuleSettings.BaudRate.ToString();

            // 패리티 비트
            comboRModuleParityBit.Items.Clear();
            comboRModuleParityBit.Items.AddRange(GDefines.COM_PARITY_BIT);
            comboRModuleParityBit.SelectedItem = GSystem.SystemData.RelayModuleSettings.ParityBit;

            // 데이터 비트
            comboRModuleDataBit.Items.Clear();
            foreach (int dataBit in GDefines.COM_DATA_BIT)
            {
                comboRModuleDataBit.Items.Add(dataBit.ToString());
            }
            comboRModuleDataBit.SelectedItem = GSystem.SystemData.RelayModuleSettings.DataBit.ToString();

            // 정지 비트
            comboRModuleStopBit.Items.Clear();
            foreach (int stopBit in GDefines.COM_STOP_BIT)
            {
                comboRModuleStopBit.Items.Add(stopBit.ToString());
            }
            comboRModuleStopBit.SelectedItem = GSystem.SystemData.RelayModuleSettings.StopBit.ToString();
        }

        private void EnableRelayModule()
        {
            //if (GSystem.DedicatedCTRL.RelayModule != null)
            //{
            //    if (GSystem.DedicatedCTRL.RelayModule.IsOpen)
            //    {
            //        comboRModulePortName.Enabled = false;
            //        comboRModuleBaudRate.Enabled = false;
            //        comboRModuleParityBit.Enabled = false;
            //        comboRModuleDataBit.Enabled = false;
            //        comboRModuleStopBit.Enabled = false;
            //        buttonRModuleOpen.Enabled = false;
            //    }
            //    else
            //    {
            //        comboRModulePortName.Enabled = true;
            //        comboRModuleBaudRate.Enabled = true;
            //        comboRModuleParityBit.Enabled = true;
            //        comboRModuleDataBit.Enabled = true;
            //        comboRModuleStopBit.Enabled = true;
            //        buttonRModuleOpen.Enabled = true;
            //    }
            //}
        }

        private void UpdateRelayModule()
        {
            //if (GSystem.DedicatedCTRL.RelayModule != null)
            //{
            //    if (GSystem.DedicatedCTRL.RelayModule.IsOpen)
            //    {
            //        for (int i = 0; i < 8; i++)
            //        {
            //            if (_RModuleRelayState[i])
            //            {
            //                if (_buttonRelay[i].BackColor != Color.OrangeRed)
            //                    _buttonRelay[i].BackColor = Color.OrangeRed;
            //                if (_buttonRelay[i].ForeColor != Color.Black)
            //                    _buttonRelay[i].ForeColor = Color.Black;
            //            }
            //            else
            //            {
            //                if (_buttonRelay[i].BackColor != Color.DarkGray)
            //                    _buttonRelay[i].BackColor = Color.DarkGray;
            //                if (_buttonRelay[i].ForeColor != Color.DimGray)
            //                    _buttonRelay[i].ForeColor = Color.DimGray;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < 8; i++)
            //        {
            //            if (_buttonRelay[i].BackColor != Color.DarkGray)
            //                _buttonRelay[i].BackColor = Color.DarkGray;
            //            if (_buttonRelay[i].ForeColor != Color.DimGray)
            //                _buttonRelay[i].ForeColor = Color.DimGray;
            //        }
            //    }
            //}
        }

        private void comboDCtrlPortName_DropDown(object sender, EventArgs e)
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

        private void comboDCtrlPortName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.DedicatedCtrlSettings.PortName = comboBox.SelectedItem.ToString();
                GSystem.TraceMessage($"DedicatedCtrlSettings.PortName = {GSystem.SystemData.DedicatedCtrlSettings.PortName}");
            }
        }

        private void comboDCtrlBaudRate_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.DedicatedCtrlSettings.BaudRate = GDefines.COM_BAUDRATE[comboBox.SelectedIndex];
                GSystem.TraceMessage($"DedicatedCtrlSettings.BaudRate = {GSystem.SystemData.DedicatedCtrlSettings.BaudRate}");
            }
        }

        private void comboDCtrlParityBit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.DedicatedCtrlSettings.ParityBit = comboBox.SelectedItem.ToString();
                GSystem.TraceMessage($"DedicatedCtrlSettings.ParityBIt = {GSystem.SystemData.DedicatedCtrlSettings.ParityBit}");
            }
        }

        private void comboDCtrlDataBit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.DedicatedCtrlSettings.DataBit = GDefines.COM_DATA_BIT[comboBox.SelectedIndex];
                GSystem.TraceMessage($"DedicatedCtrlSettings.DataBit = {GSystem.SystemData.DedicatedCtrlSettings.DataBit}");
            }
        }

        private void comboDCtrlStopBit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.DedicatedCtrlSettings.StopBit = GDefines.COM_STOP_BIT[comboBox.SelectedIndex];
                GSystem.TraceMessage($"DedicatedCtrlSettings.StopBit = {GSystem.SystemData.DedicatedCtrlSettings.StopBit}");
            }
        }

        private void buttonDCtrlOpen_Click(object sender, EventArgs e)
        {
            if (!GSystem.DedicatedCTRL.IsOpen)
            {
                if (GSystem.ConnectDedicatedCTRL())
                {
                    //GSystem.StartPollingDedicatedCTRL();
                    GSystem.DedicatedCTRL.StartAsync();
                }
            }
        }

        private void buttonDCtrlClose_Click(object sender, EventArgs e)
        {
            //GSystem.StopPollingDedicatedCTRL();
            GSystem.DedicatedCTRL.StopAsync();
            GSystem.DisconnectDedicatedCTRL();
        }

        private void comboRModulePortName_DropDown(object sender, EventArgs e)
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

        private void comboRModulePortName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.RelayModuleSettings.PortName = comboBox.SelectedItem.ToString();
                GSystem.TraceMessage($"RelayModule.PortName = {GSystem.SystemData.RelayModuleSettings.PortName}");
            }
        }
        
        private void comboRModuleBaudRate_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.RelayModuleSettings.BaudRate = GDefines.COM_BAUDRATE[comboBox.SelectedIndex];
                GSystem.TraceMessage($"RelayModuleSettings.BaudRate = {GSystem.SystemData.RelayModuleSettings.BaudRate}");
            }
        }

        private void comboRModuleParityBit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.RelayModuleSettings.ParityBit = comboBox.SelectedItem.ToString();
                GSystem.TraceMessage($"RelayModuleSettings.ParityBIt = {GSystem.SystemData.RelayModuleSettings.ParityBit}");
            }
        }

        private void comboRModuleDataBit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.RelayModuleSettings.DataBit = GDefines.COM_DATA_BIT[comboBox.SelectedIndex];
                GSystem.TraceMessage($"RelayModuleSettings.DataBit = {GSystem.SystemData.RelayModuleSettings.DataBit}");
            }
        }

        private void comboRModuleStopBit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                GSystem.SystemData.RelayModuleSettings.StopBit = GDefines.COM_STOP_BIT[comboBox.SelectedIndex];
                GSystem.TraceMessage($"RelayModuleSettings.StopBit = {GSystem.SystemData.RelayModuleSettings.StopBit}");
            }
        }
        private void buttonRModuleOpen_Click(object sender, EventArgs e)
        {
            //if (!GSystem.DedicatedCTRL.RelayModule.IsOpen)
            //{
            //    GSystem.DedicatedCTRL.RelayModule.Open(
            //        GSystem.SystemData.RelayModuleSettings.PortName,
            //        GSystem.SystemData.RelayModuleSettings.BaudRate,
            //        GSystem.SystemData.RelayModuleSettings.ParityBit,
            //        GSystem.SystemData.RelayModuleSettings.DataBit,
            //        GSystem.SystemData.RelayModuleSettings.StopBit
            //        );
            //}
        }

        private void buttonRModuleClose_Click(object sender, EventArgs e)
        {
            //GSystem.DedicatedCTRL.RelayModule.Close();
        }

        private void buttonRModuleRY_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                int ryIndex = Convert.ToInt32(button.Tag.ToString());
                ToggleRelayState(ryIndex);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            GSystem.SystemDataSave();
            string caption = "저장 완료";
            string message = "환경 설정 데이터를 저장하였습니다.";
            MessageBox.Show(this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetupGridDedicatedCTRL()
        {
            for (int i = 0; i < (int)EDedicatedCTRL_Registers.Count; i++)
            {
                gridDCtrlRegisters.Rows.Add();
                gridDCtrlRegisters[0, i].Value = i.ToString();
                gridDCtrlRegisters[1, i].Value = GDefines.DedicatedCTRL_RegisterNames[i];
                gridDCtrlRegisters[2, i].Value = GSystem.DedicatedCTRL.GetRegisterValue(i).ToString();
            }
            gridDCtrlRegisters.CurrentCell = null;
        }

        private void UpdateGridDedicatedCTRL()
        {
            if (GSystem.DedicatedCTRL.IsOpen)
            {
                for (int i = 0; i < (int)EDedicatedCTRL_Registers.Count; i++)
                {
                    if (gridDCtrlRegisters[1, i].Value.ToString() != string.Empty)
                    {
                        if (gridDCtrlRegisters[2, i].Value.ToString() != GSystem.DedicatedCTRL.GetRegisterValue(i).ToString())
                            gridDCtrlRegisters[2, i].Value = GSystem.DedicatedCTRL.GetRegisterValue(i).ToString();
                    }
                }
            }
        }

        private void SetupGridPLC()
        {
            for (int i = 0; i < (int)PLC_ReadRegister.Count; i++)
            {
                int addr = GSystem.SystemData.PLCSettings.StartAddress + i;
                gridReadPLC.Rows.Add();
                gridReadPLC[0, i].Value = $"{GSystem.SystemData.PLCSettings.DeviceCode}{addr}";
                gridReadPLC[1, i].Value = (PLC_ReadRegister)i;
            }
            for (int i = 0; i < (int)PLC_WriteRegister.Count; i++)
            {
                int addr = GSystem.SystemData.PLCSettings.StartAddress + (int)PLC_ReadRegister.Count + i;
                gridWritePLC.Rows.Add();
                gridWritePLC[0, i].Value = $"{GSystem.SystemData.PLCSettings.DeviceCode}{addr}";
                gridWritePLC[1, i].Value = (PLC_WriteRegister)i;
            }
        }

        private void EnableGridPLC()
        {
            buttonPLC_Connect.Enabled = !GSystem.MiPLC.IsConnect;
        }

        private void UpdateGridPLC()
        {
            if (GSystem.MiPLC.IsConnect)
            {
                // Read
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_Status1  ].Value = $"{GSystem.MiPLC.Ch1_R_Status1  :X04}h";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_Status2  ].Value = $"{GSystem.MiPLC.Ch1_R_Status2  :X04}h";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_RecipeNo ].Value = $"{GSystem.MiPLC.Ch1_R_RecipeNo     } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_Reserved3].Value = $"{GSystem.MiPLC.Ch1_R_Reserved3    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_State1   ].Value = $"{GSystem.MiPLC.Ch1_R_State1   :X04}h";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_State2   ].Value = $"{GSystem.MiPLC.Ch1_R_State2   :X04}h";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_ErrorCode].Value = $"{GSystem.MiPLC.Ch1_R_ErrorCode    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_Reserved7].Value = $"{GSystem.MiPLC.Ch1_R_Reserved7    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_Reserved8].Value = $"{GSystem.MiPLC.Ch1_R_Reserved8    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch1_Reserved9].Value = $"{GSystem.MiPLC.Ch1_R_Reserved9    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_Status1  ].Value = $"{GSystem.MiPLC.Ch2_R_Status1  :X04}h";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_Status2  ].Value = $"{GSystem.MiPLC.Ch2_R_Status2  :X04}h";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_RecipeNo ].Value = $"{GSystem.MiPLC.Ch2_R_RecipeNo     } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_Reserved3].Value = $"{GSystem.MiPLC.Ch2_R_Reserved3    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_State1   ].Value = $"{GSystem.MiPLC.Ch2_R_State1   :X04}h";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_State2   ].Value = $"{GSystem.MiPLC.Ch2_R_State2   :X04}h";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_ErrorCode].Value = $"{GSystem.MiPLC.Ch2_R_ErrorCode    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_Reserved7].Value = $"{GSystem.MiPLC.Ch2_R_Reserved7    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_Reserved8].Value = $"{GSystem.MiPLC.Ch2_R_Reserved8    } ";
                gridReadPLC[2, (int)PLC_ReadRegister.Ch2_Reserved9].Value = $"{GSystem.MiPLC.Ch2_R_Reserved9    } ";
                // Write
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Command1 ].Value = $"{GSystem.MiPLC.Ch1_W_Command1 :X04}h";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Command2 ].Value = $"{GSystem.MiPLC.Ch1_W_Command2 :X04}h";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_RecipeNo ].Value = $"{GSystem.MiPLC.Ch1_W_RecipeNo     } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Reserved3].Value = $"{GSystem.MiPLC.Ch1_W_Reserved3    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Reserved4].Value = $"{GSystem.MiPLC.Ch1_W_Reserved4    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Reserved5].Value = $"{GSystem.MiPLC.Ch1_W_Reserved5    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Reserved6].Value = $"{GSystem.MiPLC.Ch1_W_Reserved6    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Reserved7].Value = $"{GSystem.MiPLC.Ch1_W_Reserved7    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Reserved8].Value = $"{GSystem.MiPLC.Ch1_W_Reserved8    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch1_Reserved9].Value = $"{GSystem.MiPLC.Ch1_W_Reserved9    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Command1 ].Value = $"{GSystem.MiPLC.Ch2_W_Command1 :X04}h";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Command2 ].Value = $"{GSystem.MiPLC.Ch2_W_Command2 :X04}h";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_RecipeNo ].Value = $"{GSystem.MiPLC.Ch2_W_RecipeNo     } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Reserved3].Value = $"{GSystem.MiPLC.Ch2_W_Reserved3    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Reserved4].Value = $"{GSystem.MiPLC.Ch2_W_Reserved4    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Reserved5].Value = $"{GSystem.MiPLC.Ch2_W_Reserved5    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Reserved6].Value = $"{GSystem.MiPLC.Ch2_W_Reserved6    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Reserved7].Value = $"{GSystem.MiPLC.Ch2_W_Reserved7    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Reserved8].Value = $"{GSystem.MiPLC.Ch2_W_Reserved8    } ";
                gridWritePLC[2, (int)PLC_WriteRegister.Ch2_Reserved9].Value = $"{GSystem.MiPLC.Ch2_W_Reserved9    } ";

                if (GSystem.MiPLC.GetLoadingComplete(_CH1))
                {

                }
            }
        }


        private async void buttonTestInitCh1_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    GSystem.DedicatedCTRL.SetCommandTestInitCh1(true);
                    while (!GSystem.DedicatedCTRL.GetCommandTestInitCh1())
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.DedicatedCTRL.SetCommandTestInitCh1(false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonShortTestCh1_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    GSystem.DedicatedCTRL.SetCommandShortTestCh1(true);
                    while (!GSystem.DedicatedCTRL.GetCommandShortTestCh1())
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.DedicatedCTRL.SetCommandShortTestCh1(false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonLowPowerCh1_Click(object sender, EventArgs e)
        {
            try
            {
                GSystem.DedicatedCTRL.SetCommandActivePowerOnCh1(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonHighPowerCh1_Click(object sender, EventArgs e)
        {
            try
            {
                GSystem.DedicatedCTRL.SetCommandActivePowerOnCh1(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonTestInitCh2_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    GSystem.DedicatedCTRL.SetCommandTestInitCh2(true);
                    while (!GSystem.DedicatedCTRL.GetCommandTestInitCh2())
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.DedicatedCTRL.SetCommandTestInitCh2(false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonShortTestCh2_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
                {
                    GSystem.DedicatedCTRL.SetCommandShortTestCh2(true);
                    while (!GSystem.DedicatedCTRL.GetCommandShortTestCh2())
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.DedicatedCTRL.SetCommandShortTestCh2(false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonLowPowerCh2_Click(object sender, EventArgs e)
        {
            try
            {
                GSystem.DedicatedCTRL.SetCommandActivePowerOnCh2(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonHighPowerCh2_Click(object sender, EventArgs e)
        {
            try
            {
                GSystem.DedicatedCTRL.SetCommandActivePowerOnCh2(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonStartLampCh1_Click(object sender, EventArgs e)
        {
            GSystem.DedicatedCTRL.SetStartLampStateCh1(!GSystem.DedicatedCTRL.GetStartLampStateCh1());
        }

        private void buttonStartLampCh2_Click(object sender, EventArgs e)
        {
            GSystem.DedicatedCTRL.SetStartLampStateCh2(!GSystem.DedicatedCTRL.GetStartLampStateCh2());
        }

        private void buttonProductFolder_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.Title = "품번 폴더 선택";
            dlg.IsFolderPicker = true;
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                return;
            textProductFolder.Text = dlg.FileName;
            GSystem.SystemData.GeneralSettings.ProductFolder = dlg.FileName;
        }

        private void buttonVFlashFolder_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.Title = "VFlash 폴더 선택";
            dlg.IsFolderPicker = true;
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                return;
            textVFlashFolder.Text = dlg.FileName;
            GSystem.SystemData.GeneralSettings.VFlashFolder = dlg.FileName;
        }

        private void buttonResultFolder_Click(object sender, EventArgs e)
        {

        }

        private void buttonPLC_Connect_Click(object sender, EventArgs e)
        {
            GSystem.MiPLC.Connect();
        }

        private void buttonPLC_Disconnect_Click(object sender, EventArgs e)
        {
            GSystem.MiPLC.Disconnect();
        }

        private async void buttonPLC_SetRecipeCh1_Click(object sender, EventArgs e)
        {
            int recipeNo = Convert.ToUInt16(textPLC_RecipeNoCh1.Text);
            try
            {
                await GSystem.ChangePLCRecipeAsync(recipeNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //int bitIndex = 0;
            //if ((GSystem.MiPLC.Ch1_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_RecipeNo = Convert.ToUInt16(textPLC_RecipeNoCh1.Text);
            //    GSystem.MiPLC.Ch2_W_RecipeNo = Convert.ToUInt16(textPLC_RecipeNoCh1.Text);
            //    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
            //    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
        }

        private void buttonPLC_SetRecipeCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 0;
            //if ((GSystem.MiPLC.Ch2_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_RecipeNo = Convert.ToUInt16(textPLC_RecipeNoCh2.Text);
            //    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
        }

        private async void buttonPLC_LoadYCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 9;
            //if ((GSystem.MiPLC.Ch1_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            
            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetMoveLoadStart(channel, true);
                    while (!GSystem.MiPLC.GetMoveLoadComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetMoveLoadStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_LoadYCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 9;
            //if ((GSystem.MiPLC.Ch2_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetMoveLoadStart(channel, true);
                    while (!GSystem.MiPLC.GetMoveLoadComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetMoveLoadStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_LoadingCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 1;
            //if ((GSystem.MiPLC.Ch1_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetLoadingStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetLoadingStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetLoadingStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetLoadingStart(channel, true);
                    while (!GSystem.MiPLC.GetLoadingComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetLoadingStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_LoadingCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 1;
            //if ((GSystem.MiPLC.Ch2_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetLoadingStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetLoadingStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetLoadingStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetLoadingStart(channel, true);
                    while (!GSystem.MiPLC.GetLoadingComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetLoadingStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_TouchYCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 3;
            //if ((GSystem.MiPLC.Ch1_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetMoveTouchYStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetMoveTouchYStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetMoveTouchYStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetMoveTouchYStart(channel, true);
                    while (!GSystem.MiPLC.GetMoveTouchYComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetMoveTouchYStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_TouchYCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 3;
            //if ((GSystem.MiPLC.Ch2_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetMoveTouchYStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetMoveTouchYStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetMoveTouchYStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(() =>
                {
                    GSystem.MiPLC.SetMoveTouchYStart(channel, true);
                    while (!GSystem.MiPLC.GetMoveTouchYComplete(channel))
                    {
                        Task.Delay(10);
                    }
                    GSystem.MiPLC.SetMoveTouchYStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_TouchZDnCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 1;
            //if ((GSystem.MiPLC.Ch1_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetTouchZDownStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetTouchZDownStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetTouchZDownStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetTouchZDownStart(channel, true);
                    while (!GSystem.MiPLC.GetTouchZDownComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetTouchZDownStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_TouchZDnCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 1;
            //if ((GSystem.MiPLC.Ch2_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetTouchZDownStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetTouchZDownStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetTouchZDownStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(() =>
                {
                    GSystem.MiPLC.SetTouchZDownStart(channel, true);
                    while (!GSystem.MiPLC.GetTouchZDownComplete(channel))
                    {
                        Task.Delay(10);
                    }
                    GSystem.MiPLC.SetTouchZDownStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_TouchZUpCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 2;
            //if ((GSystem.MiPLC.Ch1_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetTouchZUpStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetTouchZUpStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetTouchZUpStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetTouchZUpStart(channel, true);
                    while (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetTouchZUpStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_TouchZUpCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 2;
            //if ((GSystem.MiPLC.Ch2_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetTouchZUpStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetTouchZUpStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetTouchZUpStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetTouchZUpStart(channel, true);
                    while (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetTouchZUpStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_CancelYCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 5;
            //if ((GSystem.MiPLC.Ch1_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetMoveCancelYStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetMoveCancelYStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetMoveCancelYStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetMoveCancelYStart(channel, true);
                    while (!GSystem.MiPLC.GetMoveCancelYComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetMoveCancelYStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_CancelYCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 5;
            //if ((GSystem.MiPLC.Ch2_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetMoveCancelYStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetMoveCancelYStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetMoveCancelYStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetMoveCancelYStart(channel, true);
                    while (!GSystem.MiPLC.GetMoveCancelYComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetMoveCancelYStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_CancelZDnCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 4;
            //if ((GSystem.MiPLC.Ch1_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetCancelZDownStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetCancelZDownStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetCancelZDownStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetCancelZDownStart(channel, true);
                    while (!GSystem.MiPLC.GetCancelZDownComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetCancelZDownStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_CancelZDnCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 4;
            //if ((GSystem.MiPLC.Ch2_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetCancelZDownStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetCancelZDownStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetCancelZDownStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetCancelZDownStart(channel, true);
                    while (!GSystem.MiPLC.GetCancelZDownComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetCancelZDownStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_CancelZUpCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 5;
            //if ((GSystem.MiPLC.Ch1_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetCancelZUpStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetCancelZUpStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetCancelZUpStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetCancelZUpStart(channel, true);
                    while (!GSystem.MiPLC.GetCancelZUpComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetCancelZUpStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_CancelZUpCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 5;
            //if ((GSystem.MiPLC.Ch2_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetCancelZUpStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetCancelZUpStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetCancelZUpStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetCancelZUpStart(channel, true);
                    while (!GSystem.MiPLC.GetCancelZUpComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetCancelZUpStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_NfcYCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 7;
            //if ((GSystem.MiPLC.Ch1_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetMoveNFCYStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetMoveNFCYStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetMoveNFCYStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetMoveNFCYStart(channel, true);
                    while (!GSystem.MiPLC.GetMoveNFCYComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetMoveNFCYStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_NfcYCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 7;
            //if ((GSystem.MiPLC.Ch2_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetMoveNFCYStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetMoveNFCYStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetMoveNFCYStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetMoveNFCYStart(channel, true);
                    while (!GSystem.MiPLC.GetMoveNFCYComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetMoveNFCYStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_NfcZDnCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 7;
            //if ((GSystem.MiPLC.Ch1_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetNFCZDownStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetNFCZDownStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetNFCZDownStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetNFCZDownStart(channel, true);
                    while (!GSystem.MiPLC.GetNFCZDownComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetNFCZDownStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_NfcZDnCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 7;
            //if ((GSystem.MiPLC.Ch2_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetNFCZDownStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetNFCZDownStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetNFCZDownStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetNFCZDownStart(channel, true);
                    while (!GSystem.MiPLC.GetNFCZDownComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetNFCZDownStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_NfcZUpCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 8;
            //if ((GSystem.MiPLC.Ch1_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetNFCZUpStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetNFCZUpStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetNFCZUpStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetNFCZUpStart(channel, true);
                    while (!GSystem.MiPLC.GetNFCZUpComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetNFCZUpStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_NfcZUpCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 8;
            //if ((GSystem.MiPLC.Ch2_W_Command2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetNFCZUpStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetNFCZUpStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetNFCZUpStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetNFCZUpStart(channel, true);
                    while (!GSystem.MiPLC.GetNFCZUpComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetNFCZUpStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_UnloadCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 11;
            //if ((GSystem.MiPLC.Ch1_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetUnloadingStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetUnloadingStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetUnloadingStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetUnloadingStart(channel, true);
                    while (!GSystem.MiPLC.GetUnloadingComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetUnloadingStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_UnloadCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 11;
            //if ((GSystem.MiPLC.Ch2_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetUnloadingStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetUnloadingStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetUnloadingStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetUnloadingStart(channel, true);
                    while (!GSystem.MiPLC.GetUnloadingComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetUnloadingStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_UnclampCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 13;
            //if ((GSystem.MiPLC.Ch1_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetUnclampForeStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetUnclampForeStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetUnclampForeStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetUnclampForeStart(channel, true);
                    while (!GSystem.MiPLC.GetUnclampForeComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetUnclampForeStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_UnclampCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 13;
            //if ((GSystem.MiPLC.Ch2_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetUnclampForeStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetUnclampForeStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetUnclampForeStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetUnclampForeStart(channel, true);
                    while (!GSystem.MiPLC.GetUnclampForeComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetUnclampForeStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_TestStopCh1_Click(object sender, EventArgs e)
        {
            //int bitIndex = 0;
            //if ((GSystem.MiPLC.Ch1_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetUnclampBackStart(_CH1))
            //    await Task.Run(() => GSystem.MiPLC.SetUnclampBackStart(_CH1, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetUnclampBackStart(_CH1, true));

            try
            {
                int channel = _CH1;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetUnclampBackStart(channel, true);
                    while (!GSystem.MiPLC.GetUnclampBackComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetUnclampBackStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonPLC_TestStopCh2_Click(object sender, EventArgs e)
        {
            //int bitIndex = 0;
            //if ((GSystem.MiPLC.Ch2_W_Command1 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}
            //else
            //{
            //    GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            //    await Task.Run(() => GSystem.MiPLC.M1402_Req_Proc());
            //}

            //if (GSystem.MiPLC.GetUnclampBackStart(_CH2))
            //    await Task.Run(() => GSystem.MiPLC.SetUnclampBackStart(_CH2, false));
            //else
            //    await Task.Run(() => GSystem.MiPLC.SetUnclampBackStart(_CH2, true));

            try
            {
                int channel = _CH2;
                await Task.Run(async () =>
                {
                    GSystem.MiPLC.SetUnclampBackStart(channel, true);
                    while (!GSystem.MiPLC.GetUnclampBackComplete(channel))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(200);
                    GSystem.MiPLC.SetUnclampBackStart(channel, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        //-----------------------------------------------------------------------------------------
        public enum DryRunStep
        {
            Standby         ,
            Prepare         ,
            WaitStart       ,
            LoadingStart    ,
            LoadingWait     ,
            MoveTouchStart  ,
            MoveTouchWait   ,
            TouchDownStart  ,
            TouchDownWait   ,
            TouchUpStart    ,
            TouchUpWait     ,
            MoveCancelStart ,
            MoveCancelWait  ,
            CancelDownStart ,
            CancelDownWait  ,
            CancelUpStart   ,
            CancelUpWait    ,
            MoveNFCStart    ,
            MoveNFCWait     ,
            NFCDownStart    ,
            NFCDownWait     ,
            NFCUpStart      ,
            NFCUpWait       ,
            MoveUnloadStart ,
            MoveUnloadWait  ,
            UnloadingStart  ,
            UnloadingWait   ,
            UnclampStart    ,
            UnclampWait     ,
            Count
        }
        DryRunStep _dryRunStep = DryRunStep.Standby;
        bool _autoDryRunExit = false;
        public DryRunStep GetDryRunStep() { return _dryRunStep; }
        public void SetDryRunStep(DryRunStep runStep) { _dryRunStep = runStep; }
        public void NextDryRunStep() { ++_dryRunStep; }

        int _CH1 = 0;
        int _CH2 = 1;
        int _stepInterval = 500;
        bool _AutoMode = false;

        public async Task PLC_AutoDryRunAsyncCh1()
        {
            await Task.Delay(10);
            _autoDryRunExit = false;
            while (!_autoDryRunExit)
            {
                await Task.Delay(10);
                switch (_dryRunStep)
                {
                    case DryRunStep.Standby         : DryRunStep_Standby        (); break;
                    case DryRunStep.Prepare         : DryRunStep_Prepare        (); break;
                    case DryRunStep.WaitStart       : DryRunStep_WaitStart      (); break;
                    case DryRunStep.LoadingStart    : DryRunStep_LoadingStart   (); break;
                    case DryRunStep.LoadingWait     : DryRunStep_LoadingWait    (); break;
                    case DryRunStep.MoveTouchStart  : DryRunStep_MoveTouchStart (); break;
                    case DryRunStep.MoveTouchWait   : DryRunStep_MoveTouchWait  (); break;
                    case DryRunStep.TouchDownStart  : DryRunStep_TouchDownStart (); break;
                    case DryRunStep.TouchDownWait   : DryRunStep_TouchDownWait  (); break;
                    case DryRunStep.TouchUpStart    : DryRunStep_TouchUpStart   (); break;
                    case DryRunStep.TouchUpWait     : DryRunStep_TouchUpWait    (); break;
                    case DryRunStep.MoveCancelStart : DryRunStep_MoveCancelStart(); break;
                    case DryRunStep.MoveCancelWait  : DryRunStep_MoveCancelWait (); break;
                    case DryRunStep.CancelDownStart : DryRunStep_CancelDownStart(); break;
                    case DryRunStep.CancelDownWait  : DryRunStep_CancelDownWait (); break;
                    case DryRunStep.CancelUpStart   : DryRunStep_CancelUpStart  (); break;
                    case DryRunStep.CancelUpWait    : DryRunStep_CancelUpWait   (); break;
                    case DryRunStep.MoveNFCStart    : DryRunStep_MoveNFCStart   (); break;
                    case DryRunStep.MoveNFCWait     : DryRunStep_MoveNFCWait    (); break;
                    case DryRunStep.NFCDownStart    : DryRunStep_NFCDownStart   (); break;
                    case DryRunStep.NFCDownWait     : DryRunStep_NFCDownWait    (); break;
                    case DryRunStep.NFCUpStart      : DryRunStep_NFCUpStart     (); break;
                    case DryRunStep.NFCUpWait       : DryRunStep_NFCUpWait      (); break;
                    case DryRunStep.MoveUnloadStart : DryRunStep_MoveUnloadStart(); break;
                    case DryRunStep.MoveUnloadWait  : DryRunStep_MoveUnloadWait (); break;
                    case DryRunStep.UnloadingStart  : DryRunStep_UnloadingStart (); break;
                    case DryRunStep.UnloadingWait   : DryRunStep_UnloadingWait  (); break;
                    case DryRunStep.UnclampStart    : DryRunStep_UnclampStart   (); break;
                    case DryRunStep.UnclampWait     : DryRunStep_UnclampWait    (); break;
                    default:
                        break;
                }
            }
            return;
        }
        void DryRunStep_Standby()
        {

        }
        void DryRunStep_Prepare()
        {
            NextDryRunStep();
        }
        void DryRunStep_WaitStart()
        {
            if (GSystem.MiPLC.GetAutoTestStart(_CH1))
            {
                // 검사 시작
                NextDryRunStep();
            }
        }
        void DryRunStep_LoadingStart()
        {
            GSystem.TraceMessage($"{_dryRunStep}");
            GSystem.MiPLC.SetLoadingStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_LoadingWait()
        {
            if (!GSystem.MiPLC.GetLoadingComplete(_CH1))
                return;
            GSystem.MiPLC.SetLoadingStart(_CH1, false);
            GSystem.TraceMessage($"{_dryRunStep}");
            if (_AutoMode)
                NextDryRunStep();
            else
            {
                _autoDryRunExit = true;
                SetDryRunStep(DryRunStep.Standby);
            }
            GSystem.TraceMessage($"{_dryRunStep}");
        }
        void DryRunStep_MoveTouchStart()
        {
            GSystem.MiPLC.SetMoveTouchYStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_MoveTouchWait()
        {
            if (!GSystem.MiPLC.GetMoveTouchYComplete(_CH1))
                return;
            GSystem.MiPLC.SetMoveTouchYStart(_CH1, false);
            NextDryRunStep();
        }
        void DryRunStep_TouchDownStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetTouchZDownStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_TouchDownWait()
        {
            if (!GSystem.MiPLC.GetTouchZDownComplete(_CH1))
                return;
            GSystem.MiPLC.SetTouchZDownStart(_CH1, false);
            NextDryRunStep();
        }
        void DryRunStep_TouchUpStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetTouchZUpStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_TouchUpWait()
        {
            if (!GSystem.MiPLC.GetTouchZUpComplete(_CH1))
                return;
            GSystem.MiPLC.SetTouchZUpStart(_CH1, false);
            GSystem.TraceMessage($"{_dryRunStep}");
            if (_AutoMode)
                NextDryRunStep();
            else
            {
                _autoDryRunExit = true;
                SetDryRunStep(DryRunStep.Standby);
            }
            GSystem.TraceMessage($"{_dryRunStep}");
        }
        void DryRunStep_MoveCancelStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetMoveCancelYStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_MoveCancelWait()
        {
            if (!GSystem.MiPLC.GetMoveCancelYComplete(_CH1))
                return;
            GSystem.MiPLC.SetMoveCancelYStart(_CH1, false);
            NextDryRunStep();
        }
        void DryRunStep_CancelDownStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetCancelZDownStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_CancelDownWait()
        {
            if (!GSystem.MiPLC.GetCancelZDownComplete(_CH1))
                return;
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetCancelZDownStart(_CH1, false);
            NextDryRunStep();
        }
        void DryRunStep_CancelUpStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetCancelZUpStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_CancelUpWait()
        {
            if (!GSystem.MiPLC.GetCancelZUpComplete(_CH1))
                return;
            GSystem.MiPLC.SetCancelZUpStart(_CH1, false);
            GSystem.TraceMessage($"{_dryRunStep}");
            if (_AutoMode)
                NextDryRunStep();
            else
            {
                _autoDryRunExit = true;
                SetDryRunStep(DryRunStep.Standby);
            }
            GSystem.TraceMessage($"{_dryRunStep}");
        }
        void DryRunStep_MoveNFCStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetMoveNFCYStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_MoveNFCWait()
        {
            if (!GSystem.MiPLC.GetMoveNFCYComplete(_CH1))
                return;
            GSystem.MiPLC.SetMoveNFCYStart(_CH1, false);
            NextDryRunStep();
        }
        void DryRunStep_NFCDownStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetNFCZDownStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_NFCDownWait()
        {
            if (!GSystem.MiPLC.GetNFCZDownComplete(_CH1))
                return;
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetNFCZDownStart(_CH1, false);
            NextDryRunStep();
        }
        void DryRunStep_NFCUpStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetNFCZUpStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_NFCUpWait()
        {
            if (!GSystem.MiPLC.GetNFCZUpComplete(_CH1))
                return;
            GSystem.MiPLC.SetNFCZUpStart(_CH1, false);
            GSystem.TraceMessage($"{_dryRunStep}");
            if (_AutoMode)
                NextDryRunStep();
            else
            {
                _autoDryRunExit = true;
                SetDryRunStep(DryRunStep.Standby);
            }
            GSystem.TraceMessage($"{_dryRunStep}");
        }
        void DryRunStep_MoveUnloadStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetMoveLoadStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_MoveUnloadWait()
        {
            if (!GSystem.MiPLC.GetMoveLoadComplete(_CH1))
                return;
            GSystem.MiPLC.SetMoveLoadStart(_CH1, false);
            GSystem.TraceMessage($"{_dryRunStep}");
            if (_AutoMode)
                NextDryRunStep();
            else
            {
                _autoDryRunExit = true;
                SetDryRunStep(DryRunStep.Standby);
            }
            GSystem.TraceMessage($"{_dryRunStep}");
        }
        void DryRunStep_UnloadingStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetUnloadingStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_UnloadingWait()
        {
            if (!GSystem.MiPLC.GetUnloadingComplete(_CH1))
                return;
            GSystem.MiPLC.SetUnloadingStart(_CH1, false);
            GSystem.TraceMessage($"{_dryRunStep}");
            if (_AutoMode)
                NextDryRunStep();
            else
            {
                _autoDryRunExit = true;
                SetDryRunStep(DryRunStep.Standby);
            }
            GSystem.TraceMessage($"{_dryRunStep}");
        }
        void DryRunStep_UnclampStart()
        {
            Thread.Sleep(_stepInterval);
            GSystem.MiPLC.SetUnclampForeStart(_CH1, true);
            NextDryRunStep();
        }
        void DryRunStep_UnclampWait()
        {
            if (!GSystem.MiPLC.GetUnclampForeComplete(_CH1))
                return;
            GSystem.MiPLC.SetUnclampForeStart(_CH1, false);
            GSystem.TraceMessage($"{_dryRunStep}");
            _autoDryRunExit = true;
            SetDryRunStep(DryRunStep.Standby);
        }

        private async void buttonAutoLoadingCh1_Click(object sender, EventArgs e)
        {
            if ((GSystem.MiPLC.GetState1(_CH1) & 0x0006) == 0x0006)
                return;
            _AutoMode = false;
            SetDryRunStep(DryRunStep.LoadingStart);
            await Task.Run(() => PLC_AutoDryRunAsyncCh1());
        }

        private async void buttonAutoTouchCh1_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetDryRunStep(DryRunStep.MoveTouchStart);
            await Task.Run(() => PLC_AutoDryRunAsyncCh1());
        }

        private async void buttonAutoCancelCh1_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetDryRunStep(DryRunStep.MoveCancelStart);
            await Task.Run(() => PLC_AutoDryRunAsyncCh1());
        }

        private async void buttonAutoNfcCh1_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetDryRunStep(DryRunStep.MoveNFCStart);
            await Task.Run(() => PLC_AutoDryRunAsyncCh1());
        }

        private async void buttonAutoMoveUnloadCh1_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetDryRunStep(DryRunStep.MoveUnloadStart);
            await Task.Run(() => PLC_AutoDryRunAsyncCh1());
        }

        private async void buttonAutoUnloadCh1_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetDryRunStep(DryRunStep.UnloadingStart);
            await Task.Run(() => PLC_AutoDryRunAsyncCh1());
        }

        private async void buttonAutoUnclampCh1_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetDryRunStep(DryRunStep.UnclampStart);
            await Task.Run(() => PLC_AutoDryRunAsyncCh1());
        }

        private async void buttonPLCAutoStartCh1_Click(object sender, EventArgs e)
        {
            _AutoMode = true;
            SetDryRunStep(DryRunStep.LoadingStart);
            await Task.Run(() => PLC_AutoDryRunAsyncCh1());
        }

        private void buttonPLCAutoStopCh1_Click(object sender, EventArgs e)
        {
            _autoDryRunExit = true;
        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string buttonTag = button.Tag.ToString();


            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            switch (buttonTag)
            {
                case "DATA_ALL":
                    dlg.Title = "전데 데이터 경로 선택";
                    break;
                case "DATA_PASS":
                    dlg.Title = "양품 데이터 경로 선택";
                    break;
                case "DATA_BACK":
                    dlg.Title = "백업 데이터 경로 선택";
                    break;
                case "DATA_REPEAT":
                    dlg.Title = "반복 데이터 경로 선택";
                    break;
                default:
                    break;
            }
            dlg.IsFolderPicker = true;
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                return;
            switch (buttonTag)
            {
                case "DATA_ALL":
                    GSystem.SystemData.GeneralSettings.DataFolderAll = dlg.FileName;
                    textDataFolderAll.Text = dlg.FileName;
                    break;
                case "DATA_PASS":
                    GSystem.SystemData.GeneralSettings.DataFolderPass = dlg.FileName;
                    textDataFolderPass.Text = dlg.FileName;
                    break;
                case "DATA_BACK":
                    GSystem.SystemData.GeneralSettings.DataFolderBack = dlg.FileName;
                    textDataFolderBack.Text = dlg.FileName;
                    break;
                case "DATA_REPEAT":
                    GSystem.SystemData.GeneralSettings.DataFolderRepeat = dlg.FileName;
                    textDataFolderRepeat.Text = dlg.FileName;
                    break;
                default:
                    break;
            }
        }

        private void buttonConnCountInit_Click(object sender, EventArgs e)
        {
            string caption = "초기화";
            string message = "커넥터 교체 주기를 초기화 하시겠습니까?";
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GSystem.SystemData.ConnectorNFCTouch1Ch1.UseCount = 0;
                GSystem.SystemDataSave();
            }
        }

        private async void buttonSetSensor_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    int sensorIndex = comboSensorModel.SelectedIndex;
                    GSystem.DedicatedCTRL.SetSensorModel(sensorIndex);
                    GSystem.DedicatedCTRL.SetCommandSensorModel(GSystem.CH1, true);
                    GSystem.DedicatedCTRL.SetCommandSensorModel(GSystem.CH2, true);
                    while (!GSystem.DedicatedCTRL.GetCommandSensorModel(_CH1) || !GSystem.DedicatedCTRL.GetCommandSensorModel(_CH2))
                    {
                        Task.Delay(10);
                    }
                    GSystem.DedicatedCTRL.SetCommandSensorModel(GSystem.CH1, false);
                    GSystem.DedicatedCTRL.SetCommandSensorModel(GSystem.CH2, false);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

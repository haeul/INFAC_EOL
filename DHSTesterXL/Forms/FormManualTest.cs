using GSCommon;
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
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace DHSTesterXL.Forms
{
    public partial class FormManualTest : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();

        //private readonly PNFCTouch _pNFCTouchFD;
        private readonly ProductConfig _tempProductSettings = new ProductConfig();
        private TickTimer _tickStepElapse = new TickTimer();

        public event EventHandler<TestStateEventArgs> TestStateChanged;
        public event EventHandler<TestStateEventArgs> TestStepProgressChanged;

        public int Channel { get; set; }

        public FormManualTest()
        {
            InitializeComponent();
            //_pNFCTouchFD = PNFCTouch.GetInstance();
            // 임시 ProductSettings를 사용한다.
            _tempProductSettings = GSystem.ProductSettings;
        }

        private void FormManualTest_Load(object sender, EventArgs e)
        {
            this.Text = $"[CH.{Channel + 1}] Manual Test";

            ControlHelper.SetDoubleBuffered(gridTestList, true);

            SetupGridTestList();

            TestStepProgressChanged += OnTestStepProgressChanged;
        }

        private void FormManualTest_Shown(object sender, EventArgs e)
        {

        }

        private void FormManualTest_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormManualTest_FormClosed(object sender, FormClosedEventArgs e)
        {

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

        private void buttonTestStart_Click(object sender, EventArgs e)
        {
            if (gridTestList.CurrentCell == null)
                return;
            int selectedItem = gridTestList.CurrentCell.RowIndex;
            GSystem.TraceMessage($"Selected row index = {selectedItem}, TestItems = {(TestItems)selectedItem}");
            // UI 상태
            buttonTestStart.Enabled = false;
            //await Task.Run(() => NFCTouchTestStep_ShortTestStart(Channel, _cts.Token));

            StartTestAsync((TestItems)selectedItem);
        }

        private void buttonTestStop_Click(object sender, EventArgs e)
        {
            StopTest();
        }

        private async void StartTestAsync(TestItems testItem)
        {
            _cts = new CancellationTokenSource();
            try
            {
                switch (testItem)
                {
                    case TestItems.Short_1_2:
                    case TestItems.Short_1_3:
                    case TestItems.Short_1_4:
                    case TestItems.Short_1_6:
                    case TestItems.Short_2_3:
                    case TestItems.Short_2_4:
                    case TestItems.Short_2_6:
                    case TestItems.Short_3_4:
                    case TestItems.Short_3_6:
                    case TestItems.Short_4_6:
                        await Task.Run(() => NFCTouchTestStep_ShortTestStart(Channel, _cts.Token));
                        break;
                    case TestItems.SerialNumber:
                        break;
                    case TestItems.DarkCurrent:
                        break;
                    case TestItems.PLightTurnOn:
                        break;
                    case TestItems.PLightCurrent:
                        break;
                    case TestItems.PLightAmbient:
                        break;
                    case TestItems.LockSen:
                        break;
                    case TestItems.Cancel:
                        break;
                    case TestItems.SecurityBit:
                        break;
                    case TestItems.NFC:
                        break;
                    case TestItems.DTC_Erase:
                        break;
                    case TestItems.HW_Version:
                        // 수신스레드를 종료하지 않으면 통신이 잘 안된다. 아직은 원인을 모르겠다. 좀 더 연구가 필요하다.
                        //_pNFCTouchFD.StopCanReceiveThread(Channel);
                        await Task.Delay(100);
                        //await Task.Run(async () => await _pNFCTouchFD.NFCTouchTestStep_HWVersion(Channel, _cts.Token));
                        await Task.Delay(100);
                        //_pNFCTouchFD.StartCanReceiveThread(Channel);
                        break;
                    case TestItems.SW_Version:
                        break;
                    case TestItems.PartNumber:
                        break;
                    case TestItems.Bootloader:
                        break;
                    case TestItems.RXSWIN:
                        break;
                    case TestItems.Manufacture:
                        break;
                    case TestItems.SupplierCode:
                        break;
                    case TestItems.OperationCurrent:
                        break;
                    default:
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // 정상 취소
                GSystem.TraceMessage($"Canceled!");
                //_pNFCTouchFD.StartCanReceiveThread(Channel);
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
            }
            finally
            {
                StopTest();
            }
        }

        private void StopTest()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
            // UI 상태
            buttonTestStart.Enabled = true;
        }

        private async Task NFCTouchTestStep_ShortTestStart(int channel, CancellationToken token)
        {
            await Task.Delay(10);

            _tickStepElapse.Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Pin Shot]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Pin Shot]");
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_2], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_3], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_4], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_6], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_3], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_4], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_6], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_4], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_6], "측정 중", TestStates.Running));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_4_6], "측정 중", TestStates.Running));
            GSystem.DedicatedCTRL.SetCommandShortTest(channel, true);

            // 완료 대기
            while (!GSystem.DedicatedCTRL.GetCommandShortTest(channel) || !GSystem.DedicatedCTRL.GetCompleteShortTest(channel))
            {
                token.ThrowIfCancellationRequested(); // ThrowIfCancellationRequested가 더 깔끔합니다.
            }
            GSystem.DedicatedCTRL.SetCommandShortTest(channel, false);

            short ShortResult_1_2;
            short ShortResult_1_3;
            short ShortResult_1_4;
            short ShortResult_1_6;
            short ShortResult_2_3;
            short ShortResult_2_4;
            short ShortResult_2_6;
            short ShortResult_3_4;
            short ShortResult_3_6;
            short ShortResult_4_6;
            // 측정 결과 반영
            if (channel == (int)DedicatedChannels.Ch1)
            {
                ShortResult_1_2 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_1_2;
                ShortResult_1_3 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_1_3;
                ShortResult_1_4 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_1_4;
                ShortResult_1_6 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_1_6;
                ShortResult_2_3 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_2_3;
                ShortResult_2_4 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_2_4;
                ShortResult_2_6 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_2_6;
                ShortResult_3_4 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_3_4;
                ShortResult_3_6 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_3_6;
                ShortResult_4_6 = (short)GSystem.DedicatedCTRL.Reg_03h_ch1_short_4_6;
            }
            else
            {
                ShortResult_1_2 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_1_2;
                ShortResult_1_3 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_1_3;
                ShortResult_1_4 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_1_4;
                ShortResult_1_6 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_1_6;
                ShortResult_2_3 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_2_3;
                ShortResult_2_4 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_2_4;
                ShortResult_2_6 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_2_6;
                ShortResult_3_4 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_3_4;
                ShortResult_3_6 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_3_6;
                ShortResult_4_6 = (short)GSystem.DedicatedCTRL.Reg_03h_ch2_short_4_6;
            }
            // 결과 
            short minShort_1_2 = (short)GSystem.ProductSettings.TestItemSpecs.Short_1_2.MinValue;
            short minShort_1_3 = (short)GSystem.ProductSettings.TestItemSpecs.Short_1_3.MinValue;
            short minShort_1_4 = (short)GSystem.ProductSettings.TestItemSpecs.Short_1_4.MinValue;
            short minShort_1_6 = (short)GSystem.ProductSettings.TestItemSpecs.Short_1_6.MinValue;
            short minShort_2_3 = (short)GSystem.ProductSettings.TestItemSpecs.Short_2_3.MinValue;
            short minShort_2_4 = (short)GSystem.ProductSettings.TestItemSpecs.Short_2_4.MinValue;
            short minShort_2_6 = (short)GSystem.ProductSettings.TestItemSpecs.Short_2_6.MinValue;
            short minShort_3_4 = (short)GSystem.ProductSettings.TestItemSpecs.Short_3_4.MinValue;
            short minShort_3_6 = (short)GSystem.ProductSettings.TestItemSpecs.Short_3_6.MinValue;
            short minShort_4_6 = (short)GSystem.ProductSettings.TestItemSpecs.Short_4_6.MinValue;

            short maxShort_1_2 = (short)GSystem.ProductSettings.TestItemSpecs.Short_1_2.MaxValue;
            short maxShort_1_3 = (short)GSystem.ProductSettings.TestItemSpecs.Short_1_3.MaxValue;
            short maxShort_1_4 = (short)GSystem.ProductSettings.TestItemSpecs.Short_1_4.MaxValue;
            short maxShort_1_6 = (short)GSystem.ProductSettings.TestItemSpecs.Short_1_6.MaxValue;
            short maxShort_2_3 = (short)GSystem.ProductSettings.TestItemSpecs.Short_2_3.MaxValue;
            short maxShort_2_4 = (short)GSystem.ProductSettings.TestItemSpecs.Short_2_4.MaxValue;
            short maxShort_2_6 = (short)GSystem.ProductSettings.TestItemSpecs.Short_2_6.MaxValue;
            short maxShort_3_4 = (short)GSystem.ProductSettings.TestItemSpecs.Short_3_4.MaxValue;
            short maxShort_3_6 = (short)GSystem.ProductSettings.TestItemSpecs.Short_3_6.MaxValue;
            short maxShort_4_6 = (short)GSystem.ProductSettings.TestItemSpecs.Short_4_6.MaxValue;

            TestStates resultShort_1_2 = TestStates.Pass;
            TestStates resultShort_1_3 = TestStates.Pass;
            TestStates resultShort_1_4 = TestStates.Pass;
            TestStates resultShort_1_6 = TestStates.Pass;
            TestStates resultShort_2_3 = TestStates.Pass;
            TestStates resultShort_2_4 = TestStates.Pass;
            TestStates resultShort_2_6 = TestStates.Pass;
            TestStates resultShort_3_4 = TestStates.Pass;
            TestStates resultShort_3_6 = TestStates.Pass;
            TestStates resultShort_4_6 = TestStates.Pass;

            if (ShortResult_1_2 < minShort_1_2 || ShortResult_1_2 > maxShort_1_2) resultShort_1_2 = TestStates.Failed;
            if (ShortResult_1_3 < minShort_1_3 || ShortResult_1_3 > maxShort_1_3) resultShort_1_3 = TestStates.Failed;
            if (ShortResult_1_4 < minShort_1_4 || ShortResult_1_4 > maxShort_1_4) resultShort_1_4 = TestStates.Failed;
            if (ShortResult_1_6 < minShort_1_6 || ShortResult_1_6 > maxShort_1_6) resultShort_1_6 = TestStates.Failed;
            if (ShortResult_2_3 < minShort_2_3 || ShortResult_2_3 > maxShort_2_3) resultShort_2_3 = TestStates.Failed;
            if (ShortResult_2_4 < minShort_2_4 || ShortResult_2_4 > maxShort_2_4) resultShort_2_4 = TestStates.Failed;
            if (ShortResult_2_6 < minShort_2_6 || ShortResult_2_6 > maxShort_2_6) resultShort_2_6 = TestStates.Failed;
            if (ShortResult_3_4 < minShort_3_4 || ShortResult_3_4 > maxShort_3_4) resultShort_3_4 = TestStates.Failed;
            if (ShortResult_3_6 < minShort_3_6 || ShortResult_3_6 > maxShort_3_6) resultShort_3_6 = TestStates.Failed;
            if (ShortResult_4_6 < minShort_4_6 || ShortResult_4_6 > maxShort_4_6) resultShort_4_6 = TestStates.Failed;

            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 1-2: [ {ShortResult_1_2,3} uA ] [ {resultShort_1_2} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 1-3: [ {ShortResult_1_3,3} uA ] [ {resultShort_1_3} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 1-4: [ {ShortResult_1_4,3} uA ] [ {resultShort_1_4} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 1-6: [ {ShortResult_1_6,3} uA ] [ {resultShort_1_6} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 2-3: [ {ShortResult_2_3,3} uA ] [ {resultShort_2_3} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 2-4: [ {ShortResult_2_4,3} uA ] [ {resultShort_2_4} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 2-6: [ {ShortResult_2_6,3} uA ] [ {resultShort_2_6} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 3-4: [ {ShortResult_3_4,3} uA ] [ {resultShort_3_4} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 3-6: [ {ShortResult_3_6,3} uA ] [ {resultShort_3_6} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Short 4-6: [ {ShortResult_4_6,3} uA ] [ {resultShort_4_6} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Pin Short step time: [ {_tickStepElapse.GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Pin Short step time: [ {_tickStepElapse.GetElapsedMilliseconds()} ms ]");

            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_2], $"{ShortResult_1_2} uA", resultShort_1_2));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_3], $"{ShortResult_1_3} uA", resultShort_1_3));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_4], $"{ShortResult_1_4} uA", resultShort_1_4));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_6], $"{ShortResult_1_6} uA", resultShort_1_6));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_3], $"{ShortResult_2_3} uA", resultShort_2_3));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_4], $"{ShortResult_2_4} uA", resultShort_2_4));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_6], $"{ShortResult_2_6} uA", resultShort_2_6));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_4], $"{ShortResult_3_4} uA", resultShort_3_4));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_6], $"{ShortResult_3_6} uA", resultShort_3_6));
            //TestStepProgressChanged?.Invoke(this, new TestStateEventArgs(channel, GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_4_6], $"{ShortResult_4_6} uA", resultShort_4_6));
            GSystem.Logger.Info ($"[CH.{channel + 1}] Pin Short Test Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Pin Short Test Complete");
        }

        private void OnTestStepProgressChanged(object sender, TestStateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string, string, TestStates>(UpdateTestStepProgressChanged), e.Channel, e.Name, e.Value, e.State);
            }
            else
            {
                UpdateTestStepProgressChanged(e.Channel, e.Name, e.Value, e.State);
            }
        }

        private void UpdateTestStepProgressChanged(int channel, string testName, string measureValue, TestStates state)
        {
            for (int i = 0; i < (int)TestItems.Count; i++)
            {
                TestSpec testSpec = _tempProductSettings.GetTestItemSpec((TestItems)i);
                if (testSpec.Name == testName)
                {
                    for (int rowIndex = 0; rowIndex < gridTestList.RowCount; rowIndex++)
                    {
                        if (gridTestList[2, rowIndex].Value.ToString().IndexOf(testName) >= 0)
                        {
                            gridTestList[5, rowIndex].Value = measureValue;
                            gridTestList[5, rowIndex].Style.BackColor = GetTestStepStateColor(state);
                            break;
                        }
                    }
                }
            }
        }
        private Color GetTestStepStateColor(TestStates state)
        {
            switch (state)
            {
                case TestStates.Ready   : return Color.White      ;
                case TestStates.Running : return Color.DodgerBlue ;
                case TestStates.Pass    : return Color.PaleGreen  ;
                case TestStates.Failed  : return Color.OrangeRed  ;
                default                 : return Color.White      ;
            }
        }

    }
}

using GSCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static DHSTesterXL.FormSettings;

namespace DHSTesterXL.Forms
{
    public partial class FormXcpData : Form
    {
        private List<int> TouchFastMutualList = new List<int>();
        private List<int> TouchFastSelfList = new List<int>();

        int idleTouchFastMutualAvg = 0;
        int idleTouchFastSelfAvg = 0;
        int deltaTouchFastMutual = 0;
        int deltaTouchFastSelf = 0;
        int thdTouchFastMutual = 0;
        int thdTouchFastSelf = 0;
        int judgeCountTouchFastMutual = 0;
        int judgeCountTouchFastSelf = 0;

        bool isTouchFastMutualIdleAverage = false;
        bool isTouchFastMutualComplete = false;
        bool isTouchFastSelfIdleAverage = false;
        bool isTouchFastSelfComplete = false;
        bool isTouchFirstExecute = true;

        private List<int> CancelFastSelfList = new List<int>();
        private List<int> CancelSlowSelfList = new List<int>();

        int idleCancelFastSelfAvg = 0;
        int idleCancelSlowSelfAvg = 0;
        int deltaCancelFastSelf = 0;
        int deltaCancelSlowSelf = 0;
        int thdCancelFastSelf = 0;
        int thdCancelSlowSelf = 0;
        int judgeCountCancelFastSelf = 0;
        int judgeCountCancelSlowSelf = 0;

        bool isCancelFastSelfIdleAverage = false;
        bool isCancelFastSelfComplete = false;
        bool isCancelSlowSelfIdleAverage = false;
        bool isCancelSlowSelfComplete = false;
        bool isCancelFirstExecute = true;

        public int Channel { get; set; }


        public FormXcpData()
        {
            InitializeComponent();
        }

        private void FormXcpData_Load(object sender, EventArgs e)
        {
            ControlHelper.SetDoubleBuffered(gridTouch, true);
            ControlHelper.SetDoubleBuffered(gridCancel, true);

            GSystem.DHSModel.TouchXcpDataChanged += OnUpdateTouchXcpData;
            GSystem.DHSModel.CancelXcpDataChanged += OnUpdateCancelXcpData;

            isTouchFastMutualIdleAverage = false;
            isTouchFastSelfIdleAverage = false;
            isTouchFirstExecute = true;
            isCancelFirstExecute = true;

            TouchFastMutualList.Clear();
            TouchFastSelfList.Clear();

            thdTouchFastMutual = Convert.ToInt32(textThdTouchFastMutual.Text);
            thdTouchFastSelf = Convert.ToInt32(textThdTouchFastSelf.Text);

            this.Text = $"[CH.{Channel + 1}] XCP Capacitance Data";

            ResetTouch();
            ResetCancel();
        }

        private void FormXcpData_Shown(object sender, EventArgs e)
        {

        }

        private void FormXcpData_FormClosing(object sender, FormClosingEventArgs e)
        {
            GSystem.DHSModel.SetTouchStepExit(Channel, true);
            GSystem.DHSModel.SetCancelStepExit(Channel, true);
        }

        private void FormXcpData_FormClosed(object sender, FormClosedEventArgs e)
        {
            GSystem.DHSModel.TouchXcpDataChanged -= OnUpdateTouchXcpData;
            GSystem.DHSModel.CancelXcpDataChanged -= OnUpdateCancelXcpData;
        }

        private async void buttonStartTouch_Click(object sender, EventArgs e)
        {
            if (isTouchFirstExecute)
            {
                timerTouchFirst.Interval = 1000;
                timerTouchFirst.Start();
            }
            buttonStartTouch.Enabled = false;
            buttonStartCancel.Enabled = false;
            buttonStopCancel.Enabled = false;
            ResetTouch();
            await Task.Run(async () => await GSystem.DHSModel.GetTouchAsync(Channel));
        }

        private void buttonStopTouch_Click(object sender, EventArgs e)
        {
            GSystem.DHSModel.SetTouchStepExit(Channel, true);
            buttonStartTouch.Enabled = true;
            buttonStartCancel.Enabled = true;
            buttonStopCancel.Enabled = true;
        }

        private void buttonResetTouch_Click(object sender, EventArgs e)
        {
            ResetTouch();
        }

        private async void buttonStartCancel_Click(object sender, EventArgs e)
        {
            if (isCancelFirstExecute)
            {
                timerCancelFirst.Interval = 1000;
                timerCancelFirst.Start();
            }
            buttonStartCancel.Enabled = false;
            buttonStartTouch.Enabled = false;
            buttonStopTouch.Enabled = false;
            ResetCancel();
            await Task.Run(async () => await GSystem.DHSModel.GetCancelAsync(Channel));
        }

        private void buttonStopCancel_Click(object sender, EventArgs e)
        {
            GSystem.DHSModel.SetCancelStepExit(Channel, true);
            buttonStartCancel.Enabled = true;
            buttonStartTouch.Enabled = true;
            buttonStopTouch.Enabled = true;
        }

        private void buttonResetCancel_Click(object sender, EventArgs e)
        {
            ResetCancel();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ResetTouch()
        {
            gridTouch.SuspendLayout();
            gridTouch.Rows.Clear();
            gridTouch.ResumeLayout();
            labelResultTouchFastMutual.BackColor = SystemColors.Control;
            labelResultTouchFastSelf.BackColor = SystemColors.Control;
            isTouchFastMutualIdleAverage = false;
            isTouchFastSelfIdleAverage = false;
            TouchFastMutualList.Clear();
            TouchFastSelfList.Clear();
            isTouchFastMutualComplete = false;
            isTouchFastSelfComplete = false;
            thdTouchFastMutual = Convert.ToInt32(textThdTouchFastMutual.Text);
            thdTouchFastSelf = Convert.ToInt32(textThdTouchFastSelf.Text);
        }

        private void ResetCancel()
        {
            gridCancel.SuspendLayout();
            gridCancel.Rows.Clear();
            gridCancel.ResumeLayout();
            labelResultCancelFastSelf.BackColor = SystemColors.Control;
            labelResultCancelSlowSelf.BackColor = SystemColors.Control;
            isCancelFastSelfIdleAverage = false;
            isCancelSlowSelfIdleAverage = false;
            CancelFastSelfList.Clear();
            CancelSlowSelfList.Clear();
            isCancelFastSelfComplete = false;
            isCancelSlowSelfComplete = false;
            thdCancelFastSelf = Convert.ToInt32(textThdCancelSlowSelf.Text);
            thdCancelSlowSelf = Convert.ToInt32(textThdCancelFastSelf.Text);
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

        private void UpdateTouchXcpData(int channel, int touchFastMutual, int touchFastSelf, int touchSlowSelf, float touchComboRate, int touchState, int intervalTime)
        {
            // idle 평균
            if (!isTouchFastMutualIdleAverage)
            {
                TouchFastMutualList.Add(touchFastMutual);
                if (TouchFastMutualList.Count > GSystem.MaxAverageCount)
                {
                    TouchFastMutualList.RemoveAt(0);
                    idleTouchFastMutualAvg = (int)TouchFastMutualList.Average();
                    isTouchFastMutualIdleAverage = true;
                }
            }

            if (!isTouchFastSelfIdleAverage)
            {
                TouchFastSelfList.Add(touchFastSelf);
                if (TouchFastSelfList.Count > GSystem.MaxAverageCount)
                {
                    TouchFastSelfList.RemoveAt(0);
                    idleTouchFastSelfAvg = (int)TouchFastSelfList.Average();
                    isTouchFastSelfIdleAverage = true;
                }
            }

            // delta 계산
            deltaTouchFastMutual = touchFastMutual - idleTouchFastMutualAvg;
            deltaTouchFastSelf = touchFastSelf - idleTouchFastSelfAvg;

            // THD 비교
            if (!isTouchFirstExecute)
            {
                if (!isTouchFastMutualComplete)
                {
                    if (deltaTouchFastMutual > thdTouchFastMutual)
                    {
                        if (++judgeCountTouchFastMutual > GSystem.MaxJudgeCount)
                        {
                            isTouchFastMutualComplete = true;
                            // OK
                            labelResultTouchFastMutual.Text = "OK";
                            labelResultTouchFastMutual.BackColor = Color.LimeGreen;
                        }
                    }
                    else
                    {
                        // reset
                        judgeCountTouchFastMutual = 0;
                    }
                }
                if (!isTouchFastSelfComplete)
                {
                    if (deltaTouchFastSelf > thdTouchFastSelf)
                    {
                        if (++judgeCountTouchFastSelf > GSystem.MaxJudgeCount)
                        {
                            isTouchFastSelfComplete = true;
                            // OK
                            labelResultTouchFastSelf.Text = "OK";
                            labelResultTouchFastSelf.BackColor = Color.LimeGreen;
                        }
                    }
                    else
                    {
                        // reset
                        judgeCountTouchFastSelf = 0;
                    }
                }
            }

            // 컨트롤 업데이트
            textTouchFastMutual.Text = touchFastMutual.ToString();
            textIdleTouchFastMutualAvg.Text = idleTouchFastMutualAvg.ToString();
            textDeltaTouchFastMutual.Text = deltaTouchFastMutual.ToString();
            textTouchFastSelf.Text = touchFastSelf.ToString();
            textIdleTouchFastSelfAvg.Text = idleTouchFastSelfAvg.ToString();
            textDeltaTouchFastSelf.Text = deltaTouchFastSelf.ToString();
            textTouchFastMutualInterval.Text = $"{intervalTime} ms";
            textTouchFastSelfInterval.Text = $"{intervalTime} ms";
            textTouchSlowSelf.Text = touchSlowSelf.ToString();
            textTouchComboRate.Text = touchComboRate.ToString("F2");
            textTouchState.Text = touchState.ToString();
            // Grid에 추가
            if (!checkPauseTouch.Checked)
            {
                string strNo = gridTouch.RowCount.ToString();
                gridTouch.Rows.Add(strNo, textTouchFastMutual.Text, textTouchFastSelf.Text, textTouchFastSelfInterval.Text);
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
        private void UpdateCancelXcpData(int channel, int cancelFastSelf, int cancelSlowSelf, int cancelState, int intervalTime)
        {
            // idle 평균
            if (!isCancelFastSelfIdleAverage)
            {
                CancelFastSelfList.Add(cancelFastSelf);
                if (CancelFastSelfList.Count > GSystem.MaxAverageCount)
                {
                    CancelFastSelfList.RemoveAt(0);
                    idleCancelFastSelfAvg = (int)CancelFastSelfList.Average();
                    isCancelFastSelfIdleAverage = true;
                }
            }

            if (!isCancelSlowSelfIdleAverage)
            {
                CancelSlowSelfList.Add(cancelSlowSelf);
                if (CancelSlowSelfList.Count > GSystem.MaxAverageCount)
                {
                    CancelSlowSelfList.RemoveAt(0);
                    idleCancelSlowSelfAvg = (int)CancelSlowSelfList.Average();
                    isCancelSlowSelfIdleAverage = true;
                }
            }

            // delta 계산
            deltaCancelFastSelf = cancelFastSelf - idleCancelFastSelfAvg;
            deltaCancelSlowSelf = cancelSlowSelf - idleCancelSlowSelfAvg;

            // THD 비교
            if (!isCancelFirstExecute)
            {
                if (!isCancelFastSelfComplete)
                {
                    if (deltaCancelFastSelf > thdCancelFastSelf)
                    {
                        if (++judgeCountCancelFastSelf > GSystem.MaxJudgeCount)
                        {
                            isCancelFastSelfComplete = true;
                            // OK
                            labelResultCancelFastSelf.Text = "OK";
                            labelResultCancelFastSelf.BackColor = Color.LimeGreen;
                        }
                    }
                    else
                    {
                        // reset
                        judgeCountCancelFastSelf = 0;
                    }
                }
                if (!isCancelSlowSelfComplete)
                {
                    if (deltaCancelSlowSelf > thdCancelSlowSelf)
                    {
                        if (++judgeCountCancelSlowSelf > GSystem.MaxJudgeCount)
                        {
                            isCancelSlowSelfComplete = true;
                            // OK
                            labelResultCancelSlowSelf.Text = "OK";
                            labelResultCancelSlowSelf.BackColor = Color.LimeGreen;
                        }
                    }
                    else
                    {
                        // reset
                        judgeCountCancelSlowSelf = 0;
                    }
                }
            }

            // 컨트롤 업데이트
            textCancelFastSelf.Text = cancelFastSelf.ToString();
            textIdleCancelFastSelfAvg.Text = idleCancelFastSelfAvg.ToString();
            textDeltaCancelFastSelf.Text = deltaCancelFastSelf.ToString();
            textCancelSlowSelf.Text = cancelSlowSelf.ToString();
            textIdleCancelSlowSelfAvg.Text = idleCancelSlowSelfAvg.ToString();
            textDeltaCancelSlowSelf.Text = deltaCancelSlowSelf.ToString();
            textCancelState.Text = cancelState.ToString();
            textCancelFastSelfInterval.Text = $"{intervalTime} ms";
            textCancelSlowSelfInterval.Text = $"{intervalTime} ms";
            // Grid에 추가
            if (!checkPauseCancel.Checked)
            {
                string strNo = gridCancel.RowCount.ToString();
                gridCancel.Rows.Add(strNo, textCancelFastSelf.Text, textCancelSlowSelf.Text, textCancelSlowSelfInterval.Text);
            }
        }

        private void timerTouchFirst_Tick(object sender, EventArgs e)
        {
            timerTouchFirst.Stop();
            if (isTouchFirstExecute)
                isTouchFirstExecute = false;
        }

        private void timerCancelFirst_Tick(object sender, EventArgs e)
        {
            timerCancelFirst.Stop();
            if (isCancelFirstExecute)
                isCancelFirstExecute = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (GSystem.MiPLC.GetTouchZDownStart(Channel))
                await Task.Run(() => GSystem.MiPLC.SetTouchZDownStart(Channel, false));
            else
                await Task.Run(() => GSystem.MiPLC.SetTouchZDownStart(Channel, true));
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (GSystem.MiPLC.GetTouchZUpStart(Channel))
                await Task.Run(() => GSystem.MiPLC.SetTouchZUpStart(Channel, false));
            else
                await Task.Run(() => GSystem.MiPLC.SetTouchZUpStart(Channel, true));
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            // 상승
            if (GSystem.MiPLC.GetCancelZUpStart(Channel))
                await Task.Run(() => GSystem.MiPLC.SetCancelZUpStart(Channel, false));
            else
                await Task.Run(() => GSystem.MiPLC.SetCancelZUpStart(Channel, true));
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            // 하강
            if (GSystem.MiPLC.GetCancelZDownStart(Channel))
                await Task.Run(() => GSystem.MiPLC.SetCancelZDownStart(Channel, false));
            else
                await Task.Run(() => GSystem.MiPLC.SetCancelZDownStart(Channel, true));
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            if (GSystem.MiPLC.GetMoveTouchYStart(Channel))
                await Task.Run(() => GSystem.MiPLC.SetMoveTouchYStart(Channel, false));
            else
                await Task.Run(() => GSystem.MiPLC.SetMoveTouchYStart(Channel, true));
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            if (GSystem.MiPLC.GetMoveCancelYStart(Channel))
                await Task.Run(() => GSystem.MiPLC.SetMoveCancelYStart(Channel, false));
            else
                await Task.Run(() => GSystem.MiPLC.SetMoveCancelYStart(Channel, true));
        }







        //-----------------------------------------------------------------------------------------

        int _totalRepeatCount = 0;
        int _repeatCount = 0;
        bool _AutoMode = false;
        TickTimer _tickStepInterval = new TickTimer();


        public enum TouchStep
        {
            Standby,
            Prepare,
            LoadingStart,
            LoadingWait,
            PowerOnStart,
            PowerOnWait,
            MoveTouchStart,
            MoveTouchWait,
            StartTouchXCP,
            TouchDownStart,
            TouchDownWait,
            TouchUpStart,
            TouchUpWait,
            StopTouchXCP,
            MoveLoadStart,
            MoveLoadWait,
            PowerOffStart,
            PowerOffWait,
            UnloadingStart,
            UnloadingWait,
            Count
        }

        TouchStep _touchStep = TouchStep.Standby;
        bool _autoTouchExit = false;
        bool _enableNM = false;
        TickTimer _tickNM = new TickTimer();
        public TouchStep GetTouchStep() { return _touchStep; }
        public void SetTouchStep(TouchStep runStep) { _touchStep = runStep; }
        public void NextTouchStep() { ++_touchStep; }

        public async Task AutoTouchTest()
        {
            await Task.Delay(10);
            GSystem.TraceMessage($"AutoTouchTest start...");
            _touchStep = TouchStep.Prepare;
            _autoTouchExit = false;
            while (!_autoTouchExit)
            {
                await Task.Delay(10);
                if (_enableNM)
                {
                    if (_tickNM.MoreThan(200))
                    {
                        _tickNM.Reset(); ;
                        GSystem.DHSModel.Send_NM(Channel);
                    }
                }
                switch (_touchStep)
                {
                    case TouchStep.Standby         : TouchStep_Standby        (); break;
                    case TouchStep.Prepare         : TouchStep_Prepare        (); break;
                    case TouchStep.LoadingStart    : TouchStep_LoadingStart   (); break;
                    case TouchStep.LoadingWait     : TouchStep_LoadingWait    (); break;
                    case TouchStep.PowerOnStart    : TouchStep_PowerOnStart   (); break;
                    case TouchStep.PowerOnWait     : TouchStep_PowerOnWait    (); break;
                    case TouchStep.MoveTouchStart  : TouchStep_MoveTouchStart (); break;
                    case TouchStep.MoveTouchWait   : TouchStep_MoveTouchWait  (); break;
                    case TouchStep.StartTouchXCP   : TouchStep_StartTouchXCP  (); break;
                    case TouchStep.TouchDownStart  : TouchStep_TouchDownStart (); break;
                    case TouchStep.TouchDownWait   : TouchStep_TouchDownWait  (); break;
                    case TouchStep.TouchUpStart    : TouchStep_TouchUpStart   (); break;
                    case TouchStep.TouchUpWait     : TouchStep_TouchUpWait    (); break;
                    case TouchStep.StopTouchXCP    : TouchStep_StopTouchXCP   (); break;
                    case TouchStep.MoveLoadStart   : TouchStep_MoveLoadStart  (); break;
                    case TouchStep.MoveLoadWait    : TouchStep_MoveLoadWait   (); break;
                    case TouchStep.PowerOffStart   : TouchStep_PowerOffStart  (); break;
                    case TouchStep.PowerOffWait    : TouchStep_PowerOffWait   (); break;
                    case TouchStep.UnloadingStart  : TouchStep_UnloadingStart (); break;
                    case TouchStep.UnloadingWait   : TouchStep_UnloadingWait  (); break;
                    default:
                        break;
                }
            }
            GSystem.TraceMessage($"AutoTouchTest terminated!");
            return;
        }
        private void TouchStep_Standby()
        {

        }
        private void TouchStep_Prepare()
        {
            NextTouchStep();
        }
        private void TouchStep_LoadingStart()
        {
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            _totalRepeatCount = Convert.ToInt16(textTouchRepeat.Text);
            _repeatCount = 0;
            GSystem.MiPLC.SetLoadingStart(Channel, true);
            NextTouchStep();
        }
        private void TouchStep_LoadingWait()
        {
            if (GSystem.MiPLC.GetLoadingStart(Channel))
            {
                if (!GSystem.MiPLC.GetLoadingComplete(Channel))
                    return;
                GSystem.MiPLC.SetLoadingStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            if (_AutoMode)
            {
                NextTouchStep();
                _tickStepInterval.Reset();
            }
            else
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
        }
        private void TouchStep_PowerOnStart()
        {
            if (_tickStepInterval.LessThan(1000))
                return;
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(Channel, true);
            NextTouchStep();
        }
        private void TouchStep_PowerOnWait()
        {
            if (GSystem.DedicatedCTRL.GetCommandActivePowerOn(Channel))
            {
                if (!GSystem.DedicatedCTRL.GetCompleteActivePowerOn(Channel))
                    return;
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            if (_AutoMode)
            {
                NextTouchStep();
                _tickStepInterval.Reset();
                _tickNM.Reset();
            }
            else
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
        }
        private void TouchStep_MoveTouchStart()
        {
            if (_tickStepInterval.LessThan(2500))
                return;
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            GSystem.MiPLC.SetMoveTouchYStart(Channel, true);
            _enableNM = true;
            NextTouchStep();
        }
        private void TouchStep_MoveTouchWait()
        {
            if (GSystem.MiPLC.GetMoveTouchYStart(Channel))
            {
                if (!GSystem.MiPLC.GetMoveTouchYComplete(Channel))
                    return;
                GSystem.MiPLC.SetMoveTouchYStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            if (_AutoMode)
            {
                NextTouchStep();
            }
            else
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
        }
        private void TouchStep_StartTouchXCP()
        {
            Task.Run(async () => await GSystem.DHSModel.GetTouchAsync(Channel));
            _tickStepInterval.Reset();
            NextTouchStep();
        }
        private void TouchStep_TouchDownStart()
        {
            if (_tickStepInterval.MoreThan(2000))
            {
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
                GSystem.MiPLC.SetTouchZDownStart(Channel, true);
                NextTouchStep();
            }
        }
        private void TouchStep_TouchDownWait()
        {
            if (GSystem.MiPLC.GetTouchZDownStart(Channel))
            {
                if (!GSystem.MiPLC.GetTouchZDownComplete(Channel))
                    return;
                GSystem.MiPLC.SetTouchZDownStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            if (_AutoMode)
            {
                _tickStepInterval.Reset();
                NextTouchStep();
            }
            else
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
        }
        private void TouchStep_TouchUpStart()
        {
            if (_tickStepInterval.LessThan(1000))
                return;
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            GSystem.MiPLC.SetTouchZUpStart(Channel, true);
            NextTouchStep();
        }
        private void TouchStep_TouchUpWait()
        {
            if (GSystem.MiPLC.GetTouchZUpStart(Channel))
            {
                if (!GSystem.MiPLC.GetTouchZUpComplete(Channel))
                    return;
                GSystem.MiPLC.SetTouchZUpStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            if (_AutoMode)
            {
                if (++_repeatCount < _totalRepeatCount)
                {
                    SetTouchStep(TouchStep.TouchDownStart);
                }
                else
                {
                    NextTouchStep();
                }
            }
            else
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
        }
        private void TouchStep_StopTouchXCP()
        {
            GSystem.DHSModel.SetTouchStepExit(Channel, true);
            _tickStepInterval.Reset();
            NextTouchStep();
        }
        private void TouchStep_MoveLoadStart()
        {
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            GSystem.MiPLC.SetMoveLoadStart(Channel, true);
            NextTouchStep();
        }
        private void TouchStep_MoveLoadWait()
        {
            if (GSystem.MiPLC.GetMoveLoadStart(Channel))
            {
                if (!GSystem.MiPLC.GetMoveLoadComplete(Channel))
                    return;
                GSystem.MiPLC.SetMoveLoadStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            if (_AutoMode)
            {
                NextTouchStep();
            }
            else
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
        }
        private void TouchStep_PowerOffStart()
        {
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(Channel, false);
            _enableNM = false;
            NextTouchStep();
        }
        private void TouchStep_PowerOffWait()
        {
            if (!GSystem.DedicatedCTRL.GetCommandActivePowerOn(Channel))
            {
                if (GSystem.DedicatedCTRL.GetCompleteActivePowerOn(Channel))
                    return;
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            if (_AutoMode)
            {
                NextTouchStep();
            }
            else
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
        }
        private void TouchStep_UnloadingStart()
        {
            GSystem.MiPLC.SetUnloadingStart(Channel, true);
            NextTouchStep();
        }
        private void TouchStep_UnloadingWait()
        {
            if (GSystem.MiPLC.GetUnloadingStart(Channel))
            {
                if (!GSystem.MiPLC.GetUnloadingComplete(Channel))
                    return;
                GSystem.MiPLC.SetUnloadingStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            if (_AutoMode)
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
            else
            {
                _autoTouchExit = true;
                SetTouchStep(TouchStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_touchStep}");
            }
        }

        private async void buttonTouchStart_Click(object sender, EventArgs e)
        {
            if (isTouchFirstExecute)
            {
                timerTouchFirst.Interval = 1000;
                timerTouchFirst.Start();
            }
            //buttonStartTouch.Enabled = false;
            //buttonStartCancel.Enabled = false;
            //buttonStopCancel.Enabled = false;
            ResetTouch();
            _AutoMode = true;
            SetTouchStep(TouchStep.LoadingStart);
            await Task.Run(() => AutoTouchTest());
        }

        private async void buttonCancelStart_Click(object sender, EventArgs e)
        {
            if (isCancelFirstExecute)
            {
                timerCancelFirst.Interval = 1000;
                timerCancelFirst.Start();
            }
            //buttonStartCancel.Enabled = false;
            //buttonStartTouch.Enabled = false;
            //buttonStopTouch.Enabled = false;
            ResetCancel();
            _AutoMode = true;
            SetCancelStep(CancelStep.LoadingStart);
            await Task.Run(() => AutoCancelTest());
        }







        public enum CancelStep
        {
            Standby,
            Prepare,
            LoadingStart,
            LoadingWait,
            PowerOnStart,
            PowerOnWait,
            MoveCancelStart,
            MoveCancelWait,
            StartCancelXCP,
            CancelUpStart,
            CancelUpWait,
            CancelDownStart,
            CancelDownWait,
            StopCancelXCP,
            MoveLoadStart,
            MoveLoadWait,
            PowerOffStart,
            PowerOffWait,
            UnloadingStart,
            UnloadingWait,
            Count
        }

        CancelStep _cancelStep = CancelStep.Standby;
        bool _autoCancelExit = false;
        public CancelStep GetCancelStep() { return _cancelStep; }
        public void SetCancelStep(CancelStep runStep) { _cancelStep = runStep; }
        public void NextCancelStep() { ++_cancelStep; }

        public async Task AutoCancelTest()
        {
            await Task.Delay(10);
            GSystem.TraceMessage($"AutoCancelTest start...");
            _cancelStep = CancelStep.Prepare;
            _autoCancelExit = false;
            while (!_autoCancelExit)
            {
                await Task.Delay(10);
                switch (_cancelStep)
                {
                    case CancelStep.Standby          : CancelStep_Standby         (); break;
                    case CancelStep.Prepare          : CancelStep_Prepare         (); break;
                    case CancelStep.LoadingStart     : CancelStep_LoadingStart    (); break;
                    case CancelStep.LoadingWait      : CancelStep_LoadingWait     (); break;
                    case CancelStep.PowerOnStart     : CancelStep_PowerOnStart    (); break;
                    case CancelStep.PowerOnWait      : CancelStep_PowerOnWait     (); break;
                    case CancelStep.MoveCancelStart  : CancelStep_MoveCancelStart (); break;
                    case CancelStep.MoveCancelWait   : CancelStep_MoveCancelWait  (); break;
                    case CancelStep.StartCancelXCP   : CancelStep_StartCancelXCP  (); break;
                    case CancelStep.CancelUpStart    : CancelStep_CancelUpStart   (); break;
                    case CancelStep.CancelUpWait     : CancelStep_CancelUpWait    (); break;
                    case CancelStep.CancelDownStart  : CancelStep_CancelDownStart (); break;
                    case CancelStep.CancelDownWait   : CancelStep_CancelDownWait  (); break;
                    case CancelStep.StopCancelXCP    : CancelStep_StopCancelXCP   (); break;
                    case CancelStep.MoveLoadStart    : CancelStep_MoveLoadStart   (); break;
                    case CancelStep.MoveLoadWait     : CancelStep_MoveLoadWait    (); break;
                    case CancelStep.PowerOffStart    : CancelStep_PowerOffStart   (); break;
                    case CancelStep.PowerOffWait     : CancelStep_PowerOffWait    (); break;
                    case CancelStep.UnloadingStart   : CancelStep_UnloadingStart  (); break;
                    case CancelStep.UnloadingWait    : CancelStep_UnloadingWait   (); break;
                    default:
                        break;
                }
            }
            GSystem.TraceMessage($"AutoCancelTest terminated!");
            return;
        }
        private void CancelStep_Standby()
        {

        }
        private void CancelStep_Prepare()
        {
            NextCancelStep();
        }
        private void CancelStep_LoadingStart()
        {
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            _totalRepeatCount = Convert.ToInt16(textCancelRepeat.Text);
            _repeatCount = 0;
            GSystem.MiPLC.SetLoadingStart(Channel, true);
            NextCancelStep();
        }
        private void CancelStep_LoadingWait()
        {
            if (GSystem.MiPLC.GetLoadingStart(Channel))
            {
                if (!GSystem.MiPLC.GetLoadingComplete(Channel))
                    return;
                GSystem.MiPLC.SetLoadingStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            if (_AutoMode)
            {
                NextCancelStep();
                _tickStepInterval.Reset();
            }
            else
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
        }
        private void CancelStep_PowerOnStart()
        {
            if (_tickStepInterval.LessThan(1000))
                return;
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(Channel, true);
            NextCancelStep();
        }
        private void CancelStep_PowerOnWait()
        {
            if (GSystem.DedicatedCTRL.GetCommandActivePowerOn(Channel))
            {
                if (!GSystem.DedicatedCTRL.GetCompleteActivePowerOn(Channel))
                    return;
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            if (_AutoMode)
            {
                NextCancelStep();
                _tickStepInterval.Reset();
            }
            else
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
        }
        private void CancelStep_MoveCancelStart()
        {
            if (_tickStepInterval.LessThan(3000))
                return;
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            GSystem.MiPLC.SetMoveCancelYStart(Channel, true);
            NextCancelStep();
        }
        private void CancelStep_MoveCancelWait()
        {
            if (GSystem.MiPLC.GetMoveCancelYStart(Channel))
            {
                if (!GSystem.MiPLC.GetMoveCancelYComplete(Channel))
                    return;
                GSystem.MiPLC.SetMoveCancelYStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            if (_AutoMode)
            {
                NextCancelStep();
            }
            else
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
        }
        private void CancelStep_StartCancelXCP()
        {
            Task.Run(async () => await GSystem.DHSModel.GetCancelAsync(Channel));
            _tickStepInterval.Reset();
            NextCancelStep();
        }
        private void CancelStep_CancelUpStart()
        {
            if (_tickStepInterval.LessThan(2000))
                return;
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            GSystem.MiPLC.SetCancelZUpStart(Channel, true);
            NextCancelStep();
        }
        private void CancelStep_CancelUpWait()
        {
            if (GSystem.MiPLC.GetCancelZUpStart(Channel))
            {
                if (!GSystem.MiPLC.GetCancelZUpComplete(Channel))
                    return;
                GSystem.MiPLC.SetCancelZUpStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            if (_AutoMode)
            {
                _tickStepInterval.Reset();
                NextCancelStep();
            }
            else
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
        }
        private void CancelStep_CancelDownStart()
        {
            if (_tickStepInterval.MoreThan(1000))
            {
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
                GSystem.MiPLC.SetCancelZDownStart(Channel, true);
                NextCancelStep();
            }
        }
        private void CancelStep_CancelDownWait()
        {
            if (GSystem.MiPLC.GetCancelZDownStart(Channel))
            {
                if (!GSystem.MiPLC.GetCancelZDownComplete(Channel))
                    return;
                GSystem.MiPLC.SetCancelZDownStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            if (_AutoMode)
            {
                if (++_repeatCount < _totalRepeatCount)
                {
                    SetCancelStep(CancelStep.CancelUpStart);
                }
                else
                {
                    NextCancelStep();
                }
            }
            else
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
        }
        private void CancelStep_StopCancelXCP()
        {
            GSystem.DHSModel.SetCancelStepExit(Channel, true);
            _tickStepInterval.Reset();
            NextCancelStep();
        }
        private void CancelStep_MoveLoadStart()
        {
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            GSystem.MiPLC.SetMoveLoadStart(Channel, true);
            NextCancelStep();
        }
        private void CancelStep_MoveLoadWait()
        {
            if (GSystem.MiPLC.GetMoveLoadStart(Channel))
            {
                if (!GSystem.MiPLC.GetMoveLoadComplete(Channel))
                    return;
                GSystem.MiPLC.SetMoveLoadStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            if (_AutoMode)
            {
                NextCancelStep();
            }
            else
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
        }
        private void CancelStep_PowerOffStart()
        {
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(Channel, false);
            NextCancelStep();
        }
        private void CancelStep_PowerOffWait()
        {
            if (!GSystem.DedicatedCTRL.GetCommandActivePowerOn(Channel))
            {
                if (GSystem.DedicatedCTRL.GetCompleteActivePowerOn(Channel))
                    return;
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            if (_AutoMode)
            {
                NextCancelStep();
            }
            else
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
        }
        private void CancelStep_UnloadingStart()
        {
            GSystem.MiPLC.SetUnloadingStart(Channel, true);
            NextCancelStep();
        }
        private void CancelStep_UnloadingWait()
        {
            if (GSystem.MiPLC.GetUnloadingStart(Channel))
            {
                if (!GSystem.MiPLC.GetUnloadingComplete(Channel))
                    return;
                GSystem.MiPLC.SetUnloadingStart(Channel, false);
            }
            GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            if (_AutoMode)
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
            else
            {
                _autoCancelExit = true;
                SetCancelStep(CancelStep.Standby);
                GSystem.TraceMessage($"[CH.{Channel + 1}] {_cancelStep}");
            }
        }

        private async void buttonLoading_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetTouchStep(TouchStep.LoadingStart);
            await Task.Run(() => AutoTouchTest());
        }

        private async void buttonUnloading_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetTouchStep(TouchStep.UnloadingStart);
            await Task.Run(() => AutoTouchTest());
        }

        private async void buttonMoveLoad_Click(object sender, EventArgs e)
        {
            _AutoMode = false;
            SetTouchStep(TouchStep.MoveLoadStart);
            await Task.Run(() => AutoTouchTest());
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    }
}

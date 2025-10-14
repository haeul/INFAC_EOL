/*
using GSCommon;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vector.vFlash.Automation;
using vxlapi_NET;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace DHSTesterXL.Forms
{
    public partial class FormVFlash : Form
    {
        // Creates wait handle for the project
        private static readonly AutoResetEvent eventWaitHandle = new AutoResetEvent(false);

        TickTimer _tickTotalElapsed = new TickTimer();

        public FormVFlash()
        {
            InitializeComponent();
        }

        private void FormVFlash_Load(object sender, EventArgs e)
        {
            checkAutoClose.Checked = true;
        }

        private void FormVFlash_Shown(object sender, EventArgs e)
        {

        }

        private void FormVFlash_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormVFlash_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void ButtonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Flash Project (*.vflash; *.vflashpack)|*.vflash;*.vflashpack";
            ofd.InitialDirectory = GSystem.SystemData.GeneralSettings.VFlashFolder;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textFileName.Text = ofd.FileName;
            }
        }

        private async void ButtonFlash_Click(object sender, EventArgs e)
        {
            if (textFileName.Text.Trim() == "")
                return;

            string message;
            string caption;

            FileInfo fi = new FileInfo(textFileName.Text);
            if (!fi.Exists)
            {
                message = $"'{textFileName.Text}' 파일이 존재하지 않습니다. 확인하시고 다시 시도해 주십시오.";
                caption = $"Error";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }

            message = $"VFLASH 쓰기는 CH.1만 가능합니다. CH.1에 제품을 연결하세요.\n'{textFileName.Text}' 파일 쓰기를 시작하시겠습니까?";
            caption = $"VFLASH Writing";
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            buttonOpen.Enabled = false;
            buttonFlash.Enabled = false;

            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    GSystem.Logger.Info($"VFLASH ROM Write start...");
                    timerUpdate.Start();
                    var token = cts.Token;
                    Task.Run(() => PowerOnAndWakeUp(token));
                    await Task.Run(() => VFlashWriting(textFileName.Text, token));
                }
                catch (Exception ex)
                {
                    GSystem.Logger.Fatal($"VFLASH Exception: {ex.Message}");
                    message = $"{ex.Message}";
                    caption = $"Exception";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    timerUpdate.Stop();
                    cts.Cancel();
                    buttonOpen.Enabled = true;
                    buttonFlash.Enabled = true;
                    GSystem.Logger.Info($"VFLASH ROM Write stop.");

                    TickTimer tickTimer = new TickTimer();
                    tickTimer.Start();
                    GSystem.DedicatedCTRL.SetCommandActivePowerOnCh1(false);
                    GSystem.DedicatedCTRL.SetCommandTestInitCh1(true);
                    while (!GSystem.DedicatedCTRL.GetCommandTestInitCh1() || !GSystem.DedicatedCTRL.GetCompleteTestInitCh1())
                    {
                        Thread.Sleep(1);
                        if (tickTimer.MoreThan(2000))
                            break;
                    }
                    GSystem.DedicatedCTRL.SetCommandTestInitCh1(false);
                }
            }

            if (checkAutoClose.Checked)
            {
                this.Close();
            }
        }

        private async Task PowerOnAndWakeUp(CancellationToken token)
        {
            GSystem.TraceMessage("PowerOnAndWakeUp start...");
            // 전원 OFF
            GSystem.DedicatedCTRL.SetCommandActivePowerOnCh1(false);
            GSystem.DedicatedCTRL.SetCommandActivePowerOnCh1(false);
            GSystem.DedicatedCTRL.SetCommandTestInitCh1(true);
            while (!GSystem.DedicatedCTRL.GetCommandTestInitCh1() || !GSystem.DedicatedCTRL.GetCompleteTestInitCh1())
            {
                token.ThrowIfCancellationRequested();
            }
            GSystem.DedicatedCTRL.SetCommandTestInitCh1(false);
            await Task.Delay(100);
            // 전원 ON
            GSystem.DedicatedCTRL.SetCommandActivePowerOnCh1(true);
            while (!GSystem.DedicatedCTRL.GetCommandActivePowerOnCh1() || !GSystem.DedicatedCTRL.GetCompleteActivePowerOnCh1())
            {
                // 전원 ON 대기
                token.ThrowIfCancellationRequested();
            }

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000);
                GSystem.DHSModel.Send_NM(GSystem.CH1);
            }
            GSystem.TraceMessage("PowerOnAndWakeUp terminated!");
        }

        private async Task VFlashWriting(string filePathName, CancellationToken token)
        {
            TickTimer tickStepElapsed = new TickTimer();
            _tickTotalElapsed.Reset();
            OnStatusMessage("Initialize Library...");
            VFlashResult result = VFlashAPI.Initialize();
            if (result != VFlashResult.Success)
            {
                OnStatusMessage($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
                throw new Exception($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
            }

            OnStatusMessage($"Load project ... [{filePathName}]");
            tickStepElapsed.Start();
            result = VFlashAPI.LoadProject(filePathName);
            OnStatusMessage($"Loading time: {tickStepElapsed.GetTotalSeconds():F3} sec");
            if (result != VFlashResult.Success)
            {
                OnStatusMessage($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
                throw new Exception($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
            }

            OnStatusMessage("Activating Reporting ...");
            VFlashAPI.ActivateReporting(Path.Combine(Path.GetTempPath(), "DHS_ANT_Reprogramming.txt"));

            TimeSpan ts = DateTime.Now.TimeOfDay;
            uint serialNumber = (uint)((ts.Hours * 100 + ts.Minutes) * 100 + ts.Seconds);
            OnStatusMessage($"Error initializing vFlash Automation. Details: {serialNumber.ToString()}");

            result = VFlashAPI.SetCustomActionAttribute("Serial Number", serialNumber.ToString());
            result = VFlashAPI.SetCustomActionAttribute("Serial Number", "ConversionToIntegerWillFail_OldValueWillBeUsed");

            OnStatusMessage("Start reprogramming ...");
            tickStepElapsed.Reset();
            result = VFlashAPI.Start(CallbackProgress, CallbackStatus);
            if (result != VFlashResult.Success)
            {
                OnStatusMessage($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
                throw new Exception($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
            }
            // Wait until project has finished.
            eventWaitHandle.WaitOne();
            OnStatusMessage($"Reprogramming time: {tickStepElapsed.GetTotalSeconds():F3} sec");

            OnStatusMessage("Unloading project ...");
            result = VFlashAPI.UnloadProject();
            if (result != VFlashResult.Success)
            {
                OnStatusMessage($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
                throw new Exception($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
            }

            OnStatusMessage("Deactivating Reporting ...");
            VFlashAPI.DeactivateReporting();

            OnStatusMessage("Deinitializing library ...");
            result = VFlashAPI.Deinitialize();
            if (result != VFlashResult.Success)
            {
                OnStatusMessage($"Error initializing vFlash Automation. Details: {VFlashAPI.GetLastErrorMessage()}");
            }
            await Task.Delay(100);
            OnStatusMessage("Ready");
        }

        private void CallbackProgress(uint progressInPercent, uint remainingTimeInSeconds)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<uint, uint>(CallbackProgress), progressInPercent, remainingTimeInSeconds);
            }
            else
            {
                UpdateProgress(progressInPercent, remainingTimeInSeconds);
            }
        }

        private void CallbackStatus(VFlashStatus flashStatus)
        {
            OnStatusMessage($"Result: {flashStatus}");
            eventWaitHandle.Set();  // signal project has finished
            if (flashStatus != VFlashStatus.Success)
            {
                string message = $"Result: {flashStatus}";
                string caption = $"Error";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void UpdateProgress(uint progressInPercent, uint remainingTimeInSeconds)
        {
            progressBar.Value = (int)progressInPercent;
            labelPercent.Text = $"{progressInPercent} %";
            labelRemainTime.Text = $"{remainingTimeInSeconds} sec";
        }

        private void OnStatusMessage(string message)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<string>(SetStatusMessage), message);
            else
                SetStatusMessage(message);
        }
        private void SetStatusMessage(string message)
        {
            labelMessage.Text = message;
            GSystem.Logger.Info(message);
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            labelElapsedTime.Text = $"{_tickTotalElapsed.GetTotalSeconds():F0} sec";
        }
    }
}
*/
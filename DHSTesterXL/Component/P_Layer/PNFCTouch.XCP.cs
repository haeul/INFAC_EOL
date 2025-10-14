using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vxlapi_NET;
using static vxlapi_NET.XLClass;
using static vxlapi_NET.XLDefine;
using static DHSTesterXL.GSystem;
using System.Runtime.InteropServices;
using GSCommon;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DHSTesterXL
{
    public partial class PNFCTouch : IDHSModel
    {
        // Touch step
        XcpTouchStep[] _xcpTtouchStep = new XcpTouchStep[] { XcpTouchStep.Standby, XcpTouchStep.Standby };
        TickTimer[] _tickXcpTouchNM = new TickTimer[] { new TickTimer(), new TickTimer() };
        TickTimer[] _tickXcpTouchTimeout = new TickTimer[] { new TickTimer(), new TickTimer() };
        TickTimer[] _tickXcpTouchInterval = new TickTimer[] { new TickTimer(), new TickTimer() };
        List<int>[] XcpTouchIntervalList = new List<int>[] { new List<int>(), new List<int>() };
        int[] _retryTouch = new int[] { 0, 0 };

        public int[] CurrentTouchFastMutual = new int[] { 0, 0 };
        public int[] CurrentTouchFastSelf = new int[] { 0, 0 };
        public int[] CurrentTouchSlowSelf = new int[] { 0, 0 };
        public float[] CurrentTouchComboRate = new float[] { 0, 0 };
        public int[] CurrentTouchState = new int[] { 0, 0 };
        public bool[] TouchStepExit = new bool[] { false, false };

        // Cancel step
        XcpCancelStep[] _xcpCancelStep = new XcpCancelStep[] { XcpCancelStep.Standby, XcpCancelStep.Standby };
        TickTimer[] _tickXcpCancelNM = new TickTimer[] { new TickTimer(), new TickTimer() };
        TickTimer[] _tickXcpCancelTimeout = new TickTimer[] { new TickTimer(), new TickTimer() };
        TickTimer[] _tickXcpCancelInterval = new TickTimer[] { new TickTimer(), new TickTimer() };
        List<int>[] XcpCancelIntervalList = new List<int>[] { new List<int>(), new List<int>() };
        int[] _retryCancel = new int[] { 0, 0 };

        public int[] CurrentCancelFastSelf = new int[] { 0, 0 };
        public int[] CurrentCancelSlowSelf = new int[] { 0, 0 };
        public int[] CurrentCancelState = new int[] { 0, 0 };
        public bool[] CancelStepExit = new bool[] { false, false };


        // -----------------------------------------------------------------------------------------------
        // TOUCH STEP
        // -----------------------------------------------------------------------------------------------
        public XcpTouchStep GetTouchStep(int channel)
        {
            return _xcpTtouchStep[channel];
        }

        public void SetTouchStep(int channel, XcpTouchStep TouchStep)
        {
            _xcpTtouchStep[channel] = TouchStep;
        }

        public XcpTouchStep NextTouchStep(int channel)
        {
            _xcpTtouchStep[channel] = (XcpTouchStep)((int)_xcpTtouchStep[channel] + 1);
            return _xcpTtouchStep[channel];
        }

        public Task SetTouchStepExit(int channel, bool exit)
        {
            TouchStepExit[channel] = exit;
            return Task.CompletedTask;
        }

        public async Task<bool> GetTouchAsync(int channel)
        {
            bool result = false;

            // 수신스레드를 종료하지 않으면 통신이 잘 안된다. 아직은 원인을 모르겠다. 좀 더 연구가 필요하다.
            StopCanReceiveThread(channel);

            await Task.Delay(10);

            _tickXcpElapse[channel].Reset();
            GSystem.CapaData[channel].Info($"//--- {DateTime.Now:yyyy-MM-dd HH:mm:sss.fff} CH.{channel + 1} [{ProductSettings.ProductInfo.PartNo}] Start touch capacitance data ---");

            _xcpTtouchStep[channel] = XcpTouchStep.Prepare;
            _retryTouch[channel] = 0;
            TouchStepExit[channel] = false;
            while (!TouchStepExit[channel])
            {
                if (_tickXcpTouchNM[channel].MoreThan(200))
                {
                    _tickXcpTouchNM[channel].Reset();
                    Send_NM(channel);
                }
                switch (_xcpTtouchStep[channel])
                {
                    case XcpTouchStep.Standby               : XcpTouchStep_Standby               (channel); break;
                    case XcpTouchStep.Prepare               : XcpTouchStep_Prepare               (channel); break;
                    case XcpTouchStep.ConnectSend           : XcpTouchStep_ConnectSend           (channel); break;
                    case XcpTouchStep.ConnectWait           : XcpTouchStep_ConnectWait           (channel); break;
                    case XcpTouchStep.TouchFastMutualSend   : XcpTouchStep_TouchFastMutualSend   (channel); break;
                    case XcpTouchStep.TouchFastMutualWait   : XcpTouchStep_TouchFastMutualWait   (channel); break;
                    case XcpTouchStep.TouchFastMutualUpload : XcpTouchStep_TouchFastMutualUpload (channel); break;
                    case XcpTouchStep.TouchFastSelfSend     : XcpTouchStep_TouchFastSelfSend     (channel); break;
                    case XcpTouchStep.TouchFastSelfWait     : XcpTouchStep_TouchFastSelfWait     (channel); break;
                    case XcpTouchStep.TouchFastSelfUpload   : XcpTouchStep_TouchFastSelfUpload   (channel); break;
                    case XcpTouchStep.TouchSlowSelfSend     : XcpTouchStep_TouchSlowSelfSend     (channel); break;
                    case XcpTouchStep.TouchSlowSelfWait     : XcpTouchStep_TouchSlowSelfWait     (channel); break;
                    case XcpTouchStep.TouchSlowSelfUpload   : XcpTouchStep_TouchSlowSelfUpload   (channel); break;
                    case XcpTouchStep.TouchComboRateSend    : XcpTouchStep_TouchComboRateSend    (channel); break;
                    case XcpTouchStep.TouchComboRateWait    : XcpTouchStep_TouchComboRateWait    (channel); break;
                    case XcpTouchStep.TouchComboRateUpload  : XcpTouchStep_TouchComboRateUpload  (channel); break;
                    case XcpTouchStep.TouchStateSend        : XcpTouchStep_TouchStateSend        (channel); break;
                    case XcpTouchStep.TouchStateWait        : XcpTouchStep_TouchStateWait        (channel); break;
                    case XcpTouchStep.TouchStateUpload      : XcpTouchStep_TouchStateUpload      (channel); break;
                    case XcpTouchStep.DisconnectSend        : XcpTouchStep_DisconnectSend        (channel); break;
                    case XcpTouchStep.DisconnectWait        : XcpTouchStep_DisconnectWait        (channel); break;
                    case XcpTouchStep.Complete              : XcpTouchStep_Complete              (channel); break;
                    default:
                        break;
                }
            }
            Send_XCPDisconnect(channel);
            GSystem.TraceMessage($"[CH.{channel + 1}] XCP Disconnect.");

            GSystem.CapaData[channel].Info($"//--- {DateTime.Now:yyyy-MM-dd HH:mm:sss.fff} CH.{channel + 1} [{ProductSettings.ProductInfo.PartNo}] Stop touch capacitance data ---");

            // 수신 스레드를 다시 살린다.
            StartCanReceiveThread(channel);

            return result;
        }

        private void XcpTouchStep_Standby(int channel)
        {

        }
        private void XcpTouchStep_Prepare(int channel)
        {
            if (!GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel) && !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
            {
                GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
            }
            NextTouchStep(channel);
        }
        private void XcpTouchStep_ConnectSend(int channel)
        {
            if (!GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel) && !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                return;
            Send_XCPConnect(channel);
            _tickXcpTouchTimeout[channel].Reset();
            NextTouchStep(channel);
        }
        private void XcpTouchStep_ConnectWait(int channel)
        {
            if (_tickXcpTouchTimeout[channel].LessThan(300))
            {
                xl_event receivedEvent = new xl_event();
                XL_Status xlStatus = XL_Status.XL_SUCCESS;
                if ((xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent)) == XL_Status.XL_SUCCESS)
                {
                    uint canID = receivedEvent.tagData.can_Msg.id;
                    if (canID == ProductSettings.XcpResID)
                    {
                        if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                        {
                            GSystem.TraceMessage($"[CH.{channel + 1}] XCP Connected.");
                            NextTouchStep(channel);
                            _tickXcpTouchInterval[channel].Reset();
                            _retryTouch[channel] = 0;
                        }
                    }
                }
            }
            else
            {
                // retry
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.ConnectSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchFastMutualSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchFastMutual.Address, 16);
            Send_SetMTA(channel, xcpAddr, true, "(TOUCH_FAST_MUTUAL)");
            NextTouchStep(channel);
        }
        private void XcpTouchStep_TouchFastMutualWait(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:SET_MTA");
                                Send_UploadCapa(channel, 2, true);
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchFastMutualSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchFastMutualUpload(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                CurrentTouchFastMutual[channel]  = (int)(receivedEvent.tagData.can_Msg.data[4] << 8);
                                CurrentTouchFastMutual[channel] += (int)(receivedEvent.tagData.can_Msg.data[3]);
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.can_Msg.data[3]:X2}h {receivedEvent.tagData.can_Msg.data[4]:X2}h ({CurrentTouchFastMutual[channel]})");
                                _retryTouch[channel] = 0;
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchFastMutualSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchFastSelfSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchFastSelf.Address, 16);
            Send_SetMTA(channel, xcpAddr, true, "(TOUCH_FAST_SELF)");
            NextTouchStep(channel);
        }
        private void XcpTouchStep_TouchFastSelfWait(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:SET_MTA");
                                Send_UploadCapa(channel, 2, true);
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchFastSelfSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchFastSelfUpload(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                CurrentTouchFastSelf[channel]  = (int)(receivedEvent.tagData.can_Msg.data[4] << 8);
                                CurrentTouchFastSelf[channel] += (int)(receivedEvent.tagData.can_Msg.data[3]);
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.can_Msg.data[3]:X2}h {receivedEvent.tagData.can_Msg.data[4]:X2}h ({CurrentTouchFastSelf[channel]})");
                                _retryTouch[channel] = 0;
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchFastSelfSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchSlowSelfSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchSlowSelf.Address, 16);
            Send_SetMTA(channel, xcpAddr, true, "(TOUCH_SLOW_SELF)");
            NextTouchStep(channel);
        }
        private void XcpTouchStep_TouchSlowSelfWait(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:SET_MTA");
                                Send_UploadCapa(channel, 2, true);
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchSlowSelfSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchSlowSelfUpload(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                CurrentTouchSlowSelf[channel]  = (int)(receivedEvent.tagData.can_Msg.data[4] << 8);
                                CurrentTouchSlowSelf[channel] += (int)(receivedEvent.tagData.can_Msg.data[3]);
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.can_Msg.data[3]:X2}h {receivedEvent.tagData.can_Msg.data[4]:X2}h ({CurrentTouchSlowSelf[channel]})");
                                _retryTouch[channel] = 0;
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchSlowSelfSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchComboRateSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchComboRate.Address, 16);
            Send_SetMTA(channel, xcpAddr, true, "(TOUCH_COMBORATE)");
            NextTouchStep(channel);
        }
        private void XcpTouchStep_TouchComboRateWait(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:SET_MTA");
                                Send_UploadCapa(channel, 4, true);
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchComboRateSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchComboRateUpload(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                byte[] byteCombo = new byte[4];
                                byteCombo[0] = receivedEvent.tagData.can_Msg.data[3];
                                byteCombo[1] = receivedEvent.tagData.can_Msg.data[4];
                                byteCombo[2] = receivedEvent.tagData.can_Msg.data[5];
                                byteCombo[3] = receivedEvent.tagData.can_Msg.data[6];
                                float comboRate = BitConverter.ToSingle(byteCombo, 0);
                                CurrentTouchComboRate[channel] = comboRate;
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={byteCombo[0]:X2}h {byteCombo[1]:X2}h {byteCombo[2]:X2}h {byteCombo[3]:X2}h ({CurrentTouchComboRate[channel]:F2})");
                                _retryTouch[channel] = 0;
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchComboRateSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchStateSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchState.Address, 16);
            Send_SetMTA(channel, xcpAddr, true, "(TOUCH_STATE)");
            NextTouchStep(channel);
        }
        private void XcpTouchStep_TouchStateWait(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:SET_MTA");
                                Send_UploadCapa(channel, 1, true);
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchStateSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_TouchStateUpload(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                CurrentTouchState[channel] = (int)(receivedEvent.tagData.can_Msg.data[3]);
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.can_Msg.data[3]:X2}h ({CurrentTouchState[channel]})");

                                int elapsedTime = (int)_tickXcpTouchInterval[channel].GetElapsedMilliseconds();
                                _tickXcpTouchInterval[channel].Reset();
                                XcpTouchIntervalList[channel].Add(elapsedTime);
                                if (XcpTouchIntervalList[channel].Count > MaxAverageCount)
                                {
                                    XcpTouchIntervalList[channel].RemoveAt(0);
                                }
                                int avgElapsedTime = (int)(XcpTouchIntervalList[channel].Average() + 0.5);
                                OnUpdateTouchXcpData(channel, CurrentTouchFastMutual[channel], CurrentTouchFastSelf[channel],
                                    CurrentTouchSlowSelf[channel], CurrentTouchComboRate[channel], CurrentTouchState[channel], avgElapsedTime);

                                //NextTouchStep();
                                SetTouchStep(channel, XcpTouchStep.TouchFastMutualSend);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryTouch[channel] < MaxRetryCount)
                    SetTouchStep(channel, XcpTouchStep.TouchStateSend);
                else
                    SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            }
        }
        private void XcpTouchStep_DisconnectSend(int channel)
        {
            Send_XCPDisconnect(channel);
            TouchStepExit[channel] = true;
            GSystem.TraceMessage($"[CH.{channel + 1}] XCP Disconnect.");
            NextTouchStep(channel);
        }
        private void XcpTouchStep_DisconnectWait(int channel)
        {
            NextTouchStep(channel);
        }
        private void XcpTouchStep_Complete(int channel)
        {
            SetTouchStep(channel, XcpTouchStep.Standby);
        }


        // -----------------------------------------------------------------------------------------------
        // CANCEL STEP
        // -----------------------------------------------------------------------------------------------
        public XcpCancelStep GetCancelStep(int channel)
        {
            return _xcpCancelStep[channel];
        }

        public void SetCancelStep(int channel, XcpCancelStep CancelStep)
        {
            _xcpCancelStep[channel] = CancelStep;
        }

        public XcpCancelStep NextCancelStep(int channel)
        {
            _xcpCancelStep[channel] = (XcpCancelStep)((int)_xcpCancelStep[channel] + 1);
            return _xcpCancelStep[channel];
        }

        public Task SetCancelStepExit(int channel, bool exit)
        {
            CancelStepExit[channel] = exit;
            return Task.CompletedTask;
        }

        public async Task<bool> GetCancelAsync(int channel)
        {
            bool result = false;

            // 수신스레드를 종료하지 않으면 통신이 잘 안된다. 아직은 원인을 모르겠다. 좀 더 연구가 필요하다.
            StopCanReceiveThread(channel);

            await Task.Delay(10);

            _tickXcpElapse[channel].Reset();
            GSystem.CapaData[channel].Info($"//--- {DateTime.Now:yyyy-MM-dd HH:mm:sss.fff} CH.{channel + 1} [{ProductSettings.ProductInfo.PartNo}] Start cancel capacitance data ---");

            _xcpCancelStep[channel] = XcpCancelStep.Prepare;
            _retryCancel[channel] = 0;
            CancelStepExit[channel] = false;
            while (!CancelStepExit[channel])
            {
                if (_tickXcpCancelNM[channel].MoreThan(200))
                {
                    _tickXcpCancelNM[channel].Reset();
                    Send_NM(channel);
                }
                switch (_xcpCancelStep[channel])
                {
                    case XcpCancelStep.Standby              : XcpCancelStep_Standby              (channel); break;
                    case XcpCancelStep.Prepare              : XcpCancelStep_Prepare              (channel); break;
                    case XcpCancelStep.ConnectSend          : XcpCancelStep_ConnectSend          (channel); break;
                    case XcpCancelStep.ConnectWait          : XcpCancelStep_ConnectWait          (channel); break;
                    case XcpCancelStep.CancelFastSelfSend   : XcpCancelStep_CancelFastSelfSend   (channel); break;
                    case XcpCancelStep.CancelFastSelfWait   : XcpCancelStep_CancelFastSelfWait   (channel); break;
                    case XcpCancelStep.CancelFastSelfUpload : XcpCancelStep_CancelFastSelfUpload (channel); break;
                    case XcpCancelStep.CancelSlowSelfSend   : XcpCancelStep_CancelSlowSelfSend   (channel); break;
                    case XcpCancelStep.CancelSlowSelfWait   : XcpCancelStep_CancelSlowSelfWait   (channel); break;
                    case XcpCancelStep.CancelSlowSelfUpload : XcpCancelStep_CancelSlowSelfUpload (channel); break;
                    case XcpCancelStep.CancelStateSend      : XcpCancelStep_CancelStateSend      (channel); break;
                    case XcpCancelStep.CancelStateWait      : XcpCancelStep_CancelStateWait      (channel); break;
                    case XcpCancelStep.CancelStateUpload    : XcpCancelStep_CancelStateUpload    (channel); break;
                    case XcpCancelStep.DisconnectSend       : XcpCancelStep_DisconnectSend       (channel); break;
                    case XcpCancelStep.DisconnectWait       : XcpCancelStep_DisconnectWait       (channel); break;
                    case XcpCancelStep.Complete             : XcpCancelStep_Complete             (channel); break;
                    default:
                        break;
                }
            }
            Send_XCPDisconnect(channel);
            GSystem.TraceMessage($"[CH.{channel + 1}] XCP Disconnect.");

            GSystem.CapaData[channel].Info($"//--- {DateTime.Now:yyyy-MM-dd HH:mm:sss.fff} CH.{channel + 1} [{ProductSettings.ProductInfo.PartNo}] Stop cancel capacitance data ---");

            // 수신 스레드를 다시 살린다.
            StartCanReceiveThread(channel);

            return result;
        }

        private void XcpCancelStep_Standby(int channel)
        {

        }
        private void XcpCancelStep_Prepare(int channel)
        {
            if (!GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel) && !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
            {
                GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
            }
            NextCancelStep(channel);
        }
        private void XcpCancelStep_ConnectSend(int channel)
        {
            if (!GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel) && !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                return;
            Send_XCPConnect(channel);
            _tickXcpCancelTimeout[channel].Reset();
            NextCancelStep(channel);
        }
        private void XcpCancelStep_ConnectWait(int channel)
        {
            if (_tickXcpCancelTimeout[channel].LessThan(300))
            {
                xl_event receivedEvent = new xl_event();
                XL_Status xlStatus = XL_Status.XL_SUCCESS;
                if ((xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent)) == XL_Status.XL_SUCCESS)
                {
                    uint canID = receivedEvent.tagData.can_Msg.id;
                    if (canID == ProductSettings.XcpResID)
                    {
                        if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                        {
                            GSystem.TraceMessage($"[CH.{channel + 1}] XCP Connected.");
                            NextCancelStep(channel);
                            _tickXcpCancelInterval[channel].Reset();
                            _retryCancel[channel] = 0;
                        }
                    }
                }
            }
            else
            {
                // retry
                if (++_retryCancel[channel] < MaxRetryCount)
                    SetCancelStep(channel, XcpCancelStep.ConnectSend);
                else
                    SetCancelStep(channel, XcpCancelStep.DisconnectSend);
            }
        }
        private void XcpCancelStep_CancelFastSelfSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.CancelFastSelf.Address, 16);
            Send_SetMTA(channel, xcpAddr, true, "(CANCEL_FAST_SELF)");
            NextCancelStep(channel);
        }
        private void XcpCancelStep_CancelFastSelfWait(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:SET_MTA");
                                Send_UploadCapa(channel, 2, true);
                                NextCancelStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryCancel[channel] < MaxRetryCount)
                    SetCancelStep(channel, XcpCancelStep.CancelFastSelfSend);
                else
                    SetCancelStep(channel, XcpCancelStep.DisconnectSend);
            }
        }
        private void XcpCancelStep_CancelFastSelfUpload(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                CurrentCancelFastSelf[channel]  = (int)(receivedEvent.tagData.can_Msg.data[4] << 8);
                                CurrentCancelFastSelf[channel] += (int)(receivedEvent.tagData.can_Msg.data[3]);
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.can_Msg.data[3]:X2}h {receivedEvent.tagData.can_Msg.data[4]:X2}h ({CurrentCancelFastSelf[channel]})");
                                _retryCancel[channel] = 0;
                                NextCancelStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryCancel[channel] < MaxRetryCount)
                    SetCancelStep(channel, XcpCancelStep.CancelFastSelfSend);
                else
                    SetCancelStep(channel, XcpCancelStep.DisconnectSend);
            }
        }
        private void XcpCancelStep_CancelSlowSelfSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.CancelSlowSelf.Address, 16);
            Send_SetMTA(channel, xcpAddr, true, "(CANCEL_SLOW_SELF)");
            NextCancelStep(channel);
        }
        private void XcpCancelStep_CancelSlowSelfWait(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:SET_MTA");
                                Send_UploadCapa(channel, 2, true);
                                NextCancelStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryCancel[channel] < MaxRetryCount)
                    SetCancelStep(channel, XcpCancelStep.CancelSlowSelfSend);
                else
                    SetCancelStep(channel, XcpCancelStep.DisconnectSend);
            }
        }
        private void XcpCancelStep_CancelSlowSelfUpload(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                CurrentCancelSlowSelf[channel]  = (int)(receivedEvent.tagData.can_Msg.data[4] << 8);
                                CurrentCancelSlowSelf[channel] += (int)(receivedEvent.tagData.can_Msg.data[3]);
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.can_Msg.data[3]:X2}h {receivedEvent.tagData.can_Msg.data[4]:X2}h ({CurrentCancelSlowSelf[channel]})");
                                _retryCancel[channel] = 0;
                                NextCancelStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryCancel[channel] < MaxRetryCount)
                    SetCancelStep(channel, XcpCancelStep.CancelSlowSelfSend);
                else
                    SetCancelStep(channel, XcpCancelStep.DisconnectSend);
            }
        }
        private void XcpCancelStep_CancelStateSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.CancelState.Address, 16);
            Send_SetMTA(channel, xcpAddr, true, "(CANCEL_STATE)");
            NextCancelStep(channel);
        }
        private void XcpCancelStep_CancelStateWait(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:SET_MTA");
                                Send_UploadCapa(channel, 1, true);
                                NextCancelStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryCancel[channel] < MaxRetryCount)
                    SetCancelStep(channel, XcpCancelStep.CancelStateSend);
                else
                    SetCancelStep(channel, XcpCancelStep.DisconnectSend);
            }
        }
        private void XcpCancelStep_CancelStateUpload(int channel)
        {
            xl_event receivedEvent = new xl_event();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = receivedEvent.tagData.can_Msg.id;
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.can_Msg.data[0] == 0xFF)
                            {
                                CurrentCancelState[channel] = receivedEvent.tagData.can_Msg.data[3];
                                GSystem.CapaData[channel].Info($"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.can_Msg.data[3]:X2}h ({CurrentCancelState[channel]})");

                                int intervalTime = (int)_tickXcpCancelInterval[channel].GetElapsedMilliseconds();
                                _tickXcpCancelInterval[channel].Reset();
                                XcpCancelIntervalList[channel].Add(intervalTime);
                                if (XcpCancelIntervalList[channel].Count > MaxAverageCount)
                                {
                                    XcpCancelIntervalList[channel].RemoveAt(0);
                                }
                                int avgIntervalTime = (int)(XcpCancelIntervalList[channel].Average() + 0.5);
                                OnUpdateCancelXcpData(channel, CurrentCancelFastSelf[channel], CurrentCancelSlowSelf[channel], CurrentCancelState[channel], avgIntervalTime);

                                //NextCancelStep();
                                SetCancelStep(channel, XcpCancelStep.CancelFastSelfSend); // 반복
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // timeout
                if (++_retryCancel[channel] < MaxRetryCount)
                    SetCancelStep(channel, XcpCancelStep.CancelStateSend);
                else
                    SetCancelStep(channel, XcpCancelStep.DisconnectSend);
            }
        }
        private void XcpCancelStep_DisconnectSend(int channel)
        {
            Send_XCPDisconnect(channel);
            CancelStepExit[channel] = true;
            GSystem.TraceMessage($"[CH.{channel + 1}] XCP Disconnect.");
            NextCancelStep(channel);
        }
        private void XcpCancelStep_DisconnectWait(int channel)
        {
            NextCancelStep(channel);
        }
        private void XcpCancelStep_Complete(int channel)
        {
            SetCancelStep(channel, XcpCancelStep.Standby);
        }
    }
}

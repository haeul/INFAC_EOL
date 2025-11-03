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
    public class XcpTouchData
    {
        public ulong Timestamp { get; set; }
        public int FastMutual { get; set; }
        public int Fastself { get; set; }
        public int Slowself { get; set; }
        public float Comborate { get; set; }
        public int State { get; set; }
        public string ZState { get; set; }

        public XcpTouchData()
        {

        }

        public XcpTouchData(ulong timestamp, int fastMutual, int fastself, int slowself, float comborate, int state, string zState)
        {
            Timestamp = timestamp;
            FastMutual = fastMutual;
            Fastself = fastself;
            Slowself = slowself;
            Comborate = comborate;
            State = state;
            ZState = zState;
        }
    }
    public class XcpCancelData
    {
        public ulong Timestamp { get; set; }
        public int Fastself { get; set; }
        public int Slowself { get; set; }
        public int State { get; set; }
        public string ZState { get; set; }

        public XcpCancelData()
        {

        }

        public XcpCancelData(ulong timestamp, int fastself, int slowself, int state, string zState)
        {
            Timestamp = timestamp;
            Fastself = fastself;
            Slowself = slowself;
            State = state;
            ZState = zState;
        }
    }

    public partial class PNFCTouchFD : IDHSModel
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
        public bool[] TouchLogging = new bool[] { false, false };

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
        public bool[] CancelLogging = new bool[] { false, false };

        private GCsvFile[] _capaDataFile = new GCsvFile[2] { new GCsvFile(), new GCsvFile() };

        public List<XcpTouchData> _xcpTouchDatas = new List<XcpTouchData>();
        public List<XcpCancelData> _xcpCancelDatas = new List<XcpCancelData>();

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
            //GSystem.CapaData[channel]?.Info($"//--- {DateTime.Now:yyyy-MM-dd HH:mm:sss.fff} CH.{channel + 1} [{ProductSettings.ProductInfo.PartNo}] Start touch capacitance data ---");

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
                    case XcpTouchStep.TouchFastMutualUpload : XcpTouchStep_TouchFastMutualUpload (channel); break;
                    case XcpTouchStep.TouchFastSelfSend     : XcpTouchStep_TouchFastSelfSend     (channel); break;
                    case XcpTouchStep.TouchFastSelfUpload   : XcpTouchStep_TouchFastSelfUpload   (channel); break;
                    case XcpTouchStep.TouchSlowSelfSend     : XcpTouchStep_TouchSlowSelfSend     (channel); break;
                    case XcpTouchStep.TouchSlowSelfUpload   : XcpTouchStep_TouchSlowSelfUpload   (channel); break;
                    case XcpTouchStep.TouchComboRateSend    : XcpTouchStep_TouchComboRateSend    (channel); break;
                    case XcpTouchStep.TouchComboRateUpload  : XcpTouchStep_TouchComboRateUpload  (channel); break;
                    case XcpTouchStep.TouchStateSend        : XcpTouchStep_TouchStateSend        (channel); break;
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

            //GSystem.CapaData[channel]?.Info($"//--- {DateTime.Now:yyyy-MM-dd HH:mm:sss.fff} CH.{channel + 1} [{ProductSettings.ProductInfo.PartNo}] Stop touch capacitance data ---");

            // 파일 열기
            string capaDataFileName = $"{DateTime.Now:yyMMdd-HHmmss}_ch{channel + 1}_{ProductSettings.ProductInfo.PartNo}_touch.csv";
            string capaDataFilePath = $"D:\\INFAC\\DHS_EOL_Data\\DATA_CAP\\{ProductSettings.ProductInfo.PartNo}\\{DateTime.Now:yyyy}\\{DateTime.Now:MM}\\{DateTime.Now:dd}";
            _capaDataFile[channel].Open(capaDataFileName, capaDataFilePath);
            string capaDataTitles = "Time,Interval,Fast Mutual,Fast Self,";
            if (GSystem.UseXcpTouchSlowSelf) capaDataTitles += "Slow Self,";
            if (GSystem.UseXcpTouchComboRate) capaDataTitles += "Combo Rate,";
            capaDataTitles += "Touch State,Touch Z Down\r\n";
            _capaDataFile[channel].Write(capaDataTitles);

            // 파일 저장
            if (_xcpTouchDatas != null && _xcpTouchDatas.Count > 0)
            {
                _xcpTouchDatas.RemoveAt(0); // 맨 앞 데이터 버리기. 첫 번째 데이터의 인터벌 시간이 상대적으로 길기 때문에 버린다.

                // 총 소요 시간 계산
                double startTimestamp = (double)_xcpTouchDatas[0].Timestamp / 1000000000.0;
                double stopTimestamp = (double)_xcpTouchDatas[_xcpTouchDatas.Count - 1].Timestamp / 1000000000.0;
                double totalTimestamp = stopTimestamp - startTimestamp;

                List<double> xcpIntervals = new List<double>();
                for (int i = 1; i < _xcpTouchDatas.Count; i++)
                {
                    ulong xcpInterval = _xcpTouchDatas[i].Timestamp - _xcpTouchDatas[i - 1].Timestamp;
                    xcpIntervals.Add((double)xcpInterval / 1000000000.0);
                }

                // 주기 평균
                double averageInterval = xcpIntervals.Average();
                GSystem.TraceMessage($"startTimestamp = {startTimestamp:F6}, stopTimestamp = {stopTimestamp:F6}, totalTimestamp = {totalTimestamp:F6}, averageInterval = {averageInterval:F6}");

                double convertedTimestamp = 0;
                double convertedMillisecond = 0;
                double convertedInterval = 0;
                double prevTimestamp = 0;

                foreach (var touchData in _xcpTouchDatas)
                {
                    StringBuilder sb = new StringBuilder();
                    //sb.Append($"{touchData.Timestemp},{touchData.FastMutual},{touchData.Fastself},{touchData.Slowself},{touchData.Comborate:F2},{touchData.State},{touchData.ZState}");
                    //sb.Append($"{touchData.Timestamp},{touchData.FastMutual},{touchData.Fastself},{touchData.State},{touchData.ZState}");
                    //sb.Append($"{touchData.Timestamp},{convertedTimestamp:F6},{convertedMillisecond:F3},{convertedInterval:F3},{touchData.FastMutual},{touchData.Fastself},{touchData.State},{touchData.ZState}");
                    sb.Append($"{convertedMillisecond:F3},{convertedInterval:F3},{touchData.FastMutual},{touchData.Fastself},");
                    if (GSystem.UseXcpTouchSlowSelf) sb.Append($"{touchData.Slowself},");
                    if (GSystem.UseXcpTouchComboRate) sb.Append($"{touchData.Comborate:F2},");
                    sb.Append($"{touchData.State},{touchData.ZState}");
                    sb.Append(Environment.NewLine);
                    convertedTimestamp += averageInterval;
                    convertedMillisecond = Math.Round(convertedTimestamp, 3);
                    convertedInterval = convertedMillisecond - prevTimestamp;
                    prevTimestamp = convertedMillisecond;
                    _capaDataFile[channel].Write(sb.ToString());
                }
            }

            // 파일 닫기
            _capaDataFile[channel].Close();

            // 수신 스레드를 다시 살린다.
            StartCanReceiveThread(channel);

            return result;
        }

        private void XcpTouchStep_Standby(int channel)
        {

        }
        private void XcpTouchStep_Prepare(int channel)
        {
            if (_xcpTouchDatas == null)
                _xcpTouchDatas = new List<XcpTouchData>();
            _xcpTouchDatas.Clear();
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
                XLcanRxEvent receivedEvent = new XLcanRxEvent();
                XL_Status xlStatus = XL_Status.XL_SUCCESS;
                if ((xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent)) == XLDefine.XL_Status.XL_SUCCESS)
                {
                    uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                    if (canID == ProductSettings.XcpResID)
                    {
                        if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
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
            //Send_SetMTA(channel, xcpAddr, true, "(TOUCH_FAST_MUTUAL)");
            TouchLogging[channel] = false;
            Send_ShortUpload(channel, xcpAddr, 2, TouchLogging[channel], "(TOUCH_FAST_MUTUAL)");
            //NextTouchStep(channel);
            SetTouchStep(channel, XcpTouchStep.TouchFastMutualUpload);
        }
        private void XcpTouchStep_TouchFastMutualUpload(int channel)
        {
            XLcanRxEvent receivedEvent = new XLcanRxEvent();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
                            {
                                int bitIndexLO = 1;
                                int bitIndexHI = 2;
                                CurrentTouchFastMutual[channel]  = (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexHI] << 8);
                                CurrentTouchFastMutual[channel] += (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]);
                                if (TouchLogging[channel])
                                    GSystem.CapaData[channel]?.Info($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]:X2}h {receivedEvent.tagData.canRxOkMsg.data[bitIndexHI]:X2}h ({CurrentTouchFastMutual[channel]})");
                                _retryTouch[channel] = 0;
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    // timeout
            //    if (++_retryTouch[channel] < MaxRetryCount)
            //        SetTouchStep(channel, XcpTouchStep.TouchFastMutualSend);
            //    else
            //        SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            //}
        }
        private void XcpTouchStep_TouchFastSelfSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchFastSelf.Address, 16);
            //Send_SetMTA(channel, xcpAddr, true, "(TOUCH_FAST_SELF)");
            Send_ShortUpload(channel, xcpAddr, 2, TouchLogging[channel], "(TOUCH_FAST_SELF)");
            //NextTouchStep(channel);
            SetTouchStep(channel, XcpTouchStep.TouchFastSelfUpload);
        }
        private void XcpTouchStep_TouchFastSelfUpload(int channel)
        {
            XLcanRxEvent receivedEvent = new XLcanRxEvent();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
                            {
                                int bitIndexLO = 1;
                                int bitIndexHI = 2;
                                CurrentTouchFastSelf[channel]  = (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexHI] << 8);
                                CurrentTouchFastSelf[channel] += (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]);
                                if (TouchLogging[channel])
                                    GSystem.CapaData[channel]?.Info($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]:X2}h {receivedEvent.tagData.canRxOkMsg.data[bitIndexHI]:X2}h ({CurrentTouchFastSelf[channel]})");
                                _retryTouch[channel] = 0;
                                //NextTouchStep(channel);
                                if (GSystem.UseXcpTouchSlowSelf)
                                    SetTouchStep(channel, XcpTouchStep.TouchSlowSelfSend); // interval == 10~11ms
                                else if (GSystem.UseXcpTouchComboRate)
                                    SetTouchStep(channel, XcpTouchStep.TouchComboRateSend); // interval == 8~9ms
                                else
                                    SetTouchStep(channel, XcpTouchStep.TouchStateSend); // interval == 6~7ms
                                break;
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    // timeout
            //    if (++_retryTouch[channel] < MaxRetryCount)
            //        SetTouchStep(channel, XcpTouchStep.TouchFastSelfSend);
            //    else
            //        SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            //}
        }
        private void XcpTouchStep_TouchSlowSelfSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchSlowSelf.Address, 16);
            //Send_SetMTA(channel, xcpAddr, true, "(TOUCH_SLOW_SELF)");
            Send_ShortUpload(channel, xcpAddr, 2, TouchLogging[channel], "(TOUCH_SLOW_SELF)");
            //NextTouchStep(channel);
            SetTouchStep(channel, XcpTouchStep.TouchSlowSelfUpload);
        }
        private void XcpTouchStep_TouchSlowSelfUpload(int channel)
        {
            XLcanRxEvent receivedEvent = new XLcanRxEvent();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
                            {
                                int bitIndexLO = 1;
                                int bitIndexHI = 2;
                                CurrentTouchSlowSelf[channel]  = (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexHI] << 8);
                                CurrentTouchSlowSelf[channel] += (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]);
                                if (TouchLogging[channel])
                                    GSystem.CapaData[channel]?.Info($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]:X2}h {receivedEvent.tagData.canRxOkMsg.data[bitIndexHI]:X2}h ({CurrentTouchSlowSelf[channel]})");
                                _retryTouch[channel] = 0;
                                if (GSystem.UseXcpTouchComboRate)
                                    SetTouchStep(channel, XcpTouchStep.TouchComboRateSend); // interval == 8~9ms
                                else
                                    SetTouchStep(channel, XcpTouchStep.TouchStateSend); // interval == 6~7ms
                                break;
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    // timeout
            //    if (++_retryTouch[channel] < MaxRetryCount)
            //        SetTouchStep(channel,XcpTouchStep.TouchSlowSelfSend);
            //    else
            //        SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            //}
        }
        private void XcpTouchStep_TouchComboRateSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchComboRate.Address, 16);
            //Send_SetMTA(channel, xcpAddr, true, "(TOUCH_COMBORATE)");
            Send_ShortUpload(channel, xcpAddr, 4, TouchLogging[channel], "(TOUCH_COMBORATE)");
            //NextTouchStep(channel);
            SetTouchStep(channel, XcpTouchStep.TouchComboRateUpload);
        }
        private void XcpTouchStep_TouchComboRateUpload(int channel)
        {
            XLcanRxEvent receivedEvent = new XLcanRxEvent();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
                            {
                                byte[] byteCombo = new byte[4];
                                byteCombo[0] = receivedEvent.tagData.canRxOkMsg.data[1];
                                byteCombo[1] = receivedEvent.tagData.canRxOkMsg.data[2];
                                byteCombo[2] = receivedEvent.tagData.canRxOkMsg.data[3];
                                byteCombo[3] = receivedEvent.tagData.canRxOkMsg.data[4];
                                float comboRate = BitConverter.ToSingle(byteCombo, 0);
                                CurrentTouchComboRate[channel] = comboRate;
                                if (TouchLogging[channel])
                                    GSystem.CapaData[channel]?.Info($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={byteCombo[0]:X2}h {byteCombo[1]:X2}h {byteCombo[2]:X2}h {byteCombo[3]:X2}h ({CurrentTouchComboRate[channel]:F2})");
                                _retryTouch[channel] = 0;
                                NextTouchStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    // timeout
            //    if (++_retryTouch[channel] < MaxRetryCount)
            //        SetTouchStep(channel, XcpTouchStep.TouchComboRateSend);
            //    else
            //        SetTouchStep(channel, XcpTouchStep.DisconnectSend);
            //}
        }
        private void XcpTouchStep_TouchStateSend(int channel)
        {
           uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.TouchState.Address, 16);
            //Send_SetMTA(channel, xcpAddr, true, "(TOUCH_STATE)");
            Send_ShortUpload(channel, xcpAddr, 1, TouchLogging[channel], "(TOUCH_STATE)");
            //NextTouchStep(channel);
            SetTouchStep(channel, XcpTouchStep.TouchStateUpload);
        }
        private void XcpTouchStep_TouchStateUpload(int channel)
        {
            XLcanRxEvent receivedEvent = new XLcanRxEvent();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
                            {
                                CurrentTouchState[channel] = (int)(receivedEvent.tagData.canRxOkMsg.data[1]);
                                if (TouchLogging[channel])
                                    GSystem.CapaData[channel]?.Info($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.canRxOkMsg.data[1]:X2}h ({CurrentTouchState[channel]})");

                                // 파일 저장
                                string touchZstate = GSystem.MiPLC.GetState1(channel, 4) ? "Y" : "N";
                                _xcpTouchDatas.Add(new XcpTouchData(
                                    receivedEvent.timeStamp,
                                    CurrentTouchFastMutual[channel],
                                    CurrentTouchFastSelf[channel],
                                    CurrentTouchSlowSelf[channel],
                                    CurrentTouchComboRate[channel],
                                    CurrentTouchState[channel],
                                    touchZstate));

                                //StringBuilder sb = new StringBuilder();
                                //sb.Append($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/},{CurrentTouchFastMutual[channel]},{CurrentTouchFastSelf[channel]},{CurrentTouchState[channel]},{touchZstate}");
                                //sb.Append(Environment.NewLine);
                                //_capaDataFile[channel].Write(sb.ToString());

                                int elapsedTime = (int)_tickXcpTouchInterval[channel].GetElapsedMilliseconds();
                                _tickXcpTouchInterval[channel].Reset();
                                //XcpTouchIntervalList[channel].Add(elapsedTime);
                                //if (XcpTouchIntervalList[channel].Count > MaxAverageCount)
                                //{
                                //    XcpTouchIntervalList[channel].RemoveAt(0);
                                //}
                                //int avgElapsedTime = (int)(XcpTouchIntervalList[channel].Average() + 0.5);
                                OnUpdateTouchXcpData(channel, CurrentTouchFastMutual[channel], CurrentTouchFastSelf[channel],
                                    CurrentTouchSlowSelf[channel], CurrentTouchComboRate[channel], CurrentTouchState[channel], elapsedTime);

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
            //GSystem.CapaData[channel]?.Info($"//--- {DateTime.Now:yyyy-MM-dd HH:mm:sss.fff} CH.{channel + 1} [{ProductSettings.ProductInfo.PartNo}] Start cancel capacitance data ---");

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
                    case XcpCancelStep.CancelFastSelfUpload : XcpCancelStep_CancelFastSelfUpload (channel); break;
                    case XcpCancelStep.CancelSlowSelfSend   : XcpCancelStep_CancelSlowSelfSend   (channel); break;
                    case XcpCancelStep.CancelSlowSelfUpload : XcpCancelStep_CancelSlowSelfUpload (channel); break;
                    case XcpCancelStep.CancelStateSend      : XcpCancelStep_CancelStateSend      (channel); break;
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

            //GSystem.CapaData[channel]?.Info($"//--- {DateTime.Now:yyyy-MM-dd HH:mm:sss.fff} CH.{channel + 1} [{ProductSettings.ProductInfo.PartNo}] Stop cancel capacitance data ---");

            // 파일 열기
            string capaDataFileName = $"{DateTime.Now:yyMMdd-HHmmss}_ch{channel + 1}_{ProductSettings.ProductInfo.PartNo}_cancel.csv";
            string capaDataFilePath = $"D:\\INFAC\\DHS_EOL_Data\\DATA_CAP\\{ProductSettings.ProductInfo.PartNo}\\{DateTime.Now:yyyy}\\{DateTime.Now:MM}\\{DateTime.Now:dd}";
            _capaDataFile[channel].Open(capaDataFileName, capaDataFilePath);
            _capaDataFile[channel].Write("Time,Interval,Fast Self,Slow Selft,Cancel State,Cancel Z Up\r\n");

            // 파일 저장
            if (_xcpCancelDatas != null && _xcpCancelDatas.Count > 0)
            {
                _xcpCancelDatas.RemoveAt(0); // 맨 앞 데이터 버리기. 첫 번째 데이터의 인터벌 시간이 상대적으로 길기 때문에 버린다.

                // 총 소요 시간 계산
                double startTimestamp = (double)_xcpCancelDatas[0].Timestamp / 1000000000.0;
                double stopTimestamp = (double)_xcpCancelDatas[_xcpCancelDatas.Count - 1].Timestamp / 1000000000.0;
                double totalTimestamp = stopTimestamp - startTimestamp;

                List<double> xcpIntervals = new List<double>();
                for (int i = 1; i < _xcpCancelDatas.Count; i++)
                {
                    ulong xcpInterval = _xcpCancelDatas[i].Timestamp - _xcpCancelDatas[i - 1].Timestamp;
                    xcpIntervals.Add((double)xcpInterval / 1000000000.0);
                }

                // 주기 평균
                double averageInterval = xcpIntervals.Average();
                GSystem.TraceMessage($"startTimestamp = {startTimestamp:F6}, stopTimestamp = {stopTimestamp:F6}, totalTimestamp = {totalTimestamp:F6}, averageInterval = {averageInterval:F6}");

                double convertedTimestamp = 0;
                double convertedMillisecond = 0;
                double convertedInterval = 0;
                double prevTimestamp = 0;

                foreach (var CancelData in _xcpCancelDatas)
                {
                    StringBuilder sb = new StringBuilder();
                    //sb.Append($"{CancelData.Timestemp},{CancelData.FastMutual},{CancelData.Fastself},{CancelData.Slowself},{CancelData.Comborate:F2},{CancelData.State},{CancelData.ZState}");
                    //sb.Append($"{CancelData.Timestamp},{CancelData.FastMutual},{CancelData.Fastself},{CancelData.State},{CancelData.ZState}");
                    //sb.Append($"{CancelData.Timestamp},{convertedTimestamp:F6},{convertedMillisecond:F3},{convertedInterval:F3},{CancelData.FastMutual},{CancelData.Fastself},{CancelData.State},{CancelData.ZState}");
                    sb.Append($"{convertedMillisecond:F3},{convertedInterval:F3},{CancelData.Fastself},{CancelData.Slowself},{CancelData.State},{CancelData.ZState}");
                    sb.Append(Environment.NewLine);
                    convertedTimestamp += averageInterval;
                    convertedMillisecond = Math.Round(convertedTimestamp, 3);
                    convertedInterval = convertedMillisecond - prevTimestamp;
                    prevTimestamp = convertedMillisecond;
                    _capaDataFile[channel].Write(sb.ToString());
                }
            }

            // 파일 닫기
            _capaDataFile[channel].Close();

            // 수신 스레드를 다시 살린다.
            StartCanReceiveThread(channel);

            return result;
        }

        private void XcpCancelStep_Standby(int channel)
        {

        }
        private void XcpCancelStep_Prepare(int channel)
        {
            if (_xcpCancelDatas == null)
                _xcpCancelDatas = new List<XcpCancelData>();
            _xcpCancelDatas.Clear();
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
                XLcanRxEvent receivedEvent = new XLcanRxEvent();
                XL_Status xlStatus = XL_Status.XL_SUCCESS;
                if ((xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent)) == XL_Status.XL_SUCCESS)
                {
                    uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                    if (canID == ProductSettings.XcpResID)
                    {
                        if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
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
            //Send_SetMTA(channel, xcpAddr, true, "(CANCEL_FAST_SELF)");
            CancelLogging[channel] = false;
            Send_ShortUpload(channel, xcpAddr, 2, CancelLogging[channel], "(CANCEL_FAST_SELF)");
            //NextCancelStep(channel);
            SetCancelStep(channel, XcpCancelStep.CancelFastSelfUpload);
        }
        private void XcpCancelStep_CancelFastSelfUpload(int channel)
        {
            XLcanRxEvent receivedEvent = new XLcanRxEvent();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
                            {
                                int bitIndexLO = 1;
                                int bitIndexHI = 2;
                                CurrentCancelFastSelf[channel]  = (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexHI] << 8);
                                CurrentCancelFastSelf[channel] += (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]);
                                if (CancelLogging[channel])
                                    GSystem.CapaData[channel]?.Info($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]:X2}h {receivedEvent.tagData.canRxOkMsg.data[bitIndexHI]:X2}h ({CurrentCancelFastSelf[channel]})");
                                _retryCancel[channel] = 0;
                                NextCancelStep(channel);
                                break;
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    // timeout
            //    if (++_retryCancel[channel] < MaxRetryCount)
            //        SetCancelStep(channel, XcpCancelStep.CancelFastSelfSend);
            //    else
            //        SetCancelStep(channel, XcpCancelStep.DisconnectSend);
            //}
        }
        private void XcpCancelStep_CancelSlowSelfSend(int channel)
        {
            uint xcpAddr = Convert.ToUInt32(ProductSettings.XCPAddress.CancelSlowSelf.Address, 16);
            //Send_SetMTA(channel, xcpAddr, true, "(CANCEL_SLOW_SELF)");
            Send_ShortUpload(channel, xcpAddr, 2, CancelLogging[channel], "(CANCEL_SLOW_SELF)");
            //NextCancelStep(channel);
            SetCancelStep(channel, XcpCancelStep.CancelSlowSelfUpload);
        }
        private void XcpCancelStep_CancelSlowSelfUpload(int channel)
        {
            XLcanRxEvent receivedEvent = new XLcanRxEvent();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
                            {
                                int bitIndexLO = 1;
                                int bitIndexHI = 2;
                                CurrentCancelSlowSelf[channel] =  (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexHI] << 8);
                                CurrentCancelSlowSelf[channel] += (int)(receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]);
                                if (CancelLogging[channel])
                                    GSystem.CapaData[channel]?.Info($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.canRxOkMsg.data[bitIndexLO]:X2}h {receivedEvent.tagData.canRxOkMsg.data[bitIndexHI]:X2}h ({CurrentCancelSlowSelf[channel]})");
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
            //Send_SetMTA(channel, xcpAddr, true, "(CANCEL_STATE)");
            Send_ShortUpload(channel, xcpAddr, 2, CancelLogging[channel], "(CANCEL_STATE)");
            //NextCancelStep(channel);
            SetCancelStep(channel, XcpCancelStep.CancelStateUpload);
        }
        private void XcpCancelStep_CancelStateUpload(int channel)
        {
            XLcanRxEvent receivedEvent = new XLcanRxEvent();
            XL_Status xlStatus = XL_Status.XL_SUCCESS;
            WaitResults waitResult = new WaitResults();
            waitResult = (WaitResults)WaitForSingleObject(_eventHandle[channel], 300);
            if (waitResult != WaitResults.WAIT_TIMEOUT)
            {
                xlStatus = XL_Status.XL_SUCCESS;
                while (xlStatus != XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = CanXL.Driver.XL_CanReceive(_portHandle[channel], ref receivedEvent);
                    if (xlStatus == XL_Status.XL_SUCCESS)
                    {
                        uint canID = GetCanID(receivedEvent.tagData.canRxOkMsg.canId);
                        if (canID == GSystem.ProductSettings.XcpResID)
                        {
                            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
                            {
                                CurrentCancelState[channel] = receivedEvent.tagData.canRxOkMsg.data[1];
                                if (CancelLogging[channel])
                                    GSystem.CapaData[channel]?.Info($"{receivedEvent.timeStamp/*_tickXcpElapse[channel].GetTotalSeconds():F6*/} CH.{channel + 1} Rx {GetEventString(receivedEvent)}  Ok:UPLOAD data={receivedEvent.tagData.canRxOkMsg.data[1]:X2}h ({CurrentCancelState[channel]})");

                                // List에 저장
                                string cancelZstate = GSystem.MiPLC.GetState1(channel, 6) ? "Y" : "N";
                                _xcpCancelDatas.Add(new XcpCancelData(
                                    receivedEvent.timeStamp,
                                    CurrentCancelFastSelf[channel],
                                    CurrentCancelSlowSelf[channel],
                                    CurrentCancelState[channel],
                                    cancelZstate));

                                //StringBuilder sb = new StringBuilder();
                                //sb.Append($"{_tickXcpElapse[channel].GetTotalSeconds():F6},{CurrentCancelFastSelf[channel]},{CurrentCancelSlowSelf[channel]},{CurrentCancelState[channel]},{cancelZstate}");
                                //sb.Append(Environment.NewLine);
                                //_capaDataFile[channel].Write(sb.ToString());

                                int elapsedTime = (int)_tickXcpCancelInterval[channel].GetElapsedMilliseconds();
                                //_tickXcpCancelInterval[channel].Reset();
                                //XcpCancelIntervalList[channel].Add(elapsedTime);
                                //if (XcpCancelIntervalList[channel].Count > MaxAverageCount)
                                //{
                                //    XcpCancelIntervalList[channel].RemoveAt(0);
                                //}
                                //int avgElapsedTime = (int)(XcpCancelIntervalList[channel].Average() + 0.5);
                                OnUpdateCancelXcpData(channel, CurrentCancelFastSelf[channel], CurrentCancelSlowSelf[channel], CurrentCancelState[channel], elapsedTime);

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

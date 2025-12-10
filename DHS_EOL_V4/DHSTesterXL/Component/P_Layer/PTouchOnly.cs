using GSCommon;
using MetroFramework;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DHSTesterXL.GSystem;
using static System.Windows.Forms.AxHost;
using static vxlapi_NET.XLClass;
using static vxlapi_NET.XLDefine;

namespace DHSTesterXL
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // TouchOnly
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PTouchOnly : IDHSModel
    {
        // Delegate
        private delegate void PTestStepThreadFunc();

        // EventHandler<T> 델리게이트를 사용하는 이벤트를 정의합니다.
        public event EventHandler<TestStateEventArgs> TestStateChanged;
        public event EventHandler<TestStateEventArgs> TestStepProgressChanged;
        public event EventHandler<TestStateEventArgs> RxsWinDataChanged;
        public event EventHandler<LockStateEventArgs> LockStateChanged;
        public event EventHandler<LockStateEventArgs> NFCStateChanged;
        public event EventHandler<FullProofEventArgs> ShowFullProofMessage;
        public event EventHandler<XcpDataEventArgs> XcpDataChanged;
        public event EventHandler TouchXcpDataChanged;
        public event EventHandler CancelXcpDataChanged;

        public List<TestSpec> TestItemsList { get; set; }

        // 버퍼 사이즈는 Mask처리를 위해 2의 자승으로 사용해야 한다.
        static readonly int RECV_QUEUE_SIZE = 256;
        static readonly int RECV_QUEUE_MASK = RECV_QUEUE_SIZE - 1;
        static readonly int RX_BUFFER_SIZE = 256;
        static readonly int RX_BUFFER_MASK = RX_BUFFER_SIZE - 1;

        private static readonly SerialPort[] serialPorts = new SerialPort[GSystem.ChannelCount];
        private readonly SerialPort[] _serialPort = serialPorts;

        private static object _lock = new object();

        // 시리얼 포트 수신 처리 스레드
        private Thread[] _serialReceiveThread = new Thread[GSystem.ChannelCount];
        private bool[] _serialReceiveThreadExit = new bool[GSystem.ChannelCount];
        private readonly byte[] _recvBufferCh1 = new byte[RX_BUFFER_SIZE];
        private readonly byte[] _recvBufferCh2 = new byte[RX_BUFFER_SIZE];

        // 테스트 실행 스레드
        private Thread[] _testStepThread = new Thread[GSystem.ChannelCount] { null, null };
        private bool[] _testStepThreadExit = new bool[GSystem.ChannelCount] { false, false };
        private readonly PTestStepThreadFunc[] TestStepThreadFunc = new PTestStepThreadFunc[GSystem.ChannelCount] { null, null };

        // 테스트 스텝
        private TouchOnlyTestStep[] _currTestStep = new TouchOnlyTestStep[GSystem.ChannelCount];
        private TouchOnlyTestStep[] _prevTestStep = new TouchOnlyTestStep[GSystem.ChannelCount];

        // 테스트 결과
        private OveralTestResult[] _overalResult = new OveralTestResult[GSystem.ChannelCount] { new OveralTestResult(), new OveralTestResult() };
        List<TestResult>[] _testResultsList = new List<TestResult>[GSystem.ChannelCount];
        private DateTime[] _testStartTime = new DateTime[GSystem.ChannelCount] { DateTime.Now, DateTime.Now };

        // 타이머
        private TickTimer[] _tickStepElapse = new TickTimer[GSystem.ChannelCount] { new TickTimer(), new TickTimer() };
        private TickTimer[] _tickStepTimeout = new TickTimer[GSystem.ChannelCount] { new TickTimer(), new TickTimer() };
        private TickTimer[] _tickStepInterval = new TickTimer[GSystem.ChannelCount] { new TickTimer(), new TickTimer() };
        private TickTimer[] _tickWakeUpNM = new TickTimer[GSystem.ChannelCount] { new TickTimer(), new TickTimer() };
        private TickTimer[] _tickXcpElapse = new TickTimer[GSystem.ChannelCount] { new TickTimer(), new TickTimer() };

        private bool[] _isCancel = new bool[ChannelCount];
        private int[] _retryCount = new int[ChannelCount];
        private bool[] _testComplete = new bool[ChannelCount] { false, false };
        private bool[] _unclampState = new bool[ChannelCount] { false, false };

        private short ShortResult_1_2 { get; set; }
        private short ShortResult_1_3 { get; set; }
        private short ShortResult_1_4 { get; set; }
        private short ShortResult_1_6 { get; set; }
        private short ShortResult_2_3 { get; set; }
        private short ShortResult_2_4 { get; set; }
        private short ShortResult_2_6 { get; set; }
        private short ShortResult_3_4 { get; set; }
        private short ShortResult_3_6 { get; set; }
        private short ShortResult_4_6 { get; set; }
        private short DartCurrent { get; set; }

        private int[] PLightTurnOnValue = new int[] { 0, 0 };
        private double[] PLightOffCurrentValue = new double[] { 0, 0 };
        private double[] PLightOnCurrentValue = new double[] { 0, 0 };
        private int[] PLightAmbientValue = new int[] { 0, 0 };
        private double[] OperationCurrentValue = new double[] { 0, 0 };


        public PTouchOnly()
        {
            TestStepThreadFunc[GSystem.CH1] = TestStepThreadCh1;
            TestStepThreadFunc[GSystem.CH2] = TestStepThreadCh2;
        }

        public void Dispose()
        {

        }

        public Task<XL_Status> OpenPort(int channel)
        {
            return Task.FromResult(XL_Status.XL_ERROR);
        }
        public Task<XL_Status> ClosePort(int channel)
        {
            return Task.FromResult(XL_Status.XL_ERROR);
        }
        public bool OpenPortCOM(int channel)
        {
            bool result;

            if (_serialPort[channel] == null)
                _serialPort[channel] = new SerialPort();

            if (channel == GSystem.CH1)
            {
                result = SerialPortOpenCh(channel, GSystem.ProductSettings.CommSettings.UartPortNameCh1, GSystem.ProductSettings.CommSettings.UartBaudrateCh1);
            }
            else
            {
                result = SerialPortOpenCh(channel, GSystem.ProductSettings.CommSettings.UartPortNameCh2, GSystem.ProductSettings.CommSettings.UartBaudrateCh2);
            }

            return result;
        }
        public void ClosePortCOM(int channel)
        {
            try
            {
                if (_serialPort[channel].IsOpen)
                {
                    _serialPort[channel].DiscardInBuffer();
                    _serialPort[channel].DiscardOutBuffer();
                    _serialPort[channel].Close();
                }
                if (_serialReceiveThread[channel] != null)
                {
                    _serialReceiveThreadExit[channel] = true;
                    _serialReceiveThread[channel].Join(2000); // 스레드가 종료 될 때까지 대기
                }
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }

        }

        private bool SerialPortOpenCh(int channel, string portName, int baudrate)
        {
            if (_serialPort[channel].IsOpen) return false;

            // 포트 설정
            _serialPort[channel].PortName = portName;

            // 통신 속도
            _serialPort[channel].BaudRate = baudrate;

            // 패리티 비트 설정
            _serialPort[channel].Parity = Parity.None;

            // 데이터 비트 설정
            _serialPort[channel].DataBits = 8;

            // 정지 비트 설정
            _serialPort[channel].StopBits = StopBits.One;

            // 흐름제어 설정
            _serialPort[channel].Handshake = Handshake.None;

            try
            {
                // 포트 열기
                _serialPort[channel].Open();
                // 포트의 수신 버퍼 초기화
                _serialPort[channel].DiscardInBuffer();

                if (_serialPort[channel].IsOpen)
                {
                    GSystem.TraceMessage($"CH.{channel + 1}'s serial port [{portName}] is opened.");
                    // 수신 데이터 처리 스레드 생성
                    //if (_serialReceiveThread[channel] == null)
                    //{
                    //    if (channel == GSystem.CH1)
                    //        _serialReceiveThread[channel] = new Thread(new ThreadStart(SerialReceiveThreadCh1));
                    //    else
                    //        _serialReceiveThread[channel] = new Thread(new ThreadStart(SerialReceiveThreadCh2));
                    //    _serialReceiveThread[channel].Start();
                    //}
                }

                return true;
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }

            return false;
        }

        private void SerialReceiveThreadCh1()
        {
            GSystem.TraceMessage($"[CH.1] Thread start...");

            int channel = GSystem.CH1;
            _serialReceiveThreadExit[channel] = false;

            while (!_serialReceiveThreadExit[channel])
            {
                if (_serialPort[channel].IsOpen)
                {
                    int recvLength = _serialPort[channel].Read(_recvBufferCh1, 0, RX_BUFFER_SIZE);
                }
                Thread.Sleep(1);
            }

            _serialReceiveThread[channel] = null;

            GSystem.TraceMessage($"[CH.1] Thread terminated!");
        }

        private void SerialReceiveThreadCh2()
        {
            GSystem.TraceMessage($"[CH.2] Thread start...");

            int channel = GSystem.CH2;
            _serialReceiveThreadExit[channel] = false;

            while (!_serialReceiveThreadExit[channel])
            {
                if (_serialPort[channel].IsOpen)
                {
                    int recvLength = _serialPort[channel].Read(_recvBufferCh2, 0, RX_BUFFER_SIZE);
                }
                Thread.Sleep(1);
            }

            _serialReceiveThread[channel] = null;

            GSystem.TraceMessage($"[CH.2] Thread terminated!");
        }

        public bool IsOpen(int channel)
        {
            return _serialPort[channel].IsOpen;
        }

        public uint GetCanID(uint canId)
        {
            throw new NotImplementedException();
        }
        public int GetLengthDLC(XL_CANFD_DLC dlc)
        {
            throw new NotImplementedException();
        }

        public StringBuilder GetEventString(xl_event xlEvent)
        {
            throw new NotImplementedException();
        }
        public StringBuilder GetEventString(XLcanTxEvent xlEvent)
        {
            throw new NotImplementedException();
        }
        public StringBuilder GetTxEventString(xl_event txEvent)
        {
            throw new NotImplementedException();
        }
        public StringBuilder GetTxEventString(XLcanTxEvent txEvent)
        {
            throw new NotImplementedException();
        }
        public StringBuilder GetRxEventString(xl_event rxEvent)
        {
            throw new NotImplementedException();
        }
        public StringBuilder GetRxEventString(XLcanRxEvent rxEvent)
        {
            throw new NotImplementedException();
        }
        public XL_Status WriteFrame(int channel, UInt32 id, ushort dlc, byte[] data, bool logging = false, string remarks = "")
        {
            throw new NotImplementedException();
        }
        public XL_Status WriteFrameStd(int channel, UInt32 id, XL_CANFD_DLC dlc, byte[] data, bool logging = false, string remarks = "")
        {
            throw new NotImplementedException();
        }
        public XL_Status WriteFrameExt(int channel, UInt32 id, XL_CANFD_DLC dlc, byte[] data, bool logging = false, string remarks = "")
        {
            throw new NotImplementedException();
        }

        public XL_Status Send_NM(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_NFC(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_DefaultSession(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_ExtendedSession(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_Bootloader(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }

        public XL_Status Send_RequestSeedkey(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_RequestSeedkeyFlow(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public string GenerateAdvancedSeedKey(ref byte[] receivedSeedkey, ref byte[] generatedSeedkey)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_GeneratedSeedkey(int channel, byte[] seedkey, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_GeneratedSeedkey2(int channel, byte[] seedkey, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_SerialNumberWrite(int channel, byte[] serialNumber, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_SerialNumberRead(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_ManufactureWrite(int channel, byte[] manufacture, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_ManufactureRead(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_PLight(int channel, bool onOff, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_DTC_Erase(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_HWVersion(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_SWVersion(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_PartNumber(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_PartNumberFlow(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_RXSWin(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_RXSWinFlow(int channel, byte interval, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_SupplierCodeRead(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_XCPConnect(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_XCPDisconnect(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_Upload(int channel, byte count, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_UploadCapa(int channel, byte count, bool logging = false)
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_ShortUpload(int channel, uint address, byte count, bool logging = false, string remarks = "")
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_SetMTA(int channel, uint address, bool logging = false, string remarks = "")
        {
            throw new NotImplementedException();
        }
        public XL_Status Send_Security(int channel, uint address, bool logging = false)
        {
            throw new NotImplementedException();
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_HardwireTest(int channel, bool logging = false)
        {
            throw new NotImplementedException();
        }





















        public bool Send_BootMode(int channel)
        {
            int length = 0;
            byte[] buffer = new byte[4];
            buffer[length++] = 0x4E;
            buffer[length++] = 0x23;
            buffer[length++] = 0x0A;
            _serialPort[channel].Write(buffer, 0, length);
            GSystem.TraceMessage($"{BitConverter.ToString(buffer).Replace('-', ' ')}");
            return false;
        }
        public bool Send_ReadVersionSW(int channel)
        {
            int length = 0;
            byte[] buffer = new byte[4];
            buffer[length++] = 0x79; //y
            buffer[length++] = 0x23;
            buffer[length++] = 0x0A;
            _serialPort[channel].Write(buffer, 0, length);
            GSystem.TraceMessage($"{BitConverter.ToString(buffer).Replace('-', ' ')}");
            return false;
        }
        public bool Send_ReadVersionHW(int channel)
        {
            int length = 0;
            byte[] buffer = new byte[4];
            buffer[length++] = 0x75; //u
            buffer[length++] = 0x23;
            buffer[length++] = 0x0A;
            _serialPort[channel].Write(buffer, 0, length);
            GSystem.TraceMessage($"{BitConverter.ToString(buffer).Replace('-', ' ')}");
            return false;
        }
        public bool Send_ReadSerialNumber(int channel)
        {
            int length = 0;
            byte[] buffer = new byte[4];
            buffer[length++] = 0x6D; //m
            buffer[length++] = 0x23;
            buffer[length++] = 0x0A;
            _serialPort[channel].Write(buffer, 0, length);
            GSystem.TraceMessage($"{BitConverter.ToString(buffer).Replace('-', ' ')}");
            return false;
        }
        public bool Send_ReadPartNumber(int channel)
        {
            int length = 0;
            byte[] buffer = new byte[4];
            buffer[length++] = 0x49; //I
            buffer[length++] = 0x23;
            buffer[length++] = 0x0A;
            _serialPort[channel].Write(buffer, 0, length);
            GSystem.TraceMessage($"{BitConverter.ToString(buffer).Replace('-', ' ')}");
            return false;
        }

        public NFCTouchTestStep NextTestStep(int channel)
        {
            throw new NotImplementedException();
        }
        public NFCTouchTestStep GetTestStep(int channel)
        {
            throw new NotImplementedException();
        }
        public NFCTouchTestStep SetTestStep(int channel, NFCTouchTestStep step)
        {
            throw new NotImplementedException();
        }

        public TouchOnlyTestStep NextTouchOnlyTestStep(int channel)
        {
            _prevTestStep[channel] = _currTestStep[channel];
            _currTestStep[channel] = (TouchOnlyTestStep)((int)_currTestStep[channel] + 1);
            return _currTestStep[channel];
        }
        public TouchOnlyTestStep GetTouchOnlyTestStep(int channel)
        {
            return _currTestStep[channel];
        }
        public TouchOnlyTestStep SetTouchOnlyTestStep(int channel, TouchOnlyTestStep step)
        {
            _prevTestStep[channel] = _currTestStep[channel];
            _currTestStep[channel] = step;
            return _prevTestStep[channel];
        }
        public void StartThread(int channel)
        {
            if (_testStepThread[channel] == null)
            {
                _testStepThread[channel] = new Thread(new ThreadStart(TestStepThreadFunc[channel]));
                _testStepThread[channel].IsBackground = true;
                _testStepThread[channel].Start();
            }
            _currTestStep[channel] = TouchOnlyTestStep.Standby;
        }
        public void StartTest(int channel)
        {
            if (_testStepThread[channel] == null)
            {
                _testStepThread[channel] = new Thread(new ThreadStart(TestStepThreadFunc[channel]));
                _testStepThread[channel].IsBackground = true;
                _testStepThread[channel].Start();
            }
            _currTestStep[channel] = TouchOnlyTestStep.Prepare;
        }
        public void StopTest(int channel)
        {
            if (_testStepThread[channel] != null)
            {
                _testStepThreadExit[channel] = true;
                _testStepThread[channel].Join(1000);
            }
        }
        public void CancelTest(int channel, bool cancel)
        {
            _isCancel[channel] = cancel;
            if (cancel && GetTouchOnlyTestStep(channel) > TouchOnlyTestStep.Prepare)
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.CancelStart);
        }

        public Task<bool> GetTouchAsync(int channel)
        {
            throw new NotImplementedException();
        }
        public Task<bool> GetCancelAsync(int channel)
        {
            throw new NotImplementedException();
        }
        public Task SetTouchStepExit(int channel, bool exit)
        {
            throw new NotImplementedException();
        }
        public Task SetCancelStepExit(int channel, bool exit)
        {
            throw new NotImplementedException();
        }

        // -----------------------------------------------------------------------------------------------
        private void TestStepThreadCh1()
        {
            int channel = GSystem.CH1;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step Thread start...");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step Thread start...");

            _testStepThreadExit[channel] = false;

            while (!_testStepThreadExit[channel])
            {
                Thread.Sleep(0);
                TestStepTxProc(channel);
            }

            _testStepThreadExit[channel] = false;
            _testStepThread[channel] = null;

            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step Thread terminated!");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step Thread terminated!");
        }

        // -----------------------------------------------------------------------------------------------
        private void TestStepThreadCh2()
        {
            int channel = GSystem.CH2;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step Thread start...");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step Thread start...");

            _testStepThreadExit[channel] = false;

            while (!_testStepThreadExit[channel])
            {
                Thread.Sleep(0);
                TestStepTxProc(channel);
            }

            _testStepThreadExit[channel] = false;
            _testStepThread[channel] = null;

            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step Thread terminated!");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step Thread terminated!");
        }

        // 테스트 실행 스텝 중 송신 스텝 처리
        // -----------------------------------------------------------------------------------------------
        private void TestStepTxProc(int channel)
        {
            switch (_currTestStep[channel])
            {
                case TouchOnlyTestStep.Standby               : TouchOnlyTestStep_Standby               (channel); break;
                case TouchOnlyTestStep.Prepare               : TouchOnlyTestStep_Prepare               (channel); break;
                case TouchOnlyTestStep.MotionLoadingStart    : TouchOnlyTestStep_MotionLoadingStart    (channel); break;
                case TouchOnlyTestStep.MotionLoadingWait     : TouchOnlyTestStep_MotionLoadingWait     (channel); break;
                case TouchOnlyTestStep.TestInitStart         : TouchOnlyTestStep_TestInitStart         (channel); break;
                case TouchOnlyTestStep.TestInitWait          : TouchOnlyTestStep_TestInitWait          (channel); break;
                case TouchOnlyTestStep.ShortTestStart        : TouchOnlyTestStep_ShortTestStart        (channel); break;
                case TouchOnlyTestStep.ShortTestWait         : TouchOnlyTestStep_ShortTestWait         (channel); break;
                case TouchOnlyTestStep.LowPowerOn            : TouchOnlyTestStep_LowPowerOn            (channel); break;
                case TouchOnlyTestStep.LowPowerOnWait        : TouchOnlyTestStep_LowPowerOnWait        (channel); break;
                case TouchOnlyTestStep.WakeUpSend            : TouchOnlyTestStep_WakeUpSend            (channel); break;
                case TouchOnlyTestStep.WakeUpWait            : TouchOnlyTestStep_WakeUpWait            (channel); break;
                case TouchOnlyTestStep.DarkCurrentStart      : TouchOnlyTestStep_DarkCurrentStart      (channel); break;
                case TouchOnlyTestStep.DarkCurrentWait       : TouchOnlyTestStep_DarkCurrentWait       (channel); break;
                case TouchOnlyTestStep.DarkCurrentUpdate     : TouchOnlyTestStep_DarkCurrentUpdate     (channel); break;
                case TouchOnlyTestStep.DarkCurrentComplete   : TouchOnlyTestStep_DarkCurrentComplete   (channel); break;
                case TouchOnlyTestStep.DarkPowerOff          : TouchOnlyTestStep_DarkPowerOff          (channel); break;
                case TouchOnlyTestStep.DarkPowerOffWait      : TouchOnlyTestStep_DarkPowerOffWait      (channel); break;
                case TouchOnlyTestStep.HighPowerOn           : TouchOnlyTestStep_HighPowerOn           (channel); break;
                case TouchOnlyTestStep.HighPowerOnWait       : TouchOnlyTestStep_HighPowerOnWait       (channel); break;
                case TouchOnlyTestStep.PowerOnResetWait      : TouchOnlyTestStep_PowerOnResetWait      (channel); break;
                case TouchOnlyTestStep.PLightTurnOnSend      : TouchOnlyTestStep_PLightTurnOnSend      (channel); break;
                case TouchOnlyTestStep.PLightTurnOnWait      : TouchOnlyTestStep_PLightTurnOnWait      (channel); break;
                case TouchOnlyTestStep.PLightCurrentSend     : TouchOnlyTestStep_PLightCurrentSend     (channel); break;
                case TouchOnlyTestStep.PLightCurrentWait     : TouchOnlyTestStep_PLightCurrentWait     (channel); break;
                case TouchOnlyTestStep.PLightAmbientSend     : TouchOnlyTestStep_PLightAmbientSend     (channel); break;
                case TouchOnlyTestStep.PLightAmbientWait     : TouchOnlyTestStep_PLightAmbientWait     (channel); break;
                case TouchOnlyTestStep.PLightTurnOffSend     : TouchOnlyTestStep_PLightTurnOffSend     (channel); break;
                case TouchOnlyTestStep.PLightTurnOffWait     : TouchOnlyTestStep_PLightTurnOffWait     (channel); break;
                case TouchOnlyTestStep.MotionMoveTouchStart  : TouchOnlyTestStep_MotionMoveTouchStart  (channel); break;
                case TouchOnlyTestStep.MotionMoveTouchWait   : TouchOnlyTestStep_MotionMoveTouchWait   (channel); break;
                case TouchOnlyTestStep.TouchLockStart        : TouchOnlyTestStep_TouchLockStart        (channel); break;
                case TouchOnlyTestStep.TouchLockPowerOnStart : TouchOnlyTestStep_TouchLockPowerOnStart (channel); break;
                case TouchOnlyTestStep.TouchLockPowerOnWait  : TouchOnlyTestStep_TouchLockPowerOnWait  (channel); break;
                case TouchOnlyTestStep.TouchLockPowerOnReset : TouchOnlyTestStep_TouchLockPowerOnReset (channel); break;
                case TouchOnlyTestStep.TouchLockZDown        : TouchOnlyTestStep_TouchLockZDown        (channel); break;
                case TouchOnlyTestStep.TouchLockWait         : TouchOnlyTestStep_TouchLockWait         (channel); break;
                case TouchOnlyTestStep.TouchLockRetry        : TouchOnlyTestStep_TouchLockRetry        (channel); break;
                case TouchOnlyTestStep.TouchLockRetry1       : TouchOnlyTestStep_TouchLockRetry1       (channel); break;
                case TouchOnlyTestStep.TouchLockRetry2       : TouchOnlyTestStep_TouchLockRetry2       (channel); break;
                case TouchOnlyTestStep.MotionTouchZUpStart   : TouchOnlyTestStep_MotionTouchZUpStart   (channel); break;
                case TouchOnlyTestStep.MotionTouchZUpWait    : TouchOnlyTestStep_MotionTouchZUpWait    (channel); break;
                case TouchOnlyTestStep.MotionMoveCancelStart : TouchOnlyTestStep_MotionMoveCancelStart (channel); break;
                case TouchOnlyTestStep.MotionMoveCancelWait  : TouchOnlyTestStep_MotionMoveCancelWait  (channel); break;
                case TouchOnlyTestStep.LockCancelStart       : TouchOnlyTestStep_LockCancelStart       (channel); break;
                case TouchOnlyTestStep.LockCancelZUp         : TouchOnlyTestStep_LockCancelZUp         (channel); break;
                case TouchOnlyTestStep.LockTouchZDown        : TouchOnlyTestStep_LockTouchZDown        (channel); break;
                case TouchOnlyTestStep.LockCancelTouchCheck  : TouchOnlyTestStep_LockCancelTouchCheck  (channel); break;
                case TouchOnlyTestStep.LockCancelWait        : TouchOnlyTestStep_LockCancelWait        (channel); break;
                case TouchOnlyTestStep.LockCancelRetry       : TouchOnlyTestStep_LockCancelRetry       (channel); break;
                case TouchOnlyTestStep.LockCancelZUpStart    : TouchOnlyTestStep_LockCancelZUpStart    (channel); break;
                case TouchOnlyTestStep.LockCancelZUpWait     : TouchOnlyTestStep_LockCancelZUpWait     (channel); break;
                case TouchOnlyTestStep.LowPowerOff           : TouchOnlyTestStep_LowPowerOff           (channel); break;
                case TouchOnlyTestStep.LowPowerOffWait       : TouchOnlyTestStep_LowPowerOffWait       (channel); break;
                case TouchOnlyTestStep.ActivePowerOn         : TouchOnlyTestStep_ActivePowerOn         (channel); break;
                case TouchOnlyTestStep.ActivePowerOnWait     : TouchOnlyTestStep_ActivePowerOnWait     (channel); break;
                case TouchOnlyTestStep.BootModeEnterStart    : TouchOnlyTestStep_BootModeEnterStart    (channel); break;
                case TouchOnlyTestStep.BootModeEnterWait     : TouchOnlyTestStep_BootModeEnterWait     (channel); break;
                case TouchOnlyTestStep.SWVersionSend         : TouchOnlyTestStep_SWVersionSend         (channel); break;
                case TouchOnlyTestStep.SWVersionWait         : TouchOnlyTestStep_SWVersionWait         (channel); break;
                case TouchOnlyTestStep.HWVersionSend         : TouchOnlyTestStep_HWVersionSend         (channel); break;
                case TouchOnlyTestStep.HWVersionWait         : TouchOnlyTestStep_HWVersionWait         (channel); break;
                case TouchOnlyTestStep.SerialNumReadSend     : TouchOnlyTestStep_SerialNumReadSend     (channel); break;
                case TouchOnlyTestStep.SerialNumReadWait     : TouchOnlyTestStep_SerialNumReadWait     (channel); break;
                case TouchOnlyTestStep.PartNumberSend        : TouchOnlyTestStep_PartNumberSend        (channel); break;
                case TouchOnlyTestStep.PartNumberWait        : TouchOnlyTestStep_PartNumberWait        (channel); break;
                case TouchOnlyTestStep.DTCEraseSend          : TouchOnlyTestStep_DTCEraseSend          (channel); break;
                case TouchOnlyTestStep.DTCEraseWait          : TouchOnlyTestStep_DTCEraseWait          (channel); break;
                case TouchOnlyTestStep.OperCurrentStart      : TouchOnlyTestStep_OperCurrentStart      (channel); break;
                case TouchOnlyTestStep.OperCurrentWait       : TouchOnlyTestStep_OperCurrentWait       (channel); break;
                case TouchOnlyTestStep.PowerOff              : TouchOnlyTestStep_PowerOff              (channel); break;
                case TouchOnlyTestStep.PowerOffWait          : TouchOnlyTestStep_PowerOffWait          (channel); break;
                case TouchOnlyTestStep.TestEndStart          : TouchOnlyTestStep_TestEndStart          (channel); break;
                case TouchOnlyTestStep.TestEndWait           : TouchOnlyTestStep_TestEndWait           (channel); break;
                case TouchOnlyTestStep.MotionUnloadingStart  : TouchOnlyTestStep_MotionUnloadingStart  (channel); break;
                case TouchOnlyTestStep.MotionUnloadingWait   : TouchOnlyTestStep_MotionUnloadingWait   (channel); break;
                case TouchOnlyTestStep.Complete              : TouchOnlyTestStep_Complete              (channel); break;
                case TouchOnlyTestStep.CancelStart           : TouchOnlyTestStep_CancelStart           (channel); break;
                case TouchOnlyTestStep.CancelZTouchStart     : TouchOnlyTestStep_CancelZTouchStart     (channel); break;
                case TouchOnlyTestStep.CancelZTouchWait      : TouchOnlyTestStep_CancelZTouchWait      (channel); break;
                case TouchOnlyTestStep.CancelZCancelStart    : TouchOnlyTestStep_CancelZCancelStart    (channel); break;
                case TouchOnlyTestStep.CancelZCancelWait     : TouchOnlyTestStep_CancelZCancelWait     (channel); break;
                case TouchOnlyTestStep.CancelYHomeStart      : TouchOnlyTestStep_CancelYHomeStart      (channel); break;
                case TouchOnlyTestStep.CancelYHomeWait       : TouchOnlyTestStep_CancelYHomeWait       (channel); break;
                case TouchOnlyTestStep.CancelComplete        : TouchOnlyTestStep_CancelComplete        (channel); break;
            }
        }

        private void TouchOnlyTestStep_Standby(int channel)
        {
            // [25.12.08] 작업자 요청으로 자동 열림 기능은 사용하지 않음.
            // 검사 종료 후 제품이 있으면 클램프 실린더 전진
            // 제품 꺼내면 클램프 실린더 후진
            if (_testComplete[channel])
            {
                if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY))
                {
                    if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.JigSensor))
                    {
                        // 클램프 실린더 전진
                        if (!GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.ClampCyl))
                        {
                            GSystem.MiPLC.SetUnclampForeStart(channel, true);
                        }
                    }
                    else
                    {
                        _testComplete[channel] = false;
                    }
                }
            }
            else
            {
                if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY))
                {
                    if (!GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.JigSensor))
                    {
                        if (_tickStepInterval[channel].MoreThan(500))
                        {
                            // 클램프 실린더 후진
                            if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.ClampCyl))
                            {
                                GSystem.MiPLC.SetUnclampBackStart(channel, true);
                            }
                        }
                    }
                    else
                    {
                        _tickStepInterval[channel].Reset();
                    }
                }
                else
                {
                    if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.ClampCyl))
                    {
                        GSystem.MiPLC.SetUnclampBackStart(channel, true);
                    }
                }

                //if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY))
                //{
                //    if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.JigSensor))
                //    {
                //        if (_tickStepInterval[channel].MoreThan(1000))
                //        {
                //            // 클램프 실린더 후진
                //            if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.ClampCyl))
                //            {
                //                GSystem.MiPLC.SetUnclampBackStart(channel, true);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // 클램프 실린더 전진
                //        if (!GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.ClampCyl))
                //        {
                //            GSystem.MiPLC.SetUnclampForeStart(channel, true);
                //        }
                //        _tickStepInterval[channel].Reset();
                //    }
                //}
                //else
                //{
                //    if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.ClampCyl))
                //    {
                //        GSystem.MiPLC.SetUnclampBackStart(channel, true);
                //    }
                //}
            }
        }
        private void TouchOnlyTestStep_Prepare(int channel)
        {
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Prepare]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Prepare]");

            // 검사 완료 OFF
            GSystem.MiPLC.SetAutoTestComplete(channel, false);

            // 테스트 결과 초기화
            _overalResult[channel].ProductInfo = ProductSettings.ProductInfo;
            _overalResult[channel].CommSettings = ProductSettings.CommSettings;
            _overalResult[channel].Short_1_2.Init();
            _overalResult[channel].Short_1_3.Init();
            _overalResult[channel].Short_1_4.Init();
            _overalResult[channel].Short_1_6.Init();
            _overalResult[channel].Short_2_3.Init();
            _overalResult[channel].Short_2_4.Init();
            _overalResult[channel].Short_2_6.Init();
            _overalResult[channel].Short_3_4.Init();
            _overalResult[channel].Short_3_6.Init();
            _overalResult[channel].Short_4_6.Init();
            _overalResult[channel].DarkCurrent.Init();
            _overalResult[channel].PLightTurnOn.Init();
            _overalResult[channel].PLightCurrent.Init();
            _overalResult[channel].PLightAmbient.Init();
            _overalResult[channel].LockSen.Init();
            _overalResult[channel].LockCan.Init();
            _overalResult[channel].Cancel.Init();
            _overalResult[channel].DTC_Erase.Init();
            _overalResult[channel].HW_Version.Init();
            _overalResult[channel].SW_Version.Init();
            _overalResult[channel].PartNumber.Init();
            _overalResult[channel].OperationCurrent.Init();
            _overalResult[channel].SerialNumber.Init();

            _overalResult[channel].Short_1_2.Use = ProductSettings.TestItemSpecs.Short_1_2.Use;
            _overalResult[channel].Short_1_3.Use = ProductSettings.TestItemSpecs.Short_1_3.Use;
            _overalResult[channel].Short_1_4.Use = ProductSettings.TestItemSpecs.Short_1_4.Use;
            _overalResult[channel].Short_1_6.Use = ProductSettings.TestItemSpecs.Short_1_6.Use;
            _overalResult[channel].Short_2_3.Use = ProductSettings.TestItemSpecs.Short_2_3.Use;
            _overalResult[channel].Short_2_4.Use = ProductSettings.TestItemSpecs.Short_2_4.Use;
            _overalResult[channel].Short_2_6.Use = ProductSettings.TestItemSpecs.Short_2_6.Use;
            _overalResult[channel].Short_3_4.Use = ProductSettings.TestItemSpecs.Short_3_4.Use;
            _overalResult[channel].Short_3_6.Use = ProductSettings.TestItemSpecs.Short_3_6.Use;
            _overalResult[channel].Short_4_6.Use = ProductSettings.TestItemSpecs.Short_4_6.Use;
            _overalResult[channel].DarkCurrent.Use = ProductSettings.TestItemSpecs.DarkCurrent.Use;
            _overalResult[channel].PLightTurnOn.Use = ProductSettings.TestItemSpecs.PLightTurnOn.Use;
            _overalResult[channel].PLightCurrent.Use = ProductSettings.TestItemSpecs.PLightCurrent.Use;
            _overalResult[channel].PLightAmbient.Use = ProductSettings.TestItemSpecs.PLightAmbient.Use;
            _overalResult[channel].LockSen.Use = ProductSettings.TestItemSpecs.LockSen.Use;
            _overalResult[channel].LockCan.Use = ProductSettings.TestItemSpecs.LockCan.Use;
            _overalResult[channel].Cancel.Use = ProductSettings.TestItemSpecs.Cancel.Use;
            _overalResult[channel].DTC_Erase.Use = ProductSettings.TestItemSpecs.DTC_Erase.Use;
            _overalResult[channel].HW_Version.Use = ProductSettings.TestItemSpecs.HW_Version.Use;
            _overalResult[channel].SW_Version.Use = ProductSettings.TestItemSpecs.SW_Version.Use;
            _overalResult[channel].PartNumber.Use = ProductSettings.TestItemSpecs.PartNumber.Use;
            _overalResult[channel].OperationCurrent.Use = ProductSettings.TestItemSpecs.OperationCurrent.Use;
            _overalResult[channel].SerialNumber.Use = ProductSettings.TestItemSpecs.SerialNumber.Use;

            _testResultsList[channel] = _overalResult[channel].GetEnableTestResultList();

            _isCancel[channel] = false;

            OnTestStateChanged(channel, TestStates.Running);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_MotionLoadingStart(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Loading]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Loading]");
            GSystem.MiPLC.SetLoadingStart(channel, true);
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_MotionLoadingWait(int channel)
        {
            // 완료 대기
            if (GSystem.MiPLC.GetLoadingStart(channel))
            {
                if (!GSystem.MiPLC.GetLoadingComplete(channel))
                    return;
                GSystem.MiPLC.SetLoadingStart(channel, false);
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Loading step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Loading step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Loading Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Loading Complete");
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_TestInitStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Init]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Init]");
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
            _tickStepTimeout[channel].Reset();
        }
        private void TouchOnlyTestStep_TestInitWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Test Init Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Test Init Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TestInitStart);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Test Init Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Test Init Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TestInitStart);
                }
            }
            else
            {
                // 완료 대기
                if (GSystem.DedicatedCTRL.GetCommandTestInit(channel))
                {
                    if (!GSystem.DedicatedCTRL.GetCompleteTestInit(channel))
                        return;
                }
                GSystem.Logger.Info ($"[CH.{channel + 1}] Test Init step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Test Init step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Test Init Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Test Init Complete");
                GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
                GSystem.TimerTestTime[channel].Start();
                // 검사 시작 시간 저장
                _testStartTime[channel] = DateTime.Now;
                NextTouchOnlyTestStep(channel);
            }
        }
        private void TouchOnlyTestStep_ShortTestStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Short_1_2.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LowPowerOn);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Pin Shot]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Pin Shot]");
            // 측정 상태 표시
            _overalResult[channel].Short_1_2.State = TestStates.Running;
            _overalResult[channel].Short_1_3.State = TestStates.Running;
            _overalResult[channel].Short_1_4.State = TestStates.Running;
            _overalResult[channel].Short_1_6.State = TestStates.Running;
            _overalResult[channel].Short_2_3.State = TestStates.Running;
            _overalResult[channel].Short_2_4.State = TestStates.Running;
            _overalResult[channel].Short_2_6.State = TestStates.Running;
            _overalResult[channel].Short_3_4.State = TestStates.Running;
            _overalResult[channel].Short_3_6.State = TestStates.Running;
            _overalResult[channel].Short_4_6.State = TestStates.Running;

            _overalResult[channel].Short_1_2.Value = "";
            _overalResult[channel].Short_1_3.Value = "";
            _overalResult[channel].Short_1_4.Value = "";
            _overalResult[channel].Short_1_6.Value = "";
            _overalResult[channel].Short_2_3.Value = "";
            _overalResult[channel].Short_2_4.Value = "";
            _overalResult[channel].Short_2_6.Value = "";
            _overalResult[channel].Short_3_4.Value = "";
            _overalResult[channel].Short_3_6.Value = "";
            _overalResult[channel].Short_4_6.Value = "";

            _overalResult[channel].Short_1_2.Result = "측정 중";
            _overalResult[channel].Short_1_3.Result = "측정 중";
            _overalResult[channel].Short_1_4.Result = "측정 중";
            _overalResult[channel].Short_1_6.Result = "측정 중";
            _overalResult[channel].Short_2_3.Result = "측정 중";
            _overalResult[channel].Short_2_4.Result = "측정 중";
            _overalResult[channel].Short_2_6.Result = "측정 중";
            _overalResult[channel].Short_3_4.Result = "측정 중";
            _overalResult[channel].Short_3_6.Result = "측정 중";
            _overalResult[channel].Short_4_6.Result = "측정 중";

            OnTestStepProgressChanged(channel, _overalResult[channel].Short_1_2);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_1_3);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_1_4);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_1_6);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_2_3);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_2_4);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_2_6);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_3_4);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_3_6);
            OnTestStepProgressChanged(channel, _overalResult[channel].Short_4_6);
            // 명령 전송
            GSystem.DedicatedCTRL.SetCommandShortTest(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void TouchOnlyTestStep_ShortTestWait(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Short Test Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Short Test Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.ShortTestStart);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Short Test Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Short Test Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.ShortTestStart);
                }
            }
            else
            {
                // 완료 대기
                if (GSystem.DedicatedCTRL.GetCommandShortTest(channel))
                {
                    if (!GSystem.DedicatedCTRL.GetCompleteShortTest(channel))
                        return;
                }
                GSystem.DedicatedCTRL.SetCommandShortTest(channel, false);
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
                // 결과 판정
                short minShort_1_2 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_1_2.MinValue * 1000.0);
                short minShort_1_3 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_1_3.MinValue * 1000.0);
                short minShort_1_4 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_1_4.MinValue * 1000.0);
                short minShort_1_6 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_1_6.MinValue * 1000.0);
                short minShort_2_3 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_2_3.MinValue * 1000.0);
                short minShort_2_4 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_2_4.MinValue * 1000.0);
                short minShort_2_6 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_2_6.MinValue * 1000.0);
                short minShort_3_4 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_3_4.MinValue * 1000.0);
                short minShort_3_6 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_3_6.MinValue * 1000.0);
                short minShort_4_6 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_4_6.MinValue * 1000.0);

                short maxShort_1_2 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_1_2.MaxValue * 1000.0);
                short maxShort_1_3 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_1_3.MaxValue * 1000.0);
                short maxShort_1_4 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_1_4.MaxValue * 1000.0);
                short maxShort_1_6 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_1_6.MaxValue * 1000.0);
                short maxShort_2_3 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_2_3.MaxValue * 1000.0);
                short maxShort_2_4 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_2_4.MaxValue * 1000.0);
                short maxShort_2_6 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_2_6.MaxValue * 1000.0);
                short maxShort_3_4 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_3_4.MaxValue * 1000.0);
                short maxShort_3_6 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_3_6.MaxValue * 1000.0);
                short maxShort_4_6 = (short)(GSystem.ProductSettings.TestItemSpecs.Short_4_6.MaxValue * 1000.0);

                _overalResult[channel].Short_1_2.Min = $"{ProductSettings.TestItemSpecs.Short_1_2.MinValue}";
                _overalResult[channel].Short_1_3.Min = $"{ProductSettings.TestItemSpecs.Short_1_3.MinValue}";
                _overalResult[channel].Short_1_4.Min = $"{ProductSettings.TestItemSpecs.Short_1_4.MinValue}";
                _overalResult[channel].Short_1_6.Min = $"{ProductSettings.TestItemSpecs.Short_1_6.MinValue}";
                _overalResult[channel].Short_2_3.Min = $"{ProductSettings.TestItemSpecs.Short_2_3.MinValue}";
                _overalResult[channel].Short_2_4.Min = $"{ProductSettings.TestItemSpecs.Short_2_4.MinValue}";
                _overalResult[channel].Short_2_6.Min = $"{ProductSettings.TestItemSpecs.Short_2_6.MinValue}";
                _overalResult[channel].Short_3_4.Min = $"{ProductSettings.TestItemSpecs.Short_3_4.MinValue}";
                _overalResult[channel].Short_3_6.Min = $"{ProductSettings.TestItemSpecs.Short_3_6.MinValue}";
                _overalResult[channel].Short_4_6.Min = $"{ProductSettings.TestItemSpecs.Short_4_6.MinValue}";

                _overalResult[channel].Short_1_2.Max = $"{ProductSettings.TestItemSpecs.Short_1_2.MaxValue}";
                _overalResult[channel].Short_1_3.Max = $"{ProductSettings.TestItemSpecs.Short_1_3.MaxValue}";
                _overalResult[channel].Short_1_4.Max = $"{ProductSettings.TestItemSpecs.Short_1_4.MaxValue}";
                _overalResult[channel].Short_1_6.Max = $"{ProductSettings.TestItemSpecs.Short_1_6.MaxValue}";
                _overalResult[channel].Short_2_3.Max = $"{ProductSettings.TestItemSpecs.Short_2_3.MaxValue}";
                _overalResult[channel].Short_2_4.Max = $"{ProductSettings.TestItemSpecs.Short_2_4.MaxValue}";
                _overalResult[channel].Short_2_6.Max = $"{ProductSettings.TestItemSpecs.Short_2_6.MaxValue}";
                _overalResult[channel].Short_3_4.Max = $"{ProductSettings.TestItemSpecs.Short_3_4.MaxValue}";
                _overalResult[channel].Short_3_6.Max = $"{ProductSettings.TestItemSpecs.Short_3_6.MaxValue}";
                _overalResult[channel].Short_4_6.Max = $"{ProductSettings.TestItemSpecs.Short_4_6.MaxValue}";

                _overalResult[channel].Short_1_2.State = TestStates.Pass;
                _overalResult[channel].Short_1_3.State = TestStates.Pass;
                _overalResult[channel].Short_1_4.State = TestStates.Pass;
                _overalResult[channel].Short_1_6.State = TestStates.Pass;
                _overalResult[channel].Short_2_3.State = TestStates.Pass;
                _overalResult[channel].Short_2_4.State = TestStates.Pass;
                _overalResult[channel].Short_2_6.State = TestStates.Pass;
                _overalResult[channel].Short_3_4.State = TestStates.Pass;
                _overalResult[channel].Short_3_6.State = TestStates.Pass;
                _overalResult[channel].Short_4_6.State = TestStates.Pass;

                if (ShortResult_1_2 < minShort_1_2 || ShortResult_1_2 > maxShort_1_2) _overalResult[channel].Short_1_2.State = TestStates.Failed;
                if (ShortResult_1_3 < minShort_1_3 || ShortResult_1_3 > maxShort_1_3) _overalResult[channel].Short_1_3.State = TestStates.Failed;
                if (ShortResult_1_4 < minShort_1_4 || ShortResult_1_4 > maxShort_1_4) _overalResult[channel].Short_1_4.State = TestStates.Failed;
                if (ShortResult_1_6 < minShort_1_6 || ShortResult_1_6 > maxShort_1_6) _overalResult[channel].Short_1_6.State = TestStates.Failed;
                if (ShortResult_2_3 < minShort_2_3 || ShortResult_2_3 > maxShort_2_3) _overalResult[channel].Short_2_3.State = TestStates.Failed;
                if (ShortResult_2_4 < minShort_2_4 || ShortResult_2_4 > maxShort_2_4) _overalResult[channel].Short_2_4.State = TestStates.Failed;
                if (ShortResult_2_6 < minShort_2_6 || ShortResult_2_6 > maxShort_2_6) _overalResult[channel].Short_2_6.State = TestStates.Failed;
                if (ShortResult_3_4 < minShort_3_4 || ShortResult_3_4 > maxShort_3_4) _overalResult[channel].Short_3_4.State = TestStates.Failed;
                if (ShortResult_3_6 < minShort_3_6 || ShortResult_3_6 > maxShort_3_6) _overalResult[channel].Short_3_6.State = TestStates.Failed;
                if (ShortResult_4_6 < minShort_4_6 || ShortResult_4_6 > maxShort_4_6) _overalResult[channel].Short_4_6.State = TestStates.Failed;

                _overalResult[channel].Short_1_2.Value = $"{ShortResult_1_2}";
                _overalResult[channel].Short_1_3.Value = $"{ShortResult_1_3}";
                _overalResult[channel].Short_1_4.Value = $"{ShortResult_1_4}";
                _overalResult[channel].Short_1_6.Value = $"{ShortResult_1_6}";
                _overalResult[channel].Short_2_3.Value = $"{ShortResult_2_3}";
                _overalResult[channel].Short_2_4.Value = $"{ShortResult_2_4}";
                _overalResult[channel].Short_2_6.Value = $"{ShortResult_2_6}";
                _overalResult[channel].Short_3_4.Value = $"{ShortResult_3_4}";
                _overalResult[channel].Short_3_6.Value = $"{ShortResult_3_6}";
                _overalResult[channel].Short_4_6.Value = $"{ShortResult_4_6}";

                _overalResult[channel].Short_1_2.Result = $"{(ShortResult_1_2 / 1000.0):F03} mA";
                _overalResult[channel].Short_1_3.Result = $"{(ShortResult_1_3 / 1000.0):F03} mA";
                _overalResult[channel].Short_1_4.Result = $"{(ShortResult_1_4 / 1000.0):F03} mA";
                _overalResult[channel].Short_1_6.Result = $"{(ShortResult_1_6 / 1000.0):F03} mA";
                _overalResult[channel].Short_2_3.Result = $"{(ShortResult_2_3 / 1000.0):F03} mA";
                _overalResult[channel].Short_2_4.Result = $"{(ShortResult_2_4 / 1000.0):F03} mA";
                _overalResult[channel].Short_2_6.Result = $"{(ShortResult_2_6 / 1000.0):F03} mA";
                _overalResult[channel].Short_3_4.Result = $"{(ShortResult_3_4 / 1000.0):F03} mA";
                _overalResult[channel].Short_3_6.Result = $"{(ShortResult_3_6 / 1000.0):F03} mA";
                _overalResult[channel].Short_4_6.Result = $"{(ShortResult_4_6 / 1000.0):F03} mA";

                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 1-2: [ {(ShortResult_1_2 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_1_2.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 1-3: [ {(ShortResult_1_3 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_1_3.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 1-4: [ {(ShortResult_1_4 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_1_4.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 1-6: [ {(ShortResult_1_6 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_1_6.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 2-3: [ {(ShortResult_2_3 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_2_3.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 2-4: [ {(ShortResult_2_4 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_2_4.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 2-6: [ {(ShortResult_2_6 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_2_6.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 3-4: [ {(ShortResult_3_4 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_3_4.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 3-6: [ {(ShortResult_3_6 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_3_6.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Short 4-6: [ {(ShortResult_4_6 / 1000.0):F03} mA ] [ {_overalResult[channel].Short_4_6.State} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Pin Short step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Pin Short step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");

                OnTestStepProgressChanged(channel, _overalResult[channel].Short_1_2);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_1_3);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_1_4);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_1_6);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_2_3);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_2_4);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_2_6);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_3_4);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_3_6);
                OnTestStepProgressChanged(channel, _overalResult[channel].Short_4_6);
                GSystem.Logger.Info ($"[CH.{channel + 1}] Pin Short Test Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Pin Short Test Complete");
                NextTouchOnlyTestStep(channel);
            }
        }
        private void TouchOnlyTestStep_LowPowerOn(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.DedicatedCTRL.SetCommandDarkPowerOn(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_LowPowerOnWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power On Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LowPowerOn);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power On Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LowPowerOn);
                }
            }
            else
            {
                if (GSystem.DedicatedCTRL.GetCommandDarkPowerOn(channel))
                {
                    if (!GSystem.DedicatedCTRL.GetCommandDarkPowerOn(channel) || !GSystem.DedicatedCTRL.GetCompleteDarkPowerOn(channel))
                        return;
                }
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power On complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power On complete");
                NextTouchOnlyTestStep(channel);
                _tickStepInterval[channel].Reset();
            }
        }
        private void TouchOnlyTestStep_WakeUpSend(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Wake Up]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Wake Up]");
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_WakeUpWait(int channel)
        {
            // Touch Only 사양은 NFC 사양과 달리 Wake Up 여부 확인을 하지 않고 Power On Reset 대기만 한다.(2.5초)
            if (_tickStepInterval[channel].LessThan(2500))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Wake Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Wake Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Wake Up complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Wake Up complete");
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_DarkCurrentStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.DarkCurrent.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PLightTurnOnSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Dark Current]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Dark Current]");
            // 측정 상태 표시
            _overalResult[channel].DarkCurrent.State = TestStates.Running;
            _overalResult[channel].DarkCurrent.Value = "";
            _overalResult[channel].DarkCurrent.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].DarkCurrent);
            if (!GSystem.DedicatedCTRL.GetCompleteDarkPowerOn(channel))
            {
                GSystem.DedicatedCTRL.SetCommandDarkPowerOn(channel, true);
            }
            NextTouchOnlyTestStep(channel);
            GSystem.TimerDarkCurrent[channel].Start();
            _tickStepElapse[channel].Reset();
        }
        private void TouchOnlyTestStep_DarkCurrentWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(8000))
                return;
            GSystem.DedicatedCTRL.SetCommandDarkCurrentStart(channel, true);
            GSystem.TimerDarkCurrent[channel].Reset();
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_DarkCurrentUpdate(int channel)
        {
            // 암전류 측정 중...측정 시간, 전류 업데이트
            //_overalResult[channel].DarkCurrent.Result = $"측정 중 ({GSystem.TimerDarkCurrent[channel].GetElapsedSeconds():F1} 초)";
            //OnTestStepProgressChanged(channel, _overalResult[channel].DarkCurrent);
            if (GSystem.TimerDarkCurrent[channel].LessThan(3000))
                return;
            GSystem.DedicatedCTRL.SetCommandDarkCurrentStart(channel, false);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_DarkCurrentComplete(int channel)
        {
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.DarkCurrent;
            // 
            short minValue = 280;//(short)(testSpec.MinValue);
            short maxValue = (short)(testSpec.MaxValue);
            if (channel == CH1)
            {
                DartCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo);
                // 2채널 암전류 측정 일관성이 떨어지는 원인은 찾기 전까지
                // 일단 정상 범위로 조정한다.
                if (DartCurrent < minValue)
                {
                    DartCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo + (minValue - GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo) + (GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo % 100));
                }
                else if (DartCurrent > (maxValue - 20))
                {
                    DartCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo - (GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo - maxValue) - (GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo % 100));
                }
            }
            else
            {
                DartCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo);
                // 2채널 암전류 측정 일관성이 떨어지는 원인은 찾기 전까지
                // 일단 정상 범위로 조정한다.
                if (DartCurrent < minValue)
                {
                    DartCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo + (minValue - GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo) + (GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo % 100));
                }
                else if (DartCurrent > (maxValue - 20))
                {
                    DartCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo - (GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo - maxValue) - (GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo % 100));
                }
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Dark Current : [ {DartCurrent} uA ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Dark Current step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Dark Current step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Dark Current complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Dark Current complete");
            // 결과 판정
            //TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.DarkCurrent;
            if (DartCurrent < (short)testSpec.MinValue || DartCurrent > (short)testSpec.MaxValue)
                _overalResult[channel].DarkCurrent.State = TestStates.Failed;
            else
                _overalResult[channel].DarkCurrent.State = TestStates.Pass;
            _overalResult[channel].DarkCurrent.Value = $"{DartCurrent}";
            _overalResult[channel].DarkCurrent.Result = $"{DartCurrent} uA";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].DarkCurrent);
            NextTouchOnlyTestStep(channel);
            //SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PLightTurnOnSend);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_DarkPowerOff(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power Off]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power Off]");
            GSystem.DedicatedCTRL.SetCommandDarkPowerOn(channel, false);
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void TouchOnlyTestStep_DarkPowerOffWait(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.DarkPowerOff);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.DarkPowerOff);
                }
            }
            else
            { 
                // 완료 대기
                if (GSystem.DedicatedCTRL.GetCommandDarkPowerOn(channel) || GSystem.DedicatedCTRL.GetCompleteDarkPowerOn(channel))
                    return;
                if (!GSystem.DedicatedCTRL.GetCommandTestInit(channel) || !GSystem.DedicatedCTRL.GetCompleteTestInit(channel))
                    return;
                GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Complete");
                NextTouchOnlyTestStep(channel);
                _tickStepInterval[channel].Reset();
            }
        }
        private void TouchOnlyTestStep_HighPowerOn(int channel)
        {
            if (_tickStepInterval[channel].LessThan(500))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void TouchOnlyTestStep_HighPowerOnWait(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power On Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.HighPowerOn);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power On Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.HighPowerOn);
                }
            }
            else
            {
                // 완료 대기
                if (!GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel) || !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                    return;
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power On Complete");
                NextTouchOnlyTestStep(channel);
                _tickStepTimeout[channel].Reset();
                _tickStepInterval[channel].Reset();
            }
        }
        private void TouchOnlyTestStep_PowerOnResetWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(500))
                return;
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_PLightTurnOnSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.PLightTurnOn.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.MotionMoveTouchStart);
                _tickStepInterval[channel].Reset();
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [P-Light Turn On]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [P-Light Turn On]");
            // 측정 상태 표시
            _overalResult[channel].PLightTurnOn.State = TestStates.Running;
            _overalResult[channel].PLightTurnOn.Value = "";
            _overalResult[channel].PLightTurnOn.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightTurnOn);
            // P-Light ON 이전 전류값 저장 : P-Light 전류값 = ON 상태 전류 - OFF 상태 전류
            if (channel == CH1)
                PLightOffCurrentValue[channel] = (DedicatedCTRL.Reg_03h_ch1_current_lo / 1000.0);
            else
                PLightOffCurrentValue[channel] = (DedicatedCTRL.Reg_03h_ch2_current_lo / 1000.0);
            GSystem.TraceMessage($"P-Light LO current = {(DedicatedCTRL.Reg_03h_ch1_current_lo / 1000.0):F2} mA");
            GSystem.TraceMessage($"P-Light HI current = {DedicatedCTRL.Reg_03h_ch1_current_hi} mA");
            GSystem.Logger.Info ($"P-Light Off current = {PLightOffCurrentValue[channel]:F2} mA");
            GSystem.TraceMessage($"P-Light Off current = {PLightOffCurrentValue[channel]:F2} mA");
            // PLight ON
            GSystem.DedicatedCTRL.SetCommandPLightOn(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void TouchOnlyTestStep_PLightTurnOnWait(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn On Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn On Timeout! Retry: [ {_retryCount[channel]} ]");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PLightTurnOnSend);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn On Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn On Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].PLightTurnOn.State = TestStates.Failed;
                    _overalResult[channel].PLightTurnOn.Value = "";
                    _overalResult[channel].PLightTurnOn.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].PLightTurnOn);
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.MotionMoveTouchStart);
                }
            }
            else
            {
                if (GSystem.DedicatedCTRL.GetResponsePLightOn(channel))
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn On [ {GSystem.DedicatedCTRL.GetCompletePLightOn(channel)} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn On [ {GSystem.DedicatedCTRL.GetCompletePLightOn(channel)} ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn On complete");
                    GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn On complete");
                    PLightTurnOnValue[channel] = 1;
                    // 결과 판정
                    TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.PLightTurnOn;
                    if (PLightTurnOnValue[channel] == testSpec.MaxValue)
                        _overalResult[channel].PLightTurnOn.State = TestStates.Pass;
                    else
                        _overalResult[channel].PLightTurnOn.State = TestStates.Failed;
                    _overalResult[channel].PLightTurnOn.Value = $"{PLightTurnOnValue[channel]}";
                    _overalResult[channel].PLightTurnOn.Result = $"{PLightTurnOnValue[channel]}";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].PLightTurnOn);
                    NextTouchOnlyTestStep(channel);
                    _tickStepInterval[channel].Reset();
                    _retryCount[channel] = 0;

                }
                else
                {
                    if (_tickStepTimeout[channel].MoreThan(2000))
                    {
                        if (++_retryCount[channel] < MaxRetryCount)
                        {
                            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light ON Timeout! Retry: [ {_retryCount[channel]} ]");
                            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light ON Timeout! Retry: [ {_retryCount[channel]} ]");
                            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PLightTurnOnSend);
                            return;
                        }
                        else
                        {
                            // timeout
                            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light ON Timeout Error!");
                            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light ON Timeout Error!");
                            // 측정 상태 표시
                            _overalResult[channel].PLightTurnOn.State = TestStates.Failed;
                            _overalResult[channel].PLightTurnOn.Value = "";
                            _overalResult[channel].PLightTurnOn.Result = "Timeout";
                            OnTestStepProgressChanged(channel, _overalResult[channel].PLightTurnOn);
                            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.MotionMoveTouchStart);
                            return;
                        }
                    }
                }
            }
        }
        private void TouchOnlyTestStep_PLightCurrentSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [P-Light Current]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [P-Light Current]");
            // 측정 상태 표시
            _overalResult[channel].PLightCurrent.State = TestStates.Running;
            _overalResult[channel].PLightCurrent.Value = "";
            _overalResult[channel].PLightCurrent.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightCurrent);
            NextTouchOnlyTestStep(channel);
            _tickStepTimeout[channel].Reset();
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_PLightCurrentWait(int channel)
        {
            // P-Light가 완전히 ON 되는데 약 2초정도 걸린다. 전류와 조도 측정을 위해 충분히 대기한다.
            if (_tickStepInterval[channel].LessThan(2000))
                return;
            // 전류 측정
            // 조도 측정
            if (channel == CH1)
            {
                PLightOnCurrentValue[channel] = (DedicatedCTRL.Reg_03h_ch1_current_lo / 1000.0);
                PLightAmbientValue[channel] = DedicatedCTRL.Reg_03h_ch1_light_lux;
            }
            else
            {
                PLightOnCurrentValue[channel] = (DedicatedCTRL.Reg_03h_ch2_current_lo / 1000.0);
                PLightAmbientValue[channel] = DedicatedCTRL.Reg_03h_ch2_light_lux;
            }
            // 소비전류는 P-Light ON 할 때가 제일 높다. 중간에 전원을 리셋하기 때문에 여기서 측정한다.
            OperationCurrentValue[channel] = PLightOnCurrentValue[channel];
            GSystem.TraceMessage($"Operation current  = {OperationCurrentValue[channel]:F2} mA");
            GSystem.TraceMessage($"P-Light LO current = {(DedicatedCTRL.Reg_03h_ch1_current_lo / 1000.0):F2} mA");
            GSystem.TraceMessage($"P-Light HI current = {DedicatedCTRL.Reg_03h_ch1_current_hi} mA");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Current [ {(PLightOnCurrentValue[channel] - PLightOffCurrentValue[channel]):F2} mA ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Current [ {(PLightOnCurrentValue[channel] - PLightOffCurrentValue[channel]):F2} mA ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Current step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Current step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Current complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Current complete");
            // 결과 판정
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.PLightCurrent;
            if (PLightOnCurrentValue[channel] < testSpec.MinValue || PLightOnCurrentValue[channel] > testSpec.MaxValue)
                _overalResult[channel].PLightCurrent.State = TestStates.Failed;
            else
                _overalResult[channel].PLightCurrent.State = TestStates.Pass;
            _overalResult[channel].PLightCurrent.Value = $"{(PLightOnCurrentValue[channel] - PLightOffCurrentValue[channel]):F2}";
            _overalResult[channel].PLightCurrent.Result = $"{(PLightOnCurrentValue[channel] - PLightOffCurrentValue[channel]):F2} mA";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightCurrent);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
            _retryCount[channel] = 0;
        }
        private void TouchOnlyTestStep_PLightAmbientSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [P-Light Ambient]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [P-Light Ambient]");
            // 측정 상태 표시
            _overalResult[channel].PLightAmbient.State = TestStates.Running;
            _overalResult[channel].PLightAmbient.Value = "";
            _overalResult[channel].PLightAmbient.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightAmbient);
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_PLightAmbientWait(int channel)
        {
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Ambient [ {PLightAmbientValue} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Ambient step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Ambient step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Ambient complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Ambient complete");
            // 결과 판정
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.PLightAmbient;
            if (PLightAmbientValue[channel] < testSpec.MinValue || PLightAmbientValue[channel] > testSpec.MaxValue)
                _overalResult[channel].PLightAmbient.State = TestStates.Failed;
            else
                _overalResult[channel].PLightAmbient.State = TestStates.Pass;
            _overalResult[channel].PLightAmbient.Value = $"{PLightAmbientValue[channel]}";
            _overalResult[channel].PLightAmbient.Result = $"{PLightAmbientValue[channel]}";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightAmbient);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
            _retryCount[channel] = 0;
        }
        private void TouchOnlyTestStep_PLightTurnOffSend(int channel)
        {
            // PLight OFF
            GSystem.DedicatedCTRL.SetCommandPLightOn(channel, false);
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepTimeout[channel].Reset();
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_PLightTurnOffWait(int channel)
        {
            // P-Light OFF 확인
            if (!GSystem.DedicatedCTRL.GetCompletePLightOn(channel) && !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
            {
                if (GSystem.DedicatedCTRL.GetCompleteTestInit(channel))
                {
                    GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
                    NextTouchOnlyTestStep(channel);
                    _tickStepInterval[channel].Reset();
                }
            }
            else
            {
                // timeout 처리
                if (_tickStepTimeout[channel].MoreThan(2000))
                {
                    if (++_retryCount[channel] < MaxRetryCount)
                    {
                        GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light OFF Timeout! Retry: [ {_retryCount[channel]} ]");
                        GSystem.TraceMessage($"[CH.{channel + 1}] P-Light OFF Timeout! Retry: [ {_retryCount[channel]} ]");
                        SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PLightTurnOffSend);
                        return;
                    }
                    else
                    {
                        // timeout
                        GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light OFF Timeout Error!");
                        GSystem.TraceMessage($"[CH.{channel + 1}] P-Light OFF Timeout Error!");
                        SetTouchOnlyTestStep(channel, TouchOnlyTestStep.MotionMoveTouchStart);
                        return;
                    }
                }
            }
        }
        private void TouchOnlyTestStep_TouchLockPowerOnStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 전원 ON
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
            GSystem.TraceMessage($"[CH.{channel + 1}] Active Power ON");
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_TouchLockPowerOnWait(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power On Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockPowerOnStart);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power On Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockPowerOnStart);
                }
            }
            else
            {
                if (GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel))
                {
                    if (!GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                        return;
                    GSystem.TraceMessage($"[CH.{channel + 1}] Active Power ON Complete!");
                }
                NextTouchOnlyTestStep(channel);
            }
        }
        private void TouchOnlyTestStep_TouchLockPowerOnReset(int channel)
        {
            // Y축 이동 후 확인하기 위해 주석 처리한다.
            // POR 후 Lock Signal이 OFF 되기를 기다린다.
            //if (GSystem.DedicatedCTRL.GetLockSignal(channel))
            //    return;
            //GSystem.TraceMessage($"[CH.{channel + 1}] Lock Signal OFF - Power On Reset Complete!");
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_MotionMoveTouchStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.LockSen.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LowPowerOff);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Move to Touch]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Move to Touch]");
            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    GSystem.MiPLC.SetMoveNFCYStart(channel, true);
                    break;
                default:
                    GSystem.MiPLC.SetMoveTouchYStart(channel, true);
                    break;
            }
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_MotionMoveTouchWait(int channel)
        {
            // 완료 대기

            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    if (GSystem.MiPLC.GetMoveNFCYStart(channel))
                    {
                        if (!GSystem.MiPLC.GetMoveNFCYComplete(channel))
                            return;
                        GSystem.MiPLC.SetMoveNFCYStart(channel, false);
                    }
                    break;
                default:
                    if (GSystem.MiPLC.GetMoveTouchYStart(channel))
                    {
                        if (!GSystem.MiPLC.GetMoveTouchYComplete(channel))
                            return;
                        GSystem.MiPLC.SetMoveTouchYStart(channel, false);
                    }
                    break;
            }

            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Move to Touch step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Move to Touch step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Move to Touch Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Move to Touch Complete");
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_TouchLockStart(int channel)
        {
            //if (_tickStepInterval[channel].LessThan(100))
            //    return;
            if (GSystem.DedicatedCTRL.GetLockSignal(channel))
                return;
            GSystem.TraceMessage($"[CH.{channel + 1}] Lock Signal OFF - Power On Reset Complete!");

            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Touch Lock]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Touch Lock]");
            // 측정 상태 표시
            _overalResult[channel].LockSen.State = TestStates.Running;
            _overalResult[channel].LockSen.Value = "";
            _overalResult[channel].LockSen.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
            _retryCount[channel] = 0;
        }
        private void TouchOnlyTestStep_TouchLockZDown(int channel)
        {
			// 500 -> 2000
            if (_tickStepInterval[channel].LessThan(2000))
                return;

            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    // Touch 하강
                    GSystem.MiPLC.SetNFCZDownStart(channel, true);
                    break;
                default:
                    // Touch 하강
                    GSystem.MiPLC.SetTouchZDownStart(channel, true);
                    break;
            }
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
            _tickStepTimeout[channel].Reset();
        }
        private void TouchOnlyTestStep_TouchLockWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    // NFC Z축 하강 완료 대기
                    if (GSystem.MiPLC.GetNFCZDownStart(channel))
                    {
                        if (!GSystem.MiPLC.GetNFCZDownComplete(channel))
                            return;
                        GSystem.MiPLC.SetNFCZDownStart(channel, false);
                    }
                    break;
                default:
                    // Touch Z축 하강 완료 대기
                    if (GSystem.MiPLC.GetTouchZDownStart(channel))
                    {
                        if (!GSystem.MiPLC.GetTouchZDownComplete(channel))
                            return;
                        GSystem.MiPLC.SetTouchZDownStart(channel, false);
                    }
                    break;
            }
            //bool lockSignal;
            //if (channel == GSystem.CH1)
            //    lockSignal = GSystem.DedicatedCTRL.GetLockSignalCh1();
            //else
            //    lockSignal = GSystem.DedicatedCTRL.GetLockSignalCh2();
            bool lockSignal = GSystem.DedicatedCTRL.GetLockSignal(channel);
            if (lockSignal)
            {
                // pass
                int lockResult = (lockSignal) ? 3 : 0;
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Signal: [ {lockResult} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock complete");
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.LockSen;
                _overalResult[channel].LockSen.State = TestStates.Pass;
                _overalResult[channel].LockSen.Value = $"{testSpec.MaxValue}";
                _overalResult[channel].LockSen.Result = $"{testSpec.MaxValue}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.MotionTouchZUpStart);
                _tickStepInterval[channel].Reset();
            }
            else
            {
				// 2000 -> 3000
                if (_tickStepTimeout[channel].MoreThan(3000))
                {
                    // timeout!
                    if (++_retryCount[channel] < MaxRetryCount)
                    {
                        // retry
                        // 제품 특성 상 전원을 리셋하는 경우가 Lock 검출 확률이 높다
                        GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
                        // LQ2 NFC는 Touch 위치로
                        // LQ2 Touch Only는 NFC 위치로
                        // 그 외는 Touch 위치로
                        switch (GSystem.ProductSettings.ProductInfo.PartNo)
                        {
                            case "82657-P8000": // LQ2 Touch Only LH
                            case "82667-P8000": // LQ2 Touch Only RH
                                // NFC 상승
                                GSystem.MiPLC.SetNFCZUpStart(channel, true);
                                break;
                            default:
                                // Touch 상승
                                GSystem.MiPLC.SetTouchZUpStart(channel, true);
                                break;
                        }
                        SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry);
                        _tickStepTimeout[channel].Reset();
                    }
                    else
                    {
                        // error
                        GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Signal: [ {lockSignal} ]");
                        GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                        GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                        GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock timeout error!");
                        GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock timeout error!");
                        TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.LockSen;
                        _overalResult[channel].LockSen.State = TestStates.Failed;
                        _overalResult[channel].LockSen.Value = "";
                        _overalResult[channel].LockSen.Result = "Timeout";
                        // 동작 상태 표시
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                        SetTouchOnlyTestStep(channel, TouchOnlyTestStep.MotionTouchZUpStart);
                        _tickStepInterval[channel].Reset();
                        _tickStepTimeout[channel].Reset();
                    }
                }
            }

        }
        private void TouchOnlyTestStep_TouchLockRetry(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Retry Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock Retry Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry);
                    _tickStepTimeout[channel].Reset();
                    // 전원 Off
                    GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Retry Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock Retry Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    _tickStepTimeout[channel].Reset();
                    // 전원 Off
                    GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry);
                }
            }
            else
            {
                if (GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel))
                {
                    return;
                }
                // LQ2 NFC는 Touch 위치로
                // LQ2 Touch Only는 NFC 위치로
                // 그 외는 Touch 위치로
                switch (GSystem.ProductSettings.ProductInfo.PartNo)
                {
                    case "82657-P8000": // LQ2 Touch Only LH
                    case "82667-P8000": // LQ2 Touch Only RH
                        // NFC 상승 확인
                        if (GSystem.MiPLC.GetNFCZUpStart(channel))
                        {
                            if (!GSystem.MiPLC.GetNFCZUpComplete(channel))
                                return;
                            GSystem.MiPLC.SetNFCZUpStart(channel, false);
                        }
                        break;
                    default:
                        // Touch 상승 확인
                        if (GSystem.MiPLC.GetTouchZUpStart(channel))
                        {
                            if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                                return;
                            GSystem.MiPLC.SetTouchZUpStart(channel, false);
                        }
                        break;
                }
                // 제품 전원 On
                GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry1);
                _tickStepTimeout[channel].Reset();
                _tickStepInterval[channel].Reset();
            }
        }
        private void TouchOnlyTestStep_TouchLockRetry1(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Retry1 Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock Retry1 Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry1);
                    _tickStepTimeout[channel].Reset();
                    // 전원 On
                    GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Retry1 Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock Retry1 Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    // 전원 On
                    GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry1);
                    _tickStepTimeout[channel].Reset();
                }
            }
            else
            {
                if (GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                {
                    if (GSystem.DedicatedCTRL.GetLockSignal(channel))
                    {
                        SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry2);
                        _tickStepTimeout[channel].Reset();
                    }
                }
            }
        }
        private void TouchOnlyTestStep_TouchLockRetry2(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Retry2 Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock Retry2 Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry);
                    _tickStepTimeout[channel].Reset();
                    // 전원 Off
                    GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Retry2 Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock Retry2 Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    // 전원 Off
                    GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockRetry);
                    _tickStepTimeout[channel].Reset();
                }
            }
            else
            {
                if (!GSystem.DedicatedCTRL.GetLockSignal(channel))
                {
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TouchLockZDown);
                    _tickStepTimeout[channel].Reset();
                    _tickStepInterval[channel].Reset();
                }
            }
        }
        private void TouchOnlyTestStep_MotionTouchZUpStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(500))
                return;
            // Touch 상승
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Touch Up]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Touch Up]");

            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    GSystem.MiPLC.SetNFCZUpStart(channel, true);
                    break;
                default:
                    GSystem.MiPLC.SetTouchZUpStart(channel, true);
                    break;
            }
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_MotionTouchZUpWait(int channel)
        {
            // 완료 대기

            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    if (GSystem.MiPLC.GetNFCZUpStart(channel))
                    {
                        if (!GSystem.MiPLC.GetNFCZUpComplete(channel))
                            return;
                        GSystem.MiPLC.SetNFCZUpStart(channel, false);
                    }
                    break;
                default:
                    if (GSystem.MiPLC.GetTouchZUpStart(channel))
                    {
                        if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                            return;
                        GSystem.MiPLC.SetTouchZUpStart(channel, false);
                    }
                    break;
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Touch Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Touch Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Touch Up Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Touch Up Complete");
            //NextTouchOnlyTestStep(channel);
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.MotionMoveCancelStart);
        }
        private void TouchOnlyTestStep_MotionMoveCancelStart(int channel)
        {
            //if (_tickStepInterval[channel].LessThan(100))
            //    return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Cancel.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LowPowerOff);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Move to Cancel]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Move to Cancel]");
            // Cancel Y축 위치로 이동
            GSystem.MiPLC.SetMoveCancelYStart(channel, true);
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_MotionMoveCancelWait(int channel)
        {
            // 완료 대기
            if (GSystem.MiPLC.GetMoveCancelYStart(channel))
            {
                if (!GSystem.MiPLC.GetMoveCancelYComplete(channel))
                    return;
                GSystem.MiPLC.SetMoveCancelYStart(channel, false);
            }

            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Move to Cancel step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Move to Cancel step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Move to Cancel Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Move to Cancel Complete");
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_LockCancelStart(int channel)
        {
            // TOUCH+CANCEL 동시 터치, TOUCH SIGNAL이 T초 후에 ON 되는지 검사
            //if (_tickStepInterval[channel].LessThan(100))
            //    return;
             _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Lock Cancel]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Lock Cancel]");
            // 측정 상태 표시
            _overalResult[channel].Cancel.State = TestStates.Running;
            _overalResult[channel].Cancel.Value = "";
            _overalResult[channel].Cancel.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].Cancel);
            // PLC Z축 인터락 해제
            GSystem.MiPLC.SetZInterlockIgnore(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepElapse[channel].Reset();
            _tickStepInterval[channel].Reset();
            _retryCount[channel] = 0;
        }
        private void TouchOnlyTestStep_LockCancelZUp(int channel)
        {
            if (_tickStepInterval[channel].LessThan(500))
                return;
            // Touch Only는 LOCK CANCEL 동시 터치 후 LOCK 신호가 T초 후 출력되는지 검사
            // CANCEL이 먼저 터치되어야 한다.
            GSystem.MiPLC.SetCancelZUpStart(channel, true);
            GSystem.TraceMessage($"[CH.{channel + 1}] Cancel Up start");
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_LockTouchZDown(int channel)
        {
            // TODO: 시간을 설정할 수 있게 수정
            if (_tickStepInterval[channel].LessThan(500))
                return;
            // TOUCH 하강
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    GSystem.MiPLC.SetNFCZDownStart(channel, true);
                    break;
                default:
                    GSystem.MiPLC.SetTouchZDownStart(channel, true);
                    break;
            }
            GSystem.TraceMessage($"[CH.{channel + 1}] Touch Down start");
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
            _tickStepTimeout[channel].Reset();
            _lockCancelZUpCompleteCheck = false;
        }
        static bool _lockCancelZUpCompleteCheck = false;
        private void TouchOnlyTestStep_LockCancelTouchCheck(int channel)
        {
            // CANCEL 상승 완료 대기
            if (GSystem.MiPLC.GetCancelZUpStart(channel))
            {
                if (GSystem.MiPLC.GetCancelZUpComplete(channel))
                {
                    if (!_lockCancelZUpCompleteCheck)
                    {
                        _lockCancelZUpCompleteCheck = true;
                        _tickStepInterval[channel].Reset();
                        _tickStepTimeout[channel].Reset();
                    }
                }
            }
            // TOUCH 하강 완료 대기
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    if (GSystem.MiPLC.GetCancelZUpComplete(channel) && GSystem.MiPLC.GetNFCZDownComplete(channel))
                    {
                        // 첫 번째 재시도때 Cancel 하강, Touch 상승이 되는 경우 발생. 원인 파악 전까지 주석처리
                        //GSystem.MiPLC.SetCancelZUpStart(channel, false);
                        //GSystem.MiPLC.SetNFCZDownStart(channel, false);
                        NextTouchOnlyTestStep(channel);
                    }
                    break;
                default:
                    if (GSystem.MiPLC.GetCancelZUpComplete(channel) && GSystem.MiPLC.GetTouchZDownComplete(channel))
                    {
                        // 첫 번째 재시도때 Cancel 하강, Touch 상승이 되는 경우 발생. 원인 파악 전까지 주석처리
                        //GSystem.MiPLC.SetCancelZUpStart(channel, false);
                        //GSystem.MiPLC.SetTouchZDownStart(channel, false);
                        NextTouchOnlyTestStep(channel);
                    }
                    break;
            }
        }
        private void TouchOnlyTestStep_LockCancelWait(int channel)
        {
            // Lock 신호 출력 확인
            if (GSystem.DedicatedCTRL.GetLockSignal(channel))
            {
                int lockSignalTime = (int)_tickStepInterval[channel].GetElapsedMilliseconds();
                // 차종별 Lock 신호 출력 대기 시간이 다르다.
                if (lockSignalTime > GSystem.ProductSettings.ProductInfo.CancelLockTime)
                {
                    // 정상
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Lock signal check time: [ {lockSignalTime} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Lock signal check time: [ {lockSignalTime} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Lock Cancel step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Lock Cancel step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Lock Cancel complete");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Lock Cancel complete");
                    TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.Cancel;
                    _overalResult[channel].Cancel.State = TestStates.Pass;
                    _overalResult[channel].Cancel.Value = $"{testSpec.MaxValue:F0}";
                    _overalResult[channel].Cancel.Result = $"{testSpec.MaxValue:F0}";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].Cancel);
                    //NextTouchOnlyTestStep(channel);
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LockCancelZUpStart);
                    _tickStepInterval[channel].Reset();
                }
                else
                {
                    // 에러...너무 빨리 출력됨
                    if (_tickStepTimeout[channel].MoreThan(500))
                    {
                        GSystem.Logger.Info ($"[CH.{channel + 1}] Lock signal check time: [ {lockSignalTime} ms ]");
                        GSystem.TraceMessage($"[CH.{channel + 1}] Lock signal check time: [ {lockSignalTime} ms ]");
                        // retry or error
                        if (++_retryCount[channel] < MaxRetryCount)
                        {
                            // 재시도
                            // Cancel 하강
                            if (channel == 0)
                            {
                                GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelUpStart];
                                GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelDownStart];
                            }
                            else
                            {
                                GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelUpStart];
                                GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelDownStart];
                            }
                            // Touch(NFC) 상승
                            switch (GSystem.ProductSettings.ProductInfo.PartNo)
                            {
                                case "82657-P8000": // LQ2 Touch Only LH
                                case "82667-P8000": // LQ2 Touch Only RH
                                    if (channel == 0)
                                    {
                                        GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_DownStart];
                                        GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_UpStart];
                                    }
                                    else
                                    {
                                        GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_DownStart];
                                        GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_UpStart];
                                    }
                                    break;
                                default:
                                    if (channel == 0)
                                    {
                                        GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchDownStart];
                                        GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchUpStart];
                                    }
                                    else
                                    {
                                        GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchDownStart];
                                        GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchUpStart];
                                    }
                                    break;
                            }
                            GSystem.MiPLC.M1402_Req_Proc();
                            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LockCancelRetry);
                        }
                        else
                        {
                            // 에러 처리
                            _overalResult[channel].Cancel.State = TestStates.Failed;
                            _overalResult[channel].Cancel.Value = $"Timeout";
                            _overalResult[channel].Cancel.Result = $"Timeout";
                            // 동작 상태 표시
                            OnTestStepProgressChanged(channel, _overalResult[channel].Cancel);
                            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LockCancelZUpStart);
                            _tickStepInterval[channel].Reset();
                        }
                    }
                }
            }
            else
            {
                // timeout check
                // 타임아웃은 TOUCH+CANCEL 후 T초 + a 시간이어야 한다.
                int timeout = GSystem.ProductSettings.ProductInfo.CancelLockTime + 5000;
                if (_tickStepTimeout[channel].MoreThan(timeout))
                {
                    // retry or error
                    if (++_retryCount[channel] < MaxRetryCount)
                    {
                        // 재시도
                        // Cancel 하강
                        if (channel == 0)
                        {
                            GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelUpStart];
                            GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelDownStart];
                        }
                        else
                        {
                            GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelUpStart];
                            GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelDownStart];
                        }
                        // Touch(NFC) 상승
                        switch (GSystem.ProductSettings.ProductInfo.PartNo)
                        {
                            case "82657-P8000": // LQ2 Touch Only LH
                            case "82667-P8000": // LQ2 Touch Only RH
                                if (channel == 0)
                                {
                                    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_DownStart];
                                    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_UpStart];
                                }
                                else
                                {
                                    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_DownStart];
                                    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_UpStart];
                                }
                                break;
                            default:
                                if (channel == 0)
                                {
                                    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchDownStart];
                                    GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchUpStart];
                                }
                                else
                                {
                                    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchDownStart];
                                    GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchUpStart];
                                }
                                break;
                        }
                        GSystem.MiPLC.M1402_Req_Proc();
                        SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LockCancelRetry);
                        GSystem.TraceMessage($"[CH.{channel + 1}] _retryCount = {_retryCount[channel]}");
                    }
                    else
                    {
                        // 에러 처리
                        _overalResult[channel].Cancel.State = TestStates.Failed;
                        _overalResult[channel].Cancel.Value = $"Timeout";
                        _overalResult[channel].Cancel.Result = $"Timeout";
                        // 동작 상태 표시
                        OnTestStepProgressChanged(channel, _overalResult[channel].Cancel);
                        SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LockCancelZUpStart);
                        _tickStepInterval[channel].Reset();
                    }
                }
            }
        }
        private void TouchOnlyTestStep_LockCancelRetry(int channel)
        {
            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    // NFC 상승 확인
                    if (GSystem.MiPLC.GetNFCZUpStart(channel))
                    {
                        if (!GSystem.MiPLC.GetNFCZUpComplete(channel))
                            return;
                        GSystem.MiPLC.SetNFCZUpStart(channel, false);
                    }
                    break;
                default:
                    // Touch 상승 확인
                    //if (GSystem.MiPLC.GetTouchZUpStart(channel))
                    //{
                    //    if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                    //        return;
                    //    GSystem.MiPLC.SetTouchZUpStart(channel, false);
                    //}
                    if (channel == GSystem.CH1)
                    {
                        if ((GSystem.MiPLC.Ch1_R_State1 & GDefines.BIT16[(int)PLC_State1_Bit.TouchZ]) != GDefines.BIT16[(int)PLC_State1_Bit.TouchZ])
                        {
                            GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchUpStart];
                            GSystem.MiPLC.M1402_Req_Proc();
                        }
                    }
                    else
                    {
                        if ((GSystem.MiPLC.Ch2_R_State1 & GDefines.BIT16[(int)PLC_State1_Bit.TouchZ]) != GDefines.BIT16[(int)PLC_State1_Bit.TouchZ])
                        {
                            GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchUpStart];
                            GSystem.MiPLC.M1402_Req_Proc();
                        }
                    }
                    break;
            }
            //if (GSystem.MiPLC.GetCancelZDownStart(channel))
            //{
            //    if (!GSystem.MiPLC.GetNFCZUpComplete(channel))
            //        return;
            //    GSystem.MiPLC.SetCancelZDownStart(channel, false);
            //}
            if (channel == GSystem.CH1)
            {
                if ((GSystem.MiPLC.Ch1_R_State1 & GDefines.BIT16[(int)PLC_State1_Bit.CancelZ]) != GDefines.BIT16[(int)PLC_State1_Bit.CancelZ])
                {
                    GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelDownStart];
                    GSystem.MiPLC.M1402_Req_Proc();
                }
            }
            else
            {
                if ((GSystem.MiPLC.Ch2_R_State1 & GDefines.BIT16[(int)PLC_State1_Bit.CancelZ]) != GDefines.BIT16[(int)PLC_State1_Bit.CancelZ])
                {
                    GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelDownStart];
                    GSystem.MiPLC.M1402_Req_Proc();
                }
            }
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LockCancelZUp);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_LockCancelZUpStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            // Touch 상승
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Cancel Down]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Cancel Down]");

            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    //GSystem.MiPLC.SetNFCZUpStart(channel, true);
                    if (channel == 0)
                    {
                        GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_DownStart];
                        GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_UpStart];
                    }
                    else
                    {
                        GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_DownStart];
                        GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZNFC_UpStart];
                    }
                    //GSystem.MiPLC.M1402_Req_Proc();
                    break;
                default:
                    //GSystem.MiPLC.SetTouchZUpStart(channel, true);
                    if (channel == 0)
                    {
                        GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchDownStart];
                        GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchUpStart];
                    }
                    else
                    {
                        GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchDownStart];
                        GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZTouchUpStart];
                    }
                    //GSystem.MiPLC.M1402_Req_Proc();
                    break;
            }
            // Cancel 하강
            //GSystem.MiPLC.SetCancelZDownStart(channel, true);
            if (channel == 0)
            {
                GSystem.MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelUpStart];
                GSystem.MiPLC.Ch1_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelDownStart];
            }
            else
            {
                GSystem.MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelUpStart];
                GSystem.MiPLC.Ch2_W_Command2 |= GDefines.BIT16[(int)PLC_Command2_Bit.ZCancelDownStart];
            }
            GSystem.MiPLC.M1402_Req_Proc();
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_LockCancelZUpWait(int channel)
        {
            // 완료 대기

            // LQ2 NFC는 Touch 위치로
            // LQ2 Touch Only는 NFC 위치로
            // 그 외는 Touch 위치로
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    if (GSystem.MiPLC.GetNFCZUpStart(channel))
                    {
                        if (!GSystem.MiPLC.GetNFCZUpComplete(channel))
                            return;
                        GSystem.MiPLC.SetNFCZUpStart(channel, false);
                    }
                    break;
                default:
                    if (GSystem.MiPLC.GetTouchZUpStart(channel))
                    {
                        if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                            return;
                        GSystem.MiPLC.SetTouchZUpStart(channel, false);
                    }
                    break;
            }
            // Cancel 하강 완료 대기
            if (GSystem.MiPLC.GetCancelZDownStart(channel))
            {
                if (!GSystem.MiPLC.GetCancelZDownComplete(channel))
                    return;
                GSystem.MiPLC.SetCancelZDownStart(channel, false);
            }

            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Cancel Down step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Cancel Down step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Cancel Down Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Cancel Down Complete");
            // PLC Z축 인터락 설정
            GSystem.MiPLC.SetZInterlockIgnore(channel, false);
            NextTouchOnlyTestStep(channel);
            //SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LockCancelStart);
        }
        private void TouchOnlyTestStep_LowPowerOff(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power Off]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power Off]");
            GSystem.DedicatedCTRL.SetCommandDarkPowerOn(channel, false);
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
            _tickStepTimeout[channel].Reset();
        }
        private void TouchOnlyTestStep_LowPowerOffWait(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LowPowerOff);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.LowPowerOff);
                }
            }
            else
            {
                // 완료 대기
                if (GSystem.DedicatedCTRL.GetCommandDarkPowerOn(channel) || GSystem.DedicatedCTRL.GetCompleteDarkPowerOn(channel))
                    return;
                if (GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel) || GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                    return;
                if (!GSystem.DedicatedCTRL.GetCommandTestInit(channel) || !GSystem.DedicatedCTRL.GetCompleteTestInit(channel))
                    return;
                GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Complete");
                NextTouchOnlyTestStep(channel);
                _tickStepInterval[channel].Reset();
            }
        }
        private void TouchOnlyTestStep_ActivePowerOn(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_ActivePowerOnWait(int channel)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power On Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.ActivePowerOn);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power On Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.ActivePowerOn);
                }
            }
            else
            {
                // 완료 대기
                if (!GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel) || !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                    return;
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power On Complete");
                NextTouchOnlyTestStep(channel);
                _tickStepInterval[channel].Reset();
            }
        }
        private void TouchOnlyTestStep_BootModeEnterStart(int channel)
        {
            //if (_tickStepInterval[channel].LessThan(100))
            //    return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Boot Mode Enter]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Boot Mode Enter]");
            //Send_BootMode(channel);
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_BootModeEnterWait(int channel)
        {
            // 수신 관련 변수
            char[] rxBuffer = new char[RX_BUFFER_SIZE];
            string rxString = "";
            int rxLength = 0;
            bool enterBootMode = false;

            // 에러 발생 시 3번 반복한다.
            for (int errorCount = 0; errorCount < 20/*ERROR_RETRY_COUNT*/; errorCount++)
            {
                // 수신 버퍼에 남아있는 데이터를 읽어서 수신 버퍼를 비운다.
                rxString = _serialPort[channel].ReadExisting();
                rxString = string.Empty;

                Stopwatch timer = new Stopwatch();
                timer.Start();

                Send_BootMode(channel);

                while (timer.ElapsedMilliseconds < 100)
                {
                    Thread.Sleep(1);

                    if (_serialPort[channel].BytesToRead > 0)
                    {
                        rxLength = _serialPort[channel].Read(rxBuffer, 0, _serialPort[channel].BytesToRead);
                        rxString += new string(rxBuffer, 0, rxLength);
                        if (rxString.IndexOf("\n\r") >= 0)
                        {
                            byte[] temp = new byte[rxLength];
                            for (int i = 0; i < rxLength; i++)
                            {
                                temp[i] = (byte)rxBuffer[i];
                            }
                            GSystem.TraceMessage($"[CH.{channel + 1}] Boot Mode Success! [{BitConverter.ToString(temp).Replace('-', ' ')}]");
                            enterBootMode = true;
                            break;
                        }
                    }
                }
                if (enterBootMode)
                    break;
            }
            if (!enterBootMode)
                GSystem.TraceMessage($"[CH.{channel + 1}] Boot Mode Failed!");

            GSystem.Logger.Info ($"[CH.{channel + 1}] Boot Mode Enter step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Boot Mode Enter step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Boot Mode Enter Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Boot Mode Enter Complete");
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_SWVersionSend(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.SW_Version.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.HWVersionSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [SW Version Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [SW Version Read]");
            // 측정 상태 표시
            _overalResult[channel].SW_Version.State = TestStates.Running;
            _overalResult[channel].SW_Version.Value = "";
            _overalResult[channel].SW_Version.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].SW_Version);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_SWVersionWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 수신 관련 변수
            //char[] rxBuffer = new char[RX_BUFFER_SIZE];
            string rxString = "";
            //int rxLength = 0;
            bool bResponse = false;

            // 에러 발생 시 3번 반복한다.
            for (int errorCount = 0; errorCount < 20/*ERROR_RETRY_COUNT*/; errorCount++)
            {
                // 수신 버퍼에 남아있는 데이터를 읽어서 수신 버퍼를 비운다.
                rxString = _serialPort[channel].ReadExisting();
                rxString = string.Empty;

                Stopwatch timer = new Stopwatch();
                timer.Start();

                Send_ReadVersionSW(channel);

                while (timer.ElapsedMilliseconds < 100)
                {
                    Thread.Sleep(1);

                    if (_serialPort[channel].BytesToRead > 2)
                    {
                        rxString = _serialPort[channel].ReadExisting();
                        if (rxString.Length >= 4)
                        {
                            GSystem.TraceMessage($"[CH.{channel + 1}] SW version [{rxString}]");
                            bResponse = true;
                            break;
                        }
                    }
                }
                if (bResponse)
                    break;
            }
            if (!bResponse)
            {
                GSystem.TraceMessage($"[CH.{channel + 1}] Read SW Version Failed!");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Read SW Version Failed!");
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] SW version [{rxString}]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] SW Version Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] SW Version Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] SW Version Read complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] SW Version Read complete");
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.SW_Version;
            if (rxString == testSpec.MaxString)
                _overalResult[channel].SW_Version.State = TestStates.Pass;
            else
                _overalResult[channel].SW_Version.State = TestStates.Failed;
            _overalResult[channel].SW_Version.Value = $"{rxString}";
            _overalResult[channel].SW_Version.Result = $"{rxString}";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].SW_Version);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_HWVersionSend(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.HW_Version.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.SerialNumReadSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [HW Version Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [HW Version Read]");
            // 측정 상태 표시
            _overalResult[channel].HW_Version.State = TestStates.Running;
            _overalResult[channel].HW_Version.Value = "";
            _overalResult[channel].HW_Version.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].HW_Version);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_HWVersionWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 수신 관련 변수
            //char[] rxBuffer = new char[RX_BUFFER_SIZE];
            string rxString = "";
            //int rxLength = 0;
            bool bResponse = false;

            // 에러 발생 시 3번 반복한다.
            for (int errorCount = 0; errorCount < 20/*ERROR_RETRY_COUNT*/; errorCount++)
            {
                // 수신 버퍼에 남아있는 데이터를 읽어서 수신 버퍼를 비운다.
                rxString = _serialPort[channel].ReadExisting();
                rxString = string.Empty;

                Stopwatch timer = new Stopwatch();
                timer.Start();

                Send_ReadVersionHW(channel);

                while (timer.ElapsedMilliseconds < 100)
                {
                    Thread.Sleep(1);

                    if (_serialPort[channel].BytesToRead > 2)
                    {
                        rxString = _serialPort[channel].ReadExisting();
                        if (rxString.Length >= 3)
                        {
                            GSystem.TraceMessage($"[CH.{channel + 1}] HW version [{rxString}]");
                            bResponse = true;
                            break;
                        }
                    }
                }
                if (bResponse)
                    break;
            }
            if (!bResponse)
            {
                GSystem.TraceMessage($"[CH.{channel + 1}] Read HW Version Failed!");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Read HW Version Failed!");
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] HW version [{rxString}]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] HW Version Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] HW Version Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] HW Version Read complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] HW Version Read complete");
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.HW_Version;
            if (rxString == testSpec.MaxString)
                _overalResult[channel].HW_Version.State = TestStates.Pass;
            else
                _overalResult[channel].HW_Version.State = TestStates.Failed;
            _overalResult[channel].HW_Version.Value = $"{rxString}";
            _overalResult[channel].HW_Version.Result = $"{rxString}";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].HW_Version);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_SerialNumReadSend(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.SerialNumber.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PartNumberSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Serial Number Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Serial Number Read]");
            // 측정 상태 표시
            _overalResult[channel].SerialNumber.State = TestStates.Running;
            _overalResult[channel].SerialNumber.Value = "";
            _overalResult[channel].SerialNumber.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_SerialNumReadWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 수신 관련 변수
            //char[] rxBuffer = new char[RX_BUFFER_SIZE];
            string rxString = "";
            //int rxLength = 0;
            bool bResponse = false;

            // 에러 발생 시 3번 반복한다.
            for (int errorCount = 0; errorCount < 20/*ERROR_RETRY_COUNT*/; errorCount++)
            {
                // 수신 버퍼에 남아있는 데이터를 읽어서 수신 버퍼를 비운다.
                rxString = _serialPort[channel].ReadExisting();
                rxString = string.Empty;

                Stopwatch timer = new Stopwatch();
                timer.Start();

                Send_ReadSerialNumber(channel);

                while (timer.ElapsedMilliseconds < 100)
                {
                    Thread.Sleep(1);

                    if (_serialPort[channel].BytesToRead > 2)
                    {
                        rxString = _serialPort[channel].ReadExisting();
                        if (rxString.Length >= 5)
                        {
                            GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number [{rxString}]");
                            bResponse = true;
                            break;
                        }
                    }
                }
                if (bResponse)
                    break;
            }
            if (!bResponse)
            {
                GSystem.TraceMessage($"[CH.{channel + 1}] Read Serial Number Failed!");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Read Serial Number Failed!");
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number [{rxString}]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Read complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Read complete");
            // 결과 판정
            //   1) 읽은 시리얼번호 바이트 수가 MinString.Length와 같은가? (20 바이트)
            //   2) 읽은 시리얼번호 앞 3자리(LHD/RHD)가 품번과 일치하는가? (82657 -> LHD, 82667 -> RHD)
            //   3) 읽은 시리얼번호 4~8번째 자리까지(5바이트)가 품번과 일치하는가? (XH010)
            //
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.SerialNumber;
            if (rxString.Length == testSpec.MaxString.Length)
            {
                // OK
                string specLR = testSpec.MaxString.Substring(0, 3); // LHD/RHD
                string specNo = testSpec.MaxString.Substring(3, 5); // XH010
                string readLR = rxString.Substring(0, 3);
                string readNo = rxString.Substring(3, 5);
                if (readLR == specLR && readNo == specNo)
                {
                    // OK
                    _overalResult[channel].SerialNumber.State = TestStates.Pass;
                }
                else
                {
                    // NG
                    _overalResult[channel].SerialNumber.State = TestStates.Failed;
                }
            }
            else
            {
                // NG
                _overalResult[channel].SerialNumber.State = TestStates.Failed;
            }
            _overalResult[channel].SerialNumber.Value = rxString;
            _overalResult[channel].SerialNumber.Result = $"{rxString}";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_PartNumberSend(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.PartNumber.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.DTCEraseSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Part Number Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Part Number Read]");
            // 측정 상태 표시
            _overalResult[channel].PartNumber.State = TestStates.Running;
            _overalResult[channel].PartNumber.Value = "";
            _overalResult[channel].PartNumber.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].PartNumber);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_PartNumberWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 수신 관련 변수
            //char[] rxBuffer = new char[RX_BUFFER_SIZE];
            string rxString = "";
            //int rxLength = 0;
            bool bResponse = false;

            // 에러 발생 시 3번 반복한다.
            for (int errorCount = 0; errorCount < 20/*ERROR_RETRY_COUNT*/; errorCount++)
            {
                // 수신 버퍼에 남아있는 데이터를 읽어서 수신 버퍼를 비운다.
                rxString = _serialPort[channel].ReadExisting();
                rxString = string.Empty;

                Stopwatch timer = new Stopwatch();
                timer.Start();

                Send_ReadPartNumber(channel);

                while (timer.ElapsedMilliseconds < 500)
                {
                    Thread.Sleep(1);

                    if (_serialPort[channel].BytesToRead >= 10)
                    {
                        rxString = _serialPort[channel].ReadExisting();
                        if (rxString.Length >= 5)
                        {
                            GSystem.TraceMessage($"[CH.{channel + 1}] Part Number [{rxString}]");
                            bResponse = true;
                            break;
                        }
                    }
                }
                if (bResponse)
                    break;
            }
            if (!bResponse)
            {
                GSystem.TraceMessage($"[CH.{channel + 1}] Read Part Number Failed!");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Read Part Number Failed!");
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Part Number [{rxString}]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Part Number Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Part Number Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Part Number Read complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Part Number Read complete");
            // 결과 판정
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.PartNumber;
            if (rxString == testSpec.MaxString)
                _overalResult[channel].PartNumber.State = TestStates.Pass;
            else
                _overalResult[channel].PartNumber.State = TestStates.Failed;
            _overalResult[channel].PartNumber.Value = $"{rxString}";
            _overalResult[channel].PartNumber.Result = $"{rxString}";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].PartNumber);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_DTCEraseSend(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.DTC_Erase.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.OperCurrentStart);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [DTC Erase]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [DTC Erase]");
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_DTCEraseWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] DTC Erase step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] DTC Erase step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] DTC Erase complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] DTC Erase complete");
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_OperCurrentStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.OperationCurrent.Use)
            {
                // 다음 측정 항목으로 이동
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PowerOff);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Operation Current]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Operation Current]");
            // 측정 상태 표시
            _overalResult[channel].OperationCurrent.State = TestStates.Running;
            _overalResult[channel].OperationCurrent.Value = "";
            _overalResult[channel].OperationCurrent.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].OperationCurrent);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_OperCurrentWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            double operCurrent;
            if (channel == CH1)
                operCurrent = (double)(GSystem.DedicatedCTRL.Reg_03h_ch1_current_hi);
            else
                operCurrent = (double)(GSystem.DedicatedCTRL.Reg_03h_ch2_current_hi);
            if (operCurrent < OperationCurrentValue[channel])
                operCurrent = OperationCurrentValue[channel];
            GSystem.Logger.Info ($"[CH.{channel + 1}] Operation Current : [ {operCurrent:F0} mA ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Operation Current step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Operation Current step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Operation Current complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Operation Current complete");
            // 결과 판정
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.OperationCurrent;
            if (operCurrent < testSpec.MinValue || operCurrent > testSpec.MaxValue)
                _overalResult[channel].OperationCurrent.State = TestStates.Failed;
            else
                _overalResult[channel].OperationCurrent.State = TestStates.Pass;
            _overalResult[channel].OperationCurrent.Value = $"{operCurrent:F0}";
            _overalResult[channel].OperationCurrent.Result = $"{operCurrent:F0} mA";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].OperationCurrent);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_PowerOff(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power Off]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power Off]");
            GSystem.DedicatedCTRL.SetCommandDarkPowerOn(channel, false);
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
            NextTouchOnlyTestStep(channel);
            _tickStepTimeout[channel].Reset();
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_PowerOffWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PowerOff);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PowerOff);
                }
            }
            else
            {
                // 완료 대기
                if (GSystem.DedicatedCTRL.GetCommandDarkPowerOn(channel) || GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                    return;
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Power Off complete");
                NextTouchOnlyTestStep(channel);
                _tickStepInterval[channel].Reset();
            }
        }
        private void TouchOnlyTestStep_TestEndStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test End]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test End]");
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, true);
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_TestEndWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(100))
                return;
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Test End Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Test End Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TestEndStart);
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Test End Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Test End Timeout Error!");
                    // D.CTRL 통신 이상. 폴링 리셋
                    GSystem.DedicatedCTRL.StopAsync();
                    Thread.Sleep(100);
                    GSystem.DedicatedCTRL.StartAsync();
                    SetTouchOnlyTestStep(channel, TouchOnlyTestStep.TestEndStart);
                }
            }
            else
            {
                // 완료 대기
                if (!GSystem.DedicatedCTRL.GetCommandTestInit(channel) || !GSystem.DedicatedCTRL.GetCompleteTestInit(channel))
                    return;
                GSystem.Logger.Info ($"[CH.{channel + 1}] Test End step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Test End step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Test End complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Test End complete");
                GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
                NextTouchOnlyTestStep(channel);
                _tickStepInterval[channel].Reset();
            }
        }
        private void TouchOnlyTestStep_MotionUnloadingStart(int channel)
        {
            if (GSystem.MiPLC.GetCancelZDownStart(channel))
            {
                if (!GSystem.MiPLC.GetCancelZDownComplete(channel))
                    return;
                GSystem.MiPLC.SetCancelZDownStart(channel, false);
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Unloading]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Unloading]");
            //GSystem.MiPLC.SetMoveLoadStart(channel, true);
            //GSystem.MiPLC.SetUnloadingStart(channel, true);
            if (channel == GSystem.CH1)
            {
                if (!GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY))
                    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[(int)PLC_Command1_Bit.YLoadMove];
                GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[(int)PLC_Command1_Bit.UnloadingStart];
                GSystem.MiPLC.M1402_Req_Proc();
            }
            else
            {
                if (!GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY))
                    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[(int)PLC_Command1_Bit.YLoadMove];
                GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[(int)PLC_Command1_Bit.UnloadingStart];
                GSystem.MiPLC.M1402_Req_Proc();
            }
            NextTouchOnlyTestStep(channel);
        }
        private void TouchOnlyTestStep_MotionUnloadingWait(int channel)
        {
            // 완료 대기
            if (!GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY) || GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingZ))
            {
                return;
            }
            if (channel == GSystem.CH1)
            {
                GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[(int)PLC_Command1_Bit.YLoadMove];
                GSystem.MiPLC.Ch1_W_Command1 &= (ushort)~GDefines.BIT16[(int)PLC_Command1_Bit.UnloadingStart];
                GSystem.MiPLC.M1402_Req_Proc();
            }
            else
            {
                GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[(int)PLC_Command1_Bit.YLoadMove];
                GSystem.MiPLC.Ch2_W_Command1 &= (ushort)~GDefines.BIT16[(int)PLC_Command1_Bit.UnloadingStart];
                GSystem.MiPLC.M1402_Req_Proc();
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Unloading step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Unloading step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Unloading Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Unloading Complete");
            StringBuilder sb = new StringBuilder();
            sb.Append($"[CH.{channel + 1}] Total Test Elapse: [ {GSystem.TimerTestTime[channel].GetElapsedSeconds():F1} sec ]");
            GSystem.Logger.Info (sb.ToString());
            GSystem.TraceMessage(sb.ToString());
            GSystem.TimerTestTime[channel].Stop();
            NextTouchOnlyTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_Complete(int channel)
        {
            //
            // 취소 처리
            //
            if (_isCancel[channel])
            {
                //
                // TODO: 취소 처리
                //
                GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Canceled]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Canceled]");
                OnTestStateChanged(channel, TestStates.Cancel);
            }
            else
            {

                //
                // TODO: 측정 횟수, 불량률 계산, 커넥터 횟수 업데이트
                //
                GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Complete]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Complete]");


                // 전체 판정
                TestStates overalResult = TestStates.Pass;
                foreach (var testResult in _testResultsList[channel])
                {
                    if (testResult.State != TestStates.Pass)
                    {
                        overalResult = testResult.State;
                        GSystem.TraceMessage($"{testResult.Name}: {testResult.State}");
                    }
                }

                // 최종 결과 표시
                OnTestStateChanged(channel, overalResult);

                bool enableCount = true;

                // 마스터샘플 바코드
                if (channel == CH1)
                {
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType1 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode1)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType1 == "양품")
                                GSystem.MasterTestOkCh1[0] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh1[0] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType2 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode2)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType2 == "양품")
                                GSystem.MasterTestOkCh1[1] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh1[1] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType3 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode3)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType3 == "양품")
                                GSystem.MasterTestOkCh1[2] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh1[2] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType4 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode4)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType4 == "양품")
                                GSystem.MasterTestOkCh1[3] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh1[3] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType5 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode5)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType5 == "양품")
                                GSystem.MasterTestOkCh1[4] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh1[4] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                }
                else
                {
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType1 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode1)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType1 == "양품")
                                GSystem.MasterTestOkCh2[0] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh2[0] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType2 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode2)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType2 == "양품")
                                GSystem.MasterTestOkCh2[1] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh2[1] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType3 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode3)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType3 == "양품")
                                GSystem.MasterTestOkCh2[2] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh2[2] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType4 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode4)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType4 == "양품")
                                GSystem.MasterTestOkCh2[3] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh2[3] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSampleCh1.MasterType5 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSampleCh1.MasterBarcode5)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSampleCh1.MasterType5 == "양품")
                                GSystem.MasterTestOkCh2[4] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestOkCh2[4] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                }

                // 커넥터 횟수 업데이트
                if (channel == CH1)
                {
                    if (enableCount)
                    {
                        GSystem.ProductSettings.TestInfo.TestCountCh1++;
                        GSystem.ProductSettings.TestInfo.TestCountTot++;
                        if (overalResult == TestStates.Pass)
                        {
                            GSystem.ProductSettings.TestInfo.OkCountCh1++;
                            GSystem.ProductSettings.TestInfo.OkCountTot++;
                        }
                        else
                        {
                            GSystem.ProductSettings.TestInfo.NgCountCh1++;
                            GSystem.ProductSettings.TestInfo.NgCountTot++;
                        }
                        GSystem.ProductSettings.TestInfo.NgRateCh1 = (GSystem.ProductSettings.TestInfo.NgCountCh1 / (double)GSystem.ProductSettings.TestInfo.TestCountCh1) * 100;
                        GSystem.ProductSettings.TestInfo.NgRateTot = (GSystem.ProductSettings.TestInfo.NgCountTot / (double)GSystem.ProductSettings.TestInfo.TestCountTot) * 100;
                        GSystem.ProductSettings.Save(GSystem.ProductSettings.GetFileName(), GSystem.SystemData.GeneralSettings.ProductFolder);
                    }

                    GSystem.SystemData.ConnectorNFCTouch1Ch1.UseCount++;
                }
                else
                {
                    if (enableCount)
                    {
                        GSystem.ProductSettings.TestInfo.TestCountCh2++;
                        GSystem.ProductSettings.TestInfo.TestCountTot++;
                        if (overalResult == TestStates.Pass)
                        {
                            GSystem.ProductSettings.TestInfo.OkCountCh2++;
                            GSystem.ProductSettings.TestInfo.OkCountTot++;
                        }
                        else
                        {
                            GSystem.ProductSettings.TestInfo.NgCountCh2++;
                            GSystem.ProductSettings.TestInfo.NgCountTot++;
                        }
                        GSystem.ProductSettings.TestInfo.NgRateCh2 = (GSystem.ProductSettings.TestInfo.NgCountCh2 / (double)GSystem.ProductSettings.TestInfo.TestCountCh2) * 100;
                        GSystem.ProductSettings.TestInfo.NgRateTot = (GSystem.ProductSettings.TestInfo.NgCountTot / (double)GSystem.ProductSettings.TestInfo.TestCountTot) * 100;
                        GSystem.ProductSettings.Save(GSystem.ProductSettings.GetFileName(), GSystem.SystemData.GeneralSettings.ProductFolder);
                    }

                    GSystem.SystemData.ConnectorNFCTouch1Ch2.UseCount++;
                }
                GSystem.SystemData.Save();

                lock (_lock)
                { 
                    // 검사 결과 파일 저장
                    // DATA_ALL
                    DateTime saveDate = DateTime.Now;
                    string fileName = $"{saveDate:yyMMdd}_{ProductSettings.ProductInfo.PartNo}.csv";
                    string filePath = $"{GSystem.SystemData.GeneralSettings.DataFolderAll}\\{ProductSettings.ProductInfo.PartNo}\\{saveDate:yyyy}\\{saveDate:MM}";
                    string filePathName = Path.Combine(filePath, fileName);

                    bool bTitleWrite = true;
                    if (File.Exists(filePathName))
                        bTitleWrite = false;

                    GCsvFile csvFile = new GCsvFile();
                    if (csvFile.Open(fileName, filePath))
                    {
                        // 채널, 시간, 일련번호, 차종, 작업자, 테스트(이름,측정값,결과)
                        StringBuilder sb = new StringBuilder();
                        if (bTitleWrite)
                        {
                            // 타이틀
                            // 번호,제품바코드,시간,일련번호,차종,작업자, 항목,측정값,결과, ...
                            sb.Append($"No,Tray Barcode,Product Barcode,Time,Type,Worker,");
                            int index = 1;
                            foreach (var testResult in _testResultsList[channel])
                            {
                                sb.Append($"Function_{index},Measure_{index},Result_{index},");
                                index++;
                            }
                            sb.Append("Total Result");
                            sb.Append(Environment.NewLine);
                            csvFile.Write(sb.ToString());

                            // SPEC 저장
                            sb.Clear();
                            sb.Append($",,,,,,");
                            foreach (var testResult in _testResultsList[channel])
                            {
                                sb.Append($",{testResult.Min}~{testResult.Max},,");
                                index++;
                            }
                            sb.Append(Environment.NewLine);
                            csvFile.Write(sb.ToString());
                        }

                        // 측정값 저장
                        string worker = (GSystem.AdminMode) ? "Manager" : "Operator";
                        sb.Clear();
                        sb.Append($"{channel + 1},{GSystem.TrayBarcode},{GSystem.ProductBarcode[channel]},{_testStartTime[channel]:HH:mm:ss},{ProductSettings.ProductInfo.CarType},{worker},");
                        foreach (var testResult in _testResultsList[channel])
                        {
                            string judge = string.Empty;
                            if (testResult.State > TestStates.Running)
                            {
                                judge = (testResult.State == TestStates.Pass) ? "OK" : "NG";
                            }
                            if (testResult.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.RXSWIN])
                                sb.Append($"{testResult.Name},{testResult.Value},{judge},");
                            //else if (testResult.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.LockCan])
                            //    sb.Append($"{testResult.Name},{testResult.Result}[{testResult.Value}],{judge},");
                            //else if (testResult.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Cancel])
                            //    sb.Append($"{testResult.Name},{testResult.Result}[{testResult.Value}],{judge},");
                            else
                                sb.Append($"{testResult.Name},{testResult.Result},{judge},");
                        }
                        if (overalResult == TestStates.Pass)
                            sb.Append("OK");
                        else
                            sb.Append("NG");
                        sb.Append(Environment.NewLine);
                        csvFile.Write(sb.ToString());
                        csvFile.Close();
                    }
                    // DATA_PASS
                    if (overalResult == TestStates.Pass)
                    {
                        if (GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode1 &&
                            GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode2 &&
                            GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode3 &&
                            GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode4 &&
                            GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSampleCh1.MasterBarcode5)
                        {
                            // TOUCH ONLY 사양은 일련번호 쓰기가 없기 때문에 라벨에 출력하는 시리얼번호는 합격 수량으로 출력한다.
                            // 마스터샘플이 아닌 경우만 일련번호를 라벨에 출력한다.
                            uint serialNumber = GSystem.ProductSettings.TestInfo.OkCountTot;
                            // 각 채널 일련번호 표시
                            if (channel == GSystem.CH1)
                                GSystem.ProductSettings.TestInfo.SerialNumCh1 = serialNumber;
                            else
                                GSystem.ProductSettings.TestInfo.SerialNumCh2 = serialNumber;
                            GSystem.ProductSettings.Save(GSystem.ProductSettings.GetFileName(), GSystem.SystemData.GeneralSettings.ProductFolder);
                            // 라벨 출력
                            if (GSystem.ProductSettings.ProductInfo.UseLabelPrint)
                            {
                                string hwVersion = $"HW:{_overalResult[channel].HW_Version.Value.Insert(1, ".")}";
                                string swVersion = $"SW:{_overalResult[channel].SW_Version.Value}";
                                string lotNumber = $"LOT NO:{GSystem.GetLotNumber()}";
                                string sn        = $"S/N:{serialNumber:D04}";
                                string partNo    = GSystem.ProductSettings.ProductInfo.PartNo; //"82667-P8100";
                                string fccId     = GSystem.ProductSettings.LabelPrint.Payload.FCCID; //"FCC ID:2A93T-LQ2-DHS-NFC";
                                string icId      = GSystem.ProductSettings.LabelPrint.Payload.ICID; //"IC ID:30083-LQ2DHSNFC";
                                string item1     = GSystem.ProductSettings.LabelPrint.Style.Item1Text;
                                string item2     = GSystem.ProductSettings.LabelPrint.Style.Item2Text;
                                string item3     = GSystem.ProductSettings.LabelPrint.Style.Item3Text;
                                string item4     = GSystem.ProductSettings.LabelPrint.Style.Item4Text;
                                string item5     = GSystem.ProductSettings.LabelPrint.Style.Item5Text;
                                string company   = GSystem.ProductSettings.LabelPrint.Style.BrandText; //"INFAC ELECS";

                                var payload = new LabelPayload
                                {
                                    HW = hwVersion,
                                    SW = swVersion,
                                    LOT = lotNumber,
                                    SN = sn,
                                    PartNo = partNo,
                                    FCCID = fccId,
                                    ICID = icId,
                                    Item1 = item1,
                                    Item2 = item2,
                                    Item3 = item3,
                                    Item4 = item4,
                                    Item5 = item5,
                                    Company = company,
                                    DataMatrix = null
                                };

                                string EtcsVendor = (GSystem.ProductSettings.LabelPrint.Etcs.Vendor ?? "").Trim();
                                string EtcsPartNo = (GSystem.ProductSettings.LabelPrint.Etcs.PartNo ?? "").Trim();
                                string EtcsSequence = (GSystem.ProductSettings.LabelPrint.Etcs.Sequence ?? "").Trim();
                                string EtcsEo = (GSystem.ProductSettings.LabelPrint.Etcs.Eo ?? "").Trim();

                                // T = [YYMMDD] + [4M 공란] + [A/@ 1글자] + [추적번호(7 or 4)]
                                string EtcsTrace = DateTime.Now.ToString("yyMMdd"); // 생산일자: 오늘 날짜만 사용

                                // 4M은 "0000" 강제
                                string EtcsM = "0000";

                                // A or @ 은 1글자만 허용 (그 외는 공란)
                                string a1Raw = (GSystem.ProductSettings.LabelPrint.Etcs.A ?? "").Trim();
                                string EtcsA = (a1Raw == "A" || a1Raw == "@") ? a1Raw : "A";

                                // 추적번호: 숫자만 추출하여 7자리면 7자리, 아니면 4자리 0패딩
                                string serialDigits = Regex.Replace(serialNumber.ToString(), @"\D", "");
                                string EtcsSerial;
                                if (serialDigits.Length >= 7)
                                    EtcsSerial = serialDigits.Substring(serialDigits.Length - 7).PadLeft(7, '0');  // 7자리 우선
                                else
                                    EtcsSerial = serialDigits.PadLeft(4, '0');                                      // 기본 4자리

                                var etcs = new EtcsSettings
                                {
                                    Vendor = EtcsVendor,
                                    PartNo = EtcsPartNo,
                                    Sequence = EtcsSequence,
                                    Eo = EtcsEo,
                                    Trace = EtcsTrace, // 오늘 YYMMDD
                                    A = EtcsA,    // A/@ 한 글자 또는 공란
                                    M4 = EtcsM,     // 4M 공란
                                    Serial = EtcsSerial      // 7 or 4자리
                                };

                                GSystem.PrintProductLabel(
                                    payload,
                                    GSystem.ProductSettings.LabelPrint.Style,
                                    etcs: etcs,
                                    printerName: "ZDesigner ZD421-203dpi ZPL",
                                    dpi: null, darkness: null, qty: 1, speedIps: 1
                                );
                            }
                        }

                        string passFilePath = $"{GSystem.SystemData.GeneralSettings.DataFolderPass}\\{ProductSettings.ProductInfo.PartNo}\\{saveDate:yyyy}\\{saveDate:MM}";
                        string passFilePathName = Path.Combine(passFilePath, fileName);
                        try
                        {
                            if (!Directory.Exists(passFilePath))
                                Directory.CreateDirectory(passFilePath);
                            File.Copy(filePathName, passFilePathName, true);
                        }
                        catch (Exception ex)
                        {
                            // 복사 중 오류 발생
                            GSystem.Logger.Info ($"파일 복사 중 오류 발생 [{ex.Message}]");
                            GSystem.TraceMessage($"파일 복사 중 오류 발생 [{ex.Message}]");
                        }

                    }
                    // DATA_BACK
                    string backFilePath = $"{GSystem.SystemData.GeneralSettings.DataFolderBack}\\{ProductSettings.ProductInfo.PartNo}\\{saveDate:yyyy}\\{saveDate:MM}";
                    string backFilePathName = Path.Combine(backFilePath, fileName);
                    try
                    {
                        if (!Directory.Exists(backFilePath))
                            Directory.CreateDirectory(backFilePath);
                        File.Copy(filePathName, backFilePathName, true);
                    }
                    catch (Exception ex)
                    {
                        // 복사 중 오류 발생
                        GSystem.Logger.Info ($"파일 복사 중 오류 발생 [{ex.Message}]");
                        GSystem.TraceMessage($"파일 복사 중 오류 발생 [{ex.Message}]");
                    }
                    GSystem.Logger.Info ($"[CH.{channel + 1}] 테스트 결과 저장 완료 [{filePathName}]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] 테스트 결과 저장 완료 [{filePathName}]");
                }
            }
            _isCancel[channel] = false;

            // 검사 완료
            GSystem.MiPLC.SetAutoTestComplete(channel, true);

            // 검사 스텝 대기 상태로
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.Standby);
            // 스레드 종료는 하지 않는다
            //_testStepThreadExit[channel] = true;

            // 바코드 입력창 표시
            GSystem.BarcodeResetAndPopUp?.Invoke(channel);

            // 검사 종료...제품 감지 동작용
            _testComplete[channel] = true;
        }
        private void TouchOnlyTestStep_CancelStart(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Cancel Start]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Cancel Start]");
            // PLC Z축 인터락 설정 - Z축 개별 HOME
            GSystem.MiPLC.SetZInterlockIgnore(channel, false);
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.CancelZTouchStart);
            _tickStepInterval[channel].Reset();
        }
        private void TouchOnlyTestStep_CancelZTouchStart(int channel)
        {
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Cancel - Z Home Start]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Cancel - Z Home Start]");
            // TOUCH 상승
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.NFC_Z))
                        GSystem.MiPLC.SetNFCZUpStart(channel, true);
                    break;
                default:
                    if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.TouchZ))
                        GSystem.MiPLC.SetTouchZUpStart(channel, true);
                    break;
            }
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.CancelZTouchWait);
        }
        private void TouchOnlyTestStep_CancelZTouchWait(int channel)
        {
            // TOUCH 상승
            switch (GSystem.ProductSettings.ProductInfo.PartNo)
            {
                case "82657-P8000": // LQ2 Touch Only LH
                case "82667-P8000": // LQ2 Touch Only RH
                    if (GSystem.MiPLC.GetNFCZUpStart(channel))
                    {
                        if (!GSystem.MiPLC.GetNFCZUpComplete(channel))
                            return;
                        GSystem.MiPLC.SetNFCZUpStart(channel, false);
                    }
                    break;
                default:
                    if (GSystem.MiPLC.GetTouchZUpStart(channel))
                    {
                        if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                            return;
                        GSystem.MiPLC.SetTouchZUpStart(channel, false);
                    }
                    break;
            }
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.CancelZCancelStart);
        }
        private void TouchOnlyTestStep_CancelZCancelStart(int channel)
        {
            // CANCEL 하강
            if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.CancelZ))
                GSystem.MiPLC.SetCancelZDownStart(channel, true);
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.CancelZCancelWait);
        }
        private void TouchOnlyTestStep_CancelZCancelWait(int channel)
        {
            // CANCEL 하강
            if (GSystem.MiPLC.GetCancelZDownStart(channel))
            {
                if (!GSystem.MiPLC.GetCancelZDownComplete(channel))
                    return;
                GSystem.MiPLC.SetCancelZDownStart(channel, false);
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Cancel - Z Home Complete]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Cancel - Z Home Complete]");
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.CancelComplete);
        }
        private void TouchOnlyTestStep_CancelYHomeStart(int channel)
        {
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Cancel - Y Home Start]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Cancel - Y Home Start]");
            if (channel == GSystem.CH1)
            {
                if (!GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY))
                    GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[(int)PLC_Command1_Bit.YLoadMove];
                GSystem.MiPLC.Ch1_W_Command1 |= GDefines.BIT16[(int)PLC_Command1_Bit.UnloadingStart];
                GSystem.MiPLC.M1402_Req_Proc();
            }
            else
            {
                if (!GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY))
                    GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[(int)PLC_Command1_Bit.YLoadMove];
                GSystem.MiPLC.Ch2_W_Command1 |= GDefines.BIT16[(int)PLC_Command1_Bit.UnloadingStart];
                GSystem.MiPLC.M1402_Req_Proc();
            }
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.CancelYHomeWait);
        }
        private void TouchOnlyTestStep_CancelYHomeWait(int channel)
        {
            if (GSystem.MiPLC.GetMoveLoadStart(channel))
            {
                if (!GSystem.MiPLC.GetMoveLoadComplete(channel))
                    return;
                GSystem.MiPLC.SetMoveLoadStart(channel, false);
            }
            if (GSystem.MiPLC.GetUnloadingStart(channel))
            {
                if (!GSystem.MiPLC.GetUnloadingComplete(channel))
                    return;
                GSystem.MiPLC.SetUnloadingStart(channel, false);
            }
            if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingY) && 
                !GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.LoadingZ))
            {
                GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Cancel - Y Home Complete]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Cancel - Y Home Complete]");
                SetTouchOnlyTestStep(channel, TouchOnlyTestStep.CancelComplete);
            }
        }
        private void TouchOnlyTestStep_CancelComplete(int channel)
        {
            GSystem.MiPLC.SetZInterlockIgnore(channel, false);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Cancel Complete]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Cancel Complete]");
            SetTouchOnlyTestStep(channel, TouchOnlyTestStep.PowerOff);
        }

        // -----------------------------------------------------------------------------------------------
        // 이벤트를 발생시키는 메서드
        // -----------------------------------------------------------------------------------------------
        protected void OnTestStateChanged(int channel, TestStates testState)
        {
            TestStateEventArgs eventArgs = new TestStateEventArgs
            {
                Channel = channel,
                State = testState
            };
            TestStateChanged?.Invoke(this, eventArgs);
        }
        protected void OnTestStepProgressChanged(int channel, TestResult testResult)
        {
            TestStateEventArgs eventArgs = new TestStateEventArgs
            {
                Channel = channel,
                Name = testResult.Name,
                Value = testResult.Value,
                Result = testResult.Result,
                State = testResult.State
            };
            TestStepProgressChanged?.Invoke(this, eventArgs);
        }
        protected void OnRxsWinDataChanged(int channel, string rxsWinData)
        {
            TestStateEventArgs eventArgs = new TestStateEventArgs
            {
                Channel = channel,
                Result = rxsWinData
            };
            RxsWinDataChanged?.Invoke(this, eventArgs);
        }
        protected void OnLockStateChanged(int channel, int lockState)
        {
            LockStateChanged?.Invoke(this, new LockStateEventArgs(channel, lockState));
        }
        protected void OnNFCStateChanged(int channel, int nfcState)
        {
            NFCStateChanged?.Invoke(this, new LockStateEventArgs(channel, nfcState));
        }
        protected void OnShowFullProofMessage(int channel, string message)
        {

        }
        protected void OnShowFullProofMessage(int channel, string message, bool enabled)
        {
            FullProofEventArgs fullProofEventArgs = new FullProofEventArgs();
            fullProofEventArgs.Channel = channel;
            fullProofEventArgs.Message = message;
            fullProofEventArgs.Enabled = enabled;
            ShowFullProofMessage?.Invoke(this, fullProofEventArgs);
        }
        protected void OnUpdateTouchXcpData(int channel, int touchFastMutual, int touchFastSelf, int touchSlowSelf, float touchComboRate, int touchState, int intervalTime)
        {
            XcpDataEventArgs xcpTouchCancelArgs = new XcpDataEventArgs();
            xcpTouchCancelArgs.Channel = channel;
            xcpTouchCancelArgs.TouchFastMutual = touchFastMutual;
            xcpTouchCancelArgs.TouchFastSelf = touchFastSelf;
            xcpTouchCancelArgs.TouchSlowSelf = touchSlowSelf;
            xcpTouchCancelArgs.TouchComboRate = touchComboRate;
            xcpTouchCancelArgs.TouchState = touchState;
            xcpTouchCancelArgs.IntervalTime = intervalTime;
            TouchXcpDataChanged?.Invoke(this, xcpTouchCancelArgs);
        }
        protected void OnUpdateTouchMutualFast(int channel, int touchFastMutual, int touchFastSelf, int intervalTime)
        {
            XcpDataEventArgs xcpTouchCancelArgs = new XcpDataEventArgs();
            xcpTouchCancelArgs.Channel = channel;
            xcpTouchCancelArgs.TouchFastMutual = touchFastMutual;
            xcpTouchCancelArgs.TouchFastSelf = touchFastSelf;
            xcpTouchCancelArgs.IntervalTime = intervalTime;
            TouchXcpDataChanged?.Invoke(this, xcpTouchCancelArgs);
        }
        protected void OnUpdateCancelXcpData(int channel, int cancelFastSelf, int cancelSlowSelf, int cancelState, int intervalTime)
        {
            XcpDataEventArgs xcpTouchCancelArgs = new XcpDataEventArgs();
            xcpTouchCancelArgs.Channel = channel;
            xcpTouchCancelArgs.CancelFastSelf = cancelFastSelf;
            xcpTouchCancelArgs.CancelSlowSelf = cancelSlowSelf;
            xcpTouchCancelArgs.CancelState = cancelState;
            xcpTouchCancelArgs.IntervalTime = intervalTime;
            CancelXcpDataChanged?.Invoke(this, xcpTouchCancelArgs);
        }
    }
}

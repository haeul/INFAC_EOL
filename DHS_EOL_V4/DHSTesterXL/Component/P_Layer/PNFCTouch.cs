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
using Microsoft.Office.Interop.Excel;
using MS.WindowsAPICodePack.Internal;
using static System.Windows.Forms.AxHost;

namespace DHSTesterXL
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // NFCTouch
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class PNFCTouch : IDHSModel
    {
        // Delegate
        private delegate void PCanReceiveThreadFunc();
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

        // Const
        public const int SerialNumberLength = 16;

        // Handle
        private static readonly int[] _eventHandle = new int[ChannelCount] { -1, -1 };
        private static readonly int[] _portHandle = new int[ChannelCount] { -1, -1 };

        // 수신 스레드
        private Thread[] _canReceiveThread     = new Thread[ChannelCount] { null, null };
        private bool  [] _canReceiveThreadExit = new bool  [ChannelCount] { false, false };
        private readonly PCanReceiveThreadFunc[] CanReceiveThreadFunc = new PCanReceiveThreadFunc[ChannelCount] { null, null };

        // 테스트 실행 스레드
        private Thread[] _testStepThread     = new Thread[ChannelCount] { null, null };
        private bool  [] _testStepThreadExit = new bool  [ChannelCount] { false, false };
        private readonly PTestStepThreadFunc[] TestStepThreadFunc = new PTestStepThreadFunc[ChannelCount] { null, null };

        // 테스트 스텝
        private NFCTouchTestStep[] _currTestStep = new NFCTouchTestStep[ChannelCount];
        private NFCTouchTestStep[] _prevTestStep = new NFCTouchTestStep[ChannelCount];

        // 테스트 결과
        private OveralTestResult[] _overalResult = new OveralTestResult[ChannelCount] { new OveralTestResult(), new OveralTestResult() };
        List<TestResult>[] _testResultsList = new List<TestResult>[ChannelCount];
        private DateTime[] _testStartTime = new DateTime[ChannelCount] { DateTime.Now, DateTime.Now };

        // 타이머
        private TickTimer[] _tickStepElapse   = new TickTimer[ChannelCount] { new TickTimer(), new TickTimer() };
        private TickTimer[] _tickStepTimeout  = new TickTimer[ChannelCount] { new TickTimer(), new TickTimer() };
        private TickTimer[] _tickStepInterval = new TickTimer[ChannelCount] { new TickTimer(), new TickTimer() };
        private TickTimer[] _tickWakeUpNM     = new TickTimer[ChannelCount] { new TickTimer(), new TickTimer() };
        private TickTimer[] _tickXcpElapse    = new TickTimer[ChannelCount] { new TickTimer(), new TickTimer() };

        private bool[] _isOpen              = new bool[ChannelCount] { false, false };
        private bool[] _pauseNM             = new bool[ChannelCount] { false, false };
        private bool[] _checkNFC            = new bool[ChannelCount] { false, false };
        private int [] _retryCount          = new int [ChannelCount];
        private int [] _serialNumberRxCount = new int [ChannelCount];
        private int [] _rxsWinByteCount     = new int [ChannelCount];
        private int [] _rxsWinRecvCount     = new int [ChannelCount];
        private uint[] _bootReqID           = new uint[ChannelCount];
        private uint[] _bootResID           = new uint[ChannelCount];
        private byte[] _securityBit         = new byte[ChannelCount];
        private int [] _canLockState        = new int [ChannelCount];
        private int [] _canNFC_State        = new int [ChannelCount];
        private bool[] _isCancel            = new bool[ChannelCount];

        private byte[,] _receivedSeedkey    = new byte[ChannelCount,   8];
        private byte[,] _generatedSeedkey   = new byte[ChannelCount,   8];
        private byte[,] _serialNumberBytes  = new byte[ChannelCount,  16];
        private byte[,] _manufactureBytes   = new byte[ChannelCount,   8];
        private byte[,] _hwVersionBytes     = new byte[ChannelCount,   3];
        private byte[,] _swVersionBytes     = new byte[ChannelCount,   4];
        private byte[,] _partNumberBytes    = new byte[ChannelCount,  10];
        private byte[,] _rxsWinBytes        = new byte[ChannelCount, 200];

        private string _nextSerialNo = "";

        public List<TestSpec> TestItemsList { get; set; }

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
        private int PLightTurnOnValue { get; set; }
        private int PLightCurrentValue { get; set; }
        private int PLightAmbientValue { get; set; }

        public int Channel { get; set; }


        // External Import
        [DllImport("HKMC_AdvancedSeedKey_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SEEDKEY_RT ASK_KeyGenerate(ref UInt64 seed, ref UInt64 key);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WaitForSingleObject(int handle, int timeOut);


        // -------------------------------------------------------------------------------------------------
        public PNFCTouch()
        {
            CanReceiveThreadFunc[CH1] = CanReceiveThreadCh1;
            CanReceiveThreadFunc[CH2] = CanReceiveThreadCh2;
            TestStepThreadFunc[CH1] = TestStepThreadCh1;
            TestStepThreadFunc[CH2] = TestStepThreadCh2;
        }

        // -------------------------------------------------------------------------------------------------
        public void Dispose()
        {

        }


        // -------------------------------------------------------------------------------------------------
        public Task<XL_Status> OpenPort(int channel)
        {
            // Open port

            XL_Status status = XL_Status.XL_ERROR;

            TraceMessage($"OpenPort - NFCTouch");

            status = CanXL.Driver.XL_OpenPort(ref _portHandle[channel], CanXL.AppName, CanXL._accessMask[channel], 
                ref CanXL._permissionMask[channel], 1024, XL_InterfaceVersion.XL_INTERFACE_VERSION, XL_BusTypes.XL_BUS_TYPE_CAN);

            GSystem.Logger.Info ($"Open Port             : [CH.{channel + 1}] {status}");
            GSystem.TraceMessage($"Open Port             : [CH.{channel + 1}] {status}");
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                CanXL.PrintFunctionError();
                return Task.FromResult(status);
            }

            // Check port
            status = CanXL.Driver.XL_CanRequestChipState(_portHandle[channel], CanXL._accessMask[channel]);
            GSystem.Logger.Info ($"Can Request Chip State: [CH.{channel + 1}] {status}");
            GSystem.TraceMessage($"Can Request Chip State: [CH.{channel + 1}] {status}");
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                CanXL.PrintFunctionError();
                return Task.FromResult(status);
            }

            uint baudrate = Convert.ToUInt32(ProductSettings.CommSettings.ArbtBitRate);
            status = CanXL.Driver.XL_CanSetChannelBitrate(_portHandle[channel], CanXL._accessMask[channel], baudrate);
            GSystem.Logger.Info ($"Set Baudrate          : [CH.{channel + 1}] {status}, baudrate: {baudrate}");
            GSystem.TraceMessage($"Set Baudrate          : [CH.{channel + 1}] {status}, baudrate: {baudrate}");
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                CanXL.PrintFunctionError();
                return Task.FromResult(status);
            }

            // Activate channel
            status = CanXL.Driver.XL_ActivateChannel(_portHandle[channel], CanXL._accessMask[channel], XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);
            GSystem.Logger.Info ($"Activate Channel      : [CH.{channel + 1}] {status}");
            GSystem.TraceMessage($"Activate Channel      : [CH.{channel + 1}] {status}");
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                CanXL.PrintFunctionError();
                return Task.FromResult(status);
            }

            // Get RX event handle
            status = CanXL.Driver.XL_SetNotification(_portHandle[channel], ref _eventHandle[channel], 1);
            GSystem.Logger.Info ($"Set Notification      : [CH.{channel + 1}] {status}");
            GSystem.TraceMessage($"Set Notification      : [CH.{channel + 1}] {status}");
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                CanXL.PrintFunctionError();
                return Task.FromResult(status);
            }

            // Reset time stamp clock
            status = CanXL.Driver.XL_ResetClock(_portHandle[channel]);
            GSystem.Logger.Info ($"Reset Clock           : [CH.{channel + 1}] {status}");
            GSystem.TraceMessage($"Reset Clock           : [CH.{channel + 1}] {status}");
            if (status != XLDefine.XL_Status.XL_SUCCESS) CanXL.PrintFunctionError();

            _isOpen[channel] = true;

            // 수신 스레드 생성
            StartCanReceiveThread(channel);

            return Task.FromResult(status);
        }

        // -------------------------------------------------------------------------------------------------
        public Task<XL_Status> ClosePort(int channel)
        {
            // Close port

            _isOpen[channel] = false;

            TraceMessage($"ClosePort - NFCTouch");

            // 스레드 종료
            if (_testStepThread[channel] != null)
            {
                _testStepThreadExit[channel] = true;
                _testStepThread[channel].Join(1000);
            }
            // 수신 스레드 종료
            StopCanReceiveThread(channel);

            XL_Status status = CanXL.Driver.XL_ClosePort(_portHandle[channel]);
            GSystem.Logger.Info ($"Close Port            : [CH.{channel + 1}] {status}");
            GSystem.TraceMessage($"Close Port            : [CH.{channel + 1}] {status}");

            return Task.FromResult(status);
        }

        // -------------------------------------------------------------------------------------------------
        public bool OpenPortCOM(int channel)
        {
            return false;
        }
        // -------------------------------------------------------------------------------------------------
        public void ClosePortCOM(int channel)
        {
            
        }

        // -------------------------------------------------------------------------------------------------
        public bool IsOpen(int channel)
        {
            return _isOpen[channel];
        }

        // -----------------------------------------------------------------------------------------------
        public uint GetCanID(uint canId)
        {
            return canId & 0x7FFFFFFF;
        }

        // -----------------------------------------------------------------------------------------------
        public int GetLengthDLC(XL_CANFD_DLC dlc)
        {
            int length = 0;

            switch (dlc)
            {
                case XL_CANFD_DLC.DLC_CAN_CANFD_0_BYTES:
                case XL_CANFD_DLC.DLC_CAN_CANFD_1_BYTES:
                case XL_CANFD_DLC.DLC_CAN_CANFD_2_BYTES:
                case XL_CANFD_DLC.DLC_CAN_CANFD_3_BYTES:
                case XL_CANFD_DLC.DLC_CAN_CANFD_4_BYTES:
                case XL_CANFD_DLC.DLC_CAN_CANFD_5_BYTES:
                case XL_CANFD_DLC.DLC_CAN_CANFD_6_BYTES:
                case XL_CANFD_DLC.DLC_CAN_CANFD_7_BYTES:
                case XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES:
                    length = (int)dlc; break;
                case XL_CANFD_DLC.DLC_CANFD_12_BYTES: length = 12; break;
                case XL_CANFD_DLC.DLC_CANFD_16_BYTES: length = 16; break;
                case XL_CANFD_DLC.DLC_CANFD_20_BYTES: length = 20; break;
                case XL_CANFD_DLC.DLC_CANFD_24_BYTES: length = 24; break;
                case XL_CANFD_DLC.DLC_CANFD_32_BYTES: length = 32; break;
                case XL_CANFD_DLC.DLC_CANFD_48_BYTES: length = 48; break;
                case XL_CANFD_DLC.DLC_CANFD_64_BYTES: length = 64; break;
                default:
                    break;
            }

            return length;
        }

        // -----------------------------------------------------------------------------------------------
        public StringBuilder GetEventString(xl_event xlEvent)
        {
            int dlcLength = xlEvent.tagData.can_Msg.dlc;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{xlEvent.tagData.can_Msg.id,4:X}h  {dlcLength,2}  ");
            for (int i = 0; i < dlcLength; i++)
                stringBuilder.Append($"{xlEvent.tagData.can_Msg.data[i]:X02} ");
            return stringBuilder;
        }
        public StringBuilder GetEventString(XLcanTxEvent xlEvent)
        {
            int dlcLength = GetLengthDLC(xlEvent.tagData.dlc);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{GetCanID(xlEvent.tagData.canId),4:X}h  {dlcLength,2}  ");
            for (int i = 0; i < dlcLength; i++)
                stringBuilder.Append($"{xlEvent.tagData.data[i]:X02} ");
            return stringBuilder;
        }

        // -----------------------------------------------------------------------------------------------
        public StringBuilder GetTxEventString(xl_event txEvent)
        {
            int dlcLength = txEvent.tagData.can_Msg.dlc;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Tx:  {txEvent.tagData.can_Msg.id,8:X}h  {dlcLength,2}  ");
            for (int i = 0; i < dlcLength; i++)
                stringBuilder.Append($"{txEvent.tagData.can_Msg.data[i]:X02} ");

            return stringBuilder;
        }
        public StringBuilder GetTxEventString(XLcanTxEvent txEvent)
        {
            int dlcLength = GetLengthDLC(txEvent.tagData.dlc);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Tx:  {GetCanID(txEvent.tagData.canId),8:X}h  {dlcLength,2}  ");
            for (int i = 0; i < dlcLength; i++)
                stringBuilder.Append($"{txEvent.tagData.data[i]:X02} ");

            return stringBuilder;
        }

        // -----------------------------------------------------------------------------------------------
        public StringBuilder GetRxEventString(xl_event rxEvent)
        {
            int dlcLength = (rxEvent.tagData.can_Msg.dlc);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Rx:  {rxEvent.tagData.can_Msg.id,8:X}h  {dlcLength,2}  ");
            for (int i = 0; i < dlcLength; i++)
                stringBuilder.Append($"{rxEvent.tagData.can_Msg.data[i]:X02} ");

            return stringBuilder;
        }
        public StringBuilder GetRxEventString(XLcanRxEvent rxEvent)
        {
            int dlcLength = GetLengthDLC(rxEvent.tagData.canRxOkMsg.dlc);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Rx:  {GetCanID(rxEvent.tagData.canRxOkMsg.canId),8:X}h  {dlcLength,2}  ");
            for (int i = 0; i < dlcLength; i++)
                stringBuilder.Append($"{rxEvent.tagData.canRxOkMsg.data[i]:X02} ");

            return stringBuilder;
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status WriteFrame(int channel, UInt32 id, ushort dlc, byte[] data, bool logging = false, string remarks = "")
        {
            XL_Status txStatus;

            // Create an event collection with 1 messages (events)
            xl_event_collection xlEventCollection = new xl_event_collection(1);

            // event 1
            xlEventCollection.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;
            xlEventCollection.xlEvent[0].tagData.can_Msg.id = id;
            xlEventCollection.xlEvent[0].tagData.can_Msg.dlc = dlc;
            for (int i = 0; i < data.Length; i++)
            {
                xlEventCollection.xlEvent[0].tagData.can_Msg.data[i] = data[i];
            }

            // Transmit events
            txStatus = CanXL.Driver.XL_CanTransmit(_portHandle[channel], CanXL._channelMask[channel], xlEventCollection);

            if (logging)
            {
                if (remarks != "")
                    Logger.Info(remarks);
                StringBuilder logString = GetTxEventString(xlEventCollection.xlEvent[0]);
                Logger.Info(logString);
                TraceMessage(logString.ToString());
            }
            return txStatus;
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status WriteFrameStd(int channel, UInt32 id, XL_CANFD_DLC dlc, byte[] data, bool logging = false, string remarks = "")
        {
            XL_Status txStatus;

            // Create an event collection with 1 messages (events)
            xl_canfd_event_collection xlEventCollection = new xl_canfd_event_collection(1);

            // event 1
            xlEventCollection.xlCANFDEvent[0].tag = XL_CANFD_TX_EventTags.XL_CAN_EV_TAG_TX_MSG;
            xlEventCollection.xlCANFDEvent[0].tagData.canId = (uint)XL_MessageFlagsExtended.XL_CAN_EXT_MSG_ID | id;
            xlEventCollection.xlCANFDEvent[0].tagData.dlc = dlc;
            xlEventCollection.xlCANFDEvent[0].tagData.msgFlags = XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_BRS | XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_EDL;
            for (int i = 0; i < data.Length; i++)
            {
                xlEventCollection.xlCANFDEvent[0].tagData.data[i] = data[i];
            }

            // Transmit events
            uint messageCounterSent = 0;
            txStatus = CanXL.Driver.XL_CanTransmitEx(_portHandle[channel], CanXL._channelMask[channel], ref messageCounterSent, xlEventCollection);

            if (logging)
            {
                if (remarks != "")
                    Logger.Info(remarks);
                StringBuilder logString = GetTxEventString(xlEventCollection.xlCANFDEvent[0]);
                Logger.Info(logString);
            }
            return txStatus;
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status WriteFrameExt(int channel, UInt32 id, XL_CANFD_DLC dlc, byte[] data, bool logging = false, string remarks = "")
        {
            XL_Status status;

            // Create an event collection with 1 messages (events)
            xl_canfd_event_collection xlEventCollection = new xl_canfd_event_collection(1);

            // event 1
            xlEventCollection.xlCANFDEvent[0].tag = XL_CANFD_TX_EventTags.XL_CAN_EV_TAG_TX_MSG;
            xlEventCollection.xlCANFDEvent[0].tagData.canId = (uint)XL_MessageFlagsExtended.XL_CAN_EXT_MSG_ID | id;
            xlEventCollection.xlCANFDEvent[0].tagData.dlc = dlc;
            xlEventCollection.xlCANFDEvent[0].tagData.msgFlags = XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_BRS | XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_EDL;
            for (int i = 0; i < data.Length; i++)
            {
                xlEventCollection.xlCANFDEvent[0].tagData.data[i] = data[i];
            }

            // Transmit events
            uint messageCounterSent = 0;
            status = CanXL.Driver.XL_CanTransmitEx(_portHandle[channel], CanXL._channelMask[channel], ref messageCounterSent, xlEventCollection);

            if (logging)
            {
                if (remarks != "")
                    Logger.Info(remarks);
                StringBuilder logString = GetTxEventString(xlEventCollection.xlCANFDEvent[0]);
                Logger.Info(logString);
            }
            return status;
        }

        // -----------------------------------------------------------------------------------------------
        private byte _commandCount = 0;
        public byte GetNextCommandCount()
        {
            _commandCount = (byte)((_commandCount + 1) & 0xFF);
            return _commandCount;
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_NM(int channel, bool logging = false)
        {
            uint id = ProductSettings.NM_ReqID;
            ushort dlc = 8;
            byte[] data = new byte[]
            {
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: WAKE_UP");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_NFC(int channel, bool logging = false)
        {
            uint id = ProductSettings.NFC_ReqID;
            ushort dlc = 8;
            byte[] data = new byte[]
            {
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: NFC_INPUT");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_DefaultSession(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x02, 0x10, 0x01, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: DEFAULT_SESSION");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_ExtendedSession(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x02, 0x10, 0x03, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: EXTENDED_SESSION");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_Bootloader(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x02, 0x10, 0x02, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: BOOTLOADER");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_RequestSeedkey(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x02, 0x27, 0x11, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: REQUEST_SEEDKEY");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_RequestSeedkeyFlow(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x30, 0x01, 0x0A, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging);
        }

        // -----------------------------------------------------------------------------------------------
        public string GenerateAdvancedSeedKey(ref byte[] receivedSeedkey, ref byte[] generatedSeedkey)
        {
            SEEDKEY_RT ret = SEEDKEY_RT.SEEDKEY_FAIL;

            try
            {
                // 배열을 Uint64(8바이트)형으로 변환
                UInt64 In = BitConverter.ToUInt64(receivedSeedkey, 0);
                UInt64 Out = 0;
                ret = ASK_KeyGenerate(ref In, ref Out);

                if (ret == SEEDKEY_RT.SEEDKEY_SUCCESS)
                {
                    generatedSeedkey = BitConverter.GetBytes(Out);
                }
                else
                {
                    GSystem.Logger.Info($"GenerateAdvancedSeedKey fail. [{ret}]");
                }
            }
            catch (Exception ex)
            {
                //throw;
                GSystem.Logger.Info(ex.Message);
            }

            return ret.ToString();
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_GeneratedSeedkey(int channel, byte[] seedkey, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x10, 0x0A, 0x27, 0x12, seedkey[0], seedkey[1], seedkey[2], seedkey[3]// 4자리
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: SEND_GENERATED_SEEDKEY");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_GeneratedSeedkey2(int channel, byte[] seedkey, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x21, seedkey[4], seedkey[5], seedkey[6], seedkey[7], 0xAA, 0xAA, 0xAA // 4자리
            };
            return WriteFrame(channel, id, dlc, data, logging);
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_SerialNumberWrite(int channel, byte[] serialNumber, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x10, 0x13, 0x2E, 0xF1, 0x8C, serialNumber[0], serialNumber[1], serialNumber[2]
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: SERIAL_NUMBER_WRITE");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_SerialNumberRead(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x03, 0x22, 0xF1, 0x8C, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: SERIAL_NUMBER_READ");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_ManufactureWrite(int channel, byte[] manufacture, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x07, 0x2E, 0xF1, 0x8B, manufacture[0], manufacture[1], manufacture[2], manufacture[3]
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: MANUFACTURE_WRITE");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_ManufactureRead(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x03, 0x22, 0xF1, 0x8B, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: MANUFACTURE_READ");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_PLight(int channel, bool onOff, bool logging = false)
        {
            uint id = ProductSettings.PLightReqID;
            ushort dlc = 8;
            byte lamp;
            if (onOff)
                lamp = 0x80;
            else
                lamp = 0;
            byte[] data = new byte[8]
            {
                lamp, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            string state = (onOff) ? "ON" : "OFF";
            return WriteFrame(channel, id, dlc, data, logging, $"CMD: P-LIGHT_{state}");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_DTC_Erase(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x04, 0x14, 0xFF, 0xFF, 0xFF, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: DTC_ERASE"); ;
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_HWVersion(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x03, 0x22, 0xF1, 0x93, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: HW_VERSION_READ");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_SWVersion(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x03, 0x22, 0xF1, 0xB1, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: SW_VERSION_READ");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_PartNumber(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x03, 0x22, 0xF1, 0x87, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: PART_NUMBER_READ");
        }
        public XL_Status Send_PartNumberFlow(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x30, 0x01, 0x0A, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging);
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_RXSWin(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x03, 0x22, 0xF1, 0xEF, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: RXSWIN");
        }
        public XL_Status Send_RXSWinFlow(int channel, byte interval, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x30, 0x11, interval, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging);
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_SupplierCodeRead(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            ushort dlc = 8;
            byte[] data = new byte[8]
            {
                0x03, 0x22, 0xF1, 0xA1, 0xAA, 0xAA, 0xAA, 0xAA
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: SUPPLIER_CODE_READ");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_XCPConnect(int channel, bool logging = false)
        {
            uint id = ProductSettings.XcpReqID;
            ushort dlc = 8;
            byte cmd_count = GetNextCommandCount();
            byte ecuAddr = ProductSettings.XcpEcuAddr;
            byte[] data = new byte[8]
            {
                0x01, cmd_count, ecuAddr, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: CCP_CONNECT");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_XCPDisconnect(int channel, bool logging = false)
        {
            uint id = ProductSettings.XcpReqID;
            ushort dlc = 8;
            byte cmd_count = GetNextCommandCount();
            byte[] data = new byte[8]
            {
                0x07, cmd_count, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            return WriteFrame(channel, id, dlc, data, logging, "CMD: CCP_DICONNECT");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_Upload(int channel, byte count, bool logging = false)
        {
            uint id = ProductSettings.XcpReqID;
            ushort dlc = 8;
            byte cmd_count = GetNextCommandCount();
            byte[] data = new byte[8]
            {
                0x04, cmd_count, count, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            return WriteFrame(channel, id, dlc, data, logging, $"CMD: UPLOAD size={count}");
        }
        public XL_Status Send_UploadCapa(int channel, byte count, bool logging = false)
        {
            uint id = ProductSettings.XcpReqID;
            ushort dlc = 8;
            // Create an event collection with 1 messages (events)
            XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(1);
            xlEventCollection.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;
            xlEventCollection.xlEvent[0].tagData.can_Msg.id = id;
            xlEventCollection.xlEvent[0].tagData.can_Msg.dlc = dlc;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[0] = 0x04;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[1] = GetNextCommandCount();
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[2] = count;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[3] = 0x00;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[4] = 0x00;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[5] = 0x00;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[6] = 0x00;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[7] = 0x00;
            // Transmit events
            XL_Status statusResult = CanXL.Driver.XL_CanTransmit(_portHandle[channel], CanXL._channelMask[channel], xlEventCollection);
            //if (logging)
            //{
            //    string logString = $"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Tx {GetEventString(xlEventCollection.xlEvent[0])}  UPLOAD size={count}";
            //    GSystem.CapaData[channel].Info(logString.ToString());
            //}
            return statusResult;
        }
        public XL_Status Send_ShortUpload(int channel, uint address, byte count, bool logging = false, string remarks = "")
        {
            uint id = ProductSettings.XcpReqID;
            byte[] byteArray = BitConverter.GetBytes(address);
            ushort dlc = 8;
            // Create an event collection with 1 messages (events)
            XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(1);
            xlEventCollection.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;
            xlEventCollection.xlEvent[0].tagData.can_Msg.id = id;
            xlEventCollection.xlEvent[0].tagData.can_Msg.dlc = dlc;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[0] = 0x0F;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[1] = GetNextCommandCount();
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[2] = count;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[3] = 0x00;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[4] = byteArray[0];
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[5] = byteArray[1];
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[6] = byteArray[2];
            xlEventCollection.xlEvent[0].tagData.can_Msg.data[7] = byteArray[3];
            // Transmit events
            XL_Status statusResult = CanXL.Driver.XL_CanTransmit(_portHandle[channel], CanXL._channelMask[channel], xlEventCollection);
            //if (logging)
            //{
            //    string logString = $"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Tx {GetEventString(xlEventCollection.xlEvent[0])}  UPLOAD size={count}";
            //    GSystem.CapaData[channel].Info(logString.ToString());
            //}
            return statusResult;
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_SetMTA(int channel, uint address, bool logging = false, string remarks = "")
        {
            uint id = ProductSettings.XcpReqID;
            byte[] byteArray = BitConverter.GetBytes(address);
            ushort dlc = 8;
            byte cmd_count = GetNextCommandCount();
            byte[] data = new byte[8]
            {
                0x02, cmd_count, 0x00, 0x00, byteArray[0], byteArray[1], byteArray[2], byteArray[3]
            };
            // Create an event collection with 1 messages (events)
            XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(1);
            xlEventCollection.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;
            xlEventCollection.xlEvent[0].tagData.can_Msg.id = id;
            xlEventCollection.xlEvent[0].tagData.can_Msg.dlc = dlc;
            for (int i = 0; i < data.Length; i++)
            {
                xlEventCollection.xlEvent[0].tagData.can_Msg.data[i] = data[i];
            }
            // Transmit events
            XL_Status statusResult = CanXL.Driver.XL_CanTransmit(_portHandle[channel], CanXL._channelMask[channel], xlEventCollection);
            //if (logging)
            //{
            //    string logString = $"{_tickXcpElapse[channel].GetTotalSeconds():F6} CH.{channel + 1} Tx {GetEventString(xlEventCollection.xlEvent[0])}  SET_MTA addr=0x{address:X08} {remarks}";
            //    GSystem.CapaData[channel].Info(logString.ToString());
            //    //GSystem.TraceMessage(logString.ToString());
            //}
            return statusResult;
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_Security(int channel, uint address, bool logging = false)
        {
            uint id = ProductSettings.XcpReqID;
            byte[] byteArray = BitConverter.GetBytes(address);
            ushort dlc = 8;
            byte cmd_count = GetNextCommandCount();
            byte[] data = new byte[8]
            {
                0x02, cmd_count, 0x00, 0x00, byteArray[0], byteArray[1], byteArray[2], byteArray[3]
            };
            return WriteFrame(channel, id, dlc, data, logging, $"CMD: SECURITY addr=0x{address:X08}");
        }

        // -----------------------------------------------------------------------------------------------
        public XL_Status Send_HardwireTest(int channel, bool logging = false)
        {
            uint id = ProductSettings.CanReqID;
            XL_CANFD_DLC dlc = XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES;
            byte[] data = new byte[8]
            {
                0x03, 0x2F, 0xF1, 0x11, 0x03, 0xAA, 0xAA, 0xAA
            };
            return WriteFrameStd(channel, id, dlc, data, logging, "CMD: HARDWIRE_TEST");
        }



        // -----------------------------------------------------------------------------------------------
        // 수신 스레드
        // -----------------------------------------------------------------------------------------------
        public void StartCanReceiveThread(int channel)
        {
            if (_canReceiveThread[channel] == null)
            {
                _canReceiveThread[channel] = new Thread(new ThreadStart(CanReceiveThreadFunc[channel]));
                _canReceiveThread[channel].IsBackground = true;
                _canReceiveThread[channel].Start();
            }
        }

        public void StopCanReceiveThread(int channel)
        {
            if (_canReceiveThread[channel] != null)
            {
                _canReceiveThreadExit[channel] = true;
                _canReceiveThread[channel].Join(500);
            }
        }

        // -----------------------------------------------------------------------------------------------
        private void CanReceiveThreadCh1()
        {
            int channel = CH1;
            GSystem.Logger.Info($"[CH.{channel + 1}] Can Receive Thread start...");
            GSystem.TraceMessage($"[CH.{channel + 1}] Can Receive Thread start...");

            _canReceiveThreadExit[channel] = false;

            // Create new object containing received data 
            XLClass.xl_event receivedEvent = new XLClass.xl_event();
            // Result of XL Driver function calls
            XLDefine.XL_Status xlStatus = XLDefine.XL_Status.XL_SUCCESS;
            // Result values of WaitForSingleObject 
            XLDefine.WaitResults waitResult = new XLDefine.WaitResults();

            GSystem.TraceMessage($"{xlStatus}");
            do
            {
                // Wait for hardware events
                waitResult = (XLDefine.WaitResults)WaitForSingleObject(_eventHandle[channel], 300);

                // If event occurred...
                if (waitResult != XLDefine.WaitResults.WAIT_TIMEOUT)
                {
                    // ...init xlStatus first
                    xlStatus = XLDefine.XL_Status.XL_SUCCESS;

                    // afterwards: while hw queue is not empty...
                    while (xlStatus != XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                    {
                        // ...receive data from hardware.
                        xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);

                        //  If receiving succeed....
                        if (xlStatus == XLDefine.XL_Status.XL_SUCCESS)
                        {
                            //GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
                            uint rxCanID = receivedEvent.tagData.can_Msg.id & 0x7FFFFFFF;
                            if (rxCanID > 0x100)
                            {
                                TestStepRxProc(channel, rxCanID, ref receivedEvent);
                            }
                        }
                    }
                }
                // No event occurred
            } while (!_canReceiveThreadExit[channel]);

            _canReceiveThreadExit[channel] = false;
            _canReceiveThread[channel] = null;

            GSystem.Logger.Info ($"[CH.{channel + 1}] Can Receive Thread terminated!");
            GSystem.TraceMessage($"[CH.{channel + 1}] Can Receive Thread terminated!");
        }

        // -----------------------------------------------------------------------------------------------
        private void CanReceiveThreadCh2()
        {
            int channel = CH2;
            GSystem.Logger.Info($"[CH.{channel + 1}] Can Receive Thread start...");
            GSystem.TraceMessage($"[CH.{channel + 1}] Can Receive Thread start...");

            _canReceiveThreadExit[channel] = false;

            // Create new object containing received data 
            XLClass.xl_event receivedEvent = new XLClass.xl_event();
            // Result of XL Driver function calls
            XLDefine.XL_Status xlStatus = XLDefine.XL_Status.XL_SUCCESS;
            // Result values of WaitForSingleObject 
            XLDefine.WaitResults waitResult = new XLDefine.WaitResults();

            do
            {
                // Wait for hardware events
                waitResult = (XLDefine.WaitResults)WaitForSingleObject(_eventHandle[channel], 300);

                // If event occurred...
                if (waitResult != XLDefine.WaitResults.WAIT_TIMEOUT)
                {
                    // ...init xlStatus first
                    xlStatus = XLDefine.XL_Status.XL_SUCCESS;

                    // afterwards: while hw queue is not empty...
                    while (xlStatus != XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                    {
                        // ...receive data from hardware.
                        xlStatus = CanXL.Driver.XL_Receive(_portHandle[channel], ref receivedEvent);

                        //  If receiving succeed....
                        if (xlStatus == XLDefine.XL_Status.XL_SUCCESS)
                        {
                            //GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
                            uint rxCanID = receivedEvent.tagData.can_Msg.id & 0x7FFFFFFF;
                            if (rxCanID > 0x100)
                            {
                                TestStepRxProc(channel, rxCanID, ref receivedEvent);
                            }
                        }
                    }
                }
                // No event occurred
            } while (!_canReceiveThreadExit[channel]);

            _canReceiveThreadExit[channel] = false;
            _canReceiveThread[channel] = null;

            GSystem.Logger.Info ($"[CH.{channel + 1}] Can Receive Thread terminated!");
            GSystem.TraceMessage($"[CH.{channel + 1}] Can Receive Thread terminated!");
        }



        // -----------------------------------------------------------------------------------------------
        // 테스트 실행 스레드
        // -----------------------------------------------------------------------------------------------
        public NFCTouchTestStep NextTestStep(int channel)
        {
            _prevTestStep[channel] = _currTestStep[channel];
            _currTestStep[channel] = (NFCTouchTestStep)((int)_currTestStep[channel] + 1);
            return _currTestStep[channel];
        }
        public NFCTouchTestStep GetTestStep(int channel)
        {
            return _currTestStep[channel];
        }
        public NFCTouchTestStep SetTestStep(int channel, NFCTouchTestStep step)
        {
            _prevTestStep[channel] = _currTestStep[channel];
            _currTestStep[channel] = step;
            return _prevTestStep[channel];
        }
        public TouchOnlyTestStep NextTouchOnlyTestStep(int channel)
        {
            throw new NotImplementedException();
        }
        public TouchOnlyTestStep GetTouchOnlyTestStep(int channel)
        {
            throw new NotImplementedException();
        }
        public TouchOnlyTestStep SetTouchOnlyTestStep(int channel, TouchOnlyTestStep step)
        {
            throw new NotImplementedException();
        }

        public void StartThread(int channel)
        {
            if (_testStepThread[channel] == null)
            {
                _testStepThread[channel] = new Thread(new ThreadStart(TestStepThreadFunc[channel]));
                _testStepThread[channel].IsBackground = true;
                _testStepThread[channel].Start();
            }
            _currTestStep[channel] = NFCTouchTestStep.Standby;
        }

        public void StartTest(int channel)
        {
            if (_testStepThread[channel] == null)
            {
                _testStepThread[channel] = new Thread(new ThreadStart(TestStepThreadFunc[channel]));
                _testStepThread[channel].IsBackground = true;
                _testStepThread[channel].Start();
            }
            _currTestStep[channel] = NFCTouchTestStep.Prepare;
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
            if (cancel && GetTestStep(channel) > NFCTouchTestStep.Prepare)
                SetTestStep(channel, NFCTouchTestStep.PowerOff);
        }


        // -----------------------------------------------------------------------------------------------
        private void TestStepThreadCh1()
        {
            int channel = CH1;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step Thread start...");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step Thread start...");

            _testStepThreadExit[channel] = false;

            while (!_testStepThreadExit[channel])
            {
                Thread.Sleep(0);
                if (!_pauseNM[channel])
                    WakeUpProc(channel);
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
            int channel = CH2;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step Thread start...");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step Thread start...");

            _testStepThreadExit[channel] = false;

            while (!_testStepThreadExit[channel])
            {
                Thread.Sleep(0);
                if (!_pauseNM[channel])
                    WakeUpProc(channel);
                TestStepTxProc(channel);
            }

            _testStepThreadExit[channel] = false;
            _testStepThread[channel] = null;

            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step Thread terminated!");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step Thread terminated!");
        }

        // -----------------------------------------------------------------------------------------------
        private void WakeUpProc(int channel)
        {
            if (_tickWakeUpNM[channel].MoreThan(200))
            {
                _tickWakeUpNM[channel].Reset();
                // NM 명령 전송
                Send_NM(channel);
            }
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

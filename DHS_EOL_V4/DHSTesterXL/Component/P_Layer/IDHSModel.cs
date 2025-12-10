using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vxlapi_NET;
using static vxlapi_NET.XLClass;
using static vxlapi_NET.XLDefine;
using static DHSTesterXL.GSystem;
using System.Threading;

namespace DHSTesterXL
{
    public enum CommunicationType
    {
        CAN_FD,
        CAN_HS,
        UART
    }

    public enum SEEDKEY_RT
    {
        SEEDKEY_SUCCESS = 0,
        SEEDKEY_FAIL = 1,
    };

    public enum TouchOnlyTestStep
    {
        Standby = 0,
        Prepare,
        TestInitStart,
        TestInitWait,
        // Motion Loading
        MotionLoadingStart,
        MotionLoadingWait,
        ShortTestStart,
        ShortTestWait,
        LowPowerOn,
        LowPowerOnWait,
        WakeUpSend,
        WakeUpWait,
        DarkCurrentStart,
        DarkCurrentWait,
        DarkCurrentUpdate,
        DarkCurrentComplete,
        DarkPowerOff,
        DarkPowerOffWait,
        HighPowerOn,
        HighPowerOnWait,
        PowerOnResetWait,
        PLightTurnOnSend,
        PLightTurnOnWait,
        PLightCurrentSend,
        PLightCurrentWait,
        PLightAmbientSend,
        PLightAmbientWait,
        PLightTurnOffSend, // 전원도 같이 OFF 한다
        PLightTurnOffWait,
        // 터치 위치로 이동 전 전원 ON
        TouchLockPowerOnStart,
        TouchLockPowerOnWait,
        // Power On Reset 대기
        TouchLockPowerOnReset,
        // Motion Move to Touch Y
        MotionMoveTouchStart,
        MotionMoveTouchWait,
        TouchLockStart,
        // Touch
        TouchLockZDown,
        TouchLockWait,
        TouchLockRetry,
        TouchLockRetry1,
        TouchLockRetry2,
        MotionTouchZUpStart,
        MotionTouchZUpWait,
        // Motion Move to Cancel Y
        MotionMoveCancelStart,
        MotionMoveCancelWait,
        // Lock/Cancel
        LockCancelStart,
        LockCancelZUp,
        LockTouchZDown,
        LockCancelTouchCheck,
        LockCancelWait,
        LockCancelRetry,
        LockCancelZUpStart,
        LockCancelZUpWait,
        LowPowerOff,
        LowPowerOffWait,
        ActivePowerOn,
        ActivePowerOnWait,
        BootModeEnterStart,
        BootModeEnterWait,
        HWVersionSend,
        HWVersionWait,
        SWVersionSend,
        SWVersionWait,
        SerialNumReadSend,
        SerialNumReadWait,
        PartNumberSend,
        PartNumberWait,
        DTCEraseSend,
        DTCEraseWait,
        OperCurrentStart,
        OperCurrentWait,
        PowerOff,
        PowerOffWait,
        TestEndStart,
        TestEndWait,
        // Motion Unloading
        MotionUnloadingStart,
        MotionUnloadingWait,
        Complete,
        CancelStart,
        CancelZTouchStart,
        CancelZTouchWait,
        CancelZCancelStart,
        CancelZCancelWait,
        CancelYHomeStart,
        CancelYHomeWait,
        CancelComplete,
        Count
    }

    public enum NFCTouchTestStep
    {
        Standby = 0,
        Prepare,
        TestInitStart,
        TestInitWait,
        // Motion_Loading
        MotionLoadingStart,
        MotionLoadingWait,
        // Start VFlash
        //VFlashPowerOnStart,
        //VFlashPowerOnWait,
        //VFlashWriteStart,
        //VFlashWriteWait,
        //VFlashWriteComplete,
        //VFlashPowerOffStart,
        //VFlashPowerOffWait,
        //VFlashInitStart,
        //VFlashInitWait,
        // End VFlash
        ShortTestStart,
        ShortTestWait,
        LowPowerOn,
        LowPowerOnWait,
        WakeUpSend,
        WakeUpWait,
        ManufacturePrepareSend,
        ManufacturePrepareWait,
        ManufactureSeedkeySend,
        ManufactureSeedkeyWait,
        ManufactureGenerateSeedkey,
        ManufactureGeneratedKeySend,
        ManufactureGeneratedKeyWait,
        ManufactureWriteSend,
        ManufactureWriteWait,
        ManufactureReadSend,
        ManufactureReadWait,
        DarkCurrentStart,
        DarkCurrentWait,
        DarkCurrentUpdate,
        DarkCurrentComplete,
        LowPowerOff,
        LowPowerOffWait,
        HighPowerOn,
        HighPowerOnWait,
        NmWakeUpWait,
        PowerOnResetWait,
        ExtendedSessionStart,
        ExtendedSessionWait,
        PLightTurnOnSend,
        PLightTurnOnWait,
        PLightCurrentSend,
        PLightCurrentWait,
        PLightAmbientSend,
        PLightAmbientWait,
        PLightTurnOffSend,
        PLightTurnOffWait,
        // Touch 위치로 이동
        MotionMoveTouchStart,
        MotionMoveTouchWait,
        TouchTestStart,
        TouchCan_ZDownStart,
        TouchCan_ZDownWait,
        TouchCan_LockSenStart,
        TouchCan_LockSenWait,
        TouchCan_LockCanWait,
        TouchCan_Retry,
        TouchCap_ZDownStart,
        TouchCap_ZDownWait,
        TouchCap_LockSenStart,
        TouchCap_LockSenWait,
        TouchCap_LockCanWait,
        TouchCap_Retry,
        //TouchLockStart,
        //MotionTouchZDownStart,
        //MotionTouchZDownWait,
        //TouchLockWait,
        //TouchLockRetry,
        //TouchCapacitanceStart,
        //TouchCapacitancePrepare,
        //TouchCapacitanceWait,
        MotionTouchZUpStart,
        MotionTouchZUpWait,
        // Cancel 위치로 이동
        MotionMoveCancelStart,
        MotionMoveCancelWait,
        CancelCapacitanceStart,
        CancelCapacitancePrepare,
        CancelCapacitanceWait,
        MotionCancelZDownStart,
        MotionCancelZDownWait,
        // NFC 위치로 이동
        MotionMoveNFC_Start,
        MotionMoveNFC_Wait,
        MotionNFC_ZDownStart,
        MotionNFC_ZDownWait,
        NFC_CheckStart,
        NFC_CheckWait,
        NFC_RetryUpStart,
        NFC_RetryUpWait,
        MotionNFC_ZUpStart,
        MotionNFC_ZUpWait,
        // Security
        XcpPrepareSend,
        XcpPrepareWait,
        XcpConnectSend,
        XcpConnectWait,
        SecuritySetMtaSend,
        SecuritySetMtaWait,
        SecurityUploadWait,
        XcpDisconnectSend,
        XcpDisconnectWait,
        DTCEraseSend,
        DTCEraseWait,
        HWVersionSend,
        HWVersionWait,
        SWVersionSend,
        SWVersionWait,
        PartNumberSend,
        PartNumberWait,
        BootloaderSend,
        BootloaderWait,
        BootDefaultSend,
        BootDefaultWait,
        BootWakeUpWait,
        BootExtendedSend,
        BootExtendedWait,
        RxsWinSend,
        RxsWinWait,
        SupplierCodeSend,
        SupplierCodeWait,
        OperCurrentStart,
        OperCurrentWait,
        SerialNumPrepareSend,
        SerialNumPrepareWait,
        SerialNumSeedkeySend,
        SerialNumSeedkeyWait,
        SerialNumGenerateSeedkey,
        SerialNumGeneratekeySend,
        SerialNumGeneratekeyWait,
        SerialNumWriteSend,
        SerialNumWriteWait,
        SerialNumReadSend,
        SerialNumReadWait,
        PowerOff,
        PowerOffWait,
        TestEndStart,
        TestEndWait,
        // Unloading
        MotionUnloadingStart,
        MotionUnloadingWait,
        Complete,
        // Unclamp
        MotionUnclampForeStart,
        MotionUnclampForeWait,
        //JigUnloadingCheck,
        //MotionUnclampBackStart,
        //MotionUnclampBackWait,
        CancelStart,
        CancelZTouchStart,
        CancelZTouchWait,
        CancelZCancelStart,
        CancelZCancelWait,
        CancelZNFCStart,
        CancelZNFCWait,
        CancelYHomeStart,
        CancelYHomeWait,
        CancelComplete,
        Count
    }

    public enum XcpTouchStep
    {
        Standby = 0,
        Prepare,
        ConnectSend,
        ConnectWait,
        TouchFastMutualSend,
        TouchFastMutualUpload,
        TouchFastSelfSend,
        TouchFastSelfUpload,
        TouchSlowSelfSend,
        TouchSlowSelfUpload,
        TouchComboRateSend,
        TouchComboRateUpload,
        TouchStateSend,
        TouchStateUpload,
        DisconnectSend,
        DisconnectWait,
        Complete,
        Count
    }

    public enum XcpCancelStep
    {
        Standby = 0,
        Prepare,
        ConnectSend,
        ConnectWait,
        CancelFastSelfSend,
        CancelFastSelfUpload,
        CancelSlowSelfSend,
        CancelSlowSelfUpload,
        CancelStateSend,
        CancelStateUpload,
        DisconnectSend,
        DisconnectWait,
        Complete,
        Count
    }

    public enum TestStates
    {
        Ready = 0,
        Running,
        Pass,
        Failed,
        Cancel,
        Count
    }

    public class TestStateEventArgs : EventArgs
    {
        public int Channel { get; set; }
        public bool Use { get; set; }
        public string Name { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Value { get; set; }
        public string Result { get; set; }
        public TestStates State { get; set; }
    }

    public class LockStateEventArgs : EventArgs
    {
        public int Channel { get; set; }
        public int LockState { get; set; }
        public LockStateEventArgs(int channel, int lockState)
        {
            Channel = channel;
            LockState = lockState;
        }
    }

    public class FullProofEventArgs : EventArgs
    {
        public int Channel { get; set; }
        public string Message { get; set; }
        public bool Enabled { get; set; }
    }

    public class XcpDataEventArgs : EventArgs
    {
        public int Channel { get; set; }
        public int TouchFastMutual { get; set; }
        public int TouchFastSelf { get; set; }
        public int TouchSlowSelf { get; set; }
        public float TouchComboRate { get; set; }
        public int TouchState { get; set; }
        public int CancelFastSelf { get; set; }
        public int CancelSlowSelf { get; set; }
        public int CancelState { get; set; }
        public int IntervalTime { get; set; }
    }

    public class BarcodeEventArgs : EventArgs
    {
        public int Channel { get; set; }
        public int TrayMaxCount { get; set; }
        public int ProductCount { get; set; }
        public string TrayBarcode { get; set; }
        public string ProductBarcode { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // IDHSModel
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface IDHSModel : IDisposable
    {
        event EventHandler<TestStateEventArgs> TestStateChanged;
        event EventHandler<TestStateEventArgs> TestStepProgressChanged;
        event EventHandler<TestStateEventArgs> RxsWinDataChanged;
        event EventHandler<LockStateEventArgs> LockStateChanged;
        event EventHandler<LockStateEventArgs> NFCStateChanged;
        event EventHandler<FullProofEventArgs> ShowFullProofMessage;
        event EventHandler<XcpDataEventArgs> XcpDataChanged;
        event EventHandler TouchXcpDataChanged;
        event EventHandler CancelXcpDataChanged;

        List<TestSpec> TestItemsList { get; set; }
        Task<XL_Status> OpenPort(int channel);
        Task<XL_Status> ClosePort(int channel);
        bool OpenPortCOM(int channel);
        void ClosePortCOM(int channel);
        bool IsOpen(int channel);

        uint GetCanID(uint canId);
        int GetLengthDLC(XL_CANFD_DLC dlc);

        StringBuilder GetEventString(xl_event xlEvent);
        StringBuilder GetEventString(XLcanTxEvent xlEvent);
        StringBuilder GetTxEventString(xl_event txEvent);
        StringBuilder GetTxEventString(XLcanTxEvent txEvent);
        StringBuilder GetRxEventString(xl_event rxEvent);
        StringBuilder GetRxEventString(XLcanRxEvent rxEvent);
        XL_Status WriteFrame(int channel, UInt32 id, ushort dlc, byte[] data, bool logging = false, string remarks = "");
        XL_Status WriteFrameStd(int channel, UInt32 id, XL_CANFD_DLC dlc, byte[] data, bool logging = false, string remarks = "");
        XL_Status WriteFrameExt(int channel, UInt32 id, XL_CANFD_DLC dlc, byte[] data, bool logging = false, string remarks = "");

        XL_Status Send_NM(int channel, bool logging = false);
        XL_Status Send_NFC(int channel, bool logging = false);
        XL_Status Send_DefaultSession(int channel, bool logging = false);
        XL_Status Send_ExtendedSession(int channel, bool logging = false);
        XL_Status Send_Bootloader(int channel, bool logging = false);

        XL_Status Send_RequestSeedkey(int channel, bool logging = false);
        XL_Status Send_RequestSeedkeyFlow(int channel, bool logging = false);
        string GenerateAdvancedSeedKey(ref byte[] receivedSeedkey, ref byte[] generatedSeedkey);
        XL_Status Send_GeneratedSeedkey(int channel, byte[] seedkey, bool logging = false);
        XL_Status Send_GeneratedSeedkey2(int channel, byte[] seedkey, bool logging = false);
        XL_Status Send_SerialNumberWrite(int channel, byte[] serialNumber, bool logging = false);
        XL_Status Send_SerialNumberRead(int channel, bool logging = false);
        XL_Status Send_ManufactureWrite(int channel, byte[] manufacture, bool logging = false);
        XL_Status Send_ManufactureRead(int channel, bool logging = false);
        XL_Status Send_PLight(int channel, bool onOff, bool logging = false);
        XL_Status Send_DTC_Erase(int channel, bool logging = false);
        XL_Status Send_HWVersion(int channel, bool logging = false);
        XL_Status Send_SWVersion(int channel, bool logging = false);
        XL_Status Send_PartNumber(int channel, bool logging = false);
        XL_Status Send_PartNumberFlow(int channel, bool logging = false);
        XL_Status Send_RXSWin(int channel, bool logging = false);
        XL_Status Send_RXSWinFlow(int channel, byte interval, bool logging = false);
        XL_Status Send_SupplierCodeRead(int channel, bool logging = false);
        XL_Status Send_XCPConnect(int channel, bool logging = false);
        XL_Status Send_XCPDisconnect(int channel, bool logging = false);
        XL_Status Send_Upload(int channel, byte count, bool logging = false);
        XL_Status Send_UploadCapa(int channel, byte count, bool logging = false);
        XL_Status Send_ShortUpload(int channel, uint address, byte count, bool logging = false, string remarks = "");
        XL_Status Send_SetMTA(int channel, uint address, bool logging = false, string remarks = "");
        XL_Status Send_Security(int channel, uint address, bool logging = false);
        XL_Status Send_HardwireTest(int channel, bool logging = false);

        NFCTouchTestStep NextTestStep(int channel);
        NFCTouchTestStep GetTestStep(int channel);
        NFCTouchTestStep SetTestStep(int channel, NFCTouchTestStep step);
        TouchOnlyTestStep NextTouchOnlyTestStep(int channel);
        TouchOnlyTestStep GetTouchOnlyTestStep(int channel);
        TouchOnlyTestStep SetTouchOnlyTestStep(int channel, TouchOnlyTestStep step);
        void StartThread(int channel);
        void StartTest(int channel);
        void StopTest(int channel);
        void CancelTest(int channel, bool cancel);

        Task<bool> GetTouchAsync(int channel);
        Task<bool> GetCancelAsync(int channel);
        Task SetTouchStepExit(int channel, bool exit);
        Task SetCancelStepExit(int channel, bool exit);
    }



}

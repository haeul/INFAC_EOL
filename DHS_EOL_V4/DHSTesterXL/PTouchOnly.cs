using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public PTouchOnly()
        {
        }

        public void Dispose()
        {

        }

        public Task<XL_Status> OpenPort(int channel)
        {
            throw new NotImplementedException();
        }
        public Task<XL_Status> ClosePort(int channel)
        {
            throw new NotImplementedException();
        }
        public bool IsOpen(int channel)
        {
            throw new NotImplementedException();
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
        public XL_Status Send_ShortUpload(int channel, uint address, byte count, bool logging = false)
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
        public void StartTest(int channel)
        {
            throw new NotImplementedException();
        }
        public void StopTest(int channel)
        {
            throw new NotImplementedException();
        }
        public void CancelTest(int channel, bool cancel)
        {
            throw new NotImplementedException();
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

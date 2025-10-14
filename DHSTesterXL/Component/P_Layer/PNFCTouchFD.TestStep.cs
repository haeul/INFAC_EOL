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
using System.IO;
using System.Runtime.Remoting.Channels;

namespace DHSTesterXL
{
    public partial class PNFCTouchFD : IDHSModel
    {
        // -----------------------------------------------------------------------------------------------
        // 테스트 실행 스텝 중 송신 스텝 처리
        // -----------------------------------------------------------------------------------------------
        private void TestStepTxProc(int channel)
        {
            switch (_currTestStep[channel])
            {
                case NFCTouchTestStep.Standby                     : NFCTouchTestStep_Standby                     (channel); break;
                case NFCTouchTestStep.Prepare                     : NFCTouchTestStep_Prepare                     (channel); break;
                case NFCTouchTestStep.MotionLoadingStart          : NFCTouchTestStep_MotionLoadingStart          (channel); break;
                case NFCTouchTestStep.MotionLoadingWait           : NFCTouchTestStep_MotionLoadingWait           (channel); break;
                case NFCTouchTestStep.TestInitStart               : NFCTouchTestStep_TestInitStart               (channel); break;
                case NFCTouchTestStep.TestInitWait                : NFCTouchTestStep_TestInitWait                (channel); break;
                case NFCTouchTestStep.ShortTestStart              : NFCTouchTestStep_ShortTestStart              (channel); break;
                case NFCTouchTestStep.ShortTestWait               : NFCTouchTestStep_ShortTestWait               (channel); break;
                case NFCTouchTestStep.LowPowerOn                  : NFCTouchTestStep_LowPowerOn                  (channel); break;
                case NFCTouchTestStep.LowPowerOnWait              : NFCTouchTestStep_LowPowerOnWait              (channel); break;
                case NFCTouchTestStep.WakeUpSend                  : NFCTouchTestStep_WakeUpSend                  (channel); break;
                case NFCTouchTestStep.ManufacturePrepareSend      : NFCTouchTestStep_ManufacturePrepareSend      (channel); break;
                case NFCTouchTestStep.ManufactureSeedkeySend      : NFCTouchTestStep_ManufactureSeedkeySend      (channel); break;
                case NFCTouchTestStep.ManufactureGenerateSeedkey  : NFCTouchTestStep_ManufactureGenerateSeedkey  (channel); break;
                case NFCTouchTestStep.ManufactureGeneratedKeySend : NFCTouchTestStep_ManufactureGeneratedKeySend (channel); break;
                case NFCTouchTestStep.ManufactureWriteSend        : NFCTouchTestStep_ManufactureWriteSend        (channel); break;
                case NFCTouchTestStep.ManufactureReadSend         : NFCTouchTestStep_ManufactureReadSend         (channel); break;
                case NFCTouchTestStep.DarkCurrentStart            : NFCTouchTestStep_DarkCurrentStart            (channel); break;
                case NFCTouchTestStep.DarkCurrentWait             : NFCTouchTestStep_DarkCurrentWait             (channel); break;
                case NFCTouchTestStep.DarkCurrentUpdate           : NFCTouchTestStep_DarkCurrentUpdate           (channel); break;
                case NFCTouchTestStep.DarkCurrentComplete         : NFCTouchTestStep_DarkCurrentComplete         (channel); break;
                case NFCTouchTestStep.LowPowerOff                 : NFCTouchTestStep_LowPowerOff                 (channel); break;
                case NFCTouchTestStep.LowPowerOffWait             : NFCTouchTestStep_LowPowerOffWait             (channel); break;
                case NFCTouchTestStep.HighPowerOn                 : NFCTouchTestStep_HighPowerOn                 (channel); break;
                case NFCTouchTestStep.HighPowerOnWait             : NFCTouchTestStep_HighPowerOnWait             (channel); break;
                case NFCTouchTestStep.PowerOnResetWait            : NFCTouchTestStep_PowerOnResetWait            (channel); break;
                case NFCTouchTestStep.ExtendedSessionStart        : NFCTouchTestStep_ExtendedSessionStart        (channel); break;
                case NFCTouchTestStep.PLightTurnOnSend            : NFCTouchTestStep_PLightTurnOnSend            (channel); break;
                case NFCTouchTestStep.PLightCurrentSend           : NFCTouchTestStep_PLightCurrentSend           (channel); break;
                case NFCTouchTestStep.PLightAmbientSend           : NFCTouchTestStep_PLightAmbientSend           (channel); break;
                case NFCTouchTestStep.PLightTurnOffSend           : NFCTouchTestStep_PLightTurnOffSend           (channel); break;
                case NFCTouchTestStep.MotionMoveTouchStart        : NFCTouchTestStep_MotionMoveTouchStart        (channel); break;
                case NFCTouchTestStep.MotionMoveTouchWait         : NFCTouchTestStep_MotionMoveTouchWait         (channel); break;
                case NFCTouchTestStep.TouchTestStart              : NFCTouchTestStep_TouchTestStart              (channel); break;
                case NFCTouchTestStep.TouchCan_ZDownStart         : NFCTouchTestStep_TouchCan_ZDownStart         (channel); break;
                case NFCTouchTestStep.TouchCan_ZDownWait          : NFCTouchTestStep_TouchCan_ZDownWait          (channel); break;
                case NFCTouchTestStep.TouchCan_LockSenStart       : NFCTouchTestStep_TouchCan_LockSenStart       (channel); break;
                case NFCTouchTestStep.TouchCan_LockSenWait        : NFCTouchTestStep_TouchCan_LockSenWait        (channel); break;
                case NFCTouchTestStep.TouchCan_Retry              : NFCTouchTestStep_TouchCan_Retry              (channel); break;
                case NFCTouchTestStep.TouchCap_ZDownStart         : NFCTouchTestStep_TouchCap_ZDownStart         (channel); break;
                case NFCTouchTestStep.TouchCap_ZDownWait          : NFCTouchTestStep_TouchCap_ZDownWait          (channel); break;
                case NFCTouchTestStep.TouchCap_LockSenStart       : NFCTouchTestStep_TouchCap_LockSenStart       (channel); break;
                case NFCTouchTestStep.TouchCap_LockSenWait        : NFCTouchTestStep_TouchCap_LockSenWait        (channel); break;
                case NFCTouchTestStep.TouchCap_LockCanWait        : NFCTouchTestStep_TouchCap_LockCanWait        (channel); break;
                case NFCTouchTestStep.TouchCap_Retry              : NFCTouchTestStep_TouchCap_Retry              (channel); break;



                //case NFCTouchTestStep.TouchLockStart              : NFCTouchTestStep_TouchLockStart              (channel); break;
                //case NFCTouchTestStep.MotionTouchZDownStart       : NFCTouchTestStep_MotionTouchZDownStart       (channel); break;
                //case NFCTouchTestStep.MotionTouchZDownWait        : NFCTouchTestStep_MotionTouchZDownWait        (channel); break;
                //case NFCTouchTestStep.TouchLockRetry              : NFCTouchTestStep_TouchLockRetry              (channel); break;
                //case NFCTouchTestStep.TouchCapacitanceStart       : NFCTouchTestStep_TouchCapacitanceStart       (channel); break;
                //case NFCTouchTestStep.TouchCapacitancePrepare     : NFCTouchTestStep_TouchCapacitancePrepare     (channel); break;
                //case NFCTouchTestStep.TouchCapacitanceWait        : NFCTouchTestStep_TouchCapacitanceWait        (channel); break;
                case NFCTouchTestStep.MotionTouchZUpStart         : NFCTouchTestStep_MotionTouchZUpStart         (channel); break;
                case NFCTouchTestStep.MotionTouchZUpWait          : NFCTouchTestStep_MotionTouchZUpWait          (channel); break;
                case NFCTouchTestStep.MotionMoveCancelStart       : NFCTouchTestStep_MotionMoveCancelStart       (channel); break;
                case NFCTouchTestStep.MotionMoveCancelWait        : NFCTouchTestStep_MotionMoveCancelWait        (channel); break;
                case NFCTouchTestStep.CancelCapacitanceStart      : NFCTouchTestStep_CancelCapacitanceStart      (channel); break;
                case NFCTouchTestStep.CancelCapacitancePrepare    : NFCTouchTestStep_CancelCapacitancePrepare    (channel); break;
                case NFCTouchTestStep.CancelCapacitanceWait       : NFCTouchTestStep_CancelCapacitanceWait       (channel); break;
                case NFCTouchTestStep.MotionCancelZDownStart      : NFCTouchTestStep_MotionCancelZDownStart      (channel); break;
                case NFCTouchTestStep.MotionCancelZDownWait       : NFCTouchTestStep_MotionCancelZDownWait       (channel); break;
                case NFCTouchTestStep.MotionMoveNFC_Start         : NFCTouchTestStep_MotionMoveNFC_Start         (channel); break;
                case NFCTouchTestStep.MotionMoveNFC_Wait          : NFCTouchTestStep_MotionMoveNFC_Wait          (channel); break;
                case NFCTouchTestStep.NfcCheckStart               : NFCTouchTestStep_NfcCheckStart               (channel); break;
                case NFCTouchTestStep.MotionNFC_UpStart           : NFCTouchTestStep_MotionNFC_UpStart           (channel); break;
                case NFCTouchTestStep.MotionNFC_UpWait            : NFCTouchTestStep_MotionNFC_UpWait            (channel); break;
                case NFCTouchTestStep.XcpPrepareSend              : NFCTouchTestStep_XcpPrepareSend              (channel); break;
                case NFCTouchTestStep.XcpConnectSend              : NFCTouchTestStep_XcpConnectSend              (channel); break;
                case NFCTouchTestStep.SecuritySetMtaSend          : NFCTouchTestStep_SecuritySetMtaSend          (channel); break;
                case NFCTouchTestStep.XcpDisconnectSend           : NFCTouchTestStep_XcpDisconnectSend           (channel); break;
                case NFCTouchTestStep.XcpDisconnectWait           : NFCTouchTestStep_XcpDisconnectWait           (channel); break;
                case NFCTouchTestStep.DTCEraseSend                : NFCTouchTestStep_DTCEraseSend                (channel); break;
                case NFCTouchTestStep.HWVersionSend               : NFCTouchTestStep_HWVersionSend               (channel); break;
                case NFCTouchTestStep.SWVersionSend               : NFCTouchTestStep_SWVersionSend               (channel); break;
                case NFCTouchTestStep.PartNumberSend              : NFCTouchTestStep_PartNumberSend              (channel); break;
                case NFCTouchTestStep.BootloaderSend              : NFCTouchTestStep_BootloaderSend              (channel); break;
                case NFCTouchTestStep.BootDefaultSend             : NFCTouchTestStep_BootDefaultSend             (channel); break;
                case NFCTouchTestStep.BootExtendedSend            : NFCTouchTestStep_BootExtendedSend            (channel); break;
                case NFCTouchTestStep.RxsWinSend                  : NFCTouchTestStep_RxsWinSend                  (channel); break;
                case NFCTouchTestStep.SupplierCodeSend            : NFCTouchTestStep_SupplierCodeSend            (channel); break;
                case NFCTouchTestStep.OperCurrentStart            : NFCTouchTestStep_OperCurrentStart            (channel); break;
                case NFCTouchTestStep.OperCurrentWait             : NFCTouchTestStep_OperCurrentWait             (channel); break;
                case NFCTouchTestStep.SerialNumPrepareSend        : NFCTouchTestStep_SerialNumPrepareSend        (channel); break;
                case NFCTouchTestStep.SerialNumSeedkeySend        : NFCTouchTestStep_SerialNumSeedkeySend        (channel); break;
                case NFCTouchTestStep.SerialNumGenerateSeedkey    : NFCTouchTestStep_SerialNumGenerateSeedkey    (channel); break;
                case NFCTouchTestStep.SerialNumGeneratekeySend    : NFCTouchTestStep_SerialNumGeneratekeySend    (channel); break;
                case NFCTouchTestStep.SerialNumWriteSend          : NFCTouchTestStep_SerialNumWriteSend          (channel); break;
                case NFCTouchTestStep.SerialNumReadSend           : NFCTouchTestStep_SerialNumReadSend           (channel); break;
                case NFCTouchTestStep.PowerOff                    : NFCTouchTestStep_PowerOff                    (channel); break;
                case NFCTouchTestStep.PowerOffWait                : NFCTouchTestStep_PowerOffWait                (channel); break;
                case NFCTouchTestStep.TestEndStart                : NFCTouchTestStep_TestEndStart                (channel); break;
                case NFCTouchTestStep.TestEndWait                 : NFCTouchTestStep_TestEndWait                 (channel); break;
                case NFCTouchTestStep.MotionUnloadingStart        : NFCTouchTestStep_MotionUnloadingStart        (channel); break;
                case NFCTouchTestStep.MotionUnloadingWait         : NFCTouchTestStep_MotionUnloadingWait         (channel); break;
                case NFCTouchTestStep.Complete                    : NFCTouchTestStep_Complete                    (channel); break;
                case NFCTouchTestStep.MotionUnclampForeStart      : NFCTouchTestStep_MotionUnclampForeStart      (channel); break;
                case NFCTouchTestStep.MotionUnclampForeWait       : NFCTouchTestStep_MotionUnclampForeWait       (channel); break;
                //case NFCTouchTestStep.JigUnloadingCheck           : NFCTouchTestStep_JigUnloadingCheck           (channel); break;
                //case NFCTouchTestStep.MotionUnclampBackStart      : NFCTouchTestStep_MotionUnclampBackStart      (channel); break;
                //case NFCTouchTestStep.MotionUnclampBackWait       : NFCTouchTestStep_MotionUnclampBackWait       (channel); break;
                default: break;
            }
        }


        // -----------------------------------------------------------------------------------------------
        // 테스트 실행 스텝 중 수신 스텝 처리
        // -----------------------------------------------------------------------------------------------
        private void TestStepRxProc(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            //if (rxCanID == 0x8000080)
            {
                GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            }
            switch (_currTestStep[channel])
            {
                case NFCTouchTestStep.WakeUpWait                  : NFCTouchTestStep_WakeUpWait                  (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.ManufacturePrepareWait      : NFCTouchTestStep_ManufacturePrepareWait      (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.ManufactureSeedkeyWait      : NFCTouchTestStep_ManufactureSeedkeyWait      (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.ManufactureGeneratedKeyWait : NFCTouchTestStep_ManufactureGeneratedKeyWait (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.ManufactureWriteWait        : NFCTouchTestStep_ManufactureWriteWait        (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.ManufactureReadWait         : NFCTouchTestStep_ManufactureReadWait         (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.NmWakeUpWait                : NFCTouchTestStep_NmWakeUpWait                (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.ExtendedSessionWait         : NFCTouchTestStep_ExtendedSessionWait         (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.PLightTurnOnWait            : NFCTouchTestStep_PLightTurnOnWait            (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.PLightCurrentWait           : NFCTouchTestStep_PLightCurrentWait           (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.PLightAmbientWait           : NFCTouchTestStep_PLightAmbientWait           (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.PLightTurnOffWait           : NFCTouchTestStep_PLightTurnOffWait           (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.TouchCan_LockCanWait        : NFCTouchTestStep_TouchCan_LockCanWait        (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.TouchLockWait               : NFCTouchTestStep_TouchLockWait               (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.NfcCheckWait                : NFCTouchTestStep_NfcCheckWait                (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.XcpPrepareWait              : NFCTouchTestStep_XcpPrepareWait              (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.XcpConnectWait              : NFCTouchTestStep_XcpConnectWait              (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SecuritySetMtaWait          : NFCTouchTestStep_SecuritySetMtaWait          (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SecurityUploadWait          : NFCTouchTestStep_SecurityUploadWait          (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.DTCEraseWait                : NFCTouchTestStep_DTCEraseWait                (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.HWVersionWait               : NFCTouchTestStep_HWVersionWait               (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SWVersionWait               : NFCTouchTestStep_SWVersionWait               (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.PartNumberWait              : NFCTouchTestStep_PartNumberWait              (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.BootloaderWait              : NFCTouchTestStep_BootloaderWait              (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.BootDefaultWait             : NFCTouchTestStep_BootDefaultWait             (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.BootWakeUpWait              : NFCTouchTestStep_BootWakeUpWait              (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.BootExtendedWait            : NFCTouchTestStep_BootExtendedWait            (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.RxsWinWait                  : NFCTouchTestStep_RxsWinWait                  (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SupplierCodeWait            : NFCTouchTestStep_SupplierCodeWait            (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SerialNumPrepareWait        : NFCTouchTestStep_SerialNumPrepareWait        (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SerialNumSeedkeyWait        : NFCTouchTestStep_SerialNumSeedkeyWait        (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SerialNumGeneratekeyWait    : NFCTouchTestStep_SerialNumGeneratekeyWait    (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SerialNumWriteWait          : NFCTouchTestStep_SerialNumWriteWait          (channel, rxCanID, ref receivedEvent); break;
                case NFCTouchTestStep.SerialNumReadWait           : NFCTouchTestStep_SerialNumReadWait           (channel, rxCanID, ref receivedEvent); break;
                default: break;
            }
        }

        private void NFCTouchTestStep_Standby(int channel)
        {

        }
        private void NFCTouchTestStep_Prepare(int channel)
        {
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Prepare]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Prepare]");

            // 검사 완료 OFF
            GSystem.MiPLC.SetAutoTestComplete(channel, false);

            // 테스트 결과 초기화
            _overalResult[channel].ProductInfo = ProductSettings.ProductInfo;
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
            _overalResult[channel].SerialNumber.Init();
            _overalResult[channel].DarkCurrent.Init();
            _overalResult[channel].PLightTurnOn.Init();
            _overalResult[channel].PLightCurrent.Init();
            _overalResult[channel].PLightAmbient.Init();
            _overalResult[channel].LockSen.Init();
            _overalResult[channel].LockCan.Init();
            _overalResult[channel].Cancel.Init();
            _overalResult[channel].SecurityBit.Init();
            _overalResult[channel].NFC.Init();
            _overalResult[channel].DTC_Erase.Init();
            _overalResult[channel].HW_Version.Init();
            _overalResult[channel].SW_Version.Init();
            _overalResult[channel].PartNumber.Init();
            _overalResult[channel].Bootloader.Init();
            _overalResult[channel].RXSWIN.Init();
            _overalResult[channel].Manufacture.Init();
            _overalResult[channel].SupplierCode.Init();
            _overalResult[channel].OperationCurrent.Init();

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
            _overalResult[channel].SerialNumber.Use = ProductSettings.TestItemSpecs.SerialNumber.Use;
            _overalResult[channel].DarkCurrent.Use = ProductSettings.TestItemSpecs.DarkCurrent.Use;
            _overalResult[channel].PLightTurnOn.Use = ProductSettings.TestItemSpecs.PLightTurnOn.Use;
            _overalResult[channel].PLightCurrent.Use = ProductSettings.TestItemSpecs.PLightCurrent.Use;
            _overalResult[channel].PLightAmbient.Use = ProductSettings.TestItemSpecs.PLightAmbient.Use;
            _overalResult[channel].LockSen.Use = ProductSettings.TestItemSpecs.LockSen.Use;
            _overalResult[channel].LockCan.Use = ProductSettings.TestItemSpecs.LockCan.Use;
            _overalResult[channel].Cancel.Use = ProductSettings.TestItemSpecs.Cancel.Use;
            _overalResult[channel].SecurityBit.Use = ProductSettings.TestItemSpecs.SecurityBit.Use;
            _overalResult[channel].NFC.Use = ProductSettings.TestItemSpecs.NFC.Use;
            _overalResult[channel].DTC_Erase.Use = ProductSettings.TestItemSpecs.DTC_Erase.Use;
            _overalResult[channel].HW_Version.Use = ProductSettings.TestItemSpecs.HW_Version.Use;
            _overalResult[channel].SW_Version.Use = ProductSettings.TestItemSpecs.SW_Version.Use;
            _overalResult[channel].PartNumber.Use = ProductSettings.TestItemSpecs.PartNumber.Use;
            _overalResult[channel].Bootloader.Use = ProductSettings.TestItemSpecs.Bootloader.Use;
            _overalResult[channel].RXSWIN.Use = ProductSettings.TestItemSpecs.RXSWIN.Use;
            _overalResult[channel].Manufacture.Use = ProductSettings.TestItemSpecs.Manufacture.Use;
            _overalResult[channel].SupplierCode.Use = ProductSettings.TestItemSpecs.SupplierCode.Use;
            _overalResult[channel].OperationCurrent.Use = ProductSettings.TestItemSpecs.OperationCurrent.Use;

            _testResultsList[channel] = _overalResult[channel].GetEnableTestResultList();

            _isCancel[channel] = false;

            OnTestStateChanged(channel, TestStates.Running);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionLoadingStart(int channel)
        {
            //if ((GSystem.MiPLC.GetState1(channel) & 0x0006) == 0x0006)
            //{
            //    SetTestStep(channel, NFCTouchTestStep.TestInitStart);
            //    return;
            //}
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Loading]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Loading]");
            GSystem.MiPLC.SetLoadingStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionLoadingWait(int channel)
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
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_TestInitStart(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test Init]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test Init]");
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_TestInitWait(int channel)
        {
            // 완료 대기
            if (!GSystem.DedicatedCTRL.GetCommandTestInit(channel) || !GSystem.DedicatedCTRL.GetCompleteTestInit(channel))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Init step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Init step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Init Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Init Complete");
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
            GSystem.TimerTestTime[channel].Start();
            // 검사 시작 시간 저장
            _testStartTime[channel] = DateTime.Now;
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_ShortTestStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Short_1_2.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.LowPowerOn);
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
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_ShortTestWait(int channel)
        {
            // 완료 대기
            if (!GSystem.DedicatedCTRL.GetCommandShortTest(channel) || !GSystem.DedicatedCTRL.GetCompleteShortTest(channel))
                return;
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
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_LowPowerOn(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_LowPowerOnWait(int channel)
        {
            // 완료 대기
            if (!GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel) || !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Power On Complete");
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_WakeUpSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Wake Up]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Wake Up]");
            NextTestStep(channel);
            Send_NM(channel, true);
            _enableNM[channel] = true;
            _enableNFC[channel] = true;
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_WakeUpWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            //
            // TODO: WakeUp 타임아웃 처리, 검사 제품과 검사 품번이 같은지 비교
            //
            if (rxCanID == GSystem.ProductSettings.NM_ResID)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
                GSystem.Logger.Info ($"[CH.{channel + 1}] Wake Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Wake Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Wake Up Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Wake Up Complete");
                _retryCount[channel] = 0;
                _tickStepInterval[channel].Reset();
                NextTestStep(channel);
            }
            else
            {
                if (rxCanID == (GSystem.ProductSettings.NM_ResID + 0x10))
                {
                    // LHD 또는 RHD 연결 불량
                    GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                    GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
                    string message = $"연결된 제품과 검사 품번이 일치하지 않습니다. [ Part No: {GSystem.ProductSettings.ProductInfo.PartNo}, Response ID: 0x{rxCanID:X}";
                    GSystem.Logger.Info (message);
                    GSystem.TraceMessage(message);
                    GSystem.MainFormMessageBox?.Invoke(message, "품번 불일치", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isCancel[channel] = true;
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
                else if (rxCanID == (GSystem.ProductSettings.NM_ResID - 0x10))
                {
                    // LHD 또는 RHD 연결 불량
                    string message = $"연결된 제품과 검사 품번이 일치하지 않습니다. [ Part No: {GSystem.ProductSettings.ProductInfo.PartNo}, Response ID: 0x{rxCanID:X}";
                    GSystem.Logger.Info (message);
                    GSystem.TraceMessage(message);
                    GSystem.MainFormMessageBox?.Invoke(message, "품번 불일치", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isCancel[channel] = true;
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
                else
                {
                    // 다른 ID, ex) 0x17FC002A or 0x17FC002B
                    // 타임아웃 처리 
                    if (_tickStepTimeout[channel].MoreThan(5000))
                    {
                        //if (++_retryCount[channel] >= MaxRetryCount)
                        {
                            string message = $"Wake Up 시간 초과";
                            GSystem.Logger.Info (message);
                            GSystem.TraceMessage(message);
                            GSystem.MainFormMessageBox?.Invoke(message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _isCancel[channel] = true;
                            SetTestStep(channel, NFCTouchTestStep.PowerOff);
                            return;
                        }
                        //else
                        //{
                        //    SetTestStep(channel, NFCTouchTestStep.WakeUpSend);
                        //    return;
                        //}
                    }
                    else
                        return;
                }
            }
        }
        private void NFCTouchTestStep_ManufacturePrepareSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Manufacture.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.DarkCurrentStart);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Extended Session]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Extended Session]");
            // Extended session으로 변경
            Send_ExtendedSession(channel, true);
            Thread.Sleep(5);
            Send_ExtendedSession(channel);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_ManufacturePrepareWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.ManufacturePrepareSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].Manufacture.State = TestStates.Failed;
                    _overalResult[channel].Manufacture.Value = "";
                    _overalResult[channel].Manufacture.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].Manufacture);
                    SetTestStep(channel, NFCTouchTestStep.DarkCurrentStart);
                    return;
                }
            }
            GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] != 0x50 || receivedEvent.tagData.canRxOkMsg.data[2] != 0x03)
                return;
            GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
            GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            // Extend Session으로 변경 완료
            GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session complete");
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Manufacture.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.DarkCurrentStart);
                return;
            }
            // 옵션에 따라 WR/RO 동작을 다르게 한다.
            if (GSystem.ProductSettings.TestItemSpecs.Manufacture.Option == 0)
            {
                // WR
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
            else
            {
                // RO
                SetTestStep(channel, NFCTouchTestStep.ManufactureReadSend);
            }
        }
        private void NFCTouchTestStep_ManufactureSeedkeySend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Manufacture.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.DarkCurrentStart);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Manufacture Request Seedkey]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Manufacture Request Seedkey]");
            // 측정 상태 표시
            _overalResult[channel].Manufacture.State = TestStates.Running;
            _overalResult[channel].Manufacture.Value = "";
            _overalResult[channel].Manufacture.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].Manufacture);
            // 명령 전송
            Send_RequestSeedkey(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_ManufactureSeedkeyWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Request Seedkey Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Request Seedkey Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.ManufactureSeedkeySend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Request Seedkey Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Request Seedkey Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].Manufacture.State = TestStates.Failed;
                    _overalResult[channel].Manufacture.Value = "";
                    _overalResult[channel].Manufacture.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].Manufacture);
                    SetTestStep(channel, NFCTouchTestStep.DarkCurrentStart);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x10 && receivedEvent.tagData.canRxOkMsg.data[1] == 0x0A &&
                receivedEvent.tagData.canRxOkMsg.data[2] == 0x67 && receivedEvent.tagData.canRxOkMsg.data[3] == 0x11)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
                _receivedSeedkey[channel, 0] = receivedEvent.tagData.canRxOkMsg.data[4];
                _receivedSeedkey[channel, 1] = receivedEvent.tagData.canRxOkMsg.data[5];
                _receivedSeedkey[channel, 2] = receivedEvent.tagData.canRxOkMsg.data[6];
                _receivedSeedkey[channel, 3] = receivedEvent.tagData.canRxOkMsg.data[7];
                Send_RequestSeedkeyFlow(channel);
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x21)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                _receivedSeedkey[channel, 4] = receivedEvent.tagData.canRxOkMsg.data[1];
                _receivedSeedkey[channel, 5] = receivedEvent.tagData.canRxOkMsg.data[2];
                _receivedSeedkey[channel, 6] = receivedEvent.tagData.canRxOkMsg.data[3];
                _receivedSeedkey[channel, 7] = receivedEvent.tagData.canRxOkMsg.data[4];
                byte[] tempSeedkey = new byte[8];
                int srcOffset = channel * _receivedSeedkey.GetLength(channel);
                Buffer.BlockCopy(_receivedSeedkey, srcOffset, tempSeedkey, 0, 8);
                GSystem.Logger.Info ($"[CH.{channel + 1}] Received Seedkey : [ {BitConverter.ToString(tempSeedkey).Replace('-', ' ')} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Request Seedkey step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Request Seedkey step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Request Seedkey complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Request Seedkey complete");
                NextTestStep(channel);
            }
        }
        private void NFCTouchTestStep_ManufactureGenerateSeedkey(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Manufacture Generate Seedkey]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Manufacture Generate Seedkey]");
            byte[] receivedSeedkey = new byte[8];
            byte[] generatedSeedkey = new byte[8];
            int srcOffset = channel * _receivedSeedkey.GetLength(channel);
            Buffer.BlockCopy(_receivedSeedkey, srcOffset, receivedSeedkey, 0, 8);
            GenerateAdvancedSeedKey(ref receivedSeedkey, ref generatedSeedkey);
            int destIndex = channel * _generatedSeedkey.GetLength(channel);
            Buffer.BlockCopy(generatedSeedkey, 0, _generatedSeedkey, destIndex, 8);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Generated Seedkey: [ {BitConverter.ToString(generatedSeedkey).Replace('-', ' ')} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Generated Seedkey step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Generated Seedkey step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Generated Seedkey complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Generated Seedkey complete");
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_ManufactureGeneratedKeySend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Manufacture Generated Seedkey Send]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Manufacture Generated Seedkey Send]");
            byte[] seedkey = new byte[8];
            int srcOffset = channel * _generatedSeedkey.GetLength(channel);
            Buffer.BlockCopy(_generatedSeedkey, srcOffset, seedkey, 0, 8);
            Send_GeneratedSeedkey(channel, seedkey, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_ManufactureGeneratedKeyWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Generated Seedkey Send Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Generated Seedkey Send Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.ManufactureGeneratedKeySend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Generated Seedkey Send Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Generated Seedkey Send Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].Manufacture.State = TestStates.Failed;
                    _overalResult[channel].Manufacture.Value = "";
                    _overalResult[channel].Manufacture.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].Manufacture);
                    SetTestStep(channel, NFCTouchTestStep.DarkCurrentStart);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x30)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                byte[] seedkey = new byte[8];
                int srcOffset = channel * _generatedSeedkey.GetLength(channel);
                Buffer.BlockCopy(_generatedSeedkey, srcOffset, seedkey, 0, 8);
                Send_GeneratedSeedkey2(channel, seedkey, true);
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x67 && receivedEvent.tagData.canRxOkMsg.data[2] == 0x12)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Generated Seedkey Send step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Generated Seedkey Send step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Generated Seedkey Send complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Generated Seedkey Send complete");
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_ManufactureWriteSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Manufacture Write]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Manufacture Write]");
            //
            // Manufacture 생성
            //
            string dateTime = DateTime.Now.ToString("yyyyMMdd");
            _manufactureBytes[channel, 0] = Convert.ToByte(dateTime.Substring(0, 2), 16);
            _manufactureBytes[channel, 1] = Convert.ToByte(dateTime.Substring(2, 2), 16);
            _manufactureBytes[channel, 2] = Convert.ToByte(dateTime.Substring(4, 2), 16);
            _manufactureBytes[channel, 3] = Convert.ToByte(dateTime.Substring(6, 2), 16);
            byte[] manufacture = new byte[4];
            manufacture[0] = _manufactureBytes[channel, 0];
            manufacture[1] = _manufactureBytes[channel, 1];
            manufacture[2] = _manufactureBytes[channel, 2];
            manufacture[3] = _manufactureBytes[channel, 3];
            Send_ManufactureWrite(channel, manufacture, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_ManufactureWriteWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Write Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Write Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.ManufactureWriteSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Write Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Write Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].Manufacture.State = TestStates.Failed;
                    _overalResult[channel].Manufacture.Value = "";
                    _overalResult[channel].Manufacture.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].Manufacture);
                    SetTestStep(channel, NFCTouchTestStep.DarkCurrentStart);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x6E &&
                receivedEvent.tagData.canRxOkMsg.data[2] == 0xF1 &&
                receivedEvent.tagData.canRxOkMsg.data[3] == 0x8B)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                string manufacture = $"{_manufactureBytes[channel, 0]:X02}{_manufactureBytes[channel, 1]:X02}{_manufactureBytes[channel, 2]:X02}{_manufactureBytes[channel, 3]:X02}";
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Write : [ {manufacture} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Write step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Write step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Write complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Write complete");
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_ManufactureReadSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Manufacture Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Manufacture Read]");
            // 측정 상태 표시
            _overalResult[channel].Manufacture.State = TestStates.Running;
            _overalResult[channel].Manufacture.Value = "";
            _overalResult[channel].Manufacture.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].Manufacture);
            // 명령 전송
            //_manufactureBytes 초기화
            for (int i = 0; i < 4; i++)
                _manufactureBytes[channel, i] = 0;
            Send_ManufactureRead(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_ManufactureReadWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.ManufactureReadSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Read Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Read Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].Manufacture.State = TestStates.Failed;
                    _overalResult[channel].Manufacture.Value = "";
                    _overalResult[channel].Manufacture.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].Manufacture);
                    SetTestStep(channel, NFCTouchTestStep.DarkCurrentStart);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x62 &&
                receivedEvent.tagData.canRxOkMsg.data[2] == 0xF1 &&
                receivedEvent.tagData.canRxOkMsg.data[3] == 0x8B)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                _manufactureBytes[channel, 0] = receivedEvent.tagData.canRxOkMsg.data[4];
                _manufactureBytes[channel, 1] = receivedEvent.tagData.canRxOkMsg.data[5];
                _manufactureBytes[channel, 2] = receivedEvent.tagData.canRxOkMsg.data[6];
                _manufactureBytes[channel, 3] = receivedEvent.tagData.canRxOkMsg.data[7];
                string manufacture = $"{_manufactureBytes[channel, 0]:X02}{_manufactureBytes[channel, 1]:X02}{_manufactureBytes[channel, 2]:X02}{_manufactureBytes[channel, 3]:X02}";
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Read : [ {manufacture} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Manufacture Read complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Manufacture Read complete");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.Manufacture;
                if (manufacture.Length == testSpec.MaxString.Length)
                {
                    int month = Convert.ToInt16(manufacture.Substring(4, 2));
                    int day = Convert.ToInt16(manufacture.Substring(6, 2));
                    if (month < 1 || month > 12 || day < 1 || day > 31)
                        _overalResult[channel].Manufacture.State = TestStates.Failed;
                    else
                        _overalResult[channel].Manufacture.State = TestStates.Pass;
                }
                else
                {
                    _overalResult[channel].Manufacture.State = TestStates.Failed;
                }
                _overalResult[channel].Manufacture.Value = $"{manufacture}";
                _overalResult[channel].Manufacture.Result = $"{manufacture}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].Manufacture);
                NextTestStep(channel);
                // NM 중지
                _enableNM[channel] = false;
                _enableNFC[channel] = false;
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_DarkCurrentStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.DarkCurrent.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.LowPowerOff);
                return;
            }
            //if (_tickStepInterval[channel].LessThan(1000))
            //    return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Dark Current]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Dark Current]");
            // 측정 상태 표시
            _overalResult[channel].DarkCurrent.State = TestStates.Running;
            _overalResult[channel].DarkCurrent.Value = "";
            _overalResult[channel].DarkCurrent.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].DarkCurrent);
            if (!GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
            // NM 중지
            _enableNM[channel] = false;
            _enableNFC[channel] = false;
            NextTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_DarkCurrentWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            GSystem.TimerDarkCurrent[channel].Start();
            _tickStepElapse[channel].Reset();
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_DarkCurrentUpdate(int channel)
        {
            // 암전류 측정 중...측정 시간, 전류 업데이트
            //_overalResult[channel].DarkCurrent.Result = $"측정 중 ({GSystem.TimerDarkCurrent[channel].GetElapsedSeconds():F1} 초)";
            //OnTestStepProgressChanged(channel, _overalResult[channel].DarkCurrent);
            if (GSystem.TimerDarkCurrent[channel].LessThan(10000))
                return;
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_DarkCurrentComplete(int channel)
        {
            if (channel == CH1)
                DartCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch1_current_lo);
            else
                DartCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch2_current_lo);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Dark Current : [ {DartCurrent} uA ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Dark Current step time: [ {_tickStepElapse[channel].GetElapsedSeconds():F1} sec ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Dark Current step time: [ {_tickStepElapse[channel].GetElapsedSeconds():F1} sec ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Dark Current complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Dark Current complete");
            // 결과 판정
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.DarkCurrent;
            if (DartCurrent < testSpec.MinValue || DartCurrent > testSpec.MaxValue)
                _overalResult[channel].DarkCurrent.State = TestStates.Failed;
            else
                _overalResult[channel].DarkCurrent.State = TestStates.Pass;
            _overalResult[channel].DarkCurrent.Value = $"{DartCurrent}";
            _overalResult[channel].DarkCurrent.Result = $"{DartCurrent} uA";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].DarkCurrent);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_LowPowerOff(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power Off]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power Off]");
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_LowPowerOffWait(int channel)
        {
            // 완료 대기
            if (GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel) || GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                return;
            if (!GSystem.DedicatedCTRL.GetCommandTestInit(channel) || !GSystem.DedicatedCTRL.GetCompleteTestInit(channel))
                return;
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Complete");
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_HighPowerOn(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power On]");
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_HighPowerOnWait(int channel)
        {
            // 완료 대기
            if (!GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel) || !GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Power On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Power On Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Power On Complete");
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_NmWakeUpWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(5000))
            {
                SetTestStep(channel, NFCTouchTestStep.LowPowerOff);
                return;
            }
            if (rxCanID != GSystem.ProductSettings.NM_ResID)
                return;
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_PowerOnResetWait(int channel)
        {
            if (_tickStepInterval[channel].LessThan(2500))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Power On Reset Wait Complete: {_tickStepInterval[channel].GetElapsedMilliseconds()} ms]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Power On Reset Wait Complete: {_tickStepInterval[channel].GetElapsedMilliseconds()} ms]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] NM Message send start");
            GSystem.TraceMessage($"[CH.{channel + 1}] NM Message send start");
            // NFC 입력 요청 메시지 송신
            Send_NFC(channel, true);
            // NM & NFC 재개
            _enableNM[channel] = true;
            _enableNFC[channel] = true;
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_ExtendedSessionStart(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Extended Session]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Extended Session]");
            // Extended session으로 변경
            Send_ExtendedSession(channel, true);
            Thread.Sleep(5);
            Send_ExtendedSession(channel);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_ExtendedSessionWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.ExtendedSessionStart);
                    return;
                }
                else
                {
                    _isCancel[channel] = true;
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] != 0x50 || receivedEvent.tagData.canRxOkMsg.data[2] != 0x03)
                return;
            GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
            GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            // Extend Session으로 변경 완료
            GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session complete");
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_PLightTurnOnSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.PLightTurnOn.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.MotionMoveTouchStart);
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
                PLightOffCurrentValue = (int)(DedicatedCTRL.Reg_03h_ch1_current_lo / 1000);
            else
                PLightOffCurrentValue = (int)(DedicatedCTRL.Reg_03h_ch2_current_lo / 1000);
            GSystem.TraceMessage($"P-Light LO current = {(int)(DedicatedCTRL.Reg_03h_ch1_current_lo / 1000)} mA");
            GSystem.TraceMessage($"P-Light HI current = {DedicatedCTRL.Reg_03h_ch1_current_hi} mA");
            GSystem.TraceMessage($"P-Light Off current = {PLightOffCurrentValue} mA");
            // 명령 전송
            Send_PLight(channel, true, true);
            _enablePLight[channel] = true;
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_PLightTurnOnWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light ON Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] P-Light ON Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.PLightTurnOnSend);
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
                    SetTestStep(channel, NFCTouchTestStep.MotionMoveTouchStart);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.PLightResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[3] == 0x02)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn On [ {receivedEvent.tagData.canRxOkMsg.data[3]} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn On step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn On complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn On complete");
                PLightTurnOnValue = 1;
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.PLightTurnOn;
                if (PLightTurnOnValue == testSpec.MaxValue)
                    _overalResult[channel].PLightTurnOn.State = TestStates.Pass;
                else
                    _overalResult[channel].PLightTurnOn.State = TestStates.Failed;
                _overalResult[channel].PLightTurnOn.Value = $"{receivedEvent.tagData.canRxOkMsg.data[3]}";
                _overalResult[channel].PLightTurnOn.Result = $"{PLightTurnOnValue}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].PLightTurnOn);
                NextTestStep(channel);
                _tickStepInterval[channel].Reset();
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_PLightCurrentSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.PLightCurrent.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.PLightAmbientSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [P-Light Current]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [P-Light Current]");
            // 측정 상태 표시
            _overalResult[channel].PLightCurrent.State = TestStates.Running;
            _overalResult[channel].PLightCurrent.Value = "";
            _overalResult[channel].PLightCurrent.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightCurrent);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_PLightCurrentWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            // P-Light가 완전히 ON 되는데 약 1.5초정도 걸린다. 전류와 조도 측정을 위해 충분히 대기한다.
            if (_tickStepInterval[channel].LessThan(2000))
                return;
            // 전류 측정
            // 조도 측정
            if (channel == CH1)
            {
                PLightOnCurrentValue = (int)(DedicatedCTRL.Reg_03h_ch1_current_lo / 1000) - PLightOffCurrentValue;
                PLightAmbientValue = DedicatedCTRL.Reg_03h_ch1_light_lux;
            }
            else
            {
                PLightOnCurrentValue = (int)(DedicatedCTRL.Reg_03h_ch2_current_lo / 1000) - PLightOffCurrentValue;
                PLightAmbientValue = DedicatedCTRL.Reg_03h_ch2_light_lux;
            }
            GSystem.TraceMessage($"P-Light LO current = {(int)(DedicatedCTRL.Reg_03h_ch1_current_lo / 1000)} mA");
            GSystem.TraceMessage($"P-Light HI current = {DedicatedCTRL.Reg_03h_ch1_current_hi} mA");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Current [ {PLightOnCurrentValue} mA ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Current [ {PLightOnCurrentValue} mA ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Current step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Current step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Current complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Current complete");
            // 결과 판정
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.PLightCurrent;
            if (PLightOnCurrentValue < testSpec.MinValue || PLightOnCurrentValue > testSpec.MaxValue)
                _overalResult[channel].PLightCurrent.State = TestStates.Failed;
            else
                _overalResult[channel].PLightCurrent.State = TestStates.Pass;
            _overalResult[channel].PLightCurrent.Value = $"{PLightOnCurrentValue}";
            _overalResult[channel].PLightCurrent.Result = $"{PLightOnCurrentValue} mA";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightCurrent);
            NextTestStep(channel);
            _tickStepInterval[channel].Reset();
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_PLightAmbientSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.PLightAmbient.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.TouchCapacitanceStart);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [P-Light Ambient]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [P-Light Ambient]");
            // 측정 상태 표시
            _overalResult[channel].PLightAmbient.State = TestStates.Running;
            _overalResult[channel].PLightAmbient.Value = "";
            _overalResult[channel].PLightAmbient.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightAmbient);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_PLightAmbientWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Ambient [ {PLightAmbientValue} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Ambient step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Ambient step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Ambient complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Ambient complete");
            // 결과 판정
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.PLightAmbient;
            if (PLightAmbientValue < testSpec.MinValue || PLightAmbientValue > testSpec.MaxValue)
                _overalResult[channel].PLightAmbient.State = TestStates.Failed;
            else
                _overalResult[channel].PLightAmbient.State = TestStates.Pass;
            _overalResult[channel].PLightAmbient.Value = $"{PLightAmbientValue}";
            _overalResult[channel].PLightAmbient.Result = $"{PLightAmbientValue}";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].PLightAmbient);
            NextTestStep(channel);
            _tickStepInterval[channel].Reset();
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_PLightTurnOffSend(int channel)
        {
            //_tickStepElapse[channel].Reset();
            //GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [P-Light Turn Off]");
            //GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [P-Light Turn Off]");
            //Send_PLight(channel, false, true);

            // P-Light ON message 중지
            _enablePLight[channel] = false;
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_PLightTurnOffWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            //if (_tickStepTimeout[channel].MoreThan(2000))
            //{
            //    if (++_retryCount[channel] < MaxRetryCount)
            //    {
            //        GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light OFF Timeout! Retry: [ {_retryCount[channel]} ]");
            //        GSystem.TraceMessage($"[CH.{channel + 1}] P-Light OFF Timeout! Retry: [ {_retryCount[channel]} ]");
            //        SetTestStep(channel, NFCTouchTestStep.PLightTurnOffSend);
            //        return;
            //    }
            //    else
            //    {
            //        // timeout
            //        _isCancel[channel] = true;
            //        SetTestStep(channel, NFCTouchTestStep.PowerOff);
            //        return;
            //    }
            //}
            //if (rxCanID != GSystem.ProductSettings.PLightResID)
            //    return;
            //if (receivedEvent.tagData.canRxOkMsg.data[3] == 0x00)
            //{
            //    GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
            //    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn Off [ {receivedEvent.tagData.canRxOkMsg.data[3]} ]");
            //    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            //    GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            //    GSystem.Logger.Info ($"[CH.{channel + 1}] P-Light Turn Off complete");
            //    GSystem.TraceMessage($"[CH.{channel + 1}] P-Light Turn Off complete");
            //    PLightTurnOnValue = 1;
            //    NextTestStep(channel);
            //    _retryCount[channel] = 0;
            //    _tickStepInterval[channel].Reset();
            //}

            // P-Light OFF 확인
            if (_tickStepInterval[channel].LessThan(500))
                return;
            NextTestStep(channel);

        }
        // Touch 위치로 이동
        private void NFCTouchTestStep_MotionMoveTouchStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.LockSen.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.MotionMoveCancelStart);
                return;
            }
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Move to Touch]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Move to Touch]");
            GSystem.MiPLC.SetMoveTouchYStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionMoveTouchWait(int channel)
        {
            // 완료 대기
            if (GSystem.MiPLC.GetMoveTouchYStart(channel))
            {
                if (!GSystem.MiPLC.GetMoveTouchYComplete(channel))
                    return;
                GSystem.MiPLC.SetMoveTouchYStart(channel, false);
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Move to Touch step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Move to Touch step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Move to Touch Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Move to Touch Complete");

            NextTestStep(channel);
        }

        private void NFCTouchTestStep_TouchTestStart(int channel)
        {
            // 측정 상태 표시
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Touch Test]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Touch Test]");
            _overalResult[channel].LockSen.State = TestStates.Running;
            _overalResult[channel].LockSen.Value = "";
            _overalResult[channel].LockSen.Result = "측정 중";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
            // 옵션에 따라서 Can 검사로 할지 Capacitance 검사로 할지 결정한다.
            if (GSystem.ProductSettings.TestItemSpecs.LockSen.Option == 0)
            {
                // Capacitance
                // XCP 시작

                GSystem.isTouchFirstExecute[channel] = true;
                GSystem.isTouchFastMutualIdleAverage[channel] = false;
                GSystem.isTouchFastSelfIdleAverage[channel] = false;
                GSystem.TouchFastMutualList[channel].Clear();
                GSystem.TouchFastSelfList[channel].Clear();
                GSystem.isTouchFastMutualComplete[channel] = false;
                GSystem.isTouchFastSelfComplete[channel] = false;

                // GetTouchAsync()를 백그라운드로 실행
                Task.Run(async () => await GetTouchAsync(channel));

                SetTestStep(channel, NFCTouchTestStep.TouchCap_ZDownStart);
                return;
            }
            else
            {
                // Can
                SetTestStep(channel, NFCTouchTestStep.TouchCan_ZDownStart);
            }
            _retryCount[channel] = 0;
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_TouchCan_ZDownStart(int channel)
        {
            // Touch Z축 하강 시작
            if (_tickStepInterval[channel].MoreThan(200))
            {
                NextTestStep(channel);
                _tickStepTimeout[channel].Reset();
                // Touch 하강
                GSystem.MiPLC.SetTouchZDownStart(channel, true);
                _tickStepInterval[channel].Reset();
            }
        }
        private void NFCTouchTestStep_TouchCan_ZDownWait(int channel)
        {
            // Touch Z축 하강 완료 대기
            if (GSystem.MiPLC.GetTouchZDownStart(channel))
            {
                if (_tickStepTimeout[channel].MoreThan(2000))
                {
                    if (++_retryCount[channel] < MaxRetryCount)
                    {
                        // retry
                        GSystem.MiPLC.SetTouchZDownStart(channel, false);
                        SetTestStep(channel, NFCTouchTestStep.TouchCan_ZDownStart);
                    }
                    else
                    {
                        // timeout error
                        GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Test Error! [Z-Axis down error]");
                        GSystem.TraceMessage($"[CH.{channel + 1}] Touch Test Error! [Z-Axis down error]");
                        // 결과 판정
                        _overalResult[channel].LockSen.State = TestStates.Failed;
                        _overalResult[channel].LockSen.Value = "";
                        _overalResult[channel].LockSen.Result = "Timeout";
                        _overalResult[channel].LockCan.State = TestStates.Failed;
                        _overalResult[channel].LockCan.Value = "";
                        _overalResult[channel].LockCan.Result = "Timeout";
                        // 동작 상태 표시
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                        SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                        return;
                    }
                }
                if (!GSystem.MiPLC.GetTouchZDownComplete(channel))
                    return;
                GSystem.MiPLC.SetTouchZDownStart(channel, false);
            }
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_TouchCan_LockSenStart(int channel)
        {
            // HardwireTest 명령 전송
            Send_ExtendedSession(channel);
            Thread.Sleep(10);
            Send_HardwireTest(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_TouchCan_LockSenWait(int channel)
        {
            // Hardwire lock signal 확인
            if (GSystem.DedicatedCTRL.GetLockSignal(channel))
            {
                // 결과 판정
                TestSpec testLockSen = GSystem.ProductSettings.TestItemSpecs.LockSen;
                _overalResult[channel].LockSen.State = TestStates.Pass;
                _overalResult[channel].LockSen.Value = "";
                _overalResult[channel].LockSen.Result = $"{testLockSen.MaxValue}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                NextTestStep(channel);
            }
            else
            {
                if (_tickStepTimeout[channel].MoreThan(2000))
                {
                    if (++_retryCount[channel] < MaxRetryCount)
                    {
                        // timeout!
                        // retry - Touch 상승
                        GSystem.MiPLC.SetTouchZUpStart(channel, true);
                        SetTestStep(channel, NFCTouchTestStep.TouchCan_Retry);
                    }
                    else
                    {
                        // timeout error
                        GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Hardwire Error!");
                        GSystem.TraceMessage($"[CH.{channel + 1}] Touch Hardwire Error!");
                        // 결과 판정
                        _overalResult[channel].LockSen.State = TestStates.Failed;
                        _overalResult[channel].LockSen.Value = "";
                        _overalResult[channel].LockSen.Result = "Timeout";
                        _overalResult[channel].LockCan.State = TestStates.Failed;
                        _overalResult[channel].LockCan.Value = "";
                        _overalResult[channel].LockCan.Result = "Timeout";
                        // 동작 상태 표시
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                        SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                        return;
                    }
                }
            }
        }
        private void NFCTouchTestStep_TouchCan_LockCanWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            // NM 응답에서 lock bit 확인
            if (rxCanID == GSystem.ProductSettings.NM_ResID)
            {
                byte touchState = (byte)(receivedEvent.tagData.canRxOkMsg.data[4] & 0x01);
                if (touchState != 0x00)
                {
                    GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                    // TOUCH 입력
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock State [ {touchState:X02} ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock complete");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock complete");
                    // 결과 판정
                    TestSpec testLockCan = GSystem.ProductSettings.TestItemSpecs.LockCan;
                    _overalResult[channel].LockCan.State = TestStates.Pass;
                    _overalResult[channel].LockCan.Value = $"0x{touchState:X02}";
                    _overalResult[channel].LockCan.Result = $"{testLockCan.MaxValue}";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                    // 다음 스텝으로
                    SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                    _tickStepInterval[channel].Reset();
                }
                else
                {
                    if (_tickStepTimeout[channel].MoreThan(2000))
                    {
                        // 에러 처리
                        if (++_retryCount[channel] < MaxRetryCount)
                        {
                            // timeout!
                            // retry - Touch 상승
                            GSystem.MiPLC.SetTouchZUpStart(channel, true);
                            SetTestStep(channel, NFCTouchTestStep.TouchCan_Retry);
                        }
                        else
                        {
                            GSystem.TouchLockState[channel] = 0;
                            GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock State [ {receivedEvent.tagData.canRxOkMsg.data[3]:X02} ]");
                            GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                            GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                            GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Timeout Error!");
                            GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock Timeout Error!");
                            // 결과 판정
                            _overalResult[channel].LockCan.State = TestStates.Failed;
                            _overalResult[channel].LockCan.Value = $"0x{touchState:X02}";
                            _overalResult[channel].LockCan.Result = $"0";
                            // 동작 상태 표시
                            OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                            // 다음 스텝으로
                            SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                            _tickStepInterval[channel].Reset();
                            _retryCount[channel] = 0;
                        }
                    }
                }
            }
        }
        private void NFCTouchTestStep_TouchCan_Retry(int channel)
        {
            // Touch 상승 확인
            if (GSystem.MiPLC.GetTouchZUpStart(channel))
            {
                if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                    return;
                GSystem.MiPLC.SetTouchZUpStart(channel, false);
            }
            SetTestStep(channel, NFCTouchTestStep.TouchCan_ZDownStart);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_TouchCap_ZDownStart(int channel)
        {
            // Touch Z축 하강 시작
            if (_tickStepInterval[channel].MoreThan(200))
            {
                GSystem.isTouchFirstExecute[channel] = false;
                NextTestStep(channel);
                _tickStepTimeout[channel].Reset();
                // Touch 하강
                GSystem.MiPLC.SetTouchZDownStart(channel, true);
                _tickStepInterval[channel].Reset();
            }
        }
        private void NFCTouchTestStep_TouchCap_ZDownWait(int channel)
        {
            // Touch Z축 하강 완료 대기
            if (GSystem.MiPLC.GetTouchZDownStart(channel))
            {
                if (_tickStepTimeout[channel].MoreThan(2000))
                {
                    if (++_retryCount[channel] < MaxRetryCount)
                    {
                        // retry
                        GSystem.MiPLC.SetTouchZDownStart(channel, false);
                        SetTestStep(channel, NFCTouchTestStep.TouchCan_ZDownStart);
                    }
                    else
                    {
                        // timeout error
                        GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Test Error! [Z-Axis down error]");
                        GSystem.TraceMessage($"[CH.{channel + 1}] Touch Test Error! [Z-Axis down error]");
                        // 결과 판정
                        _overalResult[channel].LockSen.State = TestStates.Failed;
                        _overalResult[channel].LockSen.Value = "";
                        _overalResult[channel].LockSen.Result = "Timeout";
                        _overalResult[channel].LockCan.State = TestStates.Failed;
                        _overalResult[channel].LockCan.Value = "";
                        _overalResult[channel].LockCan.Result = "Timeout";
                        // 동작 상태 표시
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                        SetTestStep(channel, NFCTouchTestStep.MotionMoveCancelStart);
                        return;
                    }
                }
                if (!GSystem.MiPLC.GetTouchZDownComplete(channel))
                    return;
                GSystem.MiPLC.SetTouchZDownStart(channel, false);
            }
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_TouchCap_LockSenStart(int channel)
        {
            // HardwireTest 명령 전송
            Send_ExtendedSession(channel);
            Thread.Sleep(10);
            Send_HardwireTest(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_TouchCap_LockSenWait(int channel)
        {
            // Hardwire lock signal 확인
            if (GSystem.DedicatedCTRL.GetLockSignal(channel))
            {
                // 결과 판정
                TestSpec testLockSen = GSystem.ProductSettings.TestItemSpecs.LockSen;
                _overalResult[channel].LockSen.State = TestStates.Pass;
                _overalResult[channel].LockSen.Value = "";
                _overalResult[channel].LockSen.Result = $"{testLockSen.MaxValue}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                NextTestStep(channel);
            }
            else
            {
                if (_tickStepTimeout[channel].MoreThan(2000))
                {
                    if (++_retryCount[channel] < MaxRetryCount)
                    {
                        // timeout!
                        // retry - Touch 상승
                        GSystem.MiPLC.SetTouchZUpStart(channel, true);
                        SetTestStep(channel, NFCTouchTestStep.TouchCap_Retry);
                    }
                    else
                    {
                        // timeout error
                        GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Test Error! [Z-Axis down error]");
                        GSystem.TraceMessage($"[CH.{channel + 1}] Touch Test Error! [Z-Axis down error]");
                        // 결과 판정
                        _overalResult[channel].LockSen.State = TestStates.Failed;
                        _overalResult[channel].LockSen.Value = "";
                        _overalResult[channel].LockSen.Result = "Timeout";
                        _overalResult[channel].LockCan.State = TestStates.Failed;
                        _overalResult[channel].LockCan.Value = "";
                        _overalResult[channel].LockCan.Result = "Timeout";
                        // 동작 상태 표시
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                        OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                        SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                        return;
                    }
                }
            }
        }
        private void NFCTouchTestStep_TouchCap_LockCanWait(int channel)
        {
            // Touch 판정 완료 대기
            if (GSystem.isTouchFastMutualComplete[channel] == true && GSystem.isTouchFastSelfComplete[channel] == true)
            {
                // 터치 검사 종료
                TouchStepExit[channel] = true;
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance Fast Mutual Delta [ {GSystem.deltaTouchFastMutual[channel]} ]({GSystem.ProductSettings.XCPAddress.TouchFastMutual.Address})");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance Fast Self   Delta [ {GSystem.deltaTouchFastSelf[channel]} ]({GSystem.ProductSettings.XCPAddress.TouchFastSelf.Address})");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Touch Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Touch Capacitance Complete");
                // 간이검사기의 특징 : 사람이 검출하기 때문에 여기까지 왔으면 OK, EOL 설비에서는 다르다
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.LockCan;
                _overalResult[channel].LockCan.State = TestStates.Pass;
                _overalResult[channel].LockCan.Value = $"{GSystem.deltaTouchFastMutual[channel]},{GSystem.deltaTouchFastSelf[channel]}";
                _overalResult[channel].LockCan.Result = $"{testSpec.MaxValue}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                _tickStepInterval[channel].Reset();
            }
            else
            {
                if (_tickStepTimeout[channel].MoreThan(5000))
                {
                    // 에러 처리

                    // 터치 검사 종료
                    TouchStepExit[channel] = true;
                    // Full proof 닫기
                    OnShowFullProofMessage(channel, "", false);

                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance Timeout!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Capacitance Timeout!");
                    // 결과 판정
                    _overalResult[channel].LockCan.State = TestStates.Failed;
                    _overalResult[channel].LockCan.Value = "";
                    _overalResult[channel].LockCan.Result = "Timeout";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                    // Touch 상승
                    SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                    _tickStepInterval[channel].Reset();
                }
            }
        }
        private void NFCTouchTestStep_TouchCap_Retry(int channel)
        {
            // Touch 상승 확인
            if (GSystem.MiPLC.GetTouchZUpStart(channel))
            {
                if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                    return;
                GSystem.MiPLC.SetTouchZUpStart(channel, false);
            }
            SetTestStep(channel, NFCTouchTestStep.TouchCap_ZDownStart);
            _tickStepInterval[channel].Reset();
        }













        private void NFCTouchTestStep_TouchLockStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.LockSen.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.MotionMoveCancelStart);
                return;
            }
            // 옵션에 따라서 Can 검사로 할지 Capacitance 검사로 할지 결정한다.
            if (GSystem.ProductSettings.TestItemSpecs.LockSen.Option == 0)
            {
                // Capacitance 검사로 전환
                SetTestStep(channel, NFCTouchTestStep.TouchCapacitanceStart);
                return;
            }
            // 측정 상태 표시
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Touch Lock]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Touch Lock]");
            _overalResult[channel].LockSen.State = TestStates.Running;
            _overalResult[channel].LockSen.Value = "";
            _overalResult[channel].LockSen.Result = "측정 중";
            _overalResult[channel].LockCan.State = TestStates.Running;
            _overalResult[channel].LockCan.Value = "";
            _overalResult[channel].LockCan.Result = "측정 중";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
            OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_MotionTouchZDownStart(int channel)
        {
            // Touch 판정 시작
            if (_tickStepInterval[channel].MoreThan(200))
            {
                NextTestStep(channel);
                _tickStepTimeout[channel].Reset();
                // Touch 하강
                GSystem.MiPLC.SetTouchZDownStart(channel, true);
                _tickStepInterval[channel].Reset();
            }
        }
        private void NFCTouchTestStep_MotionTouchZDownWait(int channel)
        {
            // Touch Z축 하강 완료 대기
            if (GSystem.MiPLC.GetTouchZDownStart(channel))
            {
                if (!GSystem.MiPLC.GetTouchZDownComplete(channel))
                    return;
                GSystem.MiPLC.SetTouchZDownStart(channel, false);
            }
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_TouchLockWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (rxCanID == GSystem.ProductSettings.NM_ResID)
            {
                bool hardwireSignal = GSystem.DedicatedCTRL.GetLockSignal(channel);
                byte touchState = (byte)(receivedEvent.tagData.canRxOkMsg.data[4] & 0x01);
                if (hardwireSignal && touchState != 0x00)
                {
                    GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                    // TOUCH 입력
                    GSystem.TouchLockState[channel] = 1;
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock State [ {touchState:X02} ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock complete");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock complete");
                    // 결과 판정
                    TestSpec testLockSen = GSystem.ProductSettings.TestItemSpecs.LockSen;
                    _overalResult[channel].LockSen.State = TestStates.Pass;
                    _overalResult[channel].LockSen.Value = $"0x{touchState:X02}";
                    _overalResult[channel].LockSen.Result = $"{testLockSen.MaxValue}";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                    TestSpec testLockCan = GSystem.ProductSettings.TestItemSpecs.LockCan;
                    _overalResult[channel].LockCan.State = TestStates.Pass;
                    _overalResult[channel].LockCan.Value = $"0x{touchState:X02}";
                    _overalResult[channel].LockCan.Result = $"{testLockCan.MaxValue}";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                    // 다음 스텝으로
                    SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                    _tickStepInterval[channel].Reset();
                }
                else
                {
                    if (_tickStepTimeout[channel].MoreThan(2000))
                    {
                        // 에러 처리
                        if (++_retryCount[channel] < MaxRetryCount)
                        {
                            // timeout!
                            // Touch 상승
                            GSystem.MiPLC.SetTouchZUpStart(channel, true);
                            SetTestStep(channel, NFCTouchTestStep.TouchLockRetry);
                        }
                        else
                        {
                            GSystem.TouchLockState[channel] = 0;
                            GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock State [ {receivedEvent.tagData.canRxOkMsg.data[3]:X02} ]");
                            GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                            GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                            GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Lock Timeout Error!");
                            GSystem.TraceMessage($"[CH.{channel + 1}] Touch Lock Timeout Error!");
                            // 결과 판정
                            _overalResult[channel].LockSen.State = TestStates.Failed;
                            _overalResult[channel].LockSen.Value = $"0x{touchState:X02}";
                            _overalResult[channel].LockSen.Result = $"0";
                            // 동작 상태 표시
                            OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                            _overalResult[channel].LockCan.State = TestStates.Failed;
                            _overalResult[channel].LockCan.Value = $"0x{touchState:X02}";
                            _overalResult[channel].LockCan.Result = $"0";
                            // 동작 상태 표시
                            OnTestStepProgressChanged(channel, _overalResult[channel].LockCan);
                            // 다음 스텝으로
                            SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                            _tickStepInterval[channel].Reset();
                            _retryCount[channel] = 0;
                        }
                    }
                }
            }
        }
        private void NFCTouchTestStep_TouchLockRetry(int channel)
        {
            // Touch 상승 확인
            if (GSystem.MiPLC.GetTouchZUpStart(channel))
            {
                if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                    return;
                GSystem.MiPLC.SetTouchZUpStart(channel, false);
            }
            SetTestStep(channel, NFCTouchTestStep.MotionTouchZDownStart);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_TouchCapacitanceStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.LockSen.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.MotionMoveCancelStart);
                return;
            }
            // 측정 상태 표시
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Touch Capacitance]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Touch Capacitance]");
            _overalResult[channel].LockSen.State = TestStates.Running;
            _overalResult[channel].LockSen.Value = "";
            _overalResult[channel].LockSen.Result = "측정 중";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);

            GSystem.isTouchFirstExecute[channel] = true;
            GSystem.isTouchFastMutualIdleAverage[channel] = false;
            GSystem.isTouchFastSelfIdleAverage[channel] = false;
            GSystem.TouchFastMutualList[channel].Clear();
            GSystem.TouchFastSelfList[channel].Clear();
            GSystem.isTouchFastMutualComplete[channel] = false;
            GSystem.isTouchFastSelfComplete[channel] = false;

            // GetTouchAsync()를 백그라운드로 실행
            Task.Run(async () => await GetTouchAsync(channel));

            // Full proof 표시
            //OnShowFullProofMessage(channel, "TOUCH를 접촉하세요", true);

            SetTestStep(channel, NFCTouchTestStep.TouchCapacitancePrepare);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_TouchCapacitancePrepare(int channel)
        {
            // Touch 판정 시작
            if (_tickStepInterval[channel].MoreThan(500))
            {
                GSystem.isTouchFirstExecute[channel] = false;
                SetTestStep(channel, NFCTouchTestStep.TouchCapacitanceWait);
                _tickStepTimeout[channel].Reset();
                // Touch 하강
                GSystem.MiPLC.SetTouchZDownStart(channel, true);
                _tickStepInterval[channel].Reset();
            }
        }
        private void NFCTouchTestStep_TouchCapacitanceWait(int channel)
        {
            // Touch Z축 하강 완료 대기
            if (GSystem.MiPLC.GetTouchZDownStart(channel))
            {
                if (!GSystem.MiPLC.GetTouchZDownComplete(channel))
                    return;
                GSystem.MiPLC.SetTouchZDownStart(channel, false);
            }
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            // Touch 판정 완료 대기
            if (GSystem.isTouchFastMutualComplete[channel] == true && GSystem.isTouchFastSelfComplete[channel] == true)
            {
                // 터치 검사 종료
                TouchStepExit[channel] = true;
                // Full proof 닫기
                OnShowFullProofMessage(channel, "", false);

                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance Fast Mutual Delta [ {GSystem.deltaTouchFastMutual[channel]} ]({GSystem.ProductSettings.XCPAddress.TouchFastMutual.Address})");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance Fast Self   Delta [ {GSystem.deltaTouchFastSelf[channel]} ]({GSystem.ProductSettings.XCPAddress.TouchFastSelf.Address})");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Touch Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Touch Capacitance Complete");
                // 간이검사기의 특징 : 사람이 검출하기 때문에 여기까지 왔으면 OK, EOL 설비에서는 다르다
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.LockSen;
                _overalResult[channel].LockSen.State = TestStates.Pass;
                _overalResult[channel].LockSen.Value = $"{GSystem.deltaTouchFastMutual[channel]},{GSystem.deltaTouchFastSelf[channel]}";
                _overalResult[channel].LockSen.Result = $"{testSpec.MaxValue}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                NextTestStep(channel);
                _tickStepInterval[channel].Reset();
            }
            else
            {
                if (_tickStepTimeout[channel].MoreThan(5000))
                {
                    // 에러 처리

                    // 터치 검사 종료
                    TouchStepExit[channel] = true;
                    // Full proof 닫기
                    OnShowFullProofMessage(channel, "", false);

                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Touch Capacitance Timeout!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Touch Capacitance Timeout!");
                    // 결과 판정
                    _overalResult[channel].LockSen.State = TestStates.Failed;
                    _overalResult[channel].LockSen.Value = "";
                    _overalResult[channel].LockSen.Result = "Timeout";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].LockSen);
                    // Touch 상승
                    SetTestStep(channel, NFCTouchTestStep.MotionTouchZUpStart);
                    _tickStepInterval[channel].Reset();
                }
            }
        }

        // Touch 상승
        private void NFCTouchTestStep_MotionTouchZUpStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            // Touch 상승
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Touch Up]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Touch Up]");
            GSystem.MiPLC.SetTouchZUpStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionTouchZUpWait(int channel)
        {
            // 완료 대기
            if (GSystem.MiPLC.GetTouchZUpStart(channel))
            {
                if (!GSystem.MiPLC.GetTouchZUpComplete(channel))
                    return;
                GSystem.MiPLC.SetTouchZUpStart(channel, false);
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Touch Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Touch Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Touch Up Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Touch Up Complete");
            NextTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        // Cancel 위치로 이동
        private void NFCTouchTestStep_MotionMoveCancelStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Cancel.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.MotionMoveNFC_Start);
                return;
            }
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Move to Cancel]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Move to Cancel]");
            GSystem.MiPLC.SetMoveCancelYStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionMoveCancelWait(int channel)
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
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_CancelCapacitanceStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Cancel.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.MotionMoveNFC_Start);
                return;
            }
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            // 측정 상태 표시
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Cancel Capacitance]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Cancel Capacitance]");
            _overalResult[channel].Cancel.State = TestStates.Running;
            _overalResult[channel].Cancel.Value = "";
            _overalResult[channel].Cancel.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].Cancel);

            GSystem.isCancelFirstExecute[channel] = true;
            GSystem.isCancelFastSelfIdleAverage[channel] = false;
            GSystem.isCancelSlowSelfIdleAverage[channel] = false;
            GSystem.CancelFastSelfList[channel].Clear();
            GSystem.CancelSlowSelfList[channel].Clear();
            GSystem.isCancelFastSelfComplete[channel] = false;
            GSystem.isCancelSlowSelfComplete[channel] = false;

            // GetTouchAsync()를 백그라운드로 실행
            Task.Run(async () => await GetCancelAsync(channel));

            // Full proof 표시
            //OnShowFullProofMessage(channel, "CANCEL을 접촉하세요", true);

            SetTestStep(channel, NFCTouchTestStep.CancelCapacitancePrepare);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_CancelCapacitancePrepare(int channel)
        {
            // Cancel 판정 시작
            if (_tickStepInterval[channel].MoreThan(500))
            {
                GSystem.isCancelFirstExecute[channel] = false;
                SetTestStep(channel, NFCTouchTestStep.CancelCapacitanceWait);
                _tickStepTimeout[channel].Reset();
                // Cancel 상승
                GSystem.MiPLC.SetCancelZUpStart(channel, true);
                _tickStepInterval[channel].Reset();
            }
        }
        private void NFCTouchTestStep_CancelCapacitanceWait(int channel)
        {
            // Cancel Z축 상승 완료 대기
            if (GSystem.MiPLC.GetCancelZUpStart(channel))
            {
                if (!GSystem.MiPLC.GetCancelZUpComplete(channel))
                    return;
                GSystem.MiPLC.SetCancelZUpStart(channel, false);
            }
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            // Cancel 판정 완료 대기
            if (GSystem.isCancelFastSelfComplete[channel] == true && GSystem.isCancelSlowSelfComplete[channel] == true)
            {
                // Cancel 검사 종료
                CancelStepExit[channel] = true;
                // Full proof 닫기
                OnShowFullProofMessage(channel, "", false);

                GSystem.Logger.Info ($"[CH.{channel + 1}] Cancel Capacitance Fast Self Delta [ {GSystem.deltaCancelFastSelf[channel]} ]({GSystem.ProductSettings.XCPAddress.CancelFastSelf.Address})");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Cancel Capacitance Slow Self Delta [ {GSystem.deltaCancelSlowSelf[channel]} ]({GSystem.ProductSettings.XCPAddress.CancelSlowSelf.Address})");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Cancel Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Cancel Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Cancel Capacitance Complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Cancel Capacitance Complete");
                // 간이검사기의 특징 : 사람이 검출하기 때문에 여기까지 왔으면 OK, EOL 설비에서는 다르다
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.Cancel;
                _overalResult[channel].Cancel.State = TestStates.Pass;
                _overalResult[channel].Cancel.Value = $"{GSystem.deltaCancelFastSelf[channel]},{GSystem.deltaCancelSlowSelf[channel]}";
                _overalResult[channel].Cancel.Result = $"{testSpec.MaxValue}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].Cancel);
                NextTestStep(channel);
                _tickStepInterval[channel].Reset();
            }
            else
            {
                if (_tickStepTimeout[channel].MoreThan(5000))
                {
                    // 에러 처리

                    // Cancel 검사 종료
                    CancelStepExit[channel] = true;
                    // Full proof 닫기
                    OnShowFullProofMessage(channel, "", false);

                    GSystem.Logger.Info ($"[CH.{channel + 1}] Cancel Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Cancel Capacitance step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Cancel Capacitance Timeout!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Cancel Capacitance Timeout!");
                    // 결과 판정
                    _overalResult[channel].Cancel.State = TestStates.Failed;
                    _overalResult[channel].Cancel.Value = "";
                    _overalResult[channel].Cancel.Result = "Timeout";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].Cancel);
                    //SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    NextTestStep(channel);
                    _tickStepInterval[channel].Reset();
                }
            }
        }
        private void NFCTouchTestStep_MotionCancelZDownStart(int channel)
        {
            // Cancel 하강
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Cancel Down]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Cancel Down]");
            GSystem.MiPLC.SetCancelZDownStart(channel, true);
            NextTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_MotionCancelZDownWait(int channel)
        {
            // 완료 대기
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
            NextTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        // NFC 위치로 이동
        private void NFCTouchTestStep_MotionMoveNFC_Start(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.NFC.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.XcpPrepareSend);
                return;
            }
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion Move to NFC]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion Move to NFC]");
            GSystem.MiPLC.SetMoveNFCYStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionMoveNFC_Wait(int channel)
        {
            // 완료 대기
            if (GSystem.MiPLC.GetMoveNFCYStart(channel))
            {
                if (!GSystem.MiPLC.GetMoveNFCYComplete(channel))
                    return;
                GSystem.MiPLC.SetMoveNFCYStart(channel, false);
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Move to NFC step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Move to NFC step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Move to NFC Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Move to NFC Complete");
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_NfcCheckStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.NFC.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.DTCEraseSend);
                return;
            }
            _tickStepTimeout[channel].Reset();
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [NFC]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [NFC]");
            // 측정 상태 표시
            _overalResult[channel].NFC.State = TestStates.Running;
            _overalResult[channel].NFC.Value = "";
            _overalResult[channel].NFC.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].NFC);
            // NFC 입력 상태 초기화
            GSystem.NFC_State[channel] = 0;
            NextTestStep(channel);
            // NFC 하강
            GSystem.MiPLC.SetNFCZDownStart(channel, true);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_NfcCheckWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (GSystem.MiPLC.GetNFCZDownStart(channel))
            {
                if (!GSystem.MiPLC.GetNFCZDownComplete(channel))
                {
                    GSystem.MiPLC.SetNFCZDownStart(channel, false);
                }
            }
            if (_tickStepTimeout[channel].MoreThan(5000))
            {
                // 에러 처리

                // Full proof 닫기
                OnShowFullProofMessage(channel, "", false);

                GSystem.NFC_State[channel] = 0;
                GSystem.Logger.Info ($"[CH.{channel + 1}] NFC State [ {receivedEvent.tagData.canRxOkMsg.data[3]:X02} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] NFC step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] NFC step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] NFC Timeout!");
                GSystem.TraceMessage($"[CH.{channel + 1}] NFC Timeout!");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.NFC;
                _overalResult[channel].NFC.State = TestStates.Failed;
                _overalResult[channel].NFC.Value = "Timeout";
                _overalResult[channel].NFC.Result = "Timeout";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].NFC);
                NextTestStep(channel);
                _tickStepInterval[channel].Reset();
                _retryCount[channel] = 0;
            }
            else
            {
                if (rxCanID == GSystem.ProductSettings.NFC_ResID)
                {
                    if ((receivedEvent.tagData.canRxOkMsg.data[3] & 0x30) != 0x00)
                    {
                        GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                        // Full proof 닫기
                        OnShowFullProofMessage(channel, "", false);
                        // NFC 입력
                        GSystem.NFC_State[channel] = 1;
                        GSystem.Logger.Info ($"[CH.{channel + 1}] NFC State [ {receivedEvent.tagData.canRxOkMsg.data[3]:X02} ]");
                        GSystem.Logger.Info ($"[CH.{channel + 1}] NFC step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                        GSystem.TraceMessage($"[CH.{channel + 1}] NFC step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                        GSystem.Logger.Info ($"[CH.{channel + 1}] NFC complete");
                        GSystem.TraceMessage($"[CH.{channel + 1}] NFC complete");
                        // 결과 판정
                        TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.NFC;
                        _overalResult[channel].NFC.State = TestStates.Pass;
                        _overalResult[channel].NFC.Value = $"0x{receivedEvent.tagData.canRxOkMsg.data[3]:X02}";
                        _overalResult[channel].NFC.Result = $"{testSpec.MaxValue}";
                        // 동작 상태 표시
                        OnTestStepProgressChanged(channel, _overalResult[channel].NFC);
                        NextTestStep(channel);
                        _tickStepInterval[channel].Reset();
                    }
                    else
                    {
                        if (_tickStepTimeout[channel].MoreThan(5000))
                        {
                            // 에러 처리

                            // Full proof 닫기
                            OnShowFullProofMessage(channel, "", false);

                            GSystem.NFC_State[channel] = 0;
                            GSystem.Logger.Info ($"[CH.{channel + 1}] NFC State [ {receivedEvent.tagData.canRxOkMsg.data[3]:X02} ]");
                            GSystem.Logger.Info ($"[CH.{channel + 1}] NFC step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                            GSystem.TraceMessage($"[CH.{channel + 1}] NFC step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                            GSystem.Logger.Info ($"[CH.{channel + 1}] NFC Timeout!");
                            GSystem.TraceMessage($"[CH.{channel + 1}] NFC Timeout!");
                            // 결과 판정
                            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.NFC;
                            _overalResult[channel].NFC.State = TestStates.Failed;
                            _overalResult[channel].NFC.Value = $"0x{receivedEvent.tagData.canRxOkMsg.data[3]:X02}";
                            _overalResult[channel].NFC.Result = $"0";
                            // 동작 상태 표시
                            OnTestStepProgressChanged(channel, _overalResult[channel].NFC);
                            NextTestStep(channel);
                            _tickStepInterval[channel].Reset();
                            _retryCount[channel] = 0;
                        }
                    }
                }
            }
        }
        // NFC 상승
        private void NFCTouchTestStep_MotionNFC_UpStart(int channel)
        {
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Motion NFC Up]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Motion NFC Up]");
            // NFC 상승
            GSystem.MiPLC.SetNFCZUpStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionNFC_UpWait(int channel)
        {
            if (GSystem.MiPLC.GetNFCZUpStart(channel))
            {
                if (!GSystem.MiPLC.GetNFCZUpComplete(channel))
                    return;
                GSystem.MiPLC.SetNFCZUpStart(channel, false);
            }
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion NFC Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion NFC Up step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion NFC Up Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion NFC Up Complete");
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_XcpPrepareSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.SecurityBit.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.DTCEraseSend);
                return;
            }
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_XcpPrepareWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_XcpConnectSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [XCP Connect]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [XCP Connect]");
            Send_XCPConnect(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_XcpConnectWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] XCP Connect Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] XCP Connect Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.XcpConnectSend);
                    return;
                }
                else
                {
                    _isCancel[channel] = true;
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.XcpResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
                // XCP의 0번 바이트 == 0xFF면 정상
                GSystem.Logger.Info ($"[CH.{channel + 1}] XCP Connect step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] XCP Connect step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] XCP Connect complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] XCP Connect complete");
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_SecuritySetMtaSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.SecurityBit.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.XcpDisconnectSend);
                return;
            }
            _tickStepTimeout[channel].Reset();
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Serurity]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Serurity]");
            // 측정 상태 표시
            _overalResult[channel].SecurityBit.State = TestStates.Running;
            _overalResult[channel].SecurityBit.Value = "";
            _overalResult[channel].SecurityBit.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].SecurityBit);
            // SET_MTA
            uint xcpAddress = Convert.ToUInt32(GSystem.ProductSettings.XCPAddress.SecurityBit.Address, 16);
            Send_Security(channel, xcpAddress, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_SecuritySetMtaWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Security SetMTA Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Security SetMTA Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SecuritySetMtaSend);
                    return;
                }
                else
                {
                    _isCancel[channel] = true;
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.XcpResID)
                return;
            // XCP의 0번 바이트 == 0xFF면 정상
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                // Security의 Upload 크기는 1 바이트
                Send_Upload(channel, 1, true);
                NextTestStep(channel);
                _tickStepTimeout[channel].Reset();
            }
        }
        private void NFCTouchTestStep_SecurityUploadWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Security Upload Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Security Upload Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SecuritySetMtaSend);
                    return;
                }
                else
                {
                    _isCancel[channel] = true;
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.XcpResID)
                return;
            // XCP의 0번 바이트 == 0xFF면 정상
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0xFF)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                // Security 결과
                _securityBit[channel] = receivedEvent.tagData.canRxOkMsg.data[1];
                GSystem.Logger.Info ($"[CH.{channel + 1}] Security Bit [ {_securityBit[channel]} ]({GSystem.ProductSettings.XCPAddress.SecurityBit.Address})");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Serurity step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Serurity step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Serurity complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Serurity complete");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.SecurityBit;
                if (_securityBit[channel] == testSpec.MaxValue)
                {
                    _overalResult[channel].SecurityBit.State = TestStates.Pass;
                    _overalResult[channel].SecurityBit.Result = $"{testSpec.MaxValue}";
                }
                else
                {
                    _overalResult[channel].SecurityBit.State = TestStates.Failed;
                    _overalResult[channel].SecurityBit.Result = "0";
                }
                _overalResult[channel].SecurityBit.Value = $"{_securityBit[channel]}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].SecurityBit);
                NextTestStep(channel);
                SetTestStep(channel, NFCTouchTestStep.XcpDisconnectSend);
            }
            else
            {
                //
                // TODO: Security 수신 데이터[0]가 0xFF가 아니면 에러 처리
                //
            }
        }
        private void NFCTouchTestStep_XcpDisconnectSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [XCP Disconnect]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [XCP Disconnect]");
            Send_XCPDisconnect(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_XcpDisconnectWait(int channel)
        {
            //
            // XCP Disconnect는 응답이 없음
            //
            GSystem.Logger.Info ($"[CH.{channel + 1}] XCP Disconnect step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] XCP Disconnect step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] XCP Disconnect complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] XCP Disconnect complete");
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_DTCEraseSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.DTC_Erase.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.HWVersionSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [DTC Erase]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [DTC Erase]");
            // 측정 상태 표시
            _overalResult[channel].DTC_Erase.State = TestStates.Running;
            _overalResult[channel].DTC_Erase.Value = "";
            _overalResult[channel].DTC_Erase.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].DTC_Erase);
            // 명령 전송
            Send_DTC_Erase(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_DTCEraseWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] DTC Erase Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] DTC Erase Timeout! Retry: [ {_retryCount[channel]} ]");
                    Send_DTC_Erase(channel, true);
                    _tickStepTimeout[channel].Reset();
                    return;
                }
                else
                {
                    // timeout
                    GSystem.Logger.Info ($"[CH.{channel + 1}] DTC Erase Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] DTC Erase Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].DTC_Erase.State = TestStates.Failed;
                    _overalResult[channel].DTC_Erase.Value = "";
                    _overalResult[channel].DTC_Erase.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].DTC_Erase);
                    NextTestStep(channel);
                    _tickStepInterval[channel].Reset();
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x01)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.Logger.Info ($"[CH.{channel + 1}] DTC Erase [ {receivedEvent.tagData.canRxOkMsg.data[1]:X02} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] DTC Erase step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] DTC Erase step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] DTC Erase complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] DTC Erase complete");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.DTC_Erase;
                int readDtc = receivedEvent.tagData.canRxOkMsg.data[1];
                int specDtc = Convert.ToInt32(testSpec.MaxString, 16);
                if (readDtc == specDtc)
                {
                    // pass
                    _overalResult[channel].DTC_Erase.State = TestStates.Pass;
                }
                else
                {
                    // failed
                    _overalResult[channel].DTC_Erase.State = TestStates.Failed;
                }
                _overalResult[channel].DTC_Erase.Value = $"{readDtc:X02}";
                _overalResult[channel].DTC_Erase.Result = $"{readDtc:X02}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].DTC_Erase);
                NextTestStep(channel);
                _retryCount[channel] = 0;
                _tickStepInterval[channel].Reset();
            }
        }
        private void NFCTouchTestStep_HWVersionSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.HW_Version.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.SWVersionSend);
                return;
            }
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            _tickStepTimeout[channel].Reset();
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [HW Version Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [HW Version Read]");
            // 측정 상태 표시
            _overalResult[channel].HW_Version.State = TestStates.Running;
            _overalResult[channel].HW_Version.Value = "";
            _overalResult[channel].HW_Version.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].HW_Version);
            // 명령 전송
            Send_HWVersion(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_HWVersionWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] HW Version Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] HW Version Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    Send_HWVersion(channel, true);
                    _tickStepTimeout[channel].Reset();
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] HW Version Read Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] HW Version Read Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].HW_Version.State = TestStates.Failed;
                    _overalResult[channel].HW_Version.Value = "";
                    _overalResult[channel].HW_Version.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].HW_Version);
                    NextTestStep(channel);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x62 &&
                receivedEvent.tagData.canRxOkMsg.data[2] == 0xF1 &&
                receivedEvent.tagData.canRxOkMsg.data[3] == 0x93)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                byte[] hwVersionBytes = new byte[3];
                hwVersionBytes[0] = receivedEvent.tagData.canRxOkMsg.data[4];
                hwVersionBytes[1] = receivedEvent.tagData.canRxOkMsg.data[5];
                hwVersionBytes[2] = receivedEvent.tagData.canRxOkMsg.data[6];
                string hwVersion = System.Text.Encoding.UTF8.GetString(hwVersionBytes, 0, hwVersionBytes.Length);
                GSystem.Logger.Info ($"[CH.{channel + 1}] HW Version [ {hwVersion} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] HW Version Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] HW Version Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] HW Version Read complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] HW Version Read complete");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.HW_Version;
                if (hwVersion == testSpec.MaxString)
                    _overalResult[channel].HW_Version.State = TestStates.Pass;
                else
                    _overalResult[channel].HW_Version.State = TestStates.Failed;
                _overalResult[channel].HW_Version.Value = $"{hwVersion}";
                _overalResult[channel].HW_Version.Result = $"{hwVersion}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].HW_Version);
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_SWVersionSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.SW_Version.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.PartNumberSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [SW Version Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [SW Version Read]");
            // 측정 상태 표시
            _overalResult[channel].SW_Version.State = TestStates.Running;
            _overalResult[channel].SW_Version.Value = "";
            _overalResult[channel].SW_Version.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].HW_Version);
            // 명령 전송
            Send_SWVersion(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_SWVersionWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] SW Version Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] SW Version Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SWVersionSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] SW Version Read Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] SW Version Read Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].HW_Version.State = TestStates.Failed;
                    _overalResult[channel].HW_Version.Value = "";
                    _overalResult[channel].HW_Version.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].HW_Version);
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x62 &&
                receivedEvent.tagData.canRxOkMsg.data[2] == 0xF1 &&
                receivedEvent.tagData.canRxOkMsg.data[3] == 0xB1)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                byte[] swVersionBytes = new byte[4];
                swVersionBytes[0] = receivedEvent.tagData.canRxOkMsg.data[4];
                swVersionBytes[1] = receivedEvent.tagData.canRxOkMsg.data[5];
                swVersionBytes[2] = receivedEvent.tagData.canRxOkMsg.data[6];
                swVersionBytes[3] = receivedEvent.tagData.canRxOkMsg.data[7];
                string swVersion = System.Text.Encoding.UTF8.GetString(swVersionBytes, 0, swVersionBytes.Length);
                GSystem.Logger.Info ($"[CH.{channel + 1}] SW Version [ {swVersion} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] SW Version Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] SW Version Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] SW Version Read complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] SW Version Read complete");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.SW_Version;
                if (swVersion == testSpec.MaxString)
                    _overalResult[channel].SW_Version.State = TestStates.Pass;
                else
                    _overalResult[channel].SW_Version.State = TestStates.Failed;
                _overalResult[channel].SW_Version.Value = $"{swVersion}";
                _overalResult[channel].SW_Version.Result = $"{swVersion}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].SW_Version);
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_PartNumberSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.PartNumber.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.BootloaderSend);
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
            // 명령 전송
            Send_PartNumber(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_PartNumberWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Part Number Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Part Number Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.PartNumberSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Part Number Read Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Part Number Read Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].PartNumber.State = TestStates.Failed;
                    _overalResult[channel].PartNumber.Value = "";
                    _overalResult[channel].PartNumber.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].PartNumber);
                    SetTestStep(channel, NFCTouchTestStep.BootloaderSend);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[2] == 0x62 &&
                receivedEvent.tagData.canRxOkMsg.data[3] == 0xF1 &&
                receivedEvent.tagData.canRxOkMsg.data[4] == 0x87)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                _partNumberBytes[channel, 0] = receivedEvent.tagData.canRxOkMsg.data[5];
                _partNumberBytes[channel, 1] = receivedEvent.tagData.canRxOkMsg.data[6];
                _partNumberBytes[channel, 2] = receivedEvent.tagData.canRxOkMsg.data[7];
                Send_PartNumberFlow(channel, true);
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x21)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                _partNumberBytes[channel, 3] = receivedEvent.tagData.canRxOkMsg.data[1];
                _partNumberBytes[channel, 4] = receivedEvent.tagData.canRxOkMsg.data[2];
                _partNumberBytes[channel, 5] = receivedEvent.tagData.canRxOkMsg.data[3];
                _partNumberBytes[channel, 6] = receivedEvent.tagData.canRxOkMsg.data[4];
                _partNumberBytes[channel, 7] = receivedEvent.tagData.canRxOkMsg.data[5];
                _partNumberBytes[channel, 8] = receivedEvent.tagData.canRxOkMsg.data[6];
                _partNumberBytes[channel, 9] = receivedEvent.tagData.canRxOkMsg.data[7];
                byte[] partNumberBytes = new byte[10];
                int srcOffset = channel * _partNumberBytes.GetLength(channel);
                Buffer.BlockCopy(_partNumberBytes, srcOffset, partNumberBytes, 0, partNumberBytes.Length);
                string partNumber = System.Text.Encoding.UTF8.GetString(partNumberBytes, 0, partNumberBytes.Length);
                GSystem.Logger.Info ($"[CH.{channel + 1}] Part Number [ {partNumber} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Part Number Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Part Number Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Part Number Read complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Part Number Read complete");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.PartNumber;
                if (partNumber == testSpec.MaxString)
                    _overalResult[channel].PartNumber.State = TestStates.Pass;
                else
                    _overalResult[channel].PartNumber.State = TestStates.Failed;
                _overalResult[channel].PartNumber.Value = $"{partNumber}";
                _overalResult[channel].PartNumber.Result = $"{partNumber}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].PartNumber);
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_BootloaderSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.Bootloader.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.RxsWinSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Bootloader]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Bootloader]");
            // 측정 상태 표시
            _overalResult[channel].Bootloader.State = TestStates.Running;
            _overalResult[channel].Bootloader.Value = "";
            _overalResult[channel].Bootloader.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].Bootloader);
            // 명령 전송
            Send_Bootloader(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_BootloaderWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(3000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Bootloader Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Bootloader Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.BootloaderSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Bootloader Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Bootloader Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].Bootloader.State = TestStates.Failed;
                    _overalResult[channel].Bootloader.Value = "";
                    _overalResult[channel].Bootloader.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].Bootloader);
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x7F &&
                receivedEvent.tagData.canRxOkMsg.data[2] == 0x10 &&
                receivedEvent.tagData.canRxOkMsg.data[3] == 0x78)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                _bootResID[channel] = receivedEvent.tagData.canRxOkMsg.canId;
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x50 && receivedEvent.tagData.canRxOkMsg.data[2] == 0x02)
            {
                uint bootReqID = GSystem.ProductSettings.CanReqID;
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.Logger.Info ($"[CH.{channel + 1}] Bootloader [ Req_ID = {bootReqID:X3}h, Res_ID = {rxCanID:X3}h ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Bootloader step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Bootloader step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Bootloader complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Bootloader complete");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.Bootloader;
                if (rxCanID == Convert.ToUInt32(testSpec.MaxString, 16))
                    _overalResult[channel].Bootloader.State = TestStates.Pass;
                else
                    _overalResult[channel].Bootloader.State = TestStates.Failed;
                _overalResult[channel].Bootloader.Value = $"{rxCanID:X3}";
                _overalResult[channel].Bootloader.Result = $"{rxCanID:X3}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].Bootloader);
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_BootDefaultSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Default Session]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Default Session]");
            // Default session으로 변경
            Send_DefaultSession(channel, true);
            // 아래 부분은 실제 타이밍에 맞춰 조정할 필요가 있음
            //Thread.Sleep(5);
            //Send_DefaultSession(channel, GSystem.ProductSettings.CanReqID, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_BootDefaultWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Default Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Default Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.BootDefaultSend);
                    return;
                }
                else
                {
                    _isCancel[channel] = true;
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] != 0x50 || receivedEvent.tagData.canRxOkMsg.data[2] != 0x01)
                return;
            GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
            // Default Session으로 변경 완료
            GSystem.Logger.Info ($"[CH.{channel + 1}] Default Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Default Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Default Session complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Default Session complete");
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_BootWakeUpWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(10000))
            {
                SetTestStep(channel, NFCTouchTestStep.BootExtendedSend);
                return;
            }
            if (rxCanID != GSystem.ProductSettings.NM_ResID)
                return;
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_BootExtendedSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Extended Session]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Extended Session]");
            // Extended session으로 변경
            Send_ExtendedSession(channel, true);
            Thread.Sleep(5);
            Send_ExtendedSession(channel);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_BootExtendedWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Boot Extended Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Boot Extended Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.BootExtendedSend);
                    return;
                }
                else
                {
                    _isCancel[channel] = true;
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] != 0x50 || receivedEvent.tagData.canRxOkMsg.data[2] != 0x03)
                return;
            GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
            GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            // Extend Session으로 변경 완료
            GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session complete");
            NextTestStep(channel);
            _retryCount[channel] = 0;
        }
        private void NFCTouchTestStep_RxsWinSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.RXSWIN.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.SerialNumPrepareSend);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [RXSWIN]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [RXSWIN]");
            // 측정 상태 표시
            _overalResult[channel].RXSWIN.State = TestStates.Running;
            _overalResult[channel].RXSWIN.Value = "";
            _overalResult[channel].RXSWIN.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].RXSWIN);
            // 명령 전송
            Send_RXSWin(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_RxsWinWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(2000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] RXSWIN Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] RXSWIN Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.RxsWinSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] RXSWIN Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] RXSWIN Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].RXSWIN.State = TestStates.Failed;
                    _overalResult[channel].RXSWIN.Value = "";
                    _overalResult[channel].RXSWIN.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].RXSWIN);
                    SetTestStep(channel, NFCTouchTestStep.SupplierCodeSend);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[2] == 0x62 &&
                receivedEvent.tagData.canRxOkMsg.data[3] == 0xF1 &&
                receivedEvent.tagData.canRxOkMsg.data[4] == 0xEF)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                _rxsWinByteCount[channel] = (int)receivedEvent.tagData.canRxOkMsg.data[1]; // RXSWIN 바이트 수
                int dstOffset = channel * _rxsWinBytes.GetLength(channel);
                Buffer.BlockCopy(receivedEvent.tagData.canRxOkMsg.data, 2, _rxsWinBytes, dstOffset, 6);
                _rxsWinRecvCount[channel] = 6;
                Send_RXSWinFlow(channel, 10, true); // 10ms
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[0] >= 0x20 && receivedEvent.tagData.canRxOkMsg.data[0] <= 0x2F)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                int dstOffset = channel * _rxsWinBytes.GetLength(channel) + _rxsWinRecvCount[channel];
                Buffer.BlockCopy(receivedEvent.tagData.canRxOkMsg.data, 1, _rxsWinBytes, dstOffset, 7);
                _rxsWinRecvCount[channel] += 7;

                if (_rxsWinRecvCount[channel] < _rxsWinByteCount[channel])
                {
                    // 계속 수신
                }
                else
                {
                    // RXSWIN 데이터 출력
                    uint id = (uint)(_rxsWinBytes[channel, 3] << 8 | _rxsWinBytes[channel, 4]);
                    StringBuilder logString = new StringBuilder();
                    logString.Append($"RXSWIN: ID [{id:X3}h], Receive bytes [{_rxsWinByteCount[channel]} bytes] ");
                    GSystem.Logger.Info (logString);

                    StringBuilder stringRxsWin = new StringBuilder();
                    OnRxsWinDataChanged(channel, string.Empty);

                    byte[] byteRxswin = new byte[_rxsWinByteCount[channel]];
                    byte[] byteStep = new byte[_rxsWinByteCount[channel]];
                    int startIndex = 6;
                    int byteCount = 0;
                    int stepCount = 0;
                    logString.Clear();
                    stringRxsWin.Clear();
                    for (int i = 0; i < _rxsWinByteCount[channel] - startIndex; i++)
                    {
                        if (_rxsWinBytes[channel, startIndex + i] < 0x20)
                        {
                            // 전체를 한 줄에 출력하는 바이트 배열
                            byteRxswin[i] = 0x20;
                            // 스텝별 출력하는 바이트 배열
                            byteStep[stepCount++] = 0x20;
                            // 스텝 한 줄 출력
                            logString.Clear();
                            logString.Append(System.Text.Encoding.UTF8.GetString(byteStep, 0, stepCount));
                            GSystem.Logger.Info (logString.ToString());
                            stringRxsWin.Append(logString);
                            stringRxsWin.Append(Environment.NewLine);
                            stepCount = 0;
                        }
                        else
                        {
                            // 전체를 한 줄에 출력하는 바이트 배열
                            byteRxswin[i] = _rxsWinBytes[channel, startIndex + i];
                            // 스텝별 출력하는 바이트 배열
                            byteStep[stepCount++] = _rxsWinBytes[channel, startIndex + i];
                        }
                        byteCount++;
                    }
                    // for문에서 마지막 줄은 처리되지 않기 때문에 여기서 처리한다.
                    logString.Clear();
                    logString.Append(System.Text.Encoding.UTF8.GetString(byteStep, 0, stepCount));
                    GSystem.Logger.Info (logString.ToString());
                    stringRxsWin.Append(logString);
                    stringRxsWin.Append(Environment.NewLine);
                    // 검사 결과 저장용...전체를 한 줄로 출력한다.
                    string rxswinValue = System.Text.Encoding.UTF8.GetString(byteRxswin, 0, byteCount);
                    // 측정 결과 표시
                    OnRxsWinDataChanged(channel, stringRxsWin.ToString());
                    GSystem.Logger.Info ($"[CH.{channel + 1}] RXSWIN step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] RXSWIN step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] RXSWIN complete");
                    GSystem.TraceMessage($"[CH.{channel + 1}] RXSWIN complete");
                    // 결과 판정
                    TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.RXSWIN;
                    int rxswinDataSize = int.Parse(Regex.Replace(testSpec.MaxString, @"\D", ""));
                    if (_rxsWinByteCount[channel] == rxswinDataSize)
                        _overalResult[channel].RXSWIN.State = TestStates.Pass;
                    else
                        _overalResult[channel].RXSWIN.State = TestStates.Failed;
                    _overalResult[channel].RXSWIN.Value = $"{rxswinValue/*stringRxsWin*/}";
                    _overalResult[channel].RXSWIN.Result = $"{_rxsWinByteCount[channel]} bytes";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].RXSWIN);
                    // 다음으로 진행
                    NextTestStep(channel);
                    _retryCount[channel] = 0;
                }
            }
        }
        private void NFCTouchTestStep_SupplierCodeSend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.SupplierCode.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.OperCurrentStart);
                return;
            }
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Supplier Code Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Supplier Code Read]");
            // 측정 상태 표시
            _overalResult[channel].SupplierCode.State = TestStates.Running;
            _overalResult[channel].SupplierCode.Value = "";
            _overalResult[channel].SupplierCode.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].SupplierCode);
            // 명령 전송
            Send_SupplierCodeRead(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_SupplierCodeWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Supplier Code Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Supplier Code Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SupplierCodeSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Supplier Code Read Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Supplier Code Read Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].SupplierCode.State = TestStates.Failed;
                    _overalResult[channel].SupplierCode.Value = "";
                    _overalResult[channel].SupplierCode.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].SupplierCode);
                    SetTestStep(channel, NFCTouchTestStep.OperCurrentStart);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x62 &&
                receivedEvent.tagData.canRxOkMsg.data[2] == 0xF1 &&
                receivedEvent.tagData.canRxOkMsg.data[3] == 0xA1)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                byte[] supplierCodeBytes = new byte[4];
                supplierCodeBytes[0] = receivedEvent.tagData.canRxOkMsg.data[4];
                supplierCodeBytes[1] = receivedEvent.tagData.canRxOkMsg.data[5];
                supplierCodeBytes[2] = receivedEvent.tagData.canRxOkMsg.data[6];
                supplierCodeBytes[3] = receivedEvent.tagData.canRxOkMsg.data[7];
                string supplierCode = System.Text.Encoding.UTF8.GetString(supplierCodeBytes, 0, supplierCodeBytes.Length);
                GSystem.Logger.Info ($"[CH.{channel + 1}] Supplier Code : [ {supplierCode} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Supplier Code Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Supplier Code Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Supplier Code Read complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Supplier Code Read complete");
                // 결과 판정
                TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.SupplierCode;
                if (supplierCode == testSpec.MaxString)
                    _overalResult[channel].SupplierCode.State = TestStates.Pass;
                else
                    _overalResult[channel].SupplierCode.State = TestStates.Failed;
                _overalResult[channel].SupplierCode.Value = $"{supplierCode}";
                _overalResult[channel].SupplierCode.Result = $"{supplierCode}";
                // 동작 상태 표시
                OnTestStepProgressChanged(channel, _overalResult[channel].SupplierCode);
                NextTestStep(channel);
            }
        }
        private void NFCTouchTestStep_OperCurrentStart(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.OperationCurrent.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.PowerOff);
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
            NextTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_OperCurrentWait(int channel)
        {
            short operCurrent;
            if (channel == CH1)
                operCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch1_current_hi);
            else
                operCurrent = (short)(GSystem.DedicatedCTRL.Reg_03h_ch2_current_hi);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Operation Current : [ {operCurrent} mA ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Operation Current step time: [ {_tickStepElapse[channel].GetElapsedSeconds():F1} sec ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Operation Current step time: [ {_tickStepElapse[channel].GetElapsedSeconds():F1} sec ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Operation Current complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Operation Current complete");
            // 결과 판정
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.OperationCurrent;
            if (operCurrent < testSpec.MinValue || operCurrent > testSpec.MaxValue)
                _overalResult[channel].OperationCurrent.State = TestStates.Failed;
            else
                _overalResult[channel].OperationCurrent.State = TestStates.Pass;
            _overalResult[channel].OperationCurrent.Value = $"{operCurrent}";
            _overalResult[channel].OperationCurrent.Result = $"{operCurrent} mA";
            // 동작 상태 표시
            OnTestStepProgressChanged(channel, _overalResult[channel].OperationCurrent);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_SerialNumPrepareSend(int channel)
        {
            if (_tickStepInterval[channel].LessThan(1000))
                return;
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Extended Session]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Extended Session]");
            // Extended session으로 변경
            Send_ExtendedSession(channel, true);
            Thread.Sleep(5);
            Send_ExtendedSession(channel);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_SerialNumPrepareWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SerialNumPrepareSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].SerialNumber.State = TestStates.Failed;
                    _overalResult[channel].SerialNumber.Value = "";
                    _overalResult[channel].SerialNumber.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[1] != 0x50 || receivedEvent.tagData.canRxOkMsg.data[2] != 0x03)
                return;
            GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
            GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
            // Extend Session으로 변경 완료
            GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Extended Session complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Extended Session complete");
            // 측정 USE 판단
            if (GSystem.ProductSettings.TestItemSpecs.SerialNumber.Use)
            {
                // 옵션에 따라 WR/RO 동작을 다르게 한다.
                if (GSystem.ProductSettings.TestItemSpecs.SerialNumber.Option == 0)
                {
                    // WR
                    NextTestStep(channel);
                    _retryCount[channel] = 0;
                }
                else
                {
                    // RO
                    SetTestStep(channel, NFCTouchTestStep.SerialNumReadSend);
                }
            }
            else
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.PowerOff);
                return;
            }
        }
        private void NFCTouchTestStep_SerialNumSeedkeySend(int channel)
        {
            // 측정 USE 판단
            if (!GSystem.ProductSettings.TestItemSpecs.SerialNumber.Use)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.PowerOff);
                return;
            }
            // 전 검사 과정 중 FAILED이 있으면 Serial Write는 하지 않는다.
            TestStates overalResult = TestStates.Pass;
            foreach (var testResult in _testResultsList[channel])
            {
                if (testResult.State == TestStates.Failed)
                {
                    overalResult = testResult.State;
                    GSystem.TraceMessage($"{testResult.Name}: {testResult.State}");
                }
            }
            if (overalResult == TestStates.Failed)
            {
                // 다음 측정 항목으로 이동
                SetTestStep(channel, NFCTouchTestStep.PowerOff);
                return;
            }

            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Serial Number Request Seedkey]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Serial Number Request Seedkey]");
            // 측정 상태 표시
            _overalResult[channel].SerialNumber.State = TestStates.Running;
            _overalResult[channel].SerialNumber.Value = "";
            _overalResult[channel].SerialNumber.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
            // 명령 전송
            Send_RequestSeedkey(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_SerialNumSeedkeyWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Request Seedkey Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Request Seedkey Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SerialNumSeedkeySend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Request Seedkey Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Request Seedkey Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].SerialNumber.State = TestStates.Failed;
                    _overalResult[channel].SerialNumber.Value = "";
                    _overalResult[channel].SerialNumber.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x10 && receivedEvent.tagData.canRxOkMsg.data[1] == 0x0A &&
                receivedEvent.tagData.canRxOkMsg.data[2] == 0x67 && receivedEvent.tagData.canRxOkMsg.data[3] == 0x11)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.TraceMessage(GetRxEventString(receivedEvent).ToString());
                _receivedSeedkey[channel, 0] = receivedEvent.tagData.canRxOkMsg.data[4];
                _receivedSeedkey[channel, 1] = receivedEvent.tagData.canRxOkMsg.data[5];
                _receivedSeedkey[channel, 2] = receivedEvent.tagData.canRxOkMsg.data[6];
                _receivedSeedkey[channel, 3] = receivedEvent.tagData.canRxOkMsg.data[7];
                Send_RequestSeedkeyFlow(channel);
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x21)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                _receivedSeedkey[channel, 4] = receivedEvent.tagData.canRxOkMsg.data[1];
                _receivedSeedkey[channel, 5] = receivedEvent.tagData.canRxOkMsg.data[2];
                _receivedSeedkey[channel, 6] = receivedEvent.tagData.canRxOkMsg.data[3];
                _receivedSeedkey[channel, 7] = receivedEvent.tagData.canRxOkMsg.data[4];
                byte[] tempSeedkey = new byte[8];
                int srcOffset = channel * _receivedSeedkey.GetLength(channel);
                Buffer.BlockCopy(_receivedSeedkey, srcOffset, tempSeedkey, 0, 8);
                GSystem.Logger.Info ($"[CH.{channel + 1}] Received Seedkey : [ {BitConverter.ToString(tempSeedkey).Replace('-', ' ')} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Request Seedkey step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Request Seedkey step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Request Seedkey complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Request Seedkey complete");
                NextTestStep(channel);
            }
        }
        private void NFCTouchTestStep_SerialNumGenerateSeedkey(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Serial Number Generate Seedkey]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Serial Number Generate Seedkey]");
            byte[] receivedSeedkey = new byte[8];
            byte[] generatedSeedkey = new byte[8];
            int srcOffset = channel * _receivedSeedkey.GetLength(channel);
            Buffer.BlockCopy(_receivedSeedkey, srcOffset, receivedSeedkey, 0, 8);
            GenerateAdvancedSeedKey(ref receivedSeedkey, ref generatedSeedkey);
            int destIndex = channel * _generatedSeedkey.GetLength(channel);
            Buffer.BlockCopy(generatedSeedkey, 0, _generatedSeedkey, destIndex, 8);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Generated Seedkey: [ {BitConverter.ToString(generatedSeedkey).Replace('-', ' ')} ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Generate Seedkey step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Generate Seedkey step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Generate Seedkey complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Generate Seedkey complete");
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_SerialNumGeneratekeySend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Serial Number Generated Seedkey Send]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Serial Number Generated Seedkey Send]");
            byte[] seedkey = new byte[8];
            int srcOffset = channel * _generatedSeedkey.GetLength(channel);
            Buffer.BlockCopy(_generatedSeedkey, srcOffset, seedkey, 0, 8);
            Send_GeneratedSeedkey(channel, seedkey, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_SerialNumGeneratekeyWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Generated Seedkey Send Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Generated Seedkey Send Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SerialNumGeneratekeySend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Generated Seedkey Send Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Generated Seedkey Send Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].SerialNumber.State = TestStates.Failed;
                    _overalResult[channel].SerialNumber.Value = "";
                    _overalResult[channel].SerialNumber.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x30)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                byte[] seedkey = new byte[8];
                int srcOffset = channel * _generatedSeedkey.GetLength(channel);
                Buffer.BlockCopy(_generatedSeedkey, srcOffset, seedkey, 0, 8);
                Send_GeneratedSeedkey2(channel, seedkey, true);
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x67 && receivedEvent.tagData.canRxOkMsg.data[2] == 0x12)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Generated Seedkey Send step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Generated Seedkey Send step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Generated Seedkey Send complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Generated Seedkey Send complete");
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_SerialNumWriteSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Serial Number Write]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Serial Number Write]");
            //
            // 시리얼번호 생성 : LHD + XH010 + LOT + 1 + 0001 + 0000
            //
            // 일련번호 생성 규칙
            // - 일련번호는 임시(기존 +1)로 생성해서 사용하고 모든 검사 항목이 합격일 경우 최종 일련번호에 반영한다.
            // - 모든 검사가 합격일 경우 종합 일련번호에 반영하고, 불합격이 있는 경우 반영하지 않는다.(채널 일련번호는 유지)
            // - 불합격 채널의 경우 이전 일련번호를 유지하고 합격일 경우 다음 일련번호를 반영한다.
            // - 임시 번호는 타채널이 SerialNumWriteSend 이상이면 타채널 일련번호 + 1, 아니면 총 일련번호 + 1 이다.
            // - 일련번호 강제 변경 기능이 필요하다
            if (GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode1 &&
                GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode2 &&
                GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode3 &&
                GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode4 &&
                GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode5)
            {
                if (channel == GSystem.CH1)
                {
                    if (GetTestStep(GSystem.CH2) > NFCTouchTestStep.SerialNumWriteSend)
                        GSystem.TempSerialNumber[channel] = GSystem.TempSerialNumber[GSystem.CH2] + 1;
                    else
                        GSystem.TempSerialNumber[channel] = GSystem.ProductSettings.TestInfo.SerialNumTot + 1;
                }
                else
                {
                    if (GetTestStep(GSystem.CH1) > NFCTouchTestStep.SerialNumWriteSend)
                        GSystem.TempSerialNumber[channel] = GSystem.TempSerialNumber[GSystem.CH1] + 1;
                    else
                        GSystem.TempSerialNumber[channel] = GSystem.ProductSettings.TestInfo.SerialNumTot + 1;
                }
            }
            else
            {
                GSystem.TempSerialNumber[channel] = 0;
            }
            TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.SerialNumber;
            string specLR = testSpec.MaxString.Substring(0, 3); // LHD/RHD
            string specNo = testSpec.MaxString.Substring(3, 5); // XH010
            //_nextSerialNo = specLR + specNo + "LOT1" + sn.ToString("D04") + "0000";
            _nextSerialNo = $"{specLR}{specNo}LOT1{GSystem.TempSerialNumber[channel]:D04}0000";
            byte[] serialNumber = new byte[SerialNumberLength];
            serialNumber = System.Text.Encoding.UTF8.GetBytes(_nextSerialNo);
            int destOffset = channel * _serialNumberBytes.GetLength(channel);
            Buffer.BlockCopy(serialNumber, 0, _serialNumberBytes, destOffset, SerialNumberLength);
            Send_SerialNumberWrite(channel, serialNumber, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_SerialNumWriteWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Write Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Write Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SerialNumWriteSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Write Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Write Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].SerialNumber.State = TestStates.Failed;
                    _overalResult[channel].SerialNumber.Value = "";
                    _overalResult[channel].SerialNumber.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x30)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                XLDefine.XL_CANFD_DLC dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES;
                byte[] data = new byte[8];
                data[0] = 0x21;
                data[1] = _serialNumberBytes[channel, 3];
                data[2] = _serialNumberBytes[channel, 4];
                data[3] = _serialNumberBytes[channel, 5];
                data[4] = _serialNumberBytes[channel, 6];
                data[5] = _serialNumberBytes[channel, 7];
                data[6] = _serialNumberBytes[channel, 8];
                data[7] = _serialNumberBytes[channel, 9];
                WriteFrameStd(channel, GSystem.ProductSettings.CanReqID, dlc, data, true);
                Thread.Sleep(5);
                data[0] = 0x22;
                data[1] = _serialNumberBytes[channel, 10];
                data[2] = _serialNumberBytes[channel, 11];
                data[3] = _serialNumberBytes[channel, 12];
                data[4] = _serialNumberBytes[channel, 13];
                data[5] = _serialNumberBytes[channel, 14];
                data[6] = _serialNumberBytes[channel, 15];
                data[7] = _serialNumberBytes[channel, 16];
                WriteFrameStd(channel, GSystem.ProductSettings.CanReqID, dlc, data, true);
                Thread.Sleep(5);
                data[0] = 0x23;
                data[1] = _serialNumberBytes[channel, 17];
                data[2] = _serialNumberBytes[channel, 18];
                data[3] = _serialNumberBytes[channel, 19];
                data[4] = 0xAA;
                data[5] = 0xAA;
                data[6] = 0xAA;
                data[7] = 0xAA;
                WriteFrameStd(channel, GSystem.ProductSettings.CanReqID, dlc, data, true);
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[1] == 0x6E && receivedEvent.tagData.canRxOkMsg.data[2] == 0xF1 && receivedEvent.tagData.canRxOkMsg.data[3] == 0x8C)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                byte[] serialNumberBytes = new byte[SerialNumberLength];
                int srcOffset = channel * SerialNumberLength;
                Buffer.BlockCopy(_serialNumberBytes, srcOffset, serialNumberBytes, 0, SerialNumberLength);
                _overalResult[channel].SerialNumber.Value = System.Text.Encoding.UTF8.GetString(serialNumberBytes);
                GSystem.Logger.Info ($"Write Serial : [ {_overalResult[channel].SerialNumber.Value} ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Write step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Write step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Write complete");
                GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Write complete");
                NextTestStep(channel);
                _retryCount[channel] = 0;
            }
        }
        private void NFCTouchTestStep_SerialNumReadSend(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Serial Number Read]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Serial Number Read]");
            // 측정 상태 표시
            _overalResult[channel].SerialNumber.State = TestStates.Running;
            _overalResult[channel].SerialNumber.Value = "";
            _overalResult[channel].SerialNumber.Result = "측정 중";
            OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
            //_serialNumberBytes 초기화
            for (int i = 0; i < SerialNumberLength; i++)
                _serialNumberBytes[channel, i] = 0;
            // 명령 전송
            Send_SerialNumberRead(channel, true);
            NextTestStep(channel);
            _tickStepTimeout[channel].Reset();
        }
        private void NFCTouchTestStep_SerialNumReadWait(int channel, uint rxCanID, ref XLcanRxEvent receivedEvent)
        {
            if (_tickStepTimeout[channel].MoreThan(1000))
            {
                if (++_retryCount[channel] < MaxRetryCount)
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Read Timeout! Retry: [ {_retryCount[channel]} ]");
                    SetTestStep(channel, NFCTouchTestStep.SerialNumReadSend);
                    return;
                }
                else
                {
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Read Timeout Error!");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Read Timeout Error!");
                    // 측정 상태 표시
                    _overalResult[channel].SerialNumber.State = TestStates.Failed;
                    _overalResult[channel].SerialNumber.Value = "";
                    _overalResult[channel].SerialNumber.Result = "Timeout";
                    OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
                    SetTestStep(channel, NFCTouchTestStep.PowerOff);
                    return;
                }
            }
            if (rxCanID != GSystem.ProductSettings.CanResID)
                return;
            if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x10)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());
                // 수신 받은 시리얼번호 1~3번째 바이트
                _serialNumberRxCount[channel] = 0;
                _serialNumberBytes[channel, _serialNumberRxCount[channel]++] = receivedEvent.tagData.canRxOkMsg.data[5];
                _serialNumberBytes[channel, _serialNumberRxCount[channel]++] = receivedEvent.tagData.canRxOkMsg.data[6];
                _serialNumberBytes[channel, _serialNumberRxCount[channel]++] = receivedEvent.tagData.canRxOkMsg.data[7];
                // 흐름 송신
                XLDefine.XL_CANFD_DLC dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES;
                byte[] data = new byte[8];
                data[0] = 0x30;
                data[1] = 0x03;
                data[2] = 0x0A;
                data[3] = 0xAA;
                data[4] = 0xAA;
                data[5] = 0xAA;
                data[6] = 0xAA;
                data[7] = 0xAA;
                WriteFrameStd(channel, GSystem.ProductSettings.CanReqID, dlc, data, true);
            }
            else if (receivedEvent.tagData.canRxOkMsg.data[0] >= 0x21 && receivedEvent.tagData.canRxOkMsg.data[0] <= 0x23)
            {
                GSystem.Logger.Info (GetRxEventString(receivedEvent).ToString());

                if (receivedEvent.tagData.canRxOkMsg.data[0] == 0x23)
                {
                    _serialNumberBytes[channel, _serialNumberRxCount[channel]++] = receivedEvent.tagData.canRxOkMsg.data[1];
                    _serialNumberBytes[channel, _serialNumberRxCount[channel]++] = receivedEvent.tagData.canRxOkMsg.data[2];
                    _serialNumberBytes[channel, _serialNumberRxCount[channel]++] = receivedEvent.tagData.canRxOkMsg.data[3];
                    byte[] serialNumberBytes = new byte[SerialNumberLength];
                    int srcOffset = channel * SerialNumberLength;
                    Buffer.BlockCopy(_serialNumberBytes, srcOffset, serialNumberBytes, 0, SerialNumberLength);
                    string readSerialNumber = System.Text.Encoding.UTF8.GetString(serialNumberBytes).Replace("\0", "").Trim();
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Read Serial : [ {readSerialNumber} ({readSerialNumber.Length} bytes) ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Read Serial : [ {readSerialNumber} ({readSerialNumber.Length} bytes) ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Read step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
                    GSystem.Logger.Info ($"[CH.{channel + 1}] Serial Number Read complete");
                    GSystem.TraceMessage($"[CH.{channel + 1}] Serial Number Read complete");
                    //
                    // 결과 판정
                    //   1) 읽은 시리얼번호 바이트 수가 MinString.Length와 같은가? (20 바이트)
                    //   2) 읽은 시리얼번호 앞 3자리(LHD/RHD)가 품번과 일치하는가? (82657 -> LHD, 82667 -> RHD)
                    //   3) 읽은 시리얼번호 4~8번째 자리까지(5바이트)가 품번과 일치하는가? (XH010)
                    //
                    TestSpec testSpec = GSystem.ProductSettings.TestItemSpecs.SerialNumber;
                    if (readSerialNumber.Length == testSpec.MaxString.Length)
                    {
                        // OK
                        string specLR = testSpec.MaxString.Substring(0, 3); // LHD/RHD
                        string specNo = testSpec.MaxString.Substring(3, 5); // XH010
                        string readLR = readSerialNumber.Substring(0, 3);
                        string readNo = readSerialNumber.Substring(3, 5);
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
                    _overalResult[channel].SerialNumber.Value = readSerialNumber;
                    _overalResult[channel].SerialNumber.Result = $"{readSerialNumber}";
                    // 동작 상태 표시
                    OnTestStepProgressChanged(channel, _overalResult[channel].SerialNumber);
                    NextTestStep(channel);
                    _tickStepInterval[channel].Reset();
                }
                else
                {
                    // 마지막이 아니면 7바이트를 저장한다.
                    for (int i = 0; i < 7; i++)
                    {
                        _serialNumberBytes[channel, _serialNumberRxCount[channel]++] = receivedEvent.tagData.canRxOkMsg.data[i + 1];
                    }
                }
            }
        }
        private void NFCTouchTestStep_PowerOff(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step [Power Off]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step [Power Off]");
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
            GSystem.DedicatedCTRL.SetCommandActivePowerOn(channel, false);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_PowerOffWait(int channel)
        {
            // 완료 대기
            if (GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel) || GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                return;
            if (GSystem.DedicatedCTRL.GetCommandActivePowerOn(channel) || GSystem.DedicatedCTRL.GetCompleteActivePowerOn(channel))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Power Off step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Power Off Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Power Off Complete");
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_TestEndStart(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Test End]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Test End]");
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_TestEndWait(int channel)
        {
            // 완료 대기
            if (!GSystem.DedicatedCTRL.GetCommandTestInit(channel) || !GSystem.DedicatedCTRL.GetCompleteTestInit(channel))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test End step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test End step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test End Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test End Complete");
            GSystem.DedicatedCTRL.SetCommandTestInit(channel, false);
            StringBuilder sb = new StringBuilder();
            sb.Append($"[CH.{channel + 1}] Total Test Elapse: [ {GSystem.TimerTestTime[channel].GetElapsedSeconds():F1} sec ]");
            GSystem.Logger.Info (sb.ToString());
            GSystem.TraceMessage(sb.ToString());
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionUnloadingStart(int channel)
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
            GSystem.MiPLC.SetMoveLoadStart(channel, true);
            GSystem.MiPLC.SetUnloadingStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionUnloadingWait(int channel)
        {
            // 완료 대기
            if (!GSystem.MiPLC.GetMoveLoadComplete(channel) || !GSystem.MiPLC.GetUnloadingComplete(channel))
                return;
            GSystem.MiPLC.SetMoveLoadStart(channel, false);
            GSystem.MiPLC.SetUnloadingStart(channel, false);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Unloading step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Unloading step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Motion Unloading Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Motion Unloading Complete");
            GSystem.TimerTestTime[channel].Stop();
            NextTestStep(channel);
            _tickStepInterval[channel].Reset();
        }
        private void NFCTouchTestStep_Complete(int channel)
        {
            // Full proof 닫기 
            OnShowFullProofMessage(channel, "", false);

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
                    if (testResult.State == TestStates.Failed)
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
                    if (GSystem.ProductSettings.MasterSample.MasterType1 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode1)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType1 == "양품")
                                GSystem.MasterTestCh1[0] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh1[0] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSample.MasterType2 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode2)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType2 == "양품")
                                GSystem.MasterTestCh1[1] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh1[1] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSample.MasterType3 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode3)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType3 == "양품")
                                GSystem.MasterTestCh1[2] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh1[2] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSample.MasterType4 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode4)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType4 == "양품")
                                GSystem.MasterTestCh1[3] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh1[3] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSample.MasterType5 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode5)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType5 == "양품")
                                GSystem.MasterTestCh1[4] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh1[4] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                }
                else
                {
                    if (GSystem.ProductSettings.MasterSample.MasterType1 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode1)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType1 == "양품")
                                GSystem.MasterTestCh2[0] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh2[0] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSample.MasterType2 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode2)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType2 == "양품")
                                GSystem.MasterTestCh2[1] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh2[1] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSample.MasterType3 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode3)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType3 == "양품")
                                GSystem.MasterTestCh2[2] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh2[2] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSample.MasterType4 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode4)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType4 == "양품")
                                GSystem.MasterTestCh2[3] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh2[3] = (overalResult == TestStates.Failed) ? true : false;
                        }
                    }
                    if (GSystem.ProductSettings.MasterSample.MasterType5 != "")
                    {
                        if (GSystem.ProductBarcode[channel] == GSystem.ProductSettings.MasterSample.MasterBarcode5)
                        {
                            enableCount = false;
                            if (GSystem.ProductSettings.MasterSample.MasterType5 == "양품")
                                GSystem.MasterTestCh2[4] = (overalResult == TestStates.Pass) ? true : false;
                            else
                                GSystem.MasterTestCh2[4] = (overalResult == TestStates.Failed) ? true : false;
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

                // 검사 결과 파일 저장
                // DATA_ALL
                DateTime saveDate = DateTime.Now;
                //string suffix = "UFD";
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
                    }

                    string worker = (GSystem.AdminMode) ? "Manager" : "Operator";
                    sb.Clear();
                    sb.Append($"{channel + 1},{GSystem.TrayBarcode},{GSystem.ProductBarcode[channel]},{_testStartTime[channel]:hh:mm:ss},{ProductSettings.ProductInfo.CarType},{worker},");
                    foreach (var testResult in _testResultsList[channel])
                    {
                        string judge = string.Empty;
                        if (testResult.State > TestStates.Running)
                        {
                            judge = (testResult.State == TestStates.Pass) ? "OK" : "NG";
                        }
                        if (testResult.Name == GDefines.TEST_ITEM_NAME_STR[(int)TestItems.RXSWIN])
                            sb.Append($"{testResult.Name},{testResult.Value},{judge},");
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
                    // 라벨 출력
                    if (GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode1 &&
                        GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode2 &&
                        GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode3 &&
                        GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode4 &&
                        GSystem.ProductBarcode[channel] != GSystem.ProductSettings.MasterSample.MasterBarcode5)
                    {
                        // 마스터샘플이 아닌 경우만 일련번호를 증가시킨다
                        if (channel == GSystem.CH1)
                        {
                            // 임시 일련번호를 채널 일련번호에 반영한다
                            GSystem.ProductSettings.TestInfo.SerialNumCh1 = GSystem.TempSerialNumber[channel];
                            // 채널 일련번호가 총 일련번호보다 크면 총 일련번호에 채널 일련번호를 반영한다
                            if (GSystem.ProductSettings.TestInfo.SerialNumCh1 > GSystem.ProductSettings.TestInfo.SerialNumTot)
                                GSystem.ProductSettings.TestInfo.SerialNumTot = GSystem.ProductSettings.TestInfo.SerialNumCh1;
                        }
                        else
                        {
                            // 임시 일련번호를 채널 일련번호에 반영한다
                            GSystem.ProductSettings.TestInfo.SerialNumCh2 = GSystem.TempSerialNumber[channel];
                            // 채널 일련번호가 총 일련번호보다 크면 총 일련번호에 채널 일련번호를 반영한다
                            if (GSystem.ProductSettings.TestInfo.SerialNumCh2 > GSystem.ProductSettings.TestInfo.SerialNumTot)
                                GSystem.ProductSettings.TestInfo.SerialNumTot = GSystem.ProductSettings.TestInfo.SerialNumCh2;
                        }
                        GSystem.ProductSettings.Save(GSystem.ProductSettings.GetFileName(), GSystem.SystemData.GeneralSettings.ProductFolder);

                        if (GSystem.ProductSettings.ProductInfo.UseLabelPrint)
                        {
                            //string hwVersion = _overalResult[channel].HW_Version.Value.Insert(1, ".");
                            //string swVersion = _overalResult[channel].SW_Version.Value;
                            //string lotNumber = GSystem.GetLotNumber();
                            //string serialNumber = GSystem.ProductSettings.GetCurrentSerialNumber().ToString("D04");
                            //string zpl = GSystem.BuildProductLabelZpl(hwVersion, swVersion, lotNumber, serialNumber, "", "", "", "", "");
                            //string printerName = "ZDesigner ZD421-203dpi ZPL";
                            //GSystem.SendRawToPrinter(printerName, zpl);


                            string hwVersion = $"HW:{_overalResult[channel].HW_Version.Value.Insert(1, ".")}";
                            string swVersion = $"SW:{_overalResult[channel].SW_Version.Value}";
                            string lotNumber = $"LOT NO:{GSystem.GetLotNumber()}";
                            string sn        = $"S/N:{GSystem.ProductSettings.GetCurrentSerialNumber():D04}";
                            string partNo    = GSystem.ProductSettings.ProductInfo.PartNo; //"82667-P8100";
                            string fccId     = GSystem.ProductSettings.LabelPrint.Payload.FCCID; //"FCC ID:2A93T-LQ2-DHS-NFC";
                            string icId      = GSystem.ProductSettings.LabelPrint.Payload.ICID; //"IC ID:30083-LQ2DHSNFC";
                            string company   = GSystem.ProductSettings.LabelPrint.Style.BrandText; //"INFAC ELECS";

                            string EtcsVendor = GSystem.ProductSettings.LabelPrint.Etcs.Vendor; //"SUR2";
                            string EtcsPartNo = GSystem.ProductSettings.LabelPrint.Etcs.PartNo; //"8266703200";
                            string EtcsSerial = GSystem.ProductSettings.LabelPrint.Etcs.Serial; //"";
                            string EtcsEo     = GSystem.ProductSettings.LabelPrint.Etcs.Eo; //"";
                            string EtcsTrace  = GSystem.ProductSettings.LabelPrint.Etcs.Trace; //"250807";
                            string EtcsA1     = GSystem.ProductSettings.LabelPrint.Etcs.A1; //"A000";
                            string EtcsM      = GSystem.ProductSettings.LabelPrint.Etcs.M; //"0";
                            string EtcsC      = GSystem.ProductSettings.LabelPrint.Etcs.C; //"001";

                            var payload = new LabelPayload
                            {
                                HW = hwVersion,
                                SW = swVersion,
                                LOT = lotNumber,
                                SN = sn,
                                PartNo = partNo,
                                FCCID = fccId,
                                ICID = icId,
                                Company = company,
                                DataMatrix = null
                            };

                            var etcs = new EtcsSettings
                            {
                                Vendor = EtcsVendor,
                                PartNo = EtcsPartNo,
                                Serial = EtcsSerial,
                                Eo = EtcsEo,
                                Trace = EtcsTrace,
                                A1 = EtcsA1,
                                M = EtcsM,
                                C = EtcsC
                            };

                            // 설정값(ProductSettings.LabelPrint.PrinterName) 우선 사용 + 신규 데이터 전달
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
            _isCancel[channel] = false;

            // 검사 완료
            GSystem.MiPLC.SetAutoTestComplete(channel, true);

            // 스레드 종료
            SetTestStep(channel, NFCTouchTestStep.Standby);
            _testStepThreadExit[channel] = true;

            // 바코드 입력창 표시
            GSystem.BarcodeResetAndPopUp?.Invoke(channel);
        }
        private void NFCTouchTestStep_MotionUnclampForeStart(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Clamp Cylinder Foreward]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Clamp Cylinder Foreward]");
            GSystem.MiPLC.SetUnclampForeStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionUnclampForeWait(int channel)
        {
            if (!GSystem.MiPLC.GetUnclampForeStart(channel) || !GSystem.MiPLC.GetUnclampForeComplete(channel))
                return;
            GSystem.MiPLC.SetUnclampForeStart(channel, false);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Clamp Cylinder Foreward step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Clamp Cylinder Foreward step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Clamp Cylinder Foreward Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Clamp Cylinder Foreward Complete");
            //NextTestStep(channel);
            _tickStepInterval[channel].Reset();

            // 스레드 종료
            SetTestStep(channel, NFCTouchTestStep.Standby);
            _testStepThreadExit[channel] = true;
        }
        private void NFCTouchTestStep_JigUnloadingCheck(int channel)
        {
            // 제품 감지가 안될때까지 대기. TODO: 타임아웃 처리
            if (GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.JigSensor) ||
                GSystem.MiPLC.GetState1(channel, (int)PLC_State1_Bit.ConnSensor))
                return;
            GSystem.Logger.Info ($"[CH.{channel + 1}] Product Remove Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Product Remove Complete");
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionUnclampBackStart(int channel)
        {
            _tickStepElapse[channel].Reset();
            GSystem.Logger.Info ($"[CH.{channel + 1}] Test Step: [Clamp Cylinder Backward]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Test Step: [Clamp Cylinder Backward]");
            GSystem.MiPLC.SetUnclampBackStart(channel, true);
            NextTestStep(channel);
        }
        private void NFCTouchTestStep_MotionUnclampBackWait(int channel)
        {
            if (!GSystem.MiPLC.GetUnclampBackStart(channel) || !GSystem.MiPLC.GetUnclampBackComplete(channel))
                return;
            GSystem.MiPLC.SetUnclampBackStart(channel, false);
            GSystem.Logger.Info ($"[CH.{channel + 1}] Clamp Cylinder Backward step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.TraceMessage($"[CH.{channel + 1}] Clamp Cylinder Backward step time: [ {_tickStepElapse[channel].GetElapsedMilliseconds()} ms ]");
            GSystem.Logger.Info ($"[CH.{channel + 1}] Clamp Cylinder Backward Complete");
            GSystem.TraceMessage($"[CH.{channel + 1}] Clamp Cylinder Backward Complete");

            // 스레드 종료
            SetTestStep(channel, NFCTouchTestStep.Standby);
            _testStepThreadExit[channel] = true;
        }
    }
}

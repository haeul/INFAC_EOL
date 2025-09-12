using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static vxlapi_NET.XLDefine;
using vxlapi_NET;

namespace DHSTesterXL
{
    public class PXLDriver
    {
        // -----------------------------------------------------------------------------------------------
        // CanXLDriver 정적 인스턴스
        // -----------------------------------------------------------------------------------------------
        private static PXLDriver _instance = null;

        // -----------------------------------------------------------------------------------------------
        // DLL Import for RX events
        // -----------------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential)]
        public struct VERSION_INFO
        {
            public UInt16 vendorID;
            public UInt16 moduleID;
            public ushort majorVersion;
            public ushort minorVersion;
            public ushort patchVersion;
        }
        [DllImport("HKMC_AdvancedSeedKey_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void vGetVersionInfo(ref VERSION_INFO pVersionInfo);

        // -----------------------------------------------------------------------------------------------
        // Global variables
        // -----------------------------------------------------------------------------------------------
        public const int CH1 = 0;
        public const int CH2 = 1;
        public const int ChannelCount = 2;

        // Driver access through XLDriver (wrapper)
        public XLDriver _xlDriver = new XLDriver();
        private string _appName = "DHSTesterXL";

        public XLDriver Driver { get { return _xlDriver; } }
        public string AppName { get { return _appName; } }

        // Driver configuration
        public XLClass.xl_driver_config _driverConfig = new XLClass.xl_driver_config();

        // Variables required by XLDriver
        private XL_HardwareType[] _hwType = new XL_HardwareType[ChannelCount] {
            XL_HardwareType.XL_HWTYPE_NONE,
            XL_HardwareType.XL_HWTYPE_NONE
        };
        public uint[]   _hwIndex        = new uint   [ChannelCount] { 0, 0 };
        public uint[]   _hwChannel      = new uint   [ChannelCount] { 0, 0 };
        public UInt64[] _accessMask     = new UInt64 [ChannelCount] { 0, 0 };
        public UInt64[] _permissionMask = new UInt64 [ChannelCount] { 0, 0 };
        public UInt64[] _channelMask    = new UInt64 [ChannelCount] { 0, 0 };
        public int[]    _channelIndex   = new int    [ChannelCount] { 0, 0 };

        public uint     _canFdModeNoIso = 0; // Global CAN FD ISO (default) / no ISO mode flag

        public bool IsLoaded { get; set; } = false;


        public PXLDriver()
        {
        
        }

        public static PXLDriver GetInstance()
        {
            if (_instance == null)
                _instance = new PXLDriver();
            return _instance;
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Error message/exit in case of a functional call does not return XL_SUCCESS
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        public int PrintFunctionError()
        {
            GSystem.Logger.Error("Function call failed!");
            GSystem.TraceMessage("Function call failed!");
            return -1;
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Error message if channel assignment is not valid.
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private void PrintAssignErrorAndPopupHwConf()
        {
            GSystem.Logger.Error("Please check application settings of \"" + _appName + " CAN1/CAN2\",\nassign it to available hardware channels and try again.");
            GSystem.TraceMessage("Please check application settings of \"" + _appName + " CAN1/CAN2\",\nassign it to available hardware channels and try again.");
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Displays the Vector Hardware Configuration.
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private void PrintConfig(int hwChannels)
        {
            GSystem.Logger.Info ("APPLICATION CONFIGURATION");
            GSystem.TraceMessage("APPLICATION CONFIGURATION");

            for (int channelIndex = 0; channelIndex < hwChannels; channelIndex++)
            {
                GSystem.Logger.Info ("-------------------------------------------------------------------");
                GSystem.Logger.Info ("Configured Hardware Channel : " + _driverConfig.channel[channelIndex].name.Replace("\0", string.Empty) + ", " + " Channel Mask [" + _driverConfig.channel[channelIndex].channelMask.ToString() + "]");
                GSystem.Logger.Info ("Hardware Driver Version     : " + _xlDriver.VersionToString(_driverConfig.channel[channelIndex].driverVersion));
                GSystem.Logger.Info ("Used Transceiver            : " + _driverConfig.channel[channelIndex].transceiverName);
                GSystem.TraceMessage("-------------------------------------------------------------------");
                GSystem.TraceMessage("Configured Hardware Channel : " + _driverConfig.channel[channelIndex].name.Replace("\0", string.Empty) + ", " + " Channel Mask [" + _driverConfig.channel[channelIndex].channelMask.ToString() + "]");
                GSystem.TraceMessage("Hardware Driver Version     : " + _xlDriver.VersionToString(_driverConfig.channel[channelIndex].driverVersion));
                GSystem.TraceMessage("Used Transceiver            : " + _driverConfig.channel[channelIndex].transceiverName);
            }
            GSystem.Logger.Info ("-------------------------------------------------------------------");
            GSystem.TraceMessage("-------------------------------------------------------------------");
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieve the application channel assignment and test if this channel can be opened
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private bool GetAppChannelAndTestIsOk(uint appChIdx, ref UInt64 chMask, ref int chIdx)
        {
            XL_Status status = _xlDriver.XL_GetApplConfig(_appName, appChIdx, ref _hwType[appChIdx], ref _hwIndex[appChIdx], ref _hwChannel[appChIdx], XL_BusTypes.XL_BUS_TYPE_CAN);
            if (status != XL_Status.XL_SUCCESS)
            {
                GSystem.Logger.Error($"XL_GetApplConfig      : {status}");
                GSystem.TraceMessage($"XL_GetApplConfig      : {status}");
                PrintFunctionError();
            }

            chMask = _xlDriver.XL_GetChannelMask(_hwType[appChIdx], (int)_hwIndex[appChIdx], (int)_hwChannel[appChIdx]);
            chIdx = _xlDriver.XL_GetChannelIndex(_hwType[appChIdx], (int)_hwIndex[appChIdx], (int)_hwChannel[appChIdx]);
            if (chIdx < 0 || chIdx >= _driverConfig.channelCount)
            {
                // the (hwType, hwIndex, hwChannel) triplet stored in the application configuration does not refer to any available channel.
                return false;
            }

            if ((_driverConfig.channel[chIdx].channelBusCapabilities & XL_BusCapabilities.XL_BUS_ACTIVE_CAP_CAN) == 0)
            {
                // CAN is not available on this channel
                return false;
            }

            if (_canFdModeNoIso > 0)
            {
                if ((_driverConfig.channel[chIdx].channelCapabilities & XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_BOSCH_SUPPORT) == 0)
                {
                    string message = string.Format("{0} ({1}) does not support CAN FD NO-ISO", _driverConfig.channel[chIdx].name.TrimEnd(' ', '\0'),
                        _driverConfig.channel[chIdx].transceiverName.TrimEnd(' ', '\0'));
                    GSystem.Logger.Error(message);
                    GSystem.TraceMessage(message);
                    return false;
                }
            }
            else
            {
                if ((_driverConfig.channel[chIdx].channelCapabilities & XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_ISO_SUPPORT) == 0)
                {
                    string message = string.Format("{0} ({1}) does not support CAN FD ISO", _driverConfig.channel[chIdx].name.TrimEnd(' ', '\0'),
                        _driverConfig.channel[chIdx].transceiverName.TrimEnd(' ', '\0'));
                    GSystem.Logger.Error(message);
                    GSystem.TraceMessage(message);
                    return false;
                }
            }

            return true;
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Load Vector XL Driver 
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        public Task<XL_Status> LoadDriver()
        {
            XL_Status status;

            // .NET wrapper version
            GSystem.Logger.Info ($"vxlapi_NET            : {typeof(XLDriver).Assembly.GetName().Version}");
            GSystem.TraceMessage($"vxlapi_NET            : {typeof(XLDriver).Assembly.GetName().Version}");

            // Open XL Driver
            status = _xlDriver.XL_OpenDriver();
            GSystem.Logger.Info ($"Open Driver           : {status}");
            GSystem.TraceMessage($"Open Driver           : {status}");
            if (status != XL_Status.XL_SUCCESS)
            {
                PrintFunctionError();
                return Task.FromResult(status);
            }

            // Get XL Driver cnfiguration
            status = _xlDriver.XL_GetDriverConfig(ref _driverConfig);
            GSystem.Logger.Info ($"Get Driver Config     : {status}");
            GSystem.TraceMessage($"Get Driver Config     : {status}");
            if (status != XL_Status.XL_SUCCESS)
            {
                PrintFunctionError();
                return Task.FromResult(status);
            }

            // Convert the dll version number into a readable string
            GSystem.Logger.Info ($"DLL Version           : {_xlDriver.VersionToString(_driverConfig.dllVersion)}");
            GSystem.TraceMessage($"DLL Version           : {_xlDriver.VersionToString(_driverConfig.dllVersion)}");

            // Display channel count
            GSystem.Logger.Info ($"Channels found        : {_driverConfig.channelCount}");
            GSystem.TraceMessage($"Channels found        : {_driverConfig.channelCount}");

            int hwChannelCount = 0;

            // Display DLL Version and Channels
            for (int i = 0; i < _driverConfig.channelCount; i++)
            {
                GSystem.Logger.Info ($"    [{i}] {_driverConfig.channel[i].name.Replace("\0", string.Empty)}");
                GSystem.TraceMessage($"    [{i}] {_driverConfig.channel[i].name.Replace("\0", string.Empty)}");

                if ((_driverConfig.channel[i].channelCapabilities & XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_ISO_SUPPORT) == XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_ISO_SUPPORT)
                {
                    GSystem.Logger.Info ("    - CAN FD Support  : yes");
                    GSystem.TraceMessage("    - CAN FD Support  : yes");
                }
                else
                {
                    GSystem.Logger.Info ("    - CAN FD Support  : no");
                    GSystem.TraceMessage("    - CAN FD Support  : no");
                }

                GSystem.Logger.Info ("    - Channel Mask    : " + _driverConfig.channel[i].channelMask);
                GSystem.Logger.Info ("    - Transceiver Name: " + _driverConfig.channel[i].transceiverName.Replace("\0", string.Empty));
                GSystem.TraceMessage("    - Channel Mask    : " + _driverConfig.channel[i].channelMask);
                GSystem.TraceMessage("    - Transceiver Name: " + _driverConfig.channel[i].transceiverName.Replace("\0", string.Empty));

                if (_driverConfig.channel[i].hwType == XL_HardwareType.XL_HWTYPE_VN1610)
                    hwChannelCount++;
            }

            GSystem.Logger.Info ($"VN1610 Channels       : {hwChannelCount}");
            GSystem.TraceMessage($"VN1610 Channels       : {hwChannelCount}");

            for (uint ch = 0; ch < hwChannelCount; ch++)
            {
                // If the application name cannot be found in VCANCONF...
                if (_xlDriver.XL_GetApplConfig(_appName, ch, ref _hwType[ch], ref _hwIndex[ch], ref _hwChannel[ch], XL_BusTypes.XL_BUS_TYPE_CAN) != XL_Status.XL_SUCCESS)
                {
                    //...create the item with two CAN channels
                    _xlDriver.XL_SetApplConfig(_appName, ch, XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XL_BusTypes.XL_BUS_TYPE_CAN);
                    PrintAssignErrorAndPopupHwConf();
                    return Task.FromResult(status);
                }

                if (!GetAppChannelAndTestIsOk(ch, ref _channelMask[ch], ref _channelIndex[ch]))
                {
                    PrintAssignErrorAndPopupHwConf();
                    return Task.FromResult(XL_Status.XL_ERR_HW_NOT_READY);
                }

                _accessMask[ch] = _channelMask[ch];
                _permissionMask[ch] = _accessMask[ch];
            }

            PrintConfig(hwChannelCount);

            IsLoaded = true;

            return Task.FromResult(status);
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Unload Vector XL Driver 
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        public Task<XL_Status> UnloadDriver()
        {
            XL_Status status;

            IsLoaded = false;

            // Close driver
            status = _xlDriver.XL_CloseDriver();
            GSystem.Logger.Info ($"Close Driver : {status}");
            GSystem.TraceMessage($"Close Driver : {status}");
            return Task.FromResult(status);
        }

    }
}

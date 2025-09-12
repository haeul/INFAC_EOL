using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHSTesterXL
{
    public enum TestChannels
    {
        Ch1 = 0,
        Ch2,
        Ch3,
        Ch4,
        Count
    }
    public enum ProductTypeName
    {
        NFC_TOUCH_LHD,
        NFC_TOUCH_RHD,
        TOUCH_ONLY_LHD,
        TOUCH_ONLY_RHD,
        Count
    }

    public enum ProductTypes
    {
        NFC_TOUCH1_LHD = 0,
        NFC_TOUCH1_RHD,
        NFC_TOUCH2_LHD,
        NFC_TOUCH2_RHD,
        TOUCH_ONLY_LHD,
        TOUCH_ONLY_RHD,
        Count
    }

    public enum TestItems
    {
        Short_1_2 = 0,
        Short_1_3,
        Short_1_4,
        //Short_1_5,
        Short_1_6,
        Short_2_3,
        Short_2_4,
        //Short_2_5,
        Short_2_6,
        Short_3_4,
        //Short_3_5,
        Short_3_6,
        //Short_4_5,
        Short_4_6,
        //Short_5_6,
        SerialNumber,
        DarkCurrent,
        PLightTurnOn,
        PLightCurrent,
        PLightAmbient,
        Touch,
        Cancel,
        SecurityBit,
        NFC,
        DTC_Erase,
        HW_Version,
        SW_Version,
        PartNumber,
        Bootloader,
        RXSWIN,
        Manufacture,
        SupplierCode,
        OperationCurrent,
        Count
    }

    public enum XCP_Items
    {
        Touch_FastSelf = 0,
        Touch_SlowSelf,
        Touch_FastMutual,
        Touch_ComboRate,
        Touch_State,
        Cancel_FastSelf,
        Cancel_SlowSelf,
        Cancel_State,
        Security_Bit,
        Count
    }

    public enum CommTypes
    {
        CAN,
        CAN_FD,
        UART,
        Count
    }

    public enum CanClockFrequency
    {
        Freq_80_MHz,
        Freq_60_MHz,
        Freq_40_MHz,
        Freq_30_MHz,
        Freq_24_MHz,
        Freq_20_MHz,
        Count
    }

    public enum TestListColumns
    {
        No = 0,
        Use,
        Item,
        Min,
        Max,
        Result,
        Count
    }

    public enum EDedicatedCTRL_Registers
    {
        // FC03h
        Reg000_Ch1_Response, 
        Reg001_Ch1_Complete, 
        Reg002_Ch1_ConnType, 
        Reg003_Ch1_Short1_2,
        Reg004_Ch1_Short1_3, 
        Reg005_Ch1_Short1_4, 
        Reg006_Ch1_Short1_5, 
        Reg007_Ch1_Short1_6,
        Reg008_Ch1_Short2_3,
        Reg009_Ch1_Short2_4,
        Reg010_Ch1_Short2_5,
        Reg011_Ch1_Short2_6,
        Reg012_Ch1_Short3_4,
        Reg013_Ch1_Short3_5,
        Reg014_Ch1_Short3_6,
        Reg015_Ch1_Short4_5,
        Reg016_Ch1_Short4_6,
        Reg017_Ch1_Short5_6,
        Reg018_Ch1_LowCurrent,
        Reg019_Ch1_HighCurrent, 
        Reg020_Ch1_LightLux, 
        Reg021_Ch1_AvgTime, 
        Reg022_Spare, 
        Reg023_Spare,
        Reg024_Ch2_Response,
        Reg025_Ch2_Complete,
        Reg026_Ch2_ConnType,
        Reg027_Ch2_Short1_2,
        Reg028_Ch2_Short1_3,
        Reg029_Ch2_Short1_4,
        Reg030_Ch2_Short1_5,
        Reg031_Ch2_Short1_6,
        Reg032_Ch2_Short2_3,
        Reg033_Ch2_Short2_4,
        Reg034_Ch2_Short2_5,
        Reg035_Ch2_Short2_6,
        Reg036_Ch2_Short3_4,
        Reg037_Ch2_Short3_5,
        Reg038_Ch2_Short3_6,
        Reg039_Ch2_Short4_5,
        Reg040_Ch2_Short4_6,
        Reg041_Ch2_Short5_6,
        Reg042_Ch2_LowCurrent,
        Reg043_Ch2_HighCurrent,
        Reg044_Ch2_LightLux,
        Reg045_Ch2_AvgTime,
        Reg046_Spare,
        Reg047_ExtInput,
        Reg048_ExtOutput,
        Reg049_Alive,
        // FC10h
        Reg050_Ch1_Command,
        Reg051_Ch1_ConnType,
        Reg052_Ch1_ShortLogic,
        Reg053_Ch1_AvgTime,
        Reg054_Spare,
        Reg055_Ch2_Command,
        Reg056_Ch2_ConnType,
        Reg057_Ch2_ShortLogic,
        Reg058_Ch2_AvgTime,
        Reg059_Spare,
        Reg060_ExtOutput,
        Reg061_Spare,
        Reg062_Spare,
        Reg063_Spare,
        Count
    }

    public enum PerformState
    {
        Ready = 0,
        Performing,
        Perform_OK,
        Perform_NG,
        Cancel,
        Count
    }

    public enum PerformSteps
    {
        Ready=0,
        ShortTest,
        SerialNumber,
        DarkCurrent,
        PLightCAN,
        PLightCurrent,
    }

    public static class GDefines
    {
        public static byte[] BIT8 = new byte[]
        {
            0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80
        };
        public static ushort[] BIT16 = new ushort[]
        {
            0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0080,
            0x0100, 0x0200, 0x0400, 0x0800, 0x1000, 0x2000, 0x4000, 0x8000
        };
        public static uint[] BIT32 = new uint[]
        {
            0x00000001, 0x00000002, 0x00000004, 0x00000008, 0x00000010, 0x00000020, 0x00000040, 0x00000080,
            0x00000100, 0x00000200, 0x00000400, 0x00000800, 0x00001000, 0x00002000, 0x00004000, 0x00008000,
            0x00010000, 0x00020000, 0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x00800000,
            0x01000000, 0x02000000, 0x04000000, 0x08000000, 0x10000000, 0x20000000, 0x40000000, 0x80000000
        };

        public static string[] PRODUCT_TYPE_NAME_STR = new string[]
        {
            "NFC TOUCH LHD",
            "NFC TOUCH RHD",
            "TOUCH ONLY LHD",
            "TOUCH ONLY RHD",
        };

        public static string[] TEST_ITEM_NAME_STR = new string[] {
            "Short 01-02",
            "Short 01-03",
            "Short 01-04",
            //"Short 01-05",
            "Short 01-06",
            "Short 02-03",
            "Short 02-04",
            //"Short 02-05",
            "Short 02-06",
            "Short 03-04",
            //"Short 03-05",
            "Short 03-06",
            //"Short 04-05",
            "Short 04-06",
            //"Short 05-06",
            "Serial Number",
            "Dark Current",
            "P-Light CAN", // CAN 통신 회신
            "P-Light Current", // 전류
            "P-Light Sens", // 조도계
            "Touch",
            "Cancel",
            "Security",
            "NFC",
            "DTC Erase",
            "HW Version",
            "SW Version",
            "Part Number",
            "Bootloader",
            "RXSWIN",
            "Manufacture",
            "Supplier Code",
            "Operation Current",
        };

        public static string[] XCP_ADDRESS_NAME_STR = {
            "Touch FastSelf"  , // Touch_FastSelf,
            "Touch SlowSelf"  , // Touch_SlowSelf,
            "Touch FastMutual", // Touch_FastMutual,
            "Touch ComboRate" , // Touch_ComboRate,
            "Touch State"     , // Touch_State,
            "Cancel FastSelf" , // Cancel_FastSelf,
            "Cancel SlowSelf" , // Cancel_SlowSelf,
            "Cancel State"    , // Cancel_State,
            "Security Bit"    , // Security_SecurityBit,
        };

        public static string[] COMM_TYPE_STR = new string[]
        {
            "CAN",
            "CAN FD",
            "UART"
        };

        // 통신 설정
        public static readonly int   [] COM_BAUDRATE   = { 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400 };
        public static readonly string[] COM_PARITY_BIT = { "None", "Odd", "Even" };
        public static readonly int   [] COM_DATA_BIT   = { 5, 6, 7, 8 };
        public static readonly int   [] COM_STOP_BIT   = { 1, 2 };
        public static readonly string[] COM_HAND_SHAKE = { "None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff" };

        // 전용 컨트롤러 모드버스 레지스터
        public static readonly string[] DedicatedCTRL_RegisterNames =
        {
            "Ch1 Response",
            "Ch1 Complete",
            "Ch1 ConnType",
            "Ch1 Short1_2",
            "Ch1 Short1_3",
            "Ch1 Short1_4",
            "Ch1 Short1_5",
            "Ch1 Short1_6",
            "Ch1 Short2_3",
            "Ch1 Short2_4",
            "Ch1 Short2_5",
            "Ch1 Short2_6",
            "Ch1 Short3_4",
            "Ch1 Short3_5",
            "Ch1 Short3_6",
            "Ch1 Short4_5",
            "Ch1 Short4_6",
            "Ch1 Short5_6",
            "Ch1 LowCurrent",
            "Ch1 HighCurrent",
            "Ch1 LightLux",
            "Ch1 AvgTime",
            "Ch1 Lock Signal",
            "",
            "Ch2 Response",
            "Ch2 Complete",
            "Ch2 ConnType",
            "Ch2 Short1_2",
            "Ch2 Short1_3",
            "Ch2 Short1_4",
            "Ch2 Short1_5",
            "Ch2 Short1_6",
            "Ch2 Short2_3",
            "Ch2 Short2_4",
            "Ch2 Short2_5",
            "Ch2 Short2_6",
            "Ch2 Short3_4",
            "Ch2 Short3_5",
            "Ch2 Short3_6",
            "Ch2 Short4_5",
            "Ch2 Short4_6",
            "Ch2 Short5_6",
            "Ch2 LowCurrent",
            "Ch2 HighCurrent",
            "Ch2 LightLux",
            "Ch2 AvgTime",
            "Ch2 Lock Signal",
            "ExtInput",
            "ExtOutput",
            "Alive",
            "Ch1 Command",
            "Ch1 ConnType",
            "Ch1 ShortLogic",
            "Ch1 AvgTime",
            "",
            "Ch2 Command",
            "Ch2 ConnType",
            "Ch2 ShortLogic",
            "Ch2 AvgTime",
            "",
            "ExtOutput",
            "",
            "",
            "",
        };

        public static readonly string[] PERFORM_STATE_STR =
        {
            "READY",    //Ready,
            "RUNNING",  //Performing,
            "PASS" ,    //Perform_OK,
            "FAIL" ,    //Perform_NG,
            "CANCEL",   //Cancel
        };

        public static readonly string[] TEST_ITEM_OPTION =
        {
            "WR", // W/R
            "RO", // R/O
        };
    }
}

using DHSTesterXL.Forms;
using DHSTesterXL;
using GSCommon;
using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vxlapi_NET;
using static vxlapi_NET.XLClass;

namespace DHSTesterXL
{
    public enum GThemeStyle
    {
        Dark,
        Light
    }

    public enum GLanguages
    {
        ko_KR,
        en_US,
        Count
    };

    public static class GConstans
    {
        public const string LANGUAGE_KO = "ko-KR";
        public const string LANGUAGE_EN = "en-US";
    }
    public static class ControlHelper
    {
        /// <summary>
        /// 컨트롤의 DoubleBuffered 속성을 변경합니다.
        /// </summary>
        /// <param name="contorl"></param>
        /// <param name="setting"></param>
        public static void SetDoubleBuffered(this Control contorl, bool setting)
        {
            Type dgvType = contorl.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(contorl, setting, null);
        }
    }

    public static class GSystem
    {
        /*
         * 폴더 구조
         * 프로그램 실행 폴더
         *    |
         *    +----+-- 프로그램 실행 파일
         *    |    |
         *    |    +-- 프로그램 데이터 파일
         *    |
         *    +-- log (로그 파일을 보관하는 폴더) - log4net 이용
         *         |
         *         +-- 2019 (년도별 폴더)
         *              |
         *              +-- 2019-02 (년-월별 폴더) - 날짜별 파일 생성
         *              |
         *              +-- 2019-03 (년-월별 폴더) - 날짜별 파일 생성
         */

        // 시스템에서 사용하는 폴더 및 파일 관련 변수들

        ////////////////////////////////////////////////////////////////////////////////////////////
        // 로그 객체
        //public static ILog _systemLogger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);  // Logger 선언
        public static ILog _systemLogger = LogManager.GetLogger("SystemLogger");  // Logger 선언
        public static ILog Logger
        {
            get { return _systemLogger; }
        }
        public const string Log_file_ext = "log";

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Fields
        private static readonly GSystemData _systemData = GSystemData.GetInstance();
        private static readonly ProductConfig _productSettings = ProductConfig.GetInstance();

        public static readonly string JSON_EXT = ".json";

        public static readonly string PRODUCTS_PATH = "Products";
        public static readonly string RESULTS_PATH = "Results";

        // 메인 폼 객체
        public static FormDHSTesterXL frmMain;
        // 품목 설정 폼 객체
        public static FormProduct ProductForm;

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Properties
        public static GSystemData SystemData { get { return _systemData; } }
        public static ProductConfig ProductSettings { get {  return _productSettings; } }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // System Special Methods
        public static bool EnableTrace { get; set; } = true;
        public static void TraceMessage(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (EnableTrace)
                Debug.WriteLine($"[ {memberName,-50} ] {message} ({BuildTag(sourceFilePath, sourceLineNumber)})");
        }

        private static string BuildTag(string file, int line)
        {
            return string.Intern($"{Path.GetFileName(file)}:{line}");
        }

        public static T StringToEnum<T>(string e)
        {
            return (T)Enum.Parse(typeof(T), e);
        }

        public static T IntToEnum<T>(int e)
        {
            return (T)(object)e;
        }

        public static void SetDoubleBuffering(System.Windows.Forms.Control control, bool value)
        {
            System.Reflection.PropertyInfo controlProperty = typeof(System.Windows.Forms.Control).GetProperty("DoubleBufferd",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(control, value, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // 색상 필터
        private static ColorSubstitutionFilter colorSubstitutionFilter = new ColorSubstitutionFilter();
        public static ColorSubstitutionFilter ColorSubstitution
        {
            get
            {
                if (colorSubstitutionFilter == null)
                    colorSubstitutionFilter = new ColorSubstitutionFilter();
                return colorSubstitutionFilter;
            }
        }

        public static void SetButtonForeColor(System.Windows.Forms.Button button, Color targetColor)
        {
            ColorSubstitution.SourceColor = button.ForeColor;
            ColorSubstitution.TargetColor = targetColor;
            if (button.Image != null)
                button.Image = BitmapHelper.SubstituteColor(button.Image as Bitmap, ColorSubstitution);
            button.ForeColor = targetColor;
        }

        public static void SetButtonForeColor(System.Windows.Forms.Button button, Color sourceColor, Color targetColor)
        {
            ColorSubstitution.SourceColor = sourceColor;
            ColorSubstitution.TargetColor = targetColor;
            if (button.Image != null)
                button.Image = BitmapHelper.SubstituteColor(button.Image as Bitmap, ColorSubstitution);
            button.ForeColor = targetColor;
        }

        public static void SetStatusLabelForeColor(ToolStripStatusLabel label, Color targetColor)
        {
            ColorSubstitution.SourceColor = label.ForeColor;
            ColorSubstitution.TargetColor = targetColor;
            label.Image = BitmapHelper.SubstituteColor(label.Image as Bitmap, ColorSubstitution);
            label.ForeColor = targetColor;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////
        // 시스템 초기 설정
        public static void Initialize(FormDHSTesterXL form)
        {
            frmMain = form;
            // 시스템 설정 로딩
            SystemDataLoad();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////
        // 시스템데이터 관련 메소드
        public static void SystemDataLoad()
        {
            SystemData.Load();
        }

        public static void SystemDataSave()
        {
            SystemData.Save();
        }



        ////////////////////////////////////////////////////////////////////////////////////////////
        // Utils

        /// <summary>
        /// 시간을 원하는 구간(timeSpan)으로 자르는 함수
        /// 출처 : https://stackoverflow.com/a/1005222
        /// </summary>
        /// <param name="dateTime">시간</param>
        /// <param name="timeSpan">자를 TimeSpan</param>
        /// <returns></returns>
        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
                return dateTime;
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
                return dateTime;
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }

        // <summary>
        /// 시간을 원하는 구간(timeSpan) 근처의 값을 찾는 함수
        /// https://stackoverflow.com/a/20046261
        /// </summary>
        /// <param name="dateTime">시간</param>
        /// <param name="timeSpan">자를 TimeSpan</param>
        /// <returns></returns>
        public static DateTime RoundToNearest(DateTime dateTime, TimeSpan timeSpan)
        {
            long lDelta = dateTime.Ticks % timeSpan.Ticks;
            bool bIsRoundUp = lDelta > timeSpan.Ticks / 2;
            long lOffset = bIsRoundUp ? timeSpan.Ticks : 0;

            return new DateTime(dateTime.Ticks + lOffset - lDelta, dateTime.Kind);
        }




        ////////////////////////////////////////////////////////////////////////////////////////////
        // Global Variables

        public const int CH1 = 0;
        public const int CH2 = 1;
        public const int ChannelCount = 2;
        public const int MaxRetryCount = 3;
        public const int MaxAverageCount = 10;
        public const int MaxJudgeCount = 5;

        private static PXLDriver _canXLDriver = new PXLDriver();
        private static IDHSModel _dhsModel = null;

        public static PXLDriver CanXL { get { return _canXLDriver; } set { _canXLDriver = value; } }
        public static IDHSModel DHSModel { get { return _dhsModel; } set { _dhsModel = value; } }

        public static bool AdminMode { get; set; }

        public delegate void MessageBoxDelegate(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
        public static MessageBoxDelegate MainFormMessageBox = null;

        public delegate void BarcodeResetAndPopUpDelegate(int channel);
        public static BarcodeResetAndPopUpDelegate BarcodeResetAndPopUp = null;

        public static int _nextSerialNo = 1;


        public static TickTimer[] TimerTestTime = new TickTimer[ChannelCount]
        {
            new TickTimer(),
            new TickTimer()
        };
        public static TickTimer[] TimerDarkCurrent = new TickTimer[ChannelCount]
        {
            new TickTimer(),
            new TickTimer()
        };
        public static byte[] TouchLockState = new byte[ChannelCount];
        public static byte[] CancelLockState = new byte[ChannelCount];
        public static byte[] NFC_State = new byte[ChannelCount];
        public static string TrayBarcode = string.Empty;
        public static string[] ProductBarcode = new string[ChannelCount];

        public static bool[] MasterTestOkCh1 = new bool[] { false, false, false, false, false };
        public static bool[] MasterTestOkCh2 = new bool[] { false, false, false, false, false };

        public static uint[] TempSerialNumber = new uint[ChannelCount] { 0, 0 };

        public static int TrayInterlockCount { get; set; } = 10;
        public static int ProductInterlockCount { get; set; } = 0;

        public static bool UseXcpTouchSlowSelf { get; set; } = false;
        public static bool UseXcpTouchComboRate { get; set; } = false;

        ////////////////////////////////////////////////////////////////////////////////////////////
        // M_Layer
        private static MDedicatedCTRL _dedicatedCTRL = new MDedicatedCTRL();
        public static MDedicatedCTRL DedicatedCTRL { get { return _dedicatedCTRL; } }
        public static bool ConnectDedicatedCTRL()
        {
            DedicatedCTRL.SlaveAddress = 2;
            return DedicatedCTRL.Open(
                    SystemData.DedicatedCtrlSettings.PortName,
                    SystemData.DedicatedCtrlSettings.BaudRate,
                    SystemData.DedicatedCtrlSettings.ParityBit,
                    SystemData.DedicatedCtrlSettings.DataBit,
                    SystemData.DedicatedCtrlSettings.StopBit
                    );
        }
        public static void DisconnectDedicatedCTRL()
        {
            //DedicatedCTRL.CloseRelayModule();
            DedicatedCTRL.Close();
        }


        private static readonly MitsubishiPLC _miplc = MitsubishiPLC.GetInstance();
        public static MitsubishiPLC MiPLC { get { return _miplc; } }


        ////////////////////////////////////////////////////////////////////////////////////////////
        // P_Layer
        public static List<int>[] TouchFastMutualList = new List<int>[] { new List<int>(), new List<int>() };
        public static List<int>[] TouchFastSelfList = new List<int>[] { new List<int>(), new List<int>() };

        public static int[] idleTouchFastMutualAvg = new int[] { 0, 0 };
        public static int[] idleTouchFastSelfAvg = new int[] { 0, 0 };
        public static int[] deltaTouchFastMutual = new int[] { 0, 0 };
        public static int[] deltaTouchFastSelf = new int[] { 0, 0 };
        public static int[] thdTouchFastMutual = new int[] { 150, 150 };
        public static int[] thdTouchFastSelf = new int[] { 150, 150 };
        public static int[] judgeCountTouchFastMutual = new int[] { 0, 0 };
        public static int[] judgeCountTouchFastSelf = new int[] { 0, 0 };

        public static bool[] isTouchFastMutualIdleAverage = new bool[] { false, false };
        public static bool[] isTouchFastMutualComplete = new bool[] { false, false };
        public static bool[] isTouchFastSelfIdleAverage = new bool[] { false, false };
        public static bool[] isTouchFastSelfComplete = new bool[] { false, false };
        public static bool[] isTouchFirstExecute = new bool[] { true, true };

        public static List<int>[] CancelFastSelfList = new List<int>[] { new List<int>(), new List<int>() };
        public static List<int>[] CancelSlowSelfList = new List<int>[] { new List<int>(), new List<int>() };

        public static int[] idleCancelFastSelfAvg = new int[] { 0, 0 };
        public static int[] idleCancelSlowSelfAvg = new int[] { 0, 0 };
        public static int[] deltaCancelFastSelf = new int[] { 0, 0 };
        public static int[] deltaCancelSlowSelf = new int[] { 0, 0 };
        public static int[] thdCancelFastSelf = new int[] { 300, 300 };
        public static int[] thdCancelSlowSelf = new int[] { 300, 300 };
        public static int[] judgeCountCancelFastSelf = new int[] { 0, 0 };
        public static int[] judgeCountCancelSlowSelf = new int[] { 0, 0 };

        public static bool[] isCancelFastSelfIdleAverage = new bool[] { false, false };
        public static bool[] isCancelFastSelfComplete = new bool[] { false, false };
        public static bool[] isCancelSlowSelfIdleAverage = new bool[] { false, false };
        public static bool[] isCancelSlowSelfComplete = new bool[] { false, false };
        public static bool[] isCancelFirstExecute = new bool[] { true, true };

        public static bool[] prevStartPressed = new bool[] { false, false };



        // LOT 
        public static string GetLotNumber()
        {
            string lotNumber;

            // 제조년
            int baseYear = 2023;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            char y = (char)('S' + (year - baseYear));
            char m = (char)('A' + month - 1);
            char d;
            if (day < 27)
                d = (char)('A' + day - 1);
            else
                d = (char)('1' + day - 27);
            lotNumber = $"{y}{m}{d}";
            return lotNumber;
        }






        // 라벨 생성
        //public static bool PrintProductLabel(string hw, string sw, string lot, string sn, string printerName = null)
        //{
        //    string zpl = BuildProductLabelZpl(hw, sw, lot, sn);

        //    if (string.IsNullOrWhiteSpace(printerName))
        //        printerName = ProductSettings?.LabelPrint?.PrinterName ?? "ZDesigner ZD421-203dpi ZPL";

        //    return SendRawToPrinter(printerName, zpl);
        //}
        public static bool PrintProductLabel(
            LabelPayload payload,
            LabelStyle style,
            EtcsSettings etcs,
            string printerName = null,
            int? dpi = null, int? darkness = null, int? qty = null, double? speedIps = 1)
        {
            var ps = ProductSettings?.LabelPrint;
            int _dpi = dpi ?? (ps?.Dpi ?? 203);
            int _dark = Math.Max(0, Math.Min(30, darkness ?? 20));
            int _qty = Math.Max(1, qty ?? 1);
            double _ips = speedIps ?? 1.0;

            string name = string.IsNullOrWhiteSpace(printerName)
                ? (ps?.PrinterName ?? "ZDesigner ZD421-203dpi ZPL")
                : printerName;

            // JSON에서 로드된 ETCS 가변값(외부 호출 기본 소스)
            if (etcs == null)
                etcs = ProductSettings?.LabelPrint?.Etcs;

            // 로고 로더
            System.Drawing.Bitmap GetLogo()
            {
                try
                {
                    if (style == null || string.IsNullOrWhiteSpace(style.LogoImagePath)) return null;
                    if (!System.IO.File.Exists(style.LogoImagePath)) return null;
                    return new System.Drawing.Bitmap(style.LogoImagePath);
                }
                catch { return null; }
            }

            // payload.DataMatrix가 비어 있으면, 빌더가 etcs로 ETCS DM을 생성함
            string zpl = ZebraZplFacade.BuildZpl(style, payload, etcs, _dpi, _qty, _dark, _ips, GetLogo);
            return LabelPrinter.SendRawToPrinter(name, zpl);
        }

        /* 하위호환 시그니처 유지(기존 외부 코드가 그대로 동작)
        public static bool PrintProductLabel(
            string hw, string sw, string lot, string sn,
            string partNo, string fccId, string icId, string company, string dataMatrix,
            string printerName = null)
        {
            var payload = new LabelPayload
            {
                HW = hw,
                SW = sw,
                LOT = lot,
                SN = sn,
                PartNo = partNo,
                FCCID = fccId,
                ICID = icId,
                Company = company,
                DataMatrix = dataMatrix
            };

            var style = ProductSettings?.LabelPrint?.Style ?? new LabelStyle();
            return PrintProductLabel(payload, style, etcs, printerName);
        }
        */

        public static async Task ChangePLCRecipeAsync(int recipe)
        {
            await Task.Run(async () =>
            {
                int bitIndex = 0;
                MiPLC.Ch1_W_RecipeNo = (ushort)recipe;
                MiPLC.Ch2_W_RecipeNo = (ushort)recipe;
                MiPLC.Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                MiPLC.Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                MiPLC.M1402_Req_Proc();
                while ((MiPLC.Ch1_R_Status2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex] || (MiPLC.Ch2_R_Status2 & GDefines.BIT16[bitIndex]) != GDefines.BIT16[bitIndex])
                {
                    await Task.Delay(10);
                }
                await Task.Delay(100);
                MiPLC.Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
                MiPLC.Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
                MiPLC.M1402_Req_Proc();
            });
        }

        public static async Task ChangeConnectorTypeAsync(int connectorType)
        {
            await Task.Run(async () =>
            {
                DedicatedCTRL.SetSensorModel(connectorType);
                DedicatedCTRL.SetCommandSensorModel(CH1, true);
                DedicatedCTRL.SetCommandSensorModel(CH2, true);
                while (!DedicatedCTRL.GetCompleteSensorModel(CH1) || !GSystem.DedicatedCTRL.GetCompleteSensorModel(CH2))
                {
                    await Task.Delay(10);
                }
                await Task.Delay(100);
                DedicatedCTRL.SetCommandSensorModel(CH1, false);
                DedicatedCTRL.SetCommandSensorModel(CH2, false);
            });
        }

        public static async Task NFC_Z_MovePosition(int channel, int position)
        {
            await Task.Run(async () =>
            {
                if (channel == 0)
                    MiPLC.Ch1_W_NFC_Z_Pos = (ushort)position;
                else
                    MiPLC.Ch2_W_NFC_Z_Pos = (ushort)position;
                MiPLC.SetNFC_Z_PositionStart(channel, true);
                while (!MiPLC.GetNFC_Z_PositionComplete(channel))
                {
                    await Task.Delay(10);
                }
                await Task.Delay(100);
                MiPLC.SetNFC_Z_PositionStart(channel, false);
            });
        }

        public static async Task ErrorResetAsync(int channel)
        {
            await Task.Run(async () =>
            {
                if (channel == CH1)
                {
                    MiPLC.SetErrorResetStart(CH1, true);
                    while (!MiPLC.GetErrorResetComplete(CH1))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(100);
                    MiPLC.SetErrorResetStart(CH1, false);
                }
                else
                {
                    MiPLC.SetErrorResetStart(CH2, true);
                    while (!MiPLC.GetErrorResetComplete(CH2))
                    {
                        await Task.Delay(10);
                    }
                    await Task.Delay(100);
                    MiPLC.SetErrorResetStart(CH2, false);
                }
            });
        }

    }
}

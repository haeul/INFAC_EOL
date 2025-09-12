using DHSTesterXL.Forms;
using GSCommon;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        public static ILog[] ChannelLogger = new ILog[]
        {
            LogManager.GetLogger("LoggerCh1"),
            LogManager.GetLogger("LoggerCh2")
        };

        public static ILog[] CapaData = new ILog[]
        {
            LogManager.GetLogger("CapaDataCh1"),
            LogManager.GetLogger("CapaDataCh2")
        };

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Fields
        private static readonly GSystemData _systemData = GSystemData.GetInstance();
        private static readonly ProductConfig _productSettings = ProductConfig.GetInstance();

        public static readonly string JSON_EXT = ".json";

        public static readonly string PRODUCTS_PATH = "Products";
        public static readonly string RESULTS_PATH = "Results";

        // 메인 폼 객체
        public static FormDHSTesterXL frmMain;

        ////////////////////////////////////////////////////////////////////////////////////////////
        // Properties
        public static GSystemData SystemData { get { return _systemData; } }
        public static ProductConfig ProductSettings { get { return _productSettings; } }

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
            System.Reflection.PropertyInfo controlProperty = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered",
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

        public static void SetButtonForeColor(Button button, Color targetColor)
        {
            ColorSubstitution.SourceColor = button.ForeColor;
            ColorSubstitution.TargetColor = targetColor;
            if (button.Image != null)
                button.Image = BitmapHelper.SubstituteColor(button.Image as Bitmap, ColorSubstitution);
            button.ForeColor = targetColor;
        }

        public static void SetButtonForeColor(Button button, Color sourceColor, Color targetColor)
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
        public static byte[] NFC_State = new byte[ChannelCount];


        ////////////////////////////////////////////////////////////////////////////////////////////
        // M_Layer
        private static MDedicatedCTRL _dedicatedCTRL = new MDedicatedCTRL();
        public static MDedicatedCTRL DedicatedCTRL { get { return _dedicatedCTRL; } }
        public static bool ConnectDedicatedCTRL()
        {
            DedicatedCTRL.OpenRelayModule(
                    SystemData.RelayModuleSettings.PortName,
                    SystemData.RelayModuleSettings.BaudRate,
                    SystemData.RelayModuleSettings.ParityBit,
                    SystemData.RelayModuleSettings.DataBit,
                    SystemData.RelayModuleSettings.StopBit
                    );

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
            DedicatedCTRL.CloseRelayModule();
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
        public static int[] thdCancelFastSelf = new int[] { 500, 500 };
        public static int[] thdCancelSlowSelf = new int[] { 500, 500 };
        public static int[] judgeCountCancelFastSelf = new int[] { 0, 0 };
        public static int[] judgeCountCancelSlowSelf = new int[] { 0, 0 };

        public static bool[] isCancelFastSelfIdleAverage = new bool[] { false, false };
        public static bool[] isCancelFastSelfComplete = new bool[] { false, false };
        public static bool[] isCancelSlowSelfIdleAverage = new bool[] { false, false };
        public static bool[] isCancelSlowSelfComplete = new bool[] { false, false };
        public static bool[] isCancelFirstExecute = new bool[] { true, true };

        // 라벨 생성
        public static bool PrintProductLabel(string hw, string sw, string lot, string sn, string printerName = null)
        {
            string zpl = BuildProductLabelZpl(hw, sw, lot, sn);

            if (string.IsNullOrWhiteSpace(printerName))
                printerName = ProductSettings?.LabelPrint?.PrinterName ?? "ZDesigner ZD421-203dpi ZPL";

            return SendRawToPrinter(printerName, zpl);
        }

        // Raw 전송 
        public static bool SendRawToPrinter(string printerName, string zpl)
        {
            IntPtr hPrinter;

            if (!OpenPrinter((printerName ?? "").Normalize(), out hPrinter, IntPtr.Zero))
                throw new InvalidOperationException($"OpenPrinter 실패 (Win32:{Marshal.GetLastWin32Error()})");

            var docInfo = new DOCINFOA { pDocName = "ZPL Job", pDataType = "RAW" };

            if (!StartDocPrinter(hPrinter, 1, docInfo))
            {
                ClosePrinter(hPrinter);
                throw new InvalidOperationException($"StartDocPrinter 실패 (Win32:{Marshal.GetLastWin32Error()})");
            }
            if (!StartPagePrinter(hPrinter))
            {
                EndDocPrinter(hPrinter);
                ClosePrinter(hPrinter);
                throw new InvalidOperationException($"StartPagePrinter 실패 (Win32:{Marshal.GetLastWin32Error()})");
            }

            IntPtr unmanagedBuffer = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(zpl ?? "");
                unmanagedBuffer = Marshal.AllocCoTaskMem(bytes.Length);
                Marshal.Copy(bytes, 0, unmanagedBuffer, bytes.Length);

                if (!WritePrinter(hPrinter, unmanagedBuffer, bytes.Length, out _))
                    throw new InvalidOperationException($"WritePrinter 실패 (Win32:{Marshal.GetLastWin32Error()})");

                return true;
            }
            finally
            {
                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);
                ClosePrinter(hPrinter);

                if (unmanagedBuffer != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(unmanagedBuffer);
            }
        }

        // P/Invoke (ANSI, DOCINFOA)
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DOCINFOA
        {
            public string pDocName;
            public string pOutputFile;
            public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In] DOCINFOA di);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);
    
        public static string BuildProductLabelZpl(string hwVersion, string swVersion, string lotNumber, string serialNumber)
        {
            // ───────────────────── 1) 설정 로드 ─────────────────────
            var ps = ProductSettings ?? throw new InvalidOperationException("ProductSettings not loaded.");
            var lp = ps.LabelPrint ?? throw new InvalidOperationException("LabelPrint section missing.");
            var style = lp.Style ?? new LabelStyle();

            // ───────────────────── 2) 가변 데이터 주입 ─────────────────────
            // (FormProductLabel.cs의 패턴에 맞춰 수치만 받아 접두사는 여기서 붙임)
            style.PartText = ps.ProductInfo?.PartNo ?? style.PartText ?? "";
            style.HWText = $"HW:{(hwVersion ?? "").Trim()}";
            style.SWText = $"SW:{(swVersion ?? "").Trim()}";
            style.LOTText = (lotNumber ?? "").Trim();
            style.SerialText = (serialNumber ?? "").Trim();

            // ───────────────────── 3) 공통 유틸(로컬 함수로 캡슐화) ─────────────────────
            int dpi = lp.Dpi > 0 ? lp.Dpi : 203;

            int MmToDots(double mm) => (int)Math.Round(mm * dpi / 25.4);
            string Escape(string s) => s?.Replace("^", "") ?? "";
            string AsciiSafeOneLine(string s) // 줄바꿈/탭 제거 + ASCII 범위만 유지
            {
                if (string.IsNullOrEmpty(s)) return "";
                var asciiSb = new StringBuilder(s.Length);
                foreach (var ch in s)
                {
                    if (ch == '\r' || ch == '\n' || ch == '\t') continue;
                    asciiSb.Append((ch >= 32 && ch <= 126) ? ch : '_');
                }
                return asciiSb.ToString();
            }
            string BuildLotDisplay(string lotRaw, string snRaw)
            {
                string lot = (lotRaw ?? "").Trim();
                string sn = (snRaw ?? "").Trim();
                if (!string.IsNullOrEmpty(lot) && !string.IsNullOrEmpty(sn)) return $"LOT NO : {lot}-{sn}";
                if (!string.IsNullOrEmpty(lot)) return $"LOT NO : {lot}";
                if (!string.IsNullOrEmpty(sn)) return $"LOT NO : {sn}";
                return "LOT NO : ";
            }

            // ───────────────────── 4) 페이로드 준비 ─────────────────────
            string part = style.PartText ?? "";
            string hw = style.HWText ?? "";
            string sw = style.SWText ?? "";
            string lotDisplay = BuildLotDisplay(style.LOTText, style.SerialText);

            // QR 데이터: part|HW:..|SW:..|LOT-SN(둘 다 있을 때 하이픈 결합)
            string lotToken = (style.LOTText ?? "").Trim();
            string snToken = (style.SerialText ?? "").Trim();
            string lotForQr = string.Join("-", new[] { lotToken, snToken }.Where(s => !string.IsNullOrWhiteSpace(s)));

            string qrRaw = $"{part}|{hw}|{sw}|{lotForQr}";
            string qrData = AsciiSafeOneLine(Escape(qrRaw));

            // ───────────────────── 5) 치수/좌표 도트 변환 ─────────────────────
            int PW = MmToDots(style.LabelWmm); // Print Width
            int LL = MmToDots(style.LabelHmm); // Label Length

            int brandX = MmToDots(style.BrandX), brandY = MmToDots(style.BrandY);
            int brandH = MmToDots(style.BrandFont);
            int partY = MmToDots(style.PartY), partH = MmToDots(style.PartFont);

            // 회전/스케일은 UI 그리드에서만 관리하므로 여기서는 기본값(N, 1.0)
            int xHW = MmToDots(style.HWx), yHW = MmToDots(style.HWy);
            int hHW = MmToDots(style.HWfont > 0 ? style.HWfont : 2.6);

            int xSW = MmToDots(style.SWx), ySW = MmToDots(style.SWy);
            int hSW = MmToDots(style.SWfont > 0 ? style.SWfont : 2.6);

            int xLOT = MmToDots(style.LOTx), yLOT = MmToDots(style.LOTy);
            int hLOT = MmToDots(style.LOTfont > 0 ? style.LOTfont : 2.6);

            // ───────────────────── 6) ZPL 조립 ─────────────────────
            var sb = new StringBuilder(1024);
            sb.AppendLine("^XA");
            sb.AppendLine("^PW" + PW);
            sb.AppendLine("^LL" + LL);
            sb.AppendLine("^LH0,0");
            sb.AppendLine("^LT0");
            sb.AppendLine("^LS0");
            sb.AppendLine("^PQ1"); // 항상 1장

            // QR (JSON: UseQr/QRMagnification 반영, 좌표는 간단히 (1mm,1mm))
            if (lp.UseQr)
            {
                int qrX = MmToDots(1.0), qrY = MmToDots(1.0);
                int mag = lp.QRMagnification > 0 ? lp.QRMagnification : 2;
                sb.AppendLine($"^FO{qrX},{qrY}^BQN,2,{mag}^FDQA,{qrData}^FS");
            }

            // 고정 요소(브랜드 / 품번 중앙 정렬)
            sb.AppendLine($"^FO{brandX},{brandY}^A0N,{brandH},{brandH}^FD{Escape(style.BrandText)}^FS");
            sb.AppendLine($"^FO0,{partY}^FB{PW},1,0,C^A0N,{partH},{partH}^FD{Escape(part)}^FS");

            // 가변 요소(HW / SW / LOT+SN)
            sb.AppendLine($"^FO{xHW},{yHW}^A0N,{hHW},{hHW}^FD{Escape(hw)}^FS");
            sb.AppendLine($"^FO{xSW},{ySW}^A0N,{hSW},{hSW}^FD{Escape(sw)}^FS");
            sb.AppendLine($"^FO{xLOT},{yLOT}^A0N,{hLOT},{hLOT}^FD{Escape(lotDisplay)}^FS");

            sb.AppendLine("^XZ");
            return sb.ToString();
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DHSTesterXL
{
    public static class LabelPrinter
    {
        // ───────────────────── 프린터 RAW 전송 ─────────────────────
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;   // RAW
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        public static bool SendRawToPrinter(string printerName, string zpl)
        {
            IntPtr hPrinter;

            if (!OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
                return false;

            var docInfo = new DOCINFOA { pDocName = "ZPL Job", pDataType = "RAW" };

            if (!StartDocPrinter(hPrinter, 1, docInfo))
            {
                ClosePrinter(hPrinter);
                return false;
            }
            if (!StartPagePrinter(hPrinter))
            {
                EndDocPrinter(hPrinter);
                ClosePrinter(hPrinter);
                return false;
            }

            IntPtr unmanagedBuffer = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(zpl);
                unmanagedBuffer = Marshal.AllocCoTaskMem(bytes.Length);
                Marshal.Copy(bytes, 0, unmanagedBuffer, bytes.Length);

                return WritePrinter(hPrinter, unmanagedBuffer, bytes.Length, out _);
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
    }
}

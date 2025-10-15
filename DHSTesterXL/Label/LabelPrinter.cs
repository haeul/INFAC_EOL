﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DHSTesterXL
{
    public static class LabelPrinter
    {
        // ───────────────────── 프린터 RAW 전송 ─────────────────────
        public static bool SendRawToPrinter(string printerName, string zpl)
        {
            IntPtr printerHandle;

            if (!OpenPrinter(printerName.Normalize(), out printerHandle, IntPtr.Zero))
                return false;

            var printJobInfo = new DOCINFOA { pDocName = "ZPL Job", pDataType = "RAW" };

            if (!StartDocPrinter(printerHandle, 1, printJobInfo))
            {
                ClosePrinter(printerHandle);
                return false;
            }
            if (!StartPagePrinter(printerHandle))
            {
                EndDocPrinter(printerHandle);
                ClosePrinter(printerHandle);
                return false;
            }

            IntPtr unmanagedZplBuffer = IntPtr.Zero;
            try
            {
                byte[] zplBytes = Encoding.ASCII.GetBytes(zpl);
                unmanagedZplBuffer = Marshal.AllocCoTaskMem(zplBytes.Length);
                Marshal.Copy(zplBytes, 0, unmanagedZplBuffer, zplBytes.Length);

                return WritePrinter(printerHandle, unmanagedZplBuffer, zplBytes.Length, out _);
            }
            finally
            {
                EndPagePrinter(printerHandle);
                EndDocPrinter(printerHandle);
                ClosePrinter(printerHandle);

                if (unmanagedZplBuffer != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(unmanagedZplBuffer);
            }
        }

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
    }
}

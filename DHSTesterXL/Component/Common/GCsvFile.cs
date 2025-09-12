using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace GSCommon
{
    public class GCsvFile
    {
        private string fileName_;
        private string filePath_;
        private string filePathName_;

        public string FileName
        {
            get { return fileName_; }
        }

        public string FilePath
        {
            get { return filePath_; }
        }

        public string FilePathName
        {
            get { return filePathName_; }
        }

        public string CellFormatDecimalPoint { get; set; }

        private StreamWriter csvWrite_;


        public GCsvFile()
        {

        }

        public bool Create(string fileName, string filePath = @".\data")
        {
            try
            {
                fileName_ = fileName;
                filePath_ = filePath;
                filePathName_ = Path.Combine(filePath, fileName);

                // 폴더 생성
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

                // 파일 생성
                csvWrite_ = File.AppendText(filePathName_);

                return true;
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public bool Open(string fileName, string filePath = @".\data")
        {
            try
            {
                fileName_ = fileName;
                filePath_ = filePath;
                filePathName_ = Path.Combine(filePath, fileName);

                // 폴더 생성
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

                // 파일 생성
                csvWrite_ = File.AppendText(filePathName_);

                return true;
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public void Close()
        {
            try
            {
                csvWrite_.Close();
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void WriteTitle(string model, string comPort, string comSettings)
        {
            try
            {
                csvWrite_.WriteLine("MODEL," + model);
                csvWrite_.WriteLine("COM Port," + comPort);
                csvWrite_.WriteLine("COM Setting," + comSettings + "\r\n");
                csvWrite_.WriteLine("DATE,TIME,ELAPSED TIME,WEIGHT,UNIT");
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Write(string strData)
        {
            try
            {
                csvWrite_.Write(strData);
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Write(string strDate, string strTime, string strWeight, string strUnit)
        {
            try
            {
                string strLine = strDate + "," + strTime + "," + strWeight + "," + strUnit + "\r\n";
                csvWrite_.Write(strLine);
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void WriteLine(string strLine)
        {
            try
            {
                csvWrite_.Write(strLine + "\r\n");
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool ToExcel(string excelFileName, bool bEnableMS = true)
        {
            try
            {
                // 커서 모래시계
                Cursor.Current = Cursors.WaitCursor;

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workBook = excelApp.Workbooks.Open(filePathName_, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                Excel.Worksheet workSheet = (Worksheet)workBook.Worksheets.get_Item(1) as Excel.Worksheet;

                Excel.Range usedRanges = workSheet.UsedRange;

                // COM Setting 셀 수정
                workSheet.Range["B3"].Value = workSheet.Range["B3"].Value + "," + workSheet.Range["C3"].Value + "," + workSheet.Range["D3"].Value + "," + workSheet.Range["E3"].Value;
                workSheet.Range["C3"].Value = "";
                workSheet.Range["D3"].Value = "";
                workSheet.Range["E3"].Value = "";

                // TIME 열 서식 설정
                Range timeColumn = workSheet.Range["B6", "B" + usedRanges.Rows.Count.ToString()];
                if (bEnableMS)
                    timeColumn.Cells.NumberFormat = "HH:mm:ss.000";
                else
                    timeColumn.Cells.NumberFormat = "HH:mm:ss";

                // ELAPSED TIME 열 서식 설정
                Range etimeColumn = workSheet.Range["C6", "C" + usedRanges.Rows.Count.ToString()];
                etimeColumn.Cells.NumberFormat = "d.HH:mm:ss";

                // WEIGHT 열 서식 설정
                Range weightColumn = workSheet.Range["D6", "D" + usedRanges.Rows.Count.ToString()];
                weightColumn.Cells.NumberFormat = CellFormatDecimalPoint;

                workSheet.Columns.AutoFit();

                workBook.SaveAs(excelFileName,
                    XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close();
                excelApp.Quit();

                // 커서 원복
                Cursor.Current = Cursors.Default;

                ReleaseObject(workBook);
                ReleaseObject(excelApp);

                return true;
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj); // 엑셀 개체 해제
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                //throw;
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                GC.Collect(); // 가비지 수집
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DHSTesterXL;

namespace GSCommon
{
    public class GTextFile
    {
        private bool _newCreate = false;
        private bool _readMode = false;
        private string _fileName;
        private string _filePath;
        private string _filePathName;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        public bool IsNew { get { return _newCreate; } }
        public string FileName { get { return _fileName; } }
        public string FilePath { get { return _filePath; } }
        public string FilePathName { get { return _filePathName; } }


        public GTextFile()
        {

        }

        public virtual bool Open(string fileName = @"Default.txt", string filePath = @".\", bool readMode = false)
        {
            try
            {
                _readMode = readMode;
                _fileName = fileName;
                _filePath = filePath;
                _filePathName = Path.Combine(filePath, fileName);

                // 폴더 생성
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

                FileInfo fi = new FileInfo(_filePathName);

                if (fi.Exists)
                    _newCreate = false;
                else
                    _newCreate = true;

                if (_readMode)
                    _streamReader = File.OpenText(_filePathName);
                else
                    _streamWriter = File.CreateText(_filePathName);

                return true;
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
            }

            return false;
        }

        public virtual void Close()
        {
            try
            {
                if (_readMode)
                    _streamReader.Close();
                else
                    _streamWriter.Close();
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
            }
        }

        public virtual string ReadLine()
        {
            try
            {
                return _streamReader.ReadLine();
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
            }

            return null;
        }

        public virtual string ReadAllText()
        {
            try
            {
                return _streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
            }

            return null;
        }

        public virtual void Write(string strData)
        {
            try
            {
                _streamWriter.Write(strData);
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
            }
        }

        public virtual void WriteLine(string strLine)
        {
            try
            {
                _streamWriter.Write(strLine + "\r\n");
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                throw;
            }
        }


    }
}

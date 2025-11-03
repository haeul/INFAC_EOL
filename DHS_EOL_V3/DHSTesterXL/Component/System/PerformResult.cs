using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DHSTesterXL
{
    public class PerformResultJson
    {
        public SProductInfo ProductInfo { get; set; }
        public SCommSettings CommSettings { get; set; }
        public STestItemSpecs TestResult { get; set; }
        public SXCPAddress XCPAddress { get; set; }
        public STestInfo TestInfo { get; set; }

        public PerformResultJson()
        {
            ProductInfo = new SProductInfo();
            CommSettings = new SCommSettings();
            TestResult = new STestItemSpecs();
            XCPAddress = new SXCPAddress();
            TestInfo = new STestInfo();
        }
    }

    public class PerformResult : PerformResultJson
    {
        private static PerformResult instance_ = null;

        public PerformState State { get; set; } = PerformState.Ready;

        public PerformResult()
        {
        }
        
        public static PerformResult GetInstance()
        {
            if (instance_ == null)
                instance_ = new PerformResult();

            return instance_;
        }

        public bool Load(string fileName = "PerformResult.json", string filePath = @".\Results")
        {
            try
            {
                // 폴더 생성
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

                string filePathName = Path.Combine(filePath, fileName);

                FileInfo fi = new FileInfo(filePathName);

                if (fi.Exists)
                {
                    // 파일이 있으면 파일을 읽는다.
                    PerformResultJson jsonData = JsonConvert.DeserializeObject<PerformResultJson>(File.ReadAllText(filePathName));
                    ProductInfo  = jsonData.ProductInfo;
                    CommSettings = jsonData.CommSettings;
                    TestResult   = jsonData.TestResult;
                    XCPAddress   = jsonData.XCPAddress;
                    TestInfo     = jsonData.TestInfo;
                }
                else
                {
                    // 파일이 없으면 기본 값으로 파일을 생성한다.
                    PerformResultJson jsonData = new PerformResultJson();
                    jsonData.ProductInfo  = ProductInfo;
                    jsonData.CommSettings = CommSettings;
                    jsonData.TestResult   = TestResult;
                    jsonData.XCPAddress   = XCPAddress;
                    jsonData.TestInfo     = TestInfo;

                    // 생성한 기본 값을 파일에 저장한다.
                    File.WriteAllText(filePathName, JsonConvert.SerializeObject(jsonData, Formatting.Indented));
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        public bool Save(string fileName = "PerformResult.json", string filePath = @".\Results")
        {
            try
            {
                // 폴더 생성
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

                string filePathName = Path.Combine(filePath, fileName);

                PerformResultJson jsonData = new PerformResultJson();
                jsonData.ProductInfo  = ProductInfo;
                jsonData.CommSettings = CommSettings;
                jsonData.TestResult   = TestResult;
                jsonData.XCPAddress   = XCPAddress;
                jsonData.TestInfo     = TestInfo;

                File.WriteAllText(filePathName, JsonConvert.SerializeObject(jsonData, Formatting.Indented));

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        public List<string> GetPerformResultList(string folderPath = @".\Results")
        {
            List<string> _fileNames = new List<string>();

            if (Directory.Exists(folderPath))
            {
                // Get all file paths in the directory
                string[] files = Directory.GetFiles(folderPath);

                // Extract file names without extensions
                _fileNames = files.Select(filePath => Path.GetFileNameWithoutExtension(filePath)).ToList();
            }
            else
            {
                // Handle the case where the directory doesn't exist
                // You might want to throw an exception or log a message here
                System.Console.WriteLine($"Error: Folder not found at '{folderPath}'");
            }

            return _fileNames;
        }
    }
}

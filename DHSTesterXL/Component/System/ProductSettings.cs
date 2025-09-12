using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DHSTesterXL
{
    public class SProductInfo
    {
        public string PartNo { get; set; } // 82657-XH010
        public string PartName { get; set; } //
        public string TypeName { get; set; } // NFC TOUCH LHD
        public string TypeNo { get; set; } // 11
        public string CarType { get; set; } // NH2
        public string AlcNo { get; set; } // NFC TOUCH

        public SProductInfo()
        {
            PartNo = "82657-XH010";
            PartName = "";
            TypeName = "NFC TOUCH LHD";
            TypeNo = "11";
            CarType = "NH2";
            AlcNo = "NFC TOUCH";
        }
    }

    public class SCommSettings
    {
        public string CommType { get; set; } // CAN, CAN FD, UART
        public int ClockFreq { get; set; } // 20 MHz
        public int ArbtBitRate { get; set; } // 500
        public int ArbtTseg1 { get; set; } // 64
        public int ArbtTseg2 { get; set; } // 16
        public int ArbtSjw { get; set; } // 16
        public int DataBitRate { get; set; } // 1000
        public int DataTseg1 { get; set; } // 14
        public int DataTseg2 { get; set; } // 5
        public int DataSjw { get; set; } // 3
        public string UartPortName { get; set; }
        public string CanReqID { get; set; } // 0x702
        public string CanResID { get; set; } // 0x70A
        public string NM_ReqID { get; set; } // 0x17FC0080
        public string NM_ResID { get; set; } // 0x8000080
        public string PLightReqID { get; set; } // 0x3D4
        public string PLightResID { get; set; } // 0x4000080
        public bool PLightUse { get; set; }

        public SCommSettings()
        {
            CommType = "CAN FD";
            ClockFreq = 20;
            ArbtBitRate = 500;
            ArbtTseg1 = 64;
            ArbtTseg2 = 16;
            ArbtSjw = 16;
            DataBitRate = 1000;
            DataTseg1 = 14;
            DataTseg2 = 5;
            DataSjw = 3;
            UartPortName = "COM6";
            CanReqID = "0x702";
            CanResID = "0x70A";
            NM_ReqID = "0x17FC0080";
            NM_ResID = "0x8000080";
            PLightReqID = "0x3D4";
            PLightResID = "0x4000080";
            PLightUse = false;
        }
    }

    public class TestSpec
    {
        public string Name { get; set; }
        public bool Use { get; set; }
        public int CtrlType { get; set; } // 0:전용 CTRL, 1:통신(CAN or UART)
        public int DataType { get; set; } // 0:double, 1:int, 2:string
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string MinString { get; set; } // ex)82657XH010
        public string MaxString { get; set; } // ex)82657XH010
        public int Option { get; set; } // 0:Write/Read, 1:Read Only
        public string Remarks { get; set; }

        public TestSpec()
        {
            Name = string.Empty;
            Use = true;
            CtrlType = 0;
            DataType = 0;
            MinValue = 0f;
            MaxValue = 1f;
            MinString = string.Empty;
            MaxString = string.Empty;
            Option = 0;
            Remarks = string.Empty;
        }
    }

    public class STestItemSpecs
    {
        public TestSpec Short_1_2 { get; set; }
        public TestSpec Short_1_3 { get; set; }
        public TestSpec Short_1_4 { get; set; }
        public TestSpec Short_1_5 { get; set; }
        public TestSpec Short_1_6 { get; set; }
        public TestSpec Short_2_3 { get; set; }
        public TestSpec Short_2_4 { get; set; }
        public TestSpec Short_2_5 { get; set; }
        public TestSpec Short_2_6 { get; set; }
        public TestSpec Short_3_4 { get; set; }
        public TestSpec Short_3_5 { get; set; }
        public TestSpec Short_3_6 { get; set; }
        public TestSpec Short_4_5 { get; set; }
        public TestSpec Short_4_6 { get; set; }
        public TestSpec Short_5_6 { get; set; }
        public TestSpec SerialNumber { get; set; }
        public TestSpec DarkCurrent { get; set; }
        public TestSpec PLightTurnOn { get; set; }
        public TestSpec PLightCurrent { get; set; }
        public TestSpec PLightAmbient { get; set; }
        public TestSpec LockSignal { get; set; }
        public TestSpec Touch { get; set; }
        public TestSpec Cancel { get; set; }
        public TestSpec NFC { get; set; }
        public TestSpec SecurityBit { get; set; }
        public TestSpec DTC_Erase { get; set; }
        public TestSpec HW_Version { get; set; }
        public TestSpec SW_Version { get; set; }
        public TestSpec PartNumber { get; set; }
        public TestSpec Bootloader { get; set; }
        public TestSpec RXSWIN { get; set; }
        public TestSpec Manufacture { get; set; }
        public TestSpec SupplierCode { get; set; }
        public TestSpec OperationCurrent { get; set; }

        public STestItemSpecs()
        {
            Short_1_2 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_2], DataType = 1 };
            Short_1_3 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_3], DataType = 1 };
            Short_1_4 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_4], DataType = 1 };
            Short_1_6 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_6], DataType = 1 };
            Short_2_3 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_3], DataType = 1 };
            Short_2_4 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_4], DataType = 1 };
            Short_2_6 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_6], DataType = 1 };
            Short_3_4 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_4], DataType = 1 };
            Short_3_6 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_6], DataType = 1 };
            Short_4_6 = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_4_6], DataType = 1 };
            SerialNumber = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SerialNumber], DataType = 2 };
            DarkCurrent = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.DarkCurrent], DataType = 1 };
            PLightTurnOn = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.PLightTurnOn], DataType = 1 };
            PLightCurrent = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.PLightCurrent], DataType = 0 };
            PLightAmbient = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.PLightAmbient], DataType = 0 };
            Touch = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Touch], DataType = 1 };
            Cancel = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Cancel], DataType = 1 };
            NFC = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.NFC], DataType = 1 };
            SecurityBit = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SecurityBit], DataType = 1 };
            DTC_Erase = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.DTC_Erase], DataType = 1 };
            HW_Version = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.HW_Version], DataType = 2 };
            SW_Version = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SW_Version], DataType = 2 };
            PartNumber = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.PartNumber], DataType = 2 };
            Bootloader = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Bootloader], DataType = 2 };
            RXSWIN = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.RXSWIN], DataType = 2 };
            Manufacture = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Manufacture], DataType = 2 };
            SupplierCode = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SupplierCode], DataType = 2 };
            OperationCurrent = new TestSpec() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.OperationCurrent], DataType = 0 };
        }
    }

    public class XCPSpec
    {
        public string Name { get; set; }
        public bool Use { get; set; }
        public string Address { get; set; }
        public XCPSpec()
        {
            Name = "";
            Use = true;
            Address = "0x20007E00";
        }
    }

    public class SXCPAddress
    {
        public bool Use { get; set; }
        public string ReqID { get; set; }
        public string ResID { get; set; }
        public string EcuAddr { get; set; }
        public XCPSpec TouchFastSelf { get; set; }
        public XCPSpec TouchSlowSelf { get; set; }
        public XCPSpec TouchFastMutual { get; set; }
        public XCPSpec TouchComboRate { get; set; }
        public XCPSpec TouchState { get; set; }
        public XCPSpec CancelFastSelf { get; set; }
        public XCPSpec CancelSlowSelf { get; set; }
        public XCPSpec CancelState { get; set; }
        public XCPSpec SecurityBit { get; set; }

        public SXCPAddress()
        {
            Use = false;
            ReqID = "0x330";
            ResID = "0x360";
            EcuAddr = "0x60";
            TouchFastSelf = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Touch_FastSelf] };
            TouchSlowSelf = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Touch_SlowSelf] };
            TouchFastMutual = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Touch_FastMutual] };
            TouchComboRate = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Touch_ComboRate] };
            TouchState = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Touch_State] };
            CancelFastSelf = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Cancel_FastSelf] };
            CancelSlowSelf = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Cancel_SlowSelf] };
            CancelState = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Cancel_State] };
            SecurityBit = new XCPSpec() { Name = GDefines.XCP_ADDRESS_NAME_STR[(int)XCP_Items.Security_Bit] };
        }
    }

    public class STestInfo
    {
        public uint TestCountTot { get; set; }
        public uint OkCountTot { get; set; }
        public uint NgCountTot { get; set; }
        public double NgRateTot { get; set; }
        public uint SerialNumTot { get; set; }

        public uint TestCountCh1 { get; set; }
        public uint OkCountCh1 { get; set; }
        public uint NgCountCh1 { get; set; }
        public double NgRateCh1 { get; set; }
        public uint SerialNumCh1 { get; set; }

        public uint TestCountCh2 { get; set; }
        public uint OkCountCh2 { get; set; }
        public uint NgCountCh2 { get; set; }
        public double NgRateCh2 { get; set; }
        public uint SerialNumCh2 { get; set; }

        public STestInfo()
        {
        }
    }
    public class LabelDefault
    {
        public string PartNo { get; set; } = "82657-DC000";
        public string HW { get; set; } = "HW : 1.00";
        public string SW { get; set; } = "SW : 2.52";
        public string Lot { get; set; } = "LOT NO : 240";
        public string Serial { get; set; } = "S/N : 1234";

        public LabelDefault Clone() => (LabelDefault)MemberwiseClone();
    }

    public class LabelPrintSettings
    {
        public string PrinterName { get; set; } = ""; // 선택 프린터명
        public int Dpi { get; set; } = 203;           // ZD421 기본 203dpi
        public int QRMagnification { get; set; } = 2; // ^BQN magnification
        public bool UseQr { get; set; } = true;

        // 레이아웃/폰트 등은 기존 클래스 재사용
        public LabelStyle Style { get; set; } = new LabelStyle();
        public LabelDefault Default { get; set; } = new LabelDefault();
        public LabelPrintSettings Clone() => new LabelPrintSettings
        {
            PrinterName = PrinterName,
            Dpi = Dpi,
            QRMagnification = QRMagnification,
            UseQr = UseQr,
            Style = Style?.Clone(),
            Default = Default?.Clone()
        };
    }

    public class ProductSettingsJson
    {
        public SProductInfo ProductInfo { get; set; }
        public SCommSettings CommSettings { get; set; }
        public STestItemSpecs TestItemSpecs { get; set; }
        public SXCPAddress XCPAddress { get; set; }
        public STestInfo TestInfo { get; set; }
        public LabelPrintSettings LabelPrint { get; set; }


        public ProductSettingsJson()
        {
            ProductInfo = new SProductInfo();
            CommSettings = new SCommSettings();
            TestItemSpecs = new STestItemSpecs();
            XCPAddress = new SXCPAddress();
            TestInfo = new STestInfo();
            LabelPrint = new LabelPrintSettings();
        }
    }

    public class ProductConfig : ProductSettingsJson
    {
        private static ProductConfig instance_ = null;

        public uint CanReqID { get; set; }
        public uint CanResID { get; set; }
        public uint NM_ReqID { get; set; }
        public uint NM_ResID { get; set; }
        public uint PLightReqID { get; set; }
        public uint PLightResID { get; set; }
        public uint XcpReqID { get; set; }
        public uint XcpResID { get; set; }
        public byte XcpEcuAddr { get; set; }

        public static ProductConfig GetInstance()
        {
            if (instance_ == null)
                instance_ = new ProductConfig();

            return instance_;
        }

        public bool Load(string fileName/* = "ProductSettings.json"*/, string filePath/* = @".\Products"*/)
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
                    ProductSettingsJson jsonData = JsonConvert.DeserializeObject<ProductSettingsJson>(File.ReadAllText(filePathName));
                    // 하위 객체 널 보정 (← 생성자 기본값을 강제로 복구)
                    ProductInfo = jsonData.ProductInfo ?? new SProductInfo();
                    CommSettings = jsonData.CommSettings ?? new SCommSettings();
                    TestItemSpecs = jsonData.TestItemSpecs ?? new STestItemSpecs();
                    XCPAddress = jsonData.XCPAddress ?? new SXCPAddress();
                    TestInfo = jsonData.TestInfo ?? new STestInfo();
                    LabelPrint = jsonData.LabelPrint ?? new LabelPrintSettings();
                }
                else
                {
                    // 파일이 없으면 기본 값으로 파일을 생성한다.
                    ProductSettingsJson jsonData = new ProductSettingsJson();
                    jsonData.ProductInfo = ProductInfo;
                    jsonData.CommSettings = CommSettings;
                    jsonData.TestItemSpecs = TestItemSpecs;
                    jsonData.XCPAddress = XCPAddress;
                    jsonData.TestInfo = TestInfo;
                    jsonData.LabelPrint = LabelPrint;

                    // 생성한 기본 값을 파일에 저장한다.
                    File.WriteAllText(filePathName, JsonConvert.SerializeObject(jsonData, Formatting.Indented));
                }

                CanReqID = GetCanReqID();
                CanResID = GetCanResID();
                NM_ReqID = GetNM_ReqID();
                NM_ResID = GetNM_ResID();
                XcpReqID = GetXcpReqID();
                XcpResID = GetXcpResID();
                XcpEcuAddr = GetXcpEcuAddr();
                PLightReqID = GetPLightReqID();
                PLightResID = GetPLightResID();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        public bool Save(string fileName/* = "ProductSettings.json"*/, string filePath/* = @".\Products"*/)
        {
            try
            {
                // 폴더 생성
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

                string filePathName = Path.Combine(filePath, fileName);

                ProductSettingsJson jsonData = new ProductSettingsJson();
                jsonData.ProductInfo = ProductInfo;
                jsonData.CommSettings = CommSettings;
                jsonData.TestItemSpecs = TestItemSpecs;
                jsonData.XCPAddress = XCPAddress;
                jsonData.TestInfo = TestInfo;
                jsonData.LabelPrint = LabelPrint;

                File.WriteAllText(filePathName, JsonConvert.SerializeObject(jsonData, Formatting.Indented));

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        public List<string> GetProductList(string folderPath/* = @".\Products"*/)
        {
            List<string> fileNames = new List<string>();

            if (Directory.Exists(folderPath))
            {
                // 디렉토리가 있을 경우
                // 디렉토리 안에 있는 모든 파일의 확장자를 제외한 파일명을 리스트에 저장한다.
                string[] filePaths = Directory.GetFiles(folderPath);
                foreach (string filePath in filePaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    fileNames.Add(fileName);
                }
            }
            else
            {
                // 디렉토리가 없을 경우
                // 디렉토리를 생성한다
                Directory.CreateDirectory(folderPath);
            }

            return fileNames;
        }

        public List<TestSpec> GetEnableTestItemsList()
        {
            List<TestSpec> testItems = new List<TestSpec>();
            for (int i = 0; i < (int)TestItems.Count; i++)
            {
                switch ((TestItems)i)
                {
                    case TestItems.Short_1_2: if (TestItemSpecs.Short_1_2.Use) testItems.Add(TestItemSpecs.Short_1_2); break;
                    case TestItems.Short_1_3: if (TestItemSpecs.Short_1_3.Use) testItems.Add(TestItemSpecs.Short_1_3); break;
                    case TestItems.Short_1_4: if (TestItemSpecs.Short_1_4.Use) testItems.Add(TestItemSpecs.Short_1_4); break;
                    case TestItems.Short_1_6: if (TestItemSpecs.Short_1_6.Use) testItems.Add(TestItemSpecs.Short_1_6); break;
                    case TestItems.Short_2_3: if (TestItemSpecs.Short_2_3.Use) testItems.Add(TestItemSpecs.Short_2_3); break;
                    case TestItems.Short_2_4: if (TestItemSpecs.Short_2_4.Use) testItems.Add(TestItemSpecs.Short_2_4); break;
                    case TestItems.Short_2_6: if (TestItemSpecs.Short_2_6.Use) testItems.Add(TestItemSpecs.Short_2_6); break;
                    case TestItems.Short_3_4: if (TestItemSpecs.Short_3_4.Use) testItems.Add(TestItemSpecs.Short_3_4); break;
                    case TestItems.Short_3_6: if (TestItemSpecs.Short_3_6.Use) testItems.Add(TestItemSpecs.Short_3_6); break;
                    case TestItems.Short_4_6: if (TestItemSpecs.Short_4_6.Use) testItems.Add(TestItemSpecs.Short_4_6); break;
                    case TestItems.SerialNumber: if (TestItemSpecs.SerialNumber.Use) testItems.Add(TestItemSpecs.SerialNumber); break;
                    case TestItems.DarkCurrent: if (TestItemSpecs.DarkCurrent.Use) testItems.Add(TestItemSpecs.DarkCurrent); break;
                    case TestItems.PLightTurnOn: if (TestItemSpecs.PLightTurnOn.Use) testItems.Add(TestItemSpecs.PLightTurnOn); break;
                    case TestItems.PLightCurrent: if (TestItemSpecs.PLightCurrent.Use) testItems.Add(TestItemSpecs.PLightCurrent); break;
                    case TestItems.PLightAmbient: if (TestItemSpecs.PLightAmbient.Use) testItems.Add(TestItemSpecs.PLightAmbient); break;
                    case TestItems.Touch: if (TestItemSpecs.Touch.Use) testItems.Add(TestItemSpecs.Touch); break;
                    case TestItems.Cancel: if (TestItemSpecs.Cancel.Use) testItems.Add(TestItemSpecs.Cancel); break;
                    case TestItems.SecurityBit: if (TestItemSpecs.SecurityBit.Use) testItems.Add(TestItemSpecs.SecurityBit); break;
                    case TestItems.NFC: if (TestItemSpecs.NFC.Use) testItems.Add(TestItemSpecs.NFC); break;
                    case TestItems.DTC_Erase: if (TestItemSpecs.DTC_Erase.Use) testItems.Add(TestItemSpecs.DTC_Erase); break;
                    case TestItems.HW_Version: if (TestItemSpecs.HW_Version.Use) testItems.Add(TestItemSpecs.HW_Version); break;
                    case TestItems.SW_Version: if (TestItemSpecs.SW_Version.Use) testItems.Add(TestItemSpecs.SW_Version); break;
                    case TestItems.PartNumber: if (TestItemSpecs.PartNumber.Use) testItems.Add(TestItemSpecs.PartNumber); break;
                    case TestItems.Bootloader: if (TestItemSpecs.Bootloader.Use) testItems.Add(TestItemSpecs.Bootloader); break;
                    case TestItems.RXSWIN: if (TestItemSpecs.RXSWIN.Use) testItems.Add(TestItemSpecs.RXSWIN); break;
                    case TestItems.Manufacture: if (TestItemSpecs.Manufacture.Use) testItems.Add(TestItemSpecs.Manufacture); break;
                    case TestItems.SupplierCode: if (TestItemSpecs.SupplierCode.Use) testItems.Add(TestItemSpecs.SupplierCode); break;
                    case TestItems.OperationCurrent: if (TestItemSpecs.OperationCurrent.Use) testItems.Add(TestItemSpecs.OperationCurrent); break;
                    default:
                        break;
                }
            }
            return testItems;
        }

        public string GetTestItemName(TestItems testItem)
        {
            string testName = string.Empty;
            //for (int i = 0; i < (int)TestItems.Count; i++)
            {
                switch (testItem)
                {
                    case TestItems.Short_1_2: testName = TestItemSpecs.Short_1_2.Name; break;
                    case TestItems.Short_1_3: testName = TestItemSpecs.Short_1_3.Name; break;
                    case TestItems.Short_1_4: testName = TestItemSpecs.Short_1_4.Name; break;
                    case TestItems.Short_1_6: testName = TestItemSpecs.Short_1_6.Name; break;
                    case TestItems.Short_2_3: testName = TestItemSpecs.Short_2_3.Name; break;
                    case TestItems.Short_2_4: testName = TestItemSpecs.Short_2_4.Name; break;
                    case TestItems.Short_2_6: testName = TestItemSpecs.Short_2_6.Name; break;
                    case TestItems.Short_3_4: testName = TestItemSpecs.Short_3_4.Name; break;
                    case TestItems.Short_3_6: testName = TestItemSpecs.Short_3_6.Name; break;
                    case TestItems.Short_4_6: testName = TestItemSpecs.Short_4_6.Name; break;
                    case TestItems.DarkCurrent: testName = TestItemSpecs.DarkCurrent.Name; break;
                    case TestItems.PLightTurnOn: testName = TestItemSpecs.PLightTurnOn.Name; break;
                    case TestItems.PLightCurrent: testName = TestItemSpecs.PLightCurrent.Name; break;
                    case TestItems.PLightAmbient: testName = TestItemSpecs.PLightAmbient.Name; break;
                    case TestItems.Touch: testName = TestItemSpecs.Touch.Name; break;
                    case TestItems.Cancel: testName = TestItemSpecs.Cancel.Name; break;
                    case TestItems.SecurityBit: testName = TestItemSpecs.SecurityBit.Name; break;
                    case TestItems.NFC: testName = TestItemSpecs.NFC.Name; break;
                    case TestItems.DTC_Erase: testName = TestItemSpecs.DTC_Erase.Name; break;
                    case TestItems.HW_Version: testName = TestItemSpecs.HW_Version.Name; break;
                    case TestItems.SW_Version: testName = TestItemSpecs.SW_Version.Name; break;
                    case TestItems.PartNumber: testName = TestItemSpecs.PartNumber.Name; break;
                    case TestItems.Bootloader: testName = TestItemSpecs.Bootloader.Name; break;
                    case TestItems.RXSWIN: testName = TestItemSpecs.RXSWIN.Name; break;
                    case TestItems.SupplierCode: testName = TestItemSpecs.SupplierCode.Name; break;
                    case TestItems.OperationCurrent: testName = TestItemSpecs.OperationCurrent.Name; break;
                    case TestItems.SerialNumber: testName = TestItemSpecs.SerialNumber.Name + $" ({GDefines.TEST_ITEM_OPTION[TestItemSpecs.SerialNumber.Option]}"; break;
                    case TestItems.Manufacture: testName = TestItemSpecs.Manufacture.Name + $" ({GDefines.TEST_ITEM_OPTION[TestItemSpecs.Manufacture.Option]}"; break;
                    default:
                        break;
                }
            }
            return testName;
        }

        public TestSpec GetTestItemSpec(TestItems item)
        {
            try
            {
                // item에 해당하는 항목의 측정 여부를 반환한다.
                switch (item)
                {
                    case TestItems.Short_1_2: return TestItemSpecs.Short_1_2;
                    case TestItems.Short_1_3: return TestItemSpecs.Short_1_3;
                    case TestItems.Short_1_4: return TestItemSpecs.Short_1_4;
                    case TestItems.Short_1_6: return TestItemSpecs.Short_1_6;
                    case TestItems.Short_2_3: return TestItemSpecs.Short_2_3;
                    case TestItems.Short_2_4: return TestItemSpecs.Short_2_4;
                    case TestItems.Short_2_6: return TestItemSpecs.Short_2_6;
                    case TestItems.Short_3_4: return TestItemSpecs.Short_3_4;
                    case TestItems.Short_3_6: return TestItemSpecs.Short_3_6;
                    case TestItems.Short_4_6: return TestItemSpecs.Short_4_6;
                    case TestItems.SerialNumber: return TestItemSpecs.SerialNumber;
                    case TestItems.DarkCurrent: return TestItemSpecs.DarkCurrent;
                    case TestItems.PLightTurnOn: return TestItemSpecs.PLightTurnOn;
                    case TestItems.PLightCurrent: return TestItemSpecs.PLightCurrent;
                    case TestItems.PLightAmbient: return TestItemSpecs.PLightAmbient;
                    case TestItems.Touch: return TestItemSpecs.Touch;
                    case TestItems.Cancel: return TestItemSpecs.Cancel;
                    case TestItems.SecurityBit: return TestItemSpecs.SecurityBit;
                    case TestItems.NFC: return TestItemSpecs.NFC;
                    case TestItems.DTC_Erase: return TestItemSpecs.DTC_Erase;
                    case TestItems.HW_Version: return TestItemSpecs.HW_Version;
                    case TestItems.SW_Version: return TestItemSpecs.SW_Version;
                    case TestItems.PartNumber: return TestItemSpecs.PartNumber;
                    case TestItems.Bootloader: return TestItemSpecs.Bootloader;
                    case TestItems.RXSWIN: return TestItemSpecs.RXSWIN;
                    case TestItems.Manufacture: return TestItemSpecs.Manufacture;
                    case TestItems.SupplierCode: return TestItemSpecs.SupplierCode;
                    case TestItems.OperationCurrent: return TestItemSpecs.OperationCurrent;
                    default:
                        return new TestSpec();
                }
            }
            catch
            {
            }
            return new TestSpec();
        }

        public void SetTestItemSpec(TestItems item, TestSpec newSpec)
        {
            try
            {
                // item에 해당하는 항목의 측정 여부를 반환한다.
                switch (item)
                {
                    case TestItems.Short_1_2: TestItemSpecs.Short_1_2 = newSpec; break;
                    case TestItems.Short_1_3: TestItemSpecs.Short_1_3 = newSpec; break;
                    case TestItems.Short_1_4: TestItemSpecs.Short_1_4 = newSpec; break;
                    case TestItems.Short_1_6: TestItemSpecs.Short_1_6 = newSpec; break;
                    case TestItems.Short_2_3: TestItemSpecs.Short_2_3 = newSpec; break;
                    case TestItems.Short_2_4: TestItemSpecs.Short_2_4 = newSpec; break;
                    case TestItems.Short_2_6: TestItemSpecs.Short_2_6 = newSpec; break;
                    case TestItems.Short_3_4: TestItemSpecs.Short_3_4 = newSpec; break;
                    case TestItems.Short_3_6: TestItemSpecs.Short_3_6 = newSpec; break;
                    case TestItems.Short_4_6: TestItemSpecs.Short_4_6 = newSpec; break;
                    case TestItems.DarkCurrent: TestItemSpecs.DarkCurrent = newSpec; break;
                    case TestItems.PLightTurnOn: TestItemSpecs.PLightTurnOn = newSpec; break;
                    case TestItems.PLightCurrent: TestItemSpecs.PLightCurrent = newSpec; break;
                    case TestItems.PLightAmbient: TestItemSpecs.PLightAmbient = newSpec; break;
                    case TestItems.Touch: TestItemSpecs.Touch = newSpec; break;
                    case TestItems.Cancel: TestItemSpecs.Cancel = newSpec; break;
                    case TestItems.SecurityBit: TestItemSpecs.SecurityBit = newSpec; break;
                    case TestItems.NFC: TestItemSpecs.NFC = newSpec; break;
                    case TestItems.DTC_Erase: TestItemSpecs.DTC_Erase = newSpec; break;
                    case TestItems.HW_Version: TestItemSpecs.HW_Version = newSpec; break;
                    case TestItems.SW_Version: TestItemSpecs.SW_Version = newSpec; break;
                    case TestItems.PartNumber: TestItemSpecs.PartNumber = newSpec; break;
                    case TestItems.Bootloader: TestItemSpecs.Bootloader = newSpec; break;
                    case TestItems.RXSWIN: TestItemSpecs.RXSWIN = newSpec; break;
                    case TestItems.SerialNumber: TestItemSpecs.SerialNumber = newSpec; break;
                    case TestItems.Manufacture: TestItemSpecs.Manufacture = newSpec; break;
                    case TestItems.SupplierCode: TestItemSpecs.SupplierCode = newSpec; break;
                    case TestItems.OperationCurrent: TestItemSpecs.OperationCurrent = newSpec; break;
                    default:
                        break;
                }
            }
            catch
            {
            }
        }

        public XCPSpec GetXcpItemSpec(XCP_Items item)
        {
            try
            {
                switch (item)
                {
                    case XCP_Items.Touch_FastSelf: return XCPAddress.TouchFastSelf;
                    case XCP_Items.Touch_SlowSelf: return XCPAddress.TouchSlowSelf;
                    case XCP_Items.Touch_FastMutual: return XCPAddress.TouchFastMutual;
                    case XCP_Items.Touch_ComboRate: return XCPAddress.TouchComboRate;
                    case XCP_Items.Touch_State: return XCPAddress.TouchState;
                    case XCP_Items.Cancel_FastSelf: return XCPAddress.CancelFastSelf;
                    case XCP_Items.Cancel_SlowSelf: return XCPAddress.CancelSlowSelf;
                    case XCP_Items.Cancel_State: return XCPAddress.CancelState;
                    case XCP_Items.Security_Bit: return XCPAddress.SecurityBit;
                    default:
                        return new XCPSpec();
                }
            }
            catch
            {
            }
            return new XCPSpec();
        }

        public void SetXcpItemSpec(XCP_Items item, XCPSpec newSpec)
        {
            try
            {
                switch (item)
                {
                    case XCP_Items.Touch_FastSelf: XCPAddress.TouchFastSelf = newSpec; break;
                    case XCP_Items.Touch_SlowSelf: XCPAddress.TouchSlowSelf = newSpec; break;
                    case XCP_Items.Touch_FastMutual: XCPAddress.TouchFastMutual = newSpec; break;
                    case XCP_Items.Touch_ComboRate: XCPAddress.TouchComboRate = newSpec; break;
                    case XCP_Items.Touch_State: XCPAddress.TouchState = newSpec; break;
                    case XCP_Items.Cancel_FastSelf: XCPAddress.CancelFastSelf = newSpec; break;
                    case XCP_Items.Cancel_SlowSelf: XCPAddress.CancelSlowSelf = newSpec; break;
                    case XCP_Items.Cancel_State: XCPAddress.CancelState = newSpec; break;
                    case XCP_Items.Security_Bit: XCPAddress.SecurityBit = newSpec; break;
                    default:
                        break;
                }
            }
            catch
            {
            }
        }

        private uint GetCanReqID() => Convert.ToUInt32(CommSettings.CanReqID, 16);
        private uint GetCanResID() => Convert.ToUInt32(CommSettings.CanResID, 16);
        private uint GetNM_ReqID() => Convert.ToUInt32(CommSettings.NM_ReqID, 16);
        private uint GetNM_ResID() => Convert.ToUInt32(CommSettings.NM_ResID, 16);
        private uint GetPLightReqID() => Convert.ToUInt32(CommSettings.PLightReqID, 16);
        private uint GetPLightResID() => Convert.ToUInt32(CommSettings.PLightResID, 16);
        private uint GetXcpReqID() => Convert.ToUInt32(XCPAddress.ReqID, 16);
        private uint GetXcpResID() => Convert.ToUInt32(XCPAddress.ResID, 16);
        private byte GetXcpEcuAddr() => (byte)Convert.ToUInt32(XCPAddress.EcuAddr, 16);

        public string GetFileName()
        {
            return ProductInfo.PartNo + GSystem.JSON_EXT;
        }

        public uint GetNextSerialNumberCh1()
        {
            uint nextSerialNo = TestInfo.SerialNumCh1 = ++TestInfo.SerialNumTot;
            Save(GetFileName(), GSystem.SystemData.GeneralSettings.ProductFolder);
            return nextSerialNo;
        }

        public uint GetNextSerialNumber(int channel)
        {
            uint nextSerialNo;
            if (channel == GSystem.CH1)
                nextSerialNo = TestInfo.SerialNumCh1 = ++TestInfo.SerialNumTot;
            else
                nextSerialNo = TestInfo.SerialNumCh2 = ++TestInfo.SerialNumTot;
            Save(GetFileName(), GSystem.SystemData.GeneralSettings.ProductFolder);
            return nextSerialNo;
        }
    }
}

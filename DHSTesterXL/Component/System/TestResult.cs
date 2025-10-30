using GSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHSTesterXL
{
    public class TestResult
    {
        public bool Use { get; set; }
        public string Name { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Value { get; set; }
        public string Result { get; set; }
        public TestStates State { get; set; }

        public TestResult()
        {
            Use = false;
            Name = string.Empty;
            Min = string.Empty;
            Max = string.Empty;
            Value = string.Empty;
            Result = string.Empty;
            State = TestStates.Ready;
        }

        public void Init()
        {
            Use = false;
            Min = string.Empty;
            Max = string.Empty;
            Value = string.Empty;
            Result = string.Empty;
            State = TestStates.Ready;
        }
    }

    public class OveralTestResult
    {
        public SProductInfo ProductInfo { get; set; }
        public SCommSettings CommSettings { get; set; }
        public TestResult Short_1_2 { get; set; }
        public TestResult Short_1_3 { get; set; }
        public TestResult Short_1_4 { get; set; }
        public TestResult Short_1_5 { get; set; }
        public TestResult Short_1_6 { get; set; }
        public TestResult Short_2_3 { get; set; }
        public TestResult Short_2_4 { get; set; }
        public TestResult Short_2_5 { get; set; }
        public TestResult Short_2_6 { get; set; }
        public TestResult Short_3_4 { get; set; }
        public TestResult Short_3_5 { get; set; }
        public TestResult Short_3_6 { get; set; }
        public TestResult Short_4_5 { get; set; }
        public TestResult Short_4_6 { get; set; }
        public TestResult Short_5_6 { get; set; }
        public TestResult SerialNumber { get; set; }
        public TestResult DarkCurrent { get; set; }
        public TestResult PLightTurnOn { get; set; }
        public TestResult PLightCurrent { get; set; }
        public TestResult PLightAmbient { get; set; }
        public TestResult LockSen { get; set; }
        public TestResult LockCan { get; set; }
        public TestResult Cancel { get; set; }
        public TestResult SecurityBit { get; set; }
        public TestResult NFC { get; set; }
        public TestResult DTC_Erase { get; set; }
        public TestResult HW_Version { get; set; }
        public TestResult SW_Version { get; set; }
        public TestResult PartNumber { get; set; }
        public TestResult Bootloader { get; set; }
        public TestResult RXSWIN { get; set; }
        public TestResult Manufacture { get; set; }
        public TestResult SupplierCode { get; set; }
        public TestResult OperationCurrent { get; set; }

        public OveralTestResult()
        {
            ProductInfo = new SProductInfo();

            Short_1_2        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_2       ] };
            Short_1_3        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_3       ] };
            Short_1_4        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_4       ] };
            //Short_1_5        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_5       ] };
            Short_1_6        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_1_6       ] };
            Short_2_3        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_3       ] };
            Short_2_4        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_4       ] };
            //Short_2_5        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_5       ] };
            Short_2_6        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_2_6       ] };
            Short_3_4        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_4       ] };
            //Short_3_5        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_5       ] };
            Short_3_6        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_3_6       ] };
            //Short_4_5        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_4_5       ] };
            Short_4_6        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_4_6       ] };
            //Short_5_6        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Short_5_6       ] };
            SerialNumber     = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SerialNumber    ] };
            DarkCurrent      = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.DarkCurrent     ] };
            PLightTurnOn     = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.PLightTurnOn    ] };
            PLightCurrent    = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.PLightCurrent   ] };
            PLightAmbient    = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.PLightAmbient   ] };
            LockSen          = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.LockSen         ] };
            LockCan          = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.LockCan         ] };
            Cancel           = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Cancel          ] };
            SecurityBit      = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SecurityBit     ] };
            NFC              = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.NFC             ] };
            DTC_Erase        = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.DTC_Erase       ] };
            HW_Version       = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.HW_Version      ] };
            SW_Version       = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SW_Version      ] };
            PartNumber       = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.PartNumber      ] };
            Bootloader       = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Bootloader      ] };
            RXSWIN           = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.RXSWIN          ] };
            Manufacture      = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.Manufacture     ] };
            SupplierCode     = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.SupplierCode    ] };
            OperationCurrent = new TestResult() { Name = GDefines.TEST_ITEM_NAME_STR[(int)TestItems.OperationCurrent] };
        }

        public List<TestResult> GetEnableTestResultList()
        {
            List<TestResult> testResults = new List<TestResult>();
            if (CommSettings.CommType == "CAN" || CommSettings.CommType == "CAN FD")
            {
                for (int i = 0; i < (int)TestItems.Count; i++)
                {
                    switch ((TestItems)i)
                    {
                        case TestItems.Short_1_2        : if (Short_1_2       .Use) testResults.Add(Short_1_2       );   break;
                        case TestItems.Short_1_3        : if (Short_1_3       .Use) testResults.Add(Short_1_3       );   break;
                        case TestItems.Short_1_4        : if (Short_1_4       .Use) testResults.Add(Short_1_4       );   break;
                        case TestItems.Short_1_6        : if (Short_1_6       .Use) testResults.Add(Short_1_6       );   break;
                        case TestItems.Short_2_3        : if (Short_2_3       .Use) testResults.Add(Short_2_3       );   break;
                        case TestItems.Short_2_4        : if (Short_2_4       .Use) testResults.Add(Short_2_4       );   break;
                        case TestItems.Short_2_6        : if (Short_2_6       .Use) testResults.Add(Short_2_6       );   break;
                        case TestItems.Short_3_4        : if (Short_3_4       .Use) testResults.Add(Short_3_4       );   break;
                        case TestItems.Short_3_6        : if (Short_3_6       .Use) testResults.Add(Short_3_6       );   break;
                        case TestItems.Short_4_6        : if (Short_4_6       .Use) testResults.Add(Short_4_6       );   break;
                        case TestItems.SerialNumber     : if (SerialNumber    .Use) testResults.Add(SerialNumber    );   break;
                        case TestItems.DarkCurrent      : if (DarkCurrent     .Use) testResults.Add(DarkCurrent     );   break;
                        case TestItems.PLightTurnOn     : if (PLightTurnOn    .Use) testResults.Add(PLightTurnOn    );   break;
                        case TestItems.PLightCurrent    : if (PLightCurrent   .Use) testResults.Add(PLightCurrent   );   break;
                        case TestItems.PLightAmbient    : if (PLightAmbient   .Use) testResults.Add(PLightAmbient   );   break;
                        case TestItems.LockSen          : if (LockSen         .Use) testResults.Add(LockSen         );   break;
                        case TestItems.LockCan          : if (LockCan         .Use) testResults.Add(LockCan         );   break;
                        case TestItems.Cancel           : if (Cancel          .Use) testResults.Add(Cancel          );   break;
                        case TestItems.SecurityBit      : if (SecurityBit     .Use) testResults.Add(SecurityBit     );   break;
                        case TestItems.NFC              : if (NFC             .Use) testResults.Add(NFC             );   break;
                        case TestItems.DTC_Erase        : if (DTC_Erase       .Use) testResults.Add(DTC_Erase       );   break;
                        case TestItems.HW_Version       : if (HW_Version      .Use) testResults.Add(HW_Version      );   break;
                        case TestItems.SW_Version       : if (SW_Version      .Use) testResults.Add(SW_Version      );   break;
                        case TestItems.PartNumber       : if (PartNumber      .Use) testResults.Add(PartNumber      );   break;
                        case TestItems.Bootloader       : if (Bootloader      .Use) testResults.Add(Bootloader      );   break;
                        case TestItems.RXSWIN           : if (RXSWIN          .Use) testResults.Add(RXSWIN          );   break;
                        case TestItems.Manufacture      : if (Manufacture     .Use) testResults.Add(Manufacture     );   break;
                        case TestItems.SupplierCode     : if (SupplierCode    .Use) testResults.Add(SupplierCode    );   break;
                        case TestItems.OperationCurrent : if (OperationCurrent.Use) testResults.Add(OperationCurrent);   break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < (int)TouchOnlyTestItems.Count; i++)
                {
                    switch ((TouchOnlyTestItems)i)
                    {
                        case TouchOnlyTestItems.Short_1_2        : if (Short_1_2       .Use) testResults.Add(Short_1_2       );   break;
                        case TouchOnlyTestItems.Short_1_3        : if (Short_1_3       .Use) testResults.Add(Short_1_3       );   break;
                        case TouchOnlyTestItems.Short_1_4        : if (Short_1_4       .Use) testResults.Add(Short_1_4       );   break;
                        case TouchOnlyTestItems.Short_1_6        : if (Short_1_6       .Use) testResults.Add(Short_1_6       );   break;
                        case TouchOnlyTestItems.Short_2_3        : if (Short_2_3       .Use) testResults.Add(Short_2_3       );   break;
                        case TouchOnlyTestItems.Short_2_4        : if (Short_2_4       .Use) testResults.Add(Short_2_4       );   break;
                        case TouchOnlyTestItems.Short_2_6        : if (Short_2_6       .Use) testResults.Add(Short_2_6       );   break;
                        case TouchOnlyTestItems.Short_3_4        : if (Short_3_4       .Use) testResults.Add(Short_3_4       );   break;
                        case TouchOnlyTestItems.Short_3_6        : if (Short_3_6       .Use) testResults.Add(Short_3_6       );   break;
                        case TouchOnlyTestItems.Short_4_6        : if (Short_4_6       .Use) testResults.Add(Short_4_6       );   break;
                        case TouchOnlyTestItems.DarkCurrent      : if (DarkCurrent     .Use) testResults.Add(DarkCurrent     );   break;
                        case TouchOnlyTestItems.PLightTurnOn     : if (PLightTurnOn    .Use) testResults.Add(PLightTurnOn    );   break;
                        case TouchOnlyTestItems.PLightCurrent    : if (PLightCurrent   .Use) testResults.Add(PLightCurrent   );   break;
                        case TouchOnlyTestItems.PLightAmbient    : if (PLightAmbient   .Use) testResults.Add(PLightAmbient   );   break;
                        case TouchOnlyTestItems.Touch            : if (LockSen         .Use) testResults.Add(LockSen         );   break;
                        case TouchOnlyTestItems.Cancel           : if (Cancel          .Use) testResults.Add(Cancel          );   break;
                        case TouchOnlyTestItems.DTC_Erase        : if (DTC_Erase       .Use) testResults.Add(DTC_Erase       );   break;
                        case TouchOnlyTestItems.HW_Version       : if (HW_Version      .Use) testResults.Add(HW_Version      );   break;
                        case TouchOnlyTestItems.SW_Version       : if (SW_Version      .Use) testResults.Add(SW_Version      );   break;
                        case TouchOnlyTestItems.PartNumber       : if (PartNumber      .Use) testResults.Add(PartNumber      );   break;
                        case TouchOnlyTestItems.OperationCurrent : if (OperationCurrent.Use) testResults.Add(OperationCurrent);   break;
                        case TouchOnlyTestItems.SerialNumber     : if (SerialNumber    .Use) testResults.Add(SerialNumber    );   break;
                        default:
                            break;
                    }
                }
            }
            return testResults;
        }
    }
}

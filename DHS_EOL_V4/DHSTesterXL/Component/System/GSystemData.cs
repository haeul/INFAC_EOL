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
    public class GGeneralSettings
    {
        [JsonProperty("Theme Style")]
        public MetroFramework.MetroThemeStyle ThemeStyle { get; set; }
        [JsonProperty("Color Style")]
        public MetroFramework.MetroColorStyle ColorStyle { get; set; }
        public string Password { get; set; }
        public string ProductFolder { get; set; } = string.Empty;
        public string VFlashFolder { get; set; } = string.Empty;
        public string DataFolderAll { get; set; } = string.Empty;
        public string DataFolderPass { get; set; } = string.Empty;
        public string DataFolderBack { get; set; } = string.Empty;
        public string DataFolderRepeat { get; set; } = string.Empty;
        public string DataFolderCap { get; set; } = string.Empty;
        public string DataFolderDelta { get; set; } = string.Empty;
        public bool LogDelete { get; set; } = false;
        public int LogExpDate { get; set; } = 50;

        public GGeneralSettings()
        {
            ThemeStyle = MetroFramework.MetroThemeStyle.Light;
            ColorStyle = MetroFramework.MetroColorStyle.Orange;
            Password = "4780";
        }
    }

    public class GProductSettings
    {
        public string LastProductNo { get; set; }
        public GProductSettings()
        {
            LastProductNo = string.Empty;
        }
    }

    public class GDedicatedCtrlSettings
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public string ParityBit { get; set; }
        public int DataBit { get; set; }
        public int StopBit { get; set; }
        public int CommInterval { get; set; }
        public byte SlaveAddress { get; set; }

        public GDedicatedCtrlSettings()
        {
            PortName = "COM3";
            BaudRate = 9600;
            ParityBit = "None";
            DataBit = 8;
            StopBit = 1;
            CommInterval = 100;
            SlaveAddress = 2;
        }
    }

    public class GRelayModuleSettings
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public string ParityBit { get; set; }
        public int DataBit { get; set; }
        public int StopBit { get; set; }

        public GRelayModuleSettings()
        {
            PortName = "COM3";
            BaudRate = 9600;
            ParityBit = "None";
            DataBit = 8;
            StopBit = 1;
        }
    }
    
    public class GPLCSettings
    {
        public string IPAddress { get; set;}
        public int PortNumber { get; set; }
        public bool AutoConnect { get; set; }
        public string DeviceCode { get; set; }
        public int StartAddress { get; set; }
        
        public GPLCSettings()
        {
            IPAddress = "192.168.219.3";
            PortNumber = 13000;
            AutoConnect = false;
            DeviceCode = "D";
            StartAddress = 5000;
        }
    }

    public class GUserInfo
    {
        public int UserType { get; set; } // 0:operator, 1:Manager, 2:Maker
        public string UserName { get; set; }
        public string Password { get; set; }

        public GUserInfo()
        {

        }
    }

    public class GConnectorSettings
    {
        public uint MaxCount { get; set; }
        public uint WarnCount { get; set; }
        public uint UseCount { get; set; }
        public GConnectorSettings()
        {
            MaxCount = 20000;
            WarnCount = 19000;
            UseCount = 0;
        }
    }

    public class GSystemDataJson
    {
        [JsonProperty("General Settings")]
        public GGeneralSettings GeneralSettings { get; set; }
        public GProductSettings ProductSettings { get; set; }
        public GDedicatedCtrlSettings DedicatedCtrlSettings { get; set; }
        public GRelayModuleSettings RelayModuleSettings { get; set; }
        public GPLCSettings PLCSettings { get; set; }
        public GConnectorSettings ConnectorNFCTouch1Ch1 { get; set; }
        public GConnectorSettings ConnectorNFCTouch2Ch1 { get; set; }
        public GConnectorSettings ConnectorTouchOnlyCh1 { get; set; }
        public GConnectorSettings ConnectorNFCTouch1Ch2 { get; set; }
        public GConnectorSettings ConnectorNFCTouch2Ch2 { get; set; }
        public GConnectorSettings ConnectorTouchOnlyCh2 { get; set; }

        public GSystemDataJson()
        {
            GeneralSettings = new GGeneralSettings();
            ProductSettings = new GProductSettings();
            DedicatedCtrlSettings = new GDedicatedCtrlSettings();
            RelayModuleSettings = new GRelayModuleSettings();
            PLCSettings = new GPLCSettings();
            ConnectorNFCTouch1Ch1 = new GConnectorSettings();
            ConnectorNFCTouch2Ch1 = new GConnectorSettings();
            ConnectorTouchOnlyCh1 = new GConnectorSettings();
            ConnectorNFCTouch1Ch2 = new GConnectorSettings();
            ConnectorNFCTouch2Ch2 = new GConnectorSettings();
            ConnectorTouchOnlyCh2 = new GConnectorSettings();
        }
    }

    public class GSystemData : GSystemDataJson
    {
        private static GSystemData instance_ = null;

        public GSystemData()
        {

        }

        public static GSystemData GetInstance()
        {
            if (instance_ == null)
                instance_ = new GSystemData();

            return instance_;
        }

        public bool Load(string fileName = "SystemData.json", string filePath = @".\")
        {
            try
            {
                string filePathName = Path.Combine(filePath, fileName);

                FileInfo fi = new FileInfo(filePathName);

                if (fi.Exists)
                {
                    // 파일이 있으면 파일을 읽는다.
                    GSystemDataJson jsonData = JsonConvert.DeserializeObject<GSystemDataJson>(File.ReadAllText(filePathName));
                    GeneralSettings       = jsonData.GeneralSettings      ;
                    ProductSettings       = jsonData.ProductSettings      ;
                    DedicatedCtrlSettings = jsonData.DedicatedCtrlSettings;
                    RelayModuleSettings   = jsonData.RelayModuleSettings  ;
                    PLCSettings           = jsonData.PLCSettings          ;
                    ConnectorNFCTouch1Ch1 = jsonData.ConnectorNFCTouch1Ch1;
                    ConnectorNFCTouch2Ch1 = jsonData.ConnectorNFCTouch2Ch1;
                    ConnectorTouchOnlyCh1 = jsonData.ConnectorTouchOnlyCh1;
                    ConnectorNFCTouch1Ch2 = jsonData.ConnectorNFCTouch1Ch2;
                    ConnectorNFCTouch2Ch2 = jsonData.ConnectorNFCTouch2Ch2;
                    ConnectorTouchOnlyCh2 = jsonData.ConnectorTouchOnlyCh2;
                }
                else
                {
                    // 파일이 없으면 기본 값으로 파일을 생성한다.
                    GSystemDataJson jsonData = new GSystemDataJson();
                    jsonData.GeneralSettings       = GeneralSettings      ;
                    jsonData.ProductSettings       = ProductSettings      ;
                    jsonData.DedicatedCtrlSettings = DedicatedCtrlSettings;
                    jsonData.RelayModuleSettings   = RelayModuleSettings  ;
                    jsonData.PLCSettings           = PLCSettings          ;
                    jsonData.ConnectorNFCTouch1Ch1 = ConnectorNFCTouch1Ch1;
                    jsonData.ConnectorNFCTouch2Ch1 = ConnectorNFCTouch2Ch1;
                    jsonData.ConnectorTouchOnlyCh1 = ConnectorTouchOnlyCh1;
                    jsonData.ConnectorNFCTouch1Ch2 = ConnectorNFCTouch1Ch2;
                    jsonData.ConnectorNFCTouch2Ch2 = ConnectorNFCTouch2Ch2;
                    jsonData.ConnectorTouchOnlyCh2 = ConnectorTouchOnlyCh2;

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

        public bool Save(string fileName = "SystemData.json", string filePath = @".\")
        {
            try
            {
                string filePathName = Path.Combine(filePath, fileName);

                GSystemDataJson jsonData = new GSystemDataJson();
                jsonData.GeneralSettings       = GeneralSettings      ;
                jsonData.ProductSettings       = ProductSettings      ;
                jsonData.DedicatedCtrlSettings = DedicatedCtrlSettings;
                jsonData.RelayModuleSettings   = RelayModuleSettings  ;
                jsonData.PLCSettings           = PLCSettings          ;
                jsonData.ConnectorNFCTouch1Ch1 = ConnectorNFCTouch1Ch1;
                jsonData.ConnectorNFCTouch2Ch1 = ConnectorNFCTouch2Ch1;
                jsonData.ConnectorTouchOnlyCh1 = ConnectorTouchOnlyCh1;
                jsonData.ConnectorNFCTouch1Ch2 = ConnectorNFCTouch1Ch2;
                jsonData.ConnectorNFCTouch2Ch2 = ConnectorNFCTouch2Ch2;
                jsonData.ConnectorTouchOnlyCh2 = ConnectorTouchOnlyCh2;

                File.WriteAllText(filePathName, JsonConvert.SerializeObject(jsonData, Formatting.Indented));

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;
using static System.Windows.Forms.AxHost;

namespace DHSTesterXL
{
    public enum PLC_ReadRegister
    {
        Ch1_Status1  ,
        Ch1_Status2  ,
        Ch1_RecipeNo ,
        Ch1_NFC_Z_Pos,
        Ch1_State1   ,
        Ch1_State2   ,
        Ch1_ErrorCode,
        Ch1_NFC_Z_Recipe_Pos,
        Ch1_Reserved8,
        Ch1_Reserved9,
        Ch2_Status1  ,
        Ch2_Status2  ,
        Ch2_RecipeNo ,
        Ch2_NFC_Z_Pos,
        Ch2_State1   ,
        Ch2_State2   ,
        Ch2_ErrorCode,
        Ch2_NFC_Z_Recipe_Pos,
        Ch2_Reserved8,
        Ch2_Reserved9,
        Count
    }
    public enum PLC_WriteRegister
    {
        Ch1_Command1 ,
        Ch1_Command2 ,
        Ch1_RecipeNo ,
        Ch1_NFC_Z_Pos,
        Ch1_TowerLamp,
        Ch1_Reserved5,
        Ch1_Reserved6,
        Ch1_Reserved7,
        Ch1_Reserved8,
        Ch1_Reserved9,
        Ch2_Command1 ,
        Ch2_Command2 ,
        Ch2_RecipeNo ,
        Ch2_NFC_Z_Pos,
        Ch2_TowerLamp,
        Ch2_Reserved5,
        Ch2_Reserved6,
        Ch2_Reserved7,
        Ch2_Reserved8,
        Ch2_Reserved9,
        Count
    }

    public enum PLC_Command1_Bit
    {
        TestComplete = 0,
        LoadingStart,
        LoadingStop,
        YTouchMove,
        YTouchStop,
        YCancelMove,
        YCancelStop,
        YNFC_Move,
        YNFC_Stop,
        YLoadMove,
        YLoadStop,
        UnloadingStart,
        UnloadingStop,
        UnclampFore,
        UnclampBack,
        TestCancel,
    }
    public enum PLC_Command2_Bit
    {
        ChangeRecipe = 0,
        ZTouchDownStart,
        ZTouchDownStop,
        ZTouchUpStart,
        ZTouchUpStop,
        ZCancelDownStart,
        ZCancelDownStop,
        ZCancelUpStart,
        ZCancelUpStop,
        ZNFC_DownStart,
        ZNFC_DownStop,
        ZNFC_UpStart,
        ZNFC_UpStop,
        ZNFC_StepDown,
        ZInterlockIgnore,
        ErrorReset,
    }

    public enum PLC_State1_Bit
    {
        AutoManual = 0,
        LoadingY,
        LoadingZ,
        TouchY,
        TouchZ,
        CancelY,
        CancelZ,
        NFC_Y,
        NFC_Z,
        JigSensor,
        ConnSensor,
        JigCyl,
        ClampCyl,
    }

    public enum PLC_State2_Bit
    {
        ErrorEMS = 0,
        ErrorLoading,
        ErrorTouch,
        ErrorCancel,
        ErrorNFC,
        ErrorMoveLoading,
        ErrorUnloading,
        ErrorUnclamp,
        ErrorAreaSensor,
        ErrorDoorOpen,
        ErrorOccurred = 15,
    }

    //public class PLC_Data
    //{
    //    // 전저울 데이터
    //    public int D11400_rd { get; set; }
    //    public int D11410_rd { get; set; }
    //    public int D11420_rd { get; set; }
    //    public int D11430_rd { get; set; }
    //    public int D11440_rd { get; set; }
    //    public int D11450_rd { get; set; }
    //    public PLC_Data()
    //    {
    //        // 전저울 데이터
    //        D11400_rd = 0;
    //        D11410_rd = 0;
    //        D11420_rd = 0;
    //        D11430_rd = 0;
    //        D11440_rd = 0;
    //        D11450_rd = 0;
    //    }
    //}

    public class MitsubishiPLC
    {
        private static MitsubishiPLC _instance = null;
        private Socket _socket = null;
        private CancellationTokenSource _cts;
        private M0403_req _mc0403_req = new M0403_req();
        private M0403_res _mc0403_res = new M0403_res();
        private M1402_req _mc1402_req = new M1402_req();
        private M1402_res _mc1402_res = new M1402_res();
        byte[] _socketBytes = new byte[1024];

        private ConcurrentQueue<ModbusCommand> _commandQue = new ConcurrentQueue<ModbusCommand>();

        public bool IsConnect { get; set; } = false;
        public short Ch1_R_Status1   { get { return _mc0403_res.D5000; } }
        public short Ch1_R_Status2   { get { return _mc0403_res.D5001; } }
        public short Ch1_R_RecipeNo  { get { return _mc0403_res.D5002; } }
        public short Ch1_R_NFC_Z_Pos { get { return _mc0403_res.D5003; } }
        public short Ch1_R_State1    { get { return _mc0403_res.D5004; } }
        public short Ch1_R_State2    { get { return _mc0403_res.D5005; } }
        public short Ch1_R_ErrorCode { get { return _mc0403_res.D5006; } }
        public short Ch1_R_NFC_Z_RecipePos { get { return _mc0403_res.D5007; } }
        public short Ch1_R_Reserved8 { get { return _mc0403_res.D5008; } }
        public short Ch1_R_Reserved9 { get { return _mc0403_res.D5009; } }
        public short Ch2_R_Status1   { get { return _mc0403_res.D5010; } }
        public short Ch2_R_Status2   { get { return _mc0403_res.D5011; } }
        public short Ch2_R_RecipeNo  { get { return _mc0403_res.D5012; } }
        public short Ch2_R_NFC_Z_Pos { get { return _mc0403_res.D5013; } }
        public short Ch2_R_State1    { get { return _mc0403_res.D5014; } }
        public short Ch2_R_State2    { get { return _mc0403_res.D5015; } }
        public short Ch2_R_ErrorCode { get { return _mc0403_res.D5016; } }
        public short Ch2_R_NFC_Z_RecipePos { get { return _mc0403_res.D5017; } }
        public short Ch2_R_Reserved8 { get { return _mc0403_res.D5018; } }
        public short Ch2_R_Reserved9 { get { return _mc0403_res.D5019; } }

        public ushort Ch1_W_Command1  { get { return _mc1402_req.D5020dat; } set { _mc1402_req.D5020dat = value; } }
        public ushort Ch1_W_Command2  { get { return _mc1402_req.D5021dat; } set { _mc1402_req.D5021dat = value; } }
        public ushort Ch1_W_RecipeNo  { get { return _mc1402_req.D5022dat; } set { _mc1402_req.D5022dat = value; } }
        public ushort Ch1_W_NFC_Z_Pos { get { return _mc1402_req.D5023dat; } set { _mc1402_req.D5023dat = value; } }
        public ushort Ch1_W_TowerLamp { get { return _mc1402_req.D5024dat; } set { _mc1402_req.D5024dat = value; } }
        public ushort Ch1_W_Reserved5 { get { return _mc1402_req.D5025dat; } set { _mc1402_req.D5025dat = value; } }
        public ushort Ch1_W_Reserved6 { get { return _mc1402_req.D5026dat; } set { _mc1402_req.D5026dat = value; } }
        public ushort Ch1_W_Reserved7 { get { return _mc1402_req.D5027dat; } set { _mc1402_req.D5027dat = value; } }
        public ushort Ch1_W_Reserved8 { get { return _mc1402_req.D5028dat; } set { _mc1402_req.D5028dat = value; } }
        public ushort Ch1_W_Reserved9 { get { return _mc1402_req.D5029dat; } set { _mc1402_req.D5029dat = value; } }
        public ushort Ch2_W_Command1  { get { return _mc1402_req.D5030dat; } set { _mc1402_req.D5030dat = value; } }
        public ushort Ch2_W_Command2  { get { return _mc1402_req.D5031dat; } set { _mc1402_req.D5031dat = value; } }
        public ushort Ch2_W_RecipeNo  { get { return _mc1402_req.D5032dat; } set { _mc1402_req.D5032dat = value; } }
        public ushort Ch2_W_NFC_Z_Pos { get { return _mc1402_req.D5033dat; } set { _mc1402_req.D5033dat = value; } }
        public ushort Ch2_W_TowerLamp { get { return _mc1402_req.D5034dat; } set { _mc1402_req.D5034dat = value; } }
        public ushort Ch2_W_Reserved5 { get { return _mc1402_req.D5035dat; } set { _mc1402_req.D5035dat = value; } }
        public ushort Ch2_W_Reserved6 { get { return _mc1402_req.D5036dat; } set { _mc1402_req.D5036dat = value; } }
        public ushort Ch2_W_Reserved7 { get { return _mc1402_req.D5037dat; } set { _mc1402_req.D5037dat = value; } }
        public ushort Ch2_W_Reserved8 { get { return _mc1402_req.D5038dat; } set { _mc1402_req.D5038dat = value; } }
        public ushort Ch2_W_Reserved9 { get { return _mc1402_req.D5039dat; } set { _mc1402_req.D5039dat = value; } }

        /// <summary>
        /// PLC 데이터 처리 클래스 생성자
        /// </summary>
        public MitsubishiPLC()
        {

        }

        public static MitsubishiPLC GetInstance()
        {
            if (_instance == null)
                _instance = new MitsubishiPLC();
            return _instance;
        }

        public void Connect()
        {
            try
            {
                // Create TCP socket
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the server
                _socket.Connect(new IPEndPoint(IPAddress.Parse(GSystem.SystemData.PLCSettings.IPAddress), GSystem.SystemData.PLCSettings.PortNumber));
                _socket.ReceiveTimeout = 1000;

                // 비동기 통신
                StartAsync();

                GSystem.Logger.Info("PLC connect success!");

            }
            catch (SocketException ex)
            {
                GSystem.TraceMessage($"SocketException = {ex.Message}");
                GSystem.Logger.Info(ex.Message.ToString());

                _socket.Close();
                _socket.Dispose();
                _socket = null;

                MessageBox.Show(ex.Message.ToString());

                string msg = $"{ex.Message}\n네트워크 카드 또는 랜 케이블 등이 정상인지 확인한 후 다시 시도해 주세요.";
                string cap = $"Exception";
                MessageBox.Show(msg, cap, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_socket != null)
                {
                    StopAsync();

                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket.Dispose();
                    _socket= null;
                }
            }
            catch (SocketException ex)
            {
                GSystem.TraceMessage($"SocketException = {ex.Message}");
                GSystem.Logger.Info(ex.Message.ToString());
            }
        }

        // 백그라운드 작업의 예외가 앱을 종료시키는 것을 막기 위한 래퍼(wrapper) 메서드
        private async Task RunSafe(Task task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                // 의도된 취소이므로 조용히 종료
            }
            catch (Exception ex)
            {
                // 오류 발생 시 서비스 상태를 강제로 중지시킬 수도 있습니다.
                StopService();
                GSystem.TraceMessage(ex.ToString());
            }
        }

        // 중지 및 정리 로직을 별도 메서드로 분리하여 재사용
        private void StopService()
        {
            if (_cts != null)
            {
                _cts.Cancel(); // 1. 취소 신호 전송
                _cts.Dispose(); // 2. 리소스 해제
                _cts = null;    // 3. 상태 초기화
                IsConnect = false;
            }
        }

        public void StartAsync()
        {
            if (_cts != null) return;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            // await 없이 백그라운드에서 실행만 시킨다. (Fire and Forget)
            // 단, 이 경우 예외 처리를 위해 Task 내부에 try-catch를 넣는 것이 안전하다.
            Task.Run(() => RunSafe(PollingAsync(token)), token);
        }

        public void StopAsync()
        {
            StopService();
        }

        public async Task PollingAsync(CancellationToken token)
        {
            try
            {
                GSystem.Logger.Info("PLC's polling start...");
                GSystem.TraceMessage("PLC's polling start...");

                while (true)
                {
                    token.ThrowIfCancellationRequested(); // ThrowIfCancellationRequested가 더 깔끔합니다.
                    // MC0403_Req
                    M0403_Req_Proc();
                    await Task.Delay(100);
                }
            }
            catch (TaskCanceledException)
            {
                // Task.Delay가 취소될 때 TaskCanceledException이 발생합니다.
                // 정상적인 종료 과정이므로 조용히 처리합니다.
            }
            catch (Exception ex)
            {
                GSystem.Logger.Fatal($"{ex.Message}");
                GSystem.TraceMessage($"{ex.Message}");
            }
            finally
            {
                GSystem.Logger.Info("PLC's polling stopped.");
                GSystem.TraceMessage("PLC's polling stopped.");
            }
        }

        public void M0403_Req_Proc()
        {
            try
            {
                byte[] msg = new byte[1024];

                byte wordCount = 20;
                byte dwordCount = 0;

                _mc0403_req.Subhead     = 0x50;
                _mc0403_req.NetworkNo   = 0x00;
                _mc0403_req.PlcNo       = 0xFF;
                _mc0403_req.ReqModuleIO = 0x03FF;
                _mc0403_req.ReqModuleCh = 0x00;
                _mc0403_req.DataLength  = (short)(8 + ((wordCount + dwordCount) * 4));
                _mc0403_req.Response    = 0;
                _mc0403_req.Command     = 0x0403;
                _mc0403_req.SubCommand  = 0;
                _mc0403_req.WordCount   = wordCount;
                _mc0403_req.DWordCount  = dwordCount;

                MCDword mc_dword;

                // WORD 데이터
                // CH.1 (LEFT)
                mc_dword.Value = 5000; mc_dword.High.High = 0xA8; _mc0403_req.D5000 = mc_dword.Value;
                mc_dword.Value = 5001; mc_dword.High.High = 0xA8; _mc0403_req.D5001 = mc_dword.Value;
                mc_dword.Value = 5002; mc_dword.High.High = 0xA8; _mc0403_req.D5002 = mc_dword.Value;
                mc_dword.Value = 5003; mc_dword.High.High = 0xA8; _mc0403_req.D5003 = mc_dword.Value;
                mc_dword.Value = 5004; mc_dword.High.High = 0xA8; _mc0403_req.D5004 = mc_dword.Value;
                mc_dword.Value = 5005; mc_dword.High.High = 0xA8; _mc0403_req.D5005 = mc_dword.Value;
                mc_dword.Value = 5006; mc_dword.High.High = 0xA8; _mc0403_req.D5006 = mc_dword.Value;
                mc_dword.Value = 5007; mc_dword.High.High = 0xA8; _mc0403_req.D5007 = mc_dword.Value;
                mc_dword.Value = 5008; mc_dword.High.High = 0xA8; _mc0403_req.D5008 = mc_dword.Value;
                mc_dword.Value = 5009; mc_dword.High.High = 0xA8; _mc0403_req.D5009 = mc_dword.Value;
                // CH.2 (RIGHT)
                mc_dword.Value = 5010; mc_dword.High.High = 0xA8; _mc0403_req.D5010 = mc_dword.Value;
                mc_dword.Value = 5011; mc_dword.High.High = 0xA8; _mc0403_req.D5011 = mc_dword.Value;
                mc_dword.Value = 5012; mc_dword.High.High = 0xA8; _mc0403_req.D5012 = mc_dword.Value;
                mc_dword.Value = 5013; mc_dword.High.High = 0xA8; _mc0403_req.D5013 = mc_dword.Value;
                mc_dword.Value = 5014; mc_dword.High.High = 0xA8; _mc0403_req.D5014 = mc_dword.Value;
                mc_dword.Value = 5015; mc_dword.High.High = 0xA8; _mc0403_req.D5015 = mc_dword.Value;
                mc_dword.Value = 5016; mc_dword.High.High = 0xA8; _mc0403_req.D5016 = mc_dword.Value;
                mc_dword.Value = 5017; mc_dword.High.High = 0xA8; _mc0403_req.D5017 = mc_dword.Value;
                mc_dword.Value = 5018; mc_dword.High.High = 0xA8; _mc0403_req.D5018 = mc_dword.Value;
                mc_dword.Value = 5019; mc_dword.High.High = 0xA8; _mc0403_req.D5019 = mc_dword.Value;

                msg = _mc0403_req.Serialize();

                //GSystem.TraceMessage($"msg size    = {msg.Length}");

                int bytesSent = _socket.Send(msg);

                if (bytesSent <= 0)
                {
                    throw new SocketException();
                }

                int bytesRec = _socket.Receive(_socketBytes);

                //
                _mc0403_res.Deserialize(ref _socketBytes);

                //GSystem.TraceMessage($"rcv size    = {bytesRec                  }");
                //GSystem.TraceMessage($"Subhead     = {_mc0403_res.Subhead    :X02}");
                //GSystem.TraceMessage($"NetworkNo   = {_mc0403_res.NetworkNo  :X02}");
                //GSystem.TraceMessage($"PlcNo       = {_mc0403_res.PlcNo      :X02}");
                //GSystem.TraceMessage($"ReqModuleIO = {_mc0403_res.ReqModuleIO:X02}");
                //GSystem.TraceMessage($"ReqModuleCh = {_mc0403_res.ReqModuleCh:X02}");
                //GSystem.TraceMessage($"DataLength  = {_mc0403_res.DataLength :X02}");
                //GSystem.TraceMessage($"Response    = {_mc0403_res.Response   :X02}");
                //GSystem.TraceMessage($"D5000       = {_mc0403_res.D5000      :X02}");
                //GSystem.TraceMessage($"D5001       = {_mc0403_res.D5001      :X02}");
                //GSystem.TraceMessage($"D5002       = {_mc0403_res.D5002      :X02}");
                //GSystem.TraceMessage($"D5003       = {_mc0403_res.D5003      :X02}");
                //GSystem.TraceMessage($"D5004       = {_mc0403_res.D5004      :X02}");
                //GSystem.TraceMessage($"D5005       = {_mc0403_res.D5005      :X02}");
                //GSystem.TraceMessage($"D5006       = {_mc0403_res.D5006      :X02}");
                //GSystem.TraceMessage($"D5007       = {_mc0403_res.D5007      :X02}");
                //GSystem.TraceMessage($"D5008       = {_mc0403_res.D5008      :X02}");
                //GSystem.TraceMessage($"D5009       = {_mc0403_res.D5009      :X02}");
                //GSystem.TraceMessage($"D5010       = {_mc0403_res.D5010      :X02}");
                //GSystem.TraceMessage($"D5011       = {_mc0403_res.D5011      :X02}");
                //GSystem.TraceMessage($"D5012       = {_mc0403_res.D5012      :X02}");
                //GSystem.TraceMessage($"D5013       = {_mc0403_res.D5013      :X02}");
                //GSystem.TraceMessage($"D5014       = {_mc0403_res.D5014      :X02}");
                //GSystem.TraceMessage($"D5015       = {_mc0403_res.D5015      :X02}");
                //GSystem.TraceMessage($"D5016       = {_mc0403_res.D5016      :X02}");
                //GSystem.TraceMessage($"D5017       = {_mc0403_res.D5017      :X02}");
                //GSystem.TraceMessage($"D5018       = {_mc0403_res.D5018      :X02}");
                //GSystem.TraceMessage($"D5019       = {_mc0403_res.D5019      :X02}");

                if (_mc0403_res.Response == 0)
                {
                    // 정상 수신
                    //GSystem.TraceMessage("ERROR NONE!");

                    IsConnect = true;

                    //Set_trigger_data setTriggerData = new Set_trigger_data(SetTriggerData);
                    //setTriggerData(
                    //    mc0403_res.D11499,
                    //    mc0403_res.D11599,
                    //    mc0403_res.D11698,
                    //    mc0403_res.D11699,
                    //    mc0403_res.D11798,
                    //    mc0403_res.D11799
                    //    );
                }
                else
                {
                    // 에러 수신
                    GSystem.TraceMessage($"ERROR CODE = {_mc0403_res.Response}");
                }
            }
            catch (SocketException ex)
            {
                GSystem.TraceMessage($"SocketException = {ex.Message}");
                IsConnect = false;
            }
        }

        public void M1402_Req_Proc()
        {
            try
            {
                byte[] messageBytes = new byte[1024];

                byte wordCount = 20;
                byte dwordCount = 0;

                _mc1402_req.Subhead     = 0x50;
                _mc1402_req.NetworkNo   = 0x00;
                _mc1402_req.PlcNo       = 0xFF;
                _mc1402_req.ReqModuleIO = 0x03FF;
                _mc1402_req.ReqModuleCh = 0x00;
                _mc1402_req.DataLength  = (short)(8 + ((wordCount + dwordCount) * 6));
                _mc1402_req.Response    = 0;
                _mc1402_req.Command     = 0x1402;
                _mc1402_req.SubCommand  = 0;
                _mc1402_req.WordCount   = wordCount;
                _mc1402_req.DWordCount  = dwordCount;

                MCDword mc_dword;

                {
                    lock (this)

                    // WORD 데이터
                    mc_dword.Value = 5020; mc_dword.High.High = 0xA8; _mc1402_req.D5020dev = mc_dword.Value;
                    mc_dword.Value = 5021; mc_dword.High.High = 0xA8; _mc1402_req.D5021dev = mc_dword.Value;
                    mc_dword.Value = 5022; mc_dword.High.High = 0xA8; _mc1402_req.D5022dev = mc_dword.Value;
                    mc_dword.Value = 5023; mc_dword.High.High = 0xA8; _mc1402_req.D5023dev = mc_dword.Value;
                    mc_dword.Value = 5024; mc_dword.High.High = 0xA8; _mc1402_req.D5024dev = mc_dword.Value;
                    mc_dword.Value = 5025; mc_dword.High.High = 0xA8; _mc1402_req.D5025dev = mc_dword.Value;
                    mc_dword.Value = 5026; mc_dword.High.High = 0xA8; _mc1402_req.D5026dev = mc_dword.Value;
                    mc_dword.Value = 5027; mc_dword.High.High = 0xA8; _mc1402_req.D5027dev = mc_dword.Value;
                    mc_dword.Value = 5028; mc_dword.High.High = 0xA8; _mc1402_req.D5028dev = mc_dword.Value;
                    mc_dword.Value = 5029; mc_dword.High.High = 0xA8; _mc1402_req.D5029dev = mc_dword.Value;
                    mc_dword.Value = 5030; mc_dword.High.High = 0xA8; _mc1402_req.D5030dev = mc_dword.Value;
                    mc_dword.Value = 5031; mc_dword.High.High = 0xA8; _mc1402_req.D5031dev = mc_dword.Value;
                    mc_dword.Value = 5032; mc_dword.High.High = 0xA8; _mc1402_req.D5032dev = mc_dword.Value;
                    mc_dword.Value = 5033; mc_dword.High.High = 0xA8; _mc1402_req.D5033dev = mc_dword.Value;
                    mc_dword.Value = 5034; mc_dword.High.High = 0xA8; _mc1402_req.D5034dev = mc_dword.Value;
                    mc_dword.Value = 5035; mc_dword.High.High = 0xA8; _mc1402_req.D5035dev = mc_dword.Value;
                    mc_dword.Value = 5036; mc_dword.High.High = 0xA8; _mc1402_req.D5036dev = mc_dword.Value;
                    mc_dword.Value = 5037; mc_dword.High.High = 0xA8; _mc1402_req.D5037dev = mc_dword.Value;
                    mc_dword.Value = 5038; mc_dword.High.High = 0xA8; _mc1402_req.D5038dev = mc_dword.Value;
                    mc_dword.Value = 5039; mc_dword.High.High = 0xA8; _mc1402_req.D5039dev = mc_dword.Value;

                    // DWORD 데이터
                }

                messageBytes = _mc1402_req.Serialize();

                //GSystem.TraceMessage($"msg size    = {messageBytes.Length}");

                int bytesSent = _socket.Send(messageBytes);

                if (bytesSent <= 0)
                {
                    throw new SocketException();
                }

                //bytes = null;
                int bytesRec = _socket.Receive(_socketBytes);

                //GSystem.TraceMessage($"rcv size    = {bytesRec}");

                //
                _mc1402_res.Deserialize(ref _socketBytes);

                //GSystem.TraceMessage($"Subhead     = {_mc1402_res.Subhead    :X02}");
                //GSystem.TraceMessage($"NetworkNo   = {_mc1402_res.NetworkNo  :X02}");
                //GSystem.TraceMessage($"PlcNo       = {_mc1402_res.PlcNo      :X02}");
                //GSystem.TraceMessage($"ReqModuleIO = {_mc1402_res.ReqModuleIO:X02}");
                //GSystem.TraceMessage($"ReqModuleCh = {_mc1402_res.ReqModuleCh:X02}");
                //GSystem.TraceMessage($"DataLength  = {_mc1402_res.DataLength :X02}");
                //GSystem.TraceMessage($"Response    = {_mc1402_res.Response   :X02}");

                if (_mc1402_res.Response == 0)
                {
                    // 정상 수신
                    //GSystem.TraceMessage("ERROR NONE!");

                    IsConnect = true;
                }
                else
                {
                    // 에러 수신
                    GSystem.TraceMessage($"ERROR CODE = {_mc1402_res.Response:X04}");
                }
            }
            catch (SocketException ex)
            {
                GSystem.TraceMessage("SocketException = " + ex.Message.ToString());

                IsConnect  = false;
            }
        }

        public ushort GetState1(int channel)
        {
            if (channel == 0)
                return (ushort)Ch1_R_State1;
            else
                return (ushort)Ch2_R_State1;
        }

        public bool GetState1(int channel, int bitIndex)
        {
            if (channel == 0)
            {
                if ((Ch1_R_State1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_State1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public ushort GetState2(int channel)
        {
            if (channel == 0)
                return (ushort)Ch1_R_State2;
            else
                return (ushort)Ch2_R_State2;
        }

        public bool GetState2(int channel, int bitIndex)
        {
            if (channel == 0)
            {
                if ((Ch1_R_State2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_State2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }


        public bool GetAutoTestStart(int channel)
        {
            int bitIndex = 0;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetAutoTestComplete(int channel)
        {
            int bitIndex = 0;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetAutoTestComplete(int channel, bool state)
        {
            int bitIndex = 0;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetMoveLoadStart(int channel)
        {
            int bitIndex = 9;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetMoveLoadStart(int channel, bool state)
        {
            int bitIndex = 9;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetMoveLoadComplete(int channel)
        {
            int bitIndex = 10;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetLoadingStart(int channel)
        {
            int bitIndex = 1;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetLoadingStart(int channel, bool state)
        {
            int bitIndex = 1;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetLoadingComplete(int channel)
        {
            int bitIndex = 2;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetMoveTouchYStart(int channel)
        {
            int bitIndex = 3;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetMoveTouchYStart(int channel, bool state)
        {
            int bitIndex = 3;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetMoveTouchYComplete(int channel)
        {
            int bitIndex = 4;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetTouchZDownStart(int channel)
        {
            int bitIndex = 1;
            if (channel == 0)
            {
                if ((Ch1_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetTouchZDownStart(int channel, bool state)
        {
            int bitIndex = 1;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetTouchZDownComplete(int channel)
        {
            int bitIndex = 2;
            if (channel == 0)
            {
                if ((Ch1_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetTouchZUpStart(int channel)
        {
            int bitIndex = 3;
            if (channel == 0)
            {
                if ((Ch1_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetTouchZUpStart(int channel, bool state)
        {
            int bitIndex = 3;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetTouchZUpComplete(int channel)
        {
            int bitIndex = 4;
            if (channel == 0)
            {
                if ((Ch1_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetMoveCancelYStart(int channel)
        {
            int bitIndex = 5;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetMoveCancelYStart(int channel, bool state)
        {
            int bitIndex = 5;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetMoveCancelYComplete(int channel)
        {
            int bitIndex = 6;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetCancelZDownStart(int channel)
        {
            int bitIndex = 5;
            if (channel == 0)
            {
                if ((Ch1_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetCancelZDownStart(int channel, bool state)
        {
            int bitIndex = 5;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetCancelZDownComplete(int channel)
        {
            int bitIndex = 6;
            if (channel == 0)
            {
                if ((Ch1_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetCancelZUpStart(int channel)
        {
            int bitIndex = 7;
            if (channel == 0)
            {
                if ((Ch1_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetCancelZUpStart(int channel, bool state)
        {
            int bitIndex = 7;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetCancelZUpComplete(int channel)
        {
            int bitIndex = 8;
            if (channel == 0)
            {
                if ((Ch1_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetMoveNFCYStart(int channel)
        {
            int bitIndex = 7;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public void SetMoveNFCYStart(int channel, bool state)
        {
            int bitIndex = 7;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetMoveNFCYComplete(int channel)
        {
            int bitIndex = 8;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetNFCZDownStart(int channel)
        {
            int bitIndex = 9;
            if (channel == 0)
            {
                if ((Ch1_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetNFCZDownStart(int channel, bool state)
        {
            int bitIndex = 9;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetNFCZDownComplete(int channel)
        {
            int bitIndex = 10;
            if (channel == 0)
            {
                if ((Ch1_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetNFC_Z_PositionStart(int channel)
        {
            int bitIndex = 13;
            if (channel == 0)
            {
                if ((Ch1_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetNFC_Z_PositionStart(int channel, bool state)
        {
            int bitIndex = 13;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetNFC_Z_PositionComplete(int channel)
        {
            int bitIndex = 14;
            if (channel == 0)
            {
                if ((Ch1_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetNFCZUpStart(int channel)
        {
            int bitIndex = 11;
            if (channel == 0)
            {
                if ((Ch1_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetNFCZUpStart(int channel, bool state)
        {
            int bitIndex = 11;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetNFCZUpComplete(int channel)
        {
            int bitIndex = 12;
            if (channel == 0)
            {
                if ((Ch1_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetUnloadingStart(int channel)
        {
            int bitIndex = 11;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetUnloadingStart(int channel, bool state)
        {
            int bitIndex = 11;
            if (channel == 0)
            {
                if (state)
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (state)
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public bool GetUnloadingComplete(int channel)
        {
            int bitIndex = 12;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetUnclampForeStart(int channel)
        {
            int bitIndex = (int)PLC_Command1_Bit.UnclampFore;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetUnclampForeStart(int channel, bool state)
        {
            int bitIndex = (int)PLC_Command1_Bit.UnclampFore;
            if (channel == 0)
            {
                if (state)
                {
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[(int)PLC_Command1_Bit.UnclampBack];
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                }
                else
                {
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
                }
            }
            else
            {
                if (state)
                {
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[(int)PLC_Command1_Bit.UnclampBack];
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                }
                else
                {
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
                }
            }
            M1402_Req_Proc();
        }

        public bool GetUnclampForeComplete(int channel)
        {
            int bitIndex = (int)PLC_Command1_Bit.UnclampFore;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetUnclampBackStart(int channel)
        {
            int bitIndex = (int)PLC_Command1_Bit.UnclampBack;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }

        public void SetUnclampBackStart(int channel, bool state)
        {
            int bitIndex = (int)PLC_Command1_Bit.UnclampBack;
            if (channel == 0)
            {
                if (state)
                {
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[(int)PLC_Command1_Bit.UnclampFore];
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                }
                else
                {
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
                }
            }
            else
            {
                if (state)
                {
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[(int)PLC_Command1_Bit.UnclampFore];
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                }
                else
                {
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
                }
            }
            M1402_Req_Proc();
        }

        public bool GetUnclampBackComplete(int channel)
        {
            int bitIndex = (int)PLC_Command1_Bit.UnclampBack;
            if (channel == 0)
            {
                if ((Ch1_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public int GetCurrentNFC_Z_Position(int channel)
        {
            if (channel == 0)
                return Ch1_R_NFC_Z_Pos;
            else
                return Ch2_R_NFC_Z_Pos;
        }

        public void SetZInterlockIgnore(int channel, bool interlock)
        {
            int bitIndex = 14;
            if (channel == 0)
            {
                if (interlock)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (interlock)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }

        public void SetErrorTowerLamp(int channel, bool towerLamp)
        {
            if (channel == 0)
            {
                if (towerLamp)
                    Ch1_W_TowerLamp = 1;
                else
                    Ch1_W_TowerLamp = 0;
            }
            else
            {
                if (towerLamp)
                    Ch2_W_TowerLamp = 1;
                else
                    Ch2_W_TowerLamp = 0;
            }
            M1402_Req_Proc();
        }

        public bool GetErrorResetStart(int channel)
        {
            int bitIndex = 15;
            if (channel == 0)
            {
                if ((Ch1_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }
        public void SetErrorResetStart(int channel, bool errorReset)
        {
            int bitIndex = 15;
            if (channel == 0)
            {
                if (errorReset)
                    Ch1_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (errorReset)
                    Ch2_W_Command2 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command2 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }
        public bool GetErrorResetComplete(int channel)
        {
            int bitIndex = 15;
            if (channel == 0)
            {
                if ((Ch1_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            else
            {
                if ((Ch2_R_Status2 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
            }
            return false;
        }

        public bool GetMeasureCompleteStart(int channel)
        {
            int bitIndex = 0;
            if (channel == 0)
            {
                if ((Ch1_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
            else
            {
                if ((Ch2_W_Command1 & GDefines.BIT16[bitIndex]) == GDefines.BIT16[bitIndex])
                    return true;
                else
                    return false;
            }
        }
        public void SetMeasureCompleteStart(int channel, bool complete)
        {
            int bitIndex = 0;
            if (channel == 0)
            {
                if (complete)
                    Ch1_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch1_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            else
            {
                if (complete)
                    Ch2_W_Command1 |= GDefines.BIT16[bitIndex];
                else
                    Ch2_W_Command1 &= (ushort)~GDefines.BIT16[bitIndex];
            }
            M1402_Req_Proc();
        }
    }
}

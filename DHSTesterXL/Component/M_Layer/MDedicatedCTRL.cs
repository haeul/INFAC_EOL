using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DHSTesterXL
{
    public enum DedicatedCommandBit
    {
        TestInit = 0,
        ShortTest,
        ActivePowerOn,
        LightOn,
        SensorModel = 8,
        AverageTime = 10,
    }
    public enum DedicatedSensorConnector
    {
        SensorOff = 0,
        Sensor_JG_NFC,
        Sensor_JG_Only,
        Sensor_NH2_NFC,
        Sensor_NH2_Only,
        Count
    }
    public enum DedicatedExtInputBit
    {
        StartSwitchCh1 = 0,
        StartSwitchCh2 = 2,
    }
    public enum DedicatedExtOutputBit
    {
        StartLampCh1 = 0,
        StartLampCh2 = 2,
    }

    public enum DedicatedChannels
    {
        Ch1,
        Ch2,
        Count
    }

    public class ModbusCommand
    {
        public byte FunctionCode { get; set; }
        public ushort StartAddress { get; set; }
        public ushort RegisterCount { get; set; }

        public ModbusCommand(byte funcCode, ushort startAddr, ushort regCount)
        {
            FunctionCode = funcCode;
            StartAddress = startAddr;
            RegisterCount = regCount;
        }
    }


    public class MDedicatedCTRL : HModbusRTU
    {
        #region *** Register Address
        // FC03h-Ch1
        public const int ADDR_03H_CH1_RESPONSE    =  0;
        public const int ADDR_03H_CH1_COMPLETE    =  1;
        public const int ADDR_03H_CH1_CONN_TYPE   =  2;
        public const int ADDR_03H_CH1_SHORT_1_2   =  3;
        public const int ADDR_03H_CH1_SHORT_1_3   =  4;
        public const int ADDR_03H_CH1_SHORT_1_4   =  5;
        public const int ADDR_03H_CH1_SHORT_1_5   =  6;
        public const int ADDR_03H_CH1_SHORT_1_6   =  7;
        public const int ADDR_03H_CH1_SHORT_2_3   =  8;
        public const int ADDR_03H_CH1_SHORT_2_4   =  9;
        public const int ADDR_03H_CH1_SHORT_2_5   = 10;
        public const int ADDR_03H_CH1_SHORT_2_6   = 11;
        public const int ADDR_03H_CH1_SHORT_3_4   = 12;
        public const int ADDR_03H_CH1_SHORT_3_5   = 13;
        public const int ADDR_03H_CH1_SHORT_3_6   = 14;
        public const int ADDR_03H_CH1_SHORT_4_5   = 15;
        public const int ADDR_03H_CH1_SHORT_4_6   = 16;
        public const int ADDR_03H_CH1_SHORT_5_6   = 17;
        public const int ADDR_03H_CH1_CURRENT_LO  = 18;
        public const int ADDR_03H_CH1_CURRENT_HI  = 19;
        public const int ADDR_03H_CH1_LIGHT_LUX   = 20;
        public const int ADDR_03H_CH1_AVA_TIME    = 21;
        public const int ADDR_03H_CH1_LOCK_SIGNAL = 22;
        // FC03h-Ch2
        public const int ADDR_03H_CH2_RESPONSE    = 24;
        public const int ADDR_03H_CH2_COMPLETE    = 25;
        public const int ADDR_03H_CH2_CONN_TYPE   = 26;
        public const int ADDR_03H_CH2_SHORT_1_2   = 27;
        public const int ADDR_03H_CH2_SHORT_1_3   = 28;
        public const int ADDR_03H_CH2_SHORT_1_4   = 29;
        public const int ADDR_03H_CH2_SHORT_1_5   = 30;
        public const int ADDR_03H_CH2_SHORT_1_6   = 31;
        public const int ADDR_03H_CH2_SHORT_2_3   = 32;
        public const int ADDR_03H_CH2_SHORT_2_4   = 33;
        public const int ADDR_03H_CH2_SHORT_2_5   = 34;
        public const int ADDR_03H_CH2_SHORT_2_6   = 35;
        public const int ADDR_03H_CH2_SHORT_3_4   = 36;
        public const int ADDR_03H_CH2_SHORT_3_5   = 37;
        public const int ADDR_03H_CH2_SHORT_3_6   = 38;
        public const int ADDR_03H_CH2_SHORT_4_5   = 39;
        public const int ADDR_03H_CH2_SHORT_4_6   = 40;
        public const int ADDR_03H_CH2_SHORT_5_6   = 41;
        public const int ADDR_03H_CH2_CURRENT_LO  = 42;
        public const int ADDR_03H_CH2_CURRENT_HI  = 43;
        public const int ADDR_03H_CH2_LIGHT_LUX   = 44;
        public const int ADDR_03H_CH2_AVA_TIME    = 45;
        public const int ADDR_03H_CH2_LOCK_SIGNAL = 46;
        // FC03h-Common
        public const int ADDR_03H_EXT_INPUT       = 47;
        public const int ADDR_03H_EXT_OUTPUT      = 48;
        public const int ADDR_03H_ALIVE           = 49;
        // FC10h-Ch1
        public const int ADDR_10H_CH1_COMMAND     = 50;
        public const int ADDR_10H_CH1_CONN_TYPE   = 51;
        public const int ADDR_10H_CH1_SHORT_TEST  = 52;
        public const int ADDR_10H_CH1_AVG_TIME    = 53;
        // FC10h-Ch1
        public const int ADDR_10H_CH2_COMMAND     = 55;
        public const int ADDR_10H_CH2_CONN_TYPE   = 56;
        public const int ADDR_10H_CH2_SHORT_TEST  = 57;
        public const int ADDR_10H_CH2_AVG_TIME    = 58;
        // FC10h-Common
        public const int ADDR_10H_EXT_OUTPUT      = 60;

        #endregion *** Register Address
        // FC03h-Ch1
        public ushort Reg_03h_ch1_response    { get { return GetRegisterValue(ADDR_03H_CH1_RESPONSE   ); } set { SetRegisterValue(ADDR_03H_CH1_RESPONSE  , value ); } }
        public ushort Reg_03h_ch1_complete    { get { return GetRegisterValue(ADDR_03H_CH1_COMPLETE   ); } set { SetRegisterValue(ADDR_03H_CH1_COMPLETE  , value ); } }
        public ushort Reg_03h_ch1_conn_type   { get { return GetRegisterValue(ADDR_03H_CH1_CONN_TYPE  ); } set { SetRegisterValue(ADDR_03H_CH1_CONN_TYPE , value ); } }
        public ushort Reg_03h_ch1_short_1_2   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_1_2  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_1_2 , value ); } }
        public ushort Reg_03h_ch1_short_1_3   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_1_3  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_1_3 , value ); } }
        public ushort Reg_03h_ch1_short_1_4   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_1_4  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_1_4 , value ); } }
        public ushort Reg_03h_ch1_short_1_5   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_1_5  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_1_5 , value ); } }
        public ushort Reg_03h_ch1_short_1_6   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_1_6  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_1_6 , value ); } }
        public ushort Reg_03h_ch1_short_2_3   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_2_3  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_2_3 , value ); } }
        public ushort Reg_03h_ch1_short_2_4   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_2_4  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_2_4 , value ); } }
        public ushort Reg_03h_ch1_short_2_5   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_2_5  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_2_5 , value ); } }
        public ushort Reg_03h_ch1_short_2_6   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_2_6  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_2_6 , value ); } }
        public ushort Reg_03h_ch1_short_3_4   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_3_4  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_3_4 , value ); } }
        public ushort Reg_03h_ch1_short_3_5   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_3_5  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_3_5 , value ); } }
        public ushort Reg_03h_ch1_short_3_6   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_3_6  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_3_6 , value ); } }
        public ushort Reg_03h_ch1_short_4_5   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_4_5  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_4_5 , value ); } }
        public ushort Reg_03h_ch1_short_4_6   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_4_6  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_4_6 , value ); } }
        public ushort Reg_03h_ch1_short_5_6   { get { return GetRegisterValue(ADDR_03H_CH1_SHORT_5_6  ); } set { SetRegisterValue(ADDR_03H_CH1_SHORT_5_6 , value ); } }
        public ushort Reg_03h_ch1_current_lo  { get { return GetRegisterValue(ADDR_03H_CH1_CURRENT_LO ); } set { SetRegisterValue(ADDR_03H_CH1_CURRENT_LO, value ); } }
        public ushort Reg_03h_ch1_current_hi  { get { return GetRegisterValue(ADDR_03H_CH1_CURRENT_HI ); } set { SetRegisterValue(ADDR_03H_CH1_CURRENT_HI, value ); } }
        public ushort Reg_03h_ch1_light_lux   { get { return GetRegisterValue(ADDR_03H_CH1_LIGHT_LUX  ); } set { SetRegisterValue(ADDR_03H_CH1_LIGHT_LUX , value ); } }
        public ushort Reg_03h_ch1_ava_time    { get { return GetRegisterValue(ADDR_03H_CH1_AVA_TIME   ); } set { SetRegisterValue(ADDR_03H_CH1_AVA_TIME  , value ); } }
        public ushort Reg_03h_ch1_lock_signal { get { return GetRegisterValue(ADDR_03H_CH1_LOCK_SIGNAL); } set { SetRegisterValue(ADDR_03H_CH1_LOCK_SIGNAL, value); } }
        // FC03h-Ch2
        public ushort Reg_03h_ch2_response    { get { return GetRegisterValue(ADDR_03H_CH2_RESPONSE   ); } set { SetRegisterValue(ADDR_03H_CH2_RESPONSE  , value ); } }
        public ushort Reg_03h_ch2_complete    { get { return GetRegisterValue(ADDR_03H_CH2_COMPLETE   ); } set { SetRegisterValue(ADDR_03H_CH2_COMPLETE  , value ); } }
        public ushort Reg_03h_ch2_conn_type   { get { return GetRegisterValue(ADDR_03H_CH2_CONN_TYPE  ); } set { SetRegisterValue(ADDR_03H_CH2_CONN_TYPE , value ); } }
        public ushort Reg_03h_ch2_short_1_2   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_1_2  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_1_2 , value ); } }
        public ushort Reg_03h_ch2_short_1_3   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_1_3  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_1_3 , value ); } }
        public ushort Reg_03h_ch2_short_1_4   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_1_4  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_1_4 , value ); } }
        public ushort Reg_03h_ch2_short_1_5   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_1_5  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_1_5 , value ); } }
        public ushort Reg_03h_ch2_short_1_6   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_1_6  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_1_6 , value ); } }
        public ushort Reg_03h_ch2_short_2_3   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_2_3  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_2_3 , value ); } }
        public ushort Reg_03h_ch2_short_2_4   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_2_4  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_2_4 , value ); } }
        public ushort Reg_03h_ch2_short_2_5   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_2_5  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_2_5 , value ); } }
        public ushort Reg_03h_ch2_short_2_6   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_2_6  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_2_6 , value ); } }
        public ushort Reg_03h_ch2_short_3_4   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_3_4  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_3_4 , value ); } }
        public ushort Reg_03h_ch2_short_3_5   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_3_5  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_3_5 , value ); } }
        public ushort Reg_03h_ch2_short_3_6   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_3_6  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_3_6 , value ); } }
        public ushort Reg_03h_ch2_short_4_5   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_4_5  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_4_5 , value ); } }
        public ushort Reg_03h_ch2_short_4_6   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_4_6  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_4_6 , value ); } }
        public ushort Reg_03h_ch2_short_5_6   { get { return GetRegisterValue(ADDR_03H_CH2_SHORT_5_6  ); } set { SetRegisterValue(ADDR_03H_CH2_SHORT_5_6 , value ); } }
        public ushort Reg_03h_ch2_current_lo  { get { return GetRegisterValue(ADDR_03H_CH2_CURRENT_LO ); } set { SetRegisterValue(ADDR_03H_CH2_CURRENT_LO, value ); } }
        public ushort Reg_03h_ch2_current_hi  { get { return GetRegisterValue(ADDR_03H_CH2_CURRENT_HI ); } set { SetRegisterValue(ADDR_03H_CH2_CURRENT_HI, value ); } }
        public ushort Reg_03h_ch2_light_lux   { get { return GetRegisterValue(ADDR_03H_CH2_LIGHT_LUX  ); } set { SetRegisterValue(ADDR_03H_CH2_LIGHT_LUX , value ); } }
        public ushort Reg_03h_ch2_ava_time    { get { return GetRegisterValue(ADDR_03H_CH2_AVA_TIME   ); } set { SetRegisterValue(ADDR_03H_CH2_AVA_TIME  , value ); } }
        public ushort Reg_03h_ch2_lock_signal { get { return GetRegisterValue(ADDR_03H_CH2_LOCK_SIGNAL); } set { SetRegisterValue(ADDR_03H_CH2_LOCK_SIGNAL, value); } }
        // FC03h-Common
        public ushort Reg_03h_ext_input       { get { return GetRegisterValue(ADDR_03H_EXT_INPUT      ); } set { SetRegisterValue(ADDR_03H_EXT_INPUT     , value ); } }
        public ushort Reg_03h_ext_outpu       { get { return GetRegisterValue(ADDR_03H_EXT_OUTPUT     ); } set { SetRegisterValue(ADDR_03H_EXT_OUTPUT    , value ); } }
        public ushort Reg_03h_alive           { get { return GetRegisterValue(ADDR_03H_ALIVE          ); } set { SetRegisterValue(ADDR_03H_ALIVE         , value ); } }
        // FC10h-Ch1
        public ushort Reg_10h_ch1_command     { get { return GetRegisterValue(ADDR_10H_CH1_COMMAND    ); } set { SetRegisterValue(ADDR_10H_CH1_COMMAND   , value ); } }
        public ushort Reg_10h_ch1_conn_type   { get { return GetRegisterValue(ADDR_10H_CH1_CONN_TYPE  ); } set { SetRegisterValue(ADDR_10H_CH1_CONN_TYPE , value ); } }
        public ushort Reg_10h_ch1_short_test  { get { return GetRegisterValue(ADDR_10H_CH1_SHORT_TEST ); } set { SetRegisterValue(ADDR_10H_CH1_SHORT_TEST, value ); } }
        public ushort Reg_10h_ch1_avg_time    { get { return GetRegisterValue(ADDR_10H_CH1_AVG_TIME   ); } set { SetRegisterValue(ADDR_10H_CH1_AVG_TIME  , value ); } }
        // FC10h-Ch1
        public ushort Reg_10h_ch2_command     { get { return GetRegisterValue(ADDR_10H_CH2_COMMAND    ); } set { SetRegisterValue(ADDR_10H_CH2_COMMAND   , value ); } }
        public ushort Reg_10h_ch2_conn_type   { get { return GetRegisterValue(ADDR_10H_CH2_CONN_TYPE  ); } set { SetRegisterValue(ADDR_10H_CH2_CONN_TYPE , value ); } }
        public ushort Reg_10h_ch2_short_test  { get { return GetRegisterValue(ADDR_10H_CH2_SHORT_TEST ); } set { SetRegisterValue(ADDR_10H_CH2_SHORT_TEST, value ); } }
        public ushort Reg_10h_ch2_avg_time    { get { return GetRegisterValue(ADDR_10H_CH2_AVG_TIME   ); } set { SetRegisterValue(ADDR_10H_CH2_AVG_TIME  , value ); } }
        // FC10h-Common
        public ushort Reg_10h_ext_output      { get { return GetRegisterValue(ADDR_10H_EXT_OUTPUT     ); } set { SetRegisterValue(ADDR_10H_EXT_OUTPUT    , value ); } }


        private byte _slaveAddress = 1;
        private ushort _startAddress03h = 0;
        private ushort _startAddress10h = 50;
        private ushort _registerCount03h = 50;
        private ushort _registerCount10h = 11;
        private int _commInterval = 100;

        private ushort[] _registerDatas = new ushort[MAX_REGISTER_COUNT];
        private ushort[] _receivedDatas = new ushort[MAX_REGISTER_COUNT];

        private ConcurrentQueue<ModbusCommand> _commandQue = new ConcurrentQueue<ModbusCommand>();


        public static readonly int MAX_REGISTER_COUNT = 64;
        public byte SlaveAddress { get { return _slaveAddress; } set { _slaveAddress = value; } }
        public ushort StartAddress03h { get { return _startAddress03h; } set { _startAddress03h = value; } }
        public ushort StartAddress10h { get { return _startAddress10h; } set { _startAddress10h = value; } }
        public ushort RegisterCount03h { get { return _registerCount03h; } set { _registerCount03h = value; } }
        public ushort RegisterCount10h { get { return _registerCount10h; } set { _registerCount10h = value; } }
        public int CommInterval { get { return _commInterval; } set { _commInterval = value; } }

        private CancellationTokenSource _cts;

        // 릴레이모듈을 간이검사기에서만 사용. EOL설비에서는 사용하지 않는다.
        //private readonly MRelayModule _relayModule;
        //public MRelayModule RelayModule { get { return _relayModule; } }
        //

        public MDedicatedCTRL()
        {
            //_relayModule = new MRelayModule();
        }

        public MDedicatedCTRL(byte slaveAddress)
        {
            //_relayModule = new MRelayModule();
            _slaveAddress = slaveAddress;
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
                GSystem.Logger.Info("DedicatedCTRL's polling start...");
                GSystem.TraceMessage("DedicatedCTRL's polling start...");
                while (true)
                {
                    token.ThrowIfCancellationRequested(); // ThrowIfCancellationRequested가 더 깔끔합니다.
                    // 취소 요청이 없을 때까지 반복합니다.
                    if (_commandQue.IsEmpty)
                    {
                        // 명령 큐가 비어 있으면 FC03h 실행
                        PushCommand(FC03h_ReadHoldingRegister, 0, (ushort)(EDedicatedCTRL_Registers.Reg049_Alive + 1));
                    }
                    else
                    {
                        // 큐에 있는 명령 수행
                        if (_commandQue.TryDequeue(out var command))
                        {
                            switch (command.FunctionCode)
                            {
                                case FC03h_ReadHoldingRegister:
                                    {
                                        _receivedDatas = await ReadHoldingRegistersAsync(_slaveAddress, command.StartAddress, command.RegisterCount);
                                        token.ThrowIfCancellationRequested(); // 취소 되면 아래 문장을 실행하지 않는다.
                                        for (int i = 0; i < command.RegisterCount; i++)
                                        {
                                            _registerDatas[_startAddress03h + i] = _receivedDatas[i];
                                        }
                                    }
                                    break;
                                case FC10h_WriteMultipleRegister:
                                    {
                                        ushort[] datas = new ushort[command.RegisterCount];
                                        for (int i = 0; i < command.RegisterCount; i++)
                                        {
                                            datas[i] = _registerDatas[command.StartAddress + i];
                                        }
                                        token.ThrowIfCancellationRequested(); // 취소 되면 아래 문장을 실행하지 않는다.
                                        await WriteMultipleRegistersAsync(_slaveAddress, command.StartAddress, datas);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    await Task.Delay(20);
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
                GSystem.Logger.Info("DedicatedCTRL's polling stopped.");
                GSystem.TraceMessage("DedicatedCTRL's polling stopped.");
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
            }
        }

        public ushort GetRegisterValue(int regAddress)
        {
            return _registerDatas[regAddress];
        }
        public void SetRegisterValue(int regAddress, ushort value)
        {
            _registerDatas[regAddress] = value;
        }

        public void PushCommand(byte funcCode, ushort startAddress, ushort registerCount)
        {
            ModbusCommand modbusCmd = new ModbusCommand(funcCode, startAddress, registerCount);
            _commandQue.Enqueue(modbusCmd);
        }

        public void SetCommand(int channel, DedicatedCommandBit commandBit, bool state)
        {
            int commandAddr = (channel == (int)DedicatedChannels.Ch1) ? ADDR_10H_CH1_COMMAND : ADDR_10H_CH2_COMMAND;
            if (state)
                _registerDatas[commandAddr] = (ushort)(_registerDatas[commandAddr] | GDefines.BIT16[(int)commandBit]);
            else
                _registerDatas[commandAddr] = (ushort)(_registerDatas[commandAddr] & ~GDefines.BIT16[(int)commandBit]);
        }

        public bool GetCommandTestInit(int channel)
        {
            int index = (int)DedicatedCommandBit.TestInit;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_10h_ch1_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_10h_ch2_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandTestInit(int channel, bool state)
        {
            SetCommand(channel, DedicatedCommandBit.TestInit, state);
            if (channel == (int)DedicatedChannels.Ch1)
                PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_COMMAND, 1);
            else
                PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_COMMAND, 1);
        }
        public bool GetResponseTestInit(int channel)
        {
            int index = (int)DedicatedCommandBit.TestInit;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_03h_ch1_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_03h_ch2_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteTestInit(int channel)
        {
            int index = (int)DedicatedCommandBit.TestInit;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_03h_ch1_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_03h_ch2_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetCommandTestInitCh1()
        {
            int index = (int)DedicatedCommandBit.TestInit;
            return ((Reg_10h_ch1_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandTestInitCh1(bool state)
        {
            SetCommand((int)DedicatedChannels.Ch1, DedicatedCommandBit.TestInit, state);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_COMMAND, 1);
        }
        public bool GetResponseTestInitCh1()
        {
            int index = (int)DedicatedCommandBit.TestInit;
            return ((Reg_03h_ch1_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteTestInitCh1()
        {
            int index = (int)DedicatedCommandBit.TestInit;
            return ((Reg_03h_ch1_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetCommandShortTest(int channel)
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_10h_ch1_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_10h_ch2_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandShortTest(int channel, bool state)
        {
            SetCommand(channel, DedicatedCommandBit.ShortTest, state);
            if (channel == (int)DedicatedChannels.Ch1)
                PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_COMMAND, 1);
            else
                PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_COMMAND, 1);
        }
        public bool GetResponseShortTest(int channel)
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_03h_ch1_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_03h_ch2_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteShortTest(int channel)
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_03h_ch1_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_03h_ch2_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetCommandShortTestCh1()
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            return ((Reg_10h_ch1_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandShortTestCh1(bool state)
        {
            SetCommand((int)DedicatedChannels.Ch1, DedicatedCommandBit.ShortTest, state);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_COMMAND, 1);
        }
        public bool GetResponseShortTestCh1()
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            return ((Reg_03h_ch1_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteShortTestCh1()
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            return ((Reg_03h_ch1_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCommandActivePowerOn(int channel)
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_10h_ch1_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_10h_ch2_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandActivePowerOn(int channel, bool state)
        {
            SetCommand(channel, DedicatedCommandBit.ActivePowerOn, state);
            if (channel == (int)DedicatedChannels.Ch1)
                PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_COMMAND, 1);
            else
                PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_COMMAND, 1);
        }
        public bool GetResponseActivePowerOn(int channel)
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_03h_ch1_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_03h_ch2_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteActivePowerOn(int channel)
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_03h_ch1_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_03h_ch2_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetCommandActivePowerOnCh1()
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            return ((Reg_10h_ch1_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandActivePowerOnCh1(bool state)
        {
            SetCommand((int)DedicatedChannels.Ch1, DedicatedCommandBit.ActivePowerOn, state);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_COMMAND, 1);
        }
        public bool GetResponseActivePowerOnCh1()
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            return ((Reg_03h_ch1_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteActivePowerOnCh1()
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            return ((Reg_03h_ch1_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetCommandSensorModel(int channel)
        {
            int index = (int)DedicatedCommandBit.SensorModel;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_10h_ch1_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_10h_ch2_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandSensorModel(int channel, bool state)
        {
            SetCommand(channel, DedicatedCommandBit.SensorModel, state);
            if (channel == (int)DedicatedChannels.Ch1)
                PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_COMMAND, 1);
            else
                PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_COMMAND, 1);
        }
        public bool GetResponseSensorModel(int channel)
        {
            int index = (int)DedicatedCommandBit.SensorModel;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_03h_ch1_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_03h_ch2_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteSensorModel(int channel)
        {
            int index = (int)DedicatedCommandBit.SensorModel;
            if (channel == (int)DedicatedChannels.Ch1)
                return ((Reg_03h_ch1_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
            else
                return ((Reg_03h_ch2_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetSensorModel(int sensorModel)
        {
            Reg_10h_ch1_conn_type = (ushort)sensorModel;
            Reg_10h_ch2_conn_type = (ushort)sensorModel;
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_CONN_TYPE, 1);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_CONN_TYPE, 1);
        }


        public bool GetCommandAverageTimeCh1()
        {
            int index = (int)DedicatedCommandBit.AverageTime;
            return ((Reg_10h_ch1_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandAverageTimeCh1(bool state)
        {
            SetCommand((int)DedicatedChannels.Ch1, DedicatedCommandBit.AverageTime, state);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH1_COMMAND, 1);
        }
        public bool GetResponseAverageTimeCh1()
        {
            int index = (int)DedicatedCommandBit.AverageTime;
            return ((Reg_03h_ch1_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteAverageTimeCh1()
        {
            int index = (int)DedicatedCommandBit.AverageTime;
            return ((Reg_03h_ch1_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetExtIStartSwitchCh1()
        {
            int index = (int)DedicatedExtInputBit.StartSwitchCh1;
            return (Reg_03h_ext_input & GDefines.BIT16[index]) == GDefines.BIT16[index];
        }
        public void SetExtIStartSwitchCh1(bool state)
        {
            int index = (int)DedicatedExtInputBit.StartSwitchCh1;
            if (state)
                Reg_03h_ext_input = (ushort)(Reg_03h_ext_input | GDefines.BIT16[index]);
            else
                Reg_03h_ext_input = (ushort)(Reg_03h_ext_input & ~GDefines.BIT16[index]);
        }

        public bool GetStartLampStateCh1()
        {
            int index = (int)DedicatedExtOutputBit.StartLampCh1;
            return (Reg_10h_ext_output & GDefines.BIT16[index]) == GDefines.BIT16[index];
        }
        public void SetStartLampStateCh1(bool state)
        {
            int index = (int)DedicatedExtOutputBit.StartLampCh1;
            if (state)
                Reg_10h_ext_output = (ushort)(Reg_10h_ext_output | GDefines.BIT16[index]);
            else
                Reg_10h_ext_output = (ushort)(Reg_10h_ext_output & ~GDefines.BIT16[index]);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_EXT_OUTPUT, 1);
        }

        public bool GetLockSignal(int channel)
        {
            if (channel == (int)DedicatedChannels.Ch1)
                return (Reg_03h_ch1_lock_signal != 0);
            else
                return (Reg_03h_ch2_lock_signal != 0);
        }

        public bool GetLockSignalCh1()
        {
            return (Reg_03h_ch1_lock_signal != 0);
        }



        // Ch2
        public bool GetCommandTestInitCh2()
        {
            int index = (int)DedicatedCommandBit.TestInit;
            return ((Reg_10h_ch2_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandTestInitCh2(bool state)
        {
            SetCommand((int)DedicatedChannels.Ch2, DedicatedCommandBit.TestInit, state);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_COMMAND, 1);
        }
        public bool GetResponseTestInitCh2()
        {
            int index = (int)DedicatedCommandBit.TestInit;
            return ((Reg_03h_ch2_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteTestInitCh2()
        {
            int index = (int)DedicatedCommandBit.TestInit;
            return ((Reg_03h_ch2_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetCommandShortTestCh2()
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            return ((Reg_10h_ch2_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandShortTestCh2(bool state)
        {
            SetCommand((int)DedicatedChannels.Ch2, DedicatedCommandBit.ShortTest, state);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_COMMAND, 1);
        }
        public bool GetResponseShortTestCh2()
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            return ((Reg_03h_ch2_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteShortTestCh2()
        {
            int index = (int)DedicatedCommandBit.ShortTest;
            return ((Reg_03h_ch2_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetCommandActivePowerOnCh2()
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            return ((Reg_10h_ch2_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandActivePowerOnCh2(bool state)
        {
            SetCommand((int)DedicatedChannels.Ch2, DedicatedCommandBit.ActivePowerOn, state);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_COMMAND, 1);
        }
        public bool GetResponseActivePowerOnCh2()
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            return ((Reg_03h_ch2_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteActivePowerOnCh2()
        {
            int index = (int)DedicatedCommandBit.ActivePowerOn;
            return ((Reg_03h_ch2_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetCommandAverageTimeCh2()
        {
            int index = (int)DedicatedCommandBit.AverageTime;
            return ((Reg_10h_ch2_command & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public void SetCommandAverageTimeCh2(bool state)
        {
            SetCommand((int)DedicatedChannels.Ch2, DedicatedCommandBit.AverageTime, state);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_CH2_COMMAND, 1);
        }
        public bool GetResponseAverageTimeCh2()
        {
            int index = (int)DedicatedCommandBit.AverageTime;
            return ((Reg_03h_ch2_response & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }
        public bool GetCompleteAverageTimeCh2()
        {
            int index = (int)DedicatedCommandBit.AverageTime;
            return ((Reg_03h_ch2_complete & GDefines.BIT16[index]) == GDefines.BIT16[index]);
        }

        public bool GetExtIStartSwitchCh2()
        {
            int index = (int)DedicatedExtInputBit.StartSwitchCh2;
            return (Reg_03h_ext_input & GDefines.BIT16[index]) == GDefines.BIT16[index];
        }
        public void SetExtIStartSwitchCh2(bool state)
        {
            int index = (int)DedicatedExtInputBit.StartSwitchCh2;
            if (state)
                Reg_03h_ext_input = (ushort)(Reg_03h_ext_input | GDefines.BIT16[index]);
            else
                Reg_03h_ext_input = (ushort)(Reg_03h_ext_input & ~GDefines.BIT16[index]);
        }

        public bool GetStartLampStateCh2()
        {
            int index = (int)DedicatedExtOutputBit.StartLampCh2;
            return (Reg_10h_ext_output & GDefines.BIT16[index]) == GDefines.BIT16[index];
        }
        public void SetStartLampStateCh2(bool state)
        {
            int index = (int)DedicatedExtOutputBit.StartLampCh2;
            if (state)
                Reg_10h_ext_output = (ushort)(Reg_10h_ext_output | GDefines.BIT16[index]);
            else
                Reg_10h_ext_output = (ushort)(Reg_10h_ext_output & ~GDefines.BIT16[index]);
            PushCommand(FC10h_WriteMultipleRegister, ADDR_10H_EXT_OUTPUT, 1);
        }

        public bool GetLockSignalCh2()
        {
            return (Reg_03h_ch2_lock_signal != 0);
        }


        // 릴레이모듈은 간이검사기에서만 사용한다. EOL설비에서는 사용하지 않는다.
        public bool OpenRelayModule(string portName, int baudRate, string parityBit = "None", int dataBit = 8, int stopBit = 1)
        {
            return false; //_relayModule.Open(portName, baudRate, parityBit, dataBit, stopBit);
        }
        public void CloseRelayModule()
        {
            //_relayModule.Close();
        }

        public void LockLampStateCh1(bool onOff)
        {
            //_relayModule.SetLockLampStateCh1(onOff);
        }

        public void PowerLampStateCh1(bool onOff)
        {
            //_relayModule.SetPowerLampStateCh1(onOff);
        }

        public void LockLampStateCh2(bool onOff)
        {
            //_relayModule.SetLockLampStateCh2(onOff);
        }

        public void PowerLampStateCh2(bool onOff)
        {
            //_relayModule.SetPowerLampStateCh2(onOff);
        }
        //
    }
}

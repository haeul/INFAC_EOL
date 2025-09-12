using Modbus.Device;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DHSTesterXL
{
    public class HModbusRTU : IDisposable
    {
        private SerialPort _serialPort = null;
        private ModbusSerialMaster _modbusMaster = null;

        public bool IsOpen { get { return (_serialPort != null) ? _serialPort.IsOpen : false; } }

        // Function code
        public const byte FC01h_ReadCoil              = 0x01;
        public const byte FC02h_ReadDiscreteInputs    = 0x02;
        public const byte FC03h_ReadHoldingRegister   = 0x03;
        public const byte FC04h_ReadInputRegister     = 0x04;
        public const byte FC05h_WriteSingleCoil       = 0x05;
        public const byte FC06h_WriteSingleRegister   = 0x06;
        public const byte FC0Fh_WriteMultipleCoils    = 0x0F;
        public const byte FC10h_WriteMultipleRegister = 0x10;

        public HModbusRTU()
        {
        }

        public HModbusRTU(string portName, int baudRate, string parityBit = "None", int dataBit = 8, int stopBit = 1)
        {
        }

        ~HModbusRTU()
        {
            Dispose();
        }

        public bool Open(string portName, int baudRate, string parityBit = "None", int dataBit = 8, int stopBit = 1)
        {
            try
            {
                _serialPort = new SerialPort(portName)
                {
                    BaudRate = baudRate,
                    DataBits = dataBit
                };

                switch (parityBit)
                {
                    case "Odd": _serialPort.Parity = Parity.Odd; break;
                    case "Even": _serialPort.Parity = Parity.Even; break;
                    default:
                        _serialPort.Parity = Parity.None;
                        break;
                }

                if (stopBit == 2)
                    _serialPort.StopBits = StopBits.Two;
                else
                    _serialPort.StopBits = StopBits.One;

                _serialPort.Open();
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();

                _modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);
                _modbusMaster.Transport.ReadTimeout = 500;
                _modbusMaster.Transport.WriteTimeout = 500;
                _modbusMaster.Transport.Retries = 1;
                
                return true;
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }

            return false;
        }

        public void Close()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }
        }

        // FC01h
        public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    return _modbusMaster.ReadCoils(slaveAddress, startAddress, numberOfPoints);
                }
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC01h: Address = {startAddress}h, Count = {numberOfPoints}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
            return null;
        }
        public async Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (_serialPort.IsOpen)
                    return await _modbusMaster.ReadCoilsAsync(slaveAddress, startAddress, numberOfPoints);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC01h: Address = {startAddress}h, Count = {numberOfPoints}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
            return null;
        }

        // FC02h
        public bool[] ReadInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (_serialPort.IsOpen)
                    return _modbusMaster.ReadInputs(slaveAddress, startAddress, numberOfPoints);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC02h: Address = {startAddress}h, Count = {numberOfPoints}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
            return null;
        }
        public async Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (_serialPort.IsOpen)
                    return await _modbusMaster.ReadInputsAsync(slaveAddress, startAddress, numberOfPoints);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC02h: Address = {startAddress}h, Count = {numberOfPoints}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
            return null;
        }

        // FC03h
        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (_serialPort.IsOpen)
                    return _modbusMaster.ReadHoldingRegisters(slaveAddress, startAddress, numberOfPoints);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC03h: Address = {startAddress}h, Count = {numberOfPoints}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
            return null;
        }
        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (_serialPort.IsOpen)
                    return await _modbusMaster.ReadHoldingRegistersAsync(slaveAddress, startAddress, numberOfPoints);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC03h: Address = {startAddress}h, Count = {numberOfPoints}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
                throw;
            }
            return null;
        }

        // FC04h
        public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (_serialPort.IsOpen)
                    return _modbusMaster.ReadInputRegisters(slaveAddress, startAddress, numberOfPoints);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC04h: Address = {startAddress}h, Count = {numberOfPoints}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
            return null;
        }
        public async Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            try
            {
                if (_serialPort.IsOpen)
                    await _modbusMaster.ReadInputRegistersAsync(slaveAddress, startAddress, numberOfPoints);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC04h: Address = {startAddress}h, Count = {numberOfPoints}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
            return null;
        }

        // FC05h
        public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value)
        {
            try
            {
                if (_serialPort.IsOpen)
                    _modbusMaster.WriteSingleCoil(slaveAddress, coilAddress, value);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC05h: Address = {coilAddress}h, Value = {value}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
        }
        public async Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value)
        {
            try
            {
                if (_serialPort.IsOpen)
                    await _modbusMaster.WriteSingleCoilAsync(slaveAddress, coilAddress, value);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC05h: Address = {coilAddress}h, Value = {value}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
        }

        // FC06h
        public void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value)
        {
            try
            {
                if (_serialPort.IsOpen)
                    _modbusMaster.WriteSingleRegister(slaveAddress, registerAddress, value);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC06h: Address = {registerAddress}h, Value = {value}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
        }
        public async Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value)
        {
            try
            {
                if (_serialPort.IsOpen)
                    await _modbusMaster.WriteSingleRegisterAsync(slaveAddress, registerAddress, value);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC06h: Address = {registerAddress}h, Value = {value}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
        }

        // FC0Fh
        public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data)
        {
            try
            {
                if (_serialPort.IsOpen)
                    _modbusMaster.WriteMultipleCoils(slaveAddress, startAddress, data);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC0Fh: Address = {startAddress}h, Count = {data.Length}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
        }
        public async Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data)
        {
            try
            {
                if (_serialPort.IsOpen)
                    await _modbusMaster.WriteMultipleCoilsAsync(slaveAddress, startAddress, data);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC0Fh: Address = {startAddress}h, Count = {data.Length}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
        }

        // FC10h
        public void WriteMultipleRegisters(byte slaveAddress, ushort startAddress, ushort[] data)
        {
            try
            {
                if (_serialPort.IsOpen)
                    _modbusMaster.WriteMultipleRegisters(slaveAddress, startAddress, data);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC10h: Address = {startAddress}h, Count = {data.Length}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
        }
        public async Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data)
        {
            try
            {
                if (_serialPort.IsOpen)
                    await _modbusMaster.WriteMultipleRegistersAsync(slaveAddress, startAddress, data);
            }
            catch (Exception ex)
            {
                // 통신 실패
                StringBuilder sb = new StringBuilder($"{ex.Message} [FC10h: Address = {startAddress}h, Count = {data.Length}]");
                GSystem.TraceMessage(sb.ToString());
                GSystem.Logger.Fatal(sb.ToString());
            }
        }

        public void Dispose()
        {
            try
            {
                _modbusMaster?.Dispose();
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage($"{ex.Message}");
                GSystem.Logger.Fatal(ex.Message);
            }
        }
    }
}

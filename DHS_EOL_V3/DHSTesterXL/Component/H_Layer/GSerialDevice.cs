using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DHSTesterXL
{
    public delegate void SerialDataReceivedHandler(string strReceivedData);

    public class GSerialDevice
    {
        protected SerialPort serialPort_ = new SerialPort();

        protected string receivedData_ = "";
        protected int receivedByte_ = 0;

        protected Thread pollingThread_ = null;
        protected volatile bool pollingThreadExit_ = false;

        protected readonly byte byteSTX = (byte)0x02;
        protected readonly byte byteETX = (byte)0x03;

        public GSerialDevice()
        {

        }

        public bool Connect(string portName, int baudRate = 9600, string parityBit = "None", int dataBit = 8, int stopBit = 1)
        {
            try
            {
                receivedData_ = "";

                serialPort_.PortName = portName;
                serialPort_.BaudRate = baudRate;

                switch (parityBit)
                {
                    case "Odd": serialPort_.Parity = Parity.Odd; break;
                    case "Even": serialPort_.Parity = Parity.Even; break;
                    default:
                        serialPort_.Parity = Parity.None;
                        break;
                }

                serialPort_.DataBits = dataBit;

                if (stopBit == 2)
                    serialPort_.StopBits = StopBits.Two;
                else
                    serialPort_.StopBits = StopBits.One;

                serialPort_.Open();
                serialPort_.DiscardInBuffer();
                serialPort_.DiscardOutBuffer();
                // DataReceived 핸들러로 처리하면 수신데이터가 많을 경우 Close 시 Freezing 되는 현상이 있다.
                //serialPort_.DataReceived += SerialPort_DataReceived;

                // 수신 처리는 스레드에서 한다.
                if (pollingThread_ == null)
                {
                    pollingThread_ = new Thread(new ThreadStart(PollingThread));
                    pollingThread_.Start();
                }
                else
                {
                    // 스레드 실행 에러
                }

                return true;
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }

            return false;
        }

        public void Disconnect(int timeOut = 5000)
        {
            try
            {
                if (pollingThread_ != null)
                {
                    pollingThreadExit_ = true;
                    pollingThread_.Join(timeOut); // 스레드가 종료 될 때까지 대기
                }

                if (serialPort_.IsOpen)
                {
                    serialPort_.DiscardInBuffer();
                    serialPort_.DiscardOutBuffer();
                    serialPort_.Close();
                }
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 고속 데이터를 DataReceived 핸들러로 처리할 경우 포트를 닫을 때 프리징 되는 현상이 있다.
            // 수신 처리를 스레드에서 한다.
        }

        private void PollingThread()
        {
            Debug.WriteLine(string.Format("[ {0,-30} ] Thread start...", System.Reflection.MethodBase.GetCurrentMethod().Name));

            pollingThreadExit_ = false;

            // 스레드 시작 시 포트 및 수신 데이터 변수 초기화
            serialPort_.DiscardInBuffer();
            serialPort_.DiscardOutBuffer();

            receivedData_ = string.Empty;
            receivedByte_ = 0;

            while (!pollingThreadExit_)
            {
                Thread.Sleep(1);
                PollingProc();
            }

            pollingThread_ = null;

            Debug.WriteLine(string.Format("[ {0,-30} ] Thread terminated!", System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        public virtual void PollingProc()
        {
            // 시리얼포트 수신 버퍼를 읽어오는 스레드
            // 이 클래스를 상속받는 하위 클래스에서 구현 한다.
        }

        public void Write(string transmitData)
        {
            try
            {
                serialPort_.Write(transmitData);
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }
        }

        public void Write(char[] buffer, int offset, int count)
        {
            try
            {
                serialPort_.Write(buffer, offset, count);
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                serialPort_.Write(buffer, offset, count);
            }
            catch (Exception ex)
            {
                GSystem.TraceMessage(ex.Message);
                GSystem.Logger.Fatal(ex.Message);
            }
        }

        public bool IsOpened
        {
            get {return serialPort_.IsOpen;}
        }

    }
}

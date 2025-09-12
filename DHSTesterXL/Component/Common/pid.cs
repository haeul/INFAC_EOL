using GSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSCommon
{
    public delegate double GetDouble();
    public delegate void SetDouble(double value);

    public class PID
    {
        #region Fields

        //Gains
        private double pGain;
        private double iGain;
        private double dGain;

        //Outputs
        private double pTerm;
        private double iTerm;
        private double dTerm;

        //Errors
        private double currError;
        private double prevError;
        private double deltaError;
        private double errorSum;

        //PID output
        private double currOut;
        private double prevOut;

        //Sensor input interval
        private double dT;

        private bool fileSave = false;

        private GCsvFile pidFile = new GCsvFile();

        #endregion

        #region Properties

        public double PGain
        {
            get { return pGain; }
            set { pGain = value; }
        }

        public double IGain
        {
            get { return iGain; }
            set { iGain = value; }
        }

        public double DGain
        {
            get { return dGain; }
            set { dGain = value; }
        }

        public double CurrentError
        {
            get { return currError; }
            //set { currError = value; }
        }

        public double PrevError
        {
            get { return prevError; }
            //set { prevError = value; }
        }

        public double DeltaError
        {
            get { return deltaError; }
            //set { deltaError = value; }
        }

        public double ErrorSum
        {
            get { return errorSum; }
            //set { errorSum = value; }
        }

        public bool FileSave
        {
            get { return fileSave; }
            set { fileSave = value; }
        }

        #endregion

        #region Construction / Deconstruction

        public PID(double gainP, double gainI, double gainD, double sensorTime)
        {
            pGain = gainP;
            iGain = gainI;
            dGain = gainD;
            dT = sensorTime;
        }

        #endregion

        #region Public Methods

        public void Initialize(double gainP, double gainI, double gainD, double sensorTime)
        {
            pGain = gainP;
            iGain = gainI;
            dGain = gainD;
            dT = sensorTime;
            Reset();
            if (fileSave)
            {
                //if (pidFile.Open(string.Format($"{DateTime.Now.ToString("yyMMdd_HHmmss")}_pid.csv")))
                //{
                //    pidFile.WriteLine($"P Gain,{pGain:F02}");
                //    pidFile.WriteLine($"I Gain,{iGain:F02}");
                //    pidFile.WriteLine($"D Gain,{dGain:F02}");
                //    pidFile.WriteLine($"Sensor Time,{dT:F02}");
                //}
            }
        }

        public void Reset()
        {
            //Outputs
            pTerm = 0f;
            iTerm = 0f;
            dTerm = 0f;

            //Errors
            currError = 0f;
            prevError = 0f;
            deltaError = 0f;
            errorSum = 0f;

            //PID output
            currOut = 0f;
            prevOut = 0f;
        }

        public double Compute(double sv, double pv)
        {
            // Compute the error
            currError = sv - pv;

            // delta error
            deltaError = currError - prevError;

            // Compute the error sum
            errorSum += currError;

            // Compute the propotional output
            pTerm = pGain * currError;

            // Compute the integal output
            iTerm += iGain * (currError * dT);

            // Compute the derivative output
            dTerm = dGain * (deltaError / dT);

            // 출력값
            currOut = (pTerm + iTerm + dTerm);

            if (fileSave)
            {
                // sv, pv, prevError, currError, deltaError, errorSum, pTerm, iTerm, dTerm, prevOut, currOut
                //pidFile.WriteLine($"{sv:F02},{pv:F02},{prevError:F02},{currError:F02},{deltaError:F02},{errorSum:F02},{pTerm:F03},{iTerm:F03},{dTerm:F03},{prevOut:F03},{currOut:F03}");
            }

            // 이전 데이터 저장
            prevError = currError;
            prevOut = currOut;

            return currOut;
        }

        #endregion
    }
}

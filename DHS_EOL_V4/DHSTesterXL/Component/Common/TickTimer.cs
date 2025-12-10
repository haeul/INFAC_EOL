using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSCommon
{
    public class TickTimer
    {
        private long _startTick = 0;
        public bool IsStart { get; private set; }

        Stopwatch _stopTick = new Stopwatch();

        public TickTimer()
        {
            _startTick = 0;
            IsStart = false;
        }

        public void Start()
        {
            IsStart = true;
            _startTick = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
            _stopTick.Start();
        }

        public void Stop()
        {
            IsStart = false;
        }

        public void Reset()
        {
            IsStart = true;
            _startTick = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
            _stopTick.Restart();
        }

        public long GetElapsedMilliseconds()
        {
            //if (!IsStart) { return 0; }
            long milliseconds = (Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond) - _startTick;
            return milliseconds;
        }

        public double GetElapsedSeconds()
        {
            //if (!IsStart) { return 0f; }
            double seconds = ((Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond) - _startTick) / 1000.000000f;
            return seconds;
        }

        public double GetTotalSeconds()
        {
            return _stopTick.Elapsed.TotalSeconds;
        }

        public bool MoreThan(long msec)
        {
            //if (!IsStart) { return false; }
            long currTick = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
            if ((currTick - _startTick) >= msec)
                return true;
            return false;
        }

        public bool LessThan(long msec)
        {
            //if (!IsStart) { return false; }
            long currTick = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
            if ((currTick - _startTick) < msec)
                return true;
            return false;
        }
    }
}

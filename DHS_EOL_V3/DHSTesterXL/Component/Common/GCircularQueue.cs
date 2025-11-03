using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSCommon
{
    public class GCircularQueue
    {
        public const int DEFAULT_QUE_SIZE = 10;
        int size_ = DEFAULT_QUE_SIZE;
        int head_ = 0;
        int tail_ = 0;
        int count_ = 0;
        double[] que_ = null;

        public GCircularQueue(int queSize = DEFAULT_QUE_SIZE)
        {
            que_ = new double[queSize + 1];
            size_ = queSize;
            head_ = 0;
            tail_ = 0;
            count_ = 0;
        }

        public void Clear()
        {
            if (que_ == null) return;
            Array.Clear(que_, 0, que_.Length);
            head_ = 0;
            tail_ = 0;
            count_ = 0;
        }

        public int GetSize()
        {
            return size_;
        }

        public bool SetSize(int size)
        {
            if (que_ == null) return false;
            if (size < DEFAULT_QUE_SIZE)
                return false;

            que_ = new double[size + 1];
            Clear();
            size_ = size;

            return true;
        }

        public bool Enqueue(double data)
        {
            if (que_ == null) return false;
            tail_ = (tail_ + 1) % (size_ + 1);
            que_[tail_] = data;

            if ((count_ + 1) > size_)
            {
                head_ = (head_ + 1) % (size_ + 1);
                que_[head_] = 0; // 기존 데이터 삭제
            }
            else
            {
                ++count_;
            }

            return true;
        }

        public bool Dequeue()
        {
            if (que_ == null) return false;
            if (head_ == tail_)
                return false;

            head_ = (head_ + 1) % size_;

            --count_;

            return true;
        }

        public double GetHead()
        {
            if (que_ == null) return 0;
            return que_[head_ + 1];
        }

        public bool GetQueData(int index, ref double queData)
        {
            if (que_ == null) return false;
            if (count_ == 0) return false;
            if (index < 0) return false;
            if (index >= size_) return false;

            int qindex = (head_ + index + 1) % (size_ + 1);

            queData = que_[qindex];

            return true;
        }

        public double Average()
        {
            double tot = 0;
            double avg = 0;

            for (int i = 0; i < count_; i++)
            {
                double qdata = 0;
                GetQueData(i, ref qdata);
                tot += qdata;
            }
            avg = tot / count_;

            return avg;
        }
    }
}

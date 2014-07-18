// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class MetricsTracker
    {
        private long mSumValues = 0;
        private long mNumberOfSums = 0;
        private long mAverageValue = 0;
        private long mMin = 0;
        private long mMax = 0;

        public bool Reset
        {
            get { return false; }
            set
            {
                mAverageValue = 0;
                mSumValues = 0;
                mNumberOfSums = 0;
                mMin = 99999999;
                mMax = 0;
            }
        }

        public long Average { get { return mAverageValue; } }
        public long Min { get { return mMin; } }
        public long Max { get { return mMax; } }

        public void NewValue(long value)
        {
            mNumberOfSums++;
            mSumValues += value;
            mAverageValue = mSumValues / mNumberOfSums;
            if (value < mMin) mMin = value;
            if (value > mMax) mMax = value;
        }

        private TimeSpan mSpan;
        public void NewPeriod(DateTime start, DateTime end)
        {
            mSpan = end - start;
            NewValue((long)mSpan.TotalMilliseconds);
        }

        public override string ToString()
        {
            return Average + " " + Min + " " + Max + " " + mNumberOfSums;
        }
    }
}

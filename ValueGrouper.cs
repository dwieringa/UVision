// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class ValueGrouper
    {
        public ValueGrouper(int low, int high, int numGroups)
        {
            mLowEnd = low;
            mHighEnd = high;
            mNumGroups = numGroups;
            mGroupSize = (mHighEnd - mLowEnd + 1)/(float)mNumGroups;
            mGroups = new GroupStats[numGroups];

            //see ValueGroup.xls if questions on math.  it's not documented, but at least lets you play with different setups

            int mGroupStart = mLowEnd;
            int mGroupEnd;
            for( int x = 0; x < numGroups; x++)
            {
                mGroupEnd = (int)Math.Truncate((x + 1) * mGroupSize + mLowEnd - 1);
                mGroups[x] = new GroupStats(x, mGroupStart, mGroupEnd);
                mGroupStart = mGroupEnd + 1;
            }
            mGroups[mNumGroups - 1].end = mHighEnd;
        }

        public GroupStats GetGroup(int x)
        {
            return mGroups[x];
        }

        public GroupStats BiggestGroupWithNeighbors()
        {
            GroupStats biggestGroup = BiggestGroup();
            GroupStats newGroup = null;
            if (biggestGroup.groupNdx > 0 && mGroups[biggestGroup.groupNdx - 1].count > 0)
            {
                GroupStats groupBefore = mGroups[biggestGroup.groupNdx - 1];
                newGroup = new GroupStats(-999, groupBefore.start, biggestGroup.end);
                newGroup.count = groupBefore.count + biggestGroup.count;
                newGroup.sum = groupBefore.sum + biggestGroup.sum;
                newGroup.min = groupBefore.min;
                newGroup.max = biggestGroup.max;
            }
            if (biggestGroup.groupNdx < NumGroups-1 && mGroups[biggestGroup.groupNdx + 1].count > 0)
            {
                GroupStats groupAfter = mGroups[biggestGroup.groupNdx + 1];
                if (newGroup == null)
                {
                    newGroup = new GroupStats(-999, biggestGroup.start, groupAfter.end);
                    newGroup.min = biggestGroup.min;
                    newGroup.count = biggestGroup.count;
                    newGroup.sum = biggestGroup.sum;
                }
                newGroup.max = groupAfter.max;
                newGroup.count += groupAfter.count;
                newGroup.sum += groupAfter.sum;
            }
            if (newGroup == null)
            {
                return biggestGroup;
            }
            else
            {
                return newGroup;
            }
        }

        public GroupStats BiggestGroup()
        {
            int biggestGroup = -1;
            int sizeOfBiggestGroup = 0;
            for (int x = 0; x < mNumGroups; x++)
            {
                if (mGroups[x].count > sizeOfBiggestGroup)
                {
                    sizeOfBiggestGroup = mGroups[x].count;
                    biggestGroup = x;
                }
            }
            if (biggestGroup < 0) return null;

            return mGroups[biggestGroup];
        }

        private int mLowEnd = 0;
        public int MinValue
        {
            get { return mLowEnd; }
        }

        private float mGroupSize = 0;
        private int mHighEnd = 0;
        public int MaxValue
        {
            get { return mHighEnd; }
        }

        private int mNumGroups = 0;
        public int NumGroups
        {
            get { return mNumGroups; }
        }
        private GroupStats[] mGroups;

        public void AddValue(int theValue)
        {
            int group = (int)((theValue - mLowEnd) / mGroupSize);
            if (group < 0) throw new ArgumentException("Invalid value 2o3ijr89j");
            if (group >= mNumGroups)
            {
                if (group == mNumGroups) // we may get a rounding issue for tha last group, so we catch those
                {
                    group = mNumGroups - 1;
                }
                else // but if we're WAY off, then something is wrong
                {
                    throw new ArgumentException("Invalid value 28j3r238j");
                }
            }
            mGroups[group].AddValue(theValue);
        }

        public class GroupStats
        {
            public GroupStats(int groupNdx, int start, int end)
            {
                this.groupNdx = groupNdx;
                this.start = start;
                this.end = end;
            }
            public int groupNdx;
            public int start;
            public int end;
            public int min = 0;
            public int max = 0;
            public long sum = 0;
            public int count = 0;
            public int Average()
            {
                if( count < 1 ) return 0;
                return (int)(sum / count);
            }
            public void AddValue(int theValue)
            {
                sum += theValue;
                count++;
                if (count == 1)
                {
                    min = theValue;
                    max = theValue;
                }
                else
                {
                    if (theValue < min) min = theValue;
                    if (theValue > max) max = theValue;
                }
            }
        }
    }
}

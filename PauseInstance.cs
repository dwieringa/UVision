// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class PauseInstance : ToolInstance
    {
        public PauseInstance(PauseDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mPauseDurationInMS = theDefinition.Duration;
        }

        private long mPauseDurationInMS = 0;
        public long Duration
        {
            get { return mPauseDurationInMS; }
        }

        private DateTime mStartTime = DateTime.MaxValue;
        private bool mIsCompleted = false;
        public override void DoWork()
        {
            //if (!AreExplicitDependenciesComplete()) return;

            if (mStartTime == DateTime.MaxValue)
            {
                TestExecution().LogMessageWithTimeFromTrigger("Pause " + Name + " started");
                mStartTime = DateTime.Now;
                return;
            }

            TimeSpan span = DateTime.Now - mStartTime;
            if (span.TotalMilliseconds >= mPauseDurationInMS)
            {
                TestExecution().LogMessageWithTimeFromTrigger("Pause " + Name + " completed");
                mIsCompleted = true;
            }
        }

        public override bool IsComplete()
        {
            return mIsCompleted;
        }
    }
}

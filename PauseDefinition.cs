// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class PauseDefinition : ToolDefinition
    {
        public PauseDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

        private long mPauseDurationInMS = 0;
        public long Duration
        {
            get { return mPauseDurationInMS; }
            set 
            {
                HandlePropertyChange(this, "Duration", mPauseDurationInMS, value);
                mPauseDurationInMS = value;
            }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new PauseInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
//                if (mStartX != null) result = Math.Max(result, mStartX.ToolMapRow);
                return result + 1;
            }
        }
    }
}

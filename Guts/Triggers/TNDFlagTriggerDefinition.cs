// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class TNDFlagTriggerDefinition : NetCams.TriggerDefinition
	{
        public TNDFlagTriggerDefinition(TestSequence testSequence)
            : base(testSequence)
		{
			PollTimer = new System.Timers.Timer(20);
		}

		public System.Timers.Timer PollTimer;
        private Int16 mDataViewIndex;
        public Int16 DataViewIndex
        {
            get { return mDataViewIndex; }
            set 
            {
                HandlePropertyChange(this, "DataViewIndex", mDataViewIndex, value);
                mDataViewIndex = value;
            }
        }

        public double PollPeriod
        {
            get { return PollTimer.Interval; }
            set 
            {
                HandlePropertyChange(this, "PollPeriod", PollTimer.Interval, value);
                PollTimer.Interval = value;
            }
        }

        public override bool TriggerEnabled
		{
            get { return PollTimer.Enabled; }
            set 
            {
                HandlePropertyChange(this, "TriggerEnabled", PollTimer.Enabled, value);
                PollTimer.Enabled = value;
            }
        }

		public override void CreateInstance(TestExecution testExecution)
		{
            new TNDFlagTriggerInstance(this, testExecution);
		}

    }
}

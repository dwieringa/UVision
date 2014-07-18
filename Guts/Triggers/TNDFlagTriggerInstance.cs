// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace NetCams
{
    class TNDFlagTriggerInstance : NetCams.TriggerInstance
	{
        public TNDFlagTriggerInstance(TNDFlagTriggerDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            theDefinition.PollTimer.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
        }

        private void ServiceTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Int32 theValue = 0;
            bool success = Definition().Window().myTNDLink.ReadFromTNDByTypeIndex(TNDLink_old.FlagType, ((TNDFlagTriggerDefinition)Definition()).DataViewIndex, ref theValue, "reading");
            if (success && theValue > 0)
            {
                mIsComplete = true;
                ((TNDFlagTriggerDefinition)Definition()).PollTimer.Elapsed -= new ElapsedEventHandler(ServiceTimer_Tick);
                success = Definition().Window().myTNDLink.WriteToTNDByTypeIndex(TNDLink_old.FlagType, ((TNDFlagTriggerDefinition)Definition()).DataViewIndex, 0, "writing");
            }
        }

        public override bool CheckTrigger() { return mIsComplete; }

        public override void PostExecutionCleanup()
        {
            // this was added as a result of memory profiling...if we rely on another trigger (e.g. TimerTrigger), then this trigger never stopped querrying TND. TODO: setup a dispose/destructor to accomplish this type of cleanup...ACTUALLY instances aren't disposed until they leave last TestExecutionCollection...need post-execution cleanup mechanism?
            // TODO: only do this is if !mIsComplete?
            ((TNDFlagTriggerDefinition)Definition()).PollTimer.Elapsed -= new ElapsedEventHandler(ServiceTimer_Tick);
        }
        
        public override bool IsComplete() { return mIsComplete; }
		private bool mIsComplete = false;
	}
}

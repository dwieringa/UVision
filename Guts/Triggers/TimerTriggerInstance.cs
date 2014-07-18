// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Timers;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class TimerTriggerInstance : NetCams.TriggerInstance
	{
		public TimerTriggerInstance(TimerTriggerDefinition theDefinition, TestExecution testExecution) : base(theDefinition, testExecution)
		{
			theDefinition.TriggerTimer.Elapsed += new ElapsedEventHandler( ServiceTimer_Tick  );
		}

		private void ServiceTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
		{
			mIsComplete = true;
			((TimerTriggerDefinition)Definition()).TriggerTimer.Elapsed -= new ElapsedEventHandler( ServiceTimer_Tick  );
        }

        public override void PostExecutionCleanup()
        {
            // this was added as a result of memory profiling...if we rely on another trigger (e.g. TimerTrigger), then this trigger never stopped querrying TND. TODO: setup a dispose/destructor to accomplish this type of cleanup...ACTUALLY instances aren't disposed until they leave last TestExecutionCollection...need post-execution cleanup mechanism?
            // TODO: only do this is if !mIsComplete?
            ((TimerTriggerDefinition)Definition()).TriggerTimer.Elapsed -= new ElapsedEventHandler(ServiceTimer_Tick);
        }

        public override bool CheckTrigger() { return mIsComplete; }

        public override bool IsComplete() { return mIsComplete; }
		private bool mIsComplete = false;
	}
}

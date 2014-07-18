// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class TimerTriggerDefinition : NetCams.TriggerDefinition
	{
		public TimerTriggerDefinition(TestSequence testSequence) : base(testSequence)
		{
			// 
			// TODO: Add constructor logic here
			//
			TriggerTimer = new System.Timers.Timer(10000);
		}

		public System.Timers.Timer TriggerTimer;
		public double Interval
		{
			get { return TriggerTimer.Interval; }
			set 
            {
                HandlePropertyChange(this, "Interval", TriggerTimer.Interval, value);
                TriggerTimer.Interval = value;
            }
		}

		public override bool TriggerEnabled
		{
			get { return TriggerTimer.Enabled; }
			set 
            {
                HandlePropertyChange(this, "TriggerEnabled", TriggerTimer.Enabled, value);
                TriggerTimer.Enabled = value;
            }
		}

		public override void CreateInstance(TestExecution testExecution)
		{
			new TimerTriggerInstance(this, testExecution);
		}

	}
}

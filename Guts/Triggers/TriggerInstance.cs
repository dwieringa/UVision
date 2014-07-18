// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class TriggerInstance : NetCams.ObjectInstance, ITriggerInstance
	{
		public TriggerInstance(ITriggerDefinition theDefinition, TestExecution theExecution) : base(theDefinition, theExecution)
		{
			theExecution.RegisterTrigger(this);
		}

        public abstract bool CheckTrigger();

        public ITriggerDefinition Definition_Trigger()
        {
            return (ITriggerDefinition)Definition();
        }
	}
}

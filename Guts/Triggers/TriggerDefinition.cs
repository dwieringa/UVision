// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class TriggerDefinition : NetCams.ObjectDefinition, ITriggerDefinition
	{
		public TriggerDefinition(TestSequence testSequence) : base(testSequence)
		{
			testSequence.TriggerRegistry.RegisterObject(this);
		}

        public override void Dispose_UVision()
        {
            TestSequence().TriggerRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        public abstract bool TriggerEnabled
        {
            get;
            set;
        }


		public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            return base.IsDependentOn(theOtherObject);
        }

		public override int ToolMapRow
		{
			get { return 0; }
		}
	}
}

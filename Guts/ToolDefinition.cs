// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;

namespace NetCams
{
	/// <summary>
	/// Abstract base class of all tools
	/// </summary>
	public abstract class ToolDefinition : NetCams.ObjectDefinition, IToolDefinition
	{
		public ToolDefinition(TestSequence testSequence)
            : base(testSequence)
		{
			testSequence.ToolRegistry.RegisterObject(this);
		}

        public override void Dispose_UVision()
        {
            TestSequence().ToolRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        private DataValueDefinition mPrerequisite = null;
        public DataValueDefinition Prerequisite
        {
            get { return mPrerequisite; }
            set
            {
                HandlePropertyChange(this, "Prerequisite", mPrerequisite, value);
                mPrerequisite = value;
                TestSequence().GarbageCollectMathOperations(); // get rid of any MathOp/MathOpResults that were used only by the Prerequisite
            }
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            return (mPrerequisite != null && mPrerequisite.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mPrerequisite != null) result = Math.Max(result, mPrerequisite.ToolMapRow);
                return result + 1;
            }
        }
	}
}

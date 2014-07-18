// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    /// <summary>
    /// This tool doesn't have significant value until the DataValue tree gets changed and I can assign my own destination (ie after multiple tools and calcs can write into the same data value)
    /// </summary>
    public class CopyValueToGlobalDefinition : NetCams.ToolDefinition
    {
        public CopyValueToGlobalDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new CopyValueToGlobalInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceValue != null && mSourceValue.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceValue != null) result = Math.Max(result, mSourceValue.ToolMapRow);
                return result + 1;
			}
		}

        private DataValueDefinition mSourceValue;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition SourceValue
        {
            get { return mSourceValue; }
            set
            {
                HandlePropertyChange(this, "SourceValue", mSourceValue, value);
                mSourceValue = value;
            }
        }

        private GlobalValue mDestinationValue = null;
        [CategoryAttribute("Output")]
        public GlobalValue DestinationValue
        {
            get { return mDestinationValue; }
            set
            {
                HandlePropertyChange(this, "DestinationValue", mDestinationValue, value);
                mDestinationValue = value;
            }
        }

    }
}

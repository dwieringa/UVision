// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Text;


namespace NetCams
{
    class AddNumbersDefinition : NetCams.ToolDefinition
    {
        public AddNumbersDefinition(TestSequence testSequence)
            : base(testSequence)
		{
			// 
			// TODO: Add constructor logic here
			//
            mResult = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "Result"));
            mResult.Type = DataType.IntegerNumber;
            mResult.AddDependency(this);
            mResult.Name = "ColorMatchCountResult";

        }

		public override void CreateInstance(TestExecution theExecution)
		{
			new AddNumbersInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mValue1 != null && mValue1.IsDependentOn(theOtherObject)) return true;
            if (mValue2 != null && mValue2.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mValue1 != null)
                {
                    result = Math.Max(result, mValue1.ToolMapRow);
                }
                if (mValue2 != null)
                {
                    result = Math.Max(result, mValue2.ToolMapRow);
                }
                return result + 1;
			}
		}


        private DataValueDefinition mValue1 = null;
        [CategoryAttribute("Input")]
        public DataValueDefinition Value1
        {
            get { return mValue1; }
            set 
            {
                HandlePropertyChange(this, "Value1", mValue1, value);
                mValue1 = value;
            }
        }
        /*        public string Value1
                {
                    get
                    {
                        if (mValue1 == null) return "";
                        return mValue1.Name;
                    }
                    set
                    {
                        mValue1 = Sequence().GetNumberObject(value);
                    }
                }*/

        private DataValueDefinition mValue2 = null;
        [CategoryAttribute("Input")]
        public DataValueDefinition Value2
        {
            get { return mValue2; }
            set 
            {
                HandlePropertyChange(this, "Value2", mValue2, value);
                mValue2 = value;
            }
        }

/*        public string Value2
        {
            get
            {
                if (mValue2 == null) return "";
                return mValue2.Name;
            }
            set
            {
                mValue2 = Sequence().GetNumberObject(value);
            }
        }*/

        private GeneratedValueDefinition mResult = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition Result
        {
            get { return mResult; }
        }

    }
}

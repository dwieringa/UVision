// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class OperatorQueryDefinition : NetCams.ToolDefinition
    {
        public OperatorQueryDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mOperatorAnswer = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "OperatorAnswer"));
            mOperatorAnswer.Type = DataType.IntegerNumber;
            mOperatorAnswer.AddDependency(this);
            mOperatorAnswer.Name = "OperatorAnswer";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new OperatorQueryInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mEnabled != null && mEnabled.IsDependentOn(theOtherObject)) return true;
            foreach (DataValueDefinition value in mValuesToReference)
            {
                if (value != null && value.IsDependentOn(theOtherObject)) return true;
            }
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mEnabled != null) result = Math.Max(result, mEnabled.ToolMapRow);
                foreach (DataValueDefinition value in mValuesToReference)
                {
                    if (value != null) result = Math.Max(result, value.ToolMapRow);
                }
                return result + 1;
			}
		}

        private DataValueDefinition mEnabled;
        [CategoryAttribute("Administrative"),
        DescriptionAttribute("")]
        public DataValueDefinition Enabled
        {
            get { return mEnabled; }
            set
            {
                HandlePropertyChange(this, "Enabled", mEnabled, value);
                mEnabled = value;
            }
        }

        public List<DataValueDefinition> mValuesToReference = new List<DataValueDefinition>();
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("A comma-seperated list of values to be used within the message.")]
        public String ValuesToReference
        {
            get
            {
                // this isn't particularly efficient, but without building this each time I can't think of a way to keep it accurate if someone changes an object's name (listen for Name changes of objects we're dependent on?)
                string result = string.Empty;
                string seperator = string.Empty;
                foreach (DataValueDefinition value in mValuesToReference)
                {
                    result += seperator + value.Name;
                    seperator = ", ";
                }
                return result;
            }
            set
            {
                string[] valuesToLog_array = value.Split(new char[] { ',' });
                string valueName_trimmed = string.Empty;
                mValuesToReference.Clear();
                foreach (string valueName in valuesToLog_array)
                {
                    valueName_trimmed = valueName.Trim();
                    if (valueName_trimmed.Length > 0)
                    {
                        mValuesToReference.Add(TestSequence().DataValueRegistry.GetObject(valueName_trimmed));
                    }
                }
            }
        }

        private String mQueryMessage;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public String QueryMessage
        {
            get { return mQueryMessage; }
            set
            {
                HandlePropertyChange(this, "QueryMessage", mQueryMessage, value);
                mQueryMessage = value;
            }
        }

        private GeneratedValueDefinition mOperatorAnswer = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition OperatorAnswer
        {
            get { return mOperatorAnswer; }
        }

    }
}

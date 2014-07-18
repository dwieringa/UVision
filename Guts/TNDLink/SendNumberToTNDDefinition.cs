// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    class SendNumberToTNDDefinition : ToolDefinition
    {
        public SendNumberToTNDDefinition(TestSequence testSequence)
            : base(testSequence)
		{
		}

        public override void CreateInstance(TestExecution testExecution)
        {
            new SendNumberToTNDInstance(this, testExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mNumberToWrite.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mNumberToWrite != null) result = Math.Max(result, mNumberToWrite.ToolMapRow);
                return result + 1;
            }
        }

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

        private DataValueDefinition mNumberToWrite;
        public string NumberToWrite
        {
            get
            {
                if (mNumberToWrite == null) return "";
                return mNumberToWrite.Name;
            }
            set
            {
                HandlePropertyChange(this, "NumberToWrite", mNumberToWrite, value);
                mNumberToWrite = TestSequence().DataValueRegistry.GetObject(value);
            }
        }

    }
}

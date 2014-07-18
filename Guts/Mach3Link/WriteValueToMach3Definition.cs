// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class WriteValueToMach3Definition : ToolDefinition
    {
        public WriteValueToMach3Definition(TestSequence testSequence)
            : base(testSequence)
        {
            
        }

        public override void CreateInstance(TestExecution testExecution)
        {
            new WriteValueToMach3Instance(this, testExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (ValueToWrite.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (ValueToWrite != null) result = Math.Max(result, ValueToWrite.ToolMapRow);
                return result + 1;
            }
        }

        private Mach3ULink.DROs mMach3DRO;
        public Mach3ULink.DROs Mach3DRO
        {
            get { return mMach3DRO; }
            set 
            {
                HandlePropertyChange(this, "Mach3DRO", mMach3DRO, value);
                mMach3DRO = value;
            }
        }

        private DataValueDefinition mValueToWrite;
        public DataValueDefinition ValueToWrite
        {
            get { return mValueToWrite; }
            set
            {
                HandlePropertyChange(this, "ValueToWrite", mValueToWrite, value);
                mValueToWrite = value;
            }
        }

        private DataValueDefinition mEabled;
        public DataValueDefinition Enabled
        {
            get { return mEabled; }
            set
            {
                HandlePropertyChange(this, "Enabled", mEabled, value);
                mEabled = value;
            }
        }
    }
}

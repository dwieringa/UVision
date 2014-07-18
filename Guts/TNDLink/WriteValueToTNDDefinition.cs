// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class WriteValueToTNDDefinition : ToolDefinition
    {
        public WriteValueToTNDDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            
        }

        public override void CreateInstance(TestExecution testExecution)
        {
            new WriteValueToTNDInstance(this, testExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (ValueToWrite != null && ValueToWrite.IsDependentOn(theOtherObject)) return true;
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

        private TNDWriteRequest_DataValueObject mTNDWriteRequest = new TNDWriteRequest_DataValueObject();
        public TNDWriteRequest_DataValueObject GetTNDWriteRequest()
        {
            return mTNDWriteRequest;
        }

        public TNDnTagWriter TNDWriter
        {
            get { return mTNDWriteRequest.TNDWriter; }
            set 
            {
                HandlePropertyChange(this, "TNDWriter", mTNDWriteRequest.TNDWriter, value);
                mTNDWriteRequest.TNDWriter = value;
            }
        }

        public Int16 TNDDataViewIndex
        {
            get { return mTNDWriteRequest.TNDDataViewIndex; }
            set 
            {
                HandlePropertyChange(this, "TNDDataViewIndex", mTNDWriteRequest.TNDDataViewIndex, value);
                mTNDWriteRequest.TNDDataViewIndex = value;
            }
        }

        public TNDLink.TNDDataTypeEnum TNDDataType
        {
            get { return mTNDWriteRequest.TNDDataType; }
            set 
            {
                HandlePropertyChange(this, "TNDDataType", mTNDWriteRequest.TNDDataType, value);
                mTNDWriteRequest.TNDDataType = value;
            }
        }

        public DataValueDefinition ValueToWrite
        {
            get
            {
                return mTNDWriteRequest.DataValue;
            }
            set
            {
                HandlePropertyChange(this, "ValueToWrite", mTNDWriteRequest.DataValue, value);
                mTNDWriteRequest.DataValue = value;
            }
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class ReadValueFromTNDDefinition : ToolDefinition
    {
        // TODO: plan on multiple types of ReadRequests...  One will copy snapshot of TND value into a DataValueInstance, another will watch a TND value as a trigger, etc
        public ReadValueFromTNDDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            mDataValue = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "DataValue"));
            mDataValue.AddDependency(this);
            mDataValue.Name = "TND Data Value";
        }

        private GeneratedValueDefinition mDataValue;
        public GeneratedValueDefinition DataValue
        {
            get { return mDataValue; }
        }


        public TNDReadRequest TNDReadRequest = new TNDReadRequest();
        public TNDnTagReader TNDReader
        {
            get { return TNDReadRequest.TNDReader; }
            set 
            {
                HandlePropertyChange(this, "TNDReader", TNDReadRequest.TNDReader, value);
                TNDReadRequest.TNDReader = value;
            }
        }

        private short mDataViewIndex;
        public short TNDDataViewIndex
        {
            get { return TNDReadRequest.TNDDataViewIndex; }
            set 
            {
                HandlePropertyChange(this, "TNDDataViewIndex", TNDReadRequest.TNDDataViewIndex, value);
                TNDReadRequest.TNDDataViewIndex = value;
            }
        }

        private TNDLink.TNDDataTypeEnum mDataViewType;
        public TNDLink.TNDDataTypeEnum TNDDataType
        {
            get { return TNDReadRequest.TNDDataType; }
            set 
            {
                HandlePropertyChange(this, "TNDDataType", TNDReadRequest.TNDDataType, value);
                TNDReadRequest.TNDDataType = value;
            }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new ReadValueFromTNDInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            //if (mTNDReader != null && mTNDReader.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                //if (mTNDReader != null) result = Math.Max(result, mTNDReader.ToolMapRow);
                return result + 1;
            }
        }
    }
}

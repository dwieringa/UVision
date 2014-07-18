// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class TNDnTagNumberTriggerDefinition : TriggerDefinition
    {
        public TNDnTagNumberTriggerDefinition(TestSequence testSeqeuence)
            : base(testSeqeuence)
        {
            TNDReadRequest.PermanentRequest = true; // since updating the taglist takes time, we'll just continuously poll TND even when there isn't a listener registered with us...(since we know it will only be for a short period compared to how long we will have a listener)
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

        /*
        //public TNDWriteRequest_OneShotConstantBoolean TNDWriteRequest = new TNDWriteRequest_OneShotConstantBoolean(false);
        public TNDnTagWriter TNDWriter
        {
            get { return null; }
            set { throw new ArgumentException("Not supported");}
            //get { return TNDWriteRequest.TNDWriter; }
            /*set 
            {
                HandlePropertyChange(this, "TNDWriter", TNDWriteRequest.TNDWriter, value);
                TNDWriteRequest.TNDWriter = value;
            }* /
        }*/

        public short TNDDataViewIndex
        {
            get { return TNDReadRequest.TNDDataViewIndex; }
            set
            {
                HandlePropertyChange(this, "TNDDataViewIndex", TNDReadRequest.TNDDataViewIndex, value);
                TNDReadRequest.TNDDataViewIndex = value;
                //TNDWriteRequest.TNDDataViewIndex = value;
            }
        }

        public TNDLink.TNDDataTypeEnum TNDDataType
        {
            get { return TNDReadRequest.TNDDataType; }
            set
            {
                HandlePropertyChange(this, "TNDDataType", TNDReadRequest.TNDDataType, value);
                TNDReadRequest.TNDDataType = value;
                //TNDWriteRequest.TNDDataType = value;
            }
        }

        private bool mEnabled = false;
        public override bool TriggerEnabled
        {
            get { return mEnabled; }
            set 
            {
                HandlePropertyChange(this, "TriggerEnabled", mEnabled, value);
                mEnabled = value;
            }
        }

        private int mTriggerValue = 1;
        public int TriggerValue
        {
            get { return mTriggerValue; }
            set 
            {
                HandlePropertyChange(this, "TriggerValue", mTriggerValue, value);
                mTriggerValue = value;
            }
        }

        private int mAckValue = 0;
        public int AckValue
        {
            get { return mAckValue; }
            set
            {
                HandlePropertyChange(this, "AckValue", mAckValue, value);
                mAckValue = value;
            }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new TNDnTagNumberTriggerInstance(this, theExecution);
        }
    }
}

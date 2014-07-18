// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class ModbusFlagTriggerDefinition : TriggerDefinition
    {
        public enum TriggerStates : ushort
        {
            Undefined = 0,
            Ready = 1,
            Busy = 2,
            Done = 3
        }

        public ModbusFlagTriggerDefinition(TestSequence testSeqeuence)
            : base(testSeqeuence)
        {
        }

        //private ModbusTCPSlave mModbusTCPSlave = null;
        public ModbusTCPSlaveDevice ModbusTCPSlave
        {
            get { return Project().globalModbusSlave; }
/*            set 
            {
                HandlePropertyChange(this, "ModbusTCPSlave", mModbusTCPSlave, value);
                mModbusTCPSlave = value;
            }*/
        }

        private short mCoilForTrigger = 1;
        public short CoilForTrigger
        {
            get { return mCoilForTrigger; }
            set
            {
                HandlePropertyChange(this, "CoilForTrigger", mCoilForTrigger, value);
                // TODO: ensure this is a valid value for the selected ModbusSlave
                mCoilForTrigger = value;
            }
        }

        private short mInputRegisterForStatus = 1;
        public short InputRegisterForStatus
        {
            get { return mInputRegisterForStatus; }
            set
            {
                HandlePropertyChange(this, "InputRegisterForStatus", mInputRegisterForStatus, value);
                // TODO: ensure this is a valid value for the selected ModbusSlave
                mInputRegisterForStatus = value;
            }
        }

        private bool mEnabled = false;
        public override bool TriggerEnabled
        {
            get { return mEnabled; }
            set 
            {
                HandlePropertyChange(this, "Enabled", mEnabled, value);
                mEnabled = value;
            }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new ModbusFlagTriggerInstance(this, theExecution);
        }
    }
}

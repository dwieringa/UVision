// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class WriteValueToModbusDefinition : ToolDefinition
    {
        public WriteValueToModbusDefinition(TestSequence testSequence)
            : base(testSequence)
        {
        }

        public override void CreateInstance(TestExecution testExecution)
        {
            new WriteValueToModbusInstance(this, testExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mValueToWrite != null && mValueToWrite.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mValueToWrite != null) result = Math.Max(result, mValueToWrite.ToolMapRow);
                return result + 1;
            }
        }

        /// <summary>
        /// TO DO: allow them to select a remote slave (e.g. a numatics value bank)
        /// TO DO: also should support serial modbus slave in addition to TCP
        /// </summary>
        private ModbusTCPSlaveDevice mModbusSlave;
        public ModbusTCPSlaveDevice ModbusSlave
        {
            get { return Project().globalModbusSlave; }
            /*
            set 
            {
                HandlePropertyChange(this, "ModbusSlave", mModbusSlave, value);
                mModbusSlave = value;
            }
             */
        }

        private short mAddressWithinModule = 1;
        public short AddressWithinModule
        {
            get { return mAddressWithinModule; }
            set
            {
                HandlePropertyChange(this, "AddressToWrite", mAddressWithinModule, value);
                // TODO: ensure this is a valid value for the selected ModbusSlave
                mAddressWithinModule = value;
            }
        }

        private ModbusModules mModuleToWriteTo = ModbusModules.InputRegisters;
        public ModbusModules ModuleToWriteTo
        {
            get { return mModuleToWriteTo; }
            set 
            {
                HandlePropertyChange(this, "ModuleToWriteTo", mModuleToWriteTo, value);
                mModuleToWriteTo = value;
            }
        }

        private DataValueDefinition mValueToWrite;
        public DataValueDefinition ValueToWrite
        {
            get
            {
                return mValueToWrite;
            }
            set
            {
                HandlePropertyChange(this, "ValueToWrite", mValueToWrite, value);
                mValueToWrite = value;
            }
        }
    }
}

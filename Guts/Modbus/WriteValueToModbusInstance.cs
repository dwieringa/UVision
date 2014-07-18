    // Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetCams
{
    class WriteValueToModbusInstance : ToolInstance
    {
        public WriteValueToModbusInstance(WriteValueToModbusDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.ValueToWrite == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to ValueToWrite");
            mValueToWrite = TestExecution().DataValueRegistry.GetObject(theDefinition.ValueToWrite.Name);

            mModbusSlave = theDefinition.ModbusSlave;

            if (mModbusSlave == null) throw new ArgumentException("ModbusSlave not defined in Modbus writer '" + Name + "'");
            mModbusDataStore = mModbusSlave.DataStore;

            mModuleToWriteTo = theDefinition.ModuleToWriteTo;
            int maxAddress = -1;
            switch (mModuleToWriteTo)
            {
                case ModbusModules.Coils:
                    maxAddress = mModbusDataStore.CoilDiscretes.Count;
                    break;
                case ModbusModules.InputDiscretes:
                    maxAddress = mModbusDataStore.InputDiscretes.Count;
                    break;
                case ModbusModules.InputRegisters:
                    maxAddress = mModbusDataStore.InputRegisters.Count;
                    break;
                case ModbusModules.HoldingRegisters:
                    maxAddress = mModbusDataStore.HoldingRegisters.Count;
                    break;
                case ModbusModules.Undefined:
                    throw new ArgumentException("WriteValueToModbus " + Name + " doesn't have ModuleToWriteTo defined.");
                    break;
                default:
                    throw new ArgumentException("WriteValueToModbus " + Name + " has an unsupported ModuleToWriteTo.");
                    break;
            }

            mAddressWithinModule = theDefinition.AddressWithinModule;
            if (mAddressWithinModule < 1 || mAddressWithinModule >= maxAddress)
            {
                // NOTE: for 100 coils, we use 99.  1-99.  TND doesn't access the 0'th coil. The modbus implementation uses collection slots 0-99, but 0 doesn't seem to be used.
                throw new ArgumentException("WriteValueToModbus " + Name + " has an invalid address for ModuleToWriteTo.  The value must be between 1 and " + maxAddress + " for the selected module.");
            }
        }

        public override void DoWork()
        {
            TestExecution().LogMessageWithTimeFromTrigger("Modbus Write " + Name + " started");
            if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger("Modbus Write " + Name + ": prerequisites not met. Skipping.");
            }
            else
            {
                try
                {
                    bool success = false;
                    switch (mModuleToWriteTo)
                    {
                        case ModbusModules.Coils:
                            mModbusDataStore.CoilDiscretes[mAddressWithinModule] = mValueToWrite.ValueAsBoolean();
                            success = true;
                            break;
                        case ModbusModules.InputDiscretes:
                            mModbusDataStore.InputDiscretes[mAddressWithinModule] = mValueToWrite.ValueAsBoolean();
                            success = true;
                            break;
                        case ModbusModules.InputRegisters:
                            long valueToWriteAsLong = mValueToWrite.ValueAsLong();
                            if (valueToWriteAsLong > ushort.MaxValue)
                            {
                                TestExecution().LogErrorWithTimeFromTrigger("WriteValueToModbus " + Name + " value too large to fit in register.  Value=" + valueToWriteAsLong + "; Limit=" + ushort.MaxValue);
                            }
                            else
                            {
                                mModbusDataStore.InputRegisters[mAddressWithinModule] = (ushort)valueToWriteAsLong;
                                success = true;
                            }
                            break;
                        case ModbusModules.HoldingRegisters:
                            valueToWriteAsLong = mValueToWrite.ValueAsLong();
                            if (valueToWriteAsLong > ushort.MaxValue)
                            {
                                TestExecution().LogErrorWithTimeFromTrigger("WriteValueToModbus " + Name + " value too large to fit in register.  Value=" + valueToWriteAsLong + "; Limit=" + ushort.MaxValue);
                            }
                            else
                            {
                                mModbusDataStore.HoldingRegisters[mAddressWithinModule] = (ushort)valueToWriteAsLong;
                                success = true;
                            }
                            break;
                        case ModbusModules.Undefined:
                            TestExecution().LogErrorWithTimeFromTrigger("WriteValueToModbus " + Name + " doesn't have ModuleToWriteTo defined.");
                            break;
                        default:
                            TestExecution().LogErrorWithTimeFromTrigger("WriteValueToModbus " + Name + " has an unsupported ModuleToWriteTo.");
                            break;
                    }

                    TestExecution().LogMessageWithTimeFromTrigger("WriteValueToModbus " + Name + " completed");
                }
                catch(Exception e)
                {
                    TestExecution().LogErrorWithTimeFromTrigger("WriteValueToModbus " + Name + " failed. Exception = '" + e.Message + "'");
                }
            }
            mIsComplete = true;
        }

		public override bool IsComplete() { return mIsComplete; }
		private bool mIsComplete = false;

        private DataValueInstance mValueToWrite;
        public string ValueToWrite
        {
            get
            {
                if (mValueToWrite == null) return "";
                return mValueToWrite.Name + " (" + mValueToWrite.ValueAsLong() + ")";
            }
        }

        private Modbus.Data.DataStore mModbusDataStore = null;
        private ModbusTCPSlaveDevice mModbusSlave;
        public ModbusTCPSlaveDevice ModbusSlave
        {
            get { return mModbusSlave; }
        }

        private short mAddressWithinModule = 1;
        public short AddressWithinModule
        {
            get { return mAddressWithinModule; }
        }

        private ModbusModules mModuleToWriteTo = ModbusModules.InputRegisters;
        public ModbusModules ModuleToWriteTo
        {
            get { return mModuleToWriteTo; }
        }

    }
}

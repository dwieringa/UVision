// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetCams
{
    class ModbusFlagTriggerInstance : TriggerInstance
    {
        public ModbusFlagTriggerInstance(ModbusFlagTriggerDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            if (theDefinition.ModbusTCPSlave == null) throw new ArgumentException("ModbusTCPSlave not defined in Flag Trigger '" + Name + "'");
            mModbusDataStore = theDefinition.ModbusTCPSlave.DataStore;

            mCoilForTrigger = theDefinition.CoilForTrigger;
            if (mCoilForTrigger < 1 || mCoilForTrigger >= mModbusDataStore.CoilDiscretes.Count)
            {
                // NOTE: for 100 coils, we use 99.  1-99.  TND doesn't access the 0'th coil. The modbus implementation uses collection slots 0-99, but 0 doesn't seem to be used.
                throw new ArgumentException("ModbusFlagTrigger " + Name + " has an invalid address for CoilForTrigger.  The value must be between 1 and " + mModbusDataStore.CoilDiscretes.Count + " for the selected Modbus data store.");
            }

            mInputRegisterForStatus = theDefinition.InputRegisterForStatus;
            if (mInputRegisterForStatus < 1 || mInputRegisterForStatus >= mModbusDataStore.InputRegisters.Count)
            {
                throw new ArgumentException("ModbusFlagTrigger " + Name + " has an invalid address for InputRegisterForStatus.  The value must be between 1 and " + mModbusDataStore.InputRegisters.Count + " for the selected Modbus data store.");
            }

            //TestExecution().LogMessageWithTimeFromCreated("ModbusFlagTrigger " + Name + " is set up");
        }

        public override bool CheckTrigger()
        {
            if (mModbusDataStore.InputRegisters[mInputRegisterForStatus] != (ushort)ModbusFlagTriggerDefinition.TriggerStates.Ready)
            {
                switch( mModbusDataStore.InputRegisters[mInputRegisterForStatus])
                {
                    case (ushort)ModbusFlagTriggerDefinition.TriggerStates.Undefined:
                        mModbusDataStore.InputRegisters[mInputRegisterForStatus] = (ushort)ModbusFlagTriggerDefinition.TriggerStates.Ready;
                        break;
                    case (ushort)ModbusFlagTriggerDefinition.TriggerStates.Done:
                        if (mModbusDataStore.CoilDiscretes[mCoilForTrigger]) return false; // we don't clear Done status until TND clears the trigger from last cycle
                        mModbusDataStore.InputRegisters[mInputRegisterForStatus] = (ushort)ModbusFlagTriggerDefinition.TriggerStates.Ready;
                        break;
                    default:
                        TestExecution().LogErrorWithTimeFromTrigger(Name + ": The Modbus input register used for trigger status is in an invalid state ('" + mModbusDataStore.InputRegisters[mInputRegisterForStatus] + "') while waiting for a trigger. Is the address accidentally being used for more than one purpose?");
                        break;
                }
                TestExecution().LogErrorWithTimeFromTrigger(Name + ": The Modbus input register used for trigger status isn't set to '" + ModbusFlagTriggerDefinition.TriggerStates.Ready + "' at the end of the test execution. Is the address accidentally being used for more than one purpose?");
            }

            // TODO: test invalid coil address
            if (mModbusDataStore.CoilDiscretes[mCoilForTrigger])
            {
                TestExecution().LogMessageWithTimeFromCreated("ModbusFlagTrigger " + Name + " triggered");

                TestExecution().ExecutionCompleted += new TestExecution.ExecutionCompletedDelegate(ModbusFlagTriggerInstance_ExecutionCompleted);
                if (mModbusDataStore.InputRegisters[mInputRegisterForStatus] != (ushort)ModbusFlagTriggerDefinition.TriggerStates.Busy)
                {
                    TestExecution().LogErrorWithTimeFromTrigger(Name + ": The Modbus input register used for trigger status isn't set to '" + ModbusFlagTriggerDefinition.TriggerStates.Ready + "' at the end of the test execution. Is the address accidentally being used for more than one purpose?");
                }
                mModbusDataStore.InputRegisters[mInputRegisterForStatus] = (ushort)ModbusFlagTriggerDefinition.TriggerStates.Busy;
                mIsComplete = true;//TND_WRITE_HACK
            }

            return mIsComplete;
        }

        void ModbusFlagTriggerInstance_ExecutionCompleted()
        {
            TestExecution().ExecutionCompleted -= new TestExecution.ExecutionCompletedDelegate(ModbusFlagTriggerInstance_ExecutionCompleted);
            if (mModbusDataStore.InputRegisters[mInputRegisterForStatus] != (ushort)ModbusFlagTriggerDefinition.TriggerStates.Busy)
            {
                TestExecution().LogErrorWithTimeFromTrigger(Name + ": The Modbus input register used for trigger status isn't set to '" + ModbusFlagTriggerDefinition.TriggerStates.Busy + "' at the end of the test execution. Is the address accidentally being used for more than one purpose?");
            }
            mModbusDataStore.InputRegisters[mInputRegisterForStatus] = (ushort)ModbusFlagTriggerDefinition.TriggerStates.Done;
        }

        private Modbus.Data.DataStore mModbusDataStore = null;
        private short mCoilForTrigger;
        private short mInputRegisterForStatus;
        private bool mIsComplete = false;
        public override bool IsComplete() { return mIsComplete; }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetCams
{
    class WriteValueToMach3Instance : ToolInstance
    {
        public WriteValueToMach3Instance(WriteValueToMach3Definition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.ValueToWrite == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to ValueToWrite");
            mValueToWrite = TestExecution().DataValueRegistry.GetObject(theDefinition.ValueToWrite.Name);

            if (theDefinition.Enabled == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to Enabled");
            mEnabled = TestExecution().DataValueRegistry.GetObject(theDefinition.Enabled.Name);
        }

        public override void DoWork()
        {
            /*
            try
            {
                LocalMach3.Singleton.Reset();
                LocalMach3.Singleton.LoadGCodeFile("C:\\Mach3\\GCode\\roadrunner.tap");
                LocalMach3.Singleton.SetOEMDRO((short)Mach3ULink.DROs.X_WORK_OFFSET_DRO, DELETETHIS++);
                LocalMach3.Singleton.SetOEMDRO((short)Mach3ULink.DROs.X_MACHINE_COORD_DRO, DELETETHIS / 2);
            }
            catch (ArgumentException exception)
            {
                TestSequence().Window().logMessage("error talking to Mach3: " + exception.Message);
            }
            */
             
            //TND_WRITE_HACK START
            TestExecution().LogMessageWithTimeFromTrigger(Name + " started");
            if (mEnabled == null || !mEnabled.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + " disabled. Did NOT write value to Mach3.");
            }
            else if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + ": prerequisites not met. Skipping.");
            }
            else
            {
                WriteValueToMach3Definition theDef = (WriteValueToMach3Definition)Definition();
                try
                {
                    LocalMach3.Singleton.SetOEMDRO((short)theDef.Mach3DRO, mValueToWrite.ValueAsDecimal());
                }
                catch (ArgumentException e)
                {
                    TestExecution().LogErrorWithTimeFromTrigger(Name + " couldn't write to Mach3; message=" + e.Message);
                }
            }

            mIsComplete = true;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " completed");
            /* // TODO: optionally get value from Mach3 to verify?  any return value from SetOEMDRO?  just exceptions?
            if (success)
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + " completed");
            }
            else
            {
                TestExecution().LogErrorWithTimeFromTrigger( Name + " completed with errors");
            }
             */
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

        private DataValueInstance mEnabled;
        public DataValueInstance Enabled
        {
            get { return mEnabled; }
        }
    }
}

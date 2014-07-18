    // Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetCams
{
    class WriteValueToTNDInstance : ToolInstance
    {
        public WriteValueToTNDInstance(WriteValueToTNDDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.ValueToWrite == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to ValueToWrite");
            mValueToWrite = TestExecution().DataValueRegistry.GetObject(theDefinition.ValueToWrite.Name);
        }

        public override void DoWork()
        {
            TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " started");
            if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + ": prerequisites not met. Skipping.");
            }
            else
            {
                //TND_WRITE_HACK START
                bool success = false;
                WriteValueToTNDDefinition theDef = (WriteValueToTNDDefinition)Definition();
                switch (theDef.TNDDataType)
                {
                    case TNDLink.TNDDataTypeEnum.Flag:
                        goto case TNDLink.TNDDataTypeEnum.Number;
                    case TNDLink.TNDDataTypeEnum.Counter:
                        goto case TNDLink.TNDDataTypeEnum.Number;
                    case TNDLink.TNDDataTypeEnum.Number:
                        success = Definition().Window().myTNDLink.WriteToTNDByTypeIndex((short)theDef.TNDDataType, theDef.TNDDataViewIndex, (int)mValueToWrite.ValueAsLong(), "writing");
                        break;
                    case TNDLink.TNDDataTypeEnum.Float:
                        success = Definition().Window().myTNDLink.WriteFloatToTNDByIndex(theDef.TNDDataViewIndex, (float)mValueToWrite.ValueAsDecimal(), "writing");
                        break;
                }
                if (success)
                {
                    TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " completed (old link)");
                }
                else
                {
                    TestExecution().LogMessageWithTimeFromTrigger("ERROR: TND Write " + Name + " completed with errors (old link)");
                }
                //TND_WRITE_HACK END

                /* //TND_WRITE_HACK
                TNDWriteRequest_DataValueObject writeRequest = ((WriteValueToTNDDefinition)Definition()).GetTNDWriteRequest();
                if (!mRequestedWrite)
                {
                    TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " started");

                    if (writeRequest.TNDWriter.Connected)
                    {
                        //TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " step 1");
                        writeRequest.DataValueInstance = mValueToWrite;
                        //TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " step 2");
                        writeRequest.Active = true;
                        //TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " step 3");
                        mRequestedWrite = true;
                        TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " step 4");
                    }
                    else
                    {
                        TestExecution().LogMessageWithTimeFromTrigger("WARNING: " + Name + " didn't write value to TND since connection was down");
                        TestExecution().Window().logMessage("WARNING: " + Name + " didn't write value to TND since connection was down");
                        mIsComplete = true;
                        TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " completed without write");
                    }
                }
                else
                {
                    /*
                    TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " checking if write active"
                        + " " + writeRequest.Active
                        + " " + writeRequest.TNDWriter.Polling
                        + " " + writeRequest.TNDWriter.NumberOfWriteRequests_Active
                        + " " + writeRequest.TNDWriter.TagListDirty
                        + " " + writeRequest.TNDWriter.mTimeoutStateIndicator
                        );
                     * /
                    if (!writeRequest.Active)
                    {
                        mIsComplete = true;
                        TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " completed");
                    }
                    //else
                    //{
                    //    writeRequest.TNDWriter.TagListDirty = true; // HACK!!!
                    //}
                }
                 */
                //TND_WRITE_HACK
            }
            mIsComplete = true;
        }

        private bool mRequestedWrite = false;

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
    }
}

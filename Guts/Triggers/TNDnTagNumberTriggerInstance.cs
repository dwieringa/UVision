// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetCams
{
    class TNDnTagNumberTriggerInstance : TriggerInstance
    {
        public TNDnTagNumberTriggerInstance(TNDnTagNumberTriggerDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            mTriggerValue = theDefinition.TriggerValue;
            mAckValue = theDefinition.AckValue;

            if (theDefinition.TNDReader == null) throw new ArgumentException("TND Reader not defined in Number Trigger '" + Name + "'");

            mIntDelegate = new TNDReadRequest.WholeNumberDelegate(HandleValue);
            theDefinition.TNDReadRequest.AddValueListener(mIntDelegate);
            theDefinition.TNDReadRequest.Active = true; // we set the request as Permanent 1/11/08 when it is created by our def object; so this should only do something the first test execution
            TestExecution().LogMessageWithTimeFromCreated("TNDNumberTrigger " + Name + " started polling");
        }

        private TNDReadRequest.WholeNumberDelegate mIntDelegate;

        public void HandleValue(long valueFromTND)
        {
            // WARNING: don't block in here...this is called from the TNDnTagReader between each value. if we block (e.g. we used to block waiting for the flag to be cleared via a TNDnTagWrite), we can screw things up
            if (valueFromTND == mTriggerValue)
            {
                TNDnTagNumberTriggerDefinition defObject = (TNDnTagNumberTriggerDefinition)Definition();
                defObject.TNDReadRequest.RemoveValueListener(mIntDelegate);
                //defObject.TNDReadRequest.SuggestGoingInActive();

                TestExecution().LogMessageWithTimeFromCreated("TNDNumberTrigger " + Name + " received trigger value of " + valueFromTND);

                /* //TND_WRITE_HACK
                // turn off flag via Writer to TND
                defObject.TNDWriteRequest.SetBooleanValue(false);
                TestExecution().LogMessageWithTimeFromCreated("TNDNumberTrigger " + Name + " step 1");
                defObject.TNDWriteRequest.Active = true; // because it is a one-shot write, it is automatically goes Inactive after each write.
                TestExecution().LogMessageWithTimeFromCreated("TNDNumberTrigger " + Name + " step 2");
                mRequestedWrite = true;
                TestExecution().LogMessageWithTimeFromCreated("TNDNumberTrigger " + Name + " requested flag clearing");
                // */ //TND_WRITE_HACK
                mRequestedWrite = true;//TND_WRITE_HACK
            }
        }

        public override void PostExecutionCleanup()
        {
            ((TNDnTagNumberTriggerDefinition)Definition()).TNDReadRequest.RemoveValueListener(mIntDelegate);
            //((TNDnTagNumberTriggerDefinition)Definition()).TNDReadRequest.SuggestGoingInActive();
        }

        public override bool CheckTrigger()
        {
            if (mRequestedWrite)//TND_WRITE_HACK START
            {
                bool success = Definition().Window().myTNDLink.WriteToTNDByTypeIndex(TNDLink_old.NumberType, ((TNDnTagNumberTriggerDefinition)Definition()).TNDDataViewIndex, mAckValue, "writing");//TND_WRITE_HACK
                mIsComplete = true;//TND_WRITE_HACK
                TestExecution().LogMessageWithTimeFromCreated("TNDNumberTrigger " + Name + " wrote ack value (old link)");//TND_WRITE_HACK
            }//TND_WRITE_HACK END

            /* //TND_WRITE_HACK
            TNDnTagNumberTriggerDefinition theDefinition = (TNDnTagNumberTriggerDefinition)Definition();
            if (mRequestedWrite && !theDefinition.TNDWriteRequest.Active)
            {
                TestExecution().LogMessageWithTimeFromCreated("TNDNumberTrigger " + Name + " completed");
                mIsComplete = true;
            }
             */ //TND_WRITE_HACK

/*            if (!mRequestedWrite && !theDefinition.TNDReadRequest.Active)
            {
                Thread.Sleep(100);
                if (!mRequestedWrite && !theDefinition.TNDReadRequest.Active) // re-test for thread safety
                {
                    throw new ArgumentException("how did we get here? 239384");
                }
            }*/
            return mIsComplete;
        }

        private int mTriggerValue = 1;
        public int TriggerValue
        {
            get { return mTriggerValue; }
        }

        private int mAckValue = 0;
        public int AckValue
        {
            get { return mAckValue; }
        }

        private bool mRequestedWrite = false;
        private bool mIsComplete = false;
        public override bool IsComplete() { return mIsComplete; }
    }
}

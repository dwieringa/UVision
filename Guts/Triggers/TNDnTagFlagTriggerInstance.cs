// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetCams
{
    class TNDnTagFlagTriggerInstance : TriggerInstance
    {
        public TNDnTagFlagTriggerInstance(TNDnTagFlagTriggerDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            if (theDefinition.TNDReader == null) throw new ArgumentException("TND Reader not defined in Flag Trigger '" + Name + "'");

            mBoolDelegate = new TNDReadRequest.BoolDelegate(HandleValue);
            theDefinition.TNDReadRequest.AddValueListener(mBoolDelegate);
            theDefinition.TNDReadRequest.Active = true; // we set the request as Permanent 1/11/08 when it is created by our def object; so this should only do something the first test execution
            TestExecution().LogMessageWithTimeFromCreated("TNDFlagTrigger " + Name + " started polling");
        }

        private TNDReadRequest.BoolDelegate mBoolDelegate;

        public void HandleValue(bool valueFromTND)
        {
            // WARNING: don't block in here...this is called from the TNDnTagReader between each value. if we block (e.g. we used to block waiting for the flag to be cleared via a TNDnTagWrite), we can screw things up
            if (valueFromTND)
            {
                TNDnTagFlagTriggerDefinition defObject = (TNDnTagFlagTriggerDefinition)Definition();
                defObject.TNDReadRequest.RemoveValueListener(mBoolDelegate);
                //defObject.TNDReadRequest.SuggestGoingInActive();

                TestExecution().LogMessageWithTimeFromCreated("TNDFlagTrigger " + Name + " received 'true' flag");

                /* //TND_WRITE_HACK
                // turn off flag via Writer to TND
                defObject.TNDWriteRequest.SetBooleanValue(false);
                TestExecution().LogMessageWithTimeFromCreated("TNDFlagTrigger " + Name + " step 1");
                defObject.TNDWriteRequest.Active = true; // because it is a one-shot write, it is automatically goes Inactive after each write.
                TestExecution().LogMessageWithTimeFromCreated("TNDFlagTrigger " + Name + " step 2");
                mRequestedWrite = true;
                TestExecution().LogMessageWithTimeFromCreated("TNDFlagTrigger " + Name + " requested flag clearing");
                // */ //TND_WRITE_HACK
                mRequestedWrite = true;//TND_WRITE_HACK
            }
        }

        public override void PostExecutionCleanup()
        {
            ((TNDnTagFlagTriggerDefinition)Definition()).TNDReadRequest.RemoveValueListener(mBoolDelegate);
            //((TNDnTagFlagTriggerDefinition)Definition()).TNDReadRequest.SuggestGoingInActive();
        }

        public override bool CheckTrigger()
        {
            if (mRequestedWrite)//TND_WRITE_HACK START
            {
                bool success = Definition().Window().myTNDLink.WriteToTNDByTypeIndex(TNDLink_old.FlagType, ((TNDnTagFlagTriggerDefinition)Definition()).TNDDataViewIndex, 0, "writing");//TND_WRITE_HACK
                mIsComplete = true;//TND_WRITE_HACK
                TestExecution().LogMessageWithTimeFromCreated("TNDFlagTrigger " + Name + " cleared flag (old link)");//TND_WRITE_HACK
            }//TND_WRITE_HACK END

            /* //TND_WRITE_HACK
            TNDnTagFlagTriggerDefinition theDefinition = (TNDnTagFlagTriggerDefinition)Definition();
            if (mRequestedWrite && !theDefinition.TNDWriteRequest.Active)
            {
                TestExecution().LogMessageWithTimeFromCreated("TNDFlagTrigger " + Name + " completed");
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

        private bool mRequestedWrite = false;
        private bool mIsComplete = false;
        public override bool IsComplete() { return mIsComplete; }
    }
}

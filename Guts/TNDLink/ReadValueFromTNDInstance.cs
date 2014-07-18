// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class ReadValueFromTNDInstance : ToolInstance
    {
        public ReadValueFromTNDInstance(ReadValueFromTNDDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            mDataValueInstance = new GeneratedValueInstance(theDefinition.DataValue, testExecution);
            if (mDataValueInstance.Type == DataType.NotDefined)
            {
                // we shouldn't get here since this should be trapped in the GeneratedValueInstance ctor
                throw new ArgumentException("Data Type for '" + mDataValueInstance.Name + "' is not defined. Can't copy value from TND.");
            }
        }

        private GeneratedValueInstance mDataValueInstance;
        public void HandleBoolValue(bool valueFromTND)
        {
            ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.RemoveValueListener(mBoolDelegate);
            ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.SuggestGoingInActive();

            mDataValueInstance.SetValue(valueFromTND);
            mDataValueInstance.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("TND Read " + Name + " completed (bool)");
        }

        public void HandleWholeNumber(long valueFromTND)
        {
            ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.RemoveValueListener(mWholeNumDelegate);
            ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.SuggestGoingInActive();

            mDataValueInstance.SetValue(valueFromTND);
            mDataValueInstance.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("TND Read " + Name + " completed (whole number)");
        }

        public void HandleDecimalNumber(double valueFromTND)
        {
            ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.RemoveDecimalNumberListener(mDecNumDelegate);
            ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.SuggestGoingInActive();

            mDataValueInstance.SetValue(valueFromTND);
            mDataValueInstance.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("TND Read " + Name + " completed (decimal)");
        }

        private TNDReadRequest.BoolDelegate mBoolDelegate;
        private TNDReadRequest.WholeNumberDelegate mWholeNumDelegate;
        private TNDReadRequest.DecimalNumberDelegate mDecNumDelegate;

        private bool mReadRequested = false;
        public override void DoWork()
        {
            //if (mReadRequested || !AreExplicitDependenciesComplete()) return;

            TestExecution().LogMessageWithTimeFromTrigger("TND Read " + Name + " started");

            switch (mDataValueInstance.Type)
            {
                case DataType.Boolean:
                    mBoolDelegate = new TNDReadRequest.BoolDelegate(HandleBoolValue);
                    ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.AddValueListener(mBoolDelegate);
                    break;
                case DataType.IntegerNumber:
                    mWholeNumDelegate = new TNDReadRequest.WholeNumberDelegate(HandleWholeNumber);
                    ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.AddValueListener(mWholeNumDelegate);
                    break;
                case DataType.DecimalNumber:
                    mDecNumDelegate = new TNDReadRequest.DecimalNumberDelegate(HandleDecimalNumber);
                    ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.AddDecimalNumberListener(mDecNumDelegate);
                    break;
                case DataType.NotDefined:
                    // we shouldn't get here since this should be trapped in the ctor
                    throw new ArgumentException("Data Type for '" + mDataValueInstance.Name + "' is not defined. Can't copy value from TND.");
                default:
                    throw new ArgumentException("Can't copy value from TND to data '" + mDataValueInstance.Name + "' since data type " + mDataValueInstance.Type + " isn't supported");
            }
            ((ReadValueFromTNDDefinition)Definition()).TNDReadRequest.Active = true;

            mReadRequested = true;
        }

        public override bool IsComplete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }
}

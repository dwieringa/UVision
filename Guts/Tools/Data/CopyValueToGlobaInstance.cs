// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    class CopyValueToGlobalInstance : ToolInstance
    {
        public CopyValueToGlobalInstance(CopyValueToGlobalDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.SourceValue == null) throw new ArgumentException(Name + " doesn't have SourceValue defined.");
            mSourceValue = testExecution.DataValueRegistry.GetObject(theDefinition.SourceValue.Name);

            if (theDefinition.DestinationValue == null) throw new ArgumentException(Name + " doesn't have DestinationValue defined.");
            mDestinationValue = theDefinition.DestinationValue;
        }

        private GlobalValue mDestinationValue = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public GlobalValue DestinationValue
        {
            get { return mDestinationValue; }
        }

        private DataValueInstance mSourceValue;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance SourceValue
        {
            get { return mSourceValue; }
        }

        private bool mIsComplete = false;
        public override bool IsComplete() { return mIsComplete; }

        public override void DoWork() 
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            try
            {
                if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
                {
                    TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met. Skipping.");
                }
                else
                {
                    switch (mDestinationValue.Type)
                    {
                        case DataType.Boolean:
                            mDestinationValue.SetValue(mSourceValue.ValueAsBoolean());
                            break;
                        case DataType.DecimalNumber:
                            mDestinationValue.SetValue(mSourceValue.ValueAsDecimal());
                            break;
                        case DataType.IntegerNumber:
                            mDestinationValue.SetValue(mSourceValue.ValueAsLong());
                            break;
                        case DataType.NotDefined:
                            TestExecution().LogErrorWithTimeFromTrigger(Name + " can't copy value since the destination doesn't have its type defined.");
                            /*
                            switch (mSourceValue.Type)
                            {
                                case DataType.Boolean:
                                    mDestinationValue.SetValue(mSourceValue.ValueAsBoolean());
                                    break;
                                case DataType.DecimalNumber:
                                    mDestinationValue.SetValue(mSourceValue.ValueAsDecimal());
                                    break;
                                case DataType.IntegerNumber:
                                    mDestinationValue.SetValue(mSourceValue.ValueAsLong());
                                    break;
                                case DataType.NotDefined:
                                    TestExecution().LogErrorWithTimeFromTrigger(Name + " can't copy value since neither the destination nor the source values have their type defined.");
                                    mDestinationValue.SetValue(mSourceValue.ValueAsDecimal());
                                    break;
                                default:
                                    TestExecution().LogErrorWithTimeFromTrigger(Name + " can't copy value since the source is an unsupported type.");
                                    break;
                            }
                             */
                            break;
                        default:
                            TestExecution().LogErrorWithTimeFromTrigger(Name + " can't copy value since the destination is an unsupported type.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                TestExecution().LogMessageWithTimeFromTrigger("ERROR: Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
            }
            finally
            {
            }

            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            mIsComplete = true;

            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
        }
    }
}

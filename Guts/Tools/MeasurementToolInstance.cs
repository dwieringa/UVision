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
    class MeasurementToolInstance : ToolInstance
    {
        public MeasurementToolInstance(MeasurementToolDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.ReferencePoint1 == null) throw new ArgumentException(Name + " doesn't have ReferencePoint1 defined.");
            mReferencePoint1 = testExecution.ReferencePointRegistry.GetObject(theDefinition.ReferencePoint1.Name);

            if (theDefinition.ReferencePoint2 == null) throw new ArgumentException(Name + " doesn't have ReferencePoint2 defined.");
            mReferencePoint2 = testExecution.ReferencePointRegistry.GetObject(theDefinition.ReferencePoint2.Name);

            if (theDefinition.PixelsPerUnit == null) throw new ArgumentException(Name + " doesn't have PixelsPerUnit defined.");
            mPixelsPerUnit = testExecution.DataValueRegistry.GetObject(theDefinition.PixelsPerUnit.Name);

            if (theDefinition.Ensure1Before2 != null)
            {
                mEnsure1Before2 = testExecution.DataValueRegistry.GetObject(theDefinition.Ensure1Before2.Name);
            }

            mDistance = new GeneratedValueInstance(theDefinition.Distance, testExecution);
            if (theDefinition.Distance_pixels != null)
            {
                mDistance_pixels = new GeneratedValueInstance(theDefinition.Distance_pixels, testExecution);
            }
        }

        private GeneratedValueInstance mDistance = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public GeneratedValueInstance Distance
        {
            get { return mDistance; }
        }

        private GeneratedValueInstance mDistance_pixels = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance Distance_pixels
        {
            get { return mDistance_pixels; }
        }

        private IReferencePointInstance mReferencePoint1 = null;
        [CategoryAttribute("Parameters")]
        public IReferencePointInstance ReferencePoint1
        {
            get { return mReferencePoint1; }
        }

        private IReferencePointInstance mReferencePoint2 = null;
        [CategoryAttribute("Parameters")]
        public IReferencePointInstance ReferencePoint2
        {
            get { return mReferencePoint2; }
        }

        private DataValueInstance mPixelsPerUnit;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance PixelsPerUnit
        {
            get { return mPixelsPerUnit; }
        }

        private DataValueInstance mEnsure1Before2;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance Ensure1Before2
        {
            get { return mEnsure1Before2; }
        }

        public override bool IsComplete() { return mDistance.IsComplete(); }

        public override void DoWork() 
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            double result_pixels = -1;
            double result = -1;

            if (mReferencePoint1.GetValueAsDouble() < 0 || mReferencePoint2.GetValueAsDouble() < 0)
            {
                TestExecution().LogMessage("ERROR: Reference point(s) for '" + Name + "' do not have valid values. ref1=" + mReferencePoint1.GetValueAsDouble() + " ref2=" + mReferencePoint2.GetValueAsDouble());
            }
            else if (mEnsure1Before2 != null && mEnsure1Before2.ValueAsBoolean() && mReferencePoint1.GetValueAsDouble() > mReferencePoint2.GetValueAsDouble())
            {
                TestExecution().LogMessage("ERROR: Reference point(s) for '" + Name + "' are out of order (1 > 2). ref1=" + mReferencePoint1.GetValueAsDouble() + " ref2=" + mReferencePoint2.GetValueAsDouble());
            }
            else
            {
                try
                {
                    result_pixels = Math.Abs(mReferencePoint2.GetValueAsDouble() - mReferencePoint1.GetValueAsDouble());
                    if (mPixelsPerUnit.ValueAsDecimal() == 0)
                    {
                        TestExecution().LogMessage("ERROR: PixelsPerUnit for '" + Name + "' is zero (0).  Unable to use it for conversion.");
                    }
                    else
                    {
                        result = result_pixels / mPixelsPerUnit.ValueAsDecimal();
                    }
                }
                catch (Exception e)
                {
                    TestExecution().LogMessageWithTimeFromTrigger("ERROR: Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
                }
                finally
                {
                }

            } // end main block ("else" after all initial setup error checks)
            mDistance.SetValue(result);
            mDistance.SetIsComplete();
            if (mDistance_pixels != null)
            {
                mDistance_pixels.SetValue(result_pixels);
                mDistance_pixels.SetIsComplete();
            }
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " computed distance of " + result);

            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
        }

    }
}

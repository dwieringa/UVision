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
    class CalibrationToolInstance : ToolInstance
    {
        public CalibrationToolInstance(CalibrationToolDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.ReferencePoint1 == null) throw new ArgumentException(Name + " doesn't have ReferencePoint1 defined.");
            mReferencePoint1 = testExecution.ReferencePointRegistry.GetObject(theDefinition.ReferencePoint1.Name);

            if (theDefinition.ReferencePoint2 == null) throw new ArgumentException(Name + " doesn't have ReferencePoint2 defined.");
            mReferencePoint2 = testExecution.ReferencePointRegistry.GetObject(theDefinition.ReferencePoint2.Name);

            if (theDefinition.KnownDistance == null) throw new ArgumentException(Name + " doesn't have KnownDistance defined.");
            mKnownDistance = testExecution.DataValueRegistry.GetObject(theDefinition.KnownDistance.Name);

            mConversionFactor = new GeneratedValueInstance(theDefinition.ConversionFactor, testExecution);
        }

        private GeneratedValueInstance mConversionFactor = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public GeneratedValueInstance ConversionFactor
        {
            get { return mConversionFactor; }
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

        private DataValueInstance mKnownDistance;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance KnownDistance
        {
            get { return mKnownDistance; }
        }

        public override bool IsComplete() { return mConversionFactor.IsComplete(); }

        public override void DoWork() 
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            double result = -1;

            if (mReferencePoint1.GetValueAsDouble() < 0 || mReferencePoint2.GetValueAsDouble() < 0)
            {
                TestExecution().LogErrorWithTimeFromTrigger("Reference point(s) for '" + Name + "' do not have valid values. ref1=" + mReferencePoint1.GetValueAsDouble() + " ref2=" + mReferencePoint2.GetValueAsDouble());
            }
            else if( mKnownDistance.ValueAsDecimal() == 0 )
            {
                TestExecution().LogErrorWithTimeFromTrigger("KnownDistance for '" + Name + "' is zero (0).  Unable to use it for conversion.");
            }
            else
            {
                try
                {
                    result = Math.Abs(mReferencePoint2.GetValueAsDouble() - mReferencePoint1.GetValueAsDouble()) / mKnownDistance.ValueAsDecimal();
                }
                catch (Exception e)
                {
                    TestExecution().LogErrorWithTimeFromTrigger("Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
                }
                finally
                {
                }

            } // end main block ("else" after all initial setup error checks)
            mConversionFactor.SetValue(result);
            mConversionFactor.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " computed conversion factor of " + result);

            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
        }

    }
}

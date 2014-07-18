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
    class DataLogInstance : ToolInstance
    {
        public DataLogInstance(DataLogDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mDataLogDefinition = theDefinition;

            if (theDefinition.Enabled == null) throw new ArgumentException(Name + " doesn't have Enabled defined.");
            mEnabled = testExecution.DataValueRegistry.GetObject(theDefinition.Enabled.Name);

            if (theDefinition.File == null || theDefinition.File == string.Empty) throw new ArgumentException(Name + " doesn't have File defined.");
            mFile = theDefinition.File;
        }

        private DataLogDefinition mDataLogDefinition = null;

        private string mFile = null;
        [CategoryAttribute("Parameters")]
        public string File
        {
            get { return mFile; }
        }

        private DataValueInstance mEnabled;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance Enabled
        {
            get { return mEnabled; }
        }

        private bool mIsComplete = false;
        public override bool IsComplete() { return mIsComplete; }

        public override void DoWork() 
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            if (mEnabled == null)
            {
                TestExecution().LogErrorWithTimeFromTrigger("Enabled isn't defined in " + Name);
            }
            else if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met. Skipping.");
            }
            else
            {
                if (mEnabled.ValueAsBoolean())
                {
                    mDataLogDefinition.AddLine(TestExecution());
                }
                else
                {
                    TestExecution().LogMessageWithTimeFromTrigger(Name + " data log disabled. Skipped entry.");
                }
            }

            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;

            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
            mIsComplete = true;
        }

    }
}

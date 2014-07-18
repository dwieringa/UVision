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
    class OperatorQueryInstance : ToolInstance
    {
        public OperatorQueryInstance(OperatorQueryDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mOperatorQueryDefinition = theDefinition;

            if (theDefinition.Enabled == null) throw new ArgumentException(Name + " doesn't have Enabled defined.");
            mEnabled = testExecution.DataValueRegistry.GetObject(theDefinition.Enabled.Name);

            if (theDefinition.QueryMessage == null || theDefinition.QueryMessage == string.Empty) throw new ArgumentException(Name + " doesn't have QueryMessage defined.");
            mQueryMessage = theDefinition.QueryMessage;

            mOperatorAnswer = new GeneratedValueInstance(theDefinition.OperatorAnswer, testExecution);
        }

        private OperatorQueryDefinition mOperatorQueryDefinition = null;

        private DataValueInstance mEnabled;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance Enabled
        {
            get { return mEnabled; }
        }

        private String mQueryMessage;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public String QueryMessage
        {
            get { return mQueryMessage; }
        }

        private GeneratedValueInstance mOperatorAnswer = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public GeneratedValueInstance OperatorAnswer
        {
            get { return mOperatorAnswer; }
        }

        public override bool IsComplete() { return mOperatorAnswer.IsComplete(); }

        public override void DoWork() 
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            long result = -1;

            try
            {
                if (mEnabled == null)
                {
                    TestExecution().LogErrorWithTimeFromTrigger("Enabled isn't defined in " + Name);
                }
                else if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
                {
                    TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met. Skipping.");
                }
                else if (!mEnabled.ValueAsBoolean())
                {
                    TestExecution().LogMessageWithTimeFromTrigger(Name + " disabled. Skipped entry.");
                }
                else
                {
                    string[] messageComponents = mOperatorQueryDefinition.QueryMessage.Split(new char[] { '|' });
                    if (messageComponents.GetLength(0) != mOperatorQueryDefinition.mValuesToReference.Count + 1)
                    {
                        string msg = Name + " has a mismatch between the QueryMessage and ValuesToReference.  " + mOperatorQueryDefinition.mValuesToReference.Count + " values were provided, but " + messageComponents.GetLength(0) + " were expected.";
                        TestExecution().LogErrorWithTimeFromTrigger(msg);
                    }
                    else
                    {
                        string queryMessage = string.Empty;
                        for (int x = 0; x < mOperatorQueryDefinition.mValuesToReference.Count; x++)
                        {
                            queryMessage += messageComponents[x] + TestExecution().DataValueRegistry.GetObject(mOperatorQueryDefinition.mValuesToReference[x].Name).Value;
                        }
                        queryMessage += messageComponents[messageComponents.GetLength(0) - 1];

                        TestSequence().StopExecutionTimeoutTimer();
                        if (DialogResult.Yes == MessageBox.Show(queryMessage, "Operator Query", MessageBoxButtons.YesNo))
                        {
                            result = 1;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TestExecution().LogErrorWithTimeFromTrigger("Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
            }
            finally
            {
                TestSequence().StartExecutionTimeoutTimer();
            }

            mOperatorAnswer.SetValue(result);
            mOperatorAnswer.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " answer was " + result);

            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
        }

    }
}

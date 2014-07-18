// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    class SendNumberToTNDInstance : ToolInstance
    {
        public SendNumberToTNDInstance(SendNumberToTNDDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mNumberToWrite = TestExecution().DataValueRegistry.GetObject(theDefinition.NumberToWrite);
        }

        public override void DoWork()
        {
            //if (!mNumberToWrite.IsComplete() || !AreExplicitDependenciesComplete()) return;
            TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " started (old link)");
            bool success = Definition().Window().myTNDLink.WriteToTNDByTypeIndex(TNDLink_old.NumberType, ((SendNumberToTNDDefinition)Definition()).DataViewIndex, (Int32)mNumberToWrite.ValueAsLong(), "writing");
            TestExecution().LogMessageWithTimeFromTrigger("TND Write " + Name + " finished (old link)");
            mIsComplete = true;
        }


		public override bool IsComplete() { return mIsComplete; }
		private bool mIsComplete = false;

        private DataValueInstance mNumberToWrite;
        public string NumberToWrite
        {
            get
            {
                if (mNumberToWrite == null) return "";
                return mNumberToWrite.Name + " (" + mNumberToWrite.ValueAsLong() + ")";
            }
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class ButtonTriggerDefinition : NetCams.TriggerDefinition, OperatorControlDefinition
	{
        public ButtonTriggerDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            testSequence.RegisterOperatorControlDefiner(this);
		}

        private string mButtonLabel = "Trigger";
		public string ButtonLabel
		{
            get { return mButtonLabel; }
            set 
            {
                HandlePropertyChange(this, "ButtonLabel", mButtonLabel, value);
                mButtonLabel = value;
            }
		}

        private bool mEnabled = true;
		public override bool TriggerEnabled
		{
            get { return mEnabled; }
            set
            {
                HandlePropertyChange(this, "TriggerEnabled", mEnabled, value);
                mEnabled = value;
            }
        }

		public override void CreateInstance(TestExecution testExecution)
		{
			new ButtonTriggerInstance(this, testExecution);
		}


        #region OperatorControlDefinition Members

        public ToolStripItem createControlInstance(int index)
        {
            return new ToolStripButton(mButtonLabel);
        }

        public int numberOfControls()
        {
            return 1;
        }

        private List<OperationForm> mNewFormsThatNeedButtonListeners = new List<OperationForm>();
        private bool mNewFormWaiting = false;
        /// <summary>
        /// This method is called by the GUI thread to let the DefinerDefinition know a new form exists.  In the TestSequence thread, the active instance will check to see if there are new forms so it can listen to their buttons
        /// </summary>
        /// <param name="theNewForm"></param>
        public void RegisterNewOperatorForm(OperationForm theNewForm)
        {
            using (TimedLock.Lock(mNewFormsThatNeedButtonListeners)) // LOCKING FOR THREAD SAFETY
            {
                mNewFormsThatNeedButtonListeners.Add(theNewForm);
                mNewFormWaiting = true;
            }
        }

        public bool IsNewFormWaiting() { return mNewFormWaiting; }
        public OperationForm GetNewWaitingForm()
        {
            OperationForm aForm = null;
            using (TimedLock.Lock(mNewFormsThatNeedButtonListeners)) // LOCKING FOR THREAD SAFETY
            {
                if (mNewFormsThatNeedButtonListeners.Count > 0)
                {
                    aForm = mNewFormsThatNeedButtonListeners[0];
                    mNewFormsThatNeedButtonListeners.RemoveAt(0);
                }

                if (mNewFormsThatNeedButtonListeners.Count == 0) mNewFormWaiting = false;
            }
            return aForm;
        }

        #endregion
    }
}

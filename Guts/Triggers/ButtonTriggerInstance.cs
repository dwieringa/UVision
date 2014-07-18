// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class ButtonTriggerInstance : NetCams.TriggerInstance
	{
		public ButtonTriggerInstance(ButtonTriggerDefinition theDefinition, TestExecution testExecution) : base(theDefinition, testExecution)
		{
            mButtonTriggerDef = theDefinition;
            // iterate through test collections, get button, register as listener
            foreach (TestExecutionCollection collection in theDefinition.TestSequence().mTestCollections)
            {
                OperationForm opForm = collection.OperationForm;
                if (opForm != null)
                {
                    RegisterAsListenerOnControl(opForm);
                }
            }
		}
        private ButtonTriggerDefinition mButtonTriggerDef;

        private void RegisterAsListenerOnControl(OperationForm opForm)
        {
            ToolStripButton control = (ToolStripButton)opForm.GetOperatorControl(mButtonTriggerDef.ButtonLabel);
            control.Click += new EventHandler(control_Click);
            buttonsRegisteredWith.Add(control);
        }
        private List<ToolStripButton> buttonsRegisteredWith = new List<ToolStripButton>();
        /*
         * The button needs to be owned/created_by the form.  There is upto one form per TestCollection
         * 
         * Process:
         x* - new operator form needs to query TestSequence to see if there are any custom controls to make available
         x* - new operator form needs to create control (button in this case) and register control with ButtonTriggerDef
         x* - new operator forms also need to register button with existing TestExecution
         * - each new TestExecution needs to register as a listener of each button (on each TestExecCollection's OpForm)
         * - after TestExecution is completed (executed fully or was cancelled) it needs to unregister with buttons
         x* - forms keep track to ensure no more than 1 listener (TestExecution) at a time?
         * 
         * TODO: only put button on "recent" TestExecutionCollection form?
         * TODO: have separate root-level form? (ability to have it always-on-top)
         * TODO: also going to want data entry fields..instead of tool bar, maybe have a screen layout)
         */
        void control_Click(object sender, EventArgs e)
        {
            mIsComplete = true;
            TestExecution().Name = "Button-triggered test at " + DateTime.Now;
        }

        public override void PostExecutionCleanup()
        {
            foreach (ToolStripButton button in buttonsRegisteredWith)
            {
                button.Click -= new EventHandler(control_Click);
            }
            buttonsRegisteredWith.Clear();
            buttonsRegisteredWith = null;
        }

        public override bool CheckTrigger()
        {
            while (mButtonTriggerDef.IsNewFormWaiting())
            {
                OperationForm theNewOpForm = mButtonTriggerDef.GetNewWaitingForm();
                if (theNewOpForm != null)
                {
                    RegisterAsListenerOnControl(theNewOpForm);
                }
            }
            return mIsComplete;
        }

        public override bool IsComplete() { return mIsComplete; }
		private bool mIsComplete = false;
	}
}

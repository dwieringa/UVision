// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NetCams
{
    public partial class OperationLogForm : DockContent
    {
        public OperationLogForm(OperationForm opForm)
        {
            InitializeComponent();
            TabText = "Log";
            mOpForm = opForm;
            mOpForm.TestSelectionChange += new TestExecution.TestExecutionDelegate(mOpForm_TestSelectionChange);
            this.Disposed += new EventHandler(OperationLogForm_Disposed);
        }

        void mOpForm_TestSelectionChange(TestExecution testExecution)
        {
            UpdateTextBox();
        }

        public void UpdateTextBox()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                Invoke(new MethodInvoker(UpdateTextBox)); // NOTE: I'm intentially called "Invoke" instead of "BeginInvoke" since the code relies heavilly on the values of the scroll bar. With BeginInvoke, the values may not be changed until "later", so Invoke ensures they are updated before we do further processing
                return;
            }
            if (mOpForm.CurrentExecution != null)
            {
                txtLogText.Text = mOpForm.CurrentExecution.GetLogText();
            }
            else
            {
                txtLogText.Text = "";
            }
        }

        private OperationForm mOpForm;

        void OperationLogForm_Disposed(object sender, EventArgs e)
        {
            mOpForm.logForm = null;
        }
    }
}
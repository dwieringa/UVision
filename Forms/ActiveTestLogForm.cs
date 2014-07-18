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
    public partial class ActiveTestLogForm : DockContent
    {
        public ActiveTestLogForm(OperationForm opForm)
        {
            InitializeComponent();
            TabText = "Log of Active Test";
            mOpForm = opForm;
            this.Disposed += new EventHandler(ActiveTestLogForm_Disposed);
        }

        private OperationForm mOpForm;

        void ActiveTestLogForm_Disposed(object sender, EventArgs e)
        {
            mOpForm.activeLogForm = null;
        }

        private void toolStripRefreshButton_Click(object sender, EventArgs e)
        {
            if (mOpForm.CurrentSequence.ActiveTestExecution() != null)
            {
                txtLogTextBox.Text = mOpForm.CurrentSequence.ActiveTestExecution().GetLogText();
            }
            else
            {
                txtLogTextBox.Text = "";
            }
        }
    }
}
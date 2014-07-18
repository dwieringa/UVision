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
    public partial class OperationReportForm : DockContent
    {
        public OperationReportForm(OperationForm opForm)
        {
            InitializeComponent();
            TabText = "Report";
            mOpForm = opForm;
            this.Disposed += new EventHandler(OperationReportForm_Disposed);
        }
        private OperationForm mOpForm;

        void OperationReportForm_Disposed(object sender, EventArgs e)
        {
            mOpForm.reportForm = null;
        }
    }
}
// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NetCams
{
    public partial class FavoriteValuesForm : DockContent
    {
        public FavoriteValuesForm(OperationForm parentForm)
        {
            InitializeComponent();
            TabText = "Favorite Values";
            mParentForm = parentForm;
            mParentForm.TestSelectionChange += new TestExecution.TestExecutionDelegate(mParentForm_TestSelectionChange);
            this.Disposed += new EventHandler(FavoriteValuesForm_Disposed);
        }

        void FavoriteValuesForm_Disposed(object sender, EventArgs e)
        {
            mParentForm.favValuesForm = null;
        }

        protected OperationForm mParentForm;

        void mParentForm_TestSelectionChange(TestExecution testExecution)
        {
            if (propertyGrid.SelectedObject == null) return;
            ((FavoriteValues)propertyGrid.SelectedObject).UpdateTestExecution(testExecution);
            RefreshValues(); //needed?
        }

        public void RefreshValues()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new MethodInvoker(RefreshValues));
                return;
            }
            propertyGrid.Refresh();
        }
    }
}
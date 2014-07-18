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
    public partial class OperationSequenceTreeForm : DockContent
    {
        public OperationSequenceTreeForm(OperationForm opForm)
        {
            InitializeComponent();
            TabText = "Sequences";
            mOpForm = opForm;
            mProject = opForm.Project;

            mProject.NewTestSequence += new TestSequence.TestSequenceDelegate(HandleNewTestSequence);
            mProject.TestSequenceDisposed += new TestSequence.TestSequenceDelegate(HandleDisposedTestSequence);

            foreach (TestSequence seq in mProject.mTestSequences)
            {
                HandleNewTestSequence(seq);
            }
            this.Disposed += new EventHandler(OperationSequenceTreeForm_Disposed);
        }
        private OperationForm mOpForm;

        void OperationSequenceTreeForm_Disposed(object sender, EventArgs e)
        {
            mOpForm.treeForm = null;
        }

        private Project mProject;

        public void HandleNewTestSequence(TestSequence seq)
        {
            TreeNode newNode = seq.GetNewTreeNode();
            treeView.Nodes.Add(newNode);
        }
        public void HandleDisposedTestSequence(TestSequence seq)
        {
            // TODO: ensure the dtor of TestSequence loops through all of it's TreeNodes and does a node.TreeView.Nodes.Remove(node)
        }

    }
}
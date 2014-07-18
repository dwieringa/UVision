// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    public class TestExecutionCollection
    {
        public TestExecutionCollection(TestSequence theSequence)
        {
            mTestSequence = theSequence;
            Name = "Recent Executions";
            theSequence.RegisterTestCollection(this);
        }

        public TestSequence TestSequence
        {
            get { return mTestSequence; }
        }
        private TestSequence mTestSequence;
        public List<TreeNode> treeNodes = new List<TreeNode>();

        public ArrayList mExecutions = new ArrayList();

        public OperationForm OperationForm = null;

        private int mCapacity = 5;
        public int Capacity
        {
            get { return mCapacity; }
            set { mCapacity = value; }
        }
        public int Count
        {
            get { return mExecutions.Count; }
        }

        private int mOperatorFormTop = 10;
        public int OperatorFormTop
        {
            get
            {
                if (OperationForm != null && OperationForm.WindowState == FormWindowState.Normal) mOperatorFormTop = OperationForm.Top;
                return mOperatorFormTop;
            }
            set
            {
                mOperatorFormTop = value;
                if (OperationForm != null)
                {
                    OperationForm.Top = value;
                }
            }
        }

        private int mOperatorFormLeft = 10;
        public int OperatorFormLeft
        {
            get
            {
                if (OperationForm != null && OperationForm.WindowState == FormWindowState.Normal) mOperatorFormLeft = OperationForm.Left;
                return mOperatorFormLeft;
            }
            set
            {
                mOperatorFormLeft = value;
                if (OperationForm != null)
                {
                    OperationForm.Left = value;
                }
            }
        }

        private int mOperatorFormWidth = 200;
        public int OperatorFormWidth
        {
            get
            {
                if (OperationForm != null && OperationForm.WindowState == FormWindowState.Normal) mOperatorFormWidth = OperationForm.Width;
                return mOperatorFormWidth;
            }
            set
            {
                mOperatorFormWidth = value;
                if (OperationForm != null)
                {
                    OperationForm.Width = value;
                }
            }
        }

        private int mOperatorFormHeight = 200;
        public int OperatorFormHeight
        {
            get
            {
                if (OperationForm != null && OperationForm.WindowState == FormWindowState.Normal) mOperatorFormHeight = OperationForm.Height;
                return mOperatorFormHeight;
            }
            set
            {
                mOperatorFormHeight = value;
                if (OperationForm != null)
                {
                    OperationForm.Height = value;
                }
            }
        }


        private String mName = "";
        public String Name
        {
            get { return mName; }
            set
            {
                mName = value;
                foreach (TreeNode node in treeNodes)
                {
                    node.Text = mName;
                }
            }
        }

        public TreeNode GetNewTreeNode()
        {
            TreeNode newNode = new TreeNode();
            newNode.Text = Name;
            newNode.Tag = this;
            treeNodes.Add(newNode); // the sequence needs to know about all nodes so that it can update the tree node text
            return newNode;
        }
        public TestExecution GetTestFromHistory(int index)
        {
            if (mExecutions.Count <= index) return null; 
            return (TestExecution)mExecutions[mExecutions.Count - index - 1];
        }

        public event TestExecution.TestExecutionDelegate NewTestExecution;

        public void AddExecution(TestExecution theExecution)
        {
            // TO DO: does it meet the collection's criteria?  (e.g. "only failed test", "only passing tests", "borderline tests", "last 100 tests", "todays tests", "last 500 parts taken within 15 minutes of startup"?)
            //Trace.WriteLine("adding execution to collection. " + theExecution.Name + " " + mExecutions.Count);
            while (mExecutions.Count >= Capacity)
            {
                //Trace.WriteLine("removing execution from collection due to size. " + mExecutions.Count);
                //                mTestSequence.Window().RemoveTreeNode( treeNode, ((TestExecution)mExecutions[0]).treeNode );
                mExecutions.RemoveAt(0);
            }

            mExecutions.Add(theExecution);
            //Trace.WriteLine("test added to collection. new count=" + mExecutions.Count);
            //            mTestSequence.Window().AddTreeNode( treeNode, theExecution.treeNode);

            if (NewTestExecution != null)
            {
                //Trace.WriteLine("notifying listeners");
                NewTestExecution(theExecution);
            }
            //Trace.WriteLine("done with addExecution");
        }
    }
}

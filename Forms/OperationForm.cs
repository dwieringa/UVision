// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NetCams
{
    public partial class OperationForm : Form
    {
        private DeserializeDockContent m_deserializeDockContent;

        public OperationForm(TestExecutionCollection collection)
        {
            InitializeComponent();
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            Top = collection.OperatorFormTop;
            Left = collection.OperatorFormLeft;
            Width = collection.OperatorFormWidth;
            Height = collection.OperatorFormHeight;

            mTestSequence = collection.TestSequence;
            mProject = mTestSequence.project();

            dockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            bigImageForm = new ImageBigViewForm(this);
            
            //            propForm = new OperationPropertiesForm();
            favSettingsForm = new FavoriteSettingsForm(this);
            favValuesForm = new FavoriteValuesForm(this);
            logForm = new OperationLogForm(this);
            //studyChartForm = new AnalysisForm(this);
            activeLogForm = new ActiveTestLogForm(this);
            reportForm = new OperationReportForm(this);
            //treeForm = new OperationSequenceTreeForm(this);

            if (File.Exists(mTestSequence.OperatorViewLayoutFile))
            {
                dockPanel.LoadFromXml(mTestSequence.OperatorViewLayoutFile, m_deserializeDockContent);
            }
            /*
            else if (File.Exists(mTestSequence.Project().DefaultOperatorViewLayoutFile))
            {
                dockPanel.LoadFromXml(mTestSequence.Project().DefaultOperatorViewLayoutFile, m_deserializeDockContent);
            }
            else if (File.Exists("UnnamedView.config"))
            {
                dockPanel.LoadFromXml("UnnamedView.config", m_deserializeDockContent);
            }*/
            else
            {
                /*
                propForm.Show(dockPanel);
                propForm.ShowHint = DockState.DockRightAutoHide;
                */
                bigImageForm.Dock = DockStyle.Fill;
                bigImageForm.Show(dockPanel, DockState.Document);
                
                favSettingsForm.Show(bigImageForm.Pane, DockAlignment.Top, 0.25);

                logForm.Show(favSettingsForm.Pane, DockAlignment.Right, 0.5);

                favValuesForm.Show(favSettingsForm.Pane, DockAlignment.Right, 0.5);

                reportForm.Show(logForm.Pane, logForm);

                //treeForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            }

            CurrentTestCollection = collection;

            favSettingsForm.propertyGrid.SelectedObject = mTestSequence.mFavoriteSettings;
            favValuesForm.propertyGrid.SelectedObject = mTestSequence.mFavoriteValues;

            int formNdx = 0;
            ImageHistoryForm imageForm;
            foreach (string imageTabDef in mTestSequence.mImageForms)
            {
                if (formNdx >= mImageForms.Count)
                {
                    AddNewImageForm();
                }
                imageForm = mImageForms[formNdx];
                imageForm.DeserializeDef(imageTabDef);
                formNdx++;
            }
            /*
            if (formNdx < mImageForms.Count - 1)
            {
                mImageForms.RemoveRange(formNdx, (mImageForms.Count - 1) - formNdx);
            }
            */
            FormClosing += new FormClosingEventHandler(OperationForm_FormClosing);

            // find all of the control definer definitions in the test sequence
            foreach (OperatorControlDefinition operatorControlDefiner in mTestSequence.mOperatorControlDefiners)
            {
                // get and add all of the controls within each definer to the toolstrip
                int numControls = operatorControlDefiner.numberOfControls();
                for( int x = 0; x < numControls; x++)
                {
                    ToolStripItem newControl = operatorControlDefiner.createControlInstance(x);
                    toolStrip_Operator.Items.Add(newControl);

                    // let the definer know that there is a new operator form...so the active test execution (running in a separate thread) can register as a listener
                    operatorControlDefiner.RegisterNewOperatorForm(this);                    
                }
            }
        }

        public ToolStripItem GetOperatorControl(string itemText)
        {
            foreach (ToolStripItem item in toolStrip_Operator.Items)
            {
                if (item.Text == itemText)
                {
                    return item;
                }
            }
            throw new ArgumentException("Can't locate Operator Control '" + itemText + "'");
        }

        void OperationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mTestExecutionCollection.OperationForm = null;
        }

        /*
         * TODO: LiveView should be a function of ImageForm NOT OperationForm.  We may want some image forms live and some historical
         * Now/Live causes new tests to be SELECTED
         * Test Selection updates scroll bar (but only if the test is outside the current view???)
         * Scroll bar changes do not change SELECTION, just VIEW
         * Selecting an old test turns off Live View
         * Now/Live should be on if no test is selected
         * ? scroll bar value should be updated as tests are dropped off the backend of the collection...we don't want this to update the view, but adjust the scroll bar
         */
        public bool mLiveView = true;
        //public OperationPropertiesForm propForm;
        //public ImageHistoryForm imageForm;
        public ImageBigViewForm bigImageForm;
        public FavoriteSettingsForm favSettingsForm;
        public FavoriteValuesForm favValuesForm;
        public OperationLogForm logForm;
        public AnalysisForm studyChartForm;
        public ActiveTestLogForm activeLogForm;
        public OperationReportForm reportForm;
        public OperationSequenceTreeForm treeForm;

        delegate void TestExecutionDelegate(TestExecution test);
        public void NewTest(TestExecution newTest)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new TestExecutionDelegate(NewTest), new object[] { newTest });
                return;
            }

            if (mLiveView)
            {
                CurrentExecution = newTest;
            }

            foreach (ImageHistoryForm iForm in mImageForms)
            {
                iForm.UpdateScrollBar();
                //Thread.Sleep(1000);
                iForm.HandleViewChange(); // technically we only need to call this if the new test falls within the current view
            }
        }

        private Project mProject;
        public Project Project
        {
            get { return mProject; }
        }

        private TestSequence mTestSequence;
        public TestSequence CurrentSequence
        {
            get { return mTestSequence; }
        }

        private TestExecutionCollection mTestExecutionCollection;
        public TestExecutionCollection CurrentTestCollection
        {
            get { return mTestExecutionCollection; }
            set
            {
                // stop listening for changes within the old TestCollection
                if (mTestExecutionCollection != null)
                {
                    mTestExecutionCollection.NewTestExecution -= new TestExecution.TestExecutionDelegate(this.NewTest);
                }

                // change the TestCollection we are looking at
                mTestExecutionCollection = value;
                Text = CurrentSequence.Name + " : " + mTestExecutionCollection.Name;

                // listen for changes within the new TestCollection
                if (mTestExecutionCollection != null)
                {
                    mTestExecutionCollection.NewTestExecution += new TestExecution.TestExecutionDelegate(this.NewTest);
                }

                // notify listeners that we've changed the TestCollection we are looking at
                if (TestCollectionSelectionChange != null)
                {
                    TestCollectionSelectionChange();
                }
            }
        }

        private TestExecution mCurrentExecution;
        public TestExecution CurrentExecution
        {
            get { return mCurrentExecution; }
            set
            {
                if (value != mCurrentExecution)
                {
                    mCurrentExecution = value;

                    // notify listeners (e.g. FavoriteValuesForm) that the selection has changed
                    if (TestSelectionChange != null)
                    {
                        TestSelectionChange(mCurrentExecution);
                    }
                }
            }
        }

        public delegate void TestCollectionSelectionChangeDelegate();
        public event TestCollectionSelectionChangeDelegate TestCollectionSelectionChange;
        public event TestExecution.TestExecutionDelegate TestSelectionChange;

        private int mCollectionIndex;
        public int CollectionIndex
        {
            get { return mCollectionIndex; }
            set { mCollectionIndex = value; }
        }

        private void lockLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
        }

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            lockLayoutToolStripMenuItem.Checked = !this.dockPanel.AllowEndUserDocking;
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(OperationSequenceTreeForm).ToString())
                return treeForm;
            else if (persistString == typeof(FavoriteSettingsForm).ToString())
                return favSettingsForm;
            else if (persistString == typeof(FavoriteValuesForm).ToString())
                return favValuesForm;
            else if (persistString == typeof(AnalysisForm).ToString())
                return studyChartForm;
            else if (persistString == typeof(OperationLogForm).ToString())
                return logForm;
            else if (persistString == typeof(ImageBigViewForm).ToString())
                return bigImageForm;
            else if (persistString == typeof(ActiveTestLogForm).ToString())
                return activeLogForm;
//            else if (persistString == typeof(OperationPropertiesForm).ToString())
//                return propForm;
            else if (persistString == typeof(OperationReportForm).ToString())
                return reportForm;
            else
            {
                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.GetUpperBound(0) != 1)
                    return null;

                if (parsedStrings[0] != typeof(ImageHistoryForm).ToString())
                    return null;

                string formName = parsedStrings[1];
                foreach( ImageHistoryForm iForm in mImageForms)
                {
                    if( iForm.TabText == formName )
                    {
                        return iForm;
                    }
                }

                // form by that name doesn't exist...so create a new one
                ImageHistoryForm imageForm = AddNewImageForm();
                imageForm.TabText = formName;

                return imageForm;
            }
        }
        public List<ImageHistoryForm> mImageForms = new List<ImageHistoryForm>();

        private ImageHistoryForm AddNewImageForm()
        {
            ImageHistoryForm newImageForm = new ImageHistoryForm(this);
            mImageForms.Add(newImageForm);
            newImageForm.TabText = "Untitled Image Form " + mImageForms.Count;
            if (bigImageForm.Pane == null)
            {
                newImageForm.Show(dockPanel, DockState.Document);
            }
            else
            {
//                newImageForm.Show(bigImageForm.Pane, bigImageForm);
                bigImageForm.Show(); // HACK_2008_02_26!!!  the history forms don't show properly until AFTER the BigImage form is shown...bug in DockPanel Suite?  BigImageForm is the first form added to this region
                bigImageForm.Hide(); // HACK_2008_02_26!!! ...hiding this form after showing it since we don't want to show it until the user needs it
                newImageForm.Show(dockPanel, DockState.Document);
//                newImageForm.Show();
            }
            return newImageForm;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDockPanelLayout();
        }

        public void SaveDockPanelLayout()
        {
            if (mTestSequence == null)
            {
                throw new ArgumentException("Need to choose a TestSequence before this view can be saved.");
            }

            if (mTestSequence.OperatorViewLayoutFile.Length == 0)
            {
                mTestSequence.OperatorViewLayoutFile = mTestSequence.Name + "View.xml";
            }

            dockPanel.SaveAsXml(mTestSequence.OperatorViewLayoutFile);
            /*
            else if (mTestSequence != null && mTestSequence.Project().DefaultOperatorViewLayoutFile.Length > 0)
            {
                dockPanel.SaveAsXml(mTestSequence.Project().DefaultOperatorViewLayoutFile);
            }
            else
            {
                dockPanel.SaveAsXml("UnnamedView.config");
            }*/
        }
        private void favoriteSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (favSettingsForm == null)
            {
                favSettingsForm = new FavoriteSettingsForm(this);
            }

            if (CurrentSequence != null)
            {
                favSettingsForm.propertyGrid.SelectedObject = CurrentSequence.mFavoriteSettings;
            }

            favSettingsForm.Show(dockPanel);
        }

        private void favoriteValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (favValuesForm == null)
            {
                favValuesForm = new FavoriteValuesForm(this);
            }

            if (CurrentSequence != null)
            {
                favValuesForm.propertyGrid.SelectedObject = CurrentSequence.mFavoriteValues;
            }

            favValuesForm.Show(dockPanel);
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (reportForm == null)
            {
                reportForm = new OperationReportForm(this);
            }

            reportForm.Show(dockPanel);
        }

        private void studyChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (studyChartForm == null)
            {
                studyChartForm = new AnalysisForm(this);
            }

            studyChartForm.Show(dockPanel);
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logForm == null)
            {
                logForm = new OperationLogForm(this);
            }

            logForm.Show(dockPanel);
        }

        private void bigImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowBigImageView();
        }

        public void ShowBigImageView()
        {
            if (bigImageForm == null)
            {
                bigImageForm = new ImageBigViewForm(this);
            }

            bigImageForm.Show(dockPanel, DockState.Document);
        }

        private void activeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeLogForm == null)
            {
                activeLogForm = new ActiveTestLogForm(this);
            }

            activeLogForm.Show(dockPanel);
        }

        private void sequencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentSequence != null)
            {
                if (treeForm == null)
                {
                    treeForm = new OperationSequenceTreeForm(this);
                }

                treeForm.Show(dockPanel);
            }
        }

        private void toolStripContainer_TopToolStripPanel_Paint(object sender, PaintEventArgs e)
        {
            toolStripLiveButton.Checked = mLiveView;
            if (CurrentSequence != null && !CurrentSequence.Enabled)
            {
                toolStripEnableSeqButton.Visible = true;
            }
            else
            {
                toolStripEnableSeqButton.Visible = false;
            }
        }

        private void toolStripLiveButton_Click(object sender, EventArgs e)
        {
            mLiveView = !mLiveView;
        }

        private void toolStripEnableSeqButton_Click(object sender, EventArgs e)
        {
            if (CurrentSequence != null)
            {
                CurrentSequence.Enabled = true;
            }

        }

        public OperationPictureBox mStudyPictureBox = null;
        public static Point NotDefinedPoint = new Point(-1, -1);
        public Point mStudyPoint1 = NotDefinedPoint;
        public Point mStudyPoint2 = NotDefinedPoint;
        private void toolStripStudyButton_Click(object sender, EventArgs e)
        {
            toolStripProbeButton.Checked = false;
            mStudyPictureBox = null;
            mStudyPoint1 = NotDefinedPoint;
            mStudyPoint2 = NotDefinedPoint;
        }

        private void toolStripProbeButton_Click(object sender, EventArgs e)
        {
            toolStripStudyButton.Checked = false;
        }

        private void toolStripSaveLayoutButton_Click(object sender, EventArgs e)
        {
            mTestSequence.mImageForms.Clear();
            foreach (ImageHistoryForm imageForm in mImageForms)
            {
                mTestSequence.mImageForms.Add(imageForm.SerializeDef());
            }
            mTestSequence.SaveDefinition();
            SaveDockPanelLayout();
            mTestSequence.project().saveSettings();// saving whole project in case TestSequence.OperatorViewLayoutFile was defined for the first time
        }


    }
}
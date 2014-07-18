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
    [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
    public partial class ImageHistoryForm : DockContent, ImageForm
    {

        public ImageHistoryForm(OperationForm parentForm)
        {
            InitializeComponent();
            TabText = "Images";
            mParentForm = parentForm;
            ResizeImageTable(1, 1);
            mParentForm.TestSelectionChange += new TestExecution.TestExecutionDelegate(HandleTestSelectionChange);
            mParentForm.TestCollectionSelectionChange += new OperationForm.TestCollectionSelectionChangeDelegate(HandleTestCollectionSelectionChange);
            Resize += new EventHandler(ImageHistoryForm_Resize);
            FormClosed += new FormClosedEventHandler(ImageHistoryForm_FormClosed);
            //            LoadImageTable(0);
//            ResizeImageTable(4, 2);
            this.ContextMenu = new ContextMenu();
            MenuItem selectImageMenu = this.ContextMenu.MenuItems.Add("Table Size");

            FormClosing += new FormClosingEventHandler(ImageHistoryForm_FormClosing);
        }

        void ImageHistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you SURE you want to close this history window? (my guess is that you didn't really mean to)", "Close History Window Verification", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        void ImageHistoryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mParentForm.TestSelectionChange -= new TestExecution.TestExecutionDelegate(HandleTestSelectionChange);
            mParentForm.TestCollectionSelectionChange -= new OperationForm.TestCollectionSelectionChangeDelegate(HandleTestCollectionSelectionChange);
            Resize -= new EventHandler(ImageHistoryForm_Resize);
            mParentForm.mImageForms.Remove(this);
        }

        ~ImageHistoryForm()
        {
        }

        public string SerializeDef()
        {
            string def = TabText + " : " + ImageColumns + " : ";
            string seperator = "";
            for (int rowNdx = 0; rowNdx < ImageRows; rowNdx++)
            {
                def += seperator + pictureBoxes[0, rowNdx].ImageDisplayDef.SerializeDef();
                seperator = " | ";
            }
            return def;
        }
        public void DeserializeDef(string defString)
        {
            string[] mainSplitComponents = defString.Split(new char[] { ':' });
            if (mainSplitComponents.GetUpperBound(0) != 2) throw new ArgumentException("Invalid image for definition; line='" + defString + "'");
            string[] rowDefs = mainSplitComponents[2].Split(new char[] { '|' });

            TabText = mainSplitComponents[0].Trim();

            ImageColumns = int.Parse(mainSplitComponents[1]);
            ImageRows = rowDefs.GetUpperBound(0) + 1;
            TabText = mainSplitComponents[0].Trim();
            for (int rowIndex = 0; rowIndex <= rowDefs.GetUpperBound(0); rowIndex++)
            {
                GetImageDisplayDef(rowIndex).DeserializeDef(rowDefs[rowIndex]);
            }
        }

        void ImageHistoryForm_Resize(object sender, EventArgs e)
        {
/*            string msg = "";
            if (Pane != null) msg += "  " + this.Pane.Width;
            if (PanelPane != null) msg += "  " + this.PanelPane.Width;
            MessageBox.Show("here! " + msg);
 */
            ResizePictureBoxes();
        }

        private OperationForm mParentForm;
        public OperationForm OpForm()
        {
            return mParentForm;
        }

        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + TabText;
        }

        private OperationPictureBox[,] pictureBoxes = new OperationPictureBox[0, 0];

        public ImageDisplayDef GetImageDisplayDef(int rowIndex)
        {
            if (rowIndex > pictureBoxes.GetUpperBound(1))
            {
                throw new ArgumentException("Invalid row chosen. 293032974");
            }

            return pictureBoxes[0, rowIndex].ImageDisplayDef;
        }

        /*
        private List<String> mImages = new List<String>();
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public List<String> Images
        {
            get { return mImages; }
            set
            {
                mImages = value;
                for (int x = 0; x <= pictureBoxes.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= pictureBoxes.GetUpperBound(1); y++)
                    {
                        OperationPictureBox picBox = pictureBoxes[x, y];
                        picBox.ImageName = mImages[y]; // update Image choices, but not the images themselves
                    }
                }
                ResizeImageTable(ImageColumns, mImages.Count);
                LoadImagesIntoPictureBoxes(); // update the images within each picture
            }
        }
         */
 
        //http://en.csharp-online.net/TableLayoutPanel
        public int ImageRows
        {
            get { return pictureBoxes.GetUpperBound(1)+1; }
            set
            {
                if (value == ImageRows) return;
                ResizeImageTable(ImageColumns, value);
            }
        }
        public int ImageColumns
        {
            get { return pictureBoxes.GetUpperBound(0)+1; }
            set
            {
                if (value == ImageColumns) return;
//                int numColsAdded = value - ImageColumns;
                ResizeImageTable(value, ImageRows);
                UpdateScrollBar();
                HandleViewChange();
            }
        }

        private void ResizeImageTable(int newNumCol, int newNumRows)
        {
            OperationPictureBox[,] newPicBoxArray = new OperationPictureBox[newNumCol, newNumRows];

            ImageDisplayDef imageDisplayDef;

            int newColumnsAdded = newNumCol - pictureBoxes.GetUpperBound(0)-1;
            if (newColumnsAdded >= 0)
            {
                // copy old picture boxes to the new array
                for (int y = 0; y <= Math.Min(pictureBoxes.GetUpperBound(1),newPicBoxArray.GetUpperBound(1)); y++)
                {
                    for (int x = 0; x <= pictureBoxes.GetUpperBound(0); x++)
                    {
                        newPicBoxArray[x, y] = pictureBoxes[x, y];
                        pictureBoxes[x, y] = null; // remove it from the old array so that we don't remove it from the Panel during cleanup (below)
                    }
                }
            }
            else if (newColumnsAdded < 0)
            {
                // copy old picture boxes to the new array...slide them over to the left while we are at it
                for (int y = 0; y <= Math.Min(pictureBoxes.GetUpperBound(1), newPicBoxArray.GetUpperBound(1)); y++)
                {
                    for (int x = 0; x <= newPicBoxArray.GetUpperBound(0); x++)
                    {
                        newPicBoxArray[x, y] = pictureBoxes[x, y];
                        pictureBoxes[x, y] = null; // remove it from the old array so that we don't remove it from the Panel during cleanup (below)
                    }
                }
            }
            // fill in any newly created slots with new picture boxes
            for (int y = 0; y <= newPicBoxArray.GetUpperBound(1); y++)
            {
                if (newPicBoxArray[0, y] != null && newPicBoxArray[0, y].ImageDisplayDef != null)
                {
                    imageDisplayDef = newPicBoxArray[0, y].ImageDisplayDef;
                }
                else
                {
                    imageDisplayDef = new ImageDisplayDef(mParentForm.CurrentSequence);
                }

                for (int x = 0; x <= newPicBoxArray.GetUpperBound(0); x++)
                {
                    if (newPicBoxArray[x, y] == null)
                    {
                        OperationPictureBox newPicBox = new OperationPictureBox(this, imageDisplayDef);
                        newPicBox.mIndex = x;
                        newPicBoxArray[x, y] = newPicBox;
                        imageLayoutPanel.Controls.Add(newPicBox);
                    }
                    newPicBoxArray[x, y].mIndex = x; // update pictureBoxes' index in case columns were shifted
                }
            }
            // remove obsolete picture boxes from the panel
            for (int y = 0; y <= pictureBoxes.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= pictureBoxes.GetUpperBound(0); x++)
                {
                    if (pictureBoxes[x, y] != null)
                    {
                        imageLayoutPanel.Controls.Remove(pictureBoxes[x, y]);
                    }
                }
            }
            pictureBoxes = newPicBoxArray;
            ResizePictureBoxes();
        }

        // This is called when the parent form resizes or ImageRows/ImageColumns changes
        private void ResizePictureBoxes()
        {
            if (imageLayoutPanel == null) return;
            int newImageWidth = (int)((float)imageLayoutPanel.Width / (float)(pictureBoxes.GetUpperBound(0) + 1)) - 2;
            // HACK_2008_02_26 #3: ugh all of a sudden the scroll bar is covering up part of the imageLayoutPanel...have an expert-exchange question out
            //int imagePanelHeight = this.ClientSize.Height - this.scrollPanel.Height;
            int imagePanelHeight = imageLayoutPanel.Height;
            // HACK_2008_02_26 #3: Console.WriteLine(imageLayoutPanel.Height + " " + this.Height + " " + this.scrollPanel.Height + " " + this.ClientSize.Height);
            // HACK_2008_02_26 #3: without the imageLayoutPanel.BringToFront() below this debug output makes imageLayoutPanel.Height the same as this.Height and this.ClientSize.Height...and we loose part of it udner the scrollPanel
            int newImageHeight = (int)((float)imagePanelHeight / (float)(pictureBoxes.GetUpperBound(1) + 1)) - 2;
            int switchValue = pictureBoxes.GetUpperBound(0) * (newImageWidth+2);
            for (int y = 0; y <= pictureBoxes.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= pictureBoxes.GetUpperBound(0); x++)
                {
                    OperationPictureBox picBox = pictureBoxes[x, y];
                    picBox.Size = new Size( newImageWidth, newImageHeight);
                    picBox.Location = new Point(x * (newImageWidth + 2), y * (newImageHeight + 2));
                }
            }
            imageLayoutPanel.BringToFront();// TODO: needed? HACK_2008_02_26 #3: if I comment this out then the bottom row of PicBox's get partially covered by the scrollPanel. WHY?  There must be a better/right-er way
        }

        // This is called when:
        // 1) the user clicks Now button
        // 2) the user moves the scroll bar
        // 3) the program adjusts the scroll bar position
        // 3a) a new test is added to the current collection AND we are in Live mode
        // 3b) an old test is dropped from the collection
        // 4) the current collection or TestSequence is changed
        public void HandleViewChange()
        {
            // NOTE: Assume the scollbar has been updated before we get here

            LoadImagesIntoPictureBoxes();
        }

        // This is called when:
        // 1) user clicks a test (image) within the current view
        // 2) a new test is added to the current collection and we are in Live mode
        public void HandleTestSelectionChange(TestExecution newlySelectedTest)
        {
            RefreshForm(); // want to repaint the picture boxes so that the selection highlighting is updated
        }

        public void RefreshForm()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new MethodInvoker(RefreshForm));
                return;
            }
            try
            {
                Refresh();
            }
            catch (Exception e)
            {
                mParentForm.Project.LogError("Exception refreshing image history form.");
            }
        }

        public void HandleTestCollectionSelectionChange()
        {
            UpdateScrollBar();
            //scrollBarValue = latest?? let it select same tests from other collection?
            HandleViewChange();
        }

        // This is called when:
        // 1) a new test is added to the current collection and we are in Live mode (max & value may change)
        // 2) a new test is added to the current collection and we are NOT in Live mode (max & value may change; since old test(s) may drop out of collection so Value must change to stick with current view)
        // 3) current collection drops old values, but gains no new ones (max & value will change)
        // NOTE: if the user adjusts scroll bar position, only Value should change, so we DO NOT call this
        // NOTE: if ImageColumns changes, only LargeChange should change, so we just update that and don't call this
        //
        // Basically, anytime there is any change to the Collection contents, or Collection selection.
        public void UpdateScrollBar()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                Invoke(new MethodInvoker(UpdateScrollBar)); // NOTE: I'm intentially called "Invoke" instead of "BeginInvoke" since the code relies heavilly on the values of the scroll bar. With BeginInvoke, the values may not be changed until "later", so Invoke ensures they are updated before we do further processing
                return;
            }
            if (mParentForm.CurrentTestCollection == null)
            {
                historyScroll.Maximum = 0;
            }
            else
            {
                historyScroll.Maximum = mParentForm.CurrentTestCollection.Count - 1; //mParentForm.CurrentTestCollection.Count-ImageColumns;
            }
            historyScroll.Minimum = 0;
            historyScroll.LargeChange = ImageColumns; // Math.Max(1, Math.Min(ImageColumns, historyScroll.Maximum - 1));
            historyScroll.SmallChange = 1;

            if (mParentForm.CurrentTestCollection != null && historyScroll.Maximum >= ImageColumns)
            {
                if (mParentForm.mLiveView)
                {
                    historyScroll.Value = historyScroll.Maximum - ImageColumns + 1; // move to end (most current tests)
                }
                else
                {
                    // update the scroll bar value such that it shows the same tests... the value would change if we just got a new test and it caused an old test to drop out of the collection
                    int ndxToHoldInPosition = 0;
                    if (mParentForm.CurrentExecution == null)
                    {
                        ndxToHoldInPosition = 0;
                    }
                    else
                    {
                        for (int colNdx = 0; colNdx < ImageColumns; colNdx++)
                        {
                            if (pictureBoxes[colNdx, 0].TestExecution == mParentForm.CurrentExecution)
                            {
                                ndxToHoldInPosition = colNdx;
                                break;
                            }
                        }
                    }
                    bool found = false;
                    for (int ndx = 0; ndx < mParentForm.CurrentTestCollection.Count; ndx++)
                    {
                        if (mParentForm.CurrentTestCollection.mExecutions[ndx] == pictureBoxes[ndxToHoldInPosition,0].TestExecution)
                        {
                            found = true;
                            historyScroll.Value = Math.Max(0,ndx - ndxToHoldInPosition);
                        }
                    }
                    if (!found)
                    {
                        historyScroll.Value = 0; // NOTE: this could produce odd results if tests are dropped out of the middle of a collection...if we were looking at a test in the middle and it was dropped, the view would jump to the oldest non-dropped tests
                        mParentForm.CurrentExecution = null;
                    }
                }
            }
            //mParentForm.lblProbeSample.Text = "" + historyScroll.Value; // TODO: remove
        }

        private void LoadImagesIntoPictureBoxes()
        {
            for (int x = 0; x < ImageColumns; x++)
            {
                for (int y = 0; y < ImageRows; y++)
                {
                    // TODO: currently we are doing this more often than needed I think.  We should only have to update the image if our view changes...not on every repaint (e.g. resizing window)

                    OperationPictureBox picBox = pictureBoxes[x, y];
                    int offsetIndex = historyScroll.Value + picBox.mIndex;
                    if (OpForm().CurrentTestCollection == null ||
                        offsetIndex >= OpForm().CurrentTestCollection.mExecutions.Count)
                    {
                        picBox.TestExecution = null;
                    }
                    else
                    {
                        picBox.TestExecution = (TestExecution)OpForm().CurrentTestCollection.mExecutions[offsetIndex];
                    }
                }
            }
        }

        // This is called when the scroll bar value is adjusted by the user OR PROGRAM
        private void historyScroll_ValueChanged(object sender, EventArgs e)
        {
            //mParentForm.lblProbeSample.Text = "" + historyScroll.Value;
//            MessageBox.Show("" + historyScroll.Value);
        }

        // This is called by the GUI thread when the scroll bar is adjusted by the user
        void historyScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            HandleViewChange();
        }

        private void liveScrollButton_Click(object sender, EventArgs e)
        {
            mParentForm.mLiveView = true;
            mParentForm.toolStrip.Refresh();

            if (mParentForm.CurrentTestCollection != null && mParentForm.CurrentTestCollection.Count > 0)
            {
                mParentForm.CurrentExecution = mParentForm.CurrentTestCollection.GetTestFromHistory(0);
                UpdateScrollBar();
                HandleViewChange();
            }
        }

        private void ImageHistoryForm_Load(object sender, EventArgs e)
        {
            HandleViewChange(); // HACK_2008_02_26!!!: seems to fix the "first view" issue...where only top row of PicBox's display until the scroll bar is adjusted
            //Show(); // HACK_2008_02_26!!! This allows us to start on the last form loaded (rather than the BigImageView...see other components of HACK_2008_02_26...REMOVED THIS SINCE I NOW HIDE THE BIGIMAGEFORM AFTER SHOWING IT...this allows the first history form to be active by default
        }

        private void ImageHistoryForm_Activated(object sender, EventArgs e)
        {
            //HandleViewChange();
        }

        private void ImageHistoryForm_Enter(object sender, EventArgs e)
        {
            //HandleViewChange();
        }

        private void ImageHistoryForm_Validated(object sender, EventArgs e)
        {

            //HandleViewChange();
            //imageLayoutPanel.Refresh();
            //RefreshForm();
        }


    }
}
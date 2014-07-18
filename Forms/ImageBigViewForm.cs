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
    public partial class ImageBigViewForm : DockContent, ImageForm
    {
        public ImageBigViewForm(OperationForm parentForm)
        {
            InitializeComponent();
            TabText = "Big View";
            mOpForm = parentForm;
            mOpPictureBox = new OperationPictureBox(this, new ImageDisplayDef(parentForm.CurrentSequence));
            imageLayoutPanel.Controls.Add(mOpPictureBox);
            Resize += new EventHandler(ImageBigViewForm_Resize);
            FormClosing += new FormClosingEventHandler(ImageBigViewForm_FormClosing);
            this.Disposed += new EventHandler(OperationLogForm_Disposed);
        }

        void ImageBigViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // we don't want them to close this form since we dock other image forms using this one
            e.Cancel = true;
            Hide();
        }

        ~ImageBigViewForm()
        {
            Resize -= new EventHandler(ImageBigViewForm_Resize);
        }

        public TestExecution TestExecution
        {
            get { return mOpPictureBox.TestExecution; }
            set { mOpPictureBox.TestExecution = value; }
        }

        public ImageDisplayDef ImageDisplayDef
        {
            get { return mOpPictureBox.ImageDisplayDef; }
        }

        public string ImageName
        {
            get
            {
                if (mOpPictureBox.ImageDisplayDef.ImageDefinition == null) return "";
                return mOpPictureBox.ImageDisplayDef.ImageDefinition.Name;
            }
        }

        void ImageBigViewForm_Resize(object sender, EventArgs e)
        {
            // NOTE, "HACK_2008_02_26 #2" is only an issue if the OperatorForm is loaded without a DockPanel view xml file.  If I load an xml file, every displays fine.  If no file exists, the ctor creates a default layout and this is when the problem with BigImageView shows up...so for now I undid the hack
            // HACK_2008_02_26 #2: the "25"'s compensate for the PictureBox getting hidden under the tabs...this seemed to appear about the same time I made all of the ImageHistoryForms dock after/with the BigImageForm. see other HACK_2008_02_26 which is different but dependent on the same issue?  both issues existed in DockPanel 2.1 & 2.2
            // this compensation isn't needed for the ImageHistoryForm...the only thing I can see different is that the HistoryFOrm also has a scroll bar below the image panel
            mOpPictureBox.Size = new Size(imageLayoutPanel.Width, imageLayoutPanel.Height);
            //mOpPictureBox.Location = new Point(0, 25);
        }
        void OperationLogForm_Disposed(object sender, EventArgs e)
        {
            // NOTE: we should never get here since I'm prevening Closing of the form by capturing the FormClosing event
            mOpForm.bigImageForm = null;
        }
        public OperationPictureBox mOpPictureBox;
        private OperationForm mOpForm;
        public OperationForm OpForm()
        {
            return mOpForm;
        }
        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + TabText;
        }

        public void RefreshForm()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new MethodInvoker(RefreshForm));
                return;
            }
            Refresh();
        }
    }
}
// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace NetCams
{
    public class OperationPictureBox : PictureBox
    {
        public OperationPictureBox(ImageForm parentForm, ImageDisplayDef imageDisplayDef)
        {
            mParentForm = parentForm;
            mOperationForm = parentForm.OpForm();
            mImageDisplayDef = imageDisplayDef;
            mImageDisplayDef.ImageDisplayDefChange += new ImageDisplayDef.ImageDisplayDefChangeDelegate(HandleDisplayDefChange);

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(image_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(image_MouseUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(image_MouseMove);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.image_PreviewKeyDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.image_KeyDown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.DoubleClick += new EventHandler(OperationPictureBox_DoubleClick);

            this.ContextMenu = new ContextMenu();
            this.ContextMenu.MenuItems.Add("Retest Image", new EventHandler(menu_retestImageSelected));
            this.ContextMenu.MenuItems.Add("Save Image", new EventHandler(menu_saveImageSelected));
            mImageDisplayDef.AddDisplayMenu(this.ContextMenu);
            /*
            popUpMenu = new ContextMenu();
            this.ContextMenu = popUpMenu;
            mImageDisplayDef.AddDisplayMenu(popUpMenu);
            */
//            Dock = DockStyle.None;
//            debugText = "" + mIndex+ "|" + x + "," + y;
        }

        ~OperationPictureBox()
        {
            mImageDisplayDef.ImageDisplayDefChange -= new ImageDisplayDef.ImageDisplayDefChangeDelegate(HandleDisplayDefChange);

            // TODO: is this right?  Is it needed since the listener is the same class?
            this.MouseDown -= new System.Windows.Forms.MouseEventHandler(image_MouseDown);
            this.MouseUp -= new System.Windows.Forms.MouseEventHandler(image_MouseUp);
            this.MouseMove -= new System.Windows.Forms.MouseEventHandler(image_MouseMove);
            this.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.image_PreviewKeyDown);
            this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.image_KeyDown);
            this.Paint -= new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
        }

        private void menu_retestImageSelected(object sender, EventArgs e)
        {
            if (mImageInstance != null && mImageInstance.Generator != null && mImageInstance.Bitmap != null)
            {
                Bitmap image = mImageInstance.Bitmap;
                mImageInstance.Generator.Definition_ImageGenerator().SimulateGeneratingImage(image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat));
            }
        }

        private SaveFileDialog saveImageDialog = null;
        private void menu_saveImageSelected(object sender, EventArgs e)
        {
            if (mImageInstance != null && mImageInstance.Generator != null && mImageInstance.Bitmap != null)
            {
                if (saveImageDialog == null)
                {
                    saveImageDialog = new SaveFileDialog();

                    // Adds a extension if the user does not
                    saveImageDialog.AddExtension = true;

                    // Restores the selected directory, next time
                    saveImageDialog.RestoreDirectory = true;

                    // Startup directory
                    //saveImageDialog.InitialDirectory = @"C:/";

                    saveImageDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                    saveImageDialog.Title = "Save an Image File";
                }
                saveImageDialog.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveImageDialog.FileName != "")
                {
                    Bitmap image = mImageInstance.Bitmap;
                    // Saves the Image via a FileStream created by the OpenFile method.
                    System.IO.FileStream fs = (System.IO.FileStream)saveImageDialog.OpenFile();
                    // Saves the Image in the appropriate ImageFormat based upon the
                    // File type selected in the dialog box.
                    // NOTE that the FilterIndex property is one-based.
                    switch (saveImageDialog.FilterIndex)
                    {
                        case 1:
                            image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;

                        case 2:
                            image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;

                        case 3:
                            image.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                    }
                    fs.Close();
                }
            }
        }

        public void HandleDisplayDefChange()
        {
            UpdateImage();
            SizeMode = mImageDisplayDef.SizeMode;
            mImageDisplayDef.UpdateDisplayComponents(mTestExecution, ref roiInstances, ref decorationInstances);
            Refresh();
        }

        //private ContextMenu popUpMenu;
        private ImageDisplayDef mImageDisplayDef;

        private ImageInstance mImageInstance;
        public List<IDecorationInstance> decorationInstances = new List<IDecorationInstance>();
        public List<ROIInstance> roiInstances = new List<ROIInstance>();

        void OperationPictureBox_DoubleClick(object sender, EventArgs e)
        {
            mOperationForm.ShowBigImageView();
            mOperationForm.bigImageForm.mOpPictureBox.mTestExecution = mTestExecution;
            mOperationForm.bigImageForm.ImageDisplayDef.Clone(ImageDisplayDef); // copy the image display def for this picture box to the big box
//            mOperationForm.bigImageForm.mOpPictureBox.HandleDisplayDefChange();
            // TODO: copy ShowROI, ShowDecoration settings?
        }
//        private string debugText;


        private OperationForm mOperationForm;
        private ImageForm mParentForm;
        protected TestExecution mTestExecution = null;
        public TestExecution TestExecution
        {
            get { return mTestExecution; }
            set
            {
                mTestExecution = value;
                mImageDisplayDef.UpdateDisplayComponents(mTestExecution, ref roiInstances, ref decorationInstances);
                UpdateImage();
            }
        }

        public ImageDisplayDef ImageDisplayDef
        {
            get { return mImageDisplayDef; }
        }

        /// <summary>
        /// NOTE: mIndex is used by OperationImageForm, but not ImageBigViewForm.  Can we move this to OperationImageForm?
        /// </summary>
        public int mIndex;

        public PictureBoxScale scale = new PictureBoxScale();
        public void UpdateScale()
        {
            if (this.Image == null)
            {
                scale.XScale = 1;
                scale.XOffset = 0;
                scale.YScale = 1;
                scale.YOffset = 0;
                return;
            }
            switch(SizeMode)
            {
                case PictureBoxSizeMode.StretchImage:
                    scale.XScale = (float)this.Width / (float)this.Image.Width;
                    scale.XOffset = 0;
                    scale.YScale = (float)this.Height / (float)this.Image.Height;
                    scale.YOffset = 0;
                    break;
                case PictureBoxSizeMode.Zoom:
                    if ((float)this.Image.Width / (float)this.Width > (float)this.Image.Height / (float)this.Height)
                    {
                        scale.XScale = (float)this.Width / (float)this.Image.Width;
                        scale.XOffset = 0;
                        scale.YScale = scale.XScale;
                        scale.YOffset = (int)((this.Height - (this.Image.Height * scale.YScale)) / 2.0);
                    }
                    else
                    {
                        scale.YScale = (float)this.Height / (float)this.Image.Height;
                        scale.YOffset = 0;
                        scale.XScale = scale.YScale;
                        scale.XOffset = (int)((this.Width - (this.Image.Width * scale.XScale)) / 2.0);
                    }
                    break;
                case PictureBoxSizeMode.CenterImage:
                    scale.XScale = 1;
                    scale.YScale = 1;
                    scale.XOffset = (this.Width - this.Image.Width) / 2;
                    scale.YOffset = (this.Height - this.Image.Height) / 2;
                    break;
                case PictureBoxSizeMode.Normal:
                    scale.XScale = 1;
                    scale.YScale = 1;
                    scale.XOffset = 0;
                    scale.YOffset = 0;
                    break;
                default:
                    throw new ArgumentException("Unsupported size mode in XScale() 238972");
            }
        }
        public float XScale()
        {
            return (float)this.Width / (float)this.Image.Width;
        }

        public float YScale()
        {
            return (float)this.Height / (float)this.Image.Height;
        }

        public void UpdateImage()
        {
            if (mTestExecution == null || mImageDisplayDef == null || mImageDisplayDef.ImageDefinition == null)
            {
                Image = null;
                return;
            }
            mImageInstance = mTestExecution.ImageRegistry.GetObjectIfExists(mImageDisplayDef.ImageDefinition.Name);
            if (mImageInstance == null)
            {
                Image = null;
            }
            else
            {
                Image = mImageInstance.Bitmap;
            }
        }

        void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PictureBox picBox = (PictureBox)sender;

            if (mTestExecution != null && mTestExecution == mOperationForm.CurrentExecution)
            {
                Pen pen = new Pen(Color.Yellow, 2);
                g.DrawRectangle(pen, 1, 1, this.Width-2, this.Height-2);
                pen.Dispose();
            }
            if (picBox.Image == null) return;
            //float xScale = XScale();
            //float yScale = YScale();
            UpdateScale();

            if (mOperationForm.mStudyPoint1 != OperationForm.NotDefinedPoint && mOperationForm.mStudyPoint2 != OperationForm.NotDefinedPoint)
            {
                int upperLeft_X = Math.Min(mOperationForm.mStudyPoint1.X, mOperationForm.mStudyPoint2.X);
                int upperLeft_Y = Math.Min(mOperationForm.mStudyPoint1.Y, mOperationForm.mStudyPoint2.Y);
                int selectionWidth = Math.Abs(mOperationForm.mStudyPoint1.X - mOperationForm.mStudyPoint2.X);
                int selectionHeight = Math.Abs(mOperationForm.mStudyPoint1.Y - mOperationForm.mStudyPoint2.Y);
                Pen pen = new Pen(Color.White, 1);
                g.DrawRectangle(pen, upperLeft_X, upperLeft_Y, selectionWidth, selectionHeight);
                pen.Dispose();
            }

            foreach (ROIInstance roi in roiInstances)
            {
                roi.Draw(g, scale);
            }

            foreach (IDecorationInstance dec in decorationInstances)
            {
                dec.Draw(g, scale);
            }

            if (mOperationForm.toolStripProbeButton.Checked)
            {
                int scaledX = ((int)(mProbeX * scale.XScale)) + scale.XOffset;
                int scaledY = ((int)(mProbeY * scale.YScale)) + scale.YOffset;
                Color color = Color.AliceBlue;
                Pen pen = new Pen(color, 1);
                g.DrawLine(pen, scaledX, scaledY + 10, scaledX, scaledY + 5);
                g.DrawLine(pen, scaledX, scaledY - 10, scaledX, scaledY - 5);
                g.DrawLine(pen, scaledX + 10, scaledY, scaledX + 5, scaledY);
                g.DrawLine(pen, scaledX - 10, scaledY, scaledX - 5, scaledY);
                pen.Dispose();
            }
        }

        private int mProbeX = 0;
        private int mProbeY = 0;
        private void image_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Image == null) return;
            if (mOperationForm.toolStripStudyButton.Checked)
            {
                mOperationForm.mStudyPoint2 = new Point(e.X, e.Y);
                Refresh();
            }
            else if( mOperationForm.toolStripProbeButton.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    PictureBox box = (PictureBox)sender;
                    UpdateScale(); // probably not necessary
                    mProbeX = (int)((e.X - scale.XOffset) / scale.XScale);
                    mProbeY = (int)((e.Y - scale.YOffset) / scale.YScale);
                    box.Focus();
                    UpdateProbeResults(box);
                }
            }

        }
        private void image_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mOperationForm.toolStripStudyButton.Checked)
            {
                mOperationForm.mStudyPoint1 = new Point(e.X, e.Y);
            }
            mOperationForm.CurrentExecution = mTestExecution;
            ((PictureBox)sender).Focus();
        }
        private void image_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mOperationForm.toolStripStudyButton.Checked && this.Image != null && mOperationForm.mStudyPoint1 != null)
            {
                if (mOperationForm.studyChartForm == null)
                {
                    mOperationForm.studyChartForm = new AnalysisForm(mOperationForm);
                    mOperationForm.studyChartForm.Show(mOperationForm.dockPanel);
                }
                mOperationForm.mStudyPoint2 = new Point(e.X, e.Y);

                int upperLeft_X = (int)((Math.Min(mOperationForm.mStudyPoint1.X, mOperationForm.mStudyPoint2.X) - scale.XOffset) / scale.XScale);
                int upperLeft_Y = (int)((Math.Min(mOperationForm.mStudyPoint1.Y, mOperationForm.mStudyPoint2.Y) - scale.YOffset) / scale.YScale);
                int width = (int)(Math.Abs(mOperationForm.mStudyPoint1.X - mOperationForm.mStudyPoint2.X) / scale.XScale);
                int height = (int)(Math.Abs(mOperationForm.mStudyPoint1.Y - mOperationForm.mStudyPoint2.Y) / scale.YScale);
                mOperationForm.studyChartForm.DrawChart(Image, new Rectangle(upperLeft_X, upperLeft_Y, width, height));
                Show();
                mOperationForm.mStudyPoint1 = OperationForm.NotDefinedPoint;
                mOperationForm.mStudyPoint2 = OperationForm.NotDefinedPoint;
                //mOperationForm.toolStripStudyButton.Checked = false;
            }

        }

        private void image_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    e.IsInputKey = true; // make arrow keys show up in KeyDown event just like others...rather than to move controls
                    break;
            }
        }

        private void image_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (mOperationForm.toolStripProbeButton.Checked)
            {
                bool updateProbe = false;
                if (e.KeyData == Keys.Left)
                {
                    mProbeX--;
                    updateProbe = true;
                }
                else if (e.KeyData == Keys.Right)
                {
                    mProbeX++;
                    updateProbe = true;
                }
                else if (e.KeyData == Keys.Up)
                {
                    mProbeY--;
                    updateProbe = true;
                }
                else if (e.KeyData == Keys.Down)
                {
                    mProbeY++;
                    updateProbe = true;
                }
                if (updateProbe)
                {
                    PictureBox box = (PictureBox)sender;
                    UpdateProbeResults(box);
                }
            }
        }

        private void UpdateProbeResults(PictureBox box)
        {
            box.Refresh();
            mOperationForm.lblProbeOutput.Text = /*"ndx=" + mIndex + "  "+ debugText + "  " + */ "x:" + String.Format("{0,-4}", mProbeX) + "y:" + String.Format("{0,-4}", mProbeY);
            if (box.Image == null) return;
            if (mProbeX >= 0 && mProbeY >= 0 && mProbeX < box.Image.Width && mProbeY < box.Image.Height)
            {
                Color color = ((Bitmap)box.Image).GetPixel(mProbeX, mProbeY);
                mOperationForm.lblProbeOutput.Text += " R:" + String.Format("{0,-4}", color.R) + "G:" + String.Format("{0,-4}", color.G) + "B:" + String.Format("{0,-4}", color.B) +
                    " | Gray: " + String.Format("{0,-4}", (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B)) +
                    " | H:" + String.Format("{0:0.0}", color.GetHue()).PadRight(6) + "S:" + String.Format("{0:0.0}", color.GetSaturation() * 100).PadRight(6) + "I:" + String.Format("{0:0.0}", color.GetBrightness() * 100).PadRight(6);
                mOperationForm.lblProbeSample.BackColor = color;
                mOperationForm.lblProbeSample.Visible = true;
            }
            else
            {
                mOperationForm.lblProbeSample.Visible = false;
            }
        }
    }
    public class PictureBoxScale
    {
        public float XScale;
        public int XOffset;
        public float YScale;
        public int YOffset;
    }
}

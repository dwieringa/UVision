// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;


namespace NetCams
{
    public class ColorMatchCountOverRectangleInstance : ToolInstance
    {
        public ColorMatchCountOverRectangleInstance(ColorMatchCountOverRectangleDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{

			// TODO: should this effort be part of the definition?
            mColorMatcher = testExecution.GetColorMatcher(theDefinition.ColorMatchDefinition.Name);
            ROIInstance theROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);
            if (theROI is IRectangleROIInstance)
            {
                mROI = (IRectangleROIInstance)theROI;
            }
            else
            {
                throw new ArgumentException("ColorMatchCount currently only supports Rectangle ROIs.");
            }
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);
            mResult = new GeneratedValueInstance(theDefinition.Result, testExecution);

            mCreateMarkedImage = theDefinition.CreateMarkedImage;
            mMarkColor = theDefinition.MarkColor;
            if (theDefinition.ImageToMark != null)
            {
                mImageToMark = testExecution.ImageRegistry.GetObject(theDefinition.ImageToMark.Name);
            }
        }

        private ColorMatchInstance mColorMatcher;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Color match rules")]
        public ColorMatchInstance ColorMatchDefinition
        {
            get { return mColorMatcher; }
        }

        private long mMatchCount = 0;
        [CategoryAttribute("Output"),
        DescriptionAttribute("Number of matching pixels counted")]
        public long MatchCountValue
        {
            get { return mMatchCount; }
        }

        private IRectangleROIInstance mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("ROI to test over")]
        public IRectangleROIInstance ROI
        {
            get { return mROI; }
        }

        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private GeneratedValueInstance mResult = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance Result
        {
            get { return mResult; }
        }

        private bool mCreateMarkedImage = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Set to True to create a marked up image")]
        public bool CreateMarkedImage
        {
            get { return mCreateMarkedImage; }
        }
        private Color mMarkColor;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("The color which is used to highlight areas that are detected as a mark")]
        public Color MarkColor
        {
            get { return mMarkColor; }
        }
        private ImageInstance mImageToMark = null;
        [CategoryAttribute("Output")]
        public ImageInstance ImageToMark
        {
            get { return mImageToMark; }
        }

		public override bool IsComplete() { return mResult.IsComplete(); }

//		public const string AnalysisType = "Color Present Fails";
//		public override string Type() { return AnalysisType; }

		public override void DoWork() 
		{
            //if (!mSourceImage.IsComplete() || !mROI.IsComplete() || !AreExplicitDependenciesComplete()) return;

            DateTime startTime = DateTime.Now;

            Bitmap sourceBitmap = SourceImage.Bitmap;
            Bitmap markedBitmap = null;

            TestExecution().LogMessageWithTimeFromTrigger("ColorMatchCount " + Name + " started");

            // All ROI are taken at 640x480
			// We need to scale ROIs to the actual image dimensions
//            RectangleROI rectangleROI = ScaleROI(roi.NetworkCamera().Resolution);
//            RectangleROI_old rectangleROI = Project().FindCamera(Camera).GetROI(TestExecution(),ROI);

            if (mCreateMarkedImage && mImageToMark != null && mImageToMark.Bitmap != null)
            {
                markedBitmap = mImageToMark.Bitmap;
            }

            mMatchCount = 0;
            if (sourceBitmap != null)
            {
                // for LockBits see http://www.bobpowell.net/lockingbits.htm & http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                BitmapData sourceBitmapData = null;
                BitmapData markedBitmapData = null;
                try
                {
                    sourceBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    if (markedBitmap != null)
                    {
                        markedBitmapData = markedBitmap.LockBits(new Rectangle(0, 0, markedBitmap.Width, markedBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    }
                    const int pixelByteWidth = 4; // determined by PixelFormat.Format32bppArgb
                    int stride = sourceBitmapData.Stride;
                    int strideOffset = stride - (sourceBitmapData.Width * pixelByteWidth);

                    int bottom = Math.Min(sourceBitmap.Height - 1, ROI.Bottom);
                    int top = Math.Max(0, ROI.Top);
                    int left = Math.Max(0, ROI.Left);
                    int right = Math.Min(sourceBitmap.Width - 1, ROI.Right);
                    Color color;

                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* sourcePointer;
                        byte* markedPointer;
                        for (int j = top; j <= bottom; j++)
                        {
                            sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                            sourcePointer += (j * stride) + (left * pixelByteWidth); // adjust to first byte of ROI
                            for (int i = left; i <= right; i++)
                            {
                                color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                if (mColorMatcher.Matches(color))
                                {
                                    mMatchCount++;
                                    if (markedBitmap != null)
                                    {
                                        markedPointer = (byte*)markedBitmapData.Scan0;
                                        markedPointer += (j * stride) + (i * pixelByteWidth);
                                        markedPointer[3] = Color.Magenta.A;
                                        markedPointer[2] = Color.Magenta.R;
                                        markedPointer[1] = Color.Magenta.G;
                                        markedPointer[0] = Color.Magenta.B;
                                    }
                                }
                                sourcePointer += pixelByteWidth; // adjust to next pixel to the right
                            }
                            //sourcePointer += (((width-right) * pixelByteWidth) + strideOffset + (left * pixelByteWidth)); // adjust to the first pixel of the next row by skipping the "extra bytes" (stride offset)
                        }
                    } // end unsafe block
                }
                finally
                {
                    sourceBitmap.UnlockBits(sourceBitmapData);
                    if (markedBitmap != null)
                    {
                        markedBitmap.UnlockBits(markedBitmapData);
                    }
                }
            }

            mResult.SetValue( mMatchCount );

            mResult.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " took " + computeTime.TotalMilliseconds + "ms");
            //MessageBox.Show("done in color count for " + Name);
        }
    }
}

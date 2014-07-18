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
    public class ColorMatchCountInstance : ToolInstance
    {
        public ColorMatchCountInstance(ColorMatchCountDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.ColorMatchDefinition == null) throw new ArgumentException(Name + " doesn't have ColorMatchDefinition defined.");
            mColorMatcher = testExecution.GetColorMatcher(theDefinition.ColorMatchDefinition.Name);

            if (theDefinition.ROI == null) throw new ArgumentException(Name + " doesn't have ROI defined.");
            mROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);

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

        private ROIInstance mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("ROI to test over")]
        public ROIInstance ROI
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
            TestExecution().LogMessageWithTimeFromTrigger("ColorMatchCount " + Name + " started");

            DateTime startTime = DateTime.Now;

            mMatchCount = 0;
            if (mSourceImage.Bitmap != null)
            {
                if (true)
                {
                    Point currentPoint = new Point(-1, -1);
                    mROI.GetFirstPointOnXAxis(mSourceImage, ref currentPoint);

                    Color color;
                    while (currentPoint.X != -1 && currentPoint.Y != -1)
                    {
                        color = mSourceImage.GetColor(currentPoint.X, currentPoint.Y);
                        if (mColorMatcher.Matches(color))
                        {
                            mMatchCount++;
                            if (mImageToMark != null && mImageToMark.Bitmap != null)
                            {
                                mImageToMark.SetColor(currentPoint.X, currentPoint.Y, mMarkColor);
                            }
                        }
                        mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
                    }
                }
                else
                {
                    Bitmap sourceBitmap = SourceImage.Bitmap;
                    Bitmap markedBitmap = null;

                    if (mCreateMarkedImage && mImageToMark != null && mImageToMark.Bitmap != null)
                    {
                        markedBitmap = mImageToMark.Bitmap;
                    }

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

                        Point currentPoint = new Point(-1, -1);
                        mROI.GetFirstPointOnXAxis(mSourceImage, ref currentPoint);

                        unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                        {
                            byte* sourcePointer;
                            byte* markedPointer;

                            Color color;
                            while (currentPoint.X != -1 && currentPoint.Y != -1)
                            {
                                sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                sourcePointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth); // adjust to current point
                                color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                if (mColorMatcher.Matches(color))
                                {
                                    mMatchCount++;
                                    if (markedBitmap != null)
                                    {
                                        markedPointer = (byte*)markedBitmapData.Scan0;
                                        markedPointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth);
                                        markedPointer[3] = mMarkColor.A;
                                        markedPointer[2] = mMarkColor.R;
                                        markedPointer[1] = mMarkColor.G;
                                        markedPointer[0] = mMarkColor.B;
                                    }
                                }
                                mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
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

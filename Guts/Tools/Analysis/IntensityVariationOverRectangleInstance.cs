// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;


namespace NetCams
{
    class IntensityVariationOverRectangleInstance : ToolInstance
    {
        public IntensityVariationOverRectangleInstance(IntensityVariationOverRectangleDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.VariationThreshhold == null) throw new ArgumentException("Intensity Variation tool '" + theDefinition.Name + "' doesn't have a value assigned to VariationThreshhold");
            mVariationThreshhold = testExecution.DataValueRegistry.GetObject(theDefinition.VariationThreshhold.Name);
            mTestXAxis = theDefinition.TestXAxis;
            mTestYAxis = theDefinition.TestYAxis;
            ROIInstance theROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);
            if (theROI is IRectangleROIInstance)
            {
                mROI = (IRectangleROIInstance)theROI;
            }
            else
            {
                throw new ArgumentException("IntensityVariation currently only supports Rectangle ROIs.");
            }
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);
            mResult = new GeneratedValueInstance(theDefinition.Result, testExecution);

            mCreateMarkedImage = theDefinition.CreateMarkedImage;
            if (mCreateMarkedImage)
            {
                mMarkedImage = new GeneratedImageInstance(theDefinition.MarkedImage, testExecution);
            }
        }

        private DataValueInstance mVariationThreshhold;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Threshhold")]
        public DataValueInstance VariationThreshhold
        {
            get { return mVariationThreshhold; }
        }

        private bool mTestXAxis;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Measure variation over the X-axis?")]
        public bool TestXAxis
        {
            get { return mTestXAxis; }
        }

        private bool mTestYAxis;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Measure variation over the Y-axis?")]
        public bool TestYAxis
        {
            get { return mTestYAxis; }
        }

        private IRectangleROIInstance mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("ROI to test over")]
        public IRectangleROIInstance ROI
        {
            get { return mROI; }
        }

        private bool mCreateMarkedImage = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Set to True to create a marked up image")]
        public bool CreateMarkedImage
        {
            get { return mCreateMarkedImage; }
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

        private GeneratedImageInstance mMarkedImage = null;
        [CategoryAttribute("Output")]
        public GeneratedImageInstance MarkedImage
        {
            get { return mMarkedImage; }
        }


		public override bool IsComplete() { return mResult.IsComplete(); }

//		public const string AnalysisType = "Color Present Fails";
//		public override string Type() { return AnalysisType; }

		public override void DoWork() 
		{
            //if (!mSourceImage.IsComplete() || !AreExplicitDependenciesComplete()) return;

            Bitmap sourceBitmap = SourceImage.Bitmap;
            Bitmap markedBitmap = null;

            TestExecution().LogMessageWithTimeFromTrigger("IntensityVariation " + Name + " started");

            if (mMarkedImage != null && sourceBitmap != null)
            {
                mMarkedImage.SetImage(new Bitmap(sourceBitmap));
                markedBitmap = mMarkedImage.Bitmap;
                TestExecution().LogMessageWithTimeFromTrigger("Created copy of image for markings");
            }

            long resultValue = 0;
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

                    Color color;
                    int pixel1Intensity;
                    int pixel2Intensity;
                    long variation = 0;
                    long threshhold = mVariationThreshhold.ValueAsLong();

                    int bottom = Math.Min(sourceBitmap.Height - 1, ROI.Bottom);
                    int top = Math.Max(0, ROI.Top);
                    int left = Math.Max(0, ROI.Left);
                    int right = Math.Min(sourceBitmap.Width - 1, ROI.Right);
                    if (mTestXAxis)
                    {
                        TestExecution().LogMessageWithTimeFromTrigger("IntensityVariation " + Name + " testing X Axis");
                        unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                        {
                            byte* sourcePointer;
                            byte* markedPointer;

                            for (int j = top; j <= bottom; j++)
                            {
                                sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                sourcePointer += (j * stride) + (left * pixelByteWidth); // adjust to first byte of ROI row

                                color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                pixel1Intensity = (int)(color.GetBrightness() * 100);
                                sourcePointer += pixelByteWidth; // adjust to next pixel to the right
                                for (int i = left + 1; i <= right; i++) // starting at left+1 since we already have the value for "left"
                                {
                                    color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                    pixel2Intensity = (int)(color.GetBrightness() * 100);
                                    variation = Math.Abs(pixel2Intensity - pixel1Intensity);
                                    if (variation > threshhold)
                                    {
                                        resultValue += variation;
                                        if (mMarkedImage != null)
                                        {
                                            markedPointer = (byte*)markedBitmapData.Scan0;
                                            markedPointer += (j * stride) + (i * pixelByteWidth);
                                            markedPointer[3] = Color.Yellow.A;
                                            markedPointer[2] = Color.Yellow.R;
                                            markedPointer[1] = Color.Yellow.G;
                                            markedPointer[0] = Color.Yellow.B;
                                        }
                                    }
                                    pixel1Intensity = pixel2Intensity;
                                    sourcePointer += pixelByteWidth; // adjust to next pixel to the right
                                }
                                //sourcePointer += ((width-right)*pixelByteWidth) + strideOffset + (left * pixelByteWidth); // adjust to the first pixel of the next row by skipping the "extra bytes" (stride offset)
                            }
                        } // end unsafe block
                    }
                    if (mTestYAxis)
                    {
                        TestExecution().LogMessageWithTimeFromTrigger("IntensityVariation " + Name + " testing X Axis");
                        unsafe
                        {
                            byte* sourcePointer;
                            byte* markedPointer;

                            for (int i = left; i <= right; i++)
                            {
                                sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                sourcePointer += (top * stride) + (i * pixelByteWidth); // adjust to top pixel of the column

                                // get value for pixel on top of column...to init our for loop below (loop references two values)
                                color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                pixel1Intensity = (int)(color.GetBrightness() * 100);
                                sourcePointer += stride; // adjust to next pixel down the column

                                for (int j = top + 1; j <= bottom; j++) // starting at top+1 since we already have the value for "top"
                                {
                                    color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                    pixel2Intensity = (int)(color.GetBrightness() * 100);
                                    variation = Math.Abs(pixel2Intensity - pixel1Intensity);
                                    if (variation > threshhold)
                                    {
                                        resultValue += variation;
                                        if (mMarkedImage != null)
                                        {
                                            markedPointer = (byte*)markedBitmapData.Scan0;
                                            markedPointer += (j * stride) + (i * pixelByteWidth);
                                            markedPointer[3] = Color.Yellow.A;
                                            markedPointer[2] = Color.Yellow.R;
                                            markedPointer[1] = Color.Yellow.G;
                                            markedPointer[0] = Color.Yellow.B;
                                        }
                                    }
                                    pixel1Intensity = pixel2Intensity;
                                    sourcePointer += stride; // move down one pixel on the y-axis
                                }
                            }
                        } // end unsafe block
                    }
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

            mResult.SetValue(resultValue);
            mResult.SetIsComplete();
            if( mMarkedImage != null ) mMarkedImage.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("IntensityVariation " + Name + " completed");
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;


namespace NetCams
{
    public class IntensityVariationInstance : ToolInstance
    {
        public IntensityVariationInstance(IntensityVariationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.VariationThreshhold == null) throw new ArgumentException("Intensity Variation tool '" + theDefinition.Name + "' doesn't have a value assigned to VariationThreshhold");
            mVariationThreshhold = testExecution.DataValueRegistry.GetObject(theDefinition.VariationThreshhold.Name);
            mTestXAxis = theDefinition.TestXAxis;
            mTestYAxis = theDefinition.TestYAxis;
            mROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);
            mResult = new GeneratedValueInstance(theDefinition.Result, testExecution);

            mCreateMarkedImage = theDefinition.CreateMarkedImage;
            if (mCreateMarkedImage)
            {
                mMarkedImage = new GeneratedImageInstance(theDefinition.MarkedImage, testExecution);
            }
            mMarkColor = theDefinition.MarkColor;
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

        private ROIInstance mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("ROI to test over")]
        public ROIInstance ROI
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

        private Color mMarkColor;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("The color which is used to highlight areas that match the color definition")]
        public Color MarkColor
        {
            get { return mMarkColor; }
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

            TestExecution().LogMessageWithTimeFromTrigger("IntensityVariation " + Name + " started");

            Bitmap sourceBitmap = SourceImage.Bitmap;
            Bitmap markedBitmap = null;

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

                    Point lastPoint = new Point(-1, -1);
                    Point currentPoint = new Point(-1, -1);
                    if (mTestXAxis)
                    {
                        TestExecution().LogMessageWithTimeFromTrigger("IntensityVariation " + Name + " testing X Axis");
                        mROI.GetFirstPointOnXAxis(mSourceImage, ref currentPoint);

                        unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                        {
                            byte* sourcePointer;
                            byte* markedPointer;

                            sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                            sourcePointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth); // adjust to current point
                            color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                            pixel1Intensity = (int)(color.GetBrightness() * 100);

                            lastPoint.X = currentPoint.X;
                            lastPoint.Y = currentPoint.Y;

                            mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
                            while (currentPoint.X != -1 && currentPoint.Y != -1)
                            {
                                sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                sourcePointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth); // adjust to current point
                                color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                pixel2Intensity = (int)(color.GetBrightness() * 100);

                                if (currentPoint.Y == lastPoint.Y)
                                {
                                    variation = Math.Abs(pixel2Intensity - pixel1Intensity);
                                    if (variation > threshhold)
                                    {
                                        resultValue += variation;
                                        if (mMarkedImage != null)
                                        {
                                            markedPointer = (byte*)markedBitmapData.Scan0;
                                            markedPointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth);
                                            markedPointer[3] = mMarkColor.A;
                                            markedPointer[2] = mMarkColor.R;
                                            markedPointer[1] = mMarkColor.G;
                                            markedPointer[0] = mMarkColor.B;
                                        }
                                    }
                                }
                                pixel1Intensity = pixel2Intensity;
                                lastPoint.X = currentPoint.X;
                                lastPoint.Y = currentPoint.Y;
                                mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
                            }
                        } // end unsafe block
                    }
                    if (mTestYAxis)
                    {
                        TestExecution().LogMessageWithTimeFromTrigger("IntensityVariation " + Name + " testing Y Axis");
                        mROI.GetFirstPointOnYAxis(mSourceImage, ref currentPoint);

                        unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                        {
                            byte* sourcePointer;
                            byte* markedPointer;

                            sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                            sourcePointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth); // adjust to current point
                            color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                            pixel1Intensity = (int)(color.GetBrightness() * 100);

                            lastPoint.X = currentPoint.X;
                            lastPoint.Y = currentPoint.Y;

                            mROI.GetNextPointOnYAxis(mSourceImage, ref currentPoint);
                            while (currentPoint.X != -1 && currentPoint.Y != -1)
                            {
                                sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                sourcePointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth); // adjust to current point
                                color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                pixel2Intensity = (int)(color.GetBrightness() * 100);

                                if (currentPoint.X == lastPoint.X)
                                {
                                    variation = Math.Abs(pixel2Intensity - pixel1Intensity);
                                    if (variation > threshhold)
                                    {
                                        resultValue += variation;
                                        if (mMarkedImage != null)
                                        {
                                            markedPointer = (byte*)markedBitmapData.Scan0;
                                            markedPointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth);
                                            markedPointer[3] = mMarkColor.A;
                                            markedPointer[2] = mMarkColor.R;
                                            markedPointer[1] = mMarkColor.G;
                                            markedPointer[0] = mMarkColor.B;
                                        }
                                    }
                                }
                                pixel1Intensity = pixel2Intensity;
                                lastPoint.X = currentPoint.X;
                                lastPoint.Y = currentPoint.Y;
                                mROI.GetNextPointOnYAxis(mSourceImage, ref currentPoint);
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

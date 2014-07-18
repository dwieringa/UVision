// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;


namespace NetCams
{
    public class PatternMatchOfAbsGrayVariationInstance : ImageScorerInstance
    {
        public PatternMatchOfAbsGrayVariationInstance(PatternMatchOfAbsGrayVariationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.VariationThreshhold == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to VariationThreshhold");
            mVariationThreshhold = testExecution.DataValueRegistry.GetObject(theDefinition.VariationThreshhold.Name);

            if (theDefinition.Sloppiness == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to Sloppiness");
            mSloppiness = testExecution.DataValueRegistry.GetObject(theDefinition.Sloppiness.Name);

            if (theDefinition.MinWindow == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to MinWindow");
            mMinWindow = testExecution.DataValueRegistry.GetObject(theDefinition.MinWindow.Name);

            if (theDefinition.BrightPixelFactor == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to BrightPixelFactor");
            mBrightPixelFactor = testExecution.DataValueRegistry.GetObject(theDefinition.BrightPixelFactor.Name);

            if (theDefinition.DarkPixelFactor == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to DarkPixelFactor");
            mDarkPixelFactor = testExecution.DataValueRegistry.GetObject(theDefinition.DarkPixelFactor.Name);

            if (theDefinition.ROI == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have an ROI assigned");
            mROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);

            if (theDefinition.SourceImage == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have an image assigned to SourceImage");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            mDeepAnalysisEnabled = theDefinition.DeepAnalysisEnabled;
            mDeepAnalysisTop = theDefinition.DeepAnalysisTop;
            mDeepAnalysisBottom = theDefinition.DeepAnalysisBottom;
            mDeepAnalysisRight = theDefinition.DeepAnalysisRight;
            mDeepAnalysisLeft = theDefinition.DeepAnalysisLeft;

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

        private DataValueInstance mSloppiness;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("A percentage indicating how far a pixel can be outside the min/max values before it is scored as a flaw.")]
        public DataValueInstance Sloppiness
        {
            get { return mSloppiness; }
        }

        private DataValueInstance mMinWindow;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance MinWindow
        {
            get { return mMinWindow; }
        }

        private DataValueInstance mBrightPixelFactor;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("BrightPixelFactor")]
        public DataValueInstance BrightPixelFactor
        {
            get { return mBrightPixelFactor; }
        }

        private DataValueInstance mDarkPixelFactor;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("DarkPixelFactor")]
        public DataValueInstance DarkPixelFactor
        {
            get { return mDarkPixelFactor; }
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

        private bool mDeepAnalysisEnabled;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public bool DeepAnalysisEnabled
        {
            get { return mDeepAnalysisEnabled; }
        }

        private int mDeepAnalysisTop;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int DeepAnalysisTop
        {
            get { return mDeepAnalysisTop; }
        }

        private int mDeepAnalysisBottom;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int DeepAnalysisBottom
        {
            get { return mDeepAnalysisBottom; }
        }

        private int mDeepAnalysisLeft;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int DeepAnalysisLeft
        {
            get { return mDeepAnalysisLeft; }
        }

        private int mDeepAnalysisRight;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int DeepAnalysisRight
        {
            get { return mDeepAnalysisRight; }
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

        /*
        private ScoreFilterInstance mScoreFilter = null;
        [CategoryAttribute("Output")]
        public ScoreFilterInstance ScoreFilter
        {
            get { return mScoreFilter; }
        }
         */
        
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
            TestExecution().LogMessageWithTimeFromTrigger("PatternMatch " + Name + " started");

            /*
            if (Definition(.ScoreFilter != null)
            {
                mScoreFilter = testExecution.GetScoreFilter(theDefinition.ScoreFilter.Name);
            }
            */

            Bitmap sourceBitmap = SourceImage.Bitmap;
            Bitmap markedBitmap = null;
            PatternMatchOfAbsGrayVariationDefinition theDef = (PatternMatchOfAbsGrayVariationDefinition)Definition();
            if (theDef.mPatternMinValues == null || theDef.mPatternMaxValues == null)
            {
                theDef.LoadPatterns(false);
            }
            Bitmap patternMinValues = theDef.mPatternMinValues;
            Bitmap patternMaxValues = theDef.mPatternMaxValues;

            if (patternMinValues == null || patternMaxValues == null)
            {
                throw new ArgumentException("Pattern to match isn't defined.");
            }

            if (mMarkedImage != null && sourceBitmap != null)
            {
                mMarkedImage.SetImage(new Bitmap(sourceBitmap));
                markedBitmap = mMarkedImage.Bitmap;
                TestExecution().LogMessageWithTimeFromTrigger("Created copy of image for markings");
            }

            long score = 0;
            if (sourceBitmap != null)
            {
                // for LockBits see http://www.bobpowell.net/lockingbits.htm & http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                BitmapData sourceBitmapData = null;
                BitmapData markedBitmapData = null;
                BitmapData patternMinValuesBitmapData = null;
                BitmapData patternMaxValuesBitmapData = null;

                if (mScoreFilter != null)
                {
                    mScoreFilter.SetImageSize(mSourceImage.Bitmap.Width, mSourceImage.Bitmap.Height);
                }

                try
                {
                    sourceBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PatternMatchOfAbsGrayVariationDefinition.TRAINING_PIXEL_FORMAT);
                    patternMinValuesBitmapData = patternMinValues.LockBits(new Rectangle(0, 0, patternMinValues.Width, patternMinValues.Height), ImageLockMode.ReadOnly, PatternMatchOfAbsGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    patternMaxValuesBitmapData = patternMaxValues.LockBits(new Rectangle(0, 0, patternMaxValues.Width, patternMaxValues.Height), ImageLockMode.ReadOnly, PatternMatchOfAbsGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    if (markedBitmap != null)
                    {
                        markedBitmapData = markedBitmap.LockBits(new Rectangle(0, 0, markedBitmap.Width, markedBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    }
                    int sourceStride = sourceBitmapData.Stride;
                    int sourceStrideOffset = sourceStride - (sourceBitmapData.Width * PatternMatchOfAbsGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH);

                    int patternStride = patternMinValuesBitmapData.Stride;
                    int patternStrideOffset = patternStride - (patternMinValuesBitmapData.Width * PatternMatchOfAbsGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH);

                    Color color;
                    int grayValue;
                    int grayValue2;
                    long variation = 0;
                    long patternWindow = 0;
                    long threshhold = mVariationThreshhold.ValueAsLong();
                    double sloppiness = mSloppiness.ValueAsDecimal() / 100.0;
                    long minWindow = Math.Max(1, mMinWindow.ValueAsLong());
                    double brightPixelFactor = mBrightPixelFactor.ValueAsDecimal();
                    double darkPixelFactor = mDarkPixelFactor.ValueAsDecimal();
                    bool needToMark = false;
                    long scoreChange = 0;
                    int varSum;

                    Point currentPoint = new Point(-1, -1);

                    TestExecution().LogMessageWithTimeFromTrigger("PatternMatch " + Name + " testing X Axis");
                    mROI.GetFirstPointOnXAxis(mSourceImage, ref currentPoint);

                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* sourcePointer;
                        byte* sourcePointer2;
                        byte* markedPointer;
                        byte* patternMinValuesPointer;
                        byte* patternMaxValuesPointer;

                        while (currentPoint.X != -1 && currentPoint.Y != -1)
                        {
                            scoreChange = -999;
                            variation = -999;
                            sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                            sourcePointer += (currentPoint.Y * sourceStride) + (currentPoint.X * PatternMatchOfAbsGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH); // adjust to current point
                            color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                            grayValue = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            // http://www.bobpowell.net/grayscale.htm
                            // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1

                            // check pixel above
                            sourcePointer2 = sourcePointer - sourceStride;
                            grayValue2 = (int)(0.3 * sourcePointer2[2] + 0.59 * sourcePointer2[1] + 0.11 * sourcePointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            varSum = Math.Abs(grayValue - grayValue2); // NOTE: using '=' to init varSum for this pixel

/*
                            // check pixel below
                            sourcePointer2 = sourcePointer + sourceStride;
                            grayValue2 = (int)(0.3 * sourcePointer2[2] + 0.59 * sourcePointer2[1] + 0.11 * sourcePointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            varSum += Math.Abs(grayValue - grayValue2); // NOTE: using '+=' to sum varSum

                            // check left pixel
                            sourcePointer2 = sourcePointer - PatternMatchOfAbsGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH;
                            grayValue2 = (int)(0.3 * sourcePointer2[2] + 0.59 * sourcePointer2[1] + 0.11 * sourcePointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            varSum += Math.Abs(grayValue - grayValue2); // NOTE: using '+=' to sum varSum

                            // check right pixel
                            sourcePointer2 = sourcePointer + PatternMatchOfAbsGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH;
                            grayValue2 = (int)(0.3 * sourcePointer2[2] + 0.59 * sourcePointer2[1] + 0.11 * sourcePointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            varSum += Math.Abs(grayValue - grayValue2); // NOTE: using '+=' to sum varSum
                            */

                            varSum = Math.Min(255, varSum);

                            patternMinValuesPointer = (byte*)patternMinValuesBitmapData.Scan0; // init to first byte of image
                            patternMinValuesPointer += (currentPoint.Y * patternStride) + (currentPoint.X * PatternMatchOfAbsGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                            patternMaxValuesPointer = (byte*)patternMaxValuesBitmapData.Scan0; // init to first byte of image
                            patternMaxValuesPointer += (currentPoint.Y * patternStride) + (currentPoint.X * PatternMatchOfAbsGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                            patternWindow = patternMaxValuesPointer[0] - patternMinValuesPointer[0]; // give tight windows more weight in the score
                            patternWindow = Math.Max(minWindow, patternWindow); // ensure minWindow>0 to prevent divideBy0

                            if (patternWindow > threshhold)
                            {
                                scoreChange = 0;
                                markedPointer = (byte*)markedBitmapData.Scan0;
                                markedPointer += (currentPoint.Y * sourceStride) + (currentPoint.X * PatternMatchOfAbsGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH);
                                markedPointer[3] = Color.Yellow.A;
                                markedPointer[2] = Color.Yellow.R;
                                markedPointer[1] = Color.Yellow.G;
                                markedPointer[0] = Color.Yellow.B;
                            }
                            else
                            {
                                if (varSum < patternMinValuesPointer[0] - sloppiness * patternWindow)
                                {
                                    variation = patternMinValuesPointer[0] - varSum;
                                    //scoreChange = (long)(((variation / patternWindow) + 1) * darkPixelFactor);
                                    scoreChange = (long)(variation*((variation / (patternWindow/2)) + 1) * darkPixelFactor);
                                    score += scoreChange;
                                    needToMark = true;
                                    TestExecution().LogMessage("Pattern Match score event: " + currentPoint.X + "," + currentPoint.Y + "  dark spot score=" + scoreChange + "  var=" + varSum + "  min=" + patternMinValuesPointer[0] + "  max=" + patternMaxValuesPointer[0] + "  window=" + patternWindow + "  var=" + variation);
                                    if (mScoreFilter != null) mScoreFilter.ProcessScore(currentPoint.X, currentPoint.Y, scoreChange);
                                }
                                else if (varSum > patternMaxValuesPointer[0] + sloppiness * patternWindow)
                                {
                                    variation = varSum - patternMaxValuesPointer[0];
                                    //scoreChange = (long)(((variation / patternWindow) + 1) * brightPixelFactor);
                                    scoreChange = (long)(variation * ((variation / (patternWindow / 2)) + 1) * brightPixelFactor);
                                    score += scoreChange;
                                    needToMark = true;
                                    TestExecution().LogMessage("Pattern Match score event: " + currentPoint.X + "," + currentPoint.Y + "  bright spot score=" + scoreChange + "  var=" + varSum + "  min=" + patternMinValuesPointer[0] + "  max=" + patternMaxValuesPointer[0] + "  window=" + patternWindow + "  var=" + variation);
                                    if (mScoreFilter != null) mScoreFilter.ProcessScore(currentPoint.X, currentPoint.Y, scoreChange);
                                }
                                else
                                {
                                    variation = 0;
                                    scoreChange = 0;
                                    needToMark = false;
                                }
                                if (needToMark && mMarkedImage != null)
                                {
                                    markedPointer = (byte*)markedBitmapData.Scan0;
                                    markedPointer += (currentPoint.Y * sourceStride) + (currentPoint.X * PatternMatchOfAbsGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH);
                                    markedPointer[3] = mMarkColor.A;
                                    markedPointer[2] = mMarkColor.R;
                                    markedPointer[1] = mMarkColor.G;
                                    markedPointer[0] = mMarkColor.B;
                                }
                            }

                            if (mDeepAnalysisEnabled && currentPoint.X >= mDeepAnalysisLeft && currentPoint.X <= mDeepAnalysisRight && currentPoint.Y >= mDeepAnalysisTop && currentPoint.Y <= mDeepAnalysisBottom)
                            {
                                string message = "DEEP ANALYSIS: '" + Name + "' " + currentPoint.X + "," + currentPoint.Y + " ";
                                if (patternWindow > threshhold) message += "PATTERN WINDOW > THRESHOLD;";
                                message += "  score change=" + scoreChange
                                    + "  var=" + varSum
                                    + "  min=" + patternMinValuesPointer[0]
                                    + "  max=" + patternMaxValuesPointer[0]
                                    + "  window=" + patternWindow
                                    + "  slop=" + (sloppiness * patternWindow)
                                    + "  marked=" + needToMark
                                    ;
                                
                                TestExecution().LogMessage(message);
                            }

                            mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
                        }
                    } // end unsafe block
                }
                finally
                {
                    sourceBitmap.UnlockBits(sourceBitmapData);
                    patternMinValues.UnlockBits(patternMinValuesBitmapData);
                    patternMaxValues.UnlockBits(patternMaxValuesBitmapData);
                    if (markedBitmap != null)
                    {
                        markedBitmap.UnlockBits(markedBitmapData);
                    }
                }
            }

            mResult.SetValue(score);
            mResult.SetIsComplete();
            if( mMarkedImage != null ) mMarkedImage.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("PatternMatch " + Name + " completed; score="+score);
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;


namespace NetCams
{
    public class PatternMatchOfGrayVariationInstance : ImageScorerInstance
    {
        public PatternMatchOfGrayVariationInstance(PatternMatchOfGrayVariationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.VariationThreshhold == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to VariationThreshhold");
            mVariationThreshhold = testExecution.DataValueRegistry.GetObject(theDefinition.VariationThreshhold.Name);

            if (theDefinition.Sloppiness == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to Sloppiness");
            mSloppiness = testExecution.DataValueRegistry.GetObject(theDefinition.Sloppiness.Name);

            if (theDefinition.MinWindow == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to MinWindow");
            mMinWindow = testExecution.DataValueRegistry.GetObject(theDefinition.MinWindow.Name);

            if (theDefinition.ScoreThreshold == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to ScoreThreshold");
            mScoreThreshold = testExecution.DataValueRegistry.GetObject(theDefinition.ScoreThreshold.Name);

            if (theDefinition.BrightPixelFactor == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to BrightPixelFactor");
            mBrightPixelFactor = testExecution.DataValueRegistry.GetObject(theDefinition.BrightPixelFactor.Name);

            if (theDefinition.DarkPixelFactor == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have a value assigned to DarkPixelFactor");
            mDarkPixelFactor = testExecution.DataValueRegistry.GetObject(theDefinition.DarkPixelFactor.Name);

            if (theDefinition.ROI == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have an ROI assigned");
            mROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);

            if (theDefinition.SourceImage == null) throw new ArgumentException("Pattern Match tool '" + theDefinition.Name + "' doesn't have an image assigned to SourceImage");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            mCollectImages = theDefinition.CollectImages;

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

            mMaxDebugDetails = theDefinition.MaxDebugDetails;
            mAutoSaveOnScore = theDefinition.AutoSaveOnScore;
            mAutoSaveOnCellScore = theDefinition.AutoSaveOnCellScore;
        }

        private int mMaxDebugDetails;
        private int mDebugDetailsCount = 0;
        private int mAutoSaveOnScore;
        private int mAutoSaveOnCellScore;

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

        private DataValueInstance mScoreThreshold;
        [CategoryAttribute("Scoring"),
        DescriptionAttribute("Scores below this value are ignored.")]
        public DataValueInstance ScoreThreshold
        {
            get { return mScoreThreshold; }
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

        private long minWindow = 1;
        private long threshhold;
        private double sloppiness;
        private BitmapData markedBitmapData = null;
        private long score = 0;
        private long scoreThreshold;
        private double brightPixelFactor;
        private double darkPixelFactor;
        private int sourceStride;

        private bool mCollectImages = false;
        [CategoryAttribute("Learning"),
        DescriptionAttribute("")]
        public bool CollectImages
        {
            get { return mCollectImages; }
        }

        public override void DoWork() 
		{
            if( mCollectImages )
            {
                try
                {
                    mSourceImage.Save(((PatternMatchOfGrayVariationDefinition)Definition()).LearningPath, Name, true);
                    TestExecution().LogMessageWithTimeFromTrigger(Name + " collected image in learning folder.  Skipping test.");
                }
                catch (ArgumentException e)
                {
                    Project().Window().logMessage("ERROR: " + e.Message);
                    TestExecution().LogErrorWithTimeFromTrigger(e.Message);
                }
                catch (Exception e)
                {
                    string errMsg = "Unable to save collected image.  Ensure path valid and disk not full.";
                    Project().Window().logMessage("ERROR: " + errMsg + "  Low-level message=" + e.Message);
                    TestExecution().LogErrorWithTimeFromTrigger(errMsg);
                }
                mResult.SetValue(score);
                mResult.SetIsComplete();
                return;
            }

            TestExecution().LogMessageWithTimeFromTrigger("PatternMatch " + Name + " started");

            /*
            if (Definition(.ScoreFilter != null)
            {
                mScoreFilter = testExecution.GetScoreFilter(theDefinition.ScoreFilter.Name);
            }
            */

            Bitmap sourceBitmap = SourceImage.Bitmap;
            Bitmap markedBitmap = null;
            PatternMatchOfGrayVariationDefinition theDef = (PatternMatchOfGrayVariationDefinition)Definition();
            if (theDef.mPatternMinDownValues == null || theDef.mPatternMaxDownValues == null)
            {
                theDef.LoadPatterns(false);
            }
            Bitmap patternMinDownValues = theDef.mPatternMinDownValues;
            Bitmap patternMaxDownValues = theDef.mPatternMaxDownValues;
            Bitmap patternMinUpValues = theDef.mPatternMinUpValues;
            Bitmap patternMaxUpValues = theDef.mPatternMaxUpValues;
            Bitmap patternMinRightValues = theDef.mPatternMinRightValues;
            Bitmap patternMaxRightValues = theDef.mPatternMaxRightValues;
            Bitmap patternMinLeftValues = theDef.mPatternMinLeftValues;
            Bitmap patternMaxLeftValues = theDef.mPatternMaxLeftValues;

            if (patternMinDownValues == null || patternMaxDownValues == null)
            {
                throw new ArgumentException("Pattern to match isn't defined.");
            }

            if (mMarkedImage != null && sourceBitmap != null)
            {
                mMarkedImage.SetImage(new Bitmap(sourceBitmap));
                markedBitmap = mMarkedImage.Bitmap;
                TestExecution().LogMessageWithTimeFromTrigger("Created copy of image for markings");
            }

            score = 0;
            if (sourceBitmap != null)
            {
                // for LockBits see http://www.bobpowell.net/lockingbits.htm & http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                BitmapData sourceBitmapData = null;
                BitmapData patternMinDownValuesBitmapData = null;
                BitmapData patternMaxDownValuesBitmapData = null;
                BitmapData patternMinUpValuesBitmapData = null;
                BitmapData patternMaxUpValuesBitmapData = null;
                BitmapData patternMinRightValuesBitmapData = null;
                BitmapData patternMaxRightValuesBitmapData = null;
                BitmapData patternMinLeftValuesBitmapData = null;
                BitmapData patternMaxLeftValuesBitmapData = null;

                if (mScoreFilter != null)
                {
                    mScoreFilter.SetImageSize(mSourceImage.Bitmap.Width, mSourceImage.Bitmap.Height);
                }

                try
                {
                    sourceBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.TRAINING_PIXEL_FORMAT);
                    patternMinDownValuesBitmapData = patternMinDownValues.LockBits(new Rectangle(0, 0, patternMinDownValues.Width, patternMinDownValues.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    patternMaxDownValuesBitmapData = patternMaxDownValues.LockBits(new Rectangle(0, 0, patternMaxDownValues.Width, patternMaxDownValues.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    patternMinUpValuesBitmapData = patternMinUpValues.LockBits(new Rectangle(0, 0, patternMinUpValues.Width, patternMinUpValues.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    patternMaxUpValuesBitmapData = patternMaxUpValues.LockBits(new Rectangle(0, 0, patternMaxUpValues.Width, patternMaxUpValues.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    patternMinRightValuesBitmapData = patternMinRightValues.LockBits(new Rectangle(0, 0, patternMinRightValues.Width, patternMinRightValues.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    patternMaxRightValuesBitmapData = patternMaxRightValues.LockBits(new Rectangle(0, 0, patternMaxRightValues.Width, patternMaxRightValues.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    patternMinLeftValuesBitmapData = patternMinLeftValues.LockBits(new Rectangle(0, 0, patternMinLeftValues.Width, patternMinLeftValues.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    patternMaxLeftValuesBitmapData = patternMaxLeftValues.LockBits(new Rectangle(0, 0, patternMaxLeftValues.Width, patternMaxLeftValues.Height), ImageLockMode.ReadOnly, PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_FORMAT);
                    if (markedBitmap != null)
                    {
                        markedBitmapData = markedBitmap.LockBits(new Rectangle(0, 0, markedBitmap.Width, markedBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    }
                    sourceStride = sourceBitmapData.Stride;
                    int sourceStrideOffset = sourceStride - (sourceBitmapData.Width * PatternMatchOfGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH);

                    int patternStride = patternMinDownValuesBitmapData.Stride;
                    int patternStrideOffset = patternStride - (patternMinDownValuesBitmapData.Width * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH);

                    //Color color;
                    int grayValue;
                    int grayValue2;
                    threshhold = mVariationThreshhold.ValueAsLong();
                    sloppiness = mSloppiness.ValueAsDecimal() / 100.0;
                    minWindow = Math.Max(1, mMinWindow.ValueAsLong());
                    scoreThreshold = mScoreThreshold.ValueAsLong();
                    brightPixelFactor = mBrightPixelFactor.ValueAsDecimal();
                    darkPixelFactor = mDarkPixelFactor.ValueAsDecimal();
                    int varSum;
                    int minVarForThisPixel;
                    int maxVarForThisPixel;

                    Point currentPoint = new Point(-1, -1);
                    int lastX = -1;
                    int lastY = -1;

                    int[] variationArray = new int[PatternMatchOfGrayVariationDefinition.PixelsPerTest];
                    int positionsUntested = 0;

                    mFirstAxisScores = new long[sourceBitmap.Width, sourceBitmap.Height];

                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* sourcePointer;
                        byte* sourcePointer2;

                        byte* patternMinValuesPointer;
                        byte* patternMaxValuesPointer;

                        TestExecution().LogMessageWithTimeFromTrigger("PatternMatch " + Name + " testing Y Axis");
                        mROI.GetFirstPointOnYAxis(mSourceImage, ref currentPoint);

                        while (currentPoint.X != -1 && currentPoint.Y != -1)
                        {
                            sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                            sourcePointer += (currentPoint.Y * sourceStride) + (currentPoint.X * PatternMatchOfGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH); // adjust to current point
                            //color = Color.FromArgb(sourcePointer[3], , , ); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                            grayValue = (int)(0.3 * sourcePointer[2] + 0.59 * sourcePointer[1] + 0.11 * sourcePointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            // http://www.bobpowell.net/grayscale.htm
                            // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1

                            // check pixel above
                            sourcePointer2 = sourcePointer - sourceStride; // TODO: ensure y>0
                            grayValue2 = (int)(0.3 * sourcePointer2[2] + 0.59 * sourcePointer2[1] + 0.11 * sourcePointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            varSum = grayValue - grayValue2; // NOTE: using '=' to init varSum for this pixel

                            if (currentPoint.X != lastX || currentPoint.Y != lastY + 1)
                            {
                                // init variationArray
                                for (int i = 0; i < PatternMatchOfGrayVariationDefinition.PixelsPerTest; ++i)
                                {
                                    variationArray[i] = PatternMatchOfGrayVariationDefinition.VALUE_NOT_DEFINED;
                                }
                                if (positionsUntested > 0)
                                {
                                    // TODO: if this isn't 0, then mark untested pixels a certain color?
                                    // this should only happen when the ROI is less than PixelsPerTest high at a particular X value
                                    TestExecution().LogMessageWithTimeFromTrigger("WARNING: " + positionsUntested + " pixels were not tested above " + lastX + "," + lastY);
                                }
                                positionsUntested = 0;
                            }

                            // shift variationArray
                            for (int i = 0; i < PatternMatchOfGrayVariationDefinition.PixelsPerTest - 1; ++i)
                            {
                                variationArray[i] = variationArray[i + 1];
                            }

                            // store most recent value
                            variationArray[PatternMatchOfGrayVariationDefinition.PixelsPerTest - 1] = varSum;

                            if (variationArray[0] == PatternMatchOfGrayVariationDefinition.VALUE_NOT_DEFINED)
                            {
                                positionsUntested++;
                            }
                            else
                            {
                                int variationSum = 0;
                                // compute sum variation over X pixel transitions
                                for (int i = 0; i < PatternMatchOfGrayVariationDefinition.PixelsPerTest; ++i)
                                {
                                    variationSum += variationArray[i];
                                }

                                variationSum = Math.Max(-127, Math.Min(128, variationSum)); // make sure we stay within 1 byte (0..255)

                                // test pixel
                                patternMinValuesPointer = (byte*)patternMinDownValuesBitmapData.Scan0; // init to first byte of image
                                patternMinValuesPointer += (currentPoint.Y * patternStride) + (currentPoint.X * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                                minVarForThisPixel = patternMinValuesPointer[0] - 127;

                                patternMaxValuesPointer = (byte*)patternMaxDownValuesBitmapData.Scan0; // init to first byte of image
                                patternMaxValuesPointer += (currentPoint.Y * patternStride) + (currentPoint.X * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                                maxVarForThisPixel = patternMaxValuesPointer[0] - 127;

                                TestPixel(currentPoint.X, currentPoint.Y, variationSum, minVarForThisPixel, maxVarForThisPixel, true);

                                if (positionsUntested > 0)
                                {
                                    // if we missed testing a pixel above us (because it was near an ROI or image top edge where there weren't pixels above it to compute from), we test them here computing in the opposite direction (up values vs down values)

                                    // current pixel - PixelsPerTest = -variationSum
                                    int testPositionY = currentPoint.Y - PatternMatchOfGrayVariationDefinition.PixelsPerTest;
                                    if (testPositionY < 0) throw new ArgumentException("Fatal logic error in test 93420rf");

                                    patternMinValuesPointer = (byte*)patternMinUpValuesBitmapData.Scan0; // init to first byte of image
                                    patternMinValuesPointer += (testPositionY * patternStride) + (currentPoint.X * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                                    minVarForThisPixel = patternMinValuesPointer[0] - 127;

                                    patternMaxValuesPointer = (byte*)patternMaxUpValuesBitmapData.Scan0; // init to first byte of image
                                    patternMaxValuesPointer += (testPositionY * patternStride) + (currentPoint.X * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                                    maxVarForThisPixel = patternMaxValuesPointer[0] - 127;

                                    TestPixel(currentPoint.X, testPositionY, -variationSum, minVarForThisPixel, maxVarForThisPixel, true);
                                    positionsUntested--;
                                }
                            }

                            lastX = currentPoint.X;
                            lastY = currentPoint.Y;
                            mROI.GetNextPointOnYAxis(mSourceImage, ref currentPoint);
                        }

                        TestExecution().LogMessageWithTimeFromTrigger("PatternMatch " + Name + " testing X Axis");
                        mROI.GetFirstPointOnXAxis(mSourceImage, ref currentPoint);

                        while (currentPoint.X != -1 && currentPoint.Y != -1)
                        {
                            sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                            sourcePointer += (currentPoint.Y * sourceStride) + (currentPoint.X * PatternMatchOfGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH); // adjust to current point
                            //color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                            grayValue = (int)(0.3 * sourcePointer[2] + 0.59 * sourcePointer[1] + 0.11 * sourcePointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            // http://www.bobpowell.net/grayscale.htm
                            // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1

                            // check pixel behind
                            sourcePointer2 = sourcePointer - PatternMatchOfGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH; // TODO: ensure y>0
                            grayValue2 = (int)(0.3 * sourcePointer2[2] + 0.59 * sourcePointer2[1] + 0.11 * sourcePointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            varSum = grayValue - grayValue2; // NOTE: using '=' to init varSum for this pixel

                            if (currentPoint.Y != lastY || currentPoint.X != lastX + 1)
                            {
                                // init variationArray
                                for (int i = 0; i < PatternMatchOfGrayVariationDefinition.PixelsPerTest; ++i)
                                {
                                    variationArray[i] = PatternMatchOfGrayVariationDefinition.VALUE_NOT_DEFINED;
                                }
                                if (positionsUntested > 0)
                                {
                                    // TODO: if this isn't 0, then mark untested pixels a certain color?
                                    // this should only happen when the ROI is less than PixelsPerTest high at a particular X value
                                    TestExecution().LogMessageWithTimeFromTrigger("WARNING: " + positionsUntested + " pixels were not tested behind " + lastX + "," + lastY);
                                }
                                positionsUntested = 0;
                            }

                            // shift variationArray
                            for (int i = 0; i < PatternMatchOfGrayVariationDefinition.PixelsPerTest - 1; ++i)
                            {
                                variationArray[i] = variationArray[i + 1];
                            }

                            // store most recent value
                            variationArray[PatternMatchOfGrayVariationDefinition.PixelsPerTest - 1] = varSum;

                            if (variationArray[0] == PatternMatchOfGrayVariationDefinition.VALUE_NOT_DEFINED)
                            {
                                positionsUntested++;
                            }
                            else
                            {
                                int variationSum = 0;
                                // compute sum variation over X pixel transitions
                                for (int i = 0; i < PatternMatchOfGrayVariationDefinition.PixelsPerTest; ++i)
                                {
                                    variationSum += variationArray[i];
                                }

                                variationSum = Math.Max(-127, Math.Min(128, variationSum)); // make sure we stay within 1 byte (0..255)

                                // test pixel
                                patternMinValuesPointer = (byte*)patternMinDownValuesBitmapData.Scan0; // init to first byte of image
                                patternMinValuesPointer += (currentPoint.Y * patternStride) + (currentPoint.X * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                                minVarForThisPixel = patternMinValuesPointer[0] - 127;

                                patternMaxValuesPointer = (byte*)patternMaxDownValuesBitmapData.Scan0; // init to first byte of image
                                patternMaxValuesPointer += (currentPoint.Y * patternStride) + (currentPoint.X * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                                maxVarForThisPixel = patternMaxValuesPointer[0] - 127;

                                TestPixel(currentPoint.X, currentPoint.Y, variationSum, minVarForThisPixel, maxVarForThisPixel, false);

                                if (positionsUntested > 0)
                                {
                                    // if we missed testing a pixel behind us (because it was near an ROI or image left edge where there weren't pixels behind it to compute from), we test them here computing in the opposite direction (left values vs right values)

                                    // current pixel - PixelsPerTest = -variationSum
                                    int testPositionX = currentPoint.X - PatternMatchOfGrayVariationDefinition.PixelsPerTest;
                                    if (testPositionX < 0) throw new ArgumentException("Fatal logic error in test 93430rf");

                                    patternMinValuesPointer = (byte*)patternMinUpValuesBitmapData.Scan0; // init to first byte of image
                                    patternMinValuesPointer += (currentPoint.Y * patternStride) + (testPositionX * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                                    minVarForThisPixel = patternMinValuesPointer[0] - 127;

                                    patternMaxValuesPointer = (byte*)patternMaxUpValuesBitmapData.Scan0; // init to first byte of image
                                    patternMaxValuesPointer += (currentPoint.Y * patternStride) + (testPositionX * PatternMatchOfGrayVariationDefinition.PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                                    maxVarForThisPixel = patternMaxValuesPointer[0] - 127;

                                    TestPixel(testPositionX, currentPoint.Y, -variationSum, minVarForThisPixel, maxVarForThisPixel, false);
                                    positionsUntested--;
                                }
                            }

                            lastX = currentPoint.X;
                            lastY = currentPoint.Y;
                            mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
                        }
                    } // end unsafe block
                }
                catch (Exception e)
                {
                    TestExecution().LogMessageWithTimeFromTrigger("ERROR: Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
                }
                finally
                {
                    mFirstAxisScores = null;

                    sourceBitmap.UnlockBits(sourceBitmapData);
                    patternMinDownValues.UnlockBits(patternMinDownValuesBitmapData);
                    patternMaxDownValues.UnlockBits(patternMaxDownValuesBitmapData);
                    patternMinUpValues.UnlockBits(patternMinUpValuesBitmapData);
                    patternMaxUpValues.UnlockBits(patternMaxUpValuesBitmapData);
                    patternMinRightValues.UnlockBits(patternMinRightValuesBitmapData);
                    patternMaxRightValues.UnlockBits(patternMaxRightValuesBitmapData);
                    patternMinLeftValues.UnlockBits(patternMinLeftValuesBitmapData);
                    patternMaxLeftValues.UnlockBits(patternMaxLeftValuesBitmapData);
                    if (markedBitmap != null)
                    {
                        markedBitmap.UnlockBits(markedBitmapData);
                    }
                }
            }

            if (mMarkedImage != null && mScoreFilter.Score > 0)
            {
                mScoreFilter.MarkImage(mMarkedImage.Bitmap, Color.Red);
            }

            mResult.SetValue(score);
            mResult.SetIsComplete();
            if( mMarkedImage != null ) mMarkedImage.SetIsComplete();
            string msg = "PatternMatch " + Name + " completed; score=" + score;
            TestExecution().LogMessageWithTimeFromTrigger(msg);
            TestExecution().LogSummaryMessage(msg);

            if (score >= mAutoSaveOnScore || mScoreFilter.Score >= mAutoSaveOnCellScore)
            {
                try
                {
                    string filePath = ((PatternMatchOfGrayVariationDefinition)Definition()).AutoSavePath;
                    mSourceImage.Save(filePath, Name, true);
                    if (mMarkedImage != null) mMarkedImage.Save(filePath, Name, "_marked_" + score + "_" + mScoreFilter.Score);
                    TestExecution().LogMessageWithTimeFromTrigger("Snapshot saved");
                }
                catch (ArgumentException e)
                {
                    Project().Window().logMessage("ERROR: " + e.Message);
                    TestExecution().LogErrorWithTimeFromTrigger(e.Message);
                }
                catch (Exception e)
                {
                    Project().Window().logMessage("ERROR: Unable to AutoSave snapshot from " + Name + ".  Ensure path valid and disk not full.  Low-level message=" + e.Message);
                    TestExecution().LogErrorWithTimeFromTrigger("Unable to AutoSave snapshot from " + Name + ".  Ensure path valid and disk not full.");
                }
            }
        }

        private long[,] mFirstAxisScores;
        private void TestPixel(int testPosX, int testPosY, int valueForPixel, int minVarForThisPixel, int maxVarForThisPixel, bool firstAxis)
        {
            bool needToMark = false;
            long variation = -999;
            long scoreChange = -999;
            long avgScoreChange = -999;
            long patternWindow = maxVarForThisPixel - minVarForThisPixel; // give tight windows more weight in the score
            patternWindow = Math.Max(minWindow, patternWindow); // ensure minWindow>0 to prevent divideBy0

            if (patternWindow > threshhold)
            {
                scoreChange = 0;
                unsafe
                {
                    byte* markedPointer;
                    markedPointer = (byte*)markedBitmapData.Scan0;
                    markedPointer += (testPosY * sourceStride) + (testPosX * PatternMatchOfGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH);
                    markedPointer[3] = Color.Yellow.A;
                    markedPointer[2] = Color.Yellow.R;
                    markedPointer[1] = Color.Yellow.G;
                    markedPointer[0] = Color.Yellow.B;
                }
            }
            else
            {
                if (valueForPixel < minVarForThisPixel - sloppiness * patternWindow)
                {
                    variation = minVarForThisPixel - valueForPixel;
                    //scoreChange = (long)(((variation / patternWindow) + 1) * darkPixelFactor);
                    scoreChange = (long)(variation * ((variation / (patternWindow / 2)) + 1) * darkPixelFactor);
                    if (scoreChange >= scoreThreshold)
                    {
                        if (firstAxis)
                        {
                            mFirstAxisScores[testPosX, testPosY] = scoreChange;
                        }
                        else
                        {
                            if (mFirstAxisScores[testPosX, testPosY] >= scoreThreshold)
                            {
                                avgScoreChange = (scoreChange + mFirstAxisScores[testPosX, testPosY]) / 2;
                                score += avgScoreChange;
                                needToMark = true;
                                if (mDebugDetailsCount <= mMaxDebugDetails)
                                {
                                    // TODO: log message as warning if only 1 axis passes threshhold...only if Deep Analysis is on?
                                    TestExecution().LogMessage("Pattern Match score event: " + testPosX + "," + testPosY + "  dark spot score=" + avgScoreChange + "(" + mFirstAxisScores[testPosX, testPosY]  + "," + scoreChange + ")  var=" + valueForPixel + "  min=" + minVarForThisPixel + "  max=" + maxVarForThisPixel + "  window=" + patternWindow + "  var=" + variation);
                                    mDebugDetailsCount++;
                                }
                                if (mScoreFilter != null) mScoreFilter.ProcessScore(testPosX, testPosY, avgScoreChange);
                            }
                        }
                        /*                        if (!firstAxis) HACK FOR TESTING 1 AXIS AT A TIME
                                                {
                                                    score += scoreChange;
                                                    needToMark = true;
                                                    if (mDebugDetailsCount <= mMaxDebugDetails)
                                                    {
                                                        // TODO: log message as warning if only 1 axis passes threshhold...only if Deep Analysis is on?
                                                        TestExecution().LogMessage("Pattern Match score event: " + testPosX + "," + testPosY + "  dark spot score=" + scoreChange + "(" + mFirstAxisScores[testPosX, testPosY] + "," + scoreChange + ")  var=" + valueForPixel + "  min=" + minVarForThisPixel + "  max=" + maxVarForThisPixel + "  window=" + patternWindow + "  var=" + variation);
                                                        mDebugDetailsCount++;
                                                    }
                                                    if (mScoreFilter != null) mScoreFilter.ProcessScore(testPosX, testPosY, scoreChange);
                                                }*/
                    }
                }
                else if (valueForPixel > maxVarForThisPixel + sloppiness * patternWindow)
                {
                    variation = valueForPixel - maxVarForThisPixel;
                    //scoreChange = (long)(((variation / patternWindow) + 1) * brightPixelFactor);
                    scoreChange = (long)(variation * ((variation / (patternWindow / 2)) + 1) * brightPixelFactor);
                    if (scoreChange >= scoreThreshold)
                    {
                        if (firstAxis)
                        {
                            mFirstAxisScores[testPosX, testPosY] = scoreChange;
                        }
                        else
                        {
                            if (mFirstAxisScores[testPosX, testPosY] >= scoreThreshold)
                            {
                                avgScoreChange = (scoreChange + mFirstAxisScores[testPosX, testPosY]) / 2;
                                score += avgScoreChange;
                                needToMark = true;
                                if (mDebugDetailsCount <= mMaxDebugDetails)
                                {
                                    TestExecution().LogMessage("Pattern Match score event: " + testPosX + "," + testPosY + "  bright spot score=" + avgScoreChange + "(" + mFirstAxisScores[testPosX, testPosY] + "," + scoreChange + ")  var=" + valueForPixel + "  min=" + minVarForThisPixel + "  max=" + maxVarForThisPixel + "  window=" + patternWindow + "  var=" + variation);
                                    mDebugDetailsCount++;
                                }
                                if (mScoreFilter != null) mScoreFilter.ProcessScore(testPosX, testPosY, avgScoreChange);
                            }
                        }
                        /*                        if (!firstAxis) HACK FOR TESTING 1 AXIS AT A TIME
                        {
                            score += scoreChange;
                            needToMark = true;
                            if (mDebugDetailsCount <= mMaxDebugDetails)
                            {
                                // TODO: log message as warning if only 1 axis passes threshhold...only if Deep Analysis is on?
                                TestExecution().LogMessage("Pattern Match score event: " + testPosX + "," + testPosY + "  dark spot score=" + scoreChange + "(" + mFirstAxisScores[testPosX, testPosY] + "," + scoreChange + ")  var=" + valueForPixel + "  min=" + minVarForThisPixel + "  max=" + maxVarForThisPixel + "  window=" + patternWindow + "  var=" + variation);
                                mDebugDetailsCount++;
                            }
                            if (mScoreFilter != null) mScoreFilter.ProcessScore(testPosX, testPosY, scoreChange);
                        }*/
                    }
                }
                else
                {
                    variation = 0;
                    scoreChange = 0;
                    needToMark = false;
                }
                if (needToMark && mMarkedImage != null)
                {
                    unsafe
                    {
                        byte* markedPointer;
                        markedPointer = (byte*)markedBitmapData.Scan0;
                        markedPointer += (testPosY * sourceStride) + (testPosX * PatternMatchOfGrayVariationDefinition.TRAINING_PIXEL_BYTE_WIDTH);
                        markedPointer[3] = mMarkColor.A;
                        markedPointer[2] = mMarkColor.R;
                        markedPointer[1] = mMarkColor.G;
                        markedPointer[0] = mMarkColor.B;
                    }
                }
            }

            if (mDeepAnalysisEnabled && testPosX >= mDeepAnalysisLeft && testPosX <= mDeepAnalysisRight && testPosY >= mDeepAnalysisTop && testPosY <= mDeepAnalysisBottom)
            {
                string message = "DEEP ANALYSIS: '" + Name + "' " + testPosX + "," + testPosY + " ";
                if (patternWindow > threshhold) message += "PATTERN WINDOW > THRESHOLD;";
                message += "  score change=" + scoreChange
                    + "  axis=" + (firstAxis ? 1 : 2)
                    + "  var=" + valueForPixel
                    + "  min=" + minVarForThisPixel
                    + "  max=" + maxVarForThisPixel
                    + "  window=" + patternWindow
                    + "  slop=" + (sloppiness * patternWindow)
                    + "  marked=" + needToMark
                    + "  threshold=" + scoreThreshold
                    ;

                TestExecution().LogMessage(message);

                if (maxVarForThisPixel < minVarForThisPixel)
                {
                    TestExecution().LogMessage("WARNING: min > max fklsd");
                }
            }
        }
    }
}

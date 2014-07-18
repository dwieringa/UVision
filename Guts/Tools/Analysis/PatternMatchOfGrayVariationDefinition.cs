// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace NetCams
{
    public class PatternMatchOfGrayVariationDefinition : NetCams.ImageScorerDefinition
    {
        public PatternMatchOfGrayVariationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mResult = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "Result"));
            mResult.Type = DataType.IntegerNumber;
            mResult.AddDependency(this);
            mResult.Name = "DifferenceScore";

            //TODO: ConstantImage to hold "average" image of pattern...along with red pixels to show what won't be matched (diff range outside of threshold)

            mMarkedImage = new GeneratedImageDefinition(testSequence, OwnerLink.newLink(this, "MarkedImage"));
            mMarkedImage.AddDependency(this);
            mMarkedImage.Name = "IntensityVariationImage";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
			new PatternMatchOfGrayVariationInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mROI != null && mROI.IsDependentOn(theOtherObject)) return true;
            if (mVariationThreshhold != null && mVariationThreshhold.IsDependentOn(theOtherObject)) return true;
            if (mSloppiness != null && mSloppiness.IsDependentOn(theOtherObject)) return true;
            if (mScoreThreshold != null && mScoreThreshold.IsDependentOn(theOtherObject)) return true;
            if (mBrightPixelFactor != null && mBrightPixelFactor.IsDependentOn(theOtherObject)) return true;
            if (mDarkPixelFactor != null && mDarkPixelFactor.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mROI != null) result = Math.Max(result, mROI.ToolMapRow);
                if (mVariationThreshhold != null) result = Math.Max(result, mVariationThreshhold.ToolMapRow);
                if (mSloppiness != null) result = Math.Max(result, mSloppiness.ToolMapRow);
                if (mScoreThreshold != null) result = Math.Max(result, mScoreThreshold.ToolMapRow);
                if (mBrightPixelFactor != null) result = Math.Max(result, mBrightPixelFactor.ToolMapRow);
                if (mDarkPixelFactor != null) result = Math.Max(result, mDarkPixelFactor.ToolMapRow);
                return result + 1;
			}
		}

        private String mLearningPath = "C:\\Runtime Projects\\Vision\\Learning\\";
        [CategoryAttribute("Learning"),
        DescriptionAttribute("The path to the folder containing the images to learn from.")]
        public String LearningPath
        {
            get { return mLearningPath; }
            set
            {
                HandlePropertyChange(this, "LearningPath", mLearningPath, value);
                mLearningPath = value;
                LoadPatterns(true);
            }
        }

        private bool mCollectImages = false;
        [CategoryAttribute("Learning"),
        DescriptionAttribute("")]
        public bool CollectImages
        {
            get { return mCollectImages; }
            set 
            {
                HandlePropertyChange(this, "CollectImages", mCollectImages, value);
                mCollectImages = value;
            }
        }

        private DataValueDefinition mVariationThreshhold;
        [CategoryAttribute("Learning"),
        DescriptionAttribute("If the difference between the min and max values of the learned images exceeds this value, that pixel is ignored (\"turned off\").")]
        public DataValueDefinition VariationThreshhold
        {
            get { return mVariationThreshhold; }
            set 
            {
                HandlePropertyChange(this, "VariationThreshhold", mVariationThreshhold, value);
                mVariationThreshhold = value;
            }
        }

        [CategoryAttribute("Learning"),
        DescriptionAttribute("Set this to true to re-learn the acceptable pattern from the images in the LearningPath.")]
        public bool RelearnNow
        {
            get { return false; }
            set
            {
                if (value == true)
                {
                    TrainPatterns();
                }
            }
        }

        private DataValueDefinition mSloppiness;
        [CategoryAttribute("Testing"),
        DescriptionAttribute("A percentage indicating how far a pixel can be outside the min/max values before it is scored as a flaw.")]
        public DataValueDefinition Sloppiness
        {
            get { return mSloppiness; }
            set
            {
                HandlePropertyChange(this, "Sloppiness", mSloppiness, value);
                mSloppiness = value;
            }
        }

        private DataValueDefinition mMinWindow;
        [CategoryAttribute("Testing"),
        DescriptionAttribute("Used with Sloppiness...")]
        public DataValueDefinition MinWindow
        {
            get { return mMinWindow; }
            set 
            {
                HandlePropertyChange(this, "MinWindow", mMinWindow, value);
                mMinWindow = value;
            }
        }

        private DataValueDefinition mScoreThreshold;
        [CategoryAttribute("Scoring"),
        DescriptionAttribute("Scores below this value are ignored.")]
        public DataValueDefinition ScoreThreshold
        {
            get { return mScoreThreshold; }
            set
            {
                HandlePropertyChange(this, "ScoreThreshold", mScoreThreshold, value);
                mScoreThreshold = value;
            }
        }

        private DataValueDefinition mBrightPixelFactor;
        [CategoryAttribute("Scoring"),
        DescriptionAttribute("A multiplier to alter the score of bright pixels")]
        public DataValueDefinition BrightPixelFactor
        {
            get { return mBrightPixelFactor; }
            set
            {
                HandlePropertyChange(this, "BrightPixelFactor", mBrightPixelFactor, value);
                mBrightPixelFactor = value;
            }
        }

        private DataValueDefinition mDarkPixelFactor;
        [CategoryAttribute("Scoring"),
        DescriptionAttribute("A multiplier to alter the score of dark pixels")]
        public DataValueDefinition DarkPixelFactor
        {
            get { return mDarkPixelFactor; }
            set 
            {
                HandlePropertyChange(this, "DarkPixelFactor", mDarkPixelFactor, value);
                mDarkPixelFactor = value;
            }
        }

        private ROIDefinition mROI;
        [CategoryAttribute("Testing"),
        DescriptionAttribute("ROI to test over")]
        public ROIDefinition ROI
        {
            get { return mROI; }
            set
            {
                HandlePropertyChange(this, "ROI", mROI, value);
                mROI = value;
            }
        }

        public Bitmap mPatternMinDownValues;
        public Bitmap mPatternMaxDownValues;
        public Bitmap mPatternMinUpValues;
        public Bitmap mPatternMaxUpValues;

        public Bitmap mPatternMinRightValues;
        public Bitmap mPatternMaxRightValues;
        public Bitmap mPatternMinLeftValues;
        public Bitmap mPatternMaxLeftValues;

		private ImageDefinition mSourceImage = null;
		[CategoryAttribute("Input")]
		public ImageDefinition SourceImage
		{
			get { return mSourceImage; }
            set
            {
                HandlePropertyChange(this, "SourceImage", mSourceImage, value);
                mSourceImage = value;
            }
        }

        private GeneratedValueDefinition mResult = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition Result
        {
            get { return mResult; }
        }

        /* TODO: can't reference the ScoreFilter from here as a property since it will be dependent on us...so it will get created AFTER us (same with Instance objects).  So need to make object creation/naming separate from setting properties...both for Def and Instance objects
        private ScoreFilterDefinition mScoreFilter = null;
        [CategoryAttribute("Output")]
        public ScoreFilterDefinition ScoreFilter
        {
            get { return mScoreFilter; }
            set
            {
                if (value != mScoreFilter) // we're making a change
                {
                    if (mScoreFilter != null) // let old filter know that it is no longer dependent upon us
                    {
                        mScoreFilter.RemoveDependency(this);
                    }
                    mScoreFilter = value;
                    if (mScoreFilter != null) // let new filter know that it is now dependent upon us
                    {
                        mScoreFilter.AddDependency(this);
                    }
                }
            }
        }
        */

        private int mMaxDebugDetails = 100;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("The maximum number of detail entries that will be placed in the test execution log.  Excessive entries can slow down test performance.")]
        public int MaxDebugDetails
        {
            get { return mMaxDebugDetails; }
            set 
            { 
                HandlePropertyChange(this, "MaxDebugDetails", mMaxDebugDetails, value);
                mMaxDebugDetails = value;
            }
        }

        private bool mCreateMarkedImage = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Set to True to create a marked up image")]
        public bool CreateMarkedImage
        {
            get { return mCreateMarkedImage; }
            set
            {
                HandlePropertyChange(this, "CreateMarkedImage", mCreateMarkedImage, value);
                mCreateMarkedImage = value;
            }
        }

        private Color mMarkColor = Color.Fuchsia;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("The color which is used to highlight areas that match the color definition")]
        public Color MarkColor
        {
            get { return mMarkColor; }
            set
            {
                HandlePropertyChange(this, "MarkColor", mMarkColor, value);
                mMarkColor = value;
            }
        }

        private bool mDeepAnalysisEnabled;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public bool DeepAnalysisEnabled
        {
            get { return mDeepAnalysisEnabled; }
            set
            {
                HandlePropertyChange(this, "DeepAnalysisEnabled", mDeepAnalysisEnabled, value);
                mDeepAnalysisEnabled = value;
            }
        }

        private int mDeepAnalysisTop;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int DeepAnalysisTop
        {
            get { return mDeepAnalysisTop; }
            set
            {
                HandlePropertyChange(this, "DeepAnalysisTop", mDeepAnalysisTop, value);
                mDeepAnalysisTop = value;
            }
        }

        private int mDeepAnalysisBottom;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int DeepAnalysisBottom
        {
            get { return mDeepAnalysisBottom; }
            set
            {
                HandlePropertyChange(this, "DeepAnalysisBottom", mDeepAnalysisBottom, value);
                mDeepAnalysisBottom = value;
            }
        }

        private int mDeepAnalysisLeft;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int DeepAnalysisLeft
        {
            get { return mDeepAnalysisLeft; }
            set 
            {
                HandlePropertyChange(this, "DeepAnalysisLeft", mDeepAnalysisLeft, value);
                mDeepAnalysisLeft = value;
            }
        }

        private int mDeepAnalysisRight;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int DeepAnalysisRight
        {
            get { return mDeepAnalysisRight; }
            set
            {
                HandlePropertyChange(this, "DeepAnalysisRight", mDeepAnalysisRight, value);
                mDeepAnalysisRight = value; 
            }
        }

        private String mAutoSavePath = "Debug\\<TESTSEQ>\\";
        [CategoryAttribute("Debug Options")]
        public String AutoSavePath
        {
            get { return mAutoSavePath; }
            set 
            {
                HandlePropertyChange(this, "AutoSavePath", mAutoSavePath, value);
                mAutoSavePath = value;
            }
        }

        private int mAutoSaveOnScore = 9999999;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int AutoSaveOnScore
        {
            get { return mAutoSaveOnScore; }
            set 
            {
                HandlePropertyChange(this, "AutoSaveOnScore", mAutoSaveOnScore, value);
                mAutoSaveOnScore = value;
            }
        }

        private int mAutoSaveOnCellScore = 9999999;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public int AutoSaveOnCellScore
        {
            get { return mAutoSaveOnCellScore; }
            set
            {
                HandlePropertyChange(this, "AutoSaveOnCellScore", mAutoSaveOnCellScore, value);
                mAutoSaveOnCellScore = value;
            }
        }

        private GeneratedImageDefinition mMarkedImage = null;
        [CategoryAttribute("Output")]
        public GeneratedImageDefinition MarkedImage
        {
            get { return mMarkedImage; }
        }

        public void LoadPatterns(bool ignoreErrors)
        {
            if (mPatternMinDownValues != null)
            {
                mPatternMinDownValues.Dispose();
                mPatternMinDownValues = null;
            }
            if (mPatternMaxDownValues != null)
            {
                mPatternMaxDownValues.Dispose();
                mPatternMaxDownValues = null;
            }

            if (mPatternMinUpValues != null)
            {
                mPatternMinUpValues.Dispose();
                mPatternMinUpValues = null;
            }
            if (mPatternMaxUpValues != null)
            {
                mPatternMaxUpValues.Dispose();
                mPatternMaxUpValues = null;
            }

            if (mPatternMinRightValues != null)
            {
                mPatternMinRightValues.Dispose();
                mPatternMinRightValues = null;
            }
            if (mPatternMaxRightValues != null)
            {
                mPatternMaxRightValues.Dispose();
                mPatternMaxRightValues = null;
            }

            if (mPatternMinLeftValues != null)
            {
                mPatternMinLeftValues.Dispose();
                mPatternMinLeftValues = null;
            }
            if (mPatternMaxLeftValues != null)
            {
                mPatternMaxLeftValues.Dispose();
                mPatternMaxLeftValues = null;
            }

            string patternDirPath = PatternPath;
            try
            {
                mPatternMinDownValues = new Bitmap(patternDirPath + "\\MinDownValues.bmp");
                mPatternMaxDownValues = new Bitmap(patternDirPath + "\\MaxDownValues.bmp");
                mPatternMinUpValues = new Bitmap(patternDirPath + "\\MinUpValues.bmp");
                mPatternMaxUpValues = new Bitmap(patternDirPath + "\\MaxUpValues.bmp");
                mPatternMinRightValues = new Bitmap(patternDirPath + "\\MinRightValues.bmp");
                mPatternMaxRightValues = new Bitmap(patternDirPath + "\\MaxRightValues.bmp");
                mPatternMinLeftValues = new Bitmap(patternDirPath + "\\MinLeftValues.bmp");
                mPatternMaxLeftValues = new Bitmap(patternDirPath + "\\MaxLeftValues.bmp");
            }
            catch (Exception e)
            {
                if (!ignoreErrors)
                {
                    throw new ArgumentException("Unable to load learned pattern from disk. Check path settings and consider Relearning.");
                }
            }
        }
        [CategoryAttribute("Learning"),
        DescriptionAttribute("The path to the folder containing the patterns resulting from the training process.")]
        public string PatternPath
        {
            get
            {
                string patternDirPath = mLearningPath;
                if (!patternDirPath.EndsWith("\\")) patternDirPath += "\\";
                patternDirPath += "Learned";
                return patternDirPath;
            }
        }

        private bool mSpreadEdgesEnabled = true;
        [CategoryAttribute("Learning"),
        DescriptionAttribute("")]
        public bool SpreadEdgesEnabled
        {
            get { return mSpreadEdgesEnabled; }
            set 
            {
                HandlePropertyChange(this, "SpreadEdgesEnabled", mSpreadEdgesEnabled, value);
                mSpreadEdgesEnabled = value;
            }
        }

        private int mSpreadEdgesThreshhold = 50;
        [CategoryAttribute("Learning"),
        DescriptionAttribute("If variation exceeds this value (either positively or negatively), then this value will be spread to surrounding pixels...since the edges are always in exactly the same location.")]
        public int SpreadEdgesThreshold
        {
            get { return mSpreadEdgesThreshhold; }
            set
            {
                HandlePropertyChange(this, "SpreadEdgesThreshold", mSpreadEdgesThreshhold, value);
                mSpreadEdgesThreshhold = value; 
            }
        }

        public static readonly int VALUE_NOT_DEFINED = 9999;
        public static readonly int PixelsPerTest = 6;
        public static readonly int TRAINING_PIXEL_BYTE_WIDTH = 4; // determined by PixelFormat.Format32bppArgb; http://www.bobpowell.net/lockingbits.htm
        public static readonly int PATTERN_PIXEL_BYTE_WIDTH = 1; // determined by PixelFormat.Format8BppIndexed; http://www.bobpowell.net/lockingbits.htm
        public static readonly PixelFormat TRAINING_PIXEL_FORMAT = PixelFormat.Format32bppArgb;
        public static readonly PixelFormat PATTERN_PIXEL_FORMAT = PixelFormat.Format8bppIndexed;
        public void TrainPatterns()
        {
            if (mPatternMinDownValues != null)
            {
                mPatternMinDownValues.Dispose();
                mPatternMinDownValues = null;
            }
            if (mPatternMaxDownValues != null)
            {
                mPatternMaxDownValues.Dispose();
                mPatternMaxDownValues = null;
            }
            if (mPatternMinUpValues != null)
            {
                mPatternMinUpValues.Dispose();
                mPatternMinUpValues = null;
            }
            if (mPatternMaxUpValues != null)
            {
                mPatternMaxUpValues.Dispose();
                mPatternMaxUpValues = null;
            }

            if (mPatternMinRightValues != null)
            {
                mPatternMinRightValues.Dispose();
                mPatternMinRightValues = null;
            }
            if (mPatternMaxRightValues != null)
            {
                mPatternMaxRightValues.Dispose();
                mPatternMaxRightValues = null;
            }
            if (mPatternMinLeftValues != null)
            {
                mPatternMinLeftValues.Dispose();
                mPatternMinLeftValues = null;
            }
            if (mPatternMaxLeftValues != null)
            {
                mPatternMaxLeftValues.Dispose();
                mPatternMaxLeftValues = null;
            }

            // for LockBits see http://www.bobpowell.net/lockingbits.htm & http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
            BitmapData trainingBitmapData = null;
            BitmapData patternMinDownValuesBitmapData = null;
            BitmapData patternMaxDownValuesBitmapData = null;
            BitmapData patternMinUpValuesBitmapData = null;
            BitmapData patternMaxUpValuesBitmapData = null;
            BitmapData patternMinRightValuesBitmapData = null;
            BitmapData patternMaxRightValuesBitmapData = null;
            BitmapData patternMinLeftValuesBitmapData = null;
            BitmapData patternMaxLeftValuesBitmapData = null;
            Bitmap trainingBitmap;
            bool patternsInitialized = false;
            DirectoryInfo di = new DirectoryInfo(mLearningPath);
            FileInfo[] rgFiles = di.GetFiles("*.jpg");

            if (rgFiles.Length == 0)
            {
                throw new ArgumentException("No images found to learn from.");
            }

            string patternDirPath = PatternPath;
            DirectoryInfo patternDir = new DirectoryInfo(patternDirPath);
            if (!patternDir.Exists) patternDir.Create();

            int fileIndex = 0;
            foreach (System.IO.FileInfo fileInfo in rgFiles)
            {
                try
                {
                    trainingBitmap = new Bitmap(mLearningPath + "\\" + fileInfo.Name);
                }
                catch(Exception e)
                {
                    throw new ArgumentException("Unable to load training image from disk.  File path = " + mLearningPath + "\\" + fileInfo.Name);
                }
                if (mPatternMinDownValues == null || mPatternMaxDownValues == null || mPatternMinUpValues == null || mPatternMaxUpValues == null || mPatternMinRightValues == null || mPatternMaxRightValues == null || mPatternMinLeftValues == null || mPatternMaxLeftValues == null)
                {
                    mPatternMinDownValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    mPatternMaxDownValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    mPatternMinUpValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    mPatternMaxUpValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    mPatternMinRightValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    mPatternMaxRightValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    mPatternMinLeftValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    mPatternMaxLeftValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    ColorPalette monoPalette = mPatternMinDownValues.Palette;
                    Color[] entries = monoPalette.Entries;
                    for (int i = 0; i < 256; i++) entries[i] = Color.FromArgb(i, i, i);
                    mPatternMinDownValues.Palette = monoPalette;
                    mPatternMaxDownValues.Palette = monoPalette;
                    mPatternMinUpValues.Palette = monoPalette;
                    mPatternMaxUpValues.Palette = monoPalette;
                    mPatternMinRightValues.Palette = monoPalette;
                    mPatternMaxRightValues.Palette = monoPalette;
                    mPatternMinLeftValues.Palette = monoPalette;
                    mPatternMaxLeftValues.Palette = monoPalette;
                    patternsInitialized = false;
                }

                if (trainingBitmap.Width != mPatternMinDownValues.Width || trainingBitmap.Height != mPatternMinDownValues.Height)
                {
                    throw new ArgumentException("Learning images are not all the same size.");
                }

                try
                {
                    trainingBitmapData = trainingBitmap.LockBits(new Rectangle(0, 0, trainingBitmap.Width, trainingBitmap.Height), ImageLockMode.ReadOnly, TRAINING_PIXEL_FORMAT);
                    int trainingStride = trainingBitmapData.Stride;
                    int trainingStrideOffset = trainingStride - (trainingBitmapData.Width * TRAINING_PIXEL_BYTE_WIDTH);

                    patternMinDownValuesBitmapData = mPatternMinDownValues.LockBits(new Rectangle(0, 0, mPatternMinDownValues.Width, mPatternMinDownValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                    patternMaxDownValuesBitmapData = mPatternMaxDownValues.LockBits(new Rectangle(0, 0, mPatternMaxDownValues.Width, mPatternMaxDownValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                    patternMinUpValuesBitmapData = mPatternMinUpValues.LockBits(new Rectangle(0, 0, mPatternMinUpValues.Width, mPatternMinUpValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                    patternMaxUpValuesBitmapData = mPatternMaxUpValues.LockBits(new Rectangle(0, 0, mPatternMaxUpValues.Width, mPatternMaxUpValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);

                    patternMinRightValuesBitmapData = mPatternMinRightValues.LockBits(new Rectangle(0, 0, mPatternMinRightValues.Width, mPatternMinRightValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                    patternMaxRightValuesBitmapData = mPatternMaxRightValues.LockBits(new Rectangle(0, 0, mPatternMaxRightValues.Width, mPatternMaxRightValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                    patternMinLeftValuesBitmapData = mPatternMinLeftValues.LockBits(new Rectangle(0, 0, mPatternMinLeftValues.Width, mPatternMinLeftValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                    patternMaxLeftValuesBitmapData = mPatternMaxLeftValues.LockBits(new Rectangle(0, 0, mPatternMaxLeftValues.Width, mPatternMaxLeftValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);

                    int patternStride = patternMinDownValuesBitmapData.Stride;
                    int patternStrideOffset = patternStride - (patternMinDownValuesBitmapData.Width * PATTERN_PIXEL_BYTE_WIDTH);

                    if (patternStride != patternMaxDownValuesBitmapData.Stride)
                    {
                        throw new ArgumentException("oops");
                    }
                    Color color;
                    int grayValue;
                    int grayValue2;
                    int variationTEST = 0;
                    int variation_byteSized = 0;
                    int upVariation_byteSized = 0;
                    int[] variationArray = new int[PixelsPerTest];

                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* trainingPointer;
                        byte* trainingPointer2;

                        byte* patternMinDownValuesPointer;
                        byte* patternMaxDownValuesPointer;
                        byte* patternMinUpValuesPointer;
                        byte* patternMaxUpValuesPointer;

                        for (int x = 1; x < trainingBitmap.Width - 1; x++) // starting 1 pixel late and stopping 1 pixel early since we look at surrounding pixels.  we don't normally need to test edge of image anyway
                        {
                            // init variationArray
                            for (int i = 0; i < PixelsPerTest; ++i)
                            {
                                variationArray[i] = VALUE_NOT_DEFINED;
                            }

                            for (int y = 1; y < trainingBitmap.Height - 1; y++)
                            {
                                // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                trainingPointer = (byte*)trainingBitmapData.Scan0; // init to first byte of image
                                trainingPointer += (y * trainingStride) + (x * TRAINING_PIXEL_BYTE_WIDTH); // adjust to current point
                                grayValue = (int)(0.3 * trainingPointer[2] + 0.59 * trainingPointer[1] + 0.11 * trainingPointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                // http://www.bobpowell.net/grayscale.htm
                                // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1

                                // check pixel above
                                trainingPointer2 = trainingPointer - trainingStride;
                                grayValue2 = (int)(0.3 * trainingPointer2[2] + 0.59 * trainingPointer2[1] + 0.11 * trainingPointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).

                                variationTEST = grayValue - grayValue2;

                                // shift variationArray
                                for (int i = 0; i < PixelsPerTest - 1; ++i)
                                {
                                    variationArray[i] = variationArray[i + 1];
                                }

                                // store most recent value
                                variationArray[PixelsPerTest - 1] = variationTEST;

                                if (variationArray[0] != VALUE_NOT_DEFINED)
                                {
                                    // compute sum variation over X pixel transitions
                                    int variationSum = 0;
                                    int upVariationSum = 0;
                                    for (int i = 0; i < PixelsPerTest; ++i)
                                    {
                                        variationSum += variationArray[i];
                                    }

                                    variationSum = Math.Max(-127, Math.Min(128, variationSum)); // make sure we stay within 1 byte (0..255)
                                    upVariationSum = -variationSum;

                                    variation_byteSized = variationSum + 127; // shift the values up so that no change = 127; biggest brightness increase = 255; biggest darkness increase = 0
                                    upVariation_byteSized = upVariationSum + 127;

                                    variation_byteSized = Math.Max(0, Math.Min(255, variation_byteSized)); // make sure we stay within 1 byte (0..255)
                                    upVariation_byteSized = Math.Max(0, Math.Min(255, upVariation_byteSized)); // make sure we stay within 1 byte (0..255)


                                    // UPDATE "DOWN" VALUES...
                                    patternMinDownValuesPointer = (byte*)patternMinDownValuesBitmapData.Scan0; // init to first byte of image
                                    patternMinDownValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                    patternMaxDownValuesPointer = (byte*)patternMaxDownValuesBitmapData.Scan0; // init to first byte of image
                                    patternMaxDownValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                    if (variation_byteSized < patternMinDownValuesPointer[0] || !patternsInitialized)
                                    {
                                        patternMinDownValuesPointer[0] = (byte)variation_byteSized; // gray value
                                    }

                                    if (variation_byteSized > patternMaxDownValuesPointer[0] || !patternsInitialized)
                                    {
                                        patternMaxDownValuesPointer[0] = (byte)variation_byteSized; // gray value
                                    }

                                    if (mSpreadEdgesEnabled)
                                    {
                                        if (variation_byteSized < -mSpreadEdgesThreshhold)
                                        {
                                            //Project().Window().logMessage(Name + " spreading sharp edge of " + variation + " at " + x + "," + y);
                                            // pixel above
                                            SpreadMinValueTo(x, y - 1, variation_byteSized, patternMinDownValuesBitmapData);

                                            // pixel below
                                            SpreadMinValueTo(x, y + 1, variation_byteSized, patternMinDownValuesBitmapData);

                                            // pixel to left
                                            SpreadMinValueTo(x - 1, y, variation_byteSized, patternMinDownValuesBitmapData);

                                            // pixel to right
                                            SpreadMinValueTo(x + 1, y, variation_byteSized, patternMinDownValuesBitmapData);
                                        }
                                        else if (variation_byteSized > mSpreadEdgesThreshhold)
                                        {
                                            //Project().Window().logMessage(Name + " spreading sharp edge of " + variation + " at " + x + "," + y);
                                            // pixel above
                                            SpreadMaxValueTo(x, y - 1, variation_byteSized, patternMaxDownValuesBitmapData);

                                            // pixel below
                                            SpreadMaxValueTo(x, y + 1, variation_byteSized, patternMaxDownValuesBitmapData);

                                            // pixel to left
                                            SpreadMaxValueTo(x - 1, y, variation_byteSized, patternMaxDownValuesBitmapData);

                                            // pixel to right
                                            SpreadMaxValueTo(x + 1, y, variation_byteSized, patternMaxDownValuesBitmapData);
                                        }
                                    }

                                    // UPDATE "UP" VALUES...
                                    int testPositionY = y - PixelsPerTest;
                                    if (testPositionY < 0) throw new ArgumentException("askdjakjlhsdkjhas");
                                    patternMinUpValuesPointer = (byte*)patternMinUpValuesBitmapData.Scan0; // init to first byte of image
                                    patternMinUpValuesPointer += (testPositionY * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                    patternMaxUpValuesPointer = (byte*)patternMaxUpValuesBitmapData.Scan0; // init to first byte of image
                                    patternMaxUpValuesPointer += (testPositionY * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                    if (upVariation_byteSized < patternMinUpValuesPointer[0] || !patternsInitialized)
                                    {
                                        patternMinUpValuesPointer[0] = (byte)upVariation_byteSized; // gray value
                                    }

                                    if (upVariation_byteSized > patternMaxUpValuesPointer[0] || !patternsInitialized)
                                    {
                                        patternMaxUpValuesPointer[0] = (byte)upVariation_byteSized; // gray value
                                    }

                                    if (patternMaxUpValuesPointer[0] < patternMinUpValuesPointer[0])
                                    {
                                        throw new ArgumentException("asldkjaslkhuweeu");
                                    }

                                    if (mSpreadEdgesEnabled)
                                    {
                                        if (upVariation_byteSized < -mSpreadEdgesThreshhold)
                                        {
                                            //Project().Window().logMessage(Name + " spreading sharp edge of " + variation + " at " + x + "," + y);
                                            // pixel above
                                            SpreadMinValueTo(x, testPositionY - 1, upVariation_byteSized, patternMinUpValuesBitmapData);

                                            // pixel below
                                            SpreadMinValueTo(x, testPositionY + 1, upVariation_byteSized, patternMinUpValuesBitmapData);

                                            // pixel to left
                                            SpreadMinValueTo(x - 1, testPositionY, upVariation_byteSized, patternMinUpValuesBitmapData);

                                            // pixel to right
                                            SpreadMinValueTo(x + 1, testPositionY, upVariation_byteSized, patternMinUpValuesBitmapData);
                                        }
                                        else if (upVariation_byteSized > mSpreadEdgesThreshhold)
                                        {
                                            //Project().Window().logMessage(Name + " spreading sharp edge of " + variation + " at " + x + "," + y);
                                            // pixel above
                                            SpreadMaxValueTo(x, testPositionY - 1, upVariation_byteSized, patternMaxUpValuesBitmapData);

                                            // pixel below
                                            SpreadMaxValueTo(x, testPositionY + 1, upVariation_byteSized, patternMaxUpValuesBitmapData);

                                            // pixel to left
                                            SpreadMaxValueTo(x - 1, testPositionY, upVariation_byteSized, patternMaxUpValuesBitmapData);

                                            // pixel to right
                                            SpreadMaxValueTo(x + 1, testPositionY, upVariation_byteSized, patternMaxUpValuesBitmapData);
                                        }
                                    }
                                }
                            } // end for y
                        } // end for x

                        byte* patternMinRightValuesPointer;
                        byte* patternMaxRightValuesPointer;
                        byte* patternMinLeftValuesPointer;
                        byte* patternMaxLeftValuesPointer;

                        for (int y = 1; y < trainingBitmap.Height - 1; y++)
                        {
                            // init variationArray
                            for (int i = 0; i < PixelsPerTest; ++i)
                            {
                                variationArray[i] = VALUE_NOT_DEFINED;
                            }

                            for (int x = 1; x < trainingBitmap.Width - 1; x++) // starting 1 pixel late and stopping 1 pixel early since we look at surrounding pixels.  we don't normally need to test edge of image anyway
                            {
                                // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha
                                trainingPointer = (byte*)trainingBitmapData.Scan0; // init to first byte of image
                                trainingPointer += (y * trainingStride) + (x * TRAINING_PIXEL_BYTE_WIDTH); // adjust to current point
                                grayValue = (int)(0.3 * trainingPointer[2] + 0.59 * trainingPointer[1] + 0.11 * trainingPointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                // http://www.bobpowell.net/grayscale.htm
                                // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1

                                // check pixel behind
                                trainingPointer2 = trainingPointer - TRAINING_PIXEL_BYTE_WIDTH;
                                grayValue2 = (int)(0.3 * trainingPointer2[2] + 0.59 * trainingPointer2[1] + 0.11 * trainingPointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).

                                variationTEST = grayValue - grayValue2;

                                // shift variationArray
                                for (int i = 0; i < PixelsPerTest - 1; ++i)
                                {
                                    variationArray[i] = variationArray[i + 1];
                                }

                                // store most recent value
                                variationArray[PixelsPerTest - 1] = variationTEST;

                                if (variationArray[0] != VALUE_NOT_DEFINED)
                                {
                                    // compute sum variation over X pixel transitions
                                    int variationSum = 0;
                                    int upVariationSum = 0;
                                    for (int i = 0; i < PixelsPerTest; ++i)
                                    {
                                        variationSum += variationArray[i];
                                    }

                                    variationSum = Math.Max(-127, Math.Min(128, variationSum)); // make sure we stay within 1 byte (0..255)
                                    upVariationSum = -variationSum;

                                    variation_byteSized = variationSum + 127; // shift the values up so that no change = 127; biggest brightness increase = 255; biggest darkness increase = 0
                                    upVariation_byteSized = upVariationSum + 127;

                                    variation_byteSized = Math.Max(0, Math.Min(255, variation_byteSized)); // make sure we stay within 1 byte (0..255)
                                    upVariation_byteSized = Math.Max(0, Math.Min(255, upVariation_byteSized)); // make sure we stay within 1 byte (0..255)


                                    // UPDATE "RIGHT" VALUES...
                                    patternMinRightValuesPointer = (byte*)patternMinRightValuesBitmapData.Scan0; // init to first byte of image
                                    patternMinRightValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                    patternMaxRightValuesPointer = (byte*)patternMaxRightValuesBitmapData.Scan0; // init to first byte of image
                                    patternMaxRightValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                    if (variation_byteSized < patternMinRightValuesPointer[0] || !patternsInitialized)
                                    {
                                        patternMinRightValuesPointer[0] = (byte)variation_byteSized; // gray value
                                    }

                                    if (variation_byteSized > patternMaxRightValuesPointer[0] || !patternsInitialized)
                                    {
                                        patternMaxRightValuesPointer[0] = (byte)variation_byteSized; // gray value
                                    }

                                    if (mSpreadEdgesEnabled)
                                    {
                                        if (variation_byteSized < -mSpreadEdgesThreshhold)
                                        {
                                            //Project().Window().logMessage(Name + " spreading sharp edge of " + variation + " at " + x + "," + y);
                                            // pixel above
                                            SpreadMinValueTo(x, y - 1, variation_byteSized, patternMinRightValuesBitmapData);

                                            // pixel below
                                            SpreadMinValueTo(x, y + 1, variation_byteSized, patternMinRightValuesBitmapData);

                                            // pixel to left
                                            SpreadMinValueTo(x - 1, y, variation_byteSized, patternMinRightValuesBitmapData);

                                            // pixel to right
                                            SpreadMinValueTo(x + 1, y, variation_byteSized, patternMinRightValuesBitmapData);
                                        }
                                        else if (variation_byteSized > mSpreadEdgesThreshhold)
                                        {
                                            //Project().Window().logMessage(Name + " spreading sharp edge of " + variation + " at " + x + "," + y);
                                            // pixel above
                                            SpreadMaxValueTo(x, y - 1, variation_byteSized, patternMaxRightValuesBitmapData);

                                            // pixel below
                                            SpreadMaxValueTo(x, y + 1, variation_byteSized, patternMaxRightValuesBitmapData);

                                            // pixel to left
                                            SpreadMaxValueTo(x - 1, y, variation_byteSized, patternMaxRightValuesBitmapData);

                                            // pixel to right
                                            SpreadMaxValueTo(x + 1, y, variation_byteSized, patternMaxRightValuesBitmapData);
                                        }
                                    }

                                    // UPDATE "LEFT" VALUES...
                                    int testPositionX = x - PixelsPerTest;
                                    if (testPositionX < 0) throw new ArgumentException("askdwdkjlhsdkjhas");
                                    patternMinLeftValuesPointer = (byte*)patternMinLeftValuesBitmapData.Scan0; // init to first byte of image
                                    patternMinLeftValuesPointer += (y * patternStride) + (testPositionX * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                    patternMaxLeftValuesPointer = (byte*)patternMaxLeftValuesBitmapData.Scan0; // init to first byte of image
                                    patternMaxLeftValuesPointer += (y * patternStride) + (testPositionX * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                    if (upVariation_byteSized < patternMinLeftValuesPointer[0] || !patternsInitialized)
                                    {
                                        patternMinLeftValuesPointer[0] = (byte)upVariation_byteSized; // gray value
                                    }

                                    if (upVariation_byteSized > patternMaxLeftValuesPointer[0] || !patternsInitialized)
                                    {
                                        patternMaxLeftValuesPointer[0] = (byte)upVariation_byteSized; // gray value
                                    }

                                    if (patternMaxLeftValuesPointer[0] < patternMinLeftValuesPointer[0])
                                    {
                                        throw new ArgumentException("asldkjaslkhuweeu");
                                    }

                                    if (mSpreadEdgesEnabled)
                                    {
                                        if (upVariation_byteSized < -mSpreadEdgesThreshhold)
                                        {
                                            //Project().Window().logMessage(Name + " spreading sharp edge of " + variation + " at " + x + "," + y);
                                            // pixel above
                                            SpreadMinValueTo(testPositionX, y - 1, upVariation_byteSized, patternMinLeftValuesBitmapData);

                                            // pixel below
                                            SpreadMinValueTo(testPositionX, y + 1, upVariation_byteSized, patternMinLeftValuesBitmapData);

                                            // pixel to left
                                            SpreadMinValueTo(testPositionX - 1, y, upVariation_byteSized, patternMinLeftValuesBitmapData);

                                            // pixel to right
                                            SpreadMinValueTo(testPositionX + 1, y, upVariation_byteSized, patternMinLeftValuesBitmapData);
                                        }
                                        else if (upVariation_byteSized > mSpreadEdgesThreshhold)
                                        {
                                            //Project().Window().logMessage(Name + " spreading sharp edge of " + variation + " at " + x + "," + y);
                                            // pixel above
                                            SpreadMaxValueTo(testPositionX, y - 1, upVariation_byteSized, patternMaxLeftValuesBitmapData);

                                            // pixel below
                                            SpreadMaxValueTo(testPositionX, y + 1, upVariation_byteSized, patternMaxLeftValuesBitmapData);

                                            // pixel to left
                                            SpreadMaxValueTo(testPositionX - 1, y, upVariation_byteSized, patternMaxLeftValuesBitmapData);

                                            // pixel to right
                                            SpreadMaxValueTo(testPositionX + 1, y, upVariation_byteSized, patternMaxLeftValuesBitmapData);
                                        }
                                    }
                                }
                            } // end for y
                        } // end for x
                        patternsInitialized = true; // after we've iterated through the first learning image, we are initialized

//                        mPatternMinValues.Save(patternDirPath + "\\MinValues" + fileIndex + ".bmp", ImageFormat.Bmp);
//                        mPatternMaxValues.Save(patternDirPath + "\\MaxValues" + fileIndex + ".bmp", ImageFormat.Bmp);
//                        fileIndex++;
                    } // end unsafe block
                } // end try
                finally
                {
                    trainingBitmap.UnlockBits(trainingBitmapData);

                    mPatternMinDownValues.UnlockBits(patternMinDownValuesBitmapData);
                    mPatternMaxDownValues.UnlockBits(patternMaxDownValuesBitmapData);
                    mPatternMinUpValues.UnlockBits(patternMinUpValuesBitmapData);
                    mPatternMaxUpValues.UnlockBits(patternMaxUpValuesBitmapData);

                    mPatternMinRightValues.UnlockBits(patternMinRightValuesBitmapData);
                    mPatternMaxRightValues.UnlockBits(patternMaxRightValuesBitmapData);
                    mPatternMinLeftValues.UnlockBits(patternMinLeftValuesBitmapData);
                    mPatternMaxLeftValues.UnlockBits(patternMaxLeftValuesBitmapData);
                }
                trainingBitmap.Dispose();
            } // end foreach file

            Bitmap patternWeaknessBitmap = new Bitmap(mPatternMinDownValues.Width, mPatternMinDownValues.Height, PATTERN_PIXEL_FORMAT);
            BitmapData patternWeaknessBitmapData = null;
            try
            {
                ColorPalette monoPalette = patternWeaknessBitmap.Palette;
                Color[] entries = monoPalette.Entries;
                for (int i = 0; i < 256; i++) entries[i] = Color.FromArgb(i, i, i);
                patternWeaknessBitmap.Palette = monoPalette;

                patternMinDownValuesBitmapData = mPatternMinDownValues.LockBits(new Rectangle(0, 0, mPatternMinDownValues.Width, mPatternMinDownValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                patternMaxDownValuesBitmapData = mPatternMaxDownValues.LockBits(new Rectangle(0, 0, mPatternMaxDownValues.Width, mPatternMaxDownValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                patternWeaknessBitmapData = patternWeaknessBitmap.LockBits(new Rectangle(0, 0, patternWeaknessBitmap.Width, patternWeaknessBitmap.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                int patternStride = patternMinDownValuesBitmapData.Stride;
                int patternStrideOffset = patternStride - (patternMinDownValuesBitmapData.Width * PATTERN_PIXEL_BYTE_WIDTH);

                unsafe
                {
                    byte* patternMinValuesPointer;
                    byte* patternMaxValuesPointer;
                    byte* patternWeaknessValuesPointer;
                    for (int x = 0; x < patternWeaknessBitmap.Width; x++) // TODO: incorporate Up values where Down values can't be computed
                    {
                        for (int y = 0; y < patternWeaknessBitmap.Height; y++)
                        {
                            patternMinValuesPointer = (byte*)patternMinDownValuesBitmapData.Scan0; // init to first byte of image
                            patternMinValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                            patternMaxValuesPointer = (byte*)patternMaxDownValuesBitmapData.Scan0; // init to first byte of image
                            patternMaxValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                            patternWeaknessValuesPointer = (byte*)patternWeaknessBitmapData.Scan0; // init to first byte of image
                            patternWeaknessValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                            patternWeaknessValuesPointer[0] = (byte)(patternMaxValuesPointer[0] - patternMinValuesPointer[0]);
                        }
                    }
                } // end unsafe bloc;
            }
            finally
            {
                mPatternMinDownValues.UnlockBits(patternMinDownValuesBitmapData);
                mPatternMaxDownValues.UnlockBits(patternMaxDownValuesBitmapData);
                patternWeaknessBitmap.UnlockBits(patternMaxDownValuesBitmapData);
            }

            // save patterns
            try
            {
                mPatternMinDownValues.Save(patternDirPath + "\\MinDownValues.bmp", ImageFormat.Bmp);
                mPatternMaxDownValues.Save(patternDirPath + "\\MaxDownValues.bmp", ImageFormat.Bmp);
                mPatternMinUpValues.Save(patternDirPath + "\\MinUpValues.bmp", ImageFormat.Bmp);
                mPatternMaxUpValues.Save(patternDirPath + "\\MaxUpValues.bmp", ImageFormat.Bmp);

                mPatternMinRightValues.Save(patternDirPath + "\\MinRightValues.bmp", ImageFormat.Bmp);
                mPatternMaxRightValues.Save(patternDirPath + "\\MaxRightValues.bmp", ImageFormat.Bmp);
                mPatternMinLeftValues.Save(patternDirPath + "\\MinLeftValues.bmp", ImageFormat.Bmp);
                mPatternMaxLeftValues.Save(patternDirPath + "\\MaxLeftValues.bmp", ImageFormat.Bmp);

                patternWeaknessBitmap.Save(patternDirPath + "\\PatternWeakness.bmp", ImageFormat.Bmp);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to save pattern files to disk.  Files open?  Disk full?  Bad path?");
            }
        }
        private void SpreadMinValueTo(int x, int y, int value, BitmapData patternBitmapData)
        {
            if (x < 0 || y < 0) return;
            unsafe // TODO: can I wrap a chunk of a class def in unsafe?
            {
                byte* patternPointer = (byte*)patternBitmapData.Scan0; // init to first byte of image
                patternPointer += (y * patternBitmapData.Stride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                if (value < patternPointer[0])
                {
                    patternPointer[0] = (byte)value;
                }
            }
        }
        private void SpreadMaxValueTo(int x, int y, int value, BitmapData patternBitmapData)
        {
            if (x < 0 || y < 0) return;
            unsafe // TODO: can I wrap a chunk of a class def in unsafe?
            {
                byte* patternPointer = (byte*)patternBitmapData.Scan0; // init to first byte of image
                patternPointer += (y * patternBitmapData.Stride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point
                if (value > patternPointer[0])
                {
                    patternPointer[0] = (byte)value;
                }
            }
        }
    }
}

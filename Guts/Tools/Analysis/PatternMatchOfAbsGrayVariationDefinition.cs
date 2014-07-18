// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace NetCams
{
    public class PatternMatchOfAbsGrayVariationDefinition : NetCams.ImageScorerDefinition
    {
        public PatternMatchOfAbsGrayVariationDefinition(TestSequence testSequence)
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
			new PatternMatchOfAbsGrayVariationInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mROI != null && mROI.IsDependentOn(theOtherObject)) return true;
            if (mVariationThreshhold != null && mVariationThreshhold.IsDependentOn(theOtherObject)) return true;
            if (mSloppiness != null && mSloppiness.IsDependentOn(theOtherObject)) return true;
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
            set
            {
                HandlePropertyChange(this, "LearningPath", mLearningPath, value);
                mLearningPath = value;
                LoadPatterns(true);
            }
            get { return mLearningPath; }
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

        public Bitmap mPatternMinValues;
        public Bitmap mPatternMaxValues;

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

        private GeneratedImageDefinition mMarkedImage = null;
        [CategoryAttribute("Output")]
        public GeneratedImageDefinition MarkedImage
        {
            get { return mMarkedImage; }
        }

        public void LoadPatterns(bool ignoreErrors)
        {
            if (mPatternMinValues != null)
            {
                mPatternMinValues.Dispose();
                mPatternMinValues = null;
            }
            if (mPatternMaxValues != null)
            {
                mPatternMaxValues.Dispose();
                mPatternMaxValues = null;
            }

            string patternDirPath = PatternPath;
            try
            {
                mPatternMinValues = new Bitmap(patternDirPath + "\\MinValues.bmp");
                mPatternMaxValues = new Bitmap(patternDirPath + "\\MaxValues.bmp");
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

        public static readonly int TRAINING_PIXEL_BYTE_WIDTH = 4; // determined by PixelFormat.Format32bppArgb; http://www.bobpowell.net/lockingbits.htm
        public static readonly int PATTERN_PIXEL_BYTE_WIDTH = 1; // determined by PixelFormat.Format8BppIndexed; http://www.bobpowell.net/lockingbits.htm
        public static readonly PixelFormat TRAINING_PIXEL_FORMAT = PixelFormat.Format32bppArgb;
        public static readonly PixelFormat PATTERN_PIXEL_FORMAT = PixelFormat.Format8bppIndexed;
        public void TrainPatterns()
        {
            if (mPatternMinValues != null)
            {
                mPatternMinValues.Dispose();
                mPatternMinValues = null;
            }
            if (mPatternMaxValues != null)
            {
                mPatternMaxValues.Dispose();
                mPatternMaxValues = null;
            }

            // for LockBits see http://www.bobpowell.net/lockingbits.htm & http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
            BitmapData trainingBitmapData = null;
            BitmapData patternMinValuesBitmapData = null;
            BitmapData patternMaxValuesBitmapData = null;
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
                if (mPatternMinValues == null || mPatternMaxValues == null)
                {
                    mPatternMinValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    mPatternMaxValues = new Bitmap(trainingBitmap.Width, trainingBitmap.Height, PATTERN_PIXEL_FORMAT);
                    ColorPalette monoPalette = mPatternMinValues.Palette;
                    Color[] entries = monoPalette.Entries;
                    for (int i = 0; i < 256; i++) entries[i] = Color.FromArgb(i, i, i);
                    mPatternMinValues.Palette = monoPalette;
                    mPatternMaxValues.Palette = monoPalette;
                    patternsInitialized = false;
                }

                if (trainingBitmap.Width != mPatternMinValues.Width || trainingBitmap.Height != mPatternMinValues.Height)
                {
                    throw new ArgumentException("Learning images are not all the same size.");
                }

                try
                {
                    trainingBitmapData = trainingBitmap.LockBits(new Rectangle(0, 0, trainingBitmap.Width, trainingBitmap.Height), ImageLockMode.ReadOnly, TRAINING_PIXEL_FORMAT);
                    int trainingStride = trainingBitmapData.Stride;
                    int trainingStrideOffset = trainingStride - (trainingBitmapData.Width * TRAINING_PIXEL_BYTE_WIDTH);

                    patternMinValuesBitmapData = mPatternMinValues.LockBits(new Rectangle(0, 0, mPatternMinValues.Width, mPatternMinValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                    patternMaxValuesBitmapData = mPatternMaxValues.LockBits(new Rectangle(0, 0, mPatternMaxValues.Width, mPatternMaxValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                    int patternStride = patternMinValuesBitmapData.Stride;
                    int patternStrideOffset = patternStride - (patternMinValuesBitmapData.Width * PATTERN_PIXEL_BYTE_WIDTH);

                    if (patternStride != patternMaxValuesBitmapData.Stride)
                    {
                        throw new ArgumentException("oops");
                    }
                    Color color;
                    int grayValue;
                    int grayValue2;
                    int varSum = 0;

                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* trainingPointer;
                        byte* trainingPointer2;
                        byte* patternMinValuesPointer;
                        byte* patternMaxValuesPointer;

                        for (int x = 1; x < trainingBitmap.Width-1; x++) // starting 1 pixel late and stopping 1 pixel early since we look at surrounding pixels.  we don't normally need to test edge of image anyway
                        {
                            for (int y = 1; y < trainingBitmap.Height-1; y++)
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
                                varSum = Math.Abs(grayValue - grayValue2); // NOTE: using '=' to init varSum for this pixel

                                /*
                                // check pixel below
                                trainingPointer2 = trainingPointer + trainingStride;
                                grayValue2 = (int)(0.3 * trainingPointer2[2] + 0.59 * trainingPointer2[1] + 0.11 * trainingPointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                varSum += Math.Abs(grayValue - grayValue2); // NOTE: using '+=' to sum varSum

                                // check left pixel
                                trainingPointer2 = trainingPointer - TRAINING_PIXEL_BYTE_WIDTH;
                                grayValue2 = (int)(0.3 * trainingPointer2[2] + 0.59 * trainingPointer2[1] + 0.11 * trainingPointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                varSum += Math.Abs(grayValue - grayValue2); // NOTE: using '+=' to sum varSum

                                // check right pixel
                                trainingPointer2 = trainingPointer + TRAINING_PIXEL_BYTE_WIDTH;
                                grayValue2 = (int)(0.3 * trainingPointer2[2] + 0.59 * trainingPointer2[1] + 0.11 * trainingPointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                varSum += Math.Abs(grayValue - grayValue2); // NOTE: using '+=' to sum varSum
                                 */

                                varSum = Math.Min(255, varSum);

                                patternMinValuesPointer = (byte*)patternMinValuesBitmapData.Scan0; // init to first byte of image
                                patternMinValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                patternMaxValuesPointer = (byte*)patternMaxValuesBitmapData.Scan0; // init to first byte of image
                                patternMaxValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                                if (varSum < patternMinValuesPointer[0] || !patternsInitialized)
                                {
                                    patternMinValuesPointer[0] = (byte)varSum; // gray value
                                }

                                if (varSum > patternMaxValuesPointer[0] || !patternsInitialized)
                                {
                                    patternMaxValuesPointer[0] = (byte)varSum; // gray value
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
                    mPatternMinValues.UnlockBits(patternMinValuesBitmapData);
                    mPatternMaxValues.UnlockBits(patternMaxValuesBitmapData);
                }
                trainingBitmap.Dispose();
            } // end foreach file

            Bitmap patternWeaknessBitmap = new Bitmap(mPatternMinValues.Width, mPatternMinValues.Height, PATTERN_PIXEL_FORMAT);
            BitmapData patternWeaknessBitmapData = null;
            try
            {
                ColorPalette monoPalette = patternWeaknessBitmap.Palette;
                Color[] entries = monoPalette.Entries;
                for (int i = 0; i < 256; i++) entries[i] = Color.FromArgb(i, i, i);
                patternWeaknessBitmap.Palette = monoPalette;

                patternMinValuesBitmapData = mPatternMinValues.LockBits(new Rectangle(0, 0, mPatternMinValues.Width, mPatternMinValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                patternMaxValuesBitmapData = mPatternMaxValues.LockBits(new Rectangle(0, 0, mPatternMaxValues.Width, mPatternMaxValues.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                patternWeaknessBitmapData = patternWeaknessBitmap.LockBits(new Rectangle(0, 0, patternWeaknessBitmap.Width, patternWeaknessBitmap.Height), ImageLockMode.ReadWrite, PATTERN_PIXEL_FORMAT);
                int patternStride = patternMinValuesBitmapData.Stride;
                int patternStrideOffset = patternStride - (patternMinValuesBitmapData.Width * PATTERN_PIXEL_BYTE_WIDTH);

                unsafe
                {
                    byte* patternMinValuesPointer;
                    byte* patternMaxValuesPointer;
                    byte* patternWeaknessValuesPointer;
                    for (int x = 0; x < patternWeaknessBitmap.Width; x++)
                    {
                        for (int y = 0; y < patternWeaknessBitmap.Height; y++)
                        {
                            patternMinValuesPointer = (byte*)patternMinValuesBitmapData.Scan0; // init to first byte of image
                            patternMinValuesPointer += (y * patternStride) + (x * PATTERN_PIXEL_BYTE_WIDTH); // adjust to current point

                            patternMaxValuesPointer = (byte*)patternMaxValuesBitmapData.Scan0; // init to first byte of image
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
                mPatternMinValues.UnlockBits(patternMinValuesBitmapData);
                mPatternMaxValues.UnlockBits(patternMaxValuesBitmapData);
                patternWeaknessBitmap.UnlockBits(patternMaxValuesBitmapData);
            }

            // save patterns
            try
            {
                mPatternMinValues.Save(patternDirPath + "\\MinValues.bmp", ImageFormat.Bmp);
                mPatternMaxValues.Save(patternDirPath + "\\MaxValues.bmp", ImageFormat.Bmp);
                patternWeaknessBitmap.Save(patternDirPath + "\\PatternWeakness.bmp", ImageFormat.Bmp);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to save pattern files to disk.  Files open?  Disk full?  Bad path?");
            }
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class FindBlobOfSizeAndColorDefinition : NetCams.ToolDefinition
    {
        public FindBlobOfSizeAndColorDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mLeftBound = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "LeftBound"));
            mLeftBound.Type = DataType.IntegerNumber;
            mLeftBound.AddDependency(this);
            mLeftBound.Name = "LeftBound";

            mRightBound = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "RightBound"));
            mRightBound.Type = DataType.IntegerNumber;
            mRightBound.AddDependency(this);
            mRightBound.Name = "RightBound";

            mTopBound = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "TopBound"));
            mTopBound.Type = DataType.IntegerNumber;
            mTopBound.AddDependency(this);
            mTopBound.Name = "TopBound";

            mBottomBound = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "BottomBound"));
            mBottomBound.Type = DataType.IntegerNumber;
            mBottomBound.AddDependency(this);
            mBottomBound.Name = "BottomBound";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindBlobOfSizeAndColorInstance(this, theExecution);
		}

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mImageToMark != null && mImageToMark.IsDependentOn(theOtherObject)) return true;
            if (mEnabled != null && mEnabled.IsDependentOn(theOtherObject)) return true;
            if (mStepSize != null && mStepSize.IsDependentOn(theOtherObject)) return true;
            if (mDetailedSearch != null && mDetailedSearch.IsDependentOn(theOtherObject)) return true;
            if (mROI != null && mROI.IsDependentOn(theOtherObject)) return true;
            if (mExpectedObjectHeight != null && mExpectedObjectHeight.IsDependentOn(theOtherObject)) return true;
            if (mExpectedObjectWidth != null && mExpectedObjectWidth.IsDependentOn(theOtherObject)) return true;
            if (mAllowedSizeVariation != null && mAllowedSizeVariation.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mImageToMark != null) result = Math.Max(result, mImageToMark.ToolMapRow);
                if (mEnabled != null) result = Math.Max(result, mEnabled.ToolMapRow);
                if (mStepSize != null) result = Math.Max(result, mStepSize.ToolMapRow);
                if (mDetailedSearch != null) result = Math.Max(result, mDetailedSearch.ToolMapRow);
                if (mROI != null) result = Math.Max(result, mROI.ToolMapRow);
                if (mExpectedObjectHeight != null) result = Math.Max(result, mExpectedObjectHeight.ToolMapRow);
                if (mExpectedObjectWidth != null) result = Math.Max(result, mExpectedObjectWidth.ToolMapRow);
                if (mAllowedSizeVariation != null) result = Math.Max(result, mAllowedSizeVariation.ToolMapRow);
                return result + 1;
            }
        }

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

        /// <summary>
        /// SearchRecord & LastMarkerUsed are used as an optimization in FindEdge.
        /// In FindBlobOfSizeAndColor, we wanted to mark each pixel that matched so that if we ran into it again, we could abort since we must have run into a region that was already discovered to be too big...so we just made it bigger by merging with it...so we want to abort
        /// We store it here, since we don't want to create the array for every instance.  We use LastMarkerUsed to mark searched pixels uniquely for each test execution...this way we don't have to clear the array for each execution...now we only clear it after MAX_INT executions
        /// </summary>
        public short[,] SearchRecord = new short[640, 480];
        public short LastMarkerUsed = 0;

        private IRectangleROIDefinition mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public IRectangleROIDefinition ROI
        {
            get { return mROI; }
            set 
            {
                HandlePropertyChange(this, "ROI", mROI, value);
                mROI = value;
            }
        }

        private ColorMatchDefinition mColorMatchDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public ColorMatchDefinition ColorMatchDefinition
        {
            get { return mColorMatchDefinition; }
            set
            {
                HandlePropertyChange(this, "ColorMatchDefinition", mColorMatchDefinition, value);
                mColorMatchDefinition = value;
            }
        }

        private DataValueDefinition mEnabled;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition Enabled
        {
            get { return mEnabled; }
            set 
            {
                HandlePropertyChange(this, "Enabled", mEnabled, value);
                mEnabled = value;
            }
        }

        private DataValueDefinition mMinObjectHeight;
        [CategoryAttribute("Parameters, Object Size, Hard Limits"),
        DescriptionAttribute("")]
        public DataValueDefinition MinObjectHeight
        {
            get { return mMinObjectHeight; }
            set
            {
                HandlePropertyChange(this, "MinObjectHeight", mMinObjectHeight, value);
                mMinObjectHeight = value;
            }
        }

        private DataValueDefinition mMaxObjectHeight;
        [CategoryAttribute("Parameters, Object Size, Hard Limits"),
        DescriptionAttribute("")]
        public DataValueDefinition MaxObjectHeight
        {
            get { return mMaxObjectHeight; }
            set
            {
                HandlePropertyChange(this, "MaxObjectHeight", mMaxObjectHeight, value);
                mMaxObjectHeight = value;
            }
        }

        private DataValueDefinition mMinObjectWidth;
        [CategoryAttribute("Parameters, Object Size, Hard Limits"),
        DescriptionAttribute("")]
        public DataValueDefinition MinObjectWidth
        {
            get { return mMinObjectWidth; }
            set
            {
                HandlePropertyChange(this, "MinObjectWidth", mMinObjectWidth, value);
                mMinObjectWidth = value;
            }
        }

        private DataValueDefinition mMaxObjectWidth;
        [CategoryAttribute("Parameters, Object Size, Hard Limits"),
        DescriptionAttribute("")]
        public DataValueDefinition MaxObjectWidth
        {
            get { return mMaxObjectWidth; }
            set
            {
                HandlePropertyChange(this, "MaxObjectWidth", mMaxObjectWidth, value);
                mMaxObjectWidth = value;
            }
        }

        private DataValueDefinition mExpectedObjectHeight;
        [CategoryAttribute("Parameters, Object Size, Tolerance Approach"),
        DescriptionAttribute("")]
        public DataValueDefinition ExpectedObjectHeight
        {
            get { return mExpectedObjectHeight; }
            set
            {
                HandlePropertyChange(this, "ObjectHeight", mExpectedObjectHeight, value);
                mExpectedObjectHeight = value;
            }
        }

        private DataValueDefinition mExpectedObjectWidth;
        [CategoryAttribute("Parameters, Object Size, Tolerance Approach"),
        DescriptionAttribute("")]
        public DataValueDefinition ExpectedObjectWidth
        {
            get { return mExpectedObjectWidth; }
            set
            {
                HandlePropertyChange(this, "ObjectWidth", mExpectedObjectWidth, value);
                mExpectedObjectWidth = value;
            }
        }

        private DataValueDefinition mAllowedSizeVariation;
        [CategoryAttribute("Parameters, Object Size, Tolerance Approach"),
        DescriptionAttribute("A percentage of allowed variation from the specified ObjectHeight and ObjectWidth.")]
        public DataValueDefinition AllowedSizeVariation
        {
            get { return mAllowedSizeVariation; }
            set
            {
                HandlePropertyChange(this, "AllowedSizeVariation", mAllowedSizeVariation, value);
                mAllowedSizeVariation = value;
            }
        }

        private DataValueDefinition mStepSize;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition StepSize
        {
            get { return mStepSize; }
            set
            {
                HandlePropertyChange(this, "StepSize", mStepSize, value);
                mStepSize = value;
            }
        }

        private DataValueDefinition mDetailedSearch;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition DetailedSearch
        {
            get { return mDetailedSearch; }
            set 
            {
                HandlePropertyChange(this, "DetailedSearch", mDetailedSearch, value);
                mDetailedSearch = value;
            }
        }

        private GeneratedValueDefinition mLeftBound = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition LeftBound
        {
            get { return mLeftBound; }
        }

        private GeneratedValueDefinition mRightBound = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition RightBound
        {
            get { return mRightBound; }
        }

        private GeneratedValueDefinition mTopBound = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition TopBound
        {
            get { return mTopBound; }
        }

        private GeneratedValueDefinition mBottomBound = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition BottomBound
        {
            get { return mBottomBound; }
        }

        private DataValueDefinition mImageMarkingEnabled;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public DataValueDefinition ImageMarkingEnabled
        {
            get { return mImageMarkingEnabled; }
            set 
            {
                HandlePropertyChange(this, "ImageMarkingEnabled", mImageMarkingEnabled, value);
                mImageMarkingEnabled = value;
            }
        }

        private ImageDefinition mImageToMark = null;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public ImageDefinition ImageToMark
        {
            get { return mImageToMark; }
            set
            {
                HandlePropertyChange(this, "ImageToMark", mImageToMark, value);
                if (value != mImageToMark)
                {
                    //if( mImageToMark != null ) mImageToMark.RemoveDependency(this);  WE NEED TO BE DEPENDENT ON the image so we know it is created (e.g. by duplicator) before we try to mark it...  if a tool wants to be dependent on a FULLY MARKED image, it must register dependencies on the markers.
                    mImageToMark = value;
                    //if( mImageToMark != null ) mImageToMark.AddDependency(this);
                }
            }
        }

        private Color mMarkColor = Color.Yellow;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public Color MarkColor
        {
            get { return mMarkColor; }
            set
            {
                HandlePropertyChange(this, "MarkColor", mMarkColor, value);
                mMarkColor = value;
            }
        }

        private bool mAutoSave = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public bool AutoSave
        {
            get { return mAutoSave; }
            set 
            {
                HandlePropertyChange(this, "AutoSave", mAutoSave, value);
                mAutoSave = value;
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
    }
}

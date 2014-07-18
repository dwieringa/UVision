// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class FindRadialLineDefinition : NetCams.ToolDefinition
    {
        public FindRadialLineDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mOuterSearchBounds = new ToolCircleDecorationDefinition(testSequence, OwnerLink.newLink(this, "OuterSearchBounds"));
            mOuterSearchBounds.Name = "Outer bound";
            mOuterSearchBounds.AddDependency(this);

            mInnerSearchBounds = new ToolCircleDecorationDefinition(testSequence, OwnerLink.newLink(this, "InnerSearchBounds"));
            mInnerSearchBounds.Name = "Inner bound";
            mInnerSearchBounds.AddDependency(this);

            SearchBoundsColor = Color.Yellow;

            mResultantAngle = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "ResultantAngle"));
            mResultantAngle.Type = DataType.IntegerNumber;
            mResultantAngle.AddDependency(this);
            mResultantAngle.Name = "ResultantAngle";

            mResultantRay = new ValueBasedLineDecorationDefinition(testSequence, OwnerLink.newLink(this, "ResultantRay"));
            mResultantRay.AddDependency(mResultantAngle);
            mResultantRay.Name = "ResultantRay";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindRadialLineInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mImageToMark != null && mImageToMark.IsDependentOn(theOtherObject)) return true;
            if (mCenterX != null && mCenterX.IsDependentOn(theOtherObject)) return true;
            if (mCenterY != null && mCenterY.IsDependentOn(theOtherObject)) return true;
            if (mOuterSearchRadius != null && mOuterSearchRadius.IsDependentOn(theOtherObject)) return true;
            if (mInnerSearchRadius != null && mInnerSearchRadius.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mImageToMark != null) result = Math.Max(result, mImageToMark.ToolMapRow);
                if (mCenterX != null) result = Math.Max(result, mCenterX.ToolMapRow);
                if (mCenterY != null) result = Math.Max(result, mCenterY.ToolMapRow);
                if (mOuterSearchRadius != null) result = Math.Max(result, mOuterSearchRadius.ToolMapRow);
                if (mInnerSearchRadius != null) result = Math.Max(result, mInnerSearchRadius.ToolMapRow);
                return result + 1;
			}
		}

        private ValueBasedLineDecorationDefinition mResultantRay = null;
        [CategoryAttribute("Debug Options")]
        public ValueBasedLineDecorationDefinition ResultantRay // TODO: needed as property??? would be nice if it's properties could be shown in tree
        {
            get { return mResultantRay; }
        }


        private ToolCircleDecorationDefinition mOuterSearchBounds;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public ToolCircleDecorationDefinition OuterSearchBounds
        {
            get { return mOuterSearchBounds; }
        }

        private ToolCircleDecorationDefinition mInnerSearchBounds;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public ToolCircleDecorationDefinition InnerSearchBounds
        {
            get { return mInnerSearchBounds; }
        }

        private DataValueDefinition mNumberOfTestsInDonut;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition NumberOfTestsInDonut
        {
            get { return mNumberOfTestsInDonut; }
            set 
            {
                HandlePropertyChange(this, "NumberOfTestsInDonut", mNumberOfTestsInDonut, value);
                mNumberOfTestsInDonut = value;
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

        private DataValueDefinition mCenterX;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition CenterX
        {
            get { return mCenterX; }
            set
            {
                HandlePropertyChange(this, "CenterX", mCenterX, value);
                mCenterX = value;
                mOuterSearchBounds.SetCenterX(mCenterX);
                mInnerSearchBounds.SetCenterX(mCenterX);
            }
        }

        private DataValueDefinition mCenterY;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition CenterY
        {
            get { return mCenterY; }
            set
            {
                HandlePropertyChange(this, "CenterY", mCenterY, value);
                mCenterY = value;
                mOuterSearchBounds.SetCenterY(mCenterY);
                mInnerSearchBounds.SetCenterY(mCenterY);
            }
        }

        private DataValueDefinition mOuterSearchRadius;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition OuterSearchRadius
        {
            get { return mOuterSearchRadius; }
            set
            {
                HandlePropertyChange(this, "OuterSearchRadius", mOuterSearchRadius, value);
                mOuterSearchRadius = value;
                mOuterSearchBounds.SetRadius(mOuterSearchRadius);
            }
        }

        private DataValueDefinition mInnerSearchRadius;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition InnerSearchRadius
        {
            get { return mInnerSearchRadius; }
            set
            {
                HandlePropertyChange(this, "InnerSearchRadius", mInnerSearchRadius, value);
                mInnerSearchRadius = value;
                mInnerSearchBounds.SetRadius(mInnerSearchRadius);
            }
        }

        private DataValueDefinition mMarkMergeDistance_Deg;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition MarkMergeDistance_Deg
        {
            get { return mMarkMergeDistance_Deg; }
            set 
            {
                HandlePropertyChange(this, "MarkMergeDistance_Deg", mMarkMergeDistance_Deg, value);
                mMarkMergeDistance_Deg = value;
            }
        }

        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public Color SearchBoundsColor
        {
            get { return mOuterSearchBounds.Color; }
            set 
            {
                HandlePropertyChange(this, "SearchBoundsColor", mOuterSearchBounds.Color, value);
                mOuterSearchBounds.Color = value; mInnerSearchBounds.Color = value;
            }
        }

        private GeneratedValueDefinition mResultantAngle = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition ResultantAngle
        {
            get { return mResultantAngle; }
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

        private Color mMarkColor = Color.Yellow;
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

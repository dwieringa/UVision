// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Text;


namespace NetCams
{
    [DefaultPropertyAttribute("Name")]
    public class IntensityVariationOverRectangleDefinition : NetCams.ToolDefinition
    {
        public IntensityVariationOverRectangleDefinition(TestSequence testSequence)
            : base(testSequence)
		{
			// 
			// TODO: Add constructor logic here
			//
            mResult = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "Result"));
            mResult.Type = DataType.IntegerNumber;
            mResult.AddDependency(this);
            mResult.Name = "IntensityVariation";

            mMarkedImage = new GeneratedImageDefinition(testSequence, OwnerLink.newLink(this, "MarkedImage"));
            mMarkedImage.AddDependency(this);
            mMarkedImage.Name = "IntensityVariationImage";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
			new IntensityVariationOverRectangleInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mROI.IsDependentOn(theOtherObject)) return true;
            if (mVariationThreshhold.IsDependentOn(theOtherObject)) return true;
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
                return result + 1;
			}
		}

        private DataValueDefinition mVariationThreshhold;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Variation below the threshhold is ignored; above the thresshold is summed.")]
        public DataValueDefinition VariationThreshhold
        {
            get { return mVariationThreshhold; }
            set
            {
                HandlePropertyChange(this, "VariationThreshhold", mVariationThreshhold, value);
                mVariationThreshhold = value;
            }
        }

        private bool mTestXAxis;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Measure variation over the X-axis?")]
        public bool TestXAxis
        {
            get { return mTestXAxis; }
            set
            {
                HandlePropertyChange(this, "TestXAxis", mTestXAxis, value);
                mTestXAxis = value;
            }
        }

        private bool mTestYAxis;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Measure variation over the Y-axis?")]
        public bool TestYAxis
        {
            get { return mTestYAxis; }
            set 
            {
                HandlePropertyChange(this, "TestYAxis", mTestYAxis, value);
                mTestYAxis = value;
            }
        }

        private IRectangleROIDefinition mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("ROI to test over")]
        public IRectangleROIDefinition ROI
        {
            get { return mROI; }
            set
            {
                HandlePropertyChange(this, "ROI", mROI, value);
                mROI = value;
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

        private GeneratedValueDefinition mResult = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition Result
        {
            get { return mResult; }
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

        private GeneratedImageDefinition mMarkedImage = null;
        [CategoryAttribute("Output")]
        public GeneratedImageDefinition MarkedImage
        {
            get { return mMarkedImage; }
        }
    }
}

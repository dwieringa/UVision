// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class FindBrightestSpotDefinition : NetCams.ToolDefinition
    {
        public FindBrightestSpotDefinition(TestSequence testSequence)
            : base(testSequence)
		{

            mBrightSpot_X = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "BrightSpot_X"));
            mBrightSpot_X.Type = DataType.IntegerNumber;
            mBrightSpot_X.AddDependency(this);
            mBrightSpot_X.Name = "BrightSpot_X";

            mBrightSpot_Y = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "BrightSpot_Y"));
            mBrightSpot_Y.Type = DataType.IntegerNumber;
            mBrightSpot_Y.AddDependency(this);
            mBrightSpot_Y.Name = "BrightSpot_Y";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindBrightestSpotInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mROI != null && mROI.IsDependentOn(theOtherObject)) return true;
            if (mBrightnessThreshold != null && mBrightnessThreshold.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mROI != null) result = Math.Max(result, mROI.ToolMapRow);
                if (mBrightnessThreshold != null) result = Math.Max(result, mBrightnessThreshold.ToolMapRow);
                return result + 1;
			}
		}

        private DataValueDefinition mBrightnessThreshold;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The minimum gray value to include")]
        public DataValueDefinition BrightnessThreshold
        {
            get { return mBrightnessThreshold; }
            set
            {
                HandlePropertyChange(this, "BrightnessThreshold", mBrightnessThreshold, value);
                mBrightnessThreshold = value;
            }
        }

        private ROIDefinition mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public ROIDefinition ROI
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


        private GeneratedValueDefinition mBrightSpot_X = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition BrightSpot_X
        {
            get { return mBrightSpot_X; }
        }

        private GeneratedValueDefinition mBrightSpot_Y = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition BrightSpot_Y
        {
            get { return mBrightSpot_Y; }
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

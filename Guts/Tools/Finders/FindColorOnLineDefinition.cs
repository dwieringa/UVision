// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    [DefaultPropertyAttribute("Name")]
    public class FindColorOnLineDefinition : NetCams.ToolDefinition
    {
        public FindColorOnLineDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mResultX = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "ResultX"));
            mResultX.Type = DataType.IntegerNumber;
            mResultX.AddDependency(this);
            mResultX.Name = "ResultX";

            mResultY = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "ResultY"));
            mResultY.Type = DataType.IntegerNumber;
            mResultY.AddDependency(this);
            mResultY.Name = "ResultY";

            mSearchEndX = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "SearchEndX"));
            mSearchEndX.Type = DataType.IntegerNumber;
            mSearchEndX.AddDependency(this);
            mSearchEndX.Name = "SearchEndX";

            mSearchEndY = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "SearchEndY"));
            mSearchEndY.Type = DataType.IntegerNumber;
            mSearchEndY.AddDependency(this);
            mSearchEndY.Name = "SearchEndY";

            mSearchPath = new ToolLineDecorationDefinition(testSequence, OwnerLink.newLink(this, "SearchPath"));
            mSearchPath.Name = "SearchPath";
            mSearchPath.AddDependency(this);
            mSearchPath.SetStartX(mStartX);
            mSearchPath.SetStartY(mStartY);
            mSearchPath.SetEndX(mSearchEndX);
            mSearchPath.SetEndY(mSearchEndY);
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindColorOnLineInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mColorMatchDefinition != null && mColorMatchDefinition.IsDependentOn(theOtherObject)) return true;
            if (mStartX != null && mStartX.IsDependentOn(theOtherObject)) return true;
            if (mStartY != null && mStartY.IsDependentOn(theOtherObject)) return true;
            if (mSlopeRise != null && mSlopeRise.IsDependentOn(theOtherObject)) return true;
            if (mSlopeRun != null && mSlopeRun.IsDependentOn(theOtherObject)) return true;
            if (mRequiredConsecutivePixels != null && mRequiredConsecutivePixels.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mColorMatchDefinition != null) result = Math.Max(result, mColorMatchDefinition.ToolMapRow);
                if (mStartX != null) result = Math.Max(result, mStartX.ToolMapRow);
                if (mStartY != null) result = Math.Max(result, mStartY.ToolMapRow);
                if (mSlopeRise != null) result = Math.Max(result, mSlopeRise.ToolMapRow);
                if (mSlopeRun != null) result = Math.Max(result, mSlopeRun.ToolMapRow);
                if (mRequiredConsecutivePixels != null) result = Math.Max(result, mRequiredConsecutivePixels.ToolMapRow);
                return result + 1;
			}
		}

        private ColorMatchDefinition mColorMatchDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the color to search for")]
        /*
        public String ColorMatchDefinition
        {
            get
            {
                if (mColorMatchDefinition == null) return "";
                return mColorMatchDefinition.Name;
            }
            set
            {
                IObjectDefinition theObject = Sequence().GetDefinitionObject(value);
                if (theObject == null)
                {
                    return; // TO DO: throw exception to notify user
                }
                if (!(theObject is ColorMatchDefinition))
                {
                    return; // TO DO: throw exception to notify user
                }
                mColorMatchDefinition = (ColorMatchDefinition)theObject;
            }
        }*/
        public ColorMatchDefinition ColorMatchDefinition
        {
            get { return mColorMatchDefinition; }
            set
            {
                HandlePropertyChange(this, "ColorMatchDefinition", mColorMatchDefinition, value);
                mColorMatchDefinition = value;
            }
        }

        private DataValueDefinition mStartX;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Position on x-axis to start searching")]
        public DataValueDefinition StartX
        {
            get { return mStartX; }
            set
            {
                HandlePropertyChange(this, "StartX", mStartX, value);
                mStartX = value;
                mSearchPath.SetStartX(value);
            }
        }

        private DataValueDefinition mStartY;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Position on y-axis to start searching")]
        public DataValueDefinition StartY
        {
            get { return mStartY; }
            set
            {
                HandlePropertyChange(this, "StartY", mStartY, value);
                mStartY = value;
                mSearchPath.SetStartY(value);
            }
        }

        private DataValueDefinition mSlopeRise;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Rise of the slope over which to search")]
        public DataValueDefinition SlopeRise
        {
            get { return mSlopeRise; }
            set 
            {
                HandlePropertyChange(this, "SlopeRise", mSlopeRise, value);
                mSlopeRise = value;
            }
        }

        private DataValueDefinition mSlopeRun;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Run of the slope over which to search")]
        public DataValueDefinition SlopeRun
        {
            get { return mSlopeRun; }
            set 
            {
                HandlePropertyChange(this, "SlopeRun", mSlopeRun, value);
                mSlopeRun = value;
            }
        }

        private DataValueDefinition mRequiredConsecutivePixels;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Number of pixels in a row that must match the color")]
        public DataValueDefinition RequiredConsecutivePixels
        {
            get { return mRequiredConsecutivePixels; }
            set 
            {
                HandlePropertyChange(this, "RequiredConsecutivePixels", mRequiredConsecutivePixels, value);
                mRequiredConsecutivePixels = value;
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

        private GeneratedValueDefinition mResultX = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition ResultX
        {
            get { return mResultX; }
        }

        private GeneratedValueDefinition mResultY = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition ResultY
        {
            get { return mResultY; }
        }

        private GeneratedValueDefinition mSearchEndX;
        [CategoryAttribute("Output"),
        DescriptionAttribute("The last pixel that was tested by the search")]
        public GeneratedValueDefinition SearchEndX
        {
            get { return mSearchEndX; }
        }

        private GeneratedValueDefinition mSearchEndY;
        [CategoryAttribute("Output"),
        DescriptionAttribute("The last pixel that was tested by the search")]
        public GeneratedValueDefinition SearchEndY
        {
            get { return mSearchEndY; }
        }

        private ToolLineDecorationDefinition mSearchPath = null;
        [CategoryAttribute("Debug Options")]
        public ToolLineDecorationDefinition SearchPath // TODO: needed as property??? would be nice if it's properties could be shown in tree
        {
            get { return mSearchPath; }
        }

        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Shows the path that was searched.")]
        public Color SearchPathColor
        {
            get { return mSearchPath.Color; }
            set 
            {
                HandlePropertyChange(this, "SearchPathColor", mSearchPath.Color, value);
                mSearchPath.Color = value;
            }
        }
    }
}

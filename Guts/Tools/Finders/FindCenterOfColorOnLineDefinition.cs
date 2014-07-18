// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class FindCenterOfColorOnLineDefinition : NetCams.ToolDefinition
    {
        public FindCenterOfColorOnLineDefinition(TestSequence testSequence)
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
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindCenterOfColorOnLineInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mSearchPath != null && mSearchPath.IsDependentOn(theOtherObject)) return true;
            //if (mRequiredConsecutiveColorMatches != null && mRequiredConsecutiveColorMatches.IsDependentOn(theOtherObject)) return true;
            if (mSearchColorDefinition != null && mSearchColorDefinition.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mSearchPath != null) result = Math.Max(result, mSearchPath.ToolMapRow);
                //if (mRequiredConsecutiveColorMatches != null) result = Math.Max(result, mRequiredConsecutiveColorMatches.ToolMapRow);
                if (mSearchColorDefinition != null) result = Math.Max(result, mSearchColorDefinition.ToolMapRow);
                return result + 1;
			}
		}

        private ColorMatchDefinition mSearchColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the color of the object to search for.")]
        public ColorMatchDefinition SearchColorDefinition
        {
            get
            {
                return mSearchColorDefinition;
            }
            set
            {
                HandlePropertyChange(this, "ObjectColorDefinition", mSearchColorDefinition, value);
                mSearchColorDefinition = value;
            }
        }

        private LineDecorationDefinition mSearchPath;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The line to search over.")]
        public LineDecorationDefinition SearchPath
        {
            get { return mSearchPath; }
            set 
            {
                HandlePropertyChange(this, "SearchPath", mSearchPath, value);
                mSearchPath = value;
            }
        }

        /*
        private DataValueDefinition mRequiredConsecutiveColorMatches;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The number of consecutive pixels that must match the color being searched for before the pixels are included.  For example, if this is set to 2, then if a matching pixel will be ignored, unless it is adjacent to another matching pixel.")]
        public DataValueDefinition RequiredConsecutiveColorMatches
        {
            get { return mRequiredConsecutiveColorMatches; }
            set
            {
                throw new ArgumentException("Support for this setting is not yet implemented so it can't be used.");
                HandlePropertyChange(this, "RequiredConsecutiveColorMatches", mRequiredConsecutiveColorMatches, value);
                mRequiredConsecutiveColorMatches = value;
            }
        }
        */

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
    }
}

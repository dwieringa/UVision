// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class FindObjectCenterOnLineDefinition : NetCams.ToolDefinition
    {
        public FindObjectCenterOnLineDefinition(TestSequence testSequence)
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
            new FindObjectCenterOnLineInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mSearchPath != null && mSearchPath.IsDependentOn(theOtherObject)) return true;
            if (mSearchSpeed != null && mSearchSpeed.IsDependentOn(theOtherObject)) return true;
            if (mRequiredConsecutiveColorMatches != null && mRequiredConsecutiveColorMatches.IsDependentOn(theOtherObject)) return true;
            if (mSearchBackgroundColorDefinition != null && mSearchBackgroundColorDefinition.IsDependentOn(theOtherObject)) return true;
            if (mObjectColorDefinition != null && mObjectColorDefinition.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mSearchPath != null) result = Math.Max(result, mSearchPath.ToolMapRow);
                if (mSearchSpeed != null) result = Math.Max(result, mSearchSpeed.ToolMapRow);
                if (mRequiredConsecutiveColorMatches != null) result = Math.Max(result, mRequiredConsecutiveColorMatches.ToolMapRow);
                if (mSearchBackgroundColorDefinition != null) result = Math.Max(result, mSearchBackgroundColorDefinition.ToolMapRow);
                if (mObjectColorDefinition != null) result = Math.Max(result, mObjectColorDefinition.ToolMapRow);
                return result + 1;
			}
		}

        private ColorMatchDefinition mObjectColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the color of the object to search for.")]
        public ColorMatchDefinition ObjectColorDefinition
        {
            get
            {
                return mObjectColorDefinition;
            }
            set
            {
                HandlePropertyChange(this, "ObjectColorDefinition", mObjectColorDefinition, value);
                mObjectColorDefinition = value;
            }
        }

        private ColorMatchDefinition mSearchBackgroundColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the background color to search through for the object.")]
        public ColorMatchDefinition SearchBackgroundColorDefinition
        {
            get
            {
                return mSearchBackgroundColorDefinition;
            }
            set
            {
                HandlePropertyChange(this, "SearchBackgroundColorDefinition", mSearchBackgroundColorDefinition, value);
                mSearchBackgroundColorDefinition = value;
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

        private Direction mSearchDirection = Direction.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The approximate direction to search along")]
        public Direction SearchDirection
        {
            get { return mSearchDirection; }
            set 
            {
                HandlePropertyChange(this, "SearchDirection", mSearchDirection, value);
                mSearchDirection = value;
            }
        }

        private DataValueDefinition mSearchSpeed;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The number of pixels to advance during the search.")]
        public DataValueDefinition SearchSpeed
        {
            get { return mSearchSpeed; }
            set 
            {
                HandlePropertyChange(this, "SearchSpeed", mSearchSpeed, value);
                mSearchSpeed = value;
            }
        }

        private DataValueDefinition mRequiredConsecutiveColorMatches;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The number of consecutive pixels that must match the color being searched for at object edges.")]
        public DataValueDefinition RequiredConsecutiveColorMatches
        {
            get { return mRequiredConsecutiveColorMatches; }
            set 
            {
                HandlePropertyChange(this, "RequiredConsecutiveColorMatches", mRequiredConsecutiveColorMatches, value);
                mRequiredConsecutiveColorMatches = value;
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
    }
}

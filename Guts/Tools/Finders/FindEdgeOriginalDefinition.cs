// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class FindEdgeOriginalDefinition : NetCams.ToolDefinition
    {
        public FindEdgeOriginalDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mEdgeMarker = new ValueBasedLineDecorationDefinition(testSequence, OwnerLink.newLink(this, "EdgeMarker"));
            mEdgeMarker.Name = "EdgeMarker";
            mEdgeMarker.AddDependency(this);

            MarkerColor = Color.Yellow;

            mEdgeLocation = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "EdgeLocation"));
            mEdgeLocation.Type = DataType.IntegerNumber;
            mEdgeLocation.AddDependency(this);
            mEdgeLocation.Name = "EdgeLocation";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindEdgeOriginalInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mSearchArea != null && mSearchArea.IsDependentOn(theOtherObject)) return true;
            if (mSurfaceNoiseThreshold != null && mSurfaceNoiseThreshold.IsDependentOn(theOtherObject)) return true;
            if (mMinSurfaceSize != null && mMinSurfaceSize.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mSearchArea != null) result = Math.Max(result, mSearchArea.ToolMapRow);
                if (mSurfaceNoiseThreshold != null) result = Math.Max(result, mSurfaceNoiseThreshold.ToolMapRow);
                if (mMinSurfaceSize != null) result = Math.Max(result, mMinSurfaceSize.ToolMapRow);
                return result + 1;
			}
		}

        private DataValueDefinition mSurfaceNoiseThreshold;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Intensity variation to ignore")]
        public DataValueDefinition SurfaceNoiseThreshold
        {
            get { return mSurfaceNoiseThreshold; }
            set
            {
                HandlePropertyChange(this, "SurfaceNoiseThreshold", mSurfaceNoiseThreshold, value);
                mSurfaceNoiseThreshold = value;
            }
        }

        private DataValueDefinition mMinSurfaceSize;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The minimum number of pixels a surface can be (otherwise it is considered part of the transition).")]
        public DataValueDefinition MinSurfaceSize
        {
            get { return mMinSurfaceSize; }
            set
            {
                HandlePropertyChange(this, "MinSurfaceSize", mMinSurfaceSize, value);
                mMinSurfaceSize = value;
            }
        }

        private ValueBasedLineDecorationDefinition mEdgeMarker = null;
        [CategoryAttribute("Debug Options")]
        public ValueBasedLineDecorationDefinition EdgeMarker
        {
            get { return mEdgeMarker; }
        }

        private IRectangleROIDefinition mSearchArea;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public IRectangleROIDefinition SearchArea
        {
            get { return mSearchArea; }
            set
            {
                HandlePropertyChange(this, "SearchArea", mSearchArea, value);
                mSearchArea = value;
            }
        }

        private Direction mSearchDirection = Direction.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The direction to search along")]
        public Direction SearchDirection
        {
            get { return mSearchDirection; }
            set
            {
                HandlePropertyChange(this, "SearchDirection", mSearchDirection, value);
                mSearchDirection = value;
            }
        }

        public enum EdgeDetectionModes
        {
            NotDefined = 0,
            SurfaceIntensity,
            MaxVariation
        }

        private EdgeDetectionModes mEdgeDetectionMode = EdgeDetectionModes.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The algorithm to detect the edge.")]
        public EdgeDetectionModes EdgeDetectionMode
        {
            get { return mEdgeDetectionMode; }
            set
            {
                HandlePropertyChange(this, "EdgeDetectionMode", mEdgeDetectionMode, value);
                mEdgeDetectionMode = value;
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


        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public Color MarkerColor
        {
            get { return mEdgeMarker.Color; }
            set
            {
                HandlePropertyChange(this, "MarkerColor", mEdgeMarker.Color, value);
                mEdgeMarker.Color = value;
            }
        }

        private GeneratedValueDefinition mEdgeLocation = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition EdgeLocation
        {
            get { return mEdgeLocation; }
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

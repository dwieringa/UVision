// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class FindTransitionDefinition : NetCams.ToolDefinition
    {
        public FindTransitionDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mTransitionMarker = new ValueBasedLineDecorationDefinition(testSequence, OwnerLink.newLink(this, "TransitionMarker"));
            mTransitionMarker.Name = "TransitionMarker";
            mTransitionMarker.AddDependency(this);

            MarkerColor = Color.Yellow;

            mTransitionLocation = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "TransitionLocation"));
            mTransitionLocation.Type = DataType.IntegerNumber;
            mTransitionLocation.AddDependency(this);
            mTransitionLocation.Name = "TransitionLocation";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindTransitionInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mSearchArea != null && mSearchArea.IsDependentOn(theOtherObject)) return true;
            if (mTransitionThreshold_Min != null && mTransitionThreshold_Min.IsDependentOn(theOtherObject)) return true;
            //if (mSurfaceNoiseThreshold_PercentOfSharpestEdge != null) result = Math.Max(result, mSurfaceNoiseThreshold_PercentOfSharpestEdge.ToolMapRow);
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mSearchArea != null) result = Math.Max(result, mSearchArea.ToolMapRow);
                if (mTransitionThreshold_Min != null) result = Math.Max(result, mTransitionThreshold_Min.ToolMapRow);
                //if (mSurfaceNoiseThreshold_PercentOfSharpestEdge != null && mSurfaceNoiseThreshold_PercentOfSharpestEdge.IsDependentOn(theOtherObject)) return true;
                return result + 1;
			}
		}

        private DataValueDefinition mTransitionThreshold_Min;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Intensity variation to ignore")]
        public DataValueDefinition TransitionThreshold_Min
        {
            get { return mTransitionThreshold_Min; }
            set
            {
                HandlePropertyChange(this, "SurfaceNoiseThreshold_Min", mTransitionThreshold_Min, value);
                mTransitionThreshold_Min = value;
            }
        }

        /* This would only be needed if we detected multiple ...vs just the largest transition
        private DataValueDefinition mSurfaceNoiseThreshold_PercentOfSharpestTransition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Intensity variation to ignore")]
        public DataValueDefinition SurfaceNoiseThreshold_PercentOfSharpestTransition
        {
            get { return mSurfaceNoiseThreshold_PercentOfSharpestTransition; }
            set
            {
                HandlePropertyChange(this, "SurfaceNoiseThreshold_PercentOfSharpestTransition", mSurfaceNoiseThreshold_PercentOfSharpestTransition, value);
                mSurfaceNoiseThreshold_PercentOfSharpestTransition = value;
            }
        }
         */

        private ValueBasedLineDecorationDefinition mTransitionMarker = null;
        [CategoryAttribute("Debug Options")]
        public ValueBasedLineDecorationDefinition TransitionMarker
        {
            get { return mTransitionMarker; }
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

        /*
        public enum TransitionDetectionModes
        {
            NotDefined = 0,
            Largest3RowAverage,
            LargestRowVariation,
            LargestVariationOverMultipleRows // sum transition over multiple rows as long as it stays in the same direction
        }
         */

        /*
        private TransitionDetectionModes mTransitionDetectionMode = TransitionDetectionModes.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The algorithm to detect the edge.")]
        public TransitionDetectionModes TransitionDetectionMode
        {
            get { return mTransitionDetectionMode; }
            set
            {
                HandlePropertyChange(this, "TransitionDetectionMode", mTransitionDetectionMode, value);
                mTransitionDetectionMode = value;
            }
        }
        */

        public enum TransitionTypeSelectionFilters
        {
            NotDefined = 0,
            BrightToDark,
            DarkToBright
        }

        private TransitionTypeSelectionFilters mTransitionTypeSelectionFilter = TransitionTypeSelectionFilters.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public TransitionTypeSelectionFilters TransitionTypeSelectionFilter
        {
            get { return mTransitionTypeSelectionFilter; }
            set
            {
                HandlePropertyChange(this, "TransitionTypeSelectionFilter", mTransitionTypeSelectionFilter, value);
                mTransitionTypeSelectionFilter = value;
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
            get { return mTransitionMarker.Color; }
            set
            {
                HandlePropertyChange(this, "MarkerColor", mTransitionMarker.Color, value);
                mTransitionMarker.Color = value;
            }
        }

        private GeneratedValueDefinition mTransitionLocation = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition TransitionLocation
        {
            get { return mTransitionLocation; }
        }

        private GeneratedValueDefinition mTransitionScore = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition TransitionScore
        {
            get { return mTransitionScore; }
        }
        private bool mTransitionScore_Enabled = false;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public bool TransitionScore_Enabled
        {
            get { return mTransitionScore_Enabled; }
            set
            {
                HandlePropertyChange(this, "TransitionScore_Enabled", mTransitionScore_Enabled, value);
                mTransitionScore_Enabled = value;
                if (!value && mTransitionScore != null)
                {
                    mTransitionScore.Dispose_UVision();
                    mTransitionScore = null;
                }
                if (value && mTransitionScore == null)
                {
                    mTransitionScore = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "TransitionScore"));
                    mTransitionScore.Type = DataType.IntegerNumber;
                    mTransitionScore.AddDependency(this);
                    mTransitionScore.Name = "TransitionScore";
                }
            }
        }

        private bool mVerboseOutput = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public bool VerboseOutput
        {
            get { return mVerboseOutput; }
            set
            {
                HandlePropertyChange(this, "VerboseOutput", mVerboseOutput, value);
                mVerboseOutput = value;
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

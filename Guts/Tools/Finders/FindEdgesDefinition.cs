// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    /* COMMENTED OUT "TEMPORARILLY" 4/16/09 SO I CAN COMPILE A NEW TOOL.  I QUIT WORKING ON THIS FINDEDGES STUFF A FEW WEEKS AGO WHEN I GOT STUCK AND RAN OUT OF TIME

    public class FindEdgesDefinition : ToolDefinition, IEdgeGeneratorDefinition
    {
        public FindEdgesDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mEdgeCollection = new EdgeCollectionDefinition(testSequence, OwnerLink.newLink(this, "EdgeCollection"));
            mEdgeCollection.Name = "EdgeCollection";
            mEdgeCollection.AddDependency(this);
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindEdgesInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mSearchArea != null && mSearchArea.IsDependentOn(theOtherObject)) return true;
            if (mSurfaceNoiseThreshold_Min != null && mSurfaceNoiseThreshold_Min.IsDependentOn(theOtherObject)) return true;
            if (mSurfaceNoiseThreshold_Max != null && mSurfaceNoiseThreshold_Max.IsDependentOn(theOtherObject)) return true;
            if (mSurfaceNoiseThreshold_PercentOfSharpestEdge != null && mSurfaceNoiseThreshold_PercentOfSharpestEdge.IsDependentOn(theOtherObject)) return true;
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
                if (mSurfaceNoiseThreshold_Min != null) result = Math.Max(result, mSurfaceNoiseThreshold_Min.ToolMapRow);
                if (mSurfaceNoiseThreshold_Max != null) result = Math.Max(result, mSurfaceNoiseThreshold_Max.ToolMapRow);
                if (mSurfaceNoiseThreshold_PercentOfSharpestEdge != null) result = Math.Max(result, mSurfaceNoiseThreshold_PercentOfSharpestEdge.ToolMapRow);
                if (mMinSurfaceSize != null) result = Math.Max(result, mMinSurfaceSize.ToolMapRow);
                return result + 1;
			}
		}

        public ChartForm mChartForm;
        private bool mChartEdgeForDebug = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public bool ChartEdgeForDebug
        {
            get { return mChartEdgeForDebug; }
            set
            {
                HandlePropertyChange(this, "ChartEdgeForDebug", mChartEdgeForDebug, value);
                mChartEdgeForDebug = value;
            }
        }

        private DataValueDefinition mDisableSurfaceTransitionFilter;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("This should only be needed in environments where surfaces may be very small (ie <6 pixels)")]
        public DataValueDefinition DisableSurfaceTransitionFilter
        {
            get { return mDisableSurfaceTransitionFilter; }
            set
            {
                HandlePropertyChange(this, "DisableSurfaceTransitionFilter", mDisableSurfaceTransitionFilter, value);
                mDisableSurfaceTransitionFilter = value;
            }
        }

        private bool mFilterWeakTransitions_MinMaxComparisons = true;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public bool FilterWeakTransitions_MinMaxComparisons
        {
            get { return mFilterWeakTransitions_MinMaxComparisons; }
            set
            {
                HandlePropertyChange(this, "FilterWeakTransitions_MinMaxComparisons", mFilterWeakTransitions_MinMaxComparisons, value);
                mFilterWeakTransitions_MinMaxComparisons = value;
            }
        }

        private bool mFilterWeakTransitions_AvgAbsVariation = true;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public bool FilterWeakTransitions_AvgAbsVariation
        {
            get { return mFilterWeakTransitions_AvgAbsVariation; }
            set
            {
                HandlePropertyChange(this, "FilterWeakTransitions_AvgAbsVariation", mFilterWeakTransitions_AvgAbsVariation, value);
                mFilterWeakTransitions_AvgAbsVariation = value;
            }
        }

        private DataValueDefinition mSurfaceNoiseThreshold_Min;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Intensity variation to ignore")]
        public DataValueDefinition SurfaceNoiseThreshold_Min
        {
            get { return mSurfaceNoiseThreshold_Min; }
            set
            {
                HandlePropertyChange(this, "SurfaceNoiseThreshold_Min", mSurfaceNoiseThreshold_Min, value);
                mSurfaceNoiseThreshold_Min = value;
            }
        }

        private DataValueDefinition mSurfaceNoiseThreshold_Max;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Limit the Intensity variation to ignore")]
        public DataValueDefinition SurfaceNoiseThreshold_Max
        {
            get { return mSurfaceNoiseThreshold_Max; }
            set
            {
                HandlePropertyChange(this, "SurfaceNoiseThreshold_Max", mSurfaceNoiseThreshold_Max, value);
                mSurfaceNoiseThreshold_Max = value;
            }
        }

        private DataValueDefinition mSurfaceNoiseThreshold_PercentOfSharpestEdge;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Intensity variation to ignore")]
        public DataValueDefinition SurfaceNoiseThreshold_PercentOfSharpestEdge
        {
            get { return mSurfaceNoiseThreshold_PercentOfSharpestEdge; }
            set
            {
                HandlePropertyChange(this, "SurfaceNoiseThreshold_PercentOfSharpestEdge", mSurfaceNoiseThreshold_PercentOfSharpestEdge, value);
                mSurfaceNoiseThreshold_PercentOfSharpestEdge = value;
            }
        }

        private DataValueDefinition mSurfaceNoiseThreshold_PercentileBase;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The percentile of variation to consider noise.  First variation falling within this percentile is considered noise, then all nearby variation is also included until a significant gap is found.")]
        public DataValueDefinition SurfaceNoiseThreshold_PercentileBase
        {
            get { return mSurfaceNoiseThreshold_PercentileBase; }
            set
            {
                HandlePropertyChange(this, "SurfaceNoiseThreshold_PercentileBase", mSurfaceNoiseThreshold_PercentileBase, value);
                mSurfaceNoiseThreshold_PercentileBase = value;
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

        private EdgeCollectionDefinition mEdgeCollection = null;
        [CategoryAttribute("Output")]
        public EdgeCollectionDefinition EdgeCollection
        {
            get { return mEdgeCollection; }
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
*/
}

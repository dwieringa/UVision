// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NetCams
{
    public class ProximityCollectorWith2GridsScoreFilterDefinition : ScoreFilterDefinition
    {
        public ProximityCollectorWith2GridsScoreFilterDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            mMaxScore = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "MaxScore"));
            mMaxScore.Type = DataType.IntegerNumber;
            mMaxScore.AddDependency(this);
            mMaxScore.Name = "MaxScore";

            mNumScoresInMax = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "NumScoresInMax"));
            mNumScoresInMax.Type = DataType.IntegerNumber;
            mNumScoresInMax.AddDependency(this);
            mNumScoresInMax.Name = "NumScoresInMax";
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new ProximityCollectorWith2GridsScoreFilterInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mMinNumScoresRequired != null && mMinNumScoresRequired.IsDependentOn(theOtherObject)) return true;
            if (mMinScoreThreshold != null && mMinScoreThreshold.IsDependentOn(theOtherObject)) return true;
            if (mGridCellHeight != null && mGridCellHeight.IsDependentOn(theOtherObject)) return true;
            if (mGridCellWidth != null && mGridCellWidth.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mMinNumScoresRequired != null) result = Math.Max(result, mMinNumScoresRequired.ToolMapRow);
                if (mMinScoreThreshold != null) result = Math.Max(result, mMinScoreThreshold.ToolMapRow);
                if (mGridCellHeight != null) result = Math.Max(result, mGridCellHeight.ToolMapRow);
                if (mGridCellWidth != null) result = Math.Max(result, mGridCellWidth.ToolMapRow);
                return result + 1;
            }
        }

        private GeneratedValueDefinition mMaxScore = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition MaxScore
        {
            get { return mMaxScore; }
        }

        private GeneratedValueDefinition mNumScoresInMax = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition NumScoresInMax
        {
            get { return mNumScoresInMax; }
        }

        private DataValueDefinition mGridCellWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Grid cell width in pixels.")]
        public DataValueDefinition GridCellWidth
        {
            get { return mGridCellWidth; }
            set 
            {
                HandlePropertyChange(this, "GridCellWidth", mGridCellWidth, value);
                mGridCellWidth = value;
            }
        }

        private DataValueDefinition mGridCellHeight;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Grid cell height in pixels.")]
        public DataValueDefinition GridCellHeight
        {
            get { return mGridCellHeight; }
            set 
            {
                HandlePropertyChange(this, "GridCellHeight", mGridCellHeight, value);
                mGridCellHeight = value;
            }
        }

        private DataValueDefinition mMinNumScoresRequired;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The minimum number of scores within a grid cell before that cell's score can register as the MaxScore.")]
        public DataValueDefinition MinNumScoresRequired
        {
            get { return mMinNumScoresRequired; }
            set 
            {
                HandlePropertyChange(this, "MinNumScoresRequired", mMinNumScoresRequired, value);
                mMinNumScoresRequired = value;
            }
        }

        private DataValueDefinition mMinScoreThreshold;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("A score must exceed this value before it is used.")]
        public DataValueDefinition MinScoreThreshold
        {
            get { return mMinScoreThreshold; }
            set 
            {
                HandlePropertyChange(this, "MinScoreThreshold", mMinScoreThreshold, value);
                mMinScoreThreshold = value;
            }
        }

        private int mMaxDebugDetails = 100;
        [CategoryAttribute("Debug"),
        DescriptionAttribute("The maximum number of detail entries that will be placed in the test execution log.  Excessive entries can slow down test performance.")]
        public int MaxDebugDetails
        {
            get { return mMaxDebugDetails; }
            set
            {
                HandlePropertyChange(this, "MaxDebugDetails", mMaxDebugDetails, value);
                mMaxDebugDetails = value;
            }
        }

    }
}

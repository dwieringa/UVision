// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class JoinedROIDefinition : ROIDefinition
    {
        public JoinedROIDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            Name = "Untitled Joined ROI";
        }

        private ROIDefinition mFirstROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIDefinition FirstROI
        {
            get { return mFirstROI; }
            set 
            {
                HandlePropertyChange(this, "FirstROI", mFirstROI, value);
                mFirstROI = value;
            }
        }

        private ROIDefinition mSecondROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIDefinition SecondROI
        {
            get { return mSecondROI; }
            set 
            {
                HandlePropertyChange(this, "SecondROI", mSecondROI, value);
                mSecondROI = value;
            }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new JoinedROIInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            return (theOtherObject == this)
                || (mFirstROI != null && mFirstROI.IsDependentOn(theOtherObject))
                || (mSecondROI != null && mSecondROI.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject)
                ;
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mFirstROI != null) result = Math.Max(result, mFirstROI.ToolMapRow);
                if (mSecondROI != null) result = Math.Max(result, mSecondROI.ToolMapRow);
                return result + 1;
            }
        }
    }
}

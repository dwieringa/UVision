// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class ROIWithHoleDefinition : ROIDefinition
    {
        public ROIWithHoleDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            Name = "Untitled ROI with hole";
        }

        private ROIDefinition mMainROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIDefinition MainROI
        {
            get { return mMainROI; }
            set 
            {
                HandlePropertyChange(this, "MainROI", mMainROI, value);
                mMainROI = value;
            }
        }

        private ROIDefinition mHoleROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIDefinition HoleROI
        {
            get { return mHoleROI; }
            set 
            {
                HandlePropertyChange(this, "HoleROI", mHoleROI, value);
                mHoleROI = value;
            }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new ROIWithHoleInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            return (theOtherObject == this)
                || (mMainROI != null && mMainROI.IsDependentOn(theOtherObject))
                || (mHoleROI != null && mHoleROI.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject)
                ;
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mMainROI != null) result = Math.Max(result, mMainROI.ToolMapRow);
                if (mHoleROI != null) result = Math.Max(result, mHoleROI.ToolMapRow);
                return result + 1;
            }
        }
    }
}

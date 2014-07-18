// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class ROIWithColorExceptionDefinition : ROIDefinition
    {
        public ROIWithColorExceptionDefinition(TestSequence testSequence)
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

        private ColorMatchDefinition mColorException;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ColorMatchDefinition ColorException
        {
            get { return mColorException; }
            set 
            {
                HandlePropertyChange(this, "ColorException", mColorException, value);
                mColorException = value;
            }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new ROIWithColorExceptionInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            return (theOtherObject == this)
                || (mMainROI != null && mMainROI.IsDependentOn(theOtherObject))
                || (mColorException != null && mColorException.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject)
                ;
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mMainROI != null) result = Math.Max(result, mMainROI.ToolMapRow);
                if (mColorException != null) result = Math.Max(result, mColorException.ToolMapRow);
                return result + 1;
            }
        }
    }
}

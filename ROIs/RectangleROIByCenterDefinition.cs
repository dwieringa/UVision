// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class RectangleROIByCenterDefinition : RectangleROIBaseDefinition
    {
        private DataValueDefinition mCenter_Y;
        private DataValueDefinition mHeight;
        private DataValueDefinition mCenter_X;
        private DataValueDefinition mWidth;

        public RectangleROIByCenterDefinition(TestSequence testSequence)
            : base(testSequence)
        {
        }

        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition Center_X
        {
            get { return mCenter_X; }
            set
            {
                HandlePropertyChange(this, "Center_X", mCenter_X, value);
                mCenter_X = value;
            }
        }
        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition Width
        {
            get { return mWidth; }
            set
            {
                HandlePropertyChange(this, "Width", mWidth, value);
                mWidth = value;
            }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition Center_Y
        {
            get { return mCenter_Y; }
            set
            {
                HandlePropertyChange(this, "Center_Y", mCenter_Y, value);
                mCenter_Y = value;
            }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition Height
        {
            get { return mHeight; }
            set
            {
                HandlePropertyChange(this, "Height", mHeight, value);
                mHeight = value;
            }
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {// TODO: these will never be null since we set them in the constructor and never change them (only their contents)
            return (theOtherObject == this)
                || (mCenter_Y != null && mCenter_Y.IsDependentOn(theOtherObject))
                || (mHeight != null && mHeight.IsDependentOn(theOtherObject))
                || (mCenter_X != null && mCenter_X.IsDependentOn(theOtherObject))
                || (mWidth != null && mWidth.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject);
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new RectangleROIByCenterInstance(this, theExecution);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mCenter_Y != null) result = Math.Max(result, mCenter_Y.ToolMapRow);
                if (mHeight != null) result = Math.Max(result, mHeight.ToolMapRow);
                if (mCenter_X != null) result = Math.Max(result, mCenter_X.ToolMapRow);
                if (mWidth != null) result = Math.Max(result, mWidth.ToolMapRow);
                return result + 1;
            }
        }
    }
}

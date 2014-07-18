// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class RectangleROIDefinition : RectangleROIBaseDefinition
    {
        private DataValueDefinition mTop;
        private DataValueDefinition mBottom;
        private DataValueDefinition mLeft;
        private DataValueDefinition mRight;
        private DataValueDefinition mTopPadding;
        private DataValueDefinition mBottomPadding;
        private DataValueDefinition mLeftPadding;
        private DataValueDefinition mRightPadding;
        private DataValueDefinition mCenter_Y;
        private DataValueDefinition mHeight;
        private DataValueDefinition mCenter_X;
        private DataValueDefinition mWidth;

        public RectangleROIDefinition(TestSequence testSequence)
            : base(testSequence)
        {
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new RectangleROIInstance(this, theExecution);
        }

        [CategoryAttribute("Location, X-axis, By Edges"),
        DescriptionAttribute("")]
        public DataValueDefinition Left
        {
            get { return mLeft; }
            set
            {
                HandlePropertyChange(this, "Left", mLeft, value);
                mLeft = value;
                if (value != null)
                {
                    mCenter_X = null;
                    mWidth = null;
                }
            }
        }
        [CategoryAttribute("Location, X-axis, By Edges"),
        DescriptionAttribute("")]
        public DataValueDefinition Right
        {
            get { return mRight; }
            set
            {
                HandlePropertyChange(this, "Right", mRight, value);
                mRight = value;
                if (value != null)
                {
                    mCenter_X = null;
                    mWidth = null;
                }
            }
        }
        [CategoryAttribute("Location, X-axis, By Center"),
        DescriptionAttribute("")]
        public DataValueDefinition Center_X
        {
            get { return mCenter_X; }
            set
            {
                HandlePropertyChange(this, "Center_X", mCenter_X, value);
                mCenter_X = value;
                if (value != null)
                {
                    mLeft = null;
                    mRight = null;
                }
            }
        }
        [CategoryAttribute("Location, X-axis, By Center"),
        DescriptionAttribute("")]
        public DataValueDefinition Width
        {
            get { return mWidth; }
            set
            {
                HandlePropertyChange(this, "Width", mWidth, value);
                mWidth = value;
                if (value != null)
                {
                    mLeft = null;
                    mRight = null;
                }
            }
        }

        [CategoryAttribute("Location, Y-axis, By Edges"),
        DescriptionAttribute("")]
        public DataValueDefinition Top
        {
            get { return mTop; }
            set
            {
                HandlePropertyChange(this, "Top", mTop, value);
                mTop = value;
                if (value != null)
                {
                    mCenter_Y = null;
                    mHeight = null;
                }
            }
        }
        [CategoryAttribute("Location, Y-axis, By Edges"),
        DescriptionAttribute("")]
        public DataValueDefinition Bottom
        {
            get { return mBottom; }
            set
            {
                HandlePropertyChange(this, "Bottom", mBottom, value);
                mBottom = value;
                if (value != null)
                {
                    mCenter_Y = null;
                    mHeight = null;
                }
            }
        }
        [CategoryAttribute("Location, Y-axis, By Center"),
        DescriptionAttribute("")]
        public DataValueDefinition Center_Y
        {
            get { return mCenter_Y; }
            set
            {
                HandlePropertyChange(this, "Center_Y", mCenter_Y, value);
                mCenter_Y = value;
                if (value != null)
                {
                    Top = null;
                    Bottom = null;
                }
            }
        }
        [CategoryAttribute("Location, Y-axis, By Center"),
        DescriptionAttribute("")]
        public DataValueDefinition Height
        {
            get { return mHeight; }
            set
            {
                HandlePropertyChange(this, "Height", mHeight, value);
                mHeight = value;
                if (value != null)
                {
                    Top = null;
                    Bottom = null;
                }
            }
        }
        [CategoryAttribute("Location, X-axis, Padding"),
        DescriptionAttribute("")]
        public DataValueDefinition LeftPadding
        {
            get { return mLeftPadding; }
            set
            {
                HandlePropertyChange(this, "LeftPadding", mLeftPadding, value);
                mLeftPadding = value;
            }
        }
        [CategoryAttribute("Location, X-axis, Padding"),
        DescriptionAttribute("")]
        public DataValueDefinition RightPadding
        {
            get { return mRightPadding; }
            set
            {
                HandlePropertyChange(this, "RightPadding", mRightPadding, value);
                mRightPadding = value;
            }
        }
        [CategoryAttribute("Location, Y-axis, Padding"),
        DescriptionAttribute("")]
        public DataValueDefinition TopPadding
        {
            get { return mTopPadding; }
            set
            {
                HandlePropertyChange(this, "TopPadding", mTopPadding, value);
                mTopPadding = value;
            }
        }
        [CategoryAttribute("Location, Y-axis, Padding"),
        DescriptionAttribute("")]
        public DataValueDefinition BottomPadding
        {
            get { return mBottomPadding; }
            set
            {
                HandlePropertyChange(this, "BottomPadding", mBottomPadding, value);
                mBottomPadding = value;
            }
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {// TODO: these will never be null since we set them in the constructor and never change them (only their contents)
            return (theOtherObject == this)
                || (mTop != null && mTop.IsDependentOn(theOtherObject))
                || (mBottom != null && mBottom.IsDependentOn(theOtherObject))
                || (mLeft != null && mLeft.IsDependentOn(theOtherObject))
                || (mRight != null && mRight.IsDependentOn(theOtherObject))
                || (mCenter_Y != null && mCenter_Y.IsDependentOn(theOtherObject))
                || (mHeight != null && mHeight.IsDependentOn(theOtherObject))
                || (mCenter_X != null && mCenter_X.IsDependentOn(theOtherObject))
                || (mWidth != null && mWidth.IsDependentOn(theOtherObject))
                || (mTopPadding != null && mTopPadding.IsDependentOn(theOtherObject))
                || (mBottomPadding != null && mBottomPadding.IsDependentOn(theOtherObject))
                || (mLeftPadding != null && mLeftPadding.IsDependentOn(theOtherObject))
                || (mRightPadding != null && mRightPadding.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mTop != null) result = Math.Max(result, mTop.ToolMapRow);
                if (mBottom != null) result = Math.Max(result, mBottom.ToolMapRow);
                if (mLeft != null) result = Math.Max(result, mLeft.ToolMapRow);
                if (mRight != null) result = Math.Max(result, mRight.ToolMapRow);
                if (mCenter_Y != null) result = Math.Max(result, mCenter_Y.ToolMapRow);
                if (mHeight != null) result = Math.Max(result, mHeight.ToolMapRow);
                if (mCenter_X != null) result = Math.Max(result, mCenter_X.ToolMapRow);
                if (mWidth != null) result = Math.Max(result, mWidth.ToolMapRow);
                if (mTopPadding != null) result = Math.Max(result, mTopPadding.ToolMapRow);
                if (mBottomPadding != null) result = Math.Max(result, mBottomPadding.ToolMapRow);
                if (mLeftPadding != null) result = Math.Max(result, mLeftPadding.ToolMapRow);
                if (mRightPadding != null) result = Math.Max(result, mRightPadding.ToolMapRow);
                return result + 1;
            }
        }
    }
}

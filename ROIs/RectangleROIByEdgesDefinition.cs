// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class RectangleROIByEdgesDefinition : RectangleROIBaseDefinition
    {
        private DataValueDefinition top;
        private DataValueDefinition bottom;
        private DataValueDefinition left;
        private DataValueDefinition right;
        private DataValueDefinition topPadding;
        private DataValueDefinition bottomPadding;
        private DataValueDefinition leftPadding;
        private DataValueDefinition rightPadding;

        public RectangleROIByEdgesDefinition(TestSequence testSequence)
            : base(testSequence)
        {
        }

        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition Left
        {
            get { return left; }
            set
            {
                HandlePropertyChange(this, "Left", left, value);
                left = value;
            }
        }
        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition Right
        {
            get { return right; }
            set
            {
                HandlePropertyChange(this, "Right", right, value);
                right = value;
            }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition Top
        {
            get { return top; }
            set
            {
                HandlePropertyChange(this, "Top", top, value);
                top = value;
            }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition Bottom
        {
            get { return bottom; }
            set
            {
                HandlePropertyChange(this, "Bottom", bottom, value);
                bottom = value;
            }
        }
        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition LeftPadding
        {
            get { return leftPadding; }
            set
            {
                HandlePropertyChange(this, "LeftPadding", leftPadding, value);
                leftPadding = value;
            }
        }
        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition RightPadding
        {
            get { return rightPadding; }
            set
            {
                HandlePropertyChange(this, "RightPadding", rightPadding, value);
                rightPadding = value;
            }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition TopPadding
        {
            get { return topPadding; }
            set
            {
                HandlePropertyChange(this, "TopPadding", topPadding, value);
                topPadding = value;
            }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public DataValueDefinition BottomPadding
        {
            get { return bottomPadding; }
            set
            {
                HandlePropertyChange(this, "BottomPadding", bottomPadding, value);
                bottomPadding = value;
            }
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {// TODO: these will never be null since we set them in the constructor and never change them (only their contents)
            return (theOtherObject == this)
                || (top != null && top.IsDependentOn(theOtherObject))
                || (bottom != null && bottom.IsDependentOn(theOtherObject))
                || (left != null && left.IsDependentOn(theOtherObject))
                || (right != null && right.IsDependentOn(theOtherObject))
                || (topPadding != null && topPadding.IsDependentOn(theOtherObject))
                || (bottomPadding != null && bottomPadding.IsDependentOn(theOtherObject))
                || (leftPadding != null && leftPadding.IsDependentOn(theOtherObject))
                || (rightPadding != null && rightPadding.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject);
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new RectangleROIByEdgesInstance(this, theExecution);
        }

        public override int ToolMapRow
        {
            get
            {// TODO: top,bottom, etc will never be null since we set them in the constructor and never change them
                int result = base.ToolMapRow - 1;
                if (top != null) result = Math.Max(result, top.ToolMapRow);
                if (bottom != null) result = Math.Max(result, bottom.ToolMapRow);
                if (left != null) result = Math.Max(result, left.ToolMapRow);
                if (right != null) result = Math.Max(result, right.ToolMapRow);
                if (topPadding != null) result = Math.Max(result, topPadding.ToolMapRow);
                if (bottomPadding != null) result = Math.Max(result, bottomPadding.ToolMapRow);
                if (leftPadding != null) result = Math.Max(result, leftPadding.ToolMapRow);
                if (rightPadding != null) result = Math.Max(result, rightPadding.ToolMapRow);
                return result + 1;
            }
        }
    }
}

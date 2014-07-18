// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class RectangleROIByEdgesInstance : RectangleROIBaseInstance
    {
        private DataValueInstance top;
        private DataValueInstance bottom;
        private DataValueInstance left;
        private DataValueInstance right;
        private DataValueInstance topPadding;
        private DataValueInstance bottomPadding;
        private DataValueInstance leftPadding;
        private DataValueInstance rightPadding;

        public RectangleROIByEdgesInstance(RectangleROIByEdgesDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            if (theDefinition.Top == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Top");
            if (theDefinition.Bottom == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Bottom");
            if (theDefinition.Left == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Left");
            if (theDefinition.Right == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Right");
            top = theExecution.DataValueRegistry.GetObject(theDefinition.Top.Name);
            bottom = theExecution.DataValueRegistry.GetObject(theDefinition.Bottom.Name);
            left = theExecution.DataValueRegistry.GetObject(theDefinition.Left.Name);
            right = theExecution.DataValueRegistry.GetObject(theDefinition.Right.Name);

            if (theDefinition.TopPadding != null)
            {
                topPadding = theExecution.DataValueRegistry.GetObject(theDefinition.TopPadding.Name);
            }
            if (theDefinition.BottomPadding != null)
            {
                bottomPadding = theExecution.DataValueRegistry.GetObject(theDefinition.BottomPadding.Name);
            }
            if (theDefinition.LeftPadding != null)
            {
                leftPadding = theExecution.DataValueRegistry.GetObject(theDefinition.LeftPadding.Name);
            }
            if (theDefinition.RightPadding != null)
            {
                rightPadding = theExecution.DataValueRegistry.GetObject(theDefinition.RightPadding.Name);
            }
        }

        public override void DoWork()
        {
            mUpperLeftCorner = new Point((int)left.ValueAsLong(), (int)top.ValueAsLong()); // note: we have to do this here instead of in the ctor since the values may not be available immediately (i.e. top.IsCompelte())
            mLowerRightCorner = new Point((int)right.ValueAsLong(), (int)bottom.ValueAsLong());

            if (leftPadding != null)
            {
                mUpperLeftCorner.X -= (int)leftPadding.ValueAsLong();
            }
            if (rightPadding != null)
            {
                mLowerRightCorner.X += (int)rightPadding.ValueAsLong();
            }
            if (topPadding != null)
            {
                mUpperLeftCorner.Y -= (int)topPadding.ValueAsLong();
            }
            if (bottomPadding != null)
            {
                mLowerRightCorner.Y += (int)bottomPadding.ValueAsLong();
            }
            if (mReferencePoint_X != null)
            {
                mUpperLeftCorner.X += mReferencePoint_X.GetValueAsRoundedInt();
                mLowerRightCorner.X += mReferencePoint_X.GetValueAsRoundedInt();
            }
            if (mReferencePoint_Y != null)
            {
                mUpperLeftCorner.Y += mReferencePoint_Y.GetValueAsRoundedInt();
                mLowerRightCorner.Y += mReferencePoint_Y.GetValueAsRoundedInt();
            }
        }
    }
}

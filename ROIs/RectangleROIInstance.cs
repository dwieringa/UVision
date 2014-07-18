// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class RectangleROIInstance : RectangleROIBaseInstance
    {
        private DataValueInstance mTop;
        private DataValueInstance mBottom;
        private DataValueInstance mLeft;
        private DataValueInstance mRight;
        private DataValueInstance mTopPadding;
        private DataValueInstance mBottomPadding;
        private DataValueInstance mLeftPadding;
        private DataValueInstance mRightPadding;
        private DataValueInstance mCenter_Y;
        private DataValueInstance mHeight;
        private DataValueInstance mCenter_X;
        private DataValueInstance mWidth;

        public RectangleROIInstance(RectangleROIDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            // TODO: support Top+Height or Bottom+Height
            if (theDefinition.Top != null || theDefinition.Bottom != null)
            {
                if (theDefinition.Top == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Top");
                if (theDefinition.Bottom == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Bottom");
                mTop = theExecution.DataValueRegistry.GetObject(theDefinition.Top.Name);
                mBottom = theExecution.DataValueRegistry.GetObject(theDefinition.Bottom.Name);
            }
            else if (theDefinition.Center_Y != null || theDefinition.Height != null)
            {
                if (theDefinition.Center_Y == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Center_Y");
                if (theDefinition.Height == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Height");
                mCenter_Y = theExecution.DataValueRegistry.GetObject(theDefinition.Center_Y.Name);
                mHeight = theExecution.DataValueRegistry.GetObject(theDefinition.Height.Name);
            }
            else
            {
                throw new ArgumentException("ROI '" + theDefinition.Name + "' has no Y-axis location settings");
            }

            if (theDefinition.Left != null || theDefinition.Right != null)
            {
                if (theDefinition.Left == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Left");
                if (theDefinition.Right == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Right");
                mLeft = theExecution.DataValueRegistry.GetObject(theDefinition.Left.Name);
                mRight = theExecution.DataValueRegistry.GetObject(theDefinition.Right.Name);
            }
            else if (theDefinition.Center_X != null || theDefinition.Width != null)
            {
                if (theDefinition.Center_X == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Center_X");
                if (theDefinition.Width == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Width");
                mCenter_X = theExecution.DataValueRegistry.GetObject(theDefinition.Center_X.Name);
                mWidth = theExecution.DataValueRegistry.GetObject(theDefinition.Width.Name);
            }
            else
            {
                throw new ArgumentException("ROI '" + theDefinition.Name + "' has no X-axis location settings");
            }
            
            if (theDefinition.TopPadding != null)
            {
                mTopPadding = theExecution.DataValueRegistry.GetObject(theDefinition.TopPadding.Name);
            }
            if (theDefinition.BottomPadding != null)
            {
                mBottomPadding = theExecution.DataValueRegistry.GetObject(theDefinition.BottomPadding.Name);
            }
            if (theDefinition.LeftPadding != null)
            {
                mLeftPadding = theExecution.DataValueRegistry.GetObject(theDefinition.LeftPadding.Name);
            }
            if (theDefinition.RightPadding != null)
            {
                mRightPadding = theExecution.DataValueRegistry.GetObject(theDefinition.RightPadding.Name);
            }
        }

        public override void DoWork()
        {
            mUpperLeftCorner = new Point(-1,-1);
            mLowerRightCorner = new Point(-1,-1);
            if (mTop != null && mBottom != null)
            {
                mUpperLeftCorner.Y = (int)mTop.ValueAsLong(); // note: we have to do this here instead of in the ctor since the values may not be available immediately (i.e. top.IsCompelte())
                mLowerRightCorner.Y = (int)mBottom.ValueAsLong();
            }
            else if(mCenter_Y != null && mHeight != null)
            {
                mUpperLeftCorner.Y = (int)mCenter_Y.ValueAsLong() - (int)(mHeight.ValueAsLong()/2); // note: we have to do this here instead of in the ctor since the values may not be available immediately (i.e. top.IsCompelte())
                mLowerRightCorner.Y = mUpperLeftCorner.Y + (int)mHeight.ValueAsLong() - 1;
            }
            else
            {
                TestExecution().LogErrorWithTimeFromTrigger("Error computing Y-axis location of ROI " + Name);
                return;
            }

            if (mLeft != null && mRight != null)
            {
                mUpperLeftCorner.X = (int)mLeft.ValueAsLong();
                mLowerRightCorner.X = (int)mRight.ValueAsLong();
            }
            else if(mCenter_X != null && mWidth != null)
            {
                mUpperLeftCorner.X = (int)mCenter_X.ValueAsLong() - (int)(mWidth.ValueAsLong() / 2); // note: we have to do this here instead of in the ctor since the values may not be available immediately (i.e. top.IsCompelte())
                mLowerRightCorner.X = mUpperLeftCorner.X + (int)mWidth.ValueAsLong() - 1;
            }
            else
            {
                TestExecution().LogErrorWithTimeFromTrigger("Error computing X-axis location of ROI " + Name);
                return;
            }

            if (mLeftPadding != null)
            {
                mUpperLeftCorner.X -= (int)mLeftPadding.ValueAsLong();
            }
            if (mRightPadding != null)
            {
                mLowerRightCorner.X += (int)mRightPadding.ValueAsLong();
            }
            if (mTopPadding != null)
            {
                mUpperLeftCorner.Y -= (int)mTopPadding.ValueAsLong();
            }
            if (mBottomPadding != null)
            {
                mLowerRightCorner.Y += (int)mBottomPadding.ValueAsLong();
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

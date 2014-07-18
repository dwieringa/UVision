// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class RectangleROIByCenterInstance : RectangleROIBaseInstance
    {
        private DataValueInstance mCenter_Y;
        private DataValueInstance mHeight;
        private DataValueInstance mCenter_X;
        private DataValueInstance mWidth;

        public RectangleROIByCenterInstance(RectangleROIByCenterDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            if (theDefinition.Center_Y == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Center_Y");
            if (theDefinition.Height == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Height");
            if (theDefinition.Center_X == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Center_X");
            if (theDefinition.Width == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Width");
            mCenter_Y = theExecution.DataValueRegistry.GetObject(theDefinition.Center_Y.Name);
            mHeight = theExecution.DataValueRegistry.GetObject(theDefinition.Height.Name);
            mCenter_X = theExecution.DataValueRegistry.GetObject(theDefinition.Center_X.Name);
            mWidth = theExecution.DataValueRegistry.GetObject(theDefinition.Width.Name);
        }

        public override void DoWork()
        {
            mUpperLeftCorner = new Point((int)mCenter_X.ValueAsLong() - (int)(mWidth.ValueAsLong()/2), (int)mCenter_Y.ValueAsLong() + (int)(mHeight.ValueAsLong()/2)); // note: we have to do this here instead of in the ctor since the values may not be available immediately (i.e. top.IsCompelte())
            mLowerRightCorner = new Point(mUpperLeftCorner.X + (int)mWidth.ValueAsLong() - 1, mUpperLeftCorner.Y - (int)mHeight.ValueAsLong() - 1);

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

        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public int Center_X
        {
            get { return (int)mCenter_X.ValueAsLong(); }
        }
        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public int Width
        {
            get { return (int)mWidth.ValueAsLong(); }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public int Center_Y
        {
            get { return (int)mCenter_Y.ValueAsLong(); }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public int Height
        {
            get { return (int)mHeight.ValueAsLong(); }
        }
    }
}

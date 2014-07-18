// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class ValueBasedLineDecorationInstance : LineDecorationInstance
    {
        public ValueBasedLineDecorationInstance(ValueBasedLineDecorationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
        }

        public void SetStartX(int value)
        {
            mStartX = value;
        }

        public void SetStartY(int value)
        {
            mStartY = value;
        }

        public void SetEndX(int value)
        {
            mEndX = value;
        }

        public void SetEndY(int value)
        {
            mEndY = value;
        }

        public void SetIsComplete()
        {
            SetCompletedTime();
            mIsComplete = true;
        }

        private int mStartX;
        [CategoryAttribute("Definition")]
        public override int StartX_int
        {
            get { return mStartX; }
        }

        private int mStartY;
        [CategoryAttribute("Definition")]
        public override int StartY_int
        {
            get { return mStartY; }
        }

        private int mEndX;
        [CategoryAttribute("Definition")]
        public override int EndX_int
        {
            get { return mEndX; }
        }

        private int mEndY;
        [CategoryAttribute("Definition")]
        public override int EndY_int
        {
            get { return mEndY; }
        }

        public override void Draw(Graphics g, PictureBoxScale scale)
        {
            if (IsComplete())
            {
                Pen pen = new Pen(mColor);
                g.DrawLine(pen, (mStartX * scale.XScale) + scale.XOffset, (mStartY * scale.YScale) + scale.YOffset, (mEndX * scale.XScale) + scale.XOffset, (mEndY * scale.YScale) + scale.YOffset);
                pen.Dispose();
            }
        }

        private bool mIsComplete = false;
        public override bool IsComplete() { return mIsComplete; }    }
}

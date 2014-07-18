// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class ObjectBasedLineDecorationInstance : LineDecorationInstance
    {
        public ObjectBasedLineDecorationInstance(ObjectBasedLineDecorationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.StartX() == null) throw new ArgumentException("LineDecoration " + Name + " doesn't have anything assigned to StartX");
            mStartX = testExecution.DataValueRegistry.GetObject(theDefinition.StartX().Name);

            if (theDefinition.StartY() == null) throw new ArgumentException("LineDecoration " + Name + " doesn't have anything assigned to StartY");
            mStartY = testExecution.DataValueRegistry.GetObject(theDefinition.StartY().Name);

            if (theDefinition.EndX() == null) throw new ArgumentException("LineDecoration " + Name + " doesn't have anything assigned to EndX");
            mEndX = testExecution.DataValueRegistry.GetObject(theDefinition.EndX().Name);

            if (theDefinition.EndY() == null) throw new ArgumentException("LineDecoration " + Name + " doesn't have anything assigned to EndY");
            mEndY = testExecution.DataValueRegistry.GetObject(theDefinition.EndY().Name);

            testExecution.ObjectBasedLineDecorationRegistry.RegisterObject(this);
        }

        private DataValueInstance mStartX;
        [CategoryAttribute("Definition")]
        public DataValueInstance StartX
        {
            get { return mStartX; }
        }

        private DataValueInstance mStartY;
        [CategoryAttribute("Definition")]
        public DataValueInstance StartY
        {
            get { return mStartY; }
        }

        private DataValueInstance mEndX;
        [CategoryAttribute("Definition")]
        public DataValueInstance EndX
        {
            get { return mEndX; }
        }

        private DataValueInstance mEndY;
        [CategoryAttribute("Definition")]
        public DataValueInstance EndY
        {
            get { return mEndY; }
        }

        [CategoryAttribute("Definition")]
        public override int StartX_int
        {
            get { return (int)mStartX.ValueAsLong(); }
        }

        [CategoryAttribute("Definition")]
        public override int StartY_int
        {
            get { return (int)mStartY.ValueAsLong(); }
        }

        [CategoryAttribute("Definition")]
        public override int EndX_int
        {
            get { return (int)mEndX.ValueAsLong(); }
        }

        [CategoryAttribute("Definition")]
        public override int EndY_int
        {
            get { return (int)mEndY.ValueAsLong(); }
        }

        public override void Draw(Graphics g, PictureBoxScale scale)
        {
            if (IsComplete())
            {
                Pen pen = new Pen(mColor);
                g.DrawLine(pen, (mStartX.ValueAsLong() * scale.XScale) + scale.XOffset, (mStartY.ValueAsLong() * scale.YScale) + scale.YOffset, (mEndX.ValueAsLong() * scale.XScale) + scale.XOffset, (mEndY.ValueAsLong() * scale.YScale) + scale.YOffset);
                pen.Dispose();
            }
        }

        public override bool IsComplete() { return mStartX.IsComplete() && mStartY.IsComplete() && mEndX.IsComplete() && mEndY.IsComplete(); }
    }
}

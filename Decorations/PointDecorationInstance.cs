// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    class PointDecorationInstance : DecorationInstance
    {
        public PointDecorationInstance(PointDecorationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mX = testExecution.DataValueRegistry.GetObject(theDefinition.GetX().Name);
            mY = testExecution.DataValueRegistry.GetObject(theDefinition.GetY().Name);
            mColor = theDefinition.Color();
        }

        private DataValueInstance mX;
        [CategoryAttribute("Definition")]
        public DataValueInstance X
        {
            get { return mX; }
        }

        private DataValueInstance mY;
        [CategoryAttribute("Definition")]
        public DataValueInstance Y
        {
            get { return mY; }
        }

        private Color mColor;
        [CategoryAttribute("Definition")]
        public Color Color
        {
            get { return mColor; }
        }

        public override void Draw(Graphics g, PictureBoxScale scale)
        {
            if (IsComplete())
            {
                Pen pen = new Pen(mColor);
                g.DrawLine(pen, (mX.ValueAsLong() * scale.XScale) + scale.XOffset - 3, (mY.ValueAsLong() * scale.YScale) + scale.YOffset + 3, (mX.ValueAsLong() * scale.XScale) + scale.XOffset + 3, (mY.ValueAsLong() * scale.YScale) + scale.YOffset - 3);
                g.DrawLine(pen, (mX.ValueAsLong() * scale.XScale) + scale.XOffset - 3, (mY.ValueAsLong() * scale.YScale) + scale.YOffset - 3, (mX.ValueAsLong() * scale.XScale) + scale.XOffset + 3, (mY.ValueAsLong() * scale.YScale) + scale.YOffset + 3);
                pen.Dispose();
            }
        }

        public override bool IsComplete() { return mX.IsComplete() && mY.IsComplete(); }
    }
}

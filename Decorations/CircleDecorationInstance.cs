// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class CircleDecorationInstance : DecorationInstance
    {
        public CircleDecorationInstance(CircleDecorationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.CenterX() == null) throw new ArgumentException("CircleDecoration " + Name + " doesn't have anything assigned to CenterX");
            mCenterX = testExecution.DataValueRegistry.GetObject(theDefinition.CenterX().Name);

            if (theDefinition.CenterY() == null) throw new ArgumentException("CircleDecoration " + Name + " doesn't have anything assigned to CenterY");
            mCenterY = testExecution.DataValueRegistry.GetObject(theDefinition.CenterY().Name);

            if (theDefinition.Radius() == null) throw new ArgumentException("CircleDecoration " + Name + " doesn't have anything assigned to Radius");
            mRadius = testExecution.DataValueRegistry.GetObject(theDefinition.Radius().Name);

            mColor = theDefinition.Color;

            testExecution.CircleDecorationRegistry.RegisterObject(this);
        }

        private DataValueInstance mCenterX;
        [CategoryAttribute("Definition")]
        public DataValueInstance CenterX
        {
            get { return mCenterX; }
        }

        private DataValueInstance mCenterY;
        [CategoryAttribute("Definition")]
        public DataValueInstance CenterY
        {
            get { return mCenterY; }
        }

        private DataValueInstance mRadius;
        [CategoryAttribute("Definition")]
        public DataValueInstance Radius
        {
            get { return mRadius; }
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
                long radius = Radius.ValueAsLong();
                g.DrawEllipse(pen, ((CenterX.ValueAsLong() - radius) * scale.XScale) + scale.XOffset, ((CenterY.ValueAsLong() - radius) * scale.YScale) + scale.YOffset, 2 * radius * scale.XScale, 2 * radius * scale.YScale);
                pen.Dispose();
            }
        }

        public override bool IsComplete() { return mCenterX.IsComplete() && mCenterY.IsComplete() && mRadius.IsComplete(); }
    }
}

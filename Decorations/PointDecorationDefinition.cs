// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    class PointDecorationDefinition : DecorationDefinition
    {
        public PointDecorationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new PointDecorationInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mX.IsDependentOn(theOtherObject)) return true;
            if (mY.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mX != null) result = Math.Max(result, mX.ToolMapRow);
                if (mY != null) result = Math.Max(result, mY.ToolMapRow);
                return result + 1;
            }
        }

        public DataValueDefinition GetX() { return mX; }
        public DataValueDefinition GetY() { return mY; }
        public Color Color() { return mColor; }

        protected DataValueDefinition mX;
        protected DataValueDefinition mY;
        protected Color mColor = System.Drawing.Color.Magenta;

    }
}

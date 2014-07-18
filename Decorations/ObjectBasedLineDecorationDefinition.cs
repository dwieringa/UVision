// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class ObjectBasedLineDecorationDefinition : LineDecorationDefinition
    {
        public ObjectBasedLineDecorationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new ObjectBasedLineDecorationInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mStartX.IsDependentOn(theOtherObject)) return true;
            if (mStartY.IsDependentOn(theOtherObject)) return true;
            if (mEndX.IsDependentOn(theOtherObject)) return true;
            if (mEndY.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mStartX != null) result = Math.Max(result, mStartX.ToolMapRow);
                if (mStartY != null) result = Math.Max(result, mStartY.ToolMapRow);
                if (mEndX != null) result = Math.Max(result, mEndX.ToolMapRow);
                if (mEndY != null) result = Math.Max(result, mEndY.ToolMapRow);
                return result + 1;
            }
        }

        public DataValueDefinition StartX() { return mStartX; }
        public DataValueDefinition StartY() { return mStartY; }
        public DataValueDefinition EndX() { return mEndX; }
        public DataValueDefinition EndY() { return mEndY; }

        protected DataValueDefinition mStartX;
        protected DataValueDefinition mStartY;
        protected DataValueDefinition mEndX;
        protected DataValueDefinition mEndY;
    }
}

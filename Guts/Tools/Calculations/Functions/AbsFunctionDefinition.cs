// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class AbsFunctionDefinition : FunctionDefinition
    {
        public AbsFunctionDefinition(TestSequence testSequence, string calcDef, List<DataValueDefinition> valueObjects)
            : base(testSequence, calcDef, valueObjects)
        {
            if (valueObjects.Count > 1) throw new ArgumentException("Abs() function can only take 1 argument");
            if (valueObjects.Count < 1) throw new ArgumentException("Abs() function needs exactly 1 argument");
            mArgument = valueObjects[0];
        }

        private DataValueDefinition mArgument;
        public DataValueDefinition Argument
        {
            get { return mArgument; }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new AbsFunctionInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mArgument == theOtherObject) return true; // this should be unnecessary if all objects return true if theOtherObject == this
            if (mArgument.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mArgument != null) result = Math.Max(result, mArgument.ToolMapRow);
                return result + 1;
            }
        }
    }
}

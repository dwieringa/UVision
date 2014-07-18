// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public abstract class MultiParameterFunctionDefinition : FunctionDefinition
    {
        // TODO: make the root-level FunctionDefinition/Instance classes into interfaces or at least split multi-parameter handling out into a subclass; e.g. see AbsFunctionDefinition

        public MultiParameterFunctionDefinition(TestSequence testSequence, string calcDef, List<DataValueDefinition> valueObjects)
            : base(testSequence, calcDef, valueObjects)
        {
            // NOTE: we're taking ownership of the Lists created in MathOperationCreator_ForCommon2PlusValueOperation.CreateMathOperation.  There they are created, passed in here and abandoned
            ValueObjects = valueObjects;
        }

        public List<DataValueDefinition> ValueObjects = new List<DataValueDefinition>();

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            foreach (DataValueDefinition num in ValueObjects)
            {
                if (num == theOtherObject) return true; // this should be unnecessary if all objects return true if theOtherObject == this
                if (num.IsDependentOn(theOtherObject)) return true;
            }
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                foreach (DataValueDefinition num in ValueObjects)
                {
                    result = Math.Max(result, num.ToolMapRow);
                }
                return result + 1;
            }
        }
    }
}

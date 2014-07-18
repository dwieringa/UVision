// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class MultiplicationOperationDefinition : BasicMathOperationDefinition
    {
        public MultiplicationOperationDefinition(TestSequence testSequence, string calcDef, List<DataValueDefinition> valueObjects, List<string> operatorsUsed)
            : base(testSequence, calcDef, valueObjects, operatorsUsed)
        {
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new MultiplicationOperationInstance(this, theExecution);
        }
    }
}

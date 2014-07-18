// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class SumOperationCreator : MathOperationCreator_ForCommon2PlusValueOperation
    {
        public static SumOperationCreator Singleton = new SumOperationCreator();

        public SumOperationCreator()
            : base(new string[] { "+", "-" })
        {
        }

        public override int Precedence()
        {
            return 7; //http://www.cplusplus.com/doc/tutorial/operators.html
        }

        protected override MathOperationDefinition CreateMathOperation(TestSequence testSequence, string calcDef_expanded, List<DataValueDefinition> valueObjects, List<string> operatorsUsed)
        {
            return new SumOperationDefinition(testSequence, calcDef_expanded, valueObjects, operatorsUsed);
        }
    }
}

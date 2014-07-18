// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class LogicalOrOperationCreator : MathOperationCreator_ForCommon2PlusValueOperation
    {
        public static LogicalOrOperationCreator Singleton = new LogicalOrOperationCreator();

        public LogicalOrOperationCreator()
            : base(new string[] { " OR ", "||", " or ", " Or " })
        {
        }

        public override int Precedence()
        {
            return 15; //http://www.cplusplus.com/doc/tutorial/operators.html
        }

        protected override MathOperationDefinition CreateMathOperation(TestSequence testSequence, string calcDef_expanded, List<DataValueDefinition> valueObjects, List<string> operatorsUsed)
        {
            return new LogicalOrOperationDefinition(testSequence, calcDef_expanded, valueObjects, operatorsUsed);
        }
    }
}

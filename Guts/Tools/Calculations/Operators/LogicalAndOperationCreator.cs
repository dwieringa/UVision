// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class LogicalAndOperationCreator : MathOperationCreator_ForCommon2PlusValueOperation
    {
        public static LogicalAndOperationCreator Singleton = new LogicalAndOperationCreator();

        public LogicalAndOperationCreator()
            : base(new string[] { " AND ", "&&", " and ", " And " })
        {
        }

        public override int Precedence()
        {
            return 14; //http://www.cplusplus.com/doc/tutorial/operators.html
        }

        protected override MathOperationDefinition CreateMathOperation(TestSequence testSequence, string calcDef_expanded, List<DataValueDefinition> valueObjects, List<string> operatorsUsed)
        {
            return new LogicalAndOperationDefinition(testSequence, calcDef_expanded, valueObjects, operatorsUsed);
        }
    }
}

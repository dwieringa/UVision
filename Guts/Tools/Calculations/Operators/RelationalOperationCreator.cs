// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class RelationalOperationCreator : MathOperationCreator_ForCommon2PlusValueOperation
    {
        public static RelationalOperationCreator Singleton = new RelationalOperationCreator();

        public RelationalOperationCreator()
            : base(new string[] { "<=", "<", ">=", ">" }) // // TODO: NOTE: HACK: currently we need to list the longer operators first, since our parsing logic quits on the first match, so if we list "<" in the array before "<=", then "<=" operators will always be mistakenly identified as "<"...do we want to force the operator to put white space before/after each operator?
        {
        }

        public override int Precedence()
        {
            return 9; // NOTE: according to http://www.cplusplus.com/doc/tutorial/operators.html relational is 9 and equality it 10
        }

        protected override MathOperationDefinition CreateMathOperation(TestSequence testSequence, string calcDef_expanded, List<DataValueDefinition> valueObjects, List<string> operatorsUsed)
        {
            return new RelationalOperationDefinition(testSequence, calcDef_expanded, valueObjects, operatorsUsed);
        }
    }
}

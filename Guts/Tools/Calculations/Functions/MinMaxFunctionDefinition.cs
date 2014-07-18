// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class MinMaxFunctionDefinition : MultiParameterFunctionDefinition
    {
        public enum Function
        {
            Min,
            Max
        }

        public MinMaxFunctionDefinition(TestSequence testSequence, string calcDef, Function theFunc, List<DataValueDefinition> valueObjects)
            : base(testSequence, calcDef, valueObjects)
        {
            mFunction = theFunc;
        }

        private Function mFunction;
        public Function FunctionSelection
        {
            get { return mFunction; }
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new MinMaxFunctionInstance(this, theExecution);
        }
    }
}

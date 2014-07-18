// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public abstract class FunctionDefinition : MathOperationDefinition
    {

        public FunctionDefinition(TestSequence testSequence, string calcDef, List<DataValueDefinition> valueObjects)
            : base(testSequence, calcDef)
        {
        }

    }
}

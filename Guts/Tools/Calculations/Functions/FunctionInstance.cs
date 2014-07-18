// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public abstract class FunctionInstance : MathOperationInstance
    {
        public FunctionInstance(FunctionDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
        }

    }
}

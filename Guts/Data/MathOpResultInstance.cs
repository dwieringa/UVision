// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class MathOpResultInstance : GeneratedValueInstance
    {
        public MathOpResultInstance(MathOpResultDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
//            SetType(testExecution.GetMathOperation(theDefinition.MathOperation().Name).Type);
		}

        public void SetType(DataType theType)
        {
            mDataType = theType;
        }

//        public override bool OkToChangeInstanceValue() { return false; }
    }
}

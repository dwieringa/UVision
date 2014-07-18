// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Text;


namespace NetCams
{
    public abstract class MathOperationInstance : ToolInstance
    {
        public MathOperationInstance(MathOperationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            theDefinition.Result.Type = DetermineDataTypeOfResult(theDefinition, testExecution);
            mResult = new MathOpResultInstance(theDefinition.Result, testExecution);
        }

        private MathOpResultInstance mResult = null;
        [CategoryAttribute("Output")]
        public MathOpResultInstance Result
        {
            get { return mResult; }
        }

        public abstract DataType DetermineDataTypeOfResult(MathOperationDefinition theDefinition, TestExecution testExecution);

        protected DataType mDataType = DataType.NotDefined;
        public DataType Type
        {
            get { return mDataType; }
        }

        public override bool IsComplete()
        {
            return Result.IsComplete();
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class CalculatedValueInstance : DataValueInstance
    {
        public CalculatedValueInstance(CalculatedValueDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            mDataValueHolder = testExecution.DataValueRegistry.GetObject(theDefinition.CalculationTool().RootOperation.Result.Name);
        }

        private DataValueInstance mDataValueHolder;

        public override string Value
        {
            get { return mDataValueHolder.Value; }
        }

        public override DataType Type
        {
            get { return mDataValueHolder.Type; }
        }

        public override long ValueAsLong()
        {
            return mDataValueHolder.ValueAsLong();
        }

        public override double ValueAsDecimal()
        {
            return mDataValueHolder.ValueAsDecimal();
        }

        public override bool ValueAsBoolean()
        {
            return mDataValueHolder.ValueAsBoolean();
        }

        public override bool IsComplete()
        {
            return mDataValueHolder.IsComplete();
        }
    }
}

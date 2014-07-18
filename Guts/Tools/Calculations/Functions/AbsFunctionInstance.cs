// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public class AbsFunctionInstance : FunctionInstance
    {
        public AbsFunctionInstance(AbsFunctionDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
        }

        public override DataType DetermineDataTypeOfResult(MathOperationDefinition theDefinition, TestExecution testExecution)
        {
            AbsFunctionDefinition theMoreSpecificDef = (AbsFunctionDefinition)theDefinition;
            mArgument = testExecution.DataValueRegistry.GetObject(theMoreSpecificDef.Argument.Name);

            return theMoreSpecificDef.Argument.Type;
        }

        private DataValueInstance mArgument;

        public override void DoWork()
        {
            TestExecution().LogMessageWithTimeFromTrigger("Function " + Name + " started");

            switch (Result.Type)
            {
                case DataType.DecimalNumber:
                    double resultAsDecimal = Math.Abs(mArgument.ValueAsDecimal());
                    Result.SetValue(resultAsDecimal);
                    break;

                case DataType.IntegerNumber:
                    long resultAsLong = Math.Abs(mArgument.ValueAsLong());
                    Result.SetValue(resultAsLong);
                    break;

                case DataType.Boolean:
                    resultAsLong = Math.Abs((mArgument.ValueAsBoolean() ? 1 : 0)); // ??? NEEDED?
                    Result.SetValue(resultAsLong);
                    break;

                default:
                    throw new ArgumentException("Data type " + Result.Type + " not supported by Abs function.");
            }
            Result.SetIsComplete();

            TestExecution().LogMessageWithTimeFromTrigger("Function " + Name + " completed");
        }
    }
}

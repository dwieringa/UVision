// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public class MinMaxFunctionInstance : MultiParameterFunctionInstance
    {
        public MinMaxFunctionInstance(MinMaxFunctionDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            mFunction = theDefinition.FunctionSelection;
        }

        private MinMaxFunctionDefinition.Function mFunction;

        public override DataType DetermineDataTypeOfResult(MathOperationDefinition theDefinition, TestExecution testExecution)
        {
            MinMaxFunctionDefinition theSpecificDefinition = (MinMaxFunctionDefinition)theDefinition;
            foreach( DataValueDefinition valueDef in theSpecificDefinition.ValueObjects)
            {
                ValueObjects.Add(testExecution.DataValueRegistry.GetObject(valueDef.Name));
            }

            return base.DetermineDataTypeOfResult(theDefinition, testExecution);
        }

        public override void DoWork()
        {
            TestExecution().LogMessageWithTimeFromTrigger("Function " + Name + " started");

            switch (mFunction)
            {
                case MinMaxFunctionDefinition.Function.Min:
                    switch (Result.Type)
                    {
                        case DataType.DecimalNumber:
                            double resultAsDecimal = Double.MaxValue;
                            double doubleValue;
                            foreach (DataValueInstance val in ValueObjects)
                            {
                                doubleValue = val.ValueAsDecimal();
                                if (doubleValue < resultAsDecimal)
                                {
                                    resultAsDecimal = doubleValue;
                                }
                            }
                            Result.SetValue(resultAsDecimal);
                            break;

                        case DataType.IntegerNumber:
                            long resultAsLong = long.MaxValue;
                            long longValue;
                            foreach (DataValueInstance val in ValueObjects)
                            {
                                longValue = val.ValueAsLong();
                                if (longValue < resultAsLong)
                                {
                                    resultAsLong = longValue;
                                }
                            }
                            Result.SetValue(resultAsLong);
                            break;

                        case DataType.Boolean:
                            resultAsLong = long.MaxValue;
                            long boolValueAsLong;
                            foreach (DataValueInstance val in ValueObjects)
                            {
                                boolValueAsLong = (val.ValueAsBoolean() ? 1 : 0);
                                if (boolValueAsLong < resultAsLong)
                                {
                                    resultAsLong = boolValueAsLong;
                                }
                            }
                            Result.SetValue(resultAsLong);
                            break;

                        default:
                            throw new ArgumentException("Data type " + Result.Type + " not supported by Min function.");
                    }
                    break;
                case MinMaxFunctionDefinition.Function.Max:
                    switch (Result.Type)
                    {
                        case DataType.DecimalNumber:
                            double resultAsDecimal = Double.MinValue;
                            double doubleValue;
                            foreach (DataValueInstance val in ValueObjects)
                            {
                                doubleValue = val.ValueAsDecimal();
                                if (doubleValue > resultAsDecimal)
                                {
                                    resultAsDecimal = doubleValue;
                                }
                            }
                            Result.SetValue(resultAsDecimal);
                            break;

                        case DataType.IntegerNumber:
                            long resultAsLong = long.MinValue;
                            long longValue;
                            foreach (DataValueInstance val in ValueObjects)
                            {
                                longValue = val.ValueAsLong();
                                if (longValue > resultAsLong)
                                {
                                    resultAsLong = longValue;
                                }
                            }
                            Result.SetValue(resultAsLong);
                            break;

                        case DataType.Boolean:
                            resultAsLong = long.MinValue;
                            long boolValueAsLong;
                            foreach (DataValueInstance val in ValueObjects)
                            {
                                boolValueAsLong = (val.ValueAsBoolean() ? 1 : 0);
                                if (boolValueAsLong > resultAsLong)
                                {
                                    resultAsLong = boolValueAsLong;
                                }
                            }
                            Result.SetValue(resultAsLong);
                            break;

                        default:
                            throw new ArgumentException("Data type " + Result.Type + " not supported by Max function.");
                    }
                    break;
                default:
                    throw new ArgumentException("Problem in MinMaxFunction code=19287.");
                    break;
            }
            Result.SetIsComplete();

            TestExecution().LogMessageWithTimeFromTrigger("Function " + Name + " completed");
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections;
using System.Text;


namespace NetCams
{
    public class SumOperationInstance : BasicMathOperationInstance
    {
        public SumOperationInstance(SumOperationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
        }

        public override void DoWork()
        {
            TestExecution().LogMessageWithTimeFromTrigger("MathOp " + Name + " started");

            // NOTE: Here we are casting every value to the same type (decimal, integer, bool) as the result before the calculations.  This may be ok, since we currently set the type of the result to the "greatest" type in the ctor (e.g. if any decimal value in calc then result is decimal...only bool if all values are bool)
            switch (Result.Type)
            {
                case DataType.DecimalNumber:
                    double doubleValue = ValueObjects[0].ValueAsDecimal();
                    for (int x = 0; x < OperatorsUsed.Count; x++)
                    {
                        switch (OperatorsUsed[x])
                        {
                            case "+":
                                doubleValue += ValueObjects[x + 1].ValueAsDecimal();
                                break;
                            case "-":
                                doubleValue -= ValueObjects[x + 1].ValueAsDecimal();
                                break;
                            default:
                                throw new ArgumentException("Invalid operator in SumOpInstance; op='" + OperatorsUsed[x] + "'");
                                break;
                        }
                    }
                    Result.SetValue(doubleValue);
                    break;

                case DataType.IntegerNumber:
                    long longValue = ValueObjects[0].ValueAsLong();
                    for (int x = 0; x < OperatorsUsed.Count; x++)
                    {
                        switch (OperatorsUsed[x])
                        {
                            case "+":
                                longValue += ValueObjects[x + 1].ValueAsLong();
                                break;
                            case "-":
                                longValue -= ValueObjects[x + 1].ValueAsLong();
                                break;
                            default:
                                throw new ArgumentException("Invalid operator in SumOpInstance; op='" + OperatorsUsed[x] + "'");
                                break;
                        }
                    }
                    Result.SetValue(longValue);
                    break;

                case DataType.Boolean:
                    long boolValueAsLong = (ValueObjects[0].ValueAsBoolean() ? 1: 0);
                    for (int x = 0; x < OperatorsUsed.Count; x++)
                    {
                        switch (OperatorsUsed[x])
                        {
                            case "+":
                                boolValueAsLong += (ValueObjects[x + 1].ValueAsBoolean() ? 1: 0);
                                break;
                            case "-":
                                boolValueAsLong -= (ValueObjects[x + 1].ValueAsBoolean() ? 1: 0);
                                break;
                            default:
                                throw new ArgumentException("Invalid operator in SumOpInstance; op='" + OperatorsUsed[x] + "'");
                                break;
                        }
                    }
                    Result.SetValue(boolValueAsLong);
                    break;

                default:
                    throw new ArgumentException("Data type " + Result.Type + " not supported by sum operation.");
            }
            Result.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("MathOp " + Name + " completed");
        }

    }
}

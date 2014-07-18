// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public class EqualityOperationInstance : BasicMathOperationInstance
    {
        public EqualityOperationInstance(EqualityOperationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
        }

        public override DataType DetermineDataTypeOfResult(MathOperationDefinition theDefinition, TestExecution testExecution)
        {
            EqualityOperationDefinition theSpecificDefinition = (EqualityOperationDefinition)theDefinition;
            foreach( DataValueDefinition valueDef in theSpecificDefinition.ValueObjects)
            {
                ValueObjects.Add(testExecution.DataValueRegistry.GetObject(valueDef.Name));
            }

            return DataType.Boolean;
        }

        public override void DoWork()
        {
            TestExecution().LogMessageWithTimeFromTrigger("MathOp " + Name + " started");

            bool result = true;
            for (int x = 0; x < OperatorsUsed.Count; x++)
            {
                if (ValueObjects[x].Type == DataType.DecimalNumber || ValueObjects[x + 1].Type == DataType.DecimalNumber)
                {
                    // only test using the decimal representation of the values if we need to, since decimal values and equality tests don't jive well together
                    switch (OperatorsUsed[x])
                    {
                        case "==":
                            result = result && (ValueObjects[x].ValueAsDecimal() == ValueObjects[x + 1].ValueAsDecimal());
                            break;
                        case "!=":
                            result = result && (ValueObjects[x].ValueAsDecimal() != ValueObjects[x + 1].ValueAsDecimal());
                            break;
                        default:
                            throw new ArgumentException("Invalid operator in EqualityOpInstance; op='" + OperatorsUsed[x] + "'");
                            break;
                    }
                }
                else
                {
                    switch (OperatorsUsed[x])
                    {
                        case "==":
                            result = result && (ValueObjects[x].ValueAsLong() == ValueObjects[x + 1].ValueAsLong());
                            break;
                        case "!=":
                            result = result && (ValueObjects[x].ValueAsLong() != ValueObjects[x + 1].ValueAsLong());
                            break;
                        default:
                            throw new ArgumentException("Invalid operator in EqualityOpInstance; op='" + OperatorsUsed[x] + "'");
                            break;
                    }
                }
            }
            Result.SetValue(result);

            Result.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("MathOp " + Name + " completed");
        }
    }
}

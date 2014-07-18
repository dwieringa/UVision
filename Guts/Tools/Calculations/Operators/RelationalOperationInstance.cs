// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public class RelationalOperationInstance : BasicMathOperationInstance
    {
        public RelationalOperationInstance(RelationalOperationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
        }

        public override DataType DetermineDataTypeOfResult(MathOperationDefinition theDefinition, TestExecution testExecution)
        {
            RelationalOperationDefinition theSpecificDefinition = (RelationalOperationDefinition)theDefinition;
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
                switch (OperatorsUsed[x])
                {
                    case "<":
                        result = result && (ValueObjects[x].ValueAsDecimal() < ValueObjects[x + 1].ValueAsDecimal());
                        break;
                    case "<=":
                        result = result && (ValueObjects[x].ValueAsDecimal() <= ValueObjects[x + 1].ValueAsDecimal());
                        break;
                    case ">":
                        result = result && (ValueObjects[x].ValueAsDecimal() > ValueObjects[x + 1].ValueAsDecimal());
                        break;
                    case ">=":
                        result = result && (ValueObjects[x].ValueAsDecimal() >= ValueObjects[x + 1].ValueAsDecimal());
                        break;
                    default:
                        throw new ArgumentException("Invalid operator in RelationalOpInstance; op='" + OperatorsUsed[x] + "'");
                        break;
                }
            }
            Result.SetValue(result);

            Result.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("MathOp " + Name + " completed");
        }
    }
}

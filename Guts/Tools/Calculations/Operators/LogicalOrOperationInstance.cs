// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public class LogicalOrOperationInstance : BasicMathOperationInstance
    {
        public LogicalOrOperationInstance(LogicalOrOperationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
        }

        public override DataType DetermineDataTypeOfResult(MathOperationDefinition theDefinition, TestExecution testExecution)
        {
            LogicalOrOperationDefinition theSpecificDefinition = (LogicalOrOperationDefinition)theDefinition;
            foreach( DataValueDefinition valueDef in theSpecificDefinition.ValueObjects)
            {
                ValueObjects.Add(testExecution.DataValueRegistry.GetObject(valueDef.Name));
            }

            return DataType.Boolean;
        }

        public override void DoWork()
        {
            TestExecution().LogMessageWithTimeFromTrigger("MathOp " + Name + " started");

            bool result = false;
            for (int x = 0; x < ValueObjects.Count; x++)
            {
                result = result || ValueObjects[x].ValueAsBoolean();
                if (result) break;
            }
            Result.SetValue(result);
            Result.SetIsComplete();

            TestExecution().LogMessageWithTimeFromTrigger("MathOp " + Name + " completed");
        }
    }
}

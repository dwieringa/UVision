// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public abstract class MultiParameterFunctionInstance : FunctionInstance
    {
        public MultiParameterFunctionInstance(MultiParameterFunctionDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
        }

        public override DataType DetermineDataTypeOfResult(MathOperationDefinition theDefinition, TestExecution testExecution)
        {
            MultiParameterFunctionDefinition theMoreSpecificDef = (MultiParameterFunctionDefinition)theDefinition;

            bool foundDecimalNumber = false;
            bool foundIntegerNumber = false;
            bool foundBooleanValue = false;
            foreach (DataValueDefinition num in theMoreSpecificDef.ValueObjects)
            {
                DataValueInstance value = testExecution.DataValueRegistry.GetObject(num.Name);
                ValueObjects.Add(value);
                switch (value.Type)
                {
                    case DataType.DecimalNumber:
                        foundDecimalNumber = true;
                        break;
                    case DataType.IntegerNumber:
                        foundIntegerNumber = true;
                        break;
                    case DataType.Boolean:
                        foundBooleanValue = true;
                        break;
                    case DataType.NotDefined:
                        throw new ArgumentException("Type not defined for '" + value.Name + "'.");
                        break;
                    default:
                        throw new ArgumentException("Type " + value.Type + " not supported by the basic math operation definition object.");
                }
            }
            if (foundDecimalNumber)
            {
                return DataType.DecimalNumber;
            }
            else if (foundIntegerNumber)
            {
                return DataType.IntegerNumber;
            }
            else if (foundBooleanValue)
            {
                return DataType.Boolean;
            }
            throw new ArgumentException("Data type of math operation result can't be determined.");
        }

        protected List<DataValueInstance> ValueObjects = new List<DataValueInstance>();
    }
}

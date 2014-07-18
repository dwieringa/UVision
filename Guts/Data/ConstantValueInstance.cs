// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
    public class ConstantValueInstance : DataValueWithStorageInstance
	{
        public ConstantValueInstance(ConstantValueDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            switch (mDataType)
            {
                case DataType.IntegerNumber:
                    SetValue(theDefinition.ValueAsLong());
                    break;
                case DataType.DecimalNumber:
                    SetValue(theDefinition.ValueAsDecimal());
                    break;
                case DataType.Boolean:
                    SetValue(theDefinition.ValueAsBoolean());
                    break;
                default:
                    throw new ArgumentException("Type not defined for '" + Name + "'");
            }
        }

//        public override bool OkToChangeInstanceValue() { return false; }

        // TODO: "Constant" value can still change by the operator...but we may want each instance to remember what value it had at the time it ran.  ie operator changes only affect future instances.  how should we handle that?  This is the simple way.
        // TODO: maybe we have 
        //   1) constants (which show up as a number in the property editor)
        //   2) named variables/parameter which show up in the property editor as a name and the operator can tweak as an object
        //   3) named variables that are results of computations (owned by a tool)...show up as name
        //  so if an operator changes a constant number (e.g. 3 to 5), then the only record of that would be
        //   1) if we could scan all live Instance objects that derive from the Definition to see if there were any changes
        //          feasible?  would have to look up in each instance as the function. e.g. smoother's xRadius
        //          it probably only makes to "look for"... (1) a particular result (always generated and thus named) [most important!...and easiest?) and (2) a certain input of a tool
        //   2) if edited Definition using the number kept track

        public override bool IsComplete()
        {
            return true;
        }
    }
}

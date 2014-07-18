// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class CommonValueInstance : DataValueInstance
    {
        public CommonValueInstance(CommonValueDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.GlobalValue == null) throw new ArgumentException(Name + " doesn't have GlobalValue defined.");

            switch( theDefinition.ReadStyle )
            {
                case CommonValueDefinition.DataReadStyle.AtTrigger:
                    if (theDefinition.mHelperTool == null)
                    {
                        throw new ArgumentException("askjdkljsdhf");
                    }
                    theDefinition.mHelperTool.CurrentInstance.ValueGetter = new AtTriggerValueGetter(theDefinition.GlobalValue);
                    mValueGetter = theDefinition.mHelperTool.CurrentInstance.ValueGetter;
                    break;
                case CommonValueDefinition.DataReadStyle.Live:
                    mValueGetter = new LiveValueGetter(theDefinition.GlobalValue);
                    break;
                default:
                    throw new ArgumentException("CommonValue '" + Name + "' has an unsupported ReadStyle. ReadStyle=" + theDefinition.ReadStyle);
                    break;
            }
		}

        public override bool IsComplete()
        {
            // we always have a value at or before the trigger
            return true;
        }

        private IValueGetter mValueGetter;
        public override string Value
        {
            get { return mValueGetter.Value; }
        }

        public override DataType Type
        {
            get { return mValueGetter.Type; }
        }

        public override long ValueAsLong()
        {
            return mValueGetter.ValueAsLong();
        }

        public override double ValueAsDecimal()
        {
            return mValueGetter.ValueAsDecimal();
        }

        public override bool ValueAsBoolean()
        {
            return mValueGetter.ValueAsBoolean();
        }


        public interface IValueGetter
        {
            DataType Type { get; }
            string Value { get; }
            long ValueAsLong();
            double ValueAsDecimal();
            bool ValueAsBoolean();
        }

        public class LiveValueGetter : IValueGetter
        {
            public LiveValueGetter(GlobalValue theGlobalValue)
            {
                mGlobalValue = theGlobalValue;
            }

            private GlobalValue mGlobalValue;

            #region IValueGetter Members

            public DataType Type
            {
                get { return mGlobalValue.Type; }
            }

            public string Value
            {
                get { return mGlobalValue.Value; }
            }

            public long ValueAsLong()
            {
                return mGlobalValue.ValueAsLong();
            }

            public double ValueAsDecimal()
            {
                return mGlobalValue.ValueAsDecimal();
            }

            public bool ValueAsBoolean()
            {
                return mGlobalValue.ValueAsBoolean();
            }

            #endregion
        }

        public class AtTriggerValueGetter : IValueGetter
        {
            public AtTriggerValueGetter(GlobalValue theGlobalValue)
            {
                mGlobalValue = theGlobalValue;
                mDataType = mGlobalValue.Type; // moved here 6/27/08 since Type for value objects must be known when mathops are created when the execution is created
            }

            public GlobalValue mGlobalValue;
            protected DataType mDataType = DataType.NotDefined;
            public DataType Type
            {
                get { return mDataType; }
            }

            public void GrabValue()
            {
                //mDataType = mGlobalValue.Type; changed 6/27/08 NOTE: Type must be defined at TestExecution creation since it is used by mathops to determine their type
                switch (mDataType)
                {
                    case DataType.Boolean:
                        SetValue(mGlobalValue.ValueAsBoolean());
                        break;
                    case DataType.DecimalNumber:
                        SetValue(mGlobalValue.ValueAsDecimal());
                        break;
                    case DataType.IntegerNumber:
                        SetValue(mGlobalValue.ValueAsBoolean());
                        break;
                    default:
                        throw new ArgumentException("Data Type of global " + mGlobalValue.Name + " not defined");
                }
            }
            // TODO: YUCK...storing the value in multiple types!  consider storing a reference to a subobject that stores the actual type. This would only make since if we had a huge number of data items.  References themselves take 4 bytes.   The best possible savings would be for an Integer (not long) or Boolean which are 2 bytes...so ref+2bytes could save 6 types over storing Long (4bytes) and Double (8 bytes).  What about using a variant (16bytes) or "generic 8 bytes"?
            private long mValueAsLong; //4 bytes?  8?
            private double mValueAsDouble; //8 bytes
            public string Value
            {
                get
                {
                    if (mDataType == DataType.Boolean)
                    {
                        if (mValueAsLong == 0) return "False";
                        return "True";
                    }
                    else if (mDataType == DataType.DecimalNumber)
                    {
                        string valueAsString = "" + mValueAsDouble;
                        if (valueAsString.IndexOf('.') < 0) valueAsString += ".0";
                        return valueAsString;
                    }
                    else if (mDataType == DataType.IntegerNumber)
                    {
                        return "" + mValueAsLong;
                    }
                    return "" + mValueAsDouble;
                }
            }

            public void SetValue(long theValue)
            {
                mValueAsLong = theValue;
                mValueAsDouble = theValue;
            }

            public void SetValue(double theValue)
            {
                mValueAsLong = (long)theValue;
                mValueAsDouble = theValue;
            }

            public void SetValue(bool theValue)
            {
                if (theValue)
                {
                    mValueAsLong = 1;
                }
                else
                {
                    mValueAsLong = 0;
                }
                mValueAsDouble = mValueAsLong;
            }

            public long ValueAsLong()
            {
                return mValueAsLong;
            }

            public double ValueAsDecimal()
            {
                return mValueAsDouble;
            }

            public bool ValueAsBoolean()
            {
                return mValueAsLong != 0;
            }

        }

        public class AtTriggerValueGetterToolDefinition : ToolDefinition
        {
            public AtTriggerValueGetterToolDefinition(TestSequence testSequence, CommonValueDefinition theCommonValueDef)
                : base(testSequence)
            {
                mCommonValueDefinition = theCommonValueDef;
                Name = mCommonValueDefinition.Name + "Helper";
            }

            public override bool IncludeObjectInConfigFile() { return false; }
            public override bool IncludeObjectInProgrammingTable() { return false; }

            private CommonValueDefinition mCommonValueDefinition;
            public CommonValueDefinition ValueDefinition() { return mCommonValueDefinition; }

            public AtTriggerValueGetterToolInstance CurrentInstance = null;

            public override void CreateInstance(TestExecution theExecution)
            {
                if (mCommonValueDefinition.ReadStyle == CommonValueDefinition.DataReadStyle.AtTrigger)
                {
                    new AtTriggerValueGetterToolInstance(this, theExecution);
                }
            }

        }

        public class AtTriggerValueGetterToolInstance : ToolInstance
        {
            public AtTriggerValueGetterToolInstance(AtTriggerValueGetterToolDefinition theDefinition, TestExecution theExecution)
                : base(theDefinition, theExecution)
            {
                // this ToolInst is created before the ValueInst (due to dependency), so we store a reference to this instance in the def object for the ValueInst to use (without having to do an instance lookup in the TestExecution)
                theDefinition.CurrentInstance = this;
            }

            public AtTriggerValueGetterToolDefinition AtTriggerValueGetterToolDefinition() { return (AtTriggerValueGetterToolDefinition)Definition(); }
            public AtTriggerValueGetter ValueGetter = null;
            public override void DoWork()
            {
                if( ValueGetter == null )
                {
                    TestExecution().LogErrorWithTimeFromTrigger("CommonValueInstance 'AtTrigger' plumbing not working");
                }
                ValueGetter.GrabValue();
                mIsComplete = true;
            }

            private bool mIsComplete = false;
            public override bool IsComplete()
            {
                return mIsComplete;
            }
        }
    }
}

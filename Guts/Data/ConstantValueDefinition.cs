// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
    public class ConstantValueDefinition : DataValueWithStorageDefinition
	{
        public ConstantValueDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            SetDataCategory(DataCategory.NamedValue); // assume it will be named...this is changed by the code that creates unnamed constants
		}

		public override void CreateInstance(TestExecution theExecution)
		{
			new ConstantValueInstance(this, theExecution);
		}

        private bool mIncludeObjectInConfigFile = true;
        public void SetIncludeObjectInConfigFile(bool value) { mIncludeObjectInConfigFile = value; }
        public override bool IncludeObjectInConfigFile() { return mIncludeObjectInConfigFile; }

        private bool mIncludeObjectInProgrammingTable = true;
        public void SetIncludeObjectInProgrammingTable(bool value) { mIncludeObjectInProgrammingTable = value; }
        public override bool IncludeObjectInProgrammingTable() { return mIncludeObjectInProgrammingTable; }

        // TODO: YUCK...storing the value in multiple types!  consider storing a reference to a subobject that stores the actual type. This would only make since if we had a huge number of data items.  References themselves take 4 bytes.   The best possible savings would be for an Integer (not long) or Boolean which are 2 bytes...so ref+2bytes could save 6 types over storing Long (4bytes) and Double (8 bytes).  What about using a variant (16bytes) or "generic 8 bytes"?
        protected long mValueAsLong; //4 bytes?  8?
        protected double mValueAsDouble; //8 bytes
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
            set
            {
                if (Name.Equals(Value) && value != Value)
                {
                    throw new ArgumentException("Can't change the value of this unnamed constant.");
                }

                if (mDataType == DataType.NotDefined)
                {
                    string trimmedValue = value.Trim();
                    // TODO: make these decisions smarter and/or improve error messages?
                    if (trimmedValue.Length > 0)
                    {
                        if (trimmedValue.IndexOf('.') >= 0)
                        {
                            Type = DataType.DecimalNumber;
                        }
                        else if (Char.IsDigit(trimmedValue[0]) || trimmedValue[0] == '-')
                        {
                            Type = DataType.IntegerNumber;
                        }
                        else
                        {
                            Type = DataType.Boolean;
                        }
                    }
                }

                // CONVERSION FROM STRING TO DATA VALUE: this code exists in multiple places currently...as it is improved, it should be improved everywhere
                switch (mDataType)
                {
                    case DataType.Boolean:
                        HandlePropertyChange(this, "Value", ValueAsBoolean().ToString(), value);
                        SetValue(bool.Parse(value));
                        break;
                    case DataType.DecimalNumber:
                        HandlePropertyChange(this, "Value", ValueAsDecimal().ToString(), value);
                        SetValue(double.Parse(value));
                        break;
                    case DataType.IntegerNumber:
                        HandlePropertyChange(this, "Value", ValueAsLong().ToString(), value);
                        SetValue(long.Parse(value));
                        break;
                    default:
                        throw new ArgumentException("Data Type not supported by ConstantValueDefinition");
                }
            }
        }

        public void SetValue(long theValue)
        {
            mValueAsLong = theValue;
            mValueAsDouble = theValue;
            if (mGlobalValueToUpdate != null) mGlobalValueToUpdate.SetValue(theValue);
        }

        public void SetValue(double theValue)
        {
            mValueAsLong = (long)theValue;
            mValueAsDouble = theValue;
            if (mGlobalValueToUpdate != null) mGlobalValueToUpdate.SetValue(theValue);
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
            if (mGlobalValueToUpdate != null) mGlobalValueToUpdate.SetValue(theValue);
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
}

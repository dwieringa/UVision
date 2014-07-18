// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class DataValueWithStorageInstance : DataValueInstance
	{
		public DataValueWithStorageInstance(DataValueWithStorageDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mDataType = theDefinition.Type;
            if (mDataType == DataType.NotDefined) throw new ArgumentException("Data type not defined for '" + Name + "'");
		}

        protected DataType mDataType = DataType.NotDefined;
        public override DataType Type
        {
            get { return mDataType; }
        }

        // TODO: YUCK...storing the value in multiple types!  consider storing a reference to a subobject that stores the actual type. This would only make since if we had a huge number of data items.  References themselves take 4 bytes.   The best possible savings would be for an Integer (not long) or Boolean which are 2 bytes...so ref+2bytes could save 6 types over storing Long (4bytes) and Double (8 bytes).  What about using a variant (16bytes) or "generic 8 bytes"?
        private long mValueAsLong; //4 bytes?  8?
        private double mValueAsDouble; //8 bytes
        public override string Value
        {
            get
            {
                switch (mDataType)
                {
                    case DataType.Boolean:
                        if (mValueAsLong == 0) return "False";
                        return "True";
                    case DataType.DecimalNumber:
                        string valueAsString = "" + mValueAsDouble;
                        if (valueAsString.IndexOf('.') < 0) valueAsString += ".0";
                        return valueAsString;
                    case DataType.IntegerNumber:
                        return "" + mValueAsLong;
                    default:
                        throw new ArgumentException("DataType not defined for value '" + Name + "' code=892374");
                        //return "" + mValueAsDouble;
                }
            }
/* there is no reason to set a value of an Instance object, is there???
            set
            {
                if (!OkToChangeInstanceValue()) throw new ArgumentException("Can't change the value");
                switch (mDataType)
                {
                    case DataType.Boolean:
                        SetValue(bool.Parse(value));
                        break;
                    case DataType.DecimalNumber:
                        SetValue(double.Parse(value));
                        break;
                    case DataType.IntegerNumber:
                        SetValue(long.Parse(value));
                        break;
                    default:
                        throw new ArgumentException("Data Type not defined");
                }
            }
*/
        }

//        public virtual bool OkToChangeInstanceValue() { return true; }


        public void SetValue(long theValue)
        {
            mValueAsLong = theValue;
            mValueAsDouble = theValue;
            if( Definition_DataValue().GlobalValueToUpdate != null ) Definition_DataValue().GlobalValueToUpdate.SetValue(theValue);
        }

        public void SetValue(double theValue)
        {
            mValueAsLong = (long)theValue;
            mValueAsDouble = theValue;
            if (Definition_DataValue().GlobalValueToUpdate != null) Definition_DataValue().GlobalValueToUpdate.SetValue(theValue);
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
            if (Definition_DataValue().GlobalValueToUpdate != null) Definition_DataValue().GlobalValueToUpdate.SetValue(theValue);
        }

        public override long ValueAsLong()
        {
            return mValueAsLong;
        }

        public override double ValueAsDecimal()
        {
            return mValueAsDouble;
        }

        public override bool ValueAsBoolean()
        {
            return mValueAsLong != 0;
        }
    }
}

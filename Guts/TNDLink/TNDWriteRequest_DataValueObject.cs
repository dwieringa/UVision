// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class TNDWriteRequest_DataValueObject : TNDWriteRequest
    {
        public TNDWriteRequest_DataValueObject()
        {
        }

        private DataValueDefinition mDataValueDef;
        public DataValueDefinition DataValue
        {
            get { return mDataValueDef; }
            set { mDataValueDef = value; }
        }

        private DataValueInstance mDataValueInstance;
        public DataValueInstance DataValueInstance
        {
            get { return mDataValueInstance; }
            set
            {
                if (value.Name != mDataValueDef.Name)
                {
                    throw new ArgumentException("Wrong data value instance supplied to TNDWriteRequest; got='" + value.Name + "   expected="+mDataValueDef.Name);
                }
                mDataValueInstance = value;
            }
        }

        public override object GetValueAsObject()
        {
            switch (TNDDataType)
            {
                case TNDLink.TNDDataTypeEnum.Flag:
                    return mDataValueInstance.ValueAsBoolean();
                case TNDLink.TNDDataTypeEnum.Input: // TODO is there any reason to write to an input???
                    return mDataValueInstance.ValueAsBoolean();
                case TNDLink.TNDDataTypeEnum.Output:
                    return mDataValueInstance.ValueAsBoolean();
                case TNDLink.TNDDataTypeEnum.Number:
                    return (int)mDataValueInstance.ValueAsLong(); // TODO: test for overflow?
                case TNDLink.TNDDataTypeEnum.Counter:
                    return (short)mDataValueInstance.ValueAsLong(); // TODO: test for overflow?
                default:
                    throw new ArgumentException("TND Write Request doesn't support converting from " + mDataValueInstance.Type + " to " + TNDDataType);
            }
        }
        public override long GetValueAsLong()
        {
            switch (TNDDataType)
            {
                case TNDLink.TNDDataTypeEnum.Flag:
                    return mDataValueInstance.ValueAsLong();
                case TNDLink.TNDDataTypeEnum.Input: // TODO is there any reason to write to an input???
                    return mDataValueInstance.ValueAsLong();
                case TNDLink.TNDDataTypeEnum.Output:
                    return mDataValueInstance.ValueAsLong();
                case TNDLink.TNDDataTypeEnum.Number:
                    return mDataValueInstance.ValueAsLong(); // TODO: test for overflow?
                case TNDLink.TNDDataTypeEnum.Counter:
                    return mDataValueInstance.ValueAsLong(); // TODO: test for overflow?
                default:
                    throw new ArgumentException("TND Write Request doesn't support converting from " + mDataValueInstance.Type + " to " + TNDDataType);
            }
        }
    }
}

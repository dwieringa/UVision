// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public abstract class DataValueInstance : DataInstance, IReferencePointInstance
    {
        public DataValueInstance(DataValueDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            testExecution.DataValueRegistry.RegisterObject(this);
            testExecution.ReferencePointRegistry.RegisterObject(this);
        }

        public DataValueDefinition Definition_DataValue() { return (DataValueDefinition)Definition(); }

        public abstract string Value
        {
            get;
        }

        public abstract DataType Type
        {
            get;
        }

        public abstract long ValueAsLong();
        public abstract double ValueAsDecimal();
        public abstract bool ValueAsBoolean();

        public override string ToString()
        {
            return Name + " (" + Value + ")";
        }

        #region IReferencePointInstance Members

        int IReferencePointInstance.GetValueAsRoundedInt()
        {
            return (int)Math.Round(ValueAsDecimal());
        }

        int IReferencePointInstance.GetValueAsTruncatedInt()
        {
            return (int)Math.Truncate(ValueAsDecimal()); // TODO: yuck...lots of extra work if value is already Integer
        }

        double IReferencePointInstance.GetValueAsDouble()
        {
            return ValueAsDecimal();
        }

        #endregion
    }
}

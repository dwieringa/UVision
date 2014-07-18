// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;

namespace NetCams
{
    public abstract class DataValueWithStorageDefinition : DataValueDefinition
	{
		public DataValueWithStorageDefinition(TestSequence testSequence) : base(testSequence)
		{
		}

        /* commented out 4/4/08 since it is exactly the same as from the parent DataValueDefinition
        protected DataType mDataType = DataType.NotDefined;
        public DataType Type
        {
            set { mDataType = value; }
            get { return mDataType; }
        }
        */
    }
}

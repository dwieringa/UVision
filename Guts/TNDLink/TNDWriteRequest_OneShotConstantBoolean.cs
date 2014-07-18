// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class TNDWriteRequest_OneShotConstantBoolean : TNDWriteRequest
    {
        public TNDWriteRequest_OneShotConstantBoolean(bool theValue)
        {
            mValue = theValue;
        }

        public override object GetValueAsObject()
        {
            return (short)(mValue ? 1 : 0);
        }
        public override long GetValueAsLong()
        {
            return (long)(mValue ? 1 : 0);
        }

        public void SetBooleanValue(bool theNewValue)
        {
            mValue = theNewValue;
        }

        private bool mValue = false;
    }
}

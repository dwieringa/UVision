// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public abstract class TNDWriteRequest
    {
        public TNDWriteRequest()
        {
        }

        public abstract long GetValueAsLong();
        public abstract object GetValueAsObject();
        public virtual bool IsOneTimeWrite() { return true; }

        private bool mActive = false;
        public bool Active
        {
            get { return mActive; }
            set
            {
                if (value != mActive)
                {
                    if (value && mTNDWriter == null)
                    {
                        throw new ArgumentException("Can't activate write request since writer not defined.");
                    }
                    // TODO: don't allow deactivation if value hasn't been written yet?

                    mActive = value;
                    if (mTNDWriter != null)
                    {
                        mTNDWriter.SetTagListDirty();
                    }
                }
            }
        }


        private TNDnTagWriter mTNDWriter = null;
        public TNDnTagWriter TNDWriter
        {
            get { return mTNDWriter; }
            set
            {
                if (value == mTNDWriter) return;
                if (mTNDWriter != null)
                {
                    mTNDWriter.RemoveWriteRequest(this);
                }

                mTNDWriter = value;

                if (value == null) return;

                mTNDWriter.AddWriteRequest(this);
            }
        }


        private short mDataViewIndex;
        public short TNDDataViewIndex
        {
            get { return mDataViewIndex; }
            set
            {
                mDataViewIndex = value;
                if (mTNDWriter != null) // added 7/10/08...isn't this true???
                {
                    mTNDWriter.SetTagListDirty();
                }
            }
        }

        private TNDLink.TNDDataTypeEnum mTNDDataType;
        public TNDLink.TNDDataTypeEnum TNDDataType
        {
            get { return mTNDDataType; }
            set
            {
                if (mTNDWriter != null) // added 7/10/08...isn't this true???
                {
                    mTNDWriter.SetTagListDirty();
                }
                mTNDDataType = value;
            }
        }

        public short TNDDataTypeAsShort()
        {
            return (short)mTNDDataType;
        }


    }
}

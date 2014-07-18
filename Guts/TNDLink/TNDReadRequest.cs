// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class TNDReadRequest
    {
        public TNDReadRequest()
        {
        }

        private bool mActive = false;
        public bool Active
        {
            get { return mActive; }
            set
            {
                if( value != mActive )
                {
                    if( value && mTNDReader == null )
                    {
                        throw new ArgumentException("Can't activate Read Request since Reader not defined.");
                    }
                    if (!value && (BoolValueReceived != null || WholeNumberReceived != null || DecimalNumberReceived != null))
                    {
                        throw new ArgumentException("Can't deactivate Read Request since listeners still defined.");
                    }

                    mActive = value;
                    if (mTNDReader != null)
                    {
                        mTNDReader.SetTagListDirty();
                    }
                }
            }
        }

        private bool mPermanentRequest = false;
        /// <summary>
        /// Some times it is wise to mark a request as permenant so that it doesn't automatically go inactive.
        /// e.g. when we poll a trigger flag from TND, we just want to keep polling even while we're executing the test even though we won't use the value (no listener will be registered until the next test execution is created) since updating the taglist takes time...and the time we aren't listening is extremely short.  ...AND I currently (1/11/08) have a lock conflict which causes issues on dual core PCs
        /// </summary>
        public bool PermanentRequest
        {
            get { return mPermanentRequest; }
            set { mPermanentRequest = value; }
        }

        public void SuggestGoingInActive()
        {
            using (TimedLock.Lock(listenerLock))
            {
                if (BoolValueReceived == null && WholeNumberReceived == null && DecimalNumberReceived == null)
                {
                    Active = false;
                }
            }
        }

        public Object listenerLock = new Object();
        public delegate void DecimalNumberDelegate(double theDouble);
        public delegate void WholeNumberDelegate(long theLong);
        public delegate void BoolDelegate(bool theBool);
        private event BoolDelegate BoolValueReceived;
        private event WholeNumberDelegate WholeNumberReceived;
        private event DecimalNumberDelegate DecimalNumberReceived;

        public void AddValueListener(BoolDelegate theDelegate)
        {
            using (TimedLock.Lock(listenerLock))
            {
                BoolValueReceived += theDelegate;
            }
        }

        public void RemoveValueListener(BoolDelegate theDelegate)
        {
            using (TimedLock.Lock(listenerLock))
            {
                BoolValueReceived -= theDelegate;
                //SuggestGoingInActive(); // TODO: here or only after certain calls to this method?  get rid of it all together? (ie always keep the same taglist but only use the values we care about...since taglist updates take time)
            }
        }

        public void AddValueListener(WholeNumberDelegate theDelegate)
        {
            using (TimedLock.Lock(listenerLock))
            {
                WholeNumberReceived += theDelegate;
            }
        }

        public void RemoveValueListener(WholeNumberDelegate theDelegate)
        {
            using (TimedLock.Lock(listenerLock))
            {
                WholeNumberReceived -= theDelegate;
                //SuggestGoingInActive(); // TODO: here or only after certain calls to this method?  get rid of it all together? (ie always keep the same taglist but only use the values we care about...since taglist updates take time)
            }
        }

        public void AddDecimalNumberListener(DecimalNumberDelegate theDelegate)
        {
            using (TimedLock.Lock(listenerLock))
            {
                DecimalNumberReceived += theDelegate;
            }
        }

        public void RemoveDecimalNumberListener(DecimalNumberDelegate theDelegate)
        {
            using (TimedLock.Lock(listenerLock))
            {
                DecimalNumberReceived -= theDelegate;
                //SuggestGoingInActive(); // TODO: here or only after certain calls to this method?  get rid of it all together? (ie always keep the same taglist but only use the values we care about...since taglist updates take time)
            }
        }

        private TNDnTagReader mTNDReader = null;
        public TNDnTagReader TNDReader
        {
            get { return mTNDReader; }
            set
            {
                if (value == mTNDReader) return;
                if (mTNDReader != null)
                {
                    mTNDReader.RemoveReadRequest(this);
                }

                mTNDReader = value;

                if (value == null) return;

                mTNDReader.AddReadRequest(this);
            }
        }


        private short mDataViewIndex;
        public short TNDDataViewIndex
        {
            get { return mDataViewIndex; }
            set
            {
                mDataViewIndex = value;
                if (mTNDReader != null) // added 7/10/08...isn't this true???
                {
                    mTNDReader.SetTagListDirty();
                }
            }
        }

        private TNDLink.TNDDataTypeEnum mTNDDataType;
        public TNDLink.TNDDataTypeEnum TNDDataType
        {
            get { return mTNDDataType; }
            set
            {
                mTNDDataType = value;
                if (mTNDReader != null) // added 7/10/08...isn't this true???
                {
                    mTNDReader.SetTagListDirty();
                }
            }
        }

        public short TNDDataTypeAsShort()
        {
            return (short)mTNDDataType;
        }

        public void HandleValue(object valueFromTND)
        {
            if (valueFromTND == null)
            {
                if (mTNDReader != null)
                {
                    mTNDReader.Project().Window().logMessageWithFlush("ERROR: TNDReadRequest.HandleValue() called with value=null");
                }
                return;
            }
            bool noticedListener = false;
            using (TimedLock.Lock(listenerLock))
            {
                if (BoolValueReceived != null)
                {
                    noticedListener = true;
                    switch (mTNDDataType)
                    {
                        case TNDLink.TNDDataTypeEnum.Input:
                            goto case TNDLink.TNDDataTypeEnum.Flag;
                        case TNDLink.TNDDataTypeEnum.Output:
                            goto case TNDLink.TNDDataTypeEnum.Flag;
                        case TNDLink.TNDDataTypeEnum.Flag:
                            BoolValueReceived((bool)valueFromTND);
                            break;
                        case TNDLink.TNDDataTypeEnum.Counter:
                            goto case TNDLink.TNDDataTypeEnum.Number;
                        case TNDLink.TNDDataTypeEnum.Number:
                            BoolValueReceived(((int)valueFromTND) == 0 ? false : true);
                            break;
                        default:
                            throw new ArgumentException("Conversion from TND type " + mTNDDataType + " to Boolean is not supported");
                    }
                }
                if (WholeNumberReceived != null)
                {
                    noticedListener = true;
                    switch (mTNDDataType)
                    {
                        case TNDLink.TNDDataTypeEnum.Input:
                            goto case TNDLink.TNDDataTypeEnum.Flag;
                        case TNDLink.TNDDataTypeEnum.Output:
                            goto case TNDLink.TNDDataTypeEnum.Flag;
                        case TNDLink.TNDDataTypeEnum.Flag:
                            WholeNumberReceived(((bool)valueFromTND) ? 1 : 0);
                            break;
                        case TNDLink.TNDDataTypeEnum.Counter:
                            goto case TNDLink.TNDDataTypeEnum.Number;
                        case TNDLink.TNDDataTypeEnum.Number:
                            WholeNumberReceived((int)valueFromTND);
                            break;
                        default:
                            throw new ArgumentException("Conversion from TND type " + mTNDDataType + " to Whole Number is not supported");
                    }
                }
                if (DecimalNumberReceived != null)
                {
                    noticedListener = true;
                    switch (mTNDDataType)
                    {
                        case TNDLink.TNDDataTypeEnum.Float:
                            DecimalNumberReceived((float)valueFromTND);
                            break;
                        case TNDLink.TNDDataTypeEnum.Input:
                            goto case TNDLink.TNDDataTypeEnum.Flag;
                        case TNDLink.TNDDataTypeEnum.Output:
                            goto case TNDLink.TNDDataTypeEnum.Flag;
                        case TNDLink.TNDDataTypeEnum.Flag:
                            DecimalNumberReceived(((bool)valueFromTND) ? 1 : 0);
                            break;
                        case TNDLink.TNDDataTypeEnum.Counter:
                            goto case TNDLink.TNDDataTypeEnum.Number;
                        case TNDLink.TNDDataTypeEnum.Number:
                            DecimalNumberReceived((int)valueFromTND);
                            break;
                        default:
                            throw new ArgumentException("Conversion from TND type " + mTNDDataType + " to Decimal Number is not supported");
                    }
                }
                if (!PermanentRequest && !noticedListener)
                {
                    Active = false; // I want this within the lock since its implementation verifes that the listeners are all empty, and I don't ever want it to throw an exception
                }
            } // end of lock
        }

        /*
        public static TNDnTagReader SetReader(ITNDReadRequestDefinition readRequest, TNDnTagReader oldValue, TNDnTagReader newValue)
        {
            if (newValue == oldValue) return oldValue;
            if (oldValue != null)
            {
                oldValue.RemoveReadRequest(readRequest);
            }

            oldValue = newValue;

            if (newValue == null) return newValue;

            newValue.AddReadRequest(readRequest);

            return newValue;
        }*/
    }
}

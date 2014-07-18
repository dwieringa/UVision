// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{

    [TypeConverter(typeof(TNDnTagWriterConverter))]
    public class TNDnTagWriter : TNDnTagLink
    {

        public TNDnTagWriter(Project theProject)
            : base(theProject)
        {
            Name = "Global TND Writer";
            mStatusIndicator.Text = Name + mStatusIndicator.Text; // HACK: the text was originally set by the parent class's ctor (see base(...)) before we defined the name
            theProject.globalTNDWriter = this;
        }

        private long mSumPeriodBetweenPolls = 0;
        private long mNumberOfSums = 0;
        private int mAveragePeriodBetweenPolls = 0;
        private int mMinPeriodBetweenPolls = 0;
        private int mMaxPeriodBetweenPolls = 0;
        private int mMaxValuesPerWrite = 0;
        private long mNumberOfWrites = 0;

        [CategoryAttribute("Advanced Debug Metrics")]
        public bool Reset
        {
            get { return false; }
            set
            {
                mAveragePeriodBetweenPolls = 0;
                mSumPeriodBetweenPolls = 0;
                mNumberOfSums = 0;
                mMinPeriodBetweenPolls = 99999999;
                mMaxPeriodBetweenPolls = 0;
                mNumberOfWrites = 0;
                mMaxValuesPerWrite = 0;
            }
        }
        [CategoryAttribute("Advanced Debug Metrics")]
        public int AveragePeriodBetweenPolls { get { return mAveragePeriodBetweenPolls; } }
        [CategoryAttribute("Advanced Debug Metrics")]
        public long NumberOfPolls { get { return mNumberOfSums; } }
        [CategoryAttribute("Advanced Debug Metrics")]
        public long NumberOfWrites { get { return mNumberOfWrites; } }
        [CategoryAttribute("Advanced Debug Metrics")]
        public long MinPeriod { get { return mMinPeriodBetweenPolls; } }
        [CategoryAttribute("Advanced Debug Metrics")]
        public long MaxPeriod { get { return mMaxPeriodBetweenPolls; } }
        [CategoryAttribute("Advanced Debug Metrics")]
        public long MaxValuesPerWrite { get { return mMaxValuesPerWrite; } }
        private DateTime mLastPollTime;

        private DateTime mStarted;
        private DateTime mGotLock;
        private DateTime mTagListUpdated;
        private DateTime mStartWrite;
        private DateTime mValuesUpdated;
        private DateTime mValueSent;
        private DateTime mRequestsCleanedUp;
        private MetricsTracker mLockAttainment = new MetricsTracker();
        private MetricsTracker mTagListUpdate = new MetricsTracker();
        private MetricsTracker mValuesUpdate = new MetricsTracker();
        private MetricsTracker mValueSending = new MetricsTracker();
        private MetricsTracker mRequestCleanup = new MetricsTracker();

        [CategoryAttribute("Advanced Debug Metrics2")]
        public string LockAttainment { get { return mLockAttainment.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public string TagListUpdate { get { return mTagListUpdate.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public string ValuesUpdate { get { return mValuesUpdate.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public string ValueSending { get { return mValueSending.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public string RequestCleanup { get { return mRequestCleanup.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public bool Reset2
        {
            get { return false; }
            set
            {
                mLockAttainment.Reset = true;
                mTagListUpdate.Reset = true;
                mValuesUpdate.Reset = true;
                mValueSending.Reset = true;
                mRequestCleanup.Reset = true;
            }
        }


        protected override void mPollTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs eventArgs)
        {
            mPollTimer.Enabled = false;
            mTimeoutTimer.Enabled = true;

            mStarted = DateTime.Now;
            if (Project().FullyInitialized)
            {

                mNumberOfSums++;
                DateTime now = DateTime.Now;
                if (mNumberOfSums > 1) //ignore the first one (==1) since mLastPollTime won't be init'ed
                {
                    TimeSpan period = now - mLastPollTime;
                    int periodInMS = (int)period.TotalMilliseconds;
                    if (periodInMS < mMinPeriodBetweenPolls) mMinPeriodBetweenPolls = periodInMS;
                    if (periodInMS > mMaxPeriodBetweenPolls) mMaxPeriodBetweenPolls = periodInMS;
                    mSumPeriodBetweenPolls += periodInMS;
                    mAveragePeriodBetweenPolls = (int)(mSumPeriodBetweenPolls / (mNumberOfSums - 1));
                }
                mLastPollTime = now;

                mTimeoutStateIndicator = "Starting Poll";

                try
                {
                    mTimeoutStateIndicator = "Entering lock";
                    //                lock (masterListLock)
                    using (TimedLock.Lock(masterListLock))
                    {
                        mTimeoutStateIndicator = "Checking if tag list dirty";
                        mGotLock = DateTime.Now;
                        mLockAttainment.NewPeriod(mStarted, mGotLock);

                        if (mTagListDirty)
                        {
                            //                        mProject.Window().logMessage(Name + " updating tag list " + mWriteRequestsInTagList.Count + " " + mWriteRequests_all.Count);
                            // start with an empty tag list
                            mTimeoutStateIndicator = "Clearing tag list";
                            mWriteRequestsInTagList.Clear();
                            int result = axTndNTag.ClearTagList();
                            if (result != ThinkAndDoSuccess)
                            {
                                mProject.Window().logMessage("ERROR: problem clearing tag list. code=" + result);
                                HandleLostConnection();
                                return;
                            }

                            // build list
                            mTimeoutStateIndicator = "starting tag list";
                            result = axTndNTag.StartTagList();
                            if (result != ThinkAndDoSuccess)
                            {
                                mProject.Window().logMessage("ERROR: problem initiating tag list. code=" + result);
                                HandleLostConnection();
                                return;
                            }

                            mTimeoutStateIndicator = "building tag list";
                            foreach (TNDWriteRequest req in mWriteRequests_all)
                            {
                                if (req.Active)
                                {
                                    result = axTndNTag.AddToTagList(req.TNDDataTypeAsShort(), req.TNDDataViewIndex, "");
                                    if (result != ThinkAndDoSuccess)
                                    {
                                        mProject.Window().logMessage("ERROR: adding to tag list. code=" + result + "  type=" + req.TNDDataTypeAsShort() + "  index=" + req.TNDDataViewIndex);
                                        HandleLostConnection();
                                        return;
                                    }
                                    mWriteRequestsInTagList.Add(req);
                                }
                            }

                            mTimeoutStateIndicator = "ending tag list";
                            result = axTndNTag.EndTagList();
                            if (result != ThinkAndDoSuccess)
                            {
                                mProject.Window().logMessage("ERROR: problem closing tag list. code=" + result);
                                HandleLostConnection();
                                return;
                            }

                            mTagListDirty = false;
                            mTagListUpdated = DateTime.Now;
                            mTagListUpdate.NewPeriod(mGotLock, mTagListUpdated);
                            //                        mProject.Window().logMessage(Name + " tag list update complete " + mWriteRequestsInTagList.Count + " " + mWriteRequests_all.Count);
                        }
                        mTimeoutStateIndicator = "exit lock";
                    } // end thread lock

                    if (mWriteRequestsInTagList.Count > 0)
                    {
                        mTimeoutStateIndicator = "starting write; updating values";
                        mStartWrite = DateTime.Now;

                        mNumberOfWrites++;
                        if (mWriteRequestsInTagList.Count > mMaxValuesPerWrite) mMaxValuesPerWrite = mWriteRequestsInTagList.Count;

                        //                    mProject.Window().logMessage(Name + " writing " + mWriteRequestsInTagList.Count + " values");
                        int result = -99911999;

                        short requestIndex = 0;
                        foreach (TNDWriteRequest req in mWriteRequestsInTagList)
                        {
                            //object valueToWrite = req.GetValueAsObject();
                            //axTndNTag.UpdatVariantValue(requestIndex, ref valueToWrite);
                            int valueToWrite = (int)req.GetValueAsLong();
                            axTndNTag.UpdateLongValue(requestIndex, valueToWrite);
                            //object valAsObject = valueToWrite;
                            //axTndNTag.UpdateVariantVal(requestIndex, ref valAsObject);
                            requestIndex++;
                        }

                        mTimeoutStateIndicator = "starting write";
                        mValuesUpdated = DateTime.Now;
                        mValuesUpdate.NewPeriod(mStartWrite, mValuesUpdated);
                        try
                        {
                            result = axTndNTag.Write();
                        }
                        catch (COMException comException)
                        {
                            mProject.Window().logMessageWithFlush("ERROR: COM Exception during TND write attempt. code=" + comException.ErrorCode + "  msg=" + comException.Message);
                        }
                        catch (Exception exception)
                        {
                            mProject.Window().logMessageWithFlush("ERROR: Exception during TND write attempt. msg=" + exception.Message);
                        }
                        mValueSent = DateTime.Now;
                        mValueSending.NewPeriod(mValuesUpdated, mValueSent);

                        if (result == TNDLink.ThinkAndDoSuccess)
                        {
                            mTimeoutStateIndicator = "finished write successfully";
                            foreach (TNDWriteRequest req in mWriteRequestsInTagList)
                            {
                                if (req.IsOneTimeWrite())
                                {
                                    req.Active = false;
                                }
                            }
                            mRequestsCleanedUp = DateTime.Now;
                            mRequestCleanup.NewPeriod(mValueSent, mRequestsCleanedUp);
                        }
                        else
                        {
                            mTimeoutStateIndicator = "finished write WITH ERRORS";
                            if (result != lastResult)
                            {
                                mProject.Window().logMessage("ERROR: TND write failed for '" + Name + "': " + TNDLink.GetTNDResultMessage(result));
                            }
                            HandleLostConnection();
                        }
                        lastResult = result;
                        //                    mProject.Window().logMessage(Name + " finished writing");
                    }
                }
                catch (Exception e)
                {
                    mProject.Window().logMessage("ERROR: Exception in TND Writer " + Name + "; msg=" + e.Message);
                }
            }

            if (Connected)
            {
                mTimeoutStateIndicator = "restarting poll timer";
                mPollTimer.Enabled = true;
            }
            mTimeoutStateIndicator = "poll completed";
            mTimeoutTimer.Enabled = false;
        }
        private int lastResult = 0;


        public void AddWriteRequest(TNDWriteRequest theRequest)
        {
            if (!Connected) Connected = true;

            //            lock (masterListLock)
            using (TimedLock.Lock(masterListLock))
            {
                if (mWriteRequests_all.Contains(theRequest))
                {
                    throw new ArgumentException("How did we get here?");
                }
                mWriteRequests_all.Add(theRequest);

                if (theRequest.Active)
                {
                    mTagListDirty = true;
                }
            }
        }

        public void RemoveWriteRequest(TNDWriteRequest theRequest)
        {
//            lock (masterListLock)
            using (TimedLock.Lock(masterListLock))
            {
                mWriteRequests_all.Remove(theRequest);

                if (theRequest.Active)
                {
                    mTagListDirty = true;
                }
            }
        }

        [CategoryAttribute("Advanced Debug")]
        public int NumberOfWriteRequests_All
        {
            get { return mWriteRequests_all.Count; }
        }

        [CategoryAttribute("Advanced Debug")]
        public int NumberOfWriteRequests_Active
        {
            get { return mWriteRequestsInTagList.Count; }
        }

        [CategoryAttribute("Advanced Debug")]
        public bool Polling
        {
            get { return mPollTimer.Enabled; }
            set { mPollTimer.Enabled = value; }
        }

        [CategoryAttribute("Advanced Debug")]
        public bool TagListDirty
        {
            get { return mTagListDirty; }
            set { mTagListDirty = value; }
        }

        private List<TNDWriteRequest> mWriteRequestsInTagList = new List<TNDWriteRequest>(); // WARNING: this should only be accessed within mPollTimer_Elapsed to avoid threading issues
        private List<TNDWriteRequest> mWriteRequests_all = new List<TNDWriteRequest>(); // WARNING: this should only be accessed with a lock of masterListLock since it is altered by the Sequence threads and iterated over with the poll timer thread
        //private Object masterListLock = new Object(); moved to TNDnTagLink 9/12/07 since I need it for tagListDirty protection
    }

    public class TNDnTagWriterConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(((IObjectDefinition)context.Instance).TestSequence().TNDnTagWriterOptions());
            }
            else
            {
                throw new ArgumentException("Unexpected context (4339)");
            }
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(TNDnTagWriter))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is TNDnTagWriter)
            {

                TNDnTagWriter idd = (TNDnTagWriter)value;

                return idd.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                IObjectDefinition objectBeingEditted;
                TNDnTagWriter namedReader;

                try
                {
                    string readerName = (string)value;

                    objectBeingEditted = (IObjectDefinition)context.Instance;

                    namedReader = objectBeingEditted.TestSequence().GetTNDnTagWriter(readerName);

                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to an TND Reader");
                }
                //				if( namedColorMatch.IsDependentOn(objectBeingEditted) )
                //				{
                //					throw new ArgumentException("That would create a circular dependency");
                //				}
                return namedReader;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

}

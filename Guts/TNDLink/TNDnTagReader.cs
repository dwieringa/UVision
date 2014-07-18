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

    [TypeConverter(typeof(TNDnTagReaderConverter))]
    public class TNDnTagReader : TNDnTagLink
    {
       
        public TNDnTagReader(Project theProject)
            : base(theProject)
        {
            Name = "Global TND Reader";
            mStatusIndicator.Text = Name + mStatusIndicator.Text; // HACK: the text was originally set by the parent class's ctor (see base(...)) before we defined the name
            theProject.globalTNDReader = this;
        }

        private DateTime mStarted;
        private DateTime mGotLock;
        private DateTime mTagListUpdated;
        private DateTime mStartRead;
        private DateTime mValuesHandled;
        private DateTime mValuesRead;
        private MetricsTracker mLockAttainment = new MetricsTracker();
        private MetricsTracker mTagListUpdate = new MetricsTracker();
        private MetricsTracker mValuesHandling = new MetricsTracker();
        private MetricsTracker mValuesRetrieval = new MetricsTracker();

        [CategoryAttribute("Advanced Debug Metrics2")]
        public string LockAttainment { get { return mLockAttainment.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public string TagListUpdate { get { return mTagListUpdate.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public string ValuesHandling { get { return mValuesHandling.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public string ValuesRetrieval { get { return mValuesRetrieval.ToString(); } }
        [CategoryAttribute("Advanced Debug Metrics2")]
        public bool Reset2
        {
            get { return false; }
            set
            {
                mLockAttainment.Reset = true;
                mTagListUpdate.Reset = true;
                mValuesHandling.Reset = true;
                mValuesRetrieval.Reset = true;
            }
        }

        protected override void mPollTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs eventArgs)
        {
            mPollTimer.Enabled = false;
            mTimeoutTimer.Enabled = true;

            mStarted = DateTime.Now;

            if (Project().FullyInitialized)
            {
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
                            // start with an empty tag list
                            mTimeoutStateIndicator = "Clearing tag list";
                            mReadRequestsInTagList.Clear();
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

                            mTimeoutStateIndicator = "sorting read requests";
                            mReadRequests_all.Sort(ReadRequestSorter.Singleton);
                            mTimeoutStateIndicator = "building tag list";
                            TNDReadRequest lastReq = null;
                            foreach (TNDReadRequest req in mReadRequests_all)
                            {
                                if (req.Active)
                                {
                                    // only add unique ones to the taglist...the requests should be shorted by Type & Index
                                    if (lastReq == null || req.TNDDataTypeAsShort() != lastReq.TNDDataTypeAsShort() || req.TNDDataViewIndex != lastReq.TNDDataViewIndex)
                                    {
                                        result = axTndNTag.AddToTagList(req.TNDDataTypeAsShort(), req.TNDDataViewIndex, "");
                                        if (result != ThinkAndDoSuccess)
                                        {
                                            mProject.Window().logMessage("ERROR: adding to tag list. code=" + result + "  type=" + req.TNDDataTypeAsShort() + "  index=" + req.TNDDataViewIndex);
                                            HandleLostConnection();
                                            return;
                                        }
                                    }
                                    mReadRequestsInTagList.Add(req);
                                    lastReq = req;
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
                        }
                        mTimeoutStateIndicator = "exit lock";

                        if (mReadRequestsInTagList.Count > 0)
                        {
                            mTimeoutStateIndicator = "starting read";
                            mStartRead = DateTime.Now;

                            int result = -99911999;
                            try
                            {
                                result = axTndNTag.Read();
                            }
                            catch (COMException comException)
                            {
                                mProject.Window().logMessage("ERROR: COM Exception during TND read attempt. code=" + comException.ErrorCode + "  msg=" + comException.Message);
                            }
                            catch (Exception exception)
                            {
                                mProject.Window().logMessage("ERROR: Exception during TND read attempt. msg=" + exception.Message);
                            }
                            mValuesRead = DateTime.Now;
                            mValuesRetrieval.NewPeriod(mStartRead, mValuesRead);

                            if (result == TNDLink.ThinkAndDoSuccess)
                            {
                                mTimeoutStateIndicator = "finished read successfully";
                                short requestIndex = 0;
                                TNDReadRequest lastReq = null;
                                object valueFromTND = null;
                                foreach (TNDReadRequest req in mReadRequestsInTagList)
                                {
                                    mTimeoutStateIndicator = "processing value " + requestIndex + " of " + mReadRequestsInTagList.Count + ": " + req.TNDDataType + " " + req.TNDDataViewIndex;
                                    if (lastReq == null || req.TNDDataTypeAsShort() != lastReq.TNDDataTypeAsShort() || req.TNDDataViewIndex != lastReq.TNDDataViewIndex)
                                    {
                                        valueFromTND = axTndNTag.GetValueAt(requestIndex);
                                        mTimeoutStateIndicator = "got value";
                                        if (valueFromTND == null)
                                        {
                                            Project().Window().logMessageWithFlush("ERROR: value from TND is null; index=" + requestIndex + "  taglist size=" + mReadRequestsInTagList.Count + "  unique requests=" + mReadRequests_all.Count + "  dirty=" + mTagListDirty);
                                        }
                                        requestIndex++;
                                    }
                                    if (valueFromTND != null)
                                    {
                                        mTimeoutStateIndicator = "going to handler";
                                        req.HandleValue(valueFromTND);
                                    }
                                    mTimeoutStateIndicator = "finished processing value " + requestIndex;
                                    lastReq = req;
                                }
                                mValuesHandled = DateTime.Now;
                                mValuesHandling.NewPeriod(mValuesRead, mValuesHandled);
                            }
                            else
                            {
                                mTimeoutStateIndicator = "finished read with ERRORS";
                                if (result != lastResult)
                                {
                                    mProject.Window().logMessage("ERROR: TND Read failed for '" + Name + "': " + TNDLink.GetTNDResultMessage(result));
                                }
                                HandleLostConnection();
                            }
                            lastResult = result;
                        }
                    } // end thread lock
                }
                catch (Exception e)
                {
                    mProject.Window().logMessage("ERROR: Exception in TND Reader " + Name + "; msg=" + e.Message + Environment.NewLine + e.StackTrace);
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


        public void AddReadRequest(TNDReadRequest theRequest)
        {
            if (!Connected) Connected = true;

//            lock (masterListLock) // updating the tagList requires interating through the master requests list...we can't change the list during iteration...consider a separate masterListLock to minized lock area (e.g. not the entire "updateTagList & read" process
            using (TimedLock.Lock(masterListLock))
            {
                if (mReadRequests_all.Contains(theRequest))
                {
                    throw new ArgumentException("How did we get here?");
                }
                mReadRequests_all.Add(theRequest);
                mReadRequests_all.Sort(ReadRequestSorter.Singleton);

                if (theRequest.Active)
                {
                    mTagListDirty = true;
                }
            }
        }

        public void RemoveReadRequest(TNDReadRequest theRequest)
        {
//            lock (masterListLock) // updating the tagList requires interating through the master requests list...we can't change the list during iteration...consider a separate masterListLock to minized lock area (e.g. not the entire "updateTagList & read" process
            using (TimedLock.Lock(masterListLock))
            {
                mReadRequests_all.Remove(theRequest);

                if (theRequest.Active)
                {
                    mTagListDirty = true;
                }
            }
        }

        [CategoryAttribute("Advanced Debug")]
        public int NumberOfReadRequests_All
        {
            get { return mReadRequests_all.Count; }
        }

        [CategoryAttribute("Advanced Debug")]
        public int NumberOfReadRequests_Active
        {
            get { return mReadRequestsInTagList.Count; }
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

        private List<TNDReadRequest> mReadRequestsInTagList = new List<TNDReadRequest>(); // WARNING: this should only be accessed WITHIN mPollTimer_Elapsed to avoid threading issues
        private List<TNDReadRequest> mReadRequests_all = new List<TNDReadRequest>(); // WARNING: this should only be accessed within a lock of masterListLock...since it is altered by Sequence threads and iterated via our poll timer thread
        //private Object masterListLock = new Object(); moved to TNDnTagLink 9/12/07 since I need it for tagListDirty protection
    }

    public class ReadRequestSorter : System.Collections.Generic.IComparer<TNDReadRequest>
    {
        public static ReadRequestSorter Singleton = new ReadRequestSorter();

        public int Compare(TNDReadRequest x, TNDReadRequest y)
        { // sort read requests by type, then index...used to group duplicate reads
            int typeDiff = x.TNDDataTypeAsShort() - y.TNDDataTypeAsShort();
            if (typeDiff != 0) return typeDiff;
            return x.TNDDataViewIndex - y.TNDDataViewIndex;
        }
    }

    public class TNDnTagReaderConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(((IObjectDefinition)context.Instance).TestSequence().TNDnTagReaderOptions());
            }
            else
            {
                throw new ArgumentException("Unexpected context (3339)");
            }
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(TNDnTagReader))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is TNDnTagReader)
            {

                TNDnTagReader idd = (TNDnTagReader)value;

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
                TNDnTagReader namedReader;

                try
                {
                    string readerName = (string)value;

                    objectBeingEditted = (IObjectDefinition)context.Instance;

                    namedReader = objectBeingEditted.TestSequence().GetTNDnTagReader(readerName);

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

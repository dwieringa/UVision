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
    public abstract class TNDnTagLink : TNDLink
    {
        protected Object masterListLock = new Object(); // NOTE: 9/12/07 this is doubling as a TagListDirty lock; consider a separate lock...basically for tagListDirty, we don't want to allow any test sequences to mark the dirty flag WHILE the taglist is in the middle of being rebuilt (after which it is cleared...in some cases the test sequence's request will be lost)

        public TNDnTagLink(Project theProject)
        {
            mProject = theProject;

            mPollTimer = new System.Timers.Timer(50);
            mPollTimer.Elapsed += new System.Timers.ElapsedEventHandler(mPollTimer_Elapsed);
            mReconnectTimer = new System.Timers.Timer(2000);
            mReconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(mReconnectTimer_Elapsed);
            mTimeoutTimer = new System.Timers.Timer(2000);
            mTimeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(mTimeoutTimer_Elapsed);
            mStatusIndicator = new TNDConnectionStatusIndicator(this);
            mProject.Window().statusBar.Controls.Add(mStatusIndicator);
        }

        protected TNDConnectionStatusIndicator mStatusIndicator;
        public bool ConnectionInitialized { get { return mConnectionInitialized; } }
        private bool mConnectionInitialized = false;
        private void InitializeConnection()
        {
            // moved this from the ctor so that we can make this optional...for PCs that don't have TND installed

            // WARNING: testing for AxInterop.TNDNTAGLib.dll doesn't actually accomplish anything here.  If it doesn't exist this method will throw a FileNotFound exception before even the first line of code is executed...just because it references axTndNTag later on.  I don't know why, but I tested this extensively in the debugger and with trace mechanisms
            // SO! testing whether or not the DLLs exist must be done before this method is called in Connected.set
            //if (!System.IO.File.Exists("AxInterop.TNDNTAGLib.dll") || !System.IO.File.Exists("Interop.TNDNTAGLib.dll"))
            //{
            //    throw new ArgumentException("Think & Do link isn't fully installed.  Connection can not be established.");
            //}

            // TODO: what does "resources" do for us?  Is it ok that I just passed in the type of this class?  should it be a form type?
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgrammingForm));
            axTndNTag = new AxTNDNTAGLib.AxTndNTag();
            ((System.ComponentModel.ISupportInitialize)(axTndNTag)).BeginInit();
            axTndNTag.Enabled = true;
            axTndNTag.Location = new System.Drawing.Point(436, 71);
            axTndNTag.Name = "axTndNTag";
            axTndNTag.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTndNTag.OcxState")));
            axTndNTag.Size = new System.Drawing.Size(32, 41);
            axTndNTag.TabIndex = 0;
            mProject.Window().Controls.Add(axTndNTag);
            //axTndNTag1.Enter += new System.EventHandler(axTndNTag1_Enter);
            ((System.ComponentModel.ISupportInitialize)(axTndNTag)).EndInit();

            mConnectionInitialized = true;
        }

        void mTimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
//            SetTagListDirty(); // HACK: ideally we wouldn't need this.  do we?
            mProject.Window().logMessage("ERROR: TND Link Timeout for " + Name + "; state=" + mTimeoutStateIndicator);
        }

        protected abstract void mPollTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e);

        void mReconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mReconnectTimer.Enabled = false;
            ConnectToRuntime();
            if (Connected)
            {
                mAttemptingReconnect = false;
            }
            else
            {
                mReconnectTimer.Enabled = true;
            }
        }

        protected Project mProject;
        public Project Project()
        {
            return mProject;
        }

        private string mName;
        public override string Name  // TODO: make this object of type ProjectDefinition & IProjectInstance
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// WARNING:
        /// The type AxTNDNTAGLib.AxTndNTag depends on the DLL "AxInterop.TNDNTAGLib.dll".
        /// If the dll can't be found, any method which references axTndNTag will throw a
        /// FileNotFound exception. Even if you put breakpoints or trace statements in the
        /// method they will not be reached -- the exception is throw when the method is
        /// "loaded".  Originally I placed a check to test if the file exists before I
        /// assigned axTndNTag an instance -- the check was never reached.  The same
        /// dependency does not exist for "Interop.TNDNTAGLib.dll" -- that file doesn't
        /// seem to be needed until the instance is created, so the File.Exists check worked.
        /// 
        /// As a solution, I'm performing the File.Exists check BEFORE the calls of the
        /// methods which reference axTndNTag.  I'm also ensuring any of these methods
        /// are covered by a high-level try-catch block.
        /// </summary>
        protected AxTNDNTAGLib.AxTndNTag axTndNTag;
        protected System.Timers.Timer mPollTimer;
        private System.Timers.Timer mReconnectTimer;
        protected System.Timers.Timer mTimeoutTimer;
        public string mTimeoutStateIndicator = "";

        private string mStationName = "";
        public string StationName
        {
            get { return mStationName; }
            set
            {
                mStationName = value.Trim();
                if (mConnected)
                {
                    //axTndNTag.Disconnect();  This "just in case" measure is performed in ConnectToRuntime. I want to limit calls to axTndNTag since they will throw an exception if the DLLs aren't installed
                    ConnectToRuntime();
                }
            }
        }

        public int PollPeriod
        {
            get { return (int)(mPollTimer.Interval); }
            set
            {
                if (value <= 0) throw new ArgumentException("Poll Period must be > 0");
                mPollTimer.Interval = value;
            }
        }

        private bool mAutoReconnectEnabled = true;
        public bool AutoReconnectEnabled
        {
            get { return mAutoReconnectEnabled; }
            set
            {
                mAutoReconnectEnabled = value;
                if (!mAutoReconnectEnabled)
                {
                    if (mAttemptingReconnect)
                    {
                        mProject.Window().logMessage("Auto reconnect disabled for '" + Name + "'");
                    }
                    mAttemptingReconnect = false;
                    mReconnectTimer.Enabled = false;
                }
            }
        }

        public long AutoReconnectPeriod
        {
            get { return (long)mReconnectTimer.Interval; }
            set { mReconnectTimer.Interval = (double)value; }
        }

        private bool mAttemptingReconnect = false;
        private bool mConnected = false;
        public override bool Connected
        {
            get { return mConnected; }
            set
            {
                if (value == false && mConnectionInitialized)
                {
                    DisconnectFromRuntime();
                }
                else if (!mConnected)
                {
                    // TODO: don't try to connect while loading a config file. wait until entire config file loaded.
                    if (!mConnectionInitialized)
                    {
                        // WARNING: DO NOT REFERENCE axTndNTag ANY WHERE IN THIS METHOD or the method will throw a FileNotFound exception if the AxInterop.TNDNTAGLib.dll doesn't exist...even before we execute the first line of this method
                        if (!System.IO.File.Exists("AxInterop.TNDNTAGLib.dll") || !System.IO.File.Exists("Interop.TNDNTAGLib.dll"))
                        {
                            throw new ArgumentException("Think & Do link isn't fully installed.  Connection can not be established.");
                        }

                        InitializeConnection();
                    }

                    //axTndNTag.Disconnect();  This "just in case" measure is performed in ConnectToRuntime. I want to limit calls to axTndNTag since they will throw an exception if the DLLs aren't installed

                    ConnectToRuntime();
                }
            }
        }

        protected bool mTagListDirty = false;
        public void SetTagListDirty()
        {
//            lock (masterListLock) // this method is called by TestSequence threads...if the TND Polling thread is in the middle of rebuidling the TagList, we want to wait so that the dirty flag isn't cleared without adding this sequence's request
            using (TimedLock.Lock(masterListLock))
            {
                mTagListDirty = true;
            }
        }

        protected void DisconnectFromRuntime()
        {
            mProject.Window().logMessage("TND connection closed for '" + Name + "'");

            DisconnectOrCleanup();

            FireTNDLinkDisabled();
        }

        private void DisconnectOrCleanup()
        {
            mPollTimer.Enabled = false;
            if (mConnectionInitialized) // if we're initialized, we know the DLLs exist(ed)
            {
                DisconnectOrCleanup_axTndNTagWrapper();
            }
            mConnected = false;
        }

        /// <summary>
        /// This should ONLY be called by DisconnectOrCleanup().
        /// It serves to keep any reference of axTndNTag out DisconnectOrCleanup(),
        /// since if AxInterop.TNDNTAGLib.dll doesn't exist, any method which references
        /// axTndNTag will throw a FileNotFound exception before the first line of code is
        /// executed.
        /// </summary>
        private void DisconnectOrCleanup_axTndNTagWrapper()
        {
            axTndNTag.Disconnect();
        }

        protected void HandleLostConnection()
        {
            mProject.Window().logMessage("ERROR: TND connection lost for '" + Name + "'");

            DisconnectOrCleanup();

            FireTNDLinkLost();

            if (mAutoReconnectEnabled)
            {
                mAttemptingReconnect = true;
                mReconnectTimer.Enabled = true;
                mProject.Window().logMessage("Auto reconnect intiated");
            }
        }

        protected void ConnectToRuntime()
        {
            mPollTimer.Enabled = false;

            if (!mConnectionInitialized) throw new ArgumentException("ConnectToRuntime was called before the connection was initialized.  How did that happen?");

            // disconnect just in case (we could rely on mConnected, but in an unusual connection failure, there could potentially be some resources allocated inside TND's code even though I don't set mConnected)
            axTndNTag.Disconnect();

            if (axTndNTag.ThinkAndDoStationName != mStationName.Trim())
            {
                axTndNTag.ThinkAndDoStationName = mStationName.Trim();
            }

            int nrc = -99933999;
            try
            {
                nrc = axTndNTag.Connect();
            }
            catch (COMException e)
            {
                mProject.Window().logMessageWithFlush("ERROR: COM Exception during TND connect attempt. code=" + e.ErrorCode + "  msg=" + e.Message);
            }
            catch (Exception e)
            {
                mProject.Window().logMessageWithFlush("ERROR: Exception during TND connect attempt. msg=" + e.Message);
            }

            if (nrc == TNDLink.ThinkAndDoSuccess)
            {
                mProject.Window().logMessage("TND Link Connection Established for '" + Name + "'");

                FireTNDLinkEstablished();

                mConnected = true;
                mPollTimer.Enabled = true;
            }
            else
            {
                if (mAttemptingReconnect)
                {
                    //mProject.Window().logMessage("Reconnect failed: " + GetTNDResultMessage(nrc));
                }
                else
                {
                    mProject.Window().logMessage("Connection attempt failed for '" + Name + "': " + TNDLink.GetTNDResultMessage(nrc));
                    if (mAutoReconnectEnabled)
                    {
                        mAttemptingReconnect = true;
                        mReconnectTimer.Enabled = true;
                        mProject.Window().logMessage("Auto reconnect initiated");
                    }
                }
            }
        }
    }
}

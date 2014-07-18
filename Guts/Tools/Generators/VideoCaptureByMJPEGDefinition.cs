// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NetCams
{
    public class VideoCaptureByMJPEGDefinition : NetCams.VideoGeneratorDefinition
	{
        public VideoCaptureByMJPEGDefinition(TestSequence testSequence)
            : base(testSequence)
		{
			mCapturedVideo = new GeneratedVideoDefinition(testSequence, OwnerLink.newLink(this, "CapturedVideo"));
			mCapturedVideo.AddDependency(this);
			mCapturedVideo.Name = "capturedVideo";

            bufferForWorker = new byte[bufferSize];
        }

        /// <summary>
        /// Buffer used by the background worker of the instance object.  I define it here so there is only one instance of the buffer shared by all instances (only 1 instance active at a time).  This keeps us from continually creating and garbage collecting it every 2 seconds.
        /// TODO: make it a static member of the worker?
        /// </summary>
        public int bufferSize = 512 * 1024;
        public byte[] bufferForWorker;
        
        public override void CreateInstance(TestExecution theExecution)
		{
			new VideoCaptureByMJPEGInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get { return base.ToolMapRow; }
		}

        private NetworkCamera mCamera = null;
        [CategoryAttribute("Input")]
        public NetworkCamera Camera
        {
            get { return mCamera; }
            set 
            {
                HandlePropertyChange(this, "Camera", mCamera, value);
                mCamera = value;
            }
        }

        private int mConnectionTimeout = 3000;
        [CategoryAttribute("Input")]
        public int ConnectionTimeout
        {
            get { return mConnectionTimeout; }
            set 
            {
                HandlePropertyChange(this, "ConnectionTimeout", mConnectionTimeout, value);
                mConnectionTimeout = value;
            }
        }

        private int mStopConditionTimeout = 3000;
        [CategoryAttribute("Input")]
        public int StopConditionTimeout
        {
            get { return mStopConditionTimeout; }
            set 
            {
                HandlePropertyChange(this, "StopConditionTimeout", mStopConditionTimeout, value);
                mStopConditionTimeout = value;
            }
        }

        private bool mAutoSaveEnabled = false;
        [CategoryAttribute("Debug Options")]
        public bool AutoSaveEnabled
        {
            get { return mAutoSaveEnabled; }
            set  
            {
                HandlePropertyChange(this, "AutoSaveEnabled", mAutoSaveEnabled, value);
                mAutoSaveEnabled = value;
            }
        }

        private String mAutoSavePath = "C:\\Runtime Projects\\Vision\\Debug\\";
        [CategoryAttribute("Debug Options")]
        public String AutoSavePath
        {
            get { return mAutoSavePath; }
            set
            {
                HandlePropertyChange(this, "AutoSavePath", mAutoSavePath, value);
                mAutoSavePath = value;
            }
        }

        private GeneratedVideoDefinition mCapturedVideo = null;
		public override GeneratedVideoDefinition ResultantVideo
		{
			get { return mCapturedVideo; }
		}
    }
}

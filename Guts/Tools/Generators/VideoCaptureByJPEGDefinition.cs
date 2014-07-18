// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class VideoCaptureByJPEGDefinition : NetCams.VideoGeneratorDefinition
	{
		public VideoCaptureByJPEGDefinition(TestSequence testSequence) : base(testSequence)
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
			new VideoCaptureByJPEGInstance(this, theExecution);
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

        private int mFrameTimeout = 3000;
        [CategoryAttribute("Input")]
        public int FrameTimeout
        {
            get { return mFrameTimeout; }
            set 
            {
                HandlePropertyChange(this, "FrameTimeout", mFrameTimeout, value);
                mFrameTimeout = value;
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

        private String mAutoSavePath = "Debug\\<TESTSEQ>\\";
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

        /*
        public enum SimulationModes
        {
            Off = 0,
            Directory_Loop = 1,
            Directory_Sequence = 2,
            Directory_Random = 3
        }
        private SimulationModes mSimulationMode = SimulationModes.Off;
        [CategoryAttribute("Simulation Options")]
        public SimulationModes SimulationMode
        {
            get { return mSimulationMode; }
            set { mSimulationMode = value; }
        }

        private string mSimulationSourceDirectory = "C:\\Runtime Projects\\Vision\\Debug\\";// = new DirectoryInfo();
        [CategoryAttribute("Simulation Options")]
        public string SimulationSourceDirectory
        {
            get { return mSimulationSourceDirectory; }
            set
            {
                DirectoryInfo dir = new DirectoryInfo(value);
                mSimulatedImages.Clear();
                foreach (FileInfo file in dir.GetFiles("*.jpg"))
                {
                    mSimulatedImages.Add((Bitmap)(Bitmap.FromFile(file.FullName)));
                }
                mSimulationSourceDirectory = value;
            }
        }
        private List<Bitmap> mSimulatedImages = new List<Bitmap>();
        private int mSimulatedImagesIndex = 0;
        private Random mRandomNumberGenerator = new Random();
        public Bitmap GetNextSimulatedImage(bool loop)
        {
            if (mSimulatedImages.Count == 0)
            {
                throw new ArgumentException("No simulated images available.");
            }
            if (mSimulatedImagesIndex >= mSimulatedImages.Count)
            {
                if (!loop)
                {
                    throw new ArgumentException("All simulated images have been used.");
                }
                mSimulatedImagesIndex = 0;
            }
            return mSimulatedImages[mSimulatedImagesIndex++];
        }

        public Bitmap GetRandomSimulatedImage()
        {
            mSimulatedImagesIndex = mRandomNumberGenerator.Next(0, mSimulatedImages.Count - 1);
            return mSimulatedImages[mSimulatedImagesIndex];
        }
        */

        private GeneratedVideoDefinition mCapturedVideo = null;
		public override GeneratedVideoDefinition ResultantVideo
		{
			get { return mCapturedVideo; }
		}

        public override bool SupportsDragAndDrop() { return true; }

        public override void VerifyValidItemsForDrop(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        public override void HandleDrop(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            /*
            String filename = String.Empty;
            Array data = ((IDataObject)e.Data).GetData("FileName") as Array;
            if (data != null)
            {
                if ((data.Length == 1) && (data.GetValue(0) is String))
                {
                    filename = ((string[])data)[0];
                    string ext = Path.GetExtension(filename).ToLower();
                    if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp"))
                    {
                        // load image from disk
                        Bitmap theImage = new Bitmap(filename);

                        // find the instance object in the active test execution
                        ObjectInstance theObject = Sequence().ActiveTestExecution().GetObject(Name);
                        if (!(theObject is CameraSnapshotInstance))
                        {
                            throw new Exception("Name conflict: snapshot name isn't unique oiioejroiwjer");
                        }
                        CameraSnapshotInstance theSnapshotInstance = (CameraSnapshotInstance)theObject;

                        // set result image of instance in active test execution
                        theSnapshotInstance.SetResultantImage(theImage);

                        // set trigger of active test execution
                        Sequence().ActiveTestExecution().TriggerFired = true;
                        Sequence().ActiveTestExecution().Name = "Drag & drop test at " + DateTime.Now;
                    }
                }
            }
            */
        }
    }
}

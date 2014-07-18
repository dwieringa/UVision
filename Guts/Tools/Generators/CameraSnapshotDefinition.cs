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
	public class CameraSnapshotDefinition : NetCams.ImageGeneratorDefinition, ITriggerDefinition
	{
		public CameraSnapshotDefinition(TestSequence testSequence) : base(testSequence)
		{
			mSnapshotImage = new GeneratedImageDefinition(testSequence, OwnerLink.newLink(this, "ResultantImage"));
			mSnapshotImage.AddDependency(this);
			mSnapshotImage.Name = "snapshotResult";

            bufferForWorker = new byte[bufferSize];

            testSequence.TriggerRegistry.RegisterObject(this);
		}

        public override void Dispose_UVision()
        {
            TestSequence().TriggerRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        /// <summary>
        /// Buffer used by the background worker of the instance object.  I define it here so there is only one instance of the buffer shared by all instances (only 1 instance active at a time).  This keeps us from continually creating and garbage collecting it every 2 seconds.
        /// TODO: make it a static member of the worker?
        /// </summary>
        public int bufferSize = 512 * 1024;//zxz
        public byte[] bufferForWorker;

		public override void CreateInstance(TestExecution theExecution)
		{
			new CameraSnapshotInstance(this, theExecution);
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

        private bool mTriggerEnabled = true;
        public bool TriggerEnabled
        {
            get
            {
                return mTriggerEnabled;
            }
            set
            {
                HandlePropertyChange(this, "TriggerEnabled", mTriggerEnabled, value);
                mTriggerEnabled = value;
            }
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

        private string mResolution;
        [CategoryAttribute("Input"),
        TypeConverter(typeof(ImageResolutionConverter))]
        public string Resolution
        {
            get { return mResolution; }
            set
            {
                HandlePropertyChange(this, "Resolution", mResolution, value);
                mResolution = value;
            }
        }

        private int mTimeout = 3000;
        [CategoryAttribute("Input")]
        public int Timeout
        {
            get { return mTimeout; }
            set 
            {
                HandlePropertyChange(this, "Timeout", mTimeout, value);
                mTimeout = value;
            }
        }

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
            set 
            {
                HandlePropertyChange(this, "SimulationMode", mSimulationMode, value);
                mSimulationMode = value;
                mSimulatedImages = null; // free up memory if turned off...force refresh of directory if turned on
            }
        }

        private string mSimulationSourceDirectory = String.Empty;
        [CategoryAttribute("Simulation Options")]
        public string SimulationSourceDirectory
        {
            get { return mSimulationSourceDirectory; }
            set
            {
                HandlePropertyChange(this, "SimulationSourceDirectory", mSimulationSourceDirectory, value);
                mSimulationSourceDirectory = value;

                // clear the list each time the directory is changed...only load when needed (after trigger)
                mSimulatedImages = null;
            }
        }
        public static readonly int MAX_SIMULATED_IMAGES = 100;
        private void LoadSimulationImages()
        {
            TestSequence().Log("Loading simulation images for '" + Name + "'...");
            string directoryPath = mSimulationSourceDirectory;
            DirectoryInfo dir = new DirectoryInfo(mSimulationSourceDirectory);
            if (mSimulatedImages != null)
            {
                mSimulatedImages.Clear();
            }
            mSimulatedImages = new List<Bitmap>();
            int imageCount = 0;
            foreach (FileInfo file in dir.GetFiles("*.jpg"))
            {
                mSimulatedImages.Add((Bitmap)(Bitmap.FromFile(file.FullName)));
                imageCount++;
                if (imageCount > MAX_SIMULATED_IMAGES)
                {
                    throw new ArgumentException("Too many images in simulation directory for '" + Name + "'.  Limit=" + MAX_SIMULATED_IMAGES);
                }
            }
            TestSequence().Log("Done loading simulation images");
        }
        private List<Bitmap> mSimulatedImages = null;
        private int mSimulatedImagesIndex = 0;
        private Random mRandomNumberGenerator = new Random();
        public Bitmap GetNextSimulatedImage(bool loop)
        {
            if (mSimulatedImages == null) LoadSimulationImages();

            if (mSimulatedImages == null || mSimulatedImages.Count == 0)
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
            if (mSimulatedImages == null) LoadSimulationImages();

            mSimulatedImagesIndex = mRandomNumberGenerator.Next(0, mSimulatedImages.Count - 1);
            return mSimulatedImages[mSimulatedImagesIndex];
        }

        private GeneratedImageDefinition mSnapshotImage = null;
		public override GeneratedImageDefinition ResultantImage
		{
			get { return mSnapshotImage; }
		}

        public override bool SupportsDragAndDrop()
        {
            return mTriggerEnabled;
            //return Sequence().ActiveTestExecution() != null && !Sequence().ActiveTestExecution().TriggerFired;
        }

        public override void VerifyValidItemsForDrop(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        protected Object queueLock = new Object(); // NOTE: 9/12/07 this is doubling as a TagListDirty lock; consider a separate lock...basically for tagListDirty, we don't want to allow any test sequences to mark the dirty flag WHILE the taglist is in the middle of being rebuilt (after which it is cleared...in some cases the test sequence's request will be lost)
        private Queue<Bitmap> RetestImages = new Queue<Bitmap>();
        public override void SimulateGeneratingImage(Bitmap theImage)
        {
            using (TimedLock.Lock(queueLock))
            {
                RetestImages.Enqueue(theImage);
            }
        }
        public int NumberOfImagesAvailableForRetesting()
        {
            using (TimedLock.Lock(queueLock))
            {
                return RetestImages.Count;
            }
        }
        public Bitmap GetImageToRetest()
        {
            using (TimedLock.Lock(queueLock))
            {
                return RetestImages.Dequeue();
            }
        }

        public Queue<string> DragAndDropFileNames = new Queue<string>();
        public override void HandleDrop(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            // only handle this if we have an active
            if (TestSequence().ActiveTestExecution() == null )
            {
                MessageBox.Show("Test Sequence isn't active. Is it enabled?");
                return;
            }

            // http://www.jonasjohn.de/snippets/csharp/drag-and-drop-example.htm
            // Extract the data from the DataObject-Container into a string list
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if( FileList != null )
            {
                foreach (string File in FileList)
                {
                    DragAndDropFileNames.Enqueue(File);
                }
                /*
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
                        theSnapshotInstance.ProcessNewImage(theImage, filename);

                        // set trigger of active test execution; WARNING! WE ARE OPERATING IN THE GUI THREAD AND trigger is watched from Test Sequence thread
                        Sequence().ActiveTestExecution().Name = "Drag & drop test at " + DateTime.Now;
                        if (Sequence().HasUnusedChanges)
                        {
                            Sequence().ActiveTestExecution().MarkNeedToRecreateExecution();
                        }
                        else
                        {
                            Sequence().ActiveTestExecution().TriggerFired = true;
                        }
                    }
                }*/
            }
        }
    }
    public class ImageResolutionConverter : StringConverter
    {
        public static string[] ImageResolutionOptions = new string[] { "1280x1024", "1280x960", "640x480", "320x240", "160x120" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(ImageResolutionOptions);
            }
            else
            {
                throw new ArgumentException("why are we here? 934483");
            }
        }
        /*
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(NetworkCamera))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is NetworkCamera)
            {

                NetworkCamera idd = (NetworkCamera)value;

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
                NetworkCamera namedCamera;

                try
                {
                    string cameraName = (string)value;

                    objectBeingEditted = (IObjectDefinition)context.Instance;

                    namedCamera = objectBeingEditted.Project().FindCamera(cameraName);

                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to a camera");
                }
                return namedCamera;
            }
            return base.ConvertFrom(context, culture, value);
        }
         */
    }
}

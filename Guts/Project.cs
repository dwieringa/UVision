// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class Project : TestContext
	{
        public TNDnTagReader globalTNDReader;
        public TNDnTagWriter globalTNDWriter;
        public ModbusTCPSlaveDevice globalModbusSlave;

		private ProgrammingForm mWindow;
		public Project(ProgrammingForm window)
		{
			mWindow = window;
            mDataValueRetentionFile = new DataValueRetentionFile(this);
            //globalTNDReader = new TNDnTagReader(this);
            //globalTNDWriter = new TNDnTagWriter(this);//zzz
        }

        private String mName = "Untitled Project";
        public override String Name
        {
            get { return mName; }
            set
            {
                mName = value;
                mWindow.Text = "UVision - " + mName;
            }
        }
        public override Project project() { return this; }

        public void CleanupForShutdown()
        {
            mDataValueRetentionFile.CloseFileFromWriting();
        }

        public void SimulateLoadingProjectFromConfigFile(TestSequence theTestSequence)
        {
//            TNDnTagFlagTriggerDefinition newTNDTrigger = new TNDnTagFlagTriggerDefinition(theTestSequence);

        }

        private string mDefinitionFile;
        public string DefinitionFile
        {
            set { mDefinitionFile = value; }
            get { return mDefinitionFile; }
        }

        public void loadSettings(string filename)
        {
            ProjectConfigFile defFile = new ProjectConfigFile(filename);
            DefinitionFile = filename;
            SplashScreen.SetStatus("Loading project file " + DefinitionFile);
            defFile.LoadObject(this);

            // If any of the GlobalValues within the Project are Retentive, then load the retentive file
            // TODO: need to check if any TestSequence values are retentive???  investigate...
            foreach (GlobalValue globalValue in mGlobalValues)
            {
                if (globalValue.IsRetentive)
                {
                    mDataValueRetentionFile.LoadDataFromFile();
                    break; // only load it once
                }
            }


            foreach (TestSequence seq in mTestSequences)
            {
                seq.SetFullyInitialized();
            }
            SetFullyInitialized();

            mWindow.logMessage("Settings loaded successfully");
        }

        public void saveSettings()
        {
            ProjectConfigFile defFile = new ProjectConfigFile(mDefinitionFile);
            defFile.OpenFileForWriting();

            if( globalTNDReader != null ) defFile.AddObject(globalTNDReader); // TestSequences are dependent on these, so they must come first
            if( globalTNDWriter != null ) defFile.AddObject(globalTNDWriter);
            if (globalModbusSlave != null) defFile.AddObject(globalModbusSlave);

            foreach (Camera anObject in cameras)
            {
                defFile.AddObject(anObject);
            }

            foreach (GlobalValue anObject in mGlobalValues)
            {
                defFile.AddObject(anObject);
            }

            foreach (TestSequence anObject in mTestSequences)
            {
                defFile.AddObject(anObject);
            }

            defFile.AddObject(this);

            defFile.CloseFileFromWriting();

            mWindow.logMessage("Settings saved successfully");
        }

        public ProgrammingForm Window()
		{
			return mWindow;
		}

        public TestSequence FindSequence(string theName)
        {
            foreach (TestSequence seq in mTestSequences)
            {
                if (seq.Name == theName)
                {
                    return seq;
                }
            }
            throw new ArgumentException("Test Sequence '" + theName + "' does not exist.");
        }
        public void RegisterTestSequence(TestSequence theSequence)
        {
            mTestSequences.Add(theSequence);
            TreeNode newNode = theSequence.GetNewTreeNode();
            Window().sequencesNode.Nodes.Add(newNode);
            if (NewTestSequence != null)
            {
                NewTestSequence(theSequence);
            }
        }
        public void UnregisterTestSequence(TestSequence theSequence)
        {
            mTestSequences.Remove(theSequence);
            // TODO: ensure the dtor of TestSequence loops through all of it's TreeNodes and does a node.TreeView.Nodes.Remove(node)
            if (TestSequenceDisposed != null)
            {
                TestSequenceDisposed(theSequence);
            }
        }
        public event TestSequence.TestSequenceDelegate NewTestSequence;
        public event TestSequence.TestSequenceDelegate TestSequenceDisposed;

        private List<NetworkCamera> cameras = new List<NetworkCamera>();
        public NetworkCamera FindCamera(string theName)
        {
            foreach (NetworkCamera cam in cameras)
            {
                if (cam.Name == theName)
                {
                    return cam;
                }
            }
            throw new ArgumentException("Camera '" + theName + "' does not exist.");
        }
        public void RegisterCamera(NetworkCamera theCamera, bool makeNameUnique)
        {
            if(!IsCameraNameUnique(theCamera.Name))
            {
                if (makeNameUnique)
                {
                    int uniquenessSuffix = 1;
                    while (!IsCameraNameUnique(theCamera.Name + " " + uniquenessSuffix))
                    {
                        uniquenessSuffix++;
                    }
                    theCamera.Name = theCamera.Name + " " + uniquenessSuffix;
                }
                else
                {
                    throw new ArgumentException("Another camera already exists with the name '" + theCamera.Name + "'.");
                }
            }
            cameras.Add(theCamera);
        }
        public bool IsCameraNameUnique(string theName)
        {
            theName = theName.Trim();
            foreach (NetworkCamera cam in cameras)
            {
                if (theName == cam.Name)
                {
                    return false;
                }
            }
            return true;
        }
        public string[] CameraOptions()
        {
            string[] options = new string[cameras.Count];
            int i = 0;
            foreach (NetworkCamera cam in cameras)
            {
                options[i++] = cam.Name;
            }
            return options;
        }
        /// <summary>
        /// TODO: We want to get rid of this (don't want to store (single) tree node in camera since it may appear in multiple tree views
        /// </summary>
        /// <param name="cameraNode"></param>
        /// <returns></returns>
        public NetworkCamera FindCamera(TreeNode cameraNode)
        {
            // If the camera form is opened and a new ROI is selected and not deleted make it blink  
            foreach (NetworkCamera camera in cameras)
            {
                if (camera.treeNode == cameraNode)
                {
                    return camera;
                }
            }
            return null;
        }

        public ArrayList mTestSequences = new ArrayList(10);

        private TestSequence mSelectedTestSequence = null;
        public TestSequence SelectedTestSequence
        {
            get { return mSelectedTestSequence; }
            set
            {
                mSelectedTestSequence = value;
                /* mSelectedTestSequence.RebuildToolGrid(); NOTE: 2/19/2008:
                   // TODO: CLEAN THIS UP
                 * I commented this out since it was causing an error while loading the Project.ini file for Red Bull.  This is the first time it was a problem.  Maybe because I only had 1 Test Sequence (and 1 camera)??? 
                 * This was how I was making a certain sequence be selected at startup and calling Rebuild refreshed the screen
                 * basically it was complaining that a collection had change....how could this be?
                 * 
                 * I don't really like this feature.  It seems most obvious that no test sequence would be selected at startup.
                 * 
                 * DECIDED 2/19/08 TO SIMPLE *NOT* SAVE THIS PROPERTY TO THE PROJECT FILE
                 */
            }
        }

//		public TestSequence SelectedTestSequence() { return mSelectedTestSequence; }

		public void AdjustTestSequenceOrder(TestSequence changedSequence, int requestedPosition)
		{
			// remove the changedSequence from the array (so it isn't processed in recursive calls)

			// loop thru the array and if another sequence is at the requested position, then recursively adjust it's position by +1

			// insert the changedSequence
		}

        public ProcessPriorityClass ProcessPriority
        {
            get { return Process.GetCurrentProcess().PriorityClass; }
            set
            {
                if (value == ProcessPriorityClass.RealTime)
                {
                    throw new ArgumentException("Process priority of 'RealTime' isn't supported.");
                }
                Process.GetCurrentProcess().PriorityClass = value;
            }
        }

        private string mDefaultOperatorViewLayoutFile = "";
        public string DefaultOperatorViewLayoutFile
        {
            get { return mDefaultOperatorViewLayoutFile; }
            set { mDefaultOperatorViewLayoutFile = value; }
        }

        [CategoryAttribute("Window")]
        private int mWindowLeft = 10;
        public int WindowLeft
        {
            get
            {
                // improve with this? http://www.codeproject.com/KB/miscctrl/FormState.aspx
                if (mWindow.WindowState == FormWindowState.Normal) mWindowLeft = mWindow.Left;
                return mWindowLeft;
            }
            set
            {
                mWindowLeft = value;
                mWindow.Left = mWindowLeft;
            }
        }
        [CategoryAttribute("Window")]
        private int mWindowTop = 10;
        public int WindowTop
        {
            get
            {
                if (mWindow.WindowState == FormWindowState.Normal) mWindowTop = mWindow.Top;
                return mWindowTop;
            }
            set
            {
                mWindowTop = value;
                mWindow.Top = mWindowTop;
            }
        }
        [CategoryAttribute("Window")]
        private int mWindowWidth = 200;
        public int WindowWidth
        {
            get
            {
                if (mWindow.WindowState == FormWindowState.Normal) mWindowWidth = mWindow.Width;
                return mWindowWidth;
            }
            set
            {
                mWindowWidth = value;
                mWindow.Width = mWindowWidth;
            }
        }
        [CategoryAttribute("Window")]
        private int mWindowHeight = 200;
        public int WindowHeight
        {
            get
            {
                if (mWindow.WindowState == FormWindowState.Normal) mWindowHeight = mWindow.Height;
                return mWindowHeight;
            }
            set
            {
                mWindowHeight = value;
                mWindow.Height = mWindowHeight;
            }
        }
    }

    public interface ProjectComponent
    {
        Project project();
    }
}

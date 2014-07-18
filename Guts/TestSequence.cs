// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Management;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
    [TypeConverter(typeof(TestSequenceConverter))]
    public class TestSequence : TestContext, ProjectComponent
	{
        public TestSequence(Project theProject)
		{
			mProject = theProject;
            Name = "Untitled Test Sequence";

            DataValueRegistry = new DataValueDefinitionRegistry(this, "DataValue"); 
            mTestExecutionTimeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(mTestExecutionTimeoutTimer_Elapsed);

            project().RegisterTestSequence(this);
            
            mDataValueRetentionFile = new DataValueRetentionFile(this);

            mThread = new Thread(new ThreadStart(Go));
        }

        public void Log(string msg)
        {
            Window().logMessage(this.Name + " : " + msg);
        }

        private bool mHasUnusedChanges = false;
        public bool HasUnusedChanges()
        {
            return mHasUnusedChanges;
        }
        public void SetUnusedChanges()
        {
            mHasUnusedChanges = true;
        }

        private bool mHasUnsavedChanges = false;
        public bool HasUnsavedChanges()
        {
            return mHasUnsavedChanges; 
        }
        public void SetUnsavedChanges()
        {
            mHasUnsavedChanges = true;
        }

        public bool RecreateAndTriggerExecution = false;
        public string NameOfExecutionAfterRecreating = string.Empty;

        void mTestExecutionTimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mTestExecutionTimeoutTimer.Enabled = false;

            if (mActiveExecution == null)
            {
                Window().logMessage("ERROR: received a execution timeout alert with no active execution.");
                return;
            }

            mActiveExecution.LogMessageWithTimeFromTrigger("ERROR: ABORTING EXECUTION DUE TO SEQUENCE TIMEOUT");
            Window().logMessage("ERROR: Aborting execution of " + Name + " due to timeout");

            //get Process objects
            System.Management.ObjectQuery oQuery = new System.Management.ObjectQuery("SELECT * FROM Win32_Process WHERE name='UVision.exe'");

            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(oQuery);

            ManagementObjectCollection oReturnCollection = objSearcher.Get();
            foreach (ManagementObject oReturn in oReturnCollection)
            {/*
                //Name of process
                Console.WriteLine(oReturn["Name"].ToString().ToLower());
                //arg to send with method invoke to return user and domain - below is link to SDK doc on it
                string[] o = new String[2];
                //Invoke the method and populate the o var with the user name and domain
                oReturn.InvokeMethod("GetOwner", (object[])o);
                //write out user info that was returned
                Console.WriteLine("PID: " + oReturn["ProcessId"].ToString());
                //get priority
                if (oReturn["Priority"] != null)
                    Console.WriteLine("Priority: " + oReturn["Priority"].ToString());
                
                //get creation date - need managed code function to convert date -
                if (oReturn["CreationDate"] != null)
                {
                    //get datetime string and convert
                    string s = oReturn["CreationDate"].ToString();
                    //see ToDateTime function in sample code
                    DateTime dc = DateTime.Parse(s);
                    //write out creation date
                    Console.WriteLine("CreationDate: " + dc.AddTicks(-TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Ticks).ToLocalTime().ToString());
                }
                */

                //this is the amount of memory used
                if (oReturn["WorkingSetSize"] != null)
                {
                    long mem = Convert.ToInt64(oReturn["WorkingSetSize"].ToString()) / 1024;
                    Window().logMessage("mem usage: " + mem);
                    //Console.WriteLine("Mem Usage: {0:#,###.##}Kb", mem);
                }
            }

            mThread.Abort();
        }

        public delegate void TestSequenceDelegate(TestSequence testSequence);

        public FavoriteSettings mFavoriteSettings = null;
        public List<string> mFavoriteSettingDefs = new List<string>();
        public FavoriteValues mFavoriteValues = null;
        public List<string> mFavoriteValuesDefs = new List<string>();
        public List<string> mImageForms = new List<string>();

        public TreeNode GetNewTreeNode()
        {
            TreeNode newNode = new TreeNode();
            newNode.Text = Name;
            newNode.Tag = this;

            treeNodes.Add(newNode); // the sequence needs to know about all nodes so that it can update the tree node text

            foreach (TestExecutionCollection collection in mTestCollections)
            {
                newNode.Nodes.Add(collection.GetNewTreeNode());
            }

            newNode.Expand();

            return newNode;
        }
        public void RegisterTestCollection(TestExecutionCollection collection)
        {
            mTestCollections.Add(collection);
            foreach (TreeNode seqNode in treeNodes)
            {
                seqNode.Nodes.Add(collection.GetNewTreeNode());
                seqNode.Expand();
            }
        }
        public TestExecution ActiveTestExecution() { return mActiveExecution; }
        private List<TreeNode> treeNodes = new List<TreeNode>();
        private TestExecution mActiveExecution = null;
        public List<TestExecutionCollection> mTestCollections = new List<TestExecutionCollection>();
		private Project mProject;
		public override Project project() { return mProject; }
        public ProgrammingForm Window() { return mProject.Window(); }
        public Thread mThread;
        public DefinitionObjectRegistry<IToolDefinition> ToolRegistry = new DefinitionObjectRegistry<IToolDefinition>("Tool");
        public MasterObjectRegistry ObjectRegistry = new MasterObjectRegistry("Object");
        public DataValueDefinitionRegistry DataValueRegistry;
        public DefinitionObjectRegistry<ROIDefinition> ROIRegistry = new DefinitionObjectRegistry<ROIDefinition>("ROI");
        public DefinitionObjectRegistry<IRectangleROIDefinition> RectangleROIRegistry = new DefinitionObjectRegistry<IRectangleROIDefinition>("RectangleROI");
        public DefinitionObjectRegistry<IReferencePointDefinition> ReferencePointRegistry = new DefinitionObjectRegistry<IReferencePointDefinition>("ReferencePoint");
        public DefinitionObjectRegistry<LineDecorationDefinition> LineDecorationRegistry = new DefinitionObjectRegistry<LineDecorationDefinition>("LineDecoration");
        public DefinitionObjectRegistry<CircleDecorationDefinition> CircleDecorationRegistry = new DefinitionObjectRegistry<CircleDecorationDefinition>("CircleDecoration");
        public DefinitionObjectRegistry<IDecorationDefinition> DecorationRegistry = new DefinitionObjectRegistry<IDecorationDefinition>("Decoration");
        public DefinitionObjectRegistry<ScoreFilterDefinition> ScoreFilterRegistry = new DefinitionObjectRegistry<ScoreFilterDefinition>("ScoreFilter");
        public DefinitionObjectRegistry<ImageScorerDefinition> ImageScorerRegistry = new DefinitionObjectRegistry<ImageScorerDefinition>("ImageScorer");
        public DefinitionObjectRegistry<ImageDefinition> ImageRegistry = new DefinitionObjectRegistry<ImageDefinition>("Image");
        public DefinitionObjectRegistry<VideoDefinition> VideoRegistry = new DefinitionObjectRegistry<VideoDefinition>("Video");
        public DefinitionObjectRegistry<ColorMatchDefinition> ColorMatchRegistry = new DefinitionObjectRegistry<ColorMatchDefinition>("ColorMatch");
        public DefinitionObjectRegistry<MathOpResultDefinition> MathOperationRegistry = new DefinitionObjectRegistry<MathOpResultDefinition>("MathOperation");
        public DefinitionObjectRegistry<CalculationToolDefinition> CalculationToolRegistry = new DefinitionObjectRegistry<CalculationToolDefinition>("CalculationTool");
        public DefinitionObjectRegistry<ITriggerDefinition> TriggerRegistry = new DefinitionObjectRegistry<ITriggerDefinition>("Trigger");


        private String mName = "test Seq";
        public override String Name
        {
            get { return mName; }
            set
            {
                mName = value;
                foreach (TreeNode node in treeNodes)
                {
                    node.Text = mName;
                }
            }
        }

        private bool mEnabled = true;
        public bool Enabled
        {
            get { return mEnabled; }
            set
            {
                mEnabled = value;
                if (!mEnabled)
                {
                    foreach (TreeNode node in treeNodes)
                    {
                        node.BackColor = Color.Red;
                    }
                }
                else
                {
                    foreach (TreeNode node in treeNodes)
                    {
                        node.BackColor = Color.White;
                    }
                }
            }
        }

        private string mOperatorViewLayoutFile = "";
        public string OperatorViewLayoutFile
        {
            get { return mOperatorViewLayoutFile; }
            set { mOperatorViewLayoutFile = value; }
        }

        private string mDefinitionFile;
        public string DefinitionFile
        {
            get { return mDefinitionFile; }
            set
            {
                mDefinitionFile = value;
                if (ObjectRegistry.Count == 0) LoadDefinition();
            }
        }

        public void SaveDefinition()
        {
            TestSequenceConfigFile defFile = new TestSequenceConfigFile(mDefinitionFile);
            defFile.OpenFileForWriting();
            foreach (GlobalValue anObject in mGlobalValues)
            {
                defFile.AddObject(anObject);
            }
            foreach (IObjectDefinition anObject in ObjectRegistry.ObjectList())
            {
                if (anObject.IncludeObjectInConfigFile())
                {
                    defFile.AddObject(anObject);
                }
            }
            foreach (TestExecutionCollection collection in mTestCollections)
            {
                defFile.AddObject(collection);
            }
            defFile.SaveFavoriteSettings(this);
            defFile.SaveFavoriteValues(this);
            defFile.SaveImageFormSettings(this);
            defFile.CloseFileFromWriting();
            mHasUnsavedChanges = false;
        }

        public void LoadDefinition()
        {
            SplashScreen.SetStatus("Loading test sequence file " + mDefinitionFile);
            TestSequenceConfigFile defFile = new TestSequenceConfigFile(mDefinitionFile);
            //mObjectSpace.Clear(); // TODO: what do I really need to do to flush out the current sequence? clear execution array too?  stop thread?  maybe dispose sequence and create a new one!
            ObjectRegistry.Purge();
            defFile.LoadObject(this);
            RebuildToolGrid();
            mThread.Start();
        }

        public void CleanupForShutdown()
        {
            mDataValueRetentionFile.CloseFileFromWriting();
        }

        // TODO: store stats on computation times of each tool...number of times ran vs number of test executions, min time, max time, average time, mean time




        public override GlobalValue GetGlobalValueIfExists(string theName)
        {
            GlobalValue result = base.GetGlobalValueIfExists(theName);
            if (result == null) result = mProject.GetGlobalValueIfExists(theName);
            return result;
        }

        public override GlobalValue GetGlobalValue(string theName)
        {
            GlobalValue result = base.GetGlobalValueIfExists(theName);
            if (result == null) result = mProject.GetGlobalValue(theName);
            return result;
        }

        public string[] GlobalValueOptions()
        {
            string[] options = new string[mGlobalValues.Count + mProject.mGlobalValues.Count];
            int i = 0;
            foreach (GlobalValue globalValue in mGlobalValues)
            {
                options[i++] = globalValue.ToString();
            }
            foreach (GlobalValue globalValue in mProject.mGlobalValues)
            {
                options[i++] = globalValue.ToString();
            }
            return options;
        }

        public List<OperatorControlDefinition> mOperatorControlDefiners = new List<OperatorControlDefinition>();
        public void RegisterOperatorControlDefiner(OperatorControlDefinition theOperatorControlDefiner)
        {
            mOperatorControlDefiners.Add(theOperatorControlDefiner);
        }

        public string[] TNDnTagReaderOptions()
        {
            string[] options = new string[1];
            options[0] = project().globalTNDReader.ToString();
            return options;
        }
        public TNDnTagReader GetTNDnTagReader(string theName)
        {
            return project().globalTNDReader;
        }

        public string[] TNDnTagWriterOptions()
        {
            string[] options = new string[1];
            options[0] = project().globalTNDWriter.ToString();
            return options;
        }
        public TNDnTagWriter GetTNDnTagWriter(string theName)
        {
            return project().globalTNDWriter;
        }

        

        public void GarbageCollectMathOperations()
        {
            // changed 5/6/2008 from checking dependencies of CalculationToolDefinitions to all tools to support dependencies from Prerequisites...I want to be able to type in calculations in Prerequisite fields and have CalcTools be temporarilly created only to support creation of the result value
            // TODO: optimize by testing CalcTool/CalculatedValueDef's first since they are the most likely...and some tools can have a many and deep dependencies?

            // changed 11/7/2008 from checking dependencies of all tools to all objects to support dependencies from Calculations in CalculatedValueDefinition

            // determine which ComputedValueDefinition's and their corresponding MathOperationDefinition's are no longer referenced by at least 1 CalculationToolDefinition
            List<MathOpResultDefinition> mathOpResultsToRemove = new List<MathOpResultDefinition>();
            bool dependencyExists = false;
            foreach (MathOpResultDefinition value in MathOperationRegistry.ObjectList())
            {
                dependencyExists = false;
                //foreach (CalculationToolDefinition calcTool in mCalcToolDefs)
                //foreach (ToolDefinition tool in ToolRegistry.ObjectList())
                foreach( ObjectDefinition def in ObjectRegistry.ObjectList())
                {
                    if (def.IsDependentOn(value))
                    {
                        dependencyExists = true;
                        break;
                    }
                }
                if (!dependencyExists)
                {
                    mathOpResultsToRemove.Add(value);
                }
            }

            // remove the unneeded values & their math ops
            foreach (MathOpResultDefinition value in mathOpResultsToRemove)
            {
                value.MathOperation().Dispose_UVision();
            }
        }

        private System.Timers.Timer mTestExecutionTimeoutTimer = new System.Timers.Timer(5000);
        public long ExecutionTimeoutPeriod
        {
            get { return (long)mTestExecutionTimeoutTimer.Interval; }
            set { mTestExecutionTimeoutTimer.Interval = value; }
        }


        private TimeSpan executionPeriod;
        private long nextTickTime = 0;
        public void Go() // TODO: consider using BackgroundWorker: http://www.albahari.com/threading/part3.html#_BackgroundWorker
		{
            while (!Window().shuttingDown)
            {
                try
                {
                    while (!Window().shuttingDown) // NOTE: this loop exists both inside and outside the try-catch, because...  We don't ever want to exit the try-catch block because the thread would terminate if we recieved a ThreadAbort while outside...but we need the outside loop to recover from any exceptions
                    {
                        if (!Enabled)
                        {
                            if (mActiveExecution != null)
                            {
                                mActiveExecution.LogMessage("Aborting test execution since sequence has been disabled.");
                                CleanupActiveTestExecution();
                            }
                            Thread.Sleep(100);
                        }
                        else
                        {
                            // if no active executor, create one
                            if (mActiveExecution == null)
                            {
                                CreateExecutor();
                                if (mActiveExecution == null)
                                {
                                    break;
                                }
                            }

                            // DEBUG: indicate the thread is still active (useful in debugging some timeout situations)
                            executionPeriod = mActiveExecution.CreatedTime - DateTime.Now;
                            if (executionPeriod.TotalMilliseconds > nextTickTime)
                            {
                                mActiveExecution.LogMessage("tick");
                                nextTickTime += 1000;
                            }

                            // executor.Go()
                            mActiveExecution.Go();
                            if (RecreateAndTriggerExecution)
                            {
                                CleanupActiveTestExecution();
                                CreateExecutor();
                                if (mActiveExecution == null)
                                {
                                    LogError("Unable to recreate execution");
                                    break;
                                }
                                mActiveExecution.TriggerFired = true;
                                mActiveExecution.Name = NameOfExecutionAfterRecreating;
                                NameOfExecutionAfterRecreating = string.Empty;
                                RecreateAndTriggerExecution = false;
                            }

                            // if executor.isDone() process it for stats, store it for history and set executor to null
                            if (mActiveExecution.IsComplete())
                            {
                                foreach (TestExecutionCollection collection in mTestCollections)
                                {
                                    collection.AddExecution(mActiveExecution);
                                }
                                CleanupActiveTestExecution();
                            }
                        }
                    } // end while
                }
                catch (ArgumentException e)
                {
                    string message = "Exception while performing test. msg='" + e.Message + "'" + Environment.NewLine + e.StackTrace + Environment.NewLine + "source=" + e.Source + Environment.NewLine + "target=" + e.TargetSite + Environment.NewLine + e.GetType().Name;
                    Window().logMessageWithFlush(message);
                    if (DialogResult.Cancel == MessageBox.Show(message, "Exception", MessageBoxButtons.RetryCancel))
                    {
                        Enabled = false;
                    }
                    CleanupActiveTestExecution(); // give up on this execution; TODO: save images for debug?
                }
                catch (ThreadAbortException e)
                { // understanding ThreadAbortException: http://www.albahari.com/threading/part2.html#_Unblocking
                    // We get here via 1 of 2 ways:
                    // 1) The TestExecution timed out, so TestSequence is aborting this thread to get it unstuck
                    // 2) The application is being shutdown and all threads are being killed

                    if (!Window().shuttingDown)
                    {
                        Window().logMessage("Sequence received an abort request");

                        if (mActiveExecution != null)
                        {
                            Window().logMessage("START OF LOG CONTENTS OF ABORTED TEST EXECUTION FOR " + Name + ":");
                            Window().logMessage(mActiveExecution.GetLogText());
                            Window().logMessageWithFlush("END OF LOG CONTENTS OF ABORTED TEST EXECUTION FOR " + Name);

                            // TODO: store images for debug?  (create a "dump" method which stores all test execution & sequence settings and images)
                            CleanupActiveTestExecution();
                        }
                    }
                    Thread.ResetAbort(); // ensure the abort exception isn't rethrown (it is by default to kill the thread)...we will drop down and exit the flow gracefully if the app is being shutdown
                }
//#if !DEBUG
                catch (InvalidOperationException ioe)
                {
                    // I was getting these sometimes when new tests were added while I was probing or something.  So the test sequence thread was updating a PictureBox.Image while the GUI thread was manipulating it.  I fixed this by using InvokeRequired in OpForm.NewTest()
                    string message = "Got InvalidOperationException while executing a test.  Need BeginInvoke?  Is test execution manipulating the GUI? . msg='" + ioe.Message + "'" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "source=" + ioe.Source + Environment.NewLine + "target=" + ioe.TargetSite + Environment.NewLine + ioe.GetType().Name;
                    Window().logMessageWithFlush(message);
                    MessageBox.Show(message);
                }
                catch (Exception e)
                {
                    string message = "Unexpected exception while performing test. msg='" + e.Message + "'" + Environment.NewLine + e.StackTrace + Environment.NewLine + "source=" + e.Source + Environment.NewLine + "target=" + e.TargetSite + Environment.NewLine + e.GetType().Name;
                    Window().logMessageWithFlush(message);
                    MessageBox.Show(message);
                }
//#endif
            } // end while
            Window().logMessage("thread for " + Name + " ending");
        }

        public void TestExecutionTriggered()
        {
            mTestExecutionTimeoutTimer.Enabled = true;
        }

        /// <summary>
        /// This may be called by tools that throw up a dialog to query the operator...the operator may take a few seconds to read the query message and make a decision and we don't want to timeout in that period
        /// </summary>
        public void StopExecutionTimeoutTimer()
        {
            mTestExecutionTimeoutTimer.Enabled = false;
        }

        public void StartExecutionTimeoutTimer()
        {
            mTestExecutionTimeoutTimer.Enabled = true;
        }

        public void CleanupActiveTestExecution()
        {
            mTestExecutionTimeoutTimer.Enabled = false;
            if (mActiveExecution != null)
            {
                mActiveExecution.PerformPostExecutionCleanup();
                mActiveExecution = null;
            }
        }

		public void CreateExecutor()
		{
            IObjectDefinition objectDefForDebug = null;
            try
            {
                ObjectRegistry.Sort(); // HACK to make sure mObjectSpace is sorted properly. sometimes when I would  change properties, new constant number objects weren't created in time.  will this fix it?

                // new SequenceExecution
                TestExecution newExecution = new TestExecution(this);

                foreach (IObjectDefinition objectDef in ObjectRegistry.ObjectList())
                {
                    objectDefForDebug = objectDef;
                    if (objectDef.GetOwnerLink() == null)
                    {
                        objectDef.CreateInstance(newExecution);
                    }
                }
                if (mActiveExecution != null)
                {
                    // we should never get here, but just in case...make sure we cleanup properly
                    mActiveExecution.PerformPostExecutionCleanup();
                }
                mActiveExecution = newExecution;
                mHasUnusedChanges = false;
                RecreateAndTriggerExecution = false;
            }
            catch (ArgumentException e)
            {
                MessageBox.Show("Unable to setup for the test. Creating '" + objectDefForDebug.Name + "'  Problem:" + e.Message + Environment.NewLine + e.StackTrace);
                Window().logMessageWithFlush("ERROR: Unable to setup for the test. Creating '" + objectDefForDebug.Name + "'  Problem:" + e.Message);//+ Environment.NewLine + e.StackTrace);
                Enabled = false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to setup for the test. Creating '" + objectDefForDebug.Name + "'");
                Window().logMessageWithFlush("ERROR: Unable to setup for the test. Creating '" + objectDefForDebug.Name + "'  Exception:" + e.Message);// + Environment.NewLine + e.StackTrace);
                Enabled = false;
            }
        }

		private static bool rebuildingGrid = false;
		public void RebuildToolGrid()
		{
			SourceGrid3.Grid toolGrid = project().Window().ToolGrid();

			// recursive protection
			if( rebuildingGrid ) return;
			rebuildingGrid = true;

			for( int row = 0; row < toolGrid.RowsCount; row++ )
				for( int col = 0; col < toolGrid.ColumnsCount; col++)
				{
					toolGrid[row,col] = null;
				}

            ObjectRegistry.Sort();
			int currentTestSequenceStartingColumn = 0;
			int nextTestSequenceStartingColumn = 0;
			TestSequence lastTestSequence = null;
            foreach (IObjectDefinition ob in ObjectRegistry.ObjectList())
			{
                if (ob.ToolMapRow >= 0 && ob.IncludeObjectInProgrammingTable()) // ignore objects with row < 0 (e.g. Parameter objects which are just connectors)
                {
                    /* removed 7/3/08 since fixed ConstantValueDefinition support for IncludeObjectInProgrammingTable()
                    if (ob is ConstantValueDefinition)
                    {
                        if (ob.Name == "" + ((ConstantValueDefinition)ob).Value)
                        {
                            continue; // don't bother including unnamed constant values
                        }
                    }
                    */
                    if (ob.TestSequence() != lastTestSequence) // NOTE: as currently implemented there is only 1 test sequence in each objectSpace...I think maybe I was originally thinking of having the object space at the project level...for object sharing??
                    {
                        currentTestSequenceStartingColumn = nextTestSequenceStartingColumn;
                        lastTestSequence = ob.TestSequence();
                    }
                    ob.ToolMapColumn = currentTestSequenceStartingColumn; // note: you won't get what you ask for if it is already taken
                    toolGrid[ob.ToolMapRow, ob.ToolMapColumn] = new SourceGrid3.Cells.Real.Cell(ob);
                    toolGrid[ob.ToolMapRow, ob.ToolMapColumn].AddController(project().Window().clickController);
                    if (ob == Window().SelectedObject()) toolGrid[ob.ToolMapRow, ob.ToolMapColumn].Select = true;
                    nextTestSequenceStartingColumn = Math.Max(nextTestSequenceStartingColumn, ob.ToolMapColumn + 1);
                }
			}
			toolGrid.AutoSize();
			rebuildingGrid = false;
		}

        public override string ToString()
        {
            return Name;
        }

	}
    /*
    public class TestSequenceTreeNode : TreeNode
    {
        public TestSequenceTreeNode(TestSequence seq)
        {
            Tag = seq;
            Text = seq.Name;
        }
        public TestSequenceTreeNode()
        {
        }

//        private TestSequence obj;
        public override string ToString()
        {
            return Tag.ToString();
        }

        public TestSequence TestSequence { get { return (TestSequence)Tag; } set { Tag = value; } }        
    }
     */
    public class TestSequenceConverter : StringConverter
    {
        /*
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(new List<TestSequence>());
            }
            else
            {
                throw new ArgumentException("why are we here? 932083");
            }
        }
         */
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(TestSequence))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is TestSequence)
            {

                TestSequence idd = (TestSequence)value;

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
                Project project;
                TestSequence namedSequence;

                try
                {
                    string sequenceName = (string)value;

                    if (context.Instance is ProjectComponent)
                    {
                        project = ((ProjectComponent)context.Instance).project();
                    }
                    else
                    {
                        project = (Project)context.Instance;
                    }

                    namedSequence = project.FindSequence(sequenceName);
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to a test sequence");
                }
                return namedSequence;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    //public class MasterObjectRegistry<T> : DefObjectRegistry<T> where T : IObjectDefinition
    public class MasterObjectRegistry : DefinitionObjectRegistry<IObjectDefinition>
    {
        public MasterObjectRegistry(string theObjectType)
            : base(theObjectType)
        {}

        public void Sort()
        {
            mDefObjects.Sort(new ToolGridPositionComparer());
        }

        public void Purge()
        {
            while (mDefObjects.Count > 0)
            {
                mDefObjects[0].Dispose_UVision();
                mDefObjects.RemoveAt(0);
            }
            // TODO: verify that all other registrys are empty (when an object is disposed it is unregistered)
        }

        public string EnsureNameIsUnique(IObjectDefinition theObjectWeAreNaming, string theDesiredName)
        {
            theDesiredName = theDesiredName.Trim();
            string uniqueName = theDesiredName;
            int unqiuenessID = 1;
            while (!NameIsUnique(theObjectWeAreNaming, uniqueName))
            {
                uniqueName = theDesiredName + " " + unqiuenessID++;
            }
            return uniqueName;
        }
        public bool NameIsUnique(IObjectDefinition theObjectWeAreNaming, string theName)
        {
            theName = theName.Trim();
            foreach (IObjectDefinition defObject in mDefObjects)
            {
                if (defObject.Name.Equals(theName) && defObject != theObjectWeAreNaming)
                {
                    return false;
                }
            }
            return true;
        }
    }


}

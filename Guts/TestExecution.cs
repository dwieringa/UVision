// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;


namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class TestExecution
	{
        public TreeNode treeNode = new TreeNode();
        private TestSequence mTestSequence;
        private List<ITriggerInstance> mTriggers = new List<ITriggerInstance>();
        private List<IToolInstance> mExecutionArray = new List<IToolInstance>();
        private DateTime mCreatedTime;
        private DateTime mTriggerTime;
        private Stopwatch mWatch = new Stopwatch();
        private string mLogText;
        
        public TestExecution(TestSequence testSequence)
		{
			mTestSequence = testSequence;
            Name = "Active Test";
            mCreatedTime = DateTime.Now;

            LogMessage("Created test execution at " + mCreatedTime.ToShortDateString() + " " + mCreatedTime.ToString("h:mm:ss.fff tt"));
        }

        public delegate void TestExecutionDelegate(TestExecution testExecution);

        public delegate void ExecutionCompletedDelegate();
        public event ExecutionCompletedDelegate ExecutionCompleted;

        public TestSequence Sequence() { return mTestSequence; }
        public ProgrammingForm Window() { return mTestSequence.Window(); }

        public void LogSummaryMessage(string msg)
        {
            mLogText = msg + Environment.NewLine + mLogText;
        }
        public void LogMessage(string msg)
        {
            mLogText += msg + Environment.NewLine;
        }
        public void LogErrorWithTimeFromTrigger(string msg)
        {
            mLogText += mWatch.ElapsedMilliseconds + "ms : ERROR: " + msg + Environment.NewLine;
        }
        public void LogMessageWithTimeFromTrigger(string msg)
        {
            mLogText += mWatch.ElapsedMilliseconds + "ms : " + msg + Environment.NewLine;
        }
        public void LogMessageWithTimeFromCreated(string msg)
        {
            mTempTimeSpanForLogging = DateTime.Now - mCreatedTime;
            mLogText += mTempTimeSpanForLogging.TotalMilliseconds + "ms : " + msg + Environment.NewLine;
        }
        private TimeSpan mTempTimeSpanForLogging = new TimeSpan();

        public string GetLogText()
        {
            return mLogText;
        }

        public String Name
        {
            get { return treeNode.Text; }
            set { Window().SetTreeNodeText(Window(), treeNode, value); }
        }

        public DateTime TriggerTime
        {
            get { return mTriggerTime; }
        }

        public DateTime CreatedTime
        {
            get { return mCreatedTime; }
        }

        public void RegisterTrigger(ITriggerInstance theTrigger)
		{
			mTriggers.Add(theTrigger);
		}
		public void RegisterWorker(ToolInstance theWorker)
		{
			mExecutionArray.Add(theWorker);
			mExecutionArray.Sort(new ToolInstanceDependencyComparer()); // TODO: only sort when needed or after all objects registered?
		}

        public MathOperationInstance GetMathOperation(string theName)
        {
            ObjectInstance theObject = ObjectRegistry.GetObject(theName);
            if (!(theObject is MathOperationInstance))
            {
                throw new ArgumentException("'" + theName + "' isn't a math operation");
            }
            return (MathOperationInstance)theObject;
        }
        public ColorMatchInstance GetColorMatcher(string theName)
        {
            ObjectInstance theObject = ObjectRegistry.GetObject(theName);
            if (!(theObject is ColorMatchInstance))
            {
                throw new Exception("'" + theName + "' isn't a ColorMatcher");
            }
            return (ColorMatchInstance)theObject;
        }

        public InstanceObjectRegistry<DataValueInstance> DataValueRegistry = new InstanceObjectRegistry<DataValueInstance>("DataValue");
        public InstanceObjectRegistry<ObjectInstance> ObjectRegistry = new InstanceObjectRegistry<ObjectInstance>("Object");
        public InstanceObjectRegistry<ImageInstance> ImageRegistry = new InstanceObjectRegistry<ImageInstance>("Image");
        public InstanceObjectRegistry<VideoInstance> VideoRegistry = new InstanceObjectRegistry<VideoInstance>("Video");
        public InstanceObjectRegistry<ROIInstance> ROIRegistry = new InstanceObjectRegistry<ROIInstance>("ROI");
        public InstanceObjectRegistry<IReferencePointInstance> ReferencePointRegistry = new InstanceObjectRegistry<IReferencePointInstance>("ReferencePoint");
        public InstanceObjectRegistry<IDecorationInstance> DecorationRegistry = new InstanceObjectRegistry<IDecorationInstance>("Decoration");
        public InstanceObjectRegistry<LineDecorationInstance> LineDecorationRegistry = new InstanceObjectRegistry<LineDecorationInstance>("LineDecoration");
        public InstanceObjectRegistry<ObjectBasedLineDecorationInstance> ObjectBasedLineDecorationRegistry = new InstanceObjectRegistry<ObjectBasedLineDecorationInstance>("ObjectBasedLineDecoration");
        public InstanceObjectRegistry<CircleDecorationInstance> CircleDecorationRegistry = new InstanceObjectRegistry<CircleDecorationInstance>("CircleDecoration");
        public InstanceObjectRegistry<ScoreFilterInstance> ScoreFilterRegistry = new InstanceObjectRegistry<ScoreFilterInstance>("ScoreFilter");
        public InstanceObjectRegistry<ImageScorerInstance> ImageScorerRegistry = new InstanceObjectRegistry<ImageScorerInstance>("ImageScorer");

        private bool mTriggerFired = false;
        public bool TriggerFired
        {
            get { return mTriggerFired; }
            set
            {
                if (mTriggerFired == false && value == true)
                {
                    mTriggerTime = DateTime.Now;
                    mWatch.Start();
                    mTriggerFired = true;
                    LogMessage("Trigger fired at " + mTriggerTime.ToShortDateString() + " " + mTriggerTime.ToString("h:mm:ss.fff tt"));
                    mTestSequence.TestExecutionTriggered();
                }
            } // TODO: only want to allow them to set to true. use a method?  what about from property editor?...not needed?
        }

		public void Go()
		{
			// if not trigger fired, check triggers and if still not fired then exit
            if (!mTriggerFired)
            {
                foreach (ITriggerInstance trigger in mTriggers)
                {
                    if (trigger.Definition_Trigger().TriggerEnabled && trigger.CheckTrigger())
                    {
                        TriggerFired = true;
                        Name = "Test at " + DateTime.Now;
                        break;
                    }
                }
                if (!TriggerFired)
                {
                    Thread.Sleep(5);
                    return;
                }
                if (Sequence().HasUnusedChanges())
                {
                    // WARNING: THIS MAY BE CALLED BY DIFFERENT THREADS (e.g. Test Sequence thread vs GUI thread)
                    LogMessageWithTimeFromTrigger("Terminating test execution due to new property changes");
                    Sequence().NameOfExecutionAfterRecreating = Name;
                    Sequence().RecreateAndTriggerExecution = true;
                    Name += " (TERMINATED DUE TO PROPERTY CHANGES)";
                    return;
                }
            }

			// loop through execution array: run Go, if isDone() then remove
            int currentDependencyIndex = 0;
            if( mExecutionArray.Count > 0) currentDependencyIndex = mExecutionArray[0].DependencyIndex;
			foreach( ToolInstance tool in mExecutionArray )
			{
                if (tool.DependencyIndex > currentDependencyIndex)
                {
                    // if we get here, it means we've cycled through the list without completing any tools at the currentDependencyIndex level (if we completed one, the loop would have been aborted (via break));

                    // sleep for the minimum amount of time to free up this thread/cpu; we are most likely waiting on something external (e.g. image download which happens in a background thread) so there is no use is wasting CPU in this loop.
                    Thread.Sleep(1); // this is an experiment added 10/23/07

                    break; // only process 1 dependency index level at a time. This way we don't have to worry about checking if dependent objects are complete in every DoWork() implementation
                }

				tool.DoWork();

                if (tool.IsComplete())
                {
                    mExecutionArray.Remove(tool);
                    //MessageBox.Show("tool " + tool.Name + " just completed; " + mExecutionArray.Count + " tools left running");
                    break;
                }
			}

			// if done, record stats
			if( mExecutionArray.Count == 0 )
			{
                // notify listeners that all tools have been executed and this execution is complete
                if (ExecutionCompleted != null)
                {
                    ExecutionCompleted();
                }

            }
	    }

        public void PerformPostExecutionCleanup()
        {
            foreach (ObjectInstance objectInstance in ObjectRegistry.ObjectList())
            {
                objectInstance.PostExecutionCleanup();
            }
        }

		public bool IsComplete() { return mExecutionArray.Count == 0; }

    }

    public class InstanceObjectRegistry<T> where T : IObjectInstance
    {
        public InstanceObjectRegistry(string theObjectType)
        {
            mObjectType = theObjectType;
        }

        private string mObjectType;
        private Dictionary<string, T> mObjects = new Dictionary<string, T>();
        public Dictionary<string, T>.ValueCollection ObjectList() { return mObjects.Values; }
        public void RegisterObject(T theInstanceObject)
        {
            mObjects.Add(theInstanceObject.Name, theInstanceObject);
        }
        public void UnRegisterObject(T theInstanceObject)
        {
            mObjects.Remove(theInstanceObject.Name);
        }
        public T GetObjectIfExists(string theName)
        {
            if (!mObjects.ContainsKey(theName)) return default(T);
            return mObjects[theName];
        }
        public T GetObject(string theName)
        {
            if (!mObjects.ContainsKey(theName)) throw new ArgumentException(mObjectType + " '" + theName + "' does not exist.");
            return mObjects[theName];
        }
        public string[] Options()
        {
            string[] options = new string[mObjects.Count];
            int i = 0;
            foreach (T defObject in mObjects.Values)
            {
                options[i++] = defObject.ToString();
            }
            return options;
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ToolInstance : NetCams.ObjectInstance, IToolInstance
	{
		public ToolInstance(ToolDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
		{
            if (theDefinition.Prerequisite != null)
            {
                mPrerequisite = theExecution.DataValueRegistry.GetObject(theDefinition.Prerequisite.Name);
            }

			theExecution.RegisterWorker(this);
		}

        protected DataValueInstance mPrerequisite = null;
        public DataValueInstance Prerequisite
        {
            get { return mPrerequisite; }
        }

        public abstract void DoWork(); // TODO: make another method that we invoke that invokes this one?  That one tracks metrics?
        // TODO: added CompletedTime()  milliseconds since midnight?  StartedTime()? (when all prerequisites met)  ElapsedTime()?

	}

	public class ToolInstanceDependencyComparer : System.Collections.Generic.IComparer<IToolInstance>
	{
		public ToolInstanceDependencyComparer()
		{
		}

        public int Compare(IToolInstance a, IToolInstance b)
        {
            return a.DependencyIndex - b.DependencyIndex;
        }
    }
}

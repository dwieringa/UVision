// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class CommonValueDefinition : DataValueDefinition
	{
        public CommonValueDefinition(TestSequence testSequence)
            : base(testSequence)
		{
		}

		public override void CreateInstance(TestExecution theExecution)
		{
            new CommonValueInstance(this, theExecution);
		}

        private GlobalValue mGlobalValue;
        public GlobalValue GlobalValue
        {
            get { return mGlobalValue; }
            set 
            {
                HandlePropertyChange(this, "GlobalValue", mGlobalValue, value);
                mGlobalValue = value;
            }
        }

        public enum DataReadStyle
        {
            NotDefined = 0,
            AtTrigger,
            Live
        }
        private DataReadStyle mReadStyle = DataReadStyle.NotDefined;
        public DataReadStyle ReadStyle
        {
            get { return mReadStyle; }
            set
            {
                if (value != mReadStyle)
                {
                    if (mReadStyle == DataReadStyle.AtTrigger)
                    {
                        // we're changing AWAY FROM AtTrigger reading, so kill the helper Tool

                        // remove dependency on helper tool
                        this.RemoveDependency(mHelperTool);

                        // TODO:? delete mHelperTool?  currently we just don't bother to create the instance of the tool
                    }
                    else if (value == DataReadStyle.AtTrigger)
                    {
                        // we're changing TO AtTrigger reading, so create a helper Tool
                        if (mHelperTool == null)
                        {
                            mHelperTool = new CommonValueInstance.AtTriggerValueGetterToolDefinition(TestSequence(), this);
                        }

                        // create dependency on helper tool
                        this.AddDependency(mHelperTool);
                    }
                    HandlePropertyChange(this, "ReadStyle", mReadStyle, value);
                    mReadStyle = value;
                }
            }
        }

        public CommonValueInstance.AtTriggerValueGetterToolDefinition mHelperTool;

	}
}

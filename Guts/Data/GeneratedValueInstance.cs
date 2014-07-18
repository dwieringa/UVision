// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class GeneratedValueInstance : DataValueWithStorageInstance
	{
		public GeneratedValueInstance(GeneratedValueDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
		}

        private bool mIsComplete = false;
        public void SetIsComplete()
        {
            SetCompletedTime();
            mIsComplete = true;
        }
        public override bool IsComplete() { return mIsComplete; }
	}
}

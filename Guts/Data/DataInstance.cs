// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class DataInstance : ObjectInstance
	{
		public DataInstance(DataDefinition theDefinition, TestExecution testExecution) : base(theDefinition, testExecution)
		{
		}

	}
}

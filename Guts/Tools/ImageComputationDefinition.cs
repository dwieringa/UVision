// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ImageComputationDefinition : NetCams.ToolDefinition
	{
		public ImageComputationDefinition(TestSequence testSequence) : base(testSequence)
		{
			// 
			// TODO: Add constructor logic here
			//
		}

		public abstract int ResultantValue
		{
			get;
		}

	}
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Drawing;


namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class VideoGeneratorDefinition : NetCams.ToolDefinition
	{
		public VideoGeneratorDefinition(TestSequence testSequence) : base(testSequence)
		{
		}

		public abstract GeneratedVideoDefinition ResultantVideo
		{
			get;
		}
	}
}

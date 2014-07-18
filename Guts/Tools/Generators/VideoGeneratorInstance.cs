// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public abstract class VideoGeneratorInstance : NetCams.ToolInstance
    {
        public VideoGeneratorInstance(VideoGeneratorDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
		}

		public abstract VideoInstance ResultantVideo
		{
			get;
		}
    }
}

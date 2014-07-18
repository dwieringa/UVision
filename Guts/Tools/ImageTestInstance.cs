// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;


namespace NetCams
{
    public abstract class ImageTestInstance : NetCams.ToolInstance
	{
        public ImageTestInstance(ImageTestDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
			// 
			// TODO: Add constructor logic here
			//
		}

        public abstract GeneratedValueInstance Result
		{
			get;
		}
	}
}

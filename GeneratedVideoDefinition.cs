// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class GeneratedVideoDefinition : VideoDefinition
    {
        public GeneratedVideoDefinition(TestSequence testSequence, OwnerLink ownerLink)
            : base(testSequence)
		{
            SetOwnerLink(ownerLink);
		}
        
        public override void CreateInstance(TestExecution theExecution)
		{
			new GeneratedVideoInstance(this, theExecution);
		}
    }
}

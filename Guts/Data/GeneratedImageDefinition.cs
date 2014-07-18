// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class GeneratedImageDefinition : ImageDefinition
	{
        public GeneratedImageDefinition(TestSequence testSequence, OwnerLink ownerLink)
            : base(testSequence)
		{
            SetOwnerLink(ownerLink);
		}
        
        public override void CreateInstance(TestExecution theExecution)
		{
			new GeneratedImageInstance(this, theExecution);
		}
	}
}

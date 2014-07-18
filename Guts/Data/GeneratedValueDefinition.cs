// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class GeneratedValueDefinition : DataValueWithStorageDefinition
	{
        public GeneratedValueDefinition(TestSequence testSequence, OwnerLink ownerLink)
            : base(testSequence)
		{
            SetOwnerLink(ownerLink);
            SetDataCategory(DataCategory.NamedValue);
		}

		public override void CreateInstance(TestExecution theExecution)
		{
			new GeneratedValueInstance(this, theExecution);
		}

	}
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class DataDefinition : ObjectDefinition
	{
		public DataDefinition(TestSequence testSequence) : base(testSequence)
		{
		}

        /*  This dependency code moved 9/28/07 up to ObjectDefinition since I want to be able to create explicit dependencies between any two objects
		private List<ObjectDefinition> mDependencies = new List<IObjectDefinition>();
		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            foreach (IObjectDefinition objectDependedOn in mDependencies)
            {
			    if( objectDependedOn.IsDependentOn(theOtherObject) )
			    {
				    return true;
			    }
            }
			return false;
		}
		public void AddDependency(IObjectDefinition theUsedObject)
		{
			/* TODO: how to handle repeat dependencies???  it is only a problem if we later remove some. 
			 * run this if we don't keep track of repeats..but it is probably safer to keep track of all
			foreach (ImageObjectDefinition dependency in mDependencies)
			{
				if( dependency == theUsedObject )
				{
					return
				}
			}
			* /
			mDependencies.Add(theUsedObject);
		}
		public void RemoveDependency(IObjectDefinition theNoLongerUsedObject)
		{
			mDependencies.Remove(theNoLongerUsedObject);
		}

		public override int ToolMapRow
		{
			get
			{
				int result = 0;
				foreach( IObjectDefinition dependency in mDependencies )
				{
					if( dependency.ToolMapRow >= result ) result = dependency.ToolMapRow+1;
				}
				return result;
			}
		}
        */
	}
}

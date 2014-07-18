// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ObjectDefinition : IObjectDefinition
	{
		public ObjectDefinition(TestSequence testSequence)
		{
			mTestSequence = testSequence;
            testSequence.ObjectRegistry.RegisterObject(this);
            ToolMapColumn = 0; // init it to 0 to force the column to be computed in a defined order (ie in the order objects are created)
        }

        public virtual void Dispose_UVision()
        {
            TestSequence().ObjectRegistry.UnRegisterObject(this);
        }

        public virtual bool IncludeObjectInConfigFile() { return true; }
        public virtual bool IncludeObjectInProgrammingTable() { return true; }

        private OwnerLink mOwnerLink = null;
        public void SetOwnerLink(OwnerLink ownerLink)
        {
            // NOTE: this is not a property since we don't want to expose it to the PropertyGrid or config file
            if (mOwnerLink != null && mOwnerLink != ownerLink) throw new Exception("should only call this once asdlkjalksjd");
            mOwnerLink = ownerLink;
        }
        public OwnerLink GetOwnerLink() { return mOwnerLink; }

		private TestSequence mTestSequence;
		public TestSequence TestSequence() { return mTestSequence; }
		public Project Project() { return mTestSequence.project(); }
		public ProgrammingForm Window() { return mTestSequence.project().Window(); }


		public abstract void CreateInstance(TestExecution theExecution);

		protected string name = "";
		/// <summary>
		/// Uniquely identifies all objects used within the software
		/// </summary>
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
                HandlePropertyChange(this, "Name", name, value);
                name = TestSequence().ObjectRegistry.EnsureNameIsUnique(this, value);
			}
		}

        /// <summary>
        /// 9/28/07 this moved from DataDefintion up to ObjectDefinition since I want to be able to create explicity dependencies on any object (e.g. a SnapshotDef on a PauseDefinition)
        /// </summary>
        private ObjectDefinitionList mDependencies = new ObjectDefinitionList();
        private ObjectDefinitionList mExplicitDependencies = new ObjectDefinitionList();
        public ObjectDefinitionList ExplicitDependencies
        {
            get { return mExplicitDependencies; }
            set
            {
                HandlePropertyChange(this, "ExplicitDependencies", mExplicitDependencies, value);
                mExplicitDependencies = value;
                for (int x = mExplicitDependencies.Count - 1; x >= 0; x--)
                {
                    if (mExplicitDependencies[x] == null)
                    {
                        mExplicitDependencies.RemoveAt(x);
                    }
                }
            }
        }

        /// <summary>
        /// 9/28/07 this moved from DataDefintion up to ObjectDefinition since I want to be able to create explicity dependencies on any object (e.g. a SnapshotDef on a PauseDefinition)
        /// </summary>
        // return true if the object is dependent on theOtherObject.  This involves calling IsDependentOn(theOtherObject) for all objects that this object references.
        // Beyond calling IsDependentOn for each referenced object, we should also compare theOtherObject reference to each referenced object.  As a shortcut, currently I am instead comparing theOtherObject to 'this'.  This achieves all of the same tests (at one level down in the call stack).  In addition it also tests theOtherObject==this to the root object...this check isn't really ideal, but testing this way originally seemed safest (least likely to forget one) and quickest (less code).  This translates to all objects being "dependent" upon themselves which doesn't truely match reality. TODO: consider switching to a more accurate approach
        public virtual bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            foreach (IObjectDefinition objectDependedOn in mDependencies)
            {
                if (objectDependedOn.IsDependentOn(theOtherObject))
                {
                    return true;
                }
            }
            foreach (IObjectDefinition objectDependedOn in mExplicitDependencies)
            {
                if (objectDependedOn.IsDependentOn(theOtherObject))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 9/28/07 this moved from DataDefintion up to ObjectDefinition since I want to be able to create explicity dependencies on any object (e.g. a SnapshotDef on a PauseDefinition)
        /// </summary>
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
            */
            if (theUsedObject == null)
            {
                throw new ArgumentException("Can't assign 'null' as a dependency");
            }
            mDependencies.Add(theUsedObject);
        }
        /// <summary>
        /// 9/28/07 this moved from DataDefintion up to ObjectDefinition since I want to be able to create explicity dependencies on any object (e.g. a SnapshotDef on a PauseDefinition)
        /// </summary>
        public void RemoveDependency(IObjectDefinition theNoLongerUsedObject)
        {
            mDependencies.Remove(theNoLongerUsedObject);
        }

        /// <summary>
        /// 9/28/07 this moved from DataDefintion up to ObjectDefinition since I want to be able to create explicity dependencies on any object (e.g. a SnapshotDef on a PauseDefinition)
        /// </summary>
        public virtual int ToolMapRow
        {
            get
            {
                int result = 0;
                foreach (IObjectDefinition dependency in mDependencies)
                {
                    result = Math.Max(result, dependency.ToolMapRow);
                }
                foreach (IObjectDefinition dependency in mExplicitDependencies)
                {
                    result = Math.Max(result, dependency.ToolMapRow);
                }
                return result + 1;
            }
        }

		private int toolMapColumn;
		public int ToolMapColumn
		{
			get
			{
				return toolMapColumn;
			}
			set
			{
				int originalToolMapRow = ToolMapRow;
				int originalToolMapColumn = toolMapColumn;

                if (originalToolMapRow < 0) // we don't insert objects with row < 0
                {
                    toolMapColumn = value;
                    return; 
                }

                if( value >= Window().ToolGrid().ColumnsCount ) Window().ToolGrid().ColumnsCount = value+1;
				if( originalToolMapRow >= Window().ToolGrid().RowsCount ) Window().ToolGrid().RowsCount = originalToolMapRow+1;

				// if the new location isn't empty and it's not this same object, then shift us over one column (recursively) to find an empty slot
				if( Window().ToolGrid()[ToolMapRow,value] != null && Window().ToolGrid()[ToolMapRow,value].Value != null && Window().ToolGrid()[ToolMapRow,value].Value != this )
				{
					ToolMapColumn = value+1;
				}
				else // once we find an empty slot (or our own) then insert this object
				{
					toolMapColumn = value;

                    // TODO: WARNING: the code below that inserts the cells into the table was my original grid building code...then I moved it to RebuildToolGrid and commented it out here.  As of 5/30/07 I am uncommenting it here because I rely on objects being in the table when assigning the initial column (e.g. object creation/registration...if I don't do it here, they don't get assigned properly
					Window().ToolGrid()[ToolMapRow, value] = new SourceGrid3.Cells.Real.Cell(this);
					Window().ToolGrid()[ToolMapRow, value].AddController(Window().clickController);
					//Window().logMessage("inserted " + Name + " at " + toolMapColumn + " " + ToolMapRow);

					Window().ToolGrid().AutoSize();
				}
				// if this object is already in the grid at a different location, then remove it from the old location
				if( (toolMapColumn != originalToolMapColumn || ToolMapRow != originalToolMapRow) && Window().ToolGrid()[originalToolMapRow,originalToolMapColumn] != null && Window().ToolGrid()[ToolMapRow,originalToolMapColumn].Value != null && Window().ToolGrid()[ToolMapRow,originalToolMapColumn].Value == this )
				{
					Window().ToolGrid()[originalToolMapRow,originalToolMapColumn] = null;
					//Window().logMessage("removed " + Name + " from " + originalToolMapColumn + " " + originalToolMapRow);
				}
			}
		}


		public override string ToString()
		{
			return Name;
		}

        /// <summary>
        /// TODO: we may want a static implementation of this in case there are IObjectDefinition classes that don't inherit from ObjectDefinition. This may apply to all methods here.
        /// TODO: what about properties that aren't of type IObjectDefinition? (e.g. Color, Direction, int, etc)
        /// </summary>
        /// <param name="propertyOwner"></param>
        /// <param name="newPropertyValue"></param>
        public void HandlePropertyChange(IObjectDefinition propertyOwner, string propertyName, IObjectDefinition originalValue, IObjectDefinition newPropertyValue)
        {
            if (originalValue != newPropertyValue)
            {
                if (newPropertyValue != null && newPropertyValue.IsDependentOn(propertyOwner))
                {
                    throw new ArgumentException("That would create a circular dependency.");
                }
                if (TestSequence().FullyInitialized)
                {
                    NotifyPropertyChanged(propertyName);
                    /*
                    bool changeIsSignificantToSequenceDefinition = true;
                    if (this is DataValueDefinition)
                    {
                        DataValueDefinition valueDef = (DataValueDefinition)this;
                        if (valueDef.Category == DataCategory.UnnamedConstant || valueDef.Category == DataCategory.CalculatedValue)
                        {
                            changeIsSignificantToSequenceDefinition = false;
                        }
                    }*/
                    if (IncludeObjectInConfigFile())
                    {
                        TestSequence().Log("Changed " + propertyOwner.Name + "." + propertyName + " from '" + (originalValue == null ? "" : originalValue.Name) + "' to '" + (newPropertyValue == null ? "" : newPropertyValue.Name) + "'");
                        TestSequence().SetUnsavedChanges();
                    }
                    TestSequence().SetUnusedChanges();
                }
            }
        }

        public void HandlePropertyChange(IObjectDefinition propertyOwner, string propertyName, object originalValue, object newPropertyValue)
        {
            //            if (originalValue != newPropertyValue)
            if (newPropertyValue == null && originalValue == null)
            {
                // do nothing
            }
            else if (originalValue == null || !originalValue.Equals(newPropertyValue))
            {
                if (TestSequence().FullyInitialized)
                {
                    NotifyPropertyChanged(propertyName);
                    if (IncludeObjectInConfigFile())
                    {
                        TestSequence().Log("Changed " + propertyOwner.Name + "." + propertyName + " from '" + (originalValue == null ? "" : originalValue) + "' to '" + (newPropertyValue == null ? "" : newPropertyValue) + "'");
                        TestSequence().SetUnsavedChanges();
                    }
                    TestSequence().SetUnusedChanges();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public virtual bool SupportsDragAndDrop() { return false; }

        public virtual void VerifyValidItemsForDrop(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        public virtual void HandleDrop(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            return;
        }
	}

    public class OwnerLink
    {
        public OwnerLink(IObjectDefinition theOwner, string theOwnersPropertyName)
        {
            mOwner = theOwner;
            mOwnersProperty = mOwner.GetType().GetProperty(theOwnersPropertyName);
        }

        // does this have value?  seemed like a cleaner way to do it...but...
        public static OwnerLink newLink(IObjectDefinition theOwner, string theOwnersPropertyName)
        {
            return new OwnerLink(theOwner, theOwnersPropertyName);
        }

        private IObjectDefinition mOwner;
        public IObjectDefinition Owner
        {
            get { return mOwner; }
        }

        private PropertyInfo mOwnersProperty;
        public PropertyInfo Property
        {
            get { return mOwnersProperty; }
        }
    }


	public class ToolGridPositionComparer : System.Collections.Generic.IComparer<IObjectDefinition>
	{
		public ToolGridPositionComparer()
		{
		}

        public int Compare(IObjectDefinition left, IObjectDefinition right)
        {
            /*
			int testSequenceDifference = left.Sequence().ToolGridOrder - right.Sequence().ToolGridOrder;
			if( testSequenceDifference != 0 )
                return testSequenceDifference;
            */
            int rowDiff = left.ToolMapRow - right.ToolMapRow;
            if (rowDiff != 0) return rowDiff;

            // commented out this check against columns 7/3/08 so that things get sorted alphabetically...trying this
            //int colDiff = left.ToolMapColumn - right.ToolMapColumn;
            //if (colDiff != 0) return colDiff;
            return left.Name.CompareTo(right.Name);
        }
    }

    [TypeConverter(typeof(ObjectDefCollectionConverter))]
    public class ObjectDefinitionList : List<IObjectDefinition>
    {
    }

    public class ObjectDefCollectionConverter : StringConverter
    {
        /*
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(((IObjectDefinition)context.Instance).Project().CameraOptions());
            }
            else
            {
                throw new ArgumentException("why are we here? 932083");
            }
        }
         */
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(ObjectDefinitionList))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is ObjectDefinitionList)
            {

                ObjectDefinitionList idd = (ObjectDefinitionList)value;
                string result = "";
                foreach (IObjectDefinition def in idd)
                {
                    if (result.Length > 0) result += ", ";
                    result += def.Name;
                }
                return result;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                IObjectDefinition objectBeingEditted;
                ObjectDefinitionList collectionFromString = new ObjectDefinitionList();

                try
                {
                    string collectionAsString = (string)value;
                    string[] objectNames = collectionAsString.Split(new char[] { ',' });
                    string objectName;

                    objectBeingEditted = (IObjectDefinition)context.Instance;

                    for (int x = 0; x <= objectNames.GetUpperBound(0); x++)
                    {
                        objectName = objectNames[x].Trim();
                        if (objectName.Length > 0)
                        {
                            collectionFromString.Add(objectBeingEditted.TestSequence().ObjectRegistry.GetObject(objectName));
                        }
                    }
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to an object collection");
                }
                return collectionFromString;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

}

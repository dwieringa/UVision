// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ObjectInstance : IObjectInstance
	{
		public ObjectInstance(IObjectDefinition theDefinition, TestExecution theExecution)
		{
			name = theDefinition.Name;
            mToolMapRow = theDefinition.ToolMapRow;
            mToolMapColumn = theDefinition.ToolMapColumn;
            mObjectDefinition = theDefinition;
			mTestExecution = theExecution;

            /* commented out 3/6/09
            foreach( IObjectDefinition def in theDefinition.ExplicitDependencies)
            {
                mDependencies.Add(theExecution.ObjectRegistry.GetObject(def.Name));
            }
            */

			theExecution.ObjectRegistry.RegisterObject(this);
		}

		private IObjectDefinition mObjectDefinition;
        private TestExecution mTestExecution;
        public TestExecution TestExecution() { return mTestExecution; }
        public TestSequence TestSequence() { return mTestExecution.Sequence(); }
        public Project Project() { return mTestExecution.Sequence().project(); }
        public ProgrammingForm Window() { return mTestExecution.Sequence().project().Window(); }

		public IObjectDefinition Definition() { return mObjectDefinition; }
		public abstract bool IsComplete(); // NOTE: this doesn't make sense for Tools...only Tool's workers

        // commented out 3/6/09 private ObjectInstanceList mDependencies = new ObjectInstanceList();

        /* commented out 3/6/09 - I thought we only used dependencies in DefinitionObjects...and this isn't referenced
        public ObjectInstanceList ExplicitDependencies
        {
            get { return mDependencies; }
        }
        */

        /* commented out 3/6/09 along with references to it...all of these were at the start of DoWork() and should be unnecessary based on checks added before DoWork is called (long ago)
        public bool AreExplicitDependenciesComplete()
        {
            foreach (ObjectInstance dependency in mDependencies)
            {
                if (!dependency.IsComplete()) return false;
            }
            return true;
        }*/

        private DateTime mCompletedTime = new DateTime(1900);
        public void SetCompletedTime()
        {
            if (mCompletedTime.Year > 1900) throw new ArgumentException("Calling SetCompletedTime more than once!");
            mCompletedTime = DateTime.Now;
        }

        [CategoryAttribute("Metrics"),
        DescriptionAttribute("")]
        public DateTime CompletedTime
        {
            get { return mCompletedTime; }
        }

        [CategoryAttribute("Metrics"),
        DescriptionAttribute("Elapsed time from test trigger to Completed Time")]
        public TimeSpan ComputationTime
        {
            get { return mCompletedTime - mTestExecution.TriggerTime; }
        }

		private string name;
		/// <summary>
		/// Uniquely identifies all tools used within the software
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
		}

		public int DependencyIndex
		{
			get { return mToolMapRow; }
		}

		private int mToolMapRow;
		public int ToolMapRow
		{
			get { return mToolMapRow; }
		}

		private int mToolMapColumn;
		public int ToolMapColumn
		{
            get { return mToolMapColumn; }
        }

        // is this only needed for Tools?
        public virtual void PostExecutionCleanup()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [TypeConverter(typeof(ObjectInstanceCollectionConverter))]
    public class ObjectInstanceList : List<ObjectInstance>
    {
    }

    public class ObjectInstanceCollectionConverter : StringConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(ObjectInstanceList))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is ObjectInstanceList)
            {

                ObjectInstanceList idd = (ObjectInstanceList)value;
                string result = "";
                foreach (ObjectInstance def in idd)
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
                ObjectInstance objectBeingEditted;
                ObjectInstanceList collectionFromString = new ObjectInstanceList();

                try
                {
                    string collectionAsString = (string)value;
                    string[] objectNames = collectionAsString.Split(new char[] { ',' });
                    string instanceName;

                    objectBeingEditted = (ObjectInstance)context.Instance;

                    for (int x = 0; x <= objectNames.GetUpperBound(0); x++)
                    {
                        instanceName = objectNames[x].Trim();
                        if (instanceName.Length > 0)
                        {
                            collectionFromString.Add(objectBeingEditted.TestExecution().ObjectRegistry.GetObject(instanceName));
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

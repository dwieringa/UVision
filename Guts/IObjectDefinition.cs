// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace NetCams
{
    [TypeConverter(typeof(ObjectDefinitionConverter))]
    public interface IObjectDefinition : INotifyPropertyChanged
    {
        void Dispose_UVision();
        void CreateInstance(TestExecution theExecution);

        string Name { get; set; }
        ProgrammingForm Window();
        Project Project();
        TestSequence TestSequence();

        OwnerLink GetOwnerLink();
        void SetOwnerLink(OwnerLink ownerLink);

        bool IncludeObjectInConfigFile();
        bool IncludeObjectInProgrammingTable();

        bool SupportsDragAndDrop();
        void VerifyValidItemsForDrop(SourceGrid3.GridVirtual sender, System.Windows.Forms.DragEventArgs e);
        void HandleDrop(SourceGrid3.GridVirtual sender, System.Windows.Forms.DragEventArgs e);

        bool IsDependentOn(IObjectDefinition theOtherObject);
        ObjectDefinitionList ExplicitDependencies { get; set; }
        int ToolMapColumn { get; set; }
        int ToolMapRow { get; }

        string ToString();

        void HandlePropertyChange(IObjectDefinition propertyOwner, string propertyName, IObjectDefinition originalValue, IObjectDefinition newPropertyValue);
        void HandlePropertyChange(IObjectDefinition propertyOwner, string propertyName, object originalValue, object newPropertyValue);
    }

    public class ObjectDefinitionConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(Options(((IObjectDefinition)context.Instance).TestSequence()));
            }
            else if (context.Instance is FavoriteSettings)
            {
                return new StandardValuesCollection(Options(((FavoriteSettings)context.Instance).mTestSequence));
            }
            else
            {
                throw new ArgumentException("Unexpected context (3563)");
            }
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(IObjectDefinition))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is IObjectDefinition)
            {
                IObjectDefinition definitionObject = (IObjectDefinition)value;

                return definitionObject.Name;
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
                IObjectDefinition objectBeingSelected;

                try
                {
                    string selectedObjectsName = (string)value;

                    if (selectedObjectsName.Length == 0) return null;

                    if (context.Instance is IObjectDefinition)
                    {
                        IObjectDefinition objectBeingEditted = (IObjectDefinition)context.Instance;
                        objectBeingSelected = LookupObject(objectBeingEditted.TestSequence(), selectedObjectsName);
                    }
                    else if (context.Instance is FavoriteSettings)
                    {
                        objectBeingSelected = LookupObject(((FavoriteSettings)context.Instance).mTestSequence, selectedObjectsName);
                    }
                    else
                    {
                        throw new ArgumentException("alksdjalksdlkajlkaj");
                    }
                }
                catch (ArgumentException e)
                {
                    throw e;
                }
                catch(Exception e)
                {
                    throw new ArgumentException("Problem finding object '" + (string)value + "'");
                }
                return objectBeingSelected;
            }
            return base.ConvertFrom(context, culture, value);
        }

        protected virtual string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.ObjectRegistry.Options();
        }

        protected virtual IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.ObjectRegistry.GetObject(theObjectName);
        }
    }

    public class DefinitionObjectRegistry<T> where T : IObjectDefinition
    {
        public DefinitionObjectRegistry(string theObjectType)
        {
            mObjectType = theObjectType;
            mDefObjects = new List<T>();
        }

        public DefinitionObjectRegistry(string theObjectType, List<T> theObjectList)
        {
            mObjectType = theObjectType;
            mDefObjects = theObjectList;
        }

        protected string mObjectType;
        protected List<T> mDefObjects;
        public ReadOnlyCollection<T> ObjectList() { return mDefObjects.AsReadOnly(); }
        public int Count
        {
            get { return mDefObjects.Count; }
        }
        public virtual void RegisterObject(T theDefObject)
        {
            mDefObjects.Add(theDefObject);
        }
        public virtual void UnRegisterObject(T theDefObject)
        {
            mDefObjects.Remove(theDefObject);
        }
        public virtual T GetObjectIfExists(string theName)
        {
            foreach (T roi in mDefObjects)
            {
                if (roi.Name.Equals(theName))
                {
                    return roi;
                }
            }
            return default(T);
        }
        public virtual T GetObject(string theName)
        {
            T result = GetObjectIfExists(theName);
            if (result == null)
            {
                throw new ArgumentException(mObjectType + " '" + theName + "' does not exist.");
            }
            return result;
        }
        public virtual string[] Options()
        {
            string[] options = new string[mDefObjects.Count];
            int i = 0;
            SortOptions();
            foreach (T defObject in mDefObjects)
            {
                options[i++] = defObject.ToString();
            }
            return options;
        }
        public virtual void SortOptions()
        {
            mDefObjects.Sort(ObjectDefinitionComparer<T>.Singleton);
        }
    }

    public class ObjectDefinitionComparer<T> : System.Collections.Generic.IComparer<T> where T : IObjectDefinition
    {
        public ObjectDefinitionComparer()
        {
        }

        public static ObjectDefinitionComparer<T> Singleton = new ObjectDefinitionComparer<T>();

        public int Compare(T a, T b)
        {
            return a.Name.CompareTo(b.Name);
        }
    }
}

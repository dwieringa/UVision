// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace NetCams
{
    [TypeConverter(typeof(ObjectInstanceConverter))]
    public interface IObjectInstance
    {
        DateTime CompletedTime { get; }
        TimeSpan ComputationTime { get; }

        IObjectDefinition Definition();
        int DependencyIndex { get; }
        bool IsComplete();
        string Name { get; }
        void PostExecutionCleanup();
        Project Project();
        void SetCompletedTime();
        TestExecution TestExecution();
        TestSequence TestSequence();
        int ToolMapColumn { get; }
        int ToolMapRow { get; }
        string ToString();
        ProgrammingForm Window();
    }

    public class ObjectInstanceConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectInstance)
            {
                return new StandardValuesCollection(Options(((IObjectInstance)context.Instance).TestExecution()));
            }
            else
            {
                throw new ArgumentException("Unexpected context (3513)");
            }
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(IObjectInstance))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is IObjectInstance)
            {
                IObjectInstance instanceObject = (IObjectInstance)value;

                return instanceObject.Name;
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
                IObjectInstance objectBeingSelected;

                try
                {
                    string selectedObjectsName = (string)value;

                    if (selectedObjectsName.Length == 0) return null;

                    if (context.Instance is IObjectInstance)
                    {
                        IObjectInstance objectBeingEditted = (IObjectInstance)context.Instance;
                        objectBeingSelected = LookupObject(objectBeingEditted.TestExecution(), selectedObjectsName);
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
                catch (Exception e)
                {
                    throw new ArgumentException("Problem finding object '" + (string)value + "'");
                }
                return objectBeingSelected;
            }
            return base.ConvertFrom(context, culture, value);
        }

        protected virtual string[] Options(TestExecution theTestExecution)
        {
            return theTestExecution.ObjectRegistry.Options();
        }

        protected virtual IObjectInstance LookupObject(TestExecution theTestExecution, string theObjectName)
        {
            return theTestExecution.ObjectRegistry.GetObject(theObjectName);
        }
    }
}

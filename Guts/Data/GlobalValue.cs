// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NetCams
{
    [TypeConverter(typeof(GlobalValueConverter))]
    public class GlobalValue
    {
        public GlobalValue(Project theProject)
        {
            mProject = theProject;
        }
        private Project mProject;

        public delegate void GlobalValueDelegate(GlobalValue globalValue);
        public event GlobalValueDelegate GlobalValueChanged;

        public enum DataScope
        {
            NotDefined = 0,
            TestSequence,
            Project
        }

        protected DataType mDataType = DataType.NotDefined;
        public DataType Type
        {
            get { return mDataType; }
            set { mDataType = value; }
        }

        protected bool mIsRetentive = true; // TODO: allow programmer to determine if values are retentive...vs defined by config file
        public bool IsRetentive
        {
            get { return mIsRetentive; }
            set
            {
                mIsRetentive = value;
                HandleValueChange(); // HACK: we want to update the ret file to include/exclude value
            }
        }

        protected DataScope mDataScope = DataScope.NotDefined;
        public DataScope Scope
        {
            get { return mDataScope; }
            /*
             * allowing the user to change the scope gets REAL messy unless their is only one updater.  if there is multiple and you switch from Project scope to TestSequence, we would need to create multiple instances of GlobalValue and fill each's mUpdaters with one DataValue
             * For now, we'll just make changes in the config files
             */
            set
            {
                if (mDataScope != DataScope.NotDefined)
                {
                    throw new ArgumentException("Scope can't be changed once it is setup.  Make changes in the configuration files.");
                }
                mDataScope = value;
                if(mDataScope == DataScope.Project)
                {
                    mProject.RegisterGlobalValue(this); // TODO: 7/10/08: why don't we register them for TestSequence scope?  aren't they retentive at that level?
                }
                    /*
                if (value != mDataScope)
                {
                    if (value == DataScope.Project && mDataScope == DataScope.TestSequence)
                    {
                        foreach (TestSequence seq in mProject.mTestSequences)
                        {
                            if (seq.mGlobalValues.Contains(this)) seq.mGlobalValues.Remove(this);
                        }
                        mProject.mGlobalValues.Add(this);
                    }
                    else if (value == DataScope.TestSequence && mDataScope == DataScope.Project)
                    {
                        mProject.mGlobalValues.Remove(this);
                        foreach (DataValueDefinition valDef in mUpdaters)
                        {
                            if (seq.mGlobalValues.Contains(this)) seq.mGlobalValues.Remove(this);
                        }
                    }
                    else
                    {
                    }
                }
             */
            }
        }

        private List<DataValueDefinition> mUpdaters = new List<DataValueDefinition>();
        public void RegisterUpdater(DataValueDefinition theUpdater)
        {
            mUpdaters.Add(theUpdater);
            if (Scope == DataScope.TestSequence && !theUpdater.TestSequence().mGlobalValues.Contains(this))
            {
                theUpdater.TestSequence().RegisterGlobalValue(this);
                int x = 0;
                foreach (TestSequence seq in mProject.mTestSequences)
                {
                    if (seq.mGlobalValues.Contains(this)) x++;
                }
                if (x > 1) throw new ArgumentException("GlobalValue is scoped to TestSequence, but is updated by " + x + " TestSequences.");
            }
        }
        public void UnregisterUpdater(DataValueDefinition theUpdater)
        {
            mUpdaters.Remove(theUpdater);
        }

        protected string name = "";
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                // TODO: ensure unique
                name = value;
            }
        }
        public override string ToString()
        {
            return Name;
        }

        // TODO: YUCK...storing the value in multiple types!  consider storing a reference to a subobject that stores the actual type. This would only make since if we had a huge number of data items.  References themselves take 4 bytes.   The best possible savings would be for an Integer (not long) or Boolean which are 2 bytes...so ref+2bytes could save 6 types over storing Long (4bytes) and Double (8 bytes).  What about using a variant (16bytes) or "generic 8 bytes"?
        protected long mValueAsLong; //4 bytes?  8?
        protected double mValueAsDouble; //8 bytes
        public string Value
        {
            get
            {
                if (mDataType == DataType.Boolean)
                {
                    if (mValueAsLong == 0) return "False";
                    return "True";
                }
                else if (mDataType == DataType.DecimalNumber)
                {
                    string valueAsString = "" + mValueAsDouble;
                    if (valueAsString.IndexOf('.') < 0) valueAsString += ".0";
                    return valueAsString;
                }
                else if (mDataType == DataType.IntegerNumber)
                {
                    return "" + mValueAsLong;
                }
                return "" + mValueAsDouble;
            }
            set
            {
                if (mDataType == DataType.NotDefined)
                {
                    string trimmedValue = value.Trim();
                    // TODO: make these decisions smarter and/or improve error messages?
                    if (trimmedValue.Length > 0)
                    {
                        if (trimmedValue.IndexOf('.') >= 0)
                        {
                            Type = DataType.DecimalNumber;
                        }
                        else if (Char.IsDigit(trimmedValue[0]) || trimmedValue[0] == '-')
                        {
                            Type = DataType.IntegerNumber;
                        }
                        else
                        {
                            Type = DataType.Boolean;
                        }
                    }
                }

                // CONVERSION FROM STRING TO DATA VALUE: this code exists in multiple places currently...as it is improved, it should be improved everywhere
                switch (mDataType)
                {
                    case DataType.Boolean:
                        SetValue(bool.Parse(value));
                        break;
                    case DataType.DecimalNumber:
                        SetValue(double.Parse(value));
                        break;
                    case DataType.IntegerNumber:
                        SetValue(long.Parse(value));
                        break;
                    default:
                        throw new ArgumentException("Data Type not supported by GlobalValue");
                }
            }
        }

        public void SetValue(long theValue)
        {
            mValueAsLong = theValue;
            mValueAsDouble = theValue;
            HandleValueChange();
        }

        public void SetValue(double theValue)
        {
            mValueAsLong = (long)theValue;
            mValueAsDouble = theValue;
            HandleValueChange();
        }

        public void SetValue(bool theValue)
        {
            if (theValue)
            {
                mValueAsLong = 1;
            }
            else
            {
                mValueAsLong = 0;
            }
            mValueAsDouble = mValueAsLong;
            HandleValueChange();
        }

        private void HandleValueChange()
        {
            if (GlobalValueChanged != null)
            {
                GlobalValueChanged(this);
            }
        }

        public long ValueAsLong()
        {
            return mValueAsLong;
        }

        public double ValueAsDecimal()
        {
            return mValueAsDouble;
        }

        public bool ValueAsBoolean()
        {
            return mValueAsLong != 0;
        }
    }

    public class GlobalValueConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(((IObjectDefinition)context.Instance).TestSequence().GlobalValueOptions());
            }
            else if (context.Instance is FavoriteSettings)
            {
                return new StandardValuesCollection(((FavoriteSettings)context.Instance).mTestSequence.GlobalValueOptions());
            }
            else
            {
                throw new ArgumentException("Unexpected context (352323)");
            }
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(GlobalValue))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is GlobalValue)
            {

                GlobalValue globalValue = (GlobalValue)value;

                return globalValue.Name;
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
                GlobalValue namedGlobalValue;

                try
                {
                    string globalValueName = (string)value;
                    if (globalValueName.Length == 0) return null;

                    if (context.Instance is IObjectDefinition)
                    {
                        IObjectDefinition objectBeingEditted = (IObjectDefinition)context.Instance;
                        namedGlobalValue = objectBeingEditted.TestSequence().GetGlobalValueIfExists(globalValueName);
                    }
                    else if (context.Instance is FavoriteSettings)
                    {
                        namedGlobalValue = ((FavoriteSettings)context.Instance).mTestSequence.GetGlobalValueIfExists(globalValueName);
                    }
                    else
                    {
                        throw new ArgumentException("alksdjalksdlkajlkaj");
                    }
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to an GlobalValue");
                }
                if (namedGlobalValue == null)
                {
                    throw new ArgumentException("GlobalValue '" + (string)value + "' couldn't be found.");
                }
                return namedGlobalValue;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

}

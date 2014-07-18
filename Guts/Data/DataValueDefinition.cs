// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace NetCams
{
    public enum DataType
    {
        NotDefined = 0,
        IntegerNumber = 1,
        DecimalNumber = 2,
        Boolean = 3
    }

    public enum DataCategory
    {
        NotDefined = 0,
        NamedValue,
        UnnamedCalculatedValue,
        UnnamedConstant
    }

    /// <summary>
	/// 
	/// </summary>
    [TypeConverter(typeof(DataValueDefinitionConverter))]
    public abstract class DataValueDefinition : DataDefinition, IReferencePointDefinition
	{
		public DataValueDefinition(TestSequence testSequence) : base(testSequence)
		{
            testSequence.DataValueRegistry.RegisterObject(this);
            testSequence.ReferencePointRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().DataValueRegistry.UnRegisterObject(this);
            TestSequence().ReferencePointRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        protected DataType mDataType = DataType.NotDefined;
        public DataType Type
        {
            get { return mDataType; }
            set
            {
                HandlePropertyChange(this, "Type", mDataType, value);
                mDataType = value;
            }
        }

        protected DataCategory mDataCategory = DataCategory.NotDefined;
        public DataCategory Category
        {
            get { return mDataCategory; }
        }
        public void SetDataCategory(DataCategory theCategory)
        {
            mDataCategory = theCategory;
        }

        protected GlobalValue mGlobalValueToUpdate;
        public virtual GlobalValue GlobalValueToUpdate
        {
            get { return mGlobalValueToUpdate; }
            set
            {
                if (value != mGlobalValueToUpdate)
                {
                    HandlePropertyChange(this, "GlobalValueToUpdate", mGlobalValueToUpdate, value);
                    if (mGlobalValueToUpdate != null)
                    {
                        mGlobalValueToUpdate.UnregisterUpdater(this);
                    }
                    mGlobalValueToUpdate = value;
                    if (mGlobalValueToUpdate != null)
                    {
                        mGlobalValueToUpdate.RegisterUpdater(this);
                    }
                }
            }
        }
    }

    public class DataValueDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.DataValueRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.DataValueRegistry.GetObject(theObjectName);
        }
    }

//    public class DataValueDefinitionRegistry<T> : DefinitionObjectRegistry<T> where T : DataValueDefinition
    public class DataValueDefinitionRegistry : DefinitionObjectRegistry<DataValueDefinition>
    {
        public DataValueDefinitionRegistry(TestSequence theTestSequence, string theObjectType)
            : base(theObjectType)
        {
            mTestSequence = theTestSequence;
        }

        protected TestSequence mTestSequence;
        private ConstantValueDefinition mTrueObject;
        private ConstantValueDefinition mFalseObject;
        private ConstantValueDefinition mPiObject;

        public override void SortOptions()
        {
            mDefObjects.Sort(DataValueDefinitionComparer.Singleton);
        }

        public override DataValueDefinition GetObjectIfExists(string theName)
        {
            theName = theName.Trim(); // make sure we've trimmed off leading/trailing whitespace before we attempt to process it
            String theName_upper = theName.ToUpper();
            if (theName_upper == "TRUE")
            {
                if (mTrueObject == null)
                {
                    mTrueObject = new ConstantValueDefinition(mTestSequence);
                    mTrueObject.SetIncludeObjectInConfigFile(false);
                    mTrueObject.SetIncludeObjectInProgrammingTable(false);
                    mTrueObject.SetDataCategory(DataCategory.UnnamedConstant);
                    mTrueObject.Type = DataType.Boolean;
                    mTrueObject.SetValue(true);
                    mTrueObject.Name = "" + true;
                }
                return mTrueObject;
            }
            else if (theName_upper == "FALSE")
            {
                if (mFalseObject == null)
                {
                    mFalseObject = new ConstantValueDefinition(mTestSequence);
                    mFalseObject.SetIncludeObjectInConfigFile(false);
                    mFalseObject.SetIncludeObjectInProgrammingTable(false);
                    mFalseObject.SetDataCategory(DataCategory.UnnamedConstant);
                    mFalseObject.Type = DataType.Boolean;
                    mFalseObject.SetValue(false);
                    mFalseObject.Name = "" + false;
                }
                return mFalseObject;
            }
            else if (theName_upper == "PI")
            {
                if (mPiObject == null)
                {
                    mPiObject = new ConstantValueDefinition(mTestSequence);
                    mPiObject.SetIncludeObjectInConfigFile(false);
                    mPiObject.SetIncludeObjectInProgrammingTable(false);
                    mPiObject.SetDataCategory(DataCategory.UnnamedConstant); // TODO: is this best?  would like name="Pi", but don't let user change the value
                    mPiObject.Type = DataType.Boolean;
                    mPiObject.SetValue(3.14159265358979323846);
                    mPiObject.Name = "" + 3.14159265358979323846;
                }
                return mPiObject;
            }
            foreach (DataValueDefinition defObject in mDefObjects)
            {
                if (defObject.Name.Equals(theName))
                {
                    return (DataValueDefinition)defObject;
                }
            }
            if (theName.Length > 0)
            {
                bool couldBeAnInteger = true;
                bool tryADecimal = false;
                bool tryCalculation = false;
                for (int x = 0; x < theName.Length; x++)
                {
                    if (Char.IsDigit(theName[x]) || theName[x] == '-' || theName[x] == ',' || theName[x] == '.')
                    {
                        if (theName[x] == '.')
                        {
                            if (couldBeAnInteger)
                            {
                                // when we see the first '.', then we change from considering it an integer to a decimal
                                couldBeAnInteger = false;
                                tryADecimal = true;
                            }
                            else if (tryADecimal)
                            {
                                // if we see multiple '.', then it can no longer be a decimal
                                tryADecimal = false;
                            }
                        }
                        else if (theName[x] == '-' && x > 0)
                        {
                            tryADecimal = false;
                            couldBeAnInteger = false;
                            tryCalculation = true;
                        }
                    }
                    else if (Char.IsWhiteSpace(theName[x]) || theName[x] == '(' || Char.IsSymbol(theName[x])) // I'm not sure what classifies as a "symbol"...at the moment, I mostly care about white space and '(' (e.g. for function call)
                    {
                        tryADecimal = false;
                        couldBeAnInteger = false;
                        tryCalculation = true;
                    }
                    else
                    {
                        tryADecimal = false;
                        couldBeAnInteger = false;
                    }
                }
                if (tryCalculation)
                {
                    CalculatedValueDefinition calcedValue = new CalculatedValueDefinition(mTestSequence); // here we are only using the CalculatedValueDefinition to create a MathOpResult which we want...after we get the MathOpResultDef, we immediately dispose of the CalculatedValueDef (and thus CalcToolDef)
                    calcedValue.SetIncludeObjectInConfigFile(false);
                    calcedValue.SetIncludeObjectInProgrammingTable(false);
                    try
                    {
                        calcedValue.Calculation = theName;
                        return calcedValue.CalculationTool().RootOperation.Result;
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("This expression can't be found as a named value or parsed as a calculation.  Expression='" + theName + "'  Message='" + e.Message + "'");
                    }
                    finally
                    {
                        calcedValue.Dispose_UVision();
                    }
                }
                if (tryADecimal || couldBeAnInteger)
                {
                    ConstantValueDefinition theNumberAsObject = new ConstantValueDefinition(mTestSequence);
                    theNumberAsObject.SetIncludeObjectInConfigFile(false);
                    theNumberAsObject.SetIncludeObjectInProgrammingTable(false);
                    theNumberAsObject.SetDataCategory(DataCategory.UnnamedConstant);
                    try
                    {
                        if (tryADecimal)
                        {
                            double theValue = double.Parse(theName); // testing to see if it works...will throw exception if it fails
                            string valueAsString = "" + theValue;
                            if (valueAsString.IndexOf('.') < 0) valueAsString += ".0";
                            theNumberAsObject.Name = valueAsString;
                            theNumberAsObject.Value = valueAsString;
                        }
                        else
                        {
                            long theValue = long.Parse(theName); // testing to see if it works...will throw exception if it fails
                            theNumberAsObject.Name = "" + theValue;
                            theNumberAsObject.Value = "" + theValue;
                        }
                    }
                    catch (Exception e)
                    {
                        theNumberAsObject.Dispose_UVision();
                        throw e;
                    }
                    return theNumberAsObject;
                }
            }
            return null;
            //throw new ArgumentException("Object '" + theName + "' doesn't exist (looking for a number).");
        }
        public DataValueDefinition GetBooleanObject(string theName)
        {
            foreach (DataValueDefinition value in mDefObjects)
            {
                if (value.Name.Equals(theName))
                {
                    if (value.Type != DataType.Boolean)
                    {
                        throw new ArgumentException("'" + theName + "' isn't a boolean value.");
                    }
                    return value;
                }
            }
            throw new ArgumentException("Object '" + theName + "' doesn't exist (looking for boolean).");
        }
        public string[] BooleanOptions()
        {
            List<DataValueDefinition> theBooleans = new List<DataValueDefinition>();
            foreach (DataValueDefinition value in mDefObjects)
            {
                if (value.Type == DataType.Boolean)
                {
                    theBooleans.Add(value);
                }
            }
            string[] options = new string[theBooleans.Count];
            int i = 0;
            foreach (DataValueDefinition ob in theBooleans)
            {
                options[i++] = ob.Name;
            }
            return options;
        }

    }
    public class DataValueDefinitionComparer : System.Collections.Generic.IComparer<DataValueDefinition>
    {
        public DataValueDefinitionComparer()
        {
        }

        public static DataValueDefinitionComparer Singleton = new DataValueDefinitionComparer();

        public int Compare(DataValueDefinition a, DataValueDefinition b)
        {
            int categoryDiff = a.Category - b.Category;
            if (categoryDiff != 0) return categoryDiff;
            return a.Name.CompareTo(b.Name);
        }
    }
}

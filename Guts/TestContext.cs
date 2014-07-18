// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public abstract class TestContext
    {
        public abstract string Name { get; set; }

        private bool mFullyInitialized = false;
        public bool FullyInitialized
        {
            get { return mFullyInitialized; }
        }
        public void SetFullyInitialized() { mFullyInitialized = true; }

        protected DataValueRetentionFile mDataValueRetentionFile;
        public List<GlobalValue> mGlobalValues = new List<GlobalValue>();
        public void RegisterGlobalValue(GlobalValue theGlobalValue)
        {
            mGlobalValues.Add(theGlobalValue);
            mDataValueRetentionFile.RegisterGlobalValue(theGlobalValue);
        }
        public void UnRegisterGlobalValue(GlobalValue theGlobalValue)
        {
            mGlobalValues.Remove(theGlobalValue); // WARNING: the implementation of mDataValueRetentionFile.UnregisterGlobalValue(value) assumes we remove the value from mGlobalValues before calling the unregister method
            mDataValueRetentionFile.UnregisterGlobalValue(theGlobalValue);
        }
        public virtual GlobalValue GetGlobalValueIfExists(string theName)
        {
            foreach (GlobalValue globalValue in mGlobalValues)
            {
                if (globalValue.Name.Equals(theName))
                {
                    return globalValue;
                }
            }
            return null;
        }
        public virtual GlobalValue GetGlobalValue(string theName)
        {
            GlobalValue result = GetGlobalValueIfExists(theName);
            if (result == null) throw new ArgumentException("GlobalValue '" + theName + "' does not exist.");
            return result;
        }

        public abstract Project project();
        public void LogMessage(string msg)
        {
            project().Window().logMessage(msg);
        }
        public void LogError(string msg)
        {
            project().Window().logMessage("ERROR: " + msg);
        }
        public void LogWarning(string msg)
        {
            project().Window().logMessage("WARNING: " + msg);
        }
    }
}

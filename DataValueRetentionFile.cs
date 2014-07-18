// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCams
{
    public class DataValueRetentionFile
    {
        public DataValueRetentionFile(TestContext testContext)
        {
            mTestContext = testContext;
            mUpdateTimer.Interval = 1000;
            mUpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(mUpdateTimer_Elapsed);
        }

        void mUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs eventArgs)
        {
            try
            {
                mUpdateTimer.Enabled = false;
                if (mFileIsOutdated)
                {
                    SaveDataToFile();
                }
            }
            catch (Exception e)
            {
                mTestContext.LogError("Problem updating retentive data file for " + mTestContext.Name + ". Message=" + e.Message);
            }
            finally
            {
                mUpdateTimer.Enabled = true;
            }
        }

        private TestContext mTestContext;

        private List<GlobalValue> mGlobalValues = new List<GlobalValue>();
        public void RegisterGlobalValue(GlobalValue theValue)
        {
            // NOTE: timer isn't enabled until the first value is registered.  It is possible to be re-enabled during a save, but that shouldn't be a problem unless the timer interval is REALLY short
            mUpdateTimer.Enabled = true;
            theValue.GlobalValueChanged += new GlobalValue.GlobalValueDelegate(GlobalValueChanged);
        }

        public void UnregisterGlobalValue(GlobalValue theValue)
        {
            theValue.GlobalValueChanged -= new GlobalValue.GlobalValueDelegate(GlobalValueChanged);
            if (mTestContext.mGlobalValues.Count == 0)
            {
                // if there are no globals left after this one, disable the timer
                // warning: this assumes they are unregistered from mTestContext.mGlobalValues before we get here
                mUpdateTimer.Enabled = false;
            }
        }

        private bool mFileIsOutdated = false;
        public void GlobalValueChanged(GlobalValue globalValue)
        {
            if (mTestContext.FullyInitialized) mFileIsOutdated = true;
        }

        private System.Timers.Timer mUpdateTimer = new System.Timers.Timer();
        public void SaveDataToFile()
        {
            if (writer == null) OpenFileForWriting();

            foreach (GlobalValue globalValue in mTestContext.mGlobalValues)
            {
                if (globalValue.IsRetentive)
                {
                    writer.WriteLine(globalValue.Name + " = " + globalValue.Value);
                }
            }

            CloseFileFromWriting();

            mFileIsOutdated = false;
        }

        private StreamWriter writer = null;
        private StreamReader reader = null;
        public void OpenFileForWriting()
        {
            if (writer != null) CloseFileFromWriting();
            writer = new StreamWriter(mTestContext.Name + ".ret");
        }
        public void CloseFileFromWriting()
        {
            writer.Close();
            //File.Delete(mFile);
            //File.Move(mFile + ".tmp", mFile);
            writer.Dispose();
            writer = null;
        }

        private GlobalValue.DataScope mDataScope = GlobalValue.DataScope.NotDefined;
        public void LoadDataFromFile()
        {
            if (writer != null) CloseFileFromWriting();
            try
            {
                mTestContext.LogMessage("Loading retentive file...");
                reader = new StreamReader(mTestContext.Name + ".ret");
                do
                {
                    // csline is case sensitive, while line is in lowercase
                    string line = reader.ReadLine().Trim();

                    try
                    {
                        int assignmentIndex = line.IndexOf("=");
                        if (assignmentIndex < 0)
                        {
                            mTestContext.LogError("Line in Data Value Retention File is missing an '=' sign.  Line='" + line + "'");
                            continue;
                        }
                        string dataValueName = line.Substring(0, assignmentIndex).Trim();
                        string dataValueAsString = line.Substring(assignmentIndex + 1).Trim();

                        GlobalValue globalValue = mTestContext.GetGlobalValueIfExists(dataValueName);
                        if (globalValue == null)
                        {
                            mTestContext.LogWarning("GlobalValue '" + dataValueName + "' exists in the retentive file, but not in the test context.");
                        }
                        else if (globalValue.IsRetentive)
                        {
                            globalValue.Value = dataValueAsString;
                        }
                    }
                    catch (Exception e)
                    {
                        mTestContext.LogError("Problem processing line in data value retentive file. Line='" + line + "'. Message=" + e.Message);
                    }
                } while (reader.Peek() != -1);
            }
            catch (Exception e)
            {
                mTestContext.LogError("Problem reading data value retentive file. Message=" + e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
        }
    }
}

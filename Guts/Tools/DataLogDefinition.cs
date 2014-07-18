// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

namespace NetCams
{
    public class DataLogDefinition : NetCams.ToolDefinition
    {
        public DataLogDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new DataLogInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mEnabled != null && mEnabled.IsDependentOn(theOtherObject)) return true;
            foreach (DataValueDefinition value in mValuesToLog)
            {
                if (value != null && value.IsDependentOn(theOtherObject)) return true;
            }
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mEnabled != null) result = Math.Max(result, mEnabled.ToolMapRow);
                foreach (DataValueDefinition value in mValuesToLog)
                {
                    if (value != null) result = Math.Max(result, value.ToolMapRow);
                }
                return result + 1;
			}
		}

        private DataValueDefinition mEnabled;
        [CategoryAttribute("Administrative"),
        DescriptionAttribute("")]
        public DataValueDefinition Enabled
        {
            get { return mEnabled; }
            set
            {
                HandlePropertyChange(this, "Enabled", mEnabled, value);
                mEnabled = value;
            }
        }

        [CategoryAttribute("Administrative"),
        DescriptionAttribute("WARNING: Setting this to True will erase all existing data.")]
        public bool ResetFileNow
        {
            get { return false; }
            set
            {
                HandlePropertyChange(this, "ResetFileNow", false, value);

                if (!value) return;

                if (writer != null) CloseFileFromWriting(null);
                if (mFile != null && mFile != string.Empty) System.IO.File.Delete(mFile);

                OpenFileForWriting(null);

                // write data log header to new empty file
                if (writer != null)
                {
                    try
                    {
                        string result = "Date, Time, ";
                        string seperator = string.Empty;
                        foreach (DataValueDefinition valueName in mValuesToLog)
                        {
                            result += seperator + valueName.Name;
                            seperator = ", ";
                        }

                        writer.WriteLine(result);
                    }
                    catch (Exception e)
                    {
                        string msg = "Unable to write header to data log file " + mFile + ".  Error='" + e.Message + "'";
                        TestSequence().LogError(msg);
                    }
                    writer.Flush();
                    if (mCloseAfterEachLine) CloseFileFromWriting(null);
                }
            }
        }

        private String mFile;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public String File
        {
            get { return mFile; }
            set
            {
                HandlePropertyChange(this, "File", mFile, value);
                mFile = value;
            }
        }

        public List<DataValueDefinition> mValuesToLog = new List<DataValueDefinition>();
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("A comma-seperated list of values to write in each entry.")]
        public String ValuesToLog
        {
            get
            {
                // this isn't particularly efficient, but without building this each time I can't think of a way to keep it accurate if someone changes an object's name (listen for Name changes of objects we're dependent on?)
                string result = string.Empty;
                string seperator = string.Empty;
                foreach (DataValueDefinition value in mValuesToLog)
                {
                    result += seperator + value.Name;
                    seperator = ", ";
                }
                return result;
            }
            set
            {
                string[] valuesToLog_array = value.Split(new char[] { ',' });
                string valueName_trimmed = string.Empty;
                mValuesToLog.Clear();
                foreach (string valueName in valuesToLog_array)
                {
                    valueName_trimmed = valueName.Trim();
                    if (valueName_trimmed.Length > 0)
                    {
                        mValuesToLog.Add(TestSequence().DataValueRegistry.GetObject(valueName_trimmed));
                    }
                }
            }
        }

        private bool mCloseAfterEachLine = false;
        [CategoryAttribute("Performance"),
        DescriptionAttribute("")]
        public bool CloseAfterEachLine
        {
            get { return mCloseAfterEachLine; }
            set
            {
                HandlePropertyChange(this, "CloseAfterEachLine", mCloseAfterEachLine, value);
                mCloseAfterEachLine = value;
            }
        }

        private bool mFlushAfterEachLine = true;
        [CategoryAttribute("Performance"),
        DescriptionAttribute("")]
        public bool FlushAfterEachLine
        {
            get { return mFlushAfterEachLine; }
            set
            {
                HandlePropertyChange(this, "FlushAfterEachLine", mFlushAfterEachLine, value);
                mFlushAfterEachLine = value;
            }
        }

        public StreamWriter writer = null;

        public void InitDataLogFile(TestExecution testExecution)
        {

        }

        public void OpenFileForWriting(TestExecution testExecution)
        {
            // WARNING: testExecution may be null
            try
            {
                if (writer != null) CloseFileFromWriting(testExecution);
            }
            catch (Exception e)
            {
                string msg = "Unable to successfully close the data log file before re-opening.  Error='" + e.Message + "'";
                if (testExecution != null) testExecution.LogErrorWithTimeFromTrigger(msg);
                TestSequence().LogError(msg);
            }

            string filePath = FileHelper.ExpandPath(this, mFile);
            string fileName = Path.GetFileName(filePath);
            filePath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(filePath))
            {
                try
                {
                    Directory.CreateDirectory(filePath);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Unable to open data log file '" + filePath + "'. Reason=Unable to create path.  Low-level message=" + e.Message);
                }
            }
            try
            {
                writer = new StreamWriter(filePath + "\\" + fileName, true); // true=append
            }
            catch (Exception e)
            {
                string msg = "Unable to open the data log file " + mFile + ".  Error='" + e.Message + "'";
                if (testExecution != null) testExecution.LogErrorWithTimeFromTrigger(msg);
                TestSequence().LogError(msg);
            }
        }

        public void CloseFileFromWriting(TestExecution testExecution)
        {
            // WARNING: testExecution may be null
            if (writer != null)
            {
                try
                {
                    writer.Close();
                }
                catch (Exception e)
                {
                    string msg = "Unable to close data log file " + mFile + ".  Error='" + e.Message + "'";
                    if (testExecution != null) testExecution.LogErrorWithTimeFromTrigger(msg);
                    TestSequence().LogError(msg);
                    return;
                }
                try
                {
                    writer.Dispose(); // can this throw exceptions?  if so, it isn't fatal to the test execution, so we catch it here
                }
                catch (Exception e)
                {
                    string msg = "Unable to dispose data log file " + mFile + ".  Error='" + e.Message + "'";
                    if( testExecution != null) testExecution.LogErrorWithTimeFromTrigger(msg);
                    TestSequence().LogError(msg);
                }
                writer = null;
            }
        }

        public void AddLine(TestExecution testExecution)
        {
            DateTime now = DateTime.Now;
            String seperator = ", ";
            String theEntry = now.ToShortDateString() + seperator + now.ToLongTimeString();
            try
            {
                foreach (DataValueDefinition value in mValuesToLog)
                {
                    theEntry += seperator + testExecution.DataValueRegistry.GetObject(value.Name).Value;
                }
            }
            catch (Exception e)
            {
                string msg = "Unable to build data log entry for " + mFile + ".  Error='" + e.Message + "'";
                testExecution.LogErrorWithTimeFromTrigger(msg);
                TestSequence().LogError(msg);
            }

            testExecution.LogMessageWithTimeFromTrigger(Name + " is logging '" + theEntry + "'");

            if (writer == null) OpenFileForWriting(testExecution);

            if (writer != null)
            {
                try
                {
                    writer.WriteLine(theEntry);
                }
                catch (Exception e)
                {
                    string msg = "Unable to write entry to data log file " + mFile + ".  Error='" + e.Message + "'";
                    testExecution.LogErrorWithTimeFromTrigger(msg);
                    TestSequence().LogError(msg);
                }
                if (mFlushAfterEachLine) writer.Flush();
                if (mCloseAfterEachLine) CloseFileFromWriting(testExecution);
            }
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Reflection;

namespace NetCams
{
    class ProjectConfigFile
    {
        public ProjectConfigFile(string theFile)
        {
            mFile = theFile;
        }

        private string mFile;
        public string FilePath
        {
            set { mFile = value; }
            get { return mFile; }
        }

        private StreamWriter writer = null;
        private StreamReader reader = null;

        public void OpenFileForWriting()
        {
            writer = new StreamWriter(mFile + ".tmp");
        }

        public void AddObject(Object theObject)
        {
            writer.WriteLine();

            Type objectType = theObject.GetType();
            //writer.WriteLine("[" + objectType.Assembly.GetName() + "." + objectType.Name + "]");
            string objectTypeLine = "[" + objectType.Name + "]";

            writer.WriteLine(objectTypeLine);
            foreach (PropertyInfo propInfo in objectType.GetProperties())
            {
                if (propInfo.CanWrite && propInfo.Name != "SelectedTestSequence") // TODO: add attributes to indicate which properties should be saved...which have defaults, etc
                {
                    object propertyValue = propInfo.GetValue(theObject, null);
                    if (propertyValue != null)
                    {
                        writer.WriteLine(propInfo.Name + " = " + TypeDescriptor.GetConverter(propertyValue).ConvertToString(propertyValue));
                    }
                }
            }
        }

        public void CloseFileFromWriting()
        {
            writer.Close();
            File.Delete(mFile);
            File.Move(mFile + ".tmp", mFile);
            writer.Dispose();
            writer = null;
        }

        public void LoadObject(Project project)
        {
            reader = new StreamReader(mFile);

            Object theObject = null;
            Type[] constructorParameterTypes = { typeof(Project) };
            Type[] emptyConstructorParameterTypes = { };
            object[] constructorParameters = { project };
			do
			{				
				// csline is case sensitive, while line is in lowercase
				string line = reader.ReadLine().Trim();

                try
                {
                    // Bypass comments and empty lines
                    if ((line.IndexOf("#") == 0) || (line.Length == 0)) continue;

                    if (line.StartsWith("["))
                    {
                        // http://www.eggheadcafe.com/articles/20050717.asp  (see comment about System.Activator.CreateInstance also)
                        if (!line.EndsWith("]")) throw new ArgumentException("invalid configuration line: '" + line + "'");
                        string typeName = line.Substring(1, line.Length - 2).Trim();

                        Type type = Type.GetType("NetCams." + typeName);
                        //theObject = Activator.CreateInstance(type);
                        if (type == null) throw new ArgumentException("Type not found. type='" + typeName + "'");
                        if (type.Name == "Project")
                        {
                            theObject = project;
                        }
                        else
                        {
                            ConstructorInfo ctorInfo = type.GetConstructor(constructorParameterTypes);
                            if (ctorInfo == null) throw new ArgumentException("Not a valid type; type='" + typeName + "'; line = '" + line + "'");
                            theObject = ctorInfo.Invoke(constructorParameters);
                            if (theObject == null) throw new ArgumentException("Can't instantiate type " + typeName);
                        }
                    }
                    else
                    {
                        if (theObject == null) throw new ArgumentException("Config error: need to define object before properties; line = '" + line + "'");
                        int assignmentIndex = line.IndexOf("=");
                        string propertyName = line.Substring(0, assignmentIndex).Trim();
                        if (propertyName == "HasUnsavedChanges" || propertyName == "HasUnusedChanges")
                        {
                            //skip these lines...I never intended these to be saved in the config and when I changed it from a property to methods we would get an error from config files that still contained it 10/9/09
                            // when the HasUnsavedChanges was in the config file, sometimes it would save as "True" and would falsely report changes when none were made
                        }
                        else
                        {
                            string propertyValueAsString = line.Substring(assignmentIndex + 1).Trim();
                            PropertyInfo propInfo = theObject.GetType().GetProperty(propertyName);
                            TypeConverter converter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                            ITypeDescriptorContext context = new ProjectContext(theObject);
                            object propertyValueAsObject = converter.ConvertFromString(context, propertyValueAsString);
                            propInfo.SetValue(theObject, propertyValueAsObject, null);
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    project.Window().logMessage("ERROR: problem loading project configuration file.");
                    project.Window().logMessage("ERROR: ...line='" + line + "'");
                    project.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                    Exception inner = e.InnerException;
                    while (inner != null)
                    {
                        project.Window().logMessage("ERROR: ...inner message='" + inner.Message + "'");
                        inner = inner.InnerException;
                    }
                }
                catch (Exception e)
                {
                    project.Window().logMessage("ERROR: exception loading project configuration file.");
                    project.Window().logMessage("ERROR: ...line='" + line + "'");
                    project.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                    Exception inner = e.InnerException;
                    while (inner != null)
                    {
                        project.Window().logMessage("ERROR: ...inner message='" + inner.Message + "'");
                        inner = inner.InnerException;
                    }
                }
            } while (reader.Peek() != -1);

            reader.Close();
            reader.Dispose();
            reader = null;
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Reflection;

// using reflection to determine object's properties: http://www.ondotnet.com/pub/a/dotnet/2003/10/06/reflectionpt1.html

namespace NetCams
{
    class TestSequenceConfigFile
    {
        public TestSequenceConfigFile(string theFile)
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
            /*
            if (theObject is MathOperationDefinition ||
                theObject is MathOpResultDefinition ||
                theObject is CalculationToolDefinition
                )
            {
                return;
            }
             */
            /* removed 7/3/08 since fixed support in ConstantValueDefinition for IncludeObjectInConfigFile()
            if (theObject is ConstantValueDefinition )
            {
                ConstantValueDefinition constValue = (ConstantValueDefinition)theObject;
                if (constValue.Name == "" + constValue.Value) return;
            }*/
            

            writer.WriteLine();

            Type objectType = theObject.GetType();
            //writer.WriteLine("[" + objectType.Assembly.GetName() + "." + objectType.Name + "]");
            string objectTypeLine = "[" + objectType.Name;

            OwnerLink ownerLink = null;
            if (theObject is IObjectDefinition)
            {
                ownerLink = ((IObjectDefinition)theObject).GetOwnerLink();
            }
            if (ownerLink != null)
            {
                objectTypeLine += " | " + ownerLink.Owner.Name + "." + ownerLink.Property.Name;
            }
            objectTypeLine += "]";
            writer.WriteLine(objectTypeLine);
            foreach (PropertyInfo propInfo in objectType.GetProperties())
            {
                if (propInfo.CanWrite)
                {
                    object propertyValue = propInfo.GetValue(theObject, null);
                    if (propertyValue == null)
                    {
                        writer.WriteLine(propInfo.Name + " = " );
                    }
                    else
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(propertyValue);
                        writer.WriteLine(propInfo.Name + " = " + converter.ConvertToString(propertyValue));
                    }
                }
            }
        }
        public void SaveFavoriteSettings(TestSequence testSequence)
        {
            writer.WriteLine(Environment.NewLine + "FavoriteSettings");
            foreach (string line in testSequence.mFavoriteSettingDefs)
            {
                writer.WriteLine(line);
            }
            writer.WriteLine("END");
        }
        public void SaveFavoriteValues(TestSequence testSequence)
        {
            writer.WriteLine(Environment.NewLine + "FavoriteValues");
            foreach (string line in testSequence.mFavoriteValuesDefs)
            {
                writer.WriteLine(line);
            }
            writer.WriteLine("END");
        }

        public void SaveImageFormSettings(TestSequence testSequence)
        {
            writer.WriteLine(Environment.NewLine + "ImageViews");
            foreach (string line in testSequence.mImageForms)
            {
                writer.WriteLine(line);
            }
            writer.WriteLine("END");
        }

        public void CloseFileFromWriting()
        {
            writer.Close();
            File.Delete(mFile);
            File.Move(mFile + ".tmp", mFile);
            writer.Dispose();
            writer = null;
        }

        public void LoadObject(TestSequence testSequence)
        {
            reader = new StreamReader(mFile);

            Object theObject = null;
            Type[] constructorParameterTypes = { typeof(TestSequence) };
            object[] constructorParameters = { testSequence };
			do
			{				
				// csline is case sensitive, while line is in lowercase
				string line = reader.ReadLine().Trim();

                try
                {
                    // Bypass comments and empty lines
                    if ((line.IndexOf("#") == 0) || (line.Length == 0)) continue;

                    if (line == "FavoriteSettings")
                    {
                        LoadFavoriteSettings(testSequence, reader);
                    }
                    else if (line == "FavoriteValues")
                    {
                        LoadFavoriteValues(testSequence, reader);
                    }
                    else if (line == "ImageViews")
                    {
                        LoadImageFormSettings(testSequence, reader);
                    }
                    else if (line.StartsWith("["))
                    {
                        theObject = null;
                        // http://www.eggheadcafe.com/articles/20050717.asp  (see comment about System.Activator.CreateInstance also)
                        if (!line.EndsWith("]")) throw new Exception("invalid configuration line: '" + line + "'");
                        string typeName = line.Substring(1, line.Length - 2).Trim();

                        int dividerIndex = line.IndexOf("|");
                        if (dividerIndex >= 0)
                        {
                            string ownerLinkPortion = typeName.Substring(dividerIndex + 1).Trim();
                            typeName = typeName.Substring(0, dividerIndex - 1).Trim();
                            int periodIndex = ownerLinkPortion.IndexOf(".");
                            string ownerName = ownerLinkPortion.Substring(0, periodIndex).Trim();
                            string ownerPropertyName = ownerLinkPortion.Substring(periodIndex + 1).Trim();
                            IObjectDefinition ownerObject = testSequence.ObjectRegistry.GetObject(ownerName);
                            theObject = ownerObject.GetType().GetProperty(ownerPropertyName).GetValue(ownerObject, null); // zzz: for rectangle.bottom the type is "string" not "ImageObjectDef"...for previous owner relationships they were objects.  change one type?  different owner/parameter link style?
                        }
                        else
                        {
                            if (typeName == "RectangleROIDefinition")
                            {
                                typeName = typeName.Trim();
                                //typeName = "RectangleROIByEdgesDefinition"; // TODO: HACK: 20080618: so that old config files don't break
                            }
                            Type type = Type.GetType("NetCams." + typeName);
                            //                    theObject = Activator.CreateInstance(type);
                            if (type == null) throw new ArgumentException("Type not found. type='" + typeName + "'");
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
                        string propertyValueAsString = line.Substring(assignmentIndex + 1).Trim();
                        PropertyInfo propInfo = theObject.GetType().GetProperty(propertyName);
                        TypeConverter converter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                        ITypeDescriptorContext context = new ProjectContext(theObject);
                        object propertyValueAsObject = converter.ConvertFromString(context, propertyValueAsString);
                        propInfo.SetValue(theObject, propertyValueAsObject, null);
                    }
                }
                catch (ArgumentException e)
                {
                    testSequence.Window().logMessage("ERROR: problem loading test sequence configuration file.");
                    testSequence.Window().logMessage("ERROR: ...file='" + mFile + "'");
                    testSequence.Window().logMessage("ERROR: ...line='" + line + "'");
                    testSequence.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                    Exception inner = e.InnerException;
                    while (inner != null)
                    {
                        testSequence.Window().logMessage("ERROR: ...inner message='" + inner.Message + "'");
                        inner = inner.InnerException;
                    }
                }
                catch (Exception e)
                {
                    testSequence.Window().logMessage("ERROR: exception loading test sequence configuration file.");
                    testSequence.Window().logMessage("ERROR: ...file='" + mFile + "'");
                    testSequence.Window().logMessage("ERROR: ...line='" + line + "'");
                    testSequence.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                    Exception inner = e.InnerException;
                    while (inner != null)
                    {
                        testSequence.Window().logMessage("ERROR: ...inner message='" + inner.Message + "'");
                        inner = inner.InnerException;
                    }
                }
            } while (reader.Peek() != -1);

            reader.Close();
            reader.Dispose();
            reader = null;
        }

        private void LoadFavoriteSettings(TestSequence testSequence, StreamReader reader)
        {
            FavoriteSettingsGenerator generator = new FavoriteSettingsGenerator(testSequence);
            string line;
            bool done = false;
            do
            {
                line = reader.ReadLine().Trim();
                try
                {
                    done = reader.Peek() == -1 || line == "END";
                    if (!done && line.Length > 0)
                    {
                        testSequence.mFavoriteSettingDefs.Add(line);
                        string[] lineComponents = line.Split(new char[] { '|' });
                        if (lineComponents.GetUpperBound(0) != 4) throw new ArgumentException("Invalid property definition for Favorite Setting; line='" + line + "'");
                        generator.AddProperty(lineComponents[0].Trim(), lineComponents[1].Trim(), lineComponents[2].Trim(), lineComponents[3].Trim(), lineComponents[4].Trim());
                    }
                }
                catch (ArgumentException e)
                {
                    testSequence.Window().logMessage("ERROR: problem loading favorite setting.");
                    testSequence.Window().logMessage("ERROR: ...file='" + mFile + "'");
                    testSequence.Window().logMessage("ERROR: ...line='" + line + "'");
                    testSequence.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                }
                catch (Exception e)
                {
                    testSequence.Window().logMessage("ERROR: exception loading favorite setting.");
                    testSequence.Window().logMessage("ERROR: ...file='" + mFile + "'");
                    testSequence.Window().logMessage("ERROR: ...line='" + line + "'");
                    testSequence.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                }
            }
            while (!done);
            generator.GenerateCode();
        }
        private void LoadFavoriteValues(TestSequence testSequence, StreamReader reader)
        {
            FavoriteValuesGenerator generator = new FavoriteValuesGenerator(testSequence);
            string line;
            bool done = false;
            do
            {
                line = reader.ReadLine().Trim();
                try
                {
                    done = reader.Peek() == -1 || line == "END";
                    if (!done && line.Length > 0)
                    {
                        testSequence.mFavoriteValuesDefs.Add(line);
                        string[] lineComponents = line.Split(new char[] { '|' });
                        if (lineComponents.GetUpperBound(0) != 4) throw new ArgumentException("Invalid property definition for Favorite Value; line='" + line + "'");
                        generator.AddValue(lineComponents[0].Trim(), lineComponents[1].Trim(), lineComponents[2].Trim(), lineComponents[3].Trim(), lineComponents[4].Trim());
                    }
                }
                catch (ArgumentException e)
                {
                    testSequence.Window().logMessage("ERROR: problem loading favorite value.");
                    testSequence.Window().logMessage("ERROR: ...file='" + mFile + "'");
                    testSequence.Window().logMessage("ERROR: ...line='" + line + "'");
                    testSequence.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                }
                catch (Exception e)
                {
                    testSequence.Window().logMessage("ERROR: exception loading favorite value.");
                    testSequence.Window().logMessage("ERROR: ...file='" + mFile + "'");
                    testSequence.Window().logMessage("ERROR: ...line='" + line + "'");
                    testSequence.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                }
            }
            while (!done);
            generator.GenerateCode();
        }

        private void LoadImageFormSettings(TestSequence testSequence, StreamReader reader)
        {
            string line;
            bool done = false;
            do
            {
                line = reader.ReadLine().Trim();
                try
                {
                    done = reader.Peek() == -1 || line == "END";
                    if (!done && line.Length > 0)
                    {
                        string[] mainSplitComponents = line.Split(new char[] { ':' });
                        if (mainSplitComponents.GetUpperBound(0) != 2) throw new ArgumentException("Invalid image for definition; line='" + line + "'");
                        testSequence.mImageForms.Add(line);
                    }
                }
                catch (ArgumentException e)
                {
                    testSequence.Window().logMessage("ERROR: problem loading image form settings.");
                    testSequence.Window().logMessage("ERROR: ...file='" + mFile + "'");
                    testSequence.Window().logMessage("ERROR: ...line='" + line + "'");
                    testSequence.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                }
                catch (Exception e)
                {
                    testSequence.Window().logMessage("ERROR: exception loading image form settings.");
                    testSequence.Window().logMessage("ERROR: ...file='" + mFile + "'");
                    testSequence.Window().logMessage("ERROR: ...line='" + line + "'");
                    testSequence.Window().logMessage("ERROR: ...message='" + e.Message + "'");
                }
            }
            while (!done);
        }

    }

    class ProjectContext : ITypeDescriptorContext
    {
        public ProjectContext(Object theObject)
        {
            mInstance = theObject;
        }

        private Object mInstance;

        #region ITypeDescriptorContext Members

        public IContainer Container
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object Instance
        {
            get { return mInstance; }
        }

        public void OnComponentChanged()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool OnComponentChanging()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

}

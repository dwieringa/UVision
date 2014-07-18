// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace NetCams
{
    class FavoriteValuesGenerator
    {
        public FavoriteValuesGenerator(TestSequence seq)
        {
            mTestSequence = seq;
            mSequenceNamed_sanitized = seq.Name.Replace(' ', '_');
            if (Char.IsDigit(mSequenceNamed_sanitized[0])) mSequenceNamed_sanitized = 'z' + mSequenceNamed_sanitized;
            // TODO: clean this up!
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('<', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('>', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('{', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('}', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('[', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace(']', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('|', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('\\', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('.', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace(',', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace(':', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace(';', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('?', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('\"', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('\'', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('`', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('~', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('!', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('@', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('#', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('$', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('%', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('^', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('&', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('*', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('(', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace(')', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('-', '_');
            mSequenceNamed_sanitized = mSequenceNamed_sanitized.Replace('+', '_');
        }

        public TestSequence mTestSequence;
        private string mSequenceNamed_sanitized;
        private string updateBody = "";
        private string classBody = "";
        private int objectCount = 1;

        public void AddValue(string favName, string category, string description, string objectName, string propertyName)
        {
            string objectNameForCode = "o" + objectCount++;
            IObjectDefinition defObj = mTestSequence.ObjectRegistry.GetObject(objectName);
            if (defObj == null) throw new ArgumentException("Can't find object '" + objectName + "'");

            string objectType = defObj.GetType().ToString();
            objectType = objectType.Substring(0, objectType.Length - 10) + "Instance";
//            string propertyType = defObj.GetType().GetProperty(propertyName).PropertyType.ToString();

            // o1 = (GeneratedValueInstance)mTestExecution.GetObjectIfExists("IntensityVariation");
            //updateBody += "\t\t" + objectNameForCode + " = (" + objectType + ")mTestExecution.GetObjectIfExists(\"" + objectName + "\");" + Environment.NewLine;
            // o1 = null; if(mTestExecution!=null) o1 = (GeneratedValueInstance)mTestExecution.GetObjectIfExists("IntensityVariation");
            updateBody += "\t\t" + objectNameForCode + " = null; if(mTestExecution!=null) " + objectNameForCode + " = (" + objectType + ")mTestExecution.ObjectRegistry.GetObject(\"" + objectName + "\");" + Environment.NewLine;

            // private GeneratedValueInstance o1;
            classBody += "\tprivate " + objectType + " " + objectNameForCode + ";" + Environment.NewLine;

            // property attributes
            // [CategoryAttribute("Parameters"),DescriptionAttribute("Definition of the color to search for")]
            classBody += "\t[CategoryAttribute(\"" + category + "\"),DescriptionAttribute(\"" + description + "\")]" + Environment.NewLine;

            // create the property
            /*
                public string FavoriteTitle
                {
                    get { return o1 == null ? "" : o1.Value; }
                }
             */
            classBody += "\tpublic string " + favName + Environment.NewLine;  // NOTE: casting everything to string so we can easilly handle o1==null...
            classBody += "\t{" + Environment.NewLine;
            classBody += "\t\tget { return " + objectNameForCode + " == null ? \"\" : " + objectNameForCode + "." + propertyName + "; }" + Environment.NewLine;
//            classBody += "\t\tset {}" + Environment.NewLine;
            classBody += "\t}" + Environment.NewLine;
        }

        public void GenerateCode()
        {
            // open the file for writing
            string fileName = "FavValues";
            Stream s = File.Open(fileName + ".cs", FileMode.Create);
            StreamWriter wrtr = new StreamWriter(s);
            wrtr.WriteLine("// Dynamically created FavValues class");
            wrtr.WriteLine("using System.ComponentModel;");
            wrtr.WriteLine("using NetCams;");

            // create the class
            string className = mSequenceNamed_sanitized+"_FavoriteValues";
            wrtr.WriteLine("class {0} : NetCams.FavoriteValues", className);
            wrtr.WriteLine("{");

            // create the method
            wrtr.WriteLine("\tpublic {0}()", className);
            wrtr.WriteLine("\t{ }");
            wrtr.WriteLine("\t");

            wrtr.WriteLine("\tprotected override void UpdateValues()");
            wrtr.WriteLine("\t{");
            wrtr.Write(updateBody);
            wrtr.WriteLine("\t}");
            wrtr.WriteLine("\t");

            wrtr.Write(classBody);

            wrtr.WriteLine("}");    // end class

            // close the writer and the stream
            wrtr.Close();
            s.Close();

            CompilerParameters cp = new CompilerParameters();

            // Generate an executable instead of a class library.
            cp.GenerateExecutable = false;

            // Specify the assembly file name to generate.
            //cp.OutputAssembly = exeName;

            // Save the assembly as a physical file.
            cp.GenerateInMemory = true;

            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;

            // Add an assembly reference.
            cp.ReferencedAssemblies.Add("UVision.exe");
            cp.ReferencedAssemblies.Add("System.dll");

            // Invoke compilation of the source file.
            string sourceName = fileName + ".cs";
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}", sourceName, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                // Display a successful compilation message.
                Console.WriteLine("Source {0} built into {1} successfully.", sourceName, cr.PathToAssembly);

                Assembly a = cr.CompiledAssembly;
                mTestSequence.mFavoriteValues = (FavoriteValues)a.CreateInstance(className, false, BindingFlags.Default | BindingFlags.CreateInstance, null, null, null, null);
            }

            //File.Delete(fileName + ".cs"); // clean up
        }
    }
}

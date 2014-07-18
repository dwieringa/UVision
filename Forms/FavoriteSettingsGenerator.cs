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
    class FavoriteSettingsGenerator
    {
        public FavoriteSettingsGenerator(TestSequence seq)
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
        private string ctorBody = "";
        private string classBody = "";
        private int objectCount = 1;

        public void AddProperty(string favName, string category, string description, string objectName, string propertyName)
        {
            bool isGlobal = false;
            string objectNameForCode = "o" + objectCount++;
            Object defObj = mTestSequence.ObjectRegistry.GetObjectIfExists(objectName);
            if (defObj == null)
            {
                defObj = mTestSequence.GetGlobalValueIfExists(objectName);
                if (defObj == null) throw new ArgumentException("Can't find object '" + objectName + "'");
                isGlobal = true;
            }

            string objectType = defObj.GetType().ToString();
            string propertyType = defObj.GetType().GetProperty(propertyName).PropertyType.ToString();

            if (isGlobal)
            {
                // colorDef = (GlobalValue)seq.GetGlobalValue("xyz");
                ctorBody += "\t\t" + objectNameForCode + " = (" + objectType + ")seq.GetGlobalValue(\"" + objectName + "\");" + Environment.NewLine;
            }
            else
            {
                // colorDef = (ColorMatchDefinition)seq.GetDefinitionObject("Light Paint");
                ctorBody += "\t\t" + objectNameForCode + " = (" + objectType + ")seq.ObjectRegistry.GetObject(\"" + objectName + "\");" + Environment.NewLine;
            }

            // ColorMatchDefinition colorDef;
            classBody += "\t" + objectType + " " + objectNameForCode + ";" + Environment.NewLine;

            // property attributes
            // [CategoryAttribute("Parameters"),DescriptionAttribute("Definition of the color to search for")]
            classBody += "\t[CategoryAttribute(\"" + category + "\"),DescriptionAttribute(\"" + description.Replace("\"","\\\"") + "\")]" + Environment.NewLine;

            // create the property
            /*
                public string LightPaintDef
                {
                    get { return colorDef.Rules; }
                    set { colorDef.Rules = value; }
                }
             */
            classBody += "\tpublic " + propertyType + " " + favName + Environment.NewLine;
            classBody += "\t{" + Environment.NewLine;
            classBody += "\t\tget { return " + objectNameForCode + "." + propertyName + "; }" + Environment.NewLine;
            classBody += "\t\tset { " + objectNameForCode + "." + propertyName + " = value; }" + Environment.NewLine;
            classBody += "\t}" + Environment.NewLine;
        }

        public void GenerateCode()
        {
            // open the file for writing
            string fileName = "FavSettings";
            Stream s = File.Open(fileName + ".cs", FileMode.Create);
            StreamWriter wrtr = new StreamWriter(s);
            wrtr.WriteLine("// Dynamically created FavSettings class");
            wrtr.WriteLine("using System.ComponentModel;");
            wrtr.WriteLine("using NetCams;");

            // create the class
            string className = mSequenceNamed_sanitized+"_FavoriteSettings";
            wrtr.WriteLine("class {0} : NetCams.FavoriteSettings", className);
            wrtr.WriteLine("{");

            // create the method
            wrtr.WriteLine("\tpublic {0}(TestSequence seq)", className);
            wrtr.WriteLine("\t\t: base(seq)");
            wrtr.WriteLine("\t{");
            wrtr.Write(ctorBody);
            wrtr.WriteLine("\t}");
            wrtr.WriteLine("\t");
            //wrtr.WriteLine("\tpublic TestSequence mTestSequence;");
            //wrtr.WriteLine("\t");

            wrtr.Write(classBody);

            wrtr.WriteLine("}");    // end class

            // close the writer and the stream
            wrtr.Close();
            s.Close();

            /*
            // Build the file
            ProcessStartInfo psi = new ProcessStartInfo();

            psi.FileName = "cmd.exe";

            string compileString = "/c C:\\WINDOWS\\Microsoft.NET\\Framework\\v2.0.50727\\csc /optimize+ ";
            compileString += "/r:\"UVision.exe\" ";
            compileString += "/target:library ";
            compileString += "{0}.cs > compile.out";

            psi.Arguments = String.Format(compileString, fileName);

            psi.WindowStyle = ProcessWindowStyle.Minimized;

            Process proc = Process.Start(psi);

            proc.WaitForExit();   // wait at most 2 seconds

            // Open the file, and get a pointer to the method info
            Assembly a = Assembly.LoadFrom(fileName + ".dll");
            mTestSequence.mFavoriteSettings = (FavoriteSettings)a.CreateInstance(className);
             */

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
//            cp.ReferencedAssemblies.Add("System.ComponentModel.dll");// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.



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
                object[] args = { mTestSequence };
                mTestSequence.mFavoriteSettings = (FavoriteSettings)a.CreateInstance(className, false, BindingFlags.Default | BindingFlags.CreateInstance, null, args, null, null);
            }

            //File.Delete(fileName + ".cs"); // clean up
        }
    }
}

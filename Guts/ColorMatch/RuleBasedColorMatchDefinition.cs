// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace NetCams
{
    public class RuleBasedColorMatchDefinition : ColorMatchDefinition
    {
        public RuleBasedColorMatchDefinition(TestSequence testSequence)
            : base(testSequence)
        {
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new RuleBasedColorMatchInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            // TODO: build a list of dependencies once we use rules that dependend on generated values
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                // TODO: once we have dependencies on generated values, then we need to implement this
                return result + 1;
            }
        }

        private string mRulesDef = "";
        public string Rules
        {
            get { return mRulesDef; }
            set 
            {
                processRuleDef(value);
            }
        }

        public override string ToString()
        {
            return Name;// +": " + mRulesDef;
        }

        private bool processRuleDef(string ruleDef)
        {
            string[] rules = ruleDef.Split(new char[] { ',' });
    
            // new array of new rules.  each time the ruledef changes we create a whole new array of new rules.  ColorMatchInstances point to the version of the array they are using
            ArrayList newRules = new ArrayList(rules.GetLength(0));

            foreach (string rule in rules)
            {
                string[] ruleComponents = rule.Trim().Split(new char[] { ' ' });
                // TODO: if rule contains "OR" and has odd number of components...ACTUALLY it's not clean to do it as a rule (currently) since we would have the user specify MatcherDefs to reference, but we need MatcherInstances.  So it's much cleaner to implement NOT & OR as separate ColorMatchDef/Instance objects
                switch (ruleComponents.GetLength(0))
                {
                    /*
                case 2:
                    if (ruleComponents[0].Trim().ToUpper() == "NOT") ACTUALLY it's not clean to do it as a rule (currently) since we would have the user specify MatcherDefs to reference, but we need MatcherInstances.  So it's much cleaner to implement NOT & OR as separate ColorMatchDef/Instance objects
                    {
                        ColorMatchDefinition theReferencedColorDef = Sequence().GetColorMatchDefinition(ruleComponents[1].Trim());
                        newRules.Add(new NotColorMatchRule(theReferencedColorDef));
                    }
                    else
                    {
                        throw new ArgumentException("Can not process color match rule '" + rule + "'");
                    }
                    break;
                     */
                    case 3: // rule def has 3 components
                        int theValue = int.Parse(ruleComponents[2]);
                        string theOperator = ruleComponents[1].Trim();
                        string colorComponent = GetColorComponent(rule, ruleComponents[0]);

                        // build code
                        string comparisonCode = colorComponent + " " + theOperator + " " + theValue;
                        string userDefinition = ruleComponents[0].Trim() + " " + theOperator + " " + theValue;

                        newRules.Add(BuildRuleClass(rule, comparisonCode, userDefinition));

                        break;
                    case 5: // rule def has 5 components
                        string mathOpField = ruleComponents[1].Trim();
                        if (mathOpField == "-" || mathOpField == "+" || mathOpField == "*")
                        { // expect format of "component1 - component2 < val"
                            string colorComponent1 = GetColorComponent(rule, ruleComponents[0]);
                            string colorComponent2 = GetColorComponent(rule, ruleComponents[2]);
                            theOperator = ruleComponents[3].Trim();
                            theValue = int.Parse(ruleComponents[4]);

                            // build code
                            comparisonCode = colorComponent1 + " " + mathOpField + " " + colorComponent2 + " " + theOperator + " " + theValue;
                            userDefinition = ruleComponents[0].Trim() + " " + mathOpField + " " + ruleComponents[2].Trim() + " " + theOperator + " " + theValue;

                            newRules.Add(BuildRuleClass(rule, comparisonCode, userDefinition));
                        }
                        else // expect it to be of format "val1 < component < val2"
                        {
                            int value1 = int.Parse(ruleComponents[0]);
                            int value2 = int.Parse(ruleComponents[4]);
                            string operator1 = ruleComponents[1].Trim();
                            string operator2 = ruleComponents[3].Trim();
                            colorComponent = GetColorComponent(rule, ruleComponents[2]);

                            // build code
                            userDefinition = value1 + " " + operator1 + " " + ruleComponents[2].Trim() + " " + operator2 + " " + value2;
                            if ((operator1[0] == '<' && operator2[0] == '<' && value1 > value2) ||
                                (operator1[0] == '>' && operator2[0] == '>' && value1 < value2))
                            {
                                // example: "30 > H > 330" means if H < 30 OR H > 330 (outside the range)
                                comparisonCode = value1 + " " + operator1 + " " + colorComponent + " || " + colorComponent + " " + operator2 + " " + value2;
                            }
                            else
                            {
                                // example: "30 < H < 330" means if H > 30 AND H < 330 (within the range)
                                comparisonCode = value1 + " " + operator1 + " " + colorComponent + " && " + colorComponent + " " + operator2 + " " + value2;
                            }

                            newRules.Add(BuildRuleClass(rule, comparisonCode, userDefinition));
                        }
                        break;

                    default: // rule def has unsupported number of components
                        throw new ArgumentException("Rule '" + rule + "' is not in a valid format.");
                }

            } // foreach rule

            HandlePropertyChange(this, "Rules", mRulesDef, ruleDef);
            // update the rules to the new rules
            mRules = newRules;
            mRulesDef = ruleDef;

            return true;
        }

        private ColorMatchRule BuildRuleClass(string rule, string comparisonCode, string userDefinition)
        {
            // open the file for writing
            string fileName = "ColorMatchRule";
            Stream s = File.Open(fileName + ".cs", FileMode.Create);
            StreamWriter wrtr = new StreamWriter(s);
            wrtr.WriteLine("// Dynamically created ColorMatchRule class");
            wrtr.WriteLine("using System.Drawing;");

            // create the class
            string className = "CustomColorMatchRuleClass";
            wrtr.WriteLine("class " + className + " : NetCams.ColorMatchRule");
            wrtr.WriteLine("{");

            // create the ctor
            wrtr.WriteLine("\tpublic " + className + "() { }");
            wrtr.WriteLine("\t");
            // create the match method
            wrtr.WriteLine("\tpublic override bool Matches(Color theColor)");
            wrtr.WriteLine("\t{");
            wrtr.WriteLine("\t\treturn " + comparisonCode + ";");
            wrtr.WriteLine("\t}");
            wrtr.WriteLine("\t");
            // create the ToString() method
            wrtr.WriteLine("\tpublic override string ToString()");
            wrtr.WriteLine("\t{");
            wrtr.WriteLine("\t\treturn \"" + userDefinition + "\";");
            wrtr.WriteLine("\t}");
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
            cp.ReferencedAssemblies.Add("system.drawing.dll");

            // Invoke compilation of the source file.
            string sourceName = fileName + ".cs";
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

            ColorMatchRule theNewRule;
            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}", sourceName, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
                throw new ArgumentException("Unable to process rule '" + rule + "'");
            }
            else
            {
                // Display a successful compilation message.
                Console.WriteLine("Source {0} built into {1} successfully.", sourceName, cr.PathToAssembly);

                Assembly a = cr.CompiledAssembly;
                object[] args = { };
                theNewRule = (ColorMatchRule)a.CreateInstance(className, false, BindingFlags.Default | BindingFlags.CreateInstance, null, args, null, null);
            }

            //File.Delete(fileName + ".cs"); // clean up        }
            return theNewRule;
        }

        private string GetColorComponent(string rule, string colorRuleComponent)
        {
            switch (colorRuleComponent.Trim().ToUpper())
            {
                case "R":
                    return "theColor.R";
                case "G":
                    return "theColor.G";
                case "B":
                    return "theColor.B";
                case "H":
                    return "theColor.GetHue()";
                case "S":
                    return "theColor.GetSaturation() * 100";
                case "I":
                    return "theColor.GetBrightness() * 100";
                case "GREY":
                    return "(0.3 * theColor.R + 0.59 * theColor.G + 0.11 * theColor.B)";
                default:
                    throw new ArgumentException("In rule '" + rule + "', '" + colorRuleComponent.Trim() + "' isn't a valid color component.");
            }
        }

        public ArrayList mRules = new ArrayList(); // TODO: change to real array for speed?
    }

    public abstract class ColorMatchRule
    {
        public abstract bool Matches(Color theColor);
    }

    /*
    public class NotColorMatchRule
    {
        public NotColorMatchRule(ColorMatchDefinition referencedColorDef) { mReferencedColorDef = referencedColorDef; }
        public bool Matches(Color theColor)
        {
            bool allRulesMatch = true;
            foreach (ColorMatchRule rule in mReferencedColorDef.mRules)
            {
                if (!rule.Matches(theColor)) allRulesMatch = false; // YUCK: we have to minic the implementation of ColorMatchInstance here ...otherwise we would have to look up the instance object each time...maybe change implementatin method to take the rules array as a parameter so it can be used by ColorMatchInstance and this method?
            }
            return !allRulesMatch;
        }
        private ColorMatchDefinition mReferencedColorDef;
    }
    */
    /*
                            switch (ruleComponents[0])
                        {
                            case "R":
                                switch (ruleComponents[1])
                                {
                                    case "<":
                                        newRules.Add(new R_LT_FV_ColorMatchRule(theValue));
                                        break;
                                    case "<=":
                                        newRules.Add(new R_LTE_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">":
                                        newRules.Add(new R_GT_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">=":
                                        newRules.Add(new R_GTE_FV_ColorMatchRule(theValue));
                                        break;
                                    default:
                                        // TODO: throw exception
                                        return false;
                                }
                                break;
                            case "G":
                                switch (ruleComponents[1])
                                {
                                    case "<":
                                        newRules.Add(new G_LT_FV_ColorMatchRule(theValue));
                                        break;
                                    case "<=":
                                        newRules.Add(new G_LTE_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">":
                                        newRules.Add(new G_GT_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">=":
                                        newRules.Add(new G_GTE_FV_ColorMatchRule(theValue));
                                        break;
                                    default:
                                        // TODO: throw exception
                                        return false;
                                }
                                break;
                            case "B":
                                switch (ruleComponents[1])
                                {
                                    case "<":
                                        newRules.Add(new B_LT_FV_ColorMatchRule(theValue));
                                        break;
                                    case "<=":
                                        newRules.Add(new B_LTE_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">":
                                        newRules.Add(new B_GT_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">=":
                                        newRules.Add(new B_GTE_FV_ColorMatchRule(theValue));
                                        break;
                                    default:
                                        // TODO: throw exception
                                        return false;
                                }
                                break;
                            case "H":
                                switch (ruleComponents[1])
                                {
                                    case "<":
                                        newRules.Add(new H_LT_FV_ColorMatchRule(theValue));
                                        break;
                                    case "<=":
                                        newRules.Add(new H_LTE_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">":
                                        newRules.Add(new H_GT_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">=":
                                        newRules.Add(new H_GTE_FV_ColorMatchRule(theValue));
                                        break;
                                    default:
                                        // TODO: throw exception
                                        return false;
                                }
                                break;
                            case "S":
                                switch (ruleComponents[1])
                                {
                                    case "<":
                                        newRules.Add(new S_LT_FV_ColorMatchRule(theValue));
                                        break;
                                    case "<=":
                                        newRules.Add(new S_LTE_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">":
                                        newRules.Add(new S_GT_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">=":
                                        newRules.Add(new S_GTE_FV_ColorMatchRule(theValue));
                                        break;
                                    default:
                                        // TODO: throw exception
                                        return false;
                                }
                                break;
                            case "I":
                                switch (ruleComponents[1])
                                {
                                    case "<":
                                        newRules.Add(new I_LT_FV_ColorMatchRule(theValue));
                                        break;
                                    case "<=":
                                        newRules.Add(new I_LTE_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">":
                                        newRules.Add(new I_GT_FV_ColorMatchRule(theValue));
                                        break;
                                    case ">=":
                                        newRules.Add(new I_GTE_FV_ColorMatchRule(theValue));
                                        break;
                                    default:
                                        // TODO: throw exception
                                        return false;
                                }
                                break;
                            default:
                                return false;
                        }

    public abstract class FixedValueColorMatchRule : ColorMatchRule
    {
        protected long mValue;
        public FixedValueColorMatchRule(long value)
        { mValue = value; }
    }

    public class R_LT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public R_LT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.R < mValue;
        }
        public override string ToString()
        {
            return "R < " + mValue;
        }
    }

    public class R_LTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public R_LTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.R <= mValue;
        }
        public override string ToString()
        {
            return "R <= " + mValue;
        }
    }

    public class R_GT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public R_GT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.R > mValue;
        }
        public override string ToString()
        {
            return "R > " + mValue;
        }
    }

    public class R_GTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public R_GTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.R >= mValue;
        }
        public override string ToString()
        {
            return "R >= " + mValue;
        }
    }

    public class G_LT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public G_LT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.G < mValue;
        }
        public override string ToString()
        {
            return "G < " + mValue;
        }
    }

    public class G_LTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public G_LTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.G <= mValue;
        }
        public override string ToString()
        {
            return "G <= " + mValue;
        }
    }

    public class G_GT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public G_GT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.G > mValue;
        }
        public override string ToString()
        {
            return "G > " + mValue;
        }
    }

    public class G_GTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public G_GTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.G >= mValue;
        }
        public override string ToString()
        {
            return "G >= " + mValue;
        }
    }

    public class B_LT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public B_LT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.B < mValue;
        }
        public override string ToString()
        {
            return "B < " + mValue;
        }
    }

    public class B_LTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public B_LTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.B <= mValue;
        }
        public override string ToString()
        {
            return "B <= " + mValue;
        }
    }

    public class B_GT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public B_GT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.B > mValue;
        }
        public override string ToString()
        {
            return "B > " + mValue;
        }
    }

    public class B_GTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public B_GTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.B >= mValue;
        }
        public override string ToString()
        {
            return "B >= " + mValue;
        }
    }

    public class H_LT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public H_LT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetHue() < mValue;
        }
        public override string ToString()
        {
            return "H < " + mValue;
        }
    }

    public class H_LTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public H_LTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetHue() <= mValue;
        }
        public override string ToString()
        {
            return "H <= " + mValue;
        }
    }

    public class H_GT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public H_GT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetHue() > mValue;
        }
        public override string ToString()
        {
            return "H > " + mValue;
        }
    }

    public class H_GTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public H_GTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetHue() >= mValue;
        }
        public override string ToString()
        {
            return "H >= " + mValue;
        }
    }

    public class S_LT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public S_LT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetSaturation() * 100 < mValue;
        }
        public override string ToString()
        {
            return "S < " + mValue;
        }
    }

    public class S_LTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public S_LTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetSaturation() * 100 <= mValue;
        }
        public override string ToString()
        {
            return "S <= " + mValue;
        }
    }

    public class S_GT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public S_GT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetSaturation() * 100 > mValue;
        }
        public override string ToString()
        {
            return "S > " + mValue;
        }
    }

    public class S_GTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public S_GTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetSaturation() * 100 >= mValue;
        }
        public override string ToString()
        {
            return "S >= " + mValue;
        }
    }

    public class I_LT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public I_LT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetBrightness() * 100 < mValue;
        }
        public override string ToString()
        {
            return "I < " + mValue;
        }
    }

    public class I_LTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public I_LTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetBrightness() * 100 <= mValue;
        }
        public override string ToString()
        {
            return "I <= " + mValue;
        }
    }

    public class I_GT_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public I_GT_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetBrightness() * 100 > mValue;
        }
        public override string ToString()
        {
            return "I > " + mValue;
        }
    }

    public class I_GTE_FV_ColorMatchRule : FixedValueColorMatchRule
    {
        public I_GTE_FV_ColorMatchRule(long value) : base(value) { }
        public override bool Matches(Color theColor)
        {
            return theColor.GetBrightness() * 100 >= mValue;
        }
        public override string ToString()
        {
            return "I >= " + mValue;
        }
    }
     */
}

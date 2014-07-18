// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace NetCams
{
    public class CalculationToolDefinition : NetCams.ToolDefinition
    {
        public const string NAME_PREFIX = "Calculation of ";

        public CalculationToolDefinition(TestSequence testSequence, CalculatedValueDefinition theCalculatedValue)
            : base(testSequence)
        {
            mResult = theCalculatedValue;

            testSequence.CalculationToolRegistry.RegisterObject(this); // TODO: needed?  for garbage collection? (looks like no)
        }

        public override void Dispose_UVision()
        {
            TestSequence().CalculationToolRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        public override bool IncludeObjectInConfigFile() { return false; }
        public override bool IncludeObjectInProgrammingTable() { return false; }

        public override void CreateInstance(TestExecution theExecution)
        {
            new CalculationToolInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mRootOperation != null && mRootOperation.Result == theOtherObject) return true; // this should be unnecessary if all objects return true if theOtherObject == this
            if (mRootOperation != null && mRootOperation.Result == null)
            {
                throw new ArgumentException("why are we here?  afddsfdsg");
            }
            return (mRootOperation != null && mRootOperation.Result.IsDependentOn(theOtherObject)) || base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mRootOperation != null) return result = Math.Max(result, mRootOperation.Result.ToolMapRow);
                return result + 1;
            }
        }

        public override string Name
        {
            get
            {
                if (mResult == null) return NAME_PREFIX + "partially constructed calculation";
                return NAME_PREFIX + mResult.Name;
            }
        }

        private string mCalculationDef = "";
        public string Calculation
        {
            get { return mCalculationDef; }
            set
            {
                value = value.Trim();
                aliasCount = 0;
                aliasMap.Clear();
                try
                {
                    mRootOperation = processDef(value);
                    mCalculationDef = value;
                    TestSequence().GarbageCollectMathOperations();
                }
                catch (ArgumentException ae)
                {
                    throw ae;
                }
                catch (Exception e)
                {
                    Window().logMessage("Error setting/changing calculation. Message=" + e.Message + Environment.NewLine + e.StackTrace);
                }
                finally
                {
                    TestSequence().RebuildToolGrid();
                }
            }
        }

        private CalculatedValueDefinition mResult = null;
        [CategoryAttribute("Output")]
        public CalculatedValueDefinition Result
        {
            get { return mResult; }
        }

        private MathOperationDefinition mRootOperation;
        public MathOperationDefinition RootOperation { get { return mRootOperation; } }

        public override string ToString()
        {
            return Name + ": " + mCalculationDef;
        }

        private ArrayList mUsedOperations = new ArrayList();

        public Dictionary<string, CalculationAlias> aliasMap = new Dictionary<string, CalculationAlias>();
        public int aliasCount = 0;
        private MathOperationDefinition processDef(string calcDef)
        {
            MathOperationDefinition lastMathOpDef = null;

            // TODO: tell every MathOperation we create/use that we are referencing it.  Then when we stop referencing it, tell it so it can dispose itself if unused
            // TODO: sanitize every calculation def to optimize reuse
            // TODO: handle negative constants (e.g. "x * -2")

            // determine if mixed-operator expression; if so, determine all operators used so that we can sort by precedence
            List<MathOperationCreator> operators = new List<MathOperationCreator>();
            SumOperationCreator sumOpCreator = null;
            MultiplicationOperationCreator multOpCreator = null;
            RelationalOperationCreator relationalOpCreator = null;
            EqualityOperationCreator equalityOpCreator = null;
            LogicalAndOperationCreator logicalAndOpCreator = null;
            LogicalOrOperationCreator logicalOrOpCreator = null;

            // strip out user-provided parathesis and determine operators used
            // * user-provided paranthesis are processed recursively and replaced with aliases
            // * since subexpressions wrapped with user-provided paranthesis are removed (replaced with aliases), we will end up with only the outer-most expression...and the operators within it...from here we process the operators by precedence
            // * operators, functions, etc within the subexpressions are handled by a recursive call
            bool lastTokenWasPossibleFunctionName = false; // checked when we find a open paranthesis
            int x = 0;
            while (x < calcDef.Length)
            {
                if (calcDef[x] == '(')
                {
                    // TODO: check if this is a function (e.g. Sin, Tan, Max, Min, Abs, Iff) or just ()'s; is it preceeded by operator or name?

                    int startPos = x;
                    int endPos = -1;
                    x++; // move past '('
                    int nestedCount = 0;
                    while (x < calcDef.Length && endPos < 0) // find the matching closing paranthesis
                    {
                        if (calcDef[x] == '(') nestedCount++;
                        if (calcDef[x] == ')')
                        {
                            if (nestedCount == 0)
                            {
                                endPos = x;
                            }
                            else
                            {
                                nestedCount--;
                            }
                        }
                        x++;
                    }
                    if (endPos < 0)
                    {
                        throw new ArgumentException("Missing a closing paranthesis");
                    }
                    else
                    {
                        string partBeforeAlias = "";
                        string partAfterAlias = "";
                        string subExpression = calcDef.Substring(startPos + 1, endPos - startPos - 1).Trim();

                        int startOfFunctionName = 0;
                        string functionName = string.Empty;
                        FunctionDefinition functionDef = null;

                        if (lastTokenWasPossibleFunctionName)
                        {
                            // find token before '('
                            bool foundStartOfToken = false;
                            bool foundToken = false;
                            for (int y = startPos - 1; y > 0 && !foundStartOfToken; y--)
                            {
                                if (Char.IsWhiteSpace(calcDef[y]))
                                {
                                    if (!foundToken)
                                    {
                                        // do nothing...skip whitespace while we look for end of token (before open paran)
                                    }
                                    else
                                    {
                                        foundStartOfToken = true;
                                        startOfFunctionName = y + 1;
                                    }
                                }
                                else
                                {
                                    foundToken = true;
                                }
                            }
                            functionName = calcDef.Substring(startOfFunctionName, startPos - startOfFunctionName).Trim();
                            startPos = startOfFunctionName;
                        }

                        if (startPos > 0)
                        {
                            partBeforeAlias = calcDef.Substring(0, Math.Max(startPos - 1, 0)).Trim();
                            if (partBeforeAlias.Length > 0) partBeforeAlias += ' ';
                        }
                        if (endPos < calcDef.Length - 1)
                        {
                            partAfterAlias = calcDef.Substring(endPos + 1, calcDef.Length - endPos - 1).Trim();
                            if (partAfterAlias.Length > 0) partAfterAlias = ' ' + partAfterAlias;
                        }

                        if (functionName.Length > 0)
                        {
                            // if we're dealing with a Function call, split subexpression up by commas
                            // TODO: add support for nested Function calls by searching for comma's rather than using split (ie commas from nested function calls can't be treated at this level) (if we find an open paran, ignore commas until matching closing paran)
                            string[] functionParameterSubExpressions = subExpression.Split(new char[] { ',' });
                            List<DataValueDefinition> valueObjects = new List<DataValueDefinition>();
                            foreach (string parameter in functionParameterSubExpressions)
                            {
                                string parameterExpression = parameter.Trim();
                                DataValueDefinition dataValueDef = TestSequence().DataValueRegistry.GetObjectIfExists(parameterExpression);
                                if (dataValueDef == null)
                                {
                                    MathOperationDefinition nestedCalculationOp;
                                    try
                                    {
                                        nestedCalculationOp = processDef(parameterExpression);
                                    }
                                    catch (Exception e)
                                    {
                                        throw new ArgumentException("Unable to parse function call parameter '" + parameterExpression + "' for function call '" + functionName + "'; error = " + e.Message);
                                    }
                                    dataValueDef = nestedCalculationOp.Result;
                                }
                                valueObjects.Add(dataValueDef);
                            }
                            string functionCallsFingerPrint = functionName + "(" + subExpression + ")";
                            // lookup token as Function Call
                            switch (functionName.ToUpper()) // TODO: look up functions in table, then have a common creator method
                            {
                                case "MIN":
                                    functionDef = new MinMaxFunctionDefinition(TestSequence(), functionCallsFingerPrint, MinMaxFunctionDefinition.Function.Min, valueObjects);
                                    break;
                                case "MAX":
                                    functionDef = new MinMaxFunctionDefinition(TestSequence(), functionCallsFingerPrint, MinMaxFunctionDefinition.Function.Max, valueObjects);
                                    break;
                                case "ABS":
                                    functionDef = new AbsFunctionDefinition(TestSequence(), functionCallsFingerPrint, valueObjects);
                                    break;
                                default:
                                    break;
                            }

                            if (partBeforeAlias.Length > 0 || partAfterAlias.Length > 0)
                            {
                                string alias = "#" + (aliasCount++);
                                CalculationAlias aliasGuts = new CalculationAlias();
                                aliasGuts.expressionToLookupValue = functionCallsFingerPrint;
                                aliasGuts.expressionForExpansion = functionCallsFingerPrint;
                                aliasMap.Add(alias, aliasGuts);
                                calcDef = partBeforeAlias + alias + partAfterAlias;
                                x = partBeforeAlias.Length;
                            }
                            else
                            {
                                lastMathOpDef = functionDef;
                                return lastMathOpDef;
                            }
                        }
                        else // just user-provided paranthesis rather than a function call...
                        {
                            if (partBeforeAlias.Length > 0 || partAfterAlias.Length > 0)
                            {
                                string alias = "#" + (aliasCount++);
                                CalculationAlias aliasGuts = new CalculationAlias();
                                aliasGuts.expressionToLookupValue = subExpression;
                                aliasGuts.expressionForExpansion = "(" + subExpression + ")";
                                aliasMap.Add(alias, aliasGuts);
                                calcDef = partBeforeAlias + alias + partAfterAlias;
                                x = partBeforeAlias.Length; // added 5/3/08...we stripped off the first ()'s, now we should search the rest of the calc def...right???
                                processDef(subExpression);
                            }
                            else
                            {
                                calcDef = subExpression; // strip off ()'s
                                x = 0; // added 5/3/08...we stripped off the outer most ()'s, now we should search again...right???
                            }
                        }
                    }
                    lastTokenWasPossibleFunctionName = false;
                }
                else if (calcDef[x] == '+' || calcDef[x] == '-') // TODO: look for unary sign operator, eg "* -x", "* -42", "/ -42", etc (minus followed by no space?  preceded by another operator?...or start of calc)
                {
                    if (sumOpCreator == null)
                    {
                        sumOpCreator = SumOperationCreator.Singleton;
                        operators.Add(sumOpCreator);
                    }
                    lastTokenWasPossibleFunctionName = false;
                }
                else if (calcDef[x] == '*' || calcDef[x] == '/')
                {
                    if (multOpCreator == null)
                    {
                        multOpCreator = MultiplicationOperationCreator.Singleton;
                        operators.Add(multOpCreator);
                    }
                    lastTokenWasPossibleFunctionName = false;
                }
                // TODO: search for ^ and replace "value ^ value" with alias
                else if (calcDef[x] == '<' || calcDef[x] == '>')
                {
                    if (relationalOpCreator == null)
                    {
                        relationalOpCreator = RelationalOperationCreator.Singleton;
                        operators.Add(relationalOpCreator);
                    }
                    lastTokenWasPossibleFunctionName = false;
                }
                else if (calcDef[x] == '=' && x > 0 && (calcDef[x - 1] == '=' || calcDef[x - 1] == '!'))
                {
                    if (relationalOpCreator == null)
                    {
                        equalityOpCreator = EqualityOperationCreator.Singleton;
                        operators.Add(equalityOpCreator);
                    }
                    lastTokenWasPossibleFunctionName = false;
                }
                else if (calcDef[x] == '&' && x > 0 && calcDef[x - 1] == '&')
                {
                    if (logicalAndOpCreator == null)
                    {
                        logicalAndOpCreator = LogicalAndOperationCreator.Singleton;
                        operators.Add(logicalAndOpCreator);
                    }
                    lastTokenWasPossibleFunctionName = false;
                }
                else if (calcDef[x] == '|' && x > 0 && calcDef[x - 1] == '|')
                {
                    if (logicalOrOpCreator == null)
                    {
                        logicalOrOpCreator = LogicalOrOperationCreator.Singleton;
                        operators.Add(logicalOrOpCreator);
                    }
                    lastTokenWasPossibleFunctionName = false;
                }
                else if (Char.IsWhiteSpace(calcDef[x]))
                {
                    if (calcDef.Length > x + 4 && (calcDef[x + 1] == 'A' || calcDef[x + 1] == 'a') && (calcDef[x + 2] == 'N' || calcDef[x + 2] == 'n') && (calcDef[x + 3] == 'D' || calcDef[x + 3] == 'd') && Char.IsWhiteSpace(calcDef[x + 4]))
                    {
                        if (logicalAndOpCreator == null)
                        {
                            logicalAndOpCreator = LogicalAndOperationCreator.Singleton;
                            operators.Add(logicalAndOpCreator);
                        }
                        x += 4; // skip "AND "
                        lastTokenWasPossibleFunctionName = false;
                    }
                    else if (calcDef.Length > x + 3 && (calcDef[x + 1] == 'O' || calcDef[x + 1] == 'o') && (calcDef[x + 2] == 'R' || calcDef[x + 2] == 'r') && Char.IsWhiteSpace(calcDef[x + 3]))
                    {
                        if (logicalOrOpCreator == null)
                        {
                            logicalOrOpCreator = LogicalOrOperationCreator.Singleton;
                            operators.Add(logicalOrOpCreator);
                        }
                        x += 3; // skip "OR "
                        lastTokenWasPossibleFunctionName = false;
                    }
                }
                else
                {
                    lastTokenWasPossibleFunctionName = true;
                }

                x++;
            }


            // TODO: look for "* -x", "* -42", "/ -42", "+ -42", etc  NOTE: we don't assume everything is separated by whitespace, but operators must be...so look for 2 in a row

            string calcDef_upperCase = calcDef.ToUpper();

            // HANDLE PRECIDENCE
            // sort all operators that are used by precedence
            operators.Sort(MathOperationPrecedenceComparer.Singleton);

            // pull out (with alias) each highest-precidence component until all have been handled
            MathOperationDefinition mathOpDef;
            while (operators.Count > 0 && calcDef.Length > 0)
            {
                mathOpDef = operators[0].CreateMathOperation(this, ref calcDef, ref calcDef_upperCase);
                if (mathOpDef == null)
                {
                    operators.RemoveAt(0);
                }
                else
                {
                    lastMathOpDef = mathOpDef;
                }
            }
            if (lastMathOpDef == null)
            {
                throw new ArgumentException("Invalid calculation");
            }
            return lastMathOpDef;
        }

        /*
        private const int NONE = 0;
        private const int SUM = 1;
        private const int MULT = 2;
         */

        public string ExpandCalcDef(string calcDefWithAliases)
        {
            string expandedCalcDef;
            bool changed = false;
            do
            {
                int x = 0;
                expandedCalcDef = "";
                changed = false;
                while (x < calcDefWithAliases.Length)
                {
                    if (calcDefWithAliases[x] == '#')
                    {
                        string alias = "" + calcDefWithAliases[x]; // copy #
                        x++; // move past #
                        while (x < calcDefWithAliases.Length && Char.IsDigit(calcDefWithAliases[x]))
                        {
                            alias += calcDefWithAliases[x];
                            x++;
                        }
                        x--;
                        expandedCalcDef += aliasMap[alias].expressionForExpansion;
                        changed = true;
                    }
                    else
                    {
                        expandedCalcDef += calcDefWithAliases[x];
                    }
                    x++;
                }
                calcDefWithAliases = expandedCalcDef;
            } while (changed);
            return expandedCalcDef;
        }

        public DataValueDefinition GetValueForAlias(string alias)
        {
            string objectAsString = ExpandCalcDef(aliasMap[alias].expressionToLookupValue);
            return TestSequence().DataValueRegistry.GetObject(objectAsString);//20080506 (MathOpResultDefinition.COMPUTED_VALUES_NAME_PREFIX + objectAsString);
        }
        /*
        private BasicMathOperationDefinition processPureOp(string calcDef, int opType)
        {

            string expandedCalcDef = ExpandCalcDef(calcDef);

            BasicMathOperationDefinition operation = null;
            if (opType == SUM)
            {
                operation = new SumOperationDefinition(Sequence(), expandedCalcDef);
            }
            else if (opType == MULT)
            {
                operation = new MultiplicationOperationDefinition(Sequence(), expandedCalcDef);
            }
            else
            {
                throw new ArgumentException("alksjdjieoldjaijs");
            }

            return operation;
        }*/
        /*
        protected DataValueDefinition GetValue(ref string calcDef, ref int index)
        {
            while (index < calcDef.Length)
            {
                if (Char.IsWhiteSpace(calcDef[index]))
                {
                    // do nothing
                }
                else if (Char.IsDigit(calcDef[index]))
                {
                    string constantValueAsString = "";
                    while (index < calcDef.Length && (Char.IsDigit(calcDef[index]) || calcDef[index] == '.'))
                    {
                        constantValueAsString += calcDef[index];
                        index++;
                    }
                    index--;
                    objectToAdd = Sequence().GetValueObject(constantValueAsString);
                }
                else if (Char.IsLetter(calcDef[index]))
                {
                    string namedValueAsString = "";
                    while (index < calcDef.Length && (Char.IsLetterOrDigit(calcDef[index]) || calcDef[index] == '_'))
                    {
                        namedValueAsString += calcDef[index];
                        index++;
                    }
                    index--;
                    objectToAdd = Sequence().GetValueObject(namedValueAsString);
                }
                else if (calcDef[index] == '"')
                {
                    index++; // move past opening quotation mark
                    string namedValueAsString = "";
                    while (calcDef[index] != '"')
                    {
                        namedValueAsString += calcDef[index];
                        index++;
                        if (index >= calcDef.Length) throw new ArgumentException("Missing closing quotation mark; expression='" + calcDef + "'");
                    }
                    //                    index++; // move past closing quotation mark
                    objectToAdd = Sequence().GetValueObject(namedValueAsString);
                }
                else if (calcDef[index] == '#')
                {
                    string alias = "" + calcDef[index]; // copy #
                    index++; // move past #
                    while (index < calcDef.Length && Char.IsDigit(calcDef[index]))
                    {
                        alias += calcDef[index];
                        index++;
                    }
                    index--;
                    string objectAsString = ExpandCalcDef((string)aliasMap[alias]);
                    objectToAdd = Sequence().GetValueObject(MathOpResultDefinition.COMPUTED_VALUES_NAME_PREFIX + objectAsString);
                }
                else
                {
                    throw new ArgumentException("parsing error");
                }
                index++;
            }
        }*/
        /*
        protected String GetOperator(ref string calcDef, ref int index)
        {
        }
         */
    }

    public class CalculationAlias
    {
        public string expressionToLookupValue;
        public string expressionForExpansion;
    }
}
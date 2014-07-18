// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public interface MathOperationCreator
    {
        int Precedence();
        MathOperationDefinition CreateMathOperation(CalculationToolDefinition calcToolDef, ref string calcDef, ref string calcDef_upperCase);
    }

    public abstract class MathOperationCreator_ForCommon2PlusValueOperation : MathOperationCreator
    {
        protected MathOperationCreator_ForCommon2PlusValueOperation(string[] theSupportedOperators)
        {
            supportedOperators = theSupportedOperators;
            numOperators = theSupportedOperators.GetLength(0);
        }

        protected abstract MathOperationDefinition CreateMathOperation(TestSequence testSequence, string calcDef_expanded, List<DataValueDefinition> valueObjects, List<string> operatorsUsed);

        protected string[] supportedOperators;
        protected int numOperators;
        public abstract int Precedence();

        /// <summary>
        /// Takes the calcDef and extracts the first expression using the operator of the creator. This is called repeated until there are no more expressions of the operator.
        /// </summary>
        /// <param name="calcToolDef"></param>
        /// <param name="calcDef_upperCase"></param>
        /// <returns></returns>
        public MathOperationDefinition CreateMathOperation(CalculationToolDefinition calcToolDef, ref string calcDef, ref string calcDef_upperCase)
        {
            List<DataValueDefinition> valueObjects = new List<DataValueDefinition>(); // NOTE: this List will be owned by the MathOperationDefinition that we create
            List<string> operatorsUsed = new List<string>(); // NOTE: this List will be owned by the MathOperationDefinition that we create

            int index;
            int indexOfFirstOperator = -1;
            int indexAfterFirstOperator = -1;
            // find first matching operator
            for (index = 0; index < calcDef_upperCase.Length && indexOfFirstOperator < 0; index++)
            {
                for (int opNdx = 0; opNdx < numOperators; opNdx++)
                {
                    if (calcDef_upperCase[index] == supportedOperators[opNdx][0] ||
                        (supportedOperators[opNdx][0] == ' ' && Char.IsWhiteSpace(calcDef_upperCase[index])))
                    {
                        bool didNotFindConflict = true;
                        for (int y = 1; y < supportedOperators[opNdx].Length; y++)
                        {
                            if (index + y >= calcDef_upperCase.Length || // if we run out of calcDef, we have a conflict (non-match)
                                calcDef_upperCase[index + y] != supportedOperators[opNdx][y])
                            {
                                if( index + y < calcDef_upperCase.Length && 
                                    supportedOperators[opNdx][y] == ' ' && Char.IsWhiteSpace(calcDef_upperCase[index+y]))
                                {
                                    // special case where they don't have to match exactly
                                }
                                else
                                {
                                    didNotFindConflict = false;
                                    break;
                                }
                            }
                        }
                        if (didNotFindConflict)
                        {
                            indexOfFirstOperator = index;
                            indexAfterFirstOperator = index + supportedOperators[opNdx].Length;
                            operatorsUsed.Add(supportedOperators[opNdx]);
                            break;
                        }
                    }
                }
            }

            if (indexOfFirstOperator < 0)
            {
                return null; //operator not found...we must have extracted all instances in previous calls to this method
            }

            // grab value before operator
            index = indexOfFirstOperator - 1;
            while (index >= 0 && Char.IsWhiteSpace(calcDef_upperCase[index]))
            {
                index--;
            }
            int indexOfValueBeforeFirstOperator = -1;
            int indexAfterValueBeforeFirstOperatorIdentifier = index + 1;
            bool foundNonDigits = false;
            bool foundSomething = false;
            while (index >= 0)
            {
                if (Char.IsLetter(calcDef_upperCase[index]))
                {
                    foundSomething = true;
                    foundNonDigits = true;
                    index--;
                }
                else if (Char.IsDigit(calcDef_upperCase[index]))
                {
                    foundSomething = true;
                    index--;
                }
                else if (calcDef_upperCase[index] == '_')
                {
                    foundSomething = true;
                    foundNonDigits = true;
                    index--;
                }
                else if (calcDef_upperCase[index] == '#' && foundSomething && !foundNonDigits && (
         (index == 0) || (index > 0 && Char.IsWhiteSpace(calcDef_upperCase[index-1]))
               )
          )
                {
                    index--;
                }
                else
                {
                    break;
                }
            }
            if (!foundSomething)
            {
                throw new ArgumentException("Error parsing calculation. code=43987598");
            }
            indexOfValueBeforeFirstOperator = index + 1;
            string theValueIdentifier = calcDef.Substring(indexOfValueBeforeFirstOperator, indexAfterValueBeforeFirstOperatorIdentifier - indexOfValueBeforeFirstOperator);
            if (foundNonDigits)
            {
                if (!Char.IsLetter(theValueIdentifier[0]))
                {
                    throw new ArgumentException("Error parsing calculation: value identifier doesn't start with a letter: '" + theValueIdentifier + "'");
                }
            }

            if (!foundNonDigits && theValueIdentifier[0] == '#')
            {
                valueObjects.Add(calcToolDef.GetValueForAlias(theValueIdentifier));
            }
            else
            {
                valueObjects.Add(calcToolDef.TestSequence().DataValueRegistry.GetObject(theValueIdentifier));
            }

            // grab value after operator
            index = indexAfterFirstOperator;
            valueObjects.Add(GrabNextValue(calcToolDef, ref calcDef, ref index));

            // while next token is a matching operator, grab next value and check next token
            string theOperator;
            do
            {
                theOperator = GrabOperator(ref calcDef_upperCase, ref index);
                if (theOperator.Length > 0)
                {
                    operatorsUsed.Add(theOperator);
                    valueObjects.Add(GrabNextValue(calcToolDef, ref calcDef, ref index));
                }
            } while (theOperator != String.Empty);

            // remove substring for parsing and replace it with alias
            int lengthOfSubString = index < calcDef_upperCase.Length ? index - indexOfValueBeforeFirstOperator : calcDef_upperCase.Length - indexOfValueBeforeFirstOperator;
            string subExpression = calcDef.Substring(indexOfValueBeforeFirstOperator, lengthOfSubString).Trim();

            string calcDefBeforeAlias = "";
            string calcDefBeforeAlias_upper = "";
            if (indexOfValueBeforeFirstOperator > 0)
            {
                calcDefBeforeAlias = calcDef.Substring(0, indexOfValueBeforeFirstOperator).Trim();
                calcDefBeforeAlias_upper = calcDef_upperCase.Substring(0, indexOfValueBeforeFirstOperator).Trim();
                if (calcDefBeforeAlias.Length > 0)
                {
                    calcDefBeforeAlias += ' ';
                    calcDefBeforeAlias_upper += ' ';
                }
            }
            string calcDefAfterAlias = "";
            string calcDefAfterAlias_upper = "";
            if (index < calcDef_upperCase.Length)
            {
                calcDefAfterAlias = calcDef.Substring(index).Trim();
                calcDefAfterAlias_upper = calcDef_upperCase.Substring(index).Trim();
                if (calcDefAfterAlias.Length > 0)
                {
                    calcDefAfterAlias = ' ' + calcDefAfterAlias;
                    calcDefAfterAlias_upper = ' ' + calcDefAfterAlias_upper;
                }
            }
            if (calcDefBeforeAlias.Length > 0 || calcDefAfterAlias.Length > 0)
            {
                string alias = "#" + (calcToolDef.aliasCount++);
                CalculationAlias aliasGuts = new CalculationAlias();
                aliasGuts.expressionToLookupValue = subExpression;
                aliasGuts.expressionForExpansion = "(" + subExpression + ")";
                calcToolDef.aliasMap.Add(alias, aliasGuts);
                calcDef = calcDefBeforeAlias + alias + calcDefAfterAlias;
                calcDef_upperCase = calcDefBeforeAlias_upper + alias + calcDefAfterAlias_upper;
                if (calcDef.Length != calcDef_upperCase.Length) throw new ArgumentException("Error parsing calculation. code=293084");
            }
            else
            {
                // nothing left to parse
                calcDef = "";
                calcDef_upperCase = "";
            }

            // return calculation object
            string expandedCalcDef = calcToolDef.ExpandCalcDef(subExpression);
            return CreateMathOperation(calcToolDef.TestSequence(), expandedCalcDef, valueObjects, operatorsUsed);
        }


        protected DataValueDefinition GrabNextValue(CalculationToolDefinition calcToolDef, ref string calcDef, ref int index)
        {
            while (index < calcDef.Length && Char.IsWhiteSpace(calcDef[index]))
            {
                index++;
            }
            int indexOfValue = index;
            int indexAfterValue = -1;
            bool foundNonDigits = false;
            bool foundSomething = false;
            bool foundDecimal = false;
            while (index < calcDef.Length)
            {
                if (Char.IsLetter(calcDef[index]))
                {
                    foundSomething = true;
                    foundNonDigits = true;
                    index++;
                }
                else if (Char.IsDigit(calcDef[index]))
                {
                    foundSomething = true;
                    index++;
                }
                else if (!foundNonDigits && !foundDecimal && calcDef[index] == '.')
                {
                    foundSomething = true;
                    foundDecimal = true;
                    index++;
                }
                else if (calcDef[index] == '_')
                {
                    foundSomething = true;
                    foundNonDigits = true;
                    index++;
                }
                else if (calcDef[index] == '#' && !foundSomething && index + 1 < calcDef.Length && Char.IsDigit(calcDef[index + 1]))
                {
                    // start of an alias
                    foundSomething = true;
                    index++;
                }
                else
                {
                    break;
                }
            }
            if (!foundSomething)
            {
                throw new ArgumentException("Error parsing calculation. code=433447598");
            }
            indexAfterValue = index;
            string theValueIdentifier = calcDef.Substring(indexOfValue, indexAfterValue - indexOfValue);
            if (foundNonDigits)
            {
                if (!Char.IsLetter(calcDef[indexOfValue]))
                {
                    throw new ArgumentException("Error parsing calculation: value identifier doesn't start with a letter: '" + theValueIdentifier + "'");
                }
            }
            if (!foundNonDigits && theValueIdentifier[0] == '#')
            {
                return calcToolDef.GetValueForAlias(theValueIdentifier);
            }
            else
            {
                return calcToolDef.TestSequence().DataValueRegistry.GetObject(theValueIdentifier);
            }
        }
        protected string GrabOperator(ref string calcDef_upperCase, ref int index)
        {
            while (index < calcDef_upperCase.Length && Char.IsWhiteSpace(calcDef_upperCase[index]))
            {
                index++;
            }
            if (index >= calcDef_upperCase.Length) return String.Empty;
            int indexOfOperator = index;
            int indexAfterOperator = -1;
            for (int opNdx = 0; opNdx < numOperators; opNdx++)
            {
                if (calcDef_upperCase[index] == supportedOperators[opNdx][0])
                {
                    bool didNotFindConflict = true;
                    for (int y = 1; y < supportedOperators[opNdx].Length; y++)
                    {
                        if (index + y >= calcDef_upperCase.Length ||
                            calcDef_upperCase[index + y] != supportedOperators[opNdx][y])
                        {
                            if (index + y < calcDef_upperCase.Length &&
                                supportedOperators[opNdx][y] == ' ' && Char.IsWhiteSpace(calcDef_upperCase[index + y])
                                )
                            {
                                // special case where they don't have to match exactly
                            }
                            else
                            {
                                didNotFindConflict = false;
                                break;
                            }
                        }
                    }
                    if (didNotFindConflict)
                    {
                        indexOfOperator = index;
                        indexAfterOperator = index + supportedOperators[opNdx].Length;
                        index = indexAfterOperator;
                        return calcDef_upperCase.Substring(indexOfOperator, indexAfterOperator - indexOfOperator);
                    }
                }
            }
            return String.Empty;
        }
    }

    public class MathOperationPrecedenceComparer : System.Collections.Generic.IComparer<MathOperationCreator>
    {
        public MathOperationPrecedenceComparer()
        {
        }

        public static MathOperationPrecedenceComparer Singleton = new MathOperationPrecedenceComparer();

        public int Compare(MathOperationCreator left, MathOperationCreator right)
        {
            return left.Precedence() - right.Precedence();
        }
    }
}


// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class RuleBasedColorMatchInstance : ColorMatchInstance
    {
        public RuleBasedColorMatchInstance(RuleBasedColorMatchDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            mRules = theDefinition.mRules; // NOTE: just copying reference to the arraylist. each change to the definition builds a whole new array list
            mRulesDef = theDefinition.Rules;
        }

        public override bool IsComplete()
        {
            // TODO: when we add rules based on generated values, then we will need to implement this
            return true;
        }

        public override bool Matches(Color theColor)
        {
            foreach (ColorMatchRule rule in mRules)
            {
                if (!rule.Matches(theColor)) return false;
            }
            return true;
        }

        private string mRulesDef;
        public string Rules
        {
            get { return mRulesDef; }
        }

        private ArrayList mRules; // TODO: change to real array for speed?

        public override void DoWork() { }
    }
}

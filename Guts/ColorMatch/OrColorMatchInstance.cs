// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class OrColorMatchInstance : ColorMatchInstance
    {
        public OrColorMatchInstance(OrColorMatchDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            foreach (ColorMatchDefinition def in theDefinition.SubMatchers)
            {
                mReferencedColorMatchers.Add(testExecution.GetColorMatcher(def.Name));
            }
        }

        public override bool Matches(System.Drawing.Color theColor)
        {
            foreach (ColorMatchInstance subMatcher in mReferencedColorMatchers)
            {
                if (subMatcher.Matches(theColor)) return true;
            }
            return false;
        }

        public override void DoWork() { }

        public override bool IsComplete()
        {
            foreach (ColorMatchInstance subMatcher in mReferencedColorMatchers)
            {
                if (!subMatcher.IsComplete()) return false;
            }
            return true;
        }

        List<ColorMatchInstance> mReferencedColorMatchers = new List<ColorMatchInstance>();
    }
}

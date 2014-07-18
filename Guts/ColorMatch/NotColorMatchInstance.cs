// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NetCams
{
    class NotColorMatchInstance : ColorMatchInstance
    {
        public NotColorMatchInstance(NotColorMatchDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            mReferencedColorMatch = testExecution.GetColorMatcher(theDefinition.SubMatcher.Name);
        }

        public override bool IsComplete()
        {
            return mReferencedColorMatch.IsComplete();
        }

        public override bool Matches(Color theColor)
        {
            return !mReferencedColorMatch.Matches(theColor);
        }

        private ColorMatchInstance mReferencedColorMatch;
        public ColorMatchInstance ReferencedColorMatch
        {
            get { return mReferencedColorMatch; }
        }

        public override void DoWork() { }
    }
}

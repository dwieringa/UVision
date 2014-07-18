// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class NotColorMatchDefinition : ColorMatchDefinition
    {
        public NotColorMatchDefinition(TestSequence testSequence)
            : base(testSequence)
        {

        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new NotColorMatchInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            bool result = false;
            if (theOtherObject == this) return true;
            if (mReferencedColorMatcher != null && mReferencedColorMatcher.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mReferencedColorMatcher != null) result = Math.Max(result, mReferencedColorMatcher.ToolMapRow);
                return result + 1;
            }
        }

        public ColorMatchDefinition SubMatcher
        {
            get { return mReferencedColorMatcher; }
            set 
            {
                HandlePropertyChange(this, "SubMatcher", mReferencedColorMatcher, value);
                mReferencedColorMatcher = value;
            }
        }

        private ColorMatchDefinition mReferencedColorMatcher;
    }
}

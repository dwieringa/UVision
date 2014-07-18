// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class OrColorMatchDefinition : ColorMatchDefinition
    {
        public OrColorMatchDefinition(TestSequence testSequence)
            : base(testSequence)
        {

        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new OrColorMatchInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            foreach (ColorMatchDefinition def in mReferencedColorMatchers)
            {
                if (theOtherObject == def) return true;
            }
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                foreach (ColorMatchDefinition def in mReferencedColorMatchers)
                {
                    result = Math.Max(result, def.ToolMapRow);
                }
                return result + 1;
            }
        }

        public List<ColorMatchDefinition> SubMatchers
        {
            get { return mReferencedColorMatchers; }
            set 
            {
                HandlePropertyChange(this, "SubMatchers", mReferencedColorMatchers, value);
                mReferencedColorMatchers = value;
            }
        }

        private List<ColorMatchDefinition> mReferencedColorMatchers = new List<ColorMatchDefinition>();
    }
}

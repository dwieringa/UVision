// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class ValueBasedLineDecorationDefinition : LineDecorationDefinition
    {
        public ValueBasedLineDecorationDefinition(TestSequence testSequence, OwnerLink ownerLink)
            : base(testSequence)
        {
            SetOwnerLink(ownerLink);
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new ValueBasedLineDecorationInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                return result + 1;
            }
        }
    }
}

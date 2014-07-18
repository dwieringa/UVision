// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Text;


namespace NetCams
{
    public class ToolLineDecorationDefinition : ObjectBasedLineDecorationDefinition
    {
        public ToolLineDecorationDefinition(TestSequence testSequence, OwnerLink ownerLink)
            : base(testSequence)
		{
            SetOwnerLink(ownerLink);
        }

        public void SetStartX(DataValueDefinition value)
        {
            mStartX = value;
        }
        public void SetStartY(DataValueDefinition value)
        {
            mStartY = value;
        }
        public void SetEndX(DataValueDefinition value)
        {
            mEndX = value;
        }
        public void SetEndY(DataValueDefinition value)
        {
            mEndY = value;
        }
    }
}

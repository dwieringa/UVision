// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    class ToolPointDecorationDefinition : PointDecorationDefinition
    {
        public ToolPointDecorationDefinition(TestSequence testSequence, OwnerLink ownerLink)
            : base(testSequence)
		{
            SetOwnerLink(ownerLink);
        }

        public void SetX(DataValueDefinition value)
        {
            mX = value;
        }
        public void SetY(DataValueDefinition value)
        {
            mY = value;
        }
    }
}

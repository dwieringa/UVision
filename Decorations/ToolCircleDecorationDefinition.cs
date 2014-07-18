// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Text;


namespace NetCams
{
    public class ToolCircleDecorationDefinition : CircleDecorationDefinition
    {
        public ToolCircleDecorationDefinition(TestSequence testSequence, OwnerLink ownerLink)
            : base(testSequence)
		{
            SetOwnerLink(ownerLink);
        }

        public void SetCenterX(DataValueDefinition value)
        {
            mCenterX = value;
        }
        public void SetCenterY(DataValueDefinition value)
        {
            mCenterY = value;
        }
        public void SetRadius(DataValueDefinition value)
        {
            mRadius = value;
        }
    }
}

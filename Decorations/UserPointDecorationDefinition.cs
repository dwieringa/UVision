// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    class UserPointDecorationDefinition : PointDecorationDefinition
    {
        public UserPointDecorationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition X
        {
            get { return mX; }
            set 
            {
                HandlePropertyChange(this, "X", mX, value);
                mX = value;
            }
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition Y
        {
            get { return mY; }
            set 
            {
                HandlePropertyChange(this, "Y", mY, value);
                mY = value;
            }
        }

        [CategoryAttribute("Definition")]
        public Color Color
        {
            get { return mColor; }
            set
            {
                HandlePropertyChange(this, "Color", mColor, value);
                mColor = value;
            }
        }
    }
}

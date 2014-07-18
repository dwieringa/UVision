// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class UserCircleDecorationDefinition : CircleDecorationDefinition
    {
        public UserCircleDecorationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition CenterX
        {
            get { return mCenterX; }
            set 
            {
                HandlePropertyChange(this, "CenterX", mCenterX, value);
                mCenterX = value; 
            }
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition CenterY
        {
            get { return mCenterY; }
            set 
            {
                HandlePropertyChange(this, "CenterY", mCenterY, value);
                mCenterY = value;
            }
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition Radius
        {
            get { return mRadius; }
            set 
            {
                HandlePropertyChange(this, "Radius", mRadius, value);
                mRadius = value;
            }
        }

        private Color mColor;
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

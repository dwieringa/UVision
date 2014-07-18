// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class UserLineDecorationDefinition : ObjectBasedLineDecorationDefinition
    {
        public UserLineDecorationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition StartX
        {
            get { return mStartX; }
            set 
            {
                HandlePropertyChange(this, "StartX", mStartX, value);
                mStartX = value;
            }
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition StartY
        {
            get { return mStartY; }
            set 
            {
                HandlePropertyChange(this, "StartY", mStartY, value);
                mStartY = value;
            }
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition EndX
        {
            get { return mEndX; }
            set 
            {
                HandlePropertyChange(this, "EndX", mEndX, value);
                mEndX = value;
            }
        }

        [CategoryAttribute("Definition")]
        public DataValueDefinition EndY
        {
            get { return mEndY; }
            set 
            {
                HandlePropertyChange(this, "EndY", mEndY, value);
                mEndY = value;
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

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public abstract class ColorMatchInstance : NetCams.ToolInstance
    {
        public ColorMatchInstance(ColorMatchDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
        }

        public abstract bool Matches(Color theColor);
    }


}

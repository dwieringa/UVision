// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public abstract class DecorationInstance : ObjectInstance, IDecorationInstance
    {
        public DecorationInstance(DecorationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            testExecution.DecorationRegistry.RegisterObject(this);
        }

        public abstract void Draw(Graphics g, PictureBoxScale scale);
    }
}

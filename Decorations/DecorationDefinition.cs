// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Text;

namespace NetCams
{
    public abstract class DecorationDefinition : ObjectDefinition, IDecorationDefinition
    {
        public DecorationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            testSequence.DecorationRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().DecorationRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }
    }
}

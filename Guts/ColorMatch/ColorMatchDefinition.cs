// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NetCams
{
    [TypeConverter(typeof(ColorMatchDefinitionConverter))]
    public abstract class ColorMatchDefinition : NetCams.ToolDefinition
    {
        public ColorMatchDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            testSequence.ColorMatchRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().ColorMatchRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }
    }

    public class ColorMatchDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.ColorMatchRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.ColorMatchRegistry.GetObject(theObjectName);
        }
    }
}

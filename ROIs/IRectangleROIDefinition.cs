// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;

namespace NetCams
{
    [TypeConverter(typeof(IRectangleROIDefinitionConverter))]
    public interface IRectangleROIDefinition : IROIDefinition
    {
        System.Drawing.Color Color { get; set; }
    }

    public class IRectangleROIDefinitionConverter  : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.RectangleROIRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.RectangleROIRegistry.GetObject(theObjectName);
        }
    }
}

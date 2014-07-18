// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace NetCams
{
    [TypeConverter(typeof(ReferencePointDefinitionConverter))]
    public interface IReferencePointDefinition : IObjectDefinition
    {
    }

    public class ReferencePointDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.ReferencePointRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.ReferencePointRegistry.GetObject(theObjectName);
        }
    }
}

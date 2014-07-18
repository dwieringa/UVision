// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NetCams
{
    [TypeConverter(typeof(VideoDefinitionConverter))]
    public abstract class VideoDefinition : DataDefinition
    {
        public VideoDefinition(TestSequence testSequence)
            : base(testSequence)
		{
			testSequence.VideoRegistry.RegisterObject(this);
		}

        public override void Dispose_UVision()
        {
            TestSequence().VideoRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }
    }

    public class VideoDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.VideoRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.VideoRegistry.GetObject(theObjectName);
        }
    }
}

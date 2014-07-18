// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace NetCams
{
    [TypeConverter(typeof(ImageScorerDefinitionConverter))]
    public abstract class ImageScorerDefinition : NetCams.ToolDefinition
    {
        public ImageScorerDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            testSequence.ImageScorerRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().ImageScorerRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        protected ScoreFilterDefinition mScoreFilter;

        public virtual void SetScoreFilter(ScoreFilterDefinition theFilter)
        {
            mScoreFilter = theFilter;
        }
    }

    public class ImageScorerDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.ImageScorerRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.ImageScorerRegistry.GetObject(theObjectName);
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NetCams
{
    [TypeConverter(typeof(ScoreFilterDefinitionConverter))]
    public abstract class ScoreFilterDefinition : ToolDefinition
    {
        public ScoreFilterDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            testSequence.ScoreFilterRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().ScoreFilterRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        private ImageScorerDefinition mImageScorer; // TODO: move link from here (ScoreFilter) to the scorer so that we can have multiple scorers dumping numbers into the same sorter...or name this a list, but I have a feeling we will run into Class heirarchy problems this way...may want a tool to be an ImageScorer AND something else; can't move it now since filter is dependent on scorer so the scorer is created first and then can't find filter (split ctor & property setting)
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The analysis tool to filter the scores of.")]
        public ImageScorerDefinition ImageScorer
        {
            get { return mImageScorer; }
            set
            {
                if (value != mImageScorer)
                {
                    HandlePropertyChange(this, "ImageScorer", mImageScorer, value);
                    if (mImageScorer != null)
                    {
                        mImageScorer.SetScoreFilter(null);
                    }
                    mImageScorer = value;
                    if (mImageScorer != null)
                    {
                        mImageScorer.SetScoreFilter(this);
                    }
                }
            }
        }


        /*
        private DataValueDefinition mImageWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The width in pixels of the image(s) to be analyzed.")]
        public DataValueDefinition ImageWidth
        {
            get { return mImageWidth; }
            set { mImageWidth = value; }
        }

        private DataValueDefinition mImageHeight;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The height in pixels of the image(s) to be analyzed.")]
        public DataValueDefinition ImageHeight
        {
            get { return mImageHeight; }
            set { mImageHeight = value; }
        }
        */
    }

    public class ScoreFilterDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.ScoreFilterRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.ScoreFilterRegistry.GetObject(theObjectName);
        }
    }
}

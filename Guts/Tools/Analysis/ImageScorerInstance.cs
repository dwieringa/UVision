// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public abstract class ImageScorerInstance : ToolInstance
    {
        public ImageScorerInstance(ImageScorerDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            testExecution.ImageScorerRegistry.RegisterObject(this);
        }
        protected ScoreFilterInstance mScoreFilter;

        public virtual void SetScoreFilter(ScoreFilterInstance theFilter)
        {
            if (theFilter == null)
            {
                throw new ArgumentException("Attempting to set the Score Filter to null. Not allowed.");
            }
            mScoreFilter = theFilter;
        }
    }
}

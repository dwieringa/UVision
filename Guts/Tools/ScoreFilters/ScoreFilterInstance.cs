// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public abstract class ScoreFilterInstance : ToolInstance
    {
        public ScoreFilterInstance(ScoreFilterDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            /*
            if (theDefinition.ImageHeight == null) throw new ArgumentException("Score Filter tool '" + theDefinition.Name + "' doesn't have a value assigned to ImageHeight");
            mImageHeight = testExecution.GetValue(theDefinition.ImageHeight.Name);

            if (theDefinition.ImageWidth == null) throw new ArgumentException("Score Filter tool '" + theDefinition.Name + "' doesn't have a value assigned to ImageWidth");
            mImageWidth = testExecution.GetValue(theDefinition.ImageWidth.Name);
            */
            if (theDefinition.ImageScorer == null) throw new ArgumentException("Score Filter tool '" + theDefinition.Name + "' doesn't have a value assigned to ImageScorer");
            mImageScorer = testExecution.ImageScorerRegistry.GetObject(theDefinition.ImageScorer.Name);

            testExecution.ScoreFilterRegistry.RegisterObject(this);

            mImageScorer.SetScoreFilter(this);
        }

        
        public abstract void SetImageSize(int width, int height);

        public abstract void ProcessScore(int x, int y, long score);
        public abstract void MarkImage(Bitmap imageToMark, Color markColor);

        public abstract long Score
        {
            get;
        }

        private ImageScorerInstance mImageScorer; // TODO: move link from here (ScoreFilter) to the scorer so that we can have multiple scorers dumping numbers into the same sorter...or name this a list, but I have a feeling we will run into Class heirarchy problems this way...may want a tool to be an ImageScorer AND something else; can't move it now since filter is dependent on scorer so the scorer is created first and then can't find filter (split ctor & property setting)
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The analysis tool to filter the scores of.")]
        public ImageScorerInstance ImageScorer
        {
            get { return mImageScorer; }
        }

        protected int mImageWidth = -1;
        protected int mImageHeight = -1;

        /*
        private DataValueInstance mImageWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The width in pixels of the image(s) to be analyzed.")]
        public DataValueInstance GridCellWidth
        {
            get { return mImageWidth; }
        }

        private DataValueInstance mImageHeight;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The height in pixels of the image(s) to be analyzed.")]
        public DataValueInstance ImageHeight
        {
            get { return mImageHeight; }
        }
        */
    }
}

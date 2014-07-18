// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public abstract class VideoInstance : DataInstance
    {
        public VideoInstance(VideoDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            testExecution.VideoRegistry.RegisterObject(this);
        }

        public abstract Video Video
        {
            get;
        }

        // TODO: add video attributes like NumberOfFrames? FramesPerSecond? GetFrame(x)?

        public abstract void AddFrame(Bitmap theImage);

        private bool mIsComplete = false;
        public void SetIsComplete()
        {
            SetCompletedTime();
            mIsComplete = true;
        }
        public override bool IsComplete() { return mIsComplete; }
    }
}

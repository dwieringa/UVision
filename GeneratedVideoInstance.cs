// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class GeneratedVideoInstance : VideoInstance
    {
        public GeneratedVideoInstance(GeneratedVideoDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mVideo = new Video();
		}
        
        private Video mVideo;
		public override Video Video
		{
            get { return mVideo; }
		}

		public override void AddFrame(Bitmap theImage)
		{
            mVideo.Frames.Add(theImage);
		}
    }
}

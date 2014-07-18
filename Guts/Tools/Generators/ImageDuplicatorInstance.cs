// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class ImageDuplicatorInstance : NetCams.ImageGeneratorInstance
	{
		public ImageDuplicatorInstance(ImageDuplicatorDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.SourceImage == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to SourceImage");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            mEnabled = theDefinition.Enabled;

            mDuplicateImage = new GeneratedImageInstance(theDefinition.ResultantImage, testExecution);
		}


        private bool mEnabled = true;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public bool Enabled
        {
            get { return mEnabled; }
        }

		private ImageInstance mSourceImage;
		public ImageInstance SourceImage
		{
			get
			{
				return mSourceImage;
			}
		}

        private GeneratedImageInstance mDuplicateImage;
        public override ImageInstance ResultantImage
		{
            get { return mDuplicateImage; }
        }

		public override bool IsComplete() { return mDuplicateImage.IsComplete(); }

        protected override void DoWork_impl()
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            if (mEnabled && mSourceImage != null && mSourceImage.Bitmap != null)
            {
                mDuplicateImage.SetImage(new Bitmap(mSourceImage.Bitmap));
            }
            mDuplicateImage.SetIsComplete();

            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
        }
	}
}

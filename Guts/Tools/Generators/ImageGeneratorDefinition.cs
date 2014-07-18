// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;


namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ImageGeneratorDefinition : NetCams.ToolDefinition
	{
		public ImageGeneratorDefinition(TestSequence testSequence) : base(testSequence)
		{
		}

		public abstract GeneratedImageDefinition ResultantImage
		{
			get;
		}

        public virtual void SimulateGeneratingImage(Bitmap theImage)
        {
            throw new ArgumentException("Retesting an image isn't handled by it's generator.");
        }

        private bool mAutoSaveEnabled = false;
        [CategoryAttribute("Debug Options")]
        public bool AutoSaveEnabled
        {
            get { return mAutoSaveEnabled; }
            set
            {
                HandlePropertyChange(this, "AutoSaveEnabled", mAutoSaveEnabled, value);
                mAutoSaveEnabled = value;
            }
        }

        private String mAutoSavePath = "Debug\\<TESTSEQ>\\";
        [CategoryAttribute("Debug Options")]
        public String AutoSavePath
        {
            get { return mAutoSavePath; }
            set
            {
                HandlePropertyChange(this, "AutoSavePath", mAutoSavePath, value);
                mAutoSavePath = value;
            }
        }


	}
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Text;

namespace NetCams
{
	public class ImageDuplicatorDefinition : NetCams.ImageGeneratorDefinition
	{
        public ImageDuplicatorDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mDuplicateImage = new GeneratedImageDefinition(testSequence, OwnerLink.newLink(this, "ResultantImage"));
            mDuplicateImage.AddDependency(this);
            mDuplicateImage.Name = "Copy of image";
		}

		public override void CreateInstance(TestExecution theExecution)
		{
			new ImageDuplicatorInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
				return result + 1;
			}
		}

		private bool mEnabled = true;
		[CategoryAttribute("Parameters"),
		DescriptionAttribute("")]
        public bool Enabled
		{
            get { return mEnabled; }
            set 
            {
                HandlePropertyChange(this, "Enabled", mEnabled, value);
                mEnabled = value;
            }
		}

		private ImageDefinition mSourceImage = null;
		[CategoryAttribute("Input")]
		public ImageDefinition SourceImage
		{
			get { return mSourceImage; }
			set
            {
                HandlePropertyChange(this, "SourceImage", mSourceImage, value);
                if (mDuplicateImage.Name.StartsWith("Copy of"))
                {
                    if (value == null)
                    {
                        mDuplicateImage.Name = "Copy of image";
                    }
                    else
                    {
                        mDuplicateImage.Name = "Copy of " + value.Name;
                    }
                }
                mSourceImage = value;
            }
		}

		private GeneratedImageDefinition mDuplicateImage = null;
		[CategoryAttribute("Output")]
		public override GeneratedImageDefinition ResultantImage
		{
            get { return mDuplicateImage; }
		}

	}
}

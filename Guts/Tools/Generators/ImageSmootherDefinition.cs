// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;


namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	[DefaultPropertyAttribute("Name")] 
	public class ImageSmootherDefinition : NetCams.ImageGeneratorDefinition
	{
		public ImageSmootherDefinition(TestSequence testSequence) : base(testSequence)
		{
            mSmoothedImage = new GeneratedImageDefinition(testSequence, OwnerLink.newLink(this, "ResultantImage"));
			mSmoothedImage.AddDependency(this);
			mSmoothedImage.Name = "smoothed";
		}

		public override void CreateInstance(TestExecution theExecution)
		{
			new ImageSmootherInstance(this, theExecution);
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

		private int mXAxisRadius = 0;
		[CategoryAttribute("Parameters"),
		DescriptionAttribute("Number of pixels on the X-axis to average over.")]
		public int XAxisRadius
		{
			get { return mXAxisRadius; }
			set 
            {
                HandlePropertyChange(this, "XAxisRadius", mXAxisRadius, value);
                mXAxisRadius = value;
            }
		}

		private int mYAxisRadius = 0;
		[CategoryAttribute("Parameters")]
		public int YAxisRadius
		{
			get { return mYAxisRadius; }
			set 
            {
                HandlePropertyChange(this, "YAxisRadius", mYAxisRadius, value);
                mYAxisRadius = value;
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
                mSourceImage = value;
            }
        }

		private GeneratedImageDefinition mSmoothedImage = null;
		[CategoryAttribute("Output")]
		public override GeneratedImageDefinition ResultantImage
		{
			get { return mSmoothedImage; }
		}

	}
}

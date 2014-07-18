// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    [DefaultPropertyAttribute("Name")]
    public class ColorMatchCountOverRectangleDefinition : NetCams.ToolDefinition
    {
        public ColorMatchCountOverRectangleDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mResult = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "Result"));
            mResult.Type = DataType.IntegerNumber;
            mResult.AddDependency(this);
            mResult.Name = "ColorMatchCountResult";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
			new ColorMatchCountOverRectangleInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mColorMatchDefinition.IsDependentOn(theOtherObject)) return true;
            if (mROI.IsDependentOn(theOtherObject)) return true;
            if (mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mImageToMark != null && mImageToMark.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mROI != null) result = Math.Max(result, mROI.ToolMapRow);
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mImageToMark != null) result = Math.Max(result, mImageToMark.ToolMapRow);
                if (mColorMatchDefinition != null) result = Math.Max(result, mColorMatchDefinition.ToolMapRow);
                return result + 1;
			}
		}

        private ColorMatchInstance mCurrentColorMatchInstance;
        private ColorMatchInstance CurrentColorMatchInstance()
        {
            return mCurrentColorMatchInstance;
        }

        private ColorMatchDefinition mColorMatchDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the color to search for")]
        public ColorMatchDefinition ColorMatchDefinition
        {
            get { return mColorMatchDefinition; }
            set 
            {
                HandlePropertyChange(this, "ColorMatchDefinition", mColorMatchDefinition, value);
                mColorMatchDefinition = value;
            }
        }

        private IRectangleROIDefinition mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("ROI to test over")]
        public IRectangleROIDefinition ROI
        {
            get { return mROI; }
            set
            {
                HandlePropertyChange(this, "ROI", mROI, value);
                mROI = value;
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

        private GeneratedValueDefinition mResult = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition Result
        {
            get { return mResult; }
        }

        private bool mCreateMarkedImage = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Set to True to create a marked up image")]
        public bool CreateMarkedImage
        {
            get { return mCreateMarkedImage; }
            set
            {
                HandlePropertyChange(this, "CreateMarkedImage", mCreateMarkedImage, value);
                mCreateMarkedImage = value;
            }
        }

        private Color mMarkColor = Color.Yellow;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("The color which is used to highlight areas that match the color definition")]
        public Color MarkColor
        {
            get { return mMarkColor; }
            set 
            {
                HandlePropertyChange(this, "MarkColor", mMarkColor, value);
                mMarkColor = value;
            }
        }

        private ImageDefinition mImageToMark = null;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public ImageDefinition ImageToMark
        {
            get { return mImageToMark; }
            set
            {
                HandlePropertyChange(this, "ImageToMark", mImageToMark, value);
                if (value != mImageToMark)
                {
                    //if( mImageToMark != null ) mImageToMark.RemoveDependency(this);  WE NEED TO BE DEPENDENT ON the image so we know it is created (e.g. by duplicator) before we try to mark it...  if a tool wants to be dependent on a FULLY MARKED image, it must register dependencies on the markers.
                    mImageToMark = value;
                    //if( mImageToMark != null ) mImageToMark.AddDependency(this);
                }
            }
        }
    }
}

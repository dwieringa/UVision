// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NetCams
{
    public class ImageCropDefinition : NetCams.ImageGeneratorDefinition
    {
        public ImageCropDefinition(TestSequence testSequence)
            : base(testSequence)
		{
			mCroppedImage = new GeneratedImageDefinition(testSequence, OwnerLink.newLink(this, "ResultantImage"));
            mCroppedImage.AddDependency(this);
            mCroppedImage.Name = "cropResult";
		}

        public override void CreateInstance(TestExecution theExecution)
        {
            new ImageCropInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mWidth != null && mWidth.IsDependentOn(theOtherObject)) return true;
            if (mLeftEdge != null && mLeftEdge.IsDependentOn(theOtherObject)) return true;
            if (mTopEdge != null && mTopEdge.IsDependentOn(theOtherObject)) return true;
            if (mHeight != null && mHeight.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mLeftEdge != null) result = Math.Max(result, mLeftEdge.ToolMapRow);
                if (mWidth != null) result = Math.Max(result, mWidth.ToolMapRow);
                if (mTopEdge != null) result = Math.Max(result, mTopEdge.ToolMapRow);
                if (mHeight != null) result = Math.Max(result, mHeight.ToolMapRow);
                return result + 1;
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

        private GeneratedImageDefinition mCroppedImage = null;
        [CategoryAttribute("Output")]
        public override GeneratedImageDefinition ResultantImage
        {
            get { return mCroppedImage; }
        }

        private DataValueDefinition mLeftEdge;
        [CategoryAttribute("Parameters, x-axis"),
        DescriptionAttribute("Position on the x-axis of the source image where the crop is to start.")]
        public DataValueDefinition LeftEdge
        {
            get { return mLeftEdge; }
            set 
            {
                HandlePropertyChange(this, "LeftEdge", mLeftEdge, value);
                mLeftEdge = value;
            }
        }

        private DataValueDefinition mWidth;
        [CategoryAttribute("Parameters, x-axis"),
        DescriptionAttribute("The width of the resultant image in pixels.")]
        public DataValueDefinition Width
        {
            get { return mWidth; }
            set 
            {
                HandlePropertyChange(this, "Width", mWidth, value);
                mWidth = value;
            }
        }

        private DataValueDefinition mTopEdge;
        [CategoryAttribute("Parameters, y-axis"),
        DescriptionAttribute("Position on the y-axis of the source image where the crop is to start.")]
        public DataValueDefinition TopEdge
        {
            get { return mTopEdge; }
            set 
            {
                HandlePropertyChange(this, "TopEdge", mTopEdge, value);
                mTopEdge = value;
            }
        }

        private DataValueDefinition mHeight;
        [CategoryAttribute("Parameters, y-axis"),
        DescriptionAttribute("The height of the resultant image in pixels.")]
        public DataValueDefinition Height
        {
            get { return mHeight; }
            set 
            {
                HandlePropertyChange(this, "Height", mHeight, value);
                mHeight = value;
            }
        }

    }
}

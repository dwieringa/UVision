// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;


namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
    public class ImageCropInstance : NetCams.ImageGeneratorInstance
    {
        public ImageCropInstance(ImageCropDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            if (theDefinition.LeftEdge == null) throw new ArgumentException(theDefinition.Name + " doesn't have a value assigned to LeftEdge");
            mLeftEdge = testExecution.DataValueRegistry.GetObject(theDefinition.LeftEdge.Name);

            if (theDefinition.Width == null) throw new ArgumentException(theDefinition.Name + " doesn't have a value assigned to Width");
            mWidth = testExecution.DataValueRegistry.GetObject(theDefinition.Width.Name);

            if (theDefinition.TopEdge == null) throw new ArgumentException(theDefinition.Name + " doesn't have a value assigned to TopEdge");
            mTopEdge = testExecution.DataValueRegistry.GetObject(theDefinition.TopEdge.Name);

            if (theDefinition.Height == null) throw new ArgumentException(theDefinition.Name + " doesn't have a value assigned to Height");
            mHeight = testExecution.DataValueRegistry.GetObject(theDefinition.Height.Name);

            if (theDefinition.SourceImage == null) throw new ArgumentException(theDefinition.Name + " doesn't have a value assigned to SourceImage");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            mCroppedImage = new GeneratedImageInstance(theDefinition.ResultantImage, testExecution);
        }

        protected override void DoWork_impl()
        {
            int left = (int)mLeftEdge.ValueAsLong();
            int width = (int)mWidth.ValueAsLong();
            int top = (int)mTopEdge.ValueAsLong();
            int height = (int)mHeight.ValueAsLong();

            if (mSourceImage == null)
            {
                TestExecution().LogErrorWithTimeFromTrigger("SourceImage isn't defined.");
            }
            else if (mSourceImage.Bitmap == null)
            {
                TestExecution().LogErrorWithTimeFromTrigger("SourceImage isn't empty.");
            }
            else if (left < 0 || left >= mSourceImage.Bitmap.Width)
            {
                TestExecution().LogErrorWithTimeFromTrigger("LeftEdge is invalid.  value=" + left);
            }
            else if( width < 0 || width > mSourceImage.Bitmap.Width )
            {
                TestExecution().LogErrorWithTimeFromTrigger("Width is invalid.  value=" + width);
            }
            else if( top < 0 || top >= mSourceImage.Bitmap.Height )
            {
                TestExecution().LogErrorWithTimeFromTrigger("TopEdge is invalid.  value=" + top);
            }
            else if( height < 0 || height > mSourceImage.Bitmap.Height )
            {
                TestExecution().LogErrorWithTimeFromTrigger("Height is invalid.  value=" + height);
            }
            else if (left + width > mSourceImage.Bitmap.Width)
            {
                TestExecution().LogErrorWithTimeFromTrigger("LeftEdge and Width create an invalid RightEdge.  value=" + left + width + "; LeftEdge=" + left + "; Width=" + width);
            }
            else if (top + height > mSourceImage.Bitmap.Height)
            {
                TestExecution().LogErrorWithTimeFromTrigger("TopEdge and Height create an invalid BottomEdge.  value=" + top + height + "; TopEdge=" + top + "; Height=" + height);
            }
            else
            {
                // parameters are all good...

                //create the destination (cropped) bitmap
                Bitmap bmpCropped = new Bitmap(width, height);
                //create the graphics object to draw with
                Graphics g = Graphics.FromImage(bmpCropped);

                Rectangle rectDestination = new Rectangle(0, 0, bmpCropped.Width, bmpCropped.Height);
                Rectangle rectCropArea = new Rectangle(left, top, width, height);

                //draw the rectCropArea of the original image to the rectDestination of bmpCropped
                g.DrawImage(mSourceImage.Bitmap, rectDestination, rectCropArea,GraphicsUnit.Pixel);
                //release system resources
                g.Dispose();

                mCroppedImage.SetImage(bmpCropped);
            }
            mCroppedImage.SetIsComplete();
        }

        public override bool IsComplete() { return mCroppedImage.IsComplete(); }

        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private GeneratedImageInstance mCroppedImage = null;
        [CategoryAttribute("Output")]
        public override ImageInstance ResultantImage
        {
            get { return mCroppedImage; }
        }

        private DataValueInstance mLeftEdge;
        [CategoryAttribute("Parameters, x-axis"),
        DescriptionAttribute("Position on the x-axis of the source image where the crop is to start.")]
        public DataValueInstance LeftEdge
        {
            get { return mLeftEdge; }
        }

        private DataValueInstance mWidth;
        [CategoryAttribute("Parameters, x-axis"),
        DescriptionAttribute("The width of the resultant image in pixels.")]
        public DataValueInstance Width
        {
            get { return mWidth; }
        }

        private DataValueInstance mTopEdge;
        [CategoryAttribute("Parameters, y-axis"),
        DescriptionAttribute("Position on the y-axis of the source image where the crop is to start.")]
        public DataValueInstance TopEdge
        {
            get { return mTopEdge; }
        }

        private DataValueInstance mHeight;
        [CategoryAttribute("Parameters, y-axis"),
        DescriptionAttribute("The height of the resultant image in pixels.")]
        public DataValueInstance Height
        {
            get { return mHeight; }
        }

    }
}

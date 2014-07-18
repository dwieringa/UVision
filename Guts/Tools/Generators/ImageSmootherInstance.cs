// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;


namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class ImageSmootherInstance : NetCams.ImageGeneratorInstance
	{
		public ImageSmootherInstance(ImageSmootherDefinition theDefinition, TestExecution testExecution) : base(theDefinition, testExecution)
		{
            if (theDefinition.XAxisRadius == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to XAxisRadius");
            mXAxisRadius = theDefinition.XAxisRadius;

            if (theDefinition.YAxisRadius == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to YAxisRadius");
            mYAxisRadius = theDefinition.YAxisRadius;

            if (theDefinition.SourceImage == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to SourceImage");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);
			
            mSmoothedImage = new GeneratedImageInstance(theDefinition.ResultantImage, testExecution);
		}


		private int mXAxisRadius;
		public int XAxisRadius
		{
			get { return mXAxisRadius; }
		}

		private int mYAxisRadius;
		public int YAxisRadius
		{
			get { return mYAxisRadius; }
		}

		private ImageInstance mSourceImage;
		public ImageInstance SourceImage
		{
			get
			{
				return mSourceImage;
			}
		}

		private ImageInstance mSmoothedImage;
		public override ImageInstance ResultantImage
		{
			get
			{
				return mSmoothedImage;
			}
		}

		public override bool IsComplete() { return mSmoothedImage.IsComplete(); }

        protected override void DoWork_impl()
		{
			// TODO: using bit locking

            // TODO: this is a pretty lame averaging method. it only looks at the row/column of the pixel...not a rectangle or elipse around each pixel

            Bitmap newImage = new Bitmap(mSourceImage.Bitmap.Width, mSourceImage.Bitmap.Height);
            long sumR = 0;
            long sumG = 0;
            long sumB = 0;
            double averageR;
            double averageG;
            double averageB;
            int numPixels = (XAxisRadius * 2) + (YAxisRadius * 2 )+ 2; // we actually count the center dot twice! oops
            Color color;
            for (int newX = 0+XAxisRadius; newX < newImage.Width-XAxisRadius; newX++)
            {
                for (int newY = 0+YAxisRadius; newY < newImage.Height-YAxisRadius; newY++)
                {
                    sumR = 0;
                    sumG = 0;
                    sumB = 0;
                    for (int oldX = newX - XAxisRadius; oldX <= newX + XAxisRadius; oldX++)
                    {
                        color = mSourceImage.Bitmap.GetPixel(oldX,newY);
                        sumR += color.R;
                        sumG += color.G;
                        sumB += color.B;
                    }
                    for (int oldY = newY - YAxisRadius; oldY <= newY + YAxisRadius; oldY++)
                    {
                        color = mSourceImage.Bitmap.GetPixel(newX, oldY);
                        sumR += color.R;
                        sumG += color.G;
                        sumB += color.B;
                    }
                    averageR = (double)sumR / (double)numPixels;
                    averageG = (double)sumG / (double)numPixels;
                    averageB = (double)sumB / (double)numPixels;
                    color = Color.FromArgb((int)averageR, (int)averageG, (int)averageB);
                    newImage.SetPixel(newX, newY, color);
                }
            }

            mSmoothedImage.SetImage(newImage);
			mSmoothedImage.SetIsComplete();
		}
	}
}

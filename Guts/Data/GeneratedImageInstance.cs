// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Drawing;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class GeneratedImageInstance : ImageInstance
	{
		public GeneratedImageInstance(GeneratedImageDefinition theDefinition, TestExecution testExecution) : base(theDefinition, testExecution)
		{
		}
        
        private Bitmap mImage;
		public override Bitmap Image_old
		{
			get	{ return mImage; }
		}

		public override void SetImage(Bitmap theBitmap)
		{
			mImage = theBitmap;
            if (theBitmap != null)
            {
                mEditableBitmap = new EditableBitmap(theBitmap);

                //cache data in member variables to decrease overhead of property calls
                //this is especially important with Width and Height, as they call
                //GdipGetImageWidth() and GdipGetImageHeight() respectively in gdiplus.dll - 
                //which means major overhead.
                bitmapStride = mEditableBitmap.Stride;
                bitmapPixelFormatSize = mEditableBitmap.PixelFormatSize;
                bitmapBits = mEditableBitmap.Bits;
                bitmapWidth = mEditableBitmap.Bitmap.Width;
                bitmapHeight = mEditableBitmap.Bitmap.Height;
            }
		}
	}
}

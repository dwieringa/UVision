// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
    [TypeConverter(typeof(ImageInstanceConverter))]
	public abstract class ImageInstance : DataInstance
	{
		public ImageInstance(ImageDefinition theDefinition, TestExecution testExecution) : base(theDefinition, testExecution)
		{
            testExecution.ImageRegistry.RegisterObject(this);
		}

        protected EditableBitmap mEditableBitmap;

        //cached bitmap properties
        protected int bitmapWidth = 0;
        protected int bitmapHeight = 0;
        protected int bitmapStride = 0;
        protected int bitmapPixelFormatSize = 0;
        protected byte[] bitmapBits = null;


        public Bitmap Bitmap
        {
            get
            {
                if (mEditableBitmap == null) return null;
                return mEditableBitmap.Bitmap;
            }
        }

        public abstract Bitmap Image_old
		{
			get;
		}

        ///<summary>Calculates and returns the byte index for the pixel (x,y).</summary>
        ///<param name="x">The x coordinate of the pixel whose byte index should be returned.</param>
        ///<param name="y">The y coordinate of the pixel whose byte index should be returned.</param>
        public int CoordsToByteIndex(int x, int y) // NOTE: original author passed x & y by ref (for performance?)
        {
            return (bitmapStride * y) + (x * bitmapPixelFormatSize);
        }


        public int GetGrayValue(int x, int y)
        {
            int idx = CoordsToByteIndex(x, y);
            return (int)(0.3 * bitmapBits[idx + 2] + 0.59 * bitmapBits[idx + 1] + 0.11 * bitmapBits[idx + 0]);
            // add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
            // http://www.bobpowell.net/grayscale.htm
            // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1
        }
        public Color GetColor(int x, int y)
        {
            int idx = CoordsToByteIndex(x, y);
            return Color.FromArgb(/*bitmapBits[idx + 3],*/ bitmapBits[idx + 2], bitmapBits[idx + 1], bitmapBits[idx + 0]);
            // Array index 0 is blue, 1 is green, 2 is red, 3 is alpha        }
        }

        public void SetColor(int x, int y, Color theNewColor)
        {
            int idx = CoordsToByteIndex(x, y);
            bitmapBits[idx + 2] = theNewColor.R;
            bitmapBits[idx + 1] = theNewColor.G;
            bitmapBits[idx + 0] = theNewColor.B;
            // Array index 0 is blue, 1 is green, 2 is red, 3 is alpha        }
        }

        public int Width { get { return bitmapWidth; } }
        public int Height { get { return bitmapHeight; } }

        private ImageGeneratorInstance mImageGeneratorInstance = null;
        public ImageGeneratorInstance Generator
        {
            get { return mImageGeneratorInstance; }
        }
        public void SetImageGenerator(ImageGeneratorInstance theGenerator)
        {
            mImageGeneratorInstance = theGenerator;
        }

        protected string mSource;
        public string Source
        {
            get
            {
                return mSource;
            }
        }
        public void SetSource(string source)
        {
            mSource = source;
        }

		public abstract void SetImage(Bitmap theImage);

        private bool mIsComplete = false;
        public void SetIsComplete()
        {
            SetCompletedTime();
            mIsComplete = true;
        }
        public override bool IsComplete() { return mIsComplete; }

        public void Save(string filePath, string fileName, string fileNameSuffix)
        {
            Save(filePath, fileName, true, fileNameSuffix);
        }
        public void Save(string filePath, string fileName, bool includeDateTime)
        {
            Save(filePath, fileName, includeDateTime, "");
        }
        public void Save(string filePath, string fileName, bool includeDateTime, string fileNameSuffix)
        {
            if (Bitmap == null) return; // TODO: throw exception?

            filePath = FileHelper.ExpandPath(Definition(), filePath);
            if (!filePath.EndsWith("\\")) filePath += "\\";

            if (!Directory.Exists(filePath))
            {
                try
                {
                    Directory.CreateDirectory(filePath);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Unable to save image to '" + filePath + "'. Reason=Unable to create directory.  Low-level message=" + e.Message);
                }
            }

            filePath += fileName;
            if (includeDateTime)
            {
                filePath += "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }

            filePath += fileNameSuffix;

            if( Bitmap.RawFormat.Equals(ImageFormat.Bmp) )
            {
                filePath += ".bmp";
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.MemoryBmp))
            {
                filePath += ".jpg"; //TODO: is this right? it's coming from camera, so it should be jpg.
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.Jpeg))
            {
                filePath += ".jpg";
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.Png))
            {
                filePath += ".png";
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.Gif))
            {
                filePath += ".gif";
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.Emf))
            {
                filePath += ".emf";
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.MemoryBmp))
            {
                filePath += "ZZZ.bmp";
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.Exif))
            {
                filePath += "ZZZ.jpg";
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.Tiff))
            {
                filePath += ".tif";
            }
            else if (Bitmap.RawFormat.Equals(ImageFormat.Wmf))
            {
                filePath += ".wmf";
            }
            else
            {
                TestExecution().LogErrorWithTimeFromTrigger("Can't save image.  Unsupported file type.  Type=" + Bitmap.RawFormat.ToString());
                filePath += ".jpg"; //hack
            }

            try
            {
                Bitmap.Save(filePath);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to save image to '" + filePath + "'.  Low-level message=" + e.Message);
            }
        }
    }

    public class ImageInstanceConverter : ObjectInstanceConverter
    {
        protected override string[] Options(TestExecution theTestExecution)
        {
            return theTestExecution.ImageRegistry.Options();
        }

        protected override IObjectInstance LookupObject(TestExecution theTestExecution, string theObjectName)
        {
            return theTestExecution.ImageRegistry.GetObject(theObjectName);
        }
    }
}

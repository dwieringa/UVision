// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace NetCams
{
    /// <summary>
    /// GlobalImage was added for DS's FixtureGage.  There we have a separate TestSequence for each measurement (40+).
    /// When a new part is tested, we first run an Init TestSequence which initializes GlobalValues.  I will also have it load
    /// a GlobalImage (scan or photo of part) and then have each TestSequence draw a pass/fail mark in the area to
    /// symbolize whether the measurement in that area of the part passed or failed.  Each individual measurement only
    /// gets a snapshot of a small part of the area, so this allows all measurements to be marked on one common image which is
    /// shown in the final TestSkews TestSequence.
    /// </summary>
    //[TypeConverter(typeof(GlobalImageConverter))]
    public class GlobalImage
    {
        public GlobalImage(Project theProject)
        {
            mProject = theProject;
        }
        private Project mProject;

        protected EditableBitmap mEditableBitmap;
        public Bitmap Bitmap
        {
            get
            {
                if (mEditableBitmap == null) return null;
                return mEditableBitmap.Bitmap;
            }
        }

        //TODO: add Scope support?  so they can limit scope to TestSequence?

        protected string name = "";
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                // TODO: ensure unique
                name = value;
            }
        }
        public override string ToString()
        {
            return Name;
        }

        /*
        //cached bitmap properties
        protected int bitmapWidth = 0;
        protected int bitmapHeight = 0;
        protected int bitmapStride = 0;
        protected int bitmapPixelFormatSize = 0;
        protected byte[] bitmapBits = null;

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
            return Color.FromArgb(/*bitmapBits[idx + 3],* / bitmapBits[idx + 2], bitmapBits[idx + 1], bitmapBits[idx + 0]);
            // Array index 0 is blue, 1 is green, 2 is red, 3 is alpha        }
        }
         
        public int Width { get { return bitmapWidth; } }
        public int Height { get { return bitmapHeight; } }

         */

        public string Width
        {
            get
            {
                if (mEditableBitmap == null || mEditableBitmap.Bitmap == null) return string.Empty;
                return mEditableBitmap.Bitmap.Width.ToString();
            }
        }
        public string Height
        {
            get
            {
                if (mEditableBitmap == null || mEditableBitmap.Bitmap == null) return string.Empty;
                return mEditableBitmap.Bitmap.Height.ToString();
            }
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

        public void SetImage(Bitmap theBitmap)
        {
            if (theBitmap != null)
            {
                // TODO: how to dispose previous image?  if TestSequences have a reference, it should be to a View of this one?
                mEditableBitmap = new EditableBitmap(theBitmap);

                /*
                //cache data in member variables to decrease overhead of property calls
                //this is especially important with Width and Height, as they call
                //GdipGetImageWidth() and GdipGetImageHeight() respectively in gdiplus.dll - 
                //which means major overhead.
                bitmapStride = mEditableBitmap.Stride;
                bitmapPixelFormatSize = mEditableBitmap.PixelFormatSize;
                bitmapBits = mEditableBitmap.Bits;
                bitmapWidth = mEditableBitmap.Bitmap.Width;
                bitmapHeight = mEditableBitmap.Bitmap.Height;
                 */
            }
        }

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

            filePath = FileHelper.ExpandPath(mProject, filePath);
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

            if (Bitmap.RawFormat.Equals(ImageFormat.Bmp))
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
                mProject.LogError("Can't save image.  Unsupported file type.  Type=" + Bitmap.RawFormat.ToString());
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

    /*
    public class GlobalImageConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(((IObjectDefinition)context.Instance).TestSequence().GlobalImageOptions());
            }
            else if (context.Instance is FavoriteSettings)
            {
                return new StandardValuesCollection(((FavoriteSettings)context.Instance).mTestSequence.GlobalImageOptions());
            }
            else
            {
                throw new ArgumentException("Unexpected context (352323)");
            }
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(GlobalImage))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is GlobalImage)
            {

                GlobalImage globalValue = (GlobalImage)value;

                return globalValue.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                GlobalImage namedGlobalImage;

                try
                {
                    string globalValueName = (string)value;
                    if (globalValueName.Length == 0) return null;

                    if (context.Instance is IObjectDefinition)
                    {
                        IObjectDefinition objectBeingEditted = (IObjectDefinition)context.Instance;
                        namedGlobalImage = objectBeingEditted.TestSequence().GetGlobalImageIfExists(globalValueName);
                    }
                    else if (context.Instance is FavoriteSettings)
                    {
                        namedGlobalImage = ((FavoriteSettings)context.Instance).mTestSequence.GetGlobalImageIfExists(globalValueName);
                    }
                    else
                    {
                        throw new ArgumentException("alksdjalksdlkajlkaj");
                    }
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to an GlobalImage");
                }
                if (namedGlobalImage == null)
                {
                    throw new ArgumentException("GlobalImage '" + (string)value + "' couldn't be found.");
                }
                return namedGlobalImage;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    */
}

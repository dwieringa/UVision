// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    class FindBrightestSpotInstance : ToolInstance
    {
        public FindBrightestSpotInstance(FindBrightestSpotDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.ROI == null) throw new ArgumentException(Name + " doesn't have ROI defined.");
            mROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);

            if (theDefinition.BrightnessThreshold == null) throw new ArgumentException(Name + " doesn't have BrightnessThreshold defined.");
            mBrightnessThreshold = testExecution.DataValueRegistry.GetObject(theDefinition.BrightnessThreshold.Name);

            mBrightSpot_X = new GeneratedValueInstance(theDefinition.BrightSpot_X, testExecution);
            mBrightSpot_Y = new GeneratedValueInstance(theDefinition.BrightSpot_Y, testExecution);

            mAutoSave = theDefinition.AutoSave;
        }
        private bool mAutoSave;

        private ValueBasedLineDecorationInstance mEdgeMarker = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public ValueBasedLineDecorationInstance EdgeMarker
        {
            get { return mEdgeMarker; }
        }

        private GeneratedValueInstance mBrightSpot_X = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public GeneratedValueInstance BrightSpot_X
        {
            get { return mBrightSpot_X; }
        }

        private GeneratedValueInstance mBrightSpot_Y = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public GeneratedValueInstance BrightSpot_Y
        {
            get { return mBrightSpot_Y; }
        }

        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private ROIInstance mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public ROIInstance ROI
        {
            get { return mROI; }
        }

        private DataValueInstance mBrightnessThreshold;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance BrightnessThreshold
        {
            get { return mBrightnessThreshold; }
        }

        public override bool IsComplete() { return mBrightSpot_X.IsComplete(); }

        private bool abort = false;
        private int x;
        private int y;
        private Color pixelColor;
        private Color prevPixelColor;
        private int pixelGrayValue;
        private int prevPixelGrayValue;

        private const int MAX_LOG_WARNINGS = 25;
        private int loggedWarnings = 0;
        private void LogWarning(string msg)
        {
            if (loggedWarnings < 0)
            {
                // do nothing...we've shut them off due to the limit
            }
            else if (loggedWarnings < MAX_LOG_WARNINGS)
            {
                TestExecution().LogMessage("WARNING: " + msg);
                loggedWarnings++;
            }
            else
            {
                TestExecution().LogMessage("WARNING: " + Name + " reached its threshold of warnings in the log. Warnings surpressed.");
                loggedWarnings = -1;
            }
        }

        private enum SurfaceSearchState
        {
            SearchingForEndOfSurface,
            SearchingForEndOfTransition
        }

        public static readonly PixelFormat PIXEL_FORMAT = PixelFormat.Format32bppArgb;
        public static readonly int PIXEL_BYTE_WIDTH = 4; // determined by PixelFormat.Format32bppArgb; http://www.bobpowell.net/lockingbits.htm
        public override void DoWork() 
		{
            /* TODO: OPTIMIZATIONS:
             * - compute surfaceNoiseLevel based on image analysis
             * - make debug output to log optional
             * - surface/transition decorations (biggest problem is that there can be a variable number...only for first edge to start?)
             * - for marked image, save decorations...don't copy/paint_on image
             */

            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            int resultX = -1;
            int resultY = -1;

            if (mSourceImage.Bitmap == null)
            {
                TestExecution().LogMessage("ERROR: source image for '" + Name + "' does not exist.");
            }
            else
            {
                Bitmap sourceBitmap = SourceImage.Bitmap;
                BitmapData sourceBitmapData = null;


                try
                {
                    sourceBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PIXEL_FORMAT);
                    int sourceStride = sourceBitmapData.Stride;
                    int sourceStrideOffset = sourceStride - (sourceBitmapData.Width * PIXEL_BYTE_WIDTH);

                    int brightnessThreshold = (int)mBrightnessThreshold.ValueAsLong();

                    Point currentPoint = new Point(-1, -1);
                    mROI.GetFirstPointOnXAxis(mSourceImage, ref currentPoint);

                    ValueGrouper xGrouper = new ValueGrouper(0, 255, 50);
                    ValueGrouper yGrouper = new ValueGrouper(0, 255, 50);
                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* sourcePointer;

                        while (currentPoint.X != -1 && currentPoint.Y != -1)
                        {
                            sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                            sourcePointer += (currentPoint.Y * sourceStride) + (currentPoint.X * PIXEL_BYTE_WIDTH); // adjust to current point
                            pixelGrayValue = (int)(0.3 * sourcePointer[2] + 0.59 * sourcePointer[1] + 0.11 * sourcePointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            // http://www.bobpowell.net/grayscale.htm
                            // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1

                            //TestExecution().LogMessage(currentPoint.X + "," + currentPoint.Y + "  " + pixelGrayValue + " " + brightnessThreshold);
                            if (pixelGrayValue >= brightnessThreshold)
                            {
                                xGrouper.AddValue(currentPoint.X);
                                yGrouper.AddValue(currentPoint.Y);
                            }

                            mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
                        }
                        TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] finished analyzing pixels");
                    } // end unsafe block

                    for (int z = 0; z < xGrouper.NumGroups; z++)
                    {
                        ValueGrouper.GroupStats groupStats = xGrouper.GetGroup(z);
                        TestExecution().LogMessage(groupStats.start + " " + groupStats.end + " " + groupStats.count + " " + groupStats.Average());
                    }
                    for (int z = 0; z < yGrouper.NumGroups; z++)
                    {
                        ValueGrouper.GroupStats groupStats = yGrouper.GetGroup(z);
                        TestExecution().LogMessage(groupStats.start + " " + groupStats.end + " " + groupStats.count + " " + groupStats.Average());
                    }
                    ValueGrouper.GroupStats biggestXGroup = xGrouper.BiggestGroupWithNeighbors();
                    if (biggestXGroup != null)
                    {
                        resultX = biggestXGroup.Average();
                    }
                    ValueGrouper.GroupStats biggestYGroup = yGrouper.BiggestGroupWithNeighbors();
                    if (biggestXGroup != null)
                    {
                        resultY = biggestYGroup.Average();
                    }
                }
                catch (Exception e)
                {
                    TestExecution().LogMessageWithTimeFromTrigger("ERROR: Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
                }
                finally
                {
                    sourceBitmap.UnlockBits(sourceBitmapData);
                }

            } // end main block ("else" after all initial setup error checks)
            mBrightSpot_X.SetValue(resultX);
            mBrightSpot_Y.SetValue(resultY);
            mBrightSpot_X.SetIsComplete();
            mBrightSpot_Y.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " computed bright spot at " + resultX + "," + resultY);

            if (mAutoSave)
            {
                try
                {
                    string filePath = ((FindBrightestSpotDefinition)Definition()).AutoSavePath;
                    mSourceImage.Save(filePath, Name, true);
                    TestExecution().LogMessageWithTimeFromTrigger("Snapshot saved");
                }
                catch (ArgumentException e)
                {
                    Project().Window().logMessage("ERROR: " + e.Message);
                    TestExecution().LogErrorWithTimeFromTrigger(e.Message);
                }
                catch (Exception e)
                {
                    Project().Window().logMessage("ERROR: Unable to AutoSave snapshot from " + Name + ".  Ensure path valid and disk not full.  Low-level message=" + e.Message);
                    TestExecution().LogErrorWithTimeFromTrigger("Unable to AutoSave snapshot from " + Name + ".  Ensure path valid and disk not full.");
                }
            }

            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
        }

    }
}

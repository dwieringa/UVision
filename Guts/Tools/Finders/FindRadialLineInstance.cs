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
    class FindRadialLineInstance : ToolInstance
    {
        public FindRadialLineInstance(FindRadialLineDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.CenterX == null) throw new ArgumentException(Name + " doesn't have CenterX defined.");
            mCenterX = testExecution.DataValueRegistry.GetObject(theDefinition.CenterX.Name);

            if (theDefinition.CenterY == null) throw new ArgumentException(Name + " doesn't have CenterY defined.");
            mCenterY = testExecution.DataValueRegistry.GetObject(theDefinition.CenterY.Name);

            if (theDefinition.NumberOfTestsInDonut == null) throw new ArgumentException(Name + " doesn't have NumberOfTestsInDonut defined.");
            mNumberOfTestsInDonut = testExecution.DataValueRegistry.GetObject(theDefinition.NumberOfTestsInDonut.Name);

            if (theDefinition.OuterSearchRadius == null) throw new ArgumentException(Name + " doesn't have OuterSearchRadius defined.");
            mOuterSearchRadius = testExecution.DataValueRegistry.GetObject(theDefinition.OuterSearchRadius.Name);

            if (theDefinition.InnerSearchRadius == null) throw new ArgumentException(Name + " doesn't have InnerSearchRadius defined.");
            mInnerSearchRadius = testExecution.DataValueRegistry.GetObject(theDefinition.InnerSearchRadius.Name);

            if (theDefinition.MarkMergeDistance_Deg == null) throw new ArgumentException(Name + " doesn't have MarkMergeDistance_Deg defined.");
            mMarkMergeDistance_Deg = testExecution.DataValueRegistry.GetObject(theDefinition.MarkMergeDistance_Deg.Name);

            /*
            if (theDefinition.OuterSearchBounds == null) throw new ArgumentException(Name + " doesn't have OuterSearchBounds defined.");
            mOuterSearchBounds = testExecution.GetCircleDecoration(theDefinition.OuterSearchBounds.Name);

            if (theDefinition.InnerSearchBounds == null) throw new ArgumentException(Name + " doesn't have InnerSearchBounds defined.");
            mInnerSearchBounds = testExecution.GetCircleDecoration(theDefinition.InnerSearchBounds.Name);
            */

            mOuterSearchBounds = new CircleDecorationInstance(theDefinition.OuterSearchBounds, testExecution);
            mInnerSearchBounds = new CircleDecorationInstance(theDefinition.InnerSearchBounds, testExecution);

            mResultantAngle = new GeneratedValueInstance(theDefinition.ResultantAngle, testExecution);
            mResultantRay = new ValueBasedLineDecorationInstance(theDefinition.ResultantRay, testExecution);

            mAutoSave = theDefinition.AutoSave;
            mCreateMarkedImage = theDefinition.CreateMarkedImage;
            mMarkColor = theDefinition.MarkColor;
            if (theDefinition.ImageToMark != null)
            {
                mImageToMark = testExecution.ImageRegistry.GetObject(theDefinition.ImageToMark.Name);
            }

        }
        private bool mAutoSave;

        private bool mCreateMarkedImage = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Set to True to create a marked up image")]
        public bool CreateMarkedImage
        {
            get { return mCreateMarkedImage; }
        }
        private Color mMarkColor;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("The color which is used to highlight areas that are detected as a mark")]
        public Color MarkColor
        {
            get { return mMarkColor; }
        }
        private ImageInstance mImageToMark = null;
        [CategoryAttribute("Output")]
        public ImageInstance ImageToMark
        {
            get { return mImageToMark; }
        }

        private ValueBasedLineDecorationInstance mResultantRay = null;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public LineDecorationInstance ResultantRay
        {
            get { return mResultantRay; }
        }

        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private DataValueInstance mCenterX;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance CenterX
        {
            get { return mCenterX; }
        }

        private DataValueInstance mCenterY;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance CenterY
        {
            get { return mCenterY; }
        }

        private DataValueInstance mOuterSearchRadius;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance OuterSearchRadius
        {
            get { return mOuterSearchRadius; }
        }

        private DataValueInstance mInnerSearchRadius;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance InnerSearchRadius
        {
            get { return mInnerSearchRadius; }
        }

        private DataValueInstance mNumberOfTestsInDonut;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance NumberOfTestsInDonut
        {
            get { return mNumberOfTestsInDonut; }
        }

        private GeneratedValueInstance mResultantAngle = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance ResultantAngle
        {
            get { return mResultantAngle; }
        }

        private CircleDecorationInstance mOuterSearchBounds;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public CircleDecorationInstance OuterSearchBounds
        {
            get { return mOuterSearchBounds; }
        }

        private CircleDecorationInstance mInnerSearchBounds;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public CircleDecorationInstance InnerSearchBounds
        {
            get { return mInnerSearchBounds; }
        }

        private DataValueInstance mMarkMergeDistance_Deg;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance MarkMergeDistance_Deg
        {
            get { return mMarkMergeDistance_Deg; }
        }

        public override bool IsComplete() { return mResultantAngle.IsComplete(); }

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

        public static readonly PixelFormat PIXEL_FORMAT = PixelFormat.Format32bppArgb;
        public static readonly int PIXEL_BYTE_WIDTH = 4; // determined by PixelFormat.Format32bppArgb; http://www.bobpowell.net/lockingbits.htm
        public override void DoWork() 
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            int resultantAngle = -1;

            if (mPrerequisite == null || mPrerequisite.ValueAsBoolean())
            {
                int centerX = (int)mCenterX.ValueAsLong();
                int centerY = (int)mCenterY.ValueAsLong();
                int outerRadius = (int)mOuterSearchRadius.ValueAsLong();
                int innerRadius = (int)mInnerSearchRadius.ValueAsLong();

                if (mSourceImage.Bitmap == null)
                {
                    TestExecution().LogMessage("ERROR: source image for '" + Name + "' does not exist.");
                }
                else if (centerX < outerRadius || centerX + outerRadius >= mSourceImage.Bitmap.Width ||
                    centerY < outerRadius || centerY + outerRadius >= mSourceImage.Bitmap.Height)
                {
                    TestExecution().LogMessage("ERROR: OuterSearchBounds for '" + Name + "' isn't completely within the image bounds; center=" + centerX + "," + centerY + "; outer search radius=" + outerRadius + "; image size=" + mSourceImage.Bitmap.Width + "x" + mSourceImage.Bitmap.Height);
                }
                else if (innerRadius > outerRadius)
                {
                    TestExecution().LogMessage("ERROR: The inner search radius for '" + Name + "' greater than the outer radius: inner radius=" + innerRadius + "; outer radius=" + outerRadius);
                }
                else
                {
                    int MaxDegDist = (int)mMarkMergeDistance_Deg.ValueAsLong();
                    int numTests = (int)mNumberOfTestsInDonut.ValueAsLong();

                    Bitmap sourceBitmap = SourceImage.Bitmap;
                    Bitmap markedBitmap = null;
                    BitmapData sourceBitmapData = null;
                    BitmapData markedBitmapData = null;

                    if (mCreateMarkedImage && mImageToMark != null && mImageToMark.Bitmap != null)
                    {
                        markedBitmap = mImageToMark.Bitmap;
                    }

                    try
                    {
                        sourceBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PIXEL_FORMAT);
                        if (markedBitmap != null)
                        {
                            markedBitmapData = markedBitmap.LockBits(new Rectangle(0, 0, markedBitmap.Width, markedBitmap.Height), ImageLockMode.ReadWrite, PIXEL_FORMAT);
                        }
                        int sourceStride = sourceBitmapData.Stride;
                        int sourceStrideOffset = sourceStride - (sourceBitmapData.Width * PIXEL_BYTE_WIDTH);

                        unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                        {
                            byte* sourcePointer;
                            byte* markedPointer;

                            ValueGrouper grouper = new ValueGrouper(0, 360, 24); // 12 groups = 30 degress each, 24 groups = 15 degrees each
                            int stepSize = Math.Max(1, (outerRadius - innerRadius) / numTests);
                            for (int radius = innerRadius; radius <= outerRadius; radius += stepSize)
                            {
                                TestExecution().LogMessage("");
                                TestExecution().LogMessageWithTimeFromTrigger(Name + " searching for mark at radius " + radius);
                                //*************************************************
                                //
                                // COLLECT GRAY VALUES AROUND THE CIRCLE (stored in "samples" array)
                                // AND COMPUTE AVERAGE GREY VALUE
                                //
                                //*************************************************
                                int x = 0;
                                int y = 0;
                                double rad = 0;
                                /*
                                for (x = centerX - outerRadius; x <= centerX + outerRadius; x++)
                                {
                                    xdist = (x-centerX);
                                    sqrt = (int)Math.Sqrt(outerRadius * outerRadius - xdist * xdist);
                                    y = centerY + sqrt;
                                    mSourceImage.Image.SetPixel(x, y, Color.Magenta);
                                    mSourceImage.Image.SetPixel(x, centerY - sqrt, Color.Magenta);
                                }*/
                                int lastX = 0;
                                int lastY = 0;
                                int change;
                                int biggestChange = 0;
                                bool inited = false;
                                int degFraction = 1;
                                int numberOfSamples = 360 * degFraction;
                                double inc = (Math.PI / 180) / degFraction; // 1 deg = Pi/180
                                int[] samples = new int[numberOfSamples];
                                long sum = 0;
                                int avg = -1;
                                // http://en.wikipedia.org/wiki/Cirlce
                                // http://en.wikipedia.org/wiki/Radians
                                for (int a = 0; a < numberOfSamples; a++)
                                {
                                    rad += inc;
                                    x = (int)(centerX + radius * Math.Cos(rad));
                                    y = (int)(centerY + radius * Math.Sin(rad));

                                    //mSourceImage.Image.SetPixel(x, y, Color.Magenta);
                                    sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                    sourcePointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point
                                    samples[a] = (int)(0.3 * sourcePointer[2] + 0.59 * sourcePointer[1] + 0.11 * sourcePointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                    sum += samples[a];
                                    change = Math.Abs(x - lastX);
                                    if (change > biggestChange && inited) biggestChange = change;
                                    change = Math.Abs(y - lastY);
                                    if (change > biggestChange && inited) biggestChange = change;
                                    lastX = x;
                                    lastY = y;
                                    inited = true;
                                }
                                avg = (int)(sum / numberOfSamples);

                                //*************************************************
                                //
                                // COMPUTE STD DEVIATION OF GRAY VALUES
                                //
                                //*************************************************
                                // compute std dev: sum squares of deviations http://en.wikipedia.org/wiki/Standard_deviation
                                long sumSqDev = 0;
                                for (int a = 0; a < numberOfSamples; a++)
                                {
                                    sumSqDev += (long)Math.Pow(avg - samples[a], 2);
                                }
                                int stdDev = Math.Max(1, (int)Math.Sqrt(sumSqDev / numberOfSamples));


                                //*************************************************
                                //
                                // SEARCH FOR MARKS BY LOOKING FOR DARK SPOTS (at least 2 std dev below avg)
                                // WE SCORE MARKS BASED ON HOW "WIDE" THEY ARE
                                //
                                //*************************************************
                                List<Mark> marks = new List<Mark>();
                                Mark lastMark = null;
                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": std dev=" + stdDev);
                                for (int a = 0; a < numberOfSamples; a++)
                                {
                                    if (samples[a] < avg - 2 * stdDev)
                                    {
                                        if (lastMark != null && lastMark.DegreesFromEnd(a, inc) <= MaxDegDist)
                                        {
                                            lastMark.endPos = a;
                                            lastMark.score += 1;
                                        }
                                        else
                                        {
                                            lastMark = new Mark();
                                            marks.Add(lastMark);
                                            lastMark.startPos = a;
                                            lastMark.endPos = a;
                                            lastMark.score = 1;
                                        }
                                        rad = a * inc;
                                        x = (int)(centerX + radius * Math.Cos(rad));
                                        y = (int)(centerY + radius * Math.Sin(rad));
                                        if (markedBitmap != null)
                                        {
                                            markedPointer = (byte*)markedBitmapData.Scan0; // init to first byte of image
                                            markedPointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point
                                            markedPointer[3] = mMarkColor.A;
                                            markedPointer[2] = mMarkColor.R;
                                            markedPointer[1] = mMarkColor.G;
                                            markedPointer[0] = mMarkColor.B;
                                        }
                                        TestExecution().LogMessageWithTimeFromTrigger(Name + ": dark at " + x + "," + y);
                                    }
                                }
                                if (marks.Count > 1) // if mark is crosses 0/360deg, then merge the two marks into one
                                {
                                    if (lastMark.DegreesFromEnd(marks[0].startPos, inc) < MaxDegDist)
                                    {
                                        TestExecution().LogMessageWithTimeFromTrigger(Name + ": MERGING MARKS AROUND 0/360.");
                                        lastMark.endPos = marks[0].endPos;
                                        lastMark.score += marks[0].score;
                                        marks.RemoveAt(0);
                                    }
                                }
                                double highestConcentrationScore = 0;
                                Mark markWithHighestConcentrationScore = null;
                                foreach (Mark mark in marks)
                                {
                                    if (mark.score > 2)
                                    {
                                        if (mark.Width(numberOfSamples) > highestConcentrationScore)
                                        {
                                            highestConcentrationScore = mark.Width(numberOfSamples);
                                            markWithHighestConcentrationScore = mark;
                                        }
                                        TestExecution().LogMessageWithTimeFromTrigger(Name + ": mark: deg=" + mark.Middle_Deg(numberOfSamples, inc) + "(" + mark.Start_Deg(inc) + " to " + mark.End_Deg(inc) + ")  score=" + mark.score + "  width=" + mark.Width(numberOfSamples) + "samples  start=" + mark.startPos + " end=" + mark.endPos);
                                    }
                                }
                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": biggest change=" + biggestChange);
                                if (markWithHighestConcentrationScore != null)
                                {
                                    int markAngle = markWithHighestConcentrationScore.Middle_Deg(numberOfSamples, inc);
                                    grouper.AddValue(markAngle);
                                    TestExecution().LogMessageWithTimeFromTrigger(Name + ": at radius " + radius + " using angle " + markAngle);
                                }
                            } // end for loop (to test each radius)


                            ValueGrouper.GroupStats biggestGroup = grouper.BiggestGroup();
                            if (biggestGroup == null)
                            {
                                TestExecution().LogMessageWithTimeFromTrigger(Name + " no radius found (no biggest in grouper)");
                            }
                            else
                            {
                                TestExecution().LogMessageWithTimeFromTrigger(Name + " biggest group: count=" + biggestGroup.count + "  ndx=" + biggestGroup.groupNdx + "  avg=" + biggestGroup.Average() + "  min=" + biggestGroup.min + "  max=" + biggestGroup.max);
                                ValueGrouper.GroupStats cw_group = null;
                                ValueGrouper.GroupStats ccw_group = null;
                                if (biggestGroup.groupNdx == 0)
                                {
                                    ccw_group = grouper.GetGroup(grouper.NumGroups - 1);
                                    ccw_group.sum = ccw_group.sum - (360 * ccw_group.count); // HACK: adjust angles to compensate for 0/360 switch
                                }
                                else
                                {
                                    ccw_group = grouper.GetGroup(biggestGroup.groupNdx - 1);
                                }
                                if (biggestGroup.groupNdx == grouper.NumGroups - 1)
                                {
                                    cw_group = grouper.GetGroup(0);
                                    cw_group.sum = cw_group.sum + (360 * cw_group.count); // HACK: adjust angles to compensate for 0/360 switch
                                }
                                else
                                {
                                    ccw_group = grouper.GetGroup(biggestGroup.groupNdx + 1);
                                }

                                long overallSum = biggestGroup.sum;
                                int overallCount = biggestGroup.count;
                                if (ccw_group != null && ccw_group.count > 0)
                                {
                                    TestExecution().LogMessageWithTimeFromTrigger(Name + " using CCW group; count=" + ccw_group.count + "  ndx=" + ccw_group.groupNdx + "  avg=" + ccw_group.Average() + "  min=" + ccw_group.min + "  max=" + ccw_group.max);
                                    overallSum += ccw_group.sum;
                                    overallCount += ccw_group.count;
                                }
                                if (cw_group != null && cw_group.count > 0)
                                {
                                    TestExecution().LogMessageWithTimeFromTrigger(Name + " using CW group; count=" + cw_group.count + "  ndx=" + cw_group.groupNdx + "  avg=" + cw_group.Average() + "  min=" + cw_group.min + "  max=" + cw_group.max);
                                    overallSum += cw_group.sum;
                                    overallCount += cw_group.count;
                                }
                                resultantAngle = (int)(overallSum / overallCount);
                                if (resultantAngle >= 360) resultantAngle -= 360;
                                else if (resultantAngle < 0) resultantAngle += 360;

                                double overallRad = (resultantAngle * Math.PI) / 180; // deg = rad*180/PI   rad=deg*PI/180
                                mResultantRay.SetStartX(centerX);
                                mResultantRay.SetStartY(centerY);
                                mResultantRay.SetEndX((int)(centerX + outerRadius * Math.Cos(overallRad)));
                                mResultantRay.SetEndY((int)(centerY + outerRadius * Math.Sin(overallRad)));
                                mResultantRay.SetIsComplete();
                            }
                        } // end unsafe block
                    }
                    catch (Exception e)
                    {
                        TestExecution().LogMessageWithTimeFromTrigger("ERROR: Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
                    }
                    finally
                    {
                        sourceBitmap.UnlockBits(sourceBitmapData);
                        if (markedBitmap != null)
                        {
                            markedBitmap.UnlockBits(markedBitmapData);
                        }
                    }
                } // end main block ("else" after all initial setup error checks)
            }
            else
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met.");
            }

            mResultantAngle.SetValue(resultantAngle);
            mResultantAngle.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " computed angle at " + resultantAngle);

            if (mAutoSave)
            {
                try
                {
                    string filePath = ((FindRadialLineDefinition)Definition()).AutoSavePath;
                    mSourceImage.Save(filePath, Name, true);
                    if (mImageToMark != null) mImageToMark.Save(filePath, Name, "_marked_" + resultantAngle);
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

        private class Mark
        {
            public int startPos;
            public int endPos;
            public int score;
            public int Start_Deg(double inc) { return (int)(startPos * inc * 180 / Math.PI); }
            public int End_Deg(double inc) { return (int)(endPos * inc * 180 / Math.PI); }
            public int DegreesFromEnd(int pos, double inc) { return Width_Deg(endPos, pos, inc); }
            public int Width(int numberOfSamples)
            {
                if (endPos > startPos) return endPos - startPos;
                return (numberOfSamples - startPos) + endPos;
            }
            public int Width_Deg(double inc) { return Width_Deg(startPos, endPos, inc); }
            public int Width_Deg(int pos1, int pos2, double inc)
            {
                double pos1Deg = (pos1 * inc) * 180 / Math.PI; // deg = rad*180/PI
                double pos2Deg = (pos2 * inc) * 180 / Math.PI;

                if (pos2 > pos1) return (int)(pos2Deg - pos1Deg);
                return (int)((pos2Deg + 360) - pos1Deg);
            }
            public double Middle_Rad(int numSamples, double inc)
            {
                double result;
                if (endPos > startPos)
                {
                    result = ((startPos + endPos) / 2) * inc;
                    return result;
                }
                // we cross 0/360...
                if (endPos > numSamples - startPos)
                {
                    // middle falls beyond 0
                    int endPos_adjusted = endPos - numSamples;
                    result = ((startPos + endPos_adjusted) / 2) * inc;
                    return result;
                }
                else
                {
                    // middle falls behind 0
                    int startPos_adjusted = numSamples + startPos;
                    result = ((startPos_adjusted + endPos) / 2) * inc;
                    return result;
                }
            }
            public int Middle_Deg(int numSamples, double inc)
            {
                int result = (int)(Middle_Rad(numSamples, inc) * 180 / Math.PI);
                if (result == 360) result = 0;
                return result;
            }
            public double ConcentrationScore(int numSamples)
            {
                return score / (double)Width(numSamples);
            }
        }

    }
}

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
    class FindBoundingRectangleInstance : ToolInstance, ImageTool
    {
        public FindBoundingRectangleInstance(FindBoundingRectangleDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mFindBoundingRectangleDefinition = theDefinition;

            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.StartPoint_X == null) throw new ArgumentException(Name + " doesn't have StartPoint_X defined.");
            mStartPoint_X = testExecution.DataValueRegistry.GetObject(theDefinition.StartPoint_X.Name);

            if (theDefinition.StartPoint_Y == null) throw new ArgumentException(Name + " doesn't have StartPoint_Y defined.");
            mStartPoint_Y = testExecution.DataValueRegistry.GetObject(theDefinition.StartPoint_Y.Name);

            if (theDefinition.ColorMatchDefinition == null) throw new ArgumentException(Name + " doesn't have ColorMatchDefinition defined.");
            mColorMatchDefinition = testExecution.GetColorMatcher(theDefinition.ColorMatchDefinition.Name);

            if (theDefinition.Enabled == null) throw new ArgumentException(Name + " doesn't have Enabled defined.");
            mEnabled = testExecution.DataValueRegistry.GetObject(theDefinition.Enabled.Name);

            if (theDefinition.StepSize == null) throw new ArgumentException(Name + " doesn't have StepSize defined.");
            mStepSize = testExecution.DataValueRegistry.GetObject(theDefinition.StepSize.Name);

            if (theDefinition.DetailedSearch == null) throw new ArgumentException(Name + " doesn't have DetailedSearch defined.");
            mDetailedSearch = testExecution.DataValueRegistry.GetObject(theDefinition.DetailedSearch.Name);

            mLeftBound = new GeneratedValueInstance(theDefinition.LeftBound, testExecution);
            mRightBound = new GeneratedValueInstance(theDefinition.RightBound, testExecution);
            mTopBound = new GeneratedValueInstance(theDefinition.TopBound, testExecution);
            mBottomBound = new GeneratedValueInstance(theDefinition.BottomBound, testExecution);

            if (theDefinition.ImageMarkingEnabled == null) throw new ArgumentException(Name + " doesn't have ImageMarkingEnabled defined.");
            mImageMarkingEnabled = testExecution.DataValueRegistry.GetObject(theDefinition.ImageMarkingEnabled.Name);

            if (theDefinition.ImageToMark != null)
            {
                mImageToMark = testExecution.ImageRegistry.GetObject(theDefinition.ImageToMark.Name);
            }

            //mResultantRay = new ValueBasedLineDecorationInstance(theDefinition.ResultantRay, testExecution);

            mAutoSave = theDefinition.AutoSave;
            mMarkColor = theDefinition.MarkColor;

        }
        private FindBoundingRectangleDefinition mFindBoundingRectangleDefinition;

        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private DataValueInstance mStartPoint_X;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance StartPoint_X
        {
            get { return mStartPoint_X; }
        }

        private DataValueInstance mStartPoint_Y;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance StartPoint_Y
        {
            get { return mStartPoint_Y; }
        }

        private ColorMatchInstance mColorMatchDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public ColorMatchInstance ColorMatchDefinition
        {
            get { return mColorMatchDefinition; }
        }

        private DataValueInstance mEnabled;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance Enabled
        {
            get { return mEnabled; }
        }

        private DataValueInstance mStepSize;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance StepSize
        {
            get { return mStepSize; }
        }

        private DataValueInstance mDetailedSearch;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance DetailedSearch
        {
            get { return mDetailedSearch; }
        }

        private GeneratedValueInstance mLeftBound = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance LeftBound
        {
            get { return mLeftBound; }
        }

        private GeneratedValueInstance mRightBound = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance RightBound
        {
            get { return mRightBound; }
        }

        private GeneratedValueInstance mTopBound = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance TopBound
        {
            get { return mTopBound; }
        }

        private GeneratedValueInstance mBottomBound = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance BottomBound
        {
            get { return mBottomBound; }
        }

        private DataValueInstance mImageMarkingEnabled;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public DataValueInstance ImageMarkingEnabled
        {
            get { return mImageMarkingEnabled; }
        }

        private ImageInstance mImageToMark = null;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public ImageInstance ImageToMark
        {
            get { return mImageToMark; }
        }

        private Color mMarkColor = Color.Yellow;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("The color which is used to highlight areas that match the color definition")]
        public Color MarkColor
        {
            get { return mMarkColor; }
        }

        private bool mAutoSave = false;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("")]
        public bool AutoSave
        {
            get { return mAutoSave; }
        }

        public override bool IsComplete() { return mLeftBound.IsComplete(); }

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

        private Bitmap sourceBitmap = null;
        private Bitmap markedBitmap = null;
        private BitmapData sourceBitmapData = null;
        private BitmapData markedBitmapData = null;
        private int sourceStride;
        private int sourceStrideOffset;

        public static readonly PixelFormat PIXEL_FORMAT = PixelFormat.Format32bppArgb;
        public static readonly int PIXEL_BYTE_WIDTH = 4; // determined by PixelFormat.Format32bppArgb; http://www.bobpowell.net/lockingbits.htm
        public override void DoWork() 
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);
            
            int startX = (int)mStartPoint_X.ValueAsLong();
            int startY = (int)mStartPoint_Y.ValueAsLong();

            int leftEdge = -1;
            int rightEdge = -1;
            int topEdge = -1;
            int bottomEdge = -1;

            if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met.  Skipping.");
            }
            else if (mSourceImage.Bitmap == null)
            {
                TestExecution().LogErrorWithTimeFromTrigger("source image for '" + Name + "' does not exist.");
            }
            else if (startX < 0 || startX >= mSourceImage.Bitmap.Width ||
            startY < 0 || startY >= mSourceImage.Bitmap.Height)
            {
                TestExecution().LogErrorWithTimeFromTrigger("Start position for '" + Name + "' isn't within the image bounds; start=" + startX + "," + startY + "; image size=" + mSourceImage.Bitmap.Width + "x" + mSourceImage.Bitmap.Height);
            }
            else
            {
                int stepSize = (int)mStepSize.ValueAsLong();
                bool detailedSearchAtEnd = mDetailedSearch.ValueAsBoolean();

                sourceBitmap = SourceImage.Bitmap;
                if (mImageToMark != null && mImageToMark.Bitmap != null)
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
                    sourceStride = sourceBitmapData.Stride;
                    sourceStrideOffset = sourceStride - (sourceBitmapData.Width * PIXEL_BYTE_WIDTH);

                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* sourcePointer;
                        byte* markedPointer;

                        sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                        sourcePointer += (startY * sourceStride) + (startX * PIXEL_BYTE_WIDTH); // adjust to current point

                        Color theColor = Color.FromArgb(sourcePointer[2], sourcePointer[1], sourcePointer[0]);
                        if (!mColorMatchDefinition.Matches(theColor))
                        {
                            TestExecution().LogErrorWithTimeFromTrigger(Name + " start position isn't within the match color; start=" + startX + "," + startY + "   color=" + theColor);
                        }
                        else
                        {
                            if (mFindBoundingRectangleDefinition.SearchRecord.GetLength(0) < sourceBitmap.Width || mFindBoundingRectangleDefinition.SearchRecord.GetLength(1) < sourceBitmap.Height)
                            {
                                mFindBoundingRectangleDefinition.SearchRecord = new short[sourceBitmap.Width, sourceBitmap.Height];
                                mFindBoundingRectangleDefinition.LastMarkerUsed = 0;
                            }
                            if (mFindBoundingRectangleDefinition.LastMarkerUsed == int.MaxValue)
                            {
                                for (int x = 0; x < mFindBoundingRectangleDefinition.SearchRecord.GetLength(0); x++)
                                {
                                    for (int y = 0; y < mFindBoundingRectangleDefinition.SearchRecord.GetLength(1); y++)
                                    {
                                        mFindBoundingRectangleDefinition.SearchRecord[x, y] = 0;
                                    }
                                }
                                mFindBoundingRectangleDefinition.LastMarkerUsed = 0;
                            }
                            mFindBoundingRectangleDefinition.LastMarkerUsed++;

                            EdgeSearch topEdgeSearch = new EdgeSearch(this, mColorMatchDefinition, Axis.X, startY, -1 * stepSize, 0, startX, 0, sourceBitmap.Width - 1, mFindBoundingRectangleDefinition.SearchRecord, mFindBoundingRectangleDefinition.LastMarkerUsed);
                            EdgeSearch bottomEdgeSearch = new EdgeSearch(this, mColorMatchDefinition, Axis.X, startY, +1 * stepSize, sourceBitmap.Height - 1, startX, 0, sourceBitmap.Width - 1, mFindBoundingRectangleDefinition.SearchRecord, mFindBoundingRectangleDefinition.LastMarkerUsed);
                            EdgeSearch leftEdgeSearch = new EdgeSearch(this, mColorMatchDefinition, Axis.Y, startX, -1 * stepSize, 0, startY, 0, sourceBitmap.Height - 1, mFindBoundingRectangleDefinition.SearchRecord, mFindBoundingRectangleDefinition.LastMarkerUsed);
                            EdgeSearch rightEdgeSearch = new EdgeSearch(this, mColorMatchDefinition, Axis.Y, startX, +1 * stepSize, sourceBitmap.Width - 1, startY, 0, sourceBitmap.Height - 1, mFindBoundingRectangleDefinition.SearchRecord, mFindBoundingRectangleDefinition.LastMarkerUsed);
                            topEdgeSearch.minSideEdge = leftEdgeSearch;
                            topEdgeSearch.maxSideEdge = rightEdgeSearch;
                            bottomEdgeSearch.minSideEdge = leftEdgeSearch;
                            bottomEdgeSearch.maxSideEdge = rightEdgeSearch;
                            leftEdgeSearch.minSideEdge = topEdgeSearch;
                            leftEdgeSearch.maxSideEdge = bottomEdgeSearch;
                            rightEdgeSearch.minSideEdge = topEdgeSearch;
                            rightEdgeSearch.maxSideEdge = bottomEdgeSearch;

                            while (!(topEdgeSearch.Done() && bottomEdgeSearch.Done() && leftEdgeSearch.Done() && rightEdgeSearch.Done()))
                            {
                                if (!topEdgeSearch.Done()) topEdgeSearch.TestLine();
                                if (!bottomEdgeSearch.Done()) bottomEdgeSearch.TestLine();
                                if (!leftEdgeSearch.Done()) leftEdgeSearch.TestLine();
                                if (!rightEdgeSearch.Done()) rightEdgeSearch.TestLine();
                            }

                            if (detailedSearchAtEnd)
                            {
                                //topEdgeSearch.mStep
                            }

                            leftEdge = leftEdgeSearch.lastPosWhereObjectSeen;
                            rightEdge = rightEdgeSearch.lastPosWhereObjectSeen;
                            topEdge = topEdgeSearch.lastPosWhereObjectSeen;
                            bottomEdge = bottomEdgeSearch.lastPosWhereObjectSeen;
                            /* TODO: rectangle decoration? force user to use ROI?
                            mResultantRay.SetStartX(centerX);
                            mResultantRay.SetStartY(centerY);
                            mResultantRay.SetEndX((int)(centerX + outerRadius * Math.Cos(overallRad)));
                            mResultantRay.SetEndY((int)(centerY + outerRadius * Math.Sin(overallRad)));
                            mResultantRay.SetIsComplete();
                            */
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
            mLeftBound.SetValue(leftEdge);
            mLeftBound.SetIsComplete();
            mRightBound.SetValue(rightEdge);
            mRightBound.SetIsComplete();
            mTopBound.SetValue(topEdge);
            mTopBound.SetIsComplete();
            mBottomBound.SetValue(bottomEdge);
            mBottomBound.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " found bounding rectangle; left=" + leftEdge + " right=" + rightEdge + " top=" + topEdge + " bottom=" + bottomEdge);

            if (mAutoSave)
            {
                try
                {
                    string filePath = ((FindRadialLineDefinition)Definition()).AutoSavePath;
                    mSourceImage.Save(filePath, Name, true);
                    if (mImageToMark != null) mImageToMark.Save(filePath, Name, "_marked_" + leftEdge + "_" + rightEdge + "_" + topEdge + "_" + bottomEdge);
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

        public Color GetPixelColor(int x, int y)
        {
            Color theColor;
            unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
            {
                byte* sourcePointer;
                byte* markedPointer;

                if (x < 0 || x >= sourceBitmap.Width) throw new ArgumentException("x out of range. " + x);
                if (y < 0 || y >= sourceBitmap.Height) throw new ArgumentException("y out of range. " + y);

                sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                sourcePointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point

                theColor = Color.FromArgb(sourcePointer[2], sourcePointer[1], sourcePointer[0]);

                if (markedBitmap != null)
                {
                    markedPointer = (byte*)markedBitmapData.Scan0; // init to first byte of image
                    markedPointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point
                    markedPointer[3] = mMarkColor.A;
                    markedPointer[2] = mMarkColor.R;
                    markedPointer[1] = mMarkColor.G;
                    markedPointer[0] = mMarkColor.B;
                }
            }
            return theColor;
        }
                 
    }
    public class EdgeSearch
    {
        public EdgeSearch(ImageTool theOwnerTool, ColorMatchInstance theColorMatch, Axis edgeAxis, int startEdgePos, int stepSizeAndDirection, int theSearchBoundary, int startLocationOnEdge, int theMinBoundary, int theMaxBoundary, short[,] searchRecord, short searchMarker)
        {
            ownerTool = theOwnerTool;
            colorMatch = theColorMatch;
            mEdgeAxis = edgeAxis;
            pos = startEdgePos;
            mStepSizeAndDirection = stepSizeAndDirection;
            mSearchBoundary = theSearchBoundary;
            mMinBoundary = theMinBoundary;
            mMaxBoundary = theMaxBoundary;
            minTested = startLocationOnEdge;
            maxTested = startLocationOnEdge;
            mSearchRecord = searchRecord;
            mSearchMarker = searchMarker;
            if (PixelMatches(startLocationOnEdge, true)) HandleFind(startLocationOnEdge);
        }
        private short[,] mSearchRecord;
        private short mSearchMarker;
        private int mSearchBoundary;
        private int mMinBoundary;
        private int mMaxBoundary;
        private ImageTool ownerTool;
        private ColorMatchInstance colorMatch;
        private Axis mEdgeAxis;
        public EdgeSearch minSideEdge;
        public EdgeSearch maxSideEdge;
        public EdgeSearch opposingEdge;
        public int maxSize = 0;
        public bool abort = false;
        public int pos;
        public int lastPosWhereObjectSeen = -1;
        public bool Done()
        {
            if (lastPosWhereObjectSeen == mSearchBoundary || abort) return true;
            return ((mStepSizeAndDirection > 0 && pos > lastPosWhereObjectSeen) || (mStepSizeAndDirection < 0 && pos < lastPosWhereObjectSeen)) &&
                (minTested == minSideEdge.lastPosWhereObjectSeen || minTested == mMinBoundary) &&
                (maxTested == maxSideEdge.lastPosWhereObjectSeen || maxTested == mMaxBoundary);
        }
        private int mStepSizeAndDirection;
        private int minTested;
        private int maxTested;
        public void TestLine()
        {
            // search un-searched portions of the current line...ie if the adjoining edges moved outward, we should search our edge to the new extreme(s) until we find at least 1 pixel...once we find one pixel, we push outward
            while (minTested > minSideEdge.lastPosWhereObjectSeen && minTested > mMinBoundary && lastPosWhereObjectSeen != mSearchBoundary && !abort)
            {
                minTested--;
                if (PixelMatches(minTested, false)) HandleFind(minTested);
            }
            while (maxTested < maxSideEdge.lastPosWhereObjectSeen && maxTested < mMaxBoundary && lastPosWhereObjectSeen != mSearchBoundary && !abort)
            {
                maxTested++;
                if (PixelMatches(maxTested, false)) HandleFind(maxTested);
            }
        }
        public bool PixelMatches(int location, bool initializing)
        {
            // TODO: convert this from a method to an abstract class so I can have many different matchers (ie ColorMatchDefinition, Color, Gray Value, etc)...this will be easier after switching from LockBits to EditableBitmap
            int x;
            int y;
            switch (mEdgeAxis)
            {
                case Axis.X:
                    x = location;
                    y = pos;
                    break;
                case Axis.Y:
                    x = pos;
                    y = location;
                    break;
                default:
                    throw new ArgumentException("Axis not defined alkjdlk");
                    break;
            }
            if (mSearchRecord[x, y] == mSearchMarker)
            {
                // optimization...
                // THIS LOGIC CREATED A BUG: we've already looked at this pixel and it matched...so it must belong to a previous search which terminated because the object was too big...since we ran into it, this object is too big too
                // BUG DISCOVERED 8/18/08...the previous search could have been terminated because the object was TOO SMALL...it depends how shapes of the blobs and their spacial relationship...ie what about a big blob with a small piece cut off by a mark (e.g. weld)
                // Possible solution... don't mark the pixels searched immediately, but rather create a list of searched pixels and then mark them only if the blob is too big or runs into the edge of the ROI...might require a BIG list...list needs to be 2D to store X & Y
                // easiest solution I can think of: if a blob is too small, then clear all markings within it's bounding rectangle
                abort = true;
                return false; // it must match since we've marked it previously, but we don't want to treat it as a find since we're aborting
            }
            Color theColor = ownerTool.GetPixelColor(x, y);
            bool matches = colorMatch.Matches(theColor);
            if (matches && !initializing) mSearchRecord[x, y] = mSearchMarker;
            return matches;
        }
        private void HandleFind(int foundAt)
        {
            // when we find a pixel, search outward at that location to find the extreme
            do
            {
                lastPosWhereObjectSeen = pos;
                pos += mStepSizeAndDirection;
                minTested = foundAt;
                maxTested = foundAt;
                if (mStepSizeAndDirection > 0 && pos > mSearchBoundary)
                {
                    if (lastPosWhereObjectSeen < mSearchBoundary)
                    {
                        pos = mSearchBoundary;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (mStepSizeAndDirection < 0 && pos < mSearchBoundary)
                {
                    if (lastPosWhereObjectSeen > mSearchBoundary)
                    {
                        pos = mSearchBoundary;
                    }
                    else
                    {
                        return;
                    }
                }
                if (maxSize > 0 && Math.Abs(opposingEdge.lastPosWhereObjectSeen - lastPosWhereObjectSeen) > maxSize)
                {
                    abort = true;
                    return;
                }
            } while (PixelMatches(foundAt, false));
        }
    }
    public enum Axis
    {
        NOT_DEFINED = -1,
        X,
        Y
    }
}

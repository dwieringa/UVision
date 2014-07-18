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
    class FindBlobOfSizeAndColorInstance : ToolInstance, ImageTool
    {
        public FindBlobOfSizeAndColorInstance(FindBlobOfSizeAndColorDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            mFindBlobOfSizeAndColorDefinition = theDefinition;

            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.ROI == null) throw new ArgumentException(Name + " doesn't have ROI defined.");
            ROIInstance theROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);
            if (!(theROI is IRectangleROIInstance)) throw new ArgumentException(Name + " requires a rectangle ROI. " + theROI.Name + " isn't a rectangle.");
            mROI = (IRectangleROIInstance)theROI;

            if (theDefinition.ExpectedObjectWidth != null)
            {
                mExpectedObjectWidth = testExecution.DataValueRegistry.GetObject(theDefinition.ExpectedObjectWidth.Name);
            }

            if (theDefinition.ExpectedObjectHeight != null)
            {
                mExpectedObjectHeight = testExecution.DataValueRegistry.GetObject(theDefinition.ExpectedObjectHeight.Name);
            }

            if (theDefinition.AllowedSizeVariation != null)
            {
                mAllowedSizeVariation = testExecution.DataValueRegistry.GetObject(theDefinition.AllowedSizeVariation.Name);
            }

            if (theDefinition.MinObjectHeight != null)
            {
                mMinObjectHeight = testExecution.DataValueRegistry.GetObject(theDefinition.MinObjectHeight.Name);
            }

            if (theDefinition.MinObjectWidth != null)
            {
                mMinObjectWidth = testExecution.DataValueRegistry.GetObject(theDefinition.MinObjectWidth.Name);
            }

            if (theDefinition.MaxObjectHeight != null)
            {
                mMaxObjectHeight = testExecution.DataValueRegistry.GetObject(theDefinition.MaxObjectHeight.Name);
            }

            if (theDefinition.MaxObjectWidth != null)
            {
                mMaxObjectWidth = testExecution.DataValueRegistry.GetObject(theDefinition.MaxObjectWidth.Name);
            }

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

        private FindBlobOfSizeAndColorDefinition mFindBlobOfSizeAndColorDefinition;

        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private IRectangleROIInstance mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public IRectangleROIInstance ROI
        {
            get { return mROI; }
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

        private DataValueInstance mMinObjectHeight;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance MinObjectHeight
        {
            get { return mMinObjectHeight; }
        }

        private DataValueInstance mMinObjectWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance MinObjectWidth
        {
            get { return mMinObjectWidth; }
        }

        private DataValueInstance mMaxObjectHeight;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance MaxObjectHeight
        {
            get { return mMaxObjectHeight; }
        }

        private DataValueInstance mMaxObjectWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance MaxObjectWidth
        {
            get { return mMaxObjectWidth; }
        }

        private DataValueInstance mExpectedObjectHeight;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance ObjectHeight
        {
            get { return mExpectedObjectHeight; }
        }

        private DataValueInstance mExpectedObjectWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance ObjectWidth
        {
            get { return mExpectedObjectWidth; }
        }

        private DataValueInstance mAllowedSizeVariation;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance AllowedSizeVariation
        {
            get { return mAllowedSizeVariation; }
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

            int leftEdge = -1;
            int rightEdge = -1;
            int topEdge = -1;
            int bottomEdge = -1;

            int minObjectHeight = -1;
            int maxObjectHeight = -1;
            int minObjectWidth = -1;
            int maxObjectWidth = -1;
            double allowedObjectSizeVariation = 0;
            if (mAllowedSizeVariation != null)
            {
                allowedObjectSizeVariation = mAllowedSizeVariation.ValueAsDecimal() / 100.0;
            }

            if (mExpectedObjectHeight != null)
            {
                minObjectHeight = (int)(mExpectedObjectHeight.ValueAsDecimal() * (1 - allowedObjectSizeVariation));
                maxObjectHeight = (int)(mExpectedObjectHeight.ValueAsDecimal() * (1 + allowedObjectSizeVariation));
            }
            if (mExpectedObjectWidth != null)
            {
                minObjectWidth = (int)(mExpectedObjectWidth.ValueAsDecimal() * (1 - allowedObjectSizeVariation));
                maxObjectWidth = (int)(mExpectedObjectWidth.ValueAsDecimal() * (1 + allowedObjectSizeVariation));
            }

            if (mMinObjectHeight != null) minObjectHeight = (int)mMinObjectHeight.ValueAsLong();
            if (mMinObjectWidth != null) minObjectWidth = (int)mMinObjectWidth.ValueAsLong();
            if (mMaxObjectHeight != null) maxObjectHeight = (int)mMaxObjectHeight.ValueAsLong();
            if (mMaxObjectWidth != null) maxObjectWidth = (int)mMaxObjectWidth.ValueAsLong();

            if (minObjectHeight < 0)
            {
                TestExecution().LogErrorWithTimeFromTrigger("A minimum height for the object hasn't been defined within '" + Name + "'.");
            }
            else if (maxObjectHeight < 0)
            {
                TestExecution().LogErrorWithTimeFromTrigger("A maximum height for the object hasn't been defined within '" + Name + "'.");
            }
            else if (minObjectWidth < 0)
            {
                TestExecution().LogErrorWithTimeFromTrigger("A minimum width for the object hasn't been defined within '" + Name + "'.");
            }
            else if (maxObjectWidth < 0)
            {
                TestExecution().LogErrorWithTimeFromTrigger("A maximum width for the object hasn't been defined within '" + Name + "'.");
            }
            else if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met.  Skipping.");
            }
            else if (mSourceImage.Bitmap == null)
            {
                TestExecution().LogErrorWithTimeFromTrigger("Source image for '" + Name + "' does not exist.");
            }
            else
            {
                int searchXStep = Math.Max(1, (int)(minObjectWidth * 0.3));
                int searchYStep = Math.Max(1, (int)(minObjectHeight * 0.3));
                int startX = (int)mROI.Left + searchXStep;
                int startY = (int)mROI.Top + searchYStep;

                if (startX < 0 || startX >= mSourceImage.Bitmap.Width || startY < 0 || startY >= mSourceImage.Bitmap.Height)
                {
                    TestExecution().LogErrorWithTimeFromTrigger("Start position for '" + Name + "' isn't within the image bounds; start=" + startX + "," + startY + "; image size=" + mSourceImage.Bitmap.Width + "x" + mSourceImage.Bitmap.Height);
                }
                else
                {
                    int stepSize = (int)mStepSize.ValueAsLong();
                    bool detailedSearchAtEnd = mDetailedSearch.ValueAsBoolean();

                    sourceBitmap = SourceImage.Bitmap;
                    if (mImageMarkingEnabled.ValueAsBoolean() && mImageToMark != null && mImageToMark.Bitmap != null)
                    {
                        markedBitmap = mImageToMark.Bitmap;
                    }

                    // TODO: replace LockBits implementation with array pointer

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

                            if (mFindBlobOfSizeAndColorDefinition.SearchRecord.GetLength(0) < sourceBitmap.Width || mFindBlobOfSizeAndColorDefinition.SearchRecord.GetLength(1) < sourceBitmap.Height)
                            {
                                mFindBlobOfSizeAndColorDefinition.SearchRecord = new short[sourceBitmap.Width, sourceBitmap.Height];
                                mFindBlobOfSizeAndColorDefinition.LastMarkerUsed = 0;
                            }
                            if (mFindBlobOfSizeAndColorDefinition.LastMarkerUsed == short.MaxValue)
                            {
                                short initialValue = short.MinValue + 1; // we don't use short.MinValue since that is a special case (see ClearSearchRecordArea(); before switching from int to short, we were initializing to 0 here and -1 in ClearSearchRecordArea())
                                for (int x = 0; x < mFindBlobOfSizeAndColorDefinition.SearchRecord.GetLength(0); x++)
                                {
                                    for (int y = 0; y < mFindBlobOfSizeAndColorDefinition.SearchRecord.GetLength(1); y++)
                                    {
                                        mFindBlobOfSizeAndColorDefinition.SearchRecord[x, y] = initialValue;
                                    }
                                }
                                mFindBlobOfSizeAndColorDefinition.LastMarkerUsed = 0;
                            }
                            mFindBlobOfSizeAndColorDefinition.LastMarkerUsed++;

                            for (int x = startX; x < ROI.Right && leftEdge < 0; x += searchXStep)
                            {
                                for (int y = startY; y < ROI.Bottom && leftEdge < 0; y += searchYStep)
                                {
                                    if (markedBitmap != null)
                                    {
                                        markedPointer = (byte*)markedBitmapData.Scan0; // init to first byte of image
                                        markedPointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point
                                        markedPointer[3] = Color.Lime.A;
                                        markedPointer[2] = Color.Lime.R;
                                        markedPointer[1] = Color.Lime.G;
                                        markedPointer[0] = Color.Lime.B;
                                    }

                                    bool failed = false;
                                    sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                    sourcePointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point

                                    Color theColor = Color.FromArgb(sourcePointer[2], sourcePointer[1], sourcePointer[0]);
                                    if (mColorMatchDefinition.Matches(theColor))
                                    {
                                        TestExecution().LogMessageWithTimeFromTrigger(Name + ": found match at " + x + "," + y + "; beginning search of area");

                                        EdgeSearch topEdgeSearch = new EdgeSearch(this, mColorMatchDefinition, Axis.X, y, -1 * stepSize, mROI.Top, x, mROI.Left, mROI.Right, mFindBlobOfSizeAndColorDefinition.SearchRecord, mFindBlobOfSizeAndColorDefinition.LastMarkerUsed);
                                        EdgeSearch bottomEdgeSearch = new EdgeSearch(this, mColorMatchDefinition, Axis.X, y, +1 * stepSize, mROI.Bottom, x, mROI.Left, mROI.Right, mFindBlobOfSizeAndColorDefinition.SearchRecord, mFindBlobOfSizeAndColorDefinition.LastMarkerUsed);
                                        EdgeSearch leftEdgeSearch = new EdgeSearch(this, mColorMatchDefinition, Axis.Y, x, -1 * stepSize, mROI.Left, y, mROI.Top, mROI.Bottom, mFindBlobOfSizeAndColorDefinition.SearchRecord, mFindBlobOfSizeAndColorDefinition.LastMarkerUsed);
                                        EdgeSearch rightEdgeSearch = new EdgeSearch(this, mColorMatchDefinition, Axis.Y, x, +1 * stepSize, mROI.Right, y, mROI.Top, mROI.Bottom, mFindBlobOfSizeAndColorDefinition.SearchRecord, mFindBlobOfSizeAndColorDefinition.LastMarkerUsed);
                                        topEdgeSearch.minSideEdge = leftEdgeSearch;
                                        topEdgeSearch.maxSideEdge = rightEdgeSearch;
                                        topEdgeSearch.opposingEdge = bottomEdgeSearch;
                                        bottomEdgeSearch.minSideEdge = leftEdgeSearch;
                                        bottomEdgeSearch.maxSideEdge = rightEdgeSearch;
                                        bottomEdgeSearch.opposingEdge = topEdgeSearch;
                                        leftEdgeSearch.minSideEdge = topEdgeSearch;
                                        leftEdgeSearch.maxSideEdge = bottomEdgeSearch;
                                        leftEdgeSearch.opposingEdge = rightEdgeSearch;
                                        rightEdgeSearch.minSideEdge = topEdgeSearch;
                                        rightEdgeSearch.maxSideEdge = bottomEdgeSearch;
                                        rightEdgeSearch.opposingEdge = leftEdgeSearch;
                                        topEdgeSearch.maxSize = maxObjectHeight;
                                        bottomEdgeSearch.maxSize = maxObjectHeight;
                                        leftEdgeSearch.maxSize = maxObjectWidth;
                                        rightEdgeSearch.maxSize = maxObjectWidth;

                                        do
                                        {
                                            if (!topEdgeSearch.Done()) topEdgeSearch.TestLine();
                                            if (!bottomEdgeSearch.Done()) bottomEdgeSearch.TestLine();
                                            if (!leftEdgeSearch.Done()) leftEdgeSearch.TestLine();
                                            if (!rightEdgeSearch.Done()) rightEdgeSearch.TestLine();
                                            if (bottomEdgeSearch.lastPosWhereObjectSeen - topEdgeSearch.lastPosWhereObjectSeen > maxObjectHeight)
                                            {
                                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": aborting area search because y-axis size exceeded; top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                                failed = true;
                                                break;
                                            }
                                            if (rightEdgeSearch.lastPosWhereObjectSeen - leftEdgeSearch.lastPosWhereObjectSeen > maxObjectWidth)
                                            {
                                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": aborting area search because x-axis size exceeded; top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                                failed = true;
                                                break;
                                            }
                                            if (rightEdgeSearch.lastPosWhereObjectSeen == mROI.Right)
                                            {
                                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": aborting area search because ran into right edge of ROI; top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                                failed = true;
                                                break;
                                            }
                                            if (leftEdgeSearch.lastPosWhereObjectSeen == mROI.Left)
                                            {
                                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": aborting area search because ran into left edge of ROI; top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                                failed = true;
                                                break;
                                            }
                                            if (topEdgeSearch.lastPosWhereObjectSeen == mROI.Top)
                                            {
                                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": aborting area search because ran into top edge of ROI; top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                                failed = true;
                                                break;
                                            }
                                            if (bottomEdgeSearch.lastPosWhereObjectSeen == mROI.Bottom)
                                            {
                                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": aborting area search because ran into bottom edge of ROI; top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                                failed = true;
                                                break;
                                            }
                                        } while (!(topEdgeSearch.Done() && bottomEdgeSearch.Done() && leftEdgeSearch.Done() && rightEdgeSearch.Done()));

                                        if (detailedSearchAtEnd)
                                        {
                                            //topEdgeSearch.mStep
                                            //TODO: finish
                                            //TODO: recheck if object too big
                                        }

                                        if (leftEdgeSearch.abort || rightEdgeSearch.abort || topEdgeSearch.abort || bottomEdgeSearch.abort)
                                        {
                                            TestExecution().LogMessageWithTimeFromTrigger(Name + ": aborting area search because an edge search aborted (probably ran into an already searched pixel); top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                            failed = true;
                                        }
                                        if (bottomEdgeSearch.lastPosWhereObjectSeen - topEdgeSearch.lastPosWhereObjectSeen < minObjectHeight)
                                        {
                                            TestExecution().LogMessageWithTimeFromTrigger(Name + ": excluding object since size too small on y-axis; top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                            failed = true;

                                            // if the object/blob was too small, then if we re-check one of the pixels during a future search we don't want to abort on the assumption the object must be too big
                                            // ...this issue came up in Head Rest's Weld Orrient 9/18/08...in certain cases we would first find a small blob to the left of the weld, but would abort because it was too small...it's edges would be close to, but below the search color boundary.  Then we would find the main chunk of the light, which would wrap partially around the small blob (to the right and below)....during it's not-so-smart-but-fast search it would retest a pixel of the small blob and immediately abort...mistakenly assuming it bumped into a previous "too large" blob.
                                            // ...it could bump into a previous "too small" blob because of the way to search within the entire bounding rectangle...as a simplified method of catching "U" or "Z" shaped blobs (ie ones that double back)
                                            ClearSearchRecordArea(leftEdgeSearch.lastPosWhereObjectSeen, rightEdgeSearch.lastPosWhereObjectSeen, topEdgeSearch.lastPosWhereObjectSeen, bottomEdgeSearch.lastPosWhereObjectSeen);
                                        }
                                        else if (rightEdgeSearch.lastPosWhereObjectSeen - leftEdgeSearch.lastPosWhereObjectSeen < minObjectWidth)
                                        {
                                            TestExecution().LogMessageWithTimeFromTrigger(Name + ": excluding object since size too small on x-axis; top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                            failed = true;

                                            // if the object/blob was too small, then if we re-check one of the pixels during a future search we don't want to abort on the assumption the object must be too big
                                            // ...this issue came up in Head Rest's Weld Orrient 9/18/08...in certain cases we would first find a small blob to the left of the weld, but would abort because it was too small...it's edges would be close to, but below the search color boundary.  Then we would find the main chunk of the light, which would wrap partially around the small blob (to the right and below)....during it's not-so-smart-but-fast search it would retest a pixel of the small blob and immediately abort...mistakenly assuming it bumped into a previous "too large" blob.
                                            // ...it could bump into a previous "too small" blob because of the way to search within the entire bounding rectangle...as a simplified method of catching "U" or "Z" shaped blobs (ie ones that double back)
                                            ClearSearchRecordArea(leftEdgeSearch.lastPosWhereObjectSeen, rightEdgeSearch.lastPosWhereObjectSeen, topEdgeSearch.lastPosWhereObjectSeen, bottomEdgeSearch.lastPosWhereObjectSeen);
                                        }
                                        if (!failed)
                                        {
                                            TestExecution().LogMessageWithTimeFromTrigger(Name + ": selected object bounded by: top=" + topEdgeSearch.lastPosWhereObjectSeen + " bottom=" + bottomEdgeSearch.lastPosWhereObjectSeen + " left=" + leftEdgeSearch.lastPosWhereObjectSeen + " right=" + rightEdgeSearch.lastPosWhereObjectSeen);
                                            leftEdge = leftEdgeSearch.lastPosWhereObjectSeen;
                                            rightEdge = rightEdgeSearch.lastPosWhereObjectSeen;
                                            topEdge = topEdgeSearch.lastPosWhereObjectSeen;
                                            bottomEdge = bottomEdgeSearch.lastPosWhereObjectSeen;
                                        }
                                    }
                                }
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
            if (leftEdge < 0)
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + " FAILED TO FIND BLOB");
            }

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

        public void ClearSearchRecordArea(int leftEdge, int rightEdge, int topEdge, int bottomEdge)
        {
            if (leftEdge < 0 || rightEdge < 0 || topEdge < 0 || bottomEdge < 0) return;
            for (int x = leftEdge; x <= rightEdge; x++)
            {
                for (int y = topEdge; y <= bottomEdge; y++)
                {
                    mFindBlobOfSizeAndColorDefinition.SearchRecord[x, y] = short.MinValue;
                }
            }
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
}

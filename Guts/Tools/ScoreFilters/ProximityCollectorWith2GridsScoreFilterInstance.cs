// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace NetCams
{
    public class ProximityCollectorWith2GridsScoreFilterInstance : ScoreFilterInstance
    {
        public ProximityCollectorWith2GridsScoreFilterInstance(ProximityCollectorWith2GridsScoreFilterDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            if (theDefinition.GridCellHeight == null) throw new ArgumentException("Score Filter tool '" + theDefinition.Name + "' doesn't have a value assigned to GridHeight");
            mGridCellHeight = testExecution.DataValueRegistry.GetObject(theDefinition.GridCellHeight.Name);

            if (theDefinition.GridCellWidth == null) throw new ArgumentException("Score Filter tool '" + theDefinition.Name + "' doesn't have a value assigned to GridWidth");
            mGridCellWidth = testExecution.DataValueRegistry.GetObject(theDefinition.GridCellWidth.Name);

            if (theDefinition.MinNumScoresRequired == null) throw new ArgumentException("Score Filter tool '" + theDefinition.Name + "' doesn't have a value assigned to MinNumScoresRequired");
            mMinNumScoresRequired = testExecution.DataValueRegistry.GetObject(theDefinition.MinNumScoresRequired.Name);

            if (theDefinition.MinScoreThreshold == null) throw new ArgumentException("Score Filter tool '" + theDefinition.Name + "' doesn't have a value assigned to MinScoreThreshold");
            mMinScoreThreshold = testExecution.DataValueRegistry.GetObject(theDefinition.MinScoreThreshold.Name);

            mMaxScore = new GeneratedValueInstance(theDefinition.MaxScore, testExecution);
            mNumScoresInMax = new GeneratedValueInstance(theDefinition.NumScoresInMax, testExecution);

            mMaxDebugDetails = theDefinition.MaxDebugDetails;
        }

        private int mMaxDebugDetails;
        private int mDebugDetailsCount = 0;

        private int mGrid1StartingX;
        private int mGrid1StartingY;
        private int mGrid2StartingX;
        private int mGrid2StartingY;
        private int mNumCols1;
        private int mNumCols2;
        private int mNumRows1;
        private int mNumRows2;
        private long[,] mGrid1; // TODO: consider moving this to Def object...less mem usage...less mem allocation/garbage collection.  Do we need all this data for review?  log scorings in string in case we want to save for analysis?
        private long[,] mGrid2;
        private int[,] mGrid1_counter;
        private int[,] mGrid2_counter;

        private long mMaxScoreAsLong = 0;
        private long mMaxScoringCell_x = -1;
        private long mMaxScoringCell_y = -1;

        public override long Score
        {
            get { return mMaxScore.ValueAsLong(); }
        }

        public override void SetImageSize(int width, int height)
        {
            if (mGrid1 == null)
            {
                mImageWidth = width;
                mImageHeight = height;

                mNumCols1 = (int)(mImageWidth / mGridCellWidth.ValueAsLong());
                mNumCols2 = mNumCols1 - 1;
                mNumRows1 = (int)(mImageHeight / mGridCellHeight.ValueAsLong());
                mNumRows2 = mNumRows1 - 1;
                mGrid1 = new long[mNumCols1, mNumRows1];
                mGrid2 = new long[mNumCols2, mNumRows2];
                mGrid1_counter = new int[mNumCols1, mNumRows1];
                mGrid2_counter = new int[mNumCols2, mNumRows2];

                mGrid1StartingX = (int)((mImageWidth - (mNumCols1 * mGridCellWidth.ValueAsLong())) / 2);
                mGrid1StartingY = (int)((mImageHeight - (mNumRows1 * mGridCellHeight.ValueAsLong())) / 2);

                mGrid2StartingX = (int)((mImageWidth - (mNumCols2 * mGridCellWidth.ValueAsLong())) / 2);
                mGrid2StartingY = (int)((mImageHeight - (mNumRows2 * mGridCellHeight.ValueAsLong())) / 2);
            }
            else
            {
                if (width != mImageWidth || height != mImageHeight)
                {
                    throw new ArgumentException("Image size for Score Filter '" + Name + "' already set to " + mImageWidth + "x" + mImageHeight + ".  New requested size is " + width + "x" + height + ".");
                }
            }

        }

        public override void ProcessScore(int x, int y, long score)
        {
            if (x < 0 || x >= mImageWidth || y < 0 || y >= mImageHeight)
            {
                throw new ArgumentException("Invalid coordinates passed into to Score Filter '" + Name + "'. x=" + x + " y=" + y + "  Image size=" + mImageWidth + "x" + mImageHeight);
            }
            if (mGrid1 == null)
            {
                throw new ArgumentException("Image size wasn't set for Score Filter '" + Name + "'. Check programming of analysis tool.  Missed call?  Missed dependency?");
            }

            if (score >= mMinScoreThreshold.ValueAsLong())
            {
                int gridCol = (int)((x - mGrid1StartingX) / mGridCellWidth.ValueAsLong());
                int gridRow = (int)((y - mGrid1StartingY) / mGridCellHeight.ValueAsLong());

                mGrid1[gridCol, gridRow] += score;
                mGrid1_counter[gridCol, gridRow] += 1;

                if (mDebugDetailsCount <= mMaxDebugDetails)
                {
                    TestExecution().LogMessageWithTimeFromTrigger("Score Filter '" + Name + "' adding score of " + score + " at " + x + "," + y + " to grid 1 cell " + gridCol + "," + gridRow + ".  Cell score=" + mGrid1[gridCol, gridRow] + "  count=" + mGrid1_counter[gridCol, gridRow]);
                    ++mDebugDetailsCount;
                }

                if (mGrid1[gridCol, gridRow] >= mMaxScoreAsLong && mGrid1_counter[gridCol, gridRow] >= mMinNumScoresRequired.ValueAsLong())
                {
                    if (mGrid1[gridCol, gridRow] == mMaxScoreAsLong)
                    {
                        if (mGrid1_counter[gridCol, gridRow] > mNumScoresInMax.ValueAsLong())
                        {
                            mNumScoresInMax.SetValue(mGrid1_counter[gridCol, gridRow]);
                        }
                    }
                    else
                    {
                        mMaxScoreAsLong = mGrid1[gridCol, gridRow];
                        mMaxScore.SetValue(mMaxScoreAsLong);
                        mNumScoresInMax.SetValue(mGrid1_counter[gridCol, gridRow]);
                        mMaxScoringCell_x = (gridCol * mGridCellWidth.ValueAsLong()) + mGrid1StartingX;
                        mMaxScoringCell_y = (gridRow * mGridCellHeight.ValueAsLong()) + mGrid1StartingY;
                    }
                }
                
                gridCol = (int)((x - mGrid2StartingX) / mGridCellWidth.ValueAsLong());
                gridRow = (int)((y - mGrid2StartingY) / mGridCellHeight.ValueAsLong());

                mGrid2[gridCol, gridRow] += score;
                mGrid2_counter[gridCol, gridRow] += 1;

                if (mDebugDetailsCount <= mMaxDebugDetails)
                {
                    TestExecution().LogMessageWithTimeFromTrigger("Score Filter '" + Name + "' adding score of " + score + " at " + x + "," + y + " to grid 2 cell " + gridCol + "," + gridRow + ".  Cell score=" + mGrid2[gridCol, gridRow] + "  count=" + mGrid2_counter[gridCol, gridRow]);
                    ++mDebugDetailsCount;
                }

                if (mGrid2[gridCol, gridRow] >= mMaxScoreAsLong && mGrid2_counter[gridCol, gridRow] >= mMinNumScoresRequired.ValueAsLong())
                {
                    if (mGrid2[gridCol, gridRow] == mMaxScoreAsLong)
                    {
                        if (mGrid2_counter[gridCol, gridRow] > mNumScoresInMax.ValueAsLong())
                        {
                            mNumScoresInMax.SetValue(mGrid2_counter[gridCol, gridRow]);
                        }
                    }
                    else
                    {
                        mMaxScoreAsLong = mGrid2[gridCol, gridRow];
                        mMaxScore.SetValue(mMaxScoreAsLong);
                        mNumScoresInMax.SetValue(mGrid2_counter[gridCol, gridRow]);
                        mMaxScoringCell_x = (gridCol * mGridCellWidth.ValueAsLong()) + mGrid2StartingX;
                        mMaxScoringCell_y = (gridRow * mGridCellHeight.ValueAsLong()) + mGrid2StartingY;
                    }
                }
            }
        }

        public override void MarkImage(Bitmap imageToMark, Color markColor)
        {
            int pixelByteWidth = 4;  // assume we are doing this with 4 byte images because they will be color (we are marking in color)
            long cellWidth = mGridCellWidth.ValueAsLong();
            long cellHeight = mGridCellHeight.ValueAsLong();
            if (Score > 0)
            {
                if (mMaxScoringCell_x + cellWidth >= imageToMark.Width) throw new ArgumentException(Name + " is unable to mark the supplied image.  Size mismatch. 238947");
                if (mMaxScoringCell_y + cellHeight >= imageToMark.Height) throw new ArgumentException(Name + " is unable to mark the supplied image.  Size mismatch. 238937");
                BitmapData imageBitmapData = imageToMark.LockBits(new Rectangle(0, 0, imageToMark.Width, imageToMark.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                try
                {
                    int stride = imageBitmapData.Stride;
                    unsafe
                    {
                        byte* imagePointer1;
                        byte* imagePointer2;
                        imagePointer1 = (byte*)imageBitmapData.Scan0;
                        imagePointer1 += (mMaxScoringCell_y * stride) + (mMaxScoringCell_x * pixelByteWidth);
                        for (int index = 0; index < cellWidth; index++)
                        {
                            imagePointer1 += pixelByteWidth;
                            imagePointer1[3] = markColor.A;
                            imagePointer1[2] = markColor.R;
                            imagePointer1[1] = markColor.G;
                            imagePointer1[0] = markColor.B;
                            imagePointer2 = imagePointer1 + (cellHeight * stride);
                            imagePointer2[3] = markColor.A;
                            imagePointer2[2] = markColor.R;
                            imagePointer2[1] = markColor.G;
                            imagePointer2[0] = markColor.B;
                        }
                        imagePointer1 = (byte*)imageBitmapData.Scan0;
                        imagePointer1 += (mMaxScoringCell_y * stride) + (mMaxScoringCell_x * pixelByteWidth);
                        for (int index = 0; index < cellHeight; index++)
                        {
                            imagePointer1 += stride;
                            imagePointer1[3] = markColor.A;
                            imagePointer1[2] = markColor.R;
                            imagePointer1[1] = markColor.G;
                            imagePointer1[0] = markColor.B;
                            imagePointer2 = imagePointer1 + (cellWidth * pixelByteWidth);
                            imagePointer2[3] = markColor.A;
                            imagePointer2[2] = markColor.R;
                            imagePointer2[1] = markColor.G;
                            imagePointer2[0] = markColor.B;
                        }
                    }
                }
                finally
                {
                    imageToMark.UnlockBits(imageBitmapData);
                }
            }
        }

        public override void DoWork()
        {
            // TODO: create sorted list of grid cells with score > 0...log to debug?
            if (IsComplete()) throw new ArgumentException("here again!");
            mMaxScore.SetIsComplete();
            mNumScoresInMax.SetIsComplete();
            string msg = "Proximity Collector finished.  Max Cell Score=" + mMaxScore.ValueAsLong() + "  Number of scores in cell=" + mNumScoresInMax.ValueAsLong();
            if (mMaxScoringCell_x >= 0)
            {
                msg += "  Cell boundaries: X: " + mMaxScoringCell_x + " to " + (mMaxScoringCell_x + mGridCellWidth.ValueAsLong()) + "  Y: " + mMaxScoringCell_y + " to " + (mMaxScoringCell_y + mGridCellHeight.ValueAsLong());
            }
            TestExecution().LogMessageWithTimeFromTrigger(msg);
            TestExecution().LogSummaryMessage(msg);
            
        }

        public override bool IsComplete()
        {
            return mMaxScore.IsComplete() && mNumScoresInMax.IsComplete();
        }

        private GeneratedValueInstance mMaxScore = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance MaxScore
        {
            get { return mMaxScore; }
        }

        private GeneratedValueInstance mNumScoresInMax = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance NumScoresInMax
        {
            get { return mNumScoresInMax; }
        }

        private DataValueInstance mGridCellWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Grid width in pixels.")]
        public DataValueInstance GridWidth
        {
            get { return mGridCellWidth; }
        }

        private DataValueInstance mGridCellHeight;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Grid height in pixels.")]
        public DataValueInstance GridHeight
        {
            get { return mGridCellHeight; }
        }

        private DataValueInstance mMinNumScoresRequired;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The minimum number of scores within a grid cell before that cell's score can register as the MaxScore.")]
        public DataValueInstance MinNumScoresRequired
        {
            get { return mMinNumScoresRequired; }
        }

        private DataValueInstance mMinScoreThreshold;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("A score must exceed this value before it is used.")]
        public DataValueInstance MinScoreThreshold
        {
            get { return mMinScoreThreshold; }
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NetCams
{
    class FindTransitionInstance : ToolInstance
    {
        public FindTransitionInstance(FindTransitionDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.SearchArea == null) throw new ArgumentException(Name + " doesn't have SearchArea defined.");
            ROIInstance theROI = testExecution.ROIRegistry.GetObject(theDefinition.SearchArea.Name);
            if( !(theROI is IRectangleROIInstance )) throw new ArgumentException(Name + " requires a rectangle ROI for its SearchArea. " + theROI.Name + " isn't a rectangle.");
            mSearchArea = (IRectangleROIInstance)theROI;

            if (theDefinition.SearchDirection == Direction.NotDefined) throw new ArgumentException(Name + " doesn't have SearchDirection defined.");
            mSearchDirection = theDefinition.SearchDirection;

            //if (theDefinition.TransitionDetectionMode == FindTransitionDefinition.TransitionDetectionModes.NotDefined) throw new ArgumentException(Name + " doesn't have TransitionDetectionMode defined.");
            //mTransitionDetectionMode = theDefinition.TransitionDetectionMode;

            mTransitionTypeSelectionFilter = theDefinition.TransitionTypeSelectionFilter;

            if (theDefinition.TransitionThreshold_Min == null) throw new ArgumentException(Name + " doesn't have SurfaceNoiseThreshold_Min defined.");
            mSurfaceNoiseThreshold_Min = testExecution.DataValueRegistry.GetObject(theDefinition.TransitionThreshold_Min.Name);

            //if (theDefinition.SurfaceNoiseThreshold_PercentOfSharpestTransition == null) throw new ArgumentException(Name + " doesn't have SurfaceNoiseThreshold_PercentOfSharpestTransition defined.");
            //mSurfaceNoiseThreshold_PercentOfSharpestTransition = testExecution.DataValueRegistry.GetObject(theDefinition.SurfaceNoiseThreshold_PercentOfSharpestTransition.Name);

            mTransitionMarker = new ValueBasedLineDecorationInstance(theDefinition.TransitionMarker, testExecution);

            mTransitionLocation = new GeneratedValueInstance(theDefinition.TransitionLocation, testExecution);

            if (theDefinition.TransitionScore != null)
            {
                mTransitionScore = new GeneratedValueInstance(theDefinition.TransitionScore, testExecution);
            }

            mAutoSave = theDefinition.AutoSave;
            mVerboseOutput = theDefinition.VerboseOutput;
        }
        private bool mVerboseOutput;
        private bool mAutoSave;

        private ValueBasedLineDecorationInstance mTransitionMarker = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public ValueBasedLineDecorationInstance TransitionMarker
        {
            get { return mTransitionMarker; }
        }

        private GeneratedValueInstance mTransitionLocation = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public GeneratedValueInstance TransitionLocation
        {
            get { return mTransitionLocation; }
        }

        private GeneratedValueInstance mTransitionScore = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance TransitionScore
        {
            get { return mTransitionScore; }
        }

        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private IRectangleROIInstance mSearchArea;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public IRectangleROIInstance SearchArea
        {
            get { return mSearchArea; }
        }

        private Direction mSearchDirection = Direction.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The direction to search along")]
        public Direction SearchDirection
        {
            get { return mSearchDirection; }
        }

        /*
        private FindTransitionDefinition.TransitionDetectionModes mTransitionDetectionMode = FindTransitionDefinition.TransitionDetectionModes.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The algorithm to detect the transition.")]
        public FindTransitionDefinition.TransitionDetectionModes TransitionDetectionMode
        {
            get { return mTransitionDetectionMode; }
        }
        */

        private FindTransitionDefinition.TransitionTypeSelectionFilters mTransitionTypeSelectionFilter = FindTransitionDefinition.TransitionTypeSelectionFilters.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public FindTransitionDefinition.TransitionTypeSelectionFilters TransitionTypeSelectionFilter
        {
            get { return mTransitionTypeSelectionFilter; }
        }

        private DataValueInstance mSurfaceNoiseThreshold_Min;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Intensity variation to ignore")]
        public DataValueInstance SurfaceNoiseThreshold
        {
            get { return mSurfaceNoiseThreshold_Min; }
        }

        /*
        private DataValueInstance mSurfaceNoiseThreshold_PercentOfSharpestTransition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance SurfaceNoiseThreshold_PercentOfSharpestTransition
        {
            get { return mSurfaceNoiseThreshold_PercentOfSharpestTransition; }
        }
        */

        private DataValueInstance mMinSurfaceSize;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The minimum number of pixels a surface can be (otherwise it is considered part of the transition).")]
        public DataValueInstance MinSurfaceSize
        {
            get { return mMinSurfaceSize; }
        }

        public override bool IsComplete() { return mTransitionLocation.IsComplete(); }

        private bool abort = false;
        private int x;
        private int y;
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

        private BitmapData sourceBitmapData = null;
        private int sourceStride;
        private int sourceStrideOffset;
        private int startXOffset;
        private int startYOffset;
        private int searchDirOffset;
        private int searchDirIncrement;
        private int numSearchDirRows;
        private int searchDirXIncrement;
        private int searchDirYIncrement;
        private int rowWidth;
        private int rowDirXIncrement;
        private int rowDirYIncrement;

        public static readonly PixelFormat PIXEL_FORMAT = PixelFormat.Format32bppArgb;
        public static readonly int PIXEL_BYTE_WIDTH = 4; // determined by PixelFormat.Format32bppArgb; http://www.bobpowell.net/lockingbits.htm
        public override void DoWork() 
		{
            TestExecution().LogMessageWithTimeFromTrigger(Name + " started");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int transitionLocation = -1;
            double transitionScore = -1;

            if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met. Skipping.");
            }
            else
            {
                switch (mSearchDirection)
                {
                    case Direction.Down:
                        startXOffset = mSearchArea.Left;
                        startYOffset = mSearchArea.Top;
                        numSearchDirRows = mSearchArea.Bottom - mSearchArea.Top + 1;
                        searchDirXIncrement = 0;
                        searchDirYIncrement = 1;
                        rowWidth = mSearchArea.Right - mSearchArea.Left + 1;
                        rowDirXIncrement = 1;
                        rowDirYIncrement = 0;
                        searchDirOffset = startYOffset;
                        searchDirIncrement = 1;
                        break;
                    case Direction.Up:
                        startXOffset = mSearchArea.Left;
                        startYOffset = mSearchArea.Bottom;
                        numSearchDirRows = mSearchArea.Bottom - mSearchArea.Top + 1;
                        searchDirXIncrement = 0;
                        searchDirYIncrement = -1;
                        rowWidth = mSearchArea.Right - mSearchArea.Left + 1;
                        rowDirXIncrement = 1;
                        rowDirYIncrement = 0;
                        searchDirOffset = startYOffset;
                        searchDirIncrement = -1;
                        break;
                    case Direction.Right:
                        startXOffset = mSearchArea.Left;
                        startYOffset = mSearchArea.Top;
                        numSearchDirRows = mSearchArea.Right - mSearchArea.Left + 1;
                        searchDirXIncrement = 1;
                        searchDirYIncrement = 0;
                        rowWidth = mSearchArea.Bottom - mSearchArea.Top + 1;
                        rowDirXIncrement = 0;
                        rowDirYIncrement = 1;
                        searchDirOffset = startXOffset;
                        searchDirIncrement = 1;
                        break;
                    case Direction.Left:
                        startXOffset = mSearchArea.Right;
                        startYOffset = mSearchArea.Top;
                        numSearchDirRows = mSearchArea.Right - mSearchArea.Left + 1;
                        searchDirXIncrement = -1;
                        searchDirYIncrement = 0;
                        rowWidth = mSearchArea.Bottom - mSearchArea.Top + 1;
                        rowDirXIncrement = 0;
                        rowDirYIncrement = 1;
                        searchDirOffset = startXOffset;
                        searchDirIncrement = -1;
                        break;
                    case Direction.NotDefined:
                        throw new ArgumentException("Search direction not defined for " + Name);
                    default:
                        throw new ArgumentException("Unexpected search direction for " + Name);
                }

                if (SourceImage.Bitmap == null)
                {
                    TestExecution().LogMessage("ERROR: source image for '" + Name + "' does not exist.");
                }
                //else if (mSurfaceNoiseThreshold_PercentOfSharpestTransition.ValueAsDecimal() < 0)
                //{
                //    TestExecution().LogMessage("ERROR: SurfaceNoiseThreshold_PercentOfSharpestTransition for '" + Name + "' can not be negative.");
                //}
                else if (mSearchArea.Left < 0 || mSearchArea.Right >= mSourceImage.Width || mSearchArea.Left > mSearchArea.Right || mSearchArea.Top < 0 || mSearchArea.Bottom >= mSourceImage.Height || mSearchArea.Top > mSearchArea.Bottom)
                {
                    TestExecution().LogMessage("ERROR: Invalid SearchArea for '" + Name + "'.");
                }
                else
                {
                    int surfaceNoiseThreshold = (int)mSurfaceNoiseThreshold_Min.ValueAsLong();

                    int x = 0;
                    int y = 0;
                    double sumIntensityWithinRow = 0;

                    RowAnalysis[] rows = new RowAnalysis[numSearchDirRows];
                    bool analyzedSuccessfully = false;

                    if (true)
                    {
                        try
                        {
                            RowAnalysis rowAnalysis;
                            RowAnalysis prevRow = null;
                            for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                            {
                                rowAnalysis = new RowAnalysis(searchDirIndex);
                                rows[searchDirIndex] = rowAnalysis;
                                sumIntensityWithinRow = 0;
                                for (int widthIndex = 0; widthIndex < rowWidth; widthIndex++)
                                {
                                    x = startXOffset + (searchDirIndex * searchDirXIncrement) + (widthIndex * rowDirXIncrement);
                                    y = startYOffset + (searchDirIndex * searchDirYIncrement) + (widthIndex * rowDirYIncrement);

                                    pixelGrayValue = mSourceImage.GetGrayValue(x, y);

                                    sumIntensityWithinRow += pixelGrayValue;
                                }
                                rowAnalysis.AverageIntensity = sumIntensityWithinRow / rowWidth;
                                if (prevRow != null)
                                {
                                    rowAnalysis.VariationFromPrevRow = rowAnalysis.AverageIntensity - prevRow.AverageIntensity;
                                }
                                prevRow = rowAnalysis;
                            }

                            // average neighboring rows together to supress noise and highlight multi-row transitions
                            RowAnalysis currentRow;
                            double largestVariation = 0;
                            int rowWithLargestVariation = -1;
                            for (int searchDirIndex = 1; searchDirIndex < numSearchDirRows - 1; searchDirIndex++)
                            {
                                currentRow = rows[searchDirIndex];
                                currentRow.AverageVariation_3Row = (currentRow.VariationFromPrevRow + rows[searchDirIndex + 1].VariationFromPrevRow + rows[searchDirIndex - 1].VariationFromPrevRow) / 3;
                                double abs_AvgVar_3Row = Math.Abs(currentRow.AverageVariation_3Row);
                                if ((mTransitionTypeSelectionFilter == FindTransitionDefinition.TransitionTypeSelectionFilters.NotDefined ||
                                    (mTransitionTypeSelectionFilter == FindTransitionDefinition.TransitionTypeSelectionFilters.DarkToBright && currentRow.AverageVariation_3Row > 0) ||
                                    (mTransitionTypeSelectionFilter == FindTransitionDefinition.TransitionTypeSelectionFilters.BrightToDark && currentRow.AverageVariation_3Row < 0)
                                    ) &&
                                    abs_AvgVar_3Row > surfaceNoiseThreshold &&
                                    abs_AvgVar_3Row > largestVariation)// &&
                                //(!ensureStraightTransition || TransitionIsStraight(currentRow)))
                                {
                                    largestVariation = abs_AvgVar_3Row;
                                    rowWithLargestVariation = searchDirIndex;
                                }
                            }

                            // Compute threshold to weed out noise (TODO: train this over a series of tests in "learn" mode? we need a way to determine if no transition present)
                            // option1: use 60% of biggest variation
                            // option2: run transitions thru a value group and weed out bottom chunk
                            //surfaceNoiseThreshold = Math.Max(surfaceNoiseThreshold, (int)(largestVariation * (mSurfaceNoiseThreshold_PercentOfSharpestTransition.ValueAsDecimal() / 100.0)));
                            //TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] surfaceNoiseThreshold=" + surfaceNoiseThreshold + "    (largestVariation=" + largestVariation + ")");
                            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] finished analyzing pixels");

                            if (rowWithLargestVariation >= 0)
                            {
                                transitionLocation = searchDirOffset + (rowWithLargestVariation * searchDirIncrement);
                                transitionScore = rows[rowWithLargestVariation].AverageVariation_3Row; // referencing array vs largestVariation since largestVariation is Math.Abs()
                            }
                            analyzedSuccessfully = true;
                        }
                        catch (Exception e)
                        {
                            TestExecution().LogMessageWithTimeFromTrigger("ERROR: Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
                        }
                    }
                    else // use old pointer-based pixel access instead...
                    {
                        Bitmap sourceBitmap = SourceImage.Bitmap;
                        try
                        {
                            sourceBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PIXEL_FORMAT);
                            sourceStride = sourceBitmapData.Stride;
                            sourceStrideOffset = sourceStride - (sourceBitmapData.Width * PIXEL_BYTE_WIDTH);

                            unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                            {
                                byte* sourcePointer;
                                RowAnalysis rowAnalysis;
                                RowAnalysis prevRow = null;
                                for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                                {
                                    rowAnalysis = new RowAnalysis(searchDirIndex);
                                    rows[searchDirIndex] = rowAnalysis;
                                    sumIntensityWithinRow = 0;
                                    for (int widthIndex = 0; widthIndex < rowWidth; widthIndex++)
                                    {
                                        x = startXOffset + (searchDirIndex * searchDirXIncrement) + (widthIndex * rowDirXIncrement);
                                        y = startYOffset + (searchDirIndex * searchDirYIncrement) + (widthIndex * rowDirYIncrement);

                                        sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                        sourcePointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point
                                        pixelGrayValue = (int)(0.3 * sourcePointer[2] + 0.59 * sourcePointer[1] + 0.11 * sourcePointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                        // http://www.bobpowell.net/grayscale.htm
                                        // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1

                                        sumIntensityWithinRow += pixelGrayValue;
                                    }
                                    rowAnalysis.AverageIntensity = sumIntensityWithinRow / rowWidth;
                                    if (prevRow != null)
                                    {
                                        rowAnalysis.VariationFromPrevRow = rowAnalysis.AverageIntensity - prevRow.AverageIntensity;
                                    }
                                    prevRow = rowAnalysis;
                                }

                                // average neighboring rows together to supress noise and highlight multi-row transitions
                                RowAnalysis currentRow;
                                double largestVariation = 0;
                                int rowWithLargestVariation = -1;
                                for (int searchDirIndex = 1; searchDirIndex < numSearchDirRows - 1; searchDirIndex++)
                                {
                                    currentRow = rows[searchDirIndex];
                                    currentRow.AverageVariation_3Row = (currentRow.VariationFromPrevRow + rows[searchDirIndex + 1].VariationFromPrevRow + rows[searchDirIndex - 1].VariationFromPrevRow) / 3;
                                    double abs_AvgVar_3Row = Math.Abs(currentRow.AverageVariation_3Row);
                                    if ((mTransitionTypeSelectionFilter == FindTransitionDefinition.TransitionTypeSelectionFilters.NotDefined ||
                                        (mTransitionTypeSelectionFilter == FindTransitionDefinition.TransitionTypeSelectionFilters.DarkToBright && currentRow.AverageVariation_3Row > 0) ||
                                        (mTransitionTypeSelectionFilter == FindTransitionDefinition.TransitionTypeSelectionFilters.BrightToDark && currentRow.AverageVariation_3Row < 0)
                                        ) &&
                                        abs_AvgVar_3Row > surfaceNoiseThreshold &&
                                        abs_AvgVar_3Row > largestVariation)// &&
                                    //(!ensureStraightTransition || TransitionIsStraight(currentRow)))
                                    {
                                        largestVariation = abs_AvgVar_3Row;
                                        rowWithLargestVariation = searchDirIndex;
                                    }
                                }

                                // Compute threshold to weed out noise (TODO: train this over a series of tests in "learn" mode? we need a way to determine if no transition present)
                                // option1: use 60% of biggest variation
                                // option2: run transitions thru a value group and weed out bottom chunk
                                //surfaceNoiseThreshold = Math.Max(surfaceNoiseThreshold, (int)(largestVariation * (mSurfaceNoiseThreshold_PercentOfSharpestTransition.ValueAsDecimal() / 100.0)));
                                //TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] surfaceNoiseThreshold=" + surfaceNoiseThreshold + "    (largestVariation=" + largestVariation + ")");
                                TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] finished analyzing pixels");

                                if (rowWithLargestVariation >= 0)
                                {
                                    transitionLocation = searchDirOffset + (rowWithLargestVariation * searchDirIncrement);
                                    transitionScore = rows[rowWithLargestVariation].AverageVariation_3Row; // referencing array vs largestVariation since largestVariation is Math.Abs()
                                }
                            } // end unsafe block
                            analyzedSuccessfully = true;
                        }
                        catch (Exception e)
                        {
                            TestExecution().LogMessageWithTimeFromTrigger("ERROR: Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
                        }
                        finally
                        {
                            if (sourceBitmapData != null) sourceBitmap.UnlockBits(sourceBitmapData);
                        }
                    } // end pixel access method selection

                    if( analyzedSuccessfully)
                    {
                        if (transitionLocation >= 0)
                        {
                            if (mSearchDirection == Direction.Left || mSearchDirection == Direction.Right)
                            {
                                mTransitionMarker.SetStartX(transitionLocation);
                                mTransitionMarker.SetEndX(transitionLocation);
                                mTransitionMarker.SetStartY(mSearchArea.Top);
                                mTransitionMarker.SetEndY(mSearchArea.Bottom);
                            }
                            else
                            {
                                mTransitionMarker.SetStartY(transitionLocation);
                                mTransitionMarker.SetEndY(transitionLocation);
                                mTransitionMarker.SetStartX(mSearchArea.Left);
                                mTransitionMarker.SetEndX(mSearchArea.Right);
                            }
                            mTransitionMarker.SetIsComplete();
                        }

                        if (mVerboseOutput)
                        {
                            for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                            {
                                RowAnalysis row = rows[searchDirIndex];
                                TestExecution().LogMessage(
                                    String.Format("{0,3:0} pos={1,4:0} avgI={2,8:0.00} rowV={3,8:0.00} stdDevV={4,8:0.00} vScore={5,8:0.00} 3RowAvgV={6,8:0.00}",
                                    row.SearchIndex,
                                    (searchDirOffset + (searchDirIncrement * row.SearchIndex)),
                                    row.AverageIntensity,
                                    row.VariationFromPrevRow,
                                    row.StdDevOfVariation,
                                    row.VariationScore,
                                    //                            row.AverageVariation_2Row,
                                    row.AverageVariation_3Row
                                    //                            row.OverallAverageIntensityBeforeRow,
                                    //                            row.OverallVariationBeforeRow
                                    ));
                            }
                        }
                    }
                } // end main block ("else" after all initial setup error checks)
            }
            mTransitionLocation.SetValue(transitionLocation);
            mTransitionLocation.SetIsComplete();
            if (mTransitionScore != null)
            {
                mTransitionScore.SetValue(transitionScore);
                mTransitionScore.SetIsComplete();
            }

            TestExecution().LogMessageWithTimeFromTrigger(Name + " computed transition at " + transitionLocation + " with a score of " + transitionScore);

            if (mAutoSave)
            {
                try
                {
                    string filePath = ((FindTransitionDefinition)Definition()).AutoSavePath;
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

            watch.Stop();
            TestExecution().LogMessageWithTimeFromTrigger(Name + " took " + watch.ElapsedMilliseconds + "ms  (" + watch.ElapsedTicks + " ticks)");
        }

        private class RowAnalysis
        {
            public RowAnalysis(int theSearchIndex)
            {
                SearchIndex = theSearchIndex;
            }

            // indicates the row this data belongs to
            public int SearchIndex = -1;

            // stats for pixels within this particular row
            //public double AverageVariation = 0;
            public double AverageIntensity = -1;
            public double StdDevOfVariation = -1;
            public double VariationScore = -1;

            public double VariationFromPrevRow; // this row's avgVar - previous row's avgVar

            //public double AverageVariation_2Row = 0; // I didn't like the 2row average since it skewed the transition a pixel
            public double AverageVariation_3Row = 0; // filters out noise and highlights transitions by sticking out "merging" variations on neighboring rows
        }
    }
}

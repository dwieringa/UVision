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
    class FindEdgeOriginalInstance : ToolInstance
    {
        public FindEdgeOriginalInstance(FindEdgeOriginalDefinition theDefinition, TestExecution testExecution)
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

            if (theDefinition.EdgeDetectionMode == FindEdgeOriginalDefinition.EdgeDetectionModes.NotDefined) throw new ArgumentException(Name + " doesn't have EdgeDetectionMode defined.");
            mEdgeDetectionMode = theDefinition.EdgeDetectionMode;

            if (theDefinition.SurfaceNoiseThreshold == null) throw new ArgumentException(Name + " doesn't have SurfaceNoiseThreshold defined.");
            mSurfaceNoiseThreshold = testExecution.DataValueRegistry.GetObject(theDefinition.SurfaceNoiseThreshold.Name);

            if (theDefinition.MinSurfaceSize == null) throw new ArgumentException(Name + " doesn't have MinSurfaceSize defined.");
            mMinSurfaceSize = testExecution.DataValueRegistry.GetObject(theDefinition.MinSurfaceSize.Name);

            mEdgeMarker = new ValueBasedLineDecorationInstance(theDefinition.EdgeMarker, testExecution);

            mEdgeLocation = new GeneratedValueInstance(theDefinition.EdgeLocation, testExecution);

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

        private GeneratedValueInstance mEdgeLocation = null;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public GeneratedValueInstance EdgeLocation
        {
            get { return mEdgeLocation; }
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

        private FindEdgeOriginalDefinition.EdgeDetectionModes mEdgeDetectionMode = FindEdgeOriginalDefinition.EdgeDetectionModes.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The algorithm to detect the edge.")]
        public FindEdgeOriginalDefinition.EdgeDetectionModes EdgeDetectionMode
        {
            get { return mEdgeDetectionMode; }
        }

        private DataValueInstance mSurfaceNoiseThreshold;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Intensity variation to ignore")]
        public DataValueInstance SurfaceNoiseThreshold
        {
            get { return mSurfaceNoiseThreshold; }
        }

        private DataValueInstance mMinSurfaceSize;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The minimum number of pixels a surface can be (otherwise it is considered part of the transition).")]
        public DataValueInstance MinSurfaceSize
        {
            get { return mMinSurfaceSize; }
        }

        public override bool IsComplete() { return mEdgeLocation.IsComplete(); }

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

            int edgeLocation = -1;

            int minSurfaceSize = Math.Max(1, (int)mMinSurfaceSize.ValueAsLong());
            int surfaceNoiseThreshold = (int)mSurfaceNoiseThreshold.ValueAsLong();

            int startXOffset;
            int startYOffset;
            int searchDirOffset;
            int searchDirIncrement;
            int numSearchDirRows;
            int searchDirXIncrement;
            int searchDirYIncrement;
            int rowWidth;
            int rowDirXIncrement;
            int rowDirYIncrement;
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

            if (mSourceImage.Bitmap == null)
            {
                TestExecution().LogMessage("ERROR: source image for '" + Name + "' does not exist.");
            }
            else
            {
                Bitmap sourceBitmap = SourceImage.Bitmap;
                BitmapData sourceBitmapData = null;

                int x = 0;
                int y = 0;
                double sumIntensityWithinRow = 0;
                double sumVariationWithinRow = 0;
                //double overallSumIntensity = 0;
                //double overallSumVariation = 0;

                RowAnalysis[] rows = new RowAnalysis[numSearchDirRows];
                int[] rowValues = new int[rowWidth];
                bool analyzedSuccessfully = false;
                try
                {
                    sourceBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PIXEL_FORMAT);
                    int sourceStride = sourceBitmapData.Stride;
                    int sourceStrideOffset = sourceStride - (sourceBitmapData.Width * PIXEL_BYTE_WIDTH);

                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* sourcePointer;
                        byte* sourcePointer2;

                        int variation;
                        for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                        {
                            RowAnalysis rowAnalysis = new RowAnalysis(searchDirIndex);
                            rows[searchDirIndex] = rowAnalysis;
                            //if (searchDirIndex > 0)
                            //{
                            //    rowAnalysis.OverallAverageIntensityBeforeRow = overallSumIntensity / searchDirIndex;
                            //    rowAnalysis.OverallVariationBeforeRow  = overallSumVariation / searchDirIndex;
                            //}
                            sumIntensityWithinRow = 0;
                            sumVariationWithinRow = 0;
                            for (int widthIndex = 0; widthIndex < rowWidth; widthIndex++)
                            {
                                x = startXOffset + (searchDirIndex * searchDirXIncrement) + (widthIndex * rowDirXIncrement);
                                y = startYOffset + (searchDirIndex * searchDirYIncrement) + (widthIndex * rowDirYIncrement);

                                sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                sourcePointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point
                                pixelGrayValue = (int)(0.3 * sourcePointer[2] + 0.59 * sourcePointer[1] + 0.11 * sourcePointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                // http://www.bobpowell.net/grayscale.htm
                                // https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=440425&SiteID=1

                                // check pixel "behind"
                                sourcePointer2 = sourcePointer - (searchDirXIncrement*PIXEL_BYTE_WIDTH) - (searchDirYIncrement*sourceStride);
                                prevPixelGrayValue = (int)(0.3 * sourcePointer2[2] + 0.59 * sourcePointer2[1] + 0.11 * sourcePointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).

                                //pixelColor = mSourceImage.Image.GetPixel(x, y);
                                //pixelGrayValue = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / 3.0);
                                //prevPixelColor = mSourceImage.Image.GetPixel(x - searchDirXIncrement, y - searchDirYIncrement);
                                //prevPixelGrayValue = (int)((prevPixelColor.R + prevPixelColor.G + prevPixelColor.B) / 3.0);
                                variation = pixelGrayValue - prevPixelGrayValue;
                                sumVariationWithinRow += variation;
                                sumIntensityWithinRow += pixelGrayValue;
                                rowValues[widthIndex] = variation;
                            }
                            rowAnalysis.AverageIntensity = sumIntensityWithinRow / rowWidth;
                            rowAnalysis.AverageVariation = sumVariationWithinRow / rowWidth;
                            double sumSqDiffs = 0;
                            for (int widthIndex = 0; widthIndex < rowWidth; widthIndex++)
                            {
                                sumSqDiffs += Math.Pow(rowAnalysis.AverageVariation - rowValues[widthIndex],2);
                            }
                            rowAnalysis.StdDevOfVariation = Math.Sqrt(sumSqDiffs / rowWidth);
                            if (Math.Abs(rowAnalysis.AverageVariation) > surfaceNoiseThreshold)
                            {
                                rowAnalysis.VariationScore = Math.Abs(rowAnalysis.AverageVariation / rowAnalysis.StdDevOfVariation);
                            }
                            else
                            {
                                rowAnalysis.VariationScore = 0;
                            }
                            //overallSumIntensity += rowAnalysis.AverageIntensity;
                            //overallSumVariation += Math.Abs(rowAnalysis.AverageVariation);
                        }
                        RowAnalysis currentRow;
                        for (int searchDirIndex = 1; searchDirIndex < numSearchDirRows-1; searchDirIndex++)
                        {
                            currentRow = rows[searchDirIndex];
                            //currentRow.AverageVariation_2Row = (currentRow.AverageVariation + nextRow.AverageVariation) / 2;
                            currentRow.AverageVariation_3Row = (currentRow.AverageVariation + rows[searchDirIndex + 1].AverageVariation + rows[searchDirIndex - 1].AverageVariation) / 3;
                        }
                        TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] finished analyzing pixels");
                    } // end unsafe block
                    analyzedSuccessfully = true;
                }
                catch (Exception e)
                {
                    TestExecution().LogMessageWithTimeFromTrigger("ERROR: Failure in " + Name + "; msg=" + e.Message + " " + Environment.NewLine + e.StackTrace);
                }
                finally
                {
                    sourceBitmap.UnlockBits(sourceBitmapData);
                }

                if (analyzedSuccessfully)
                {
                    SurfaceSearchState state = SurfaceSearchState.SearchingForEndOfSurface;
                    List<Surface> surfaces = new List<Surface>();
                    List<Transition> transitions = new List<Transition>();
                    Surface surface = new Surface(1);
                    surfaces.Add(surface);
                    Transition transition = null;
                    surface.StartPos = 0;
                    int conseqQuietRows = 0;
                    for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                    {
                        switch (state)
                        {
                            case SurfaceSearchState.SearchingForEndOfSurface:
                                if (Math.Abs(rows[searchDirIndex].AverageVariation) > surfaceNoiseThreshold &&
                                    Math.Abs(rows[searchDirIndex].AverageVariation) > rows[searchDirIndex].StdDevOfVariation)
                                {
                                    if (state == SurfaceSearchState.SearchingForEndOfSurface &&
                                        surface.SurfaceNumber == 1 &&
                                        searchDirIndex - surface.StartPos < minSurfaceSize)
                                    {
                                        TestExecution().LogErrorWithTimeFromTrigger("Initial surface too small.  Is an edge at the ROI edge?");
                                        break;
                                    }
                                    surface.EndPos = searchDirIndex - 1;
                                    surface.ComputeValues(rows);
                                    surface = null;
                                    transition = new Transition(transitions.Count + 1);
                                    transition.StartPos = searchDirIndex;
                                    transitions.Add(transition);
                                    state = SurfaceSearchState.SearchingForEndOfTransition;
                                    conseqQuietRows = 0;
                                }
                                break;
                            case SurfaceSearchState.SearchingForEndOfTransition:
                                if (Math.Abs(rows[searchDirIndex].AverageVariation) <= surfaceNoiseThreshold)
                                {
                                    conseqQuietRows++;
                                }
                                else
                                {
                                    conseqQuietRows = 0;
                                }
                                if (conseqQuietRows >= minSurfaceSize)
                                {
                                    transition.EndPos = searchDirIndex - minSurfaceSize;
                                    transition.ComputeValues(rows);
                                    surface = new Surface(surfaces.Count + 1);
                                    surface.StartPos = transition.EndPos + 1;
                                    surfaces.Add(surface);
                                    transition = null;
                                    state = SurfaceSearchState.SearchingForEndOfSurface;
                                    conseqQuietRows = 0;
                                }
                                break;
                        }
                    }
                    if (surface != null)
                    {
                        surface.EndPos = numSearchDirRows - 1;
                        surface.ComputeValues(rows);
                        surface = null;
                    }
                    if (transition != null)
                    {
                        transition.EndPos = numSearchDirRows - 1;
                        transition.ComputeValues(rows);
                        transition = null;
                    }

                    foreach (Surface s in surfaces)
                    {
                        TestExecution().LogMessage(String.Format("Found surface from {0,3:0} to {1,3:0}: avgIntensity = {2,8:0.00} (min={3,8:0.00}, max={4,8:0.00}) avgVar = {5,8:0.00} avgAbsVar = {6,8:0.00} ({7,8:0.00}, {8,8:0.00}) sumVar = {9,8:0.00} sumAbsVar = {10,8:0.00}, avgVarScore = {11,8:0.00}",
                            (searchDirOffset + (searchDirIncrement * s.StartPos)),
                            (searchDirOffset + (searchDirIncrement * s.EndPos)),
                            s.AverageIntensity,
                            s.MinIntensity,
                            s.MaxIntensity,
                            s.AverageVariation,
                            s.AverageAbsVariation,
                            s.MinVariation,
                            s.MaxVariation,
                            s.SumVariation,
                            s.SumAbsVariation,
                            s.AverageVariationScore
                            ));
                    }

                    foreach (Transition t in transitions)
                    {
                        TestExecution().LogMessage(String.Format("Found transition from {0,3:0} to {1,3:0}: avgIntensity = {2,8:0.00} (min={3,8:0.00}, max={4,8:0.00}) avgVar = {5,8:0.00} avgAbsVar = {6,8:0.00} ({7,8:0.00}, {8,8:0.00}) sumVar = {9,8:0.00} sumAbsVar = {10,8:0.00}, avgVarScore = {11,8:0.00}",
                            (searchDirOffset + (searchDirIncrement * t.StartPos)),
                            (searchDirOffset + (searchDirIncrement * t.EndPos)),
                            t.AverageIntensity,
                            t.MinIntensity,
                            t.MaxIntensity,
                            t.AverageVariation,
                            t.AverageAbsVariation,
                            t.MinVariation,
                            t.MaxVariation,
                            t.SumVariation,
                            t.SumAbsVariation,
                            t.AverageVariationScore
                            ));
                    }

                    // remove "false"/weak transitions
                    bool removedTransitions = false;
                    for (int tNdx = 0; tNdx < transitions.Count; tNdx++ )
                    {
                        if (transitions[tNdx].SumAbsVariation < 2 * surfaceNoiseThreshold)
                        {
                            removedTransitions = true;
                            TestExecution().LogMessage("Deleting transition " + transitions[tNdx].TransitionNumber);
                            if (transitions[tNdx].Size() > 2)
                            {
                                TestExecution().LogMessage("WARNING: removing large 'weak' transition.  Is SurfaceNoiseThreshold too low?");
                            }
                            if (surfaces.Count > tNdx + 1)
                            {
                                TestExecution().LogMessage("Merging surfaces " + surfaces[tNdx].SurfaceNumber + " & " + surfaces[tNdx + 1].SurfaceNumber);
                                surfaces[tNdx].EndPos = surfaces[tNdx + 1].EndPos;
                                surfaces.RemoveAt(tNdx + 1);
                            }
                            else
                            {
                                TestExecution().LogMessage("Merging surface " + surfaces[tNdx].SurfaceNumber + " &  transition " + transitions[tNdx].TransitionNumber);
                                surfaces[tNdx].EndPos = transitions[tNdx].EndPos;
                            }
                            surfaces[tNdx].ComputeValues(rows);
                            transitions.RemoveAt(tNdx); tNdx--; // remove transition, then decrement tNdx since for-loop increments it
                        }
                    }

                    if (removedTransitions)
                    {
                        TestExecution().LogMessage("Results after removals:");
                        foreach (Surface s in surfaces)
                        {
                            TestExecution().LogMessage(String.Format("Found surface from {0,3:0} to {1,3:0}: avgIntensity = {2,8:0.00} (min={3,8:0.00}, max={4,8:0.00}) avgVar = {5,8:0.00} avgAbsVar = {6,8:0.00} ({7,8:0.00}, {8,8:0.00}) sumVar = {9,8:0.00} sumAbsVar = {10,8:0.00}, avgVarScore = {11,8:0.00}",
                                (searchDirOffset + (searchDirIncrement * s.StartPos)),
                                (searchDirOffset + (searchDirIncrement * s.EndPos)),
                                s.AverageIntensity,
                                s.MinIntensity,
                                s.MaxIntensity,
                                s.AverageVariation,
                                s.AverageAbsVariation,
                                s.MinVariation,
                                s.MaxVariation,
                                s.SumVariation,
                                s.SumAbsVariation,
                                s.AverageVariationScore
                                ));
                        }

                        foreach (Transition t in transitions)
                        {
                            TestExecution().LogMessage(String.Format("Found transition from {0,3:0} to {1,3:0}: avgIntensity = {2,8:0.00} (min={3,8:0.00}, max={4,8:0.00}) avgVar = {5,8:0.00} avgAbsVar = {6,8:0.00} ({7,8:0.00}, {8,8:0.00}) sumVar = {9,8:0.00} sumAbsVar = {10,8:0.00}, avgVarScore = {11,8:0.00}",
                                (searchDirOffset + (searchDirIncrement * t.StartPos)),
                                (searchDirOffset + (searchDirIncrement * t.EndPos)),
                                t.AverageIntensity,
                                t.MinIntensity,
                                t.MaxIntensity,
                                t.AverageVariation,
                                t.AverageAbsVariation,
                                t.MinVariation,
                                t.MaxVariation,
                                t.SumVariation,
                                t.SumAbsVariation,
                                t.AverageVariationScore
                                ));
                        }
                    }
                    List<Edge> edges = new List<Edge>();
                    int transitionNdx = 0;
                    int edgeRowNdx;
                    double greatestEdgeScore = -1;
                    int edgeNdxWithGreatestScore = -1;
                    for (int surfaceNdx = 0; surfaceNdx < surfaces.Count - 1; surfaceNdx++)
                    {
                        Edge edge = new Edge(edges.Count + 1);
                        edge.leadingSurface = surfaces[surfaceNdx];
                        edge.transition = transitions[transitionNdx]; transitionNdx++;
                        edge.trailingSurface = surfaces[surfaceNdx+1];
                        edges.Add(edge);
                        if (edge.transition.AverageVariationScore > greatestEdgeScore)
                        {
                            greatestEdgeScore = edge.transition.AverageVariationScore;
                            edgeNdxWithGreatestScore = edge.EdgeNumber - 1;
                        }
                        
                        edgeRowNdx = edge.FindEdgeBasedOnIntensity(rows);
                        if (edgeRowNdx >= 0)
                        {
                            TestExecution().LogMessage("Found edge by intensity at row " + edgeRowNdx + " (image location=" + (searchDirOffset + (searchDirIncrement * edgeRowNdx)) + ")");
                        }
                        else
                        {
                            TestExecution().LogMessage("WARNING: unable to find edge by intensity from edge " + edge.EdgeNumber);
                        }
                        edgeRowNdx = edge.FindEdgeBasedOnVariation(rows);
                        if (edgeRowNdx >= 0)
                        {
                            TestExecution().LogMessage("Found edge by variation at row " + edgeRowNdx + " (image location=" + (searchDirOffset + (searchDirIncrement * edgeRowNdx)) + ")");
                        }
                        else
                        {
                            TestExecution().LogMessage("WARNING: unable to find edge by variation from edge " + edge.EdgeNumber);
                        }
                    }
                    if (edgeNdxWithGreatestScore >= 0)
                    {
                        Edge edge = edges[edgeNdxWithGreatestScore];
                        switch (mEdgeDetectionMode)
                        {
                            case FindEdgeOriginalDefinition.EdgeDetectionModes.MaxVariation:
                                edgeRowNdx = edge.FindEdgeBasedOnVariation(rows);
                                edgeLocation = SetEdgeLocation(edgeRowNdx, searchDirOffset, searchDirIncrement);
                                TestExecution().LogMessage("Using edge by variation at row " + edgeRowNdx + " (image location=" + (searchDirOffset + (searchDirIncrement * edgeRowNdx)) + ")");
                                break;
                            case FindEdgeOriginalDefinition.EdgeDetectionModes.SurfaceIntensity:
                                edgeRowNdx = edge.FindEdgeBasedOnIntensity(rows);
                                edgeLocation = SetEdgeLocation(edgeRowNdx, searchDirOffset, searchDirIncrement);
                                TestExecution().LogMessage("Using edge by intensity at row " + edgeRowNdx + " (image location=" + (searchDirOffset + (searchDirIncrement * edgeRowNdx)) + ")");
                                break;
                            case FindEdgeOriginalDefinition.EdgeDetectionModes.NotDefined:
                                TestExecution().LogErrorWithTimeFromTrigger("EdgeDetectionMode for " + Name + " isn't defined.");
                                break;
                            default:
                                TestExecution().LogErrorWithTimeFromTrigger("Edge Detection Mode '" + mEdgeDetectionMode + "' isn't fully implemented.");
                                break;
                        }
                    }

//                    TestExecution().LogMessage("index | pos  | avgI | avgV | stdDevV | V score");
                    for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                    {
                        RowAnalysis row = rows[searchDirIndex];
                        TestExecution().LogMessage(
                            String.Format("{0,3:0} pos={1,4:0} avgI={2,8:0.00} avgV={3,8:0.00} stdDevV={4,8:0.00} vScore={5,8:0.00} 3RowAvgV={6,8:0.00}",
                            row.SearchIndex,
                            (searchDirOffset + (searchDirIncrement*row.SearchIndex)),
                            row.AverageIntensity,
                            row.AverageVariation,
                            row.StdDevOfVariation,
                            row.VariationScore,
//                            row.AverageVariation_2Row,
                            row.AverageVariation_3Row
//                            row.OverallAverageIntensityBeforeRow,
//                            row.OverallVariationBeforeRow
                            ));
                    }
                }
            } // end main block ("else" after all initial setup error checks)
            mEdgeLocation.SetValue(edgeLocation);
            mEdgeLocation.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " computed edge at " + edgeLocation);

            if (mAutoSave)
            {
                try
                {
                    string filePath = ((FindEdgeOriginalDefinition)Definition()).AutoSavePath;
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

        private int SetEdgeLocation(int edgeRowNdx, int searchDirOffset, int searchDirIncrement)
        {
            int edgeLocation = searchDirOffset + (edgeRowNdx * searchDirIncrement);
            if (mSearchDirection == Direction.Left || mSearchDirection == Direction.Right)
            {
                mEdgeMarker.SetStartX(edgeLocation);
                mEdgeMarker.SetEndX(edgeLocation);
                mEdgeMarker.SetStartY(mSearchArea.Top);
                mEdgeMarker.SetEndY(mSearchArea.Bottom);
            }
            else
            {
                mEdgeMarker.SetStartY(edgeLocation);
                mEdgeMarker.SetEndY(edgeLocation);
                mEdgeMarker.SetStartX(mSearchArea.Left);
                mEdgeMarker.SetEndX(mSearchArea.Right);
            }
            mEdgeMarker.SetIsComplete();
            return edgeLocation;
        }

        private class Edge
        {
            public Edge(int theEdgeNumber)
            {
                EdgeNumber = theEdgeNumber;
            }
            public int EdgeNumber = -1;
            public Surface leadingSurface;
            public Transition transition;
            public Surface trailingSurface;
            public int FindEdgeBasedOnVariation(RowAnalysis[] rows)
            {
                if (leadingSurface == null || trailingSurface == null || transition == null)
                {
                    throw new ArgumentException("asldji231dilasj");
                }
                int rowWithBiggestChange = -1;
                double biggestChange = 0;
                for (int rowNdx = transition.StartPos; rowNdx <= transition.EndPos; rowNdx++)
                {
                    double absVariation = Math.Abs(rows[rowNdx].AverageVariation);
                    if (absVariation > biggestChange)
                    {
                        biggestChange = absVariation;
                        rowWithBiggestChange = rowNdx;
                    }
                }
                return rowWithBiggestChange - 1;
            }
            public int FindEdgeBasedOnIntensity(RowAnalysis[] rows)
            {
                if (leadingSurface == null || trailingSurface == null || transition == null)
                {
                    throw new ArgumentException("asldjiajdilasj");
                }
                for (int rowNdx = transition.StartPos; rowNdx <= transition.EndPos; rowNdx++)
                {
                    double rowIntensity = rows[rowNdx].AverageIntensity;
                    if( Math.Abs( trailingSurface.AverageIntensity - rowIntensity ) < Math.Abs( leadingSurface.AverageIntensity - rowIntensity ) )
                    {
                        // if this row is closer in intensity to the trailing surface than the leading surface, consider the PREVIOUS row the edge of surface 1
                        return rowNdx - 1;
                    }
                }
                return -1; // shouldn't get here
            }
        }

        private class Section
        {
            public Section()
            {
            }

            public int Size() { return EndPos - StartPos + 1; }
            public void ComputeValues(RowAnalysis[] rows)
            {
                double intensity = 0;
                double absVariation = 0;
                double variation = 0;
                double sumIntensity = 0;
                double sumVariationScores = 0;
                int numValues = 0;
                for (int rowNdx = StartPos; rowNdx <= EndPos; rowNdx++)
                {
                    intensity = rows[rowNdx].AverageIntensity;
                    variation = rows[rowNdx].AverageVariation;
                    absVariation = Math.Abs(variation);
                    if (intensity < MinIntensity) MinIntensity = intensity;
                    if (intensity > MaxIntensity) MaxIntensity = intensity;
                    if (absVariation < MinVariation) MinVariation = absVariation;
                    if (absVariation > MaxVariation) MaxVariation = absVariation;
                    sumIntensity += intensity;
                    SumVariation += variation;
                    SumAbsVariation += absVariation;
                    sumVariationScores += rows[rowNdx].VariationScore;
                    numValues++;
                }
                AverageIntensity = sumIntensity / numValues;
                AverageVariation = SumVariation / numValues;
                AverageAbsVariation = SumAbsVariation / numValues;
                AverageVariationScore = sumVariationScores / numValues;
            }

            public int StartPos = -1;
            public int EndPos = -1;
            public double AverageIntensity = -1;
            public double AverageAbsVariation = -1;
            public double AverageVariation = -1;
            public double MinVariation = 999999;
            public double MaxVariation = -1;
            public double MinIntensity = 999999;
            public double MaxIntensity = -1;
            public double SumAbsVariation = -1;
            public double SumVariation = -1;
            public double AverageVariationScore = -1;
        }

        private class Surface : Section
        {
            public Surface(int theSurfaceNumber)
            {
                SurfaceNumber = theSurfaceNumber;
            }

            public int SurfaceNumber = -1;
        }

        private class Transition : Section
        {
            public Transition(int theSurfaceNumber)
            {
                TransitionNumber = theSurfaceNumber;
            }

            public int TransitionNumber = -1;
        }

        private class RowAnalysis
        {
            public RowAnalysis(int theSearchIndex)
            {
                SearchIndex = theSearchIndex;
            }

            // indicates the row this data belows to
            public int SearchIndex = -1;

            // stats for pixels within this particular row
            public double AverageVariation = 0;
            public double AverageIntensity = -1;
            public double StdDevOfVariation = -1;
            public double VariationScore = -1;

            //public double AverageVariation_2Row = 0; // I didn't like the 2row average since it skewed the transition a pixel
            public double AverageVariation_3Row = 0; // filters out noise and highlights edges by sticking out "merging" variations on neighboring rows

            // averaging rows before this one ...this is meaningless after the first surface
            //public double OverallVariationBeforeRow = -1;
            //public double OverallAverageIntensityBeforeRow = -1;
        }
    }
}

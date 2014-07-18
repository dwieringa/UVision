// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace NetCams
{
    public class FindEdgeInstance : ToolInstance
    {
        public FindEdgeInstance(FindEdgeDefinition theDefinition, TestExecution testExecution)
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

            if (theDefinition.EdgeDetectionMode == FindEdgeDefinition.EdgeDetectionModes.NotDefined) throw new ArgumentException(Name + " doesn't have EdgeDetectionMode defined.");
            mEdgeDetectionMode = theDefinition.EdgeDetectionMode;

            mEdgeTypeSelectionFilter = theDefinition.EdgeTypeSelectionFilter;
            if (theDefinition.DarkSurface_AvgIntensity_Max != null)
            {
                mDarkSurface_AvgIntensity_Max = testExecution.DataValueRegistry.GetObject(theDefinition.DarkSurface_AvgIntensity_Max.Name);
            }

            if (theDefinition.DisableSurfaceTransitionFilter != null)
            {
                mDisableSurfaceTransitionFilter = testExecution.DataValueRegistry.GetObject(theDefinition.DisableSurfaceTransitionFilter.Name);
            }

            if (theDefinition.SurfaceNoiseThreshold_Min != null)
            {
                mSurfaceNoiseThreshold_Min = testExecution.DataValueRegistry.GetObject(theDefinition.SurfaceNoiseThreshold_Min.Name);
            }

            if (theDefinition.SurfaceNoiseThreshold_Max != null)
            {
                mSurfaceNoiseThreshold_Max = testExecution.DataValueRegistry.GetObject(theDefinition.SurfaceNoiseThreshold_Max.Name);
            }

            if (theDefinition.SurfaceNoiseThreshold_PercentOfSharpestEdge != null)
            {
                mSurfaceNoiseThreshold_PercentOfSharpestEdge = testExecution.DataValueRegistry.GetObject(theDefinition.SurfaceNoiseThreshold_PercentOfSharpestEdge.Name);
            }

            if (theDefinition.SurfaceNoiseThreshold_PercentileBase != null)
            {
                mSurfaceNoiseThreshold_PercentileBase = testExecution.DataValueRegistry.GetObject(theDefinition.SurfaceNoiseThreshold_PercentileBase.Name);
            }


            if (theDefinition.MinSurfaceSize == null) throw new ArgumentException(Name + " doesn't have MinSurfaceSize defined.");
            mMinSurfaceSize = testExecution.DataValueRegistry.GetObject(theDefinition.MinSurfaceSize.Name);

            mEdgeMarker = new ValueBasedLineDecorationInstance(theDefinition.EdgeMarker, testExecution);
            mEdgeLocation = new GeneratedValueInstance(theDefinition.EdgeLocation, testExecution);

            mAutoSave = theDefinition.AutoSave;
            mVerboseOutput = theDefinition.VerboseOutput;
            mOldSlowModeTest = theDefinition.OldSlowModeTest;
            mFilterWeakTransitions_AvgAbsVariation = theDefinition.FilterWeakTransitions_AvgAbsVariation;
            mFilterWeakTransitions_MinMaxComparisons = theDefinition.FilterWeakTransitions_MinMaxComparisons;
        }

        private bool mOldSlowModeTest;
        private bool mVerboseOutput;
        private bool mAutoSave;
        private bool mFilterWeakTransitions_MinMaxComparisons;
        private bool mFilterWeakTransitions_AvgAbsVariation;

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

        private FindEdgeDefinition.EdgeDetectionModes mEdgeDetectionMode = FindEdgeDefinition.EdgeDetectionModes.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The algorithm to detect the edge.")]
        public FindEdgeDefinition.EdgeDetectionModes EdgeDetectionMode
        {
            get { return mEdgeDetectionMode; }
        }

        private FindEdgeDefinition.EdgeTypeSelectionFilters mEdgeTypeSelectionFilter = FindEdgeDefinition.EdgeTypeSelectionFilters.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public FindEdgeDefinition.EdgeTypeSelectionFilters EdgeTypeSelectionFilter
        {
            get { return mEdgeTypeSelectionFilter; }
        }

        private DataValueInstance mDarkSurface_AvgIntensity_Max;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Defines the maximum allowed 'average intensity' of the dark surface. This only applies if EdgeTypeSelectionFilter is defined.  Any surface with an average intensity above this value won't be considered dark.")]
        public DataValueInstance DarkSurface_AvgIntensity_Max
        {
            get { return mDarkSurface_AvgIntensity_Max; }
        }

        private DataValueInstance mDisableSurfaceTransitionFilter;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance DisableSurfaceTransitionFilter
        {
            get { return mDisableSurfaceTransitionFilter; }
        }

        private DataValueInstance mSurfaceNoiseThreshold_Min;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Intensity variation to ignore")]
        public DataValueInstance SurfaceNoiseThreshold_Min
        {
            get { return mSurfaceNoiseThreshold_Min; }
        }

        private DataValueInstance mSurfaceNoiseThreshold_Max;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Limit the intensity variation to ignore")]
        public DataValueInstance SurfaceNoiseThreshold_Max
        {
            get { return mSurfaceNoiseThreshold_Max; }
        }
        
        private DataValueInstance mSurfaceNoiseThreshold_PercentOfSharpestEdge;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance SurfaceNoiseThreshold_PercentOfSharpestEdge
        {
            get { return mSurfaceNoiseThreshold_PercentOfSharpestEdge; }
        }

        private DataValueInstance mSurfaceNoiseThreshold_PercentileBase;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance SurfaceNoiseThreshold_PercentileBase
        {
            get { return mSurfaceNoiseThreshold_PercentileBase; }
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
            /* TODO: OPTIMIZATIONS:
             * - compute surfaceNoiseLevel based on image analysis
             * - make debug output to log optional
             * - surface/transition decorations (biggest problem is that there can be a variable number...only for first edge to start?)
             * - for marked image, save decorations...don't copy/paint_on image
             */

            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            double edgeLocation = -1;

            if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met. Skipping.");
            }
            else
            {
                bool ensureStraightEdge = true; // TODO: make optional
                int minSurfaceSize = Math.Max(1, (int)mMinSurfaceSize.ValueAsLong());

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
                else if (mSurfaceNoiseThreshold_PercentOfSharpestEdge != null && mSurfaceNoiseThreshold_PercentOfSharpestEdge.ValueAsDecimal() < 0)
                {
                    TestExecution().LogMessage("ERROR: SurfaceNoiseThreshold_PercentOfSharpestEdge for '" + Name + "' can not be negative.");
                }
                else if (mSearchArea.Left < 0 || mSearchArea.Right >= mSourceImage.Width || mSearchArea.Left > mSearchArea.Right || mSearchArea.Top < 0 || mSearchArea.Bottom >= mSourceImage.Height || mSearchArea.Top > mSearchArea.Bottom)
                {
                    TestExecution().LogMessage("ERROR: Invalid SearchArea for '" + Name + "'.");
                }
                else if (mSurfaceNoiseThreshold_Min != null && mSurfaceNoiseThreshold_Max != null && mSurfaceNoiseThreshold_Min.ValueAsLong() > mSurfaceNoiseThreshold_Max.ValueAsLong())
                {
                    TestExecution().LogMessage("ERROR: SurfaceNoiseThreshold_Min > SurfaceNoiseThreshold_Max for '" + Name + "'.");
                }
                else
                {
                    int surfaceNoiseThreshold = 0;
                    if (mSurfaceNoiseThreshold_Min != null)
                    {
                        surfaceNoiseThreshold = (int)mSurfaceNoiseThreshold_Min.ValueAsLong();
                    }

                    int x = 0;
                    int y = 0;
                    double sumIntensityWithinRow = 0;

                    RowAnalysis[] rows = new RowAnalysis[numSearchDirRows];
                    bool analyzedSuccessfully = false;

                    if (!mOldSlowModeTest)
                    {
                        // Compute the average grey value ("intensity") of each row of the ROI
                        RowAnalysis rowAnalysis;
                        RowAnalysis prevRow = null;
                        for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++) // iterate thru each row
                        {
                            rowAnalysis = new RowAnalysis(searchDirIndex);
                            rows[searchDirIndex] = rowAnalysis;
                            sumIntensityWithinRow = 0;
                            for (int widthIndex = 0; widthIndex < rowWidth; widthIndex++) // sum up the grey value within the current row
                            {
                                x = startXOffset + (searchDirIndex * searchDirXIncrement) + (widthIndex * rowDirXIncrement);
                                y = startYOffset + (searchDirIndex * searchDirYIncrement) + (widthIndex * rowDirYIncrement);

                                pixelGrayValue = mSourceImage.GetGrayValue(x, y);

                                sumIntensityWithinRow += pixelGrayValue;
                            }
                            rowAnalysis.AverageValue = sumIntensityWithinRow / rowWidth; // compute average for the current row
                            if (prevRow != null)
                            {
                                rowAnalysis.VariationFromPrevRow_raw = rowAnalysis.AverageValue - prevRow.AverageValue; // compute variation from previous row
                            }
                            prevRow = rowAnalysis;
                        }

                        TestExecution().LogMessageWithTimeFromTrigger("Finished analyzing rows");
                        analyzedSuccessfully = true;
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
                                    rowAnalysis.AverageValue = sumIntensityWithinRow / rowWidth;
                                    if (prevRow != null)
                                    {
                                        rowAnalysis.VariationFromPrevRow_raw = rowAnalysis.AverageValue - prevRow.AverageValue;
                                    }
                                    prevRow = rowAnalysis;
                                }

                            } // end unsafe block
                            TestExecution().LogMessageWithTimeFromTrigger("Finished analyzing rows");
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

                    if (analyzedSuccessfully)
                    {
                        RowAnalysis currentRow;
                        if (mDisableSurfaceTransitionFilter == null || !mDisableSurfaceTransitionFilter.ValueAsBoolean())
                        {
                            // if filtering is not explicitly disabled... 

                            // average neighboring rows together to supress noise and highlight multi-row transitions
                            for (int searchDirIndex = 1; searchDirIndex < numSearchDirRows - 1; searchDirIndex++)
                            {
                                currentRow = rows[searchDirIndex];
                                currentRow.VariationFromPrevRow_used = (currentRow.VariationFromPrevRow_raw + rows[searchDirIndex + 1].VariationFromPrevRow_raw + rows[searchDirIndex - 1].VariationFromPrevRow_raw) / 3;
                            }
                            TestExecution().LogMessageWithTimeFromTrigger("Performed filtering");
                        }
                        else // if filtering is disabled, then use the raw values...
                        {
                            // if 3Row Average is disabled, then just copy the raw values to use
                            for (int searchDirIndex = 1; searchDirIndex < numSearchDirRows - 1; searchDirIndex++)
                            {
                                currentRow = rows[searchDirIndex];
                                currentRow.VariationFromPrevRow_used = currentRow.VariationFromPrevRow_raw;
                            }
                        }

                        //
                        // COMPUTE SURFACE NOISE THRESHOLD USED TO DETERMINE THE DIFFERENCE BETWEEN A SURFACE VARIATION AND A TRANSITION VARIATION
                        //

                        if (mSurfaceNoiseThreshold_PercentOfSharpestEdge != null)
                        {
                            // compute largestVariation for surfaceNoiseThreshold
                            double largestVariation = 0;
                            for (int searchDirIndex = 1; searchDirIndex < numSearchDirRows - 1; searchDirIndex++)
                            {
                                currentRow = rows[searchDirIndex];
                                if (Math.Abs(currentRow.VariationFromPrevRow_used) > surfaceNoiseThreshold &&
                                    Math.Abs(currentRow.VariationFromPrevRow_used) > largestVariation &&
                                    (!ensureStraightEdge || TransitionIsStraight(currentRow)))
                                {
                                    largestVariation = Math.Abs(currentRow.VariationFromPrevRow_used);
                                }
                            }

                            // Compute threshold to weed out noise (TODO: train this over a series of tests in "learn" mode? we need a way to determine if no edge present)
                            // option1: use 60% of biggest variation
                            // option2: run transitions thru a value group and weed out bottom chunk
                            surfaceNoiseThreshold = Math.Max(surfaceNoiseThreshold, (int)(largestVariation * (mSurfaceNoiseThreshold_PercentOfSharpestEdge.ValueAsDecimal() / 100.0)));
                            TestExecution().LogMessageWithTimeFromTrigger(Name + ": largestVariation=" + largestVariation);
                        }

                        if (mSurfaceNoiseThreshold_PercentileBase != null)
                        {
                            double percentileBase = mSurfaceNoiseThreshold_PercentileBase.ValueAsDecimal();
                            ValueGrouper variationGrouper = new ValueGrouper(0, 256, 100);
                            for (int searchDirIndex = 1; searchDirIndex < numSearchDirRows - 1; searchDirIndex++)
                            {
                                variationGrouper.AddValue(Math.Abs((int)rows[searchDirIndex].VariationFromPrevRow_used));
                            }
                            int rowsAccountedFor = 0;
                            int rowsNeededForPercentile = (int)(numSearchDirRows * percentileBase)/100;
                            int groupNdx = 0;
                            for (; groupNdx < variationGrouper.NumGroups && rowsAccountedFor < rowsNeededForPercentile; groupNdx++)
                            {
                                rowsAccountedFor += variationGrouper.GetGroup(groupNdx).count;
                            }
                            TestExecution().LogMessageWithTimeFromTrigger("Found " + percentileBase + "-percentile at group " + groupNdx);
                            int numConseqQuietGroups = 0;
                            for (; groupNdx < variationGrouper.NumGroups; groupNdx++)
                            {
                                if (variationGrouper.GetGroup(groupNdx).count < 5)
                                {
                                    TestExecution().LogMessage("Found quiet group at " + groupNdx);
                                    numConseqQuietGroups++;
                                }
                                else
                                {
                                    TestExecution().LogMessage("Found nonquiet group at " + groupNdx);
                                    numConseqQuietGroups = 0;
                                }
                                if (numConseqQuietGroups >= 2)
                                {
                                    TestExecution().LogMessage("Found large enough quiet run; ended search at " + groupNdx);
                                    break;
                                }
                            }
                            for (; groupNdx < variationGrouper.NumGroups; groupNdx++)
                            {
                                if (variationGrouper.GetGroup(groupNdx).count > 0)
                                {
                                    int lowValue = variationGrouper.GetGroup(groupNdx).min;
                                    TestExecution().LogMessage("First value after quiet period at group " + groupNdx + "; low value = " + lowValue);
                                    surfaceNoiseThreshold = Math.Max(surfaceNoiseThreshold, lowValue - 1);
                                    break;
                                }
                            }
                            if (mVerboseOutput)
                            {
                                TestExecution().LogMessageWithTimeFromTrigger(Name + ": Variation Grouping:");
                                ValueGrouper.GroupStats group;
                                for (groupNdx = 0; groupNdx < variationGrouper.NumGroups; groupNdx++)
                                {
                                    group = variationGrouper.GetGroup(groupNdx);
                                    TestExecution().LogMessage(String.Format(" {0,4:0}  {1,4:0}  {2,4:0}  {3,4:0}",
                                        groupNdx,
                                        group.count,
                                        group.start,
                                        group.end
                                        ));
                                }
                            }
                        }

                        if (SurfaceNoiseThreshold_Max != null)
                        {
                            surfaceNoiseThreshold = (int)Math.Min(mSurfaceNoiseThreshold_Max.ValueAsLong(), surfaceNoiseThreshold);
                        }
                        TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] surfaceNoiseThreshold=" + surfaceNoiseThreshold);
                        TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] finished analyzing pixels");

                        if (((FindEdgeDefinition)Definition()).ChartEdgeForDebug)
                        {
                            GraphPane myPane = new GraphPane(new RectangleF(0, 0, 640, 480),
                                        "Intensity Graph", "ROI Rows", "Intensity");

                            PointPairList ppl = new PointPairList();
                            for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                            {
                                ppl.Add(searchDirIndex, rows[searchDirIndex].AverageValue);
                            }

                            LineItem myCurve = myPane.AddCurve("Intensity", ppl, Color.Blue, SymbolType.Diamond);
                            Bitmap bm = new Bitmap(1, 1);
                            using (Graphics g = Graphics.FromImage(bm))
                                myPane.AxisChange(g);
                            myPane.GetImage().Save("IntensityGraph" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png", ImageFormat.Png);
                            bm.Dispose();
                            /*
                            ChartForm chartForm = ((FindEdgeDefinition)Definition()).mChartForm;
                            if (chartForm == null)
                            {
                                chartForm = new ChartForm();//(TestSequence().mTestCollections[0].OperationForm);
                                ((FindEdgeDefinition)Definition()).mChartForm = chartForm;
                            }
                            chartForm.Clear();
                            for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                            {
                                chartForm.AddPoint(searchDirIndex, rows[searchDirIndex].AverageIntensity);
                            }
                            chartForm.UpdateChart();*/

                            //chartForm.Show(Window());
                            //Bitmap chart = new Bitmap(chartForm.zedGraphControl1.Width, chartForm.zedGraphControl1.Height);
                            //chartForm.zedGraphControl1.DrawToBitmap(chart, new Rectangle(0, 0, chartForm.zedGraphControl1.Width, chartForm.zedGraphControl1.Height));
                            //chart.Save("g:\\test.bmp");
                            //chart.Dispose();
                            //chartForm.Show(chartForm.mOpForm.dockPanel); DOESN"T WORK...plus get multithreaded errors later
                        }

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
                                    if (Math.Abs(rows[searchDirIndex].VariationFromPrevRow_used) > surfaceNoiseThreshold) //&&
                                    //Math.Abs(rows[searchDirIndex].VariationFromPrevRow) > rows[searchDirIndex].StdDevOfVariation)
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
                                    if (Math.Abs(rows[searchDirIndex].VariationFromPrevRow_used) <= surfaceNoiseThreshold)
                                    {
                                        conseqQuietRows++;
                                    }
                                    else
                                    {
                                        conseqQuietRows = 0;
                                    }
                                    if (conseqQuietRows >= minSurfaceSize)
                                    {
                                        // before 10/21/08 fix: transition.EndPos = searchDirIndex - minSurfaceSize - 1; // added "-1" 10/3/08 for DS cell; noticed that transitions were ending 1 pixel into surface...reason: if variation is low, then BOTH pixels used in comparison are part of this surface (out of transition)...we need -1 to grab previous pixel
                                        transition.EndPos = Math.Max(searchDirIndex - conseqQuietRows - 1,transition.StartPos); // added "-1" 10/3/08 for DS cell; noticed that transitions were ending 1 pixel into surface...reason: if variation is low, then BOTH pixels used in comparison are part of this surface (out of transition)...we need -1 to grab previous pixel
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
                            TestExecution().LogMessage(String.Format("Found surface from {0,3:0} to {1,3:0}: avgIntensity = {2,6:0.00} (min={3,6:0.00}, max={4,6:0.00}) avgVar = {5,6:0.00} avgAbsVar = {6,6:0.00} ({7,6:0.00}, {8,6:0.00}) sumVar = {9,6:0.00} sumAbsVar = {10,8:0.00}, avgVarScore = {11,5:0.00}",
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
                            TestExecution().LogMessage(String.Format("Found transition from {0,3:0} to {1,3:0}: avgIntensity = {2,6:0.00} (min={3,6:0.00}, max={4,6:0.00}) avgVar = {5,6:0.00} avgAbsVar = {6,6:0.00} ({7,6:0.00}, {8,6:0.00}) sumVar = {9,6:0.00} sumAbsVar = {10,8:0.00}, avgVarScore = {11,5:0.00}",
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
                        bool removeThisTransition = false;
                        for (int tNdx = 0; tNdx < transitions.Count; tNdx++)
                        {
                            removeThisTransition = false;
                            //before 10/21/08 fix: if (transitions[tNdx].SumAbsVariation < 2 * surfaceNoiseThreshold)  This doesn't work for 1-pixel transitions...should be looking at AveragePerPixelVariation, not Sum
                            if (mFilterWeakTransitions_AvgAbsVariation && transitions[tNdx].AverageAbsVariation < surfaceNoiseThreshold)
                            {
                                removeThisTransition = true;
                                TestExecution().LogMessage("Deleting transition " + transitions[tNdx].TransitionNumber + " due to average absolute variation");
                                if (transitions[tNdx].Size() > 2)
                                {
                                    TestExecution().LogMessage("WARNING: removing large 'weak' transition.  Is SurfaceNoiseThreshold too low?");
                                }
                            }
                            if (mFilterWeakTransitions_MinMaxComparisons)
                            {
                                if (transitions[tNdx].MaxIntensity < surfaces[tNdx].MaxIntensity && transitions[tNdx].MinIntensity > surfaces[tNdx].MinIntensity)
                                {
                                    removeThisTransition = true;
                                    TestExecution().LogMessage("Deleting transition " + transitions[tNdx].TransitionNumber + " because it can be swallowed by previous surface");
                                }
                                if (surfaces.Count > tNdx + 1 && transitions[tNdx].MaxIntensity < surfaces[tNdx + 1].MaxIntensity && transitions[tNdx].MinIntensity > surfaces[tNdx + 1].MinIntensity)
                                {
                                    removeThisTransition = true;
                                    TestExecution().LogMessage("Deleting transition " + transitions[tNdx].TransitionNumber + " because it can be swallowed by next surface");
                                }
                            }

                            if (removeThisTransition)
                            {
                                removedTransitions = true;
                                if (surfaces.Count > tNdx + 1)
                                {
                                    TestExecution().LogMessage("Merging surfaces " + surfaces[tNdx].SurfaceNumber + " & " + surfaces[tNdx + 1].SurfaceNumber);
                                    surfaces[tNdx].EndPos = surfaces[tNdx + 1].EndPos;
                                    surfaces.RemoveAt(tNdx + 1);
                                }
                                else
                                {
                                    TestExecution().LogMessage("Merging surface " + surfaces[tNdx].SurfaceNumber + " & transition " + transitions[tNdx].TransitionNumber);
                                    surfaces[tNdx].EndPos = transitions[tNdx].EndPos;
                                }
                                surfaces[tNdx].ComputeValues(rows);
                                transitions.RemoveAt(tNdx); tNdx--; // remove transition, then decrement tNdx since for-loop increments it
                            }
                        }

                        if (removedTransitions && mVerboseOutput)
                        {
                            TestExecution().LogMessage("Results after removals:");
                            foreach (Surface s in surfaces)
                            {
                                TestExecution().LogMessage(String.Format("Found surface from {0,3:0} to {1,3:0}: avgIntensity = {2,6:0.00} (min={3,6:0.00}, max={4,6:0.00}) avgVar = {5,6:0.00} avgAbsVar = {6,6:0.00} ({7,6:0.00}, {8,6:0.00}) sumVar = {9,6:0.00} sumAbsVar = {10,8:0.00}, avgVarScore = {11,5:0.00}",
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
                                TestExecution().LogMessage(String.Format("Found transition from {0,3:0} to {1,3:0}: avgIntensity = {2,6:0.00} (min={3,6:0.00}, max={4,6:0.00}) avgVar = {5,6:0.00} avgAbsVar = {6,6:0.00} ({7,6:0.00}, {8,6:0.00}) sumVar = {9,6:0.00} sumAbsVar = {10,8:0.00}, avgVarScore = {11,5:0.00}",
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
                            Edge edge = new Edge(this, edges.Count + 1);
                            edge.leadingSurface = surfaces[surfaceNdx];
                            edge.transition = transitions[transitionNdx]; transitionNdx++;
                            edge.trailingSurface = surfaces[surfaceNdx + 1];
                            edges.Add(edge);
                            if ((mEdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.NotDefined ||
                                (mEdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.DarkToBright && edge.transition.SumVariation > 0) ||
                                (mEdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.BrightToDark && edge.transition.SumVariation < 0)
                                ) &&
                                Math.Abs(edge.transition.SumVariation) > greatestEdgeScore)
                            {
                                if (mEdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.DarkToBright &&
                                    mDarkSurface_AvgIntensity_Max != null &&
                                    edge.leadingSurface.AverageIntensity > mDarkSurface_AvgIntensity_Max.ValueAsLong())
                                {
                                    TestExecution().LogMessage("Ignoring transition " + edge.transition.TransitionNumber + " (" + edge.transition.StartPos + "-" + edge.transition.EndPos +") because the dark surface doesn't meet DarkSurface_AvgIntensity_Max (avg intensity=" + edge.leadingSurface.AverageIntensity + ")");
                                }
                                else if (mEdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.BrightToDark &&
                                    mDarkSurface_AvgIntensity_Max != null &&
                                    edge.trailingSurface.AverageIntensity > mDarkSurface_AvgIntensity_Max.ValueAsLong())
                                {
                                    TestExecution().LogMessage("Ignoring transition " + edge.transition.TransitionNumber + " (" + edge.transition.StartPos + "-" + edge.transition.EndPos + ") because the dark surface doesn't meet DarkSurface_AvgIntensity_Max (avg intensity=" + edge.trailingSurface.AverageIntensity + ")");
                                }
                                else
                                {
                                    greatestEdgeScore = Math.Abs(edge.transition.SumVariation);
                                    edgeNdxWithGreatestScore = edge.EdgeNumber - 1;
                                }
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
                                case FindEdgeDefinition.EdgeDetectionModes.MaxVariation:
                                    edgeRowNdx = edge.FindEdgeBasedOnVariation(rows);
                                    edgeLocation = searchDirOffset + (edgeRowNdx * searchDirIncrement);
                                    SetEdgeLocation((int)edgeLocation);
                                    TestExecution().LogMessage("Using edge by variation at row " + edgeRowNdx + " (image location=" + (searchDirOffset + (searchDirIncrement * edgeRowNdx)) + ")");
                                    break;
                                case FindEdgeDefinition.EdgeDetectionModes.SurfaceIntensity:
                                    edgeRowNdx = edge.FindEdgeBasedOnIntensity(rows);
                                    edgeLocation = searchDirOffset + (edgeRowNdx * searchDirIncrement);
                                    SetEdgeLocation((int)edgeLocation);
                                    TestExecution().LogMessage("Using edge by intensity at row " + edgeRowNdx + " (image location=" + (searchDirOffset + (searchDirIncrement * edgeRowNdx)) + ")");
                                    break;
                                case FindEdgeDefinition.EdgeDetectionModes.SubPixelBySurfaceIntensity:
                                    edgeLocation = edge.FindSubPixelEdgeBasedOnIntensity(rows, 60, searchDirOffset, searchDirIncrement); // I'm not completely sold on 60% over 50%...I started with 75% because I was working with bright-to-BLACK transitions and I figured I wanted to include anything not black. 50% probably makes the most sense?
                                    SetEdgeLocation((int)Math.Round(edgeLocation));
                                    TestExecution().LogMessage("Using subpixel edge by intensity at image location " + edgeLocation);
                                    break;
                                case FindEdgeDefinition.EdgeDetectionModes.NotDefined:
                                    TestExecution().LogErrorWithTimeFromTrigger("EdgeDetectionMode for " + Name + " isn't defined.");
                                    break;
                                default:
                                    TestExecution().LogErrorWithTimeFromTrigger("Edge Detection Mode '" + mEdgeDetectionMode + "' isn't fully implemented.");
                                    break;
                            }
                        }

                        if (mVerboseOutput)
                        {
                            for (int searchDirIndex = 0; searchDirIndex < numSearchDirRows; searchDirIndex++)
                            {
                                RowAnalysis row = rows[searchDirIndex];
                                TestExecution().LogMessage(
                                    String.Format("{0,3:0} pos={1,4:0} avgI={2,8:0.00} rowV_raw={3,8:0.00} stdDevV={4,8:0.00} vScore={5,8:0.00} rowV_used={6,8:0.00}",
                                    row.SearchIndex,
                                    (searchDirOffset + (searchDirIncrement * row.SearchIndex)),
                                    row.AverageValue,
                                    row.VariationFromPrevRow_raw,
                                    row.StdDevOfVariation,
                                    row.VariationScore,
                                    row.VariationFromPrevRow_used
                                    ));
                            }
                        }
                    }
                } // end main block ("else" after all initial setup error checks)
            }
            mEdgeLocation.SetValue(edgeLocation);
            mEdgeLocation.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessageWithTimeFromTrigger(Name + " computed edge at " + edgeLocation);

            if (mAutoSave)
            {
                try
                {
                    string filePath = ((FindEdgeDefinition)Definition()).AutoSavePath;
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

        private bool TransitionIsStraight(RowAnalysis rowAnalysis)
        {
            // TODO: OPTIONALLY, weed out contrasting specks in narrow ROI's by ensuring transition is consistant across row
            // ensure std dev of variation across row is "low enough"?
            // ensure all varitation across the row is in the same direction? (dark-to-light or vice versa ...and bigger than some threshold?)
            if (rowAnalysis.StdDevOfVariation < 0)
            {
                if (mOldSlowModeTest)
                {
                    unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                    {
                        byte* sourcePointer;
                        byte* sourcePointer2;
                        int x;
                        int y;
                        double sumSqDiffs = 0;
                        for (int widthIndex = 0; widthIndex < rowWidth; widthIndex++)
                        {
                            x = startXOffset + (rowAnalysis.SearchIndex * searchDirXIncrement) + (widthIndex * rowDirXIncrement);
                            y = startYOffset + (rowAnalysis.SearchIndex * searchDirYIncrement) + (widthIndex * rowDirYIncrement);

                            sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                            sourcePointer += (y * sourceStride) + (x * PIXEL_BYTE_WIDTH); // adjust to current point
                            pixelGrayValue = (int)(0.3 * sourcePointer[2] + 0.59 * sourcePointer[1] + 0.11 * sourcePointer[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).

                            sourcePointer2 = sourcePointer - (searchDirXIncrement * PIXEL_BYTE_WIDTH) - (searchDirYIncrement * sourceStride);
                            prevPixelGrayValue = (int)(0.3 * sourcePointer2[2] + 0.59 * sourcePointer2[1] + 0.11 * sourcePointer2[0]); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).

                            sumSqDiffs += Math.Pow(rowAnalysis.VariationFromPrevRow_raw - (pixelGrayValue - prevPixelGrayValue), 2);
                        }
                        rowAnalysis.StdDevOfVariation = Math.Sqrt(sumSqDiffs / rowWidth);
                        if (rowAnalysis.StdDevOfVariation > 0)
                        {
                            rowAnalysis.VariationScore = Math.Abs(rowAnalysis.VariationFromPrevRow_raw / rowAnalysis.StdDevOfVariation);
                        }
                        else
                        {
                            rowAnalysis.VariationScore = 0;
                        }
                    } // end unsafe section
                }
                else
                {
                    int x;
                    int y;
                    double sumSqDiffs = 0;
                    for (int widthIndex = 0; widthIndex < rowWidth; widthIndex++)
                    {
                        x = startXOffset + (rowAnalysis.SearchIndex * searchDirXIncrement) + (widthIndex * rowDirXIncrement);
                        y = startYOffset + (rowAnalysis.SearchIndex * searchDirYIncrement) + (widthIndex * rowDirYIncrement);

                        pixelGrayValue = mSourceImage.GetGrayValue(x, y);
                        prevPixelGrayValue = mSourceImage.GetGrayValue(x - searchDirXIncrement, y - searchDirYIncrement);

                        sumSqDiffs += Math.Pow(rowAnalysis.VariationFromPrevRow_raw - (pixelGrayValue - prevPixelGrayValue), 2);
                    }
                    rowAnalysis.StdDevOfVariation = Math.Sqrt(sumSqDiffs / rowWidth);
                    if (rowAnalysis.StdDevOfVariation > 0)
                    {
                        rowAnalysis.VariationScore = Math.Abs(rowAnalysis.VariationFromPrevRow_raw / rowAnalysis.StdDevOfVariation);
                    }
                    else
                    {
                        rowAnalysis.VariationScore = 0;
                    }
                }
            }
            return true; // TODO: fix
        }

        private void SetEdgeLocation(int edgeLocation)
        {
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
            //return edgeLocation;
        }

        private class Edge
        {
            public Edge(FindEdgeInstance theTool, int theEdgeNumber)
            {
                EdgeNumber = theEdgeNumber;
                mTool = theTool;
            }
            public FindEdgeInstance mTool;
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
                for (int rowNdx = transition.StartPos - 1; rowNdx <= transition.EndPos + 1; rowNdx++)
                {
                    switch (mTool.EdgeTypeSelectionFilter)
                    {
                        case FindEdgeDefinition.EdgeTypeSelectionFilters.BrightToDark:
                            if (rows[rowNdx].VariationFromPrevRow_raw < biggestChange)
                            {
                                biggestChange = rows[rowNdx].VariationFromPrevRow_raw * 1.2; // "*1.4" = we want the first big one...following ones need to beat the first by 40%
                                rowWithBiggestChange = rowNdx;
                            }
                            break;
                        case FindEdgeDefinition.EdgeTypeSelectionFilters.DarkToBright:
                            if (rows[rowNdx].VariationFromPrevRow_raw > biggestChange)
                            {
                                biggestChange = rows[rowNdx].VariationFromPrevRow_raw * 1.2; // "*1.4" = we want the first big one...following ones need to beat the first by 40%
                                rowWithBiggestChange = rowNdx;
                            }
                            break;
                        case FindEdgeDefinition.EdgeTypeSelectionFilters.NotDefined:
                            double absVariation = Math.Abs(rows[rowNdx].VariationFromPrevRow_raw);
                            if (absVariation > biggestChange)
                            {
                                biggestChange = absVariation * 1.2; // "*1.4" = we want the first big one...following ones need to beat the first by 40%
                                rowWithBiggestChange = rowNdx;
                            }
                            break;
                        default:
                            throw new ArgumentException("EdgeTypeSelectionFilter '" + mTool.EdgeTypeSelectionFilter + "' isn't supported by FindEdgeBasedOnVariation.");
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
                for (int rowNdx = transition.StartPos - 1; rowNdx <= transition.EndPos + 1; rowNdx++)
                {
                    double rowIntensity = rows[rowNdx].AverageValue;
                    if (Math.Abs(trailingSurface.AverageIntensity - rowIntensity) < Math.Abs(leadingSurface.AverageIntensity - rowIntensity))
                    {
                        // if this row is closer in intensity to the trailing surface (the one we're searching TOWARD) than the leading surface (search FROM), consider the PREVIOUS row the edge of surface 1
                        return rowNdx - 1;
                    }
                }
                return transition.EndPos;
            }
            public double FindSubPixelEdgeBasedOnIntensity(RowAnalysis[] rows, double targetSurfaceTransitionPercentage, int searchDirOffset, int searchDirIncrement)
            {
                if (leadingSurface == null || trailingSurface == null || transition == null)
                {
                    throw new ArgumentException("asldjiajdilasj");
                }
                double targetIntensity = leadingSurface.AverageIntensity + ((trailingSurface.AverageIntensity - leadingSurface.AverageIntensity) * (targetSurfaceTransitionPercentage/100.0));
                double rowIntensity;
                double lastRowIntensity = rows[transition.StartPos-1].AverageValue;
                for (int rowNdx = transition.StartPos; rowNdx <= transition.EndPos + 1; rowNdx++)
                {
                    rowIntensity = rows[rowNdx].AverageValue;
                    if (((mTool.EdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.DarkToBright || mTool.EdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.NotDefined) && lastRowIntensity <= targetIntensity && targetIntensity <= rowIntensity) ||
                        ((mTool.EdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.BrightToDark || mTool.EdgeTypeSelectionFilter == FindEdgeDefinition.EdgeTypeSelectionFilters.NotDefined) && lastRowIntensity >= targetIntensity && targetIntensity >= rowIntensity)
                        )
                    {
                        double subPixelComponent = Math.Abs(targetIntensity - lastRowIntensity) / Math.Abs(rowIntensity - lastRowIntensity);
                        double edgeLocation = searchDirOffset + ((rowNdx-1) * searchDirIncrement) + (subPixelComponent * searchDirIncrement);
                        return edgeLocation;
                    }
                    lastRowIntensity = rowIntensity;
                }
                return searchDirOffset + (transition.EndPos * searchDirIncrement);
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
                    intensity = rows[rowNdx].AverageValue;
                    variation = rows[rowNdx].VariationFromPrevRow_raw;
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
            //public double AverageVariation = 0;
            public double AverageValue = -1; // renamed to AverageValue from AverageIntensity 10/22/08 since we're actually (currently) using gray value
            public double StdDevOfVariation = -1;
            public double VariationScore = -1;

            public double VariationFromPrevRow_raw; // this row's avgVal - previous row's avgVal
            public double VariationFromPrevRow_used = 0; // the variation value used within algorithms...this may be identical to "_raw" or it may be filtered (e.g. 3RowAverage)

            // Before 10/22/08, I used a 3Row Average of Variation for the FindEdge algorithms, but this didn't work well for SHORT surfaces (eg small 3 row black gaps on sides of DS's UGDO buttons),
            // so I decided to make the 3Row Avearage optional.  To acomplish this, I changed AverageVariation_3Row to VariationFromPrevRow_used and depending on settings I either write the raw values in or compute a 3Row average
            // filters out noise and highlights edges by sticking out "merging" variations on neighboring rows

            // 3Row Average works well when you have fuzzy/slow transitions which occur over multiple rows, since the slow variations get "accumulated".
            // 3Row Average DOESN'T work well if SMALL surfaces are present with sharp transitions, since the transitions "blur" into the small surface and create noise...and prevent the variation from settling out long enough for the surface to be detected.

            // _used is used for detecting edges/transitions.  This may hold a copy of the raw values or filtered values such as the 3Row Average.
            // Regardless of filtering chosen, _raw is used within surface/edge analysis once surfaces are chosen.

            // I originally tested a 2Row Average, but I didn't like the 2row average since it skewed the transition a pixel
        }
    }
}

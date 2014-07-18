// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    class FindCornerFromInsideInstance : ToolInstance
    {
        public FindCornerFromInsideInstance(FindCornerFromInsideDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            //            mColorMatcher = testExecution.GetColorMatcher(theDefinition.ColorMatchDefinition);
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);
            mStartX = testExecution.DataValueRegistry.GetObject(theDefinition.StartX.Name);
            mStartY = testExecution.DataValueRegistry.GetObject(theDefinition.StartY.Name);
            mFollowingEdgeColorDefinition = testExecution.GetColorMatcher(theDefinition.FollowingEdgeColorDefinition.Name);
            mTargetEdgeColorDefinition = testExecution.GetColorMatcher(theDefinition.TargetEdgeColorDefinition.Name);
            mSearchBackgroundColorDefinition = testExecution.GetColorMatcher(theDefinition.SearchBackgroundColorDefinition.Name);
            mSearchDirection = theDefinition.SearchDirection;
            mTargetEdgeDirection = theDefinition.TargetEdgeDirection;
            mTargetEdgeWidth = testExecution.DataValueRegistry.GetObject(theDefinition.TargetEdgeWidth.Name);

            mResultX = new GeneratedValueInstance(theDefinition.ResultX, testExecution);
            mResultY = new GeneratedValueInstance(theDefinition.ResultY, testExecution);

            /*
            mSearchEndX = new GeneratedValueInstance(theDefinition.SearchEndX, testExecution);
            mSearchEndY = new GeneratedValueInstance(theDefinition.SearchEndY, testExecution);

            mSearchPath = new LineDecorationInstance(theDefinition.SearchPath, testExecution);
             */
        }

        private ColorMatchInstance mFollowingEdgeColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the color of the edge to search along")]
        public ColorMatchInstance FollowingEdgeColorDefinition
        {
            get { return mFollowingEdgeColorDefinition; }
        }

        private ColorMatchInstance mTargetEdgeColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the color of the perpendicular edge to search for")]
        public ColorMatchInstance TargetEdgeColorDefinition
        {
            get { return mTargetEdgeColorDefinition; }
        }

        private ColorMatchInstance mSearchBackgroundColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the background color to search through for the edge")]
        public ColorMatchInstance SearchBackgroundColorDefinition
        {
            get { return mSearchBackgroundColorDefinition; }
        }

        private DataValueInstance mStartX;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Position on x-axis to start searching. This should be on the Following Edge.")]
        public DataValueInstance StartX
        {
            get { return mStartX; }
        }

        private DataValueInstance mStartY;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Position on y-axis to start searching. This should be on the Following Edge.")]
        public DataValueInstance StartY
        {
            get { return mStartY; }
        }

        private Direction mSearchDirection;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The approximate direction to search along.")]
        public Direction SearchDirection
        {
            get { return mSearchDirection; }
        }

        private Direction mTargetEdgeDirection;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The approximate direction of the perpendicular target edge.")]
        public Direction TargetEdgeDirection
        {
            get { return mTargetEdgeDirection; }
        }

        private DataValueInstance mTargetEdgeWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The height/width of the perpendicular edge to search for (in pixels).  It must be at least this high to be considered found.")]
        public DataValueInstance TargetEdgeWidth
        {
            get { return mTargetEdgeWidth; }
        }


        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private GeneratedValueInstance mResultX = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance ResultX
        {
            get { return mResultX; }
        }

        private GeneratedValueInstance mResultY = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance ResultY
        {
            get { return mResultY; }
        }

        /*
        private LineDecorationInstance mSearchPath = null;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Shows the path that was searched.")]
        public LineDecorationInstance SearchPath
        {
            get { return mSearchPath; }
        }
        */

        public override bool IsComplete() { return mResultX.IsComplete() && mResultY.IsComplete(); }

        private double slope = 0;
        private double sumOfSlopes = 0;
        private int numSummedSlopes = 0;
        private double averageSlope = 0;
        private double bigSlope = 0;
        private bool foundFollowingEdge = false;
        private int followingEdgeDistance = 0;
        private bool abort = false;
        private bool foundTarget = false;
        private int x;
        private int y;
        private int xSearchChange;
        private int ySearchChange;
        private int xStepAwayChange;
        private int yStepAwayChange;
        private int searchDistFromFollowingEdge;
        private int searchLength;
        private int maxSearchLength; // this will be updated with the slope
        private int lastXOnFollowingEdge;
        private int lastYOnFollowingEdge;

        private Color pixelColor;

		public override void DoWork() 
		{
            /*
            if (!mStartX.IsComplete() ||
                !mStartY.IsComplete() ||
                !mSearchBackgroundColorDefinition.IsComplete() ||
                !mFollowingEdgeColorDefinition.IsComplete() ||
                !mTargetEdgeColorDefinition.IsComplete() ||
                !mTargetEdgeWidth.IsComplete() ||
                !mSourceImage.IsComplete() ||
                !AreExplicitDependenciesComplete()
                ) return;*/

            //MessageBox.Show("in find corner for " + Name);
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            int resultX = -1;
            int resultY = -1;

            if (mSourceImage.Bitmap == null)
            {
                TestExecution().LogMessage("ERROR: source image does not exist.");
            }
            else if( mStartX.ValueAsLong() < 0 || mStartX.ValueAsLong() >= mSourceImage.Bitmap.Width || mStartY.ValueAsLong() < 0 || mStartY.ValueAsLong() >= mSourceImage.Bitmap.Height)
            {
                TestExecution().LogMessage("ERROR: The search start point isn't valid: " + mStartX.ValueAsLong() + "," + mStartY.ValueAsLong());
            }
            else
            {
                switch (mSearchDirection)
                {
                    case Direction.Left:
                        xSearchChange = -1;
                        ySearchChange = 0;
                        break;
                    case Direction.Right:
                        xSearchChange = 1;
                        ySearchChange = 0;
                        break;
                    case Direction.Up:
                        xSearchChange = 0;
                        ySearchChange = -1;
                        break;
                    case Direction.Down:
                        xSearchChange = 0;
                        ySearchChange = 1;
                        break;
                    case Direction.NotDefined:
                        TestExecution().LogMessage("ERROR: Search direction not defined.");
                        abort = true;
                        break;
                    default:
                        TestExecution().LogMessage("ERROR: Unsupported Search direction; direction=" + mSearchDirection);
                        abort = true;
                        break;
                }
                switch (mTargetEdgeDirection)
                {
                    case Direction.Left:
                        xStepAwayChange = -1;
                        yStepAwayChange = 0;
                        break;
                    case Direction.Right:
                        xStepAwayChange = 1;
                        yStepAwayChange = 0;
                        break;
                    case Direction.Up:
                        xStepAwayChange = 0;
                        yStepAwayChange = -1;
                        break;
                    case Direction.Down:
                        xStepAwayChange = 0;
                        yStepAwayChange = 1;
                        break;
                    case Direction.NotDefined:
                        TestExecution().LogMessage("ERROR: Target Edge Direction not defined.");
                        abort = true;
                        break;
                    default:
                        TestExecution().LogMessage("ERROR: Unsupported Target Edge Direction; direction=" + mTargetEdgeDirection);
                        abort = true;
                        break;
                }

                x = (int)mStartX.ValueAsLong();
                y = (int)mStartY.ValueAsLong();

                pixelColor = mSourceImage.Bitmap.GetPixel(x, y);
                if (!mFollowingEdgeColorDefinition.Matches(pixelColor))
                {
                    TestExecution().LogMessage("ERROR: Start position isn't on the following edge.");
                    abort = true;
                    // TODO: try to find it by search in the opposite direction of the target edge for about 5-10 pixels.
                }

                lastXOnFollowingEdge = x;
                lastYOnFollowingEdge = y;
                searchDistFromFollowingEdge = Math.Min(10, (int)(mTargetEdgeWidth.ValueAsLong() * 0.75));
                searchLength = searchDistFromFollowingEdge;
                maxSearchLength = searchLength; // this will be updated with the slope
                int searchIndex = 0;

                TestExecution().LogMessage("Starting at " + x + "," + y + " searchLength=" + searchLength + "  searchDistFromFollowingEdge=" + searchDistFromFollowingEdge +  "  xStepAwayChange=" + xStepAwayChange + "  yStepAwayChange=" + yStepAwayChange + "  xSearchChange=" + xSearchChange  + "  ySearchChange=" + ySearchChange);

                abort = false;
                foundTarget = false;
                while (!foundTarget && !abort && x >= 0 && x < mSourceImage.Bitmap.Width && y >= 0 && y < mSourceImage.Bitmap.Height)
                {
                    //                LineDecorationInstance searchPath = new LineDecorationInstance(theDefinition.SearchPath, testExecution);

                    // move away from following edge and one pixel along it to start searching for the target edge.  We want to stay far enough away from the following edge so that we don't accidently run into it if it isn't not exactly parallel.
                    x = lastXOnFollowingEdge + xSearchChange + (xStepAwayChange * searchDistFromFollowingEdge);
                    y = lastYOnFollowingEdge + ySearchChange + (yStepAwayChange * searchDistFromFollowingEdge);

                    // move a certain distance (searchLength) along the Following Edge in search of the Target Edge
                    searchIndex = 0;
                    int consecutiveUnexpectedColors = 0;
                    int totalUnexpectedColors = 0;
                    while (!foundTarget && !abort && searchIndex < searchLength && x >= 0 && x < mSourceImage.Bitmap.Width && y >= 0 && y < mSourceImage.Bitmap.Height)
                    {
                        pixelColor = SourceImage.Bitmap.GetPixel(x, y);
                        if (mTargetEdgeColorDefinition.Matches(pixelColor))
                        {
                            consecutiveUnexpectedColors = 0;
                            TestExecution().LogMessage("Found target at x=" + x + "  y=" + y);
                            foundTarget = true;
                        }
                        else if (mFollowingEdgeColorDefinition.Matches(pixelColor))
                        {
                            // NOTE: must test this after the TargettedEdgeColor since these may be the exact same color defs!
                            TestExecution().LogMessage("ERROR: Unexpectedly ran into Following Edge.");
                            abort = true;
                            // TODO: does it also match the background? if so, throw an error about definition overlap
                            // TODO: handle differently if slope computed and we were near following edge?
                        }
                        else
                        {
                            if (!mSearchBackgroundColorDefinition.Matches(pixelColor))
                            {
                                TestExecution().LogMessage("WARNING: Ran into unexpected color at " + x + "," + y + ".");
                                totalUnexpectedColors++;
                                consecutiveUnexpectedColors++;
                            }
                            else
                            {
                                consecutiveUnexpectedColors = 0;
                            }

                            if (consecutiveUnexpectedColors > 2 || totalUnexpectedColors > 10)
                            {
                                abort = true;
                                TestExecution().LogMessage("ERROR: aborting due to too many unexpected colors; consecutive=" + consecutiveUnexpectedColors + "   total=" + totalUnexpectedColors);
                            }
                            else
                            {
                                x += xSearchChange;
                                y += ySearchChange;
                                searchIndex++;
                            }
                        }
                    }

                    if (!foundTarget && !abort)
                    {
                        TestExecution().LogMessage("Finished search stint at x=" + x + "  y=" + y + "; search length=" + searchLength + "  max=" + maxSearchLength);

                        ReFindFollowingEdge();

                        if (foundFollowingEdge)
                        {
                            ComputeSlope();
                        }
                    }
                }

                if (foundTarget)
                {
                    const int backupDistance = 5;
                    // back away 5 pixels from the Target Edge and then re-find the following edge.  From there we will estimate the corner
                    x -= xSearchChange * backupDistance;
                    y -= ySearchChange * backupDistance;

                    ReFindFollowingEdge();

                    if (foundFollowingEdge)
                    {
                        ComputeSlope();
                    }
                    else
                    {
                        TestExecution().LogMessage("WARNING: couldn't find Following Edge " + backupDistance + " pixel away from Target Edge");
                    }

                    if (numSummedSlopes > 0)
                    {
                        resultX = (int)(lastXOnFollowingEdge + backupDistance * xSearchChange + bigSlope * backupDistance * xStepAwayChange);
                        resultY = (int)(lastYOnFollowingEdge + backupDistance * ySearchChange + bigSlope * backupDistance * yStepAwayChange);
                    }
                    else
                    {
                        resultX = lastXOnFollowingEdge + backupDistance * xSearchChange;
                        resultY = lastYOnFollowingEdge + backupDistance * ySearchChange;
                    }
                }
                else
                {
                    TestExecution().LogMessage("ERROR: Couldn't find Target Edge. x=" + x + "  y=" + y);
                    abort = true;
                }
            }
            mResultX.SetValue(resultX);
            mResultY.SetValue(resultY);
            mResultX.SetIsComplete();
            mResultY.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessage("Corner at x=" + resultX + "  y=" + resultY);
            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
            //            mSearchEndX.SetIsComplete();
//            mSearchEndY.SetIsComplete();
            //MessageBox.Show("done in find corner for " + Name);
        }

        private void ReFindFollowingEdge()
        {
            // search toward the following edge and compute search slope
            foundFollowingEdge = false;
            followingEdgeDistance = 0;
            while (!foundFollowingEdge && !abort && x >= 0 && x < mSourceImage.Bitmap.Width && y >= 0 && y < mSourceImage.Bitmap.Height)
            {
                x -= xStepAwayChange;
                y -= yStepAwayChange;
                followingEdgeDistance++;
                if (followingEdgeDistance > searchDistFromFollowingEdge * 3)
                {
                    TestExecution().LogMessage("ERROR: Can't find following edge. x=" + x + "  y=" + y);
                    abort = true;
                }
                if ( !abort && x >= 0 && x < mSourceImage.Bitmap.Width && y >= 0 && y < mSourceImage.Bitmap.Height)
                {
                    pixelColor = SourceImage.Bitmap.GetPixel(x, y);
                    if (mFollowingEdgeColorDefinition.Matches(pixelColor))
                    {
                        TestExecution().LogMessage("Found following edge at x=" + x + "  y=" + y);
                        foundFollowingEdge = true;
                        lastXOnFollowingEdge = x;
                        lastYOnFollowingEdge = y;
                        if (mSearchDirection == Direction.Left || mSearchDirection == Direction.Right)
                        {
                            // TODO: should these equations be different between left & right?
                            bigSlope = (y - mStartY.ValueAsDecimal()) / (x - mStartX.ValueAsDecimal());
                        }
                        else
                        {
                            // TODO: should these equations be different between up & down?
                            bigSlope = (x - mStartX.ValueAsDecimal()) / (y - mStartY.ValueAsDecimal());
                        }
                    }
                    else if (mSearchBackgroundColorDefinition.Matches(pixelColor))
                    {
                        // do nothing...just loop
                    }
                    else
                    {
                        TestExecution().LogMessage("WARNING: ran into unknown color refinding Following Edge; x=" + x + " y=" + y + " color=" + pixelColor);
                    }
                }
            }
        }
        private void ComputeSlope()
        {
            // compute/update slope
            double slopeRise = followingEdgeDistance - searchDistFromFollowingEdge;
            slope = slopeRise / (double)searchLength;
            if (numSummedSlopes > 0)
            {
                if (Math.Abs(Math.Abs(slopeRise) - Math.Abs(averageSlope * searchLength)) > Math.Max(3, 0.10 * mTargetEdgeWidth.ValueAsDecimal()))
                {
                    TestExecution().LogMessage("WARNING: Huge slope change during search. average slope=" + averageSlope + "  (over " + numSummedSlopes + " slopes)   new=" + slope);
                }
            }
            numSummedSlopes++;
            sumOfSlopes += slope;
            averageSlope = sumOfSlopes / numSummedSlopes;
            TestExecution().LogMessage("Computed slope of " + slope + "; average=" + averageSlope + "  (count=" + numSummedSlopes + ")  bigSlope=" + bigSlope);
            if (numSummedSlopes > 2)
            {
                if (Math.Abs(averageSlope) > 0.001) // TODO: what is a good value?
                {
                    maxSearchLength = Math.Min(50, (int)Math.Abs(2.0 / averageSlope)); // we don't want to search any further than would generate a "2 pixel rise" of the following edge....we don't want to bump into the following edge if we aren't parallel to it!
                    searchLength = Math.Min(searchLength * 2, maxSearchLength);
                }
                else
                {
                    maxSearchLength = 50; // we don't want to search any further than would generate a "2 pixel rise" of the following edge....we don't want to bump into the following edge if we aren't parallel to it!
                    searchLength = 50;
                }
            }
        }
    }
}

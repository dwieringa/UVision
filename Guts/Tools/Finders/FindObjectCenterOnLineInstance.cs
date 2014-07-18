// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    class FindObjectCenterOnLineInstance : ToolInstance
    {
        public FindObjectCenterOnLineInstance(FindObjectCenterOnLineDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.ObjectColorDefinition == null) throw new ArgumentException(Name + " doesn't have ObjectColorDefinition defined.");
            mObjectColorDefinition = testExecution.GetColorMatcher(theDefinition.ObjectColorDefinition.Name);

            if (theDefinition.SearchBackgroundColorDefinition == null) throw new ArgumentException(Name + " doesn't have SearchBackgroundColorDefinition defined.");
            mSearchBackgroundColorDefinition = testExecution.GetColorMatcher(theDefinition.SearchBackgroundColorDefinition.Name);

            if (theDefinition.SearchPath == null) throw new ArgumentException(Name + " doesn't have SearchPath defined.");
            mSearchPath = testExecution.ObjectBasedLineDecorationRegistry.GetObject(theDefinition.SearchPath.Name);

            if (theDefinition.SearchSpeed == null) throw new ArgumentException(Name + " doesn't have SearchSpeed defined.");
            mSearchSpeed = testExecution.DataValueRegistry.GetObject(theDefinition.SearchSpeed.Name);

            if (theDefinition.RequiredConsecutiveColorMatches == null) throw new ArgumentException(Name + " doesn't have RequiredConsecutiveColorMatches defined.");
            mRequiredConsecutiveColorMatches = testExecution.DataValueRegistry.GetObject(theDefinition.RequiredConsecutiveColorMatches.Name);

            if (theDefinition.SearchDirection == Direction.NotDefined) throw new ArgumentException(Name + " doesn't have SearchDirection defined.");
            mSearchDirection = theDefinition.SearchDirection;

            mResultX = new GeneratedValueInstance(theDefinition.ResultX, testExecution);
            mResultY = new GeneratedValueInstance(theDefinition.ResultY, testExecution);
        }

        private ColorMatchInstance mObjectColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public ColorMatchInstance ObjectColorDefinition
        {
            get { return mObjectColorDefinition; }
        }

        private ColorMatchInstance mSearchBackgroundColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the background color to search through for the object")]
        public ColorMatchInstance SearchBackgroundColorDefinition
        {
            get { return mSearchBackgroundColorDefinition; }
        }

        private ObjectBasedLineDecorationInstance mSearchPath = null;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Shows the path that was searched.")]
        public LineDecorationInstance SearchPath
        {
            get { return mSearchPath; }
        }

        private Direction mSearchDirection;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The approximate direction to search along.")]
        public Direction SearchDirection
        {
            get { return mSearchDirection; }
        }

        private DataValueInstance mSearchSpeed;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueInstance SearchSpeed
        {
            get { return mSearchSpeed; }
        }

        private DataValueInstance mRequiredConsecutiveColorMatches;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The number of consecutive pixels that must match the color being searched for at object edges.")]
        public DataValueInstance RequiredConsecutiveColorMatches
        {
            get { return mRequiredConsecutiveColorMatches; }
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

        public override bool IsComplete() { return mResultX.IsComplete() && mResultY.IsComplete(); }

        private enum SearchState
        {
            FindBackground = 0,
            FindObject = 1,
            FindFarEdgeOfObject = 2,
            Done = 3
        }

        private double slope = 0;
        private bool abort = false;
        private SearchState state = 0;
        private int x;
        private int y;
        private int xSearchChange;
        private int ySearchChange;
        private long startX = -1;
        private long startY = -1;
        private long endX = -1;
        private long endY = -1;
        private long leftEdgeOfSearch = -1;
        private long rightEdgeOfSearch = -1;
        private long topEdgeOfSearch = -1;
        private long bottomEdgeOfSearch = -1;

        private Color pixelColor;

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

        private enum LineType
        {
            Horizontal = 0,
            Vertical = 1,
            Slanted = 2
        }

		public override void DoWork() 
		{
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            int objectStartEdgeX = -1;
            int objectStartEdgeY = -1;
            int objectEndEdgeX = -1;
            int objectEndEdgeY = -1;
            int resultX = -1;
            int resultY = -1;

            long requiredConsecMatches = mRequiredConsecutiveColorMatches.ValueAsLong();
            int consecutiveMatches = 0;
            int firstMatch_x = -1;
            int firstMatch_y = -1;

            if (mSourceImage.Bitmap == null)
            {
                TestExecution().LogMessage("ERROR: source image for '" + Name + "' does not exist.");
            }
            else if (mSearchPath == null)
            {
                TestExecution().LogMessage("ERROR: search line for '" + Name + "' isn't defined.");
            }
            else if (mSearchPath.StartX == null || mSearchPath.StartY == null || mSearchPath.EndX == null || mSearchPath.EndY == null)
            {
                TestExecution().LogMessage("ERROR: search line '" + mSearchPath.Name + "' for '" + Name + "' isn't fully defined.");
            }
            else if (mSearchPath.StartX.ValueAsLong() < 0 || mSearchPath.StartX.ValueAsLong() >= mSourceImage.Bitmap.Width ||
                mSearchPath.StartY.ValueAsLong() < 0 || mSearchPath.StartY.ValueAsLong() >= mSourceImage.Bitmap.Height)
            {
                TestExecution().LogMessage("ERROR: The search line start point for '" + Name + "' isn't valid: " + mSearchPath.StartX.ValueAsLong() + "," + mSearchPath.StartY.ValueAsLong());
            }
            else if (mSearchPath.EndX.ValueAsLong() < 0 || mSearchPath.EndX.ValueAsLong() >= mSourceImage.Bitmap.Width ||
                mSearchPath.EndY.ValueAsLong() < 0 || mSearchPath.EndY.ValueAsLong() >= mSourceImage.Bitmap.Height)
            {
                TestExecution().LogMessage("ERROR: The search line end point for '" + Name + "' isn't valid: " + mSearchPath.EndX.ValueAsLong() + "," + mSearchPath.EndY.ValueAsLong());
            }
            else
            {
                switch (mSearchDirection)
                {
                    case Direction.Left:
                        xSearchChange = -1;
                        ySearchChange = 0;
                        if (mSearchPath.StartX.ValueAsLong() == mSearchPath.EndX.ValueAsLong() && mSearchPath.StartY.ValueAsLong() != mSearchPath.EndY.ValueAsLong())
                        {
                            TestExecution().LogMessage("ERROR: can't search left on line '" + mSearchPath.Name + "' for '" + Name + "' since line is vertical.");
                        }
                        else if (mSearchPath.StartX.ValueAsLong() < mSearchPath.EndX.ValueAsLong())
                        {
                            startX = mSearchPath.EndX.ValueAsLong();
                            startY = mSearchPath.EndY.ValueAsLong();
                            endX = mSearchPath.StartX.ValueAsLong();
                            endY = mSearchPath.StartY.ValueAsLong();
                        }
                        else
                        {
                            startX = mSearchPath.StartX.ValueAsLong();
                            startY = mSearchPath.StartY.ValueAsLong();
                            endX = mSearchPath.EndX.ValueAsLong();
                            endY = mSearchPath.EndY.ValueAsLong();
                        }
                        break;
                    case Direction.Right:
                        xSearchChange = 1;
                        ySearchChange = 0;
                        if (mSearchPath.StartX.ValueAsLong() == mSearchPath.EndX.ValueAsLong() && mSearchPath.StartY.ValueAsLong() != mSearchPath.EndY.ValueAsLong())
                        {
                            TestExecution().LogMessage("ERROR: can't search right on line '" + mSearchPath.Name + "' for '" + Name + "' since line is vertical.");
                        }
                        else if (mSearchPath.StartX.ValueAsLong() < mSearchPath.EndX.ValueAsLong())
                        {
                            startX = mSearchPath.StartX.ValueAsLong();
                            startY = mSearchPath.StartY.ValueAsLong();
                            endX = mSearchPath.EndX.ValueAsLong();
                            endY = mSearchPath.EndY.ValueAsLong();
                        }
                        else
                        {
                            startX = mSearchPath.EndX.ValueAsLong();
                            startY = mSearchPath.EndY.ValueAsLong();
                            endX = mSearchPath.StartX.ValueAsLong();
                            endY = mSearchPath.StartY.ValueAsLong();
                        }
                        break;
                    case Direction.Up:
                        xSearchChange = 0;
                        ySearchChange = -1;
                        if (mSearchPath.StartY.ValueAsLong() == mSearchPath.EndY.ValueAsLong() && mSearchPath.StartX.ValueAsLong() != mSearchPath.EndX.ValueAsLong())
                        {
                            TestExecution().LogMessage("ERROR: can't search up on line '" + mSearchPath.Name + "' for '" + Name + "' since line is horizontal.");
                        }
                        else if (mSearchPath.StartY.ValueAsLong() < mSearchPath.EndY.ValueAsLong()) // line is down
                        {
                            startX = mSearchPath.EndX.ValueAsLong();
                            startY = mSearchPath.EndY.ValueAsLong();
                            endX = mSearchPath.StartX.ValueAsLong();
                            endY = mSearchPath.StartY.ValueAsLong();
                        }
                        else // line is up
                        {
                            startX = mSearchPath.StartX.ValueAsLong();
                            startY = mSearchPath.StartY.ValueAsLong();
                            endX = mSearchPath.EndX.ValueAsLong();
                            endY = mSearchPath.EndY.ValueAsLong();
                        }
                        break;
                    case Direction.Down:
                        xSearchChange = 0;
                        ySearchChange = 1;
                        if (mSearchPath.StartY.ValueAsLong() == mSearchPath.EndY.ValueAsLong() && mSearchPath.StartX.ValueAsLong() != mSearchPath.EndX.ValueAsLong())
                        {
                            TestExecution().LogMessage("ERROR: can't search down on line '" + mSearchPath.Name + "' for '" + Name + "' since line is horizontal.");
                        }
                        else if (mSearchPath.StartY.ValueAsLong() < mSearchPath.EndY.ValueAsLong()) // line is down
                        {
                            startX = mSearchPath.StartX.ValueAsLong();
                            startY = mSearchPath.StartY.ValueAsLong();
                            endX = mSearchPath.EndX.ValueAsLong();
                            endY = mSearchPath.EndY.ValueAsLong();
                        }
                        else // line is up
                        {
                            startX = mSearchPath.EndX.ValueAsLong();
                            startY = mSearchPath.EndY.ValueAsLong();
                            endX = mSearchPath.StartX.ValueAsLong();
                            endY = mSearchPath.StartY.ValueAsLong();
                        }
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

                leftEdgeOfSearch = Math.Max(0,Math.Min( startX, endX));
                rightEdgeOfSearch = Math.Min(mSourceImage.Bitmap.Width, Math.Max( startX, endX));
                topEdgeOfSearch = Math.Max( 0, Math.Min( startY, endY));
                bottomEdgeOfSearch = Math.Min( mSourceImage.Bitmap.Height, Math.Max( startY, endY));

                LineType lineType;
                if( startY == endY )
                {
                    lineType = LineType.Horizontal;
                    slope = 0;
                }
                else if (startX == endX)
                {
                    lineType = LineType.Vertical;
                    slope = 999999999999 / 0.000000001;
                }
                else
                {
                    lineType = LineType.Slanted;
                    slope = (double)(endY - startY) / (double)(endX - startX);
                }

                x = (int)startX;
                y = (int)startY;

                TestExecution().LogMessage(Name + " starting at " + x + "," + y );

                abort = false;
                state = SearchState.FindBackground;
                int searchIndex = 0;
                while (state != SearchState.Done && !abort)
                {
                    switch (lineType)
                    {
                        case LineType.Horizontal:
                            x = (int)(startX + (searchIndex * xSearchChange) );
                            break;
                        case LineType.Vertical:
                            y = (int)(startY + (searchIndex * ySearchChange));
                            break;
                        case LineType.Slanted:
                            x = (int)(startX + (searchIndex * xSearchChange) + ((searchIndex * ySearchChange) / slope));
                            y = (int)(startY + (searchIndex * ySearchChange) + ((searchIndex * xSearchChange) * slope));
                            break;
                    }

                    if( x < leftEdgeOfSearch || x > rightEdgeOfSearch || y < topEdgeOfSearch || y > bottomEdgeOfSearch )
                    {
                        TestExecution().LogMessage("ERROR: " + Name + " exhausted search without finding full object; end position = " + x + "," + y + "; state = " + state);
                        state = SearchState.Done;
                    }
                    else
                    {
                        pixelColor = SourceImage.Bitmap.GetPixel(x, y);

                        switch (state)
                        {
                            case SearchState.FindBackground:
                                if (mSearchBackgroundColorDefinition.Matches(pixelColor))
                                {
                                    TestExecution().LogMessage(Name + " found background at " + x + "," + y);
                                    state = SearchState.FindObject;
                                }
                                else if (mObjectColorDefinition.Matches(pixelColor))
                                {
                                }
                                else
                                {
                                    LogWarning(Name + " found unexpected color searching for initial background at " + x + "," + y);
                                }
                                break;
                            case SearchState.FindObject:
                                if (mSearchBackgroundColorDefinition.Matches(pixelColor))
                                {
                                    consecutiveMatches = 0;
                                    firstMatch_x = -1;
                                    firstMatch_y = -1;
                                }
                                else if (mObjectColorDefinition.Matches(pixelColor))
                                {
                                    TestExecution().LogMessage(Name + " found start of object at " + x + "," + y);
                                    consecutiveMatches++;
                                    if (consecutiveMatches == 1)
                                    {
                                        firstMatch_x = x;
                                        firstMatch_y = y;
                                    }
                                    if (consecutiveMatches >= requiredConsecMatches)
                                    {
                                        // remember edge
                                        objectStartEdgeX = firstMatch_x;
                                        objectStartEdgeY = firstMatch_y;
                                        TestExecution().LogMessage(Name + " found the " + requiredConsecMatches + " needed consec matches for start of object at " + x + "," + y + "; set start to " + objectStartEdgeX + "," + objectStartEdgeY);

                                        // get ready for next state
                                        state = SearchState.FindFarEdgeOfObject;
                                        consecutiveMatches = 0;
                                        firstMatch_x = -1;
                                        firstMatch_y = -1;
                                    }
                                }
                                else
                                {
                                    consecutiveMatches = 0;
                                    firstMatch_x = -1;
                                    firstMatch_y = -1;
                                    LogWarning(Name + " found unexpected color before object at " + x + "," + y);
                                }
                                break;
                            case SearchState.FindFarEdgeOfObject:
                                if (mSearchBackgroundColorDefinition.Matches(pixelColor))
                                {
                                    TestExecution().LogMessage(Name + " found end of object at " + x + "," + y);
                                    consecutiveMatches++;
                                    if (consecutiveMatches == 1)
                                    {
                                        firstMatch_x = x;
                                        firstMatch_y = y;
                                    }
                                    if (consecutiveMatches >= requiredConsecMatches)
                                    {
                                        // remember edge
                                        objectEndEdgeX = firstMatch_x;
                                        objectEndEdgeY = firstMatch_y;
                                        resultX = (objectStartEdgeX + objectEndEdgeX) / 2;
                                        resultY = (objectStartEdgeY + objectEndEdgeY) / 2;
                                        TestExecution().LogMessage(Name + " found the " + requiredConsecMatches + " needed consec matches for end of object at " + x + "," + y + "; set end to " + objectEndEdgeX + "," + objectEndEdgeY);

                                        // get ready for next state
                                        state = SearchState.Done;
                                        consecutiveMatches = 0;
                                        firstMatch_x = -1;
                                        firstMatch_y = -1;
                                    }
                                }
                                else if (mObjectColorDefinition.Matches(pixelColor))
                                {
                                    consecutiveMatches = 0;
                                    firstMatch_x = -1;
                                    firstMatch_y = -1;
                                }
                                else
                                {
                                    consecutiveMatches = 0;
                                    firstMatch_x = -1;
                                    firstMatch_y = -1;
                                    LogWarning(Name + " found unexpected color within object at " + x + "," + y);
                                }
                                break;
                        } // end switch
                        searchIndex++;
                    } // end if for x,y verification
                } // end search loop
            } // end main block ("else" after all initial setup error checks)
            mResultX.SetValue(resultX);
            mResultY.SetValue(resultY);
            mResultX.SetIsComplete();
            mResultY.SetIsComplete();
            DateTime doneTime = DateTime.Now;
            TimeSpan computeTime = doneTime - startTime;
            TestExecution().LogMessage(Name + " computed object center at " + resultX + "," + resultY);
            TestExecution().LogMessageWithTimeFromTrigger(Name + " finished at " + doneTime + "  | took " + computeTime.TotalMilliseconds + "ms");
        }

    }
}

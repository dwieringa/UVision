// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    class FindCenterOfColorOnLineInstance : ToolInstance
    {
        public FindCenterOfColorOnLineInstance(FindCenterOfColorOnLineDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.SearchColorDefinition == null) throw new ArgumentException(Name + " doesn't have ObjectColorDefinition defined.");
            mSearchColorDefinition = testExecution.GetColorMatcher(theDefinition.SearchColorDefinition.Name);

            if (theDefinition.SearchPath == null) throw new ArgumentException(Name + " doesn't have SearchPath defined.");
            mSearchPath = testExecution.ObjectBasedLineDecorationRegistry.GetObject(theDefinition.SearchPath.Name);

            //if (theDefinition.RequiredConsecutiveColorMatches != null)
            //{
            //    mRequiredConsecutiveColorMatches = testExecution.DataValueRegistry.GetObject(theDefinition.RequiredConsecutiveColorMatches.Name);
            //}

            mResultX = new GeneratedValueInstance(theDefinition.ResultX, testExecution);
            mResultY = new GeneratedValueInstance(theDefinition.ResultY, testExecution);
        }

        private ColorMatchInstance mSearchColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public ColorMatchInstance ObjectColorDefinition
        {
            get { return mSearchColorDefinition; }
        }

        private ObjectBasedLineDecorationInstance mSearchPath = null;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Shows the path that was searched.")]
        public LineDecorationInstance SearchPath
        {
            get { return mSearchPath; }
        }

        /*
        private DataValueInstance mRequiredConsecutiveColorMatches;
        [CategoryAttribute("Parameters")]
        public DataValueInstance RequiredConsecutiveColorMatches
        {
            get { return mRequiredConsecutiveColorMatches; }
        }
        */

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

        private bool abort = false;
        //private SearchState state = 0;
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
            // NOTE: this code was adapted from FindObjectCenterOnLine; one difference is that it searched until it found the object, then quit.  We search the entire path
            DateTime startTime = DateTime.Now;
            TestExecution().LogMessageWithTimeFromTrigger("[" + Name + "] started at " + startTime + Environment.NewLine);

            int resultX = -1;
            int resultY = -1;

            if (mSourceImage.Bitmap == null)
            {
                TestExecution().LogErrorWithTimeFromTrigger("source image for '" + Name + "' does not exist.");
            }
            else if (mSearchPath == null)
            {
                TestExecution().LogErrorWithTimeFromTrigger("search line for '" + Name + "' isn't defined.");
            }
            else if (mSearchPath.StartX == null || mSearchPath.StartY == null || mSearchPath.EndX == null || mSearchPath.EndY == null)
            {
                TestExecution().LogErrorWithTimeFromTrigger("search line '" + mSearchPath.Name + "' for '" + Name + "' isn't fully defined.");
            }
            else if (mSearchPath.StartX.ValueAsLong() < 0 || mSearchPath.StartX.ValueAsLong() >= mSourceImage.Bitmap.Width ||
                mSearchPath.StartY.ValueAsLong() < 0 || mSearchPath.StartY.ValueAsLong() >= mSourceImage.Bitmap.Height)
            {
                TestExecution().LogErrorWithTimeFromTrigger("The search line start point for '" + Name + "' isn't valid: " + mSearchPath.StartX.ValueAsLong() + "," + mSearchPath.StartY.ValueAsLong());
            }
            else if (mSearchPath.EndX.ValueAsLong() < 0 || mSearchPath.EndX.ValueAsLong() >= mSourceImage.Bitmap.Width ||
      mSearchPath.EndY.ValueAsLong() < 0 || mSearchPath.EndY.ValueAsLong() >= mSourceImage.Bitmap.Height)
            {
                TestExecution().LogErrorWithTimeFromTrigger("The search line end point for '" + Name + "' isn't valid: " + mSearchPath.EndX.ValueAsLong() + "," + mSearchPath.EndY.ValueAsLong());
            }
            else if (Math.Abs(mSearchPath.StartX.ValueAsLong() - mSearchPath.EndX.ValueAsLong()) < 1 && Math.Abs(mSearchPath.StartY.ValueAsLong() - mSearchPath.EndY.ValueAsLong()) < 1)
            {
                TestExecution().LogErrorWithTimeFromTrigger("Search path is too small.");
            }
            else
            {
                long width = Math.Abs(mSearchPath.StartX.ValueAsLong() - mSearchPath.EndX.ValueAsLong()) + 1;
                long height = Math.Abs(mSearchPath.StartY.ValueAsLong() - mSearchPath.EndY.ValueAsLong()) + 1;
                double run;
                double rise;
                double angle;
                double sineOfAngle = -1;
                double cosineOfAngle = -1;
                LineType lineType;
                long length;
                startX = mSearchPath.StartX.ValueAsLong();
                startY = mSearchPath.StartY.ValueAsLong();
                endX = mSearchPath.EndX.ValueAsLong();
                endY = mSearchPath.EndY.ValueAsLong();
                if (mSearchPath.StartY.ValueAsLong() == mSearchPath.EndY.ValueAsLong()) // if it is horizonal line (no Y deviation)
                {
                    lineType = LineType.Horizontal;
                    length = width;
                    ySearchChange = 0;
                    if (mSearchPath.StartX.ValueAsLong() < mSearchPath.EndX.ValueAsLong())
                    {
                        xSearchChange = 1;
                    }
                    else
                    {
                        xSearchChange = -1;
                    }
                }
                else if (mSearchPath.StartX.ValueAsLong() == mSearchPath.EndX.ValueAsLong()) // if it is vertical line (no X deviation)
                {
                    lineType = LineType.Vertical;
                    length = height;
                    xSearchChange = 0;
                    if (mSearchPath.StartY.ValueAsLong() < mSearchPath.EndY.ValueAsLong()) // line is down
                    {
                        ySearchChange = 1;
                    }
                    else // line is up
                    {
                        ySearchChange = -1;
                    }
                }
                else // slanted line
                {
                    run = mSearchPath.EndX.ValueAsLong() - mSearchPath.StartX.ValueAsLong();
                    rise = mSearchPath.EndY.ValueAsLong() - mSearchPath.StartY.ValueAsLong();
                    angle = Math.Atan(rise / run);
                    sineOfAngle = Math.Sin(angle);
                    cosineOfAngle = Math.Cos(angle);
                    lineType = LineType.Slanted;
                    length = (long)Math.Sqrt(height * height + width * width);

                }
                leftEdgeOfSearch = Math.Max(0, Math.Min(startX, endX));
                rightEdgeOfSearch = Math.Min(mSourceImage.Bitmap.Width, Math.Max(startX, endX));
                topEdgeOfSearch = Math.Max(0, Math.Min(startY, endY));
                bottomEdgeOfSearch = Math.Min(mSourceImage.Bitmap.Height, Math.Max(startY, endY));

                x = (int)startX;
                y = (int)startY;
                TestExecution().LogMessage(Name + " starting at " + x + "," + y);

                abort = false;
                Point firstPoint = new Point(-1, -1);
                Point lastPoint = new Point(-1, -1);
                for (int searchIndex = 0; searchIndex <= length && !abort; searchIndex++ )
                {
                    switch (lineType)
                    {
                        case LineType.Horizontal:
                            x = (int)(startX + (searchIndex * xSearchChange));
                            break;
                        case LineType.Vertical:
                            y = (int)(startY + (searchIndex * ySearchChange));
                            break;
                        case LineType.Slanted:
                            x = (int)(startX + Math.Round(searchIndex * cosineOfAngle));
                            y = (int)(startY + Math.Round(searchIndex * sineOfAngle));
                            break;
                    }

                    if (x < leftEdgeOfSearch || x > rightEdgeOfSearch || y < topEdgeOfSearch || y > bottomEdgeOfSearch)
                    {
                        TestExecution().LogErrorWithTimeFromTrigger(Name + " aborting at " + x + "," + y + ". Out of range from path for some reason.");
                        abort = true;
                    }
                    else
                    {
                        pixelColor = mSourceImage.GetColor(x, y);
                        if (mSearchColorDefinition.Matches(pixelColor))
                        {
                            TestExecution().LogMessage(Name + " found color match at " + x + "," + y);
                            if (firstPoint.X < 0)
                            {
                                firstPoint.X = x;
                                firstPoint.Y = y;
                            }
                            lastPoint.X = x;
                            lastPoint.Y = y;
                        }
                        else
                        {
                            //mSourceImage.SetColor(x, y, Color.Lime);
                            //TestExecution().LogMessage(Name + " " + x + "," + y + " is not a match");
                        }

                    } // end if for x,y verification
                } // end search loop
                resultX = (firstPoint.X + lastPoint.X) / 2;
                resultY = (firstPoint.Y + lastPoint.Y) / 2;
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

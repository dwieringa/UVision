// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class FindColorOnLineInstance : ToolInstance
    {
        public FindColorOnLineInstance(FindColorOnLineDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
            if (theDefinition.ColorMatchDefinition == null) throw new ArgumentException(Name + " doesn't have ColorMatchDefinition defined.");
            mColorMatcher = testExecution.GetColorMatcher(theDefinition.ColorMatchDefinition.Name);

            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.StartX == null) throw new ArgumentException(Name + " doesn't have StartX defined.");
            mStartX = testExecution.DataValueRegistry.GetObject(theDefinition.StartX.Name);

            if (theDefinition.StartY == null) throw new ArgumentException(Name + " doesn't have StartY defined.");
            mStartY = testExecution.DataValueRegistry.GetObject(theDefinition.StartY.Name);

            if (theDefinition.SlopeRise == null) throw new ArgumentException(Name + " doesn't have SlopeRise defined.");
            mSlopeRise = testExecution.DataValueRegistry.GetObject(theDefinition.SlopeRise.Name);

            if (theDefinition.SlopeRun == null) throw new ArgumentException(Name + " doesn't have SlopeRun defined.");
            mSlopeRun = testExecution.DataValueRegistry.GetObject(theDefinition.SlopeRun.Name);

            if (theDefinition.RequiredConsecutivePixels == null) throw new ArgumentException(Name + " doesn't have RequiredConsecutivePixels defined.");
            mRequiredConsecutivePixels = testExecution.DataValueRegistry.GetObject(theDefinition.RequiredConsecutivePixels.Name);

            mResultX = new GeneratedValueInstance(theDefinition.ResultX, testExecution);
            mResultY = new GeneratedValueInstance(theDefinition.ResultY, testExecution);

            mSearchEndX = new GeneratedValueInstance(theDefinition.SearchEndX, testExecution);
            mSearchEndY = new GeneratedValueInstance(theDefinition.SearchEndY, testExecution);

            mSearchPath = new ObjectBasedLineDecorationInstance(theDefinition.SearchPath, testExecution);
        }

        private ColorMatchInstance mColorMatcher;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Color match rules")]
        public ColorMatchInstance ColorMatchDefinition
        {
            get { return mColorMatcher; }
        }

        private DataValueInstance mStartX;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Position on x-axis to start searching")]
        public DataValueInstance StartX
        {
            get { return mStartX; }
        }

        private DataValueInstance mStartY;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Position on y-axis to start searching")]
        public DataValueInstance StartY
        {
            get { return mStartY; }
        }

        private DataValueInstance mSlopeRise;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Rise of the slope over which to search")]
        public DataValueInstance SlopeRise
        {
            get { return mSlopeRise; }
        }

        private DataValueInstance mSlopeRun;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Run of the slope over which to search")]
        public DataValueInstance SlopeRun
        {
            get { return mSlopeRun; }
        }

        private DataValueInstance mRequiredConsecutivePixels;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Number of pixels in a row that must match the color")]
        public DataValueInstance RequiredConsecutivePixels
        {
            get { return mRequiredConsecutivePixels; }
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

        private GeneratedValueInstance mSearchEndX;
        [CategoryAttribute("Output"),
        DescriptionAttribute("The last pixel that was tested by the search")]
        public GeneratedValueInstance SearchEndX
        {
            get { return mSearchEndX; }
        }

        private GeneratedValueInstance mSearchEndY;
        [CategoryAttribute("Output"),
        DescriptionAttribute("The last pixel that was tested by the search")]
        public GeneratedValueInstance SearchEndY
        {
            get { return mSearchEndY; }
        }

        private ObjectBasedLineDecorationInstance mSearchPath = null;
        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Shows the path that was searched.")]
        public LineDecorationInstance SearchPath
        {
            get { return mSearchPath; }
        }
        
        public override bool IsComplete() { return mResultX.IsComplete() && mResultY.IsComplete(); }

		public override void DoWork() 
		{
            /*
            if (!mStartX.IsComplete() ||
                !mStartY.IsComplete() ||
                !mSlopeRise.IsComplete() ||
                !mSlopeRun.IsComplete() ||
                !mRequiredConsecutivePixels.IsComplete() ||
                !mColorMatcher.IsComplete() ||
                !mSourceImage.IsComplete() ||
                !AreExplicitDependenciesComplete()
                ) return;*/

            TestExecution().LogMessageWithTimeFromTrigger("FindColorOnLine " + Name + " started");

            int resultX = -1;
            int resultY = -1;
            if (mSourceImage.Bitmap != null)
            {
                Color pixelColor;
                int consecutivePixels = 0;
                bool done = false;
                int x = (int)mStartX.ValueAsLong();
                int y = (int)mStartY.ValueAsLong();
                int rise = (int)mSlopeRise.ValueAsLong();
                int run = (int)mSlopeRun.ValueAsLong();
                TestExecution().LogMessageWithTimeFromTrigger(Name + " starting searching at " + x + "," + y);
                while (!done && x >= 0 && x < mSourceImage.Bitmap.Width && y >= 0 && y < mSourceImage.Bitmap.Height)
                {
                    pixelColor = SourceImage.Bitmap.GetPixel(x, y);
                    if (mColorMatcher.Matches(pixelColor))
                    {
                        if (consecutivePixels == 0)
                        {
                            resultX = x;
                            resultY = y;
                            TestExecution().LogMessageWithTimeFromTrigger(Name + " found 1st match at " + x + "," + y);
                        }
                        consecutivePixels++;
                        if (consecutivePixels >= mRequiredConsecutivePixels.ValueAsLong())
                        {
                            TestExecution().LogMessageWithTimeFromTrigger(Name + " found last consecutive match at " + x + "," + y);
                            done = true;
                        }
                    }
                    else
                    {
                        consecutivePixels = 0;
                        resultX = -2;
                        resultY = -2;
                    }

                    mSearchEndX.SetValue(x);
                    mSearchEndY.SetValue(y);

                    x += run;
                    y += rise;
                }
            }
            else
            {
                mSearchEndX.SetValue(-1);
                mSearchEndY.SetValue(-1);
            }

            mResultX.SetValue( resultX );
            mResultY.SetValue( resultY );

            mResultX.SetIsComplete();
            mResultY.SetIsComplete();
            mSearchEndX.SetIsComplete();
            mSearchEndY.SetIsComplete();

            TestExecution().LogMessageWithTimeFromTrigger("FindColorOnLine " + Name + " completed; result=" + resultX + "," + resultY);
        }
    }
}

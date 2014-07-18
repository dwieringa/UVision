// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public abstract class RectangleROIBaseInstance : ROIInstance, IRectangleROIInstance
    {
        protected Color color;

        public RectangleROIBaseInstance(RectangleROIBaseDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            color = theDefinition.Color;
        }

        public override void Draw(System.Drawing.Graphics g, PictureBoxScale scale)
        {
            Pen pen = new Pen(color, 1);
            g.DrawRectangle(pen, (mUpperLeftCorner.X * scale.XScale) + scale.XOffset, (mUpperLeftCorner.Y * scale.YScale) + scale.YOffset, (mLowerRightCorner.X - mUpperLeftCorner.X) * scale.XScale, (mLowerRightCorner.Y - mUpperLeftCorner.Y) * scale.YScale);
            pen.Dispose();
        }

        public override bool IsComplete()
        {
            return mUpperLeftCorner != Point.Empty && mLowerRightCorner != Point.Empty;
        }

        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public int Left
        {
            get { return mUpperLeftCorner.X; }
        }
        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public int Right
        {
            get { return mLowerRightCorner.X; }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public int Top
        {
            get { return mUpperLeftCorner.Y; }
        }
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public int Bottom
        {
            get { return mLowerRightCorner.Y; }
        }
        
        // NOTE: we're passing in the point as reference for speed (avoid creating new Point objects) and for thread safety
        public override Point GetFirstPointOnXAxis(ImageInstance theImage, ref Point theFirstPoint)
        {
            if (mUpperLeftCorner.Y >= theImage.Height ||
                mUpperLeftCorner.X >= theImage.Width ||
                mLowerRightCorner.Y < 0 ||
                mLowerRightCorner.X < 0)
            {
                theFirstPoint.X = -1;
                theFirstPoint.Y = -1;
                return theFirstPoint;
            }

            theFirstPoint.X = Math.Max(0, mUpperLeftCorner.X);
            theFirstPoint.Y = Math.Max(0, mUpperLeftCorner.Y);
            return theFirstPoint;
        }
        public override Point GetFirstPointOnYAxis(ImageInstance theImage, ref Point theFirstPoint)
        {
            return GetFirstPointOnXAxis(theImage, ref theFirstPoint);
        }

        protected Point mUpperLeftCorner = Point.Empty; // this is for optimization while iterating over the recantangle (so we don't have to repeated compute these values due to padding)
        protected Point mLowerRightCorner = Point.Empty; // this is for optimization while iterating over the recantangle (so we don't have to repeated compute these values due to padding)
        /*
        public override Point GetNextPoint(ref Point theNextPoint)
        {
            return GetNextPointOnXAxis(ref theNextPoint);
        }
        */
        public override Point GetNextPointOnXAxis(ImageInstance theImage, ref Point theNextPoint)
        {
            theNextPoint.X++;
            if (theNextPoint.X <= mLowerRightCorner.X && theNextPoint.X < theImage.Width) return theNextPoint;
            theNextPoint.X = Math.Max(0, mUpperLeftCorner.X);
            theNextPoint.Y++;
            if (theNextPoint.Y <= mLowerRightCorner.Y && theNextPoint.Y < theImage.Height) return theNextPoint;
            theNextPoint.X = -1;
            theNextPoint.Y = -1;
            return theNextPoint;
        }
        public override Point GetNextPointOnYAxis(ImageInstance theImage, ref Point theNextPoint)
        {
            theNextPoint.Y++;
            if (theNextPoint.Y <= mLowerRightCorner.Y && theNextPoint.Y < theImage.Height) return theNextPoint;
            theNextPoint.Y = Math.Max(0, mUpperLeftCorner.Y);
            theNextPoint.X++;
            if (theNextPoint.X <= mLowerRightCorner.X && theNextPoint.X < theImage.Width) return theNextPoint;
            theNextPoint.X = -1;
            theNextPoint.Y = -1;
            return theNextPoint;
        }

        public override bool ContainsPoint(ImageInstance theImage, Point thePoint)
        {
            return thePoint.X >= mUpperLeftCorner.X &&
                thePoint.X <= mLowerRightCorner.X &&
                thePoint.Y >= mUpperLeftCorner.Y &&
                thePoint.Y <= mLowerRightCorner.Y;
        }
    }
}

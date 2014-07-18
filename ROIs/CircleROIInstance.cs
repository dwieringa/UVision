// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class CircleROIInstance : NetCams.ROIInstance
    {
        private DataValueInstance centerX;
        private DataValueInstance centerY;
        private DataValueInstance radius;
        private Color color;

        public CircleROIInstance(CircleROIDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            if (theDefinition.CenterX == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to CenterX");
            if (theDefinition.CenterY == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to CenterY");
            if (theDefinition.Radius == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to Radius");
            centerX = theExecution.DataValueRegistry.GetObject(theDefinition.CenterX.Name);
            centerY = theExecution.DataValueRegistry.GetObject(theDefinition.CenterY.Name);
            radius = theExecution.DataValueRegistry.GetObject(theDefinition.Radius.Name);
            color = theDefinition.Color;
        }

        public override void Draw(System.Drawing.Graphics g, PictureBoxScale scale)
        {
            Pen pen = new Pen(color, 1);
            g.DrawEllipse(pen, (mUpperLeftCorner.X * scale.XScale) + scale.XOffset, (mUpperLeftCorner.Y * scale.YScale) + scale.YOffset, 2 * mRadius * scale.XScale, 2 * mRadius * scale.YScale);
            pen.Dispose();
        }

        public override void DoWork()
        {
            mRadius = (int)radius.ValueAsLong();
            mCircleCenter = new Point((int)centerX.ValueAsLong(), (int)centerY.ValueAsLong());
            if (mReferencePoint_X != null)
            {
                mCircleCenter.X += mReferencePoint_X.GetValueAsRoundedInt();
            }
            if (mReferencePoint_Y != null)
            {
                mCircleCenter.Y += mReferencePoint_Y.GetValueAsRoundedInt();
            }

            mUpperLeftCorner = new Point(mCircleCenter.X - mRadius, mCircleCenter.Y - mRadius);
            mLowerRightCorner = new Point(mCircleCenter.X + mRadius, mCircleCenter.Y + mRadius);
        }

        public override bool IsComplete()
        {
            return mUpperLeftCorner != null;// && mLowerRightCorner != null && mCircleCenter != null;
        }

        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public int CenterX
        {
            get { return mCircleCenter.X; }
        }

        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public int CenterY
        {
            get { return mCircleCenter.Y; }
        }

        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public int Radius
        {
            get { return mRadius; }
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

            theFirstPoint.X = mUpperLeftCorner.X; // always starting at the left edge (even if <0) because GetNextPoint() must find left edge of circle even if it doesn't return it
            theFirstPoint.Y = Math.Max(0, mUpperLeftCorner.Y); // don't waste time on rows above the image

            mOppositeEdgeOfThisRow = -1;
            // NOTE: the upper left corner of the bounding square is never within the circle, so find the next pixel on the top row that is within the circle
            return GetNextPointOnXAxis(theImage, ref theFirstPoint);
        }
        public override Point GetFirstPointOnYAxis(ImageInstance theImage, ref Point theFirstPoint)
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

            theFirstPoint.X = Math.Max(0, mUpperLeftCorner.X); // don't waste time on columns left of image
            theFirstPoint.Y = mUpperLeftCorner.Y; // always starting at top edge (even if <0) because GetNextPoint() must find top edge of circle even if it doesn't return it

            mOppositeEdgeOfThisRow = -1;
            // NOTE: the upper left corner of the bounding square is never within the circle, so find the next pixel on the top row that is within the circle
            return GetNextPointOnYAxis(theImage, ref theFirstPoint);
        }

        private Point mUpperLeftCorner; // this is for optimization while iterating over the recantangle (so we don't have to repeated compute these values due to padding)
        private Point mLowerRightCorner; // this is for optimization while iterating over the recantangle (so we don't have to repeated compute these values due to padding)
        private Point mCircleCenter; // this is for optimization while iterating over the recantangle (so we don't have to repeated compute these values due to padding)
        private int mOppositeEdgeOfThisRow;
        private int mRadius;
        /*
        public override Point GetNextPoint(ref Point theNextPoint)
        {
            return GetNextPointOnXAxis(ref theNextPoint);
        }
         */
        public override Point GetNextPointOnXAxis(ImageInstance theImage, ref Point theNextPoint)
        {
            do
            {
                theNextPoint.X++;
                if (mOppositeEdgeOfThisRow > 0)
                {
                    if (theNextPoint.X < 0) theNextPoint.X = 0;
                    if (theNextPoint.X <= mOppositeEdgeOfThisRow && theNextPoint.X < theImage.Width) return theNextPoint;
                    theNextPoint.Y++; // move to next row
                    mOppositeEdgeOfThisRow = -1;
                    theNextPoint.X = mUpperLeftCorner.X;
                    if (theNextPoint.Y > mLowerRightCorner.Y || theNextPoint.Y >= theImage.Height)
                    {
                        theNextPoint.X = -1;
                        theNextPoint.Y = -1;
                        return theNextPoint;
                    }
                }

                // find left edge of circle on this row
                int deltaX;
                int deltaY = Math.Abs(mCircleCenter.Y - theNextPoint.Y);
                int distance;
                do
                {
                    theNextPoint.X++;
                    deltaX = Math.Abs(mCircleCenter.X - theNextPoint.X);
                    distance = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                } while (theNextPoint.X <= mCircleCenter.X && distance > mRadius);
                if (theNextPoint.X > mCircleCenter.X) throw new ArgumentException("Missed finding left edge of circle ROI! Code 89472");
                mOppositeEdgeOfThisRow = mCircleCenter.X + deltaX;
            } while (theNextPoint.X < 0 || theNextPoint.X >= theImage.Width);

            return theNextPoint;
        }
        public override Point GetNextPointOnYAxis(ImageInstance theImage, ref Point theNextPoint)
        {
            do
            {
                theNextPoint.Y++;
                if (mOppositeEdgeOfThisRow > 0)
                {
                    if (theNextPoint.Y < 0) theNextPoint.Y = 0;
                    if (theNextPoint.Y <= mOppositeEdgeOfThisRow && theNextPoint.Y < theImage.Height) return theNextPoint;
                    theNextPoint.X++; // move to next column
                    mOppositeEdgeOfThisRow = -1;
                    theNextPoint.Y = mUpperLeftCorner.Y;
                    if (theNextPoint.X > mLowerRightCorner.X || theNextPoint.Y >= theImage.Height)
                    {
                        theNextPoint.X = -1;
                        theNextPoint.Y = -1;
                        return theNextPoint;
                    }
                }

                // find left edge of circle on this row
                int deltaX = Math.Abs(mCircleCenter.X - theNextPoint.X);
                int deltaY;
                int distance;
                do
                {
                    theNextPoint.Y++;
                    deltaY = Math.Abs(mCircleCenter.Y - theNextPoint.Y);
                    distance = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                } while (theNextPoint.Y <= mCircleCenter.Y && distance > mRadius);
                if (theNextPoint.Y > mCircleCenter.Y)
                {
                    throw new ArgumentException("Missed finding top edge of circle ROI! Code 89473");
                }
                mOppositeEdgeOfThisRow = mCircleCenter.Y + deltaY;
            } while (theNextPoint.Y < 0 || theNextPoint.Y >= theImage.Height);

            return theNextPoint;
            /*
            theNextPoint.Y++;
            if (mOppositeEdgeOfThisRow > 0)
            {
                if (theNextPoint.Y <= mOppositeEdgeOfThisRow) return theNextPoint;
                theNextPoint.X++; // move to next column
                mOppositeEdgeOfThisRow = -1;
                theNextPoint.Y = mUpperLeftCorner.Y;
                if (theNextPoint.X > mLowerRightCorner.X)
                {
                    theNextPoint.X = -1;
                    theNextPoint.Y = -1;
                    return theNextPoint;
                }
            }

            // find left edge of circle on this row
            int deltaX = Math.Abs(mCircleCenter.X - theNextPoint.X);
            int deltaY;
            int distance;
            do
            {
                theNextPoint.Y++;
                deltaY = Math.Abs(mCircleCenter.Y - theNextPoint.Y); 
                distance = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            } while (theNextPoint.Y <= mCircleCenter.Y && distance > mRadius);
            if (theNextPoint.Y > mCircleCenter.Y)
            {
                throw new ArgumentException("Missed finding top edge of circle ROI! Code 89473");
            }
            mOppositeEdgeOfThisRow = mCircleCenter.Y + deltaY;

            return theNextPoint;
             */
        }

        /// <summary>
        /// POSSIBLE OPTIMIZATION: first test if outside bounding square, if no, return false.  If inside, check if inside inner square, if yes, return true, else compute radius
        /// </summary>
        /// <param name="theImage"></param>
        /// <param name="thePoint"></param>
        /// <returns></returns>
        public override bool ContainsPoint(ImageInstance theImage, Point thePoint)
        {
            int deltaX = thePoint.X - mCircleCenter.X;
            int deltaY = thePoint.Y - mCircleCenter.Y;
            bool result = Math.Sqrt(deltaX * deltaX + deltaY * deltaY) <= mRadius;
            if (result)
            {
                return true;
            }
            return false;
        }
    }
}

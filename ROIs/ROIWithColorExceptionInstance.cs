// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    class ROIWithColorExceptionInstance : ROIInstance
    {
        public ROIWithColorExceptionInstance(ROIWithColorExceptionDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            if (theDefinition.MainROI == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to MainROI");
            if (theDefinition.ColorException == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to ColorException");
            mMainROI = theExecution.ROIRegistry.GetObject(theDefinition.MainROI.Name);
            mColorException = theExecution.GetColorMatcher(theDefinition.ColorException.Name);
        }

        private ROIInstance mMainROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIInstance MainROI
        {
            get { return mMainROI; }
        }

        private ColorMatchInstance mColorException;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ColorMatchInstance ColorException
        {
            get { return mColorException; }
        }

        public override System.Drawing.Point GetFirstPointOnXAxis(ImageInstance theImage, ref System.Drawing.Point theFirstPoint)
        {
            mMainROI.GetFirstPointOnXAxis(theImage, ref theFirstPoint);
            while (theFirstPoint.X != -1 && theFirstPoint.Y != -1 && mColorException.Matches(theImage.GetColor(theFirstPoint.X, theFirstPoint.Y)))
            {
                mMainROI.GetNextPointOnXAxis(theImage, ref theFirstPoint);
            }
            return theFirstPoint;
        }

        public override System.Drawing.Point GetFirstPointOnYAxis(ImageInstance theImage, ref System.Drawing.Point theFirstPoint)
        {
            mMainROI.GetFirstPointOnYAxis(theImage, ref theFirstPoint);
            while (theFirstPoint.X != -1 && theFirstPoint.Y != -1 && mColorException.Matches(theImage.GetColor(theFirstPoint.X, theFirstPoint.Y)))
            {
                mMainROI.GetNextPointOnYAxis(theImage, ref theFirstPoint);
            }
            return theFirstPoint;
        }

        public override System.Drawing.Point GetNextPointOnXAxis(ImageInstance theImage, ref System.Drawing.Point theNextPoint)
        {

            do
            {
                mMainROI.GetNextPointOnXAxis(theImage, ref theNextPoint);
            } while (theNextPoint.X != -1 && theNextPoint.Y != -1 && mColorException.Matches(theImage.GetColor(theNextPoint.X, theNextPoint.Y)));
            return theNextPoint;
        }

        public override System.Drawing.Point GetNextPointOnYAxis(ImageInstance theImage, ref System.Drawing.Point theNextPoint)
        {
            do
            {
                mMainROI.GetNextPointOnYAxis(theImage, ref theNextPoint);
            } while (theNextPoint.X != -1 && theNextPoint.Y != -1 && mColorException.Matches(theImage.GetColor(theNextPoint.X, theNextPoint.Y)));
            return theNextPoint;
        }

        public override bool ContainsPoint(ImageInstance theImage, System.Drawing.Point thePoint)
        {
            return mMainROI.ContainsPoint(theImage, thePoint) && !mColorException.Matches(theImage.GetColor(thePoint.X, thePoint.Y));
        }

        public override void Draw(System.Drawing.Graphics g, PictureBoxScale scale)
        {
            mMainROI.Draw(g, scale);
        }

        public override void DoWork()
        {
        }

        public override bool IsComplete()
        {
            // our DoWork() is empty, so we just rely on the completeness of the underlying objects that we use
            return mMainROI.IsComplete() && mColorException.IsComplete();
        }
    }
}

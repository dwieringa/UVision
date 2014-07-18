// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class ROIWithHoleInstance : ROIInstance
    {
        public ROIWithHoleInstance(ROIWithHoleDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            if (theDefinition.MainROI == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to MainROI");
            if (theDefinition.HoleROI == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to HoleROI");
            mMainROI = theExecution.ROIRegistry.GetObject(theDefinition.MainROI.Name);
            mHoleROI = theExecution.ROIRegistry.GetObject(theDefinition.HoleROI.Name);
        }

        private ROIInstance mMainROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIInstance MainROI
        {
            get { return mMainROI; }
        }

        private ROIInstance mHoleROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIInstance HoleROI
        {
            get { return mHoleROI; }
        }

        public override System.Drawing.Point GetFirstPointOnXAxis(ImageInstance theImage, ref System.Drawing.Point theFirstPoint)
        {
            mMainROI.GetFirstPointOnXAxis(theImage, ref theFirstPoint);
            while (theFirstPoint.X != -1 && theFirstPoint.Y != -1 && mHoleROI.ContainsPoint(theImage, theFirstPoint))
            {
                mMainROI.GetNextPointOnXAxis(theImage, ref theFirstPoint);
            }
            return theFirstPoint;
        }

        public override System.Drawing.Point GetFirstPointOnYAxis(ImageInstance theImage, ref System.Drawing.Point theFirstPoint)
        {
            mMainROI.GetFirstPointOnYAxis(theImage, ref theFirstPoint);
            while (theFirstPoint.X != -1 && theFirstPoint.Y != -1 && mHoleROI.ContainsPoint(theImage, theFirstPoint))
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
            } while (theNextPoint.X != -1 && theNextPoint.Y != -1 && mHoleROI.ContainsPoint(theImage, theNextPoint));
            return theNextPoint;
        }

        public override System.Drawing.Point GetNextPointOnYAxis(ImageInstance theImage, ref System.Drawing.Point theNextPoint)
        {
            do
            {
                mMainROI.GetNextPointOnYAxis(theImage, ref theNextPoint);
            } while (theNextPoint.X != -1 && theNextPoint.Y != -1 && mHoleROI.ContainsPoint(theImage, theNextPoint));
            return theNextPoint;
        }

        public override bool ContainsPoint(ImageInstance theImage, System.Drawing.Point thePoint)
        {
            return mMainROI.ContainsPoint(theImage, thePoint) && !mHoleROI.ContainsPoint(theImage, thePoint);
        }

        public override void Draw(System.Drawing.Graphics g, PictureBoxScale scale)
        {
            mMainROI.Draw(g, scale);
            mHoleROI.Draw(g, scale);
        }

        public override void DoWork()
        {
        }

        public override bool IsComplete()
        {
            // our DoWork() is empty, so we just rely on the completeness of the underlying ROIs that we use
            return mMainROI.IsComplete() && mHoleROI.IsComplete();
        }
    }
}

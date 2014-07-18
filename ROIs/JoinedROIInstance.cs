// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    /// <summary>
    /// Join to ROIs to create a single one.  It is important that if the component ROIs overlap that we only return each
    /// pixel only once (ie for "scoring" type analysises we don't want to score a group of pixels twice because they
    /// happen to be in an overlap region.
    /// 
    /// Approach:  iterate through the first ROI, then iterate through the 2nd ROI but ignore any points that are contained
    /// by the first ROI.
    /// 
    /// POSSIBLE IMPROVEMENT: determine first if the ROIs intersect.  If not, just iterate through both blindly.
    /// POSSIBLE IMPROVEMENT: first determine size/complexity of each ROI.  Iterate through the biggest one first? Iterate through one with simplest "Contains()" method first?
    /// </summary>
    public class JoinedROIInstance : ROIInstance
    {
        public JoinedROIInstance(JoinedROIDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            if (theDefinition.FirstROI == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to FirstROI");
            if (theDefinition.SecondROI == null) throw new ArgumentException("ROI '" + theDefinition.Name + "' doesn't have a value assigned to SecondROI");
            mFirstROI = theExecution.ROIRegistry.GetObject(theDefinition.FirstROI.Name);
            mSecondROI = theExecution.ROIRegistry.GetObject(theDefinition.SecondROI.Name);
        }

        private ROIInstance mFirstROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIInstance FirstROI
        {
            get { return mFirstROI; }
        }

        private ROIInstance mSecondROI;
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public ROIInstance SecondROI
        {
            get { return mSecondROI; }
        }

        private bool mCompletedFirstROI = false;
        public override System.Drawing.Point GetFirstPointOnXAxis(ImageInstance theImage, ref System.Drawing.Point theFirstPoint)
        {
            mCompletedFirstROI = false;
            mFirstROI.GetFirstPointOnXAxis(theImage, ref theFirstPoint);
            return theFirstPoint;
        }

        public override System.Drawing.Point GetFirstPointOnYAxis(ImageInstance theImage, ref System.Drawing.Point theFirstPoint)
        {
            mCompletedFirstROI = false;
            mFirstROI.GetFirstPointOnYAxis(theImage, ref theFirstPoint);
            return theFirstPoint;
        }

        public override System.Drawing.Point GetNextPointOnXAxis(ImageInstance theImage, ref System.Drawing.Point theNextPoint)
        {
            if (!mCompletedFirstROI)
            {
                // first iterate through the FirstROI blindly
                mFirstROI.GetNextPointOnXAxis(theImage, ref theNextPoint);
                if (theNextPoint.X != -1 && theNextPoint.Y != -1) return theNextPoint;

                // once we hit the end of FirstROI, check if the first point in SecondROI is outside of FirstROI, if so, use it
                mCompletedFirstROI = true;
                theNextPoint = mSecondROI.GetFirstPointOnXAxis(theImage, ref theNextPoint);
                if (!mFirstROI.ContainsPoint(theImage, theNextPoint)) return theNextPoint;
            }
            // after using up FirstROI and the first point of SecondROI, then iterate through SecondROI and weed out any duplicate point
            do
            {
                mSecondROI.GetNextPointOnXAxis(theImage, ref theNextPoint);
            } while (theNextPoint.X != -1 && theNextPoint.Y != -1 && mFirstROI.ContainsPoint(theImage, theNextPoint));
            return theNextPoint;
        }

        public override System.Drawing.Point GetNextPointOnYAxis(ImageInstance theImage, ref System.Drawing.Point theNextPoint)
        {
            if (!mCompletedFirstROI)
            {
                // first iterate through the FirstROI blindly
                mFirstROI.GetNextPointOnYAxis(theImage, ref theNextPoint);
                if (theNextPoint.X != -1 && theNextPoint.Y != -1) return theNextPoint;

                // once we hit the end of FirstROI, check if the first point in SecondROI is outside of FirstROI, if so, use it
                mCompletedFirstROI = true;
                theNextPoint = mSecondROI.GetFirstPointOnXAxis(theImage, ref theNextPoint);
                if (!mFirstROI.ContainsPoint(theImage, theNextPoint)) return theNextPoint;
            }
            // after using up FirstROI and the first point of SecondROI, then iterate through SecondROI and weed out any duplicate point
            do
            {
                mSecondROI.GetNextPointOnYAxis(theImage, ref theNextPoint);
            } while (theNextPoint.X != -1 && theNextPoint.Y != -1 && mFirstROI.ContainsPoint(theImage, theNextPoint));
            return theNextPoint;
        }

        public override bool ContainsPoint(ImageInstance theImage, System.Drawing.Point thePoint)
        {
            return mFirstROI.ContainsPoint(theImage, thePoint) || mSecondROI.ContainsPoint(theImage, thePoint);
        }

        public override void Draw(System.Drawing.Graphics g, PictureBoxScale scale)
        {
            mFirstROI.Draw(g, scale);
            mSecondROI.Draw(g, scale);
        }

        public override void DoWork()
        {
        }

        public override bool IsComplete()
        {
            // our DoWork() is empty, so we just rely on the completeness of the underlying ROIs that we use
            return mFirstROI.IsComplete() && mSecondROI.IsComplete();
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace NetCams
{
    [TypeConverter(typeof(ROIInstanceConverter))]
    public abstract class ROIInstance : NetCams.ToolInstance, IROIInstance
    {
        public ROIInstance(ROIDefinition theDefinition, TestExecution theExecution)
            : base(theDefinition, theExecution)
        {
            if (theDefinition.ReferencePoint_X != null)
            {
                mReferencePoint_X = theExecution.ReferencePointRegistry.GetObject(theDefinition.ReferencePoint_X.Name);
            }
            if (theDefinition.ReferencePoint_Y != null)
            {
                mReferencePoint_Y = theExecution.ReferencePointRegistry.GetObject(theDefinition.ReferencePoint_Y.Name);
            }

            theExecution.ROIRegistry.RegisterObject(this);
        }

        protected IReferencePointInstance mReferencePoint_X = null;
        public IReferencePointInstance ReferencePoint_X
        {
            get { return mReferencePoint_X; }
        }

        protected IReferencePointInstance mReferencePoint_Y = null;
        public IReferencePointInstance ReferencePoint_Y
        {
            get { return mReferencePoint_Y; }
        }

        // NOTE: we're passing in the point as reference for speed (avoid creating new Point objects) and for thread safety
        // FURTHER NOTE:  Do we need to be thread safe??? There is a separate instance of this class for each test.  Why would one test have multiple threads???  ...besides to retrieve multiple images from multiple cameras.  processing of images should be in one thread
//        public abstract Point GetFirstPoint(ref Point theFirstPoint);
        public abstract Point GetFirstPointOnXAxis(ImageInstance theImage, ref Point theFirstPoint);
        public abstract Point GetFirstPointOnYAxis(ImageInstance theImage, ref Point theFirstPoint);
//        public abstract Point GetNextPoint(ref Point theNextPoint);
        /// <summary>
        /// Note: some users of this assume that we move along the x-axis in positive direction 1 pixel at a time.
        /// </summary>
        /// <param name="theImage"></param>
        /// <param name="theNextPoint"></param>
        /// <returns></returns>
        public abstract Point GetNextPointOnXAxis(ImageInstance theImage, ref Point theNextPoint);

        /// <summary>
        /// Note: some users of this assume that we move along the y-axis in positive direction 1 pixel at a time.
        /// </summary>
        /// <param name="theImage"></param>
        /// <param name="theNextPoint"></param>
        /// <returns></returns>
        public abstract Point GetNextPointOnYAxis(ImageInstance theImage, ref Point theNextPoint);
        public abstract bool ContainsPoint(ImageInstance theImage, Point thePoint);

        public abstract void Draw(Graphics g, PictureBoxScale scale);
    }

    public class ROIInstanceConverter : ObjectInstanceConverter
    {
        protected override string[] Options(TestExecution theTestExecution)
        {
            return theTestExecution.ROIRegistry.Options();
        }

        protected override IObjectInstance LookupObject(TestExecution theTestExecution, string theObjectName)
        {
            return theTestExecution.ROIRegistry.GetObject(theObjectName);
        }
    }


}

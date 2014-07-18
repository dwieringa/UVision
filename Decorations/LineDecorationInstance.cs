// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public abstract class LineDecorationInstance : DecorationInstance, IReferencePointInstance
    {
        public LineDecorationInstance(LineDecorationDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            mColor = theDefinition.Color;

            testExecution.LineDecorationRegistry.RegisterObject(this);
            testExecution.ReferencePointRegistry.RegisterObject(this);
        }

    /* COMMENTED OUT "TEMPORARILLY" 4/16/09 SO I CAN COMPILE A NEW TOOL.  I QUIT WORKING ON THIS FINDEDGES STUFF A FEW WEEKS AGO WHEN I GOT STUCK AND RAN OUT OF TIME
        public LineDecorationInstance(LineGroupDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            mColor = theDefinition.Color;

            testExecution.LineDecorationRegistry.RegisterObject(this);
            testExecution.ReferencePointRegistry.RegisterObject(this);
        }
     */

        [CategoryAttribute("Definition")]
        public abstract int StartX_int { get; }

        [CategoryAttribute("Definition")]
        public abstract int StartY_int { get; }

        [CategoryAttribute("Definition")]
        public abstract int EndX_int { get; }

        [CategoryAttribute("Definition")]
        public abstract int EndY_int { get; }

        protected Color mColor;
        [CategoryAttribute("Definition")]
        public Color Color
        {
            get { return mColor; }
        }


        #region IReferencePointInstance Members

        /// <summary>
        /// TODO: create an ILineReferencePointDef/Instance.  Add properties which allows the user to choose which axis to use as the reference point and provide a value on the other axis to determine the ref point.
        /// Then fix the property editor to support nested properties...hopefully if the user chooses a LineReferencePointDef for a ReferencePointDef the property nesting will automatically show all child-properties by default
        /// </summary>
        /// <returns></returns>
        public int GetValueAsRoundedInt()
        {
            if (StartX_int == EndX_int && StartY_int != EndY_int) return StartX_int;
            if (StartY_int == EndY_int && StartX_int != EndX_int) return StartY_int;
            TestExecution().LogErrorWithTimeFromTrigger("Using line " + Name + " as a reference point and the line doesn't fall on an x- or y-axis. Currently this isn't supported.");
            return -1;
        }

        public int GetValueAsTruncatedInt()
        {
            return GetValueAsRoundedInt();
        }

        public double GetValueAsDouble()
        {
            return GetValueAsRoundedInt();
        }

        #endregion
    }
}

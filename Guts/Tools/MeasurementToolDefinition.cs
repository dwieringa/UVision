// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class MeasurementToolDefinition : NetCams.ToolDefinition
    {
        public MeasurementToolDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mDistance = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "Distance"));
            mDistance.Type = DataType.DecimalNumber;
            mDistance.AddDependency(this);
            mDistance.Name = "Distance";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new MeasurementToolInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mReferencePoint1 != null && mReferencePoint1.IsDependentOn(theOtherObject)) return true;
            if (mReferencePoint2 != null && mReferencePoint2.IsDependentOn(theOtherObject)) return true;
            if (mPixelsPerUnit != null && mPixelsPerUnit.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mReferencePoint1 != null) result = Math.Max(result, mReferencePoint1.ToolMapRow);
                if (mReferencePoint2 != null) result = Math.Max(result, mReferencePoint2.ToolMapRow);
                if (mPixelsPerUnit != null) result = Math.Max(result, mPixelsPerUnit.ToolMapRow);
                return result + 1;
			}
		}

        private DataValueDefinition mPixelsPerUnit;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition PixelsPerUnit
        {
            get { return mPixelsPerUnit; }
            set
            {
                HandlePropertyChange(this, "PixelsPerUnit", mPixelsPerUnit, value);
                mPixelsPerUnit = value;
            }
        }

        private DataValueDefinition mEnsure1Before2;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition Ensure1Before2
        {
            get { return mEnsure1Before2; }
            set
            {
                HandlePropertyChange(this, "Ensure1Before2", mEnsure1Before2, value);
                mEnsure1Before2 = value;
            }
        }

        private IReferencePointDefinition mReferencePoint1;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public IReferencePointDefinition ReferencePoint1
        {
            get { return mReferencePoint1; }
            set
            {
                HandlePropertyChange(this, "ReferencePoint1", mReferencePoint1, value);
                mReferencePoint1 = value;
            }
        }

        private IReferencePointDefinition mReferencePoint2;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public IReferencePointDefinition ReferencePoint2
        {
            get { return mReferencePoint2; }
            set
            {
                HandlePropertyChange(this, "ReferencePoint2", mReferencePoint2, value);
                mReferencePoint2 = value;
            }
        }

        private GeneratedValueDefinition mDistance = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition Distance
        {
            get { return mDistance; }
        }

        private GeneratedValueDefinition mDistance_pixels = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition Distance_pixels
        {
            get { return mDistance_pixels; }
        }
        private bool mDistance_pixels_Enabled = false;
        [CategoryAttribute("Output"),
        DescriptionAttribute("")]
        public bool Distance_pixels_Enabled
        {
            get { return mDistance_pixels_Enabled; }
            set
            {
                HandlePropertyChange(this, "Distance_pixels_Enabled", mDistance_pixels_Enabled, value);
                mDistance_pixels_Enabled = value;
                if (!value && mDistance_pixels != null)
                {
                    mDistance_pixels.Dispose_UVision();
                    mDistance_pixels = null;
                }
                if (value && mDistance_pixels == null)
                {
                    mDistance_pixels = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "Distance_pixels"));
                    mDistance_pixels.Type = DataType.IntegerNumber;
                    mDistance_pixels.AddDependency(this);
                    mDistance_pixels.Name = "Distance_pixels";
                }
            }
        }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public class CalibrationToolDefinition : NetCams.ToolDefinition
    {
        public CalibrationToolDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mConversionFactor = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "ConversionFactor"));
            mConversionFactor.Type = DataType.DecimalNumber;
            mConversionFactor.AddDependency(this);
            mConversionFactor.Name = "ConversionFactor";
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new CalibrationToolInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mReferencePoint1 != null && mReferencePoint1.IsDependentOn(theOtherObject)) return true;
            if (mReferencePoint2 != null && mReferencePoint2.IsDependentOn(theOtherObject)) return true;
            if (mKnownDistance != null && mKnownDistance.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mReferencePoint1 != null) result = Math.Max(result, mReferencePoint1.ToolMapRow);
                if (mReferencePoint2 != null) result = Math.Max(result, mReferencePoint2.ToolMapRow);
                if (mKnownDistance != null) result = Math.Max(result, mKnownDistance.ToolMapRow);
                return result + 1;
			}
		}

        private DataValueDefinition mKnownDistance;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("")]
        public DataValueDefinition KnownDistance
        {
            get { return mKnownDistance; }
            set
            {
                HandlePropertyChange(this, "KnownDistance", mKnownDistance, value);
                mKnownDistance = value;
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

        private GeneratedValueDefinition mConversionFactor = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition ConversionFactor
        {
            get { return mConversionFactor; }
        }

    }
}

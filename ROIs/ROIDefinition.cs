// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace NetCams
{
    [TypeConverter(typeof(ROIDefinitionConverter))]
    public abstract class ROIDefinition : ToolDefinition, IROIDefinition
    {
        public ROIDefinition(TestSequence testSequence)
            : base(testSequence)
        {
			testSequence.ROIRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().ROIRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        protected IReferencePointDefinition mReferencePoint_X = null;
        [CategoryAttribute("Location, X-axis"),
        DescriptionAttribute("")]
        public IReferencePointDefinition ReferencePoint_X
        {
            get { return mReferencePoint_X; }
            set
            {
                HandlePropertyChange(this, "ReferencePoint_X", mReferencePoint_X, value);
                mReferencePoint_X = value;
            }
        }

        protected IReferencePointDefinition mReferencePoint_Y = null;
        [CategoryAttribute("Location, Y-axis"),
        DescriptionAttribute("")]
        public IReferencePointDefinition ReferencePoint_Y
        {
            get { return mReferencePoint_Y; }
//            set { HandlePropertyChange(this, this.GetType().GetProperty("ReferencePoint_Y"), value); }
            set
            {
                HandlePropertyChange(this, "ReferencePoint_X", mReferencePoint_X, value);
                mReferencePoint_Y = value;
            }
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            return (theOtherObject == this)
                || (mReferencePoint_X != null && mReferencePoint_X.IsDependentOn(theOtherObject))
                || (mReferencePoint_Y != null && mReferencePoint_Y.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mReferencePoint_X != null) result = Math.Max(result, mReferencePoint_X.ToolMapRow);
                if (mReferencePoint_Y != null) result = Math.Max(result, mReferencePoint_Y.ToolMapRow);
                return result + 1;
            }
        }
    }

    public class ROIDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.ROIRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.ROIRegistry.GetObject(theObjectName);
        }
    }

}

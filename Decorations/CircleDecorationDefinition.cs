// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;


namespace NetCams
{
    [TypeConverter(typeof(CircleDecorationDefinitionConverter))]
    public abstract class CircleDecorationDefinition : DecorationDefinition
    {
        public CircleDecorationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            testSequence.CircleDecorationRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().CircleDecorationRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new CircleDecorationInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mCenterX.IsDependentOn(theOtherObject)) return true;
            if (mCenterY.IsDependentOn(theOtherObject)) return true;
            if (mRadius.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mCenterX != null) result = Math.Max(result, mCenterX.ToolMapRow);
                if (mCenterY != null) result = Math.Max(result, mCenterY.ToolMapRow);
                if (mRadius != null) result = Math.Max(result, mRadius.ToolMapRow);
                return result + 1;
            }
        }

        public DataValueDefinition CenterX() { return mCenterX; }
        public DataValueDefinition CenterY() { return mCenterY; }
        public DataValueDefinition Radius() { return mRadius; }

        public Color Color
        {
            get { return mColor; }
            set 
            {
                HandlePropertyChange(this, "Color", mColor, value);
                mColor = value;
            }
        }

        protected DataValueDefinition mCenterX;
        protected DataValueDefinition mCenterY;
        protected DataValueDefinition mRadius;
        protected Color mColor = System.Drawing.Color.Magenta;

    }

    public class CircleDecorationDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.CircleDecorationRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.CircleDecorationRegistry.GetObject(theObjectName);
        }
    }
}

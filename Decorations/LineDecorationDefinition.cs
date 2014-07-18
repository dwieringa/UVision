// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;


namespace NetCams
{
    /// <summary>
    /// This is for lines that fit the 1-to-1 mapping of instance-to-definition object.  When we have this 1:1 relationship
    /// we register the line's defintion object so it is available in property dropdowns for tools that work with a specific
    /// line (since we know it's name).
    /// </summary>
    [TypeConverter(typeof(LineDecorationDefinitionConverter))]
    public abstract class LineDecorationDefinition : DecorationDefinition, IReferencePointDefinition
    {
        public LineDecorationDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            testSequence.LineDecorationRegistry.RegisterObject(this);
            testSequence.ReferencePointRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().LineDecorationRegistry.UnRegisterObject(this);
            TestSequence().ReferencePointRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        public Color Color
        {
            get { return mColor; }
            set 
            {
                HandlePropertyChange(this, "Color", mColor, value);
                mColor = value;
            }
        }

        protected Color mColor = System.Drawing.Color.Magenta;

    }

    public class LineDecorationDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.LineDecorationRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.LineDecorationRegistry.GetObject(theObjectName);
        }
    }
}

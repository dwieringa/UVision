// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public abstract class RectangleROIBaseDefinition : ROIDefinition, IRectangleROIDefinition
    {
        protected Color color = Color.Yellow;

		public RectangleROIBaseDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            Name = "Untitled Rectangle ROI";
            testSequence.RectangleROIRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().RectangleROIRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        [CategoryAttribute("Appearance"),
        DescriptionAttribute("")]
        public Color Color
        {
            get { return color; }
            set 
            {
                HandlePropertyChange(this, "Color", color, value);
                color = value;
            }
        }
    }
}

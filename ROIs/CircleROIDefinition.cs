// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace NetCams
{
    public class CircleROIDefinition : ROIDefinition
    {
        private DataValueDefinition centerX;
        private DataValueDefinition centerY;
        private DataValueDefinition radius;
        private Color color = Color.Yellow;

        public CircleROIDefinition(TestSequence testSequence)
            : base(testSequence)
        {
            Name = "Untitled Cicle ROI";
        }

        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public DataValueDefinition CenterX
        {
            get { return centerX; }
            set
            {
                HandlePropertyChange(this, "CenterX", centerX, value);
                centerX = value;
            }
        }
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public DataValueDefinition CenterY
        {
            get { return centerY; }
            set
            {
                HandlePropertyChange(this, "CenterY", centerY, value);
                centerY = value;
            }
        }
        [CategoryAttribute("Location"),
        DescriptionAttribute("")]
        public DataValueDefinition Radius
        {
            get { return radius; }
            set
            {
                HandlePropertyChange(this, "Radius", radius, value);
                radius = value;
            }
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

        public override void CreateInstance(TestExecution theExecution)
        {
            new CircleROIInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {// TODO: these will never be null since we set them in the constructor and never change them (only their contents)
            return (theOtherObject == this)
                || (centerX != null && centerX.IsDependentOn(theOtherObject))
                || (centerY != null && centerY.IsDependentOn(theOtherObject))
                || (radius != null && radius.IsDependentOn(theOtherObject))
                || base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {// TODO: top,bottom, etc will never be null since we set them in the constructor and never change them
                int result = base.ToolMapRow - 1;
                if (centerX != null) result = Math.Max(result, centerX.ToolMapRow);
                if (centerY != null) result = Math.Max(result, centerY.ToolMapRow);
                if (radius != null) result = Math.Max(result, radius.ToolMapRow);
                return result + 1;
            }
        }
    }
}

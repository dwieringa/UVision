// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
namespace NetCams
{
    public interface IROIInstance : IToolInstance
    {
        bool ContainsPoint(ImageInstance theImage, System.Drawing.Point thePoint);
        void Draw(System.Drawing.Graphics g, PictureBoxScale scale);
        System.Drawing.Point GetFirstPointOnXAxis(ImageInstance theImage, ref System.Drawing.Point theFirstPoint);
        System.Drawing.Point GetFirstPointOnYAxis(ImageInstance theImage, ref System.Drawing.Point theFirstPoint);
        System.Drawing.Point GetNextPointOnXAxis(ImageInstance theImage, ref System.Drawing.Point theNextPoint);
        System.Drawing.Point GetNextPointOnYAxis(ImageInstance theImage, ref System.Drawing.Point theNextPoint);
        IReferencePointInstance ReferencePoint_X { get; }
        IReferencePointInstance ReferencePoint_Y { get; }
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;

namespace NetCams
{
    [TypeConverter(typeof(ROIDefinitionConverter))]
    public interface IROIDefinition : IToolDefinition
    {
        bool IsDependentOn(IObjectDefinition theOtherObject);
        IReferencePointDefinition ReferencePoint_X { get; set; }
        IReferencePointDefinition ReferencePoint_Y { get; set; }
        int ToolMapRow { get; }
    }
}

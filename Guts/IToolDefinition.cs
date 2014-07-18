// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
namespace NetCams
{
    public interface IToolDefinition : IObjectDefinition
    {
        void Dispose_UVision();
        bool IsDependentOn(IObjectDefinition theOtherObject);
        DataValueDefinition Prerequisite { get; set; }
        int ToolMapRow { get; }
    }
}

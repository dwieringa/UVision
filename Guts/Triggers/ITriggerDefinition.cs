// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
namespace NetCams
{
    public interface ITriggerDefinition : IObjectDefinition
    {
        bool TriggerEnabled { get; set; }
        //bool IsDependentOn(IObjectDefinition theOtherObject);
        //int ToolMapRow { get; }
    }
}

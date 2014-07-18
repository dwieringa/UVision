// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
namespace NetCams
{
    public interface ITriggerInstance : IObjectInstance
    {
        bool CheckTrigger();
        ITriggerDefinition Definition_Trigger();
    }
}

// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
namespace NetCams
{
    public interface IToolInstance : IObjectInstance
    {
        void DoWork();
        DataValueInstance Prerequisite { get; }
    }
}

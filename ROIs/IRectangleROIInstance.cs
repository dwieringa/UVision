// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
namespace NetCams
{
    public interface IRectangleROIInstance : IROIInstance
    {
        int Top { get; }
        int Bottom { get; }
        int Left { get; }
        int Right { get; }
    }
}

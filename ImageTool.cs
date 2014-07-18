// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public interface ImageTool
    {
        Color GetPixelColor(int x, int y);
    }
}

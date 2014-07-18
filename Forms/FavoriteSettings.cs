// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class FavoriteSettings
    {
        public FavoriteSettings(TestSequence seq)
        {
            mTestSequence = seq;
        }
        public TestSequence mTestSequence;
    }
}

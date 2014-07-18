// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public abstract class FavoriteValues
    {
        public FavoriteValues()
        {
        }
        protected TestExecution mTestExecution;

    	public void UpdateTestExecution(TestExecution selectedTest)
	    {
            mTestExecution = selectedTest;
            UpdateValues();
	    }

        protected abstract void UpdateValues();
    }
}

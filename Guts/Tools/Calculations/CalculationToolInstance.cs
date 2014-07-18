// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections;
using System.Text;


namespace NetCams
{
    class CalculationToolInstance : ToolInstance
    {
        public CalculationToolInstance(CalculationToolDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            if (theDefinition.RootOperation == null) throw new ArgumentException(theDefinition.Name + " does not seem to have a calculation defined.");
            mRootOperation = testExecution.GetMathOperation(theDefinition.RootOperation.Name);
            mCalculationDef = theDefinition.Calculation;
        }

        public override bool IsComplete()
        {
            return mRootOperation.Result.IsComplete();
        }

        /*
        [CategoryAttribute("Output")]
        private CalculatedValueInstance mResult;
        public CalculatedValueInstance Result
        {
            get { return mResult; }
        }
         */

        private string mCalculationDef;
        public string Calculation
        {
            get { return mCalculationDef; }
        }

        private MathOperationInstance mRootOperation = null;
        public MathOperationInstance RootOperation
        {
            get { return mRootOperation; }
        }
        
        public override void DoWork() {}
    }
}

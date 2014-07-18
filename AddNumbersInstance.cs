// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Text;


namespace NetCams
{
    class AddNumbersInstance : ToolInstance
    {
        public AddNumbersInstance(AddNumbersDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
			// 
			// TODO: Add constructor logic here
			//

            mValue1 = testExecution.DataValueRegistry.GetObject(theDefinition.Value1.Name);
            mValue2 = testExecution.DataValueRegistry.GetObject(theDefinition.Value2.Name);
            mResult = new GeneratedValueInstance(theDefinition.Result, testExecution);
        }

        private DataValueInstance mValue1 = null;
        [CategoryAttribute("Input")]
        public DataValueInstance Value1
        {
            get { return mValue1; }
        }

        private DataValueInstance mValue2 = null;
        [CategoryAttribute("Input")]
        public DataValueInstance Value2
        {
            get { return mValue2; }
        }

        private GeneratedValueInstance mResult = null;
        [CategoryAttribute("Output")]
        public GeneratedValueInstance Result
        {
            get { return mResult; }
        }


		public override bool IsComplete() { return mResult.IsComplete(); }

//		public const string AnalysisType = "Color Present Fails";
//		public override string Type() { return AnalysisType; }

		public override void DoWork() 
		{
            mResult.SetValue( mValue1.ValueAsLong() + mValue2.ValueAsLong() );

            mResult.SetIsComplete();
		}
    }
}

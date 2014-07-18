// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class MathOpResultDefinition : GeneratedValueDefinition
    {
        public MathOpResultDefinition(TestSequence testSequence, MathOperationDefinition mathOp, string calcDef)
            : base(testSequence, OwnerLink.newLink(mathOp,"Result"))
		{
            testSequence.MathOperationRegistry.RegisterObject(this);
            mMathOp = mathOp;
            base.Name = calcDef;
            SetDataCategory(DataCategory.UnnamedCalculatedValue);
		}

        public override void Dispose_UVision()
        {
            TestSequence().MathOperationRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        public override bool IncludeObjectInConfigFile() { return false; }
        public override bool IncludeObjectInProgrammingTable() { return false; }
        
        public override void CreateInstance(TestExecution theExecution)
		{
			new MathOpResultInstance(this, theExecution);
		}

        //public const string COMPUTED_VALUES_NAME_PREFIX = "Result of ";

        private MathOperationDefinition mMathOp = null;
        public MathOperationDefinition MathOperation() { return mMathOp; }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                throw new ArgumentException("The name of a computed value can not be changed.");
                //20080506
                //if( Name.StartsWith( COMPUTED_VALUES_NAME_PREFIX ) )
                //{
                //    throw new ArgumentException("The name of a computed value can not be changed.");
                //}
                //base.Name = value;
            }
        }

        public override GlobalValue GlobalValueToUpdate
        {
            get { return mGlobalValueToUpdate; }
            set { throw new ArgumentException("Can't change GlobalValueToUpdate for a MathOpResultDefinition. This is controlled by CalculatedValueDefinition."); }
        }
    }
}

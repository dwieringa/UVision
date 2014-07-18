// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Collections;
using System.Text;


namespace NetCams
{
    public abstract class MathOperationDefinition : NetCams.ToolDefinition
    {
        // NOTE: MathOperations aren't to be created or edited by the user.  User creates CalculationTools which in turn create/manage MathOperations are needed

        public MathOperationDefinition(TestSequence testSequence, string calcDef)
            : base(testSequence)
        {
            mResult = new MathOpResultDefinition(testSequence, this, calcDef);
            mResult.Type = DataType.NotDefined; // let this be decided by the operation/values. currently doing this when the instance is created in case the operator changes the type of a value after it is added to an operation...currently I don't have a way to seeing when changes are made to values used by the mathOps
            mResult.AddDependency(this);
            //20080506 mResult.Name = MathOpResultDefinition.COMPUTED_VALUES_NAME_PREFIX + calcDef;

            base.Name = MATH_OP_NAME_PREFIX + calcDef;
        }

        public override void Dispose_UVision()
        {
            mResult.Dispose_UVision();
            base.Dispose_UVision();
        }

        public override bool IncludeObjectInConfigFile() { return false; }
        public override bool IncludeObjectInProgrammingTable() { return false; }

        public const string MATH_OP_NAME_PREFIX = "Calculation of ";

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                throw new ArgumentException("Names of math operations can not be changed.");
            }
        }

        private MathOpResultDefinition mResult = null;
        [CategoryAttribute("Output")]
        public MathOpResultDefinition Result
        {
            get { return mResult; }
        }
    }
}

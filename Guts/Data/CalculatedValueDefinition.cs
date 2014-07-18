// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public class CalculatedValueDefinition : DataValueDefinition
    {
        public CalculatedValueDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mCalcTool = new CalculationToolDefinition(testSequence, this);
            SetDataCategory(DataCategory.NamedValue);
		}

        public override void Dispose_UVision()
        {
            mCalcTool.Dispose_UVision();
            base.Dispose_UVision();
        }

        private bool mIncludeObjectInConfigFile = true;
        public void SetIncludeObjectInConfigFile(bool value) { mIncludeObjectInConfigFile = value; }
        public override bool IncludeObjectInConfigFile() { return mIncludeObjectInConfigFile; }

        private bool mIncludeObjectInProgrammingTable = true;
        public void SetIncludeObjectInProgrammingTable(bool value) { mIncludeObjectInProgrammingTable = value; }
        public override bool IncludeObjectInProgrammingTable() { return mIncludeObjectInProgrammingTable; }

        public override void CreateInstance(TestExecution theExecution)
		{
			new CalculatedValueInstance(this, theExecution);
		}

        /* TO DO: need to rethink this...CalculationTool doesn't use DoWork..but rather MathOp
        public DataValueDefinition Prerequisite
        {
            get { return mCalcTool.Prerequisite; }
            set
            {
                HandlePropertyChange(this, "Prerequisite", mCalcTool.Prerequisite, value);
                mCalcTool.Prerequisite = value;
            }
        }
        */

        public string Calculation
        {
            get { return mCalcTool.Calculation; }
            set
            {
                HandlePropertyChange(this, "Calculation", mCalcTool.Calculation, value);
                if (mGlobalValueToUpdate != null && mCalcTool.RootOperation.Result != null)
                {
                    mGlobalValueToUpdate.UnregisterUpdater(mCalcTool.RootOperation.Result);
                }
                mCalcTool.Calculation = value;
                if (mGlobalValueToUpdate != null && mCalcTool.RootOperation.Result != null)
                {
                    mGlobalValueToUpdate.RegisterUpdater(mCalcTool.RootOperation.Result);
                }
            }
        }

        public override GlobalValue GlobalValueToUpdate
        {
            get { return mGlobalValueToUpdate; }
            set
            {
                if (value != mGlobalValueToUpdate)
                {
                    HandlePropertyChange(this, "GlobalValueToUpdate", mGlobalValueToUpdate, value);
                    if (mGlobalValueToUpdate != null && mCalcTool.RootOperation.Result != null)
                    {
                        mGlobalValueToUpdate.UnregisterUpdater(mCalcTool.RootOperation.Result);
                    }
                    mGlobalValueToUpdate = value;
                    if (mGlobalValueToUpdate != null && mCalcTool.RootOperation.Result != null)
                    {
                        mGlobalValueToUpdate.RegisterUpdater(mCalcTool.RootOperation.Result);
                    }
                }
            }
        }
            
        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (mCalcTool == theOtherObject) return true; // this should be unnecessary if all objects return true if theOtherObject == this
            if (mCalcTool != null && mCalcTool.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mCalcTool != null) result = Math.Max(result, mCalcTool.ToolMapRow);
                return result + 1;
            }
        }

        private CalculationToolDefinition mCalcTool = null;
        public CalculationToolDefinition CalculationTool() { return mCalcTool; }
    }
}

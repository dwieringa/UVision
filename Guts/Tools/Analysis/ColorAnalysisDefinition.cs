// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;

namespace NetCams
{
    /*
     * R, G, B, Grey, H, S, I
     * avg, min, max, stddev
     * 
     * iterate thru once for avg, min, max
     * iterate thru again for stddev
     */

    public class ColorAnalysisDefinition : NetCams.ToolDefinition
    {
        public ColorAnalysisDefinition(TestSequence testSequence)
            : base(testSequence)
        {
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            new ColorAnalysisInstance(this, theExecution);
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            if (theOtherObject == this) return true;
            if (mROI != null && mROI.IsDependentOn(theOtherObject)) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
        }

        public override int ToolMapRow
        {
            get
            {
                int result = base.ToolMapRow - 1;
                if (mROI != null) result = Math.Max(result, mROI.ToolMapRow);
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                return result + 1;
            }
        }

        private ImageDefinition mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageDefinition SourceImage
        {
            get { return mSourceImage; }
            set
            {
                HandlePropertyChange(this, "SourceImage", mSourceImage, value);
                mSourceImage = value;
            }
        }

        private ROIDefinition mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("ROI to test over")]
        public ROIDefinition ROI
        {
            get { return mROI; }
            set
            {
                HandlePropertyChange(this, "ROI", mROI, value);
                mROI = value;
            }
        }

        private GeneratedValueDefinition mH_Average = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueDefinition H_Average
        {
            get { return mH_Average; }
        }
        private bool mH_Average_Enabled = false;
        [CategoryAttribute("Output : HSI : H"),
        DescriptionAttribute("")]
        public bool H_Average_Enabled
        {
            get { return mH_Average_Enabled; }
            set
            {
                HandlePropertyChange(this, "H_Average_Enabled", mH_Average_Enabled, value);
                mH_Average_Enabled = value;
                if (!value && mH_Average != null)
                {
                    mH_Average.Dispose_UVision();
                    mH_Average = null;
                }
                if (value && mH_Average == null)
                {
                    mH_Average = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "H_Average"));
                    mH_Average.Type = DataType.IntegerNumber;
                    mH_Average.AddDependency(this);
                    mH_Average.Name = "H_Average";
                }
            }
        }

        private GeneratedValueDefinition mH_Min = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueDefinition H_Min
        {
            get { return mH_Min; }
        }
        private GeneratedValueDefinition mH_Max = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueDefinition H_Max
        {
            get { return mH_Max; }
        }
        private bool mH_MinMax_Enabled = false;
        [CategoryAttribute("Output : HSI : H"),
        DescriptionAttribute("")]
        public bool H_MinMax_Enabled
        {
            get { return mH_MinMax_Enabled; }
            set
            {
                HandlePropertyChange(this, "H_MinMax_Enabled", mH_MinMax_Enabled, value);
                mH_MinMax_Enabled = value;
                if (!value && mH_Min != null)
                {
                    mH_Min.Dispose_UVision();
                    mH_Min = null;
                }
                if (!value && mH_Max != null)
                {
                    mH_Max.Dispose_UVision();
                    mH_Max = null;
                }
                if (value && mH_Min == null)
                {
                    mH_Min = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "H_Min"));
                    mH_Min.Type = DataType.IntegerNumber;
                    mH_Min.AddDependency(this);
                    mH_Min.Name = "H_Min";
                }
                if (value && mH_Max == null)
                {
                    mH_Max = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "H_Max"));
                    mH_Max.Type = DataType.IntegerNumber;
                    mH_Max.AddDependency(this);
                    mH_Max.Name = "H_Max";
                }
            }
        }

        private GeneratedValueDefinition mH_StdDev = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueDefinition H_StdDev
        {
            get { return mH_StdDev; }
        }
        private bool mH_StdDev_Enabled = false;
        [CategoryAttribute("Output : HSI : H"),
        DescriptionAttribute("")]
        public bool H_StdDev_Enabled
        {
            get { return mH_StdDev_Enabled; }
            set
            {
                HandlePropertyChange(this, "H_StdDev_Enabled", mH_StdDev_Enabled, value);
                mH_StdDev_Enabled = value;
                if (!value && mH_StdDev != null)
                {
                    mH_StdDev.Dispose_UVision();
                    mH_StdDev = null;
                }
                if (value && mH_StdDev == null)
                {
                    mH_StdDev = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "H_StdDev"));
                    mH_StdDev.Type = DataType.DecimalNumber;
                    mH_StdDev.AddDependency(this);
                    mH_StdDev.Name = "H_StdDev";
                }
            }
        }

        private GeneratedValueDefinition mS_Average = null;
        [CategoryAttribute("Output : HSI : S")]
        public GeneratedValueDefinition S_Average
        {
            get { return mS_Average; }
        }
        private bool mS_Average_Enabled = false;
        [CategoryAttribute("Output : HSI : S"),
        DescriptionAttribute("")]
        public bool S_Average_Enabled
        {
            get { return mS_Average_Enabled; }
            set
            {
                HandlePropertyChange(this, "S_Average_Enabled", mS_Average_Enabled, value);
                mS_Average_Enabled = value;
                if (!value && mS_Average != null)
                {
                    mS_Average.Dispose_UVision();
                    mS_Average = null;
                }
                if (value && mS_Average == null)
                {
                    mS_Average = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "S_Average"));
                    mS_Average.Type = DataType.IntegerNumber;
                    mS_Average.AddDependency(this);
                    mS_Average.Name = "S_Average";
                }
            }
        }

        private GeneratedValueDefinition mS_Min = null;
        [CategoryAttribute("Output : HSI : S")]
        public GeneratedValueDefinition S_Min
        {
            get { return mS_Min; }
        }
        private GeneratedValueDefinition mS_Max = null;
        [CategoryAttribute("Output : HSI : S")]
        public GeneratedValueDefinition S_Max
        {
            get { return mS_Max; }
        }
        private bool mS_MinMax_Enabled = false;
        [CategoryAttribute("Output : HSI : S"),
        DescriptionAttribute("")]
        public bool S_MinMax_Enabled
        {
            get { return mS_MinMax_Enabled; }
            set
            {
                HandlePropertyChange(this, "S_MinMax_Enabled", mS_MinMax_Enabled, value);
                mS_MinMax_Enabled = value;
                if (!value && mS_Min != null)
                {
                    mS_Min.Dispose_UVision();
                    mS_Min = null;
                }
                if (!value && mS_Max != null)
                {
                    mS_Max.Dispose_UVision();
                    mS_Max = null;
                }
                if (value && mS_Min == null)
                {
                    mS_Min = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "S_Min"));
                    mS_Min.Type = DataType.IntegerNumber;
                    mS_Min.AddDependency(this);
                    mS_Min.Name = "S_Min";
                }
                if (value && mS_Max == null)
                {
                    mS_Max = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "S_Max"));
                    mS_Max.Type = DataType.IntegerNumber;
                    mS_Max.AddDependency(this);
                    mS_Max.Name = "S_Max";
                }
            }
        }

        private GeneratedValueDefinition mS_StdDev = null;
        [CategoryAttribute("Output : HSI : S")]
        public GeneratedValueDefinition S_StdDev
        {
            get { return mS_StdDev; }
        }
        private bool mS_StdDev_Enabled = false;
        [CategoryAttribute("Output : HSI : S"),
        DescriptionAttribute("")]
        public bool S_StdDev_Enabled
        {
            get { return mS_StdDev_Enabled; }
            set
            {
                HandlePropertyChange(this, "S_StdDev_Enabled", mS_StdDev_Enabled, value);
                mS_StdDev_Enabled = value;
                if (!value && mS_StdDev != null)
                {
                    mS_StdDev.Dispose_UVision();
                    mS_StdDev = null;
                }
                if (value && mS_StdDev == null)
                {
                    mS_StdDev = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "S_StdDev"));
                    mS_StdDev.Type = DataType.DecimalNumber;
                    mS_StdDev.AddDependency(this);
                    mS_StdDev.Name = "S_StdDev";
                }
            }
        }

        private GeneratedValueDefinition mI_Average = null;
        [CategoryAttribute("Output : HSI : I")]
        public GeneratedValueDefinition I_Average
        {
            get { return mI_Average; }
        }
        private bool mI_Average_Enabled = false;
        [CategoryAttribute("Output : HSI : I"),
        DescriptionAttribute("")]
        public bool I_Average_Enabled
        {
            get { return mI_Average_Enabled; }
            set
            {
                HandlePropertyChange(this, "I_Average_Enabled", mI_Average_Enabled, value);
                mI_Average_Enabled = value;
                if (!value && mI_Average != null)
                {
                    mI_Average.Dispose_UVision();
                    mI_Average = null;
                }
                if (value && mI_Average == null)
                {
                    mI_Average = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "I_Average"));
                    mI_Average.Type = DataType.IntegerNumber;
                    mI_Average.AddDependency(this);
                    mI_Average.Name = Name+"_I_Avg";
                }
            }
        }

        private GeneratedValueDefinition mI_Min = null;
        [CategoryAttribute("Output : HSI : I")]
        public GeneratedValueDefinition I_Min
        {
            get { return mI_Min; }
        }
        private GeneratedValueDefinition mI_Max = null;
        [CategoryAttribute("Output : HSI : I")]
        public GeneratedValueDefinition I_Max
        {
            get { return mI_Max; }
        }
        private bool mI_MinMax_Enabled = false;
        [CategoryAttribute("Output : HSI : I"),
        DescriptionAttribute("")]
        public bool I_MinMax_Enabled
        {
            get { return mI_MinMax_Enabled; }
            set
            {
                HandlePropertyChange(this, "I_MinMax_Enabled", mI_MinMax_Enabled, value);
                mI_MinMax_Enabled = value;
                if (!value && mI_Min != null)
                {
                    mI_Min.Dispose_UVision();
                    mI_Min = null;
                }
                if (!value && mI_Max != null)
                {
                    mI_Max.Dispose_UVision();
                    mI_Max = null;
                }
                if (value && mI_Min == null)
                {
                    mI_Min = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "I_Min"));
                    mI_Min.Type = DataType.IntegerNumber;
                    mI_Min.AddDependency(this);
                    mI_Min.Name = "I_Min";
                }
                if (value && mI_Max == null)
                {
                    mI_Max = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "I_Max"));
                    mI_Max.Type = DataType.IntegerNumber;
                    mI_Max.AddDependency(this);
                    mI_Max.Name = "I_Max";
                }
            }
        }

        private GeneratedValueDefinition mI_StdDev = null;
        [CategoryAttribute("Output : HSI : I")]
        public GeneratedValueDefinition I_StdDev
        {
            get { return mI_StdDev; }
        }
        private bool mI_StdDev_Enabled = false;
        [CategoryAttribute("Output : HSI : I"),
        DescriptionAttribute("")]
        public bool I_StdDev_Enabled
        {
            get { return mI_StdDev_Enabled; }
            set
            {
                HandlePropertyChange(this, "I_StdDev_Enabled", mI_StdDev_Enabled, value);
                mI_StdDev_Enabled = value;
                if (!value && mI_StdDev != null)
                {
                    mI_StdDev.Dispose_UVision();
                    mI_StdDev = null;
                }
                if (value && mI_StdDev == null)
                {
                    mI_StdDev = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "I_StdDev"));
                    mI_StdDev.Type = DataType.DecimalNumber;
                    mI_StdDev.AddDependency(this);
                    mI_StdDev.Name = "I_StdDev";
                }
            }
        }

        private GeneratedValueDefinition mR_Average = null;
        [CategoryAttribute("Output : RGB : R")]
        public GeneratedValueDefinition R_Average
        {
            get { return mR_Average; }
        }
        private bool mR_Average_Enabled = false;
        [CategoryAttribute("Output : RGB : R"),
        DescriptionAttribute("")]
        public bool R_Average_Enabled
        {
            get { return mR_Average_Enabled; }
            set
            {
                HandlePropertyChange(this, "R_Average_Enabled", mR_Average_Enabled, value);
                mR_Average_Enabled = value;
                if (!value && mR_Average != null)
                {
                    mR_Average.Dispose_UVision();
                    mR_Average = null;
                }
                if (value && mR_Average == null)
                {
                    mR_Average = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "R_Average"));
                    mR_Average.Type = DataType.IntegerNumber;
                    mR_Average.AddDependency(this);
                    mR_Average.Name = "R_Average";
                }
            }
        }

        private GeneratedValueDefinition mR_Min = null;
        [CategoryAttribute("Output : RGB : R")]
        public GeneratedValueDefinition R_Min
        {
            get { return mR_Min; }
        }
        private GeneratedValueDefinition mR_Max = null;
        [CategoryAttribute("Output : RGB : R")]
        public GeneratedValueDefinition R_Max
        {
            get { return mR_Max; }
        }
        private bool mR_MinMax_Enabled = false;
        [CategoryAttribute("Output : RGB : R"),
        DescriptionAttribute("")]
        public bool R_MinMax_Enabled
        {
            get { return mR_MinMax_Enabled; }
            set
            {
                HandlePropertyChange(this, "R_MinMax_Enabled", mR_MinMax_Enabled, value);
                mR_MinMax_Enabled = value;
                if (!value && mR_Min != null)
                {
                    mR_Min.Dispose_UVision();
                    mR_Min = null;
                }
                if (!value && mR_Max != null)
                {
                    mR_Max.Dispose_UVision();
                    mR_Max = null;
                }
                if (value && mR_Min == null)
                {
                    mR_Min = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "R_Min"));
                    mR_Min.Type = DataType.IntegerNumber;
                    mR_Min.AddDependency(this);
                    mR_Min.Name = "R_Min";
                }
                if (value && mR_Max == null)
                {
                    mR_Max = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "R_Max"));
                    mR_Max.Type = DataType.IntegerNumber;
                    mR_Max.AddDependency(this);
                    mR_Max.Name = "R_Max";
                }
            }
        }

        private GeneratedValueDefinition mR_StdDev = null;
        [CategoryAttribute("Output : RGB : R")]
        public GeneratedValueDefinition R_StdDev
        {
            get { return mR_StdDev; }
        }
        private bool mR_StdDev_Enabled = false;
        [CategoryAttribute("Output : RGB : R"),
        DescriptionAttribute("")]
        public bool R_StdDev_Enabled
        {
            get { return mR_StdDev_Enabled; }
            set
            {
                HandlePropertyChange(this, "R_StdDev_Enabled", mR_StdDev_Enabled, value);
                mR_StdDev_Enabled = value;
                if (!value && mR_StdDev != null)
                {
                    mR_StdDev.Dispose_UVision();
                    mR_StdDev = null;
                }
                if (value && mR_StdDev == null)
                {
                    mR_StdDev = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "R_StdDev"));
                    mR_StdDev.Type = DataType.DecimalNumber;
                    mR_StdDev.AddDependency(this);
                    mR_StdDev.Name = "R_StdDev";
                }
            }
        }

        private GeneratedValueDefinition mG_Average = null;
        [CategoryAttribute("Output : RGB : G")]
        public GeneratedValueDefinition G_Average
        {
            get { return mG_Average; }
        }
        private bool mG_Average_Enabled = false;
        [CategoryAttribute("Output : RGB : G"),
        DescriptionAttribute("")]
        public bool G_Average_Enabled
        {
            get { return mG_Average_Enabled; }
            set
            {
                HandlePropertyChange(this, "G_Average_Enabled", mG_Average_Enabled, value);
                mG_Average_Enabled = value;
                if (!value && mG_Average != null)
                {
                    mG_Average.Dispose_UVision();
                    mG_Average = null;
                }
                if (value && mG_Average == null)
                {
                    mG_Average = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "G_Average"));
                    mG_Average.Type = DataType.IntegerNumber;
                    mG_Average.AddDependency(this);
                    mG_Average.Name = "G_Average";
                }
            }
        }

        private GeneratedValueDefinition mG_Min = null;
        [CategoryAttribute("Output : RGB : G")]
        public GeneratedValueDefinition G_Min
        {
            get { return mG_Min; }
        }
        private GeneratedValueDefinition mG_Max = null;
        [CategoryAttribute("Output : RGB : G")]
        public GeneratedValueDefinition G_Max
        {
            get { return mG_Max; }
        }
        private bool mG_MinMax_Enabled = false;
        [CategoryAttribute("Output : RGB : G"),
        DescriptionAttribute("")]
        public bool G_MinMax_Enabled
        {
            get { return mG_MinMax_Enabled; }
            set
            {
                HandlePropertyChange(this, "G_MinMax_Enabled", mG_MinMax_Enabled, value);
                mG_MinMax_Enabled = value;
                if (!value && mG_Min != null)
                {
                    mG_Min.Dispose_UVision();
                    mG_Min = null;
                }
                if (!value && mG_Max != null)
                {
                    mG_Max.Dispose_UVision();
                    mG_Max = null;
                }
                if (value && mG_Min == null)
                {
                    mG_Min = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "G_Min"));
                    mG_Min.Type = DataType.IntegerNumber;
                    mG_Min.AddDependency(this);
                    mG_Min.Name = "G_Min";
                }
                if (value && mG_Max == null)
                {
                    mG_Max = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "G_Max"));
                    mG_Max.Type = DataType.IntegerNumber;
                    mG_Max.AddDependency(this);
                    mG_Max.Name = "G_Max";
                }
            }
        }

        private GeneratedValueDefinition mG_StdDev = null;
        [CategoryAttribute("Output : RGB : G")]
        public GeneratedValueDefinition G_StdDev
        {
            get { return mG_StdDev; }
        }
        private bool mG_StdDev_Enabled = false;
        [CategoryAttribute("Output : RGB : G"),
        DescriptionAttribute("")]
        public bool G_StdDev_Enabled
        {
            get { return mG_StdDev_Enabled; }
            set
            {
                HandlePropertyChange(this, "G_StdDev_Enabled", mG_StdDev_Enabled, value);
                mG_StdDev_Enabled = value;
                if (!value && mG_StdDev != null)
                {
                    mG_StdDev.Dispose_UVision();
                    mG_StdDev = null;
                }
                if (value && mG_StdDev == null)
                {
                    mG_StdDev = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "G_StdDev"));
                    mG_StdDev.Type = DataType.DecimalNumber;
                    mG_StdDev.AddDependency(this);
                    mG_StdDev.Name = "G_StdDev";
                }
            }
        }

        private GeneratedValueDefinition mB_Average = null;
        [CategoryAttribute("Output : RGB : B")]
        public GeneratedValueDefinition B_Average
        {
            get { return mB_Average; }
        }
        private bool mB_Average_Enabled = false;
        [CategoryAttribute("Output : RGB : B"),
        DescriptionAttribute("")]
        public bool B_Average_Enabled
        {
            get { return mB_Average_Enabled; }
            set
            {
                HandlePropertyChange(this, "B_Average_Enabled", mB_Average_Enabled, value);
                mB_Average_Enabled = value;
                if (!value && mB_Average != null)
                {
                    mB_Average.Dispose_UVision();
                    mB_Average = null;
                }
                if (value && mB_Average == null)
                {
                    mB_Average = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "B_Average"));
                    mB_Average.Type = DataType.IntegerNumber;
                    mB_Average.AddDependency(this);
                    mB_Average.Name = "B_Average";
                }
            }
        }

        private GeneratedValueDefinition mB_Min = null;
        [CategoryAttribute("Output : RGB : B")]
        public GeneratedValueDefinition B_Min
        {
            get { return mB_Min; }
        }
        private GeneratedValueDefinition mB_Max = null;
        [CategoryAttribute("Output : RGB : B")]
        public GeneratedValueDefinition B_Max
        {
            get { return mB_Max; }
        }
        private bool mB_MinMax_Enabled = false;
        [CategoryAttribute("Output : RGB : B"),
        DescriptionAttribute("")]
        public bool B_MinMax_Enabled
        {
            get { return mB_MinMax_Enabled; }
            set
            {
                HandlePropertyChange(this, "B_MinMax_Enabled", mB_MinMax_Enabled, value);
                mB_MinMax_Enabled = value;
                if (!value && mB_Min != null)
                {
                    mB_Min.Dispose_UVision();
                    mB_Min = null;
                }
                if (!value && mB_Max != null)
                {
                    mB_Max.Dispose_UVision();
                    mB_Max = null;
                }
                if (value && mB_Min == null)
                {
                    mB_Min = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "B_Min"));
                    mB_Min.Type = DataType.IntegerNumber;
                    mB_Min.AddDependency(this);
                    mB_Min.Name = "B_Min";
                }
                if (value && mB_Max == null)
                {
                    mB_Max = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "B_Max"));
                    mB_Max.Type = DataType.IntegerNumber;
                    mB_Max.AddDependency(this);
                    mB_Max.Name = "B_Max";
                }
            }
        }

        private GeneratedValueDefinition mB_StdDev = null;
        [CategoryAttribute("Output : RGB : B")]
        public GeneratedValueDefinition B_StdDev
        {
            get { return mB_StdDev; }
        }
        private bool mB_StdDev_Enabled = false;
        [CategoryAttribute("Output : RGB : B"),
        DescriptionAttribute("")]
        public bool B_StdDev_Enabled
        {
            get { return mB_StdDev_Enabled; }
            set
            {
                HandlePropertyChange(this, "B_StdDev_Enabled", mB_StdDev_Enabled, value);
                mB_StdDev_Enabled = value;
                if (!value && mB_StdDev != null)
                {
                    mB_StdDev.Dispose_UVision();
                    mB_StdDev = null;
                }
                if (value && mB_StdDev == null)
                {
                    mB_StdDev = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "B_StdDev"));
                    mB_StdDev.Type = DataType.DecimalNumber;
                    mB_StdDev.AddDependency(this);
                    mB_StdDev.Name = "B_StdDev";
                }
            }
        }

        private GeneratedValueDefinition mGrey_Average = null;
        [CategoryAttribute("Output : Grey")]
        public GeneratedValueDefinition Grey_Average
        {
            get { return mGrey_Average; }
        }
        private bool mGrey_Average_Enabled = false;
        [CategoryAttribute("Output : Grey"),
        DescriptionAttribute("")]
        public bool Grey_Average_Enabled
        {
            get { return mGrey_Average_Enabled; }
            set
            {
                HandlePropertyChange(this, "Grey_Average_Enabled", mGrey_Average_Enabled, value);
                mGrey_Average_Enabled = value;
                if (!value && mGrey_Average != null)
                {
                    mGrey_Average.Dispose_UVision();
                    mGrey_Average = null;
                }
                if (value && mGrey_Average == null)
                {
                    mGrey_Average = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "Grey_Average"));
                    mGrey_Average.Type = DataType.IntegerNumber;
                    mGrey_Average.AddDependency(this);
                    mGrey_Average.Name = "Grey_Average";
                }
            }
        }

        private GeneratedValueDefinition mGrey_Min = null;
        [CategoryAttribute("Output : Grey")]
        public GeneratedValueDefinition Grey_Min
        {
            get { return mGrey_Min; }
        }
        private GeneratedValueDefinition mGrey_Max = null;
        [CategoryAttribute("Output : Grey")]
        public GeneratedValueDefinition Grey_Max
        {
            get { return mGrey_Max; }
        }
        private bool mGrey_MinMax_Enabled = false;
        [CategoryAttribute("Output : Grey"),
        DescriptionAttribute("")]
        public bool Grey_MinMax_Enabled
        {
            get { return mGrey_MinMax_Enabled; }
            set
            {
                HandlePropertyChange(this, "Grey_MinMax_Enabled", mGrey_MinMax_Enabled, value);
                mGrey_MinMax_Enabled = value;
                if (!value && mGrey_Min != null)
                {
                    mGrey_Min.Dispose_UVision();
                    mGrey_Min = null;
                }
                if (!value && mGrey_Max != null)
                {
                    mGrey_Max.Dispose_UVision();
                    mGrey_Max = null;
                }
                if (value && mGrey_Min == null)
                {
                    mGrey_Min = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "Grey_Min"));
                    mGrey_Min.Type = DataType.IntegerNumber;
                    mGrey_Min.AddDependency(this);
                    mGrey_Min.Name = "Grey_Min";
                }
                if (value && mGrey_Max == null)
                {
                    mGrey_Max = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "Grey_Max"));
                    mGrey_Max.Type = DataType.IntegerNumber;
                    mGrey_Max.AddDependency(this);
                    mGrey_Max.Name = "Grey_Max";
                }
            }
        }

        private GeneratedValueDefinition mGrey_StdDev = null;
        [CategoryAttribute("Output : Grey")]
        public GeneratedValueDefinition Grey_StdDev
        {
            get { return mGrey_StdDev; }
        }
        private bool mGrey_StdDev_Enabled = false;
        [CategoryAttribute("Output : Grey"),
        DescriptionAttribute("")]
        public bool Grey_StdDev_Enabled
        {
            get { return mGrey_StdDev_Enabled; }
            set
            {
                HandlePropertyChange(this, "Grey_StdDev_Enabled", mGrey_StdDev_Enabled, value);
                mGrey_StdDev_Enabled = value;
                if (!value && mGrey_StdDev != null)
                {
                    mGrey_StdDev.Dispose_UVision();
                    mGrey_StdDev = null;
                }
                if (value && mGrey_StdDev == null)
                {
                    mGrey_StdDev = new GeneratedValueDefinition(TestSequence(), OwnerLink.newLink(this, "Grey_StdDev"));
                    mGrey_StdDev.Type = DataType.DecimalNumber;
                    mGrey_StdDev.AddDependency(this);
                    mGrey_StdDev.Name = "Grey_StdDev";
                }
            }
        }
    }
}

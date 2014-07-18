// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public interface IReferencePointInstance : IObjectInstance
    {
        // Added support for rounding and doubles 8/28/2008 so I could use decimal values in MeasurementTool and CalibrationTool to support subpixel edge finding
        int GetValueAsRoundedInt();
        int GetValueAsTruncatedInt();
        double GetValueAsDouble();
    }
}

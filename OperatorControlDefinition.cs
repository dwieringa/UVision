// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Windows.Forms;

namespace NetCams
{
    public interface OperatorControlDefinition
    {
        ToolStripItem createControlInstance(int controlIndex);
        int numberOfControls();

        void RegisterNewOperatorForm(OperationForm theNewForm);
        bool IsNewFormWaiting();
        OperationForm GetNewWaitingForm();
    }
}

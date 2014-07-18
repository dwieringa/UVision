// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NetCams
{
    public partial class OperationPropertiesForm : DockContent
    {
        public OperationPropertiesForm()
        {
            InitializeComponent();
            TabText = "Setup";
        }
    }
}
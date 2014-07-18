// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NetCams
{
    public partial class FavoriteSettingsForm : DockContent
    {
        public FavoriteSettingsForm(OperationForm opForm)
        {
            InitializeComponent();
            TabText = "Favorite Settings";
            mOpForm = opForm;
            this.Disposed += new EventHandler(FavoriteSettingsForm_Disposed);
        }
        private OperationForm mOpForm;

        void FavoriteSettingsForm_Disposed(object sender, EventArgs e)
        {
            mOpForm.favSettingsForm = null;
        }
    }
}
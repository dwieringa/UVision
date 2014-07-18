// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

namespace NetCams
{
    partial class OperationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblProbeOutput = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblProbeSample = new System.Windows.Forms.ToolStripStatusLabel();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.favoriteSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.favoriteValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.activeLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.studyChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sequencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bigImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newImageGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.cmbTestCollection = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLiveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripEnableSeqButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripProbeButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripStudyButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSaveLayoutButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Operator = new System.Windows.Forms.ToolStrip();
            this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.BottomToolStripPanel
            // 
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.AutoScroll = true;
            this.toolStripContainer.ContentPanel.Controls.Add(this.dockPanel);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(632, 340);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(632, 440);
            this.toolStripContainer.TabIndex = 2;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip_Operator);
            this.toolStripContainer.TopToolStripPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.toolStripContainer_TopToolStripPanel_Paint);
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProbeOutput,
            this.lblProbeSample});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(632, 23);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblProbeOutput
            // 
            this.lblProbeOutput.Name = "lblProbeOutput";
            this.lblProbeOutput.Size = new System.Drawing.Size(33, 18);
            this.lblProbeOutput.Text = "     ";
            // 
            // lblProbeSample
            // 
            this.lblProbeSample.Name = "lblProbeSample";
            this.lblProbeSample.Size = new System.Drawing.Size(33, 18);
            this.lblProbeSample.Text = "     ";
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.dockPanel.Location = new System.Drawing.Point(0, 0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(632, 340);
            this.dockPanel.TabIndex = 2;
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(632, 26);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lockLayoutToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.favoriteSettingsToolStripMenuItem,
            this.favoriteValuesToolStripMenuItem,
            this.reportToolStripMenuItem,
            this.logToolStripMenuItem,
            this.activeLogToolStripMenuItem,
            this.studyChartToolStripMenuItem,
            this.sequencesToolStripMenuItem,
            this.toolStripSeparator2,
            this.bigImageToolStripMenuItem,
            this.newImageGridToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(49, 22);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // lockLayoutToolStripMenuItem
            // 
            this.lockLayoutToolStripMenuItem.Name = "lockLayoutToolStripMenuItem";
            this.lockLayoutToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.lockLayoutToolStripMenuItem.Text = "Lock Layout";
            this.lockLayoutToolStripMenuItem.Click += new System.EventHandler(this.lockLayoutToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(208, 6);
            // 
            // favoriteSettingsToolStripMenuItem
            // 
            this.favoriteSettingsToolStripMenuItem.Name = "favoriteSettingsToolStripMenuItem";
            this.favoriteSettingsToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.favoriteSettingsToolStripMenuItem.Text = "Favorite Settings";
            this.favoriteSettingsToolStripMenuItem.Click += new System.EventHandler(this.favoriteSettingsToolStripMenuItem_Click);
            // 
            // favoriteValuesToolStripMenuItem
            // 
            this.favoriteValuesToolStripMenuItem.Name = "favoriteValuesToolStripMenuItem";
            this.favoriteValuesToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.favoriteValuesToolStripMenuItem.Text = "Favorite Values";
            this.favoriteValuesToolStripMenuItem.Click += new System.EventHandler(this.favoriteValuesToolStripMenuItem_Click);
            // 
            // reportToolStripMenuItem
            // 
            this.reportToolStripMenuItem.Name = "reportToolStripMenuItem";
            this.reportToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.reportToolStripMenuItem.Text = "Report";
            this.reportToolStripMenuItem.Click += new System.EventHandler(this.reportToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // activeLogToolStripMenuItem
            // 
            this.activeLogToolStripMenuItem.Name = "activeLogToolStripMenuItem";
            this.activeLogToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.activeLogToolStripMenuItem.Text = "Log of Active Test";
            this.activeLogToolStripMenuItem.Click += new System.EventHandler(this.activeLogToolStripMenuItem_Click);
            // 
            // studyChartToolStripMenuItem
            // 
            this.studyChartToolStripMenuItem.Name = "studyChartToolStripMenuItem";
            this.studyChartToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.studyChartToolStripMenuItem.Text = "Study Chart";
            this.studyChartToolStripMenuItem.Click += new System.EventHandler(this.studyChartToolStripMenuItem_Click);
            // 
            // sequencesToolStripMenuItem
            // 
            this.sequencesToolStripMenuItem.Name = "sequencesToolStripMenuItem";
            this.sequencesToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.sequencesToolStripMenuItem.Text = "Sequences";
            this.sequencesToolStripMenuItem.Click += new System.EventHandler(this.sequencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(208, 6);
            // 
            // bigImageToolStripMenuItem
            // 
            this.bigImageToolStripMenuItem.Name = "bigImageToolStripMenuItem";
            this.bigImageToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.bigImageToolStripMenuItem.Text = "Big Image";
            this.bigImageToolStripMenuItem.Click += new System.EventHandler(this.bigImageToolStripMenuItem_Click);
            // 
            // newImageGridToolStripMenuItem
            // 
            this.newImageGridToolStripMenuItem.Name = "newImageGridToolStripMenuItem";
            this.newImageGridToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.newImageGridToolStripMenuItem.Text = "New Image Grid";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(48, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbTestCollection,
            this.toolStripLiveButton,
            this.toolStripEnableSeqButton,
            this.toolStripProbeButton,
            this.toolStripStudyButton,
            this.toolStripSaveLayoutButton});
            this.toolStrip.Location = new System.Drawing.Point(3, 26);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(552, 26);
            this.toolStrip.TabIndex = 0;
            // 
            // cmbTestCollection
            // 
            this.cmbTestCollection.Name = "cmbTestCollection";
            this.cmbTestCollection.Size = new System.Drawing.Size(121, 26);
            // 
            // toolStripLiveButton
            // 
            this.toolStripLiveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripLiveButton.Name = "toolStripLiveButton";
            this.toolStripLiveButton.Size = new System.Drawing.Size(71, 23);
            this.toolStripLiveButton.Text = "Live View";
            this.toolStripLiveButton.Click += new System.EventHandler(this.toolStripLiveButton_Click);
            // 
            // toolStripEnableSeqButton
            // 
            this.toolStripEnableSeqButton.BackColor = System.Drawing.Color.Red;
            this.toolStripEnableSeqButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripEnableSeqButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripEnableSeqButton.Name = "toolStripEnableSeqButton";
            this.toolStripEnableSeqButton.Size = new System.Drawing.Size(143, 23);
            this.toolStripEnableSeqButton.Text = "Disabled - RESTART";
            this.toolStripEnableSeqButton.ToolTipText = "This Test Sequence has been disabled.  Click to re-enabled it.";
            this.toolStripEnableSeqButton.Click += new System.EventHandler(this.toolStripEnableSeqButton_Click);
            // 
            // toolStripProbeButton
            // 
            this.toolStripProbeButton.CheckOnClick = true;
            this.toolStripProbeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripProbeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripProbeButton.Name = "toolStripProbeButton";
            this.toolStripProbeButton.Size = new System.Drawing.Size(49, 23);
            this.toolStripProbeButton.Text = "Probe";
            this.toolStripProbeButton.ToolTipText = "Determine color attributes of individual pixels.";
            this.toolStripProbeButton.Click += new System.EventHandler(this.toolStripProbeButton_Click);
            // 
            // toolStripStudyButton
            // 
            this.toolStripStudyButton.CheckOnClick = true;
            this.toolStripStudyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStudyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStudyButton.Name = "toolStripStudyButton";
            this.toolStripStudyButton.Size = new System.Drawing.Size(49, 23);
            this.toolStripStudyButton.Text = "Study";
            this.toolStripStudyButton.ToolTipText = "Study a user-specified retangle within an image.";
            this.toolStripStudyButton.Click += new System.EventHandler(this.toolStripStudyButton_Click);
            // 
            // toolStripSaveLayoutButton
            // 
            this.toolStripSaveLayoutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSaveLayoutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSaveLayoutButton.Name = "toolStripSaveLayoutButton";
            this.toolStripSaveLayoutButton.Size = new System.Drawing.Size(105, 23);
            this.toolStripSaveLayoutButton.Text = "Save Changes";
            this.toolStripSaveLayoutButton.Click += new System.EventHandler(this.toolStripSaveLayoutButton_Click);
            // 
            // toolStrip_Operator
            // 
            this.toolStrip_Operator.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip_Operator.Location = new System.Drawing.Point(3, 52);
            this.toolStrip_Operator.Name = "toolStrip_Operator";
            this.toolStrip_Operator.Size = new System.Drawing.Size(43, 25);
            this.toolStrip_Operator.TabIndex = 2;
            // 
            // OperationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 440);
            this.Controls.Add(this.toolStripContainer);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "OperationForm";
            this.Text = "OperationForm";
            this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        public System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripComboBox cmbTestCollection;
        public WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        public System.Windows.Forms.ToolStripStatusLabel lblProbeOutput;
        public System.Windows.Forms.ToolStripStatusLabel lblProbeSample;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lockLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem favoriteSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem favoriteValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sequencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem bigImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newImageGridToolStripMenuItem;
        public System.Windows.Forms.ToolStripButton toolStripLiveButton;
        private System.Windows.Forms.ToolStripButton toolStripEnableSeqButton;
        private System.Windows.Forms.ToolStripMenuItem activeLogToolStripMenuItem;
        public System.Windows.Forms.ToolStripButton toolStripStudyButton;
        private System.Windows.Forms.ToolStripMenuItem studyChartToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripSaveLayoutButton;
        public System.Windows.Forms.ToolStripButton toolStripProbeButton;
        private System.Windows.Forms.ToolStrip toolStrip_Operator;

    }
}
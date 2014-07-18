// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

namespace NetCams
{
    partial class ImageHistoryForm
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
            this.scrollPanel = new System.Windows.Forms.Panel();
            this.historyScroll = new System.Windows.Forms.HScrollBar();
            this.liveScrollButton = new System.Windows.Forms.Button();
            this.imageLayoutPanel = new System.Windows.Forms.Panel();
            this.scrollPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrollPanel
            // 
            this.scrollPanel.Controls.Add(this.historyScroll);
            this.scrollPanel.Controls.Add(this.liveScrollButton);
            this.scrollPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scrollPanel.Location = new System.Drawing.Point(0, 469);
            this.scrollPanel.Name = "scrollPanel";
            this.scrollPanel.Size = new System.Drawing.Size(785, 26);
            this.scrollPanel.TabIndex = 3;
            // 
            // historyScroll
            // 
            this.historyScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyScroll.Location = new System.Drawing.Point(0, 0);
            this.historyScroll.Name = "historyScroll";
            this.historyScroll.Size = new System.Drawing.Size(733, 26);
            this.historyScroll.TabIndex = 0;
            this.historyScroll.ValueChanged += new System.EventHandler(this.historyScroll_ValueChanged);
            this.historyScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.historyScroll_Scroll);
            // 
            // liveScrollButton
            // 
            this.liveScrollButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.liveScrollButton.Location = new System.Drawing.Point(733, 0);
            this.liveScrollButton.Name = "liveScrollButton";
            this.liveScrollButton.Size = new System.Drawing.Size(52, 26);
            this.liveScrollButton.TabIndex = 1;
            this.liveScrollButton.Text = "Now";
            this.liveScrollButton.UseVisualStyleBackColor = true;
            this.liveScrollButton.Click += new System.EventHandler(this.liveScrollButton_Click);
            // 
            // imageLayoutPanel
            // 
            this.imageLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.imageLayoutPanel.Name = "imageLayoutPanel";
            this.imageLayoutPanel.Size = new System.Drawing.Size(785, 495);
            this.imageLayoutPanel.TabIndex = 4;
            // 
            // ImageHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 495);
            this.Controls.Add(this.scrollPanel);
            this.Controls.Add(this.imageLayoutPanel);
            this.Name = "ImageHistoryForm";
            this.TabText = "OperationImageForm";
            this.Text = "OperationImageForm";
            this.Enter += new System.EventHandler(this.ImageHistoryForm_Enter);
            this.Activated += new System.EventHandler(this.ImageHistoryForm_Activated);
            this.Validated += new System.EventHandler(this.ImageHistoryForm_Validated);
            this.Load += new System.EventHandler(this.ImageHistoryForm_Load);
            this.scrollPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel scrollPanel;
        public System.Windows.Forms.HScrollBar historyScroll;
        private System.Windows.Forms.Button liveScrollButton;
        private System.Windows.Forms.Panel imageLayoutPanel;

    }
}
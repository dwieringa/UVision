// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

namespace NetCams
{
    partial class ImageBigViewForm
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
            this.imageLayoutPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // imageLayoutPanel
            // 
            this.imageLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.imageLayoutPanel.Name = "imageLayoutPanel";
            this.imageLayoutPanel.Size = new System.Drawing.Size(764, 407);
            this.imageLayoutPanel.TabIndex = 0;
            // 
            // ImageBigViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 407);
            this.Controls.Add(this.imageLayoutPanel);
            this.Name = "ImageBigViewForm";
            this.TabText = "Big View";
            this.Text = "ImageBigViewForm";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel imageLayoutPanel;
    }
}
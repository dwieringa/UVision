// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

namespace NetCams
{
    partial class OperationLogForm
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
            this.txtLogText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtLogText
            // 
            this.txtLogText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogText.Location = new System.Drawing.Point(0, 0);
            this.txtLogText.Multiline = true;
            this.txtLogText.Name = "txtLogText";
            this.txtLogText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogText.Size = new System.Drawing.Size(292, 260);
            this.txtLogText.TabIndex = 0;
            // 
            // OperationLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 260);
            this.Controls.Add(this.txtLogText);
            this.Name = "OperationLogForm";
            this.TabText = "OperationLogForm";
            this.Text = "OperationLogForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLogText;
    }
}
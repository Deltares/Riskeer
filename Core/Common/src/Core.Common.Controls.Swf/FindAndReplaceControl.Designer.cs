﻿namespace Core.Common.Controls.Swf
{
    partial class FindAndReplaceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindAndReplaceControl));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelReplaceWith = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonFind = new System.Windows.Forms.Button();
            this.buttonReplaceAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Find what";
            resources.ApplyResources(this.label1, "label1");
            // 
            // textBoxFind
            // 
            this.FindTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FindTextBox.Location = new System.Drawing.Point(81, 5);
            this.FindTextBox.Name = "FindTextBox";
            this.FindTextBox.Size = new System.Drawing.Size(223, 20);
            this.FindTextBox.TabIndex = 0;
            this.FindTextBox.TextChanged += new System.EventHandler(this.textBoxFind_TextChanged);
            this.FindTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFind_KeyDown);
            // 
            // buttonClose
            // 
            this.buttonClose.FlatAppearance.BorderSize = 0;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Image = global::Core.Common.Controls.Swf.Properties.Resources.cross;
            this.buttonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonClose.Location = new System.Drawing.Point(340, 5);
            this.buttonClose.Name = "buttonClose";
            this.toolTip1.SetToolTip(this.buttonClose, resources.GetString("buttonClose.ToolTip"));
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            this.buttonClose.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button_KeyDown);
            resources.ApplyResources(this.buttonClose, "buttonClose");
            // 
            // labelReplaceWith
            // 
            this.labelReplaceWith.Name = "labelReplaceWith";
            this.labelReplaceWith.Size = new System.Drawing.Size(69, 13);
            this.labelReplaceWith.TabIndex = 6;
            this.labelReplaceWith.Text = "Replace with";
            resources.ApplyResources(this.labelReplaceWith, "labelReplaceWith");
            // 
            // textBoxReplace
            // 
            this.ReplaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceTextBox.Location = new System.Drawing.Point(81, 30);
            this.ReplaceTextBox.Name = "ReplaceTextBox";
            this.ReplaceTextBox.Size = new System.Drawing.Size(223, 20);
            this.ReplaceTextBox.TabIndex = 1;
            this.ReplaceTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFind_KeyDown);
            // 
            // buttonReplace
            // 
            this.buttonReplace.FlatAppearance.BorderSize = 0;
            this.buttonReplace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonReplace.Image = ((System.Drawing.Image)(resources.GetObject("buttonReplace.Image")));
            this.buttonReplace.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonReplace.Location = new System.Drawing.Point(310, 30);
            this.buttonReplace.Name = "buttonReplace";
            this.toolTip1.SetToolTip(this.buttonReplace, resources.GetString("buttonReplace.ToolTip"));
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            this.buttonReplace.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button_KeyDown);
            resources.ApplyResources(this.buttonReplace, "buttonReplace");
            // 
            // buttonFind
            // 
            this.buttonFind.FlatAppearance.BorderSize = 0;
            this.buttonFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFind.Image = global::Core.Common.Controls.Swf.Properties.Resources.binocular;
            this.buttonFind.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFind.Location = new System.Drawing.Point(310, 5);
            this.buttonFind.Name = "buttonFind";
            this.toolTip1.SetToolTip(this.buttonFind, resources.GetString("buttonFind.ToolTip"));
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            this.buttonFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button_KeyDown);
            resources.ApplyResources(this.buttonFind, "buttonFind");
            // 
            // buttonReplaceAll
            // 
            this.buttonReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReplaceAll.FlatAppearance.BorderSize = 0;
            this.buttonReplaceAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonReplaceAll.Image = ((System.Drawing.Image)(resources.GetObject("buttonReplaceAll.Image")));
            this.buttonReplaceAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonReplaceAll.Location = new System.Drawing.Point(340, 30);
            this.buttonReplaceAll.Name = "buttonReplaceAll";
            this.toolTip1.SetToolTip(this.buttonReplaceAll, resources.GetString("buttonReplaceAll.ToolTip"));
            this.buttonReplaceAll.UseVisualStyleBackColor = true;
            this.buttonReplaceAll.Click += new System.EventHandler(this.buttonReplaceAll_Click);
            this.buttonReplaceAll.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button_KeyDown);
            resources.ApplyResources(this.buttonReplaceAll, "buttonReplaceAll");
            // 
            // FindAndReplaceControl
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.buttonReplace);
            this.Controls.Add(this.buttonReplaceAll);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelReplaceWith);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.ReplaceTextBox);
            this.Controls.Add(this.FindTextBox);
            this.Name = "FindAndReplaceControl";
            this.Size = new System.Drawing.Size(368, 54);
            this.VisibleChanged += new System.EventHandler(this.FindAndReplaceControl_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonReplaceAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Label labelReplaceWith;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

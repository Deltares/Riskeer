// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Riskeer.Common.Forms
{
    partial class SelectionDialogBase<T>
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.ButtonGroupBox = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.DoForSelectedButton = new System.Windows.Forms.Button();
            this.CustomCancelButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButtonGroupBox.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridViewControl
            // 
            this.DataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridViewControl.Location = new System.Drawing.Point(3, 16);
            this.DataGridViewControl.MultiSelect = true;
            this.DataGridViewControl.Name = "DataGridViewControl";
            this.DataGridViewControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.DataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.DataGridViewControl.Size = new System.Drawing.Size(261, 66);
            this.DataGridViewControl.TabIndex = 0;
            // 
            // ButtonGroupBox
            // 
            this.ButtonGroupBox.Controls.Add(this.DataGridViewControl);
            this.ButtonGroupBox.Controls.Add(this.panel2);
            this.ButtonGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.Size = new System.Drawing.Size(267, 124);
            this.ButtonGroupBox.TabIndex = 7;
            this.ButtonGroupBox.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.SelectAllButton);
            this.panel2.Controls.Add(this.DeselectAllButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 82);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(261, 39);
            this.panel2.TabIndex = 6;
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SelectAllButton.Location = new System.Drawing.Point(9, 6);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(111, 23);
            this.SelectAllButton.TabIndex = 5;
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DeselectAllButton.Location = new System.Drawing.Point(126, 6);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(111, 23);
            this.DeselectAllButton.TabIndex = 4;
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // DoForSelectedButton
            // 
            this.DoForSelectedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DoForSelectedButton.Enabled = false;
            this.DoForSelectedButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DoForSelectedButton.Location = new System.Drawing.Point(80, 6);
            this.DoForSelectedButton.Name = "DoForSelectedButton";
            this.DoForSelectedButton.Size = new System.Drawing.Size(86, 23);
            this.DoForSelectedButton.TabIndex = 3;
            this.DoForSelectedButton.UseVisualStyleBackColor = true;
            this.DoForSelectedButton.Click += new System.EventHandler(this.DoForSelectedButton_Click);
            // 
            // CustomCancelButton
            // 
            this.CustomCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CustomCancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CustomCancelButton.Location = new System.Drawing.Point(172, 6);
            this.CustomCancelButton.Name = "CustomCancelButton";
            this.CustomCancelButton.Size = new System.Drawing.Size(86, 23);
            this.CustomCancelButton.TabIndex = 6;
            this.CustomCancelButton.UseVisualStyleBackColor = true;
            this.CustomCancelButton.Click += new System.EventHandler(this.CustomCancelButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DoForSelectedButton);
            this.panel1.Controls.Add(this.CustomCancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 124);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(267, 35);
            this.panel1.TabIndex = 8;
            // 
            // SelectionDialogBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(240, 90);
            this.ClientSize = new System.Drawing.Size(267, 159);
            this.Controls.Add(this.ButtonGroupBox);
            this.Controls.Add(this.panel1);
            this.Name = "SelectionDialogBase";
            this.Text = "SelectionDialogBase";
            this.ButtonGroupBox.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl DataGridViewControl;
        private System.Windows.Forms.GroupBox ButtonGroupBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.Button DeselectAllButton;
        protected System.Windows.Forms.Button DoForSelectedButton;
        protected System.Windows.Forms.Button CustomCancelButton;
        private System.Windows.Forms.Panel panel1;
    }
}
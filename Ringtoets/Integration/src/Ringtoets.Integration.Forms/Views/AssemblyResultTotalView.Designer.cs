// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    partial class AssemblyResultTotalView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.RefreshAssemblyResultsButton = new System.Windows.Forms.Button();
            this.buttonGroupBox = new System.Windows.Forms.GroupBox();
            this.buttonGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 43);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(789, 373);
            this.dataGridViewControl.TabIndex = 3;
            // 
            // RefreshAssemblyResultsButton
            // 
            this.RefreshAssemblyResultsButton.AutoSize = true;
            this.RefreshAssemblyResultsButton.Location = new System.Drawing.Point(3, 14);
            this.RefreshAssemblyResultsButton.Name = "RefreshAssemblyResultsButton";
            this.RefreshAssemblyResultsButton.Size = new System.Drawing.Size(164, 23);
            this.RefreshAssemblyResultsButton.TabIndex = 0;
            this.RefreshAssemblyResultsButton.Text = Resources.RefreshAssemblyResultsButton_Text;
            this.RefreshAssemblyResultsButton.UseVisualStyleBackColor = true;
            this.RefreshAssemblyResultsButton.Click += new System.EventHandler(this.RefreshAssemblyResults_Click);
            // 
            // buttonGroupBox
            // 
            this.buttonGroupBox.Controls.Add(this.RefreshAssemblyResultsButton);
            this.buttonGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonGroupBox.Location = new System.Drawing.Point(0, 0);
            this.buttonGroupBox.MinimumSize = new System.Drawing.Size(180, 43);
            this.buttonGroupBox.Name = "buttonGroupBox";
            this.buttonGroupBox.Size = new System.Drawing.Size(789, 43);
            this.buttonGroupBox.TabIndex = 2;
            this.buttonGroupBox.TabStop = false;
            // 
            // AssemblyResultTotalView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewControl);
            this.Controls.Add(this.buttonGroupBox);
            this.Name = "AssemblyResultTotalView";
            this.Size = new System.Drawing.Size(789, 416);
            this.buttonGroupBox.ResumeLayout(false);
            this.buttonGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.Button RefreshAssemblyResultsButton;
        private System.Windows.Forms.GroupBox buttonGroupBox;
    }
}

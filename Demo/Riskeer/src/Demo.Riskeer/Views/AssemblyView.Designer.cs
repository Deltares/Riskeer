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

using Riskeer.Common.Forms.Views;

namespace Demo.Riskeer.Views
{
    partial class AssemblyView
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
            this.riskeerMapControl = new RiskeerMapControl();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.readAssemblyButton = new System.Windows.Forms.Button();
            this.buttonGroupBox = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            //
            // readAssemblyButton
            //
            this.readAssemblyButton.AutoSize = true;
            this.readAssemblyButton.Enabled = true;
            this.readAssemblyButton.Location = new System.Drawing.Point(3, 14);
            this.readAssemblyButton.Name = "readAssemblyButton";
            this.readAssemblyButton.Size = new System.Drawing.Size(164, 23);
            this.readAssemblyButton.TabIndex = 0;
            this.readAssemblyButton.Text = "Importeer GML";
            this.readAssemblyButton.UseVisualStyleBackColor = true;
            this.readAssemblyButton.Click += new System.EventHandler(this.ReadAssembly_Click);
            // 
            // buttonGroupBox
            // 
            this.buttonGroupBox.Controls.Add(this.readAssemblyButton);
            this.buttonGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonGroupBox.Location = new System.Drawing.Point(0, 0);
            this.buttonGroupBox.MinimumSize = new System.Drawing.Size(180, 43);
            this.buttonGroupBox.Name = "buttonGroupBox";
            this.buttonGroupBox.Size = new System.Drawing.Size(789, 43);
            this.buttonGroupBox.TabIndex = 2;
            this.buttonGroupBox.TabStop = false;
            // 
            // riskeerMapControl
            // 
            this.riskeerMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.riskeerMapControl.Location = new System.Drawing.Point(0, 43);
            this.riskeerMapControl.Name = "riskeerMapControl";
            this.riskeerMapControl.Size = new System.Drawing.Size(150, 50);
            this.riskeerMapControl.TabIndex = 0;
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(150, 96);
            this.dataGridViewControl.TabIndex = 0;
            // 
            // AssemblyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AssemblyView";
            this.Controls.Add(this.buttonGroupBox);
            this.Controls.Add(this.riskeerMapControl);
            this.Controls.Add(this.dataGridViewControl);
            this.ResumeLayout(false);

        }

        #endregion

        private RiskeerMapControl riskeerMapControl;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.Button readAssemblyButton;
        private System.Windows.Forms.GroupBox buttonGroupBox;
    }
}

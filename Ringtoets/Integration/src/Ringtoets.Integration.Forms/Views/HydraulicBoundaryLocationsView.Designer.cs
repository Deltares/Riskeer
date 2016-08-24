// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Integration.Forms.Views
{
    partial class HydraulicBoundaryLocationsView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        protected System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HydraulicBoundaryLocationsView));
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.CalculateForSelectedButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.ButtonGroupBox = new System.Windows.Forms.GroupBox();
            this.ButtonGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            resources.ApplyResources(this.dataGridViewControl, "dataGridViewControl");
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // CalculateForSelectedButton
            // 
            resources.ApplyResources(this.CalculateForSelectedButton, "CalculateForSelectedButton");
            this.CalculateForSelectedButton.Name = "CalculateForSelectedButton";
            this.CalculateForSelectedButton.UseVisualStyleBackColor = true;
            this.CalculateForSelectedButton.Click += new System.EventHandler(this.CalculateForSelectedButton_Click);
            // 
            // DeselectAllButton
            // 
            resources.ApplyResources(this.DeselectAllButton, "DeselectAllButton");
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // SelectAllButton
            // 
            resources.ApplyResources(this.SelectAllButton, "SelectAllButton");
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // ButtonGroupBox
            // 
            this.ButtonGroupBox.Controls.Add(this.CalculateForSelectedButton);
            this.ButtonGroupBox.Controls.Add(this.DeselectAllButton);
            this.ButtonGroupBox.Controls.Add(this.SelectAllButton);
            resources.ApplyResources(this.ButtonGroupBox, "ButtonGroupBox");
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.TabStop = false;
            // 
            // HydraulicBoundaryLocationsView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewControl);
            this.Controls.Add(this.ButtonGroupBox);
            this.Name = "HydraulicBoundaryLocationsView";
            this.ButtonGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.Button CalculateForSelectedButton;
        private System.Windows.Forms.Button DeselectAllButton;
        private System.Windows.Forms.Button SelectAllButton;
        protected System.Windows.Forms.GroupBox ButtonGroupBox;
    }
}

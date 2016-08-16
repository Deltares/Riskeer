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
    partial class HydraulicBoundaryLocationDesignWaterLevelsView
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonGroupBox = new System.Windows.Forms.GroupBox();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.CalculateForSelectedButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.ButtonGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(529, 264);
            this.dataGridViewControl.TabIndex = 0;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.dataGridViewControl, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.ButtonGroupBox, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(535, 329);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // ButtonGroupBox
            // 
            this.ButtonGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonGroupBox.Controls.Add(this.CalculateForSelectedButton);
            this.ButtonGroupBox.Controls.Add(this.DeselectAllButton);
            this.ButtonGroupBox.Controls.Add(this.SelectAllButton);
            this.ButtonGroupBox.Location = new System.Drawing.Point(3, 275);
            this.ButtonGroupBox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.Size = new System.Drawing.Size(529, 51);
            this.ButtonGroupBox.TabIndex = 1;
            this.ButtonGroupBox.TabStop = false;
            this.ButtonGroupBox.Text = "Toetspeilen berekenen";
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.Location = new System.Drawing.Point(6, 19);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(98, 23);
            this.SelectAllButton.TabIndex = 0;
            this.SelectAllButton.Text = "Selecteer alles";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.Location = new System.Drawing.Point(110, 19);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(98, 23);
            this.DeselectAllButton.TabIndex = 1;
            this.DeselectAllButton.Text = "Deselecteer alles";
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // CalculateForSelectedButton
            // 
            this.CalculateForSelectedButton.Location = new System.Drawing.Point(214, 19);
            this.CalculateForSelectedButton.Name = "CalculateForSelectedButton";
            this.CalculateForSelectedButton.Size = new System.Drawing.Size(191, 23);
            this.CalculateForSelectedButton.TabIndex = 2;
            this.CalculateForSelectedButton.Text = "Bereken voor geselecteerde locaties";
            this.CalculateForSelectedButton.UseVisualStyleBackColor = true;
            // 
            // HydraulicBoundaryLocationDesignWaterLevelsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "HydraulicBoundaryLocationDesignWaterLevelsView";
            this.Size = new System.Drawing.Size(535, 329);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ButtonGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.GroupBox ButtonGroupBox;
        private System.Windows.Forms.Button CalculateForSelectedButton;
        private System.Windows.Forms.Button DeselectAllButton;
        private System.Windows.Forms.Button SelectAllButton;
    }
}

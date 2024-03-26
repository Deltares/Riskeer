// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.Windows.Forms;

namespace Riskeer.DuneErosion.Forms.Views
{
    partial class DuneLocationCalculationsView
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
            this.components = new System.ComponentModel.Container();
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox = new System.Windows.Forms.CheckBox();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.buttonGroupBox = new System.Windows.Forms.GroupBox();
            this.calculateForSelectedButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.deselectAllButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.selectAllButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.calculateForSelectedButtonErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.calculateForSelectedButtonErrorProvider)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // showHydraulicBoundaryDatabaseFileNameColumnCheckBox
            // 
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox.AutoSize = true;
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox.Location = new System.Drawing.Point(3, 3);
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox.Name = "showHydraulicBoundaryDatabaseFileNameColumnCheckBox";
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox.Size = new System.Drawing.Size(23, 14);
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox.TabIndex = 3;
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox.UseVisualStyleBackColor = true;
            this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox.CheckedChanged += new System.EventHandler(this.ShowHydraulicBoundaryDatabaseFileNameColumnCheckBox_CheckedChanged);
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(3, 23);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(554, 326);
            this.dataGridViewControl.TabIndex = 2;
            // 
            // buttonGroupBox
            // 
            this.buttonGroupBox.Controls.Add(this.calculateForSelectedButton);
            this.buttonGroupBox.Controls.Add(this.deselectAllButton);
            this.buttonGroupBox.Controls.Add(this.selectAllButton);
            this.buttonGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonGroupBox.Location = new System.Drawing.Point(3, 355);
            this.buttonGroupBox.Name = "buttonGroupBox";
            this.buttonGroupBox.Size = new System.Drawing.Size(554, 61);
            this.buttonGroupBox.TabIndex = 3;
            this.buttonGroupBox.TabStop = false;
            // 
            // calculateForSelectedButton
            // 
            this.calculateForSelectedButton.Enabled = false;
            this.calculateForSelectedButtonErrorProvider.SetIconPadding(this.calculateForSelectedButton, 2);
            this.calculateForSelectedButton.Location = new System.Drawing.Point(227, 29);
            this.calculateForSelectedButton.Name = "calculateForSelectedButton";
            this.calculateForSelectedButton.Size = new System.Drawing.Size(207, 23);
            this.calculateForSelectedButton.TabIndex = 2;
            this.calculateForSelectedButton.UseVisualStyleBackColor = true;
            this.calculateForSelectedButton.Click += new System.EventHandler(this.CalculateForSelectedButton_Click);
            // 
            // deselectAllButton
            // 
            this.deselectAllButton.Location = new System.Drawing.Point(110, 29);
            this.deselectAllButton.Name = "deselectAllButton";
            this.deselectAllButton.Size = new System.Drawing.Size(111, 23);
            this.deselectAllButton.TabIndex = 1;
            this.deselectAllButton.UseVisualStyleBackColor = true;
            this.deselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.Location = new System.Drawing.Point(6, 29);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(98, 23);
            this.selectAllButton.TabIndex = 0;
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // calculateForSelectedButtonErrorProvider
            // 
            this.calculateForSelectedButtonErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.calculateForSelectedButtonErrorProvider.ContainerControl = this;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.buttonGroupBox, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.dataGridViewControl, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.showHydraulicBoundaryDatabaseFileNameColumnCheckBox, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(560, 419);
            this.tableLayoutPanel.TabIndex = 5;
            // 
            // DuneLocationCalculationsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(536, 85);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "DuneLocationCalculationsView";
            this.Size = new System.Drawing.Size(560, 419);
            this.buttonGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.calculateForSelectedButtonErrorProvider)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private Core.Common.Controls.Forms.EnhancedButton calculateForSelectedButton;
        private Core.Common.Controls.Forms.EnhancedButton deselectAllButton;
        private Core.Common.Controls.Forms.EnhancedButton selectAllButton;
        protected System.Windows.Forms.GroupBox buttonGroupBox;
        private System.Windows.Forms.ErrorProvider calculateForSelectedButtonErrorProvider;
        private System.Windows.Forms.CheckBox showHydraulicBoundaryDatabaseFileNameColumnCheckBox;
    }
}
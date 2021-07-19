// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Common.Forms.Views
{
    partial class CalculationsView<TCalculation, TCalculationInput, TCalculationRow, TFailureMechanism>
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
            this.generateButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(4, 4);
            this.dataGridViewControl.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(1341, 596);
            this.dataGridViewControl.TabIndex = 0;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.generateButton, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.dataGridViewControl, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1349, 644);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(3, 607);
            this.generateButton.Name = "generateButton";
            this.generateButton.Padding = new System.Windows.Forms.Padding(2);
            this.generateButton.Size = new System.Drawing.Size(170, 34);
            this.generateButton.TabIndex = 1;
            this.generateButton.Text = global::Riskeer.Common.Forms.Properties.Resources.CalculationGroup_Generate_calculations;
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // CalculationsView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "CalculationsView";
            this.Size = new System.Drawing.Size(1349, 644);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private Core.Common.Controls.Forms.EnhancedButton generateButton;

        protected Core.Common.Controls.Forms.EnhancedButton GenerateButton
        {
            get => this.generateButton;
        }

        protected Core.Common.Controls.DataGrid.DataGridViewControl DataGridViewControl
        {
            get => this.dataGridViewControl;
        }
    }
}

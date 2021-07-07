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
            this.dataGridTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.calculationsLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.generateButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.dataGridTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridTableLayoutPanel
            // 
            this.dataGridTableLayoutPanel.ColumnCount = 1;
            this.dataGridTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dataGridTableLayoutPanel.Controls.Add(this.dataGridViewControl, 0, 0);
            this.dataGridTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.dataGridTableLayoutPanel.Name = "dataGridTableLayoutPanel";
            this.dataGridTableLayoutPanel.RowCount = 1;
            this.dataGridTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dataGridTableLayoutPanel.Size = new System.Drawing.Size(1343, 582);
            this.dataGridTableLayoutPanel.TabIndex = 1;
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(4, 4);
            this.dataGridViewControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(1335, 574);
            this.dataGridViewControl.TabIndex = 0;
            // 
            // calculationsLabel
            // 
            this.calculationsLabel.AutoSize = true;
            this.calculationsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.calculationsLabel.Location = new System.Drawing.Point(0, 0);
            this.calculationsLabel.Name = "calculationsLabel";
            this.calculationsLabel.Size = new System.Drawing.Size(230, 16);
            this.calculationsLabel.TabIndex = 0;
            this.calculationsLabel.Text = "Berekeningen voor geselecteerd vak";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.dataGridTableLayoutPanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.generateButton, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 16);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1349, 628);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(3, 591);
            this.generateButton.Name = "generateButton";
            this.generateButton.Padding = new System.Windows.Forms.Padding(2);
            this.generateButton.Size = new System.Drawing.Size(170, 14);
            this.generateButton.TabIndex = 1;
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // CalculationsView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.calculationsLabel);
            this.Name = "CalculationsView";
            this.Size = new System.Drawing.Size(1349, 644);
            this.dataGridTableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label calculationsLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel dataGridTableLayoutPanel;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private Core.Common.Controls.Forms.EnhancedButton generateButton;

        protected System.Windows.Forms.TableLayoutPanel DataGridTableLayoutPanel
        {
            get => this.dataGridTableLayoutPanel;
        }

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

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

namespace Riskeer.Common.Forms.Views
{
    partial class ScenariosView<TCalculationScenario>
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanelListBox = new System.Windows.Forms.TableLayoutPanel();
            this.label = new System.Windows.Forms.Label();
            this.listBox = new System.Windows.Forms.ListBox();
            this.tableLayoutPanelDataGrid = new System.Windows.Forms.TableLayoutPanel();
            this.labelCalculations = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanelListBox.SuspendLayout();
            this.tableLayoutPanelDataGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.MinimumSize = new System.Drawing.Size(150, 150);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tableLayoutPanelListBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanelDataGrid);
            this.splitContainer.Size = new System.Drawing.Size(150, 150);
            this.splitContainer.SplitterDistance = 38;
            this.splitContainer.TabIndex = 0;
            // 
            // tableLayoutPanelListBox
            // 
            this.tableLayoutPanelListBox.ColumnCount = 1;
            this.tableLayoutPanelListBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelListBox.Controls.Add(this.label, 0, 0);
            this.tableLayoutPanelListBox.Controls.Add(this.listBox, 0, 1);
            this.tableLayoutPanelListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelListBox.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelListBox.Name = "tableLayoutPanelListBox";
            this.tableLayoutPanelListBox.RowCount = 2;
            this.tableLayoutPanelListBox.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelListBox.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelListBox.Size = new System.Drawing.Size(38, 150);
            this.tableLayoutPanelListBox.TabIndex = 0;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(3, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(26, 13);
            this.label.TabIndex = 0;
            this.label.Text = Properties.Resources.Section_DisplayName;
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(3, 16);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(44, 131);
            this.listBox.TabIndex = 1;
            // 
            // tableLayoutPanelDataGrid
            // 
            this.tableLayoutPanelDataGrid.ColumnCount = 1;
            this.tableLayoutPanelDataGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDataGrid.Controls.Add(this.labelCalculations, 0, 0);
            this.tableLayoutPanelDataGrid.Controls.Add(this.dataGridViewControl, 0, 1);
            this.tableLayoutPanelDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDataGrid.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelDataGrid.Name = "tableLayoutPanelDataGrid";
            this.tableLayoutPanelDataGrid.RowCount = 2;
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.Size = new System.Drawing.Size(108, 150);
            this.tableLayoutPanelDataGrid.TabIndex = 0;
            // 
            // labelCalculations
            // 
            this.labelCalculations.AutoSize = true;
            this.labelCalculations.Location = new System.Drawing.Point(3, 0);
            this.labelCalculations.Name = "labelCalculations";
            this.labelCalculations.Size = new System.Drawing.Size(183, 13);
            this.labelCalculations.TabIndex = 0;
            this.labelCalculations.Text = Properties.Resources.Calculations_per_FailureMechanismSection_DisplayName;
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(183, 131);
            this.dataGridViewControl.TabIndex = 1;
            // 
            // ScenariosView
            // 
            this.Controls.Add(this.splitContainer);
            this.Name = "ScenariosView";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanelListBox.ResumeLayout(false);
            this.tableLayoutPanelListBox.PerformLayout();
            this.tableLayoutPanelDataGrid.ResumeLayout(false);
            this.tableLayoutPanelDataGrid.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelListBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDataGrid;
        private System.Windows.Forms.Label labelCalculations;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
    }
}

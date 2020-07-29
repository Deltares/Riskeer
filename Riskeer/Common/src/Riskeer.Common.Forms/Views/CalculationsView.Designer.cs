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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listBox = new System.Windows.Forms.ListBox();
            this.sectionsLabel = new System.Windows.Forms.Label();
            this.dataGridTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.calculationsLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.generateButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.dataGridTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(3, 3);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listBox);
            this.splitContainer.Panel1.Controls.Add(this.sectionsLabel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.dataGridTableLayoutPanel);
            this.splitContainer.Panel2.Controls.Add(this.calculationsLabel);
            this.splitContainer.Size = new System.Drawing.Size(1162, 470);
            this.splitContainer.SplitterDistance = 331;
            this.splitContainer.TabIndex = 0;
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 13);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(331, 457);
            this.listBox.TabIndex = 1;
            // 
            // sectionsLabel
            // 
            this.sectionsLabel.AutoSize = true;
            this.sectionsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sectionsLabel.Location = new System.Drawing.Point(0, 0);
            this.sectionsLabel.Name = "sectionsLabel";
            this.sectionsLabel.Size = new System.Drawing.Size(26, 13);
            this.sectionsLabel.TabIndex = 0;
            this.sectionsLabel.Text = "Vak";
            // 
            // dataGridTableLayoutPanel
            // 
            this.dataGridTableLayoutPanel.ColumnCount = 1;
            this.dataGridTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dataGridTableLayoutPanel.Controls.Add(this.dataGridViewControl, 0, 0);
            this.dataGridTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridTableLayoutPanel.Location = new System.Drawing.Point(0, 13);
            this.dataGridTableLayoutPanel.Name = "dataGridTableLayoutPanel";
            this.dataGridTableLayoutPanel.RowCount = 1;
            this.dataGridTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dataGridTableLayoutPanel.Size = new System.Drawing.Size(827, 457);
            this.dataGridTableLayoutPanel.TabIndex = 1;
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(821, 451);
            this.dataGridViewControl.TabIndex = 0;
            // 
            // calculationsLabel
            // 
            this.calculationsLabel.AutoSize = true;
            this.calculationsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.calculationsLabel.Location = new System.Drawing.Point(0, 0);
            this.calculationsLabel.Name = "calculationsLabel";
            this.calculationsLabel.Size = new System.Drawing.Size(182, 13);
            this.calculationsLabel.TabIndex = 0;
            this.calculationsLabel.Text = "Berekeningen voor geselecteerd vak";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.splitContainer, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.generateButton, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(1168, 508);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(3, 479);
            this.generateButton.Name = "generateButton";
            this.generateButton.Padding = new System.Windows.Forms.Padding(2);
            this.generateButton.Size = new System.Drawing.Size(147, 26);
            this.generateButton.TabIndex = 1;
            this.generateButton.Text = global::Riskeer.Common.Forms.Properties.Resources.CalculationGroup_Generate_calculations;
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // CalculationsView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "CalculationsView";
            this.Size = new System.Drawing.Size(1168, 508);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.dataGridTableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Label sectionsLabel;
        private System.Windows.Forms.Label calculationsLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel dataGridTableLayoutPanel;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.Button generateButton;

        protected System.Windows.Forms.Button GenerateButton
        {
            get => this.generateButton;
        }

        protected Core.Common.Controls.DataGrid.DataGridViewControl DataGridViewControl
        {
            get => this.dataGridViewControl;
        }
    }
}

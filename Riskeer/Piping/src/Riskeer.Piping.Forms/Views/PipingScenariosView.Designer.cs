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

namespace Riskeer.Piping.Forms.Views
{
    partial class PipingScenariosView
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
            this.calculationConfigurationTypeTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.selectScenarioConfigurationTypeLabel = new System.Windows.Forms.Label();
            this.selectConfigurationTypeComboBox = new System.Windows.Forms.ComboBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.warningIcon = new System.Windows.Forms.PictureBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanelListBox = new System.Windows.Forms.TableLayoutPanel();
            this.label = new System.Windows.Forms.Label();
            this.listBox = new System.Windows.Forms.ListBox();
            this.tableLayoutPanelDataGrid = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonsPanel = new System.Windows.Forms.Panel();
            this.radioButtonProbabilistic = new System.Windows.Forms.RadioButton();
            this.radioButtonSemiProbabilistic = new System.Windows.Forms.RadioButton();
            this.labelCalculations = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.calculationConfigurationTypeTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.warningIcon)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanelListBox.SuspendLayout();
            this.tableLayoutPanelDataGrid.SuspendLayout();
            this.radioButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // calculationConfigurationTypeTableLayoutPanel
            // 
            this.calculationConfigurationTypeTableLayoutPanel.ColumnCount = 3;
            this.calculationConfigurationTypeTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.calculationConfigurationTypeTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.calculationConfigurationTypeTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.calculationConfigurationTypeTableLayoutPanel.Controls.Add(this.selectScenarioConfigurationTypeLabel, 0, 0);
            this.calculationConfigurationTypeTableLayoutPanel.Controls.Add(this.selectConfigurationTypeComboBox, 1, 0);
            this.calculationConfigurationTypeTableLayoutPanel.Controls.Add(this.warningIcon, 2, 0);
            this.calculationConfigurationTypeTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.calculationConfigurationTypeTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.calculationConfigurationTypeTableLayoutPanel.Name = "calculationConfigurationTypeTableLayoutPanel";
            this.calculationConfigurationTypeTableLayoutPanel.RowCount = 1;
            this.calculationConfigurationTypeTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.calculationConfigurationTypeTableLayoutPanel.Size = new System.Drawing.Size(150, 35);
            this.calculationConfigurationTypeTableLayoutPanel.TabIndex = 1;
            // 
            // selectScenarioConfigurationTypeLabel
            // 
            this.selectScenarioConfigurationTypeLabel.AutoSize = true;
            this.selectScenarioConfigurationTypeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectScenarioConfigurationTypeLabel.Location = new System.Drawing.Point(3, 0);
            this.selectScenarioConfigurationTypeLabel.Name = "selectScenarioConfigurationTypeLabel";
            this.selectScenarioConfigurationTypeLabel.Size = new System.Drawing.Size(60, 35);
            this.selectScenarioConfigurationTypeLabel.TabIndex = 2;
            this.selectScenarioConfigurationTypeLabel.Text = "Type toets:";
            this.selectScenarioConfigurationTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectScenarioConfigurationTypeLabel.Margin = new System.Windows.Forms.Padding(3);
            // 
            // selectConfigurationTypeComboBox
            // 
            this.selectConfigurationTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectConfigurationTypeComboBox.FormattingEnabled = true;
            this.selectConfigurationTypeComboBox.Location = new System.Drawing.Point(75, 7);
            this.selectConfigurationTypeComboBox.Name = "selectConfigurationTypeComboBox";
            this.selectConfigurationTypeComboBox.Size = new System.Drawing.Size(140, 35);
            this.selectConfigurationTypeComboBox.TabIndex = 3;
            this.selectConfigurationTypeComboBox.Margin = new System.Windows.Forms.Padding(7);
            this.selectConfigurationTypeComboBox.SelectedIndexChanged += SelectConfigurationTypeComboBox_OnSelectedIndexChanged;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // warningIcon
            // 
            this.warningIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.warningIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.warningIcon.Location = new System.Drawing.Point(0, 7);
            this.warningIcon.Margin = new System.Windows.Forms.Padding(7);
            this.warningIcon.Name = "warningIcon";
            this.warningIcon.Size = new System.Drawing.Size(16, 16);
            this.warningIcon.TabIndex = 0;
            this.warningIcon.TabStop = false;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 35);
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
            this.label.Text = "Vak";
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
            this.tableLayoutPanelDataGrid.Controls.Add(this.radioButtonsPanel, 0, 0);
            this.tableLayoutPanelDataGrid.Controls.Add(this.labelCalculations, 0, 1);
            this.tableLayoutPanelDataGrid.Controls.Add(this.dataGridViewControl, 0, 2);
            this.tableLayoutPanelDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDataGrid.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelDataGrid.Name = "tableLayoutPanelDataGrid";
            this.tableLayoutPanelDataGrid.RowCount = 3;
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.Size = new System.Drawing.Size(108, 150);
            this.tableLayoutPanelDataGrid.TabIndex = 0;
            // 
            // radioButtonsPanel
            // 
            this.radioButtonsPanel.Controls.Add(this.radioButtonProbabilistic);
            this.radioButtonsPanel.Controls.Add(this.radioButtonSemiProbabilistic);
            this.radioButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonsPanel.Location = new System.Drawing.Point(3, 3);
            this.radioButtonsPanel.Name = "radioButtonsPanel";
            this.radioButtonsPanel.Size = new System.Drawing.Size(200, 30);
            this.radioButtonsPanel.TabIndex = 0;
            // 
            // radioButtonProbabilistic
            // 
            this.radioButtonProbabilistic.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButtonProbabilistic.Location = new System.Drawing.Point(0, 0);
            this.radioButtonProbabilistic.Name = "radioButtonProbabilistic";
            this.radioButtonProbabilistic.AutoSize = true;
            this.radioButtonProbabilistic.TabIndex = 1;
            this.radioButtonProbabilistic.TabStop = true;
            this.radioButtonProbabilistic.Text = global::Riskeer.Piping.Forms.Properties.Resources.ScenarioConfigurationType_Probabilistic_DisplayName;
            this.radioButtonProbabilistic.UseVisualStyleBackColor = true;
            this.radioButtonProbabilistic.Checked = false;
            this.radioButtonProbabilistic.CheckedChanged += RadioButton_OnCheckedChanged;
            // 
            // radioButtonSemiProbabilistic
            // 
            this.radioButtonSemiProbabilistic.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButtonSemiProbabilistic.Location = new System.Drawing.Point(0, 0);
            this.radioButtonSemiProbabilistic.Name = "radioButtonSemiProbabilistic";
            this.radioButtonSemiProbabilistic.AutoSize = true;
            this.radioButtonSemiProbabilistic.TabIndex = 0;
            this.radioButtonSemiProbabilistic.TabStop = true;
            this.radioButtonSemiProbabilistic.Text = global::Riskeer.Piping.Forms.Properties.Resources.ScenarioConfigurationType_SemiProbabilistic_DisplayName;
            this.radioButtonSemiProbabilistic.UseVisualStyleBackColor = true;
            this.radioButtonSemiProbabilistic.Checked = true;
            this.radioButtonSemiProbabilistic.CheckedChanged += RadioButton_OnCheckedChanged;
            // 
            // labelCalculations
            // 
            this.labelCalculations.AutoSize = true;
            this.labelCalculations.Location = new System.Drawing.Point(3, 0);
            this.labelCalculations.Name = "labelCalculations";
            this.labelCalculations.Size = new System.Drawing.Size(182, 13);
            this.labelCalculations.TabIndex = 0;
            this.labelCalculations.Text = global::Riskeer.Piping.Forms.Properties.Resources.Calculations_per_FailureMechanismSection_And_ScenarioConfigurationType_DisplayName;
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
            // PipingScenariosView
            // 
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.calculationConfigurationTypeTableLayoutPanel);
            this.Name = "PipingScenariosView";
            this.calculationConfigurationTypeTableLayoutPanel.ResumeLayout(false);
            this.calculationConfigurationTypeTableLayoutPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.warningIcon)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanelListBox.ResumeLayout(false);
            this.tableLayoutPanelListBox.PerformLayout();
            this.tableLayoutPanelDataGrid.ResumeLayout(false);
            this.tableLayoutPanelDataGrid.PerformLayout();
            this.radioButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel calculationConfigurationTypeTableLayoutPanel;
        private System.Windows.Forms.Label selectScenarioConfigurationTypeLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelListBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDataGrid;
        private System.Windows.Forms.Label labelCalculations;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.ComboBox selectConfigurationTypeComboBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox warningIcon;
        private System.Windows.Forms.Panel radioButtonsPanel;
        private System.Windows.Forms.RadioButton radioButtonProbabilistic;
        private System.Windows.Forms.RadioButton radioButtonSemiProbabilistic;
    }
}

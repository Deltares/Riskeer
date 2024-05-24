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

using System;
using System.Drawing;
using System.Windows.Forms;

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
            this.warningIcon = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanelListBox = new System.Windows.Forms.TableLayoutPanel();
            this.label = new System.Windows.Forms.Label();
            this.listBox = new System.Windows.Forms.ListBox();
            this.tableLayoutPanelDataGrid = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonsPanel = new System.Windows.Forms.Panel();
            this.radioButtonProbabilistic = new System.Windows.Forms.RadioButton();
            this.radioButtonSemiProbabilistic = new System.Windows.Forms.RadioButton();
            this.lengthEffectTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lengthEffectALabel = new System.Windows.Forms.Label();
            this.lengthEffectNRoundedLabel = new System.Windows.Forms.Label();
            this.lengthEffectATextBox = new System.Windows.Forms.TextBox();
            this.lengthEffectNRoundedTextBox = new System.Windows.Forms.TextBox();
            this.labelCalculations = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.labelTotalScenarioContribution = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.lengthEffectErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.calculationConfigurationTypeTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.warningIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanelListBox.SuspendLayout();
            this.tableLayoutPanelDataGrid.SuspendLayout();
            this.radioButtonsPanel.SuspendLayout();
            this.lengthEffectTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.lengthEffectErrorProvider)).BeginInit();
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
            this.calculationConfigurationTypeTableLayoutPanel.Size = new System.Drawing.Size(1348, 35);
            this.calculationConfigurationTypeTableLayoutPanel.TabIndex = 1;
            // 
            // selectScenarioConfigurationTypeLabel
            // 
            this.selectScenarioConfigurationTypeLabel.AutoSize = true;
            this.selectScenarioConfigurationTypeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectScenarioConfigurationTypeLabel.Location = new System.Drawing.Point(3, 5);
            this.selectScenarioConfigurationTypeLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.selectScenarioConfigurationTypeLabel.Name = "selectScenarioConfigurationTypeLabel";
            this.selectScenarioConfigurationTypeLabel.Size = new System.Drawing.Size(57, 27);
            this.selectScenarioConfigurationTypeLabel.TabIndex = 2;
            this.selectScenarioConfigurationTypeLabel.Text = global::Riskeer.Piping.Forms.Properties.Resources.PipingScenariosView_ScenarioConfigurationType_DisplayName;
            this.selectScenarioConfigurationTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // selectConfigurationTypeComboBox
            // 
            this.selectConfigurationTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectConfigurationTypeComboBox.FormattingEnabled = true;
            this.selectConfigurationTypeComboBox.Location = new System.Drawing.Point(70, 7);
            this.selectConfigurationTypeComboBox.Name = "selectConfigurationTypeComboBox";
            this.selectConfigurationTypeComboBox.Size = new System.Drawing.Size(140, 21);
            this.selectConfigurationTypeComboBox.TabIndex = 3;
            this.selectConfigurationTypeComboBox.Margin = new System.Windows.Forms.Padding(7);
            this.selectConfigurationTypeComboBox.SelectedIndexChanged += SelectConfigurationTypeComboBox_OnSelectedIndexChanged;
            // 
            // warningIcon
            // 
            this.warningIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.warningIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.warningIcon.Location = new System.Drawing.Point(224, 7);
            this.warningIcon.Margin = new System.Windows.Forms.Padding(7);
            this.warningIcon.Name = "warningIcon";
            this.warningIcon.Size = new System.Drawing.Size(16, 21);
            this.warningIcon.TabIndex = 0;
            this.warningIcon.TabStop = false;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
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
            this.splitContainer.Size = new System.Drawing.Size(1348, 762);
            this.splitContainer.SplitterDistance = 341;
            this.splitContainer.TabIndex = 0;
            // 
            // tableLayoutPanelListBox
            // 
            this.tableLayoutPanelListBox.AutoSize = true;
            this.tableLayoutPanelListBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelListBox.ColumnCount = 1;
            this.tableLayoutPanelListBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelListBox.Controls.Add(this.label, 0, 0);
            this.tableLayoutPanelListBox.Controls.Add(this.listBox, 0, 1);
            this.tableLayoutPanelListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelListBox.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelListBox.Name = "tableLayoutPanelListBox";
            this.tableLayoutPanelListBox.RowCount = 2;
            this.tableLayoutPanelListBox.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelListBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelListBox.Size = new System.Drawing.Size(341, 762);
            this.tableLayoutPanelListBox.TabIndex = 0;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(3, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(0, 13);
            this.label.TabIndex = 0;
            this.label.Text = global::Riskeer.Common.Forms.Properties.Resources.Section_DisplayName;
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(3, 16);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(335, 743);
            this.listBox.TabIndex = 1;
            // 
            // tableLayoutPanelDataGrid
            // 
            this.tableLayoutPanelDataGrid.ColumnCount = 1;
            this.tableLayoutPanelDataGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDataGrid.Controls.Add(this.radioButtonsPanel, 0, 0);
            this.tableLayoutPanelDataGrid.Controls.Add(this.lengthEffectTableLayoutPanel, 0, 1);
            this.tableLayoutPanelDataGrid.Controls.Add(this.labelCalculations, 0, 2);
            this.tableLayoutPanelDataGrid.Controls.Add(this.dataGridViewControl, 0, 3);
            this.tableLayoutPanelDataGrid.Controls.Add(this.labelTotalScenarioContribution, 0, 4);
            this.tableLayoutPanelDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDataGrid.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelDataGrid.Name = "tableLayoutPanelDataGrid";
            this.tableLayoutPanelDataGrid.RowCount = 5;
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.Size = new System.Drawing.Size(1003, 762);
            this.tableLayoutPanelDataGrid.TabIndex = 0;
            // 
            // radioButtonsPanel
            // 
            this.radioButtonsPanel.Controls.Add(this.radioButtonProbabilistic);
            this.radioButtonsPanel.Controls.Add(this.radioButtonSemiProbabilistic);
            this.radioButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonsPanel.Location = new System.Drawing.Point(3, 19);
            this.radioButtonsPanel.Margin = new System.Windows.Forms.Padding(3, 19, 0, 0);
            this.radioButtonsPanel.Name = "radioButtonsPanel";
            this.radioButtonsPanel.Size = new System.Drawing.Size(1000, 30);
            this.radioButtonsPanel.TabIndex = 0;
            // 
            // radioButtonProbabilistic
            // 
            this.radioButtonProbabilistic.AutoSize = true;
            this.radioButtonProbabilistic.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioButtonProbabilistic.Location = new System.Drawing.Point(117, 0);
            this.radioButtonProbabilistic.Name = "radioButtonProbabilistic";
            this.radioButtonProbabilistic.Size = new System.Drawing.Size(92, 30);
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
            // lengthEffectTableLayoutPanel
            // 
            this.lengthEffectTableLayoutPanel.AutoSize = true;
            this.lengthEffectTableLayoutPanel.ColumnCount = 2;
            this.lengthEffectTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.lengthEffectTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.lengthEffectTableLayoutPanel.Controls.Add(this.lengthEffectALabel, 0, 0);
            this.lengthEffectTableLayoutPanel.Controls.Add(this.lengthEffectNRoundedLabel, 0, 1);
            this.lengthEffectTableLayoutPanel.Controls.Add(this.lengthEffectATextBox, 1, 0);
            this.lengthEffectTableLayoutPanel.Controls.Add(this.lengthEffectNRoundedTextBox, 1, 1);
            this.lengthEffectTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectTableLayoutPanel.Location = new System.Drawing.Point(0, 52);
            this.lengthEffectTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lengthEffectTableLayoutPanel.Name = "lengthEffectTableLayoutPanel";
            this.lengthEffectTableLayoutPanel.RowCount = 2;
            this.lengthEffectTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.lengthEffectTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.lengthEffectTableLayoutPanel.Size = new System.Drawing.Size(1003, 52);
            this.lengthEffectTableLayoutPanel.TabIndex = 0;
            // 
            // lengthEffectALabel
            // 
            this.lengthEffectALabel.AutoSize = true;
            this.lengthEffectALabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectALabel.Location = new System.Drawing.Point(3, 0);
            this.lengthEffectALabel.Name = "lengthEffectALabel";
            this.lengthEffectALabel.Size = new System.Drawing.Size(165, 26);
            this.lengthEffectALabel.TabIndex = 0;
            this.lengthEffectALabel.Text = global::Riskeer.Common.Forms.Properties.Resources.LengthEffect_A_DisplayName;
            this.lengthEffectALabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthEffectNRoundedLabel
            // 
            this.lengthEffectNRoundedLabel.AutoSize = true;
            this.lengthEffectNRoundedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectNRoundedLabel.Location = new System.Drawing.Point(3, 26);
            this.lengthEffectNRoundedLabel.Name = "lengthEffectNRoundedLabel";
            this.lengthEffectNRoundedLabel.Size = new System.Drawing.Size(165, 26);
            this.lengthEffectNRoundedLabel.TabIndex = 1;
            this.lengthEffectNRoundedLabel.Text = global::Riskeer.Common.Forms.Properties.Resources.LengthEffect_RoundedNSection_DisplayName;
            this.lengthEffectNRoundedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthEffectATextBox
            // 
            this.lengthEffectATextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.lengthEffectATextBox.Location = new System.Drawing.Point(174, 3);
            this.lengthEffectATextBox.Name = "lengthEffectATextBox";
            this.lengthEffectATextBox.Size = new System.Drawing.Size(100, 20);
            this.lengthEffectATextBox.TabIndex = 3;
            this.lengthEffectATextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LengthEffectATextBoxKeyDown);
            this.lengthEffectATextBox.Leave += new System.EventHandler(this.LengthEffectATextBoxLeave);
            // 
            // lengthEffectNRoundedTextBox
            // 
            this.lengthEffectNRoundedTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.lengthEffectNRoundedTextBox.Location = new System.Drawing.Point(174, 29);
            this.lengthEffectNRoundedTextBox.Name = "lengthEffectNRoundedTextBox";
            this.lengthEffectNRoundedTextBox.ReadOnly = true;
            this.lengthEffectNRoundedTextBox.Enabled = false;
            this.lengthEffectNRoundedTextBox.Size = new System.Drawing.Size(100, 20);
            this.lengthEffectNRoundedTextBox.TabIndex = 4;
            // 
            // labelCalculations
            // 
            this.labelCalculations.AutoSize = true;
            this.labelCalculations.Location = new System.Drawing.Point(3, 107);
            this.labelCalculations.Name = "labelCalculations";
            this.labelCalculations.Size = new System.Drawing.Size(173, 13);
            this.labelCalculations.TabIndex = 0;
            this.labelCalculations.Text = global::Riskeer.Common.Forms.Properties.Resources.Calculations_per_FailureMechanismSection_DisplayName;
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.AutoScroll = true;
            this.dataGridViewControl.AutoSize = true;
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(3, 123);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(997, 623);
            this.dataGridViewControl.TabIndex = 1;
            // 
            // labelTotalScenarioContribution
            // 
            this.labelTotalScenarioContribution.AutoSize = true;
            this.labelTotalScenarioContribution.Location = new System.Drawing.Point(3, 749);
            this.labelTotalScenarioContribution.Name = "labelTotalScenarioContribution";
            this.labelTotalScenarioContribution.Size = new System.Drawing.Size(0, 13);
            this.labelTotalScenarioContribution.TabIndex = 2;
            this.labelTotalScenarioContribution.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // lengthEffectErrorProvider
            // 
            this.lengthEffectErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.lengthEffectErrorProvider.ContainerControl = this;
            // 
            // PipingScenariosView
            // 
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.calculationConfigurationTypeTableLayoutPanel);
            this.Name = "PipingScenariosView";
            this.Size = new System.Drawing.Size(1348, 797);
            this.calculationConfigurationTypeTableLayoutPanel.ResumeLayout(false);
            this.calculationConfigurationTypeTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.warningIcon)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanelListBox.ResumeLayout(false);
            this.tableLayoutPanelListBox.PerformLayout();
            this.tableLayoutPanelDataGrid.ResumeLayout(false);
            this.tableLayoutPanelDataGrid.PerformLayout();
            this.radioButtonsPanel.ResumeLayout(false);
            this.radioButtonsPanel.PerformLayout();
            this.lengthEffectTableLayoutPanel.ResumeLayout(false);
            this.lengthEffectTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.lengthEffectErrorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TextBox lengthEffectNRoundedTextBox;

        private System.Windows.Forms.ErrorProvider lengthEffectErrorProvider;

        private System.Windows.Forms.Label lengthEffectALabel;
        private System.Windows.Forms.Label lengthEffectNRoundedLabel;
        private System.Windows.Forms.TextBox lengthEffectATextBox;

        private System.Windows.Forms.TableLayoutPanel lengthEffectTableLayoutPanel;

        #endregion

        private System.Windows.Forms.TableLayoutPanel calculationConfigurationTypeTableLayoutPanel;
        private System.Windows.Forms.Label selectScenarioConfigurationTypeLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelListBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDataGrid;
        private System.Windows.Forms.Label labelCalculations;
        private System.Windows.Forms.Label labelTotalScenarioContribution;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.ComboBox selectConfigurationTypeComboBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox warningIcon;
        private System.Windows.Forms.Panel radioButtonsPanel;
        private System.Windows.Forms.RadioButton radioButtonProbabilistic;
        private System.Windows.Forms.RadioButton radioButtonSemiProbabilistic;
        private ErrorProvider errorProvider;
    }
}

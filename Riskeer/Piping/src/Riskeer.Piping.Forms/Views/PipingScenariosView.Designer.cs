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
using Riskeer.Common.Forms.Controls;

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
            this.failureMechanismSectionsGroupBox = new System.Windows.Forms.GroupBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.calculationsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewControlGroupBox = new System.Windows.Forms.GroupBox();
            this.dataGridViewControlTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelTotalScenarioContribution = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.calculationSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.calculationSettingsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lengthEffectSettingsControl = new Riskeer.Common.Forms.Controls.LengthEffectSettingsControl();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.radioButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButtonSemiProbabilistic = new System.Windows.Forms.RadioButton();
            this.radioButtonProbabilistic = new System.Windows.Forms.RadioButton();
            this.calculationConfigurationTypeTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.warningIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.failureMechanismSectionsGroupBox.SuspendLayout();
            this.calculationsTableLayoutPanel.SuspendLayout();
            this.dataGridViewControlGroupBox.SuspendLayout();
            this.dataGridViewControlTableLayoutPanel.SuspendLayout();
            this.calculationSettingsGroupBox.SuspendLayout();
            this.calculationSettingsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).BeginInit();
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
            this.calculationConfigurationTypeTableLayoutPanel.Size = new System.Drawing.Size(1348, 35);
            this.calculationConfigurationTypeTableLayoutPanel.TabIndex = 1;
            // 
            // selectScenarioConfigurationTypeLabel
            // 
            this.selectScenarioConfigurationTypeLabel.AutoSize = true;
            this.selectScenarioConfigurationTypeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectScenarioConfigurationTypeLabel.Location = new System.Drawing.Point(3, 3);
            this.selectScenarioConfigurationTypeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.selectScenarioConfigurationTypeLabel.Name = "selectScenarioConfigurationTypeLabel";
            this.selectScenarioConfigurationTypeLabel.Size = new System.Drawing.Size(57, 29);
            this.selectScenarioConfigurationTypeLabel.TabIndex = 2;
            this.selectScenarioConfigurationTypeLabel.Text = global::Riskeer.Piping.Forms.Properties.Resources.PipingScenariosView_ScenarioConfigurationType_DisplayName;
            this.selectScenarioConfigurationTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // selectConfigurationTypeComboBox
            // 
            this.selectConfigurationTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectConfigurationTypeComboBox.FormattingEnabled = true;
            this.selectConfigurationTypeComboBox.Location = new System.Drawing.Point(70, 7);
            this.selectConfigurationTypeComboBox.Margin = new System.Windows.Forms.Padding(7);
            this.selectConfigurationTypeComboBox.Name = "selectConfigurationTypeComboBox";
            this.selectConfigurationTypeComboBox.Size = new System.Drawing.Size(140, 21);
            this.selectConfigurationTypeComboBox.TabIndex = 3;
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
            this.splitContainer.Panel1.Controls.Add(this.failureMechanismSectionsGroupBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.calculationsTableLayoutPanel);
            this.splitContainer.Size = new System.Drawing.Size(1348, 762);
            this.splitContainer.SplitterDistance = 341;
            this.splitContainer.TabIndex = 0;
            // 
            // failureMechanismSectionsGroupBox
            // 
            this.failureMechanismSectionsGroupBox.Controls.Add(this.listBox);
            this.failureMechanismSectionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismSectionsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.failureMechanismSectionsGroupBox.Name = "failureMechanismSectionsGroupBox";
            this.failureMechanismSectionsGroupBox.Size = new System.Drawing.Size(341, 762);
            this.failureMechanismSectionsGroupBox.TabIndex = 0;
            this.failureMechanismSectionsGroupBox.TabStop = false;
            this.failureMechanismSectionsGroupBox.Text = global::Riskeer.Common.Forms.Properties.Resources.Select_section_DisplayName;
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(3, 16);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(335, 743);
            this.listBox.TabIndex = 0;
            // 
            // calculationsTableLayoutPanel
            // 
            this.calculationsTableLayoutPanel.AutoSize = true;
            this.calculationsTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.calculationsTableLayoutPanel.ColumnCount = 1;
            this.calculationsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.calculationsTableLayoutPanel.Controls.Add(this.dataGridViewControlGroupBox, 0, 1);
            this.calculationsTableLayoutPanel.Controls.Add(this.calculationSettingsGroupBox, 0, 0);
            this.calculationsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calculationsTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.calculationsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.calculationsTableLayoutPanel.Name = "calculationsTableLayoutPanel";
            this.calculationsTableLayoutPanel.RowCount = 2;
            this.calculationsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.calculationsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.calculationsTableLayoutPanel.Size = new System.Drawing.Size(1003, 762);
            this.calculationsTableLayoutPanel.TabIndex = 0;
            // 
            // dataGridViewControlGroupBox
            // 
            this.dataGridViewControlGroupBox.AutoSize = true;
            this.dataGridViewControlGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dataGridViewControlGroupBox.Controls.Add(this.dataGridViewControlTableLayoutPanel);
            this.dataGridViewControlGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControlGroupBox.Location = new System.Drawing.Point(3, 109);
            this.dataGridViewControlGroupBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.dataGridViewControlGroupBox.Name = "dataGridViewControlGroupBox";
            this.dataGridViewControlGroupBox.Size = new System.Drawing.Size(997, 653);
            this.dataGridViewControlGroupBox.TabIndex = 0;
            this.dataGridViewControlGroupBox.TabStop = false;
            this.dataGridViewControlGroupBox.Text = global::Riskeer.Common.Forms.Properties.Resources.Calculations_per_FailureMechanismSection_DisplayName;
            // 
            // dataGridViewControlTableLayoutPanel
            // 
            this.dataGridViewControlTableLayoutPanel.AutoSize = true;
            this.dataGridViewControlTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dataGridViewControlTableLayoutPanel.ColumnCount = 1;
            this.dataGridViewControlTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.dataGridViewControlTableLayoutPanel.Controls.Add(this.labelTotalScenarioContribution, 0, 1);
            this.dataGridViewControlTableLayoutPanel.Controls.Add(this.dataGridViewControl, 0, 0);
            this.dataGridViewControlTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControlTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewControlTableLayoutPanel.Name = "dataGridViewControlTableLayoutPanel";
            this.dataGridViewControlTableLayoutPanel.RowCount = 2;
            this.dataGridViewControlTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dataGridViewControlTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.dataGridViewControlTableLayoutPanel.Size = new System.Drawing.Size(991, 634);
            this.dataGridViewControlTableLayoutPanel.TabIndex = 0;
            // 
            // labelTotalScenarioContribution
            // 
            this.labelTotalScenarioContribution.AutoSize = true;
            this.labelTotalScenarioContribution.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelTotalScenarioContribution.Location = new System.Drawing.Point(3, 621);
            this.labelTotalScenarioContribution.Name = "labelTotalScenarioContribution";
            this.labelTotalScenarioContribution.Size = new System.Drawing.Size(0, 13);
            this.labelTotalScenarioContribution.TabIndex = 0;
            this.labelTotalScenarioContribution.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.AutoScroll = true;
            this.dataGridViewControl.AutoSize = true;
            this.dataGridViewControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(985, 615);
            this.dataGridViewControl.TabIndex = 1;
            // 
            // calculationSettingsGroupBox
            // 
            this.calculationSettingsGroupBox.AutoSize = true;
            this.calculationSettingsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.calculationSettingsGroupBox.Controls.Add(this.calculationSettingsTableLayoutPanel);
            this.calculationSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calculationSettingsGroupBox.Location = new System.Drawing.Point(3, 0);
            this.calculationSettingsGroupBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.calculationSettingsGroupBox.Name = "calculationSettingsGroupBox";
            this.calculationSettingsGroupBox.Size = new System.Drawing.Size(997, 106);
            this.calculationSettingsGroupBox.TabIndex = 1;
            this.calculationSettingsGroupBox.TabStop = false;
            this.calculationSettingsGroupBox.Text = global::Riskeer.Common.Forms.Properties.Resources.Calculation_settings_per_FailureMechanismSection_DisplayName;
            // 
            // calculationSettingsTableLayoutPanel
            // 
            this.calculationSettingsTableLayoutPanel.AutoSize = true;
            this.calculationSettingsTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.calculationSettingsTableLayoutPanel.ColumnCount = 1;
            this.calculationSettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.calculationSettingsTableLayoutPanel.Controls.Add(this.lengthEffectSettingsControl, 0, 1);
            this.calculationSettingsTableLayoutPanel.Controls.Add(this.radioButtonsPanel, 0, 0);
            this.calculationSettingsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calculationSettingsTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.calculationSettingsTableLayoutPanel.Name = "calculationSettingsTableLayoutPanel";
            this.calculationSettingsTableLayoutPanel.RowCount = 2;
            this.calculationSettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.calculationSettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.calculationSettingsTableLayoutPanel.Size = new System.Drawing.Size(991, 87);
            this.calculationSettingsTableLayoutPanel.TabIndex = 0;
            // 
            // lengthEffectSettingsControl
            // 
            this.lengthEffectSettingsControl.AutoSize = true;
            this.lengthEffectSettingsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.lengthEffectSettingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectSettingsControl.Location = new System.Drawing.Point(3, 32);
            this.lengthEffectSettingsControl.Name = "lengthEffectSettingsControl";
            this.lengthEffectSettingsControl.Size = new System.Drawing.Size(985, 52);
            this.lengthEffectSettingsControl.TabIndex = 1;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // radioButtonsPanel
            // 
            this.radioButtonsPanel.AutoSize = true;
            this.radioButtonsPanel.Controls.Add(this.radioButtonSemiProbabilistic);
            this.radioButtonsPanel.Controls.Add(this.radioButtonProbabilistic);
            this.radioButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonsPanel.Location = new System.Drawing.Point(3, 3);
            this.radioButtonsPanel.Name = "radioButtonsPanel";
            this.radioButtonsPanel.Size = new System.Drawing.Size(985, 23);
            this.radioButtonsPanel.TabIndex = 2;
            // 
            // radioButtonSemiProbabilistic
            // 
            this.radioButtonSemiProbabilistic.AutoSize = true;
            this.radioButtonSemiProbabilistic.Location = new System.Drawing.Point(3, 3);
            this.radioButtonSemiProbabilistic.Name = "radioButtonSemiProbabilistic";
            this.radioButtonSemiProbabilistic.Size = new System.Drawing.Size(85, 17);
            this.radioButtonSemiProbabilistic.TabIndex = 0;
            this.radioButtonSemiProbabilistic.TabStop = true;
            this.radioButtonSemiProbabilistic.UseVisualStyleBackColor = true;
            this.radioButtonSemiProbabilistic.Checked = true;
            this.radioButtonSemiProbabilistic.CheckedChanged += RadioButton_OnCheckedChanged;
            this.radioButtonSemiProbabilistic.Text = global::Riskeer.Piping.Forms.Properties.Resources.ScenarioConfigurationType_SemiProbabilistic_DisplayName;
            // 
            // radioButtonProbabilistic
            // 
            this.radioButtonProbabilistic.AutoSize = true;
            this.radioButtonProbabilistic.Location = new System.Drawing.Point(94, 3);
            this.radioButtonProbabilistic.Name = "radioButtonProbabilistic";
            this.radioButtonProbabilistic.Size = new System.Drawing.Size(85, 17);
            this.radioButtonProbabilistic.TabIndex = 1;
            this.radioButtonProbabilistic.TabStop = true;
            this.radioButtonProbabilistic.UseVisualStyleBackColor = true;
            this.radioButtonProbabilistic.Checked = false;
            this.radioButtonProbabilistic.CheckedChanged += RadioButton_OnCheckedChanged;
            this.radioButtonProbabilistic.Text = global::Riskeer.Piping.Forms.Properties.Resources.ScenarioConfigurationType_Probabilistic_DisplayName;
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
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.failureMechanismSectionsGroupBox.ResumeLayout(false);
            this.calculationsTableLayoutPanel.ResumeLayout(false);
            this.calculationsTableLayoutPanel.PerformLayout();
            this.dataGridViewControlGroupBox.ResumeLayout(false);
            this.dataGridViewControlGroupBox.PerformLayout();
            this.dataGridViewControlTableLayoutPanel.ResumeLayout(false);
            this.dataGridViewControlTableLayoutPanel.PerformLayout();
            this.calculationSettingsGroupBox.ResumeLayout(false);
            this.calculationSettingsGroupBox.PerformLayout();
            this.calculationSettingsTableLayoutPanel.ResumeLayout(false);
            this.calculationSettingsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).EndInit();
            this.radioButtonsPanel.ResumeLayout(false);
            this.radioButtonsPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.RadioButton radioButtonProbabilistic;

        private System.Windows.Forms.RadioButton radioButtonSemiProbabilistic;

        private System.Windows.Forms.FlowLayoutPanel radioButtonsPanel;

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;

        private System.Windows.Forms.Label labelTotalScenarioContribution;

        private System.Windows.Forms.TableLayoutPanel dataGridViewControlTableLayoutPanel;

        private Riskeer.Common.Forms.Controls.LengthEffectSettingsControl lengthEffectSettingsControl;

        private System.Windows.Forms.TableLayoutPanel calculationSettingsTableLayoutPanel;

        private System.Windows.Forms.GroupBox dataGridViewControlGroupBox;
        private System.Windows.Forms.GroupBox calculationSettingsGroupBox;

        private System.Windows.Forms.TableLayoutPanel calculationsTableLayoutPanel;

        private System.Windows.Forms.ListBox listBox;

        private System.Windows.Forms.GroupBox failureMechanismSectionsGroupBox;

        #endregion

        private System.Windows.Forms.TableLayoutPanel calculationConfigurationTypeTableLayoutPanel;
        private System.Windows.Forms.Label selectScenarioConfigurationTypeLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ComboBox selectConfigurationTypeComboBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox warningIcon;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}

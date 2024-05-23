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

using System.ComponentModel;
using System.Windows.Forms;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    partial class MacroStabilityInwardsScenariosView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        
        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanelListBox = new System.Windows.Forms.TableLayoutPanel();
            this.label = new System.Windows.Forms.Label();
            this.listBox = new System.Windows.Forms.ListBox();
            this.tableLayoutPanelDataGrid = new System.Windows.Forms.TableLayoutPanel();
            this.labelTotalScenarioContribution = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.labelCalculations = new System.Windows.Forms.Label();
            this.lengthEffectTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lengthEffectALabel = new System.Windows.Forms.Label();
            this.lengthEffectNRoundedLabel = new System.Windows.Forms.Label();
            this.lengthEffectATextBox = new System.Windows.Forms.TextBox();
            this.lengthEffectNRoundedTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanelListBox.SuspendLayout();
            this.tableLayoutPanelDataGrid.SuspendLayout();
            this.lengthEffectTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tableLayoutPanelListBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanelDataGrid);
            this.splitContainer.Size = new System.Drawing.Size(1348, 797);
            this.splitContainer.SplitterDistance = 449;
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
            this.tableLayoutPanelListBox.Size = new System.Drawing.Size(449, 797);
            this.tableLayoutPanelListBox.TabIndex = 0;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label.Location = new System.Drawing.Point(3, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(443, 13);
            this.label.TabIndex = 0;
            this.label.Text = global::Riskeer.Common.Forms.Properties.Resources.Section_DisplayName;
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(3, 16);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(443, 778);
            this.listBox.TabIndex = 1;
            // 
            // tableLayoutPanelDataGrid
            // 
            this.tableLayoutPanelDataGrid.ColumnCount = 1;
            this.tableLayoutPanelDataGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDataGrid.Controls.Add(this.labelTotalScenarioContribution, 0, 3);
            this.tableLayoutPanelDataGrid.Controls.Add(this.dataGridViewControl, 0, 2);
            this.tableLayoutPanelDataGrid.Controls.Add(this.labelCalculations, 0, 1);
            this.tableLayoutPanelDataGrid.Controls.Add(this.lengthEffectTableLayoutPanel, 0, 0);
            this.tableLayoutPanelDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDataGrid.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelDataGrid.Name = "tableLayoutPanelDataGrid";
            this.tableLayoutPanelDataGrid.RowCount = 4;
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDataGrid.Size = new System.Drawing.Size(895, 797);
            this.tableLayoutPanelDataGrid.TabIndex = 0;
            // 
            // labelTotalScenarioContribution
            // 
            this.labelTotalScenarioContribution.AutoSize = true;
            this.labelTotalScenarioContribution.Location = new System.Drawing.Point(3, 784);
            this.labelTotalScenarioContribution.Name = "labelTotalScenarioContribution";
            this.labelTotalScenarioContribution.Size = new System.Drawing.Size(0, 13);
            this.labelTotalScenarioContribution.TabIndex = 0;
            this.labelTotalScenarioContribution.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.AutoSize = true;
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(3, 82);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(889, 699);
            this.dataGridViewControl.TabIndex = 1;
            // 
            // labelCalculations
            // 
            this.labelCalculations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCalculations.Location = new System.Drawing.Point(3, 56);
            this.labelCalculations.Name = "labelCalculations";
            this.labelCalculations.Size = new System.Drawing.Size(889, 23);
            this.labelCalculations.TabIndex = 2;
            this.labelCalculations.Text = global::Riskeer.Common.Forms.Properties.Resources.Calculations_per_FailureMechanismSection_DisplayName;
            this.labelCalculations.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthEffectTableLayoutPanel
            // 
            
            this.lengthEffectTableLayoutPanel.AutoSize = true;
            this.lengthEffectTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowOnly;
            this.lengthEffectTableLayoutPanel.ColumnCount = 2;
            this.lengthEffectTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.lengthEffectTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lengthEffectTableLayoutPanel.Controls.Add(this.lengthEffectALabel, 0, 0);
            this.lengthEffectTableLayoutPanel.Controls.Add(this.lengthEffectNRoundedLabel, 0, 1);
            this.lengthEffectTableLayoutPanel.Controls.Add(this.lengthEffectATextBox, 1, 0);
            this.lengthEffectTableLayoutPanel.Controls.Add(this.lengthEffectNRoundedTextBox, 1, 1);
            this.lengthEffectTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.lengthEffectTableLayoutPanel.Location = new System.Drawing.Point(0, 3);
            this.lengthEffectTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lengthEffectTableLayoutPanel.Name = "lengthEffectTableLayoutPanel";
            this.lengthEffectTableLayoutPanel.RowCount = 2;
            this.lengthEffectTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.lengthEffectTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.lengthEffectTableLayoutPanel.Size = new System.Drawing.Size(889, 50);
            this.lengthEffectTableLayoutPanel.TabIndex = 3;
            // 
            // lengthEffectALabel
            // 
            this.lengthEffectALabel.AutoSize = true;
            this.lengthEffectALabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectALabel.Location = new System.Drawing.Point(3, 0);
            this.lengthEffectALabel.Name = "lengthEffectALabel";
            this.lengthEffectALabel.Size = new System.Drawing.Size(1, 26);
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
            this.lengthEffectNRoundedLabel.Size = new System.Drawing.Size(1, 26);
            this.lengthEffectNRoundedLabel.TabIndex = 1;
            this.lengthEffectNRoundedLabel.Text = global::Riskeer.Common.Forms.Properties.Resources.LengthEffect_RoundedNSection_DisplayName;
            this.lengthEffectNRoundedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthEffectATextBox
            // 
            this.lengthEffectATextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.lengthEffectATextBox.Location = new System.Drawing.Point(9, 3);
            this.lengthEffectATextBox.Name = "lengthEffectATextBox";
            this.lengthEffectATextBox.Size = new System.Drawing.Size(100, 20);
            this.lengthEffectATextBox.TabIndex = 2;
            // 
            // lengthEffectNRoundedTextBox
            // 
            this.lengthEffectNRoundedTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.lengthEffectNRoundedTextBox.Location = new System.Drawing.Point(9, 29);
            this.lengthEffectNRoundedTextBox.Name = "lengthEffectNRoundedTextBox";
            this.lengthEffectNRoundedTextBox.ReadOnly = true;
            this.lengthEffectNRoundedTextBox.Size = new System.Drawing.Size(100, 20);
            this.lengthEffectNRoundedTextBox.TabIndex = 3;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            this.errorProvider.Icon = global::Riskeer.Common.Forms.Properties.Resources.ErrorIcon;
            // 
            // MacroStabilityInwardsScenariosView
            // 
            this.Controls.Add(this.splitContainer);
            this.Name = "MacroStabilityInwardsScenariosView";
            this.Size = new System.Drawing.Size(1348, 797);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanelListBox.ResumeLayout(false);
            this.tableLayoutPanelListBox.PerformLayout();
            this.tableLayoutPanelDataGrid.ResumeLayout(false);
            this.tableLayoutPanelDataGrid.PerformLayout();
            this.lengthEffectTableLayoutPanel.ResumeLayout(false);
            this.lengthEffectTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TextBox lengthEffectATextBox;
        private System.Windows.Forms.TextBox lengthEffectNRoundedTextBox;

        private System.Windows.Forms.Label lengthEffectALabel;
        private System.Windows.Forms.Label lengthEffectNRoundedLabel;

        private System.Windows.Forms.TableLayoutPanel lengthEffectTableLayoutPanel;

        private System.Windows.Forms.ErrorProvider errorProvider;

        private System.Windows.Forms.Label labelCalculations;

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;

        private System.Windows.Forms.Label labelTotalScenarioContribution;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDataGrid;

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.ListBox listBox;

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelListBox;

        #endregion
    }
}
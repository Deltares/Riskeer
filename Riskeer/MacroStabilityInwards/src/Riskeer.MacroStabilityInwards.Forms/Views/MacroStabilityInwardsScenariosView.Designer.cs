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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Riskeer.Common.Forms.Controls;

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
            this.failureMechanismSectionsGroupBox = new System.Windows.Forms.GroupBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.calculationsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.calculationSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.lengthEffectSettingsControl = new Riskeer.Common.Forms.Controls.LengthEffectSettingsControl();
            this.dataGridViewControlGroupBox = new System.Windows.Forms.GroupBox();
            this.dataGridViewControlTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelTotalScenarioContribution = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanelListBox.SuspendLayout();
            this.failureMechanismSectionsGroupBox.SuspendLayout();
            this.calculationsTableLayoutPanel.SuspendLayout();
            this.calculationSettingsGroupBox.SuspendLayout();
            this.dataGridViewControlGroupBox.SuspendLayout();
            this.dataGridViewControlTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).BeginInit();
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
            this.splitContainer.Panel2.Controls.Add(this.calculationsTableLayoutPanel);
            this.splitContainer.Size = new System.Drawing.Size(1348, 797);
            this.splitContainer.SplitterDistance = 341;
            this.splitContainer.TabIndex = 0;
            // 
            // tableLayoutPanelListBox
            // 
            this.tableLayoutPanelListBox.AutoSize = true;
            this.tableLayoutPanelListBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelListBox.ColumnCount = 1;
            this.tableLayoutPanelListBox.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelListBox.Controls.Add(this.failureMechanismSectionsGroupBox, 0, 1);
            this.tableLayoutPanelListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelListBox.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelListBox.Name = "tableLayoutPanelListBox";
            this.tableLayoutPanelListBox.RowCount = 2;
            this.tableLayoutPanelListBox.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelListBox.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelListBox.Size = new System.Drawing.Size(341, 797);
            this.tableLayoutPanelListBox.TabIndex = 0;
            // 
            // failureMechanismSectionsGroupBox
            // 
            this.failureMechanismSectionsGroupBox.AutoSize = true;
            this.failureMechanismSectionsGroupBox.Controls.Add(this.listBox);
            this.failureMechanismSectionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismSectionsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.failureMechanismSectionsGroupBox.Name = "failureMechanismSectionsGroupBox";
            this.failureMechanismSectionsGroupBox.Size = new System.Drawing.Size(335, 791);
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
            this.listBox.Size = new System.Drawing.Size(329, 772);
            this.listBox.TabIndex = 0;
            // 
            // calculationsTableLayoutPanel
            // 
            this.calculationsTableLayoutPanel.ColumnCount = 1;
            this.calculationsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.calculationsTableLayoutPanel.Controls.Add(this.calculationSettingsGroupBox, 0, 0);
            this.calculationsTableLayoutPanel.Controls.Add(this.dataGridViewControlGroupBox, 0, 1);
            this.calculationsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calculationsTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.calculationsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.calculationsTableLayoutPanel.Name = "calculationsTableLayoutPanel";
            this.calculationsTableLayoutPanel.RowCount = 2;
            this.calculationsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.calculationsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.calculationsTableLayoutPanel.Size = new System.Drawing.Size(1003, 797);
            this.calculationsTableLayoutPanel.TabIndex = 0;
            // 
            // calculationSettingsGroupBox
            // 
            this.calculationSettingsGroupBox.AutoSize = true;
            this.calculationSettingsGroupBox.Controls.Add(this.lengthEffectSettingsControl);
            this.calculationSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calculationSettingsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.calculationSettingsGroupBox.Name = "calculationSettingsGroupBox";
            this.calculationSettingsGroupBox.Size = new System.Drawing.Size(997, 71);
            this.calculationSettingsGroupBox.TabIndex = 0;
            this.calculationSettingsGroupBox.TabStop = false;
            this.calculationSettingsGroupBox.Text = global::Riskeer.Common.Forms.Properties.Resources.Calculation_settings_per_FailureMechanismSection_DisplayName;
            // 
            // lengthEffectSettingsControl
            // 
            this.lengthEffectSettingsControl.AutoSize = true;
            this.lengthEffectSettingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectSettingsControl.Location = new System.Drawing.Point(3, 16);
            this.lengthEffectSettingsControl.Name = "lengthEffectSettingsControl";
            this.lengthEffectSettingsControl.Size = new System.Drawing.Size(991, 52);
            this.lengthEffectSettingsControl.TabIndex = 0;
            // 
            // dataGridViewControlGroupBox
            // 
            this.dataGridViewControlGroupBox.AutoSize = true;
            this.dataGridViewControlGroupBox.Controls.Add(this.dataGridViewControlTableLayoutPanel);
            this.dataGridViewControlGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControlGroupBox.Location = new System.Drawing.Point(3, 80);
            this.dataGridViewControlGroupBox.Name = "dataGridViewControlGroupBox";
            this.dataGridViewControlGroupBox.Size = new System.Drawing.Size(997, 714);
            this.dataGridViewControlGroupBox.TabIndex = 1;
            this.dataGridViewControlGroupBox.TabStop = false;
            this.dataGridViewControlGroupBox.Text = global::Riskeer.Common.Forms.Properties.Resources.Calculations_per_FailureMechanismSection_DisplayName;
            // 
            // dataGridViewControlTableLayoutPanel
            // 
            this.dataGridViewControlTableLayoutPanel.AutoSize = true;
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
            this.dataGridViewControlTableLayoutPanel.Size = new System.Drawing.Size(991, 695);
            this.dataGridViewControlTableLayoutPanel.TabIndex = 0;
            // 
            // labelTotalScenarioContribution
            // 
            this.labelTotalScenarioContribution.AutoSize = true;
            this.labelTotalScenarioContribution.Location = new System.Drawing.Point(3, 682);
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
            this.dataGridViewControl.Size = new System.Drawing.Size(985, 676);
            this.dataGridViewControl.TabIndex = 1;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
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
            this.failureMechanismSectionsGroupBox.ResumeLayout(false);
            this.calculationsTableLayoutPanel.ResumeLayout(false);
            this.calculationsTableLayoutPanel.PerformLayout();
            this.calculationSettingsGroupBox.ResumeLayout(false);
            this.calculationSettingsGroupBox.PerformLayout();
            this.dataGridViewControlGroupBox.ResumeLayout(false);
            this.dataGridViewControlGroupBox.PerformLayout();
            this.dataGridViewControlTableLayoutPanel.ResumeLayout(false);
            this.dataGridViewControlTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }


        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;

        private System.Windows.Forms.TableLayoutPanel dataGridViewControlTableLayoutPanel;
        private System.Windows.Forms.Label labelTotalScenarioContribution;

        private Riskeer.Common.Forms.Controls.LengthEffectSettingsControl lengthEffectSettingsControl;

        private System.Windows.Forms.GroupBox calculationSettingsGroupBox;
        private System.Windows.Forms.GroupBox dataGridViewControlGroupBox;

        private System.Windows.Forms.TableLayoutPanel calculationsTableLayoutPanel;

        private System.Windows.Forms.ListBox listBox;

        private System.Windows.Forms.GroupBox failureMechanismSectionsGroupBox;

        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelListBox;

        #endregion
    }
}
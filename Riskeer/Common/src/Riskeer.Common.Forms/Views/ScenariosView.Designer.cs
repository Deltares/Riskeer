// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

namespace Riskeer.Common.Forms.Views
{
    partial class ScenariosView<TCalculationScenario, TCalculationInput, TScenarioRow, TFailureMechanism>
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.failureMechanismSectionsGroupBox = new System.Windows.Forms.GroupBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.dataGridViewControlGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.labelTotalScenarioContribution = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.failureMechanismSectionsGroupBox.SuspendLayout();
            this.dataGridViewControlGroupBox.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
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
            this.splitContainer.Panel1.Controls.Add(this.failureMechanismSectionsGroupBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.dataGridViewControlGroupBox);
            this.splitContainer.Size = new System.Drawing.Size(561, 396);
            this.splitContainer.SplitterDistance = 142;
            this.splitContainer.TabIndex = 0;
            // 
            // failureMechanismSectionsGroupBox
            // 
            this.failureMechanismSectionsGroupBox.AutoSize = true;
            this.failureMechanismSectionsGroupBox.Controls.Add(this.listBox);
            this.failureMechanismSectionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismSectionsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.failureMechanismSectionsGroupBox.Name = "failureMechanismSectionsGroupBox";
            this.failureMechanismSectionsGroupBox.Size = new System.Drawing.Size(142, 396);
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
            this.listBox.Size = new System.Drawing.Size(136, 377);
            this.listBox.TabIndex = 0;
            // 
            // dataGridViewControlGroupBox
            // 
            this.dataGridViewControlGroupBox.AutoSize = true;
            this.dataGridViewControlGroupBox.Controls.Add(this.tableLayoutPanel);
            this.dataGridViewControlGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControlGroupBox.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewControlGroupBox.Name = "dataGridViewControlGroupBox";
            this.dataGridViewControlGroupBox.Size = new System.Drawing.Size(415, 396);
            this.dataGridViewControlGroupBox.TabIndex = 0;
            this.dataGridViewControlGroupBox.TabStop = false;
            this.dataGridViewControlGroupBox.Text = global::Riskeer.Common.Forms.Properties.Resources.Calculations_per_FailureMechanismSection_DisplayName;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.dataGridViewControl, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelTotalScenarioContribution, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(409, 377);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.AutoScroll = true;
            this.dataGridViewControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(403, 358);
            this.dataGridViewControl.TabIndex = 0;
            // 
            // labelTotalScenarioContribution
            // 
            this.labelTotalScenarioContribution.AutoSize = true;
            this.labelTotalScenarioContribution.Location = new System.Drawing.Point(3, 364);
            this.labelTotalScenarioContribution.Name = "labelTotalScenarioContribution";
            this.labelTotalScenarioContribution.Size = new System.Drawing.Size(0, 13);
            this.labelTotalScenarioContribution.TabIndex = 1;
            this.labelTotalScenarioContribution.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            this.errorProvider.Icon = global::Riskeer.Common.Forms.Properties.Resources.ErrorIcon;
            // 
            // ScenariosView
            // 
            this.Controls.Add(this.splitContainer);
            this.Name = "ScenariosView";
            this.Size = new System.Drawing.Size(561, 396);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.failureMechanismSectionsGroupBox.ResumeLayout(false);
            this.dataGridViewControlGroupBox.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label labelTotalScenarioContribution;

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;

        private System.Windows.Forms.GroupBox dataGridViewControlGroupBox;

        private System.Windows.Forms.ListBox listBox;

        private System.Windows.Forms.GroupBox failureMechanismSectionsGroupBox;

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private ErrorProvider errorProvider;
    }
}

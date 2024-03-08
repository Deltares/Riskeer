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

using System.Windows.Forms;
using Riskeer.Integration.Forms.Controls;
using Riskeer.Integration.Forms.Properties;

namespace Riskeer.Integration.Forms.Views
{
    partial class AssemblyResultTotalView
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
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.refreshAssemblyResultsButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.assessmentSectionAssemblyControl = new Riskeer.Integration.Forms.Controls.AssessmentSectionAssemblyResultControl();
            this.refreshButtonPanel = new System.Windows.Forms.Panel();
            this.warningProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.refreshButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.warningProvider)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 108);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(772, 291);
            this.dataGridViewControl.TabIndex = 3;
            // 
            // refreshAssemblyResultsButton
            // 
            this.refreshAssemblyResultsButton.AutoSize = true;
            this.refreshAssemblyResultsButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.refreshAssemblyResultsButton.Enabled = false;
            this.refreshAssemblyResultsButton.Location = new System.Drawing.Point(5, 5);
            this.refreshAssemblyResultsButton.Name = "refreshAssemblyResultsButton";
            this.refreshAssemblyResultsButton.Size = new System.Drawing.Size(164, 25);
            this.refreshAssemblyResultsButton.TabIndex = 0;
            this.refreshAssemblyResultsButton.Text = global::Riskeer.Integration.Forms.Properties.Resources.RefreshAssemblyResultsButton_Text;
            this.refreshAssemblyResultsButton.UseVisualStyleBackColor = true;
            this.refreshAssemblyResultsButton.Click += new System.EventHandler(this.RefreshAssemblyResults_Click);
            // 
            // assessmentSectionAssemblyControl
            // 
            this.assessmentSectionAssemblyControl.AutoSize = true;
            this.assessmentSectionAssemblyControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assessmentSectionAssemblyControl.Location = new System.Drawing.Point(3, 3);
            this.assessmentSectionAssemblyControl.Name = "assessmentSectionAssemblyControl";
            this.assessmentSectionAssemblyControl.Size = new System.Drawing.Size(230, 42);
            this.assessmentSectionAssemblyControl.TabIndex = 3;
            // 
            // refreshButtonPanel
            // 
            this.refreshButtonPanel.Controls.Add(this.refreshAssemblyResultsButton);
            this.refreshButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.refreshButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.refreshButtonPanel.Name = "refreshButtonPanel";
            this.refreshButtonPanel.Padding = new System.Windows.Forms.Padding(5);
            this.refreshButtonPanel.Size = new System.Drawing.Size(772, 35);
            this.refreshButtonPanel.TabIndex = 1;
            // 
            // warningProvider
            // 
            this.warningProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.warningProvider.ContainerControl = this;
            this.warningProvider.Icon = Core.Gui.Properties.Resources.warning;
            this.warningProvider.SetIconPadding(this.refreshAssemblyResultsButton, 4);
            // 
            // label
            // 
            this.label.Dock = System.Windows.Forms.DockStyle.Top;
            this.label.Location = new System.Drawing.Point(0, 83);
            this.label.Margin = new System.Windows.Forms.Padding(3);
            this.label.Name = "label";
            this.label.Padding = new System.Windows.Forms.Padding(5);
            this.label.Size = new System.Drawing.Size(772, 25);
            this.label.TabIndex = 4;
            this.label.Text = global::Riskeer.Integration.Forms.Properties.Resources.AssemblyResultTotalView_Results_per_failureMechanism;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionAssemblyControl, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.checkBox, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 35);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(772, 48);
            this.tableLayoutPanel.TabIndex = 5;
            // 
            // checkBox
            // 
            this.checkBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.checkBox.Location = new System.Drawing.Point(239, 21);
            this.checkBox.Name = "checkBox";
            this.checkBox.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.checkBox.Size = new System.Drawing.Size(530, 24);
            this.checkBox.TabIndex = 4;
            this.checkBox.Text = global::Riskeer.Integration.Forms.Properties.Resources.AssemblyResultTotalView_GrassCoverErosionInwards_and_HeightStructures_correlated;
            this.checkBox.UseVisualStyleBackColor = true;
            this.checkBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // AssemblyResultTotalView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScrollMinSize = new System.Drawing.Size(350, 250);
            this.Controls.Add(this.dataGridViewControl);
            this.Controls.Add(this.label);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.refreshButtonPanel);
            this.Name = "AssemblyResultTotalView";
            this.Size = new System.Drawing.Size(772, 399);
            this.refreshButtonPanel.ResumeLayout(false);
            this.refreshButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.warningProvider)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.CheckBox checkBox;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;

        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private Core.Common.Controls.Forms.EnhancedButton refreshAssemblyResultsButton;
        private Riskeer.Integration.Forms.Controls.AssessmentSectionAssemblyResultControl assessmentSectionAssemblyControl;
        private System.Windows.Forms.Panel refreshButtonPanel;
        private System.Windows.Forms.ErrorProvider warningProvider;
        private System.Windows.Forms.Label label;
    }
}

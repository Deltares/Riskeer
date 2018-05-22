﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Drawing;

namespace Ringtoets.Integration.Forms.Views
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
            this.RefreshAssemblyResultsButton = new System.Windows.Forms.Button();
            this.assemblyResultGroupBox = new System.Windows.Forms.GroupBox();
            this.assemblyResultTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.totalResultLabel = new System.Windows.Forms.Label();
            this.failureMechanismsWithProbabilityLabel = new System.Windows.Forms.Label();
            this.failureMechanismsWithoutProbabilityLabel = new System.Windows.Forms.Label();
            this.totalAssemblyCategoryGroupControl = new Ringtoets.Integration.Forms.Controls.AssessmentSectionAssemblyCategoryGroupControl();
            this.failureMechanismsWithProbabilityAssemblyControl = new Ringtoets.Integration.Forms.Controls.AssessmentSectionAssemblyControl();
            this.failureMechanismsWithoutProbabilityAssemblyControl = new Ringtoets.Integration.Forms.Controls.AssessmentSectionAssemblyCategoryGroupControl();
            this.refreshButtonPanel = new System.Windows.Forms.Panel();
            this.warningProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.assemblyResultGroupBox.SuspendLayout();
            this.assemblyResultTableLayoutPanel.SuspendLayout();
            this.refreshButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 145);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(789, 271);
            this.dataGridViewControl.TabIndex = 3;
            // 
            // RefreshAssemblyResultsButton
            // 
            this.RefreshAssemblyResultsButton.AutoSize = true;
            this.RefreshAssemblyResultsButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.RefreshAssemblyResultsButton.Enabled = false;
            this.RefreshAssemblyResultsButton.Location = new System.Drawing.Point(5, 5);
            this.RefreshAssemblyResultsButton.Name = "RefreshAssemblyResultsButton";
            this.RefreshAssemblyResultsButton.Size = new System.Drawing.Size(164, 25);
            this.RefreshAssemblyResultsButton.TabIndex = 0;
            this.RefreshAssemblyResultsButton.Text = global::Ringtoets.Integration.Forms.Properties.Resources.RefreshAssemblyResultsButton_Text;
            this.RefreshAssemblyResultsButton.UseVisualStyleBackColor = true;
            this.RefreshAssemblyResultsButton.Click += new System.EventHandler(this.RefreshAssemblyResults_Click);
            // 
            // assemblyResultGroupBox
            // 
            this.assemblyResultGroupBox.Controls.Add(this.assemblyResultTableLayoutPanel);
            this.assemblyResultGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.assemblyResultGroupBox.Location = new System.Drawing.Point(0, 35);
            this.assemblyResultGroupBox.Margin = new System.Windows.Forms.Padding(5);
            this.assemblyResultGroupBox.Name = "assemblyResultGroupBox";
            this.assemblyResultGroupBox.Size = new System.Drawing.Size(789, 110);
            this.assemblyResultGroupBox.TabIndex = 1;
            this.assemblyResultGroupBox.TabStop = false;
            this.assemblyResultGroupBox.Text = global::Ringtoets.Integration.Forms.Properties.Resources.AssemblyResultTotalView_AssemblyResultGroupBox_Text;
            // 
            // assemblyResultTableLayoutPanel
            // 
            this.assemblyResultTableLayoutPanel.AutoSize = true;
            this.assemblyResultTableLayoutPanel.ColumnCount = 2;
            this.assemblyResultTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.assemblyResultTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.assemblyResultTableLayoutPanel.Controls.Add(this.totalResultLabel, 0, 0);
            this.assemblyResultTableLayoutPanel.Controls.Add(this.failureMechanismsWithProbabilityLabel, 0, 1);
            this.assemblyResultTableLayoutPanel.Controls.Add(this.failureMechanismsWithoutProbabilityLabel, 0, 2);
            this.assemblyResultTableLayoutPanel.Controls.Add(this.totalAssemblyCategoryGroupControl, 1, 0);
            this.assemblyResultTableLayoutPanel.Controls.Add(this.failureMechanismsWithProbabilityAssemblyControl, 1, 1);
            this.assemblyResultTableLayoutPanel.Controls.Add(this.failureMechanismsWithoutProbabilityAssemblyControl, 1, 2);
            this.assemblyResultTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assemblyResultTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.assemblyResultTableLayoutPanel.Name = "assemblyResultTableLayoutPanel";
            this.assemblyResultTableLayoutPanel.RowCount = 3;
            this.assemblyResultTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.assemblyResultTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.assemblyResultTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.assemblyResultTableLayoutPanel.Size = new System.Drawing.Size(783, 91);
            this.assemblyResultTableLayoutPanel.TabIndex = 0;
            // 
            // totalResultLabel
            // 
            this.totalResultLabel.AutoSize = true;
            this.totalResultLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalResultLabel.Location = new System.Drawing.Point(3, 0);
            this.totalResultLabel.Name = "totalResultLabel";
            this.totalResultLabel.Size = new System.Drawing.Size(81, 30);
            this.totalResultLabel.TabIndex = 0;
            this.totalResultLabel.Text = global::Ringtoets.Integration.Forms.Properties.Resources.AssemblyResultTotalView_TotalResultLabel_Text;
            this.totalResultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // failureMechanismsWithProbabilityLabel
            // 
            this.failureMechanismsWithProbabilityLabel.AutoSize = true;
            this.failureMechanismsWithProbabilityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismsWithProbabilityLabel.Location = new System.Drawing.Point(3, 30);
            this.failureMechanismsWithProbabilityLabel.Name = "failureMechanismsWithProbabilityLabel";
            this.failureMechanismsWithProbabilityLabel.Size = new System.Drawing.Size(81, 30);
            this.failureMechanismsWithProbabilityLabel.TabIndex = 1;
            this.failureMechanismsWithProbabilityLabel.Text = global::Ringtoets.Integration.Forms.Properties.Resources.AssemblyResultTotalView_FailureMechanismsWithProbabilityResultLabel_Text;
            this.failureMechanismsWithProbabilityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // failureMechanismsWithoutProbabilityLabel
            // 
            this.failureMechanismsWithoutProbabilityLabel.AutoSize = true;
            this.failureMechanismsWithoutProbabilityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismsWithoutProbabilityLabel.Location = new System.Drawing.Point(3, 60);
            this.failureMechanismsWithoutProbabilityLabel.Name = "failureMechanismsWithoutProbabilityLabel";
            this.failureMechanismsWithoutProbabilityLabel.Size = new System.Drawing.Size(81, 31);
            this.failureMechanismsWithoutProbabilityLabel.TabIndex = 2;
            this.failureMechanismsWithoutProbabilityLabel.Text = global::Ringtoets.Integration.Forms.Properties.Resources.AssemblyResultTotalView_FailureMechanismsWithoutProbabilityResultLabel_Text;
            this.failureMechanismsWithoutProbabilityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // totalAssemblyCategoryGroupControl
            // 
            this.totalAssemblyCategoryGroupControl.AutoSize = true;
            this.totalAssemblyCategoryGroupControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalAssemblyCategoryGroupControl.Location = new System.Drawing.Point(90, 3);
            this.totalAssemblyCategoryGroupControl.Name = "totalAssemblyCategoryGroupControl";
            this.totalAssemblyCategoryGroupControl.Size = new System.Drawing.Size(690, 24);
            this.totalAssemblyCategoryGroupControl.TabIndex = 3;
            // 
            // failureMechanismsWithProbabilityAssemblyControl
            // 
            this.failureMechanismsWithProbabilityAssemblyControl.AutoSize = true;
            this.failureMechanismsWithProbabilityAssemblyControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.failureMechanismsWithProbabilityAssemblyControl.Location = new System.Drawing.Point(90, 33);
            this.failureMechanismsWithProbabilityAssemblyControl.Name = "failureMechanismsWithProbabilityAssemblyControl";
            this.failureMechanismsWithProbabilityAssemblyControl.Size = new System.Drawing.Size(112, 24);
            this.failureMechanismsWithProbabilityAssemblyControl.TabIndex = 4;
            // 
            // failureMechanismsWithoutProbabilityAssemblyControl
            // 
            this.failureMechanismsWithoutProbabilityAssemblyControl.AutoSize = true;
            this.failureMechanismsWithoutProbabilityAssemblyControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.failureMechanismsWithoutProbabilityAssemblyControl.Location = new System.Drawing.Point(90, 63);
            this.failureMechanismsWithoutProbabilityAssemblyControl.Name = "failureMechanismsWithoutProbabilityAssemblyControl";
            this.failureMechanismsWithoutProbabilityAssemblyControl.Size = new System.Drawing.Size(56, 25);
            this.failureMechanismsWithoutProbabilityAssemblyControl.TabIndex = 5;
            // 
            // refreshButtonPanel
            // 
            this.refreshButtonPanel.Controls.Add(this.RefreshAssemblyResultsButton);
            this.refreshButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.refreshButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.refreshButtonPanel.Name = "refreshButtonPanel";
            this.refreshButtonPanel.Padding = new System.Windows.Forms.Padding(5);
            this.refreshButtonPanel.Size = new System.Drawing.Size(789, 35);
            this.refreshButtonPanel.TabIndex = 1;
            // 
            // warningProvider
            // 
            this.warningProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.warningProvider.ContainerControl = this;
            this.warningProvider.Icon = global::Ringtoets.Common.Forms.Properties.Resources.warning;
            // 
            // AssemblyResultTotalView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScrollMinSize = new System.Drawing.Size(350, 250);
            this.Controls.Add(this.dataGridViewControl);
            this.Controls.Add(this.assemblyResultGroupBox);
            this.Controls.Add(this.refreshButtonPanel);
            this.Name = "AssemblyResultTotalView";
            this.Size = new System.Drawing.Size(789, 416);
            this.assemblyResultGroupBox.ResumeLayout(false);
            this.assemblyResultGroupBox.PerformLayout();
            this.assemblyResultTableLayoutPanel.ResumeLayout(false);
            this.assemblyResultTableLayoutPanel.PerformLayout();
            this.refreshButtonPanel.ResumeLayout(false);
            this.refreshButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.Button RefreshAssemblyResultsButton;
        private System.Windows.Forms.GroupBox assemblyResultGroupBox;
        private System.Windows.Forms.TableLayoutPanel assemblyResultTableLayoutPanel;
        private System.Windows.Forms.Label totalResultLabel;
        private System.Windows.Forms.Label failureMechanismsWithProbabilityLabel;
        private System.Windows.Forms.Label failureMechanismsWithoutProbabilityLabel;
        private Controls.AssessmentSectionAssemblyCategoryGroupControl totalAssemblyCategoryGroupControl;
        private Controls.AssessmentSectionAssemblyControl failureMechanismsWithProbabilityAssemblyControl;
        private Controls.AssessmentSectionAssemblyCategoryGroupControl failureMechanismsWithoutProbabilityAssemblyControl;
        private System.Windows.Forms.Panel refreshButtonPanel;
        private System.Windows.Forms.ErrorProvider warningProvider;
    }
}

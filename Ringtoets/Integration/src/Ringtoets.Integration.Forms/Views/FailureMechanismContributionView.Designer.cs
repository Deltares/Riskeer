// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Windows.Forms;

namespace Ringtoets.Integration.Forms.Views
{
    partial class FailureMechanismContributionView
    {
        private Label normLabel;
        private NumericUpDown normInput;
        private Label perYearLabel;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailureMechanismContributionView));
            this.normLabel = new System.Windows.Forms.Label();
            this.normInput = new System.Windows.Forms.NumericUpDown();
            this.perYearLabel = new System.Windows.Forms.Label();
            this.assessmentSectionConfigurationLabel = new System.Windows.Forms.Label();
            this.assessmentSectionCompositionComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.probabilityDistributionGrid = new Core.Common.Controls.DataGrid.DataGridViewControl();
            ((System.ComponentModel.ISupportInitialize)(this.normInput)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // normLabel
            // 
            resources.ApplyResources(this.normLabel, "normLabel");
            this.normLabel.Name = "normLabel";
            // 
            // normInput
            // 
            resources.ApplyResources(this.normInput, "normInput");
            this.normInput.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.normInput.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.normInput.Name = "normInput";
            this.normInput.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.normInput.Validating += new System.ComponentModel.CancelEventHandler(this.NormNumericUpDown_Validating);
            this.normInput.Validated += new System.EventHandler(this.NormNumericUpDown_Validated);
            // 
            // perYearLabel
            // 
            resources.ApplyResources(this.perYearLabel, "perYearLabel");
            this.perYearLabel.Name = "perYearLabel";
            // 
            // assessmentSectionConfigurationLabel
            // 
            resources.ApplyResources(this.assessmentSectionConfigurationLabel, "assessmentSectionConfigurationLabel");
            this.assessmentSectionConfigurationLabel.Name = "assessmentSectionConfigurationLabel";
            // 
            // assessmentSectionCompositionComboBox
            // 
            resources.ApplyResources(this.assessmentSectionCompositionComboBox, "assessmentSectionCompositionComboBox");
            this.assessmentSectionCompositionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.assessmentSectionCompositionComboBox.FormattingEnabled = true;
            this.assessmentSectionCompositionComboBox.Name = "assessmentSectionCompositionComboBox";
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionConfigurationLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionCompositionComboBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.normLabel, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.normInput, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.perYearLabel, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.probabilityDistributionGrid, 0, 1);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // probabilityDistributionGrid
            // 
            this.tableLayoutPanel.SetColumnSpan(this.probabilityDistributionGrid, 5);
            resources.ApplyResources(this.probabilityDistributionGrid, "probabilityDistributionGrid");
            this.probabilityDistributionGrid.MultiSelect = true;
            this.probabilityDistributionGrid.Name = "probabilityDistributionGrid";
            this.probabilityDistributionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // FailureMechanismContributionView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "FailureMechanismContributionView";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.normInput)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Label assessmentSectionConfigurationLabel;
        private ComboBox assessmentSectionCompositionComboBox;
        private TableLayoutPanel tableLayoutPanel;
        private Core.Common.Controls.DataGrid.DataGridViewControl probabilityDistributionGrid;

    }
}

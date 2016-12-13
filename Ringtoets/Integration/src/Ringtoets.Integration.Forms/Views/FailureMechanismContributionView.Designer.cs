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
            this.probabilityDistributionGrid = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.assessmentSectionConfigurationLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.returnPeriodLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // probabilityDistributionGrid
            // 
            this.tableLayoutPanel.SetColumnSpan(this.probabilityDistributionGrid, 2);
            resources.ApplyResources(this.probabilityDistributionGrid, "probabilityDistributionGrid");
            this.probabilityDistributionGrid.MultiSelect = true;
            this.probabilityDistributionGrid.Name = "probabilityDistributionGrid";
            this.probabilityDistributionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // assessmentSectionConfigurationLabel
            // 
            resources.ApplyResources(this.assessmentSectionConfigurationLabel, "assessmentSectionConfigurationLabel");
            this.assessmentSectionConfigurationLabel.Name = "assessmentSectionConfigurationLabel";
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionConfigurationLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.returnPeriodLabel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.probabilityDistributionGrid, 0, 1);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // returnPeriodLabel
            // 
            resources.ApplyResources(this.returnPeriodLabel, "returnPeriodLabel");
            this.returnPeriodLabel.Name = "returnPeriodLabel";
            // 
            // FailureMechanismContributionView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "FailureMechanismContributionView";
            resources.ApplyResources(this, "$this");
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl probabilityDistributionGrid;
        private TableLayoutPanel tableLayoutPanel;
        private Label assessmentSectionConfigurationLabel;
        private Label returnPeriodLabel;


    }
}

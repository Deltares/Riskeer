// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Riskeer.Integration.Forms.Views
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
            this.groupBoxAssessmentSectionDetails = new System.Windows.Forms.GroupBox();
            this.returnPeriodLabel = new System.Windows.Forms.Label();
            this.assessmentSectionCompositionLabel = new System.Windows.Forms.Label();
            this.probabilityDistributionGrid = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.groupBoxAssessmentSectionDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxAssessmentSectionDetails
            // 
            this.groupBoxAssessmentSectionDetails.Controls.Add(this.returnPeriodLabel);
            this.groupBoxAssessmentSectionDetails.Controls.Add(this.assessmentSectionCompositionLabel);
            resources.ApplyResources(this.groupBoxAssessmentSectionDetails, "groupBoxAssessmentSectionDetails");
            this.groupBoxAssessmentSectionDetails.Name = "groupBoxAssessmentSectionDetails";
            this.groupBoxAssessmentSectionDetails.TabStop = false;
            // 
            // returnPeriodLabel
            // 
            resources.ApplyResources(this.returnPeriodLabel, "returnPeriodLabel");
            this.returnPeriodLabel.Name = "returnPeriodLabel";
            // 
            // assessmentSectionCompositionLabel
            // 
            resources.ApplyResources(this.assessmentSectionCompositionLabel, "assessmentSectionCompositionLabel");
            this.assessmentSectionCompositionLabel.Name = "assessmentSectionCompositionLabel";
            // 
            // probabilityDistributionGrid
            // 
            resources.ApplyResources(this.probabilityDistributionGrid, "probabilityDistributionGrid");
            this.probabilityDistributionGrid.MultiSelect = true;
            this.probabilityDistributionGrid.Name = "probabilityDistributionGrid";
            this.probabilityDistributionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // FailureMechanismContributionView
            // 
            this.Controls.Add(this.probabilityDistributionGrid);
            this.Controls.Add(this.groupBoxAssessmentSectionDetails);
            this.Name = "FailureMechanismContributionView";
            resources.ApplyResources(this, "$this");
            this.groupBoxAssessmentSectionDetails.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBoxAssessmentSectionDetails;
        private Label returnPeriodLabel;
        private Label assessmentSectionCompositionLabel;
        private Core.Common.Controls.DataGrid.DataGridViewControl probabilityDistributionGrid;


    }
}

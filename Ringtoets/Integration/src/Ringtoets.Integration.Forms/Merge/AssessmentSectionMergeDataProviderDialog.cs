// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Forms.Merge
{
    /// <summary>
    /// A dialog for providing the data to merge.
    /// </summary>
    public partial class AssessmentSectionMergeDataProviderDialog : DialogBase, IAssessmentSectionMergeDataProvider
    {
        private FailureMechanismMergeDataRow[] failureMechanismMergeDataRows;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMergeDataProviderDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionMergeDataProviderDialog(IWin32Window dialogParent)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 720, 590)
        {
            InitializeComponent();
            InitializeComboBox();
            InitializeTooltip();
            InitializeDataGridView();
        }

        public AssessmentSectionMergeData GetMergeData(IEnumerable<AssessmentSection> assessmentSections)
        {
            if (assessmentSections == null)
            {
                throw new ArgumentNullException(nameof(assessmentSections));
            }

            if (!assessmentSections.Any())
            {
                throw new ArgumentException($@"{nameof(assessmentSections)} must at least have one element.", nameof(assessmentSections));
            }

            assessmentSectionComboBox.DataSource = assessmentSections.ToArray();

            return ShowDialog() == DialogResult.OK
                       ? new AssessmentSectionMergeData((AssessmentSection) assessmentSectionComboBox.SelectedItem,
                                                        failureMechanismMergeDataRows.Where(row => row.IsSelected)
                                                                                     .Select(row => row.FailureMechanism)
                                                                                     .ToArray())
                       : null;
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }

        private void InitializeComboBox()
        {
            assessmentSectionComboBox.DisplayMember = nameof(AssessmentSection.Name);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.IsSelected),
                                                  Resources.FailureMechanismMergeDataRow_IsSelected_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismMergeDataRow.Name),
                                                 Resources.FailureMechanism_Name_DisplayName,
                                                 true);
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.IsRelevant),
                                                  Resources.FailureMechanismMergeDataRow_IsRelevant_DisplayName,
                                                  true);
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.HasSections),
                                                  Resources.FailureMechanismMergeDataRow_HasSections_DisplayName,
                                                  true);
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismMergeDataRow.NumberOfCalculations),
                                                 Resources.FailureMechanismMergeDataRow_NumberOfCalculations_DisplayName,
                                                 true);
        }

        private void InitializeTooltip()
        {
            infoIcon.BackgroundImage = CoreCommonGuiResources.information;
            toolTip.SetToolTip(infoIcon, Resources.AssessmentSectionMergeDataProviderDialog_InfoToolTip);
        }

        #region Event Handling

        private void AssessmentSectionComboBox_OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (assessmentSectionComboBox.SelectedIndex == -1)
            {
                return;
            }

            SetDataGridViewData((AssessmentSection) assessmentSectionComboBox.SelectedItem);
        }

        #endregion

        #region Data Setters

        private void SetDataGridViewData(AssessmentSection assessmentSection)
        {
            failureMechanismMergeDataRows = new[]
            {
                new FailureMechanismMergeDataRow(assessmentSection.Piping),
                new FailureMechanismMergeDataRow(assessmentSection.GrassCoverErosionInwards),
                new FailureMechanismMergeDataRow(assessmentSection.MacroStabilityInwards),
                new FailureMechanismMergeDataRow(assessmentSection.MacroStabilityOutwards),
                new FailureMechanismMergeDataRow(assessmentSection.Microstability),
                new FailureMechanismMergeDataRow(assessmentSection.StabilityStoneCover),
                new FailureMechanismMergeDataRow(assessmentSection.WaveImpactAsphaltCover),
                new FailureMechanismMergeDataRow(assessmentSection.WaterPressureAsphaltCover),
                new FailureMechanismMergeDataRow(assessmentSection.GrassCoverErosionOutwards),
                new FailureMechanismMergeDataRow(assessmentSection.GrassCoverSlipOffOutwards),
                new FailureMechanismMergeDataRow(assessmentSection.GrassCoverSlipOffInwards),
                new FailureMechanismMergeDataRow(assessmentSection.HeightStructures),
                new FailureMechanismMergeDataRow(assessmentSection.ClosingStructures),
                new FailureMechanismMergeDataRow(assessmentSection.PipingStructure),
                new FailureMechanismMergeDataRow(assessmentSection.StabilityPointStructures),
                new FailureMechanismMergeDataRow(assessmentSection.StrengthStabilityLengthwiseConstruction),
                new FailureMechanismMergeDataRow(assessmentSection.DuneErosion),
                new FailureMechanismMergeDataRow(assessmentSection.TechnicalInnovation)
            };

            dataGridViewControl.SetDataSource(failureMechanismMergeDataRows);
        }

        #endregion
    }
}
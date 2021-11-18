// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Forms.Properties;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;
using CoreGuiResources = Core.Gui.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Merge
{
    /// <summary>
    /// A dialog for providing the data to merge.
    /// </summary>
    public partial class AssessmentSectionMergeDataProviderDialog : DialogBase, IAssessmentSectionMergeDataProvider
    {
        private FailurePathMergeDataRow[] failurePathMergeDataRows;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMergeDataProviderDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionMergeDataProviderDialog(IWin32Window dialogParent)
            : base(dialogParent, RiskeerCommonFormsResources.SelectionDialogIcon, 720, 590)
        {
            InitializeComponent();
            InitializeTooltip();
            InitializeDataGridView();
        }

        public AssessmentSectionMergeData GetMergeData(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            SetDataGridViewData(assessmentSection);

            if (ShowDialog() == DialogResult.OK)
            {
                var constructionProperties = new AssessmentSectionMergeData.ConstructionProperties
                {
                    MergePiping = FailureMechanismIsSelectedToMerge<PipingFailureMechanism>(),
                    MergeGrassCoverErosionInwards = FailureMechanismIsSelectedToMerge<GrassCoverErosionInwardsFailureMechanism>(),
                    MergeMacroStabilityInwards = FailureMechanismIsSelectedToMerge<MacroStabilityInwardsFailureMechanism>(),
                    MergeMacroStabilityOutwards = FailureMechanismIsSelectedToMerge<MacroStabilityOutwardsFailureMechanism>(),
                    MergeMicrostability = FailureMechanismIsSelectedToMerge<MicrostabilityFailureMechanism>(),
                    MergeStabilityStoneCover = FailureMechanismIsSelectedToMerge<StabilityStoneCoverFailureMechanism>(),
                    MergeWaveImpactAsphaltCover = FailureMechanismIsSelectedToMerge<WaveImpactAsphaltCoverFailureMechanism>(),
                    MergeWaterPressureAsphaltCover = FailureMechanismIsSelectedToMerge<WaterPressureAsphaltCoverFailureMechanism>(),
                    MergeGrassCoverErosionOutwards = FailureMechanismIsSelectedToMerge<GrassCoverErosionOutwardsFailureMechanism>(),
                    MergeGrassCoverSlipOffOutwards = FailureMechanismIsSelectedToMerge<GrassCoverSlipOffOutwardsFailureMechanism>(),
                    MergeGrassCoverSlipOffInwards = FailureMechanismIsSelectedToMerge<GrassCoverSlipOffInwardsFailureMechanism>(),
                    MergeHeightStructures = FailureMechanismIsSelectedToMerge<HeightStructuresFailureMechanism>(),
                    MergeClosingStructures = FailureMechanismIsSelectedToMerge<ClosingStructuresFailureMechanism>(),
                    MergePipingStructure = FailureMechanismIsSelectedToMerge<PipingStructureFailureMechanism>(),
                    MergeStabilityPointStructures = FailureMechanismIsSelectedToMerge<StabilityPointStructuresFailureMechanism>(),
                    MergeStrengthStabilityLengthwiseConstruction = FailureMechanismIsSelectedToMerge<StrengthStabilityLengthwiseConstructionFailureMechanism>(),
                    MergeDuneErosion = FailureMechanismIsSelectedToMerge<DuneErosionFailureMechanism>(),
                    MergeTechnicalInnovation = FailureMechanismIsSelectedToMerge<TechnicalInnovationFailureMechanism>()
                };
                constructionProperties.MergeSpecificFailurePaths.AddRange(GetSelectedSpecificFailurePathsToMerge());

                return new AssessmentSectionMergeData(assessmentSection,
                                                      constructionProperties);
            }

            return null;
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.IsSelected),
                                                  Resources.FailureMechanismMergeDataRow_IsSelected_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismMergeDataRow.Name),
                                                 Resources.FailureMechanism_Name_DisplayName,
                                                 true);
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.InAssembly),
                                                  RiskeerCommonFormsResources.FailurePath_InAssembly_DisplayName,
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
            infoIcon.BackgroundImage = CoreGuiResources.information;
            toolTip.SetToolTip(infoIcon, Resources.AssessmentSectionMergeDataProviderDialog_InfoToolTip);
        }

        private bool FailureMechanismIsSelectedToMerge<TFailureMechanism>()
            where TFailureMechanism : IFailureMechanism
        {
            return failurePathMergeDataRows.Any(row => row.FailurePath is TFailureMechanism && row.IsSelected);
        }

        private IEnumerable<IFailurePath> GetSelectedSpecificFailurePathsToMerge()
        {
            return failurePathMergeDataRows.Where(row => row.IsSelected && !(row.FailurePath is IFailureMechanism))
                                           .Select(row => row.FailurePath)
                                           .ToArray();
        }

        #region Data Setters

        private void SetDataGridViewData(AssessmentSection assessmentSection)
        {
            failurePathMergeDataRows = new[]
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
                                       }
                                       .Concat(assessmentSection.SpecificFailurePaths.Select(fp => new FailurePathMergeDataRow(fp)))
                                       .ToArray();

            dataGridViewControl.SetDataSource(failurePathMergeDataRows);
        }

        #endregion
    }
}
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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.Forms.Observers;
using Riskeer.Integration.Forms.Properties;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// The view for the combined assembly result for all failure mechanisms of 
    /// the <see cref="AssessmentSection"/>.
    /// </summary>
    public partial class AssemblyResultTotalView : UserControl, IView
    {
        private readonly Observer assessmentSectionObserver;
        private readonly Observer assessmentSectionResultObserver;
        private IEnumerable<FailureMechanismAssemblyResultRowBase> assemblyResultRows;

        private bool updateDataSource;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultTotalView"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create the view for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssemblyResultTotalView(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;

            InitializeComponent();

            assessmentSectionObserver = new Observer(() =>
            {
                updateDataSource = true;
                EnableRefreshButton();
            })
            {
                Observable = assessmentSection
            };

            assessmentSectionResultObserver = new Observer(EnableRefreshButton)
            {
                Observable = new AssessmentSectionResultObserver(assessmentSection)
            };
        }

        /// <summary>
        /// Gets the <see cref="AssessmentSection"/> the view belongs to.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }

        public object Data { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitializeDataGridView();
            CheckManualAssemblyResults();
            UpdateAssemblyResultControls();

            dataGridViewControl.CellFormatting += HandleCellStyling;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                assessmentSectionObserver.Dispose();
                assessmentSectionResultObserver.Dispose();
            }

            base.Dispose(disposing);
        }

        private void EnableRefreshButton()
        {
            if (!refreshAssemblyResultsButton.Enabled)
            {
                refreshAssemblyResultsButton.Enabled = true;
                warningProvider.SetError(refreshAssemblyResultsButton,
                                         Resources.AssemblyResultView_RefreshAssemblyResultsButton_Warning_Result_is_outdated_Press_Refresh_button_to_recalculate);
                SetManualAssemblyWarningPadding();
            }
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Name),
                                                 Resources.FailureMechanism_Name_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Code),
                                                 RiskeerCommonFormsResources.FailureMechanism_Code_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Group),
                                                 RiskeerCommonFormsResources.FailureMechanism_Group_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.CategoryGroup),
                                                 RiskeerCommonFormsResources.AssemblyResult_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Probability),
                                                 Resources.AssemblyResultTotalView_Probability_DisplayName,
                                                 true);

            SetDataSource();
        }

        private void SetDataSource()
        {
            assemblyResultRows = new List<FailureMechanismAssemblyResultRowBase>
            {
                CreatePipingFailureMechanismAssemblyResultRow(),
                CreateGrassCoverErosionInwardsFailureMechanismAssemblyResultRow(),
                CreateMacroStabilityInwardsFailureMechanismAssemblyResultRow(),
                CreateMacroStabilityOutwardsFailureMechanismAssemblyResultRow(),
                CreateMicrostabilityFailureMechanismAssemblyResultRow(),
                CreateStabilityStoneCoverFailureMechanismAssemblyResultRow(),
                CreateWaveImpactFailureMechanismAssemblyResultRow(),
                CreateWaterPressureAsphaltCoverFailureMechanismAssemblyResultRow(),
                CreateGrassCoverErosionOutwardsFailureMechanismAssemblyResultRow(),
                CreateGrassCoverSlipOffOutwardsFailureMechanismAssemblyResultRow(),
                CreateGrassCoverSlipOffInwardsFailureMechanismAssemblyResultRow(),
                CreateHeightStructuresFailureMechanismAssemblyResultRow(),
                CreateClosingStructuresFailureMechanismAssemblyResultRow(),
                CreatePipingStructureFailureMechanismAssemblyResultRow(),
                CreateStabilityPointsStructuresFailureMechanismAssemblyResultRow(),
                CreateStrengthStabilityLengthWiseConstructionFailureMechanismAssemblyResultRow(),
                CreateDuneErosionFailureMechanismAssemblyResultRow(),
                CreateTechnicalInnovationFailureMechanismAssemblyResultRow()
            };

            dataGridViewControl.SetDataSource(assemblyResultRows);
            updateDataSource = false;
        }

        private void RefreshAssemblyResults_Click(object sender, EventArgs e)
        {
            ResetRefreshAssemblyResultsButton();

            CheckManualAssemblyResults();

            if (updateDataSource)
            {
                SetDataSource();
            }
            else
            {
                assemblyResultRows.ForEachElementDo(row => row.Update());
                dataGridViewControl.RefreshDataGridView();
            }

            UpdateAssemblyResultControls();
        }

        private void CheckManualAssemblyResults()
        {
            if (AssessmentSectionHelper.HasManualAssemblyResults(AssessmentSection))
            {
                SetManualAssemblyWarningPadding();
                manualAssemblyWarningProvider.SetError(refreshAssemblyResultsButton,
                                                       RiskeerCommonFormsResources.ManualAssemblyWarning_FailureMechanismAssemblyResult_is_based_on_manual_assemblies);
            }
        }

        private void SetManualAssemblyWarningPadding()
        {
            manualAssemblyWarningProvider.SetIconPadding(refreshAssemblyResultsButton, string.IsNullOrEmpty(warningProvider.GetError(refreshAssemblyResultsButton)) ? 4 : 24);
        }

        private void ResetRefreshAssemblyResultsButton()
        {
            refreshAssemblyResultsButton.Enabled = false;
            warningProvider.SetError(refreshAssemblyResultsButton, string.Empty);
            manualAssemblyWarningProvider.SetError(refreshAssemblyResultsButton, string.Empty);
        }

        private void UpdateAssemblyResultControls()
        {
            UpdateTotalAssemblyCategoryGroupControl();
            UpdateFailureMechanismsWithProbabilityAssemblyControl();
            UpdateFailureMechanismsWithoutProbabilityAssemblyControl();
        }

        private void UpdateFailureMechanismsWithoutProbabilityAssemblyControl()
        {
            failureMechanismsWithoutProbabilityAssemblyControl.ClearAssemblyResult();
            failureMechanismsWithoutProbabilityAssemblyControl.ClearMessages();

            try
            {
                failureMechanismsWithoutProbabilityAssemblyControl.SetAssemblyResult(
                    AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(AssessmentSection, true));
            }
            catch (AssemblyException e)
            {
                failureMechanismsWithoutProbabilityAssemblyControl.SetError(e.Message);
            }
        }

        private void UpdateFailureMechanismsWithProbabilityAssemblyControl()
        {
            failureMechanismsWithProbabilityAssemblyControl.ClearAssemblyResult();
            failureMechanismsWithProbabilityAssemblyControl.ClearMessages();

            try
            {
                failureMechanismsWithProbabilityAssemblyControl.SetAssemblyResult(
                    AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(AssessmentSection, true));
            }
            catch (AssemblyException e)
            {
                failureMechanismsWithProbabilityAssemblyControl.SetError(e.Message);
            }
        }

        private void UpdateTotalAssemblyCategoryGroupControl()
        {
            totalAssemblyCategoryGroupControl.ClearAssemblyResult();
            totalAssemblyCategoryGroupControl.ClearMessages();

            try
            {
                totalAssemblyCategoryGroupControl.SetAssemblyResult(
                    AssessmentSectionAssemblyFactory.AssembleAssessmentSection(AssessmentSection, true));
            }
            catch (AssemblyException e)
            {
                totalAssemblyCategoryGroupControl.SetError(e.Message);
            }
        }

        private void HandleCellStyling(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridViewControl.FormatCellWithColumnStateDefinition(e.RowIndex, e.ColumnIndex);
        }

        #region Failure mechanism assembly result rows

        #region Group 1

        private FailureMechanismAssemblyResultRow CreateClosingStructuresFailureMechanismAssemblyResultRow()
        {
            ClosingStructuresFailureMechanism closingStructures = AssessmentSection.ClosingStructures;
            return new FailureMechanismAssemblyResultRow(closingStructures,
                                                         () => ClosingStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(closingStructures,
                                                                                                                                         AssessmentSection,
                                                                                                                                         true));
        }

        private FailureMechanismAssemblyResultRow CreateHeightStructuresFailureMechanismAssemblyResultRow()
        {
            HeightStructuresFailureMechanism heightStructures = AssessmentSection.HeightStructures;
            return new FailureMechanismAssemblyResultRow(heightStructures,
                                                         () => HeightStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(heightStructures,
                                                                                                                                        AssessmentSection,
                                                                                                                                        true));
        }

        private FailureMechanismAssemblyResultRow CreateStabilityPointsStructuresFailureMechanismAssemblyResultRow()
        {
            StabilityPointStructuresFailureMechanism stabilityPointStructures = AssessmentSection.StabilityPointStructures;
            return new FailureMechanismAssemblyResultRow(stabilityPointStructures,
                                                         () => StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(stabilityPointStructures,
                                                                                                                                                AssessmentSection,
                                                                                                                                                true));
        }

        private FailureMechanismAssemblyResultRow CreateGrassCoverErosionInwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = AssessmentSection.GrassCoverErosionInwards;
            return new FailureMechanismAssemblyResultRow(grassCoverErosionInwards,
                                                         () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverErosionInwards,
                                                                                                                                                AssessmentSection,
                                                                                                                                                true));
        }

        #endregion

        #region Group 2

        private FailureMechanismAssemblyResultRowBase CreatePipingFailureMechanismAssemblyResultRow()
        {
            PipingFailureMechanism piping = AssessmentSection.Piping;
            return new FailureMechanismAssemblyResultRow(piping,
                                                         () => PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism(piping,
                                                                                                                              AssessmentSection,
                                                                                                                              true));
        }

        private FailureMechanismAssemblyResultRowBase CreateMacroStabilityInwardsFailureMechanismAssemblyResultRow()
        {
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = AssessmentSection.MacroStabilityInwards;
            return new FailureMechanismAssemblyResultRow(macroStabilityInwards,
                                                         () => MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(macroStabilityInwards,
                                                                                                                                             AssessmentSection,
                                                                                                                                             true));
        }

        #endregion

        #region Group 3

        private FailureMechanismAssemblyCategoryGroupResultRow CreateStabilityStoneCoverFailureMechanismAssemblyResultRow()
        {
            StabilityStoneCoverFailureMechanism stabilityStoneCover = AssessmentSection.StabilityStoneCover;
            return new FailureMechanismAssemblyCategoryGroupResultRow(stabilityStoneCover,
                                                                      () => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(stabilityStoneCover, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateWaveImpactFailureMechanismAssemblyResultRow()
        {
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = AssessmentSection.WaveImpactAsphaltCover;
            return new FailureMechanismAssemblyCategoryGroupResultRow(waveImpactAsphaltCover,
                                                                      () => WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(waveImpactAsphaltCover, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateGrassCoverErosionOutwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = AssessmentSection.GrassCoverErosionOutwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(grassCoverErosionOutwards,
                                                                      () => GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverErosionOutwards, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateDuneErosionFailureMechanismAssemblyResultRow()
        {
            DuneErosionFailureMechanism duneErosion = AssessmentSection.DuneErosion;
            return new FailureMechanismAssemblyCategoryGroupResultRow(duneErosion,
                                                                      () => DuneErosionFailureMechanismAssemblyFactory.AssembleFailureMechanism(duneErosion, true));
        }

        #endregion

        #region Group 4

        private FailureMechanismAssemblyResultRowBase CreateMacroStabilityOutwardsFailureMechanismAssemblyResultRow()
        {
            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = AssessmentSection.MacroStabilityOutwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(macroStabilityOutwards,
                                                                      () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(macroStabilityOutwards,
                                                                                                                                                           AssessmentSection,
                                                                                                                                                           true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateMicrostabilityFailureMechanismAssemblyResultRow()
        {
            MicrostabilityFailureMechanism microstability = AssessmentSection.Microstability;
            return new FailureMechanismAssemblyCategoryGroupResultRow(microstability,
                                                                      () => MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(microstability, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateWaterPressureAsphaltCoverFailureMechanismAssemblyResultRow()
        {
            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = AssessmentSection.WaterPressureAsphaltCover;
            return new FailureMechanismAssemblyCategoryGroupResultRow(waterPressureAsphaltCover,
                                                                      () => WaterPressureAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(waterPressureAsphaltCover, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateGrassCoverSlipOffOutwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = AssessmentSection.GrassCoverSlipOffOutwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(grassCoverSlipOffOutwards,
                                                                      () => GrassCoverSlipOffOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverSlipOffOutwards, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateGrassCoverSlipOffInwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = AssessmentSection.GrassCoverSlipOffInwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(grassCoverSlipOffInwards,
                                                                      () => GrassCoverSlipOffInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverSlipOffInwards, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreatePipingStructureFailureMechanismAssemblyResultRow()
        {
            PipingStructureFailureMechanism pipingStructure = AssessmentSection.PipingStructure;
            return new FailureMechanismAssemblyCategoryGroupResultRow(pipingStructure,
                                                                      () => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(pipingStructure, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateStrengthStabilityLengthWiseConstructionFailureMechanismAssemblyResultRow()
        {
            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = AssessmentSection.StrengthStabilityLengthwiseConstruction;
            return new FailureMechanismAssemblyCategoryGroupResultRow(strengthStabilityLengthwiseConstruction,
                                                                      () => StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.AssembleFailureMechanism(strengthStabilityLengthwiseConstruction, true));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateTechnicalInnovationFailureMechanismAssemblyResultRow()
        {
            TechnicalInnovationFailureMechanism technicalInnovation = AssessmentSection.TechnicalInnovation;
            return new FailureMechanismAssemblyCategoryGroupResultRow(technicalInnovation,
                                                                      () => TechnicalInnovationFailureMechanismAssemblyFactory.AssembleFailureMechanism(technicalInnovation, true));
        }

        #endregion

        #endregion
    }
}
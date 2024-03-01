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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.Forms.Factories;
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
        private IEnumerable<FailureMechanismAssemblyResultRow> assemblyResultRows;

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

            assessmentSectionObserver = new Observer(UpdateUserControls)
            {
                Observable = assessmentSection
            };

            assessmentSectionResultObserver = new Observer(UpdateUserControls)
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
            UpdateAssemblyResultControls();
            UpdateFailureMechanismsCorrelatedCheckBox();

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

        private void UpdateUserControls()
        {
            EnableRefreshButton();
            UpdateFailureMechanismsCorrelatedCheckBox();
        }

        private void EnableRefreshButton()
        {
            if (!refreshAssemblyResultsButton.Enabled)
            {
                refreshAssemblyResultsButton.Enabled = true;
                warningProvider.SetError(refreshAssemblyResultsButton,
                                         Resources.AssemblyResultTotalView_RefreshAssemblyResultsButton_Warning_Result_is_outdated_Press_Refresh_button_to_recalculate);
            }
        }

        private void UpdateFailureMechanismsCorrelatedCheckBox()
        {
            checkBox.Visible = AssessmentSectionAssemblyHelper.AllCorrelatedFailureMechanismsInAssembly(AssessmentSection);
            checkBox.Checked = AssessmentSection.AreFailureMechanismsCorrelated;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRow.Name),
                                                 Resources.FailureMechanism_Name_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRow.Code),
                                                 RiskeerCommonFormsResources.FailureMechanism_Code_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRow.Probability),
                                                 Resources.AssemblyResult_Probability_DisplayName,
                                                 true);

            SetDataSource();
        }

        private void SetDataSource()
        {
            var failureMechanismAssemblyResultRows = new List<FailureMechanismAssemblyResultRow>();
            failureMechanismAssemblyResultRows.AddRange(CreateFailureMechanismResultRows());
            failureMechanismAssemblyResultRows.AddRange(CreateSpecificFailureMechanismResultRows());

            assemblyResultRows = failureMechanismAssemblyResultRows;

            dataGridViewControl.SetDataSource(assemblyResultRows);
        }

        private void RefreshAssemblyResults_Click(object sender, EventArgs e)
        {
            ResetRefreshAssemblyResultsButton();

            SetDataSource();

            UpdateAssemblyResultControls();
        }

        private void ResetRefreshAssemblyResultsButton()
        {
            refreshAssemblyResultsButton.Enabled = false;
            warningProvider.SetError(refreshAssemblyResultsButton, string.Empty);
        }

        private void UpdateAssemblyResultControls()
        {
            UpdateTotalAssemblyGroupControl();
        }

        private void UpdateTotalAssemblyGroupControl()
        {
            assessmentSectionAssemblyControl.ClearAssemblyResult();
            assessmentSectionAssemblyControl.ClearErrors();

            try
            {
                assessmentSectionAssemblyControl.SetAssemblyResult(
                    AssessmentSectionAssemblyFactory.AssembleAssessmentSection(AssessmentSection).AssemblyResult);
            }
            catch (AssemblyException e)
            {
                assessmentSectionAssemblyControl.SetError(e.Message);
            }
        }

        private void HandleCellStyling(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridViewControl.FormatCellWithColumnStateDefinition(e.RowIndex, e.ColumnIndex);
        }

        #region Specific failure mechanism assembly result rows

        private IEnumerable<FailureMechanismAssemblyResultRow> CreateSpecificFailureMechanismResultRows()
        {
            return AssessmentSection.SpecificFailureMechanisms
                                    .Select(fp => FailureMechanismAssemblyResultRowFactory.CreateRow(
                                                fp, () => FailureMechanismAssemblyFactory.AssembleFailureMechanism(fp, AssessmentSection))
                                    )
                                    .ToArray();
        }

        #endregion

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableRefreshButton();
            AssessmentSection.AreFailureMechanismsCorrelated = checkBox.Checked;
        }

        #region Failure mechanism assembly result rows

        private IEnumerable<FailureMechanismAssemblyResultRow> CreateFailureMechanismResultRows()
        {
            return new[]
            {
                CreatePipingFailureMechanismAssemblyResultRow(),
                CreateGrassCoverErosionInwardsFailureMechanismAssemblyResultRow(),
                CreateMacroStabilityInwardsFailureMechanismAssemblyResultRow(),
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
                CreateDuneErosionFailureMechanismAssemblyResultRow()
            };
        }

        private FailureMechanismAssemblyResultRow CreateClosingStructuresFailureMechanismAssemblyResultRow()
        {
            ClosingStructuresFailureMechanism closingStructures = AssessmentSection.ClosingStructures;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                closingStructures, () => ClosingStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(closingStructures, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateHeightStructuresFailureMechanismAssemblyResultRow()
        {
            HeightStructuresFailureMechanism heightStructures = AssessmentSection.HeightStructures;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                heightStructures, () => HeightStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(heightStructures, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateStabilityPointsStructuresFailureMechanismAssemblyResultRow()
        {
            StabilityPointStructuresFailureMechanism stabilityPointStructures = AssessmentSection.StabilityPointStructures;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                stabilityPointStructures, () => StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(stabilityPointStructures, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateGrassCoverErosionInwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = AssessmentSection.GrassCoverErosionInwards;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                grassCoverErosionInwards, () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverErosionInwards, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreatePipingFailureMechanismAssemblyResultRow()
        {
            PipingFailureMechanism piping = AssessmentSection.Piping;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                piping, () => PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism(piping, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateMacroStabilityInwardsFailureMechanismAssemblyResultRow()
        {
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = AssessmentSection.MacroStabilityInwards;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                macroStabilityInwards, () => MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(macroStabilityInwards, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateStabilityStoneCoverFailureMechanismAssemblyResultRow()
        {
            StabilityStoneCoverFailureMechanism stabilityStoneCover = AssessmentSection.StabilityStoneCover;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                stabilityStoneCover, () => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(stabilityStoneCover, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateWaveImpactFailureMechanismAssemblyResultRow()
        {
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = AssessmentSection.WaveImpactAsphaltCover;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                waveImpactAsphaltCover, () => WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(waveImpactAsphaltCover, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateGrassCoverErosionOutwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = AssessmentSection.GrassCoverErosionOutwards;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                grassCoverErosionOutwards, () => GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverErosionOutwards, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateDuneErosionFailureMechanismAssemblyResultRow()
        {
            DuneErosionFailureMechanism duneErosion = AssessmentSection.DuneErosion;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                duneErosion, () => DuneErosionFailureMechanismAssemblyFactory.AssembleFailureMechanism(duneErosion, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateMicrostabilityFailureMechanismAssemblyResultRow()
        {
            return CreateStandAloneFailureMechanismAssemblyResultRow(AssessmentSection.Microstability);
        }

        private FailureMechanismAssemblyResultRow CreateWaterPressureAsphaltCoverFailureMechanismAssemblyResultRow()
        {
            return CreateStandAloneFailureMechanismAssemblyResultRow(AssessmentSection.WaterPressureAsphaltCover);
        }

        private FailureMechanismAssemblyResultRow CreateGrassCoverSlipOffOutwardsFailureMechanismAssemblyResultRow()
        {
            return CreateStandAloneFailureMechanismAssemblyResultRow(AssessmentSection.GrassCoverSlipOffOutwards);
        }

        private FailureMechanismAssemblyResultRow CreateGrassCoverSlipOffInwardsFailureMechanismAssemblyResultRow()
        {
            return CreateStandAloneFailureMechanismAssemblyResultRow(AssessmentSection.GrassCoverSlipOffInwards);
        }

        private FailureMechanismAssemblyResultRow CreatePipingStructureFailureMechanismAssemblyResultRow()
        {
            PipingStructureFailureMechanism pipingStructure = AssessmentSection.PipingStructure;
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                pipingStructure, () => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(pipingStructure, AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateStandAloneFailureMechanismAssemblyResultRow<TFailureMechanism>(TFailureMechanism failureMechanism)
            where TFailureMechanism : IHasGeneralInput, IFailureMechanism<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>
        {
            return FailureMechanismAssemblyResultRowFactory.CreateRow(
                failureMechanism, () => FailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, AssessmentSection));
        }

        #endregion
    }
}
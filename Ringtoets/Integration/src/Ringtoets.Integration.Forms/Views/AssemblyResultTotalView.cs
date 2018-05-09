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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// The view for the combined assembly result for all failure mechanisms of 
    /// the <see cref="AssessmentSection"/>.
    /// </summary>
    public partial class AssemblyResultTotalView : UserControl, IView
    {
        private IEnumerable<FailureMechanismAssemblyResultRowBase> assemblyResultRows;

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

            dataGridViewControl.CellFormatting += HandleCellStyling;
        }

        protected override void Dispose(bool disposing)
        {
            dataGridViewControl.CellFormatting -= HandleCellStyling;

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Name),
                                                 Resources.FailureMechanismContributionView_GridColumn_Assessment,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Code),
                                                 RingtoetsCommonFormsResources.FailureMechanism_Code_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Group),
                                                 RingtoetsCommonFormsResources.FailureMechanism_Group_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.CategoryGroup),
                                                 Resources.AssemblyCategory_Group_DisplayName,
                                                 true);

            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismAssemblyResultRowBase.Probablity),
                                                 Resources.AssemblyResultTotalView_Probability_DisplayName,
                                                 true);

            InitializeRows();
        }

        private void InitializeRows()
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
        }

        private void RefreshAssemblyResults_Click(object sender, EventArgs e)
        {
            assemblyResultRows.ForEachElementDo(row => row.Update());
            dataGridViewControl.RefreshDataGridView();
            UpdateAssemblyResultControls();
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
            failureMechanismsWithoutProbabilityAssemblyControl.ClearError();

            try
            {
                failureMechanismsWithoutProbabilityAssemblyControl.SetAssemblyResult(
                    AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(AssessmentSection));
            }
            catch (AssemblyException e)
            {
                failureMechanismsWithoutProbabilityAssemblyControl.SetError(e.Message);
            }
        }

        private void UpdateFailureMechanismsWithProbabilityAssemblyControl()
        {
            failureMechanismsWithProbabilityAssemblyControl.ClearAssemblyResult();
            failureMechanismsWithProbabilityAssemblyControl.ClearError();

            try
            {
                failureMechanismsWithProbabilityAssemblyControl.SetAssemblyResult(
                    AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(AssessmentSection));
            }
            catch (AssemblyException e)
            {
                failureMechanismsWithProbabilityAssemblyControl.SetError(e.Message);
            }
        }

        private void UpdateTotalAssemblyCategoryGroupControl()
        {
            totalAssemblyCategoryGroupControl.ClearAssemblyResult();
            totalAssemblyCategoryGroupControl.ClearError();

            try
            {
                totalAssemblyCategoryGroupControl.SetAssemblyResult(
                    AssessmentSectionAssemblyFactory.AssembleAssessmentSection(AssessmentSection));
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
                                                                                                                                         AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateHeightStructuresFailureMechanismAssemblyResultRow()
        {
            HeightStructuresFailureMechanism heightStructures = AssessmentSection.HeightStructures;
            return new FailureMechanismAssemblyResultRow(heightStructures,
                                                         () => HeightStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(heightStructures,
                                                                                                                                        AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateStabilityPointsStructuresFailureMechanismAssemblyResultRow()
        {
            StabilityPointStructuresFailureMechanism stabilityPointStructures = AssessmentSection.StabilityPointStructures;
            return new FailureMechanismAssemblyResultRow(stabilityPointStructures,
                                                         () => StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(stabilityPointStructures,
                                                                                                                                                AssessmentSection));
        }

        private FailureMechanismAssemblyResultRow CreateGrassCoverErosionInwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = AssessmentSection.GrassCoverErosionInwards;
            return new FailureMechanismAssemblyResultRow(grassCoverErosionInwards,
                                                         () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverErosionInwards,
                                                                                                                                                AssessmentSection));
        }

        #endregion

        #region Group 2

        private FailureMechanismAssemblyResultRowBase CreatePipingFailureMechanismAssemblyResultRow()
        {
            PipingFailureMechanism piping = AssessmentSection.Piping;
            return new FailureMechanismAssemblyResultRow(piping,
                                                         () => PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism(piping,
                                                                                                                              AssessmentSection));
        }

        private FailureMechanismAssemblyResultRowBase CreateMacroStabilityInwardsFailureMechanismAssemblyResultRow()
        {
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = AssessmentSection.MacroStabilityInwards;
            return new FailureMechanismAssemblyResultRow(macroStabilityInwards,
                                                         () => MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(macroStabilityInwards,
                                                                                                                                             AssessmentSection));
        }

        #endregion

        #region Group 3

        private FailureMechanismAssemblyCategoryGroupResultRow CreateStabilityStoneCoverFailureMechanismAssemblyResultRow()
        {
            StabilityStoneCoverFailureMechanism stabilityStoneCover = AssessmentSection.StabilityStoneCover;
            return new FailureMechanismAssemblyCategoryGroupResultRow(stabilityStoneCover,
                                                                      () => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(stabilityStoneCover));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateWaveImpactFailureMechanismAssemblyResultRow()
        {
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = AssessmentSection.WaveImpactAsphaltCover;
            return new FailureMechanismAssemblyCategoryGroupResultRow(waveImpactAsphaltCover,
                                                                      () => WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(waveImpactAsphaltCover));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateGrassCoverErosionOutwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = AssessmentSection.GrassCoverErosionOutwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(grassCoverErosionOutwards,
                                                                      () => GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverErosionOutwards));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateDuneErosionFailureMechanismAssemblyResultRow()
        {
            DuneErosionFailureMechanism duneErosion = AssessmentSection.DuneErosion;
            return new FailureMechanismAssemblyCategoryGroupResultRow(duneErosion,
                                                                      () => DuneErosionFailureMechanismAssemblyFactory.AssembleFailureMechanism(duneErosion));
        }

        #endregion

        #region Group 4

        private FailureMechanismAssemblyResultRowBase CreateMacroStabilityOutwardsFailureMechanismAssemblyResultRow()
        {
            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = AssessmentSection.MacroStabilityOutwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(macroStabilityOutwards,
                                                                      () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(macroStabilityOutwards,
                                                                                                                                                           AssessmentSection));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateMicrostabilityFailureMechanismAssemblyResultRow()
        {
            MicrostabilityFailureMechanism microstability = AssessmentSection.Microstability;
            return new FailureMechanismAssemblyCategoryGroupResultRow(microstability,
                                                                      () => MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(microstability));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateWaterPressureAsphaltCoverFailureMechanismAssemblyResultRow()
        {
            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = AssessmentSection.WaterPressureAsphaltCover;
            return new FailureMechanismAssemblyCategoryGroupResultRow(waterPressureAsphaltCover,
                                                                      () => WaterPressureAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(waterPressureAsphaltCover));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateGrassCoverSlipOffOutwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = AssessmentSection.GrassCoverSlipOffOutwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(grassCoverSlipOffOutwards,
                                                                      () => GrassCoverSlipOffOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverSlipOffOutwards));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateGrassCoverSlipOffInwardsFailureMechanismAssemblyResultRow()
        {
            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = AssessmentSection.GrassCoverSlipOffInwards;
            return new FailureMechanismAssemblyCategoryGroupResultRow(grassCoverSlipOffInwards,
                                                                      () => GrassCoverSlipOffInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(grassCoverSlipOffInwards));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreatePipingStructureFailureMechanismAssemblyResultRow()
        {
            PipingStructureFailureMechanism pipingStructure = AssessmentSection.PipingStructure;
            return new FailureMechanismAssemblyCategoryGroupResultRow(pipingStructure,
                                                                      () => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(pipingStructure));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateStrengthStabilityLengthWiseConstructionFailureMechanismAssemblyResultRow()
        {
            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = AssessmentSection.StrengthStabilityLengthwiseConstruction;
            return new FailureMechanismAssemblyCategoryGroupResultRow(strengthStabilityLengthwiseConstruction,
                                                                      () => StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.AssembleFailureMechanism(strengthStabilityLengthwiseConstruction));
        }

        private FailureMechanismAssemblyCategoryGroupResultRow CreateTechnicalInnovationFailureMechanismAssemblyResultRow()
        {
            TechnicalInnovationFailureMechanism technicalInnovation = AssessmentSection.TechnicalInnovation;
            return new FailureMechanismAssemblyCategoryGroupResultRow(technicalInnovation,
                                                                      () => TechnicalInnovationFailureMechanismAssemblyFactory.AssembleFailureMechanism(technicalInnovation));
        }

        #endregion

        #endregion
    }
}
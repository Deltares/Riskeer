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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Forms.Properties;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;
using MacroStabilityInwardsDataResources = Ringtoets.MacroStabilityInwards.Data.Properties.Resources;
using IntegrationDataResources = Ringtoets.Integration.Data.Properties.Resources;
using StabilityStoneCoverDataResources = Ringtoets.StabilityStoneCover.Data.Properties.Resources;
using WaveImpactAsphaltCoverDataResources = Ringtoets.WaveImpactAsphaltCover.Data.Properties.Resources;
using GrassCoverErosionOutwardsDataResources = Ringtoets.GrassCoverErosionOutwards.Data.Properties.Resources;
using HeightStructuresDataResources = Ringtoets.HeightStructures.Data.Properties.Resources;
using ClosingStructuresDataResources = Ringtoets.ClosingStructures.Data.Properties.Resources;
using StabilityPointStructuresDataResources = Ringtoets.StabilityPointStructures.Data.Properties.Resources;
using DuneErosionDataResources = Ringtoets.DuneErosion.Data.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// The view for the assembly result per section for all failure mechanisms of 
    /// the <see cref="AssessmentSection"/>. 
    /// </summary>
    public partial class AssemblyResultPerSectionView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultPerSectionView"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create the view for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public AssemblyResultPerSectionView(AssessmentSection assessmentSection)
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

        private void HandleCellStyling(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridViewControl.FormatCellWithColumnStateDefinition(e.RowIndex, e.ColumnIndex);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.SectionStart),
                                                 Resources.AssemblyResultPerSectionView_GridColumn_SectionStart,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.SectionEnd),
                                                 Resources.AssemblyResultPerSectionView_GridColumn_SectionEnd,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.TotalResult),
                                                 Resources.AssemblyResultPerSectionView_GridColumn_SectionTotalAssembly,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.Piping),
                                                 PipingDataResources.PipingFailureMechanism_DisplayCode,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.GrassCoverErosionInwards),
                                                 GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsFailureMechanism_DisplayCode,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.MacroStabilityInwards),
                                                 MacroStabilityInwardsDataResources.MacroStabilityInwardsFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.MacroStabilityOutwards),
                                                 IntegrationDataResources.MacroStabilityOutwardsFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.Microstability),
                                                 IntegrationDataResources.MicrostabilityFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.StabilityStoneCover),
                                                 StabilityStoneCoverDataResources.StabilityStoneCoverFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.WaveImpactAsphaltCover),
                                                 WaveImpactAsphaltCoverDataResources.WaveImpactAsphaltCoverFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.WaterPressureAsphaltCover),
                                                 IntegrationDataResources.WaterPressureAsphaltCoverFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.GrassCoverErosionOutwards),
                                                 GrassCoverErosionOutwardsDataResources.GrassCoverErosionOutwardsFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.GrassCoverSlipOffOutwards),
                                                 IntegrationDataResources.GrassCoverSlipOffOutwardsFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.GrassCoverSlipOffInwards),
                                                 IntegrationDataResources.GrassCoverSlipOffInwardsFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.HeightStructures),
                                                 HeightStructuresDataResources.HeightStructuresFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.ClosingStructures),
                                                 ClosingStructuresDataResources.ClosingStructuresFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.PipingStructures),
                                                 IntegrationDataResources.PipingStructureFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.StabilityPointStructures),
                                                 StabilityPointStructuresDataResources.StabilityPointStructuresFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.StrengthStabilityLengthwise),
                                                 IntegrationDataResources.StrengthStabilityLengthwiseConstructionFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.DuneErosion),
                                                 DuneErosionDataResources.DuneErosionFailureMechanism_Code,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(CombinedFailureMechanismSectionAssemblyResultRow.TechnicalInnovation),
                                                 IntegrationDataResources.TechnicalInnovationFailureMechanism_Code,
                                                 true);

            SetDataSource();
        }

        private void RefreshAssemblyResults_Click(object sender, EventArgs e)
        {
            SetDataSource();
        }

        private void SetDataSource()
        {
            ClearControls();
            try
            {
                dataGridViewControl.SetDataSource(CreateResults().Select(r => new CombinedFailureMechanismSectionAssemblyResultRow(r))
                                                                 .ToArray());
            }
            catch (AssemblyException e)
            {
                errorProvider.SetError(RefreshAssemblyResultsButton, e.Message);
            }
        }

        private void ClearControls()
        {
            errorProvider.Clear();
            dataGridViewControl.SetDataSource(Enumerable.Empty<CombinedFailureMechanismSectionAssemblyResult>());
        }

        private static IEnumerable<CombinedFailureMechanismSectionAssemblyResult> CreateResults()
        {
            var random = new Random();

            return Enumerable.Repeat(CreateResult(), random.Next(1, 5)).ToArray();
        }

        private static CombinedFailureMechanismSectionAssemblyResult CreateResult()
        {
            var random = new Random();
            return new CombinedFailureMechanismSectionAssemblyResult(
                random.NextDouble(),
                random.NextDouble(),
                GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                {
                    Piping = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    GrassCoverErosionInwards = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    MacroStabilityInwards = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    MacroStabilityOutwards = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    Microstability = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    StabilityStoneCover = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    WaveImpactAsphaltCover = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    WaterPressureAsphaltCover = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    GrassCoverErosionOutwards = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    GrassCoverSlipOffOutwards = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    GrassCoverSlipOffInwards = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    HeightStructures = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    ClosingStructures = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    PipingStructures = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    StabilityPointStructures = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    StrengthStabilityLengthwise = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    DuneErosion = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random),
                    TechnicalInnovation = GetRandomFailureMechanismSectionAssemblyCategoryGroup(random)
                });
        }

        private static FailureMechanismSectionAssemblyCategoryGroup GetRandomFailureMechanismSectionAssemblyCategoryGroup(Random random)
        {
            var failureMechanismSectionAssemblyCategoryGroups =
                (FailureMechanismSectionAssemblyCategoryGroup[]) Enum.GetValues(typeof(FailureMechanismSectionAssemblyCategoryGroup));

            return failureMechanismSectionAssemblyCategoryGroups[random.Next(failureMechanismSectionAssemblyCategoryGroups.Length)];
        }
    }
}
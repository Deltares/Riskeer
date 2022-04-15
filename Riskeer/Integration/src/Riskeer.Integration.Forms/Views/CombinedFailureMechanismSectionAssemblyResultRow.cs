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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Forms.Properties;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of the <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
    /// </summary>
    public class CombinedFailureMechanismSectionAssemblyResultRow : IHasColumnStateDefinitions
    {
        private const int pipingIndex = 2;
        private const int grassCoverErosionInwardsIndex = 3;
        private const int macroStabilityInwardsIndex = 4;
        private const int microstabililityIndex = 5;
        private const int stabilityStoneCoverIndex = 6;
        private const int waveImpactAsphaltCoverIndex = 7;
        private const int waterPressureAsphaltCoverIndex = 8;
        private const int grassCoverErosionOutwardsIndex = 9;
        private const int grassCoverSlipOffOutwardsIndex = 10;
        private const int grassCoverSlipOffInwardsIndex = 11;
        private const int heightStructuresIndex = 12;
        private const int closingStructuresIndex = 13;
        private const int pipingStructureIndex = 14;
        private const int stabilityPointStructuresIndex = 15;
        private const int duneErosionIndex = 16;
        private const int specificFailureMechanismStartIndex = 17;
        private readonly int totalResultIndex;

        private readonly CombinedFailureMechanismSectionAssemblyResult combinedFailureMechanismSectionAssemblyResult;

        /// <summary>
        /// Creates a new instance of <see cref="CombinedFailureMechanismSectionAssemblyResultRow"/>.
        /// </summary>
        /// <param name="combinedFailureMechanismSectionAssemblyResult">The <see cref="CombinedFailureMechanismSectionAssemblyResult"/> 
        /// to wrap so that it can be displayed as a row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="combinedFailureMechanismSectionAssemblyResult"/>
        /// is <c>null</c>.</exception>
        public CombinedFailureMechanismSectionAssemblyResultRow(
            CombinedFailureMechanismSectionAssemblyResult combinedFailureMechanismSectionAssemblyResult)
        {
            if (combinedFailureMechanismSectionAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(combinedFailureMechanismSectionAssemblyResult));
            }

            this.combinedFailureMechanismSectionAssemblyResult = combinedFailureMechanismSectionAssemblyResult;

            totalResultIndex = specificFailureMechanismStartIndex + SpecificFailureMechanisms.Length;

            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();
            SetColumnStateDefinitionColors();
        }

        /// <summary>
        /// Gets the start of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public RoundedDouble SectionStart => new RoundedDouble(2, combinedFailureMechanismSectionAssemblyResult.SectionStart);

        /// <summary>
        /// Gets the end of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public RoundedDouble SectionEnd => new RoundedDouble(2, combinedFailureMechanismSectionAssemblyResult.SectionEnd);

        /// <summary>
        /// Gets the total assembly result.
        /// </summary>
        public string TotalResult => EnumDisplayNameHelper.GetDisplayName(combinedFailureMechanismSectionAssemblyResult.TotalResult);

        /// <summary>
        /// Gets the assembly result for piping.
        /// </summary>
        public string Piping => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.Piping);

        /// <summary>
        /// Gets the assembly result for grass cover erosion inwards.
        /// </summary>
        public string GrassCoverErosionInwards => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionInwards);

        /// <summary>
        /// Gets the assembly result for macro stability inwards.
        /// </summary>
        public string MacroStabilityInwards => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.MacroStabilityInwards);

        /// <summary>
        /// Gets the assembly result for microstability.
        /// </summary>
        public string Microstability => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.Microstability);

        /// <summary>
        /// Gets the assembly result for stability stone cover.
        /// </summary>
        public string StabilityStoneCover => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.StabilityStoneCover);

        /// <summary>
        /// Gets the assembly result for wave impact asphalt cover.
        /// </summary>
        public string WaveImpactAsphaltCover => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.WaveImpactAsphaltCover);

        /// <summary>
        /// Gets the assembly result for water pressure asphalt cover.
        /// </summary>
        public string WaterPressureAsphaltCover => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.WaterPressureAsphaltCover);

        /// <summary>
        /// Gets the assembly result for grass cover erosion outwards.
        /// </summary>
        public string GrassCoverErosionOutwards => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionOutwards);

        /// <summary>
        /// Gets the assembly result for grass cover slip off outwards.
        /// </summary>
        public string GrassCoverSlipOffOutwards => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffOutwards);

        /// <summary>
        /// Gets the assembly result for grass cover slip off inwards.
        /// </summary>
        public string GrassCoverSlipOffInwards => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffInwards);

        /// <summary>
        /// Gets the assembly result for height structures.
        /// </summary>
        public string HeightStructures => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.HeightStructures);

        /// <summary>
        /// Gets the assembly result for closing structures.
        /// </summary>
        public string ClosingStructures => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.ClosingStructures);

        /// <summary>
        /// Gets the assembly result for piping structure.
        /// </summary>
        public string PipingStructure => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.PipingStructure);

        /// <summary>
        /// Gets the assembly result for stability point structures.
        /// </summary>
        public string StabilityPointStructures => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.StabilityPointStructures);

        /// <summary>
        /// Gets the assembly result for dune erosion.
        /// </summary>
        public string DuneErosion => GetDisplayNameForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.DuneErosion);

        /// <summary>
        /// Gets the assembly result for specific failure mechanisms.
        /// </summary>
        public string[] SpecificFailureMechanisms =>
            combinedFailureMechanismSectionAssemblyResult.SpecificFailureMechanisms
                                                         .Select(GetDisplayNameForFailureMechanism).ToArray();

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(pipingIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverErosionInwardsIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(macroStabilityInwardsIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(microstabililityIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(stabilityStoneCoverIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(waveImpactAsphaltCoverIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(waterPressureAsphaltCoverIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverErosionOutwardsIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverSlipOffOutwardsIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverSlipOffInwardsIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(heightStructuresIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(closingStructuresIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(pipingStructureIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(stabilityPointStructuresIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(duneErosionIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());

            for (var i = 0; i < SpecificFailureMechanisms.Length; i++)
            {
                ColumnStateDefinitions.Add(specificFailureMechanismStartIndex + i, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            }

            ColumnStateDefinitions.Add(totalResultIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
        }

        private void SetColumnStateDefinitionColors()
        {
            ColumnStateDefinitions[pipingIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.Piping);
            ColumnStateDefinitions[grassCoverErosionInwardsIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionInwards);
            ColumnStateDefinitions[macroStabilityInwardsIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.MacroStabilityInwards);
            ColumnStateDefinitions[microstabililityIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.Microstability);
            ColumnStateDefinitions[stabilityStoneCoverIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.StabilityStoneCover);
            ColumnStateDefinitions[waveImpactAsphaltCoverIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.WaveImpactAsphaltCover);
            ColumnStateDefinitions[waterPressureAsphaltCoverIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.WaterPressureAsphaltCover);
            ColumnStateDefinitions[grassCoverErosionOutwardsIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionOutwards);
            ColumnStateDefinitions[grassCoverSlipOffOutwardsIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffOutwards);
            ColumnStateDefinitions[grassCoverSlipOffInwardsIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffInwards);
            ColumnStateDefinitions[heightStructuresIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.HeightStructures);
            ColumnStateDefinitions[closingStructuresIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.ClosingStructures);
            ColumnStateDefinitions[pipingStructureIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.PipingStructure);
            ColumnStateDefinitions[stabilityPointStructuresIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.StabilityPointStructures);
            ColumnStateDefinitions[duneErosionIndex].Style = CreateCellStyleForFailureMechanism(combinedFailureMechanismSectionAssemblyResult.DuneErosion);

            FailureMechanismSectionAssemblyGroup?[] specificFailureMechanismAssemblyResults = combinedFailureMechanismSectionAssemblyResult.SpecificFailureMechanisms;
            int nrOfSpecificFailureMechanisms = specificFailureMechanismAssemblyResults.Length;
            for (var i = 0; i < nrOfSpecificFailureMechanisms; i++)
            {
                ColumnStateDefinitions[specificFailureMechanismStartIndex + i].Style = CreateCellStyleForFailureMechanism(specificFailureMechanismAssemblyResults[i]);
            }

            ColumnStateDefinitions[totalResultIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.TotalResult);
        }

        private static string GetDisplayNameForFailureMechanism(FailureMechanismSectionAssemblyGroup? failureMechanismSectionAssemblyGroup)
        {
            return failureMechanismSectionAssemblyGroup.HasValue
                       ? EnumDisplayNameHelper.GetDisplayName(failureMechanismSectionAssemblyGroup.Value)
                       : Resources.CombinedFailureMechanismSectionAssemblyResultRow_Not_in_assembly_dash;
        }

        private static CellStyle CreateCellStyle(FailureMechanismSectionAssemblyGroup failureMechanismSectionAssemblyGroup)
        {
            return new CellStyle(Color.FromKnownColor(KnownColor.ControlText),
                                 FailureMechanismSectionAssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyGroupColor(failureMechanismSectionAssemblyGroup));
        }

        private static CellStyle CreateCellStyleForFailureMechanism(FailureMechanismSectionAssemblyGroup? failureMechanismSectionAssemblyGroup)
        {
            return failureMechanismSectionAssemblyGroup.HasValue
                       ? CreateCellStyle(failureMechanismSectionAssemblyGroup.Value)
                       : new CellStyle(Color.FromKnownColor(KnownColor.ControlText), Color.FromArgb(255, 255, 255));
        }
    }
}
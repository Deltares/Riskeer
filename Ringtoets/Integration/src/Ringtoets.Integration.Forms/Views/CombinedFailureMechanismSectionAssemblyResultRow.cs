// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Integration.Data.Assembly;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of the <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
    /// </summary>
    public class CombinedFailureMechanismSectionAssemblyResultRow : IHasColumnStateDefinitions
    {
        private const int totalResultIndex = 3;
        private const int pipingIndex = 4;
        private const int grassCoverErosionInwardsIndex = 5;
        private const int macroStabilityInwardsIndex = 6;
        private const int macroStabilityOutwardsIndex = 7;
        private const int microstabililityIndex = 8;
        private const int stabilityStoneCoverIndex = 9;
        private const int waveImpactAsphaltCoverIndex = 10;
        private const int waterPressureAsphaltCoverIndex = 11;
        private const int grassCoverErosionOutwardsIndex = 12;
        private const int grassCoverSlipOffOutwardsIndex = 13;
        private const int grassCoverSlipOffInwardsIndex = 14;
        private const int heightStructuresIndex = 15;
        private const int closingStructuresIndex = 16;
        private const int pipingStructureIndex = 17;
        private const int stabilityPointStructuresIndex = 18;
        private const int strengthStabilityLengthwiseConstructionIndex = 19;
        private const int duneErosionIndex = 20;
        private const int technicalInnovationIndex = 21;

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

            ColumnStateDefinitions = new Dictionary<int, DataGridViewColumnStateDefinition>();
            CreateColumnStateDefinitions();
            SetColumnStateDefinitionColors();
        }

        /// <summary>
        /// Gets the number of the section.
        /// </summary>
        public int SectionNumber
        {
            get
            {
                return combinedFailureMechanismSectionAssemblyResult.SectionNumber;
            }
        }

        /// <summary>
        /// Gets the start of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public RoundedDouble SectionStart
        {
            get
            {
                return new RoundedDouble(2, combinedFailureMechanismSectionAssemblyResult.SectionStart);
            }
        }

        /// <summary>
        /// Gets the end of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public RoundedDouble SectionEnd
        {
            get
            {
                return new RoundedDouble(2, combinedFailureMechanismSectionAssemblyResult.SectionEnd);
            }
        }

        /// <summary>
        /// Gets the total assembly result.
        /// </summary>
        public string TotalResult
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.TotalResult);
            }
        }

        /// <summary>
        /// Gets the assembly result for piping.
        /// </summary>
        public string Piping
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.Piping);
            }
        }

        /// <summary>
        /// Gets the assembly result for grass cover erosion inwards.
        /// </summary>
        public string GrassCoverErosionInwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionInwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for macro stability inwards.
        /// </summary>
        public string MacroStabilityInwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.MacroStabilityInwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for macro stability outwards.
        /// </summary>
        public string MacroStabilityOutwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.MacroStabilityOutwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for microstability.
        /// </summary>
        public string Microstability
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.Microstability);
            }
        }

        /// <summary>
        /// Gets the assembly result for stability stone cover.
        /// </summary>
        public string StabilityStoneCover
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.StabilityStoneCover);
            }
        }

        /// <summary>
        /// Gets the assembly result for wave impact asphalt cover.
        /// </summary>
        public string WaveImpactAsphaltCover
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.WaveImpactAsphaltCover);
            }
        }

        /// <summary>
        /// Gets the assembly result for water pressure asphalt cover.
        /// </summary>
        public string WaterPressureAsphaltCover
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.WaterPressureAsphaltCover);
            }
        }

        /// <summary>
        /// Gets the assembly result for grass cover erosion outwards.
        /// </summary>
        public string GrassCoverErosionOutwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionOutwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for grass cover slip off outwards.
        /// </summary>
        public string GrassCoverSlipOffOutwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffOutwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for grass cover slip off inwards.
        /// </summary>
        public string GrassCoverSlipOffInwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffInwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for height structures.
        /// </summary>
        public string HeightStructures
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.HeightStructures);
            }
        }

        /// <summary>
        /// Gets the assembly result for closing structures.
        /// </summary>
        public string ClosingStructures
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.ClosingStructures);
            }
        }

        /// <summary>
        /// Gets the assembly result for piping structure.
        /// </summary>
        public string PipingStructure
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.PipingStructure);
            }
        }

        /// <summary>
        /// Gets the assembly result for stability point structures.
        /// </summary>
        public string StabilityPointStructures
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.StabilityPointStructures);
            }
        }

        /// <summary>
        /// Gets the assembly result for strength stability lengthwise construction.
        /// </summary>
        public string StrengthStabilityLengthwiseConstruction
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.StrengthStabilityLengthwiseConstruction);
            }
        }

        /// <summary>
        /// Gets the assembly result for dune erosion.
        /// </summary>
        public string DuneErosion
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.DuneErosion);
            }
        }

        /// <summary>
        /// Gets the assembly result for technical innovation.
        /// </summary>
        public string TechnicalInnovation
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayName(combinedFailureMechanismSectionAssemblyResult.TechnicalInnovation);
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(totalResultIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(pipingIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverErosionInwardsIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(macroStabilityInwardsIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(macroStabilityOutwardsIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
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
            ColumnStateDefinitions.Add(strengthStabilityLengthwiseConstructionIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(duneErosionIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
            ColumnStateDefinitions.Add(technicalInnovationIndex, DataGridViewColumnStateDefinitionFactory.CreateReadOnlyColumnStateDefinition());
        }

        private void SetColumnStateDefinitionColors()
        {
            ColumnStateDefinitions[totalResultIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.TotalResult);
            ColumnStateDefinitions[pipingIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.Piping);
            ColumnStateDefinitions[grassCoverErosionInwardsIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionInwards);
            ColumnStateDefinitions[macroStabilityInwardsIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.MacroStabilityInwards);
            ColumnStateDefinitions[macroStabilityOutwardsIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.MacroStabilityOutwards);
            ColumnStateDefinitions[microstabililityIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.Microstability);
            ColumnStateDefinitions[stabilityStoneCoverIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.StabilityStoneCover);
            ColumnStateDefinitions[waveImpactAsphaltCoverIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.WaveImpactAsphaltCover);
            ColumnStateDefinitions[waterPressureAsphaltCoverIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.WaterPressureAsphaltCover);
            ColumnStateDefinitions[grassCoverErosionOutwardsIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionOutwards);
            ColumnStateDefinitions[grassCoverSlipOffOutwardsIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffOutwards);
            ColumnStateDefinitions[grassCoverSlipOffInwardsIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffInwards);
            ColumnStateDefinitions[heightStructuresIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.HeightStructures);
            ColumnStateDefinitions[closingStructuresIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.ClosingStructures);
            ColumnStateDefinitions[pipingStructureIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.PipingStructure);
            ColumnStateDefinitions[stabilityPointStructuresIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.StabilityPointStructures);
            ColumnStateDefinitions[strengthStabilityLengthwiseConstructionIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.StrengthStabilityLengthwiseConstruction);
            ColumnStateDefinitions[duneErosionIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.DuneErosion);
            ColumnStateDefinitions[technicalInnovationIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.TechnicalInnovation);
        }

        private static CellStyle CreateCellStyle(FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup)
        {
            return new CellStyle(Color.FromKnownColor(KnownColor.ControlText),
                                 AssemblyCategoryGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(assemblyCategoryGroup));
        }
    }
}
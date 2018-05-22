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
using System.Drawing;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Integration.Data.Assembly;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of the <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
    /// </summary>
    public class CombinedFailureMechanismSectionAssemblyResultRow : IHasColumnStateDefinitions
    {
        private const int totalResultIndex = 2;
        private const int pipingIndex = 3;
        private const int grassCoverErosionInwardsIndex = 4;
        private const int macroStabilityInwardsIndex = 5;
        private const int macroStabilityOutwardsIndex = 6;
        private const int microstabililityIndex = 7;
        private const int stabilityStoneCoverIndex = 8;
        private const int waveImpactAsphaltCoverIndex = 9;
        private const int waterPressureAsphaltCoverIndex = 10;
        private const int grassCoverErosionOutwardsIndex = 11;
        private const int grassCoverSlipOffOutwardsIndex = 12;
        private const int grassCoverSlipOffInwardsIndex = 13;
        private const int heightStructuresIndex = 14;
        private const int closingStructuresIndex = 15;
        private const int pipingStructureIndex = 16;
        private const int stabilityPointStructuresIndex = 17;
        private const int strengthStabilityLengthwiseIndex = 18;
        private const int duneErosionIndex = 19;
        private const int technicalInnovationIndex = 20;

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
        /// Gets the start of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public RoundedDouble SectionStart
        {
            get
            {
                return new RoundedDouble(3, combinedFailureMechanismSectionAssemblyResult.SectionStart);
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
                return new RoundedDouble(3, combinedFailureMechanismSectionAssemblyResult.SectionEnd);
            }
        }

        /// <summary>
        /// Gets the total assembly result.
        /// </summary>
        public string TotalResult
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.TotalResult);
            }
        }

        /// <summary>
        /// Gets the assembly result for piping.
        /// </summary>
        public string Piping
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.Piping);
            }
        }

        /// <summary>
        /// Gets the assembly result for grass cover erosion inwards.
        /// </summary>
        public string GrassCoverErosionInwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionInwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for macro stability inwards.
        /// </summary>
        public string MacroStabilityInwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.MacroStabilityInwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for macro stability outwards.
        /// </summary>
        public string MacroStabilityOutwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.MacroStabilityOutwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for microstability.
        /// </summary>
        public string Microstability
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.Microstability);
            }
        }

        /// <summary>
        /// Gets the assembly result for stability stone cover.
        /// </summary>
        public string StabilityStoneCover
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.StabilityStoneCover);
            }
        }

        /// <summary>
        /// Gets the assembly result for wave impact asphalt cover.
        /// </summary>
        public string WaveImpactAsphaltCover
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.WaveImpactAsphaltCover);
            }
        }

        /// <summary>
        /// Gets the assembly result for water pressure asphalt cover.
        /// </summary>
        public string WaterPressureAsphaltCover
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.WaterPressureAsphaltCover);
            }
        }

        /// <summary>
        /// Gets the assembly result for grass cover erosion outwards.
        /// </summary>
        public string GrassCoverErosionOutwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.GrassCoverErosionOutwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for grass cover slip off outwards.
        /// </summary>
        public string GrassCoverSlipOffOutwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffOutwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for grass cover slip off inwards.
        /// </summary>
        public string GrassCoverSlipOffInwards
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.GrassCoverSlipOffInwards);
            }
        }

        /// <summary>
        /// Gets the assembly result for height structures.
        /// </summary>
        public string HeightStructures
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.HeightStructures);
            }
        }

        /// <summary>
        /// Gets the assembly result for closing structures.
        /// </summary>
        public string ClosingStructures
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.ClosingStructures);
            }
        }

        /// <summary>
        /// Gets the assembly result for piping structure.
        /// </summary>
        public string PipingStructure
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.PipingStructure);
            }
        }

        /// <summary>
        /// Gets the assembly result for stability point structures.
        /// </summary>
        public string StabilityPointStructures
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.StabilityPointStructures);
            }
        }

        /// <summary>
        /// Gets the assembly result for strength stability lengthwise.
        /// </summary>
        public string StrengthStabilityLengthwise
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.StrengthStabilityLengthwise);
            }
        }

        /// <summary>
        /// Gets the assembly result for dune erosion.
        /// </summary>
        public string DuneErosion
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.DuneErosion);
            }
        }

        /// <summary>
        /// Gets the assembly result for technical innovation.
        /// </summary>
        public string TechnicalInnovation
        {
            get
            {
                return FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(combinedFailureMechanismSectionAssemblyResult.TechnicalInnovation);
            }
        }

        public IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }

        private void CreateColumnStateDefinitions()
        {
            ColumnStateDefinitions.Add(totalResultIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(pipingIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverErosionInwardsIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(macroStabilityInwardsIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(macroStabilityOutwardsIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(microstabililityIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(stabilityStoneCoverIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(waveImpactAsphaltCoverIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(waterPressureAsphaltCoverIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverErosionOutwardsIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverSlipOffOutwardsIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(grassCoverSlipOffInwardsIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(heightStructuresIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(closingStructuresIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(pipingStructureIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(stabilityPointStructuresIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(strengthStabilityLengthwiseIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(duneErosionIndex, CreateDefaultDataGridViewColumnStateDefinition());
            ColumnStateDefinitions.Add(technicalInnovationIndex, CreateDefaultDataGridViewColumnStateDefinition());
        }

        private static DataGridViewColumnStateDefinition CreateDefaultDataGridViewColumnStateDefinition()
        {
            return new DataGridViewColumnStateDefinition
            {
                ReadOnly = true
            };
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
            ColumnStateDefinitions[strengthStabilityLengthwiseIndex].Style = CreateCellStyle(combinedFailureMechanismSectionAssemblyResult.StrengthStabilityLengthwise);
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
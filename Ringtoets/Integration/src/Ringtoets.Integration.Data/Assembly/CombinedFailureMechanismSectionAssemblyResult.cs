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
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.Integration.Data.Assembly
{
    /// <summary>
    /// Assembly result for the combined failure mechanism section.
    /// </summary>
    public class CombinedFailureMechanismSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="sectionNumber">The number of the section.</param>
        /// <param name="sectionStart">The start of the section from the beginning of the reference line
        /// in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of the reference line
        /// in meters.</param>
        /// <param name="totalResult">The total result of the section.</param>
        /// <param name="properties">The container of the properties for the
        /// <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/>
        /// is <c>null</c>.</exception>
        public CombinedFailureMechanismSectionAssemblyResult(int sectionNumber, double sectionStart, double sectionEnd,
                                                             FailureMechanismSectionAssemblyCategoryGroup totalResult,
                                                             ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            SectionNumber = sectionNumber;
            SectionStart = sectionStart;
            SectionEnd = sectionEnd;
            TotalResult = totalResult;
            Piping = properties.Piping;
            GrassCoverErosionInwards = properties.GrassCoverErosionInwards;
            MacroStabilityInwards = properties.MacroStabilityInwards;
            MacroStabilityOutwards = properties.MacroStabilityOutwards;
            Microstability = properties.Microstability;
            StabilityStoneCover = properties.StabilityStoneCover;
            WaveImpactAsphaltCover = properties.WaveImpactAsphaltCover;
            WaterPressureAsphaltCover = properties.WaterPressureAsphaltCover;
            GrassCoverErosionOutwards = properties.GrassCoverErosionOutwards;
            GrassCoverSlipOffOutwards = properties.GrassCoverSlipOffOutwards;
            GrassCoverSlipOffInwards = properties.GrassCoverSlipOffInwards;
            HeightStructures = properties.HeightStructures;
            ClosingStructures = properties.ClosingStructures;
            PipingStructure = properties.PipingStructure;
            StabilityPointStructures = properties.StabilityPointStructures;
            StrengthStabilityLengthwiseConstruction = properties.StrengthStabilityLengthwiseConstruction;
            DuneErosion = properties.DuneErosion;
            TechnicalInnovation = properties.TechnicalInnovation;
        }

        /// <summary>
        /// Gets the number of the section.
        /// </summary>
        public int SectionNumber { get; }

        /// <summary>
        /// Gets the start of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public double SectionStart { get; }

        /// <summary>
        /// Gets the end of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public double SectionEnd { get; }

        /// <summary>
        /// Gets the total assembly result.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup TotalResult { get; }

        /// <summary>
        /// Gets the assembly result for piping.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup Piping { get; }

        /// <summary>
        /// Gets the assembly result for grass cover erosion inwards.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup GrassCoverErosionInwards { get; }

        /// <summary>
        /// Gets the assembly result for macro stability inwards.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup MacroStabilityInwards { get; }

        /// <summary>
        /// Gets the assembly result for macro stability outwards.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup MacroStabilityOutwards { get; }

        /// <summary>
        /// Gets the assembly result for microstability.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup Microstability { get; }

        /// <summary>
        /// Gets the assembly result for stability stone cover.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup StabilityStoneCover { get; }

        /// <summary>
        /// Gets the assembly result for wave impact asphalt cover.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup WaveImpactAsphaltCover { get; }

        /// <summary>
        /// Gets the assembly result for water pressure asphalt cover.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup WaterPressureAsphaltCover { get; }

        /// <summary>
        /// Gets the assembly result for grass cover erosion outwards.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup GrassCoverErosionOutwards { get; }

        /// <summary>
        /// Gets the assembly result for grass cover slip off outwards.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup GrassCoverSlipOffOutwards { get; }

        /// <summary>
        /// Gets the assembly result for grass cover slip off inwards.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup GrassCoverSlipOffInwards { get; }

        /// <summary>
        /// Gets the assembly result for height structures.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup HeightStructures { get; }

        /// <summary>
        /// Gets the assembly result for closing structures.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup ClosingStructures { get; }

        /// <summary>
        /// Gets the assembly result for piping structure.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup PipingStructure { get; }

        /// <summary>
        /// Gets the assembly result for stability point structures.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup StabilityPointStructures { get; }

        /// <summary>
        /// Gets the assembly result for strength stability lengthwise construction.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup StrengthStabilityLengthwiseConstruction { get; }

        /// <summary>
        /// Gets the assembly result for dune erosion.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup DuneErosion { get; }

        /// <summary>
        /// Gets the assembly result for technical innovation.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup TechnicalInnovation { get; }

        /// <summary>
        /// Container for properties for constructing a <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>s
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                Piping = FailureMechanismSectionAssemblyCategoryGroup.None;
                GrassCoverErosionInwards = FailureMechanismSectionAssemblyCategoryGroup.None;
                MacroStabilityInwards = FailureMechanismSectionAssemblyCategoryGroup.None;
                MacroStabilityOutwards = FailureMechanismSectionAssemblyCategoryGroup.None;
                Microstability = FailureMechanismSectionAssemblyCategoryGroup.None;
                StabilityStoneCover = FailureMechanismSectionAssemblyCategoryGroup.None;
                WaveImpactAsphaltCover = FailureMechanismSectionAssemblyCategoryGroup.None;
                WaterPressureAsphaltCover = FailureMechanismSectionAssemblyCategoryGroup.None;
                GrassCoverErosionOutwards = FailureMechanismSectionAssemblyCategoryGroup.None;
                GrassCoverSlipOffOutwards = FailureMechanismSectionAssemblyCategoryGroup.None;
                GrassCoverSlipOffInwards = FailureMechanismSectionAssemblyCategoryGroup.None;
                HeightStructures = FailureMechanismSectionAssemblyCategoryGroup.None;
                ClosingStructures = FailureMechanismSectionAssemblyCategoryGroup.None;
                PipingStructure = FailureMechanismSectionAssemblyCategoryGroup.None;
                StabilityPointStructures = FailureMechanismSectionAssemblyCategoryGroup.None;
                StrengthStabilityLengthwiseConstruction = FailureMechanismSectionAssemblyCategoryGroup.None;
                DuneErosion = FailureMechanismSectionAssemblyCategoryGroup.None;
                TechnicalInnovation = FailureMechanismSectionAssemblyCategoryGroup.None;
            }

            /// <summary>
            /// Gets or sets the assembly result for piping.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup Piping { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover erosion inwards.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup GrassCoverErosionInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for macro stability inwards.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup MacroStabilityInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for macro stability outwards.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup MacroStabilityOutwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for microstability.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup Microstability { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for stability stone cover.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup StabilityStoneCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for wave impact asphalt cover.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup WaveImpactAsphaltCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for water pressure asphalt cover.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup WaterPressureAsphaltCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover erosion outwards.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup GrassCoverErosionOutwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover slip off outwards.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup GrassCoverSlipOffOutwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover slip off inwards.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup GrassCoverSlipOffInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for height structures.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup HeightStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for closing structures.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup ClosingStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for piping structure.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup PipingStructure { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for stability point structures.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup StabilityPointStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for strength stability lengthwise construction.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup StrengthStabilityLengthwiseConstruction { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for dune erosion.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup DuneErosion { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for technical innovation.
            /// </summary>
            public FailureMechanismSectionAssemblyCategoryGroup TechnicalInnovation { internal get; set; }
        }
    }
}
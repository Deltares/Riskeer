// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Integration.Data.Assembly
{
    /// <summary>
    /// Assembly result for the combined failure mechanism section.
    /// </summary>
    public class CombinedFailureMechanismSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="sectionStart">The start of the section from the beginning of the reference line
        /// in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of the reference line
        /// in meters.</param>
        /// <param name="totalResult">The total result of the section.</param>
        /// <param name="commonSectionAssemblyMethod">The <see cref="AssemblyMethod"/>
        /// that is used to get the common sections.</param>
        /// <param name="failureMechanismResultsAssemblyMethod">The <see cref="AssemblyMethod"/>
        /// that is used to assemble the failure mechanism results.</param>
        /// <param name="combinedSectionResultAssemblyMethod">The <see cref="AssemblyMethod"/>
        /// that is used to assemble the combined section results.</param>
        /// <param name="properties">The container of the properties for the
        /// <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/>
        /// is <c>null</c>.</exception>
        public CombinedFailureMechanismSectionAssemblyResult(double sectionStart, double sectionEnd,
                                                             FailureMechanismSectionAssemblyGroup totalResult,
                                                             AssemblyMethod commonSectionAssemblyMethod,
                                                             AssemblyMethod failureMechanismResultsAssemblyMethod,
                                                             AssemblyMethod combinedSectionResultAssemblyMethod,
                                                             ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            SectionStart = sectionStart;
            SectionEnd = sectionEnd;
            Piping = properties.Piping;
            GrassCoverErosionInwards = properties.GrassCoverErosionInwards;
            MacroStabilityInwards = properties.MacroStabilityInwards;
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
            DuneErosion = properties.DuneErosion;
            SpecificFailureMechanisms = properties.SpecificFailureMechanisms;
            TotalResult = totalResult;

            CommonSectionAssemblyMethod = commonSectionAssemblyMethod;
            FailureMechanismResultsAssemblyMethod = failureMechanismResultsAssemblyMethod;
            CombinedSectionResultAssemblyMethod = combinedSectionResultAssemblyMethod;
        }

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
        /// Gets the assembly result for piping or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? Piping { get; }

        /// <summary>
        /// Gets the assembly result for grass cover erosion inwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? GrassCoverErosionInwards { get; }

        /// <summary>
        /// Gets the assembly result for macro stability inwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? MacroStabilityInwards { get; }

        /// <summary>
        /// Gets the assembly result for microstability or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? Microstability { get; }

        /// <summary>
        /// Gets the assembly result for stability stone cover or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? StabilityStoneCover { get; }

        /// <summary>
        /// Gets the assembly result for wave impact asphalt cover or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? WaveImpactAsphaltCover { get; }

        /// <summary>
        /// Gets the assembly result for water pressure asphalt cover or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? WaterPressureAsphaltCover { get; }

        /// <summary>
        /// Gets the assembly result for grass cover erosion outwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? GrassCoverErosionOutwards { get; }

        /// <summary>
        /// Gets the assembly result for grass cover slip off outwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? GrassCoverSlipOffOutwards { get; }

        /// <summary>
        /// Gets the assembly result for grass cover slip off inwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? GrassCoverSlipOffInwards { get; }

        /// <summary>
        /// Gets the assembly result for height structures or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? HeightStructures { get; }

        /// <summary>
        /// Gets the assembly result for closing structures or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? ClosingStructures { get; }

        /// <summary>
        /// Gets the assembly result for piping structure or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? PipingStructure { get; }

        /// <summary>
        /// Gets the assembly result for stability point structures or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? StabilityPointStructures { get; }

        /// <summary>
        /// Gets the assembly result for dune erosion or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
        /// </summary>
        public FailureMechanismSectionAssemblyGroup? DuneErosion { get; }

        /// <summary>
        /// Gets the collection of assembly results for specific failure mechanisms.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup?[] SpecificFailureMechanisms { get; }

        /// <summary>
        /// Gets the total assembly result.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup TotalResult { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to get the common sections.
        /// </summary> 
        public AssemblyMethod CommonSectionAssemblyMethod { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to assemble the failure mechanism results.
        /// </summary>
        public AssemblyMethod FailureMechanismResultsAssemblyMethod { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to assemble the combined section results.
        /// </summary>
        public AssemblyMethod CombinedSectionResultAssemblyMethod { get; }

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
                Piping = FailureMechanismSectionAssemblyGroup.Gr;
                GrassCoverErosionInwards = FailureMechanismSectionAssemblyGroup.Gr;
                MacroStabilityInwards = FailureMechanismSectionAssemblyGroup.Gr;
                Microstability = FailureMechanismSectionAssemblyGroup.Gr;
                StabilityStoneCover = FailureMechanismSectionAssemblyGroup.Gr;
                WaveImpactAsphaltCover = FailureMechanismSectionAssemblyGroup.Gr;
                WaterPressureAsphaltCover = FailureMechanismSectionAssemblyGroup.Gr;
                GrassCoverErosionOutwards = FailureMechanismSectionAssemblyGroup.Gr;
                GrassCoverSlipOffOutwards = FailureMechanismSectionAssemblyGroup.Gr;
                GrassCoverSlipOffInwards = FailureMechanismSectionAssemblyGroup.Gr;
                HeightStructures = FailureMechanismSectionAssemblyGroup.Gr;
                ClosingStructures = FailureMechanismSectionAssemblyGroup.Gr;
                PipingStructure = FailureMechanismSectionAssemblyGroup.Gr;
                StabilityPointStructures = FailureMechanismSectionAssemblyGroup.Gr;
                DuneErosion = FailureMechanismSectionAssemblyGroup.Gr;
                SpecificFailureMechanisms = Array.Empty<FailureMechanismSectionAssemblyGroup?>();
            }

            /// <summary>
            /// Gets or sets the assembly result for piping or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? Piping { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover erosion inwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? GrassCoverErosionInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for macro stability inwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? MacroStabilityInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for microstability or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? Microstability { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for stability stone cover or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? StabilityStoneCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for wave impact asphalt cover or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? WaveImpactAsphaltCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for water pressure asphalt cover or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? WaterPressureAsphaltCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover erosion outwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? GrassCoverErosionOutwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover slip off outwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? GrassCoverSlipOffOutwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover slip off inwards or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? GrassCoverSlipOffInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for height structures or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? HeightStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for closing structures or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? ClosingStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for piping structure or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? PipingStructure { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for stability point structures or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? StabilityPointStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for dune erosion or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup? DuneErosion { internal get; set; }

            /// <summary>
            /// Gets or sets the collection of assembly results for specific failure mechanisms or <c>null</c> (which indicates this failure mechanism is not part of the assembly).
            /// </summary>
            public FailureMechanismSectionAssemblyGroup?[] SpecificFailureMechanisms { internal get; set; }
        }
    }
}
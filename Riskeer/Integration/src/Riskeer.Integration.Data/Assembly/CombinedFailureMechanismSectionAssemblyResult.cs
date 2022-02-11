﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
                                                             FailureMechanismSectionAssemblyGroup totalResult,
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
        public FailureMechanismSectionAssemblyGroup TotalResult { get; }

        /// <summary>
        /// Gets the assembly result for piping.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup Piping { get; }

        /// <summary>
        /// Gets the assembly result for grass cover erosion inwards.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup GrassCoverErosionInwards { get; }

        /// <summary>
        /// Gets the assembly result for macro stability inwards.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup MacroStabilityInwards { get; }

        /// <summary>
        /// Gets the assembly result for microstability.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup Microstability { get; }

        /// <summary>
        /// Gets the assembly result for stability stone cover.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup StabilityStoneCover { get; }

        /// <summary>
        /// Gets the assembly result for wave impact asphalt cover.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup WaveImpactAsphaltCover { get; }

        /// <summary>
        /// Gets the assembly result for water pressure asphalt cover.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup WaterPressureAsphaltCover { get; }

        /// <summary>
        /// Gets the assembly result for grass cover erosion outwards.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup GrassCoverErosionOutwards { get; }

        /// <summary>
        /// Gets the assembly result for grass cover slip off outwards.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup GrassCoverSlipOffOutwards { get; }

        /// <summary>
        /// Gets the assembly result for grass cover slip off inwards.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup GrassCoverSlipOffInwards { get; }

        /// <summary>
        /// Gets the assembly result for height structures.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup HeightStructures { get; }

        /// <summary>
        /// Gets the assembly result for closing structures.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup ClosingStructures { get; }

        /// <summary>
        /// Gets the assembly result for piping structure.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup PipingStructure { get; }

        /// <summary>
        /// Gets the assembly result for stability point structures.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup StabilityPointStructures { get; }

        /// <summary>
        /// Gets the assembly result for dune erosion.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup DuneErosion { get; }

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
            }

            /// <summary>
            /// Gets or sets the assembly result for piping.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup Piping { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover erosion inwards.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup GrassCoverErosionInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for macro stability inwards.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup MacroStabilityInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for microstability.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup Microstability { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for stability stone cover.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup StabilityStoneCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for wave impact asphalt cover.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup WaveImpactAsphaltCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for water pressure asphalt cover.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup WaterPressureAsphaltCover { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover erosion outwards.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup GrassCoverErosionOutwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover slip off outwards.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup GrassCoverSlipOffOutwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for grass cover slip off inwards.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup GrassCoverSlipOffInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for height structures.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup HeightStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for closing structures.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup ClosingStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for piping structure.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup PipingStructure { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for stability point structures.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup StabilityPointStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the assembly result for dune erosion.
            /// </summary>
            public FailureMechanismSectionAssemblyGroup DuneErosion { internal get; set; }
        }
    }
}
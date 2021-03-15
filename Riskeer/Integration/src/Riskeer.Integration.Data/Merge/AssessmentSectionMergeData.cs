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

namespace Riskeer.Integration.Data.Merge
{
    /// <summary>
    /// Class that holds the merge data.
    /// </summary>
    public class AssessmentSectionMergeData
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionMergeData"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to merge.</param>
        /// <param name="properties">The container of the properties for the
        /// <see cref="AssessmentSectionMergeData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionMergeData(AssessmentSection assessmentSection,
                                          ConstructionProperties properties)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            AssessmentSection = assessmentSection;

            MergePiping = properties.MergePiping;
            MergeGrassCoverErosionInwards = properties.MergeGrassCoverErosionInwards;
            MergeMacroStabilityInwards = properties.MergeMacroStabilityInwards;
            MergeStabilityStoneCover = properties.MergeStabilityStoneCover;
            MergeWaveImpactAsphaltCover = properties.MergeWaveImpactAsphaltCover;
            MergeGrassCoverErosionOutwards = properties.MergeGrassCoverErosionOutwards;
            MergeHeightStructures = properties.MergeHeightStructures;
            MergeClosingStructures = properties.MergeClosingStructures;
            MergeStabilityPointStructures = properties.MergeStabilityPointStructures;
            MergeDuneErosion = properties.MergeDuneErosion;
        }

        /// <summary>
        /// Gets the assessment section to merge.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the indicator whether piping should be merged.
        /// </summary>
        public bool MergePiping { get; }

        /// <summary>
        /// Gets the indicator whether grass cover erosion inwards should be merged.
        /// </summary>
        public bool MergeGrassCoverErosionInwards { get; }

        /// <summary>
        /// Gets the indicator whether macro stability inwards should be merged.
        /// </summary>
        public bool MergeMacroStabilityInwards { get; }

        /// <summary>
        /// Gets the indicator whether stability stone cover should be merged.
        /// </summary>
        public bool MergeStabilityStoneCover { get; }

        /// <summary>
        /// Gets the indicator whether wave impact asphalt cover should be merged.
        /// </summary>
        public bool MergeWaveImpactAsphaltCover { get; }

        /// <summary>
        /// Gets the indicator whether grass cover erosion outwards should be merged.
        /// </summary>
        public bool MergeGrassCoverErosionOutwards { get; }

        /// <summary>
        /// Gets the indicator whether height structures should be merged.
        /// </summary>
        public bool MergeHeightStructures { get; }

        /// <summary>
        /// Gets the indicator whether closing structures should be merged.
        /// </summary>
        public bool MergeClosingStructures { get; }

        /// <summary>
        /// Gets the indicator whether stability point structures should be merged.
        /// </summary>
        public bool MergeStabilityPointStructures { get; }

        /// <summary>
        /// Gets the indicator whether dune erosion should be merged.
        /// </summary>
        public bool MergeDuneErosion { get; }

        /// <summary>
        /// Container for properties for constructing an <see cref="AssessmentSectionMergeData"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the indicator whether piping should be merged.
            /// </summary>
            public bool MergePiping { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether grass cover erosion inwards should be merged.
            /// </summary>
            public bool MergeGrassCoverErosionInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether macro stability inwards should be merged.
            /// </summary>
            public bool MergeMacroStabilityInwards { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether stability stone cover should be merged.
            /// </summary>
            public bool MergeStabilityStoneCover { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether wave impact asphalt cover should be merged.
            /// </summary>
            public bool MergeWaveImpactAsphaltCover { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether grass cover erosion outwards should be merged.
            /// </summary>
            public bool MergeGrassCoverErosionOutwards { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether height structures should be merged.
            /// </summary>
            public bool MergeHeightStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether closing structures should be merged.
            /// </summary>
            public bool MergeClosingStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether stability point structures should be merged.
            /// </summary>
            public bool MergeStabilityPointStructures { internal get; set; }

            /// <summary>
            /// Gets or sets the indicator whether dune erosion should be merged.
            /// </summary>
            public bool MergeDuneErosion { internal get; set; }
        }
    }
}
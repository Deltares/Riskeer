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

using Riskeer.Common.Primitives;

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Class that contains the data that is necessary to determine the failure mechanism section assembly. 
    /// </summary>
    public class FailureMechanismSectionAssemblyInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyInput"/>.
        /// </summary>
        /// <param name="lowerLimitNorm">The lower limit norm.</param>
        /// <param name="signalingNorm">The signaling norm.</param>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="hasProbabilitySpecified">Indicator whether the section has a probability specified.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisNeeded">The indicator whether the section needs further analysis.</param>
        /// <param name="furtherAnalysisType">The <see cref="FailureMechanismSectionResultFurtherAnalysisType"/>.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        public FailureMechanismSectionAssemblyInput(double lowerLimitNorm, double signalingNorm,
                                                    bool isRelevant, bool hasProbabilitySpecified,
                                                    double initialSectionProbability, bool furtherAnalysisNeeded,
                                                    FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
                                                    double refinedSectionProbability)
        {
            LowerLimitNorm = lowerLimitNorm;
            SignalingNorm = signalingNorm;

            IsRelevant = isRelevant;
            HasProbabilitySpecified = hasProbabilitySpecified;
            InitialSectionProbability = initialSectionProbability;
            FurtherAnalysisNeeded = furtherAnalysisNeeded;
            FurtherAnalysisType = furtherAnalysisType;
            RefinedSectionProbability = refinedSectionProbability;
        }

        /// <summary>
        /// Gets the lower limit norm.
        /// </summary>
        public double LowerLimitNorm { get; }

        /// <summary>
        /// Gets the signaling norm.
        /// </summary>
        public double SignalingNorm { get; }

        /// <summary>
        /// Gets the indicator whether the section is relevant.
        /// </summary>
        public bool IsRelevant { get; }

        /// <summary>
        /// Gets the indicator whether the section has a probability specified.
        /// </summary>
        public bool HasProbabilitySpecified { get; }

        /// <summary>
        /// Gets the probability for the section.
        /// </summary>
        public double InitialSectionProbability { get; }

        /// <summary>
        /// Gets the indicator whether the section needs refinement.
        /// </summary>
        public bool FurtherAnalysisNeeded { get; }
        
        /// <summary>
        /// Gets the <see cref="FailureMechanismSectionResultFurtherAnalysisType"/>.
        /// </summary>
        public FailureMechanismSectionResultFurtherAnalysisType FurtherAnalysisType { get; }

        /// <summary>
        /// Gets the refined probability for the section.
        /// </summary>
        public double RefinedSectionProbability { get; }
    }
}
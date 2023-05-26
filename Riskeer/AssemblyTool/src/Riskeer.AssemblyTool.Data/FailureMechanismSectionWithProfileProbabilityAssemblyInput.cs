// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    /// Class that contains the data that is necessary to determine
    /// the failure mechanism section assembly with profile probabilities. 
    /// </summary>
    public class FailureMechanismSectionWithProfileProbabilityAssemblyInput : FailureMechanismSectionAssemblyInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionWithProfileProbabilityAssemblyInput"/>.
        /// </summary>
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability.</param>
        /// <param name="signalFloodingProbability">The signal flooding probability.</param>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="hasProbabilitySpecified">Indicator whether the section has a probability specified.</param>
        /// <param name="initialProfileProbability">The initial probability for the profile.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisType">The <see cref="FailureMechanismSectionResultFurtherAnalysisType"/>.</param>
        /// <param name="refinedProfileProbability">The refined probability for the profile.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        public FailureMechanismSectionWithProfileProbabilityAssemblyInput(double maximumAllowableFloodingProbability, double signalFloodingProbability,
                                                                          bool isRelevant, bool hasProbabilitySpecified,
                                                                          double initialProfileProbability, double initialSectionProbability,
                                                                          FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
                                                                          double refinedProfileProbability, double refinedSectionProbability)
            : base(maximumAllowableFloodingProbability, signalFloodingProbability, isRelevant, hasProbabilitySpecified, initialSectionProbability, furtherAnalysisType, refinedSectionProbability)
        {
            InitialProfileProbability = initialProfileProbability;
            RefinedProfileProbability = refinedProfileProbability;
        }

        /// <summary>
        /// Gets the probability for the profile.
        /// </summary>
        public double InitialProfileProbability { get; }

        /// <summary>
        /// Gets the refined probability for the profile.
        /// </summary>
        public double RefinedProfileProbability { get; }
    }
}
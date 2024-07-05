// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability.</param>
        /// <param name="signalFloodingProbability">The signal flooding probability.</param>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="hasProbabilitySpecified">Indicator whether the section has a probability specified.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisType">The <see cref="FailureMechanismSectionResultFurtherAnalysisType"/>.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        public FailureMechanismSectionAssemblyInput(double maximumAllowableFloodingProbability, double signalFloodingProbability,
                                                    bool isRelevant, bool hasProbabilitySpecified,
                                                    double initialSectionProbability,
                                                    FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
                                                    double refinedSectionProbability)
        {
            MaximumAllowableFloodingProbability = maximumAllowableFloodingProbability;
            SignalFloodingProbability = signalFloodingProbability;

            IsRelevant = isRelevant;
            HasProbabilitySpecified = hasProbabilitySpecified;
            InitialSectionProbability = initialSectionProbability;
            FurtherAnalysisType = furtherAnalysisType;
            RefinedSectionProbability = refinedSectionProbability;
        }

        /// <summary>
        /// Gets the maximum allowable flooding probability.
        /// </summary>
        public double MaximumAllowableFloodingProbability { get; }

        /// <summary>
        /// Gets the signal flooding probability.
        /// </summary>
        public double SignalFloodingProbability { get; }

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
        /// Gets the <see cref="FailureMechanismSectionResultFurtherAnalysisType"/>.
        /// </summary>
        public FailureMechanismSectionResultFurtherAnalysisType FurtherAnalysisType { get; }

        /// <summary>
        /// Gets the refined probability for the section.
        /// </summary>
        public double RefinedSectionProbability { get; }
    }
}
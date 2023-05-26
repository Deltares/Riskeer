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

using System;
using System.Linq;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Factory to create instances of <see cref="IFailureMechanismSectionResultCalculateProbabilityStrategy"/> for section results
    /// of the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public static class PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactory
    {
        /// <summary>
        /// Creates a <see cref="IFailureMechanismSectionResultCalculateProbabilityStrategy"/> based on the input arguments.
        /// </summary>
        /// <param name="sectionResult">The section result to create the strategy for.</param>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> to create the strategy with.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to create the strategy with.</param>
        /// <returns>An <see cref="IFailureMechanismSectionResultCalculateProbabilityStrategy"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IFailureMechanismSectionResultCalculateProbabilityStrategy CreateCalculateStrategy(AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                                                         PipingFailureMechanism failureMechanism,
                                                                                                         IAssessmentSection assessmentSection)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            bool scenarioConfigurationTypeIsSemiProbabilistic = failureMechanism.ScenarioConfigurationTypeIsSemiProbabilistic(
                failureMechanism.GetScenarioConfigurationForSection(sectionResult));

            return scenarioConfigurationTypeIsSemiProbabilistic
                       ? (IFailureMechanismSectionResultCalculateProbabilityStrategy) CreateSemiProbabilisticCalculateStrategy(failureMechanism, sectionResult, assessmentSection)
                       : CreateProbabilisticCalculateStrategy(failureMechanism, sectionResult);
        }

        private static ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy CreateProbabilisticCalculateStrategy(
            PipingFailureMechanism failureMechanism,
            AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            return new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, failureMechanism.Calculations.OfType<ProbabilisticPipingCalculationScenario>());
        }

        private static SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy CreateSemiProbabilisticCalculateStrategy(
            PipingFailureMechanism failureMechanism,
            AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
            IAssessmentSection assessmentSection)
        {
            return new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, failureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>(),
                failureMechanism, assessmentSection);
        }
    }
}
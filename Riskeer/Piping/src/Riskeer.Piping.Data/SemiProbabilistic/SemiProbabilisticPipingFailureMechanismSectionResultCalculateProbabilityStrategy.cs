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
using System.Collections.Generic;
using System.Linq;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;

namespace Riskeer.Piping.Data.SemiProbabilistic
{
    /// <summary>
    /// Strategy to calculate probabilities for <see cref="SemiProbabilisticPipingCalculationScenario"/>.
    /// </summary>
    public class SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy : IFailureMechanismSectionResultCalculateProbabilityStrategy
    {
        private readonly AdoptableFailureMechanismSectionResult sectionResult;
        private readonly IEnumerable<SemiProbabilisticPipingCalculationScenario> calculationScenarios;
        private readonly PipingFailureMechanism failureMechanism;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="AdoptableFailureMechanismSectionResult"/> to get the probabilities for.</param>
        /// <param name="calculationScenarios">All the <see cref="SemiProbabilisticPipingCalculationScenario"/> of the failure mechanism. </param>
        /// <param name="failureMechanism">The failure mechanism the calculation scenarios belong to.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                IEnumerable<SemiProbabilisticPipingCalculationScenario> calculationScenarios,
                                                                                                PipingFailureMechanism failureMechanism,
                                                                                                IAssessmentSection assessmentSection)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.sectionResult = sectionResult;
            this.calculationScenarios = calculationScenarios;
            this.failureMechanism = failureMechanism;
            this.assessmentSection = assessmentSection;
        }

        public double CalculateSectionProbability()
        {
            double probability = sectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios,
                                                                                           assessmentSection.FailureMechanismContribution.NormativeProbability);

            PipingScenarioConfigurationPerFailureMechanismSection sectionConfiguration = 
                failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.Single(c => ReferenceEquals(c.Section, sectionResult.Section));
            
            return Math.Min(1.0, probability * sectionConfiguration.GetN(failureMechanism.ProbabilityAssessmentInput.B));
        }
    }
}
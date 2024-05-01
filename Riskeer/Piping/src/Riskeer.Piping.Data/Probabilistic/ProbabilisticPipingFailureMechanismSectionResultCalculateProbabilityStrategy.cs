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
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Piping.Data.Probabilistic
{
    /// <summary>
    /// Strategy to calculate probabilities for <see cref="ProbabilisticPipingCalculationScenario"/>.
    /// </summary>
    public class ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy : IFailureMechanismSectionResultCalculateProbabilityStrategy
    {
        private readonly AdoptableFailureMechanismSectionResult sectionResult;
        private readonly IEnumerable<ProbabilisticPipingCalculationScenario> calculationScenarios;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="AdoptableFailureMechanismSectionResult"/> to get the probabilities for.</param>
        /// <param name="calculationScenarios">All the <see cref="ProbabilisticPipingCalculationScenario"/> of the failure mechanism. </param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                            IEnumerable<ProbabilisticPipingCalculationScenario> calculationScenarios)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            this.sectionResult = sectionResult;
            this.calculationScenarios = calculationScenarios;
        }

        public double CalculateProfileProbability()
        {
            return sectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios, scenario => scenario.Output.ProfileSpecificOutput);
        }

        public double CalculateSectionProbability()
        {
            return sectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios, scenario => scenario.Output.SectionSpecificOutput);
        }
    }
}
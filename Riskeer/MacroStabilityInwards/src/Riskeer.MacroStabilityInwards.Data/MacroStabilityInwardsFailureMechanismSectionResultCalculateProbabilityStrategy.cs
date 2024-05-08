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
using Riskeer.Common.Data.Probability;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// Strategy to calculate probabilities for <see cref="MacroStabilityInwardsCalculationScenario"/>.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy : IFailureMechanismSectionResultCalculateProbabilityStrategy
    {
        private readonly AdoptableFailureMechanismSectionResult sectionResult;
        private readonly IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios;
        private readonly MacroStabilityInwardsFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="AdoptableFailureMechanismSectionResult"/> to get the probabilities for.</param>
        /// <param name="calculationScenarios">All the <see cref="MacroStabilityInwardsCalculationScenario"/> of the failure mechanism. </param>
        /// <param name="failureMechanism">The failure mechanism the calculation scenarios belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
            AdoptableFailureMechanismSectionResult sectionResult,
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios,
            MacroStabilityInwardsFailureMechanism failureMechanism)
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

            this.sectionResult = sectionResult;
            this.calculationScenarios = calculationScenarios;
            this.failureMechanism = failureMechanism;
        }

        public double CalculateSectionProbability()
        {
            double probability = sectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios,
                                                                                           failureMechanism.GeneralInput.ModelFactor);
            return Math.Min(1.0, probability * failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.GetN(sectionResult.Section.Length));
        }
    }
}
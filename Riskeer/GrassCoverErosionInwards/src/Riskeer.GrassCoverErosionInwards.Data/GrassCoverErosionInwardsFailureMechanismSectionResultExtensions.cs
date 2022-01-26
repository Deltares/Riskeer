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
using System.Collections.Generic;
using System.Linq;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Helpers;
using Riskeer.Common.Data.Probability;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Extension methods for obtaining probabilities for a section result
    /// of the grass cover erosion inwards failure mechanism.
    /// </summary>
    public static class GrassCoverErosionInwardsFailureMechanismSectionResultExtensions
    {
        /// <summary>
        /// Gets the value for the initial failure mechanism result per failure mechanism section as a probability.
        /// </summary>
        /// <param name="sectionResult">The section result to get the initial failure mechanism result probability for.</param>
        /// <param name="calculationScenarios">All probabilistic calculation scenarios in the failure mechanism.</param>
        /// <returns>The calculated initial failure mechanism result probability; or <see cref="double.NaN"/> when there
        /// are no relevant calculations, when not all relevant calculations are performed or when the
        /// contributions of the relevant calculations don't add up to 1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double GetInitialFailureMechanismResultProbability(this AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                         IEnumerable<GrassCoverErosionInwardsCalculationScenario> calculationScenarios)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            GrassCoverErosionInwardsCalculationScenario[] relevantScenarios = sectionResult.GetRelevantCalculationScenarios<GrassCoverErosionInwardsCalculationScenario>(
                                                                                               calculationScenarios,
                                                                                               (scenario, lineSegments) => scenario.IsDikeProfileIntersectionWithReferenceLineInSection(lineSegments))
                                                                                           .ToArray();

            if (!CalculationScenarioHelper.ScenariosAreValid(relevantScenarios))
            {
                return double.NaN;
            }

            double totalInitialFailureMechanismResult = 0;
            foreach (GrassCoverErosionInwardsCalculationScenario scenario in relevantScenarios)
            {
                ProbabilityAssessmentOutput derivedOutput = ProbabilityAssessmentOutputFactory.Create(scenario.Output.OvertoppingOutput.Reliability);
                totalInitialFailureMechanismResult += derivedOutput.Probability * (double) scenario.Contribution;
            }

            return totalInitialFailureMechanismResult;
        }
    }
}
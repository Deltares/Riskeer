// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// Extension methods for obtaining detailed assessment probabilities from output for an assessment of 
    /// the macro stability inwards failure mechanism.
    /// </summary>
    public static class MacroStabilityInwardsFailureMechanismSectionResultDetailedAssessmentExtensions
    {
        /// <summary>
        /// Gets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        /// <param name="sectionResult">The section result to get the detailed assessment probability for.</param>
        /// <param name="calculationScenarios">All calculation scenarios in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism the calculations belong to.</param>
        /// <param name="assessmentSection">The assessment section the calculations belong to.</param>
        /// <returns>The calculated detailed assessment probability; or <see cref="double.NaN"/> when there are no
        /// performed or relevant calculations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double GetDetailedAssessmentProbability(this MacroStabilityInwardsFailureMechanismSectionResult sectionResult,
                                                              IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios,
                                                              MacroStabilityInwardsFailureMechanism failureMechanism,
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

            MacroStabilityInwardsCalculationScenario[] relevantScenarios = sectionResult.GetCalculationScenarios(calculationScenarios).ToArray();

            if (relevantScenarios.Length == 0 || !relevantScenarios.All(s => s.HasOutput) || Math.Abs(sectionResult.GetTotalContribution(relevantScenarios) - 1.0) > 1e-6)
            {
                return double.NaN;
            }

            double totalDetailedAssessmentProbability = 0;
            foreach (MacroStabilityInwardsCalculationScenario scenario in relevantScenarios)
            {
                DerivedMacroStabilityInwardsOutput derivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(scenario.Output,
                                                                                                                    failureMechanism,
                                                                                                                    assessmentSection);

                totalDetailedAssessmentProbability += derivedOutput.MacroStabilityInwardsProbability * (double) scenario.Contribution;
            }

            return totalDetailedAssessmentProbability;
        }

        /// <summary>
        /// Gets the total contribution of all relevant calculation scenarios.
        /// </summary>
        /// <param name="sectionResult">The section result to get the total contribution for.</param>
        /// <param name="calculationScenarios">The calculation scenarios to get the total contribution for.</param>
        /// <returns>The total contribution of all relevant calculation scenarios.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static RoundedDouble GetTotalContribution(this MacroStabilityInwardsFailureMechanismSectionResult sectionResult,
                                                         IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            return (RoundedDouble) sectionResult
                                   .GetCalculationScenarios(calculationScenarios)
                                   .Aggregate<ICalculationScenario, double>(0, (current, calculationScenario) => current + calculationScenario.Contribution);
        }

        /// <summary>
        /// Gets a collection of the relevant <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        /// <param name="sectionResult">The section result to get the relevant scenarios for.</param>
        /// <param name="calculationScenarios">The calculation scenarios to get the relevant scenarios from.</param>
        /// <returns>A collection of relevant calculation scenarios.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<MacroStabilityInwardsCalculationScenario> GetCalculationScenarios(
            this MacroStabilityInwardsFailureMechanismSectionResult sectionResult,
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculationScenarios)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(sectionResult.Section.Points);

            return calculationScenarios
                .Where(pc => pc.IsRelevant && pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }
    }
}
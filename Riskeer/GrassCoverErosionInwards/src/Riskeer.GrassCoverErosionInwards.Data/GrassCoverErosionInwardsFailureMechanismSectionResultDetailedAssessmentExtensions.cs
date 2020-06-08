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
using Riskeer.Common.Data.Probability;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Extension methods for obtaining detailed assessment probabilities from output for an assessment of the 
    /// grass cover erosion inwards failure mechanism.
    /// </summary>
    public static class GrassCoverErosionInwardsFailureMechanismSectionResultDetailedAssessmentExtensions
    {
        /// <summary>
        /// Gets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        /// <param name="sectionResult">The section result to get the detailed assessment probability for.</param>
        /// <param name="calculationScenarios">All calculation scenarios in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism the section result belongs to.</param>
        /// <param name="assessmentSection">The assessment section the section result belongs to.</param>
        /// <returns>The calculated detailed assessment probability or <see cref="double.NaN"/> when there is no
        /// calculation assigned to the section result or the calculation is not performed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double GetDetailedAssessmentProbability(this GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult,
                                                              IEnumerable<GrassCoverErosionInwardsCalculationScenario> calculationScenarios,
                                                              GrassCoverErosionInwardsFailureMechanism failureMechanism,
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

            GrassCoverErosionInwardsCalculationScenario[] relevantScenarios = sectionResult.GetCalculationScenarios(calculationScenarios).ToArray();

            if (relevantScenarios.Length == 0 || !relevantScenarios.All(s => s.HasOutput) || Math.Abs(sectionResult.GetTotalContribution(relevantScenarios) - 1.0) > 1e-6)
            {
                return double.NaN;
            }

            double totalDetailedAssessmentProbability = 0;
            foreach (GrassCoverErosionInwardsCalculationScenario scenario in relevantScenarios)
            {
                ProbabilityAssessmentOutput derivedOutput = GrassCoverErosionInwardsProbabilityAssessmentOutputFactory.Create(scenario.Output.OvertoppingOutput,
                                                                                                                              failureMechanism, assessmentSection);

                totalDetailedAssessmentProbability += derivedOutput.Probability * (double) scenario.Contribution;
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
        public static RoundedDouble GetTotalContribution(this GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult,
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

            return (RoundedDouble) sectionResult
                                   .GetCalculationScenarios(calculationScenarios)
                                   .Aggregate<ICalculationScenario, double>(0, (current, calculationScenario) => current + calculationScenario.Contribution);
        }

        /// <summary>
        /// Gets a collection of the relevant <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        /// <param name="sectionResult">The section result to get the relevant scenarios for.</param>
        /// <param name="calculationScenarios">The calculation scenarios to get the relevant scenarios from.</param>
        /// <returns>A collection of relevant calculation scenarios.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<GrassCoverErosionInwardsCalculationScenario> GetCalculationScenarios(
            this GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult,
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

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(sectionResult.Section.Points);

            return calculationScenarios
                .Where(cs => cs.IsRelevant && cs.IsDikeProfileIntersectionWithReferenceLineInSection(lineSegments));
        }
    }
}
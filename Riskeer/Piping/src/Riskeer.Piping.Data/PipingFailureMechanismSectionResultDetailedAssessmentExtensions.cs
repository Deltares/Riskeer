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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Util;
using Riskeer.Common.Data.Calculation;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Extension methods for obtaining detailed assessment probabilities from output for an assessment of the piping failure mechanism.
    /// </summary>
    public static class PipingFailureMechanismSectionResultDetailedAssessmentExtensions
    {
        /// <summary>
        /// Gets the value for the initial failure mechanism result of safety per failure mechanism section as a probability.
        /// </summary>
        /// <param name="sectionResult">The section result to get the detailed assessment probability for.</param>
        /// <param name="calculationScenarios">All probabilistic calculation scenarios in the failure mechanism.</param>
        /// <param name="getOutputFunc">The func to get the output from a calculation scenario.</param>
        /// <returns>The calculated detailed assessment probability; or <see cref="double.NaN"/> when there
        /// are no relevant calculations, when not all relevant calculations are performed or when the
        /// contribution of the relevant calculations don't add up to 1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double GetInitialFailureMechanismResultProbability(this PipingFailureMechanismSectionResult sectionResult,
                                                                         IEnumerable<ProbabilisticPipingCalculationScenario> calculationScenarios,
                                                                         Func<ProbabilisticPipingCalculationScenario, IPartialProbabilisticPipingOutput> getOutputFunc)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            if (getOutputFunc == null)
            {
                throw new ArgumentNullException(nameof(getOutputFunc));
            }

            ProbabilisticPipingCalculationScenario[] relevantScenarios = sectionResult.GetCalculationScenarios<ProbabilisticPipingCalculationScenario>(
                                                                                          calculationScenarios)
                                                                                      .ToArray();

            if (relevantScenarios.Length == 0 || !relevantScenarios.All(s => s.HasOutput) || Math.Abs(sectionResult.GetTotalContribution(relevantScenarios) - 1.0) > 1e-6)
            {
                return double.NaN;
            }

            return relevantScenarios.Sum(scenario => StatisticsConverter.ReliabilityToProbability(getOutputFunc(scenario).Reliability) * (double) scenario.Contribution);
        }

        /// <summary>
        /// Gets the value for the initial failure mechanism result of safety per failure mechanism section as a probability.
        /// </summary>
        /// <param name="sectionResult">The section result to get the detailed assessment probability for.</param>
        /// <param name="calculationScenarios">All semi probabilistic calculation scenarios in the failure mechanism.</param>
        /// <param name="norm">The norm to assess for.</param>
        /// <returns>The calculated detailed assessment probability; or <see cref="double.NaN"/> when there
        /// are no relevant calculations, when not all relevant calculations are performed or when the
        /// contribution of the relevant calculations don't add up to 1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double GetInitialFailureMechanismResultProbability(this PipingFailureMechanismSectionResult sectionResult,
                                                                         IEnumerable<SemiProbabilisticPipingCalculationScenario> calculationScenarios,
                                                                         double norm)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            SemiProbabilisticPipingCalculationScenario[] relevantScenarios = sectionResult.GetCalculationScenarios<SemiProbabilisticPipingCalculationScenario>(
                                                                                              calculationScenarios)
                                                                                          .ToArray();

            if (relevantScenarios.Length == 0 || !relevantScenarios.All(s => s.HasOutput) || Math.Abs(sectionResult.GetTotalContribution(relevantScenarios) - 1.0) > 1e-6)
            {
                return double.NaN;
            }

            double totalDetailedAssessmentProbability = 0;
            foreach (SemiProbabilisticPipingCalculationScenario scenario in relevantScenarios)
            {
                DerivedSemiProbabilisticPipingOutput derivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(scenario.Output, norm);

                totalDetailedAssessmentProbability += derivedOutput.PipingProbability * (double) scenario.Contribution;
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
        public static RoundedDouble GetTotalContribution(this PipingFailureMechanismSectionResult sectionResult,
                                                         IEnumerable<IPipingCalculationScenario<PipingInput>> calculationScenarios)
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
                                   .GetCalculationScenarios<IPipingCalculationScenario<PipingInput>>(calculationScenarios)
                                   .Aggregate<ICalculationScenario, double>(0, (current, calculationScenario) => current + calculationScenario.Contribution);
        }

        /// <summary>
        /// Gets a collection of the relevant <see cref="SemiProbabilisticPipingCalculationScenario"/>.
        /// </summary>
        /// <param name="sectionResult">The section result to get the relevant scenarios for.</param>
        /// <param name="calculationScenarios">The calculation scenarios to get the relevant scenarios from.</param>
        /// <typeparam name="T">The type of the calculation scenarios.</typeparam>
        /// <returns>A collection of relevant calculation scenarios.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<T> GetCalculationScenarios<T>(
            this PipingFailureMechanismSectionResult sectionResult,
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculationScenarios)
            where T : IPipingCalculationScenario<PipingInput>
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

            return calculationScenarios.OfType<T>()
                                       .Where(pc => pc.IsRelevant && pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }
    }
}
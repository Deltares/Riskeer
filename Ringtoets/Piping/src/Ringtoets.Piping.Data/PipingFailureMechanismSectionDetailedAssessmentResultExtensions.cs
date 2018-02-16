// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Extension methods for obtaining detailed assessment probabilities from output for an assessment of the piping failure mechanism.
    /// </summary>
    public static class PipingFailureMechanismSectionDetailedAssessmentResultExtensions
    {
        /// <summary>
        /// Gets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        /// <param name="sectionResult">The section result to get the detailed assessment probability for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism the calculations belong to.</param>
        /// <param name="assessmentSection">The assessment section the calculations belong to.</param>
        /// <returns>The calculated detailed assessment probability; or <see cref="double.NaN"/> when there are no
        /// performed or relevant calculations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double GetDetailedAssessmentProbability(this PipingFailureMechanismSectionResult sectionResult,
                                                              IEnumerable<PipingCalculationScenario> calculations,
                                                              PipingFailureMechanism failureMechanism,
                                                              IAssessmentSection assessmentSection)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            PipingCalculationScenario[] relevantScenarios = sectionResult.GetCalculationScenarios(calculations).ToArray();
            bool relevantScenarioAvailable = relevantScenarios.Length != 0;

            if (relevantScenarioAvailable && Math.Abs(sectionResult.GetTotalContribution(relevantScenarios) - 1.0) > 1e-6)
            {
                return double.NaN;
            }

            if (!relevantScenarioAvailable || sectionResult.GetCalculationScenarioStatus(relevantScenarios) != CalculationScenarioStatus.Done)
            {
                return double.NaN;
            }

            double totalDetailedAssessmentProbability = 0;
            foreach (PipingCalculationScenario scenario in relevantScenarios)
            {
                DerivedPipingOutput derivedOutput = DerivedPipingOutputFactory.Create(scenario.Output, failureMechanism, assessmentSection);

                totalDetailedAssessmentProbability += derivedOutput.PipingProbability * (double) scenario.Contribution;
            }

            return totalDetailedAssessmentProbability;
        }

        /// <summary>
        /// Gets the contribution of all relevant <see cref="GetCalculationScenarios"/> together.
        /// </summary>
        /// <param name="pipingFailureMechanismSectionResult">The result to get the result for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        public static RoundedDouble GetTotalContribution(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult,
                                                         IEnumerable<PipingCalculationScenario> calculations)
        {
            return (RoundedDouble) pipingFailureMechanismSectionResult
                                   .GetCalculationScenarios(calculations)
                                   .Aggregate<ICalculationScenario, double>(0, (current, calculationScenario) => current + calculationScenario.Contribution);
        }

        /// <summary>
        /// Gets a list of the relevant <see cref="ICalculationScenario"/>.
        /// </summary>
        /// <param name="pipingFailureMechanismSectionResult">The result to get the result for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        public static IEnumerable<PipingCalculationScenario> GetCalculationScenarios(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult,
                                                                                     IEnumerable<PipingCalculationScenario> calculations)
        {
            IEnumerable<Segment2D> lineSegments = Math2D.ConvertLinePointsToLineSegments(pipingFailureMechanismSectionResult.Section.Points);

            return calculations
                .Where(pc => pc.IsRelevant && pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));
        }

        /// <summary>
        /// Gets the status of the section result depending on the relevant calculation scenarios.
        /// </summary>
        /// <param name="pipingFailureMechanismSectionResult">The result to get the result for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when any of the relevant calculations 
        /// in <paramref name="pipingFailureMechanismSectionResult"/> has an invalid <see cref="CalculationScenarioStatus"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when any of the relevant scenarios has an unsupported value of <see cref="CalculationScenarioStatus"/>.</exception>
        public static CalculationScenarioStatus GetCalculationScenarioStatus(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult,
                                                                             IEnumerable<PipingCalculationScenario> calculations)
        {
            var failed = false;
            var notCalculated = false;
            foreach (PipingCalculationScenario calculationScenario in pipingFailureMechanismSectionResult.GetCalculationScenarios(calculations).Where(cs => cs.IsRelevant))
            {
                CalculationScenarioStatus calculationScenarioStatus = calculationScenario.Status;
                if (!Enum.IsDefined(typeof(CalculationScenarioStatus), calculationScenarioStatus))
                {
                    throw new InvalidEnumArgumentException(nameof(pipingFailureMechanismSectionResult),
                                                           (int) calculationScenarioStatus,
                                                           typeof(CalculationScenarioStatus));
                }

                switch (calculationScenarioStatus)
                {
                    case CalculationScenarioStatus.Failed:
                        failed = true;
                        break;
                    case CalculationScenarioStatus.NotCalculated:
                        notCalculated = true;
                        break;
                    case CalculationScenarioStatus.Done:
                        continue;
                    default:
                        throw new NotSupportedException($"The enum value {nameof(CalculationScenarioStatus)}.{calculationScenarioStatus} is not supported.");
                }
            }

            if (failed)
            {
                return CalculationScenarioStatus.Failed;
            }

            if (notCalculated)
            {
                return CalculationScenarioStatus.NotCalculated;
            }

            return CalculationScenarioStatus.Done;
        }
    }
}
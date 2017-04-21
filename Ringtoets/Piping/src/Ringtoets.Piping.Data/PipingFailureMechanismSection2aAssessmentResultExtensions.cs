// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Extension methods for obtaining level 2a results from output for an assessment of the piping failure mechanism.
    /// </summary>
    public static class PipingFailureMechanismSection2aAssessmentResultExtensions
    {
        /// <summary>
        /// Gets the value for the detailed assessment of safety per failure mechanism section as a probability.
        /// </summary>
        /// <param name="pipingFailureMechanismSectionResult">The result to get the result for.</param>
        /// <param name="calculations">All calculations in the failure mechanism.</param>
        public static double GetAssessmentLayerTwoA(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult,
                                                    IEnumerable<PipingCalculationScenario> calculations)
        {
            List<PipingCalculationScenario> calculationScenarios = pipingFailureMechanismSectionResult
                .GetCalculationScenarios(calculations)
                .Where(cs => cs.Status == CalculationScenarioStatus.Done)
                .ToList();

            return calculationScenarios.Any()
                       ? calculationScenarios.Sum(scenario => scenario.Probability * scenario.Contribution.Value)
                       : double.NaN;
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
        public static CalculationScenarioStatus GetCalculationScenarioStatus(this PipingFailureMechanismSectionResult pipingFailureMechanismSectionResult,
                                                                             IEnumerable<PipingCalculationScenario> calculations)
        {
            var failed = false;
            var notCalculated = false;
            foreach (PipingCalculationScenario calculationScenario in pipingFailureMechanismSectionResult.GetCalculationScenarios(calculations).Where(cs => cs.IsRelevant))
            {
                switch (calculationScenario.Status)
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
                        throw new InvalidEnumArgumentException("pipingFailureMechanismSectionResult",
                                                               (int) calculationScenario.Status,
                                                               typeof(CalculationScenarioStatus));
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
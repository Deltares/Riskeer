// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Helpers;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Providers
{
    /// <summary>
    /// This class provides error messages about the failure mechanism result rows that contains calculated probabilities.
    /// </summary>
    /// <typeparam name="T">The type of calculation scenario.</typeparam>
    public class FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider<T> : FailureMechanismSectionResultRowErrorProvider,
                                                                                             IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider
        where T : ICalculationScenario
    {
        private readonly FailureMechanismSectionResult sectionResult;
        private readonly IEnumerable<ICalculationScenario> calculationScenarios;
        private readonly Func<T, IEnumerable<Segment2D>, bool> intersectionFunc;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider{T}"/>.
        /// </summary>
        /// <param name="sectionResult">The <see cref="FailureMechanismSectionResult"/> to validate for.</param>
        /// <param name="calculationScenarios">The calculation scenarios to validate.</param>
        /// <param name="intersectionFunc">The function to determine whether a scenario is belonging
        /// to the given <paramref name="sectionResult"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider(FailureMechanismSectionResult sectionResult,
                                                                                      IEnumerable<ICalculationScenario> calculationScenarios,
                                                                                      Func<T, IEnumerable<Segment2D>, bool> intersectionFunc)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            if (intersectionFunc == null)
            {
                throw new ArgumentNullException(nameof(intersectionFunc));
            }

            this.sectionResult = sectionResult;
            this.calculationScenarios = calculationScenarios;
            this.intersectionFunc = intersectionFunc;
        }

        public string GetCalculatedProbabilityValidationError(Func<double> getProbabilityFunc)
        {
            if (getProbabilityFunc == null)
            {
                throw new ArgumentNullException(nameof(getProbabilityFunc));
            }

            T[] relevantScenarios = sectionResult.GetRelevantCalculationScenarios(calculationScenarios, intersectionFunc).ToArray();

            if (relevantScenarios.Length == 0)
            {
                return Resources.FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider_No_relevant_calculation_scenarios_present;
            }

            if (Math.Abs(CalculationScenarioHelper.GetTotalContribution(relevantScenarios) - 1.0) > 1e-6)
            {
                return Resources.CalculationScenarios_Scenario_contribution_for_this_section_not_100;
            }

            if (!relevantScenarios.All(s => s.HasOutput))
            {
                return Resources.FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider_Not_all_relevant_calculation_scenarios_have_been_executed;
            }

            if (double.IsNaN(getProbabilityFunc()))
            {
                return Resources.FailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider_All_relevant_calculation_scenarios_must_have_valid_output;
            }

            return string.Empty;
        }
    }
}
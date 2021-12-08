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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.DataGrid;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for updating states of a <see cref="DataGridViewColumnStateDefinition"/>
    /// in a <see cref="FailureMechanismSectionResultRowOld{T}"/>.
    /// </summary>
    public static class FailureMechanismSectionResultRowHelperOld
    {
        /// <summary>
        /// Gets the error text to display when the detailed assessment probability fails.
        /// </summary>
        /// <typeparam name="TCalculationScenario">The type of <see cref="ICalculationScenario"/>.</typeparam>
        /// <param name="relevantScenarios">All relevant scenarios to use.</param>
        /// <param name="getTotalContributionFunc">The <see cref="Func{T1,TResult}"/> to get
        /// the total contribution.</param>
        /// <param name="getDetailedAssessmentProbabilityFunc">The <see cref="Func{T1,TResult}"/>
        /// to get the detailed assessment probability.</param>
        /// <returns>The error message when an error is present; <see cref="string.Empty"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static string GetDetailedAssessmentProbabilityError<TCalculationScenario>(
            TCalculationScenario[] relevantScenarios, Func<IEnumerable<TCalculationScenario>, double> getTotalContributionFunc,
            Func<IEnumerable<TCalculationScenario>, double> getDetailedAssessmentProbabilityFunc)
            where TCalculationScenario : ICalculationScenario
        {
            if (relevantScenarios == null)
            {
                throw new ArgumentNullException(nameof(relevantScenarios));
            }

            if (getTotalContributionFunc == null)
            {
                throw new ArgumentNullException(nameof(getTotalContributionFunc));
            }

            if (getDetailedAssessmentProbabilityFunc == null)
            {
                throw new ArgumentNullException(nameof(getDetailedAssessmentProbabilityFunc));
            }

            if (relevantScenarios.Length == 0)
            {
                return Resources.FailureMechanismResultView_DataGridViewCellFormatting_No_relevant_calculation_scenarios_present;
            }

            if (Math.Abs(getTotalContributionFunc(relevantScenarios) - 1.0) > 1e-6)
            {
                return Resources.FailureMechanismResultView_DataGridViewCellFormatting_Scenario_contribution_for_this_section_not_100;
            }

            if (!relevantScenarios.All(s => s.HasOutput))
            {
                return Resources.FailureMechanismResultView_DataGridViewCellFormatting_Not_all_relevant_calculation_scenarios_have_been_executed;
            }

            if (double.IsNaN(getDetailedAssessmentProbabilityFunc(relevantScenarios)))
            {
                return Resources.FailureMechanismResultView_DataGridViewCellFormatting_All_relevant_calculation_scenarios_must_have_valid_output;
            }

            return string.Empty;
        }

        /// <summary>
        /// Helper method that determines whether the simple assessment is sufficient.
        /// </summary>
        /// <param name="simpleAssessmentResult">The simple assessment result to check.</param>
        /// <returns><c>true</c> when the simple assessment is <see cref="SimpleAssessmentResultType.ProbabilityNegligible"/>
        /// or <see cref="SimpleAssessmentResultType.NotApplicable"/>, <c>false</c> otherwise.</returns>
        public static bool SimpleAssessmentIsSufficient(SimpleAssessmentResultType simpleAssessmentResult)
        {
            return simpleAssessmentResult == SimpleAssessmentResultType.ProbabilityNegligible
                   || simpleAssessmentResult == SimpleAssessmentResultType.NotApplicable;
        }

        /// <summary>
        /// Helper method that determines whether the simple assessment is sufficient.
        /// </summary>
        /// <param name="simpleAssessmentResult">The simple assessment result to check.</param>
        /// <returns><c>true</c> when the simple assessment is <see cref="SimpleAssessmentResultType.NotApplicable"/>, 
        /// <c>false</c> otherwise.</returns>
        public static bool SimpleAssessmentIsSufficient(SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            return simpleAssessmentResult == SimpleAssessmentValidityOnlyResultType.NotApplicable;
        }

        /// <summary>
        /// Helper method that determines whether the detailed assessment
        /// is <see cref="DetailedAssessmentProbabilityOnlyResultType.Probability"/>.
        /// </summary>
        /// <param name="detailedAssessmentResult">The detailed assessment result to check.</param>
        /// <returns><c>true</c> when the detailed assessment is
        /// <see cref="DetailedAssessmentProbabilityOnlyResultType.Probability"/>, <c>false</c> otherwise.</returns>
        public static bool DetailedAssessmentResultIsProbability(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult)
        {
            return detailedAssessmentResult == DetailedAssessmentProbabilityOnlyResultType.Probability;
        }

        /// <summary>
        /// Helper method that determines whether the tailor made assessment
        /// is <see cref="TailorMadeAssessmentProbabilityCalculationResultType.Probability"/>.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The tailor made assessment result to check.</param>
        /// <returns><c>true</c> when the tailor made assessment
        /// is <see cref="TailorMadeAssessmentProbabilityCalculationResultType.Probability"/>, <c>false</c> otherwise.</returns>
        public static bool TailorMadeAssessmentResultIsProbability(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult)
        {
            return tailorMadeAssessmentResult == TailorMadeAssessmentProbabilityCalculationResultType.Probability;
        }

        /// <summary>
        /// Helper method that determines whether the tailor made assessment
        /// is <see cref="TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability"/>.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The tailor made assessment result to check.</param>
        /// <returns><c>true</c> when the tailor made assessment
        /// is <see cref="TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability"/>, <c>false</c> otherwise.</returns>
        public static bool TailorMadeAssessmentResultIsProbability(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult)
        {
            return tailorMadeAssessmentResult == TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability;
        }

        /// <summary>
        /// Helper method that sets the style of a <paramref name="columnStateDefinition"/> based on a
        /// <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The column state definition to set the style for.</param>
        /// <param name="assemblyCategoryGroup">The assembly category group to base the style on.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyCategoryGroup"/>
        /// has an invalid value for <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyCategoryGroup"/>
        /// is not supported.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnStateDefinition"/>
        /// is <c>null</c>.</exception>
        public static void SetAssemblyCategoryGroupStyle(DataGridViewColumnStateDefinition columnStateDefinition,
                                                         FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup)
        {
            if (columnStateDefinition == null)
            {
                throw new ArgumentNullException(nameof(columnStateDefinition));
            }

            columnStateDefinition.Style = new CellStyle(
                Color.FromKnownColor(KnownColor.ControlText),
                AssemblyCategoryGroupColorHelper.GetFailureMechanismSectionAssemblyCategoryGroupColor(assemblyCategoryGroup));
        }
    }
}
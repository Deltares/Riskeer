// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for updating states of a <see cref="DataGridViewColumnStateDefinition"/>
    /// in a <see cref="FailureMechanismSectionResultRow{T}"/>.
    /// </summary>
    public static class FailureMechanismSectionResultRowHelper
    {
        /// <summary>
        /// Gets the error text to display when the detailed assessment fails.
        /// </summary>
        /// <param name="detailedAssessmentProbability">The value representing the probability of the detailed assessment.</param>
        /// <param name="normativeCalculation">The <see cref="ICalculation"/> set for the 
        /// section result. May be <c>null</c> if the section result does not have a calculation set.</param>
        public static string GetDetailedAssessmentError(double detailedAssessmentProbability,
                                                        ICalculation normativeCalculation)
        {
            if (normativeCalculation == null)
            {
                return Resources.FailureMechanismResultView_DataGridViewCellFormatting_Calculation_not_set;
            }

            CalculationScenarioStatus calculationScenarioStatus = GetCalculationStatus(normativeCalculation,
                                                                                       detailedAssessmentProbability);
            if (calculationScenarioStatus == CalculationScenarioStatus.NotCalculated)
            {
                return Resources.FailureMechanismResultView_DataGridViewCellFormatting_Calculation_not_calculated;
            }

            if (calculationScenarioStatus == CalculationScenarioStatus.Failed)
            {
                return Resources.FailureMechanismResultView_DataGridViewCellFormatting_Calculation_must_have_valid_output;
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

        /// <summary>
        /// Helper method that sets the state of the <paramref name="columnStateDefinition"/>
        /// based on <paramref name="shouldDisable"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The column state definition to set the state for.</param>
        /// <param name="shouldDisable">Indicator whether the column should be disabled or not.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnStateDefinition"/>
        /// is <c>null</c>.</exception>
        public static void SetColumnState(DataGridViewColumnStateDefinition columnStateDefinition, bool shouldDisable)
        {
            if (columnStateDefinition == null)
            {
                throw new ArgumentNullException(nameof(columnStateDefinition));
            }

            if (shouldDisable)
            {
                DisableColumn(columnStateDefinition);
            }
            else
            {
                EnableColumn(columnStateDefinition);
            }
        }

        /// <summary>
        /// Helper method that enables the <paramref name="columnStateDefinition"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The column state definition to enable.</param>
        /// <param name="readOnly">Indicator whether the column should be read-only or not.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnStateDefinition"/>
        /// is <c>null</c>.</exception>
        public static void EnableColumn(DataGridViewColumnStateDefinition columnStateDefinition, bool readOnly = false)
        {
            if (columnStateDefinition == null)
            {
                throw new ArgumentNullException(nameof(columnStateDefinition));
            }

            columnStateDefinition.ReadOnly = readOnly;
            columnStateDefinition.Style = CellStyle.Enabled;
        }

        /// <summary>
        /// Helper method that disables the <paramref name="columnStateDefinition"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The column state definition to enable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnStateDefinition"/>
        /// is <c>null</c>.</exception>
        public static void DisableColumn(DataGridViewColumnStateDefinition columnStateDefinition)
        {
            if (columnStateDefinition == null)
            {
                throw new ArgumentNullException(nameof(columnStateDefinition));
            }

            columnStateDefinition.ReadOnly = true;
            columnStateDefinition.Style = CellStyle.Disabled;
        }

        private static CalculationScenarioStatus GetCalculationStatus(ICalculation calculation,
                                                                      double detailedAssessmentProbability)
        {
            if (!calculation.HasOutput)
            {
                return CalculationScenarioStatus.NotCalculated;
            }

            if (double.IsNaN(detailedAssessmentProbability))
            {
                return CalculationScenarioStatus.Failed;
            }

            return CalculationScenarioStatus.Done;
        }
    }
}
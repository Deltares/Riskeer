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
using System.Windows.Forms;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for showing error messages in <see cref="FailureMechanismSectionResultRow{T}"/>.
    /// </summary>
    public static class FailureMechanismSectionResultRowHelper
    {
        /// <summary>
        /// Sets the <see cref="DataGridViewCell.ErrorText"/> when the detailed assessment fails.
        /// </summary>
        /// <param name="dataGridViewCell">The current data grid view cell.</param>
        /// <param name="simpleAssessmentResult">The value representing the simple assessment result type.</param>
        /// <param name="detailedAssessmentProbability">The value representing the result of the detailed assessment.</param>
        /// <param name="normativeCalculation">The <see cref="ICalculation"/> set for the 
        /// section result. May be <c>null</c> if the section result does not have a calculation set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataGridViewCell"/> is <c>null</c>.</exception>
        public static void SetDetailedAssessmentError(DataGridViewCell dataGridViewCell,
                                                       SimpleAssessmentResultType simpleAssessmentResult,
                                                      double detailedAssessmentProbability,
                                                      ICalculation normativeCalculation)
        {
            if (dataGridViewCell == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCell));
            }

            if (simpleAssessmentResult == SimpleAssessmentResultType.NotApplicable
                || simpleAssessmentResult == SimpleAssessmentResultType.ProbabilityNegligible)
            {
                dataGridViewCell.ErrorText = string.Empty;
                return;
            }

            SetDetailedAssessmentError(dataGridViewCell, detailedAssessmentProbability, normativeCalculation);
        }

        /// <summary>
        /// Sets the <see cref="DataGridViewCell.ErrorText"/> when the detailed assessment fails.
        /// </summary>
        /// <param name="dataGridViewCell">The current data grid view cell.</param>
        /// <param name="simpleAssessmentResult">The value representing the simple assessment result.</param>
        /// <param name="detailedAssessmentProbability">The value representing the probability of the detailed assessment.</param>
        /// <param name="normativeCalculation">The <see cref="ICalculation"/> set for the 
        /// section result. May be <c>null</c> if the section result does not have a calculation set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataGridViewCell"/> is <c>null</c>.</exception>
        public static void SetDetailedAssessmentError(DataGridViewCell dataGridViewCell,
                                                       SimpleAssessmentResultValidityOnlyType simpleAssessmentResult,
                                                      double detailedAssessmentProbability,
                                                      ICalculation normativeCalculation)
        {
            if (dataGridViewCell == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCell));
            }

            if (simpleAssessmentResult == SimpleAssessmentResultValidityOnlyType.NotApplicable)
            {
                dataGridViewCell.ErrorText = string.Empty;
                return;
            }

            SetDetailedAssessmentError(dataGridViewCell, detailedAssessmentProbability, normativeCalculation);
        }

        private static void SetDetailedAssessmentError(DataGridViewCell dataGridViewCell,
                                                       double detailedAssessmentProbability,
                                                       ICalculation normativeCalculation)
        {
            if (normativeCalculation == null)
            {
                dataGridViewCell.ErrorText = Resources.FailureMechanismResultView_DataGridViewCellFormatting_Calculation_not_set;
                return;
            }

            CalculationScenarioStatus calculationScenarioStatus = GetCalculationStatus(normativeCalculation,
                                                                                       detailedAssessmentProbability);
            if (calculationScenarioStatus == CalculationScenarioStatus.NotCalculated)
            {
                dataGridViewCell.ErrorText = Resources.FailureMechanismResultView_DataGridViewCellFormatting_Calculation_not_calculated;
                return;
            }

            if (calculationScenarioStatus == CalculationScenarioStatus.Failed)
            {
                dataGridViewCell.ErrorText = Resources.FailureMechanismResultView_DataGridViewCellFormatting_Calculation_must_have_valid_output;
                return;
            }

            dataGridViewCell.ErrorText = string.Empty;
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
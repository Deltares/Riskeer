﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for showing error messages in <see cref="FailureMechanismSectionResultRow{T}"/>.
    /// </summary>
    public static class FailureMechanismSectionResultRowHelper
    {
        /// <summary>
        /// Sets the <see cref="DataGridViewCell.ErrorText"/> when layer 2a assessment fails.
        /// </summary>
        /// <param name="dataGridViewCell">The current data grid view cell.</param>
        /// <param name="passedAssessmentLayerOne">The value representing whether the section passed the layer 0 assessment.</param>
        /// <param name="assessmentLayerTwoA">The value representing the result of the layer 2a assessment.</param>
        /// <param name="normativeCalculation">The <see cref="ICalculation"/> set for the 
        /// section result. May be <c>null</c> if the section result does not have a calculation set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataGridViewCell"/> is <c>null</c>.</exception>
        public static void SetAssessmentLayerTwoAError(DataGridViewCell dataGridViewCell,
                                                       AssessmentLayerOneState passedAssessmentLayerOne, 
                                                       double assessmentLayerTwoA,
                                                       ICalculation normativeCalculation)
        {
            if (dataGridViewCell == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCell));
            }

            if (passedAssessmentLayerOne == AssessmentLayerOneState.Sufficient)
            {
                dataGridViewCell.ErrorText = string.Empty;
                return;
            }

            SetAssessmentLayerTwoAError(dataGridViewCell, assessmentLayerTwoA, normativeCalculation);
        }

        /// <summary>
        /// Sets the <see cref="DataGridViewCell.ErrorText"/> when layer 2a assessment fails.
        /// </summary>
        /// <param name="dataGridViewCell">The current data grid view cell.</param>
        /// <param name="simpleAssessmentResult">The value representing whether the simple assessment result.</param>
        /// <param name="assessmentLayerTwoA">The value representing the result of the layer 2a assessment.</param>
        /// <param name="normativeCalculation">The <see cref="ICalculation"/> set for the 
        /// section result. May be <c>null</c> if the section result does not have a calculation set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataGridViewCell"/> is <c>null</c>.</exception>
        public static void SetAssessmentLayerTwoAError(DataGridViewCell dataGridViewCell,
                                                       SimpleAssessmentResultValidityOnlyType simpleAssessmentResult, 
                                                       double assessmentLayerTwoA,
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

            SetAssessmentLayerTwoAError(dataGridViewCell, assessmentLayerTwoA, normativeCalculation);
        }

        private static void SetAssessmentLayerTwoAError(DataGridViewCell dataGridViewCell,
                                                        double assessmentLayerTwoA,
                                                        ICalculation normativeCalculation)
        {
            if (normativeCalculation == null)
            {
                dataGridViewCell.ErrorText = Resources.FailureMechanismResultView_DataGridViewCellFormatting_Calculation_not_set;
                return;
            }

            CalculationScenarioStatus calculationScenarioStatus = GetCalculationStatus(normativeCalculation,
                                                                                       assessmentLayerTwoA);
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
                                                                      double assessmentLayerTwoA)
        {
            if (!calculation.HasOutput)
            {
                return CalculationScenarioStatus.NotCalculated;
            }

            if (double.IsNaN(assessmentLayerTwoA))
            {
                return CalculationScenarioStatus.Failed;
            }

            return CalculationScenarioStatus.Done;
        }
    }
}
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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Service.Properties;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service that provides validation methods for probabilistic distributions.
    /// </summary>
    public static class DistributionValidation
    {
        /// <summary>
        /// Performs the validation of a <see cref="NormalDistribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to validate.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The validation errors found. Collection is empty if <paramref name="distribution"/> is valid. </returns>
        public static string[] ValidateDistribution(NormalDistribution distribution, string parameterName)
        {
            var validationResult = new List<string>();

            if (IsValidNumber(distribution.Mean))
            {
                validationResult.Add(string.Format(Resources.DistributionValidationService_ValidateDistribution_Mean_of_0_must_be_a_valid_number,
                                                   parameterName));
            }

            if (IsValidNumber(distribution.StandardDeviation))
            {
                validationResult.Add(string.Format(Resources.DistributionValidationService_ValidateDistribution_StandardDeviation_of_ParameterName_0_must_be_larger_or_equal_to_zero,
                                                   parameterName));
            }

            return validationResult.ToArray();
        }

        /// <summary>
        /// Performs the validation of a <see cref="LogNormalDistribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to validate.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The validation errors found. Collection is empty if <paramref name="distribution"/> is valid.</returns>
        public static string[] ValidateDistribution(LogNormalDistribution distribution, string parameterName)
        {
            var validationResult = new List<string>();

            if (IsValidNumber(distribution.Mean))
            {
                validationResult.Add(string.Format(Resources.DistributionValidationService_ValidateDistribution_Mean_of_0_must_be_positive_value,
                                                   parameterName));
            }

            if (IsValidNumber(distribution.StandardDeviation))
            {
                validationResult.Add(string.Format(Resources.DistributionValidationService_ValidateDistribution_StandardDeviation_of_ParameterName_0_must_be_larger_or_equal_to_zero,
                                                   parameterName));
            }

            return validationResult.ToArray();
        }

        /// <summary>
        /// Performs the validation of a <see cref="VariationCoefficientNormalDistribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to validate.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The validation errors found. Collection is empty if <paramref name="distribution"/> is valid. </returns>
        public static string[] ValidateDistribution(VariationCoefficientNormalDistribution distribution, string parameterName)
        {
            var validationResult = new List<string>();

            if (IsValidNumber(distribution.Mean))
            {
                validationResult.Add(string.Format(Resources.DistributionValidationService_ValidateDistribution_Mean_of_0_must_be_a_valid_number,
                                                   parameterName));
            }

            if (IsValidNumber(distribution.CoefficientOfVariation))
            {
                validationResult.Add(string.Format(Resources.DistributionValidationService_ValidateDistribution_CoefficientOfVariation_of_ParameterName_0_must_be_larger_or_equal_to_zero,
                                                   parameterName));
            }

            return validationResult.ToArray();
        }

        /// <summary>
        /// Performs the validation of a <see cref="VariationCoefficientLogNormalDistribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to validate.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The validation errors found. Collection is empty if <paramref name="distribution"/> is valid. </returns>
        public static string[] ValidateDistribution(VariationCoefficientLogNormalDistribution distribution, string parameterName)
        {
            var validationResult = new List<string>();

            if (IsValidNumber(distribution.Mean))
            {
                validationResult.Add(string.Format(Resources.DistributionValidationService_ValidateDistribution_Mean_of_0_must_be_positive_value,
                                                   parameterName));
            }

            if (IsValidNumber(distribution.CoefficientOfVariation))
            {
                validationResult.Add(string.Format(Resources.DistributionValidationService_ValidateDistribution_CoefficientOfVariation_of_ParameterName_0_must_be_larger_or_equal_to_zero,
                                                   parameterName));
            }

            return validationResult.ToArray();
        }

        private static bool IsValidNumber(RoundedDouble value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }
    }
}
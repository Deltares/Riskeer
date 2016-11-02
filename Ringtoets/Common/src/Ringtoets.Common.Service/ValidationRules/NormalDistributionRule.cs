﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Service.Properties;

namespace Ringtoets.Common.Service.ValidationRules
{
    /// <summary>
    /// Validation rule to validate a <see cref="NormalDistribution"/>.
    /// </summary>
    public class NormalDistributionRule : ValidationRule
    {
        private readonly NormalDistribution distribution;
        private readonly string parameterName;

        /// <summary>
        /// Instantiates a <see cref="NormalDistributionRule"/> to validate a <see cref="NormalDistribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to validate.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The validation errors found. Collection is empty if <paramref name="distribution"/> is valid.</returns>
        public NormalDistributionRule(NormalDistribution distribution, string parameterName)
        {
            this.distribution = distribution;
            this.parameterName = parameterName;
        }

        public override IEnumerable<string> Validate()
        {
            var validationResults = new List<string>();

            if (IsNotConcreteNumber(distribution.Mean))
            {
                validationResults.Add(string.Format(Resources.ProbabilisticDistributionValidationRule_Mean_of_0_must_be_a_valid_number,
                                                    parameterName));
            }

            if (IsNotConcreteNumber(distribution.StandardDeviation))
            {
                validationResults.Add(string.Format(Resources.ProbabilisticDistributionValidationRule_StandardDeviation_of_ParameterName_0_must_be_larger_or_equal_to_zero,
                                                    parameterName));
            }

            return validationResults;
        }
    }
}
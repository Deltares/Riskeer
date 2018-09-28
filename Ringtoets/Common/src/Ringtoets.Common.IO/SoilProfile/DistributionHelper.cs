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
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class which provides helper methods for validating distributions coming from
    /// D-Soil Model files.
    /// </summary>
    public static class DistributionHelper
    {
        private const double tolerance = 1e-6;

        /// <summary>
        /// Validates that the distribution is a non-shifted log normal distribution.
        /// </summary>
        /// <param name="distributionType">The distribution type.</param>
        /// <param name="shift">The value of the shift.</param>
        /// <exception cref="DistributionValidationException">Thrown when the parameter is not a 
        /// log normal distribution with a zero shift.</exception>
        public static void ValidateLogNormalDistribution(long? distributionType, double shift)
        {
            if (distributionType.HasValue)
            {
                if (distributionType.Value != SoilLayerConstants.LogNormalDistributionValue)
                {
                    throw new DistributionValidationException(Resources.StochasticDistribution_must_be_a_lognormal_distribution);
                }

                if (Math.Abs(shift) > tolerance)
                {
                    throw new DistributionValidationException(Resources.StochasticDistribution_must_be_a_lognormal_distribution_with_zero_shift);
                }
            }
        }

        /// <summary>
        /// Validates that the distribution is a shifted log normal distribution.
        /// </summary>
        /// <param name="distributionType">The distribution type.</param>
        /// <exception cref="DistributionValidationException">Thrown when the parameter is not a 
        /// log normal distribution.</exception>
        public static void ValidateShiftedLogNormalDistribution(long? distributionType)
        {
            if (distributionType.HasValue && distributionType != SoilLayerConstants.LogNormalDistributionValue)
            {
                throw new DistributionValidationException(Resources.StochasticDistribution_has_no_shifted_lognormal_distribution);
            }
        }
    }
}
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
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class which provides helper methods for validating distributions coming from
    /// D-Soil model files.
    /// </summary>
    public static class DistributionHelper
    {
        private const double tolerance = 1e-6;

        /// <summary>
        /// Validates that the distribution is a non-shifted log normal distribution.
        /// </summary>
        /// <param name="distributionType">The distribution type.</param>
        /// <param name="shift">The value of the shift.</param>
        /// <param name="parameterName">The name of the parameter to be validated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameterName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when the parameter is not a 
        /// log normal distribution with a zero shift.</exception>
        public static void ValidateIsNonShiftedLogNormal(long? distributionType, double shift, string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (distributionType.HasValue && (distributionType.Value != SoilLayerConstants.LogNormalDistributionValue
                                          || Math.Abs(shift) > tolerance))
            {
                throw new ImportedDataTransformException(string.Format(
                                                             Resources.Stochastic_parameter_0_has_no_lognormal_distribution,
                                                             parameterName));
            }
        }

        /// <summary>
        /// Validates that the distribution is a log normal distribution.
        /// </summary>
        /// <param name="distributionType">The distribution type.</param>
        /// <param name="parameterName">The name of the parameter to be validated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameterName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when the parameter is not a 
        /// log normal distribution.</exception>
        public static void ValidateIsLogNormal(long? distributionType, string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (distributionType.HasValue && distributionType != SoilLayerConstants.LogNormalDistributionValue)
            {
                throw new ImportedDataTransformException(string.Format(
                                                             Resources.SoilLayer_Stochastic_parameter_0_has_no_shifted_lognormal_distribution,
                                                             parameterName));
            }
        }
    }
}
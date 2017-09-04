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
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class which provides helper methods for validating distributions of 
    /// <see cref="SoilLayerBase"/>.
    /// </summary>
    public static class SoilLayerDistributionHelper
    {
        private const double tolerance = 1e-6;

        /// <summary>
        /// Validates if the distribution is a non-shifted log normal distribution.
        /// </summary>
        /// <param name="distribution">The distribution type.</param>
        /// <param name="shift">The value of the shift.</param>
        /// <param name="parameterName">The name of the parameter to be validated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameterName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when the parameter is not a 
        /// non-shifted log normal distribution.</exception>
        public static void ValidateIsNonShiftedLogNormal(long? distribution, double shift, string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (distribution.HasValue && (distribution.Value != SoilLayerConstants.LogNormalDistributionValue
                                          || Math.Abs(shift) > tolerance))
            {
                throw new ImportedDataTransformException(string.Format(
                                                             Resources.SoilLayer_Stochastic_parameter_0_has_no_lognormal_distribution,
                                                             parameterName));
            }
        }

        /// <summary>
        /// Validates if the distribution is a log normal distribution.
        /// </summary>
        /// <param name="distribution">The distribution type.</param>
        /// <param name="parameterName">The name of the parameter to be validated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameterName"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when the parameter is not a 
        /// log-normal distribution.</exception>
        public static void ValidateIsLogNormal(long? distribution, string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (distribution.HasValue && distribution != SoilLayerConstants.LogNormalDistributionValue)
            {
                throw new ImportedDataTransformException(string.Format(
                                                             Resources.SoilLayer_Stochastic_parameter_0_has_no_shifted_lognormal_distribution,
                                                             parameterName));
            }
        }
    }
}
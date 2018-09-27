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
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Methods for helping assign values from configurations to calculations.
    /// </summary>
    public static class ConfigurationImportHelper
    {
        /// <summary>
        /// Sets the stochast parameters.
        /// </summary>
        /// <typeparam name="TDistribution">The type of the distribution to read.</typeparam>
        /// <typeparam name="TCalculationInput">The type of the calculation input.</typeparam>
        /// <param name="stochastName">The stochast's name.</param>
        /// <param name="calculationName">The name of the calculation to configure.</param>
        /// <param name="input">The input for which to assign the read stochast.</param>
        /// <param name="stochastConfiguration">The configuration of the stochast.</param>
        /// <param name="getStochast">The function for obtaining the stochast to read.</param>
        /// <param name="setStochast">The function to set the stochast with the read parameters.</param>
        /// <param name="log">Log used to write out errors.</param>
        /// <returns><c>true</c> if reading all required stochast parameters was successful,
        /// <c>false</c> otherwise.</returns>
        public static bool TrySetStandardDeviationStochast<TDistribution, TCalculationInput>(
            string stochastName,
            string calculationName,
            TCalculationInput input,
            StochastConfiguration stochastConfiguration,
            Func<TCalculationInput, TDistribution> getStochast,
            Action<TCalculationInput, TDistribution> setStochast,
            ILog log)
            where TDistribution : IDistribution
            where TCalculationInput : ICalculationInput
        {
            if (stochastConfiguration == null)
            {
                return true;
            }

            if (stochastConfiguration.VariationCoefficient.HasValue)
            {
                log.LogCalculationConversionError(string.Format(
                                                      Resources.ConfigurationImportHelper_TrySetStandardDeviationStochast_Stochast_0_requires_standard_deviation_but_variation_coefficient_found_for_Calculation_1_,
                                                      stochastName,
                                                      calculationName),
                                                  calculationName);

                return false;
            }

            var distribution = (TDistribution) getStochast(input).Clone();
            if (!distribution.TrySetDistributionProperties(stochastConfiguration,
                                                           stochastName,
                                                           calculationName))
            {
                return false;
            }

            setStochast(input, distribution);
            return true;
        }

        /// <summary>
        /// Sets the stochast parameters.
        /// </summary>
        /// <typeparam name="TDistribution">The type of the distribution to read.</typeparam>
        /// <typeparam name="TCalculationInput">The type of the calculation input.</typeparam>
        /// <param name="stochastName">The stochast's name.</param>
        /// <param name="calculationName">The name of the calculation to configure.</param>
        /// <param name="input">The input for which to assign the read stochast.</param>
        /// <param name="stochastConfiguration">The configuration of the stochast.</param>
        /// <param name="getStochast">The function for obtaining the stochast to read.</param>
        /// <param name="setStochast">The function to set the stochast with the read parameters.</param>
        /// <param name="log">Log used to write out errors.</param>
        /// <returns><c>true</c> if reading all required stochast parameters was successful,
        /// <c>false</c> otherwise.</returns>
        public static bool TrySetVariationCoefficientStochast<TDistribution, TCalculationInput>(
            string stochastName,
            string calculationName,
            TCalculationInput input,
            StochastConfiguration stochastConfiguration,
            Func<TCalculationInput, TDistribution> getStochast,
            Action<TCalculationInput, TDistribution> setStochast,
            ILog log)
            where TDistribution : IVariationCoefficientDistribution
            where TCalculationInput : ICalculationInput
        {
            if (stochastConfiguration == null)
            {
                return true;
            }

            if (stochastConfiguration.StandardDeviation.HasValue)
            {
                log.LogCalculationConversionError(string.Format(
                                                      Resources.ConfigurationImportHelper_TrySetVariationCoefficientStochast_Stochast_0_requires_variation_coefficient_but_standard_deviation_found_for_Calculation_1_,
                                                      stochastName,
                                                      calculationName),
                                                  calculationName);

                return false;
            }

            var distribution = (TDistribution) getStochast(input).Clone();
            if (!distribution.TrySetDistributionProperties(stochastConfiguration,
                                                           stochastName,
                                                           calculationName))
            {
                return false;
            }

            setStochast(input, distribution);
            return true;
        }
    }
}
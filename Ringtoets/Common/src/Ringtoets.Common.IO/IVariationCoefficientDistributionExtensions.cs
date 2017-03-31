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

using System;
using Core.Common.Base.Data;
using log4net;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Extension methods for <see cref="IVariationCoefficientDistribution"/>, related to the scope of
    /// <see cref="CalculationConfigurationImporter{TCalculationConfigurationReader,TReadCalculation}"/>.
    /// </summary>
    public static class IVariationCoefficientDistributionExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IVariationCoefficientDistributionExtensions));

        /// <summary>
        /// Attempts to set the parameters of an <see cref="IVariationCoefficientDistribution"/>.
        /// </summary>
        /// <param name="distribution">The <see cref="IVariationCoefficientDistribution"/> to be updated.</param>
        /// <param name="mean">The new value for <see cref="IVariationCoefficientDistribution.Mean"/>.</param>
        /// <param name="variationCoefficient">The new value for <see cref="IVariationCoefficientDistribution.CoefficientOfVariation"/>.</param>
        /// <param name="stochastName">The descriptive name of <paramref name="distribution"/>.</param>
        /// <param name="calculationName">The name of the calculation to which <paramref name="distribution"/>
        /// is associated.</param>
        /// <returns><c>true</c> if setting all properties was successful, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static bool TrySetDistributionProperties(this IVariationCoefficientDistribution distribution,
                                                        double? mean, double? variationCoefficient,
                                                        string stochastName, string calculationName)
        {
            return distribution.TrySetMean(mean, stochastName, calculationName)
                   && distribution.TrySetVariationCoefficient(variationCoefficient, stochastName, calculationName);
        }

        /// <summary>
        /// Attempts to set the parameters of an <see cref="IVariationCoefficientDistribution"/>.
        /// </summary>
        /// <param name="distribution">The <see cref="IVariationCoefficientDistribution"/> to be updated.</param>
        /// <param name="configuration">The configuration containing the new values for 
        /// <see cref="IVariationCoefficientDistribution.Mean"/> and <see cref="IVariationCoefficientDistribution.CoefficientOfVariation"/>.</param>
        /// <param name="stochastName">The descriptive name of <paramref name="distribution"/>.</param>
        /// <param name="calculationName">The name of the calculation to which <paramref name="distribution"/>
        /// is associated.</param>
        /// <returns><c>true</c> if setting all properties was successful, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static bool TrySetDistributionProperties(this IVariationCoefficientDistribution distribution,
                                                        MeanVariationCoefficientStochastConfiguration configuration,
                                                        string stochastName, string calculationName)
        {
            return distribution.TrySetMean(configuration.Mean, stochastName, calculationName)
                   && distribution.TrySetVariationCoefficient(configuration.VariationCoefficient, stochastName, calculationName);
        }

        /// <summary>
        /// Attempts to set <see cref="IVariationCoefficientDistribution.Mean"/>.
        /// </summary>
        /// <param name="distribution">The <see cref="IVariationCoefficientDistribution"/> to be updated.</param>
        /// <param name="mean">The new value for <see cref="IVariationCoefficientDistribution.Mean"/>.</param>
        /// <param name="stochastName">The descriptive name of <paramref name="distribution"/>.</param>
        /// <param name="calculationName">The name of the calculation to which <paramref name="distribution"/>
        /// is associated.</param>
        /// <returns><c>true</c> if setting <see cref="IVariationCoefficientDistribution.Mean"/> was successful,
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static bool TrySetMean(this IVariationCoefficientDistribution distribution, double? mean,
                                      string stochastName, string calculationName)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            if (mean.HasValue)
            {
                try
                {
                    distribution.Mean = (RoundedDouble) mean.Value;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    string errorMessage = string.Format(
                        Resources.IVariationCoefficientDistributionExtensions_TrySetMean_Mean_0_is_invalid_for_Stochast_1_,
                        mean, stochastName);

                    LogOutOfRangeException(errorMessage,
                                           calculationName,
                                           e);

                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Attempts to set <see cref="IVariationCoefficientDistribution.CoefficientOfVariation"/>.
        /// </summary>
        /// <param name="distribution">The <see cref="IVariationCoefficientDistribution"/> to be updated.</param>
        /// <param name="variationCoefficient">The new value for <see cref="IVariationCoefficientDistribution.CoefficientOfVariation"/>.</param>
        /// <param name="stochastName">The descriptive name of <paramref name="distribution"/>.</param>
        /// <param name="calculationName">The name of the calculation to which <paramref name="distribution"/>
        /// is associated.</param>
        /// <returns><c>true</c> if setting <see cref="IVariationCoefficientDistribution.CoefficientOfVariation"/>
        /// was successful, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static bool TrySetVariationCoefficient(this IVariationCoefficientDistribution distribution, double? variationCoefficient,
                                                   string stochastName, string calculationName)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            if (variationCoefficient.HasValue)
            {
                try
                {
                    distribution.CoefficientOfVariation = (RoundedDouble) variationCoefficient.Value;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    string errorMessage = string.Format(
                        Resources.IVariationCoefficientDistributionExtensions_TrySetVariationCoefficient_VariationCoefficient_0_is_invalid_for_Stochast_1_,
                        variationCoefficient, stochastName);

                    LogOutOfRangeException(errorMessage,
                                           calculationName,
                                           e);

                    return false;
                }
            }
            return true;
        }

        private static void LogOutOfRangeException(string errorMessage, string calculationName, ArgumentOutOfRangeException e)
        {
            log.ErrorFormat(Resources.CalculationConfigurationImporter_ValidateCalculation_ErrorMessage_0_Calculation_1_skipped,
                            $"{errorMessage} {e.Message}", calculationName);
        }
    }
}
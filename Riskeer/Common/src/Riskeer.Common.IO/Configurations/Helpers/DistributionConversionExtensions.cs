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
using Core.Common.Base.Data;
using log4net;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for converting <see cref="IDistribution"/> to <see cref="StochastConfiguration"/>.
    /// </summary>
    public static class DistributionConversionExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DistributionConversionExtensions));

        /// <summary>
        /// Configure a new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.Mean"/> and 
        /// <see cref="StochastConfiguration.StandardDeviation"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.Mean"/> and 
        /// <see cref="StochastConfiguration.StandardDeviation"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static StochastConfiguration ToStochastConfiguration(this IDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            return new StochastConfiguration
            {
                Mean = distribution.Mean,
                StandardDeviation = distribution.StandardDeviation
            };
        }

        /// <summary>
        /// Configure a new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.Mean"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.Mean"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static StochastConfiguration ToStochastConfigurationWithMean(this IDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            return new StochastConfiguration
            {
                Mean = distribution.Mean
            };
        }

        /// <summary>
        /// Configure a new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.StandardDeviation"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.StandardDeviation"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static StochastConfiguration ToStochastConfigurationWithStandardDeviation(this IDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            return new StochastConfiguration
            {
                StandardDeviation = distribution.StandardDeviation
            };
        }

        /// <summary>
        /// Configure a new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.Mean"/> and 
        /// <see cref="StochastConfiguration.VariationCoefficient"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.Mean"/> and 
        /// <see cref="StochastConfiguration.VariationCoefficient"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static StochastConfiguration ToStochastConfiguration(this IVariationCoefficientDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            return new StochastConfiguration
            {
                Mean = distribution.Mean,
                VariationCoefficient = distribution.CoefficientOfVariation
            };
        }

        /// <summary>
        /// Configure a new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.Mean"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.Mean"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static StochastConfiguration ToStochastConfigurationWithMean(this IVariationCoefficientDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            return new StochastConfiguration
            {
                Mean = distribution.Mean
            };
        }

        /// <summary>
        /// Configure a new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.VariationCoefficient"/> taken from
        /// <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution to take the values from.</param>
        /// <returns>A new <see cref="StochastConfiguration"/> with 
        /// <see cref="StochastConfiguration.VariationCoefficient"/> set.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public static StochastConfiguration ToStochastConfigurationWithVariationCoefficient(this IVariationCoefficientDistribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            return new StochastConfiguration
            {
                VariationCoefficient = distribution.CoefficientOfVariation
            };
        }

        /// <summary>
        /// Attempts to set the parameters of an <see cref="IDistribution"/>.
        /// </summary>
        /// <param name="distribution">The <see cref="IDistribution"/> to be updated.</param>
        /// <param name="mean">The new value for <see cref="IDistribution.Mean"/>.</param>
        /// <param name="standardDeviation">The new value for <see cref="IDistribution.StandardDeviation"/>.</param>
        /// <param name="stochastName">The descriptive name of <paramref name="distribution"/>.</param>
        /// <param name="calculationName">The name of the calculation to which <paramref name="distribution"/>
        /// is associated.</param>
        /// <returns><c>true</c> if setting all properties was successful, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static bool TrySetDistributionProperties(this IDistribution distribution,
                                                        double? mean, double? standardDeviation,
                                                        string stochastName, string calculationName)
        {
            return distribution.TrySetMean(mean, stochastName, calculationName)
                   && distribution.TrySetStandardDeviation(standardDeviation, stochastName, calculationName);
        }

        /// <summary>
        /// Attempts to set the parameters of an <see cref="IDistribution"/>.
        /// </summary>
        /// <param name="distribution">The <see cref="IDistribution"/> to be updated.</param>
        /// <param name="configuration">The configuration containing the new values for 
        /// <see cref="IDistribution.Mean"/> and <see cref="IDistribution.StandardDeviation"/>.</param>
        /// <param name="stochastName">The descriptive name of <paramref name="distribution"/>.</param>
        /// <param name="calculationName">The name of the calculation to which <paramref name="distribution"/>
        /// is associated.</param>
        /// <returns><c>true</c> if setting all properties was successful, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static bool TrySetDistributionProperties(this IDistribution distribution,
                                                        StochastConfiguration configuration,
                                                        string stochastName, string calculationName)
        {
            return distribution.TrySetMean(configuration.Mean, stochastName, calculationName)
                   && distribution.TrySetStandardDeviation(configuration.StandardDeviation, stochastName, calculationName);
        }

        /// <summary>
        /// Attempts to set <see cref="IDistribution.Mean"/>.
        /// </summary>
        /// <param name="distribution">The <see cref="IDistribution"/> to be updated.</param>
        /// <param name="mean">The new value for <see cref="IDistribution.Mean"/>.</param>
        /// <param name="stochastName">The descriptive name of <paramref name="distribution"/>.</param>
        /// <param name="calculationName">The name of the calculation to which <paramref name="distribution"/>
        /// is associated.</param>
        /// <returns><c>true</c> if setting <see cref="IDistribution.Mean"/> was successful,
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static bool TrySetMean(this IDistribution distribution, double? mean,
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
                        Resources.IDistributionExtensions_TrySetMean_Mean_0_is_invalid_for_Stochast_1_,
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
        /// Attempts to set <see cref="IDistribution.StandardDeviation"/>.
        /// </summary>
        /// <param name="distribution">The <see cref="IDistribution"/> to be updated.</param>
        /// <param name="standardDeviation">The new value for <see cref="IDistribution.StandardDeviation"/>.</param>
        /// <param name="stochastName">The descriptive name of <paramref name="distribution"/>.</param>
        /// <param name="calculationName">The name of the calculation to which <paramref name="distribution"/>
        /// is associated.</param>
        /// <returns><c>true</c> if setting <see cref="IDistribution.StandardDeviation"/>
        /// was successful, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/>
        /// is <c>null</c>.</exception>
        public static bool TrySetStandardDeviation(this IDistribution distribution, double? standardDeviation,
                                                   string stochastName, string calculationName)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            if (standardDeviation.HasValue)
            {
                try
                {
                    distribution.StandardDeviation = (RoundedDouble) standardDeviation.Value;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    string errorMessage = string.Format(
                        Resources.IDistributionExtensions_TrySetStandardDeviation_StandardDeviation_0_is_invalid_for_Stochast_1_,
                        standardDeviation, stochastName);

                    LogOutOfRangeException(errorMessage,
                                           calculationName,
                                           e);

                    return false;
                }
            }

            return true;
        }

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
                                                        StochastConfiguration configuration,
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
            log.ErrorFormat(Resources.ILogExtensions_LogCalculationConversionError_ErrorMessage_0_Calculation_1_skipped,
                            $"{errorMessage} {e.Message}", calculationName);
        }
    }
}
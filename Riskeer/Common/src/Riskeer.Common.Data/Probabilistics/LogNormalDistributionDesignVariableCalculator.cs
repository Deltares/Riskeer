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

namespace Riskeer.Common.Data.Probabilistics
{
    /// <summary>
    /// Calculator for design variables for log normal distributions.
    /// </summary>
    internal class LogNormalDistributionDesignVariableCalculator : PercentileBasedDesignVariableCalculator
    {
        private LogNormalDistributionDesignVariableCalculator(
            double mean,
            double standardDeviation,
            double coefficientOfVariation,
            double shift) : base(mean, standardDeviation, coefficientOfVariation)
        {
            Shift = shift;
        }

        /// <summary>
        /// Creates a <see cref="PercentileBasedDesignVariableCalculator"/> for normal distributions based
        /// on mean and standard deviation.
        /// </summary>
        /// <param name="mean">The mean of the log normal distribution.</param>
        /// <param name="standardDeviation">The standard deviation of the log normal distribution.</param>
        /// <param name="shift">The shift of the log normal distribution.</param>
        /// <returns>A new <see cref="PercentileBasedDesignVariableCalculator"/>.</returns>
        internal static PercentileBasedDesignVariableCalculator CreateWithStandardDeviation(double mean, double standardDeviation, double shift)
        {
            return new LogNormalDistributionDesignVariableCalculator(mean, standardDeviation, standardDeviation / (mean - shift), shift);
        }

        /// <summary>
        /// Creates a <see cref="PercentileBasedDesignVariableCalculator"/> for normal distributions based
        /// on mean and standard deviation.
        /// </summary>
        /// <param name="mean">The mean of the log normal distribution.</param>
        /// <param name="coefficientOfVariation">The coefficient of variation of the log normal distribution.</param>
        /// <param name="shift">The shift of the log normal distribution.</param>
        /// <returns>A new <see cref="PercentileBasedDesignVariableCalculator"/>.</returns>
        internal static PercentileBasedDesignVariableCalculator CreateWithCoefficientOfVariation(double mean, double coefficientOfVariation, double shift)
        {
            return new LogNormalDistributionDesignVariableCalculator(mean, (mean - shift) * coefficientOfVariation, coefficientOfVariation, shift);
        }

        internal override double GetDesignValue(double percentile)
        {
            double normalSpaceDesignValue = DetermineDesignValueInNormalDistributionSpace(percentile);
            return ProjectFromNormalToLogNormalSpace(normalSpaceDesignValue) + Shift;
        }

        private double Shift { get; }

        /// <summary>
        /// Projects <see cref="PercentileBasedDesignVariable{TDistributionType}.Distribution"/> into 'normal
        /// distribution' space and calculates the design value for that value space.
        /// </summary>
        /// <returns>The design value in 'normal distribution' space.</returns>
        private double DetermineDesignValueInNormalDistributionSpace(double percentile)
        {
            // Determine normal distribution parameters from log-normal parameters, as
            // design value can only be determined in 'normal distribution' space.
            // Below formula's come from Tu-Delft College dictaat "b3 Probabilistisch Ontwerpen"
            // by ir. A.C.W.M. Vrouwenvelder and ir.J.K. Vrijling 5th reprint 1987.
            double sigmaNormal = Math.Sqrt(Math.Log(CoefficientOfVariation * CoefficientOfVariation + 1.0));
            double muNormal = Math.Log(Mean - Shift) - 0.5 * sigmaNormal * sigmaNormal;

            return DetermineDesignValue(muNormal, sigmaNormal, percentile);
        }

        private static double ProjectFromNormalToLogNormalSpace(double normalSpaceDesignValue)
        {
            return Math.Exp(normalSpaceDesignValue);
        }
    }
}
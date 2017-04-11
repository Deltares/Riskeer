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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using MathNet.Numerics.Distributions;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// This class defines a percentile based design variable for a log-normal distribution
    /// defined with a variation coefficient.
    /// </summary>
    public class VariationCoefficientLogNormalDistributionDesignVariable : VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution>
    {
        private static readonly Range<double> percentileValidityRange = new Range<double>(0, 1);
        private double percentile;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogNormalDistributionDesignVariable"/> class.
        /// </summary>
        /// <param name="distribution">A log-normal distribution.</param>
        public VariationCoefficientLogNormalDistributionDesignVariable(VariationCoefficientLogNormalDistribution distribution) : base(distribution)
        {
            percentile = 0.5;
        }

        /// <summary>
        /// Gets or sets the percentile used to derive a deterministic value based on <see cref="DesignVariable{TDistributionType}.Distribution"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> 
        /// is not in range [0,1].</exception>
        public double Percentile
        {
            get
            {
                return percentile;
            }
            set
            {
                if (!percentileValidityRange.InRange(value))
                {
                    string message = string.Format(Resources.DesignVariable_Percentile_must_be_in_Range_0_,
                                                   percentileValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                    throw new ArgumentOutOfRangeException(null, message);
                }
                percentile = value;
            }
        }

        public override RoundedDouble GetDesignValue()
        {
            double normalSpaceDesignValue = DetermineDesignValueInNormalDistributionSpace();
            return ProjectFromNormalToLogNormalSpace(normalSpaceDesignValue);
        }

        /// <summary>
        /// Projects <see cref="VariationCoefficientDesignVariable{TDistributionType}.Distribution"/> into 'normal
        /// distribution' space and calculates the design value for that value space.
        /// </summary>
        /// <returns>The design value in 'normal distribution' space.</returns>
        private double DetermineDesignValueInNormalDistributionSpace()
        {
            // Determine normal distribution parameters from log-normal parameters, as
            // design value can only be determined in 'normal distribution' space.
            // Below formula's come from Tu-Delft College dictaat "b3 Probabilistisch Ontwerpen"
            // by ir. A.C.W.M. Vrouwenvelder and ir.J.K. Vrijling 5th reprint 1987.
            double sigmaLogOverMuLog = Distribution.CoefficientOfVariation;
            double sigmaNormal = Math.Sqrt(Math.Log(sigmaLogOverMuLog * sigmaLogOverMuLog + 1.0));
            double muNormal = Math.Log(Distribution.Mean) - 0.5 * sigmaNormal * sigmaNormal;
            return DetermineDesignValue(muNormal, sigmaNormal);
        }

        /// <summary>
        /// Determines the design value based on a 'normal space' expected value and variation coefficient.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="variationCoefficient">The standard deviation.</param>
        /// <returns>The design value</returns>
        private double DetermineDesignValue(double expectedValue, double variationCoefficient)
        {
            // Design factor is determined using the 'probit function', which is the inverse
            // CDF function of the standard normal distribution. For more information see:
            // "Quantile function" https://en.wikipedia.org/wiki/Normal_distribution
            double designFactor = Normal.InvCDF(0.0, 1.0, Percentile);
            return expectedValue + designFactor * variationCoefficient;
        }

        private RoundedDouble ProjectFromNormalToLogNormalSpace(double normalSpaceDesignValue)
        {
            return new RoundedDouble(Distribution.Mean.NumberOfDecimalPlaces, Math.Exp(normalSpaceDesignValue));
        }
    }
}
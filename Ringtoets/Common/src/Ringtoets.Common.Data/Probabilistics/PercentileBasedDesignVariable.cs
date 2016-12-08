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
using MathNet.Numerics.Distributions;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// This class is a representation of a variable derived from a probabilistic distribution,
    /// based on a percentile.
    /// </summary>
    /// <typeparam name="TDistributionType">The type of the underlying distribution from which a value is 
    /// derived.</typeparam>
    public abstract class PercentileBasedDesignVariable<TDistributionType> : DesignVariable<TDistributionType> where TDistributionType : IDistribution
    {
        private double percentile;

        /// <summary>
        /// Initializes a new instance of the <see cref="PercentileBasedDesignVariable{TDistributionType}"/> class with 
        /// <see cref="Percentile"/> equal to 0.5.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="DesignVariable{TDistributionType}.Distribution"/> is 
        /// <c>null</c>.</exception>
        protected PercentileBasedDesignVariable(TDistributionType distribution) : base(distribution)
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
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.DesignVariable_Percentile_must_be_in_range);
                }
                percentile = value;
            }
        }

        /// <summary>
        /// Determines the design value based on a 'normal space' expected value and standard deviation.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="standardDeviation">The standard deviation.</param>
        /// <returns>The design value</returns>
        protected double DetermineDesignValue(double expectedValue, double standardDeviation)
        {
            // Design factor is determined using the 'probit function', which is the inverse
            // CDF function of the standard normal distribution. For more information see:
            // "Quantile function" https://en.wikipedia.org/wiki/Normal_distribution
            double designFactor = Normal.InvCDF(0.0, 1.0, Percentile);
            return expectedValue + designFactor*standardDeviation;
        }
    }
}
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

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// This class is a representation of a variable derived from a probabilistic distribution,
    /// based on a percentile.
    /// </summary>
    public abstract class DesignVariable<DistributionType> where DistributionType : IDistribution
    {
        private double percentile;
        private DistributionType distribution;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignVariable{T}"/> class with 
        /// <see cref="Percentile"/> equal to 0.5.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="Distribution"/> is null.</exception>
        protected DesignVariable(DistributionType distribution)
        {
            Distribution = distribution;
            percentile = 0.5;
        }

        /// <summary>
        /// Gets or sets the probabilistic distribution of the parameter being modeled.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public DistributionType Distribution
        {
            get
            {
                return distribution;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", Resources.DesignVariable_GetDesignValue_Distribution_must_be_set);
                }
                distribution = value;
            }
        }

        /// <summary>
        /// Gets or sets the percentile used to derive a deterministic value based on <see cref="Distribution"/>.
        /// </summary>
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
        /// Gets the design value based on the <see cref="Distribution"/> and <see cref="Percentile"/>.
        /// </summary>
        /// <returns>A design value.</returns>
        public abstract double GetDesignValue();

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
            var designFactor = Normal.InvCDF(0.0, 1.0, Percentile);
            return expectedValue + designFactor * standardDeviation;
        }
    }
}
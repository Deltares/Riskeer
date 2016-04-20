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
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// Class representing a log-normal distribution.
    /// </summary>
    public class LognormalDistribution : IDistribution
    {
        private RoundedDouble standardDeviation;
        private RoundedDouble mean;

        /// <summary>
        /// Initializes a new instance of the <see cref="LognormalDistribution"/> class,
        /// initialized as the standard log-normal distribution (mu=0, sigma=1).
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [1, <see cref="RoundedDouble.MaximumNumberOfDecimalPlaces"/>].
        /// </exception>
        public LognormalDistribution(int numberOfDecimalPlaces)
        {
            if (numberOfDecimalPlaces == 0)
            {
                // This causes the default initialization set mean to 0, which is invalid.
                throw new ArgumentOutOfRangeException("numberOfDecimalPlaces",
                                                      "Value must be in range [1, 15].");
            }
            // Simplified calculation mean and standard deviation given mu=0 and sigma=1.
            mean = new RoundedDouble(numberOfDecimalPlaces, Math.Exp(-0.5));
            standardDeviation = new RoundedDouble(numberOfDecimalPlaces, Math.Sqrt((Math.Exp(1) - 1)*Math.Exp(1)));
        }

        /// <summary>
        /// Gets or sets the mean (expected value, E(X)) of the distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Expected value is less then or equal to 0.</exception>
        public RoundedDouble Mean
        {
            get
            {
                return mean;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.LognormalDistribution_Mean_must_be_greater_equal_to_zero);
                }
                mean = value.ToPrecision(mean.NumberOfDecimalPlaces);
            }
        }

        public RoundedDouble StandardDeviation
        {
            get
            {
                return standardDeviation;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.StandardDeviation_Should_be_greater_than_or_equal_to_zero);
                }
                standardDeviation = value.ToPrecision(standardDeviation.NumberOfDecimalPlaces);
            }
        }
    }
}
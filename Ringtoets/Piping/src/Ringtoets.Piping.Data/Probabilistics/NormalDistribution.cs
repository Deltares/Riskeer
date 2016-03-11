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

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// Class representing a normal (or Gaussian) distribution.
    /// </summary>
    public class NormalDistribution : IDistribution
    {
        private RoundedDouble standardDeviation;
        private RoundedDouble mean;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalDistribution"/> class,
        /// initialized as the standard normal distribution.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places of the distribution.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, <see cref="RoundedDouble.MaximumNumberOfDecimalPlaces"/>].
        /// </exception>
        public NormalDistribution(int numberOfDecimalPlaces)
        {
            mean = new RoundedDouble(numberOfDecimalPlaces, 0.0);
            standardDeviation = new RoundedDouble(numberOfDecimalPlaces, 1.0);
        }

        public RoundedDouble Mean
        {
            get
            {
                return mean;
            }
            set
            {
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
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
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// Class representing a log-normal distribution expressed in terms of standard deviation.
    /// </summary>
    /// <seealso cref="VariationCoefficientLogNormalDistribution"/>
    public class LogNormalDistribution : IDistribution
    {
        private RoundedDouble mean;
        private RoundedDouble standardDeviation;
        private RoundedDouble shift;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogNormalDistribution"/> class,
        /// initialized as the standard log-normal distribution (mu=0, sigma=1, theta=0) 
        /// with the amount of decimals equal to <see cref="RoundedDouble.MaximumNumberOfDecimalPlaces"/>. 
        /// </summary>
        public LogNormalDistribution() : this(RoundedDouble.MaximumNumberOfDecimalPlaces) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="LogNormalDistribution"/> class,
        /// initialized as the standard log-normal distribution (mu=0, sigma=1, theta=0).
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [1, <see cref="RoundedDouble.MaximumNumberOfDecimalPlaces"/>].
        /// </exception>
        public LogNormalDistribution(int numberOfDecimalPlaces)
        {
            if (numberOfDecimalPlaces == 0)
            {
                // This causes the default initialization set mean to 0, which is invalid.
                throw new ArgumentOutOfRangeException(nameof(numberOfDecimalPlaces),
                                                      @"Value must be in range [1, 15].");
            }

            // Initialize mean, standard deviation and shift of the normal distribution which is the log of the 
            // log-normal distribution with scale parameter mu=0, shape parameter sigma=1 and location parameter theta=0.
            mean = new RoundedDouble(numberOfDecimalPlaces, Math.Exp(-0.5));
            standardDeviation = new RoundedDouble(numberOfDecimalPlaces, Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1)));
            shift = new RoundedDouble(numberOfDecimalPlaces);
        }

        /// <summary>
        /// Gets or sets the shift of the normal distribution which is the log of the log-normal distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the shift is larger than the mean.</exception>
        public RoundedDouble Shift
        {
            get
            {
                return shift;
            }
            set
            {
                RoundedDouble newShift = value.ToPrecision(shift.NumberOfDecimalPlaces);
                if (newShift > Mean)
                {
                    throw new ArgumentOutOfRangeException(null, Resources.LogNormalDistribution_Shift_may_not_exceed_Mean);
                }

                shift = newShift;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the normal distribution which is the log of the
        /// log-normal distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is less than or
        /// equal to 0 or less than <see cref="Shift"/>.</exception>
        public RoundedDouble Mean
        {
            get
            {
                return mean;
            }
            set
            {
                RoundedDouble roundedValue = value.ToPrecision(mean.NumberOfDecimalPlaces);

                if (roundedValue <= 0)
                {
                    throw new ArgumentOutOfRangeException(null, Resources.LogNormalDistribution_Mean_must_be_greater_than_zero);
                }

                if (Shift > roundedValue)
                {
                    throw new ArgumentOutOfRangeException(null, Resources.LogNormalDistribution_Shift_may_not_exceed_Mean);
                }

                mean = roundedValue;
            }
        }

        /// <summary>
        /// Gets or sets the standard deviation of the normal distribution which is the
        /// log of the log-normal distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when standard deviation
        /// is less than 0.</exception>
        public RoundedDouble StandardDeviation
        {
            get
            {
                return standardDeviation;
            }
            set
            {
                RoundedDouble roundedValue = value.ToPrecision(standardDeviation.NumberOfDecimalPlaces);

                if (roundedValue < 0)
                {
                    throw new ArgumentOutOfRangeException(null, Resources.StandardDeviation_Should_be_greater_or_equal_zero);
                }

                standardDeviation = roundedValue;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((LogNormalDistribution) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Mean.GetHashCode();
                hashCode = (hashCode * 397) ^ StandardDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ Shift.GetHashCode();
                return hashCode;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        private bool Equals(LogNormalDistribution other)
        {
            return Mean.Equals(other.Mean)
                   && StandardDeviation.Equals(other.StandardDeviation)
                   && Shift.Equals(other.Shift);
        }
    }
}
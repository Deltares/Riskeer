// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
    /// Class representing a log-normal distribution expressed in terms of a coefficient
    /// of variation.
    /// </summary>
    /// <seealso cref="LogNormalDistribution"/>
    public class VariationCoefficientLogNormalDistribution : IVariationCoefficientDistribution
    {
        private RoundedDouble mean;
        private RoundedDouble coefficientOfVariation;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariationCoefficientLogNormalDistribution"/> class,
        /// initialized as a log normal distribution (mean=1 and coefficient of variation, CV=1) and with 
        /// the amount of decimal places equal to <see cref="RoundedDouble.MaximumNumberOfDecimalPlaces"/>.
        /// </summary>
        public VariationCoefficientLogNormalDistribution() : this(RoundedDouble.MaximumNumberOfDecimalPlaces) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="VariationCoefficientLogNormalDistribution"/> class,
        /// initialized as a log normal distribution (mean=1 and coefficient of variation, CV=1).
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, <see cref="RoundedDouble.MaximumNumberOfDecimalPlaces"/>].
        /// </exception>
        public VariationCoefficientLogNormalDistribution(int numberOfDecimalPlaces)
        {
            mean = new RoundedDouble(numberOfDecimalPlaces, 1.0);
            coefficientOfVariation = new RoundedDouble(numberOfDecimalPlaces, 1.0);
        }

        /// <summary>
        /// Gets or sets the mean (expected value, E(X)) of the distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Expected value is less than or equal to 0.</exception>
        /// <remarks>As <see cref="CoefficientOfVariation"/> cannot be negative, the absolute
        /// value of the mean is used when the standard deviation needs to be calculated.</remarks>
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

                mean = roundedValue;
            }
        }

        public RoundedDouble CoefficientOfVariation
        {
            get
            {
                return coefficientOfVariation;
            }
            set
            {
                RoundedDouble roundedValue = value.ToPrecision(coefficientOfVariation.NumberOfDecimalPlaces);

                if (roundedValue < 0)
                {
                    throw new ArgumentOutOfRangeException(null, Resources.CoefficientOfVariation_Should_be_greater_or_equal_to_zero);
                }

                coefficientOfVariation = roundedValue;
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
            return Equals((VariationCoefficientLogNormalDistribution) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Mean.GetHashCode();
                hashCode = (hashCode * 397) ^ CoefficientOfVariation.GetHashCode();
                return hashCode;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        private bool Equals(VariationCoefficientLogNormalDistribution other)
        {
            return Mean.Equals(other.Mean)
                   && CoefficientOfVariation.Equals(other.CoefficientOfVariation);
        }
    }
}
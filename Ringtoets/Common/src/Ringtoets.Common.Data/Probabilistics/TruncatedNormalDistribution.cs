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

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// Class representing a truncated normal distribution expressed in terms of standard
    /// deviation.
    /// </summary>
    public class TruncatedNormalDistribution : NormalDistribution
    {
        private RoundedDouble lowerBoundary;
        private RoundedDouble upperBoundary;

        /// <summary>
        /// Initializes a new instance of the <see cref="TruncatedNormalDistribution"/> class.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places of the distribution.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, <see cref="RoundedDouble.MaximumNumberOfDecimalPlaces"/>].
        /// </exception>
        public TruncatedNormalDistribution(int numberOfDecimalPlaces) : base(numberOfDecimalPlaces)
        {
            lowerBoundary = new RoundedDouble(numberOfDecimalPlaces);
            upperBoundary = new RoundedDouble(numberOfDecimalPlaces);
        }

        /// <summary>X
        /// Gets or sets the lower boundary of the distribution.
        /// </summary>
        public RoundedDouble LowerBoundary
        {
            get
            {
                return lowerBoundary;
            }
            set
            {
                lowerBoundary = value.ToPrecision(lowerBoundary.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the upper boundary of the distribution.
        /// </summary>
        public RoundedDouble UpperBoundary
        {
            get
            {
                return upperBoundary;
            }
            set
            {
                upperBoundary = value.ToPrecision(upperBoundary.NumberOfDecimalPlaces);
            }
        }
    }
}
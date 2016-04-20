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

using Core.Common.Base.Data;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// Class represents a specialized case of <see cref="LognormalDistribution"/> that has
    /// been shifted along the X-axis.
    /// </summary>
    public class ShiftedLognormalDistribution : LognormalDistribution
    {
        private RoundedDouble shift;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftedLognormalDistribution"/> class,
        /// initialized as the standard log-normal distribution (mu=0, sigma=1).
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, <see cref="RoundedDouble.MaximumNumberOfDecimalPlaces"/>].
        /// </exception>
        public ShiftedLognormalDistribution(int numberOfDecimalPlaces) : base(numberOfDecimalPlaces)
        {
            shift = new RoundedDouble(numberOfDecimalPlaces);
        }

        /// <summary>
        /// Gets or sets the shift applied to the log-normal distribution.
        /// </summary>
        public RoundedDouble Shift
        {
            get
            {
                return shift;
            }
            set
            {
                shift = value.ToPrecision(shift.NumberOfDecimalPlaces);
            }
        }
    }
}
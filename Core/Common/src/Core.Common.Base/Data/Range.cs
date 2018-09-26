// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// Defines a range of values with inclusive bounds.
    /// </summary>
    /// <typeparam name="T">The variable type used to define the range in.</typeparam>
    public class Range<T> : IFormattable where T : IComparable<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="Range{T}"/>.
        /// </summary>
        /// <param name="min">The minimum bound of the range.</param>
        /// <param name="max">The maximum bound of the range.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="min"/> is greater
        /// then <paramref name="max"/>.</exception>
        public Range(T min, T max)
        {
            if (min.CompareTo(max) > 0)
            {
                throw new ArgumentException(@"Minimum must be smaller or equal to Maximum.", nameof(min));
            }

            Minimum = min;
            Maximum = max;
        }

        /// <summary>
        /// Gets the minimum bound of the range.
        /// </summary>
        public T Minimum { get; }

        /// <summary>
        /// Gets the maximum bound of the range.
        /// </summary>
        public T Maximum { get; }

        /// <summary>
        /// Checks if an value falls within the inclusive bounds of the range.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if the value falls within the inclusive bounds, or <c>false</c> otherwise.</returns>
        public bool InRange(T value)
        {
            return Minimum.CompareTo(value) <= 0 && Maximum.CompareTo(value) >= 0;
        }

        public override string ToString()
        {
            return $"[{Minimum}, {Maximum}]";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var formattableMinimum = Minimum as IFormattable;
            if (formattableMinimum != null)
            {
                var formattableMaximum = (IFormattable) Maximum;
                return $"[{formattableMinimum.ToString(format, formatProvider)}, {formattableMaximum.ToString(format, formatProvider)}]";
            }

            return ToString();
        }
    }
}
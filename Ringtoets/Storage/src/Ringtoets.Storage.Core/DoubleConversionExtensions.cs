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

namespace Ringtoets.Storage.Core
{
    /// <summary>
    /// Class that contains extension methods for <see cref="double"/> to convert them to
    /// other value types.
    /// </summary>
    public static class DoubleConversionExtensions
    {
        /// <summary>
        /// Converts a <see cref="double"/> to a <see cref="Nullable{T}"/> <see cref="double"/>. If
        /// <paramref name="value"/> is <see cref="double.NaN"/>, the result is <c>null</c>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><c>null</c> if <paramref name="value"/> is <see cref="double.NaN"/>, the value
        /// of <paramref name="value"/> otherwise.</returns>
        public static double? ToNaNAsNull(this double value)
        {
            if (double.IsNaN(value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        /// Converts a <see cref="Nullable{T}"/> <see cref="double"/> to a <see cref="double"/>. If
        /// <paramref name="value"/> is <c>null</c>, the result is <see cref="double.NaN"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><see cref="double.NaN"/> if <paramref name="value"/> is <c>null</c>, the value
        /// of <paramref name="value"/> otherwise.</returns>
        public static double ToNullAsNaN(this double? value)
        {
            if (value == null)
            {
                return double.NaN;
            }

            return value.Value;
        }
    }
}
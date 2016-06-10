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

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// Class that contains extension methods for <see cref="decimal"/> to convert them to
    /// other value types.
    /// </summary>
    public static class DecimalConversionExtensions
    {
        /// <summary>
        /// Converts a nullable <see cref="decimal"/> into a <see cref="double"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><see cref="double.NaN"/> when <paramref name="value"/> is <c>null</c>,
        /// or the <see cref="double"/> representation of the <paramref name="value"/>
        /// otherwise.</returns>
        public static double ToNanableDouble(this decimal? value)
        {
            if (value.HasValue)
            {
                return Convert.ToDouble(value);
            }
            return double.NaN;
        }
    }
}
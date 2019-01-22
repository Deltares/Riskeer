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
using System.Drawing;

namespace Riskeer.Storage.Core
{
    /// <summary>
    /// Class that contains extension methods for <see cref="Color"/> to convert them to
    /// other value types.
    /// </summary>
    internal static class ColorConversionExtensions
    {
        /// <summary>
        /// Convert <paramref name="value"/> to its 64-bit ARGB value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The 64-bit ARGB value, or null when the
        /// color is empty.</returns>
        public static long? ToInt64(this Color value)
        {
            return value.IsEmpty
                       ? (long?) null
                       : value.ToArgb();
        }

        /// <summary>
        /// Converts a 32-bit ARGB value to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The 32-bit ARGB value to convert.</param>
        /// <returns>The created <see cref="Color"/>.</returns>
        public static Color ToColor(this long? value)
        {
            return value == null ? Color.Empty : Color.FromArgb(Convert.ToInt32(value));
        }
    }
}
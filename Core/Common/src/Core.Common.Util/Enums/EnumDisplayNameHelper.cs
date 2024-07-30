// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.ComponentModel;

namespace Core.Common.Util.Enums
{
    /// <summary>
    /// Helper class to retrieve display names for <see cref="Enum"/>.
    /// </summary>
    public static class EnumDisplayNameHelper
    {
        /// <summary>
        /// Gets the display name from a given <see cref="Enum"/>.
        /// </summary>
        /// <param name="value">The <see cref="Enum"/> to get the display name for.</param>
        /// <typeparam name="TEnum">The type of <see cref="Enum"/> to get the display name for.</typeparam>
        /// <returns>The display name.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value"/> is an
        /// invalid <typeparamref name="TEnum"/>.</exception>
        public static string GetDisplayName<TEnum>(TEnum value)
            where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new InvalidEnumArgumentException(nameof(value), Convert.ToInt32(value), typeof(TEnum));
            }

            return new EnumDisplayWrapper<TEnum>(value).DisplayName;
        }
    }
}
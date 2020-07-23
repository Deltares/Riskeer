// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.Util;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for generating <see cref="EnumDisplayWrapper{T}"/> from enum values.
    /// </summary>
    public static class EnumDisplayWrapperHelper
    {
        /// <summary>
        /// Creates a collection of <see cref="EnumDisplayWrapper{T}"/> from enum values.
        /// </summary>
        /// <typeparam name="T">The type of enum.</typeparam>
        /// <returns>A collection of <see cref="EnumDisplayWrapper{T}"/>.</returns>
        public static EnumDisplayWrapper<T>[] GetEnumTypes<T>()
        {
            return Enum.GetValues(typeof(T))
                       .OfType<T>()
                       .Select(et => new EnumDisplayWrapper<T>(et))
                       .ToArray();
        }
    }
}

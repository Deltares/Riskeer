﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Drawing;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// This class provides helpers for converting values from the D-Soil Model database into
    /// <see cref="Color"/>.
    /// </summary>
    public static class SoilLayerColorConverter
    {
        /// <summary>
        /// Converts a nullable <see cref="double"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="colorValue">The value to convert.</param>
        /// <returns>A <see cref="Color"/> instance based on the <paramref name="colorValue"/>.</returns>
        public static Color Convert(double? colorValue)
        {
            return colorValue == null
                       ? Color.Empty
                       : Color.FromArgb(System.Convert.ToInt32(colorValue));
        }
    }
}
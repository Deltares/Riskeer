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

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// This class provides helpers for converting values from D-Soil Model database
    /// into valid valued of the 'is aquifer' of soil layers.
    /// </summary>
    public static class SoilLayerIsAquiferConverter
    {
        private const double tolerance = 1e-6;

        /// <summary>
        /// Converts a nullable <see cref="double"/> to a <see cref="bool"/>.
        /// </summary>
        /// <param name="isAquifer">The value to convert.</param>
        /// <returns>A <see cref="bool"/> based on the <paramref name="isAquifer"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="isAquifer"/>
        /// cannot be converted.</exception>
        public static bool Convert(double? isAquifer)
        {
            if (isAquifer.HasValue)
            {
                if (Math.Abs(0.0 - isAquifer.Value) < tolerance)
                {
                    return false;
                }

                if (Math.Abs(1.0 - isAquifer.Value) < tolerance)
                {
                    return true;
                }
            }

            throw new NotSupportedException($"A value of {isAquifer} for {nameof(isAquifer)} cannot be converted to a valid boolean.");
        }
    }
}
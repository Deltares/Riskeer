﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class provides helpers for converting values from DSoil Model database
    /// into valid valued of the 'is aquifer' of soil layers.
    /// </summary>
    public static class SoilLayerIsAquiferConverter
    {
        /// <summary>
        /// Converts a nullable <see cref="double"/> to a <see cref="bool"/>.
        /// </summary>
        /// <param name="isAquifer">The value to convert.</param>
        /// <returns>A <see cref="bool"/> based on the <paramref name="isAquifer"/>.
        /// </returns>
        /// <exception cref="ImportedDataTransformException">Thrown when <paramref name="isAquifer"/>
        /// cannot be converted.</exception>
        public static bool Convert(double? isAquifer)
        {
            if (isAquifer.HasValue)
            {
                if (isAquifer.Equals(0.0))
                {
                    return false;
                }
                if (isAquifer.Equals(1.0))
                {
                    return true;
                }
            }

            throw new ImportedDataTransformException(string.Format(Resources.Convert_Invalid_value_ParameterName_0,
                                                                   Resources.SoilLayerProperties_IsAquifer_Description));
        }
    }
}
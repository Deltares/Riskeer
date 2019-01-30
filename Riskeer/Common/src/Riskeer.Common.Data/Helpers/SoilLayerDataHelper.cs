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

using System.Drawing;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.Helpers
{
    /// <summary>
    /// Helper class for getting valid soil layer data.
    /// </summary>
    public static class SoilLayerDataHelper
    {
        /// <summary>
        /// Gets a valid name for the soil layer.
        /// </summary>
        /// <param name="name">The name to turn into a valid name.</param>
        /// <returns><paramref name="name"/> when it's not <see cref="string.IsNullOrWhiteSpace"/>;
        /// a default value otherwise.</returns>
        public static string GetValidName(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                       ? Resources.SoilLayerDataHelper_GetValidName_Unknown
                       : name;
        }

        /// <summary>
        /// Gets a valid color for the soil layer.
        /// </summary>
        /// <param name="color">The color to turn into a valid color.</param>
        /// <returns><paramref name="color"/> when it's not <see cref="Color.Empty"/>;
        /// a default value otherwise.</returns>
        public static Color GetValidColor(Color color)
        {
            return color == Color.Empty
                       ? Color.White
                       : color;
        }
    }
}
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

using System.Drawing;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for visualizing soil layer data.
    /// </summary>
    public static class SoilLayerVisualizationHelper
    {
        /// <summary>
        /// Gets the display name of the soil layer.
        /// </summary>
        /// <param name="name">The name to check whether it's a valid display name.</param>
        /// <returns><paramref name="name"/> when it's not <see cref="string.IsNullOrWhiteSpace"/>;
        /// a default value otherwise.</returns>
        public static string GetDisplayName(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                       ? Resources.SoilLayerVisualizationHelper_GetDisplayName_Unknown
                       : name;
        }

        /// <summary>
        /// Gets the display color of the soil layer.
        /// </summary>
        /// <param name="color">The color to check whether it's a valid display color.</param>
        /// <returns><paramref name="color"/> when it's not <see cref="Color.Empty"/>;
        /// a default value otherwise.</returns>
        public static Color GetDisplayColor(Color color)
        {
            return color == Color.Empty
                       ? Color.White
                       : color;
        }
    }
}
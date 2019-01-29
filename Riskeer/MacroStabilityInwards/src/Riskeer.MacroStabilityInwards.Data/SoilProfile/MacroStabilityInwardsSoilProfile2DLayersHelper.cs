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

using System.Collections.Generic;
using System.Linq;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// Helper methods for dealing with 2D layers in <see cref="IMacroStabilityInwardsSoilProfile{T}"/>.
    /// </summary>
    public static class MacroStabilityInwardsSoilProfile2DLayersHelper
    {
        /// <summary>
        /// Method for obtaining all <see cref="MacroStabilityInwardsSoilLayer2D"/> in <paramref name="layers"/> recursively.
        /// </summary>
        /// <param name="layers">The collection of layers to recursively get the layers from.</param>
        /// <returns>An enumerable with <see cref="MacroStabilityInwardsSoilLayer2D"/> or an empty enumerable
        /// when <paramref name="layers"/> is <c>null</c>.</returns>
        public static IEnumerable<MacroStabilityInwardsSoilLayer2D> GetLayersRecursively(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers)
        {
            return layers?.SelectMany(GetLayersRecursively) ?? Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>();
        }

        private static IEnumerable<MacroStabilityInwardsSoilLayer2D> GetLayersRecursively(MacroStabilityInwardsSoilLayer2D layer)
        {
            var layers = new List<MacroStabilityInwardsSoilLayer2D>
            {
                layer
            };

            foreach (MacroStabilityInwardsSoilLayer2D nestedLayer in layer.NestedLayers)
            {
                layers.AddRange(GetLayersRecursively(nestedLayer));
            }

            return layers;
        }
    }
}
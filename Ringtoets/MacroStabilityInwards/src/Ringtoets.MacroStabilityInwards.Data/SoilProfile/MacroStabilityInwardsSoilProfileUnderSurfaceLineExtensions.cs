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

using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// Extension methods dealing with <see cref="IMacroStabilityInwardsSoilProfileUnderSurfaceLine"/> instances.
    /// </summary>
    public static class MacroStabilityInwardsSoilProfileUnderSurfaceLineExtensions
    {
        /// <summary>
        /// Method for obtaining all <see cref="IMacroStabilityInwardsSoilLayer2D"/> in <param name="profile"/> recursively.
        /// </summary>
        /// <param name="profile">The profile to get the layers from.</param>
        /// <returns>An enumerable with <see cref="IMacroStabilityInwardsSoilLayer2D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <param name="profile"/> is <c>null</c>.</exception>
        public static IEnumerable<IMacroStabilityInwardsSoilLayer2D> GetLayersRecursively(this IMacroStabilityInwardsSoilProfileUnderSurfaceLine profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return profile.Layers.SelectMany(GetLayersRecursively);
        }

        private static IEnumerable<IMacroStabilityInwardsSoilLayer2D> GetLayersRecursively(IMacroStabilityInwardsSoilLayer2D layer)
        {
            var layers = new List<IMacroStabilityInwardsSoilLayer2D>
            {
                layer
            };

            foreach (IMacroStabilityInwardsSoilLayer2D nestedLayer in layer.NestedLayers)
            {
                layers.AddRange(GetLayersRecursively(nestedLayer));
            }

            return layers;
        }
    }
}
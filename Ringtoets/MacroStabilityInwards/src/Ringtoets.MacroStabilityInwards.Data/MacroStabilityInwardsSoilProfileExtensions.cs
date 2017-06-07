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

using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="MacroStabilityInwardsSoilProfile"/> instances.
    /// </summary>
    public static class MacroStabilityInwardsSoilProfileExtensions
    {
        /// <summary>
        /// Retrieves the thickness of the consecutive aquifer layers (if any) which are (partly) under a certain <paramref name="level"/>.
        /// Only the thickness of the part of the aquifer layer under the level is determined. 
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="MacroStabilityInwardsSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the aquifer layers are sought.</param>
        /// <returns>The thickness of the part of the consecutive aquifer layer(s) (partly) under the <paramref name="level"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the bottommost <see cref= "MacroStabilityInwardsSoilLayer" /> is not part of
        /// <paramref name="soilProfile"/>.</exception>
        public static double GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(this MacroStabilityInwardsSoilProfile soilProfile, double level)
        {
            return TotalThicknessOfConsecutiveLayersBelowLevel(
                soilProfile,
                level,
                soilProfile.GetConsecutiveAquiferLayersBelowLevel(level).ToArray());
        }

        /// <summary>
        /// Retrieves the thickness of the consecutive coverage layers (if any) which are (partly) under a certain <paramref name="level"/>.
        /// Only the thickness of the part of the coverage layer under the level is determined. 
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="MacroStabilityInwardsSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the coverage layers are sought.</param>
        /// <returns>The thickness of the part of the consecutive coverage layer(s) (partly) under the <paramref name="level"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the bottommost<see cref= "MacroStabilityInwardsSoilLayer" /> is not part of
        /// <paramref name="soilProfile"/>.</exception>
        public static double GetConsecutiveCoverageLayerThicknessBelowLevel(this MacroStabilityInwardsSoilProfile soilProfile, double level)
        {
            return TotalThicknessOfConsecutiveLayersBelowLevel(
                soilProfile,
                level,
                soilProfile.GetConsecutiveCoverageLayersBelowLevel(level).ToArray());
        }

        /// <summary>
        /// Retrieves the collection of aquifer layers below a certain <paramref name="level"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="MacroStabilityInwardsSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the aquifer layers are sought.</param>
        /// <returns>The collection of consecutive aquifer layer(s) (partly) under the <paramref name="level"/>.</returns>
        public static IEnumerable<MacroStabilityInwardsSoilLayer> GetConsecutiveAquiferLayersBelowLevel(this MacroStabilityInwardsSoilProfile soilProfile, double level)
        {
            return GetConsecutiveLayers(soilProfile, level, true);
        }

        /// <summary>
        /// Retrieves the collection of aquitard layers below a certain <paramref name="level"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="MacroStabilityInwardsSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the aquitard layers are sought.</param>
        /// <returns>The collection of consecutive aquitard layer(s) (partly) under the <paramref name="level"/>.</returns>
        public static IEnumerable<MacroStabilityInwardsSoilLayer> GetConsecutiveCoverageLayersBelowLevel(this MacroStabilityInwardsSoilProfile soilProfile, double level)
        {
            MacroStabilityInwardsSoilLayer topAquiferLayer = soilProfile.GetConsecutiveAquiferLayersBelowLevel(level).FirstOrDefault();
            if (topAquiferLayer != null)
            {
                MacroStabilityInwardsSoilLayer[] aquitardLayers = GetConsecutiveLayers(soilProfile, level, false).ToArray();
                if (aquitardLayers.Any() && topAquiferLayer.Top < aquitardLayers.First().Top)
                {
                    return aquitardLayers;
                }
            }
            return Enumerable.Empty<MacroStabilityInwardsSoilLayer>();
        }

        /// <summary>
        /// Calculates the thickness of a collection of consecutive <see cref="MacroStabilityInwardsSoilLayer"/>.
        /// </summary>
        /// <param name="soilProfile">The <see cref="MacroStabilityInwardsSoilProfile"/> containing the <paramref name="layers"/>.</param>
        /// <param name="level">The level under which to calculate the total thickness.</param>
        /// <param name="layers">Collection of consecutive <see cref="MacroStabilityInwardsSoilLayer"/>, ordered by 
        /// <see cref="MacroStabilityInwardsSoilLayer.Top"/> which are part of <paramref name="soilProfile"/>.</param>
        /// <returns>The total thickness of the consecutive layers below the given <paramref name="level"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="soilProfile"/> or
        /// <paramref name="layers"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the bottommost <see cref="MacroStabilityInwardsSoilLayer"/> is not part of
        /// <paramref name="soilProfile"/>.</exception>
        private static double TotalThicknessOfConsecutiveLayersBelowLevel(MacroStabilityInwardsSoilProfile soilProfile, double level, MacroStabilityInwardsSoilLayer[] layers)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }
            if (layers == null)
            {
                throw new ArgumentNullException(nameof(layers));
            }
            if (layers.Length == 0)
            {
                return double.NaN;
            }

            MacroStabilityInwardsSoilLayer bottomLayer = layers.Last();
            MacroStabilityInwardsSoilLayer topLayer = layers.First();

            return Math.Min(topLayer.Top, level) - (bottomLayer.Top - soilProfile.GetLayerThickness(bottomLayer));
        }

        /// <summary>
        /// Gets consecutive layers in the <see cref="soilProfile"/> which have an aquifer property of <paramref name="isAquifer"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="MacroStabilityInwardsSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the aquitard layers are sought.</param>
        /// <param name="isAquifer">Value indicating whether the consecutive layers should be aquifer or aquitard.</param>
        /// <returns>The collection of consecutive layer(s) with an aquifer property equal to <paramref name="isAquifer"/>
        /// under the <paramref name="level"/>.</returns>
        private static IEnumerable<MacroStabilityInwardsSoilLayer> GetConsecutiveLayers(MacroStabilityInwardsSoilProfile soilProfile, double level, bool isAquifer)
        {
            if (level < soilProfile.Bottom)
            {
                yield break;
            }

            var yielding = false;
            foreach (MacroStabilityInwardsSoilLayer soilLayer in soilProfile.Layers)
            {
                if (soilLayer.IsAquifer == isAquifer && IsSoilLayerPartlyBelowLevel(soilProfile, soilLayer, level))
                {
                    yielding = true;
                    yield return soilLayer;
                }

                if (yielding && soilLayer.IsAquifer != isAquifer)
                {
                    yield break;
                }
            }
        }

        private static bool IsSoilLayerPartlyBelowLevel(MacroStabilityInwardsSoilProfile soilProfile, MacroStabilityInwardsSoilLayer soilLayer, double level)
        {
            return soilLayer.Top < level || soilLayer.Top - soilProfile.GetLayerThickness(soilLayer) < level;
        }
    }
}
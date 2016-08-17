﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data
{
    public static class PipingSoilProfileExtensions
    {
        /// <summary>
        /// Retrieves the thickness of the consecutive aquifer layers (if any) which are (partly) under a certain <paramref name="level"/>.
        /// Only the thickness of the part of the aquifer layer under the level is determined. 
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="PipingSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the aquifer layers are sought.</param>
        /// <returns>The thickness of the part of the consecutive aquifer layer(s) (partly) under the <paramref name="level"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="level"/> is less than <see cref="PipingSoilProfile.Bottom"/>.</exception>
        public static double GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(this PipingSoilProfile soilProfile, double level)
        {
            var aquiferLayers = soilProfile.GetConsecutiveAquiferLayersBelowLevel(level).ToArray();

            if (aquiferLayers.Length == 0)
            {
                return double.NaN;
            }

            var bottomLayer = aquiferLayers.Last();
            var topLayer = aquiferLayers.First();

            return Math.Min(topLayer.Top, level) - (bottomLayer.Top - soilProfile.GetLayerThickness(bottomLayer));
        }

        /// <summary>
        /// Retrieves the collection of aquifer layers below a certain <paramref name="level"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="PipingSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the aquifer layers are sought.</param>
        /// <returns>The collection of consecutive aquifer layer(s) (partly) under the <paramref name="level"/>.</returns>
        public static IEnumerable<PipingSoilLayer> GetConsecutiveAquiferLayersBelowLevel(this PipingSoilProfile soilProfile, double level)
        {
            return GetConsecutiveLayers(soilProfile, level, true);
        }

        /// <summary>
        /// Retrieves the collection of aquitard layers below a certain <paramref name="level"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="PipingSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the aquitard layers are sought.</param>
        /// <returns>The collection of consecutive aquitard layer(s) (partly) under the <paramref name="level"/>.</returns>
        public static IEnumerable<PipingSoilLayer> GetConsecutiveCoverageLayersBelowLevel(this PipingSoilProfile soilProfile, double level)
        {
            var topAquiferLayer = soilProfile.GetConsecutiveAquiferLayersBelowLevel(level).FirstOrDefault();
            if (topAquiferLayer != null)
            {
                var aquitardLayers = GetConsecutiveLayers(soilProfile, level, false).ToArray();
                if (aquitardLayers.Any() && topAquiferLayer.Top < aquitardLayers.First().Top)
                {
                    return aquitardLayers;
                }
            }
            return Enumerable.Empty<PipingSoilLayer>();
        }

        /// <summary>
        /// Gets consecutive layers in the <see cref="soilProfile"/> which have an aquifer property of <paramref name="isAquifer"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing <see cref="PipingSoilLayer"/> to consider.</param>
        /// <param name="level">The level under which the aquitard layers are sought.</param>
        /// <param name="isAquifer">Value indicating whether the consecutive layers should be aquifer or aquitard.</param>
        /// <returns>The collection of consecutive layer(s) with an aquifer property equal to <paramref name="isAquifer"/>
        /// under the <paramref name="level"/>.</returns>
        private static IEnumerable<PipingSoilLayer> GetConsecutiveLayers(PipingSoilProfile soilProfile, double level, bool isAquifer)
        {
            if (level < soilProfile.Bottom)
            {
                yield break;
            }

            var yielding = false;
            foreach (var pipingSoilLayer in soilProfile.Layers)
            {
                if (pipingSoilLayer.IsAquifer == isAquifer && IsSoilLayerPartlyBelowLevel(soilProfile, pipingSoilLayer, level))
                {
                    yielding = true;
                    yield return pipingSoilLayer;
                }

                if (yielding && pipingSoilLayer.IsAquifer != isAquifer)
                {
                    yield break;
                }
            }
        }

        private static bool IsSoilLayerPartlyBelowLevel(PipingSoilProfile soilProfile, PipingSoilLayer pipingSoilLayer, double level)
        {
            return (pipingSoilLayer.Top < level || pipingSoilLayer.Top - soilProfile.GetLayerThickness(pipingSoilLayer) < level);
        }
    }
}
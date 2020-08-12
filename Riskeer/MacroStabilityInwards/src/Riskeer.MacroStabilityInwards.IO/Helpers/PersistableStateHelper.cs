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
using System.Collections.Generic;
using System.Linq;
using Components.Persistence.Stability.Data;
using Core.Common.Base.Data;
using Core.Common.Geometry;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Helpers
{
    /// <summary>
    /// Helper class that can be used used while exporting <see cref="PersistableState"/> instances.
    /// </summary>
    public static class PersistableStateHelper
    {
        /// <summary>
        /// Indicator whether a <paramref name="soilProfile"/> has valid state points.
        /// </summary>
        /// <param name="soilProfile">The <see cref="IMacroStabilityInwardsSoilProfileUnderSurfaceLine"/> to get the state points from.</param>
        /// <returns><c>true</c> when every layer in <paramref name="soilProfile"/> has max. 1 state point; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public static bool HasValidStatePoints(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            MacroStabilityInwardsSoilLayer2D[] layers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfile.Layers).ToArray();

            Dictionary<IMacroStabilityInwardsPreconsolidationStress, MacroStabilityInwardsSoilLayer2D> stressesWithLayers =
                soilProfile.PreconsolidationStresses.ToDictionary(pcs => pcs, pcs => GetLayerForPreconsolidationStress(layers, pcs));

            IEnumerable<IGrouping<MacroStabilityInwardsSoilLayer2D, IMacroStabilityInwardsPreconsolidationStress>> duplicateLayers =
                stressesWithLayers.ToLookup(pair => pair.Value, pair => pair.Key).Where(x => x.Count() > 1);

            return !duplicateLayers.Any()
                   && stressesWithLayers.All(pair => pair.Value != null)
                   && !stressesWithLayers.Any(stressWithLayer => stressWithLayer.Value.Data.UsePop && HasValidPop(stressWithLayer.Value.Data.Pop));
        }

        /// <summary>
        /// Indicator whether the <paramref name="pop"/> is valid.
        /// </summary>
        /// <param name="pop">The pop to validate.</param>
        /// <returns><c>true</c> when has no <see cref="RoundedDouble.NaN"/> values; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pop"/> is <c>null</c>.</exception>
        public static bool HasValidPop(IVariationCoefficientDistribution pop)
        {
            if (pop == null)
            {
                throw new ArgumentNullException(nameof(pop));
            }

            return pop.Mean != RoundedDouble.NaN
                   && pop.CoefficientOfVariation != RoundedDouble.NaN;
        }

        /// <summary>
        /// Gets the <see cref="MacroStabilityInwardsSoilLayer2D"/> the <paramref name="preconsolidationStress"/> is placed on.
        /// </summary>
        /// <param name="layers">The layers of the profile.</param>
        /// <param name="preconsolidationStress">The <see cref="IMacroStabilityInwardsPreconsolidationStress"/> to get the layer for.</param>
        /// <returns>The <see cref="MacroStabilityInwardsSoilLayer2D"/> the <paramref name="preconsolidationStress"/> is placed on;
        /// or <c>null</c> when no layer can be found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilLayer2D GetLayerForPreconsolidationStress(
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers, IMacroStabilityInwardsPreconsolidationStress preconsolidationStress)
        {
            if (layers == null)
            {
                throw new ArgumentNullException(nameof(layers));
            }

            if (preconsolidationStress == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStress));
            }

            return layers.SingleOrDefault(l => AdvancedMath2D.PointInPolygon(
                                              preconsolidationStress.Location,
                                              l.OuterRing.Points,
                                              l.NestedLayers.Select(nl => nl.OuterRing.Points)));
        }
    }
}
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
using Deltares.MacroStability.Geometry;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using SoilLayer = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="FixedSoilStress"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    internal static class FixedSoilStressCreator
    {
        /// <summary>
        /// Creates <see cref="FixedSoilStress"/> objects based on the given layers.
        /// </summary>
        /// <param name="layerLookup">The layers to create <see cref="FixedSoilStress"/> for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="FixedSoilStress"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layerLookup"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<FixedSoilStress> Create(IDictionary<SoilLayer, LayerWithSoil> layerLookup)
        {
            if (layerLookup == null)
            {
                throw new ArgumentNullException(nameof(layerLookup));
            }

            return layerLookup.Where(ll => ll.Key.UsePop)
                              .Select(keyValuePair => new FixedSoilStress(keyValuePair.Value.Soil, StressValueType.POP, keyValuePair.Key.Pop))
                              .ToArray();
        }
    }
}
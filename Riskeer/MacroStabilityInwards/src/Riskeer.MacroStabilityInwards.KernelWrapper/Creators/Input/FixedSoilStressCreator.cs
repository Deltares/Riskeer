using System;
using System.Collections.Generic;
using System.Linq;
using Deltares.MacroStability.Geometry;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using SoilLayer = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="FixedSoilStress"/>instances which are required by <see cref="IUpliftVanKernel"/>.
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
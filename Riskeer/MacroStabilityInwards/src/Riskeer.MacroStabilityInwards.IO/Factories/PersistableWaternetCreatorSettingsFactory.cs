using System;
using System.Collections.Generic;
using Components.Persistence.Stability.Data;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableWaternetCreatorSettings"/>.
    /// </summary>
    internal static class PersistableWaternetCreatorSettingsFactory
    {
        /// <summary>
        /// Creates a new collection of <see cref="PersistableWaternetCreatorSettings"/>.
        /// </summary>
        /// <param name="input">The input to use.</param>
        /// <param name="idFactory">The factory fo IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>A collection of <see cref="PersistableWaternetCreatorSettings"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public static IEnumerable<PersistableWaternetCreatorSettings> Create(MacroStabilityInwardsInput input, IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (idFactory == null)
            {
                throw new ArgumentNullException(nameof(idFactory));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            return null;
        }
    }
}
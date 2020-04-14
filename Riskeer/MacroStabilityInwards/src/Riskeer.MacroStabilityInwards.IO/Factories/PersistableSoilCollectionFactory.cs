using System;
using Components.Persistence.Stability.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableSoilCollection"/>.
    /// </summary>
    internal static class PersistableSoilCollectionFactory
    {
        public static PersistableSoilCollection Create(IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile, IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            if (idFactory == null)
            {
                throw new ArgumentNullException(nameof(idFactory));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            return new PersistableSoilCollection();
        }
    }
}
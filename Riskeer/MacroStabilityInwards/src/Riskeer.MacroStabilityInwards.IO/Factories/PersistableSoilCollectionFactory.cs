using System;
using System.Linq;
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
        public static PersistableSoilCollection Create(IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile,
                                                       IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
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

            return new PersistableSoilCollection
            {
                Soils = soilProfile.Layers.Select(l => Create(l, idFactory, registry)).ToArray()
            };
        }

        private static PersistableSoil Create(IMacroStabilityInwardsSoilLayer layer, IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            MacroStabilityInwardsSoilLayerData layerData = layer.Data;

            var soil = new PersistableSoil
            {
                Id = idFactory.Create(),
                Name = layerData.MaterialName,
                IsProbabilistic = true,
                Cohesion = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(layerData).GetDesignValue(),
                CohesionAndFrictionAngleCorrelated = false,
                ShearStrengthRatioAndShearStrengthExponentCorrelated = false
            };

            soil.Code = $"{soil.Name}-{soil.Id}";

            registry.Add(layer, soil.Id);

            return soil;
        }
    }
}
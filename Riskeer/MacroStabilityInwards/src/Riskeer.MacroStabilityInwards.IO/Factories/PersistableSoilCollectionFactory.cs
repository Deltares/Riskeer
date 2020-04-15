using System;
using System.ComponentModel;
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
                CohesionStochasticParameter = PersistableStochasticParameterFactory.Create(layerData.Cohesion),
                FrictionAngle = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(layerData).GetDesignValue(),
                FrictionAngleStochasticParameter = PersistableStochasticParameterFactory.Create(layerData.FrictionAngle),
                ShearStrengthRatio = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(layerData).GetDesignValue(),
                ShearStrengthRatioStochasticParameter = PersistableStochasticParameterFactory.Create(layerData.ShearStrengthRatio),
                StrengthIncreaseExponent = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(layerData).GetDesignValue(),
                StrengthIncreaseExponentStochasticParameter = PersistableStochasticParameterFactory.Create(layerData.ShearStrengthRatio),
                CohesionAndFrictionAngleCorrelated = false,
                ShearStrengthRatioAndShearStrengthExponentCorrelated = false,
                ShearStrengthModelTypeAbovePhreaticLevel = GetShearStrengthModelTypeForAbovePhreaticLevel(layerData.ShearStrengthModel),
                ShearStrengthModelTypeBelowPhreaticLevel = GetShearStrengthModelTypeForBelowPhreaticLevel(layerData.ShearStrengthModel),
                VolumetricWeightAbovePhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(layerData).GetDesignValue(),
                VolumetricWeightBelowPhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(layerData).GetDesignValue()
            };

            soil.Code = $"{soil.Name}-{soil.Id}";

            registry.Add(layer, soil.Id);

            return soil;
        }

        /// <summary>
        /// Gets the <see cref="PersistableShearStrengthModelType"/> for
        /// above phreatic level.
        /// </summary>
        /// <param name="shearStrengthModel">The <see cref="MacroStabilityInwardsShearStrengthModel"/>
        /// to get the <see cref="PersistableShearStrengthModelType"/> for.</param>
        /// <returns>The <see cref="PersistableShearStrengthModelType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shearStrengthModel"/>
        /// has an invalid value for <see cref="MacroStabilityInwardsShearStrengthModel"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// is not supported.</exception>
        private static PersistableShearStrengthModelType GetShearStrengthModelTypeForAbovePhreaticLevel(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsShearStrengthModel), shearStrengthModel))
            {
                throw new InvalidEnumArgumentException(nameof(shearStrengthModel),
                                                       (int)shearStrengthModel,
                                                       typeof(MacroStabilityInwardsShearStrengthModel));
            }

            switch (shearStrengthModel)
            {
                case MacroStabilityInwardsShearStrengthModel.CPhi:
                case MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated:
                    return PersistableShearStrengthModelType.CPhi;
                case MacroStabilityInwardsShearStrengthModel.SuCalculated:
                    return PersistableShearStrengthModelType.Su;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the <see cref="PersistableShearStrengthModelType"/> for
        /// below phreatic level.
        /// </summary>
        /// <param name="shearStrengthModel">The <see cref="MacroStabilityInwardsShearStrengthModel"/>
        /// to get the <see cref="PersistableShearStrengthModelType"/> for.</param>
        /// <returns>The <see cref="PersistableShearStrengthModelType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shearStrengthModel"/>
        /// has an invalid value for <see cref="MacroStabilityInwardsShearStrengthModel"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// is not supported.</exception>
        private static PersistableShearStrengthModelType GetShearStrengthModelTypeForBelowPhreaticLevel(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsShearStrengthModel), shearStrengthModel))
            {
                throw new InvalidEnumArgumentException(nameof(shearStrengthModel),
                                                       (int)shearStrengthModel,
                                                       typeof(MacroStabilityInwardsShearStrengthModel));
            }

            switch (shearStrengthModel)
            {
                case MacroStabilityInwardsShearStrengthModel.CPhi:
                    return PersistableShearStrengthModelType.CPhi;
                case MacroStabilityInwardsShearStrengthModel.SuCalculated:
                case MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated:
                    return PersistableShearStrengthModelType.Su;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
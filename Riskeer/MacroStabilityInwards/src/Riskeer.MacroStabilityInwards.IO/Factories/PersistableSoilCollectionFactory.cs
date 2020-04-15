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
        /// <summary>
        /// Creates a new instance of <see cref="PersistableSoilCollection"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use.</param>
        /// <param name="idFactory">The factory fo IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>The created <see cref="PersistableSoilCollection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when
        /// <see cref="MacroStabilityInwardsShearStrengthModel"/> has an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="MacroStabilityInwardsShearStrengthModel"/> has a valid value but is not supported;</item>
        /// <item>The given <paramref name="soilProfile"/> type is not supported.</item>
        /// </list>
        /// </exception>
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
                Soils = GetLayers(soilProfile).Select(l => Create(l, idFactory, registry)).ToArray()
            };
        }

        /// <summary>
        /// Gets all the layers within <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The <see cref="IMacroStabilityInwardsSoilProfile{T}"/> to get the layers from.</param>
        /// <returns>All layers from <paramref name="soilProfile"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the given <paramref name="soilProfile"/> type
        /// is not supported.</exception>
        private static IEnumerable<IMacroStabilityInwardsSoilLayer> GetLayers(IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile)
        {
            if (soilProfile is MacroStabilityInwardsSoilProfile1D soilProfile1D)
            {
                return soilProfile1D.Layers;
            }

            if (soilProfile is MacroStabilityInwardsSoilProfile2D soilProfile2D)
            {
                return MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfile2D.Layers);
            }

            throw new NotSupportedException($"{soilProfile.GetType().Name} is not supported. " +
                                            $"Supported types are: {nameof(MacroStabilityInwardsSoilProfile1D)} " +
                                            $"and {nameof(MacroStabilityInwardsSoilProfile2D)}.");
        }

        /// <summary>
        /// Creates a new instance of <see cref="PersistableSoil"/>.
        /// </summary>
        /// <param name="layer">The layer to use.</param>
        /// <param name="idFactory">The factory fo IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>The created <see cref="PersistableSoil"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when
        /// <see cref="MacroStabilityInwardsShearStrengthModel"/> has an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="MacroStabilityInwardsShearStrengthModel"/>
        /// has a valid value but is not supported.</exception>
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
                                                       (int) shearStrengthModel,
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
                                                       (int) shearStrengthModel,
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
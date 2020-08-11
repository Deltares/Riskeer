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
using Core.Common.Base.Geometry;
using Core.Common.Geometry;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.Helpers;
using Riskeer.MacroStabilityInwards.IO.Properties;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableState"/>.
    /// </summary>
    internal static class PersistableStateFactory
    {
        /// <summary>
        /// Creates a new collection of <see cref="PersistableState"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to use.</param>
        /// <param name="idFactory">The factory for creating IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>A collection of <see cref="PersistableState"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<PersistableState> Create(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile,
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

            return new[]
            {
                Create(soilProfile, MacroStabilityInwardsExportStageType.Daily, idFactory, registry)
            };
        }

        private static PersistableState Create(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile, MacroStabilityInwardsExportStageType stageType,
                                               IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            MacroStabilityInwardsSoilLayer2D[] layers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfile.Layers).ToArray();

            var statePoints = new List<PersistableStatePoint>();

            if (PersistableStateHelper.HasValidStatePoints(soilProfile))
            {
                statePoints.AddRange(layers.Where(l => l.Data.UsePop && PersistableStateHelper.HasValidPop(l.Data.Pop))
                                           .Select(l => CreatePOPStatePoint(l, stageType, idFactory, registry))
                                           .ToArray());

                var preconsolidationStressPoints = new List<PersistableStatePoint>();

                foreach (IMacroStabilityInwardsPreconsolidationStress preconsolidationStress in soilProfile.PreconsolidationStresses)
                {
                    MacroStabilityInwardsSoilLayer2D layer = GetLayerForPreconsolidationStress(layers, preconsolidationStress);
                    if (layer != null)
                    {
                        preconsolidationStressPoints.Add(
                            CreateYieldStressStatePoint(layer, preconsolidationStress, stageType, idFactory, registry));
                    }
                }

                statePoints.AddRange(preconsolidationStressPoints);
            }

            var state = new PersistableState
            {
                Id = idFactory.Create(),
                StateLines = Enumerable.Empty<PersistableStateLine>(),
                StatePoints = statePoints
            };

            registry.AddState(stageType, state.Id);

            return state;
        }

        private static MacroStabilityInwardsSoilLayer2D GetLayerForPreconsolidationStress(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers, IMacroStabilityInwardsPreconsolidationStress pcs)
        {
            return layers.SingleOrDefault(l => AdvancedMath2D.PointInPolygon(
                                              pcs.Location,
                                              l.OuterRing.Points,
                                              l.NestedLayers.Select(nl => nl.OuterRing.Points)));
        }

        private static PersistableStatePoint CreateYieldStressStatePoint(MacroStabilityInwardsSoilLayer2D layer, IMacroStabilityInwardsPreconsolidationStress preconsolidationStress,
                                                                         MacroStabilityInwardsExportStageType stageType, IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            return new PersistableStatePoint
            {
                Id = idFactory.Create(),
                LayerId = registry.GeometryLayers[stageType][layer],
                IsProbabilistic = false,
                Point = new PersistablePoint(preconsolidationStress.Location.X, preconsolidationStress.Location.Y),
                Stress = CreateYieldStress(preconsolidationStress),
                Label = string.Format(Resources.PersistableStateFactory_CreateStatePoint_PreconsolidationStress_LayerName_0, layer.Data.MaterialName)
            };
        }

        private static PersistableStatePoint CreatePOPStatePoint(MacroStabilityInwardsSoilLayer2D layer, MacroStabilityInwardsExportStageType stageType,
                                                                 IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            Point2D interiorPoint = AdvancedMath2D.GetPolygonInteriorPoint(layer.OuterRing.Points, layer.NestedLayers.Select(layers => layers.OuterRing.Points));

            return  new PersistableStatePoint
            {
                Id = idFactory.Create(),
                LayerId = registry.GeometryLayers[stageType][layer],
                IsProbabilistic = true,
                Point = new PersistablePoint(interiorPoint.X, interiorPoint.Y),
                Stress = CreatePOPStress(layer.Data),
                Label = string.Format(Resources.PersistableStateFactory_CreateStatePoint_POP_LayerName_0, layer.Data.MaterialName)
            };
        }

        private static PersistableStress CreateYieldStress(IMacroStabilityInwardsPreconsolidationStress preconsolidationStress)
        {
            return new PersistableStress
            {
                YieldStress = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(preconsolidationStress).GetDesignValue(),
                StateType = PersistableStateType.YieldStress
            };
        }

        private static PersistableStress CreatePOPStress(MacroStabilityInwardsSoilLayerData layerData)
        {
            return new PersistableStress
            {
                Pop = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(layerData).GetDesignValue(),
                PopStochasticParameter = PersistableStochasticParameterFactory.Create(layerData.Pop),
                StateType = PersistableStateType.Pop
            };
        }
    }
}
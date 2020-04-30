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
using Deltares.MacroStability.Geometry;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;
using ShearStrengthModel = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.ShearStrengthModel;
using SoilLayer = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;
using WtiStabilitySoil = Deltares.MacroStability.Geometry.Soil;
using WtiStabilityShearStrengthModel = Deltares.MacroStability.Geometry.ShearStrengthModel;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="LayerWithSoil"/> instances which are (indirectly) required in a calculation.
    /// </summary>
    internal static class LayerWithSoilCreator
    {
        /// <summary>
        /// Creates <see cref="LayerWithSoil"/> objects based on <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The <see cref="SoilProfile"/> to create <see cref="LayerWithSoil"/> objects for.</param>
        /// <param name="layerLookup">The lookup to fill with the created layers.</param>
        /// <returns>An <see cref="Array"/> of <see cref="LayerWithSoil"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="ShearStrengthModel"/>,
        /// <see cref="WaterPressureInterpolationModel"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="ShearStrengthModel"/>,
        /// <see cref="WaterPressureInterpolationModel"/> is a valid value, but unsupported.</exception>
        public static LayerWithSoil[] Create(SoilProfile soilProfile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            layerLookup = new Dictionary<SoilLayer, LayerWithSoil>();
            return GetLayersWithSoilRecursively(soilProfile.Layers, layerLookup).ToArray();
        }

        /// <summary>
        /// Gets <see cref="LayerWithSoil"/> recursively.
        /// </summary>
        /// <param name="soilLayers">The soil layers to obtain the <see cref="LayerWithSoil"/> objects from.</param>
        /// <param name="layerLookup">The lookup to fill with the created layers.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="LayerWithSoil"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="ShearStrengthModel"/>,
        /// <see cref="WaterPressureInterpolationModel"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="ShearStrengthModel"/>,
        /// <see cref="WaterPressureInterpolationModel"/> is a valid value, but unsupported.</exception>
        private static IEnumerable<LayerWithSoil> GetLayersWithSoilRecursively(IEnumerable<SoilLayer> soilLayers, IDictionary<SoilLayer, LayerWithSoil> layerLookup)
        {
            var layersWithSoil = new List<LayerWithSoil>();

            foreach (SoilLayer layer in soilLayers)
            {
                var layerWithSoil = new LayerWithSoil(
                    layer.OuterRing,
                    GetInnerLoopsRecursively(layer),
                    new WtiStabilitySoil(layer.MaterialName)
                    {
                        ShearStrengthModel = ConvertShearStrengthModel(layer.ShearStrengthModel),
                        AbovePhreaticLevel = layer.AbovePhreaticLevel,
                        BelowPhreaticLevel = layer.BelowPhreaticLevel,
                        Cohesion = layer.Cohesion,
                        FrictionAngle = layer.FrictionAngle,
                        RatioCuPc = layer.ShearStrengthRatio,
                        StrengthIncreaseExponent = layer.StrengthIncreaseExponent,
                        Dilatancy = 0.0
                    },
                    layer.IsAquifer,
                    ConvertWaterPressureInterpolationModel(layer.WaterPressureInterpolationModel));

                layersWithSoil.Add(layerWithSoil);
                layerLookup.Add(layer, layerWithSoil);

                layersWithSoil.AddRange(GetLayersWithSoilRecursively(layer.NestedLayers, layerLookup));
            }

            return layersWithSoil;
        }

        private static IEnumerable<IEnumerable<Point2D>> GetInnerLoopsRecursively(SoilLayer layer)
        {
            var innerLoops = new List<IEnumerable<Point2D>>();

            foreach (SoilLayer nestedLayer in layer.NestedLayers)
            {
                innerLoops.Add(nestedLayer.OuterRing);

                innerLoops.AddRange(GetInnerLoopsRecursively(nestedLayer));
            }

            return innerLoops;
        }

        /// <summary>
        /// Converts a <see cref="ShearStrengthModel"/> into a <see cref="WtiStabilityShearStrengthModel"/>.
        /// </summary>
        /// <param name="shearStrengthModel">The <see cref="ShearStrengthModel"/> to convert.</param>
        /// <returns>A <see cref="WtiStabilityShearStrengthModel"/> based on <paramref name="shearStrengthModel"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// is a valid value, but unsupported.</exception>
        private static WtiStabilityShearStrengthModel ConvertShearStrengthModel(ShearStrengthModel shearStrengthModel)
        {
            if (!Enum.IsDefined(typeof(ShearStrengthModel), shearStrengthModel))
            {
                throw new InvalidEnumArgumentException(nameof(shearStrengthModel),
                                                       (int) shearStrengthModel,
                                                       typeof(ShearStrengthModel));
            }

            switch (shearStrengthModel)
            {
                case ShearStrengthModel.SuCalculated:
                    return WtiStabilityShearStrengthModel.CuCalculated;
                case ShearStrengthModel.CPhi:
                    return WtiStabilityShearStrengthModel.CPhi;
                case ShearStrengthModel.CPhiOrSuCalculated:
                    return WtiStabilityShearStrengthModel.CPhiOrCuCalculated;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="WaterPressureInterpolationModel"/> into a <see cref="WaterpressureInterpolationModel"/>.
        /// </summary>
        /// <param name="waterPressureInterpolationModel">The <see cref="WaterPressureInterpolationModel"/> to convert.</param>
        /// <returns>A <see cref="WaterpressureInterpolationModel"/> based on <paramref name="waterPressureInterpolationModel"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="waterPressureInterpolationModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="waterPressureInterpolationModel"/>
        /// is a valid value, but unsupported.</exception>
        private static WaterpressureInterpolationModel ConvertWaterPressureInterpolationModel(WaterPressureInterpolationModel waterPressureInterpolationModel)
        {
            if (!Enum.IsDefined(typeof(WaterPressureInterpolationModel), waterPressureInterpolationModel))
            {
                throw new InvalidEnumArgumentException(nameof(waterPressureInterpolationModel),
                                                       (int) waterPressureInterpolationModel,
                                                       typeof(WaterPressureInterpolationModel));
            }

            switch (waterPressureInterpolationModel)
            {
                case WaterPressureInterpolationModel.Automatic:
                    return WaterpressureInterpolationModel.Automatic;
                case WaterPressureInterpolationModel.Hydrostatic:
                    return WaterpressureInterpolationModel.Hydrostatic;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
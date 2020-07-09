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
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Helpers;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.Converters
{
    /// <summary>
    /// Converter to convert <see cref="IMacroStabilityInwardsSoilProfileUnderSurfaceLine"/>
    /// into <see cref="SoilProfile"/>.
    /// </summary>
    public static class SoilProfileConverter
    {
        /// <summary>
        /// Converts <see cref="IMacroStabilityInwardsSoilProfileUnderSurfaceLine"/>
        /// into <see cref="SoilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to convert.</param>
        /// <returns>The converted <see cref="SoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when 
        /// <see cref="MacroStabilityInwardsSoilLayerData.ShearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when 
        /// <see cref="MacroStabilityInwardsSoilLayerData.ShearStrengthModel"/>
        /// is a valid value, but unsupported.</exception>
        public static SoilProfile Convert(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            IEnumerable<SoilLayer> layers = ConvertLayers(soilProfile.Layers);
            IEnumerable<PreconsolidationStress> preconsolidationStresses = ConvertPreconsolidationStresses(soilProfile.PreconsolidationStresses);

            return new SoilProfile(layers, preconsolidationStresses);
        }

        /// <summary>
        /// Converts <see cref="MacroStabilityInwardsSoilLayer2D"/> objects into <see cref="SoilLayer"/> objects.
        /// </summary>
        /// <param name="layers">The layers to convert.</param>
        /// <returns>The converted <see cref="SoilLayer"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when 
        /// <see cref="MacroStabilityInwardsSoilLayerData.ShearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when 
        /// <see cref="MacroStabilityInwardsSoilLayerData.ShearStrengthModel"/>
        /// is a valid value, but unsupported.</exception>
        private static IEnumerable<SoilLayer> ConvertLayers(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers)
        {
            return layers.Select(l =>
            {
                MacroStabilityInwardsSoilLayerData data = l.Data;

                return new SoilLayer(RingToPoints(l.OuterRing),
                                     new SoilLayer.ConstructionProperties
                                     {
                                         MaterialName = SoilLayerDataHelper.GetValidName(data.MaterialName),
                                         UsePop = data.UsePop,
                                         IsAquifer = data.IsAquifer,
                                         ShearStrengthModel = ConvertShearStrengthModel(data.ShearStrengthModel),
                                         AbovePhreaticLevel = data.AbovePhreaticLevel.Mean,
                                         BelowPhreaticLevel = data.BelowPhreaticLevel.Mean,
                                         Cohesion = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(data).GetDesignValue(),
                                         FrictionAngle = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(data).GetDesignValue(),
                                         ShearStrengthRatio = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(data).GetDesignValue(),
                                         StrengthIncreaseExponent = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(data).GetDesignValue(),
                                         Pop = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(data).GetDesignValue(),
                                         Dilatancy = 0.0,
                                         WaterPressureInterpolationModel = WaterPressureInterpolationModel.Automatic
                                     },
                                     ConvertLayers(l.NestedLayers));
            }).ToArray();
        }

        private static IEnumerable<Point2D> RingToPoints(Ring ring)
        {
            return ring.Points.ToArray();
        }

        private static IEnumerable<PreconsolidationStress> ConvertPreconsolidationStresses(
            IEnumerable<IMacroStabilityInwardsPreconsolidationStress> preconsolidationStresses)
        {
            return preconsolidationStresses.Select(ps => new PreconsolidationStress(
                                                       ps.Location,
                                                       MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(ps).GetDesignValue()))
                                           .ToArray();
        }

        /// <summary>
        /// Converts a <see cref="MacroStabilityInwardsShearStrengthModel"/> to a <see cref="ShearStrengthModel"/>.
        /// </summary>
        /// <param name="shearStrengthModel">The <see cref="MacroStabilityInwardsShearStrengthModel"/> to convert.</param>
        /// <returns>A <see cref="ShearStrengthModel"/> based on the information of <paramref name="shearStrengthModel"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// is a valid value, but unsupported.</exception>
        private static ShearStrengthModel ConvertShearStrengthModel(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsShearStrengthModel), shearStrengthModel))
            {
                throw new InvalidEnumArgumentException(nameof(shearStrengthModel),
                                                       (int) shearStrengthModel,
                                                       typeof(MacroStabilityInwardsShearStrengthModel));
            }

            switch (shearStrengthModel)
            {
                case MacroStabilityInwardsShearStrengthModel.SuCalculated:
                    return ShearStrengthModel.SuCalculated;
                case MacroStabilityInwardsShearStrengthModel.CPhi:
                    return ShearStrengthModel.CPhi;
                case MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated:
                    return ShearStrengthModel.CPhiOrSuCalculated;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
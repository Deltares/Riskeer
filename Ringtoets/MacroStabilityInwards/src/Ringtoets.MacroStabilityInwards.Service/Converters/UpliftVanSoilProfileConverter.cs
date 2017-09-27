﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.Service.Converters
{
    /// <summary>
    /// Converter to convert <see cref="MacroStabilityInwardsSoilProfileUnderSurfaceLine"/>
    /// into <see cref="UpliftVanSoilProfile"/>.
    /// </summary>
    internal static class UpliftVanSoilProfileConverter
    {
        /// <summary>
        /// Converts <see cref="MacroStabilityInwardsSoilProfileUnderSurfaceLine"/>
        /// into <see cref="UpliftVanSoilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to convert.</param>
        /// <returns>The converted <see cref="UpliftVanSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when 
        /// <see cref="MacroStabilityInwardsSoilLayerProperties.ShearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when 
        /// <see cref="MacroStabilityInwardsSoilLayerProperties.ShearStrengthModel"/>
        /// is a valid value but unsupported.</exception>
        public static UpliftVanSoilProfile Convert(MacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            IEnumerable<UpliftVanSoilLayer> layers = ConvertLayers(soilProfile.Layers);
            IEnumerable<UpliftVanPreconsolidationStress> preconsolidationStresses = ConvertPreconsolidationStresses(soilProfile.PreconsolidationStresses);

            return new UpliftVanSoilProfile(layers, preconsolidationStresses);
        }

        /// <summary>
        /// Converts <see cref="MacroStabilityInwardsSoilLayerProperties"/>
        /// into <see cref="UpliftVanSoilLayer"/>.
        /// </summary>
        /// <param name="layers">The layers to convert.</param>
        /// <returns>The converted <see cref="UpliftVanSoilLayer"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when 
        /// <see cref="MacroStabilityInwardsSoilLayerProperties.ShearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when 
        /// <see cref="MacroStabilityInwardsSoilLayerProperties.ShearStrengthModel"/>
        /// is a valid value but unsupported.</exception>
        private static IEnumerable<UpliftVanSoilLayer> ConvertLayers(IEnumerable<MacroStabilityInwardsSoilLayerUnderSurfaceLine> layers)
        {
            return layers.Select(l =>
            {
                MacroStabilityInwardsSoilLayerProperties properties = l.Properties;
                return new UpliftVanSoilLayer(l.OuterRing, l.Holes, new UpliftVanSoilLayer.ConstructionProperties
                {
                    MaterialName = properties.MaterialName,
                    UsePop = properties.UsePop,
                    IsAquifer = properties.IsAquifer,
                    ShearStrengthModel = ConvertShearStrengthModel(properties.ShearStrengthModel),
                    AbovePhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(properties).GetDesignValue(),
                    BelowPhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(properties).GetDesignValue(),
                    Cohesion = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(properties).GetDesignValue(),
                    FrictionAngle = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(properties).GetDesignValue(),
                    ShearStrengthRatio = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(properties).GetDesignValue(),
                    StrengthIncreaseExponent = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(properties).GetDesignValue(),
                    Pop = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(properties).GetDesignValue()
                });
            }).ToArray();
        }

        private static IEnumerable<UpliftVanPreconsolidationStress> ConvertPreconsolidationStresses(
            IEnumerable<MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine> preconsolidationStresses)
        {
            return preconsolidationStresses.Select(ps => new UpliftVanPreconsolidationStress(
                                                       new Point2D(ps.XCoordinate, ps.ZCoordinate),
                                                       MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(ps).GetDesignValue()));
        }

        /// <summary>
        /// Converts a <see cref="MacroStabilityInwardsShearStrengthModel"/> to a <see cref="UpliftVanShearStrengthModel"/>.
        /// </summary>
        /// <param name="shearStrengthModel">The <see cref="MacroStabilityInwardsShearStrengthModel"/> to convert.</param>
        /// <returns>A <see cref="UpliftVanShearStrengthModel"/> based on the information of <paramref name="shearStrengthModel"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// is a valid value but unsupported.</exception>
        private static UpliftVanShearStrengthModel ConvertShearStrengthModel(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
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
                    return UpliftVanShearStrengthModel.SuCalculated;
                case MacroStabilityInwardsShearStrengthModel.CPhi:
                    return UpliftVanShearStrengthModel.CPhi;
                case MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated:
                    return UpliftVanShearStrengthModel.CPhiOrSuCalculated;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
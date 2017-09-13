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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils.Extensions;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsSoilLayer1D"/> related to creating 
    /// a <see cref="MacroStabilityInwardsSoilLayerOneDEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilLayer1DCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsSoilLayerOneDEntity"/> based on the information 
        /// of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to create a database entity for.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilLayerOneDEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilLayerOneDEntity Create(this MacroStabilityInwardsSoilLayer1D soilLayer,
                                                                      int order)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            MacroStabilityInwardsSoilLayerProperties properties = soilLayer.Properties;
            return new MacroStabilityInwardsSoilLayerOneDEntity
            {
                Top = soilLayer.Top.ToNaNAsNull(),

                IsAquifer = Convert.ToByte(properties.IsAquifer),
                MaterialName = properties.MaterialName.DeepClone(),
                Color = properties.Color.ToArgb(),
                UsePop = Convert.ToByte(properties.UsePop),
                ShearStrengthModel = Convert.ToByte(properties.ShearStrengthModel),

                AbovePhreaticLevelMean = properties.AbovePhreaticLevelMean.ToNaNAsNull(),
                AbovePhreaticLevelCoefficientOfVariation = properties.AbovePhreaticLevelCoefficientOfVariation.ToNaNAsNull(),
                AbovePhreaticLevelShift = properties.AbovePhreaticLevelShift.ToNaNAsNull(),

                BelowPhreaticLevelMean = properties.BelowPhreaticLevelMean.ToNaNAsNull(),
                BelowPhreaticLevelCoefficientOfVariation = properties.BelowPhreaticLevelCoefficientOfVariation.ToNaNAsNull(),
                BelowPhreaticLevelShift = properties.BelowPhreaticLevelShift.ToNaNAsNull(),

                CohesionMean = properties.CohesionMean.ToNaNAsNull(),
                CohesionCoefficientOfVariation = properties.CohesionCoefficientOfVariation.ToNaNAsNull(),

                FrictionAngleMean = properties.FrictionAngleMean.ToNaNAsNull(),
                FrictionAngleCoefficientOfVariation = properties.FrictionAngleCoefficientOfVariation.ToNaNAsNull(),

                ShearStrengthRatioMean = properties.ShearStrengthRatioMean.ToNaNAsNull(),
                ShearStrengthRatioCoefficientOfVariation = properties.ShearStrengthRatioCoefficientOfVariation.ToNaNAsNull(),

                StrengthIncreaseExponentMean = properties.StrengthIncreaseExponentMean.ToNaNAsNull(),
                StrengthIncreaseExponentCoefficientOfVariation = properties.StrengthIncreaseExponentCoefficientOfVariation.ToNaNAsNull(),

                PopMean = properties.PopMean.ToNaNAsNull(),
                PopCoefficientOfVariation = properties.PopCoefficientOfVariation.ToNaNAsNull(),

                Order = order
            };
        }
    }
}
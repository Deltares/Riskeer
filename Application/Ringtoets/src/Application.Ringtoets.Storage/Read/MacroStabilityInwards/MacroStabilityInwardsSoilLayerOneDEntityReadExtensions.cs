// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityInwardsSoilLayer1D"/> 
    /// based on the <see cref="MacroStabilityInwardsSoilLayerOneDEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilLayerOneDEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="MacroStabilityInwardsSoilLayerOneDEntity"/> and use the information 
        /// to construct a <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilLayerOneDEntity"/> to create 
        /// <see cref="MacroStabilityInwardsSoilLayer1D"/> for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilLayer1D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilLayer1D Read(this MacroStabilityInwardsSoilLayerOneDEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new MacroStabilityInwardsSoilLayer1D(entity.Top.ToNullAsNaN())
            {
                Properties =
                {
                    IsAquifer = Convert.ToBoolean(entity.IsAquifer),
                    MaterialName = entity.MaterialName ?? string.Empty,
                    Color = Color.FromArgb(Convert.ToInt32(entity.Color)),
                    UsePop = Convert.ToBoolean(entity.UsePop),
                    ShearStrengthModel = (MacroStabilityInwardsShearStrengthModel) entity.ShearStrengthModel,
                    AbovePhreaticLevelMean = entity.AbovePhreaticLevelMean.ToNullAsNaN(),
                    AbovePhreaticLevelCoefficientOfVariation = entity.AbovePhreaticLevelCoefficientOfVariation.ToNullAsNaN(),
                    AbovePhreaticLevelShift = entity.AbovePhreaticLevelShift.ToNullAsNaN(),
                    BelowPhreaticLevelMean = entity.BelowPhreaticLevelMean.ToNullAsNaN(),
                    BelowPhreaticLevelCoefficientOfVariation = entity.BelowPhreaticLevelCoefficientOfVariation.ToNullAsNaN(),
                    BelowPhreaticLevelShift = entity.BelowPhreaticLevelShift.ToNullAsNaN(),
                    CohesionMean = entity.CohesionMean.ToNullAsNaN(),
                    CohesionCoefficientOfVariation = entity.CohesionCoefficientOfVariation.ToNullAsNaN(),
                    FrictionAngleMean = entity.FrictionAngleMean.ToNullAsNaN(),
                    FrictionAngleCoefficientOfVariation = entity.FrictionAngleCoefficientOfVariation.ToNullAsNaN(),
                    ShearStrengthRatioMean = entity.ShearStrengthRatioMean.ToNullAsNaN(),
                    ShearStrengthRatioCoefficientOfVariation = entity.ShearStrengthRatioCoefficientOfVariation.ToNullAsNaN(),
                    StrengthIncreaseExponentMean = entity.StrengthIncreaseExponentMean.ToNullAsNaN(),
                    StrengthIncreaseExponentCoefficientOfVariation = entity.StrengthIncreaseExponentCoefficientOfVariation.ToNullAsNaN(),
                    PopMean = entity.PopMean.ToNullAsNaN(),
                    PopCoefficientOfVariation = entity.PopCoefficientOfVariation.ToNullAsNaN()
                }
            };
        }
    }
}
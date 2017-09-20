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
using System.Drawing;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityInwardsSoilLayer2D"/> 
    /// based on the <see cref="MacroStabilityInwardsSoilLayerTwoDEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilLayerTwoDEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="MacroStabilityInwardsSoilLayerTwoDEntity"/> and use the information 
        /// to construct a <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilLayerTwoDEntity"/> to create 
        /// <see cref="MacroStabilityInwardsSoilLayer2D"/> for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilLayer2D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilLayer2D Read(this MacroStabilityInwardsSoilLayerTwoDEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var outerRing = new Ring(new Point2DCollectionXmlSerializer().FromXml(entity.OuterRingXml));
            Ring[] holes = new RingCollectionXmlSerializer().FromXml(entity.HolesXml);

            return new MacroStabilityInwardsSoilLayer2D(outerRing, holes)
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
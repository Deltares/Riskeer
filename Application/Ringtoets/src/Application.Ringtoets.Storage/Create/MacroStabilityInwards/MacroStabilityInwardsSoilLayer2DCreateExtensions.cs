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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Utils.Extensions;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Application.Ringtoets.Storage.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsSoilLayer2D"/> related to creating 
    /// a <see cref="MacroStabilityInwardsSoilLayerTwoDEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilLayer2DCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsSoilLayerTwoDEntity"/> based on the information 
        /// of the <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to create a database entity for.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilLayerOneDEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilLayerTwoDEntity Create(this MacroStabilityInwardsSoilLayer2D soilLayer,
                                                                      int order)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            MacroStabilityInwardsSoilLayerProperties properties = soilLayer.Properties;
            return new MacroStabilityInwardsSoilLayerTwoDEntity
            {
                OuterRingXml = new Point2DXmlSerializer().ToXml(soilLayer.OuterRing.Points),
                HolesXml = new RingXmlSerializer().ToXml(soilLayer.Holes),
                IsAquifer = Convert.ToByte(properties.IsAquifer),
                MaterialName = properties.MaterialName.DeepClone(),
                Color = properties.Color.ToArgb(),
                UsePop = Convert.ToByte(properties.UsePop),
                ShearStrengthModel = Convert.ToByte(properties.ShearStrengthModel),
                AbovePhreaticLevelMean = properties.AbovePhreaticLevel.Mean.ToNaNAsNull(),
                AbovePhreaticLevelCoefficientOfVariation = properties.AbovePhreaticLevel.CoefficientOfVariation.ToNaNAsNull(),
                AbovePhreaticLevelShift = properties.AbovePhreaticLevel.Shift.ToNaNAsNull(),
                BelowPhreaticLevelMean = properties.BelowPhreaticLevel.Mean.ToNaNAsNull(),
                BelowPhreaticLevelCoefficientOfVariation = properties.BelowPhreaticLevel.CoefficientOfVariation.ToNaNAsNull(),
                BelowPhreaticLevelShift = properties.BelowPhreaticLevel.Shift.ToNaNAsNull(),
                CohesionMean = properties.Cohesion.Mean.ToNaNAsNull(),
                CohesionCoefficientOfVariation = properties.Cohesion.CoefficientOfVariation.ToNaNAsNull(),
                FrictionAngleMean = properties.FrictionAngle.Mean.ToNaNAsNull(),
                FrictionAngleCoefficientOfVariation = properties.FrictionAngle.CoefficientOfVariation.ToNaNAsNull(),
                ShearStrengthRatioMean = properties.ShearStrengthRatio.Mean.ToNaNAsNull(),
                ShearStrengthRatioCoefficientOfVariation = properties.ShearStrengthRatio.CoefficientOfVariation.ToNaNAsNull(),
                StrengthIncreaseExponentMean = properties.StrengthIncreaseExponent.Mean.ToNaNAsNull(),
                StrengthIncreaseExponentCoefficientOfVariation = properties.StrengthIncreaseExponent.CoefficientOfVariation.ToNaNAsNull(),
                PopMean = properties.Pop.Mean.ToNaNAsNull(),
                PopCoefficientOfVariation = properties.Pop.CoefficientOfVariation.ToNaNAsNull(),
                Order = order
            };
        }
    }
}
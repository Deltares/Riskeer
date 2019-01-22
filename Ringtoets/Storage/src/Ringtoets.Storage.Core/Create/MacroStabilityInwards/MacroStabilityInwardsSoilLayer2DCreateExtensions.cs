// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Util.Extensions;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Create.MacroStabilityInwards
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

            MacroStabilityInwardsSoilLayerData data = soilLayer.Data;
            var entity = new MacroStabilityInwardsSoilLayerTwoDEntity
            {
                OuterRingXml = new Point2DCollectionXmlSerializer().ToXml(soilLayer.OuterRing.Points),
                IsAquifer = Convert.ToByte(data.IsAquifer),
                MaterialName = data.MaterialName.DeepClone(),
                Color = data.Color.ToInt64(),
                UsePop = Convert.ToByte(data.UsePop),
                ShearStrengthModel = Convert.ToByte(data.ShearStrengthModel),
                AbovePhreaticLevelMean = data.AbovePhreaticLevel.Mean.ToNaNAsNull(),
                AbovePhreaticLevelCoefficientOfVariation = data.AbovePhreaticLevel.CoefficientOfVariation.ToNaNAsNull(),
                AbovePhreaticLevelShift = data.AbovePhreaticLevel.Shift.ToNaNAsNull(),
                BelowPhreaticLevelMean = data.BelowPhreaticLevel.Mean.ToNaNAsNull(),
                BelowPhreaticLevelCoefficientOfVariation = data.BelowPhreaticLevel.CoefficientOfVariation.ToNaNAsNull(),
                BelowPhreaticLevelShift = data.BelowPhreaticLevel.Shift.ToNaNAsNull(),
                CohesionMean = data.Cohesion.Mean.ToNaNAsNull(),
                CohesionCoefficientOfVariation = data.Cohesion.CoefficientOfVariation.ToNaNAsNull(),
                FrictionAngleMean = data.FrictionAngle.Mean.ToNaNAsNull(),
                FrictionAngleCoefficientOfVariation = data.FrictionAngle.CoefficientOfVariation.ToNaNAsNull(),
                ShearStrengthRatioMean = data.ShearStrengthRatio.Mean.ToNaNAsNull(),
                ShearStrengthRatioCoefficientOfVariation = data.ShearStrengthRatio.CoefficientOfVariation.ToNaNAsNull(),
                StrengthIncreaseExponentMean = data.StrengthIncreaseExponent.Mean.ToNaNAsNull(),
                StrengthIncreaseExponentCoefficientOfVariation = data.StrengthIncreaseExponent.CoefficientOfVariation.ToNaNAsNull(),
                PopMean = data.Pop.Mean.ToNaNAsNull(),
                PopCoefficientOfVariation = data.Pop.CoefficientOfVariation.ToNaNAsNull(),
                Order = order
            };

            AddNestedLayers(entity, soilLayer);

            return entity;
        }

        private static void AddNestedLayers(MacroStabilityInwardsSoilLayerTwoDEntity entity,
                                            MacroStabilityInwardsSoilLayer2D soilLayer)
        {
            for (var i = 0; i < soilLayer.NestedLayers.Count(); i++)
            {
                entity.MacroStabilityInwardsSoilLayerTwoDEntity1.Add(soilLayer.NestedLayers.ElementAt(i).Create(i));
            }
        }
    }
}
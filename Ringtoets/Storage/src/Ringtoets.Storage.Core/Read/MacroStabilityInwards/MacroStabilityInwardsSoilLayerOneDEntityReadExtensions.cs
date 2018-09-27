// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.MacroStabilityInwards
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

            return new MacroStabilityInwardsSoilLayer1D(entity.Top.ToNullAsNaN(), ReadData(entity));
        }

        private static MacroStabilityInwardsSoilLayerData ReadData(MacroStabilityInwardsSoilLayerOneDEntity entity)
        {
            return new MacroStabilityInwardsSoilLayerData
            {
                IsAquifer = Convert.ToBoolean(entity.IsAquifer),
                MaterialName = entity.MaterialName ?? string.Empty,
                Color = entity.Color.ToColor(),
                UsePop = Convert.ToBoolean(entity.UsePop),
                ShearStrengthModel = (MacroStabilityInwardsShearStrengthModel) entity.ShearStrengthModel,
                AbovePhreaticLevel =
                {
                    Mean = (RoundedDouble) entity.AbovePhreaticLevelMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.AbovePhreaticLevelCoefficientOfVariation.ToNullAsNaN(),
                    Shift = (RoundedDouble) entity.AbovePhreaticLevelShift.ToNullAsNaN()
                },
                BelowPhreaticLevel =
                {
                    Mean = (RoundedDouble) entity.BelowPhreaticLevelMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.BelowPhreaticLevelCoefficientOfVariation.ToNullAsNaN(),
                    Shift = (RoundedDouble) entity.BelowPhreaticLevelShift.ToNullAsNaN()
                },
                Cohesion =
                {
                    Mean = (RoundedDouble) entity.CohesionMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.CohesionCoefficientOfVariation.ToNullAsNaN()
                },
                FrictionAngle =
                {
                    Mean = (RoundedDouble) entity.FrictionAngleMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.FrictionAngleCoefficientOfVariation.ToNullAsNaN()
                },
                ShearStrengthRatio =
                {
                    Mean = (RoundedDouble) entity.ShearStrengthRatioMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.ShearStrengthRatioCoefficientOfVariation.ToNullAsNaN()
                },
                StrengthIncreaseExponent =
                {
                    Mean = (RoundedDouble) entity.StrengthIncreaseExponentMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.StrengthIncreaseExponentCoefficientOfVariation.ToNullAsNaN()
                },
                Pop =
                {
                    Mean = (RoundedDouble) entity.PopMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.PopCoefficientOfVariation.ToNullAsNaN()
                }
            };
        }
    }
}
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
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using CommonShearStrengthModel = Ringtoets.Common.IO.SoilProfile.ShearStrengthModel;
using ShearStrengthModel = Ringtoets.MacroStabilityInwards.Primitives.ShearStrengthModel;

namespace Ringtoets.MacroStabilityInwards.IO.SoilProfiles
{
    /// <summary>
    /// Transforms generic <see cref="SoilLayerBase"/> into <see cref="MacroStabilityInwardsSoilLayer1D"/>
    /// or <see cref="MacroStabilityInwardsSoilLayer2D"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilLayerTransformer
    {
        /// <summary>
        /// Transforms the generic <paramref name="soilLayer"/> into a
        /// <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to use in the transformation.</param>
        /// <returns>A <see cref="MacroStabilityInwardsSoilLayer1D"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is
        /// <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        public static MacroStabilityInwardsSoilLayer1D Transform(SoilLayer1D soilLayer)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            var layer = new MacroStabilityInwardsSoilLayer1D(soilLayer.Top)
            {
                Properties =
                {
                    MaterialName = soilLayer.MaterialName,
                    IsAquifer = soilLayer.IsAquifer,
                    Color = soilLayer.Color,
                    UsePop = soilLayer.UsePop,
                    AbovePhreaticLevelMean = soilLayer.AbovePhreaticLevelMean,
                    AbovePhreaticLevelDeviation = soilLayer.AbovePhreaticLevelDeviation,
                    BelowPhreaticLevelMean = soilLayer.BelowPhreaticLevelMean,
                    BelowPhreaticLevelDeviation = soilLayer.BelowPhreaticLevelDeviation,
                    CohesionMean = soilLayer.CohesionMean,
                    CohesionDeviation = soilLayer.CohesionDeviation,
                    CohesionShift = soilLayer.CohesionShift,
                    FrictionAngleMean = soilLayer.FrictionAngleMean,
                    FrictionAngleDeviation = soilLayer.FrictionAngleDeviation,
                    FrictionAngleShift = soilLayer.FrictionAngleShift,
                    ShearStrengthRatioMean = soilLayer.ShearStrengthRatioMean,
                    ShearStrengthRatioDeviation = soilLayer.ShearStrengthRatioDeviation,
                    ShearStrengthRatioShift = soilLayer.ShearStrengthRatioShift,
                    StrengthIncreaseExponentMean = soilLayer.StrengthIncreaseExponentMean,
                    StrengthIncreaseExponentDeviation = soilLayer.StrengthIncreaseExponentDeviation,
                    StrengthIncreaseExponentShift = soilLayer.StrengthIncreaseExponentShift,
                    PopMean = soilLayer.PopMean,
                    PopDeviation = soilLayer.PopDeviation,
                    PopShift = soilLayer.PopShift
                }
            };

            try
            {
                layer.Properties.ShearStrengthModel = TransformShearStrengthModel(soilLayer.ShearStrengthModel);
            }
            catch (NotSupportedException e)
            {
                throw new ImportedDataTransformException("Er ging iets mis met transformeren.", e);
            }

            return layer;
        }

        /// <summary>
        /// Transforms the <see cref="CommonShearStrengthModel"/> to <see cref="ShearStrengthModel"/>.
        /// </summary>
        /// <param name="shearStrengthModel">The model to transform.</param>
        /// <returns>A <see cref="ShearStrengthModel"/> based on the given data.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// has an invalid value.</exception>
        private static ShearStrengthModel TransformShearStrengthModel(CommonShearStrengthModel shearStrengthModel)
        {
            switch (shearStrengthModel)
            {
                case CommonShearStrengthModel.None:
                    return ShearStrengthModel.None;
                case CommonShearStrengthModel.SuCalculated:
                    return ShearStrengthModel.SuCalculated;
                case CommonShearStrengthModel.CPhi:
                    return ShearStrengthModel.CPhi;
                case CommonShearStrengthModel.CPhiOrSuCalculated:
                    return ShearStrengthModel.CPhiOrSuCalculated;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
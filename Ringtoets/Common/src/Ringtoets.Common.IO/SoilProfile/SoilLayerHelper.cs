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

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class which provides helper methods to set values on <see cref="SoilLayerBase"/>
    /// objects.
    /// </summary>
    internal static class SoilLayerHelper
    {
        /// <summary>
        /// Sets the values of a <see cref="LayerProperties"/> object onto a 
        /// <see cref="SoilLayerBase"/> object.
        /// </summary>
        /// <param name="soilLayer">The <see cref="SoilLayerBase"/> to set the values of <see cref="LayerProperties"/>
        /// on.</param>
        /// <param name="properties">The <see cref="LayerProperties"/> containing 
        /// the values that needs to be set on the <see cref="SoilLayerBase"/>.</param>
        public static void SetSoilLayerBaseProperties(SoilLayerBase soilLayer, LayerProperties properties)
        {
            soilLayer.MaterialName = properties.MaterialName ?? string.Empty;
            if (properties.IsAquifer.HasValue)
            {
                soilLayer.IsAquifer = properties.IsAquifer.Value.Equals(1.0);
            }
            if (properties.Color.HasValue)
            {
                soilLayer.Color = SoilLayerColorConverter.Convert(properties.Color);
            }
            if (properties.BelowPhreaticLevelDistribution.HasValue)
            {
                soilLayer.BelowPhreaticLevelDistribution = properties.BelowPhreaticLevelDistribution.Value;
            }
            if (properties.BelowPhreaticLevelShift.HasValue)
            {
                soilLayer.BelowPhreaticLevelShift = properties.BelowPhreaticLevelShift.Value;
            }
            if (properties.BelowPhreaticLevelMean.HasValue)
            {
                soilLayer.BelowPhreaticLevelMean = properties.BelowPhreaticLevelMean.Value;
            }
            if (properties.BelowPhreaticLevelDeviation.HasValue)
            {
                soilLayer.BelowPhreaticLevelDeviation = properties.BelowPhreaticLevelDeviation.Value;
            }
            if (properties.DiameterD70Distribution.HasValue)
            {
                soilLayer.DiameterD70Distribution = properties.DiameterD70Distribution.Value;
            }
            if (properties.DiameterD70Shift.HasValue)
            {
                soilLayer.DiameterD70Shift = properties.DiameterD70Shift.Value;
            }
            if (properties.DiameterD70Mean.HasValue)
            {
                soilLayer.DiameterD70Mean = properties.DiameterD70Mean.Value;
            }
            if (properties.DiameterD70CoefficientOfVariation.HasValue)
            {
                soilLayer.DiameterD70CoefficientOfVariation = properties.DiameterD70CoefficientOfVariation.Value;
            }
            if (properties.PermeabilityDistribution.HasValue)
            {
                soilLayer.PermeabilityDistribution = properties.PermeabilityDistribution.Value;
            }
            if (properties.PermeabilityShift.HasValue)
            {
                soilLayer.PermeabilityShift = properties.PermeabilityShift.Value;
            }
            if (properties.PermeabilityMean.HasValue)
            {
                soilLayer.PermeabilityMean = properties.PermeabilityMean.Value;
            }
            if (properties.PermeabilityCoefficientOfVariation.HasValue)
            {
                soilLayer.PermeabilityCoefficientOfVariation = properties.PermeabilityCoefficientOfVariation.Value;
            }
            soilLayer.UsePop = properties.UsePop;
            soilLayer.ShearStrengthModel = properties.ShearStrengthModel;

            if (properties.AbovePhreaticLevelDistribution.HasValue)
            {
                soilLayer.AbovePhreaticLevelDistribution = properties.AbovePhreaticLevelDistribution.Value;
            }
            if (properties.AbovePhreaticLevelShift.HasValue)
            {
                soilLayer.AbovePhreaticLevelShift = properties.AbovePhreaticLevelShift.Value;
            }
            if (properties.AbovePhreaticLevelMean.HasValue)
            {
                soilLayer.AbovePhreaticLevelMean = properties.AbovePhreaticLevelMean.Value;
            }
            if (properties.AbovePhreaticLevelCoefficientOfVariation.HasValue)
            {
                soilLayer.AbovePhreaticLevelDeviation = properties.AbovePhreaticLevelCoefficientOfVariation.Value;
            }
            if (properties.CohesionDistribution.HasValue)
            {
                soilLayer.CohesionDistribution = properties.CohesionDistribution.Value;
            }
            if (properties.CohesionShift.HasValue)
            {
                soilLayer.CohesionShift = properties.CohesionShift.Value;
            }
            if (properties.CohesionMean.HasValue)
            {
                soilLayer.CohesionMean = properties.CohesionMean.Value;
            }
            if (properties.CohesionCoefficientOfVariation.HasValue)
            {
                soilLayer.CohesionDeviation = properties.CohesionCoefficientOfVariation.Value;
            }
            if (properties.FrictionAngleDistribution.HasValue)
            {
                soilLayer.FrictionAngleDistribution = properties.FrictionAngleDistribution.Value;
            }
            if (properties.FrictionAngleShift.HasValue)
            {
                soilLayer.FrictionAngleShift = properties.FrictionAngleShift.Value;
            }
            if (properties.FrictionAngleMean.HasValue)
            {
                soilLayer.FrictionAngleMean = properties.FrictionAngleMean.Value;
            }
            if (properties.FrictionAngleCoefficientOfVariation.HasValue)
            {
                soilLayer.FrictionAngleDeviation = properties.FrictionAngleCoefficientOfVariation.Value;
            }
            if (properties.ShearStrengthRatioDistribution.HasValue)
            {
                soilLayer.ShearStrengthRatioDistribution = properties.ShearStrengthRatioDistribution.Value;
            }
            if (properties.ShearStrengthRatioShift.HasValue)
            {
                soilLayer.ShearStrengthRatioShift = properties.ShearStrengthRatioShift.Value;
            }
            if (properties.ShearStrengthRatioMean.HasValue)
            {
                soilLayer.ShearStrengthRatioMean = properties.ShearStrengthRatioMean.Value;
            }
            if (properties.ShearStrengthRatioCoefficientOfVariation.HasValue)
            {
                soilLayer.ShearStrengthRatioDeviation = properties.ShearStrengthRatioCoefficientOfVariation.Value;
            }
            if (properties.StrengthIncreaseExponentDistribution.HasValue)
            {
                soilLayer.StrengthIncreaseExponentDistribution = properties.StrengthIncreaseExponentDistribution.Value;
            }
            if (properties.StrengthIncreaseExponentShift.HasValue)
            {
                soilLayer.StrengthIncreaseExponentShift = properties.StrengthIncreaseExponentShift.Value;
            }
            if (properties.StrengthIncreaseExponentMean.HasValue)
            {
                soilLayer.StrengthIncreaseExponentMean = properties.StrengthIncreaseExponentMean.Value;
            }
            if (properties.StrengthIncreaseExponentCoefficientOfVariation.HasValue)
            {
                soilLayer.StrengthIncreaseExponentDeviation = properties.StrengthIncreaseExponentCoefficientOfVariation.Value;
            }
            if (properties.PopDistribution.HasValue)
            {
                soilLayer.PopDistribution = properties.PopDistribution.Value;
            }
            if (properties.PopShift.HasValue)
            {
                soilLayer.PopShift = properties.PopShift.Value;
            }
            if (properties.PopMean.HasValue)
            {
                soilLayer.PopMean = properties.PopMean.Value;
            }
            if (properties.PopCoefficientOfVariation.HasValue)
            {
                soilLayer.PopDeviation = properties.PopCoefficientOfVariation.Value;
            }
        }
    }
}
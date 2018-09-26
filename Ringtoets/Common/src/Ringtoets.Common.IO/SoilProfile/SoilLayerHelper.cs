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

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class which provides helper methods for <see cref="SoilLayerBase"/>
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
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameter
        /// is <c>null</c>.</exception>
        public static void SetSoilLayerBaseProperties(SoilLayerBase soilLayer, LayerProperties properties)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (properties.MaterialName != null)
            {
                soilLayer.MaterialName = properties.MaterialName;
            }

            soilLayer.IsAquifer = properties.IsAquifer;
            soilLayer.Color = properties.Color;

            if (properties.BelowPhreaticLevelDistributionType.HasValue)
            {
                soilLayer.BelowPhreaticLevelDistributionType = properties.BelowPhreaticLevelDistributionType.Value;
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

            if (properties.BelowPhreaticLevelCoefficientOfVariation.HasValue)
            {
                soilLayer.BelowPhreaticLevelCoefficientOfVariation = properties.BelowPhreaticLevelCoefficientOfVariation.Value;
            }

            if (properties.DiameterD70DistributionType.HasValue)
            {
                soilLayer.DiameterD70DistributionType = properties.DiameterD70DistributionType.Value;
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

            if (properties.PermeabilityDistributionType.HasValue)
            {
                soilLayer.PermeabilityDistributionType = properties.PermeabilityDistributionType.Value;
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

            if (properties.AbovePhreaticLevelDistributionType.HasValue)
            {
                soilLayer.AbovePhreaticLevelDistributionType = properties.AbovePhreaticLevelDistributionType.Value;
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
                soilLayer.AbovePhreaticLevelCoefficientOfVariation = properties.AbovePhreaticLevelCoefficientOfVariation.Value;
            }

            if (properties.CohesionDistributionType.HasValue)
            {
                soilLayer.CohesionDistributionType = properties.CohesionDistributionType.Value;
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
                soilLayer.CohesionCoefficientOfVariation = properties.CohesionCoefficientOfVariation.Value;
            }

            if (properties.FrictionAngleDistributionType.HasValue)
            {
                soilLayer.FrictionAngleDistributionType = properties.FrictionAngleDistributionType.Value;
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
                soilLayer.FrictionAngleCoefficientOfVariation = properties.FrictionAngleCoefficientOfVariation.Value;
            }

            if (properties.ShearStrengthRatioDistributionType.HasValue)
            {
                soilLayer.ShearStrengthRatioDistributionType = properties.ShearStrengthRatioDistributionType.Value;
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
                soilLayer.ShearStrengthRatioCoefficientOfVariation = properties.ShearStrengthRatioCoefficientOfVariation.Value;
            }

            if (properties.StrengthIncreaseExponentDistributionType.HasValue)
            {
                soilLayer.StrengthIncreaseExponentDistributionType = properties.StrengthIncreaseExponentDistributionType.Value;
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
                soilLayer.StrengthIncreaseExponentCoefficientOfVariation = properties.StrengthIncreaseExponentCoefficientOfVariation.Value;
            }

            if (properties.PopDistributionType.HasValue)
            {
                soilLayer.PopDistributionType = properties.PopDistributionType.Value;
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
                soilLayer.PopCoefficientOfVariation = properties.PopCoefficientOfVariation.Value;
            }
        }
    }
}
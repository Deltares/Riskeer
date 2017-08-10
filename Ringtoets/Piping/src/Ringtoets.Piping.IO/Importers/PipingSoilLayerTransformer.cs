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
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.Primitives;
using SoilLayer1D = Ringtoets.Common.IO.SoilProfile.SoilLayer1D;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Transforms generic <see cref="SoilLayerBase"/> into <see cref="PipingSoilLayer"/>.
    /// </summary>
    public static class PipingSoilLayerTransformer
    {
        /// <summary>
        /// Transforms the generic <paramref name="soilLayer"/> into a mechanism specific 
        /// soil profile of type <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to use in the transformation.</param>
        /// <returns>A new <see cref="PipingSoilLayer"/> based on the given data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilLayer"/> is <c>null</c>.</exception>
        /// <exception cref="ImportedDataTransformException">Thrown when transformation would not result
        /// in a valid transformed instance.</exception>
        public static PipingSoilLayer Transform(SoilLayer1D soilLayer)
        {
            if (soilLayer == null)
            {
                throw new ArgumentNullException(nameof(soilLayer));
            }

            ValidateStochasticParametersForPiping(soilLayer);

            var pipingSoilLayer = new PipingSoilLayer(soilLayer.Top)
            {
                IsAquifer = soilLayer.IsAquifer,
                MaterialName = soilLayer.MaterialName,
                Color = soilLayer.Color
            };

            SetOptionalStochasticParameters(pipingSoilLayer, soilLayer);

            return pipingSoilLayer;
        }

        /// <summary>
        /// Validates whether the values of the distribution and shift for the stochastic parameters 
        /// are correct for creating a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <exception cref="ImportedDataTransformException">Thrown when any of the distributions of the
        /// stochastic parameters is not defined as lognormal or is shifted when it should not be.</exception>
        private static void ValidateStochasticParametersForPiping(SoilLayerBase soilLayer1D)
        {
            ValidateIsLogNormal(
                soilLayer1D.BelowPhreaticLevelDistribution,
                Resources.SoilLayer_BelowPhreaticLevelDistribution_Description);
            ValidateIsNonShiftedLogNormal(
                soilLayer1D.DiameterD70Distribution,
                soilLayer1D.DiameterD70Shift,
                Resources.SoilLayer_DiameterD70Distribution_Description);
            ValidateIsNonShiftedLogNormal(
                soilLayer1D.PermeabilityDistribution,
                soilLayer1D.PermeabilityShift,
                Resources.SoilLayer_PermeabilityDistribution_Description);
        }

        /// <summary>
        /// Sets the values of the optional stochastic parameters for the given <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="pipingSoilLayer">The <see cref="PipingSoilLayer"/> to set the property values for.</param>
        /// <param name="soilLayer1D">The <see cref="SoilLayerBase"/> to get the properties from.</param>
        /// <remarks>This method does not perform validation. Use <see cref="ValidateStochasticParametersForPiping"/> to 
        /// verify whether the distributions for the stochastic parameters are correctly defined.</remarks>
        private static void SetOptionalStochasticParameters(PipingSoilLayer pipingSoilLayer, SoilLayerBase soilLayer1D)
        {
            pipingSoilLayer.BelowPhreaticLevelMean = soilLayer1D.BelowPhreaticLevelMean;
            pipingSoilLayer.BelowPhreaticLevelDeviation = soilLayer1D.BelowPhreaticLevelDeviation;
            pipingSoilLayer.BelowPhreaticLevelShift = soilLayer1D.BelowPhreaticLevelShift;
            pipingSoilLayer.DiameterD70Mean = soilLayer1D.DiameterD70Mean;
            pipingSoilLayer.DiameterD70CoefficientOfVariation = soilLayer1D.DiameterD70CoefficientOfVariation;
            pipingSoilLayer.PermeabilityMean = soilLayer1D.PermeabilityMean;
            pipingSoilLayer.PermeabilityCoefficientOfVariation = soilLayer1D.PermeabilityCoefficientOfVariation;
        }

        private static void ValidateIsNonShiftedLogNormal(long? distribution, double shift, string incorrectDistibutionParameter)
        {
            if (distribution.HasValue && (distribution != SoilLayerConstants.LogNormalDistributionValue || shift != 0.0))
            {
                throw new ImportedDataTransformException(string.Format(
                                                             Resources.SoilLayer_Stochastic_parameter_0_has_no_lognormal_distribution,
                                                             incorrectDistibutionParameter));
            }
        }

        private static void ValidateIsLogNormal(long? distribution, string incorrectDistibutionParameter)
        {
            if (distribution.HasValue && distribution != SoilLayerConstants.LogNormalDistributionValue)
            {
                throw new ImportedDataTransformException(string.Format(
                                                             Resources.SoilLayer_Stochastic_parameter_0_has_no_shifted_lognormal_distribution,
                                                             incorrectDistibutionParameter));
            }
        }
    }
}
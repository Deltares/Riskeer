﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Builders
{
    internal abstract class GenericSoilLayerParameters {
        /// <summary>
        /// Gets or sets the name of the material that was assigned to the <see cref="SoilLayer1D"/>.
        /// </summary>
        internal string MaterialName { get; set; }

        /// <summary>
        /// Gets or sets the value representing a color that was used to represent the <see cref="SoilLayer1D"/>.
        /// </summary>
        internal double? Color { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the volumic weight of the <see cref="SoilLayer1D"/> below the 
        /// phreatic level.
        /// [kN/m³]
        /// </summary>
        internal long? BelowPhreaticLevelDistribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the volumic weight of the <see cref="SoilLayer1D"/> 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        internal double? BelowPhreaticLevelShift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the volumic weight of the <see cref="SoilLayer1D"/> 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        internal double? BelowPhreaticLevelMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the volumic weight of the <see cref="SoilLayer1D"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        internal double? BelowPhreaticLevelDeviation { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the mean diameter of small scale tests applied to different kinds of sand, on which the 
        /// formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        internal long? DiameterD70Distribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        internal double? DiameterD70Shift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        internal double? DiameterD70Mean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        internal double? DiameterD70Deviation { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        internal long? PermeabilityDistribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        internal double? PermeabilityShift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        internal double? PermeabilityMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        internal double? PermeabilityDeviation { get; set; }

        /// <summary>
        /// Sets the values of the optional stochastic parameters for the given <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="pipingSoilLayer">The <see cref="PipingSoilLayer"/> to set the property values for.</param>
        /// <remarks>This method does not perform validation. Use <see cref="ValidateStochasticParametersForPiping"/> to 
        /// verify whether the distributions for the stochastic parameters are correctly defined.</remarks>
        protected void SetOptionalStochasticParameters(PipingSoilLayer pipingSoilLayer)
        {
            if (BelowPhreaticLevelMean.HasValue)
            {
                pipingSoilLayer.BelowPhreaticLevelMean = BelowPhreaticLevelMean.Value;
            }
            if (BelowPhreaticLevelDeviation.HasValue)
            {
                pipingSoilLayer.BelowPhreaticLevelDeviation = BelowPhreaticLevelDeviation.Value;
            }
            if (DiameterD70Mean.HasValue)
            {
                pipingSoilLayer.DiameterD70Mean = DiameterD70Mean.Value;
            }
            if (DiameterD70Deviation.HasValue)
            {
                pipingSoilLayer.DiameterD70Deviation = DiameterD70Deviation.Value;
            }
            if (PermeabilityMean.HasValue)
            {
                pipingSoilLayer.PermeabilityMean = PermeabilityMean.Value;
            }
            if (PermeabilityDeviation.HasValue)
            {
                pipingSoilLayer.PermeabilityDeviation = PermeabilityDeviation.Value;
            }
        }

        /// <summary>
        /// Validates whether the values of the distribution and shift for the stochastic parameters 
        /// are correct for creating a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <exception cref="SoilLayerConversionException">Thrown when any of the distributions of the
        /// stochastic parameters is not defined as lognormal.</exception>
        protected void ValidateStochasticParametersForPiping()
        {
            ValidateIsLogNormal(
                BelowPhreaticLevelDistribution, 
                BelowPhreaticLevelShift,
                Resources.SoilLayer_BelowPhreaticLevelDistribution_Description);
            ValidateIsLogNormal(
                DiameterD70Distribution, 
                DiameterD70Shift,
                Resources.SoilLayer_DiameterD70Distribution_Description);
            ValidateIsLogNormal(
                PermeabilityDistribution, 
                PermeabilityShift,
                Resources.SoilLayer_PermeabilityDistribution_Description);
        }

        private static void ValidateIsLogNormal(long? distribution, double? shift, string incorrectDistibutionParameter)
        {
            if (distribution.HasValue && (distribution != SoilLayerConstants.LogNormalDistributionValue || shift != 0.0))
            {
                throw new SoilLayerConversionException(string.Format(
                    Resources.SoilLayer_Stochastic_parameter_0_has_no_lognormal_distribution,
                    incorrectDistibutionParameter));
            }
        }
    }
}
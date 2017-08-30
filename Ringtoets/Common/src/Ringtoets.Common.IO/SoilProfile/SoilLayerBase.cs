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

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class containing parameters which are defined for all soil layers.
    /// </summary>
    public abstract class SoilLayerBase
    {
        private string materialName;

        /// <summary>
        /// Creates a new instance of <see cref="SoilLayerBase"/>.
        /// </summary>
        protected SoilLayerBase()
        {
            materialName = string.Empty;

            BelowPhreaticLevelMean = double.NaN;
            BelowPhreaticLevelDeviation = double.NaN;
            BelowPhreaticLevelShift = double.NaN;

            DiameterD70Mean = double.NaN;
            DiameterD70CoefficientOfVariation = double.NaN;
            DiameterD70Shift = double.NaN;

            PermeabilityMean = double.NaN;
            PermeabilityCoefficientOfVariation = double.NaN;
            PermeabilityShift = double.NaN;

            AbovePhreaticLevelMean = double.NaN;
            AbovePhreaticLevelDeviation = double.NaN;
            AbovePhreaticLevelShift = double.NaN;

            CohesionMean = double.NaN;
            CohesionDeviation = double.NaN;
            CohesionShift = double.NaN;

            FrictionAngleMean = double.NaN;
            FrictionAngleDeviation = double.NaN;
            FrictionAngleShift = double.NaN;

            ShearStrengthRatioMean = double.NaN;
            ShearStrengthRatioDeviation = double.NaN;
            ShearStrengthRatioShift = double.NaN;

            StrengthIncreaseExponentMean = double.NaN;
            StrengthIncreaseExponentDeviation = double.NaN;
            StrengthIncreaseExponentShift = double.NaN;

            PopMean = double.NaN;
            PopDeviation = double.NaN;
            PopShift = double.NaN;
        }

        /// <summary>
        /// Gets or sets a value representing whether the layer is an aquifer.
        /// </summary>
        public bool IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the name of the material that was assigned to the layer.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public string MaterialName
        {
            get
            {
                return materialName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                materialName = value;
            }
        }

        /// <summary>
        /// Gets or sets the value representing the color that was used to represent the layer.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the volumic weight of the layer below the 
        /// phreatic level.
        /// [kN/m³]
        /// </summary>
        public long? BelowPhreaticLevelDistribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the volumic weight of the layer 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelShift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the volumic weight of the layer 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelDeviation { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the mean diameter of small scale tests applied to different kinds of sand, on which the 
        /// formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public long? DiameterD70Distribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double DiameterD70Shift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double DiameterD70Mean { get; set; }

        /// <summary>
        /// Gets or sets the coefficient of variation of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double DiameterD70CoefficientOfVariation { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public long? PermeabilityDistribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double PermeabilityShift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double PermeabilityMean { get; set; }

        /// <summary>
        /// Gets or sets the coefficient of variation of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double PermeabilityCoefficientOfVariation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use POP for the layer.
        /// </summary>
        public double? UsePop { get; set; }

        /// <summary>
        /// Gets or sets the shear strength model to use for the layer.
        /// </summary>
        public double? ShearStrengthModel { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public long? AbovePhreaticLevelDistribution { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double AbovePhreaticLevelMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double AbovePhreaticLevelDeviation { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double AbovePhreaticLevelShift { get; set; }
        
        /// <summary>
        /// Gets or sets the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public long? CohesionDistribution { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public double CohesionMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public double CohesionDeviation { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public double CohesionShift { get; set; }

        /// <summary>
        /// Gets or sets the the distribution for the friction angle
        /// [°]
        /// </summary>
        public long? FrictionAngleDistribution { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the friction angle
        /// [°]
        /// </summary>
        public double FrictionAngleMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the friction angle.
        /// [°]
        /// </summary>
        public double FrictionAngleDeviation { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the friction angle.
        /// [°]
        /// </summary>
        public double FrictionAngleShift { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the ratio of shear strength S
        /// [-]
        /// </summary>
        public long? ShearStrengthRatioDistribution { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the ratio of shear strength S
        /// [-]
        /// </summary>
        public double ShearStrengthRatioMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the ratio of shear strength S.
        /// [-]
        /// </summary>
        public double ShearStrengthRatioDeviation { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the ratio of shear strength S.
        /// [-]
        /// </summary>
        public double ShearStrengthRatioShift { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the strength increase exponent (m)
        /// [-]
        /// </summary>
        public long? StrengthIncreaseExponentDistribution { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the strength increase exponent (m)
        /// [-]
        /// </summary>
        public double StrengthIncreaseExponentMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the strength increase exponent (m).
        /// [-]
        /// </summary>
        public double StrengthIncreaseExponentDeviation { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the strength increase exponent (m).
        /// [-]
        /// </summary>
        public double StrengthIncreaseExponentShift { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the POP
        /// [kN/m²]
        /// </summary>
        public long? PopDistribution { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the POP
        /// [kN/m²]
        /// </summary>
        public double PopMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the POP.
        /// [kN/m²]
        /// </summary>
        public double PopDeviation { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the POP.
        /// [kN/m²]
        /// </summary>
        public double PopShift { get; set; }
    }
}
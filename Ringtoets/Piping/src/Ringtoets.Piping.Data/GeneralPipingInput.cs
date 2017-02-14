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

using System;
using System.Globalization;
using Core.Common.Base.Data;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class that holds all the overarching piping calculation input parameters, e.g. the
    /// values that apply for all calculations.
    /// </summary>
    public class GeneralPipingInput
    {
        private const int waterVolumicWeightNumberOfDecimalPlaces = 2;
        private RoundedDouble waterVolumetricWeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralPipingInput"/> class.
        /// </summary>
        public GeneralPipingInput()
        {
            UpliftModelFactor = 1.0;
            SellmeijerModelFactor = 1.0;
            waterVolumetricWeight = new RoundedDouble(waterVolumicWeightNumberOfDecimalPlaces, 9.81);
            CriticalHeaveGradient = 0.3;
            WhitesDragCoefficient = 0.25;
            BeddingAngle = 37;
            WaterKinematicViscosity = 1.33e-6;
            Gravity = 9.81;
            MeanDiameter70 = 2.08e-4;
            SellmeijerReductionFactor = 0.3;
        }

        #region General parameters (used by multiple calculations)

        private static readonly Range<RoundedDouble> waterVolumetricWeightValidityRange = new Range<RoundedDouble>(new RoundedDouble(waterVolumicWeightNumberOfDecimalPlaces),
                                                                                                                   new RoundedDouble(waterVolumicWeightNumberOfDecimalPlaces, 20.0));

        /// <summary>
        /// Gets the volumetric weight of water.
        /// [kN/m³]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value
        /// is set to <see cref="double.NaN"/> or falls out of range [0, 20].</exception>
        public RoundedDouble WaterVolumetricWeight
        {
            get
            {
                return waterVolumetricWeight;
            }
            set
            {
                var newValue = value.ToPrecision(waterVolumicWeightNumberOfDecimalPlaces);

                if (!waterVolumetricWeightValidityRange.InRange(newValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(Resources.GeneralPipingInput_WaterVolumetricWeight_must_be_in_Range__0_,
                                                                                       waterVolumetricWeightValidityRange));
                }

                waterVolumetricWeight = newValue;
            }
        }

        #endregion

        #region Heave specific parameters

        /// <summary>
        /// Gets the critical exit gradient for heave.
        /// </summary>
        public double CriticalHeaveGradient { get; private set; }

        #endregion

        #region Model factors

        /// <summary>
        /// Gets the calculation value used to account for uncertainty in the model for uplift.
        /// </summary>
        public double UpliftModelFactor { get; private set; }

        /// <summary>
        /// Gets the calculation value used to account for uncertainty in the model for Sellmeijer.
        /// </summary>
        public double SellmeijerModelFactor { get; private set; }

        #endregion

        #region Sellmeijer specific parameters

        /// <summary>
        /// Gets the (lowerbound) volumic weight of sand grain material of a sand layer 
        /// under water.
        /// [kN/m³]
        /// </summary>
        public RoundedDouble SandParticlesVolumicWeight
        {
            get
            {
                return new RoundedDouble(2, 26.0 - waterVolumetricWeight.Value);
            }
        }

        /// <summary>
        /// Gets the White's drag coefficient.
        /// </summary>
        public double WhitesDragCoefficient { get; private set; }

        /// <summary>
        /// Gets the angle of the force balance representing the amount in which sand 
        /// grains resist rolling.
        /// [°]
        /// </summary>
        public double BeddingAngle { get; private set; }

        /// <summary>
        /// Gets the kinematic viscosity of water at 10 °C.
        /// [m²/s]
        /// </summary>
        public double WaterKinematicViscosity { get; private set; }

        /// <summary>
        /// Gets the gravitational acceleration.
        /// [m/s²]
        /// </summary>
        public double Gravity { get; private set; }

        /// <summary>
        /// Gets the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70 { get; private set; }

        /// <summary>
        /// Gets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor { get; private set; }

        #endregion
    }
}
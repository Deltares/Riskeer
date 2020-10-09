// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Piping.Data.Properties;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// The general input parameters that apply to each piping calculation.
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
            UpliftModelFactor = new LogNormalDistribution
            {
                Mean = (RoundedDouble) 1.0,
                StandardDeviation = (RoundedDouble) 0.1
            };
            sellmeijerModelFactor = new LogNormalDistribution
            {
                Mean = (RoundedDouble)1.0,
                StandardDeviation = (RoundedDouble)0.12
            };
            waterVolumetricWeight = new RoundedDouble(waterVolumicWeightNumberOfDecimalPlaces, 9.81);
            CriticalHeaveGradient = 0.3;
            WhitesDragCoefficient = 0.25;
            BeddingAngle = 37;
            WaterKinematicViscosity = 1.33e-6;
            Gravity = 9.81;
            MeanDiameter70 = 2.08e-4;
            SellmeijerReductionFactor = 0.3;
        }

        #region Heave specific parameters

        /// <summary>
        /// Gets the critical exit gradient for heave.
        /// </summary>
        public double CriticalHeaveGradient { get; }

        #endregion

        #region General parameters (used by multiple calculations)

        private static readonly Range<RoundedDouble> waterVolumetricWeightValidityRange = new Range<RoundedDouble>(new RoundedDouble(waterVolumicWeightNumberOfDecimalPlaces),
                                                                                                                   new RoundedDouble(waterVolumicWeightNumberOfDecimalPlaces, 20.0));

        private LogNormalDistribution upliftModelFactor;
        private readonly LogNormalDistribution sellmeijerModelFactor;

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
                RoundedDouble newValue = value.ToPrecision(waterVolumicWeightNumberOfDecimalPlaces);

                if (!waterVolumetricWeightValidityRange.InRange(newValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(Resources.GeneralPipingInput_WaterVolumetricWeight_must_be_in_Range_0_,
                                                                                       waterVolumetricWeightValidityRange));
                }

                waterVolumetricWeight = newValue;
            }
        }

        #endregion

        #region Model factors

        /// <summary>
        /// Gets or sets the calculation value used to account for uncertainty in the model for uplift.
        /// </summary>
        public LogNormalDistribution UpliftModelFactor 
        {
            get
            {
                return upliftModelFactor;
            }
            set
            {
                upliftModelFactor.Mean = value.Mean;
                upliftModelFactor.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets the calculation value used to account for uncertainty in the model for Sellmeijer.
        /// </summary>
        public LogNormalDistribution SellmeijerModelFactor
        {
            get
            {
                return sellmeijerModelFactor;
            }
            set
            {
                sellmeijerModelFactor.Mean = value.Mean;
                sellmeijerModelFactor.StandardDeviation = value.StandardDeviation;
            }
        }

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
        public double WhitesDragCoefficient { get; }

        /// <summary>
        /// Gets the angle of the force balance representing the amount in which sand 
        /// grains resist rolling.
        /// [°]
        /// </summary>
        public double BeddingAngle { get; }

        /// <summary>
        /// Gets the kinematic viscosity of water at 10 °C.
        /// [m²/s]
        /// </summary>
        public double WaterKinematicViscosity { get; }

        /// <summary>
        /// Gets the gravitational acceleration.
        /// [m/s²]
        /// </summary>
        public double Gravity { get; }

        /// <summary>
        /// Gets the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70 { get; }

        /// <summary>
        /// Gets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor { get; }

        #endregion
    }
}
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

using Core.Common.Base;

using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class that holds all piping calculation specific input parameters, e.g. the values
    /// that can differ various calculations.
    /// </summary>
    public class PipingInput : Observable
    {
        public const double SeepageLengthStandardDeviationFraction = 0.1;
        private readonly GeneralPipingInput generalInputParameters;
        private double assessmentLevel;
        private double exitPointL;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingInput"/> class.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that
        /// are the same across all piping calculations.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="generalInputParameters"/>
        /// is <c>null</c>.</exception>
        public PipingInput(GeneralPipingInput generalInputParameters)
        {
            if (generalInputParameters == null)
            {
                throw new ArgumentNullException("generalInputParameters");
            }

            this.generalInputParameters = generalInputParameters;

            // Defaults as they have been defined in 'functional design semi-probabilistic assessments 1209431-008-ZWS-0009 Version 2 Final'
            ExitPointL = double.NaN;
            PhreaticLevelExit = new NormalDistribution();
            DampingFactorExit = new LognormalDistribution
            {
                Mean = 1.0
            };
            ThicknessCoverageLayer = new LognormalDistribution
            {
                Mean = double.NaN,
                StandardDeviation = 0.5
            };
            SeepageLength = new LognormalDistribution
            {
                Mean = double.NaN,
                StandardDeviation = double.NaN
            };
            Diameter70 = new LognormalDistribution();
            DarcyPermeability = new LognormalDistribution();
            ThicknessAquiferLayer = new LognormalDistribution
            {
                Mean = double.NaN,
                StandardDeviation = 0.5
            };
            assessmentLevel = double.NaN;
        }

        /// <summary>
        /// Gets or sets the outside high water level.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is <see cref="double.NaN"/>.</exception>
        public double AssessmentLevel
        {
            get
            {
                return assessmentLevel;
            }
            set
            {
                if (double.IsNaN(value))
                {
                    throw new ArgumentException(Resources.PipingInput_AssessmentLevel_Cannot_set_to_NaN);
                }
                assessmentLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less or equal to 0.</exception>
        public double ExitPointL
        {
            get
            {
                return exitPointL;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.PipingInput_ExitPointL_Value_must_be_greater_than_zero);
                }
                exitPointL = value;
            }
        }

        /// <summary>
        /// Gets or sets the piezometric head at the exit point.
        /// [m]
        /// </summary>
        public double PiezometricHeadExit { get; set; }

        /// <summary>
        /// Gets or sets the piezometric head in the hinterland.
        /// [m]
        /// </summary>
        public double PiezometricHeadPolder { get; set; }

        /// <summary>
        /// Gets or sets the surface line.
        /// </summary>
        public RingtoetsPipingSurfaceLine SurfaceLine { get; set; }

        /// <summary>
        /// Gets or sets the profile which contains a 1 dimensional definition of soil layers with properties.
        /// </summary>
        public PipingSoilProfile SoilProfile { get; set; }

        /// <summary>
        /// Gets or set the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        #region General input parameters

        /// <summary>
        /// Gets or sets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor
        {
            get
            {
                return generalInputParameters.SellmeijerReductionFactor;
            }
        }

        /// <summary>
        /// Gets or sets the volumetric weight of water.
        /// [kN/m³]
        /// </summary>
        public double WaterVolumetricWeight
        {
            get
            {
                return generalInputParameters.WaterVolumetricWeight;
            }
        }

        /// <summary>
        /// Gets or sets the (lowerbound) volumic weight of sand grain material of a sand layer under water.
        /// [kN/m³]
        /// </summary>
        public double SandParticlesVolumicWeight
        {
            get
            {
                return generalInputParameters.SandParticlesVolumicWeight;
            }
        }

        /// <summary>
        /// Gets or sets the White's drag coefficient.
        /// </summary>
        public double WhitesDragCoefficient
        {
            get
            {
                return generalInputParameters.WhitesDragCoefficient;
            }
        }

        /// <summary>
        /// Gets or sets the kinematic viscosity of water at 10 degrees Celsius.
        /// [m²/s]
        /// </summary>
        public double WaterKinematicViscosity
        {
            get
            {
                return generalInputParameters.WaterKinematicViscosity;
            }
        }

        /// <summary>
        /// Gets or sets the gravitational acceleration.
        /// [m/s²]
        /// </summary>
        public double Gravity
        {
            get
            {
                return generalInputParameters.Gravity;
            }
        }

        /// <summary>
        /// Gets or sets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70
        {
            get
            {
                return generalInputParameters.MeanDiameter70;
            }
        }

        /// <summary>
        /// Gets or sets the angle of the force balance representing the amount in which sand grains resist rolling.
        /// [°]
        /// </summary>
        public double BeddingAngle
        {
            get
            {
                return generalInputParameters.BeddingAngle;
            }
        }

        /// <summary>
        /// Gets or sets the calculation value used to account for uncertainty in the model for uplift.
        /// </summary>
        public double UpliftModelFactor
        {
            get
            {
                return generalInputParameters.UpliftModelFactor;
            }
        }

        /// <summary>
        /// Gets or sets the calculation value used to account for uncertainty in the model for Sellmeijer.
        /// </summary>
        public double SellmeijerModelFactor
        {
            get
            {
                return generalInputParameters.SellmeijerModelFactor;
            }
        }

        /// <summary>
        /// Gets or sets the critical exit gradient for heave.
        /// </summary>
        public double CriticalHeaveGradient
        {
            get
            {
                return generalInputParameters.CriticalHeaveGradient;
            }
        }

        #endregion

        #region Probabilistic parameters

        /// <summary>
        /// Gets or sets the phreatic level at the exit point.
        /// [m]
        /// </summary>
        public NormalDistribution PhreaticLevelExit { get; set; }

        /// <summary>
        /// Gets or sets the horizontal distance between entree and exit point.
        /// [m]
        /// </summary>
        public LognormalDistribution SeepageLength { get; set; }

        /// <summary>
        /// Gets or sets the sieve size through which 70% fraction of the grains of the top part of the aquifer passes.
        /// [m]
        /// </summary>
        public LognormalDistribution Diameter70 { get; set; }

        /// <summary>
        /// Gets or sets the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public LognormalDistribution DarcyPermeability { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the aquifer layer.
        /// [m]
        /// </summary>
        public LognormalDistribution ThicknessAquiferLayer { get; set; }

        /// <summary>
        /// Gets or sets the total thickness of the coverage layer at the exit point.
        /// [m]
        /// </summary>
        public LognormalDistribution ThicknessCoverageLayer { get; set; }

        /// <summary>
        /// Gets or sets the damping factor at the exit point.
        /// </summary>
        public LognormalDistribution DampingFactorExit { get; set; }

        #endregion
    }
}
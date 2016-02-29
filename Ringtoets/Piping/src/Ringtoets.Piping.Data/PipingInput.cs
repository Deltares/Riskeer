// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class that holds all piping calculation input parameters.
    /// </summary>
    public class PipingInput : Observable
    {
        private RingtoetsPipingSurfaceLine surfaceLine;
        private double assessmentLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingInput"/> class.
        /// </summary>
        public PipingInput()
        {
            // Defaults as they have been defined in 'functional design semi-probabilistic assessments 1209431-008-ZWS-0009 Version 2 Final'
            UpliftModelFactor = 1.0;
            SellmeijerModelFactor = 1.0;
            WaterVolumetricWeight = 10.0;
            WhitesDragCoefficient = 0.25;
            SandParticlesVolumicWeight = 16.5;
            WaterKinematicViscosity = 1.33e-6;
            Gravity = 9.81;
            MeanDiameter70 = 2.08e-4;
            BeddingAngle = 37.0;
            SellmeijerReductionFactor = 0.3;
            CriticalHeaveGradient = 0.3;

            PhreaticLevelExit = new NormalDistribution();
            DampingFactorExit = new LognormalDistribution
            {
                Mean = 1.0
            };
            ThicknessCoverageLayer = new LognormalDistribution();
            SeepageLength = new LognormalDistribution();
            Diameter70 = new LognormalDistribution();
            DarcyPermeability = new LognormalDistribution();
            ThicknessAquiferLayer = new LognormalDistribution();
        }

        /// <summary>
        /// Gets or sets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor { get; set; }

        /// <summary>
        /// Gets or sets the volumetric weight of water.
        /// [kN/m³]
        /// </summary>
        public double WaterVolumetricWeight { get; set; }

        /// <summary>
        /// Gets or sets the (lowerbound) volumic weight of sand grain material of a sand layer under water.
        /// [kN/m³]
        /// </summary>
        public double SandParticlesVolumicWeight { get; set; }

        /// <summary>
        /// Gets or sets the White's drag coefficient.
        /// </summary>
        public double WhitesDragCoefficient { get; set; }

        /// <summary>
        /// Gets or sets the kinematic viscosity of water at 10 degrees Celsius.
        /// [m²/s]
        /// </summary>
        public double WaterKinematicViscosity { get; set; }

        /// <summary>
        /// Gets or sets the gravitational acceleration.
        /// [m/s²]
        /// </summary>
        public double Gravity { get; set; }

        /// <summary>
        /// Gets or sets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70 { get; set; }

        /// <summary>
        /// Gets or sets the angle of the force balance representing the amount in which sand grains resist rolling.
        /// [°]
        /// </summary>
        public double BeddingAngle { get; set; }

        /// <summary>
        /// Gets or sets the calculation value used to account for uncertainty in the model for uplift.
        /// </summary>
        public double UpliftModelFactor { get; set; }

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
        /// Gets or sets the calculation value used to account for uncertainty in the model for Sellmeijer.
        /// </summary>
        public double SellmeijerModelFactor { get; set; }

        /// <summary>
        /// Gets or sets the L-coordinate of the exit point.
        /// [m]
        /// </summary>
        public double ExitPointL { get; set; }

        #region Constants

        /// <summary>
        /// Gets or sets the critical exit gradient for heave.
        /// </summary>
        public double CriticalHeaveGradient { get; private set; }

        #endregion

        /// <summary>
        /// Gets or sets the surface line.
        /// </summary>
        public RingtoetsPipingSurfaceLine SurfaceLine
        {
            get
            {
                return surfaceLine;
            }
            set
            {
                surfaceLine = value;
                UpdateValuesBasedOnSurfaceLine();
            }
        }

        /// <summary>
        /// Gets or sets the profile which contains a 1 dimensional definition of soil layers with properties.
        /// </summary>
        public PipingSoilProfile SoilProfile { get; set; }

        /// <summary>
        /// Gets or set the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        private void UpdateValuesBasedOnSurfaceLine()
        {
            var entryPointIndex = Array.IndexOf(surfaceLine.Points, surfaceLine.DikeToeAtRiver);
            var exitPointIndex = Array.IndexOf(surfaceLine.Points, surfaceLine.DikeToeAtPolder);

            var localGeometry = surfaceLine.ProjectGeometryToLZ().ToArray();

            var entryPointL = localGeometry[0].X;
            var exitPointL = localGeometry[localGeometry.Length - 1].X;

            var differentPoints = entryPointIndex < 0 || exitPointIndex < 0 || entryPointIndex < exitPointIndex;
            if (differentPoints && exitPointIndex > 0)
            {
                exitPointL = localGeometry.ElementAt(exitPointIndex).X;
            }
            if (differentPoints && entryPointIndex > -1)
            {
                entryPointL = localGeometry.ElementAt(entryPointIndex).X;
            }

            ExitPointL = exitPointL;
            SeepageLength.Mean = exitPointL - entryPointL;
        }

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
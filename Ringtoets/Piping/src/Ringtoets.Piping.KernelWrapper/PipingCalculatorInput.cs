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

using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper
{
    /// <summary>
    /// This class contains all the parameters that are required to perform a piping assessment.
    /// </summary>
    public class PipingCalculatorInput
    {
        private readonly double waterVolumetricWeight;
        private readonly double upliftModelFactor;
        private readonly double assessmentLevel;
        private readonly double piezometricHeadExit;
        private readonly double dampingFactorExit;
        private readonly double phreaticLevelExit;
        private readonly double criticalHeaveGradient;
        private readonly double thicknessCoverageLayer;
        private readonly double effectiveThicknessCoverageLayer;
        private readonly double sellmeijerModelFactor;
        private readonly double sellmeijerReductionFactor;
        private readonly double seepageLength;
        private readonly double sandParticlesVolumicWeight;
        private readonly double whitesDragCoefficient;
        private readonly double diameter70;
        private readonly double darcyPermeability;
        private readonly double waterKinematicViscosity;
        private readonly double gravity;
        private readonly double thicknessAquiferLayer;
        private readonly double meanDiameter70;
        private readonly double beddingAngle;
        private readonly double exitPointXCoordinate;
        private readonly RingtoetsPipingSurfaceLine surfaceLine;
        private readonly PipingSoilProfile soilProfile;
        private readonly double saturatedVolumicWeightOfCoverageLayer;

        /// <summary>
        /// Constructs a new <see cref="PipingCalculatorInput"/>, which contains values for the parameters used
        /// in the piping sub calculations.
        /// </summary>
        /// <param name="waterVolumetricWeight">The volumetric weight of water. [kN/m³]</param>
        /// <param name="saturatedVolumicWeightOfCoverageLayer">The volumic weight of the coverage layer when saturated.</param>
        /// <param name="upliftModelFactor">The calculation value used to account for uncertainty in the model for uplift.</param>
        /// <param name="assessmentLevel">The outside high water level. [m]</param>
        /// <param name="piezometricHeadExit">The piezometric head at the exit point. [m]</param>
        /// <param name="dampingFactorExit">The damping factor at the exit point. </param>
        /// <param name="phreaticLevelExit">The phreatic level at the exit point. [m]</param>
        /// <param name="criticalHeaveGradient">The critical exit gradient for heave.</param>
        /// <param name="thicknessCoverageLayer">The total thickness of the coverage layer at the exit point. [m]</param>
        /// <param name="effectiveThicknessCoverageLayer">The effective thickness of the coverage layer at the exit point. [m]</param>
        /// <param name="sellmeijerModelFactor">The calculation value used to account for uncertainty in the model for Sellmeijer.</param>
        /// <param name="sellmeijerReductionFactor">The reduction factor Sellmeijer.</param>
        /// <param name="seepageLength">The horizontal distance between entry and exit point. [m]</param>
        /// <param name="sandParticlesVolumicWeight">The (lowerbound) volumic weight of sand grain material of a sand layer under water. [kN/m³]</param>
        /// <param name="whitesDragCoefficient">The White's drag coefficient.</param>
        /// <param name="diameter70">The sieve size through which 70% of the grains of the top part of the aquifer pass. [m]</param>
        /// <param name="darcyPermeability">The Darcy-speed with which water flows through the aquifer layer. [m/s]</param>
        /// <param name="waterKinematicViscosity">The kinematic viscosity of water at 10 °C. [m²/s]</param>
        /// <param name="gravity">The gravitational acceleration. [m/s²]</param>
        /// <param name="thicknessAquiferLayer">The thickness of the aquifer layer. [m]</param>
        /// <param name="meanDiameter70">The mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit. [m]</param>
        /// <param name="beddingAngle">The angle of the force balance representing the amount in which sand grains resist rolling. [°]</param>
        /// <param name="exitPointXCoordinate">The X-coordinate of the exit point. [m]</param>
        /// <param name="surfaceLine">The surface line.</param>
        /// <param name="soilProfile">The profile which contains a 1 dimensional definition of soil layers with properties.</param>
        public PipingCalculatorInput(double waterVolumetricWeight, double saturatedVolumicWeightOfCoverageLayer, double upliftModelFactor, double assessmentLevel, double piezometricHeadExit, double dampingFactorExit, double phreaticLevelExit, double criticalHeaveGradient, double thicknessCoverageLayer, double effectiveThicknessCoverageLayer, double sellmeijerModelFactor, double sellmeijerReductionFactor, double seepageLength, double sandParticlesVolumicWeight, double whitesDragCoefficient, double diameter70, double darcyPermeability, double waterKinematicViscosity, double gravity, double thicknessAquiferLayer, double meanDiameter70, double beddingAngle, double exitPointXCoordinate, RingtoetsPipingSurfaceLine surfaceLine, PipingSoilProfile soilProfile)
        {
            this.waterVolumetricWeight = waterVolumetricWeight;
            this.saturatedVolumicWeightOfCoverageLayer = saturatedVolumicWeightOfCoverageLayer;
            this.upliftModelFactor = upliftModelFactor;
            this.assessmentLevel = assessmentLevel;
            this.piezometricHeadExit = piezometricHeadExit;
            this.dampingFactorExit = dampingFactorExit;
            this.phreaticLevelExit = phreaticLevelExit;
            this.criticalHeaveGradient = criticalHeaveGradient;
            this.thicknessCoverageLayer = thicknessCoverageLayer;
            this.effectiveThicknessCoverageLayer = effectiveThicknessCoverageLayer;
            this.sellmeijerModelFactor = sellmeijerModelFactor;
            this.sellmeijerReductionFactor = sellmeijerReductionFactor;
            this.seepageLength = seepageLength;
            this.sandParticlesVolumicWeight = sandParticlesVolumicWeight;
            this.whitesDragCoefficient = whitesDragCoefficient;
            this.diameter70 = diameter70;
            this.darcyPermeability = darcyPermeability;
            this.waterKinematicViscosity = waterKinematicViscosity;
            this.gravity = gravity;
            this.thicknessAquiferLayer = thicknessAquiferLayer;
            this.meanDiameter70 = meanDiameter70;
            this.beddingAngle = beddingAngle;
            this.exitPointXCoordinate = exitPointXCoordinate;
            this.surfaceLine = surfaceLine;
            this.soilProfile = soilProfile;
        }

        #region properties

        /// <summary>
        /// Gets the volumetric weight of water.
        /// [kN/m³]
        /// </summary>
        public double WaterVolumetricWeight
        {
            get
            {
                return waterVolumetricWeight;
            }
        }

        /// <summary>
        /// Gets the calculation value used to account for uncertainty in the model for uplift.
        /// </summary>
        public double UpliftModelFactor
        {
            get
            {
                return upliftModelFactor;
            }
        }

        /// <summary>
        /// Gets the outside high water level.
        /// [m]
        /// </summary>
        public double AssessmentLevel
        {
            get
            {
                return assessmentLevel;
            }
        }

        /// <summary>
        /// Gets the piezometric head at the exit point.
        /// [m]
        /// </summary>
        public double PiezometricHeadExit
        {
            get
            {
                return piezometricHeadExit;
            }
        }

        /// <summary>
        /// Gets the damping factor at the exit point.
        /// </summary>
        public double DampingFactorExit
        {
            get
            {
                return dampingFactorExit;
            }
        }

        /// <summary>
        /// Gets the phreatic level at the exit point.
        /// [m]
        /// </summary>
        public double PhreaticLevelExit
        {
            get
            {
                return phreaticLevelExit;
            }
        }

        /// <summary>
        /// Gets the critical exit gradient for heave.
        /// </summary>
        public double CriticalHeaveGradient
        {
            get
            {
                return criticalHeaveGradient;
            }
        }

        /// <summary>
        /// Gets the total thickness of the coverage layer at the exit point.
        /// [m]
        /// </summary>
        public double ThicknessCoverageLayer
        {
            get
            {
                return thicknessCoverageLayer;
            }
        }

        /// <summary>
        /// Gets the effective thickness of the coverage layer at the exit point.
        /// [m]
        /// </summary>
        public double EffectiveThicknessCoverageLayer
        {
            get
            {
                return effectiveThicknessCoverageLayer;
            }
        }

        /// <summary>
        /// Gets the calculation value used to account for uncertainty in the model for Sellmeijer.
        /// </summary>
        public double SellmeijerModelFactor
        {
            get
            {
                return sellmeijerModelFactor;
            }
        }

        /// <summary>
        /// Gets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor
        {
            get
            {
                return sellmeijerReductionFactor;
            }
        }

        /// <summary>
        /// Gets the horizontal distance between entry and exit point.
        /// [m]
        /// </summary>
        public double SeepageLength
        {
            get
            {
                return seepageLength;
            }
        }

        /// <summary>
        /// Gets the (lowerbound) volumic weight of sand grain material of a sand layer under water.
        /// [kN/m³]
        /// </summary>
        public double SandParticlesVolumicWeight
        {
            get
            {
                return sandParticlesVolumicWeight;
            }
        }

        /// <summary>
        /// Gets the White's drag coefficient.
        /// </summary>
        public double WhitesDragCoefficient
        {
            get
            {
                return whitesDragCoefficient;
            }
        }

        /// <summary>
        /// Gets the sieve size through which 70% of the grains of the top part of the aquifer pass.
        /// [m]
        /// </summary>
        public double Diameter70
        {
            get
            {
                return diameter70;
            }
        }

        /// <summary>
        /// Gets the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double DarcyPermeability
        {
            get
            {
                return darcyPermeability;
            }
        }

        /// <summary>
        /// Gets the kinematic viscosity of water at 10 °C.
        /// [m²/s]
        /// </summary>
        public double WaterKinematicViscosity
        {
            get
            {
                return waterKinematicViscosity;
            }
        }

        /// <summary>
        /// Gets the gravitational acceleration.
        /// [m/s²]
        /// </summary>
        public double Gravity
        {
            get
            {
                return gravity;
            }
        }

        /// <summary>
        /// Gets the thickness of the aquifer layer.
        /// [m]
        /// </summary>
        public double ThicknessAquiferLayer
        {
            get
            {
                return thicknessAquiferLayer;
            }
        }

        /// <summary>
        /// Gets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70
        {
            get
            {
                return meanDiameter70;
            }
        }

        /// <summary>
        /// Gets the angle of the force balance representing the amount in which sand grains resist rolling.
        /// [°]
        /// </summary>
        public double BeddingAngle
        {
            get
            {
                return beddingAngle;
            }
        }

        /// <summary>
        /// Gets the X-coordinate of the exit point.
        /// [m]
        /// </summary>
        public double ExitPointXCoordinate
        {
            get
            {
                return exitPointXCoordinate;
            }
        }

        /// <summary>
        /// Gets the surface line.
        /// </summary>
        public RingtoetsPipingSurfaceLine SurfaceLine
        {
            get
            {
                return surfaceLine;
            }
        }

        /// <summary>
        /// Gets the profile which contains a 1 dimensional definition of soil layers with properties.
        /// </summary>
        public PipingSoilProfile SoilProfile
        {
            get
            {
                return soilProfile;
            }
        }

        /// <summary>
        /// Gets the volumic weight of the coverage layer when saturated.
        /// </summary>
        public double SaturatedVolumicWeightOfCoverageLayer
        {
            get
            {
                return saturatedVolumicWeightOfCoverageLayer;
            }
        }

        #endregion
    }
}
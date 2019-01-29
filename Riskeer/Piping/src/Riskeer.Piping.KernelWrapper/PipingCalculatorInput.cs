// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Piping.Primitives;

namespace Riskeer.Piping.KernelWrapper
{
    /// <summary>
    /// This class contains all the parameters that are required to perform a piping assessment.
    /// </summary>
    public class PipingCalculatorInput
    {
        /// <summary>
        /// Constructs a new <see cref="PipingCalculatorInput"/>, which contains values for the parameters used
        /// in the piping sub calculations.
        /// </summary>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="PipingCalculatorInput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is <c>null</c>.</exception>
        public PipingCalculatorInput(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            WaterVolumetricWeight = properties.WaterVolumetricWeight;
            SaturatedVolumicWeightOfCoverageLayer = properties.SaturatedVolumicWeightOfCoverageLayer;
            UpliftModelFactor = properties.UpliftModelFactor;
            AssessmentLevel = properties.AssessmentLevel;
            PiezometricHeadExit = properties.PiezometricHeadExit;
            DampingFactorExit = properties.DampingFactorExit;
            PhreaticLevelExit = properties.PhreaticLevelExit;
            CriticalHeaveGradient = properties.CriticalHeaveGradient;
            ThicknessCoverageLayer = properties.ThicknessCoverageLayer;
            EffectiveThicknessCoverageLayer = properties.EffectiveThicknessCoverageLayer;
            SellmeijerModelFactor = properties.SellmeijerModelFactor;
            SellmeijerReductionFactor = properties.SellmeijerReductionFactor;
            SeepageLength = properties.SeepageLength;
            SandParticlesVolumicWeight = properties.SandParticlesVolumicWeight;
            WhitesDragCoefficient = properties.WhitesDragCoefficient;
            Diameter70 = properties.Diameter70;
            DarcyPermeability = properties.DarcyPermeability;
            WaterKinematicViscosity = properties.WaterKinematicViscosity;
            Gravity = properties.Gravity;
            ThicknessAquiferLayer = properties.ThicknessAquiferLayer;
            MeanDiameter70 = properties.MeanDiameter70;
            BeddingAngle = properties.BeddingAngle;
            ExitPointXCoordinate = properties.ExitPointXCoordinate;
            SurfaceLine = properties.SurfaceLine;
            SoilProfile = properties.SoilProfile;
        }

        public class ConstructionProperties
        {
            /// <summary>
            /// Creates new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                WaterVolumetricWeight = double.NaN;
                SaturatedVolumicWeightOfCoverageLayer = double.NaN;
                UpliftModelFactor = double.NaN;
                AssessmentLevel = double.NaN;
                PiezometricHeadExit = double.NaN;
                DampingFactorExit = double.NaN;
                PhreaticLevelExit = double.NaN;
                CriticalHeaveGradient = double.NaN;
                ThicknessCoverageLayer = double.NaN;
                EffectiveThicknessCoverageLayer = double.NaN;
                SellmeijerModelFactor = double.NaN;
                SellmeijerReductionFactor = double.NaN;
                SeepageLength = double.NaN;
                SandParticlesVolumicWeight = double.NaN;
                WhitesDragCoefficient = double.NaN;
                Diameter70 = double.NaN;
                DarcyPermeability = double.NaN;
                WaterKinematicViscosity = double.NaN;
                Gravity = double.NaN;
                ThicknessAquiferLayer = double.NaN;
                MeanDiameter70 = double.NaN;
                BeddingAngle = double.NaN;
                ExitPointXCoordinate = double.NaN;
                SurfaceLine = null;
                SoilProfile = null;
            }

            #region Properties

            /// <summary>
            /// Gets the volumetric weight of water.
            /// [kN/m³]
            /// </summary>
            public double WaterVolumetricWeight { internal get; set; }

            /// <summary>
            /// Gets the calculation value used to account for uncertainty in the model for uplift.
            /// </summary>
            public double UpliftModelFactor { internal get; set; }

            /// <summary>
            /// Gets the outside high water level.
            /// [m]
            /// </summary>
            public double AssessmentLevel { internal get; set; }

            /// <summary>
            /// Gets the piezometric head at the exit point.
            /// [m]
            /// </summary>
            public double PiezometricHeadExit { internal get; set; }

            /// <summary>
            /// Gets the damping factor at the exit point.
            /// </summary>
            public double DampingFactorExit { internal get; set; }

            /// <summary>
            /// Gets the phreatic level at the exit point.
            /// [m]
            /// </summary>
            public double PhreaticLevelExit { internal get; set; }

            /// <summary>
            /// Gets the critical exit gradient for heave.
            /// </summary>
            public double CriticalHeaveGradient { internal get; set; }

            /// <summary>
            /// Gets the total thickness of the coverage layer at the exit point.
            /// [m]
            /// </summary>
            public double ThicknessCoverageLayer { internal get; set; }

            /// <summary>
            /// Gets the effective thickness of the coverage layer at the exit point.
            /// [m]
            /// </summary>
            public double EffectiveThicknessCoverageLayer { internal get; set; }

            /// <summary>
            /// Gets the calculation value used to account for uncertainty in the model for Sellmeijer.
            /// </summary>
            public double SellmeijerModelFactor { internal get; set; }

            /// <summary>
            /// Gets the reduction factor Sellmeijer.
            /// </summary>
            public double SellmeijerReductionFactor { internal get; set; }

            /// <summary>
            /// Gets the horizontal distance between entry and exit point.
            /// [m]
            /// </summary>
            public double SeepageLength { internal get; set; }

            /// <summary>
            /// Gets the (lowerbound) volumic weight of sand grain material of a sand layer under water.
            /// [kN/m³]
            /// </summary>
            public double SandParticlesVolumicWeight { internal get; set; }

            /// <summary>
            /// Gets the White's drag coefficient.
            /// </summary>
            public double WhitesDragCoefficient { internal get; set; }

            /// <summary>
            /// Gets the sieve size through which 70% of the grains of the top part of the aquifer pass.
            /// [m]
            /// </summary>
            public double Diameter70 { internal get; set; }

            /// <summary>
            /// Gets the Darcy-speed with which water flows through the aquifer layer.
            /// [m/s]
            /// </summary>
            public double DarcyPermeability { internal get; set; }

            /// <summary>
            /// Gets the kinematic viscosity of water at 10 °C.
            /// [m²/s]
            /// </summary>
            public double WaterKinematicViscosity { internal get; set; }

            /// <summary>
            /// Gets the gravitational acceleration.
            /// [m/s²]
            /// </summary>
            public double Gravity { internal get; set; }

            /// <summary>
            /// Gets the thickness of the aquifer layer.
            /// [m]
            /// </summary>
            public double ThicknessAquiferLayer { internal get; set; }

            /// <summary>
            /// Gets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
            /// [m]
            /// </summary>
            public double MeanDiameter70 { internal get; set; }

            /// <summary>
            /// Gets the angle of the force balance representing the amount in which sand grains resist rolling.
            /// [°]
            /// </summary>
            public double BeddingAngle { internal get; set; }

            /// <summary>
            /// Gets the X-coordinate of the exit point.
            /// [m]
            /// </summary>
            public double ExitPointXCoordinate { internal get; set; }

            /// <summary>
            /// Gets the surface line.
            /// </summary>
            public PipingSurfaceLine SurfaceLine { internal get; set; }

            /// <summary>
            /// Gets the profile which contains a 1 dimensional definition of soil layers with properties.
            /// </summary>
            public PipingSoilProfile SoilProfile { internal get; set; }

            /// <summary>
            /// Gets the volumic weight of the coverage layer when saturated.
            /// </summary>
            public double SaturatedVolumicWeightOfCoverageLayer { internal get; set; }

            #endregion
        }

        #region Properties

        /// <summary>
        /// Gets the volumetric weight of water.
        /// [kN/m³]
        /// </summary>
        public double WaterVolumetricWeight { get; }

        /// <summary>
        /// Gets the calculation value used to account for uncertainty in the model for uplift.
        /// </summary>
        public double UpliftModelFactor { get; }

        /// <summary>
        /// Gets the outside high water level.
        /// [m]
        /// </summary>
        public double AssessmentLevel { get; }

        /// <summary>
        /// Gets the piezometric head at the exit point.
        /// [m]
        /// </summary>
        public double PiezometricHeadExit { get; }

        /// <summary>
        /// Gets the damping factor at the exit point.
        /// </summary>
        public double DampingFactorExit { get; }

        /// <summary>
        /// Gets the phreatic level at the exit point.
        /// [m]
        /// </summary>
        public double PhreaticLevelExit { get; }

        /// <summary>
        /// Gets the critical exit gradient for heave.
        /// </summary>
        public double CriticalHeaveGradient { get; }

        /// <summary>
        /// Gets the total thickness of the coverage layer at the exit point.
        /// [m]
        /// </summary>
        public double ThicknessCoverageLayer { get; }

        /// <summary>
        /// Gets the effective thickness of the coverage layer at the exit point.
        /// [m]
        /// </summary>
        public double EffectiveThicknessCoverageLayer { get; }

        /// <summary>
        /// Gets the calculation value used to account for uncertainty in the model for Sellmeijer.
        /// </summary>
        public double SellmeijerModelFactor { get; }

        /// <summary>
        /// Gets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor { get; }

        /// <summary>
        /// Gets the horizontal distance between entry and exit point.
        /// [m]
        /// </summary>
        public double SeepageLength { get; }

        /// <summary>
        /// Gets the (lowerbound) volumic weight of sand grain material of a sand layer under water.
        /// [kN/m³]
        /// </summary>
        public double SandParticlesVolumicWeight { get; }

        /// <summary>
        /// Gets the White's drag coefficient.
        /// </summary>
        public double WhitesDragCoefficient { get; }

        /// <summary>
        /// Gets the sieve size through which 70% of the grains of the top part of the aquifer pass.
        /// [m]
        /// </summary>
        public double Diameter70 { get; }

        /// <summary>
        /// Gets the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double DarcyPermeability { get; }

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
        /// Gets the thickness of the aquifer layer.
        /// [m]
        /// </summary>
        public double ThicknessAquiferLayer { get; }

        /// <summary>
        /// Gets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70 { get; }

        /// <summary>
        /// Gets the angle of the force balance representing the amount in which sand grains resist rolling.
        /// [°]
        /// </summary>
        public double BeddingAngle { get; }

        /// <summary>
        /// Gets the X-coordinate of the exit point.
        /// [m]
        /// </summary>
        public double ExitPointXCoordinate { get; }

        /// <summary>
        /// Gets the surface line.
        /// </summary>
        public PipingSurfaceLine SurfaceLine { get; }

        /// <summary>
        /// Gets the profile which contains a 1 dimensional definition of soil layers with properties.
        /// </summary>
        public PipingSoilProfile SoilProfile { get; }

        /// <summary>
        /// Gets the volumic weight of the coverage layer when saturated.
        /// </summary>
        public double SaturatedVolumicWeightOfCoverageLayer { get; }

        #endregion
    }
}
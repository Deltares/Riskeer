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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class that holds a configuration that can be changed and used to create a <see cref="PipingCalculatorInput"/>
    /// with.
    /// </summary>
    public class TestPipingInput
    {
        private readonly Random random = new Random(22);
        public double WaterVolumetricWeight;
        public double SaturatedVolumicWeightOfCoverageLayer;
        public double UpliftModelFactor;
        public RoundedDouble AssessmentLevel;
        public RoundedDouble PiezometricHeadExit;
        public double DampingFactorExit;
        public double PhreaticLevelExit;
        public double CriticalHeaveGradient;
        public double ThicknessCoverageLayer;
        public double EffectiveThicknessCoverageLayer;
        public double SellmeijerModelFactor;
        public double SellmeijerReductionFactor;
        public double SeepageLength;
        public double SandParticlesVolumicWeight;
        public double WhitesDragCoefficient;
        public double Diameter70;
        public double DarcyPermeability;
        public double WaterKinematicViscosity;
        public double Gravity;
        public RoundedDouble ExitPointXCoordinate;
        public double BeddingAngle;
        public double MeanDiameter70;
        public double ThicknessAquiferLayer;
        public RingtoetsPipingSurfaceLine SurfaceLine;
        public PipingSoilProfile SoilProfile;
        private double last;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPipingInput"/> class with all
        /// parameters relevant for <see cref="PipingCalculatorInput"/> set.
        /// </summary>
        public TestPipingInput()
        {
            WaterVolumetricWeight = NextIncrementalDouble();
            SaturatedVolumicWeightOfCoverageLayer = NextIncrementalDouble();
            UpliftModelFactor = NextIncrementalDouble();
            AssessmentLevel = (RoundedDouble) NextIncrementalDouble();
            PiezometricHeadExit = (RoundedDouble) NextIncrementalDouble();
            PhreaticLevelExit = NextIncrementalDouble();
            DampingFactorExit = NextIncrementalDouble();
            CriticalHeaveGradient = NextIncrementalDouble();
            ThicknessCoverageLayer = NextIncrementalDouble();
            EffectiveThicknessCoverageLayer = NextIncrementalDouble();
            SellmeijerModelFactor = NextIncrementalDouble();
            SellmeijerReductionFactor = NextIncrementalDouble();
            SeepageLength = NextIncrementalDouble();
            SandParticlesVolumicWeight = NextIncrementalDouble();
            WhitesDragCoefficient = NextIncrementalDouble();
            Diameter70 = NextIncrementalDouble();
            DarcyPermeability = NextIncrementalDouble();
            WaterKinematicViscosity = NextIncrementalDouble();
            Gravity = NextIncrementalDouble();
            ExitPointXCoordinate = (RoundedDouble) 0.5;
            BeddingAngle = NextIncrementalDouble();
            MeanDiameter70 = NextIncrementalDouble();
            ThicknessAquiferLayer = NextIncrementalDouble();
            SurfaceLine = CreateValidSurfaceLine();
            SoilProfile = CreateValidSoilProfile();
        }

        /// <summary>
        /// Returns the current set value as a <see cref="PipingCalculatorInput"/>
        /// </summary>
        public PipingCalculatorInput AsRealInput()
        {
            return new PipingCalculatorInput(
                WaterVolumetricWeight,
                SaturatedVolumicWeightOfCoverageLayer,
                UpliftModelFactor,
                AssessmentLevel,
                PiezometricHeadExit,
                DampingFactorExit,
                PhreaticLevelExit,
                CriticalHeaveGradient,
                ThicknessCoverageLayer,
                EffectiveThicknessCoverageLayer,
                SellmeijerModelFactor,
                SellmeijerReductionFactor,
                SeepageLength,
                SandParticlesVolumicWeight,
                WhitesDragCoefficient,
                Diameter70,
                DarcyPermeability,
                WaterKinematicViscosity,
                Gravity,
                ThicknessAquiferLayer,
                MeanDiameter70,
                BeddingAngle,
                ExitPointXCoordinate,
                SurfaceLine,
                SoilProfile);
        }

        private PipingSoilProfile CreateValidSoilProfile()
        {
            return new PipingSoilProfile(string.Empty, -2, new[]
            {
                new PipingSoilLayer(9),
                new PipingSoilLayer(4)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2),
                new PipingSoilLayer(-1),
            }, SoilProfileType.SoilProfile1D, 1234L);
        }

        private RingtoetsPipingSurfaceLine CreateValidSurfaceLine()
        {
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            ringtoetsPipingSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, 8),
                new Point3D(2, 0, -1)
            });
            return ringtoetsPipingSurfaceLine;
        }

        /// <summary>
        /// The returned double is sure to be different from the last time it was called.
        /// </summary>
        private double NextIncrementalDouble()
        {
            return last += random.NextDouble() + 1e-6;
        }
    }
}
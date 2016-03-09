using System;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.TestUtil
{
    public class TestPipingInput
    {
        public double WaterVolumetricWeight;
        public double UpliftModelFactor;
        public RoundedDouble AssessmentLevel;
        public double PiezometricHeadExit;
        public double DampingFactorExit;
        public double PhreaticLevelExit;
        public double PiezometricHeadPolder;
        public double CriticalHeaveGradient;
        public double ThicknessCoverageLayer;
        public double SellmeijerModelFactor;
        public double SellmeijerReductionFactor;
        public double SeepageLength;
        public double SandParticlesVolumicWeight;
        public double WhitesDragCoefficient;
        public double Diameter70;
        public double DarcyPermeability;
        public double WaterKinematicViscosity;
        public double Gravity;
        public double ExitPointXCoordinate;
        public double BeddingAngle;
        public double MeanDiameter70;
        public double ThicknessAquiferLayer;
        public RingtoetsPipingSurfaceLine SurfaceLine;
        public PipingSoilProfile SoilProfile;

        private readonly Random random = new Random(22);
        private double last;

        public TestPipingInput()
        {
            WaterVolumetricWeight = NextIncrementalDouble();
            UpliftModelFactor = NextIncrementalDouble();
            AssessmentLevel = new RoundedDouble(2, NextIncrementalDouble());
            PiezometricHeadExit = NextIncrementalDouble();
            PhreaticLevelExit = NextIncrementalDouble();
            DampingFactorExit = NextIncrementalDouble();
            PiezometricHeadPolder = NextIncrementalDouble();
            CriticalHeaveGradient = NextIncrementalDouble();
            ThicknessCoverageLayer = NextIncrementalDouble();
            SellmeijerModelFactor = NextIncrementalDouble();
            SellmeijerReductionFactor = NextIncrementalDouble();
            SeepageLength = NextIncrementalDouble();
            SandParticlesVolumicWeight = NextIncrementalDouble();
            WhitesDragCoefficient = NextIncrementalDouble();
            Diameter70 = NextIncrementalDouble();
            DarcyPermeability = NextIncrementalDouble();
            WaterKinematicViscosity = NextIncrementalDouble();
            Gravity = NextIncrementalDouble();
            ExitPointXCoordinate = NextIncrementalDouble();
            BeddingAngle = NextIncrementalDouble();
            MeanDiameter70 = NextIncrementalDouble();
            ThicknessAquiferLayer = NextIncrementalDouble();
            SurfaceLine = CreateValidSurfaceLine();
            SoilProfile = CreateValidSoilProfile();
        }

        private PipingSoilProfile CreateValidSoilProfile()
        {
            return new PipingSoilProfile(String.Empty, -2, new []
            {
                new PipingSoilLayer(9)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2), 
                new PipingSoilLayer(-1), 
            });
        }

        private RingtoetsPipingSurfaceLine CreateValidSurfaceLine()
        {
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            ringtoetsPipingSurfaceLine.SetGeometry(new[]
            {
                new Point3D (0, 0, 2),
                new Point3D (1, 0, 8),
                new Point3D (2, 0, -1)
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

        /// <summary>
        /// Returns the current set value as a <see cref="PipingCalculatorInput"/>
        /// </summary>
        public PipingCalculatorInput AsRealInput()
        {
            return new PipingCalculatorInput(
                WaterVolumetricWeight,
                UpliftModelFactor,
                AssessmentLevel,
                PiezometricHeadExit,
                DampingFactorExit,
                PhreaticLevelExit,
                PiezometricHeadPolder,
                CriticalHeaveGradient,
                ThicknessCoverageLayer,
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
    }
}
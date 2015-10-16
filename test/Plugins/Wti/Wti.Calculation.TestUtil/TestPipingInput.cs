using System;
using Wti.Calculation.Piping;

namespace Wti.Calculation.Test.Piping.Stub
{
    public class TestPipingInput
    {
        public double WaterVolumetricWeight;
        public double UpliftModelFactor;
        public double AssessmentLevel;
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

        private readonly Random random = new Random(22);
        private double last;

        public TestPipingInput()
        {
            WaterVolumetricWeight = NextIncrementalDouble();
            UpliftModelFactor = NextIncrementalDouble();
            AssessmentLevel = NextIncrementalDouble();
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
        }

        /// <summary>
        /// The returned double is sure to be different from the last time it was called.
        /// </summary>
        /// <returns></returns>
        private double NextIncrementalDouble()
        {
            return last += random.NextDouble() + 1e-6;
        }

        /// <summary>
        /// Returns the current set value as a <see cref="PipingCalculationInput"/>
        /// </summary>
        /// <returns></returns>
        public PipingCalculationInput AsRealInput()
        {
            return new PipingCalculationInput(
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
                ExitPointXCoordinate
                );
        }
    }
}
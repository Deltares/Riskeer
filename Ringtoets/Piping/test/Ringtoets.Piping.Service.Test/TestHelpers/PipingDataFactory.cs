using System;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service.Test.TestHelpers
{
    public static class PipingDataFactory
    {
        public static PipingData CreateCalculationWithInvalidData()
        {
            return new PipingData();
        }

        public static PipingData CreateCalculationWithValidInput()
        {
            var random = new Random(22);
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble()) 
            });
            return new PipingData
            {
                AssessmentLevel = 1.0,
                BeddingAngle = 1.0,
                CriticalHeaveGradient = 1.0,
                DampingFactorExit = 1.0,
                DarcyPermeability = 1.0,
                Diameter70 = 1.0,
                ExitPointXCoordinate = 1.0,
                Gravity = 1.0,
                MeanDiameter70 = 1.0,
                PiezometricHeadExit = 1.0,
                PiezometricHeadPolder = 1.0,
                PhreaticLevelExit = 2.0,
                SandParticlesVolumicWeight = 1.0,
                SeepageLength = 1.0,
                SellmeijerModelFactor = 1.0,
                SellmeijerReductionFactor = 1.0,
                ThicknessAquiferLayer = 1.0,
                ThicknessCoverageLayer = 1.0,
                UpliftModelFactor = 1.0,
                WaterKinematicViscosity = 1.0,
                WaterVolumetricWeight = 1.0,
                WhitesDragCoefficient = 1.0,
                SurfaceLine = new RingtoetsPipingSurfaceLine(),
                SoilProfile = soilProfile
            };
        }
    }
}
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
                {
                    IsAquifer = true
                }
            });
            return new PipingData
            {
                AssessmentLevel = 1.0,
                BeddingAngle = 1.0,
                DampingFactorExit =
                {
                    Mean = 1.0
                },
                DarcyPermeability =
                {
                    Mean = 1.0
                },
                Diameter70 =
                {
                    Mean = 1.0
                },
                ExitPointXCoordinate = 1.0,
                Gravity = 1.0,
                MeanDiameter70 = 1.0,
                PiezometricHeadExit = 1.0,
                PiezometricHeadPolder = 1.0,
                PhreaticLevelExit =
                {
                    Mean = 2.0
                },
                SandParticlesVolumicWeight = 1.0,
                SeepageLength =
                {
                    Mean = 1.0
                },
                SellmeijerModelFactor = 1.0,
                SellmeijerReductionFactor = 1.0,
                ThicknessAquiferLayer =
                {
                    Mean = 1.0
                },
                ThicknessCoverageLayer =
                {
                    Mean = 1.0
                },
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
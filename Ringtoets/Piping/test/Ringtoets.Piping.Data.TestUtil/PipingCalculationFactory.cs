﻿using System;

namespace Ringtoets.Piping.Data.TestUtil
{
    public static class PipingCalculationFactory
    {
        public static PipingCalculation CreateCalculationWithInvalidData()
        {
            return new PipingCalculation();
        }

        public static PipingCalculation CreateCalculationWithValidInput()
        {
            var random = new Random(22);
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            return new PipingCalculation
            {
                InputParameters =
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
                    ExitPointL = 1.0,
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
                }
            };
        }
    }
}
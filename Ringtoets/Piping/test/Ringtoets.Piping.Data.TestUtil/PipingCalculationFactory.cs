using System;
using Core.Common.Base.Geometry;

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
            var bottom = random.NextDouble();
            var top = bottom + random.NextDouble();
            var soilProfile = new PipingSoilProfile(String.Empty, bottom, new[]
            {
                new PipingSoilLayer(top)
                {
                    IsAquifer = true
                }
            });
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new []
            {
                new Point3D
                {
                    X = 0.0,
                    Y = 0.0,
                    Z = 0.0
                }, 
                new Point3D
                {
                    X = 1.0,
                    Y = 0.0,
                    Z = top 
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
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile
                }
            };
        }
    }
}
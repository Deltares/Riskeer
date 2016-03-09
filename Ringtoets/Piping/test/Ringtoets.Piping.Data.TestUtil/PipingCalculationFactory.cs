using System;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Ringtoets.Piping.Data.TestUtil
{
    public static class PipingCalculationFactory
    {
        public static PipingCalculation CreateCalculationWithInvalidData()
        {
            return new PipingCalculation(new GeneralPipingInput());
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
                new Point3D(0.0, 0.0, 0.0), 
                new Point3D(1.0, 0.0, top)
            });
            return new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    AssessmentLevel = new RoundedDouble(2, 1.0),
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
                    PiezometricHeadExit = 1.0,
                    PiezometricHeadPolder = 1.0,
                    PhreaticLevelExit =
                    {
                        Mean = 2.0
                    },
                    SeepageLength =
                    {
                        Mean = 1.0
                    },
                    ThicknessAquiferLayer =
                    {
                        Mean = 1.0
                    },
                    ThicknessCoverageLayer =
                    {
                        Mean = 1.0
                    },
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile
                }
            };
        }
    }
}
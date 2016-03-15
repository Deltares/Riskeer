using System;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.HydraRing.Data;

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
            var firstCharacteristicPointLocation = new Point3D(0.2, 0.0, top / 6);
            var secondCharacteristicPointLocation = new Point3D(0.3, 0.0, 2 * top / 6);
            var thirdCharacteristicPointLocation = new Point3D(0.4, 0.0, 3 * top / 6);
            var fourthCharacteristicPointLocation = new Point3D(0.5, 0.0, 4 * top / 6);
            var fifthCharacteristicPointLocation = new Point3D(0.6, 0.0, 5 * top / 6);
            surfaceLine.SetGeometry(new []
            {
                new Point3D(0.0, 0.0, 0.0), 
                firstCharacteristicPointLocation, 
                secondCharacteristicPointLocation, 
                thirdCharacteristicPointLocation, 
                fourthCharacteristicPointLocation, 
                fifthCharacteristicPointLocation, 
                new Point3D(1.0, 0.0, top)
            });
            surfaceLine.SetDikeToeAtPolderAt(firstCharacteristicPointLocation);
            surfaceLine.SetDitchDikeSideAt(secondCharacteristicPointLocation);
            surfaceLine.SetBottomDitchDikeSideAt(thirdCharacteristicPointLocation);
            surfaceLine.SetBottomDitchPolderSideAt(fourthCharacteristicPointLocation);
            surfaceLine.SetDitchPolderSideAt(fifthCharacteristicPointLocation);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0)
            {
                DesignWaterLevel = (RoundedDouble) 1.0
            };
            return new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble)1.0
                    },
                    DarcyPermeability =
                    {
                        Mean = (RoundedDouble)1.0
                    },
                    Diameter70 =
                    {
                        Mean = (RoundedDouble)1.0
                    },
                    ExitPointL = (RoundedDouble)1.0,
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble)2.0
                    },
                    SeepageLength =
                    {
                        Mean = (RoundedDouble)1.0
                    },
                    ThicknessAquiferLayer =
                    {
                        Mean = (RoundedDouble)1.0
                    },
                    ThicknessCoverageLayer =
                    {
                        Mean = (RoundedDouble)1.0
                    },
                    PiezometricHeadExit = (RoundedDouble)2.1,
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
        }
    }
}
using System;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Deltares.WTIPiping;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.InputParameterCalculation.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.InputParameterCalculation.Test
{
    [TestFixture]
    public class InputParameterCalculationServiceTest
    {
        [Test]
        public static void CalculateThicknessCoverageLayer_InValidPipingCalculationWithOutput_ReturnsNaN()
        {
            // Setup
            PipingCalculation invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();

            // Make invalid by having surfaceline partially above soil profile:
            double highestLevelSurfaceLine = invalidPipingCalculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
            var soilProfileTop = highestLevelSurfaceLine - 0.5;
            var soilProfileBottom = soilProfileTop - 0.5;
            invalidPipingCalculation.InputParameters.SoilProfile = new PipingSoilProfile("A", soilProfileBottom, new[]
            {
                new PipingSoilLayer(soilProfileTop)
                {
                    IsAquifer = true
                }
            }, 0);

            // Call
            PipingInput input = invalidPipingCalculation.InputParameters;
            var result = InputParameterCalculationService.CalculateThicknessCoverageLayer(input.WaterVolumetricWeight, PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), input.ExitPointL, input.SurfaceLine, input.SoilProfile);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public static void CalculateThicknessCoverageLayer_ValidInput_ReturnsThickness()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 10),
                new Point3D(20, 0, 10)
            });

            var soilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(5)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(20)
                {
                    IsAquifer = false
                }
            }, 0);

            PipingInput input = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 10,
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            // Call
            var thickness = InputParameterCalculationService.CalculateThicknessCoverageLayer(input.WaterVolumetricWeight, PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), input.ExitPointL, input.SurfaceLine, input.SoilProfile);

            // Assert
            Assert.AreEqual(5, thickness);
        }

        [Test]
        public static void CalculatePiezometricHeadAtExit_Always_ReturnsResult()
        {
            // Setup
            PipingInput input = new PipingInput(new GeneralPipingInput())
            {
                AssessmentLevel = (RoundedDouble) 0.0
            };

            // Call
            var result = InputParameterCalculationService.CalculatePiezometricHeadAtExit(input.AssessmentLevel, PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue());

            // Assert
            Assert.IsFalse(Double.IsNaN(result));
        }

        [Test]
        public static void CalculateThicknessCoverageLayer_CompleteInput_InputSetOnSubCalculator()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new InputParameterCalculationServiceConfig())
            {
                // Call
                PipingInput input1 = validPipingCalculation.InputParameters;
                InputParameterCalculationService.CalculateThicknessCoverageLayer(input1.WaterVolumetricWeight, PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input1).GetDesignValue(), input1.ExitPointL, input1.SurfaceLine, input1.SoilProfile);

                // Assert
                var testFactory = (TestPipingSubCalculatorFactory)InputParameterCalculationService.SubCalculatorFactory;
                var effectiveThicknessCalculator = testFactory.LastCreatedEffectiveThicknessCalculator;

                Assert.AreEqual(input.ExitPointL.Value, effectiveThicknessCalculator.ExitPointXCoordinate);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), effectiveThicknessCalculator.PhreaticLevel,
                                Math.Pow(10.0, -input.PhreaticLevelExit.Mean.NumberOfDecimalPlaces));
                AssertEqualSoilProfiles(input.SoilProfile, effectiveThicknessCalculator.SoilProfile);
                AssertEqualSurfaceLines(input.SurfaceLine, effectiveThicknessCalculator.SurfaceLine);
                Assert.AreEqual(input.WaterVolumetricWeight, effectiveThicknessCalculator.VolumicWeightOfWater);
            }
        }

        [Test]
        public static void CalculatePiezometricHeadAtExit_CompleteInput_InputSetOnSubCalculator()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new InputParameterCalculationServiceConfig())
            {
                // Call
                PipingInput input1 = validPipingCalculation.InputParameters;
                InputParameterCalculationService.CalculatePiezometricHeadAtExit(input1.AssessmentLevel, PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input1).GetDesignValue(), PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input1).GetDesignValue());

                // Assert
                var testFactory = (TestPipingSubCalculatorFactory)InputParameterCalculationService.SubCalculatorFactory;
                var piezometricHeadAtExitCalculator = testFactory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(input.AssessmentLevel.Value, piezometricHeadAtExitCalculator.HRiver);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                GetAccuracy(input.PhreaticLevelExit.Mean));
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                GetAccuracy(input.DampingFactorExit.Mean));
            }
        }

        private static void AssertEqualSurfaceLines(RingtoetsPipingSurfaceLine pipingSurfaceLine, PipingSurfaceLine otherSurfaceLine)
        {
            AssertPointsAreEqual(pipingSurfaceLine.DitchDikeSide, otherSurfaceLine.DitchDikeSide);
            AssertPointsAreEqual(pipingSurfaceLine.BottomDitchDikeSide, otherSurfaceLine.BottomDitchDikeSide);
            AssertPointsAreEqual(pipingSurfaceLine.BottomDitchPolderSide, otherSurfaceLine.BottomDitchPolderSide);
            AssertPointsAreEqual(pipingSurfaceLine.DitchPolderSide, otherSurfaceLine.DitchPolderSide);
            AssertPointsAreEqual(pipingSurfaceLine.DikeToeAtPolder, otherSurfaceLine.DikeToeAtPolder);

            Assert.AreEqual(pipingSurfaceLine.Points.Length, otherSurfaceLine.Points.Count);
            for (int i = 0; i < pipingSurfaceLine.Points.Length; i++)
            {
                AssertPointsAreEqual(pipingSurfaceLine.Points[i], otherSurfaceLine.Points[i]);
            }
        }

        private static void AssertPointsAreEqual(Point3D point, PipingPoint otherPoint)
        {
            if (point == null)
            {
                Assert.IsNull(otherPoint);
                return;
            }
            if (otherPoint == null)
            {
                Assert.Fail("Expected value for otherPoint.");
            }
            Assert.AreEqual(point.X, otherPoint.X);
            Assert.AreEqual(point.Y, otherPoint.Y);
            Assert.AreEqual(point.Z, otherPoint.Z);
        }

        private static void AssertEqualSoilProfiles(PipingSoilProfile pipingProfile, PipingProfile otherPipingProfile)
        {
            Assert.AreEqual(pipingProfile.Bottom, otherPipingProfile.BottomLevel);
            Assert.AreEqual(pipingProfile.Layers.First().Top, otherPipingProfile.TopLevel);
            Assert.AreEqual(pipingProfile.Layers.Last(l => l.IsAquifer).Top, otherPipingProfile.BottomAquiferLayer.TopLevel);
            Assert.AreEqual(pipingProfile.Layers.First(l => l.IsAquifer).Top, otherPipingProfile.TopAquiferLayer.TopLevel);

            Assert.AreEqual(pipingProfile.Layers.Count(), otherPipingProfile.Layers.Count);
            for (int i = 0; i < pipingProfile.Layers.Count(); i++)
            {
                Assert.AreEqual(pipingProfile.Layers.ElementAt(i).Top, otherPipingProfile.Layers[i].TopLevel);
            }
        }

        private static double GetAccuracy(RoundedDouble roundedDouble)
        {
            return Math.Pow(10.0, -roundedDouble.NumberOfDecimalPlaces);
        }
    }
}
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

using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Deltares.WTIPiping;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.InputParameterCalculation.Test
{
    [TestFixture]
    public class InputParameterCalculationServiceTest
    {
        [Test]
        public static void CalculateEffectiveThicknessCoverageLayer_InvalidPipingCalculationWithOutput_ReturnsNaN()
        {
            // Setup
            PipingCalculation invalidPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Make invalid by having surfaceline partially above soil profile:
            double highestLevelSurfaceLine = invalidPipingCalculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
            var soilProfileTop = highestLevelSurfaceLine - 0.5;
            var soilProfileBottom = soilProfileTop - 0.5;
            invalidPipingCalculation.InputParameters.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile("A", soilProfileBottom, new[]
                {
                    new PipingSoilLayer(soilProfileTop)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            PipingInput input = invalidPipingCalculation.InputParameters;
            double result = InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(input.WaterVolumetricWeight,
                                                                                                      PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                                                                                                      input.ExitPointL,
                                                                                                      input.SurfaceLine,
                                                                                                      input.StochasticSoilProfile.SoilProfile);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public static void CalculateEffectiveThicknessCoverageLayer_WithMultipleCharacteristicTypesOnSamePoint_ReturnsThickness()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 10),
                new Point3D(20, 0, 10)
            });
            surfaceLine.SetDikeToeAtRiverAt(surfaceLine.Points[0]);
            surfaceLine.SetDikeToeAtPolderAt(surfaceLine.Points[0]);
            surfaceLine.SetDitchDikeSideAt(surfaceLine.Points[0]);
            surfaceLine.SetBottomDitchPolderSideAt(surfaceLine.Points[1]);
            surfaceLine.SetBottomDitchDikeSideAt(surfaceLine.Points[1]);
            surfaceLine.SetDitchPolderSideAt(surfaceLine.Points[1]);

            var stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(5)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(20)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            var input = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 10,
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile
            };

            // Call
            double thickness = InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(
                input.WaterVolumetricWeight,
                PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                input.ExitPointL,
                input.SurfaceLine,
                input.StochasticSoilProfile.SoilProfile);

            // Assert
            Assert.AreEqual(5, thickness);
        }

        [Test]
        public static void CalculateEffectiveThicknessCoverageLayer_ValidInput_ReturnsThickness()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 10),
                new Point3D(20, 0, 10)
            });

            var stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(5)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(20)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            var input = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 10,
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile
            };

            // Call
            double thickness = InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(
                input.WaterVolumetricWeight,
                PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                input.ExitPointL,
                input.SurfaceLine,
                input.StochasticSoilProfile.SoilProfile);

            // Assert
            Assert.AreEqual(5, thickness);
        }

        [Test]
        public static void CalculatePiezometricHeadAtExit_Always_ReturnsResult()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(0)
            };

            // Call
            double result = InputParameterCalculationService.CalculatePiezometricHeadAtExit(
                input.AssessmentLevel,
                PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(),
                PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue());

            // Assert
            Assert.IsFalse(double.IsNaN(result));
        }

        [Test]
        public static void CalculateEffectiveThicknessCoverageLayer_CompleteInput_InputSetOnSubCalculator()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingInput inputParameters = validPipingCalculation.InputParameters;
                InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(
                    inputParameters.WaterVolumetricWeight,
                    PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(inputParameters).GetDesignValue(),
                    inputParameters.ExitPointL,
                    inputParameters.SurfaceLine,
                    inputParameters.StochasticSoilProfile.SoilProfile);

                // Assert
                var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                EffectiveThicknessCalculatorStub effectiveThicknessCalculator = testFactory.LastCreatedEffectiveThicknessCalculator;

                Assert.AreEqual(input.ExitPointL.Value, effectiveThicknessCalculator.ExitPointXCoordinate);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                                effectiveThicknessCalculator.PhreaticLevel,
                                input.PhreaticLevelExit.GetAccuracy());
                AssertEqualSoilProfiles(input.StochasticSoilProfile.SoilProfile, effectiveThicknessCalculator.SoilProfile);
                AssertEqualSurfaceLines(input.SurfaceLine, effectiveThicknessCalculator.SurfaceLine);
                Assert.AreEqual(input.WaterVolumetricWeight, effectiveThicknessCalculator.VolumicWeightOfWater);
            }
        }

        [Test]
        public static void CalculatePiezometricHeadAtExit_CompleteInput_InputSetOnSubCalculator()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingInput input1 = validPipingCalculation.InputParameters;
                InputParameterCalculationService.CalculatePiezometricHeadAtExit(
                    input1.AssessmentLevel,
                    PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input1).GetDesignValue(),
                    PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input1).GetDesignValue());

                // Assert
                var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                PiezoHeadCalculatorStub piezometricHeadAtExitCalculator = testFactory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(input.AssessmentLevel.Value, piezometricHeadAtExitCalculator.HRiver);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                                piezometricHeadAtExitCalculator.PhiPolder,
                                input.PhreaticLevelExit.GetAccuracy());
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(),
                                piezometricHeadAtExitCalculator.RExit,
                                input.DampingFactorExit.GetAccuracy());
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
            Assert.AreEqual(point.X, otherPoint.X, 1e-2);
            Assert.AreEqual(point.Y, otherPoint.Y, 1e-2);
            Assert.AreEqual(point.Z, otherPoint.Z, 1e-2);
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
    }
}
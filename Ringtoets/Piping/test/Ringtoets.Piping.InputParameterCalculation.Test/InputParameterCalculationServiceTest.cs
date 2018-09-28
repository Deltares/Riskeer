// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;
using WTIPipingSurfaceLine = Deltares.WTIPiping.PipingSurfaceLine;
using WTIPipingPoint = Deltares.WTIPiping.PipingPoint;
using WTIPipingProfile = Deltares.WTIPiping.PipingProfile;

namespace Ringtoets.Piping.InputParameterCalculation.Test
{
    [TestFixture]
    public class InputParameterCalculationServiceTest
    {
        [Test]
        public static void CalculateEffectiveThicknessCoverageLayer_InvalidPipingCalculationWithOutput_ReturnsNaN()
        {
            // Setup
            PipingCalculation invalidPipingCalculation = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            // Make invalid by having surface line partially above soil profile:
            double highestLevelSurfaceLine = invalidPipingCalculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
            double soilProfileTop = highestLevelSurfaceLine - 0.5;
            double soilProfileBottom = soilProfileTop - 0.5;
            invalidPipingCalculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0,
                new PipingSoilProfile("A", soilProfileBottom, new[]
                {
                    new PipingSoilLayer(soilProfileTop)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            PipingInput input = invalidPipingCalculation.InputParameters;
            double result = InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(
                input.WaterVolumetricWeight,
                PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(),
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
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 10),
                new Point3D(20, 0, 10)
            });
            surfaceLine.SetDikeToeAtRiverAt(surfaceLine.Points.ElementAt(0));
            surfaceLine.SetDikeToeAtPolderAt(surfaceLine.Points.ElementAt(0));
            surfaceLine.SetDitchDikeSideAt(surfaceLine.Points.ElementAt(0));
            surfaceLine.SetBottomDitchPolderSideAt(surfaceLine.Points.ElementAt(1));
            surfaceLine.SetBottomDitchDikeSideAt(surfaceLine.Points.ElementAt(1));
            surfaceLine.SetDitchPolderSideAt(surfaceLine.Points.ElementAt(1));

            var stochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(5)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(20)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D));

            var input = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 10,
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile
            };

            // Call
            double thickness = InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(
                input.WaterVolumetricWeight,
                PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(),
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
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 10),
                new Point3D(20, 0, 10)
            });

            var stochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(5)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(20)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D));

            var input = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 10,
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile
            };

            // Call
            double thickness = InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(
                input.WaterVolumetricWeight,
                PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(),
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
            var input = new PipingInput(new GeneralPipingInput());

            // Call
            double result = InputParameterCalculationService.CalculatePiezometricHeadAtExit(
                (RoundedDouble) 0.0,
                PipingSemiProbabilisticDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue(),
                PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue());

            // Assert
            Assert.IsFalse(double.IsNaN(result));
        }

        [Test]
        public static void CalculateEffectiveThicknessCoverageLayer_CompleteInput_InputSetOnSubCalculator()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingInput inputParameters = validPipingCalculation.InputParameters;
                InputParameterCalculationService.CalculateEffectiveThicknessCoverageLayer(
                    inputParameters.WaterVolumetricWeight,
                    PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(inputParameters).GetDesignValue(),
                    inputParameters.ExitPointL,
                    inputParameters.SurfaceLine,
                    inputParameters.StochasticSoilProfile.SoilProfile);

                // Assert
                var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                EffectiveThicknessCalculatorStub effectiveThicknessCalculator = testFactory.LastCreatedEffectiveThicknessCalculator;

                Assert.AreEqual(input.ExitPointL.Value, effectiveThicknessCalculator.ExitPointXCoordinate);
                Assert.AreEqual(PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(),
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
            RoundedDouble assessmentLevel = new Random(21).NextRoundedDouble();
            PipingCalculation validPipingCalculation = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                InputParameterCalculationService.CalculatePiezometricHeadAtExit(
                    assessmentLevel,
                    PipingSemiProbabilisticDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue(),
                    PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue());

                // Assert
                var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                PiezoHeadCalculatorStub piezometricHeadAtExitCalculator = testFactory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(assessmentLevel.Value, piezometricHeadAtExitCalculator.HRiver);
                Assert.AreEqual(PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                                piezometricHeadAtExitCalculator.PhiPolder,
                                input.PhreaticLevelExit.GetAccuracy());
                Assert.AreEqual(PipingSemiProbabilisticDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue(),
                                piezometricHeadAtExitCalculator.RExit,
                                input.DampingFactorExit.GetAccuracy());
            }
        }

        private static void AssertEqualSurfaceLines(PipingSurfaceLine pipingSurfaceLine, WTIPipingSurfaceLine otherSurfaceLine)
        {
            AssertPointsAreEqual(pipingSurfaceLine.DitchDikeSide, otherSurfaceLine.DitchDikeSide);
            AssertPointsAreEqual(pipingSurfaceLine.BottomDitchDikeSide, otherSurfaceLine.BottomDitchDikeSide);
            AssertPointsAreEqual(pipingSurfaceLine.BottomDitchPolderSide, otherSurfaceLine.BottomDitchPolderSide);
            AssertPointsAreEqual(pipingSurfaceLine.DitchPolderSide, otherSurfaceLine.DitchPolderSide);
            AssertPointsAreEqual(pipingSurfaceLine.DikeToeAtPolder, otherSurfaceLine.DikeToeAtPolder);

            Assert.AreEqual(pipingSurfaceLine.Points.Count(), otherSurfaceLine.Points.Count);
            for (var i = 0; i < pipingSurfaceLine.Points.Count(); i++)
            {
                AssertPointsAreEqual(pipingSurfaceLine.Points.ElementAt(i), otherSurfaceLine.Points[i]);
            }
        }

        private static void AssertPointsAreEqual(Point3D point, WTIPipingPoint otherPoint)
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

        private static void AssertEqualSoilProfiles(PipingSoilProfile pipingProfile, WTIPipingProfile otherPipingProfile)
        {
            Assert.AreEqual(pipingProfile.Bottom, otherPipingProfile.BottomLevel);
            Assert.AreEqual(pipingProfile.Layers.First().Top, otherPipingProfile.TopLevel);
            Assert.AreEqual(pipingProfile.Layers.Last(l => l.IsAquifer).Top, otherPipingProfile.BottomAquiferLayer.TopLevel);
            Assert.AreEqual(pipingProfile.Layers.First(l => l.IsAquifer).Top, otherPipingProfile.TopAquiferLayer.TopLevel);

            Assert.AreEqual(pipingProfile.Layers.Count(), otherPipingProfile.Layers.Count);
            for (var i = 0; i < pipingProfile.Layers.Count(); i++)
            {
                Assert.AreEqual(pipingProfile.Layers.ElementAt(i).Top, otherPipingProfile.Layers[i].TopLevel);
            }
        }
    }
}
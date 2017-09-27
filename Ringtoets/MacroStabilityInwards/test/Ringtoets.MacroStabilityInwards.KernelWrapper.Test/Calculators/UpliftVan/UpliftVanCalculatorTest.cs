// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.Primitives;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan
{
    [TestFixture]
    public class UpliftVanCalculatorTest
    {
        [Test]
        public void Constructor_InputNull_ArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new UpliftVanCalculator(null, factory);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ArgumentNullException()
        {
            // Call
            UpliftVanCalculatorInput input = CreateValidCalculatorInput();
            TestDelegate call = () => new UpliftVanCalculator(input, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            UpliftVanCalculatorInput input = CreateValidCalculatorInput();

            // Call
            var calculator = new UpliftVanCalculator(input, factory);

            // Assert
            Assert.IsInstanceOf<IUpliftVanCalculator>(calculator);
        }

        [Test]
        public void Calculate_CalculatorWithValidInputAndKernelWithValidOutput_KernelCalculateMethodCalled()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateValidCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            UpliftVanKernelStub upliftVanKernel = testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel;
            SetValidKernelOutput(upliftVanKernel);

            // Call
            new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            Assert.IsTrue(testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel.Calculated);
        }

        [Test]
        public void Calculate_CalculatorWithCompleteInput_InputCorrectlySetToKernel()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            UpliftVanKernelStub upliftVanKernel = testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel;
            SetValidKernelOutput(upliftVanKernel);

            // Call
            new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            Assert.AreEqual(input.MoveGrid, upliftVanKernel.MoveGrid);
            Assert.AreEqual(input.MaximumSliceWidth, upliftVanKernel.MaximumSliceWidth);

            Soil[] soils = SoilCreator.Create(input.UpliftVanSoilProfile);
            Dictionary<UpliftVanSoilLayer, Soil> layersWithSoils =
                input.UpliftVanSoilProfile.Layers
                     .Zip(soils, (layer, soil) => new
                     {
                         layer,
                         soil
                     })
                     .ToDictionary(x => x.layer, x => x.soil);

            UpliftVanKernelInputHelper.AssertSoilModels(SoilModelCreator.Create(soils), upliftVanKernel.SoilModel);
            UpliftVanKernelInputHelper.AssertSoilProfiles(SoilProfileCreator.Create(input.UpliftVanSoilProfile, layersWithSoils), upliftVanKernel.SoilProfile);
            UpliftVanKernelInputHelper.AssertStabilityLocations(StabilityLocationCreator.Create(input), upliftVanKernel.Location);

            Assert.AreEqual(input.GridAutomaticDetermined, upliftVanKernel.GridAutomaticDetermined);
            Assert.AreEqual(input.CreateZones, upliftVanKernel.CreateZones);
            Assert.AreEqual(input.AutomaticForbiddenZones, upliftVanKernel.AutomaticForbiddenZones);
            Assert.AreEqual(input.SlipPlaneMinimumDepth, upliftVanKernel.SlipPlaneMinimumDepth);
            Assert.AreEqual(input.SlipPlaneMinimumLength, upliftVanKernel.SlipPlaneMinimumLength);
        }

        [Test]
        public void Calculate_KernelWithCompleteOutput_OutputCorrectlySetToCalculator()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateValidCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            UpliftVanKernelStub upliftVanKernel = testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel;
            SetCompleteKernelOutput(upliftVanKernel);

            // Call
            UpliftVanCalculatorResult result = new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(upliftVanKernel.FactorOfStability, result.FactorOfStability);
            Assert.AreEqual(upliftVanKernel.ZValue, result.ZValue);
            Assert.AreEqual(upliftVanKernel.ForbiddenZonesXEntryMax, result.ForbiddenZonesXEntryMax);
            Assert.AreEqual(upliftVanKernel.ForbiddenZonesXEntryMin, result.ForbiddenZonesXEntryMin);
            UpliftVanCalculatorOutputHelper.AssertSlidingCurve(UpliftVanSlidingCurveResultCreator.Create(upliftVanKernel.SlidingCurveResult),
                                                               result.SlidingCurveResult);
            UpliftVanCalculatorOutputHelper.AssertSlipPlaneGrid(UpliftVanCalculationGridResultCreator.Create(upliftVanKernel.SlipPlaneResult),
                                                                result.CalculationGridResult);
        }

        [Test]
        public void Validate_Always_ReturnEmptyList()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateValidCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            // Call
            List<string> validationResult = new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Validate();

            // Assert
            CollectionAssert.IsEmpty(validationResult);
        }

        private static UpliftVanCalculatorInput CreateValidCalculatorInput()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();
            return new UpliftVanCalculatorInput(new UpliftVanCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = random.NextDouble(),
                SurfaceLine = surfaceLine,
                UpliftVanSoilProfile = CreateValidSoilProfile(surfaceLine),
                LeftGrid = new MacroStabilityInwardsGrid(),
                RightGrid = new MacroStabilityInwardsGrid()
            });
        }

        private static UpliftVanCalculatorInput CreateCompleteCalculatorInput()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();

            return new UpliftVanCalculatorInput(new UpliftVanCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = random.NextDouble(),
                SurfaceLine = surfaceLine,
                UpliftVanSoilProfile = CreateValidSoilProfile(surfaceLine),
                WaterLevelRiverAverage = random.Next(),
                WaterLevelPolder = random.Next(),
                XCoordinateDrainageConstruction = random.Next(),
                ZCoordinateDrainageConstruction = random.Next(),
                MinimumLevelPhreaticLineAtDikeTopRiver = random.Next(),
                MinimumLevelPhreaticLineAtDikeTopPolder = random.Next(),
                PhreaticLineOffsetBelowDikeTopAtRiver = random.Next(),
                PhreaticLineOffsetBelowDikeTopAtPolder = random.Next(),
                PhreaticLineOffsetBelowShoulderBaseInside = random.Next(),
                PhreaticLineOffsetBelowDikeToeAtPolder = random.Next(),
                LeakageLengthOutwardsPhreaticLine3 = random.Next(),
                LeakageLengthInwardsPhreaticLine3 = random.Next(),
                LeakageLengthOutwardsPhreaticLine4 = random.Next(),
                LeakageLengthInwardsPhreaticLine4 = random.Next(),
                PiezometricHeadPhreaticLine2Outwards = random.Next(),
                PiezometricHeadPhreaticLine2Inwards = random.Next(),
                PenetrationLength = random.Next(),
                UseDefaultOffsets = random.NextBoolean(),
                AdjustPhreaticLine3And4ForUplift = random.NextBoolean(),
                DrainageConstructionPresent = random.NextBoolean(),
                DikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>(),
                MoveGrid = random.NextBoolean(),
                MaximumSliceWidth = random.Next(),
                GridAutomaticDetermined = random.NextBoolean(),
                LeftGrid = new MacroStabilityInwardsGrid(),
                RightGrid = new MacroStabilityInwardsGrid(),
                TangentLineAutomaticAtBoundaries = random.NextBoolean(),
                TangentLineZTop = random.Next(),
                TangentLineZBottom = random.Next(),
                CreateZones = random.NextBoolean(),
                AutomaticForbiddenZones = random.NextBoolean(),
                SlipPlaneMinimumDepth = random.Next(),
                SlipPlaneMinimumLength = random.Next()
            });
        }

        private static UpliftVanSoilProfile CreateValidSoilProfile(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return new UpliftVanSoilProfile(new[]
            {
                new UpliftVanSoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties()),
                new UpliftVanSoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties
                    {
                        IsAquifer = true
                    }),
                new UpliftVanSoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties()),
                new UpliftVanSoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new UpliftVanSoilLayer.ConstructionProperties())
            }, Enumerable.Empty<UpliftVanPreconsolidationStress>());
        }

        private static MacroStabilityInwardsSurfaceLine CreateValidSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, 8),
                new Point3D(2, 0, -1)
            });
            return surfaceLine;
        }

        private static void SetValidKernelOutput(UpliftVanKernelStub upliftVanKernel)
        {
            upliftVanKernel.SlidingCurveResult = SlidingDualCircleTestFactory.Create();
            upliftVanKernel.SlipPlaneResult = SlipPlaneUpliftVanTestFactory.Create();
        }

        private static void SetCompleteKernelOutput(UpliftVanKernelStub upliftVanKernel)
        {
            var random = new Random(11);

            upliftVanKernel.FactorOfStability = random.NextDouble();
            upliftVanKernel.ZValue = random.NextDouble();
            upliftVanKernel.ForbiddenZonesXEntryMax = random.NextDouble();
            upliftVanKernel.ForbiddenZonesXEntryMin = random.NextDouble();
            upliftVanKernel.SlidingCurveResult = SlidingDualCircleTestFactory.Create();
            upliftVanKernel.SlipPlaneResult = SlipPlaneUpliftVanTestFactory.Create();
        }
    }
}
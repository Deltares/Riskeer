﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Deltares.WTIStability;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Primitives;
using Point2D = Core.Common.Base.Geometry.Point2D;
using SoilLayer = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;

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
            // Setup
            UpliftVanCalculatorInput input = CreateValidCalculatorInput();

            // Call
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
            mocks.VerifyAll();
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
            Assert.IsTrue(upliftVanKernel.Calculated);
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

            Soil[] soils = SoilCreator.Create(input.SoilProfile);
            Dictionary<SoilLayer, Soil> layersWithSoils =
                input.SoilProfile.Layers
                     .Zip(soils, (layer, soil) => new
                     {
                         layer,
                         soil
                     })
                     .ToDictionary(x => x.layer, x => x.soil);

            KernelInputAssert.AssertSoilModels(SoilModelCreator.Create(soils), upliftVanKernel.SoilModel);
            KernelInputAssert.AssertSoilProfiles(SoilProfileCreator.Create(input.SoilProfile, layersWithSoils), upliftVanKernel.SoilProfile);
            KernelInputAssert.AssertStabilityLocations(UpliftVanStabilityLocationCreator.CreateExtreme(input), upliftVanKernel.LocationExtreme);
            KernelInputAssert.AssertStabilityLocations(UpliftVanStabilityLocationCreator.CreateDaily(input), upliftVanKernel.LocationDaily);
            KernelInputAssert.AssertSurfaceLines(SurfaceLineCreator.Create(input.SurfaceLine, input.LandwardDirection), upliftVanKernel.SurfaceLine);
            UpliftVanKernelInputAssert.AssertSlipPlanesUpliftVan(SlipPlaneUpliftVanCreator.Create(input.SlipPlane), upliftVanKernel.SlipPlaneUpliftVan);

            Assert.AreEqual(input.SlipPlane.GridAutomaticDetermined, upliftVanKernel.GridAutomaticDetermined);
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
            UpliftVanCalculatorOutputAssert.AssertSlidingCurve(UpliftVanSlidingCurveResultCreator.Create(upliftVanKernel.SlidingCurveResult),
                                                               result.SlidingCurveResult);
            UpliftVanCalculatorOutputAssert.AssertSlipPlaneGrid(UpliftVanCalculationGridResultCreator.Create(upliftVanKernel.SlipPlaneResult),
                                                                result.CalculationGridResult);
        }

        [Test]
        public void Calculate_KernelThrowsUpliftVanKernelWrapperException_ThrowUpliftVanCalculatorException()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateValidCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            UpliftVanKernelStub upliftVanKernel = testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel;
            upliftVanKernel.ThrowExceptionOnCalculate = true;

            // Call
            TestDelegate test = () => new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanCalculatorException>(test);
            Assert.IsInstanceOf<UpliftVanKernelWrapperException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void Validate_CalculatorWithValidInput_KernelValidateMethodCalled()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateValidCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            // Call
            new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Validate();

            // Assert
            Assert.IsTrue(testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel.Validated);
        }

        [Test]
        public void Validate_CalculatorWithValidInput_ReturnEmptyEnumerable()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateValidCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            // Call
            IEnumerable<UpliftVanValidationResult> validationResult = new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Validate();

            // Assert
            CollectionAssert.IsEmpty(validationResult);
        }

        [Test]
        public void Validate_KernelReturnsValidationResults_ReturnsEnumerableWithOnlyErrorsAndWarnings()
        {
            // Setup
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();
            UpliftVanKernelStub upliftVanKernel = testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel;
            upliftVanKernel.ReturnValidationResults = true;

            // Call
            IEnumerable<UpliftVanValidationResult> results = new UpliftVanCalculator(CreateValidCalculatorInput(), testMacroStabilityInwardsKernelFactory).Validate();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Validation Warning", results.ElementAt(0).Message);
            Assert.AreEqual(UpliftVanValidationResultType.Warning, results.ElementAt(0).ResultType);
            Assert.AreEqual("Validation Error", results.ElementAt(1).Message);
            Assert.AreEqual(UpliftVanValidationResultType.Error, results.ElementAt(1).ResultType);
        }

        [Test]
        public void Validate_KernelThrowsUpliftVanKernelWrapperException_ThrowUpliftVanCalculatorException()
        {
            // Setup
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();
            UpliftVanKernelStub upliftVanKernel = testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel;
            upliftVanKernel.ThrowExceptionOnValidate = true;

            // Call
            TestDelegate test = () => new UpliftVanCalculator(CreateValidCalculatorInput(), testMacroStabilityInwardsKernelFactory).Validate();

            // Assert
            var exception = Assert.Throws<UpliftVanCalculatorException>(test);
            Assert.IsInstanceOf<UpliftVanKernelWrapperException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        private static UpliftVanCalculatorInput CreateValidCalculatorInput()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();
            return new UpliftVanCalculatorInput(new UpliftVanCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = random.NextDouble(),
                SurfaceLine = surfaceLine,
                SoilProfile = CreateValidSoilProfile(surfaceLine),
                DrainageConstruction = new DrainageConstruction(),
                PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                SlipPlane = new UpliftVanSlipPlane()
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
                SoilProfile = CreateValidSoilProfile(surfaceLine),
                DrainageConstruction = new DrainageConstruction(),
                PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                SlipPlane = new UpliftVanSlipPlane(),
                WaterLevelRiverAverage = random.NextDouble(),
                WaterLevelPolderExtreme = random.NextDouble(),
                WaterLevelPolderDaily = random.NextDouble(),
                MinimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble(),
                MinimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble(),
                LeakageLengthOutwardsPhreaticLine3 = random.NextDouble(),
                LeakageLengthInwardsPhreaticLine3 = random.NextDouble(),
                LeakageLengthOutwardsPhreaticLine4 = random.NextDouble(),
                LeakageLengthInwardsPhreaticLine4 = random.NextDouble(),
                PiezometricHeadPhreaticLine2Outwards = random.NextDouble(),
                PiezometricHeadPhreaticLine2Inwards = random.NextDouble(),
                PenetrationLengthExtreme = random.NextDouble(),
                PenetrationLengthDaily = random.NextDouble(),
                AdjustPhreaticLine3And4ForUplift = random.NextBoolean(),
                DikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>(),
                MoveGrid = random.NextBoolean(),
                MaximumSliceWidth = random.NextDouble(),
                CreateZones = random.NextBoolean(),
                AutomaticForbiddenZones = random.NextBoolean(),
                SlipPlaneMinimumDepth = random.NextDouble(),
                SlipPlaneMinimumLength = random.NextDouble()
            });
        }

        private static SoilProfile CreateValidSoilProfile(MechanismSurfaceLineBase surfaceLine)
        {
            return new SoilProfile(new[]
            {
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties()),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties
                    {
                        IsAquifer = true
                    }),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties()),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties())
            }, new[]
            {
                new PreconsolidationStress(new Point2D(0, 0), 1.1)
            });
        }

        private static MacroStabilityInwardsSurfaceLine CreateValidSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var dikeToeAtRiver = new Point3D(1, 0, 8);

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                dikeToeAtRiver,
                new Point3D(2, 0, -1)
            });

            surfaceLine.SetDikeToeAtRiverAt(dikeToeAtRiver);

            return surfaceLine;
        }

        private static void SetValidKernelOutput(UpliftVanKernelStub upliftVanKernel)
        {
            upliftVanKernel.SlidingCurveResult = CreateSlidingDualCircle();
            upliftVanKernel.SlipPlaneResult = CreateSlipPlaneUpliftVan();
        }

        private static void SetCompleteKernelOutput(UpliftVanKernelStub upliftVanKernel)
        {
            var random = new Random(11);

            upliftVanKernel.FactorOfStability = random.NextDouble();
            upliftVanKernel.ZValue = random.NextDouble();
            upliftVanKernel.ForbiddenZonesXEntryMax = random.NextDouble();
            upliftVanKernel.ForbiddenZonesXEntryMin = random.NextDouble();
            SetValidKernelOutput(upliftVanKernel);
        }

        private static SlipPlaneUpliftVan CreateSlipPlaneUpliftVan()
        {
            return new SlipPlaneUpliftVan
            {
                SlipPlaneLeftGrid = new SlipCircleGrid
                {
                    GridXLeft = 0.1,
                    GridXRight = 0.2,
                    GridZTop = 0.3,
                    GridZBottom = 0.4,
                    GridXNumber = 1,
                    GridZNumber = 2
                },
                SlipPlaneRightGrid = new SlipCircleGrid
                {
                    GridXLeft = 0.5,
                    GridXRight = 0.6,
                    GridZTop = 0.7,
                    GridZBottom = 0.8,
                    GridXNumber = 3,
                    GridZNumber = 4
                },
                SlipPlaneTangentLine = new SlipCircleTangentLine
                {
                    BoundaryHeights =
                    {
                        new TangentLine(1.1),
                        new TangentLine(2.2)
                    }
                }
            };
        }

        private static SlidingDualCircle CreateSlidingDualCircle()
        {
            return new SlidingDualCircle
            {
                LeftCircleIsActive = false,
                ActiveCircle = new GeometryPoint(0.1, 0.2),
                ActiveForce = 0.3,
                ActiveForce0 = 0.4,
                ActiveRadius = 0.5,
                DrivingMomentActive = 0.6,
                ResistingMomentActive = 0.7,
                PassiveCircle = new GeometryPoint(0.8, 0.9),
                PassiveForce = 1.0,
                PassiveForce0 = 1.1,
                PassiveRadius = 1.2,
                DrivingMomentPassive = 1.3,
                ResistingMomentPassive = 1.4,
                HorizontalForce = 1.5,
                HorizontalForce0 = 1.6,
                Slices =
                {
                    new Slice
                    {
                        TopLeftX = 1.7,
                        TopLeftZ = 1.8,
                        TopRightX = 1.9,
                        TopRightZ = 2.0,
                        BottomLeftX = 2.1,
                        BottomLeftZ = 2.2,
                        BottomRightX = 2.3,
                        BottomRightZ = 2.4,
                        Cohesion = 2.5,
                        Phi = 2.6,
                        PGrens = 2.7,
                        OCR = 2.8,
                        POP = 2.9,
                        DegreeofConsolidationPorePressure = 3.0,
                        PorePressureDueToDegreeOfConsolidationLoad = 3.1,
                        Dilatancy = 3.2,
                        ExternalLoad = 3.3,
                        HydrostaticPorePressure = 3.4,
                        LeftForce = 3.5,
                        LeftForceAngle = 3.6,
                        LeftForceY = 3.7,
                        RightForce = 3.8,
                        RightForceAngle = 3.9,
                        RightForceY = 4.0,
                        LoadStress = 4.1,
                        NormalStress = 4.2,
                        PoreOnSurface = 4.3,
                        HPoreOnSurface = 4.4,
                        VPoreOnSurface = 4.5,
                        PiezometricPorePressure = 4.6,
                        EffectiveStress = 4.7,
                        EffectiveStressDaily = 4.8,
                        ExcessPorePressure = 4.9,
                        ShearStress = 5.0,
                        SoilStress = 5.1,
                        TotalPorePressure = 5.2,
                        TotalStress = 5.3,
                        Weight = 5.4
                    }
                }
            };
        }
    }
}
// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Standard;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.Primitives;
using Point2D = Core.Common.Base.Geometry.Point2D;
using SoilLayer = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan
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
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

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

            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

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
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                SetValidKernelOutput(upliftVanKernel);

                // Call
                new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                Assert.IsTrue(upliftVanKernel.Calculated);
            }
        }

        [Test]
        public void Calculate_CalculatorWithCompleteInput_InputCorrectlySetToKernel()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                SetValidKernelOutput(upliftVanKernel);

                // Call
                new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                Assert.AreEqual(input.MoveGrid, upliftVanKernel.MoveGrid);
                Assert.AreEqual(input.MaximumSliceWidth, upliftVanKernel.MaximumSliceWidth);

                LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile);

                KernelInputAssert.AssertSoilModels(SoilModelCreator.Create(layersWithSoil.Select(lws => lws.Soil).ToArray()), upliftVanKernel.SoilModel);
                KernelInputAssert.AssertSoilProfiles(SoilProfileCreator.Create(input.SoilProfile.PreconsolidationStresses, layersWithSoil), upliftVanKernel.SoilProfile);
                KernelInputAssert.AssertSurfaceLines(SurfaceLineCreator.Create(input.SurfaceLine), upliftVanKernel.SurfaceLine);
                UpliftVanKernelInputAssert.AssertSlipPlanesUpliftVan(SlipPlaneUpliftVanCreator.Create(input.SlipPlane), upliftVanKernel.SlipPlaneUpliftVan);
                UpliftVanKernelInputAssert.AssertSlipPlaneConstraints(SlipPlaneConstraintsCreator.Create(input.SlipPlaneConstraints), upliftVanKernel.SlipPlaneConstraints);
                Assert.AreEqual(input.SlipPlane.GridAutomaticDetermined, upliftVanKernel.GridAutomaticDetermined);
                CollectionAssert.IsEmpty(upliftVanKernel.CalculationMessages);
            }
        }

        [Test]
        public void Calculate_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                SetCompleteKernelOutput(upliftVanKernel);

                // Call
                UpliftVanCalculatorResult result = new UpliftVanCalculator(input, factory).Calculate();

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
        }

        [Test]
        public void Calculate_KernelReturnsLogMessages_ReturnsExpectedLogMessages()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                upliftVanKernel.ReturnLogMessages = true;
                SetCompleteKernelOutput(upliftVanKernel);

                // Call
                new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                Assert.AreEqual(6, upliftVanKernel.CalculationMessages.Count());
                LogMessage firstMessage = upliftVanKernel.CalculationMessages.ElementAt(0);
                Assert.AreEqual("Calculation Trace", firstMessage.Message);
                Assert.AreEqual(LogMessageType.Trace, firstMessage.MessageType);

                LogMessage secondMessage = upliftVanKernel.CalculationMessages.ElementAt(1);
                Assert.AreEqual("Calculation Debug", secondMessage.Message);
                Assert.AreEqual(LogMessageType.Debug, secondMessage.MessageType);

                LogMessage thirdMessage = upliftVanKernel.CalculationMessages.ElementAt(2);
                Assert.AreEqual("Calculation Info", thirdMessage.Message);
                Assert.AreEqual(LogMessageType.Info, thirdMessage.MessageType);

                LogMessage fourthMessage = upliftVanKernel.CalculationMessages.ElementAt(3);
                Assert.AreEqual("Calculation Warning", fourthMessage.Message);
                Assert.AreEqual(LogMessageType.Warning, fourthMessage.MessageType);

                LogMessage fifthMessage = upliftVanKernel.CalculationMessages.ElementAt(4);
                Assert.AreEqual("Calculation Error", fifthMessage.Message);
                Assert.AreEqual(LogMessageType.Error, fifthMessage.MessageType);

                LogMessage sixthMessage = upliftVanKernel.CalculationMessages.ElementAt(5);
                Assert.AreEqual("Calculation Fatal Error", sixthMessage.Message);
                Assert.AreEqual(LogMessageType.FatalError, sixthMessage.MessageType);
            }
        }

        [Test]
        public void Calculate_KernelThrowsUpliftVanKernelWrapperException_ThrowUpliftVanCalculatorException()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                upliftVanKernel.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate test = () => new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                var exception = Assert.Throws<UpliftVanCalculatorException>(test);
                Assert.IsInstanceOf<UpliftVanKernelWrapperException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void Validate_CalculatorWithValidInput_KernelValidateMethodCalled()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;

                // Call
                new UpliftVanCalculator(input, factory).Validate();

                // Assert
                Assert.IsTrue(factory.LastCreatedUpliftVanKernel.Validated);
            }
        }

        [Test]
        public void Validate_CalculatorWithValidInput_ReturnEmptyEnumerable()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;

                // Call
                IEnumerable<UpliftVanKernelMessage> kernelMessages = new UpliftVanCalculator(input, factory).Validate();

                // Assert
                CollectionAssert.IsEmpty(kernelMessages);
            }
        }

        [Test]
        public void Validate_KernelReturnsValidationResults_ReturnsEnumerableWithOnlyErrorsAndWarnings()
        {
            // Setup
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                upliftVanKernel.ReturnValidationResults = true;

                // Call
                IEnumerable<UpliftVanKernelMessage> kernelMessages = new UpliftVanCalculator(CreateCompleteCalculatorInput(),
                                                                                             factory).Validate().ToList();

                // Assert
                Assert.AreEqual(2, kernelMessages.Count());
                UpliftVanKernelMessage firstMessage = kernelMessages.ElementAt(0);
                Assert.AreEqual("Validation Warning", firstMessage.Message);
                Assert.AreEqual(UpliftVanKernelMessageType.Warning, firstMessage.ResultType);
                UpliftVanKernelMessage secondMessage = kernelMessages.ElementAt(1);
                Assert.AreEqual("Validation Error", secondMessage.Message);
                Assert.AreEqual(UpliftVanKernelMessageType.Error, secondMessage.ResultType);
            }
        }

        [Test]
        public void Validate_KernelThrowsUpliftVanKernelWrapperException_ThrowUpliftVanCalculatorException()
        {
            // Setup
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                upliftVanKernel.ThrowExceptionOnValidate = true;

                // Call
                TestDelegate test = () => new UpliftVanCalculator(CreateCompleteCalculatorInput(), factory).Validate();

                // Assert
                var exception = Assert.Throws<UpliftVanCalculatorException>(test);
                Assert.IsInstanceOf<UpliftVanKernelWrapperException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        private static UpliftVanCalculatorInput CreateCompleteCalculatorInput()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();

            return new UpliftVanCalculatorInput(new UpliftVanCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = -1,
                SurfaceLine = surfaceLine,
                SoilProfile = CreateValidSoilProfile(),
                DrainageConstruction = new DrainageConstruction(),
                PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                SlipPlane = new UpliftVanSlipPlane(),
                SlipPlaneConstraints = new UpliftVanSlipPlaneConstraints(random.NextDouble(), random.NextDouble(), random.NextBoolean()),
                WaterLevelRiverAverage = -1,
                WaterLevelPolderExtreme = -1,
                WaterLevelPolderDaily = -1,
                MinimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble(),
                MinimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble(),
                LeakageLengthOutwardsPhreaticLine3 = random.NextDouble() + 1,
                LeakageLengthInwardsPhreaticLine3 = random.NextDouble() + 1,
                LeakageLengthOutwardsPhreaticLine4 = random.NextDouble() + 1,
                LeakageLengthInwardsPhreaticLine4 = random.NextDouble() + 1,
                PiezometricHeadPhreaticLine2Outwards = 0.2,
                PiezometricHeadPhreaticLine2Inwards = 0.1,
                PenetrationLengthExtreme = random.NextDouble(),
                PenetrationLengthDaily = random.NextDouble(),
                AdjustPhreaticLine3And4ForUplift = random.NextBoolean(),
                DikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>(),
                MoveGrid = random.NextBoolean(),
                MaximumSliceWidth = random.NextDouble(),
                
            });
        }

        private static SoilProfile CreateValidSoilProfile()
        {
            return new SoilProfile(new[]
            {
                new SoilLayer(
                    new[]
                    {
                        new Point2D(-5, 0),
                        new Point2D(0, 5),
                        new Point2D(5, 0)
                    },
                    new SoilLayer.ConstructionProperties(),
                    Enumerable.Empty<SoilLayer>()),
                new SoilLayer(
                    new[]
                    {
                        new Point2D(-10, 0),
                        new Point2D(10, 0),
                        new Point2D(10, -5),
                        new Point2D(-10, -5)
                    },
                    new SoilLayer.ConstructionProperties(),
                    Enumerable.Empty<SoilLayer>()),
                new SoilLayer(
                    new[]
                    {
                        new Point2D(-10, -5),
                        new Point2D(10, -5),
                        new Point2D(10, -10),
                        new Point2D(-10, -10)
                    },
                    new SoilLayer.ConstructionProperties
                    {
                        IsAquifer = true
                    },
                    Enumerable.Empty<SoilLayer>())
            }, new[]
            {
                new PreconsolidationStress(new Point2D(0, 0), 1.1)
            });
        }

        private static MacroStabilityInwardsSurfaceLine CreateValidSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(-10, 0, 0),
                new Point3D(-5, 0, 0),
                new Point3D(0, 0, 5), 
                new Point3D(5, 0, 0),
                new Point3D(10, 0, 0)
            });

            surfaceLine.SetSurfaceLevelOutsideAt(new Point3D(-10, 0, 0));
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(-5, 0, 0));
            surfaceLine.SetDikeTopAtRiverAt(new Point3D(0, 0, 5));
            surfaceLine.SetDikeTopAtPolderAt(new Point3D(0, 0, 5));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(5, 0, 0));
            surfaceLine.SetSurfaceLevelInsideAt(new Point3D(10, 0, 0));

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
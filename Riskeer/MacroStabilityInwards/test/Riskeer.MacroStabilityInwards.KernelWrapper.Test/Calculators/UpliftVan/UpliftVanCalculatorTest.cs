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
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.CSharpWrapper.Output;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
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
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet.Input;
using Riskeer.MacroStabilityInwards.Primitives;
using CSharpWrapperPoint2D = Deltares.MacroStability.CSharpWrapper.Point2D;
using CSharpWrapperSoilProfile = Deltares.MacroStability.CSharpWrapper.Input.SoilProfile;
using Point2D = Core.Common.Base.Geometry.Point2D;
using PreconsolidationStress = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.PreconsolidationStress;
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
            void Call() => new UpliftVanCalculator(null, factory);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ArgumentNullException()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

            // Call
            void Call() => new UpliftVanCalculator(input, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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

                LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup);
                SurfaceLine surfaceLine = SurfaceLineCreator.Create(input.SurfaceLine);
                CSharpWrapperSoilProfile soilProfile = SoilProfileCreator.Create(layersWithSoil);

                MacroStabilityInput dailyWaternetInputForUpliftVan = MacroStabilityInputCreator.CreateDailyWaternetForUpliftVan(input, layersWithSoil, surfaceLine, soilProfile);
                MacroStabilityInput extremeWaternetInputForUpliftVan = MacroStabilityInputCreator.CreateExtremeWaternetForUpliftVan(input, layersWithSoil, surfaceLine, soilProfile);

                WaternetKernelStub waternetDailyKernel = (WaternetKernelStub) factory.CreateWaternetDailyKernel(dailyWaternetInputForUpliftVan);
                WaternetKernelStub waternetExtremeKernel = (WaternetKernelStub) factory.CreateWaternetExtremeKernel(extremeWaternetInputForUpliftVan);
                SetValidKernelOutput(upliftVanKernel);

                // Call
                new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                WaternetKernelInputAssert.AssertMacroStabilityInput(dailyWaternetInputForUpliftVan, waternetDailyKernel.KernelInput);
                WaternetKernelInputAssert.AssertMacroStabilityInput(extremeWaternetInputForUpliftVan, waternetExtremeKernel.KernelInput);
                UpliftVanKernelInputAssert.AssertMacroStabilityInput(
                    MacroStabilityInputCreator.CreateUpliftVan(
                        input, layersWithSoil, layerLookup, surfaceLine, soilProfile,
                        waternetDailyKernel.Waternet, waternetExtremeKernel.Waternet),
                    upliftVanKernel.KernelInput);
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
                Assert.AreEqual(upliftVanKernel.ForbiddenZonesXEntryMax, result.ForbiddenZonesXEntryMax);
                Assert.AreEqual(upliftVanKernel.ForbiddenZonesXEntryMin, result.ForbiddenZonesXEntryMin);
                UpliftVanCalculatorOutputAssert.AssertSlidingCurve(UpliftVanSlidingCurveResultCreator.Create(upliftVanKernel.SlidingCurveResult),
                                                                   result.SlidingCurveResult);
                UpliftVanCalculatorOutputAssert.AssertUpliftVanCalculationGridResult(UpliftVanCalculationGridResultCreator.Create(upliftVanKernel.UpliftVanCalculationGridResult),
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
                Assert.AreEqual(3, upliftVanKernel.CalculationMessages.Count());

                Message thirdMessage = upliftVanKernel.CalculationMessages.ElementAt(0);
                Assert.AreEqual("Calculation Info", thirdMessage.Content);
                Assert.AreEqual(MessageType.Info, thirdMessage.MessageType);

                Message fourthMessage = upliftVanKernel.CalculationMessages.ElementAt(1);
                Assert.AreEqual("Calculation Warning", fourthMessage.Content);
                Assert.AreEqual(MessageType.Warning, fourthMessage.MessageType);

                Message fifthMessage = upliftVanKernel.CalculationMessages.ElementAt(2);
                Assert.AreEqual("Calculation Error", fifthMessage.Content);
                Assert.AreEqual(MessageType.Error, fifthMessage.MessageType);
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
                void Call() => new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                var exception = Assert.Throws<UpliftVanCalculatorException>(Call);
                Assert.IsInstanceOf<UpliftVanKernelWrapperException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
                CollectionAssert.IsEmpty(exception.KernelMessages);
            }
        }

        [Test]
        public void Calculate_KernelThrowsUpliftVanKernelWrapperExceptionWithLogMessages_ThrowUpliftVanCalculatorException()
        {
            // Setup
            UpliftVanCalculatorInput input = CreateCompleteCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                upliftVanKernel.ThrowExceptionOnCalculate = true;
                upliftVanKernel.ReturnLogMessages = true;

                // Call
                void Call() => new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                var exception = Assert.Throws<UpliftVanCalculatorException>(Call);
                Assert.IsInstanceOf<UpliftVanKernelWrapperException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);

                IEnumerable<Message> expectedMessages = GetSupportMessages(upliftVanKernel.CalculationMessages);
                Assert.AreEqual(expectedMessages.Count(), exception.KernelMessages.Count());

                for (var i = 0; i < expectedMessages.Count(); i++)
                {
                    Message upliftVanKernelCalculationMessage = expectedMessages.ElementAt(i);
                    MacroStabilityInwardsKernelMessage exceptionKernelMessage = exception.KernelMessages.ElementAt(i);

                    Assert.AreEqual(upliftVanKernelCalculationMessage.Content, exceptionKernelMessage.Message);
                    Assert.AreEqual(GetMessageType(upliftVanKernelCalculationMessage.MessageType), exceptionKernelMessage.Type);
                }
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
                IEnumerable<MacroStabilityInwardsKernelMessage> kernelMessages = new UpliftVanCalculator(input, factory).Validate();

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
                IEnumerable<MacroStabilityInwardsKernelMessage> kernelMessages = new UpliftVanCalculator(CreateCompleteCalculatorInput(),
                                                                                                         factory).Validate().ToList();

                // Assert
                Assert.AreEqual(2, kernelMessages.Count());
                MacroStabilityInwardsKernelMessage firstMessage = kernelMessages.ElementAt(0);
                Assert.AreEqual("Validation Warning", firstMessage.Message);
                Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Warning, firstMessage.Type);
                MacroStabilityInwardsKernelMessage secondMessage = kernelMessages.ElementAt(1);
                Assert.AreEqual("Validation Error", secondMessage.Message);
                Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Error, secondMessage.Type);
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
                void Call() => new UpliftVanCalculator(CreateCompleteCalculatorInput(), factory).Validate();

                // Assert
                var exception = Assert.Throws<UpliftVanCalculatorException>(Call);
                Assert.IsInstanceOf<UpliftVanKernelWrapperException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        private static IEnumerable<Message> GetSupportMessages(IEnumerable<Message> messages)
        {
            return messages.Where(lm => lm.MessageType == MessageType.Error || lm.MessageType == MessageType.Warning);
        }

        private MacroStabilityInwardsKernelMessageType GetMessageType(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Error:
                    return MacroStabilityInwardsKernelMessageType.Error;
                case MessageType.Warning:
                    return MacroStabilityInwardsKernelMessageType.Warning;
                default:
                    throw new NotSupportedException();
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
                SlipPlaneConstraints = new UpliftVanSlipPlaneConstraints(random.NextDouble(), random.NextDouble()),
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
                MaximumSliceWidth = random.NextDouble()
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
                    new SoilLayer.ConstructionProperties
                    {
                        MaterialName = "Material 1",
                        UsePop = true,
                        Pop = new Random(21).NextDouble()
                    },
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
            upliftVanKernel.UpliftVanCalculationGridResult = CreateUpliftVanCalculationGrid();
        }

        private static void SetCompleteKernelOutput(UpliftVanKernelStub upliftVanKernel)
        {
            var random = new Random(11);

            upliftVanKernel.FactorOfStability = random.NextDouble();
            upliftVanKernel.ForbiddenZonesXEntryMax = random.NextDouble();
            upliftVanKernel.ForbiddenZonesXEntryMin = random.NextDouble();
            SetValidKernelOutput(upliftVanKernel);
        }

        private static UpliftVanCalculationGrid CreateUpliftVanCalculationGrid()
        {
            return new UpliftVanCalculationGrid
            {
                LeftGrid = new CalculationGrid
                {
                    GridXLeft = 0.1,
                    GridXRight = 0.2,
                    GridZTop = 0.3,
                    GridZBottom = 0.4,
                    GridXNumber = 1,
                    GridZNumber = 2
                },
                RightGrid = new CalculationGrid
                {
                    GridXLeft = 0.5,
                    GridXRight = 0.6,
                    GridZTop = 0.7,
                    GridZBottom = 0.8,
                    GridXNumber = 3,
                    GridZNumber = 4
                },
                TangentLines = new[]
                {
                    1.1,
                    2.2
                }
            };
        }

        private static DualSlidingCircleMinimumSafetyCurve CreateSlidingDualCircle()
        {
            return new DualSlidingCircleMinimumSafetyCurve
            {
                ActiveCircleCenter = new CSharpWrapperPoint2D(0.1, 0.2),
                IteratedActiveForce = 0.3,
                NonIteratedActiveForce = 0.4,
                ActiveCircleRadius = 0.5,
                DrivingActiveMoment = 0.6,
                ResistingActiveMoment = 0.7,
                PassiveCircleCenter = new CSharpWrapperPoint2D(0.8, 0.9),
                IteratedPassiveForce = 1.0,
                NonIteratedPassiveForce = 1.1,
                PassiveCircleRadius = 1.2,
                DrivingPassiveMoment = 1.3,
                ResistingPassiveMoment = 1.4,
                IteratedHorizontalForce = 1.5,
                NonIteratedHorizontalForce = 1.6,
                Slices = new[]
                {
                    new Slice
                    {
                        TopLeftPoint = new CSharpWrapperPoint2D(1.7, 1.8),
                        TopRightPoint = new CSharpWrapperPoint2D(1.9, 2.0),
                        BottomLeftPoint = new CSharpWrapperPoint2D(2.1, 2.2),
                        BottomRightPoint = new CSharpWrapperPoint2D(2.3, 2.4),
                        Cohesion = 2.5,
                        FrictionAngleInput = 2.6,
                        YieldStress = 2.7,
                        OCR = 2.8,
                        POP = 2.9,
                        DegreeOfConsolidationPorePressure = 3.0,
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
                        PorePressure = 4.3,
                        HorizontalPorePressure = 4.4,
                        VerticalPorePressure = 4.5,
                        PiezometricPorePressure = 4.6,
                        EffectiveStress = 4.7,
                        ExcessPorePressure = 4.8,
                        ShearStress = 4.9,
                        SoilStress = 5.0,
                        TotalPorePressure = 5.1,
                        TotalStress = 5.2,
                        Weight = 5.3
                    }
                }
            };
        }
    }
}
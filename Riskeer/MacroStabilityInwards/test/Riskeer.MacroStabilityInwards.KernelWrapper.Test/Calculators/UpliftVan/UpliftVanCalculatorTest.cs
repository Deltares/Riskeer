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
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet.Input;
using CSharpWrapperPoint2D = Deltares.MacroStability.CSharpWrapper.Point2D;
using CSharpWrapperSoilProfile = Deltares.MacroStability.CSharpWrapper.Input.SoilProfile;
using CSharpWrapperWaternet = Deltares.MacroStability.CSharpWrapper.Waternet;

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
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

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

            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

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
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                SetValidKernelOutput(upliftVanKernel);

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

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
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                WaternetKernelStub waternetDailyKernel = factory.LastCreatedWaternetDailyKernel;
                WaternetKernelStub waternetExtremeKernel = factory.LastCreatedWaternetExtremeKernel;

                SetValidKernelOutput(waternetDailyKernel);
                SetValidKernelOutput(waternetExtremeKernel);

                SetValidKernelOutput(upliftVanKernel);

                LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup);
                List<Soil> soils = layersWithSoil.Select(lws => lws.Soil).ToList();
                SurfaceLine surfaceLine = SurfaceLineCreator.Create(input.SurfaceLine);
                CSharpWrapperSoilProfile soilProfile = SoilProfileCreator.Create(layersWithSoil);

                // Call
                new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                WaternetKernelInputAssert.AssertMacroStabilityInput(
                    MacroStabilityInputCreator.CreateDailyWaternetForUpliftVan(input, soils, surfaceLine, soilProfile),
                    waternetDailyKernel.KernelInput);
                WaternetKernelInputAssert.AssertMacroStabilityInput(
                    MacroStabilityInputCreator.CreateExtremeWaternetForUpliftVan(input, soils, surfaceLine, soilProfile),
                    waternetExtremeKernel.KernelInput);
                UpliftVanKernelInputAssert.AssertMacroStabilityInput(
                    MacroStabilityInputCreator.CreateUpliftVan(
                        input, soils, layerLookup, surfaceLine, soilProfile,
                        waternetDailyKernel.Waternet, waternetExtremeKernel.Waternet),
                    upliftVanKernel.KernelInput);
            }
        }

        [Test]
        public void Calculate_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                SetCompleteKernelOutput(upliftVanKernel);

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

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
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                upliftVanKernel.ReturnLogMessages = true;
                SetCompleteKernelOutput(upliftVanKernel);

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

                // Call
                new UpliftVanCalculator(input, factory).Calculate();

                // Assert
                Assert.AreEqual(3, upliftVanKernel.CalculationMessages.Count());

                MessageHelper.AssertMessage(MessageType.Warning, "Calculation Warning", upliftVanKernel.CalculationMessages.ElementAt(0));
                MessageHelper.AssertMessage(MessageType.Error, "Calculation Error", upliftVanKernel.CalculationMessages.ElementAt(1));
                MessageHelper.AssertMessage(MessageType.Info, "Calculation Info", upliftVanKernel.CalculationMessages.ElementAt(2));
            }
        }

        [Test]
        public void Calculate_KernelThrowsUpliftVanKernelWrapperException_ThrowUpliftVanCalculatorException()
        {
            // Setup
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                upliftVanKernel.ThrowExceptionOnCalculate = true;

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

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
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                UpliftVanKernelStub upliftVanKernel = factory.LastCreatedUpliftVanKernel;
                upliftVanKernel.ThrowExceptionOnCalculate = true;
                upliftVanKernel.ReturnLogMessages = true;

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

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
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

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
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

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

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

                // Call
                IEnumerable<MacroStabilityInwardsKernelMessage> kernelMessages = new UpliftVanCalculator(
                    UpliftVanCalculatorInputTestFactory.Create(),
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

                SetValidKernelOutput(factory.LastCreatedWaternetDailyKernel);
                SetValidKernelOutput(factory.LastCreatedWaternetExtremeKernel);

                // Call
                void Call() => new UpliftVanCalculator(UpliftVanCalculatorInputTestFactory.Create(), factory).Validate();

                // Assert
                var exception = Assert.Throws<UpliftVanCalculatorException>(Call);
                Assert.IsInstanceOf<UpliftVanKernelWrapperException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        private static void SetValidKernelOutput(WaternetKernelStub waternetKernel)
        {
            waternetKernel.Waternet = new CSharpWrapperWaternet
            {
                HeadLines = new List<HeadLine>(),
                ReferenceLines = new List<ReferenceLine>(),
                PhreaticLine = new HeadLine
                {
                    Name = string.Empty
                }
            };
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
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;
using CSharpWrapperWaternet = Deltares.MacroStability.CSharpWrapper.Waternet;
using CSharpWrapperPoint2D = Deltares.MacroStability.CSharpWrapper.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.Waternet
{
    [TestFixture]
    public class WaternetCalculatorTest
    {
        [Test]
        public void Constructor_InputNull_ArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestWaternetCalculator(null, factory);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ArgumentNullException()
        {
            // Setup
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            // Call
            void Call() => new TestWaternetCalculator(input, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            // Call
            var calculator = new TestWaternetCalculator(input, factory);

            // Assert
            Assert.IsInstanceOf<IWaternetCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculatorWithValidInput_KernelCalculateMethodCalled()
        {
            // Setup
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                WaternetKernelStub waternetKernel = factory.LastCreatedWaternetExtremeKernel;
                SetCompleteKernelOutput(waternetKernel);

                // Call
                new TestWaternetCalculator(input, factory).Calculate();

                // Assert
                Assert.IsTrue(waternetKernel.Calculated);
            }
        }

        [Test]
        public void Calculate_KernelThrowsWaternetKernelWrapperException_ThrowWaternetCalculatorException()
        {
            // Setup
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                WaternetKernelStub waternetKernel = factory.LastCreatedWaternetExtremeKernel;
                waternetKernel.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => new TestWaternetCalculator(input, factory).Calculate();

                // Assert
                var exception = Assert.Throws<WaternetCalculatorException>(Call);
                Assert.IsInstanceOf<WaternetKernelWrapperException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void Calculate_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                WaternetKernelStub kernel = factory.LastCreatedWaternetExtremeKernel;
                SetCompleteKernelOutput(kernel);

                // Call
                WaternetCalculatorResult result = new TestWaternetCalculator(input, factory).Calculate();

                // Assert
                Assert.IsNotNull(result);
                var expectedPhreaticLines = new List<HeadLine>
                {
                    kernel.Waternet.PhreaticLine
                };
                expectedPhreaticLines.AddRange(kernel.Waternet.HeadLines);

                WaternetCalculatorOutputAssert.AssertPhreaticLines(expectedPhreaticLines.ToArray(), result.PhreaticLines.ToArray());
                WaternetCalculatorOutputAssert.AssertReferenceLines(kernel.Waternet.ReferenceLines.ToArray(), result.WaternetLines.ToArray());
            }
        }

        [Test]
        public void Validate_CalculatorWithValidInput_KernelValidateMethodCalled()
        {
            // Setup
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;

                WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();
                var calculator = new TestWaternetCalculator(input, factory);

                // Call
                calculator.Validate();

                // Assert
                Assert.IsTrue(factory.LastCreatedWaternetExtremeKernel.Validated);
            }
        }

        [Test]
        public void Validate_CalculatorWithValidInput_ReturnEmptyEnumerable()
        {
            // Setup
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;

                WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();
                var calculator = new TestWaternetCalculator(input, factory);

                // Call
                IEnumerable<MacroStabilityInwardsKernelMessage> kernelMessages = calculator.Validate();

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
                WaternetKernelStub waternetKernel = factory.LastCreatedWaternetExtremeKernel;
                waternetKernel.ReturnValidationResults = true;

                WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();
                var calculator = new TestWaternetCalculator(input, factory);

                // Call
                IEnumerable<MacroStabilityInwardsKernelMessage> kernelMessages = calculator.Validate();

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
        public void Validate_KernelThrowsWaternetKernelWrapperException_ThrowWaternetCalculatorException()
        {
            // Setup
            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                WaternetKernelStub WaternetKernel = factory.LastCreatedWaternetExtremeKernel;
                WaternetKernel.ThrowExceptionOnValidate = true;

                WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();
                var calculator = new TestWaternetCalculator(input, factory);

                // Call
                void Call() => calculator.Validate();

                // Assert
                var exception = Assert.Throws<WaternetCalculatorException>(Call);
                Assert.IsInstanceOf<WaternetKernelWrapperException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        private static void SetCompleteKernelOutput(WaternetKernelStub kernel)
        {
            var headLine = new HeadLine
            {
                Name = "line 1",
                Points =
                {
                    new CSharpWrapperPoint2D(0, 0),
                    new CSharpWrapperPoint2D(1, 1)
                }
            };

            kernel.Waternet = new CSharpWrapperWaternet
            {
                HeadLines =
                {
                    headLine
                },
                PhreaticLine = new HeadLine
                {
                    Name = "line 2",
                    Points =
                    {
                        new CSharpWrapperPoint2D(2, 2),
                        new CSharpWrapperPoint2D(3, 3)
                    }
                },
                ReferenceLines =
                {
                    new ReferenceLine
                    {
                        Name = "line 3",
                        Points =
                        {
                            new CSharpWrapperPoint2D(4, 4),
                            new CSharpWrapperPoint2D(5, 5)
                        },
                        AssociatedHeadLine = headLine
                    }
                }
            };
        }

        private class TestWaternetCalculator : WaternetCalculator
        {
            public TestWaternetCalculator(WaternetCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
                : base(input, factory) {}

            protected override IWaternetKernel CreateWaternetKernel(MacroStabilityInput kernelInput)
            {
                return Factory.CreateWaternetExtremeKernel(kernelInput);
            }
        }
    }
}
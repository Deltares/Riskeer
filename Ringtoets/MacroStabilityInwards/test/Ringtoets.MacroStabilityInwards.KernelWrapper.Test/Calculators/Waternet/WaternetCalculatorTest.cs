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
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;
using WtiStabilityWaternet = Deltares.WTIStability.Data.Geo.Waternet;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.Waternet
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
            TestDelegate call = () => new TestWaternetCalculator(null, factory);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ArgumentNullException()
        {
            // Setup
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            // Call
            TestDelegate call = () => new TestWaternetCalculator(input, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            WaternetKernelStub waternetKernel = testMacroStabilityInwardsKernelFactory.LastCreatedWaternetKernel;
            SetCompleteKernelOutput(waternetKernel);

            // Call
            new TestWaternetCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            Assert.IsTrue(waternetKernel.Calculated);
        }

        [Test]
        public void Calculate_KernelThrowsWaternetKernelWrapperException_ThrowWaternetCalculatorException()
        {
            // Setup
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            WaternetKernelStub waternetKernel = testMacroStabilityInwardsKernelFactory.LastCreatedWaternetKernel;
            waternetKernel.ThrowExceptionOnCalculate = true;

            // Call
            TestDelegate test = () => new TestWaternetCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            var exception = Assert.Throws<WaternetCalculatorException>(test);
            Assert.IsInstanceOf<WaternetKernelWrapperException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void Calculate_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            WaternetKernelStub kernel = testMacroStabilityInwardsKernelFactory.LastCreatedWaternetKernel;
            SetCompleteKernelOutput(kernel);

            // Call
            WaternetCalculatorResult result = new TestWaternetCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            Assert.IsNotNull(result);
            var expectedPhreaticLines = new List<GeometryPointString>
            {
                kernel.Waternet.PhreaticLine
            };
            expectedPhreaticLines.AddRange(kernel.Waternet.HeadLineList);

            WaternetCalculatorOutputAssert.AssertPhreaticLines(expectedPhreaticLines.ToArray(), result.PhreaticLines.ToArray());
            WaternetCalculatorOutputAssert.AssertWaternetLines(kernel.Waternet.WaternetLineList.ToArray(), result.WaternetLines.ToArray());
        }

        private static void SetCompleteKernelOutput(WaternetKernelStub kernel)
        {
            var headLine = new HeadLine
            {
                Name = "line 1",
                Points =
                {
                    new GeometryPoint(0, 0),
                    new GeometryPoint(1, 1)
                }
            };

            kernel.Waternet = new WtiStabilityWaternet
            {
                HeadLineList =
                {
                    headLine
                },
                PhreaticLine = new PhreaticLine
                {
                    Name = "line 2",
                    Points =
                    {
                        new GeometryPoint(2, 2),
                        new GeometryPoint(3, 3)
                    }
                },
                WaternetLineList =
                {
                    new WaternetLine
                    {
                        Name = "line 3",
                        Points =
                        {
                            new GeometryPoint(4, 4),
                            new GeometryPoint(5, 5)
                        },
                        HeadLine = headLine
                    }
                }
            };
        }

        private class TestWaternetCalculator : WaternetCalculator
        {
            public TestWaternetCalculator(WaternetCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
                : base(input, factory) {}

            protected override IWaternetKernel CreateWaternetKernel()
            {
                return Factory.CreateWaternetExtremeKernel();
            }
        }
    }
}
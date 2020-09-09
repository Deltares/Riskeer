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
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Output;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Kernels.UpliftVan
{
    [TestFixture]
    public class UpliftVanKernelWrapperTest
    {
        [Test]
        public void Constructor_CalculatorNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            // Call
            void Call() => new UpliftVanKernelWrapper(null, validator);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculator", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidatorNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            mocks.ReplayAll();

            // Call
            void Call() => new UpliftVanKernelWrapper(calculator, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("validator", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            // Call
            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Assert
            Assert.IsInstanceOf<IUpliftVanKernel>(kernel);
            Assert.IsNaN(kernel.FactorOfStability);
            Assert.IsNaN(kernel.ForbiddenZonesXEntryMin);
            Assert.IsNaN(kernel.ForbiddenZonesXEntryMax);
            Assert.IsNull(kernel.SlidingCurveResult);
            Assert.IsNull(kernel.UpliftVanCalculationGridResult);
            Assert.IsNull(kernel.CalculationMessages);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_SuccessfulCalculation_OutputSet()
        {
            // Setup
            var random = new Random(21);
            var calculatorOutput = new MacroStabilityOutput
            {
                StabilityOutput = new StabilityOutput
                {
                    Succeeded = true,
                    SafetyFactor = random.NextDouble()
                },
                PreprocessingOutputBase = new UpliftVanPreprocessingOutput
                {
                    ForbiddenZone = new ForbiddenZones
                    {
                        XEntryMin = random.NextDouble(),
                        XEntryMax = random.NextDouble()
                    }
                }
            };

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.Calculate()).Return(calculatorOutput);
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Call
            kernel.Calculate();

            // Assert
            Assert.AreEqual(calculatorOutput.StabilityOutput.SafetyFactor, kernel.FactorOfStability);
            Assert.AreEqual(calculatorOutput.PreprocessingOutputBase.ForbiddenZone.XEntryMin, kernel.ForbiddenZonesXEntryMin);
            Assert.AreEqual(calculatorOutput.PreprocessingOutputBase.ForbiddenZone.XEntryMax, kernel.ForbiddenZonesXEntryMax);

            Assert.AreSame((DualSlidingCircleMinimumSafetyCurve) calculatorOutput.StabilityOutput.MinimumSafetyCurve, kernel.SlidingCurveResult);
            Assert.AreSame(((UpliftVanPreprocessingOutput) calculatorOutput.PreprocessingOutputBase).UpliftVanCalculationGrid, kernel.UpliftVanCalculationGridResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationNotSuccessful_OutputSet()
        {
            // Setup
            var random = new Random(21);
            var calculatorOutput = new MacroStabilityOutput
            {
                StabilityOutput = new StabilityOutput
                {
                    Succeeded = false,
                    Messages = new []
                    {
                        MessageHelper.CreateMessage(MessageType.Error, "Message 1"),
                        MessageHelper.CreateMessage(MessageType.Error, "Message 2")
                    }
                }
            };

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.Calculate()).Return(calculatorOutput);
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Call
            void Call() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(Call);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual($"Exception of type '{typeof(UpliftVanKernelWrapperException)}' was thrown.", exception.Message);
            CollectionAssert.AreEqual(calculatorOutput.StabilityOutput.Messages, exception.Messages);

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculatorMessagesNull_CalculationMessagesEmpty()
        {
            // Setup
            var calculatorOutput = new MacroStabilityOutput
            {
                StabilityOutput = new StabilityOutput
                {
                    Succeeded = true
                },
                PreprocessingOutputBase = new UpliftVanPreprocessingOutput
                {
                    ForbiddenZone = new ForbiddenZones()
                }
            };

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.Calculate()).Return(calculatorOutput);
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Call
            kernel.Calculate();

            // Assert
            CollectionAssert.IsEmpty(kernel.CalculationMessages);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculatorHasMessages_CalculationMessagesSet()
        {
            // Setup
            var calculatorOutput = new MacroStabilityOutput
            {
                StabilityOutput = new StabilityOutput
                {
                    Messages = new Message[0],
                    Succeeded = true
                },
                PreprocessingOutputBase = new UpliftVanPreprocessingOutput
                {
                    ForbiddenZone = new ForbiddenZones()
                }
            };

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.Calculate()).Return(calculatorOutput);
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Call
            kernel.Calculate();

            // Assert
            Assert.AreSame(calculatorOutput.StabilityOutput.Messages, kernel.CalculationMessages);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ExceptionInWrappedKernel_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var exceptionToThrow = new Exception();

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.Calculate()).Throw(exceptionToThrow);
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Call
            void Call() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(Call);
            Assert.AreSame(exceptionToThrow, exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ExceptionDuringCalculation_OutputPropertiesNotSet()
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.Calculate()).Throw(new Exception());
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Call
            void Call() => kernel.Calculate();

            // Assert
            Assert.Throws<UpliftVanKernelWrapperException>(Call);
            Assert.IsNaN(kernel.FactorOfStability);
            Assert.IsNaN(kernel.ForbiddenZonesXEntryMax);
            Assert.IsNaN(kernel.ForbiddenZonesXEntryMin);
            Assert.IsNull(kernel.SlidingCurveResult);
            Assert.IsNull(kernel.UpliftVanCalculationGridResult);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_ValidationCompleted_ReturnsValidationMessages()
        {
            // Setup
            var validationOutput = new ValidationOutput
            {
                Messages = new Message[0]
            };

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            var validator = mocks.Stub<IValidator>();
            validator.Stub(v => v.Validate()).Return(validationOutput);
            mocks.ReplayAll();

            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Call
            IEnumerable<Message> validationMessages = kernel.Validate();

            // Assert
            Assert.AreSame(validationOutput.Messages, validationMessages);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_ExceptionInWrappedKernel_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var exceptionToThrow = new Exception();

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            var validator = mocks.Stub<IValidator>();
            validator.Stub(v => v.Validate()).Throw(exceptionToThrow);
            mocks.ReplayAll();

            var kernel = new UpliftVanKernelWrapper(calculator, validator);

            // Call
            void Call() => kernel.Validate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(Call);
            Assert.AreSame(exceptionToThrow, exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }
    }
}
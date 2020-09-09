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
using Deltares.MacroStability.CSharpWrapper.Output.WaternetCreator;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using CSharpWrapperWaternet = Deltares.MacroStability.CSharpWrapper.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Kernels.Waternet
{
    [TestFixture]
    public class WaternetKernelWrapperTest
    {
        [Test]
        public void Constructor_CalculatorNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var validator = mocks.Stub<IValidator>();
            mocks.ReplayAll();

            // Call
            void Call() => new WaternetKernelWrapper(null, validator, string.Empty);

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
            void Call() => new WaternetKernelWrapper(calculator, null, string.Empty);

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
            var kernel = new WaternetKernelWrapper(calculator, validator, "Waternet");

            // Assert
            Assert.IsInstanceOf<IWaternetKernel>(kernel);
            Assert.IsNull(kernel.Waternet);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_SuccessfulCalculation_WaternetSet()
        {
            // Setup
            const string name = "Waternet";

            var output = new WaternetCreatorOutput
            {
                Waternet = new CSharpWrapperWaternet(),
                Messages = new Message[0]
            };

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.CalculateWaternet(0)).Return(output);
            var validator = mocks.Stub<IValidator>();
            validator.Stub(v => v.ValidateWaternetCreator()).Return(new ValidationOutput
            {
                IsValid = true
            });
            mocks.ReplayAll();

            var kernel = new WaternetKernelWrapper(calculator, validator, name);

            // Call
            kernel.Calculate();

            // Assert
            Assert.AreSame(output.Waternet, kernel.Waternet);
            Assert.AreEqual(name, kernel.Waternet.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_WaternetCannotBeGenerated_ThrowsWaternetKernelWrapperException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            var validator = mocks.Stub<IValidator>();
            validator.Stub(v => v.ValidateWaternetCreator()).Return(new ValidationOutput
            {
                IsValid = false
            });
            mocks.ReplayAll();

            var kernel = new WaternetKernelWrapper(calculator, validator, string.Empty);

            // Call
            void Call() => kernel.Calculate();

            // Assert
            Assert.Throws<WaternetKernelWrapperException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ExceptionInWrappedKernel_ThrowsWaternetKernelWrapperException()
        {
            // Setup
            var exceptionToThrow = new Exception();

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.CalculateWaternet(0)).Throw(exceptionToThrow);
            var validator = mocks.Stub<IValidator>();
            validator.Stub(v => v.ValidateWaternetCreator()).Return(new ValidationOutput
            {
                IsValid = true
            });
            mocks.ReplayAll();

            var kernel = new WaternetKernelWrapper(calculator, validator, string.Empty);

            // Call
            void Call() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(Call);
            Assert.AreSame(exceptionToThrow, exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ErrorMessageInCalculationOutput_ThrowsWaternetKernelWrapperException()
        {
            // Setup
            const string message1 = "Message1";
            const string message2 = "Message2";
            const string message3 = "Message3";

            var waternetCreatorOutput = new WaternetCreatorOutput
            {
                Messages = new List<Message>
                {
                    new Message
                    {
                        Content = message1,
                        MessageType = MessageType.Error
                    },
                    new Message
                    {
                        Content = message2,
                        MessageType = MessageType.Error
                    },
                    new Message
                    {
                        Content = message3,
                        MessageType = MessageType.Warning
                    }
                },
                Waternet = new CSharpWrapperWaternet()
            };

            var mocks = new MockRepository();
            var calculator = mocks.Stub<ICalculator>();
            calculator.Stub(c => c.CalculateWaternet(0)).Return(waternetCreatorOutput);
            var validator = mocks.Stub<IValidator>();
            validator.Stub(v => v.ValidateWaternetCreator()).Return(new ValidationOutput
            {
                IsValid = true
            });
            mocks.ReplayAll();

            var kernel = new WaternetKernelWrapper(calculator, validator, string.Empty);

            // Call
            void Call() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(Call);
            string expectedMessage = $"{message1}{Environment.NewLine}" +
                                     $"{message2}";
                                     Assert.AreEqual(expectedMessage, exception.Message);
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
            validator.Stub(v => v.ValidateWaternetCreator()).Return(validationOutput);
            mocks.ReplayAll();

            var kernel = new WaternetKernelWrapper(calculator, validator, string.Empty);

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
            validator.Stub(v => v.ValidateWaternetCreator()).Throw(exceptionToThrow);
            mocks.ReplayAll();

            var kernel = new WaternetKernelWrapper(calculator, validator, string.Empty);

            // Call
            void Call() => kernel.Validate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(Call);
            Assert.AreSame(exceptionToThrow, exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }
    }
}
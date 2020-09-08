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
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.CSharpWrapper.Output;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan
{
    [TestFixture]
    public class UpliftVanKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new UpliftVanKernelStub();

            // Assert
            Assert.IsInstanceOf<IUpliftVanKernel>(kernel);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsFalse(kernel.Validated);
            Assert.IsFalse(kernel.ThrowExceptionOnCalculate);
            Assert.IsFalse(kernel.ThrowExceptionOnValidate);
            Assert.IsFalse(kernel.ReturnValidationResults);
            Assert.IsFalse(kernel.ReturnLogMessages);

            Assert.IsNull(kernel.KernelInput);

            Assert.AreEqual(0.0, kernel.FactorOfStability);
            Assert.AreEqual(0.0, kernel.ForbiddenZonesXEntryMin);
            Assert.AreEqual(0.0, kernel.ForbiddenZonesXEntryMax);
            Assert.IsNull(kernel.SlidingCurveResult);
            Assert.IsNull(kernel.UpliftVanCalculationGridResult);
            Assert.IsNull(kernel.CalculationMessages);
        }

        [Test]
        public void SetInput_Always_SetsKernelInputOnKernel()
        {
            // Setup
            var input = new MacroStabilityInput();

            var kernel = new UpliftVanKernelStub();

            // Call
            kernel.SetInput(input);

            // Assert
            Assert.AreSame(input, kernel.KernelInput);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateFalse_SetCalculatedTrue()
        {
            // Setup
            var kernel = new UpliftVanKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.Calculate();

            // Assert
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateTrue_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var kernel = new UpliftVanKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            void Call() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(Call);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsFalse(kernel.Validated);
            Assert.IsTrue(kernel.ThrowExceptionOnCalculate);
            Assert.IsFalse(kernel.ThrowExceptionOnValidate);
            Assert.IsFalse(kernel.ReturnValidationResults);
            Assert.IsFalse(kernel.ReturnLogMessages);
        }

        [Test]
        public void Calculate_ReturnLogMessagesTrue_ReturnsLogMessages()
        {
            // Setup
            var calculator = new UpliftVanKernelStub
            {
                ReturnLogMessages = true
            };

            // Call
            calculator.Calculate();

            // Assert
            IEnumerable<Message> results = calculator.CalculationMessages.ToList();
            Assert.IsTrue(calculator.Calculated);
            Assert.AreEqual(3, results.Count());
            AssertMessage(CreateMessage(MessageType.Info, "Calculation Info"), results.ElementAt(0));
            AssertMessage(CreateMessage(MessageType.Warning, "Calculation Warning"), results.ElementAt(1));
            AssertMessage(CreateMessage(MessageType.Error, "Calculation Error"), results.ElementAt(2));
        }

        [Test]
        public void Calculate_ReturnLogMessagesFalse_ReturnsNoLogMessages()
        {
            // Setup
            var calculator = new UpliftVanKernelStub
            {
                ReturnLogMessages = false
            };

            // Call
            calculator.Calculate();

            // Assert
            Assert.IsTrue(calculator.Calculated);
            CollectionAssert.IsEmpty(calculator.CalculationMessages);
        }

        [Test]
        public void Validate_ThrowExceptionOnValidateFalse_SetValidatedTrue()
        {
            // Setup
            var kernel = new UpliftVanKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Validated);

            // Call
            kernel.Validate().ToArray();

            // Assert
            Assert.IsTrue(kernel.Validated);
        }

        [Test]
        public void Validate_ThrowExceptionOnValidateTrue_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var kernel = new UpliftVanKernelStub
            {
                ThrowExceptionOnValidate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Validated);

            // Call
            void Call() => kernel.Validate().ToArray();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(Call);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Validated);
        }

        [Test]
        public void Validate_ReturnValidationResultsTrue_ReturnsValidationResults()
        {
            // Setup
            var kernel = new UpliftVanKernelStub
            {
                ReturnValidationResults = true
            };

            // Call
            Message[] messages = kernel.Validate().ToArray();

            // Assert
            Assert.IsTrue(kernel.Validated);
            Assert.AreEqual(3, messages.Length);
            AssertMessage(CreateMessage(MessageType.Warning, "Validation Warning"), messages[0]);
            AssertMessage(CreateMessage(MessageType.Error, "Validation Error"), messages[1]);
            AssertMessage(CreateMessage(MessageType.Info, "Validation Info"), messages[2]);
        }

        [Test]
        public void Validate_ReturnValidationResultsFalse_ReturnsNoValidationResults()
        {
            // Setup
            var kernel = new UpliftVanKernelStub
            {
                ReturnValidationResults = false
            };

            // Call
            Message[] results = kernel.Validate().ToArray();

            // Assert
            Assert.IsTrue(kernel.Validated);
            CollectionAssert.IsEmpty(results);
        }

        private static Message CreateMessage(MessageType messageType, string message)
        {
            return new Message
            {
                Content = message,
                MessageType = messageType
            };
        }

        private static void AssertMessage(Message expected, Message actual)
        {
            Assert.AreEqual(expected.MessageType, actual.MessageType);
            Assert.AreEqual(expected.Content, actual.Content);
        }
    }
}
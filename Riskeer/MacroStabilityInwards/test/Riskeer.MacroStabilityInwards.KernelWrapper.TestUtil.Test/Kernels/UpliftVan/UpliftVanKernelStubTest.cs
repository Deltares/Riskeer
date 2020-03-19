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
using Deltares.MacroStability.Standard;
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
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
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
            IEnumerable<LogMessage> results = calculator.CalculationMessages.ToList();
            Assert.IsTrue(calculator.Calculated);
            Assert.AreEqual(6, results.Count());
            AssertLogMessage(new LogMessage(LogMessageType.Trace, "subject", "Calculation Trace"), results.ElementAt(0));
            AssertLogMessage(new LogMessage(LogMessageType.Debug, "subject", "Calculation Debug"), results.ElementAt(1));
            AssertLogMessage(new LogMessage(LogMessageType.Info, "subject", "Calculation Info"), results.ElementAt(2));
            AssertLogMessage(new LogMessage(LogMessageType.Warning, "subject", "Calculation Warning"), results.ElementAt(3));
            AssertLogMessage(new LogMessage(LogMessageType.Error, "subject", "Calculation Error"), results.ElementAt(4));
            AssertLogMessage(new LogMessage(LogMessageType.FatalError, "subject", "Calculation Fatal Error"), results.ElementAt(5));
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
            kernel.Validate().ToList();

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
            TestDelegate test = () => kernel.Validate().ToList();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Validated);
        }

        [Test]
        public void Validate_ReturnValidationResultsTrue_ReturnsValidationResults()
        {
            // Setup
            var calculator = new UpliftVanKernelStub
            {
                ReturnValidationResults = true
            };

            // Call
            IEnumerable<ValidationResult> results = calculator.Validate().ToList();

            // Assert
            Assert.IsTrue(calculator.Validated);
            Assert.AreEqual(4, results.Count());
            AssertValidationResult(new ValidationResult(ValidationResultType.Warning, "Validation Warning"), results.ElementAt(0));
            AssertValidationResult(new ValidationResult(ValidationResultType.Error, "Validation Error"), results.ElementAt(1));
            AssertValidationResult(new ValidationResult(ValidationResultType.Info, "Validation Info"), results.ElementAt(2));
            AssertValidationResult(new ValidationResult(ValidationResultType.Debug, "Validation Debug"), results.ElementAt(3));
        }

        [Test]
        public void Validate_ReturnValidationResultsFalse_ReturnsNoValidationResults()
        {
            // Setup
            var calculator = new UpliftVanKernelStub
            {
                ReturnValidationResults = false
            };

            // Call
            IEnumerable<ValidationResult> results = calculator.Validate().ToList();

            // Assert
            Assert.IsTrue(calculator.Validated);
            CollectionAssert.IsEmpty(results);
        }

        private static void AssertValidationResult(IValidationResult expected, IValidationResult actual)
        {
            Assert.AreEqual(expected.MessageType, actual.MessageType);
            Assert.AreEqual(expected.Text, actual.Text);
        }

        private static void AssertLogMessage(LogMessage expected, LogMessage actual)
        {
            Assert.AreEqual(expected.MessageType, actual.MessageType);
            Assert.AreEqual(expected.Message, actual.Message);
            Assert.AreEqual(expected.Subject, actual.Subject);
        }
    }
}
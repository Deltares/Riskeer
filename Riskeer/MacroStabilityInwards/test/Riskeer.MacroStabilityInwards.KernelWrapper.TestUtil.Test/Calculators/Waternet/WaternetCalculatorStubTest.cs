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
using System.Linq;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators.Waternet
{
    [TestFixture]
    public class WaternetCalculatorStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculator = new WaternetCalculatorStub();

            // Assert
            Assert.IsInstanceOf<IWaternetCalculator>(calculator);
            Assert.IsNull(calculator.Input);
            Assert.IsNull(calculator.Output);
            Assert.IsFalse(calculator.ThrowExceptionOnCalculate);
            Assert.IsFalse(calculator.ThrowExceptionOnValidate);
            Assert.IsFalse(calculator.ReturnValidationWarning);
            Assert.IsFalse(calculator.ReturnValidationError);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateFalse_ReturnResult()
        {
            // Setup
            var calculator = new WaternetCalculatorStub();

            // Call
            WaternetCalculatorResult result = calculator.Calculate();

            // Assert
            Assert.AreEqual(1, result.PhreaticLines.Count());
            Assert.AreEqual(1, result.WaternetLines.Count());
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateTrue_ThrowWaternetCalculatorException()
        {
            // Setup
            var calculator = new WaternetCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            void Call() => calculator.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetCalculatorException>(Call);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNull(calculator.Output);
        }

        [Test]
        public void Validate_ReturnValidationErrorAndWarningFalse_ReturnsEmptyEnumerable()
        {
            // Setup
            var calculator = new WaternetCalculatorStub();

            // Call
            MacroStabilityInwardsKernelMessage[] messages = calculator.Validate().ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void Validate_ReturnValidationErrorAndWarningTrue_ReturnsKernelMessages()
        {
            // Setup
            var calculator = new WaternetCalculatorStub
            {
                ReturnValidationError = true,
                ReturnValidationWarning = true
            };

            // Call
            MacroStabilityInwardsKernelMessage[] messages = calculator.Validate().ToArray();

            // Assert
            Assert.AreEqual(2, messages.Length);

            MacroStabilityInwardsKernelMessage firstMessage = messages[0];
            Assert.AreEqual("Validation Error", firstMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Error, firstMessage.ResultType);

            MacroStabilityInwardsKernelMessage secondMessage = messages[1];
            Assert.AreEqual("Validation Warning", secondMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Warning, secondMessage.ResultType);
        }

        [Test]
        public void Validate_ThrowExceptionOnValidateTrue_ThrowWaternetCalculatorException()
        {
            // Setup
            var calculator = new WaternetCalculatorStub
            {
                ThrowExceptionOnValidate = true
            };

            // Call
            void Call() => calculator.Validate().ToArray();

            // Assert
            var exception = Assert.Throws<WaternetCalculatorException>(Call);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
        }
    }
}
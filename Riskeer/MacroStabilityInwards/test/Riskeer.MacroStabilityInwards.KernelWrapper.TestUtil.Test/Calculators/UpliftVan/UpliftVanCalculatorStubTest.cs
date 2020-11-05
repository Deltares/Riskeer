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
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators.UpliftVan
{
    [TestFixture]
    public class UpliftVanCalculatorStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculator = new UpliftVanCalculatorStub();

            // Assert
            Assert.IsInstanceOf<IUpliftVanCalculator>(calculator);
            Assert.IsNull(calculator.Input);
            Assert.IsNull(calculator.Output);
            Assert.IsFalse(calculator.ThrowExceptionOnCalculate);
            Assert.IsFalse(calculator.ThrowExceptionOnValidate);
            Assert.IsFalse(calculator.ReturnValidationWarning);
            Assert.IsFalse(calculator.ReturnValidationError);
            Assert.IsFalse(calculator.ReturnCalculationError);
            Assert.IsFalse(calculator.ReturnCalculationWarning);
            Assert.IsFalse(calculator.Calculated);
        }

        [Test]
        public void Calculate_ReturnCalculationErrorAndWarningFalse_ReturnsEmptyEnumerable()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub();

            // Call
            calculator.Calculate();

            // Assert
            CollectionAssert.IsEmpty(calculator.Output.CalculationMessages);
        }

        [Test]
        public void Calculate_ReturnCalculationErrorAndWarningTrue_ReturnsKernelMessages()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub
            {
                ReturnCalculationError = true,
                ReturnCalculationWarning = true
            };

            // Call
            calculator.Calculate();

            // Assert
            MacroStabilityInwardsKernelMessage[] messages = calculator.Output.CalculationMessages.ToArray();
            Assert.AreEqual(4, messages.Length);

            MacroStabilityInwardsKernelMessage firstMessage = messages[0];
            Assert.AreEqual("Calculation Error 1", firstMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Error, firstMessage.Type);

            MacroStabilityInwardsKernelMessage secondMessage = messages[1];
            Assert.AreEqual("Calculation Warning 1", secondMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Warning, secondMessage.Type);

            MacroStabilityInwardsKernelMessage thirdMessage = messages[2];
            Assert.AreEqual("Calculation Error 2", thirdMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Error, thirdMessage.Type);

            MacroStabilityInwardsKernelMessage fourthMessage = messages[3];
            Assert.AreEqual("Calculation Warning 2", fourthMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Warning, fourthMessage.Type);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateFalse_CalculatedTrueAndReturnResult()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub();

            // Call
            UpliftVanCalculatorResult result = calculator.Calculate();

            // Assert
            Assert.IsTrue(calculator.Calculated);
            Assert.AreEqual(0.1, result.FactorOfStability);
            Assert.AreEqual(0.3, result.ForbiddenZonesXEntryMin);
            Assert.AreEqual(0.4, result.ForbiddenZonesXEntryMax);
            Assert.IsNotNull(result.SlidingCurveResult);
            Assert.IsNotNull(result.CalculationGridResult);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateTrue_ThrowUpliftVanCalculatorException()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            void Call() => calculator.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanCalculatorException>(Call);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNull(calculator.Output);
        }

        [Test]
        public void Validate_ReturnValidationErrorAndWarningFalse_ReturnsEmptyEnumerable()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub();

            // Call
            MacroStabilityInwardsKernelMessage[] messages = calculator.Validate().ToArray();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void Validate_ReturnValidationErrorAndWarningTrue_ReturnsKernelMessages()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub
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
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Error, firstMessage.Type);

            MacroStabilityInwardsKernelMessage secondMessage = messages[1];
            Assert.AreEqual("Validation Warning", secondMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Warning, secondMessage.Type);
        }

        [Test]
        public void Validate_ThrowExceptionOnValidateTrue_ThrowUpliftVanCalculatorException()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub
            {
                ThrowExceptionOnValidate = true
            };

            // Call
            void Call() => calculator.Validate().ToArray();

            // Assert
            var exception = Assert.Throws<UpliftVanCalculatorException>(Call);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
        }
    }
}
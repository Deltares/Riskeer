// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators.UpliftVan
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
            Assert.IsFalse(calculator.ReturnValidationWarning);
            Assert.IsFalse(calculator.ReturnValidationError);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateFalse_ReturnResult()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub();

            // Call
            UpliftVanCalculatorResult result = calculator.Calculate();

            // Assert
            Assert.AreEqual(0.1, result.FactorOfStability);
            Assert.AreEqual(0.2, result.ZValue);
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
            TestDelegate test = () => calculator.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanCalculatorException>(test);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNull(calculator.Output);
        }

        [Test]
        public void Validate_ReturnValidationResultsFalse_ReturnsEmptyValidationResult()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub();

            // Call
            IEnumerable<UpliftVanValidationResult> result = calculator.Validate();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Validate_ReturnValidationErrorAndWarningTrue_ReturnsValidationResults()
        {
            // Setup
            var calculator = new UpliftVanCalculatorStub
            {
                ReturnValidationError = true,
                ReturnValidationWarning = true
            };

            // Call
            IEnumerable<UpliftVanValidationResult> results = calculator.Validate();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Validation Error", results.ElementAt(0).Message);
            Assert.AreEqual(UpliftVanValidationResultType.Error, results.ElementAt(0).ResultType);
            Assert.AreEqual("Validation Warning", results.ElementAt(1).Message);
            Assert.AreEqual(UpliftVanValidationResultType.Warning, results.ElementAt(1).ResultType);
        }
    }
}
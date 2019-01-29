// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using NUnit.Framework;
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
            TestDelegate test = () => calculator.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetCalculatorException>(test);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNull(calculator.Output);
        }
    }
}
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

using System.Collections.Generic;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculator = new MacroStabilityInwardsCalculatorStub();

            // Assert
            Assert.IsInstanceOf<IUpliftVanCalculator>(calculator);
            Assert.IsNull(calculator.Input);
            Assert.IsNull(calculator.Output);
        }

        [Test]
        public void Validate_Always_ReturnEmptyList()
        {
            // Setup
            var calculator = new MacroStabilityInwardsCalculatorStub();

            // Call
            List<string> messages = calculator.Validate();

            // Assert
            CollectionAssert.IsEmpty(messages);
        }

        [Test]
        public void Calculate_Always_ReturnResult()
        {
            // Setup
            var calculator = new MacroStabilityInwardsCalculatorStub();
            
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
    }
}
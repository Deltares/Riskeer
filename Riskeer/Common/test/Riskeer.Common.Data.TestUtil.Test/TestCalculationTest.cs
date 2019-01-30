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

using Core.Common.Base;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestCalculationTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var calculation = new TestCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.Comments);
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const string name = "Name";

            // Call
            var calculation = new TestCalculation(name);

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreEqual(name, calculation.Name);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.Comments);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new TestCalculation
            {
                Output = new object()
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(hasOutput);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new TestCalculation
            {
                Output = null
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsFalse(hasOutput);
        }

        [Test]
        public void ShouldCalculate_OutputSet_ReturnsFalse()
        {
            // Setup
            var calculation = new TestCalculation
            {
                Output = new object()
            };

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsFalse(shouldCalculate);
        }

        [Test]
        public void ShouldCalculate_OutputNull_ReturnsTrue()
        {
            // Setup
            var calculation = new TestCalculation
            {
                Output = null
            };

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsTrue(shouldCalculate);
        }

        [Test]
        public void ClearOutput_OutputSet_OutputSetToNull()
        {
            // Setup
            var calculation = new TestCalculation
            {
                Output = new object()
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }
    }
}
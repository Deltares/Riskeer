// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresCalculationTest
    {
        [Test]
        public void Constructor_Default_ExpectedValues()
        {
            // Call
            var calculation = new TestStructuresCalculation();

            // Assert
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new TestStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new TestStructuresCalculation
            {
                Output = null
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsFalse(calculationHasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new TestStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };

            // Call 
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(calculationHasOutput);
        }

        [Test]
        public void ToString_Always_ReturnName()
        {
            // Setup
            var expectedName = "someTestName";
            var calculation = new TestStructuresCalculation
            {
                Name = expectedName
            };

            // Call
            string result = calculation.ToString();

            // Assert
            Assert.AreEqual(expectedName, result);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Property_Comments_ReturnsExpectedValues(string comments)
        {
            // Setup
            var calculation = new TestStructuresCalculation();

            // Call
            calculation.Comments.Body = comments;

            // Assert
            Assert.AreEqual(comments, calculation.Comments.Body);
        }

        private class TestStructuresCalculation : StructuresCalculation<TestStructuresInput> {}

        private class TestStructuresInput : ICalculationInput
        {
            public void Attach(IObserver observer) {}

            public void Detach(IObserver observer) {}

            public void NotifyObservers() {}
        }
    }
}
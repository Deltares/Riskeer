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

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class StabilityPointStructuresCalculationTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var calculation = new StabilityPointStructuresCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Properties_Name_ReturnsExpectedValues(string name)
        {
            // Setup
            var calculation = new StabilityPointStructuresCalculation();

            // Call
            calculation.Name = name;

            // Assert
            Assert.AreEqual(name, calculation.Name);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Property_Comments_ReturnsExpectedValues(string comments)
        {
            // Setup
            var calculation = new StabilityPointStructuresCalculation();

            // Call
            calculation.Comments = comments;

            // Assert
            Assert.AreEqual(comments, calculation.Comments);
        }

        [Test]
        public void ToString_Always_ReturnsExpectedValue()
        {
            // Setup
            const string name = "StabilityPointStructuresCalculation";
            var calculation = new StabilityPointStructuresCalculation()
            {
                Name = name
            };

            // Call
            string toString = calculation.ToString();

            // Assert
            Assert.AreEqual(name, toString);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new StabilityPointStructuresCalculation
            {
                Output = new TestStabilityPointStructuresOutput()
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
            var calculation = new StabilityPointStructuresCalculation
            {
                Output = null
            };

            // Call & Assert
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new StabilityPointStructuresCalculation
            {
                Output = new TestStabilityPointStructuresOutput()
            };

            // Call & Assert
            Assert.IsTrue(calculation.HasOutput);
        }

        [Test]
        public void GetObservableInput_Always_ReturnsInputParameters()
        {
            // Setup
            var calculation = new StabilityPointStructuresCalculation();
            var inputParameters = calculation.InputParameters;

            // Call
            ICalculationInput input = calculation.GetObservableInput();

            // Assert
            Assert.AreSame(inputParameters, input);
        }

        [Test]
        public void GetObservableOutput_Always_ReturnsOutput()
        {
            // Setup
            var output = new TestStabilityPointStructuresOutput();
            var calculation = new StabilityPointStructuresCalculation
            {
                Output = output
            };

            // Call
            ICalculationOutput calculationOutput = calculation.GetObservableOutput();

            // Assert
            Assert.AreSame(output, calculationOutput);
        }

        private class TestStabilityPointStructuresOutput : StabilityPointStructuresOutput {}
    }
}
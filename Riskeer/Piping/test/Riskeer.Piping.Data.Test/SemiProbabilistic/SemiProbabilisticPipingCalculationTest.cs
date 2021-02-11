// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingCalculationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculation = new TestSemiProbabilisticPipingCalculation();

            // Assert
            Assert.IsInstanceOf<PipingCalculation<SemiProbabilisticPipingInput>>(calculation);
            Assert.IsInstanceOf<SemiProbabilisticPipingInput>(calculation.InputParameters);

            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void ShouldCalculate_HasOutputFalse_ReturnsTrue()
        {
            // Setup
            var calculation = new TestSemiProbabilisticPipingCalculation
            {
                Output = null
            };

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsTrue(shouldCalculate);
        }

        [Test]
        public void ShouldCalculate_HasOutputTrue_ReturnsFalse()
        {
            // Setup
            var calculation = new TestSemiProbabilisticPipingCalculation
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            };

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsFalse(shouldCalculate);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new TestSemiProbabilisticPipingCalculation
            {
                Output = null
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsFalse(hasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new TestSemiProbabilisticPipingCalculation
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(hasOutput);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new TestSemiProbabilisticPipingCalculation
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            SemiProbabilisticPipingCalculation original = CreateRandomCalculationWithoutOutput();

            original.Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            SemiProbabilisticPipingCalculation original = CreateRandomCalculationWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        private static SemiProbabilisticPipingCalculation CreateRandomCalculationWithoutOutput()
        {
            var calculation = new TestSemiProbabilisticPipingCalculation
            {
                Name = "Random name",
                Comments =
                {
                    Body = "Random body"
                }
            };

            PipingTestDataGenerator.SetRandomDataToPipingInput(calculation.InputParameters);

            return calculation;
        }
    }
}
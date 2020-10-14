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
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingCalculationTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var calculation = new TestPipingCalculation();

            // Assert
            Assert.IsInstanceOf<IPipingCalculation<PipingInput>>(calculation);
            Assert.IsInstanceOf<CloneableObservable>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);

            Assert.IsInstanceOf<PipingInput>(calculation.InputParameters);

            Assert.IsNull(calculation.Comments.Body);
        }

        [Test]
        public void Constructor_PipingInputIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestPipingCalculation(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("pipingInput", paramName);
        }

        [Test]
        public void ShouldCalculate_HasOutputFalse_ReturnsTrue()
        {
            // Setup
            var calculation = new TestPipingCalculation();

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsTrue(shouldCalculate);
        }

        [Test]
        public void ShouldCalculate_HasOutputTrue_ReturnsFalse()
        {
            // Setup
            var calculation = new TestPipingCalculation(true);

            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.IsFalse(shouldCalculate);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            PipingCalculation<PipingInput, SemiProbabilisticPipingOutput> original = CreateRandomCalculationWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            PipingCalculation<PipingInput, SemiProbabilisticPipingOutput> original = CreateRandomCalculationWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        private static PipingCalculation<PipingInput, SemiProbabilisticPipingOutput> CreateRandomCalculationWithoutOutput()
        {
            var calculation = new TestPipingCalculation
            {
                Comments =
                {
                    Body = "Random body"
                },
                Name = "Random name"
            };

            PipingTestDataGenerator.SetRandomDataToPipingInput(calculation.InputParameters);

            return calculation;
        }
    }
}
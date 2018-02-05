﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingCalculationTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();

            // Call
            var calculation = new PipingCalculation(generalInputParameters);

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<CloneableObservable>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);

            Assert.IsInstanceOf<PipingInput>(calculation.InputParameters);

            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Constructor_GeneralPipingInputIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingCalculation(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
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
            var calculation = new PipingCalculation(new GeneralPipingInput())
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
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(calculationHasOutput);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            PipingCalculation original = CreateRandomCalculationWithoutOutput();

            original.Output = PipingTestDataGenerator.GetRandomPipingOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            PipingCalculation original = CreateRandomCalculationWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        private static PipingCalculation CreateRandomCalculationWithoutOutput()
        {
            var calculation = new PipingCalculation(new GeneralPipingInput())
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
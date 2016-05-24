﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresCalculationTest
    {
        [Test]
        public void Constructor_NullGeneralInput_ThrowsArgumentNullException()
        {
            // Setup
            var normProbabilityInput = new NormProbabilityInput();

            // Call
            TestDelegate test = () => new HeightStructuresCalculation(null, normProbabilityInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalInputParameters", exception.ParamName);
        }

        [Test]
        public void Constructor_NullNormProbabilityInput_ThrowsArgumentNullException()
        {
            // Setup
            var generalInput = new GeneralHeightStructuresInput();

            // Call
            TestDelegate test = () => new HeightStructuresCalculation(generalInput, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("normProbabilityInput", exception.ParamName);
        }

        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Setup
            var generalInput = new GeneralHeightStructuresInput();
            var normProbabilityInput = new NormProbabilityInput();

            // Call
            var calculation = new HeightStructuresCalculation(generalInput, normProbabilityInput);

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.AreEqual(normProbabilityInput, calculation.NormProbabilityInput);
            Assert.IsNull(calculation.Comments);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var generalInput = new GeneralHeightStructuresInput();
            var normProbabilityInput = new NormProbabilityInput();
            var calculation = new HeightStructuresCalculation(generalInput, normProbabilityInput)
            {
                Output = new TestHeightStructuresOutput()
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
            var generalInput = new GeneralHeightStructuresInput();
            var normProbabilityInput = new NormProbabilityInput();
            var calculation = new HeightStructuresCalculation(generalInput, normProbabilityInput)
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
            var generalInput = new GeneralHeightStructuresInput();
            var normProbabilityInput = new NormProbabilityInput();
            var calculation = new HeightStructuresCalculation(generalInput, normProbabilityInput)
            {
                Output = new TestHeightStructuresOutput()
            };

            // Call & Assert
            Assert.IsTrue(calculation.HasOutput);
        }

        [Test]
        public void GetObservableInput_Always_ReturnsInputParameters()
        {
            // Setup
            var generalInput = new GeneralHeightStructuresInput();
            var normProbabilityInput = new NormProbabilityInput();
            var calculation = new HeightStructuresCalculation(generalInput, normProbabilityInput);

            // Call
            ICalculationInput input = calculation.GetObservableInput();

            // Assert
            Assert.AreSame(calculation.InputParameters, input);
        }

        [Test]
        public void GetObservableOutput_Always_ReturnsOutput()
        {
            // Setup
            var generalInput = new GeneralHeightStructuresInput();
            var normProbabilityInput = new NormProbabilityInput();
            var calculation = new HeightStructuresCalculation(generalInput, normProbabilityInput)
            {
                Output = new TestHeightStructuresOutput()
            };

            // Call
            ICalculationOutput output = calculation.GetObservableOutput();

            // Assert
            Assert.AreSame(calculation.Output, output);
        }

        private class TestHeightStructuresOutput : ProbabilisticOutput
        {
            public TestHeightStructuresOutput() : base(0, 0, 0, 0, 0) {}
        }
    }
}
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

using System;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingProbabilityAssessmentInputTest
    {
        [Test]
        public void Constructor_DefaultPropertiesSet()
        {
            // Call
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Assert
            Assert.AreEqual(0.4, pipingProbabilityAssessmentInput.A);
            Assert.AreEqual(300, pipingProbabilityAssessmentInput.B);

            Assert.AreEqual(1.2, pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor.Value);
            Assert.AreEqual(1, pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor.NumberOfDecimalPlaces);

            Assert.IsNaN(pipingProbabilityAssessmentInput.SectionLength);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        public void A_InvalidValue_ThrowsArgumentException(double value)
        {
            // Setup
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            TestDelegate call = () => pipingProbabilityAssessmentInput.A = value;

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("De waarde moet in het bereik [0, 1] liggen.", exception.Message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.1)]
        [TestCase(1)]
        [TestCase(0.0000001)]
        [TestCase(0.9999999)]
        public void A_ValidValue_SetsValue(double value)
        {
            // Setup
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            pipingProbabilityAssessmentInput.A = value;

            // Assert
            Assert.AreEqual(value, pipingProbabilityAssessmentInput.A);
        }

        [Test]
        [TestCase(-0.05)]
        [TestCase(0.0)]
        [TestCase(0.04)]
        [TestCase(50.05)]
        [TestCase(50.1)]
        public void UpliftCriticalSafetyFactor_InvalidValue_ThrowsArgumentException(double value)
        {
            // Setup
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            TestDelegate test = () => pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor = (RoundedDouble) value;

            // Assert
            var expectedMessage = "Kritische veiligheidsfactor voor opbarsten moet in het bereik (0, 50] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(0.05)]
        [TestCase(50.04)]
        [TestCase(50.0)]
        public void UpliftCriticalSafetyFactor_ValidValue_SetsValue(double value)
        {
            // Setup
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(value, pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor, pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor.GetAccuracy());
        }
    }
}
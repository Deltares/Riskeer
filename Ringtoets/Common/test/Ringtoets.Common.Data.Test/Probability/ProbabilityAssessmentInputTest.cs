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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Data.Test.Probability
{
    [TestFixture]
    public class ProbabilityAssessmentInputTest
    {
        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        [TestCase(double.NaN)]
        public void Constructor_InvalidA_ThrowsArgumentOutOfRangeException(double a)
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new TestProbabilityAssessmentInput(a,
                                                                         random.NextDouble());

            // Assert
            const string expectedMessage = "De waarde voor 'a' moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var random = new Random(39);
            double a = random.NextDouble();
            double b = random.NextDouble();
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(a, b);

            // Assert
            Assert.AreEqual(a, probabilityAssessmentInput.A);
            Assert.AreEqual(b, probabilityAssessmentInput.B);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        [TestCase(double.NaN)]
        public void A_InvalidValue_ThrowsArgumentOutOfRangeException(double a)
        {
            // Setup
            var random = new Random(39);
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(random.NextDouble(),
                                                                                random.NextDouble());

            // Call
            TestDelegate call = () => probabilityAssessmentInput.A = a;

            // Assert
            const string expectedMessage = "De waarde voor 'a' moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.1)]
        [TestCase(1)]
        [TestCase(0.0000001)]
        [TestCase(0.9999999)]
        public void A_ValidValue_SetsValue(double a)
        {
            // Setup
            var random = new Random(39);
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(random.NextDouble(),
                                                                                random.NextDouble());

            // Call
            probabilityAssessmentInput.A = a;

            // Assert
            Assert.AreEqual(a, probabilityAssessmentInput.A);
        }

        private class TestProbabilityAssessmentInput : ProbabilityAssessmentInput
        {
            public TestProbabilityAssessmentInput(double a, double b) : base(a, b) {}
        }
    }
}
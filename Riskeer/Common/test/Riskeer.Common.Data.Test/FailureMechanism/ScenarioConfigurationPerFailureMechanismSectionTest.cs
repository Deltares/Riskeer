// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class ScenarioConfigurationPerFailureMechanismSectionTest
    {
        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new TestScenarioConfigurationPerFailureMechanismSection(null, random.NextRoundedDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            RoundedDouble a = random.NextRoundedDouble();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var scenarioConfigurationPerFailureMechanismSection = new TestScenarioConfigurationPerFailureMechanismSection(section, a);

            // Assert
            Assert.AreEqual(3, scenarioConfigurationPerFailureMechanismSection.A.NumberOfDecimalPlaces);
            Assert.AreEqual(a, scenarioConfigurationPerFailureMechanismSection.A, scenarioConfigurationPerFailureMechanismSection.A.GetAccuracy());

            Assert.AreSame(section, scenarioConfigurationPerFailureMechanismSection.Section);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-0.0005)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(1.0005)]
        [TestCase(8)]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void A_InvalidValue_ThrowsArgumentOutOfRangeException(double a)
        {
            // Setup
            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfigurationPerFailureMechanismSection = new TestScenarioConfigurationPerFailureMechanismSection(section, random.NextRoundedDouble());

            // Call
            void Call() => scenarioConfigurationPerFailureMechanismSection.A = (RoundedDouble) a;

            // Assert
            const string expectedMessage = "De waarde voor 'a' moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(-0.0004)]
        [TestCase(0)]
        [TestCase(0.1)]
        [TestCase(1)]
        [TestCase(1.0004)]
        [TestCase(0.0000001)]
        [TestCase(0.9999999)]
        public void A_ValidValue_SetsValue(double a)
        {
            // Setup
            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfigurationPerFailureMechanismSection = new TestScenarioConfigurationPerFailureMechanismSection(section, random.NextRoundedDouble());

            // Call
            scenarioConfigurationPerFailureMechanismSection.A = (RoundedDouble) a;

            // Assert
            Assert.AreEqual(3, scenarioConfigurationPerFailureMechanismSection.A.NumberOfDecimalPlaces);
            Assert.AreEqual(a, scenarioConfigurationPerFailureMechanismSection.A, scenarioConfigurationPerFailureMechanismSection.A.GetAccuracy());
        }

        private class TestScenarioConfigurationPerFailureMechanismSection : ScenarioConfigurationPerFailureMechanismSection
        {
            public TestScenarioConfigurationPerFailureMechanismSection(FailureMechanismSection section, RoundedDouble a) : base(section, a) {}
        }
    }
}
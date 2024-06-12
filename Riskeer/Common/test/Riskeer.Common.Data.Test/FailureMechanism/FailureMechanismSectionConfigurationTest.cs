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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionConfigurationTest
    {
        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismSectionConfiguration(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var sectionConfiguration = new FailureMechanismSectionConfiguration(section);

            // Assert
            Assert.AreEqual(3, sectionConfiguration.A.NumberOfDecimalPlaces);
            Assert.AreEqual(1, sectionConfiguration.A, sectionConfiguration.A.GetAccuracy());

            Assert.AreSame(section, sectionConfiguration.Section);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetInvalidAValues))]
        public void A_InvalidValue_ThrowsArgumentOutOfRangeException(double a)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new FailureMechanismSectionConfiguration(section);

            // Call
            void Call() => sectionConfiguration.A = (RoundedDouble) a;

            // Assert
            const string expectedMessage = "De waarde voor 'a' moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(-0.0004)]
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1)]
        [TestCase(1.0004)]
        public void A_ValidValue_SetsValue(double a)
        {
            // Setup
            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new FailureMechanismSectionConfiguration(section);

            // Call
            sectionConfiguration.A = (RoundedDouble) a;

            // Assert
            Assert.AreEqual(3, sectionConfiguration.A.NumberOfDecimalPlaces);
            Assert.AreEqual(a, sectionConfiguration.A, sectionConfiguration.A.GetAccuracy());
        }

        private static IEnumerable<TestCaseData> GetInvalidAValues()
        {
            yield return new TestCaseData(double.NegativeInfinity);
            yield return new TestCaseData(-1);
            yield return new TestCaseData(-0.0005);
            yield return new TestCaseData(1.0005);
            yield return new TestCaseData(2);
            yield return new TestCaseData(double.PositiveInfinity);
            yield return new TestCaseData(double.NaN);
        }
    }
}
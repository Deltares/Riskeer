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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class NonNonAdoptableWithProfileProbabilityFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<NonAdoptableFailureMechanismSectionResult>(sectionResult);
            Assert.IsNaN(sectionResult.ManualInitialFailureMechanismResultProfileProbability);
            Assert.IsNaN(sectionResult.RefinedProfileProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void ManualInitialFailureMechanismResultProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => result.ManualInitialFailureMechanismResultProfileProbability = newValue;

            // Assert
            const string message = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [TestCase(0.2)]
        [TestCase(0.1 + 1e-5)]
        public void GivenValidSectionResult_WhenSettingInitialProfileProbabilityGreaterThanSectionProbability_ThenArgumentOutOfRangeExceptionThrown(double invalidProbability)
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                ManualInitialFailureMechanismResultSectionProbability = 0.1
            };

            // When
            void Call() => result.ManualInitialFailureMechanismResultProfileProbability = invalidProbability;

            // Then
            const string message = "De faalkans van het initiële mechanisme per doorsnede moet kleiner of gelijk zijn aan de faalkans van het initiële mechanisme per vak.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void ManualInitialFailureMechanismResultSectionProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => result.ManualInitialFailureMechanismResultSectionProbability = newValue;

            // Assert
            const string message = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        [TestCase(double.NaN)]
        public void ManualInitialFailureMechanismResultSectionProbability_ValidValue_NewValueSet(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            result.ManualInitialFailureMechanismResultSectionProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.ManualInitialFailureMechanismResultSectionProbability);
        }

        [Test]
        [TestCase(0.2)]
        [TestCase(0.3 - 1e-5)]
        public void GivenValidSectionResult_WhenSettingInitialSectionProbabilitySmallerThanProfileProbability_ThenArgumentOutOfRangeExceptionThrown(double invalidProbability)
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                ManualInitialFailureMechanismResultProfileProbability = 0.3
            };

            // When
            void Call() => result.ManualInitialFailureMechanismResultSectionProbability = invalidProbability;

            // Then
            const string message = "De faalkans van het initiële mechanisme per doorsnede moet kleiner of gelijk zijn aan de faalkans van het initiële mechanisme per vak.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        [TestCase(double.NaN)]
        public void ManualInitialFailureMechanismResultProfileProbability_ValidValue_NewValueSet(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            result.ManualInitialFailureMechanismResultProfileProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.ManualInitialFailureMechanismResultProfileProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void RefinedProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => result.RefinedProfileProbability = newValue;

            // Assert
            const string message = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void RefinedSectionProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => result.RefinedSectionProbability = newValue;

            // Assert
            const string message = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        [TestCase(double.NaN)]
        public void RefinedSectionProbability_ValidValue_NewValueSet(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            result.RefinedSectionProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.RefinedSectionProbability);
        }

        [Test]
        [TestCase(0.2)]
        [TestCase(0.1 + 1e-5)]
        public void GivenValidSectionResult_WhenSettingRefinedProfileProbabilityGreaterThanSectionProbability_ThenArgumentOutOfRangeExceptionThrown(double invalidProbability)
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                RefinedSectionProbability = 0.1
            };

            // When
            void Call() => result.RefinedProfileProbability = invalidProbability;

            // Then
            const string message = "De aangescherpte faalkans per doorsnede moet kleiner of gelijk zijn aan de aangescherpte faalkans per vak.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [TestCase(0.2)]
        [TestCase(0.3 - 1e-5)]
        public void GivenValidSectionResult_WhenSettingRefinedSectionProbabilitySmallerThanProfileProbability_ThenArgumentOutOfRangeExceptionThrown(double invalidProbability)
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                RefinedProfileProbability = 0.3
            };

            // When
            void Call() => result.RefinedSectionProbability = invalidProbability;

            // Then
            const string message = "De aangescherpte faalkans per doorsnede moet kleiner of gelijk zijn aan de aangescherpte faalkans per vak.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        [TestCase(double.NaN)]
        public void RefinedProfileProbability_ValidValue_NewValueSet(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            result.RefinedProfileProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.RefinedProfileProbability);
        }
    }
}
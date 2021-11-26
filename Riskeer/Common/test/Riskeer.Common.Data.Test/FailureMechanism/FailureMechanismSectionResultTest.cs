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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<Observable>(sectionResult);
            Assert.AreSame(section, sectionResult.Section);
            Assert.IsTrue(sectionResult.IsRelevant);
            Assert.AreEqual(InitialFailureMechanismResultType.Adopt, sectionResult.InitialFailureMechanismResult);
            Assert.IsNaN(sectionResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.IsFalse(sectionResult.FurtherAnalysisNeeded);
            Assert.IsNaN(sectionResult.RefinedSectionProbability);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestFailureMechanismSectionResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
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
            var result = new TestFailureMechanismSectionResult(section);

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
            var result = new TestFailureMechanismSectionResult(section);

            // Call
            result.ManualInitialFailureMechanismResultSectionProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.ManualInitialFailureMechanismResultSectionProbability);
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
            var result = new TestFailureMechanismSectionResult(section);

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
            var result = new TestFailureMechanismSectionResult(section);

            // Call
            result.RefinedSectionProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.RefinedSectionProbability);
        }

        private class TestFailureMechanismSectionResult : FailureMechanismSectionResult
        {
            public TestFailureMechanismSectionResult(FailureMechanismSection section) : base(section) {}
        }
    }
}
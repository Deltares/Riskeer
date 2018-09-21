// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(sectionResult);
            Assert.IsNull(sectionResult.Calculation);
            Assert.AreEqual(SimpleAssessmentResultType.None, sectionResult.SimpleAssessmentResult);
            Assert.AreEqual(DetailedAssessmentProbabilityOnlyResultType.Probability, sectionResult.DetailedAssessmentResult);
            Assert.AreEqual(TailorMadeAssessmentProbabilityCalculationResultType.None, sectionResult.TailorMadeAssessmentResult);
            Assert.IsNaN(sectionResult.TailorMadeAssessmentProbability);
            Assert.AreSame(section, sectionResult.Section);
            Assert.IsFalse(sectionResult.UseManualAssembly);
            Assert.IsNaN(sectionResult.ManualAssemblyProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(1.1)]
        [TestCase(-0.1)]
        public void TailorMadeAssessmentProbability_SetInvalidValue_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            TestDelegate call = () => sectionResult.TailorMadeAssessmentProbability = invalidValue;

            // Assert
            const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        public void TailorMadeAssessmentProbability_SetValidValue_SetsValue(double validValue)
        {
            // Setup
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            sectionResult.TailorMadeAssessmentProbability = validValue;

            // Assert
            Assert.AreEqual(validValue, sectionResult.TailorMadeAssessmentProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void ManualAssemblyProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new HeightStructuresFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => result.ManualAssemblyProbability = newValue;

            // Assert
            const string message = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        [TestCase(double.NaN)]
        public void ManualAssemblyProbability_ValidValue_NewValueSet(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new HeightStructuresFailureMechanismSectionResult(section);

            // Call
            result.ManualAssemblyProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.ManualAssemblyProbability);
        }
    }
}
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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.Test.StandAlone.SectionResults
{
    [TestFixture]
    public class MacroStabilityOutwardsFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(result);
            Assert.AreSame(section, result.Section);
            Assert.AreEqual(SimpleAssessmentResultType.None, result.SimpleAssessmentResult);
            Assert.AreEqual(DetailedAssessmentResultType.Probability, result.DetailedAssessmentResult);
            Assert.IsNaN(result.DetailedAssessmentProbability);
            Assert.AreEqual(TailorMadeAssessmentResultType.None, result.TailorMadeAssessmentResult);
            Assert.IsNaN(result.TailorMadeAssessmentProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void DetailedAssessmentProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => result.DetailedAssessmentProbability = newValue;

            // Assert
            string message = Assert.Throws<ArgumentOutOfRangeException>(test).Message;
            const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        [TestCase(double.NaN)]
        public void DetailedAssessmentProbability_ValidValue_NewValueSet(double newValue)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(section);

            // Call
            result.DetailedAssessmentProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.DetailedAssessmentProbability);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(5)]
        [TestCase(0.5)]
        public void TailorMadeAssessmentProbability_SetNewValue_ReturnsNewValue(double newValue)
        {
            // Setup
            var failureMechanismSectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            failureMechanismSectionResult.TailorMadeAssessmentProbability = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismSectionResult.TailorMadeAssessmentProbability,
                            failureMechanismSectionResult.TailorMadeAssessmentProbability.GetAccuracy());
        }
    }
}
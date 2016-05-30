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
using Core.Common.TestUtil;
using NUnit.Framework;

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
            Assert.AreEqual(1.0, pipingProbabilityAssessmentInput.A);
            Assert.AreEqual(350.0, pipingProbabilityAssessmentInput.B);

            Assert.IsNaN(pipingProbabilityAssessmentInput.SectionLength);
            Assert.AreEqual(0, pipingProbabilityAssessmentInput.Norm);
            Assert.IsNaN(pipingProbabilityAssessmentInput.Contribution);
        }

        [Test]
        [TestCase(0)]
        [TestCase(45.67)]
        [TestCase(100)]
        public void Contribution_SetNewValidValue_GetNewValue(double newContributionValue)
        {
            // Setup
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            pipingProbabilityAssessmentInput.Contribution = newContributionValue;

            // Assert
            Assert.AreEqual(newContributionValue, pipingProbabilityAssessmentInput.Contribution);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-123.545)]
        [TestCase(100 + 1e-6)]
        [TestCase(5678.9)]
        public void Contribution_SetNewInvalidValue_ThrowArgumentOutOfRangeException(double newContributionValue)
        {
            // Setup
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();

            // Call
            TestDelegate call = () => pipingProbabilityAssessmentInput.Contribution = newContributionValue;

            // Assert
            const string expectedMessage = "De waarde voor de toegestane bijdrage aan faalkans moet in interval [0,100] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }
    }
}
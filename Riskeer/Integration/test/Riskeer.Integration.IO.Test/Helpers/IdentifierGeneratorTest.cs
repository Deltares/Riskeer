// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class IdentifierGeneratorTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetNewId_InvalidPrefix_ThrowsArgumentException(string invalidPrefix)
        {
            // Setup
            var generator = new IdentifierGenerator();

            // Call
            TestDelegate call = () => generator.GetNewId(invalidPrefix);

            // Assert
            const string expectedMessage = "'prefix' is null, empty or consists of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void GetNewId_WithPrefix_ReturnsExpectedValue()
        {
            // Setup
            const string prefix = "prefix";
            var generator = new IdentifierGenerator();

            // Call
            string id = generator.GetNewId(prefix);

            // Assert
            Assert.AreEqual($"{prefix}.0", id);
        }

        [Test]
        public void GetNewId_PrefixAlreadyUsed_ReturnsExpectedValue()
        {
            // Setup
            const string prefix = "prefix";
            var generator = new IdentifierGenerator();
            string currentId = generator.GetNewId(prefix);

            // Precondition
            Assert.AreEqual($"{prefix}.0", currentId);

            // Call
            string generatedId = generator.GetNewId(prefix);

            // Assert
            Assert.AreEqual($"{prefix}.1", generatedId);
        }

        [Test]
        public void GetNewId_NewPrefix_ReturnsExpectedValues()
        {
            // Given
            const string prefix = "prefix";
            var generator = new IdentifierGenerator();

            // Precondition
            Assert.AreEqual($"{prefix}.0", generator.GetNewId(prefix));

            const string newPrefix = "NewPrefix";

            // When
            string newPrefixId = generator.GetNewId(newPrefix);

            // Then
            Assert.AreEqual($"{newPrefix}.0", newPrefixId);
        }

        [Test]
        public void GivenIdGenerator_WhenMultiplePrefixesUsed_ThenReturnsExpectedValues()
        {
            // Given
            const string prefix = "prefix";
            var generator = new IdentifierGenerator();

            // Precondition
            Assert.AreEqual($"{prefix}.0", generator.GetNewId(prefix));

            const string newPrefix = "NewPrefix";

            // When
            string oldPrefixId = generator.GetNewId(prefix);
            string newPrefixId = generator.GetNewId(newPrefix);

            // Then
            Assert.AreEqual($"{prefix}.1", oldPrefixId);
            Assert.AreEqual($"{newPrefix}.0", newPrefixId);
        }

        [Test]
        public void GenerateId_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => IdentifierGenerator.GeneratedId(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GenerateId_WithAssessmentSection_GeneratesId()
        {
            // Setup
            const string assessmentSectionId = "AssessmentSectionId";
            ExportableAssessmentSection assessmentSection = CreateAssessmentSection(assessmentSectionId);

            // Call
            string generatedId = IdentifierGenerator.GeneratedId(assessmentSection);

            // Assert
            Assert.AreEqual($"Wks.{assessmentSection.Id}", generatedId);
        }

        private static ExportableAssessmentSection CreateAssessmentSection(string id)
        {
            return new ExportableAssessmentSection(string.Empty,
                                                   id,
                                                   Enumerable.Empty<Point2D>(),
                                                   ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                                                   ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithProbability(),
                                                   ExportableFailureMechanismAssemblyResultTestFactory.CreateResultWithoutProbability(),
                                                   Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                   Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                   Enumerable.Empty<ExportableCombinedSectionAssembly>());
        }
    }
}
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
using Ringtoets.Integration.IO.Helpers;

namespace Ringtoets.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class UniqueIdentifierGeneratorTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetNewId_InvalidPrefix_ThrowsArgumentException(string invalidPrefix)
        {
            // Setup
            var generator = new UniqueIdentifierGenerator();

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
            var generator = new UniqueIdentifierGenerator();

            // Call
            string id = generator.GetNewId(prefix);

            // Assert
            Assert.AreEqual($"{prefix}.0", id);
        }

        [Test]
        public void GivenGeneratedId_WhenGetNewIdCalledWithSamePrefix_ThenNewIdGenerated()
        {
            // Given
            const string prefix = "prefix";
            var generator = new UniqueIdentifierGenerator();
            string currentId = generator.GetNewId(prefix);

            // Precondition
            Assert.AreEqual($"{prefix}.0", currentId);

            // When
            string[] generatedIds =
            {
                generator.GetNewId(prefix),
                generator.GetNewId(prefix)
            };

            // Then
            CollectionAssert.AreEqual(new[]
            {
                $"{prefix}.1",
                $"{prefix}.2"
            }, generatedIds);
        }

        [Test]
        public void GivenGeneratedId_WhenGetNewIdCalledWithDifferentPrefix_ThenNewIdGenerated()
        {
            // Given
            const string prefix = "prefix";
            var generator = new UniqueIdentifierGenerator();
            generator.GetNewId(prefix);

            // Precondition
            Assert.AreEqual($"{prefix}.1", generator.GetNewId(prefix));

            const string newPrefix = "NewPrefix";

            // When
            string newPrefixId = generator.GetNewId(newPrefix);

            // Then
            Assert.AreEqual($"{newPrefix}.0", newPrefixId);
        }
    }
}
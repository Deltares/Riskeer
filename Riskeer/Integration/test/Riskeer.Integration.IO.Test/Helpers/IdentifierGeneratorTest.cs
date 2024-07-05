﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class IdentifierGeneratorTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetUniqueId_InvalidPrefix_ThrowsArgumentException(string invalidPrefix)
        {
            // Setup
            var generator = new IdentifierGenerator();

            // Call
            void Call() => generator.GetUniqueId(invalidPrefix);

            // Assert
            const string expectedMessage = "'prefix' is null, empty or consists of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void GetUniqueId_WithPrefix_ReturnsExpectedValue()
        {
            // Setup
            const string prefix = "prefix";
            var generator = new IdentifierGenerator();

            // Call
            string id = generator.GetUniqueId(prefix);

            // Assert
            Assert.AreEqual($"{prefix}.0", id);
        }

        [Test]
        public void GetUniqueId_PrefixAlreadyUsed_ReturnsExpectedValue()
        {
            // Setup
            const string prefix = "prefix";
            var generator = new IdentifierGenerator();
            string currentId = generator.GetUniqueId(prefix);

            // Precondition
            Assert.AreEqual($"{prefix}.0", currentId);

            // Call
            string generatedId = generator.GetUniqueId(prefix);

            // Assert
            Assert.AreEqual($"{prefix}.1", generatedId);
        }

        [Test]
        public void GetUniqueId_NewPrefix_ReturnsExpectedValue()
        {
            // Given
            const string prefix = "prefix";
            var generator = new IdentifierGenerator();

            // Precondition
            Assert.AreEqual($"{prefix}.0", generator.GetUniqueId(prefix));

            const string newPrefix = "NewPrefix";

            // When
            string newPrefixId = generator.GetUniqueId(newPrefix);

            // Then
            Assert.AreEqual($"{newPrefix}.0", newPrefixId);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GenerateId_InvalidPrefix_ThrowsArgumentException(string invalidPrefix)
        {
            // Call
            void Call() => IdentifierGenerator.GenerateId(invalidPrefix, "id");

            // Assert
            const string expectedMessage = "'prefix' is null, empty or consists of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GenerateId_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            void Call() => IdentifierGenerator.GenerateId("prefix", invalidId);

            // Assert
            const string expectedMessage = "'id' is null, empty or consists of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void GenerateId_WithValidArguments_GeneratesId()
        {
            // Setup
            const string prefix = "prefix";
            const string id = "id";

            // Call
            string generatedId = IdentifierGenerator.GenerateId(prefix, id);

            // Assert
            Assert.AreEqual($"{prefix}.{id}", generatedId);
        }
    }
}
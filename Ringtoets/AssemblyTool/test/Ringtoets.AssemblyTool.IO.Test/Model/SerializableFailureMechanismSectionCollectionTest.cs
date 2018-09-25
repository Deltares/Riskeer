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
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableFailureMechanismSectionCollectionTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var sections = new SerializableFailureMechanismSectionCollection();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(sections);
            Assert.IsNull(sections.Id);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableFailureMechanismSectionCollection), "Vakindeling");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionCollection>(
                nameof(SerializableFailureMechanismSectionCollection.Id), "VakindelingID");
        }

        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_WithInvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionCollection(invalidId);

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_WithValidId_ReturnsExpectedValues()
        {
            // Setup
            const string id = "collectionId";

            // Call
            var collection = new SerializableFailureMechanismSectionCollection(id);

            // Assert
            Assert.AreEqual(id, collection.Id);
        }
    }
}
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
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableFailureMechanismSectionsTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var sections = new SerializableFailureMechanismSections();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(sections);
            Assert.IsNull(sections.Id);
            Assert.IsNull(sections.FailureMechanismId);

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSections>(
                nameof(SerializableFailureMechanismSections.Id), "VakindelingID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSections>(
                nameof(SerializableFailureMechanismSections.FailureMechanismId), "ToetsspoorIDRef");
        }

        [Test]
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSections(null,
                                                                               new SerializableFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("id", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSections("id",
                                                                               null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "section id";

            var failureMechanism = new SerializableFailureMechanism();

            // Call
            var sections = new SerializableFailureMechanismSections(id,
                                                                    failureMechanism);

            // Assert
            Assert.AreEqual(id, sections.Id);
            Assert.AreEqual(failureMechanism.Id, failureMechanism.Id);
        }
    }
}
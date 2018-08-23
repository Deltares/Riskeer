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
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
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
            Assert.IsNull(sections.FailureMechanismId);
            Assert.IsNull(sections.TotalAssemblyResultId);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableFailureMechanismSectionCollection), "Vakindeling");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionCollection>(
                nameof(SerializableFailureMechanismSectionCollection.Id), "VakindelingID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionCollection>(
                nameof(SerializableFailureMechanismSectionCollection.FailureMechanismId), "ToetsspoorIDRef");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionCollection>(
                nameof(SerializableFailureMechanismSectionCollection.TotalAssemblyResultId), "VeiligheidsoordeelIDRef");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void ConstructorWithFailureMechanism_IdInvalid_ThrowsArgumentException(string id)
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionCollection(id,
                                                                                        new SerializableFailureMechanism());

            // Assert
            const string expectedMessage = "'id' must have a value.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ConstructorWithFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionCollection("id",
                                                                                        (SerializableFailureMechanism) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ConstructorWithFailureMechanism_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "section id";

            var random = new Random(39);
            var failureMechanism = new SerializableFailureMechanism("fm id",
                                                                    new SerializableTotalAssemblyResult(),
                                                                    random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                    random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                    new SerializableFailureMechanismAssemblyResult());

            // Call
            var sections = new SerializableFailureMechanismSectionCollection(id,
                                                                             failureMechanism);

            // Assert
            Assert.AreEqual(id, sections.Id);
            Assert.AreEqual(failureMechanism.Id, sections.FailureMechanismId);
            Assert.IsNull(sections.TotalAssemblyResultId);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void ConstructorWithTotalAssemblyResult_IdInvalid_ThrowsArgumentException(string id)
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionCollection(id,
                                                                                        new SerializableTotalAssemblyResult());

            // Assert
            const string expectedMessage = "'id' must have a value.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ConstructorWithTotalAssemblyResult_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionCollection("id",
                                                                                        (SerializableTotalAssemblyResult) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("totalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void ConstructorWithTotalAssemblyResult_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "section id";

            var totalAssemblyResult = new SerializableTotalAssemblyResult("result id",
                                                                          new SerializableAssessmentProcess(),
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          new SerializableFailureMechanismAssemblyResult(),
                                                                          new SerializableAssessmentSectionAssemblyResult());

            // Call
            var sections = new SerializableFailureMechanismSectionCollection(id,
                                                                             totalAssemblyResult);

            // Assert
            Assert.AreEqual(id, sections.Id);
            Assert.AreEqual(totalAssemblyResult.Id, sections.TotalAssemblyResultId);
            Assert.IsNull(sections.FailureMechanismId);
        }
    }
}
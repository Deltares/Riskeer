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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var failureMechanism = new SerializableFailureMechanism();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(failureMechanism);
            Assert.IsNull(failureMechanism.Id);
            Assert.IsNull(failureMechanism.TotalAssemblyResultId);
            Assert.AreEqual((SerializableFailureMechanismType) 0, failureMechanism.FailureMechanismType);
            Assert.IsNull(failureMechanism.GenericFailureMechanismCode);
            Assert.IsNull(failureMechanism.SpecificFailureMechanismName);
            Assert.IsNull(failureMechanism.FailureMechanismAssemblyResult);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableFailureMechanism), "Faalmechanisme");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.Id), "FaalmechanismeID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.TotalAssemblyResultId), "VeiligheidsoordeelIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.FailureMechanismType), "typeFaalmechanisme");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.GenericFailureMechanismCode), "generiekFaalmechanisme");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.SpecificFailureMechanismName), "specifiekFaalmechanisme");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.FailureMechanismAssemblyResult), "analyseFaalmechanisme");
        }

        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new SerializableFailureMechanism(invalidId,
                                                            random.NextEnumValue<SerializableFailureMechanismType>(),
                                                            "code",
                                                            "name",
                                                            new SerializableTotalAssemblyResult(),
                                                            new SerializableFailureMechanismAssemblyResult());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new SerializableFailureMechanism("id", random.NextEnumValue<SerializableFailureMechanismType>(),
                                                            "code", "name", null, new SerializableFailureMechanismAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("totalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new SerializableFailureMechanism("id", random.NextEnumValue<SerializableFailureMechanismType>(),
                                                            "code", "name", new SerializableTotalAssemblyResult(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);

            const string id = "sectionId";
            var failureMechanismType = random.NextEnumValue<SerializableFailureMechanismType>();
            const string code = "code";
            const string name = "name";
            const string totalResultId = "totalResultId";
            var assemblyResult = new SerializableFailureMechanismAssemblyResult();

            // Call
            var failureMechanism = new SerializableFailureMechanism(
                id, failureMechanismType, code, name,
                new SerializableTotalAssemblyResult(
                    totalResultId, new SerializableAssessmentProcess(),
                    random.NextEnumValue<SerializableAssemblyMethod>(),
                    random.NextEnumValue<SerializableAssessmentSectionAssemblyGroup>(),
                    random.NextDouble()),
                assemblyResult);

            // Assert
            Assert.AreEqual(id, failureMechanism.Id);
            Assert.AreEqual(totalResultId, failureMechanism.TotalAssemblyResultId);
            Assert.AreEqual(failureMechanismType, failureMechanism.FailureMechanismType);
            Assert.AreEqual(code, failureMechanism.GenericFailureMechanismCode);
            Assert.AreEqual(name, failureMechanism.SpecificFailureMechanismName);
            Assert.AreSame(assemblyResult, failureMechanism.FailureMechanismAssemblyResult);
        }

        [Test]
        [TestCase(SerializableFailureMechanismType.Generic, true, false)]
        [TestCase(SerializableFailureMechanismType.Specific, false, true)]
        public void GivenSerializableFailureMechanismWithFailureMechanismType_WhenShouldSerializeProperties_ThenReturnsExpectedValues(
            SerializableFailureMechanismType failureMechanismType, bool expectedShouldSerializeGeneric, bool expectedShouldSerializeSpecific)
        {
            // Given
            var failureMechanism = new SerializableFailureMechanism(
                "id", failureMechanismType, "code", "name", new SerializableTotalAssemblyResult(),
                new SerializableFailureMechanismAssemblyResult());

            // When
            bool shouldSerializeGeneric = failureMechanism.ShouldSerializeGenericFailureMechanismCode();
            bool shouldSerializeSpecific = failureMechanism.ShouldSerializeSpecificFailureMechanismName();

            // Then
            Assert.AreEqual(expectedShouldSerializeGeneric, shouldSerializeGeneric);
            Assert.AreEqual(expectedShouldSerializeSpecific, shouldSerializeSpecific);
        }
    }
}
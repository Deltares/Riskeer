// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.AssemblyTool.IO.TestUtil;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;

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
            Assert.AreEqual("DIRECT", failureMechanism.DirectFailureMechanism);
            Assert.IsNull(failureMechanism.Id);
            Assert.IsNull(failureMechanism.TotalAssemblyResultId);
            Assert.IsNull(failureMechanism.FailureMechanismAssemblyResult);
            Assert.AreEqual((SerializableFailureMechanismGroup) 0, failureMechanism.FailureMechanismGroup);
            Assert.AreEqual((SerializableFailureMechanismType) 0, failureMechanism.FailureMechanismType);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableFailureMechanism), "Toetsspoor");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.Id), "ToetsspoorID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.TotalAssemblyResultId), "VeiligheidsoordeelIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.FailureMechanismType), "typeToetsspoor");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.FailureMechanismGroup), "toetsspoorGroep");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.DirectFailureMechanism), "typeFaalmechanisme");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanism>(
                nameof(SerializableFailureMechanism.FailureMechanismAssemblyResult), "toetsoordeel");
        }

        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanism(invalidId,
                                                                       new SerializableTotalAssemblyResult(),
                                                                       random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                       random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                       new SerializableFailureMechanismAssemblyResult());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_TotalAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanism("id",
                                                                       null,
                                                                       random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                       random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                       new SerializableFailureMechanismAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("totalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanism("id",
                                                                       new SerializableTotalAssemblyResult(),
                                                                       random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                       random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "sectionId";
            const string totalResultId = "totalResultId";

            var random = new Random(39);
            var type = random.NextEnumValue<SerializableFailureMechanismType>();
            var group = random.NextEnumValue<SerializableFailureMechanismGroup>();
            var assemblyResult = new SerializableFailureMechanismAssemblyResult();

            // Call
            var failureMechanism = new SerializableFailureMechanism(id,
                                                                    new SerializableTotalAssemblyResult(totalResultId,
                                                                                                        new SerializableAssessmentProcess(),
                                                                                                        new SerializableFailureMechanismAssemblyResult(),
                                                                                                        new SerializableFailureMechanismAssemblyResult(),
                                                                                                        new SerializableAssessmentSectionAssemblyResult()),
                                                                    type,
                                                                    group,
                                                                    assemblyResult);

            // Assert
            Assert.AreEqual(id, failureMechanism.Id);
            Assert.AreEqual(totalResultId, failureMechanism.TotalAssemblyResultId);
            Assert.AreEqual(type, failureMechanism.FailureMechanismType);
            Assert.AreEqual(group, failureMechanism.FailureMechanismGroup);
            Assert.AreEqual("DIRECT", failureMechanism.DirectFailureMechanism);
            Assert.AreSame(assemblyResult, failureMechanism.FailureMechanismAssemblyResult);
        }
    }
}
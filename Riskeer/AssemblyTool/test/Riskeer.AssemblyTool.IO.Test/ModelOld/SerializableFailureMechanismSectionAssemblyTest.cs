// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.ModelOld;
using Riskeer.AssemblyTool.IO.ModelOld.DataTypes;
using Riskeer.AssemblyTool.IO.ModelOld.Enums;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.ModelOld
{
    [TestFixture]
    public class SerializableFailureMechanismSectionAssemblyTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var sectionAssembly = new SerializableFailureMechanismSectionAssembly();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(sectionAssembly);
            Assert.IsNull(sectionAssembly.Id);
            Assert.IsNull(sectionAssembly.FailureMechanismId);
            Assert.IsNull(sectionAssembly.FailureMechanismSectionId);
            Assert.IsNull(sectionAssembly.SectionResult);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableFailureMechanismSectionAssembly), "Faalanalyse");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.Id), "FaalanalyseID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.FailureMechanismId), "FaalmechanismeIDRef");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.FailureMechanismSectionId), "WaterkeringsectieIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.SectionResult), "analyseVak");
        }

        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            void Call() => new SerializableFailureMechanismSectionAssembly(invalidId, new SerializableFailureMechanism(),
                                                                           new SerializableFailureMechanismSection(),
                                                                           new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SerializableFailureMechanismSectionAssembly("id", null, new SerializableFailureMechanismSection(),
                                                                           new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SerializableFailureMechanismSectionAssembly("id", new SerializableFailureMechanism(),
                                                                           null, new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SerializableFailureMechanismSectionAssembly("id", new SerializableFailureMechanism(),
                                                                           new SerializableFailureMechanismSection(),
                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "id";

            var random = new Random(39);
            var failureMechanism = new SerializableFailureMechanism(
                "failureMechanismID", random.NextEnumValue<SerializableFailureMechanismType>(),
                "code", "name", new SerializableTotalAssemblyResult(), new SerializableFailureMechanismAssemblyResult());
            var section = new SerializableFailureMechanismSection("sectionID",
                                                                  new SerializableFailureMechanismSectionCollection(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble(),
                                                                  new[]
                                                                  {
                                                                      new Point2D(random.NextDouble(), random.NextDouble())
                                                                  },
                                                                  SerializableFailureMechanismSectionType.FailureMechanism);
            var sectionResult = new SerializableFailureMechanismSectionAssemblyResult();

            // Call
            var sectionAssembly = new SerializableFailureMechanismSectionAssembly(
                id, failureMechanism, section, sectionResult);

            // Assert
            Assert.AreEqual(id, sectionAssembly.Id);
            Assert.AreEqual(failureMechanism.Id, sectionAssembly.FailureMechanismId);
            Assert.AreEqual(section.Id, sectionAssembly.FailureMechanismSectionId);
            Assert.AreSame(sectionResult, sectionAssembly.SectionResult);
        }
    }
}
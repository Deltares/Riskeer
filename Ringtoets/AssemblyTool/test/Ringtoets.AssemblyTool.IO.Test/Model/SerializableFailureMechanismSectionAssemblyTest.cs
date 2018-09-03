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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableFailureMechanismSectionAssemblyTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var sectionAssembly = new SerializableFailureMechanismSectionAssembly();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(sectionAssembly);
            Assert.IsNull(sectionAssembly.Id);
            Assert.IsNull(sectionAssembly.FailureMechanismId);
            Assert.IsNull(sectionAssembly.FailureMechanismSectionId);
            Assert.IsNull(sectionAssembly.CombinedSectionResult);
            Assert.IsNull(sectionAssembly.SectionResults);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableFailureMechanismSectionAssembly), "Toets");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.Id), "ToetsID");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.FailureMechanismId), "ToetsspoorIDRef");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.FailureMechanismSectionId), "WaterkeringsectieIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.CombinedSectionResult), "eindtoetsoordeel");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSectionAssembly>(
                nameof(SerializableFailureMechanismSectionAssembly.SectionResults), "toetsoordeelVak");
        }

        [Test]
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly(null,
                                                                                      new SerializableFailureMechanism(),
                                                                                      new SerializableFailureMechanismSection(),
                                                                                      new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                      new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("id", exception.ParamName);
        }

        [Test]
        [TestCase(" ")]
        [TestCase("")]
        [TestCase(" InvalidId")]
        public void Constructor_InvalidId_ThrowsArgumentNullException(string invalidId)
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly(invalidId,
                                                                                      new SerializableFailureMechanism(),
                                                                                      new SerializableFailureMechanismSection(),
                                                                                      new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                      new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly("id",
                                                                                      null,
                                                                                      new SerializableFailureMechanismSection(),
                                                                                      new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                      new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly("id",
                                                                                      new SerializableFailureMechanism(),
                                                                                      null,
                                                                                      new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                      new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly("id",
                                                                                      new SerializableFailureMechanism(),
                                                                                      new SerializableFailureMechanismSection(),
                                                                                      null,
                                                                                      new SerializableFailureMechanismSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableFailureMechanismSectionAssembly("id",
                                                                                      new SerializableFailureMechanism(),
                                                                                      new SerializableFailureMechanismSection(),
                                                                                      new SerializableFailureMechanismSectionAssemblyResult[0],
                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionResult", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "id";

            var random = new Random(39);
            var failureMechanism = new SerializableFailureMechanism("failureMechanismID",
                                                                    new SerializableTotalAssemblyResult(),
                                                                    random.NextEnumValue<SerializableFailureMechanismType>(),
                                                                    random.NextEnumValue<SerializableFailureMechanismGroup>(),
                                                                    new SerializableFailureMechanismAssemblyResult());
            var section = new SerializableFailureMechanismSection("sectionID",
                                                                  new SerializableFailureMechanismSectionCollection(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble(),
                                                                  new[]
                                                                  {
                                                                      new Point2D(random.NextDouble(), random.NextDouble())
                                                                  },
                                                                  SerializableFailureMechanismSectionType.FailureMechanism);
            var sectionResults = new SerializableFailureMechanismSectionAssemblyResult[0];
            var combinedSectionResult = new SerializableFailureMechanismSectionAssemblyResult();

            // Call
            var sectionAssembly = new SerializableFailureMechanismSectionAssembly(id,
                                                                                  failureMechanism,
                                                                                  section,
                                                                                  sectionResults,
                                                                                  combinedSectionResult);

            // Assert
            Assert.AreEqual(id, sectionAssembly.Id);
            Assert.AreEqual(failureMechanism.Id, sectionAssembly.FailureMechanismId);
            Assert.AreEqual(section.Id, sectionAssembly.FailureMechanismSectionId);
            Assert.AreSame(sectionResults, sectionAssembly.SectionResults);
            Assert.AreSame(combinedSectionResult, sectionAssembly.CombinedSectionResult);
        }
    }
}
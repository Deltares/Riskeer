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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.AssemblyTool.IO.Model.Helpers;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableFailureMechanismSectionTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var section = new SerializableFailureMechanismSection();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(section);
            Assert.IsNull(section.Id);
            Assert.IsNull(section.FailureMechanismSectionCollectionId);
            Assert.IsNull(section.StartDistance);
            Assert.IsNull(section.EndDistance);
            Assert.IsNull(section.Geometry);
            Assert.AreEqual((SerializableFailureMechanismSectionType) 0, section.FailureMechanismSectionType);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableFailureMechanismSection), "ToetsVak");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.Id), "id", "http://www.opengis.net/gml/3.2");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.FailureMechanismSectionCollectionId), "VakindelingIDRef");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.StartDistance), "afstandBegin");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.EndDistance), "afstandEinde");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.Geometry), "geometrieLijn2D");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.Length), "lengte");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.FailureMechanismSectionType), "typeWaterkeringsectie");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.AssemblyMethod), "assemblagemethode");
        }

        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanismSection(invalidId,
                                                                              new SerializableFailureMechanismSectionCollection(),
                                                                              random.NextDouble(),
                                                                              random.NextDouble(),
                                                                              Enumerable.Empty<Point2D>(),
                                                                              random.NextEnumValue<SerializableFailureMechanismSectionType>());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanismSection("id",
                                                                              null,
                                                                              random.NextDouble(),
                                                                              random.NextDouble(),
                                                                              Enumerable.Empty<Point2D>(),
                                                                              random.NextEnumValue<SerializableFailureMechanismSectionType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionCollection", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanismSection("id",
                                                                              new SerializableFailureMechanismSectionCollection(),
                                                                              random.NextDouble(),
                                                                              random.NextDouble(),
                                                                              null,
                                                                              random.NextEnumValue<SerializableFailureMechanismSectionType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "sectionId";

            var random = new Random(39);
            var sectionCollection = new SerializableFailureMechanismSectionCollection("sectionCollectionId");
            double startDistance = random.NextDouble();
            double endDistance = random.NextDouble();
            var assemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var sectionType = random.NextEnumValue<SerializableFailureMechanismSectionType>();
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            var section = new SerializableFailureMechanismSection(id,
                                                                  sectionCollection,
                                                                  startDistance,
                                                                  endDistance,
                                                                  geometry,
                                                                  sectionType,
                                                                  assemblyMethod);

            // Assert
            Assert.AreEqual(id, section.Id);
            Assert.AreEqual(sectionCollection.Id, section.FailureMechanismSectionCollectionId);
            Assert.AreEqual(startDistance, section.StartDistance.Value);
            Assert.AreEqual(endDistance, section.EndDistance.Value);
            Assert.AreEqual(GeometrySerializationFormatter.Format(geometry), section.Geometry.LineString.Geometry);
            Assert.AreEqual(Math2D.Length(geometry), section.Length.Value);
            Assert.AreEqual(assemblyMethod, section.AssemblyMethod);
            Assert.AreEqual(sectionType, section.FailureMechanismSectionType);
        }

        [Test]
        [TestCase(SerializableAssemblyMethod.WBI0A1, true)]
        [TestCase(null, false)]
        public void ShouldSerializeAssemblyMethod_WithAssemblyMethodValues_ReturnsExpectedValue(SerializableAssemblyMethod? assemblyMethod, bool expectedShouldSerialize)
        {
            // Setup
            var random = new Random(39);
            var section = new SerializableFailureMechanismSection("id",
                                                                  new SerializableFailureMechanismSectionCollection(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble(),
                                                                  new[]
                                                                  {
                                                                      new Point2D(random.NextDouble(), random.NextDouble())
                                                                  },
                                                                  random.NextEnumValue<SerializableFailureMechanismSectionType>(),
                                                                  assemblyMethod);

            // Call
            bool shouldSerialize = section.ShouldSerializeAssemblyMethod();

            // Assert
            Assert.AreEqual(expectedShouldSerialize, shouldSerialize);
        }
    }
}
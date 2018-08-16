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
            Assert.IsNull(section.FailureMechanismSectionsId);
            Assert.IsNull(section.StartDistance);
            Assert.IsNull(section.EndDistance);
            Assert.IsNull(section.Geometry);
            Assert.AreEqual("TOETSSSTE", section.FailureMechanismSectionType);

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.Id), "id", "http://www.opengis.net/gml/3.2");
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableFailureMechanismSection>(
                nameof(SerializableFailureMechanismSection.FailureMechanismSectionsId), "VakindelingIDRef");

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
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanismSection(null,
                                                                              new SerializableFailureMechanismSections(),
                                                                              random.NextDouble(),
                                                                              random.NextDouble(),
                                                                              Enumerable.Empty<Point2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("id", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanismSection("id",
                                                                              null,
                                                                              random.NextDouble(),
                                                                              random.NextDouble(),
                                                                              Enumerable.Empty<Point2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSections", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableFailureMechanismSection("id",
                                                                              new SerializableFailureMechanismSections(),
                                                                              random.NextDouble(),
                                                                              random.NextDouble(),
                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "section id";

            var random = new Random(39);
            var sections = new SerializableFailureMechanismSections("sections id", new SerializableFailureMechanism());
            double startDistance = random.NextDouble();
            double endDistance = random.NextDouble();
            var assemblyMethod = random.NextEnumValue<SerializableAssemblyMethod>();
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            var section = new SerializableFailureMechanismSection(id,
                                                                  sections,
                                                                  startDistance,
                                                                  endDistance,
                                                                  geometry,
                                                                  assemblyMethod);

            // Assert
            Assert.AreEqual(id, section.Id);
            Assert.AreEqual(sections.Id, section.FailureMechanismSectionsId);
            Assert.AreEqual("m", section.StartDistance.UnitOfMeasure);
            Assert.AreEqual(startDistance, section.StartDistance.Value);
            Assert.AreEqual("m", section.EndDistance.UnitOfMeasure);
            Assert.AreEqual(endDistance, section.EndDistance.Value);
            Assert.AreEqual(GeometrySerializationFormatter.Format(geometry), section.Geometry.LineString.Geometry);
            Assert.AreEqual("m", section.Length.UnitOfMeasure);
            Assert.AreEqual(Math2D.Length(geometry), section.Length.Value);
            Assert.AreEqual("TOETSSSTE", section.FailureMechanismSectionType);
            Assert.AreEqual(assemblyMethod, section.AssemblyMethod);
        }

        [Test]
        [TestCase(SerializableAssemblyMethod.WBI0A1, true)]
        [TestCase(null, false)]
        public void ShouldSerializeAssemblyMethod_WithAssemblyMethodValues_ReturnsExpectedValue(SerializableAssemblyMethod? assemblyMethod, bool expectedShouldSerialize)
        {
            // Setup
            var random = new Random(39);
            var section = new SerializableFailureMechanismSection("id",
                                                                  new SerializableFailureMechanismSections(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble(),
                                                                  new[]
                                                                  {
                                                                      new Point2D(random.NextDouble(), random.NextDouble())
                                                                  },
                                                                  assemblyMethod);

            // Call
            bool shouldSerialize = section.ShouldSerializeAssemblyMethod();

            // Assert
            Assert.AreEqual(expectedShouldSerialize, shouldSerialize);
        }
    }
}
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.TestUtil;
using Riskeer.AssemblyTool.IO.Model;

namespace Riskeer.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableAssessmentSectionTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var assessmentSection = new SerializableAssessmentSection();

            // Assert
            Assert.IsInstanceOf<SerializableFeatureMember>(assessmentSection);
            Assert.AreEqual("DKTRJCT", assessmentSection.AssessmentSectionType);
            Assert.IsNull(assessmentSection.Id);
            Assert.IsNull(assessmentSection.Name);
            Assert.IsNull(assessmentSection.ReferenceLineGeometry);
            Assert.IsNull(assessmentSection.ReferenceLineLength);

            SerializableAttributeTestHelper.AssertXmlTypeAttribute(typeof(SerializableAssessmentSection), "Waterkeringstelsel");

            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableAssessmentSection>(
                nameof(SerializableAssessmentSection.Id), "id", "http://www.opengis.net/gml/3.2");

            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentSection>(
                nameof(SerializableAssessmentSection.AssessmentSectionType), "typeWaterkeringstelsel");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentSection>(
                nameof(SerializableAssessmentSection.Name), "naam");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentSection>(
                nameof(SerializableAssessmentSection.ReferenceLineLength), "lengte");
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssessmentSection>(
                nameof(SerializableAssessmentSection.ReferenceLineGeometry), "geometrie2D");
        }

        [Test]
        [TestCaseSource(typeof(InvalidIdTestHelper), nameof(InvalidIdTestHelper.InvalidIdCases))]
        public void Constructor_InvalidId_ThrowsArgumentException(string invalidId)
        {
            // Call
            TestDelegate call = () => new SerializableAssessmentSection(invalidId,
                                                                        "name",
                                                                        Enumerable.Empty<Point2D>());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableAssessmentSection("id",
                                                                        null,
                                                                        Enumerable.Empty<Point2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SerializableAssessmentSection("id",
                                                                        "name",
                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string name = "section name";
            const string id = "sectionId";

            var random = new Random(39);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            var assessmentSection = new SerializableAssessmentSection(id, name, geometry);

            // Assert
            Assert.AreEqual(id, assessmentSection.Id);
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(Math2D.Length(geometry), assessmentSection.ReferenceLineLength.Value);
            Assert.IsNotNull(assessmentSection.ReferenceLineGeometry);
            Assert.AreEqual("DKTRJCT", assessmentSection.AssessmentSectionType);
        }
    }
}
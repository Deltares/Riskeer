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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.AssemblyTool.IO.TestUtil;

namespace Ringtoets.AssemblyTool.IO.Test.Model
{
    [TestFixture]
    public class SerializableAssemblyTest
    {
        [Test]
        public void DefaultConstructor_ReturnsDefaultValues()
        {
            // Call
            var assembly = new SerializableAssembly();

            // Assert
            Assert.IsNull(assembly.Id);
            Assert.IsNull(assembly.Boundary);
            Assert.IsNull(assembly.FeatureMembers);

            object[] serializableAttributes = typeof(SerializableAssembly).GetCustomAttributes(typeof(SerializableAttribute), false);
            Assert.AreEqual(1, serializableAttributes.Length);

            var xmlRootAttribute = (XmlRootAttribute) typeof(SerializableAssembly).GetCustomAttributes(typeof(XmlRootAttribute), false).Single();
            Assert.AreEqual("Assemblage", xmlRootAttribute.ElementName);
            Assert.AreEqual("http://localhost/standaarden/assemblage", xmlRootAttribute.Namespace);

            const string gmlNamespace = "http://www.opengis.net/gml/3.2";
            SerializableAttributeTestHelper.AssertXmlAttributeAttribute<SerializableAssembly>(
                nameof(SerializableAssembly.Id), "id", gmlNamespace);
            SerializableAttributeTestHelper.AssertXmlElementAttribute<SerializableAssembly>(
                nameof(SerializableAssembly.Boundary), "boundedBy", gmlNamespace);

            XmlArrayAttribute xmlArrayAttribute = TypeUtils.GetPropertyAttributes<SerializableAssembly, XmlArrayAttribute>(nameof(SerializableAssembly.FeatureMembers)).Single();
            Assert.AreEqual("featureMember", xmlArrayAttribute.ElementName);

            IEnumerable<XmlArrayItemAttribute> xmlArrayItemAttributes = TypeUtils.GetPropertyAttributes<SerializableAssembly, XmlArrayItemAttribute>(nameof(SerializableAssembly.FeatureMembers));
            Assert.AreEqual(8, xmlArrayItemAttributes.Count());
            Assert.AreEqual(typeof(SerializableAssessmentSection), xmlArrayItemAttributes.ElementAt(0).Type);
            Assert.AreEqual(typeof(SerializableAssessmentProcess), xmlArrayItemAttributes.ElementAt(1).Type);
            Assert.AreEqual(typeof(SerializableTotalAssemblyResult), xmlArrayItemAttributes.ElementAt(2).Type);
            Assert.AreEqual(typeof(SerializableFailureMechanism), xmlArrayItemAttributes.ElementAt(3).Type);
            Assert.AreEqual(typeof(SerializableFailureMechanismSectionAssembly), xmlArrayItemAttributes.ElementAt(4).Type);
            Assert.AreEqual(typeof(SerializableCombinedFailureMechanismSectionAssembly), xmlArrayItemAttributes.ElementAt(5).Type);
            Assert.AreEqual(typeof(SerializableFailureMechanismSectionCollection), xmlArrayItemAttributes.ElementAt(6).Type);
            Assert.AreEqual(typeof(SerializableFailureMechanismSection), xmlArrayItemAttributes.ElementAt(7).Type);
        }

        [Test]
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(null,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("id", exception.ParamName);
        }

        [Test]
        public void Constructor_LowerCornerNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               null,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("lowerCorner", exception.ParamName);
        }

        [Test]
        public void Constructor_UpperCornerNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               null,
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("upperCorner", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               null,
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSections", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentProcessesNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               null,
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentProcesses", exception.ParamName);
        }

        [Test]
        public void Constructor_TotalAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               null,
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("totalAssemblyResults", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               null,
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionAssembliesNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               null,
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionAssemblies", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedFailureMechanismSectionAssembliesNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               null,
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedFailureMechanismSectionAssemblies", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionCollectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               null,
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionCollections", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(string.Empty,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               Enumerable.Empty<SerializableAssessmentSection>(),
                                                               Enumerable.Empty<SerializableAssessmentProcess>(),
                                                               Enumerable.Empty<SerializableTotalAssemblyResult>(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               null);
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSections", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ReturnsExpectedValues()
        {
            // Setup
            const string id = "assembly id 1";

            var random = new Random(39);
            var lowerCorner = new Point2D(random.NextDouble(), random.NextDouble());
            var upperCorner = new Point2D(random.NextDouble(), random.NextDouble());
            var assessmentSection = new SerializableAssessmentSection();
            var assessmentProcess = new SerializableAssessmentProcess();
            var totalAssemblyResult = new SerializableTotalAssemblyResult();
            var failureMechanism = new SerializableFailureMechanism();
            var failureMechanismSectionAssembly = new SerializableFailureMechanismSectionAssembly();
            var combinedFailureMechanismSectionAssembly = new SerializableCombinedFailureMechanismSectionAssembly();
            var failureMechanismSections = new SerializableFailureMechanismSectionCollection();
            var failureMechanismSection = new SerializableFailureMechanismSection();

            // Call
            var assembly = new SerializableAssembly(id,
                                                    lowerCorner,
                                                    upperCorner,
                                                    new[]
                                                    {
                                                        assessmentSection
                                                    },
                                                    new[]
                                                    {
                                                        assessmentProcess
                                                    },
                                                    new[]
                                                    {
                                                        totalAssemblyResult
                                                    },
                                                    new[]
                                                    {
                                                        failureMechanism
                                                    },
                                                    new[]
                                                    {
                                                        failureMechanismSectionAssembly
                                                    },
                                                    new[]
                                                    {
                                                        combinedFailureMechanismSectionAssembly
                                                    },
                                                    new[]
                                                    {
                                                        failureMechanismSections
                                                    },
                                                    new[]
                                                    {
                                                        failureMechanismSection
                                                    });

            // Assert
            Assert.AreEqual(id, assembly.Id);
            Assert.AreEqual(lowerCorner.X.ToString(CultureInfo.InvariantCulture) + " " + lowerCorner.Y.ToString(CultureInfo.InvariantCulture),
                            assembly.Boundary.Envelope.LowerCorner);
            Assert.AreEqual(upperCorner.X.ToString(CultureInfo.InvariantCulture) + " " + upperCorner.Y.ToString(CultureInfo.InvariantCulture),
                            assembly.Boundary.Envelope.UpperCorner);
            CollectionAssert.AreEqual(new SerializableFeatureMember[]
            {
                assessmentSection,
                assessmentProcess,
                totalAssemblyResult,
                failureMechanism,
                failureMechanismSectionAssembly,
                combinedFailureMechanismSectionAssembly,
                failureMechanismSections,
                failureMechanismSection
            }, assembly.FeatureMembers);
        }

        [Test]
        [Explicit("XSD validation requires internet connection and takes about 30 seconds to complete.")]
        public void GivenFullyConfiguredAssembly_WhenExported_ThenCreatesValidSerializedFile()
        {
            // Given
            var assessmentSection = new SerializableAssessmentSection
            {
                Id = "section1",
                SurfaceLineLength = new SerializableMeasure
                {
                    UnitOfMeasure = "m",
                    Value = 100
                },
                Name = "Traject A",
                SurfaceLineGeometry = new SerializableLine(new[]
                {
                    new Point2D(0.35, 10.642),
                    new Point2D(10.1564, 20.23)
                })
            };

            var assessmentProcess = new SerializableAssessmentProcess("beoordelingsproces1",
                                                                      assessmentSection,
                                                                      2018,
                                                                      2020);

            var totalAssemblyResult = new SerializableTotalAssemblyResult(
                "veiligheidsoordeel_1",
                assessmentProcess,
                new SerializableFailureMechanismAssemblyResult(SerializableAssemblyMethod.WBI2B1, SerializableFailureMechanismCategoryGroup.IIt),
                new SerializableFailureMechanismAssemblyResult(SerializableAssemblyMethod.WBI3C1, SerializableFailureMechanismCategoryGroup.NotApplicable, 0.000124),
                new SerializableAssessmentSectionAssemblyResult(SerializableAssemblyMethod.WBI2C1, SerializableAssessmentSectionCategoryGroup.B));

            var failureMechanism = new SerializableFailureMechanism(
                "toetsspoorGABI",
                totalAssemblyResult,
                SerializableFailureMechanismType.GABI,
                SerializableAssemblyGroup.Group4,
                new SerializableFailureMechanismAssemblyResult(SerializableAssemblyMethod.WBI1A1, SerializableFailureMechanismCategoryGroup.IIt));

            var sections1 = new SerializableFailureMechanismSectionCollection("vakindelingGABI", failureMechanism);
            var section1 = new SerializableFailureMechanismSection(
                "vak_GABI_1",
                sections1,
                0.12,
                10.23,
                new[]
                {
                    new Point2D(0.23, 0.24),
                    new Point2D(10.23, 10.24)
                },
                SerializableFailureMechanismSectionType.FailureMechanism);

            var result = new SerializableFailureMechanismSectionAssembly(
                "resultaat_GABI_1",
                failureMechanism,
                section1,
                new[]
                {
                    new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI0E1, SerializableAssessmentLevel.SimpleAssessment, SerializableFailureMechanismSectionCategoryGroup.IIv, 0.5),
                    new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI0T5, SerializableAssessmentLevel.TailorMadeAssessment, SerializableFailureMechanismSectionCategoryGroup.IIIv)
                },
                new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI0A1, SerializableAssessmentLevel.CombinedAssessment, SerializableFailureMechanismSectionCategoryGroup.IIIv));

            var sections2 = new SerializableFailureMechanismSectionCollection("vakindeling_gecombineerd", totalAssemblyResult);
            var section2 = new SerializableFailureMechanismSection(
                "vak_gecombineerd_1",
                sections2,
                0.12,
                10.23,
                new[]
                {
                    new Point2D(0.23, 0.24),
                    new Point2D(10.23, 10.24)
                },
                SerializableFailureMechanismSectionType.Combined,
                SerializableAssemblyMethod.WBI3B1);
            var combinedResult = new SerializableCombinedFailureMechanismSectionAssembly(
                "resultaat_gecombineerd_1",
                totalAssemblyResult,
                section2,
                new[]
                {
                    new SerializableCombinedFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI3C1, SerializableFailureMechanismType.HTKW, SerializableFailureMechanismSectionCategoryGroup.IIIv),
                    new SerializableCombinedFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI3C1, SerializableFailureMechanismType.STPH, SerializableFailureMechanismSectionCategoryGroup.IVv)
                },
                new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI3B1, SerializableAssessmentLevel.CombinedSectionAssessment, SerializableFailureMechanismSectionCategoryGroup.VIv));

            var assembly = new SerializableAssembly(
                "assemblage_1",
                new Point2D(12.0, 34.0),
                new Point2D(56.053, 78.0002345),
                new[]
                {
                    assessmentSection
                },
                new[]
                {
                    assessmentProcess
                },
                new[]
                {
                    totalAssemblyResult
                },
                new[]
                {
                    failureMechanism
                },
                new[]
                {
                    result
                },
                new[]
                {
                    combinedResult
                },
                new[]
                {
                    sections1,
                    sections2
                },
                new[]
                {
                    section1,
                    section2
                });

            var serializer = new XmlSerializer(typeof(SerializableAssembly));
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add("gml", AssemblyXmlIdentifiers.GmlNamespace);
            xmlns.Add("asm", AssemblyXmlIdentifiers.AssemblyNamespace);

            var writer = new StringWriter();
            
            // When
            serializer.Serialize(writer, assembly, xmlns);
            string xml = writer.ToString();
            Console.WriteLine(xml);

            // Then
            var schema = new XmlSchemaSet();
            schema.Add("http://localhost/standaarden/assemblage", Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.AssemblyTool.IO), "assemblage.xsd"));
            XDocument doc = XDocument.Parse(xml);            
            
            string msg = string.Empty;
            doc.Validate(schema, (o, e) => { msg += e.Message + Environment.NewLine; });
            if (msg == string.Empty)
            {
                Assert.Pass("Serialized document is valid" + Environment.NewLine);
            }
            else
            {
                Assert.Fail("Serialized document is invalid:" + Environment.NewLine + msg);
            }
        }
    }
}
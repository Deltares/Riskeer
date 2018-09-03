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
using System.Linq;
using System.Xml.Serialization;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
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
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
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
        [TestCase(" ")]
        [TestCase("")]
        [TestCase(" InvalidId")]
        public void Constructor_InvalidId_ThrowsArgumentNullException(string invalidId)
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly(invalidId,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());

            // Assert
            const string expectedMessage = "'id' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_LowerCornerNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly("id",
                                                               null,
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
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
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               null,
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
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
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               null,
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentProcessesNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               null,
                                                               new SerializableTotalAssemblyResult(),
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentProcess", exception.ParamName);
        }

        [Test]
        public void Constructor_TotalAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               null,
                                                               Enumerable.Empty<SerializableFailureMechanism>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                                                               Enumerable.Empty<SerializableFailureMechanismSection>());
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("totalAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
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
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
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
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
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
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
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
            TestDelegate call = () => new SerializableAssembly("id",
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                                               new SerializableAssessmentSection(),
                                                               new SerializableAssessmentProcess(),
                                                               new SerializableTotalAssemblyResult(),
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
            const string id = "assemblyId1";

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
                                                    assessmentSection,
                                                    assessmentProcess,
                                                    totalAssemblyResult,
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
    }
}
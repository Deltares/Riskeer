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
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.AssemblyTool.IO.Test
{
    [TestFixture]
    public class SerializableAssemblyWriterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.AssemblyTool.IO,
                                                                          nameof(SerializableAssemblyWriterTest));

        [Test]
        public void WriteAssembly_SerializableAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SerializableAssemblyWriter.WriteAssembly(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("serializableAssembly", exception.ParamName);
        }

        [Test]
        public void WriteAssembly_FilePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void WriteAssembly_InvalidData_ThrowsCriticalFileWriteException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteAssembly_InvalidData_ThrowsCriticalFileWriteException));

            var assembly = new SerializableAssembly(
                "id",
                new Point2D(0.0, 10.0),
                new Point2D(10.0, 20.0),
                new SerializableAssessmentSection(),
                new SerializableAssessmentProcess(),
                new SerializableTotalAssemblyResult(),
                new[]
                {
                    new SerializableFailureMechanism()
                },
                Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                Enumerable.Empty<SerializableFailureMechanismSection>());

            try
            {
                // Call
                TestDelegate call = () => SerializableAssemblyWriter.WriteAssembly(assembly, filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
                Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void WriteAssembly_FilePathInvalid_ThrowsCriticalFileWriteException(string filePath)
        {
            // Call
            TestDelegate call = () => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void WriteAssembly_FilePathTooLong_ThrowsCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);

            // Call
            TestDelegate call = () => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void WriteAssembly_InvalidDirectoryRights_ThrowsCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(WriteAssembly_InvalidDirectoryRights_ThrowsCriticalFileWriteException));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(WriteAssembly_InvalidDirectoryRights_ThrowsCriticalFileWriteException)))
            {
                string filePath = Path.Combine(directoryPath, "test.bnd");
                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                TestDelegate call = () => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
                Assert.IsInstanceOf<UnauthorizedAccessException>(exception.InnerException);
            }
        }

        [Test]
        public void WriteAssembly_FileInUse_ThrowsCriticalFileWriteException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteAssembly_FileInUse_ThrowsCriticalFileWriteException));

            using (var fileDisposeHelper = new FileDisposeHelper(filePath))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void WriteAssembly_ValidData_ValidFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteAssembly_ValidData_ValidFile));
            SerializableAssembly assembly = CreateSerializableAssembly();

            try
            {
                // Call
                SerializableAssemblyWriter.WriteAssembly(assembly, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Assert.AreEqual(GetSerializedAssembly(assembly), fileContent);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void WriteAssembly_FullyConfiguredAssembly_ReturnsExpectedFile()
        {
            // Setup
            SerializableAssembly assembly = CreateSerializableAssembly();
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteAssembly_FullyConfiguredAssembly_ReturnsExpectedFile));

            try
            {
                // Call
                SerializableAssemblyWriter.WriteAssembly(assembly, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string pathToExpectedFile = Path.Combine(testDataPath, "configuredAssembly.gml");
                string expectedXml = File.ReadAllText(pathToExpectedFile);
                string actualXml = File.ReadAllText(filePath);
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [Explicit("Use for writer validation after changes. XSD validation requires internet connection and takes about 20 seconds to complete.")]
        public void GivenFullyConfiguredAssembly_WhenWrittenToFile_ThenValidFileCreated()
        {
            // Given
            SerializableAssembly assembly = CreateSerializableAssembly();
            string filePath = TestHelper.GetScratchPadPath(nameof(GivenFullyConfiguredAssembly_WhenWrittenToFile_ThenValidFileCreated));

            try
            {
                // When
                SerializableAssemblyWriter.WriteAssembly(assembly, filePath);

                // Then
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Console.WriteLine(fileContent);

                var schema = new XmlSchemaSet();
                schema.Add("http://localhost/standaarden/assemblage", Path.Combine(testDataPath, "assemblage.xsd"));
                XDocument doc = XDocument.Parse(fileContent);

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
            finally
            {
                File.Delete(filePath);
            }
        }

        private static SerializableAssembly CreateSerializableAssembly()
        {
            var assessmentSection = new SerializableAssessmentSection
            {
                Id = "section1",
                SurfaceLineLength = new SerializableMeasure
                {
                    UnitOfMeasure = "m", Value = 100
                },
                Name = "Traject A",
                SurfaceLineGeometry = new SerializableLine(new[]
                {
                    new Point2D(0.35, 10.642),
                    new Point2D(10.1564, 20.23)
                })
            };

            var assessmentProcess = new SerializableAssessmentProcess("beoordelingsproces1",
                                                                      assessmentSection);

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
                SerializableFailureMechanismGroup.Group4,
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
                    new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI0E1,
                                                                          SerializableAssessmentType.SimpleAssessment,
                                                                          SerializableFailureMechanismSectionCategoryGroup.IIv, 0.5),
                    new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI0T5,
                                                                          SerializableAssessmentType.TailorMadeAssessment,
                                                                          SerializableFailureMechanismSectionCategoryGroup.IIIv)
                },
                new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI0A1,
                                                                      SerializableAssessmentType.CombinedAssessment,
                                                                      SerializableFailureMechanismSectionCategoryGroup.IIIv));

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
                new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethod.WBI3B1, SerializableAssessmentType.CombinedSectionAssessment, SerializableFailureMechanismSectionCategoryGroup.VIv));

            var assembly = new SerializableAssembly(
                "assemblage_1",
                new Point2D(12.0, 34.0),
                new Point2D(56.053, 78.0002345),
                assessmentSection,
                assessmentProcess,
                totalAssemblyResult,
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
            return assembly;
        }

        private static string GetSerializedAssembly(SerializableAssembly assembly)
        {
            var serializer = new XmlSerializer(typeof(SerializableAssembly));
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add("gml", "http://www.opengis.net/gml/3.2");
            xmlns.Add("asm", "http://localhost/standaarden/assemblage");

            var writer = new StringWriterUtf8();
            serializer.Serialize(writer, assembly, xmlns);
            return writer.ToString();
        }

        private class StringWriterUtf8 : StringWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }
        }
    }
}
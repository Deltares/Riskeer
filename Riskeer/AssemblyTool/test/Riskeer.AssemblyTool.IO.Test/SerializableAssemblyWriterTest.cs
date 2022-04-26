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
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Test
{
    [TestFixture]
    public class SerializableAssemblyWriterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.AssemblyTool.IO,
                                                                          nameof(SerializableAssemblyWriterTest));

        [Test]
        public void WriteAssembly_SerializableAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SerializableAssemblyWriter.WriteAssembly(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("serializableAssembly", exception.ParamName);
        }

        [Test]
        public void WriteAssembly_FilePathNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
                void Call() => SerializableAssemblyWriter.WriteAssembly(assembly, filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(Call);
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
            void Call() => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(Call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void WriteAssembly_FilePathTooLong_ThrowsCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);

            // Call
            void Call() => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(Call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void WriteAssembly_InvalidDirectoryRights_ThrowsCriticalFileWriteException()
        {
            // Setup
            const string directoryName = nameof(WriteAssembly_InvalidDirectoryRights_ThrowsCriticalFileWriteException);
            string directoryPath = TestHelper.GetScratchPadPath(directoryName);
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), directoryName))
            {
                string filePath = Path.Combine(directoryPath, "test.bnd");
                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                void Call() => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(Call);
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
                void Call() => SerializableAssemblyWriter.WriteAssembly(new SerializableAssembly(), filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(Call);
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
            string folderPath = TestHelper.GetScratchPadPath(nameof(WriteAssembly_FullyConfiguredAssembly_ReturnsExpectedFile));
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "actualAssembly.gml");

            SerializableAssembly assembly = CreateSerializableAssembly();

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
                DirectoryHelper.TryDelete(folderPath);
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

                var msg = string.Empty;
                doc.Validate(schema, (o, e) =>
                {
                    msg += e.Message + Environment.NewLine;
                });
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
            var assessmentSection = new SerializableAssessmentSection("section1", "Traject A", new[]
            {
                new Point2D(0, 0),
                new Point2D(100.0, 0.0)
            });

            var assessmentProcess = new SerializableAssessmentProcess("beoordelingsproces1",
                                                                      assessmentSection);

            var totalAssemblyResult = new SerializableTotalAssemblyResult(
                "veiligheidsoordeel_1", assessmentProcess,
                SerializableAssemblyMethod.BOI2A1, SerializableAssemblyMethod.BOI2B1,
                SerializableAssessmentSectionAssemblyGroup.B, 0.00068354);

            var failureMechanism = new SerializableFailureMechanism(
                "toetsspoorGABI", SerializableFailureMechanismType.Generic, "GABI", "Faalmechanisme GABI", totalAssemblyResult,
                new SerializableFailureMechanismAssemblyResult(0.08419, SerializableAssemblyMethod.BOI1A1));

            var specificFailureMechanism = new SerializableFailureMechanism(
                "specifiekFaalmechanisme", SerializableFailureMechanismType.Specific, "NIEUW", "Specifiek faalmechanisme",
                totalAssemblyResult, new SerializableFailureMechanismAssemblyResult(0.002834, SerializableAssemblyMethod.BOI1A1));

            var sections1 = new SerializableFailureMechanismSectionCollection("vakindelingGABI");
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
                "resultaat_GABI_1", failureMechanism, section1,
                new SerializableFailureMechanismSectionAssemblyResult(
                    SerializableAssemblyMethod.BOI0A2, SerializableAssemblyMethod.BOI0B1,
                    SerializableFailureMechanismSectionAssemblyGroup.III, 0.00073));

            var sections2 = new SerializableFailureMechanismSectionCollection("vakindeling_gecombineerd");
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
                SerializableAssemblyMethod.BOI3A1);
            var combinedResult = new SerializableCombinedFailureMechanismSectionAssembly(
                "resultaat_gecombineerd_1",
                totalAssemblyResult,
                section2,
                new[]
                {
                    new SerializableCombinedFailureMechanismSectionAssemblyResult(
                        SerializableAssemblyMethod.BOI3B1,
                        SerializableFailureMechanismType.Generic, "HTKW", "Hoogte kunstwerk",
                        SerializableFailureMechanismSectionAssemblyGroup.III),
                    new SerializableCombinedFailureMechanismSectionAssemblyResult(
                        SerializableAssemblyMethod.BOI3B1,
                        SerializableFailureMechanismType.Generic, "STPH", "Piping",
                        SerializableFailureMechanismSectionAssemblyGroup.II),
                    new SerializableCombinedFailureMechanismSectionAssemblyResult(
                        SerializableAssemblyMethod.BOI3B1,
                        SerializableFailureMechanismType.Specific, "NIEUW", "Specifiek",
                        SerializableFailureMechanismSectionAssemblyGroup.Zero)
                },
                new SerializableFailureMechanismSubSectionAssemblyResult(
                    SerializableAssemblyMethod.BOI3C1, SerializableFailureMechanismSectionAssemblyGroup.I));

            var assembly = new SerializableAssembly(
                "assemblage_1",
                new Point2D(12.0, 34.0),
                new Point2D(56.053, 78.0002345),
                assessmentSection,
                assessmentProcess,
                totalAssemblyResult,
                new[]
                {
                    failureMechanism,
                    specificFailureMechanism
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
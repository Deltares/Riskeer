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
using System.Xml.Serialization;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model;

namespace Ringtoets.AssemblyTool.IO.Test
{
    [TestFixture]
    public class SerializableAssemblyWriterTest
    {
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
            string directoryPath = TestHelper.GetScratchPadPath("WriteAssembly_ValidData_ValidFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test_assembly.gml");

            var assembly = new SerializableAssembly(
                "id",
                new Point2D(0.0, 10.0),
                new Point2D(10.0, 20.0),
                Enumerable.Empty<SerializableAssessmentSection>(),
                Enumerable.Empty<SerializableAssessmentProcess>(),
                Enumerable.Empty<SerializableTotalAssemblyResult>(),
                new []
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
                Directory.Delete(directoryPath, true);
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
            string directoryPath = TestHelper.GetScratchPadPath("WriteAssembly_ValidData_ValidFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test_assembly.gml");

            var assembly = new SerializableAssembly(
                "id",
                new Point2D(0.0, 10.0),
                new Point2D(10.0, 20.0),
                new[]
                {
                    new SerializableAssessmentSection()
                },
                new[]
                {
                    new SerializableAssessmentProcess()
                },
                new[]
                {
                    new SerializableTotalAssemblyResult()
                },
                Enumerable.Empty<SerializableFailureMechanism>(),
                Enumerable.Empty<SerializableFailureMechanismSectionAssembly>(),
                Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>(),
                Enumerable.Empty<SerializableFailureMechanismSectionCollection>(),
                Enumerable.Empty<SerializableFailureMechanismSection>());

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
                Directory.Delete(directoryPath, true);
            }
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
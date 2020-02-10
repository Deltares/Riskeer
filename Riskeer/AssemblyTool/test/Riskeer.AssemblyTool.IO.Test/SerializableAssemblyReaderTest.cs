using System;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;

namespace Riskeer.AssemblyTool.IO.Test
{
    [TestFixture]
    public class SerializableAssemblyReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.AssemblyTool.IO,
                                                                          nameof(SerializableAssemblyReaderTest));

        [Test]
        public void Constructor_ValidFilePath_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "configuredAssembly.gml");

            // Call
            using (var reader = new SerializableAssemblyReader(filePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowCriticalFileReadException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new SerializableAssemblyReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowCriticalFileReadException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = Path.Combine(testDataPath, "configuredAssembly.gml");
            string invalidFilePath = validFilePath.Replace(".", invalidPathChars[3].ToString());

            // Call
            TestDelegate call = () => new SerializableAssemblyReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new SerializableAssemblyReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "I_do_not_exist.gml");

            // Call
            TestDelegate call = () => new SerializableAssemblyReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_FileInUse_ThrowCriticalFileReadException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath(nameof(Constructor_FileInUse_ThrowCriticalFileReadException));

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new SerializableAssemblyReader(path);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_ValidFile_ReturnsReadSerializableAssembly()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "configuredAssembly.gml");

            using (var reader = new SerializableAssemblyReader(filePath))
            {
                // Call
                SerializableAssembly readData = reader.Read();

                // Assert
                Assert.AreEqual("assemblage_1", readData.Id);
                Assert.AreEqual(10, readData.FeatureMembers.Length);
            }
        }

        [Test]
        public void Read_InvalidFile_ThrowCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "noAssembly.gml");

            using (var reader = new SerializableAssemblyReader(filePath))
            {
                // Call
                TestDelegate call = () => reader.Read();

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            }
        }
    }
}
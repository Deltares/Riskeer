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
        public void Read_WithFilepath_ReturnsReadSerializableAssembly()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "configuredAssembly.gml");

            // Call
            SerializableAssembly readData = SerializableAssemblyReader.Read(filePath);

            // Assert
            Assert.AreEqual("assemblage_1", readData.Id);
            Assert.AreEqual(10, readData.FeatureMembers.Length);
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Read_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => SerializableAssemblyReader.Read(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Read_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = Path.Combine(testDataPath, "configuredAssembly.gml");
            string invalidFilePath = validFilePath.Replace(".", invalidPathChars[3].ToString());

            // Call
            TestDelegate call = () => SerializableAssemblyReader.Read(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Read_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => SerializableAssemblyReader.Read(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Read_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "I_do_not_exist.gml");

            // Call
            TestDelegate call = () => SerializableAssemblyReader.Read(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Read_FileInUse_ThrowCriticalFileReadException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath(nameof(Read_FileInUse_ThrowCriticalFileReadException));

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => SerializableAssemblyReader.Read(path);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }
    }
}
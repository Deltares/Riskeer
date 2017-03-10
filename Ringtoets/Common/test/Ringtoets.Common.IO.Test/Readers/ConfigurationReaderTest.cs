// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.Common.IO.Test.Readers
{
    [TestFixture]
    public class ConfigurationReaderTest
    {
        private readonly string schemaString;

        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                               "ConfigurationReader");

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new TestConfigurationReader(invalidFilePath, schemaString);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");
            string invalidFilePath = validFilePath.Replace("Config", invalidPathChars[3].ToString());

            // Call
            TestDelegate call = () => new TestConfigurationReader(invalidFilePath, schemaString);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDirectoryPath, Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new TestConfigurationReader(invalidFilePath, schemaString);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDirectoryPath, "notExisting.xml");

            // Call
            TestDelegate call = () => new TestConfigurationReader(invalidFilePath, schemaString);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoSchemaString_ThrowArgumentException(string invalidSchemaString)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");

            // Call
            TestDelegate call = () => new TestConfigurationReader(filePath, invalidSchemaString);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'mainSchemaDefinition' null, empty or only containing white spaces.", exception.Message);
        }

        [Test]
        [TestCase("textContent.xsd",
            "'mainSchemaDefinition' containing invalid schema definition: Data at the root level is invalid. Line 1, position 1.",
            typeof(XmlException))]
        [TestCase("invalidXsdContent.xsd",
            "'mainSchemaDefinition' containing invalid schema definition: The 'http://www.w3.org/2001/XMLSchema:redefine' element is not supported in this context.",
            typeof(XmlSchemaException))]
        [TestCase("notReferencingBaseXsd.xsd",
            "'nestedSchemaDefinitions' contains one or more schema definitions that are not referenced.")]
        public void Constructor_InvalidSchemaString_ThrowArgumentException(string fileName, string expectedMessage, Type expectedInnerExceptionType = null)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");
            string xsdPath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new TestConfigurationReader(filePath, File.ReadAllText(xsdPath));

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);

            if (expectedInnerExceptionType != null)
            {
                Assert.IsInstanceOf(expectedInnerExceptionType, exception.InnerException);
            }
        }

        [Test]
        [TestCase("empty.xml")]
        [TestCase("textContent.xml")]
        [TestCase("invalidXmlContent.xml")]
        public void Constructor_FileDoesNotContainValidXml_ThrowCriticalFileReadException(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new TestConfigurationReader(filePath, schemaString);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<XmlException>(exception.InnerException);
        }

        [Test]
        public void Constructor_FileInUse_ThrowCriticalFileReadException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath($"{nameof(ConfigurationReaderTest)}.{nameof(Constructor_FileInUse_ThrowCriticalFileReadException)}");

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new TestConfigurationReader(path, schemaString);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadExceptionWithExpectedMessage()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "invalidFolderNoName.xml");

            // Call
            TestDelegate call = () => new TestConfigurationReader(filePath, schemaString);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het XML-document dat de configuratie" +
                                     " voor de berekeningen beschrijft is niet geldig. De validatie geeft de volgende melding" +
                                     " op regel 3, positie 4: The required attribute \'naam\' is missing.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        public ConfigurationReaderTest()
        {
            schemaString = File.ReadAllText(Path.Combine(testDirectoryPath, "validConfigurationSchema.xsd"));
        }

        private class TestConfigurationReader : ConfigurationReader<TestReadConfigurationItem>
        {
            public TestConfigurationReader(string xmlFilePath, string mainSchemaDefinition)
                : base(xmlFilePath, mainSchemaDefinition, new Dictionary<string, string>()) {}

            protected override TestReadConfigurationItem ParseCalculationElement(XElement calculationElement)
            {
                return new TestReadConfigurationItem("Test");
            }
        }

        private class TestReadConfigurationItem : IReadConfigurationItem
        {
            public TestReadConfigurationItem(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}
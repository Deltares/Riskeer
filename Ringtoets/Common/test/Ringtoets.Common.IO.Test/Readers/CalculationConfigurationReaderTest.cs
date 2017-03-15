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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Test.Readers
{
    [TestFixture]
    public class CalculationConfigurationReaderTest
    {
        private readonly string validMainSchemaDefinition;

        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                               "CalculationConfigurationReader");

        private static IEnumerable<TestCaseData> InvalidConfigurations
        {
            get
            {
                yield return new TestCaseData("invalidFolderNoName.xml",
                                              "The required attribute 'naam' is missing.")
                    .SetName("invalidFolderNoName");
                yield return new TestCaseData("invalidCalculationNoName.xml",
                                              "The required attribute 'naam' is missing.")
                    .SetName("invalidCalculationNoName");
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new CalculationConfigurationReader(invalidFilePath, validMainSchemaDefinition, new Dictionary<string, string>());

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
            TestDelegate call = () => new CalculationConfigurationReader(invalidFilePath, validMainSchemaDefinition, new Dictionary<string, string>());

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
            TestDelegate call = () => new CalculationConfigurationReader(invalidFilePath, validMainSchemaDefinition, new Dictionary<string, string>());

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
            TestDelegate call = () => new CalculationConfigurationReader(invalidFilePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
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
            TestDelegate call = () => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

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
            string path = TestHelper.GetScratchPadPath($"{nameof(CalculationConfigurationReaderTest)}.{nameof(Constructor_FileInUse_ThrowCriticalFileReadException)}");

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new CalculationConfigurationReader(path, validMainSchemaDefinition, new Dictionary<string, string>());

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void Constructor_MainSchemaDefinitionNotReferencingDefaultSchema_ThrowArgumentException()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");
            string xsdPath = Path.Combine(testDirectoryPath, "mainSchemaDefinitionNotReferencingDefaultSchema.xsd");

            // Call
            TestDelegate call = () => new CalculationConfigurationReader(filePath, File.ReadAllText(xsdPath), new Dictionary<string, string>());

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'mainSchemaDefinition' does not reference the default schema 'ConfiguratieSchema.xsd'.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            Assert.IsTrue(exception.InnerException?.Message.Contains(expectedParsingMessage));
        }

        [Test]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadExceptionWithExpectedMessage()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "invalidFolderNoName.xml");

            // Call
            TestDelegate call = () => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het XML-document dat de configuratie" +
                                     " voor de berekeningen beschrijft is niet geldig. De validatie geeft de volgende melding" +
                                     " op regel 3, positie 4: The required attribute \'naam\' is missing.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_FileEmpty_ThrowCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "emptyConfiguration.xml");

            // Call
            TestDelegate call = () => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het XML-document dat de configuratie" +
                                     " voor de berekeningen beschrijft bevat geen berekeningselementen.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_ValidParameters_DoesNotThrow()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");

            // Call
            TestDelegate call = () => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyFolder_ReturnExpectedReadCalculationGroup()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyFolder.xml");
            var calculationConfigurationReader = new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Call
            IList<IReadConfigurationItem> readConfigurationItems = calculationConfigurationReader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readConfigurationItems.Count);

            var group = readConfigurationItems[0] as ReadCalculationGroup;
            Assert.IsNotNull(group);
            Assert.AreEqual("Calculation group", group.Name);
            CollectionAssert.IsEmpty(group.Items);
        }

        [Test]
        public void Read_ValidConfigurationWithNesting_ReturnExpectedReadConfigurationItems()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");
            var calculationConfigurationReader = new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Call
            IList<IReadConfigurationItem> readConfigurationItems = calculationConfigurationReader.Read().ToList();

            // Assert
            Assert.AreEqual(5, readConfigurationItems.Count);

            var group1 = readConfigurationItems[0] as ReadCalculationGroup;
            Assert.IsNotNull(group1);
            Assert.AreEqual("Group 1", group1.Name);

            var calculation1 = readConfigurationItems[1] as ReadCalculation;
            Assert.IsNotNull(calculation1);
            Assert.AreEqual("Calculation 1", calculation1.Name);

            var group2 = readConfigurationItems[2] as ReadCalculationGroup;
            Assert.IsNotNull(group2);
            Assert.AreEqual("Group 2", group2.Name);

            var calculation2 = readConfigurationItems[3] as ReadCalculation;
            Assert.IsNotNull(calculation2);
            Assert.AreEqual("Calculation 2", calculation2.Name);

            var group3 = readConfigurationItems[4] as ReadCalculationGroup;
            Assert.IsNotNull(group3);
            Assert.AreEqual("Group 3", group3.Name);

            List<IReadConfigurationItem> group1Items = group1.Items.ToList();
            Assert.AreEqual(1, group1Items.Count);

            var calculation3 = group1Items[0] as ReadCalculation;
            Assert.IsNotNull(calculation3);
            Assert.AreEqual("Calculation 3", calculation3.Name);

            List<IReadConfigurationItem> group2Items = group2.Items.ToList();
            Assert.AreEqual(2, group2Items.Count);

            var group4 = group2Items[0] as ReadCalculationGroup;
            Assert.IsNotNull(group4);
            Assert.AreEqual("Group 4", group4.Name);

            var calculation4 = group2Items[1] as ReadCalculation;
            Assert.IsNotNull(calculation4);
            Assert.AreEqual("Calculation 4", calculation4.Name);

            List<IReadConfigurationItem> group3Items = group3.Items.ToList();
            Assert.AreEqual(0, group3Items.Count);

            List<IReadConfigurationItem> group4Items = group4.Items.ToList();
            Assert.AreEqual(1, group4Items.Count);

            var calculation5 = group4Items[0] as ReadCalculation;
            Assert.IsNotNull(calculation5);
            Assert.AreEqual("Calculation 5", calculation5.Name);
        }

        public CalculationConfigurationReaderTest()
        {
            validMainSchemaDefinition = File.ReadAllText(Path.Combine(testDirectoryPath, "validConfigurationSchema.xsd"));
        }

        private class CalculationConfigurationReader : CalculationConfigurationReader<ReadCalculation>
        {
            public CalculationConfigurationReader(string xmlFilePath,
                                                  string mainSchemaDefinition,
                                                  IDictionary<string, string> nestedSchemaDefinitions)
                : base(xmlFilePath, mainSchemaDefinition, nestedSchemaDefinitions) {}

            protected override ReadCalculation ParseCalculationElement(XElement calculationElement)
            {
                return new ReadCalculation(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value);
            }
        }

        private class ReadCalculation : IReadConfigurationItem
        {
            public ReadCalculation(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}
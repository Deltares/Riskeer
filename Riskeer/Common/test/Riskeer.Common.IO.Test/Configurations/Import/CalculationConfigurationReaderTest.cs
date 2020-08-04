// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Import;

namespace Riskeer.Common.IO.Test.Configurations.Import
{
    [TestFixture]
    public class CalculationConfigurationReaderTest
    {
        private string validMainSchemaDefinition;

        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
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

        private static IEnumerable<TestCaseData> InvalidXml
        {
            get
            {
                yield return new TestCaseData(
                        "empty.xml",
                        "Root element is missing.")
                    .SetName("FileDoesNotContainValidXml_empty.xml");
                yield return new TestCaseData(
                        "textContent.xml",
                        "Data at the root level is invalid. Line 1, position 1.")
                    .SetName("FileDoesNotContainValidXml_textContent.xml");
                yield return new TestCaseData(
                        "invalidXmlContent.xml",
                        "The 'map' start tag on line 4 position 4 does not match the end tag of 'configuratie'. Line 5, position 3.")
                    .SetName("FileDoesNotContainValidXml_invalidXmlContent.xml");
                yield return new TestCaseData(
                        "withoutQuotationMarks.xml",
                        "'Nieuw' is an unexpected token. The expected token is '\"' or '''. Line 3, position 20.")
                    .SetName("FileDoesNotContainValidXml_withoutQoutationMarks.xml");
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            void Call() => new CalculationConfigurationReader(invalidFilePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();
            string validFilePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");
            string invalidFilePath = validFilePath.Replace("Config", invalidPathChars[3].ToString());

            // Call
            void Call() => new CalculationConfigurationReader(invalidFilePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDirectoryPath, Path.DirectorySeparatorChar.ToString());

            // Call
            void Call() => new CalculationConfigurationReader(invalidFilePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDirectoryPath, "notExisting.xml");

            // Call
            void Call() => new CalculationConfigurationReader(invalidFilePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.";
            string message = Assert.Throws<CriticalFileReadException>(Call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCaseSource(nameof(InvalidXml))]
        public void Constructor_FileDoesNotContainValidXml_ThrowCriticalFileReadException(string fileName, string expectedInnerMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            void Call() => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': " +
                                     "het XML-document dat de configuratie voor de berekeningen beschrijft is niet geldig. " +
                                     $"De validatie geeft de volgende melding: {expectedInnerMessage}";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<XmlException>(exception.InnerException);
            Assert.IsTrue(exception.InnerException.Message.Contains(expectedInnerMessage));
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
                void Call() => new CalculationConfigurationReader(path, validMainSchemaDefinition, new Dictionary<string, string>());

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(Call);
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
            void Call() => new CalculationConfigurationReader(filePath, File.ReadAllText(xsdPath), new Dictionary<string, string>());

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual("'mainSchemaDefinition' does not reference the default schema 'ConfiguratieSchema.xsd'.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(InvalidConfigurations))]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName, string expectedParsingMessage)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            void Call() => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
            Assert.IsTrue(exception.InnerException?.Message.Contains(expectedParsingMessage));
        }

        [Test]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadExceptionWithExpectedMessage()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "invalidFolderNoName.xml");

            // Call
            void Call() => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het XML-document dat de configuratie" +
                                     " voor de berekeningen beschrijft is niet geldig. De validatie geeft de volgende melding" +
                                     " op regel 3, positie 4: The required attribute \'naam\' is missing.";
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_EmptyConfiguration_ThrowCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "emptyConfiguration.xml");

            // Call
            void Call() => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': " +
                                     "het XML-document dat de configuratie voor de berekeningen beschrijft, " +
                                     "moet mappen en/of berekeningen bevatten.";
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_ValidParameters_DoesNotThrow()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");

            // Call
            void Call() => new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Assert
            Assert.DoesNotThrow(Call);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyFolder_ReturnExpectedReadCalculationGroup()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyFolder.xml");
            var calculationConfigurationReader = new CalculationConfigurationReader(filePath, validMainSchemaDefinition, new Dictionary<string, string>());

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = calculationConfigurationReader.Read().ToArray();

            // Assert
            var group = (CalculationGroupConfiguration) readConfigurationItems.Single();

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
            IConfigurationItem[] readConfigurationItems = calculationConfigurationReader.Read().ToArray();

            // Assert
            Assert.AreEqual(5, readConfigurationItems.Length);

            var group1 = readConfigurationItems[0] as CalculationGroupConfiguration;
            Assert.IsNotNull(group1);
            Assert.AreEqual("Group 1", group1.Name);

            var calculation1 = readConfigurationItems[1] as ReadCalculation;
            Assert.IsNotNull(calculation1);
            Assert.AreEqual("Calculation 1", calculation1.Name);

            var group2 = readConfigurationItems[2] as CalculationGroupConfiguration;
            Assert.IsNotNull(group2);
            Assert.AreEqual("Group 2", group2.Name);

            var calculation2 = readConfigurationItems[3] as ReadCalculation;
            Assert.IsNotNull(calculation2);
            Assert.AreEqual("Calculation 2", calculation2.Name);

            var group3 = readConfigurationItems[4] as CalculationGroupConfiguration;
            Assert.IsNotNull(group3);
            Assert.AreEqual("Group 3", group3.Name);

            List<IConfigurationItem> group1Items = group1.Items.ToList();
            Assert.AreEqual(1, group1Items.Count);

            var calculation3 = group1Items[0] as ReadCalculation;
            Assert.IsNotNull(calculation3);
            Assert.AreEqual("Calculation 3", calculation3.Name);

            List<IConfigurationItem> group2Items = group2.Items.ToList();
            Assert.AreEqual(2, group2Items.Count);

            var group4 = group2Items[0] as CalculationGroupConfiguration;
            Assert.IsNotNull(group4);
            Assert.AreEqual("Group 4", group4.Name);

            var calculation4 = group2Items[1] as ReadCalculation;
            Assert.IsNotNull(calculation4);
            Assert.AreEqual("Calculation 4", calculation4.Name);

            List<IConfigurationItem> group3Items = group3.Items.ToList();
            Assert.AreEqual(0, group3Items.Count);

            List<IConfigurationItem> group4Items = group4.Items.ToList();
            Assert.AreEqual(1, group4Items.Count);

            var calculation5 = group4Items[0] as ReadCalculation;
            Assert.IsNotNull(calculation5);
            Assert.AreEqual("Calculation 5", calculation5.Name);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            validMainSchemaDefinition = File.ReadAllText(Path.Combine(testDirectoryPath, "validConfigurationSchema.xsd"));
        }

        private class CalculationConfigurationReader : CalculationConfigurationReader<ReadCalculation>
        {
            public CalculationConfigurationReader(string xmlFilePath,
                                                  string mainSchemaDefinition,
                                                  IDictionary<string, string> nestedSchemaDefinitions)
                : base(xmlFilePath, new[]
                {
                    new CalculationConfigurationSchemaDefinition(
                        mainSchemaDefinition, nestedSchemaDefinitions)
                }) {}

            protected override ReadCalculation ParseCalculationElement(XElement calculationElement)
            {
                return new ReadCalculation(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value);
            }
        }

        private class ReadCalculation : IConfigurationItem
        {
            public ReadCalculation(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}
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
using System.Xml.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class CalculationConfigurationImporterTest
    {
        private readonly string readerPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "CalculationConfigurationReader");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new TestCalculationConfigurationImporter("",
                                                                    new CalculationGroup());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<CalculationGroup>>(importer);
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            var importer = new TestCalculationConfigurationImporter(filePath,
                                                                    new CalculationGroup());

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam. " + Environment.NewLine +
                                     "Er is geen berekeningenconfiguratie geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist");

            var importer = new TestCalculationConfigurationImporter(filePath,
                                                                    new CalculationGroup());

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand bestaat niet. " + Environment.NewLine +
                                     "Er is geen berekeningenconfiguratie geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_InvalidFile_CancelImportWithErrorMessage()
        {
            // Setup
            string filePath = Path.Combine(readerPath, "invalidFolderNoName.xml");
            var importer = new TestCalculationConfigurationImporter(filePath,
                                                                    new CalculationGroup());

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith($"Fout bij het lezen van bestand '{filePath}': het XML-document dat de configuratie voor de berekeningen beschrijft is niet geldig.", msgs[0]);
            });

            Assert.IsFalse(importSuccessful);
        }

        [Test]
        [TestCase("Inlezen")]
        [TestCase("Valideren")]
        public void Import_CancelingImport_CancelImportAndLog(string expectedProgressMessage)
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            string filePath = Path.Combine(readerPath, "validConfiguration.xml");
            var importer = new TestCalculationConfigurationImporter(filePath,
                                                                    calculationGroup);

            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(expectedProgressMessage))
                {
                    importer.Cancel();
                }
            });

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Berekeningenconfiguratie importeren afgebroken. Geen data ingelezen.", 1);
            CollectionAssert.IsEmpty(calculationGroup.Children);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void GivenImport_WhenImporting_ThenExpectedProgressMessagesGenerated()
        {
            // Given
            string filePath = Path.Combine(readerPath, "validConfiguration.xml");
            var importer = new TestCalculationConfigurationImporter(filePath,
                                                                    new CalculationGroup());

            var expectedProgressMessages = new[]
            {
                new ExpectedProgressNotification
                {
                    Text = "Inlezen berekeningenconfiguratie.", CurrentStep = 1, TotalNumberOfSteps = 3
                },
                new ExpectedProgressNotification
                {
                    Text = "Valideren berekeningenconfiguratie.", CurrentStep = 2, TotalNumberOfSteps = 3
                },
                new ExpectedProgressNotification
                {
                    Text = "Geïmporteerde data toevoegen aan het toetsspoor.", CurrentStep = 3, TotalNumberOfSteps = 3
                }
            };

            var progressChangedCallCount = 0;
            importer.SetProgressChanged((description, step, steps) =>
            {
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].Text, description);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].CurrentStep, step);
                Assert.AreEqual(expectedProgressMessages[progressChangedCallCount].TotalNumberOfSteps, steps);
                progressChangedCallCount++;
            });

            // When
            importer.Import();

            // Then
            Assert.AreEqual(expectedProgressMessages.Length, progressChangedCallCount);
        }

        private class TestCalculationConfigurationImporter : CalculationConfigurationImporter<TestCalculationConfigurationReader, ReadCalculation>
        {
            public TestCalculationConfigurationImporter(string filePath, CalculationGroup importTarget)
                : base(filePath, importTarget) {}

            protected override TestCalculationConfigurationReader CreateConfigurationReader(string xmlFilePath)
            {
                return new TestCalculationConfigurationReader(xmlFilePath);
            }

            protected override ICalculationBase ParseReadCalculation(ReadCalculation readCalculation)
            {
                return new TestCalculation
                {
                    Name = readCalculation.Name
                };
            }
        }

        private class TestCalculationConfigurationReader : CalculationConfigurationReader<ReadCalculation>
        {
            private static readonly string mainSchemaDefinition =
                File.ReadAllText(Path.Combine(TestHelper.GetTestDataPath(
                                                  TestDataPath.Ringtoets.Common.IO,
                                                  "CalculationConfigurationReader"),
                                              "validConfigurationSchema.xsd"));

            public TestCalculationConfigurationReader(string xmlFilePath)
                : base(xmlFilePath, mainSchemaDefinition, new Dictionary<string, string>()) {}

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

        private class TestCalculation : Observable, ICalculationBase
        {
            public string Name { get; set; }
        }

        private class ExpectedProgressNotification
        {
            public string Text { get; set; }
            public int CurrentStep { get; set; }
            public int TotalNumberOfSteps { get; set; }
        }
    }
}
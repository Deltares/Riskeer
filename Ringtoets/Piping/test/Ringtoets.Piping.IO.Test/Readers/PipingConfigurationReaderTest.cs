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
using System.Xml.Schema;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.IO.Readers;

namespace Ringtoets.Piping.IO.Test.Readers
{
    [TestFixture]
    public class PipingConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                                               "PipingConfigurationReader");

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new PipingConfigurationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = Path.Combine(testDirectoryPath, "validPipingConfiguration.xml");
            string invalidFilePath = validFilePath.Replace("Piping", invalidPathChars[3].ToString());

            // Call
            TestDelegate call = () => new PipingConfigurationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet de volgende tekens bevatten: {string.Join(", ", invalidPathChars)}";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDirectoryPath, Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new PipingConfigurationReader(invalidFilePath);

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
            TestDelegate call = () => new PipingConfigurationReader(invalidFilePath);

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
            TestDelegate call = () => new PipingConfigurationReader(filePath);

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
            string filePath = Path.Combine(testDirectoryPath, "validConfiguration.xml");

            using (new FileStream(filePath, FileMode.Open))
            {
                // Call
                TestDelegate call = () => new PipingConfigurationReader(filePath);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        [TestCase("invalidNoItems.xml")]
        [TestCase("invalidFolderWithoutName.xml")]
        public void Constructor_FileInvalidBasedOnSchemaDefinition_ThrowCriticalFileReadException(string fileName)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, fileName);

            // Call
            TestDelegate call = () => new PipingConfigurationReader(filePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het XML-document dat de configuratie voor de berekeningen beschrijft is niet geldig.";
            var exception = Assert.Throws<CriticalFileReadException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<XmlSchemaValidationException>(exception.InnerException);
        }

        [Test]
        public void Read_ValidConfigurationWithNesting_ReturnExpectedReadPipingCalculationItems()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationWithNesting.xml");
            var pipingConfigurationReader = new PipingConfigurationReader(filePath);

            // Call
            IList<IReadPipingCalculationItem> readPipingCalculationItems = pipingConfigurationReader.Read().ToList();

            // Assert
            Assert.AreEqual(5, readPipingCalculationItems.Count);

            var group1 = readPipingCalculationItems[0] as ReadPipingCalculationGroup;
            Assert.IsNotNull(group1);
            Assert.AreEqual("Group 1", group1.Name);

            var calculation1 = readPipingCalculationItems[1] as ReadPipingCalculation;
            Assert.IsNotNull(calculation1);
            Assert.AreEqual("Calculation 1", calculation1.Name);

            var group2 = readPipingCalculationItems[2] as ReadPipingCalculationGroup;
            Assert.IsNotNull(group2);
            Assert.AreEqual("Group 2", group2.Name);

            var calculation2 = readPipingCalculationItems[3] as ReadPipingCalculation;
            Assert.IsNotNull(calculation2);
            Assert.AreEqual("Calculation 2", calculation2.Name);

            var group3 = readPipingCalculationItems[4] as ReadPipingCalculationGroup;
            Assert.IsNotNull(group3);
            Assert.AreEqual("Group 3", group3.Name);

            List<IReadPipingCalculationItem> group1Items = group1.Items.ToList();
            Assert.AreEqual(1, group1Items.Count);

            var calculation3 = group1Items[0] as ReadPipingCalculation;
            Assert.IsNotNull(calculation3);
            Assert.AreEqual("Calculation 3", calculation3.Name);

            List<IReadPipingCalculationItem> group2Items = group2.Items.ToList();
            Assert.AreEqual(2, group2Items.Count);

            var group4 = group2Items[0] as ReadPipingCalculationGroup;
            Assert.IsNotNull(group4);
            Assert.AreEqual("Group 4", group4.Name);

            var calculation4 = group2Items[1] as ReadPipingCalculation;
            Assert.IsNotNull(calculation4);
            Assert.AreEqual("Calculation 4", calculation4.Name);

            List<IReadPipingCalculationItem> group3Items = group3.Items.ToList();
            Assert.AreEqual(0, group3Items.Count);

            List<IReadPipingCalculationItem> group4Items = group4.Items.ToList();
            Assert.AreEqual(1, group4Items.Count);

            var calculation5 = group4Items[0] as ReadPipingCalculation;
            Assert.IsNotNull(calculation5);
            Assert.AreEqual("Calculation 5", calculation5.Name);
        }

        [Test]
        public void Read_ValidConfigurationWithEmptyCalculation_ReturnExpectedReadPipingCalculationItems()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationWithEmptyCalculation.xml");
            var pipingConfigurationReader = new PipingConfigurationReader(filePath);

            // Call
            IList<IReadPipingCalculationItem> readPipingCalculationItems = pipingConfigurationReader.Read().ToList();

            // Assert
            Assert.AreEqual(1, readPipingCalculationItems.Count);

            var calculation = readPipingCalculationItems[0] as ReadPipingCalculation;
            Assert.IsNotNull(calculation);
            Assert.AreEqual("Calculation", calculation.Name);
            Assert.IsNull(calculation.AssessmentLevel);
            Assert.IsNull(calculation.HydraulicBoundaryLocation);
            Assert.IsNull(calculation.SurfaceLine);
            Assert.IsNull(calculation.EntryPointL);
            Assert.IsNull(calculation.ExitPointL);
            Assert.IsNull(calculation.StochasticSoilModel);
            Assert.IsNull(calculation.StochasticSoilProfile);
            Assert.IsNull(calculation.PhreaticLevelExitMean);
            Assert.IsNull(calculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNull(calculation.DampingFactorExitMean);
            Assert.IsNull(calculation.DampingFactorExitStandardDeviation);
        }
    }
}
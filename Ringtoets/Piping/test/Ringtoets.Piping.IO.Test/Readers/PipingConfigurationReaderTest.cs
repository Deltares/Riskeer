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
using System.IO;
using System.Xml;
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
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

            string validFilePath = Path.Combine(testDirectoryPath, "validPipingConfiguration.xml");
            string invalidFilePath = validFilePath.Replace("Piping", invalidFileNameChars[3].ToString());

            // Call
            TestDelegate call = () => new PipingConfigurationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet de volgende tekens bevatten: {string.Join(", ", invalidFileNameChars)}";
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
    }
}
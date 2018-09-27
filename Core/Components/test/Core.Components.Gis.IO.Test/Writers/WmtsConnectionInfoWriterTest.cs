// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.Gis.IO.Writers;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Writers
{
    [TestFixture]
    public class WmtsConnectionInfoWriterTest
    {
        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowArgumentException(string filePath)
        {
            // Call
            TestDelegate call = () => new WmtsConnectionInfoWriter(filePath);

            // Assert
            const string expectedMessage = "bestandspad mag niet leeg of ongedefinieerd zijn.";
            string message = Assert.Throws<ArgumentException>(call).Message;
            StringAssert.Contains(expectedMessage, message);
        }

        [Test]
        public void Constructor_FilePathIsDirectory_ThrowArgumentException()
        {
            // Call
            TestDelegate call = () => new WmtsConnectionInfoWriter("c:/");

            // Assert
            const string expectedMessage = "bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            string message = Assert.Throws<ArgumentException>(call).Message;
            StringAssert.Contains(expectedMessage, message);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();
            string filePath = "c:/_.config".Replace('_', invalidPathChars[0]);

            // Call
            TestDelegate call = () => new WmtsConnectionInfoWriter(filePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void WriteWmtsConnectionInfo_DirectoryDoesNotExist_CreatesDirectoryAndFile()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(WriteWmtsConnectionInfo_DirectoryDoesNotExist_CreatesDirectoryAndFile));
            string filePath = Path.Combine(directoryPath, Path.GetRandomFileName());
            var wmtsConfigurationWriter = new WmtsConnectionInfoWriter(filePath);

            try
            {
                // Call
                wmtsConfigurationWriter.WriteWmtsConnectionInfo(Enumerable.Empty<WmtsConnectionInfo>());

                // Assert
                Assert.IsTrue(File.Exists(filePath));
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void WriteWmtsConnectionInfo_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(WriteWmtsConnectionInfo_InvalidDirectoryRights_ThrowCriticalFileWriteException));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, Path.GetRandomFileName());
            var wmtsConfigurationWriter = new WmtsConnectionInfoWriter(filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    TestDelegate call = () => wmtsConfigurationWriter.WriteWmtsConnectionInfo(Enumerable.Empty<WmtsConnectionInfo>());

                    // Assert
                    string message = Assert.Throws<CriticalFileWriteException>(call).Message;
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.";
                    Assert.AreEqual(expectedMessage, message);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void WriteWmtsConnectionInfo_WmtsConnectionInfosNull_ThrowArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteWmtsConnectionInfo_WmtsConnectionInfosNull_ThrowArgumentNullException));
            var wmtsConfigurationWriter = new WmtsConnectionInfoWriter(filePath);

            // Call
            TestDelegate call = () => wmtsConfigurationWriter.WriteWmtsConnectionInfo(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wmtsConnectionInfos", paramName);
        }

        [Test]
        public void WriteWmtsConnectionInfo_ValidWmtsConnectionInfo_SavesWmtsConnectionInfoToFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(WriteWmtsConnectionInfo_ValidWmtsConnectionInfo_SavesWmtsConnectionInfoToFile));
            var wmtsConfigurationWriter = new WmtsConnectionInfoWriter(filePath);

            var wmtsConnectionInfos = new[]
            {
                new WmtsConnectionInfo("name1", "url1"),
                new WmtsConnectionInfo("name2", "url2")
            };

            using (new FileDisposeHelper(filePath))
            {
                // Call
                wmtsConfigurationWriter.WriteWmtsConnectionInfo(wmtsConnectionInfos);

                // Assert
                string actualContent = GetFileContent(filePath);
                string expectedContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine +
                                         "<WmtsConnections>" + Environment.NewLine +
                                         "  <WmtsConnection>" + Environment.NewLine +
                                         "    <Name>name1</Name>" + Environment.NewLine +
                                         "    <URL>url1</URL>" + Environment.NewLine +
                                         "  </WmtsConnection>" + Environment.NewLine +
                                         "  <WmtsConnection>" + Environment.NewLine +
                                         "    <Name>name2</Name>" + Environment.NewLine +
                                         "    <URL>url2</URL>" + Environment.NewLine +
                                         "  </WmtsConnection>" + Environment.NewLine +
                                         "</WmtsConnections>";
                Assert.AreEqual(expectedContent, actualContent);
            }
        }

        private static string GetFileContent(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
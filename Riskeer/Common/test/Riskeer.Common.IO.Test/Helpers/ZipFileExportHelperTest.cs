// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Helpers;

namespace Riskeer.Common.IO.Test.Helpers
{
    [TestFixture]
    public class ZipFileExportHelperTest
    {
        [Test]
        public void CreateZipFileFromExportedFiles_SourceFolderPathNull_ThrowsArgumentException()
        {
            // Call
            void Call() => ZipFileExportHelper.CreateZipFileFromExportedFiles(null, "test");

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void CreateZipFileFromExportedFiles_DestinationFilePathNull_ThrowsArgumentException()
        {
            // Call
            void Call() => ZipFileExportHelper.CreateZipFileFromExportedFiles("test", null);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void CreateZipFileFromExportedFiles_ValidPaths_CreatesExpectedZipFile()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(CreateZipFileFromExportedFiles_ValidPaths_CreatesExpectedZipFile));
            Directory.CreateDirectory(directoryPath);

            string sourceFolderPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO),
                                                   nameof(ZipFileExportHelper));
            string destinationFilePath = Path.Combine(directoryPath, "test.zip");

            try
            {
                // Call
                ZipFileExportHelper.CreateZipFileFromExportedFiles(sourceFolderPath, destinationFilePath);

                // Assert
                Assert.IsTrue(File.Exists(destinationFilePath));

                using (ZipArchive zipArchive = ZipFile.OpenRead(destinationFilePath))
                {
                    var expectedFiles = new[]
                    {
                        "TestFile1.txt",
                        "TestFile2.txt",
                        "TestFile3.txt",
                        "TestFile4.txt"
                    };
                    CollectionAssert.IsSubsetOf(expectedFiles, zipArchive.Entries.Select(e => e.FullName));
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void CreateZipFileFromExportedFiles_FileAlreadyExists_DoesNotThrow()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(CreateZipFileFromExportedFiles_FileAlreadyExists_DoesNotThrow));
            Directory.CreateDirectory(directoryPath);

            string sourceFolderPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO),
                                                   nameof(ZipFileExportHelper));
            string destinationFilePath = Path.Combine(directoryPath, "test.zip");

            try
            {
                ZipFileExportHelper.CreateZipFileFromExportedFiles(sourceFolderPath, destinationFilePath);

                // Precondition
                Assert.IsTrue(File.Exists(destinationFilePath));

                // Call
                void Call() => ZipFileExportHelper.CreateZipFileFromExportedFiles(sourceFolderPath, destinationFilePath);

                // Assert
                Assert.DoesNotThrow(Call);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void CreateZipFileFromExportedFiles_OperationThrowsIOException_ThrowsCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(CreateZipFileFromExportedFiles_OperationThrowsIOException_ThrowsCriticalFileWriteException));
            Directory.CreateDirectory(directoryPath);

            string sourceFolderPath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO),
                                                   nameof(ZipFileExportHelper));
            string destinationFilePath = Path.Combine(directoryPath, "test.zip");

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    void Call() => ZipFileExportHelper.CreateZipFileFromExportedFiles(sourceFolderPath, destinationFilePath);

                    // Assert
                    var exception = Assert.Throws<CriticalFileWriteException>(Call);
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{destinationFilePath}'.";
                    Assert.AreEqual(expectedMessage, exception.Message);
                    Assert.IsNotNull(exception.InnerException);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}
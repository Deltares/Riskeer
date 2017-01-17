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
using System.Security.AccessControl;
using Application.Ringtoets.Storage.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class BackedUpFileWriterTest
    {
        private readonly string testWorkDir = Path.Combine(".", "SafeOverwriteFileHelperTest");

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            Directory.CreateDirectory(testWorkDir);
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {
            Directory.Delete(testWorkDir);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("\"")]
        [TestCase("/")]
        public void Constructor_InvalidPath_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate test = () => new BackedUpFileWriter(path);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Perform_ValidFile_TemporaryFileCreatedFromOriginalAndDeletedAfterwards()
        {
            // Setup
            var writableDirectory = Path.Combine(testWorkDir, "Writable");
            var filePath = Path.Combine(writableDirectory, "iDoExist.txt");
            var temporaryFilePath = filePath + "~";
            var testContent = "Some test text to write into file.";

            Directory.CreateDirectory(writableDirectory);
            File.WriteAllText(filePath, testContent);

            var writer = new BackedUpFileWriter(filePath);

            try
            {
                // Precondition
                Assert.IsTrue(File.Exists(filePath));

                // Call
                writer.Perform(() =>
                {
                    // Assert
                    Assert.IsFalse(File.Exists(filePath));
                    Assert.IsTrue(File.Exists(temporaryFilePath));
                    Assert.AreEqual(testContent, File.ReadAllText(temporaryFilePath));
                });
                Assert.False(File.Exists(temporaryFilePath));
            }
            finally
            {
                Directory.Delete(writableDirectory, true);
            }
        }

        [Test]
        public void Perform_ActionThrowsExceptionValidPathFileExists_OriginalFileRevertedAndExceptionThrown()
        {
            // Setup
            var writableDirectory = Path.Combine(testWorkDir, "Writable");
            var filePath = Path.Combine(writableDirectory, "iDoExist.txt");
            var temporaryFilePath = filePath + "~";
            var testContent = "Some test text to write into file.";

            Directory.CreateDirectory(writableDirectory);
            File.WriteAllText(filePath, testContent);

            var writer = new BackedUpFileWriter(filePath);
            var exception = new IOException();

            // Precondition
            Assert.IsTrue(File.Exists(filePath));

            try
            {
                // Call
                TestDelegate test = () => writer.Perform(() => { throw exception; });

                var actualException = Assert.Throws(exception.GetType(), test);
                Assert.AreSame(exception, actualException);

                Assert.IsFalse(File.Exists(temporaryFilePath));
                Assert.IsTrue(File.Exists(filePath));
                Assert.AreEqual(testContent, File.ReadAllText(filePath));
            }
            finally
            {
                Directory.Delete(writableDirectory, true);
            }
        }

        [Test]
        public void Perform_ValidPathFileExistsTemporaryFileExistsAndCannotBeDeleted_ThrowsIOException()
        {
            // Setup
            var filePath = Path.Combine(testWorkDir, "iDoExist.txt");
            var temporaryFilePath = filePath + "~";

            using (File.Create(filePath)) {}
            var temporaryFileStream = File.Create(temporaryFilePath);

            var writer = new BackedUpFileWriter(filePath);

            // Precondition
            Assert.IsTrue(File.Exists(filePath));
            Assert.IsTrue(File.Exists(temporaryFilePath));

            // Call
            TestDelegate test = () => writer.Perform(() => { });

            try
            {
                // Assert
                var message = Assert.Throws<IOException>(test).Message;
                var expectedMessage = string.Format(
                    "Er bestaat al een tijdelijk bestand ({0}) dat niet verwijderd kan worden. Dit bestand dient handmatig verwijderd te worden.",
                    temporaryFilePath);
                Assert.AreEqual(message, expectedMessage);
                temporaryFileStream.Dispose();
            }
            finally
            {
                File.Delete(filePath);
                File.Delete(temporaryFilePath);
            }
        }

        [Test]
        public void Perform_ValidPathFileExistsDirectoryNotWritable_ThrowsIOException()
        {
            // Setup
            var notWritableDirectory = Path.Combine(testWorkDir, "NotWritable");
            var filePath = Path.Combine(notWritableDirectory, "iDoExist.txt");

            Directory.CreateDirectory(notWritableDirectory);
            using (File.Create(filePath)) {}
            var writer = new BackedUpFileWriter(filePath);

            // Precondition
            Assert.IsTrue(File.Exists(filePath));

            // Call
            TestDelegate test = () => writer.Perform(() => { });

            try
            {
                using (new DirectoryPermissionsRevoker(notWritableDirectory, FileSystemRights.Write))
                {
                    // Assert
                    var expectedMessage = string.Format("Kan geen tijdelijk bestand maken van het originele bestand ({0}).",
                                                        filePath);
                    var message = Assert.Throws<IOException>(test).Message;
                    Assert.AreEqual(expectedMessage, message);
                }
            }
            finally
            {
                Directory.Delete(notWritableDirectory, true);
            }
        }

        [Test]
        public void Perform_TargetFileDoesNotExistDeleteRightsRevoked_DoesNotThrow()
        {
            // Setup
            var noAccessDirectory = Path.Combine(testWorkDir, "NoAccess");
            var filePath = Path.Combine(noAccessDirectory, "iDoNotExist.txt");

            Directory.CreateDirectory(noAccessDirectory);
            var helper = new BackedUpFileWriter(filePath);

            // Call
            TestDelegate test = () => helper.Perform(() => { });

            try
            {
                using (new DirectoryPermissionsRevoker(noAccessDirectory, FileSystemRights.Delete))
                {
                    // Assert
                    Assert.DoesNotThrow(test);
                }
            }
            finally
            {
                Directory.Delete(noAccessDirectory, true);
            }
        }

        [Test]
        public void Perform_TargetFileExistsCannotDeleteFile_ThrowsCannotDeleteBackupFileException()
        {
            // Setup
            var noAccessDirectory = Path.Combine(testWorkDir, "Access");
            var filePath = Path.Combine(noAccessDirectory, "iDoExist.txt");
            var temporaryFilePath = filePath + "~";

            Directory.CreateDirectory(noAccessDirectory);
            using (File.Create(filePath)) {}

            var helper = new BackedUpFileWriter(filePath);

            FileStream fileStream = null;

            try
            {
                // Call
                TestDelegate test = () => helper.Perform(() => { fileStream = File.Open(temporaryFilePath, FileMode.Open); });

                // Assert
                var expectedMessage = string.Format(
                    "Kan het tijdelijke bestand ({0}) niet opruimen. Het tijdelijke bestand dient handmatig verwijderd te worden.",
                    temporaryFilePath);
                var message = Assert.Throws<CannotDeleteBackupFileException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
                Directory.Delete(noAccessDirectory, true);
            }
        }

        [Test]
        public void Perform_ActionThrowsExceptionTargetFileExistsCannotDeleteFile_ThrowsIOException()
        {
            // Setup
            var noAccessDirectory = Path.Combine(testWorkDir, "Access");
            var filePath = Path.Combine(noAccessDirectory, "iDoExist.txt");
            var temporaryFilePath = filePath + "~";

            Directory.CreateDirectory(noAccessDirectory);
            using (File.Create(filePath)) {}

            var helper = new BackedUpFileWriter(filePath);

            FileStream fileStream = null;

            try
            {
                // Call
                TestDelegate test = () => helper.Perform(() =>
                {
                    fileStream = File.Open(temporaryFilePath, FileMode.Open);
                    throw new Exception();
                });

                // Assert
                var expectedMessage = string.Format(
                    "Kan het originele bestand ({0}) niet herstellen. Het tijdelijke bestand dient handmatig hersteld te worden.",
                    filePath);
                var message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
                Directory.Delete(noAccessDirectory, true);
            }
        }

        [Test]
        public void Perform_ActionThrowsExceptionTargetFileExistsCannotMoveFile_ThrowsIOException()
        {
            // Setup
            var noAccessDirectory = Path.Combine(testWorkDir, "NoAccess");
            var filePath = Path.Combine(noAccessDirectory, "iDoExist.txt");

            Directory.CreateDirectory(noAccessDirectory);
            using (File.Create(filePath)) {}

            var helper = new BackedUpFileWriter(filePath);

            DirectoryPermissionsRevoker fileRightsHelper = null;
            try
            {
                // Call
                TestDelegate test = () => helper.Perform(() =>
                {
                    fileRightsHelper = new DirectoryPermissionsRevoker(noAccessDirectory, FileSystemRights.FullControl);
                    throw new Exception();
                });

                // Assert
                var expectedMessage = string.Format(
                    "Kan het originele bestand ({0}) niet herstellen. Het tijdelijke bestand dient handmatig hersteld te worden.",
                    filePath);
                var message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
            finally
            {
                if (fileRightsHelper != null)
                {
                    fileRightsHelper.Dispose();
                }
                Directory.Delete(noAccessDirectory, true);
            }
        }
    }
}
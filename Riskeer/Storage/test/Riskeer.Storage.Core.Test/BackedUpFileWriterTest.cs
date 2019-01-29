// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Storage.Core.Exceptions;

namespace Riskeer.Storage.Core.Test
{
    [TestFixture]
    public class BackedUpFileWriterTest
    {
        private readonly string testWorkDir = TestHelper.GetScratchPadPath(nameof(BackedUpFileWriterTest));

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
            string writableDirectory = Path.Combine(testWorkDir, nameof(Perform_ValidFile_TemporaryFileCreatedFromOriginalAndDeletedAfterwards));
            string filePath = Path.Combine(writableDirectory, "iDoExist.txt");
            string temporaryFilePath = filePath + "~";
            const string testContent = "Some test text to write into file.";

            using (new DirectoryDisposeHelper(testWorkDir, nameof(Perform_ValidFile_TemporaryFileCreatedFromOriginalAndDeletedAfterwards)))
            using (new FileDisposeHelper(filePath))
            using (new FileDisposeHelper(temporaryFilePath))
            {
                File.WriteAllText(filePath, testContent);

                var writer = new BackedUpFileWriter(filePath);

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
        }

        [Test]
        public void Perform_ActionThrowsExceptionValidPathFileExists_OriginalFileRevertedAndExceptionThrown()
        {
            // Setup
            string writableDirectory = Path.Combine(testWorkDir, nameof(Perform_ActionThrowsExceptionValidPathFileExists_OriginalFileRevertedAndExceptionThrown));
            string filePath = Path.Combine(writableDirectory, "iDoExist.txt");
            string temporaryFilePath = filePath + "~";
            const string testContent = "Some test text to write into file.";

            using (new DirectoryDisposeHelper(testWorkDir, nameof(Perform_ActionThrowsExceptionValidPathFileExists_OriginalFileRevertedAndExceptionThrown)))
            {
                File.WriteAllText(filePath, testContent);

                var writer = new BackedUpFileWriter(filePath);
                var exception = new IOException();

                // Precondition
                Assert.IsTrue(File.Exists(filePath));

                // Call
                TestDelegate test = () => writer.Perform(() =>
                {
                    throw exception;
                });

                Exception actualException = Assert.Throws(exception.GetType(), test);
                Assert.AreSame(exception, actualException);

                Assert.IsFalse(File.Exists(temporaryFilePath));
                Assert.IsTrue(File.Exists(filePath));
                Assert.AreEqual(testContent, File.ReadAllText(filePath));
            }
        }

        [Test]
        public void Perform_ValidPathFileExistsTemporaryFileExistsAndCannotBeDeleted_ThrowsIOException()
        {
            // Setup
            string filePath = Path.Combine(testWorkDir, "iDoExist.txt");
            string temporaryFilePath = filePath + "~";

            using (new FileDisposeHelper(filePath))
            using (var tempFileHelper = new FileDisposeHelper(temporaryFilePath))
            {
                tempFileHelper.LockFiles();

                var writer = new BackedUpFileWriter(filePath);

                // Call
                TestDelegate test = () => writer.Perform(() => {});

                // Assert
                string message = Assert.Throws<IOException>(test).Message;
                string expectedMessage = $"Er bestaat al een tijdelijk bestand ({temporaryFilePath}) dat niet verwijderd kan worden. Dit bestand dient handmatig verwijderd te worden.";
                Assert.AreEqual(message, expectedMessage);
            }
        }

        [Test]
        public void Perform_ValidPathFileExistsDirectoryNotWritable_ThrowsIOException()
        {
            // Setup
            string notWritableDirectory = Path.Combine(testWorkDir, nameof(Perform_ValidPathFileExistsDirectoryNotWritable_ThrowsIOException));
            string filePath = Path.Combine(notWritableDirectory, "iDoExist.txt");

            using (var directoryHelper = new DirectoryDisposeHelper(testWorkDir, nameof(Perform_ValidPathFileExistsDirectoryNotWritable_ThrowsIOException)))
            using (new FileDisposeHelper(filePath))
            {
                var writer = new BackedUpFileWriter(filePath);

                // Call
                TestDelegate test = () => writer.Perform(() => {});

                directoryHelper.LockDirectory(FileSystemRights.Write);
                // Assert
                string expectedMessage = $"Kan geen tijdelijk bestand maken van het originele bestand ({filePath}).";
                string message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void Perform_TargetFileDoesNotExistDeleteRightsRevoked_DoesNotThrow()
        {
            // Setup
            string noAccessDirectory = Path.Combine(testWorkDir, nameof(Perform_TargetFileDoesNotExistDeleteRightsRevoked_DoesNotThrow));
            string filePath = Path.Combine(noAccessDirectory, "iDoNotExist.txt");

            using (var directoryHelper = new DirectoryDisposeHelper(testWorkDir, nameof(Perform_TargetFileDoesNotExistDeleteRightsRevoked_DoesNotThrow)))
            {
                var helper = new BackedUpFileWriter(filePath);

                // Call
                TestDelegate test = () => helper.Perform(() => {});

                directoryHelper.LockDirectory(FileSystemRights.Delete);
                // Assert
                Assert.DoesNotThrow(test);
            }
        }

        [Test]
        public void Perform_TargetFileExistsCannotDeleteFile_ThrowsCannotDeleteBackupFileException()
        {
            // Setup
            string filePath = Path.Combine(testWorkDir, nameof(Perform_TargetFileExistsCannotDeleteFile_ThrowsCannotDeleteBackupFileException));
            string temporaryFilePath = filePath + "~";

            using (new FileDisposeHelper(filePath))
            using (var temporaryFileHelper = new FileDisposeHelper(temporaryFilePath))
            {
                var helper = new BackedUpFileWriter(filePath);

                // Call
                TestDelegate test = () => helper.Perform(() => temporaryFileHelper.LockFiles());

                // Assert
                string expectedMessage = $"Kan het tijdelijke bestand ({temporaryFilePath}) niet opruimen. Het tijdelijke bestand dient handmatig verwijderd te worden.";
                string message = Assert.Throws<CannotDeleteBackupFileException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void Perform_ActionThrowsExceptionTargetFileExistsCannotDeleteFile_ThrowsIOException()
        {
            // Setup
            string filePath = Path.Combine(testWorkDir, nameof(Perform_ActionThrowsExceptionTargetFileExistsCannotDeleteFile_ThrowsIOException));
            string temporaryFilePath = filePath + "~";

            using (new FileDisposeHelper(filePath))
            using (var temporaryFileHelper = new FileDisposeHelper(temporaryFilePath))
            {
                var helper = new BackedUpFileWriter(filePath);

                // Call
                TestDelegate test = () => helper.Perform(() =>
                {
                    temporaryFileHelper.LockFiles();
                    throw new Exception();
                });

                // Assert
                string expectedMessage = $"Kan het originele bestand ({filePath}) niet herstellen. Het tijdelijke bestand dient handmatig hersteld te worden.";
                string message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void Perform_ActionThrowsExceptionTargetFileExistsCannotMoveFile_ThrowsIOException()
        {
            // Setup
            string noAccessDirectory = Path.Combine(testWorkDir, nameof(Perform_ActionThrowsExceptionTargetFileExistsCannotMoveFile_ThrowsIOException));
            string filePath = Path.Combine(noAccessDirectory, "iDoExist.txt");

            using (var directoryHelper = new DirectoryDisposeHelper(testWorkDir, nameof(Perform_ActionThrowsExceptionTargetFileExistsCannotMoveFile_ThrowsIOException)))
            using (new FileDisposeHelper(filePath))
            {
                var helper = new BackedUpFileWriter(filePath);

                // Call
                TestDelegate test = () => helper.Perform(() =>
                {
                    directoryHelper.LockDirectory(FileSystemRights.FullControl);
                    throw new Exception();
                });

                // Assert
                string expectedMessage = $"Kan het originele bestand ({filePath}) niet herstellen. Het tijdelijke bestand dient handmatig hersteld te worden.";
                string message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            Directory.CreateDirectory(testWorkDir);
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {
            Directory.Delete(testWorkDir, true);
        }
    }
}
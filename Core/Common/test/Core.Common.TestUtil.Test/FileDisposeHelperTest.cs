// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class FileDisposeHelperTest
    {
        private readonly string workingDirectory = TestHelper.GetScratchPadPath(nameof(FileDisposeHelperTest));
        private DirectoryDisposeHelper directoryDisposeHelper;

        [Test]
        public void Constructor_NotExistingFile_DoesNotThrowException()
        {
            // Setup
            string filePath = Path.Combine(workingDirectory, nameof(Constructor_NotExistingFile_DoesNotThrowException));
            FileDisposeHelper disposeHelper = null;

            // Precondition
            Assert.IsFalse(File.Exists(filePath), $"Precondition failed: File '{filePath}' should not exist");

            // Call
            TestDelegate test = () => disposeHelper = new FileDisposeHelper(filePath);

            // Assert
            Assert.DoesNotThrow(test);
            disposeHelper.Dispose();
        }

        [Test]
        public void Constructor_ExistingFile_DoesNotThrowException()
        {
            // Setup
            string filePath = Path.Combine(workingDirectory, nameof(Constructor_ExistingFile_DoesNotThrowException));
            FileDisposeHelper disposeHelper = null;

            try
            {
                using (File.Create(filePath)) {}

                // Call
                TestDelegate test = () => disposeHelper = new FileDisposeHelper(filePath);

                // Assert
                Assert.DoesNotThrow(test);
                disposeHelper.Dispose();
            }
            catch (Exception exception)
            {
                File.Delete(filePath);
                Assert.Fail(exception.Message);
            }

            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Constructor_FileDoesNotExist_Createsfile()
        {
            // Setup
            string filePath = Path.Combine(workingDirectory, nameof(Constructor_FileDoesNotExist_Createsfile));

            try
            {
                // Precondition
                Assert.IsFalse(File.Exists(filePath));

                // Call
                using (new FileDisposeHelper(filePath))
                {
                    // Assert
                    Assert.IsTrue(File.Exists(filePath));
                }
            }
            catch (Exception exception)
            {
                File.Delete(filePath);
                Assert.Fail(exception.Message);
            }

            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Constructor_InvalidPath_DoesNotThrowException()
        {
            // Setup
            string filePath = string.Empty;

            // Call
            TestDelegate test = () => new FileDisposeHelper(filePath);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Constructor_MultipleFiles_CreatesFiles()
        {
            // Setup
            string[] filePaths =
            {
                Path.Combine(workingDirectory, $"{nameof(Constructor_MultipleFiles_CreatesFiles)}_0"),
                Path.Combine(workingDirectory, $"{nameof(Constructor_MultipleFiles_CreatesFiles)}_1")
            };

            try
            {
                // Call
                using (new FileDisposeHelper(filePaths))
                {
                    // Assert
                    foreach (string filePath in filePaths)
                    {
                        Assert.IsTrue(File.Exists(filePath));
                    }
                }
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void Constructor_FilePathThatCannotBeCreated_ThrowsArgumentException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("nonExistingPath", "fileThatCannotBeCreated"));

            // Call
            TestDelegate test = () => new FileDisposeHelper(filePath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Constructor_FilePathsThatCannotBeCreated_ThrowsArgumentException()
        {
            // Setup
            string[] filePaths =
            {
                TestHelper.GetScratchPadPath(Path.Combine("nonExistingPath", "fileThatCannotBeCreated"))
            };

            // Call
            TestDelegate test = () => new FileDisposeHelper(filePaths);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void LockFiles_ValidFilePath_LocksFile()
        {
            // Setup
            string filePath = Path.Combine(workingDirectory, nameof(LockFiles_ValidFilePath_LocksFile));

            try
            {
                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                {
                    // Call
                    fileDisposeHelper.LockFiles();

                    // Assert
                    Assert.IsFalse(IsFileWritable(filePath), $"'{filePath}' is not locked for writing.");
                }

                Assert.IsFalse(File.Exists(filePath), $"'{filePath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void LockFiles_FileAlreadyLocked_ThrowsInvalidOperationException()
        {
            // Setup
            string filePath = Path.Combine(workingDirectory, nameof(LockFiles_FileAlreadyLocked_ThrowsInvalidOperationException));

            try
            {
                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                using (File.OpenWrite(filePath))
                {
                    // Call
                    TestDelegate call = () => fileDisposeHelper.LockFiles();

                    // Assert
                    var exception = Assert.Throws<InvalidOperationException>(call);
                    Assert.AreEqual($"Unable to lock '{filePath}'.", exception.Message);
                    Assert.IsNotNull(exception.InnerException);
                }
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void LockFiles_DirectoryAlreadyDeleted_ThrowsInvalidOperationException()
        {
            // Setup
            string directoryPath = Path.Combine(workingDirectory, Path.GetRandomFileName());
            string filePath = Path.Combine(directoryPath, nameof(LockFiles_DirectoryAlreadyDeleted_ThrowsInvalidOperationException));

            try
            {
                Directory.CreateDirectory(directoryPath);
                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                {
                    Directory.Delete(directoryPath, true);

                    // Precondition
                    Assert.IsFalse(Directory.Exists(directoryPath), $"'{directoryPath}' should have been deleted.");

                    // Call
                    TestDelegate call = () => fileDisposeHelper.LockFiles();

                    // Assert
                    var exception = Assert.Throws<InvalidOperationException>(call);
                    Assert.AreEqual($"Unable to lock '{filePath}'.", exception.Message);
                    Assert.IsNotNull(exception.InnerException);
                }
            }
            catch (Exception exception)
            {
                if (Directory.Exists(filePath))
                {
                    Directory.Delete(directoryPath, true);
                    Assert.Fail($"File was not deleted: '{filePath}': {exception}");
                }
            }
        }

        [Test]
        public void LockedFiles_LockFilesCalledAlready_DoesNotThrowException()
        {
            // Setup
            string filePath = Path.Combine(workingDirectory, nameof(LockedFiles_LockFilesCalledAlready_DoesNotThrowException));

            try
            {
                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                {
                    fileDisposeHelper.LockFiles();

                    // Call
                    TestDelegate call = () => fileDisposeHelper.LockFiles();

                    // Assert
                    Assert.DoesNotThrow(call);
                }

                Assert.IsFalse(File.Exists(filePath), $"'{filePath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void Dispose_InvalidPath_DoesNotThrowException()
        {
            // Setup
            string filePath = string.Empty;

            // Call
            TestDelegate test = () => new FileDisposeHelper(filePath);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Dispose_ExistingFile_DeletesFile()
        {
            // Setup
            string filePath = Path.Combine(workingDirectory, nameof(Dispose_ExistingFile_DeletesFile));

            try
            {
                using (File.Create(filePath)) {}

                // Precondition
                Assert.IsTrue(File.Exists(filePath));

                // Call
                using (new FileDisposeHelper(filePath)) {}
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            // Assert
            Assert.IsFalse(File.Exists(filePath), $"'{filePath}' should have been deleted.");
        }

        [Test]
        public void Dispose_MultipleFiles_DeletesFiles()
        {
            // Setup
            string[] filePaths =
            {
                Path.Combine(workingDirectory, $"{nameof(Dispose_MultipleFiles_DeletesFiles)}_0"),
                Path.Combine(workingDirectory, $"{nameof(Dispose_MultipleFiles_DeletesFiles)}_1")
            };

            try
            {
                foreach (string filePath in filePaths)
                {
                    using (File.Create(filePath)) {}

                    // Precondition
                    Assert.IsTrue(File.Exists(filePath));
                }

                // Call
                using (new FileDisposeHelper(filePaths)) {}
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            // Assert
            foreach (string filePath in filePaths)
            {
                Assert.IsFalse(File.Exists(filePath), $"'{filePath}' should have been deleted.");
            }
        }

        [Test]
        public void Dispose_FileAlreadyDeleted_DoesNotThrowException()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath("willBeRemoved");
            string filePath = Path.Combine(directoryPath, Path.GetRandomFileName());

            // Call
            try
            {
                Directory.CreateDirectory(directoryPath);
                using (new FileDisposeHelper(filePath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            finally
            {
                // Assert
                if (Directory.Exists(filePath))
                {
                    File.Delete(filePath);
                    Directory.Delete(directoryPath, true);
                    Assert.Fail("File was not deleted: {0}", filePath);
                }
            }
        }

        [Test]
        public void Dispose_FileInUse_DoesNotThrowIOException()
        {
            // Setup
            string filePath = Path.Combine(workingDirectory, nameof(Dispose_FileInUse_DoesNotThrowIOException));

            var disposeHelper = new FileDisposeHelper(filePath);

            using (File.OpenWrite(filePath))
            {
                // Call
                TestDelegate test = () => disposeHelper.Dispose();

                // Assert
                Assert.DoesNotThrow(test);
                Assert.True(File.Exists(filePath), $"'{filePath}' should still exist.");
            }
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(FileDisposeHelperTest));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            directoryDisposeHelper.Dispose();
        }

        private static bool IsFileWritable(string filePath)
        {
            try
            {
                using (File.OpenWrite(filePath)) {}
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }
    }
}
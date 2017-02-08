// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        private static readonly TestDataPath testPath = TestDataPath.Core.Common.TestUtils;

        [Test]
        public void Constructor_NotExistingFile_DoesNotThrowException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());
            FileDisposeHelper disposeHelper = null;

            // Precondition
            Assert.IsFalse(File.Exists(filePath), $"Precondition failed: File '{filePath}' should not exist");

            // Call
            TestDelegate test = () => disposeHelper = new FileDisposeHelper(filePath);

            // Assert
            Assert.DoesNotThrow(test);
            disposeHelper.Dispose();
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Constructor_ExistingFile_DoesNotThrowException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());
            FileDisposeHelper disposeHelper = null;

            try
            {
                using (File.Create(filePath)) {}

                // Precondition
                Assert.IsTrue(File.Exists(filePath), $"Precondition failed: File '{filePath}' should exist");

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
            string filePath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());

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
            var filePath = string.Empty;

            // Call
            TestDelegate test = () => new FileDisposeHelper(filePath);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Constructor_MultipleFiles_CreatesFiles()
        {
            // Setup
            var filePaths = new[]
            {
                TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName()),
                TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName())
            };

            try
            {
                // Call
                using (new FileDisposeHelper(filePaths))
                {
                    // Assert
                    foreach (var filePath in filePaths)
                    {
                        Assert.IsTrue(File.Exists(filePath));
                    }
                }
            }
            catch (Exception exception)
            {
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void Constructor_FilePathThatCannotBeCreated_ThrowsArgumentException()
        {
            // Setup
            string filePath = Path.Combine(TestHelper.GetTestDataPath(testPath), "nonExistingPath", "fileThatCannotBeCreated");

            // Call
            TestDelegate test = () => new FileDisposeHelper(filePath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Constructor_FilePathsThatCannotBeCreated_ThrowsArgumentException()
        {
            // Setup
            var filePaths = new[]
            {
                Path.Combine(TestHelper.GetTestDataPath(testPath), "nonExistingPath", "fileThatCannotBeCreated")
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
            string filePath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());

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
                File.Delete(filePath);
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void LockFiles_FileAlreadyLocked_ThrowsInvalidOperationException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());

            try
            {
                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                using (File.OpenWrite(filePath))
                {
                    // Call
                    TestDelegate call = () => fileDisposeHelper.LockFiles();

                    // Assert
                    InvalidOperationException exception = Assert.Throws<InvalidOperationException>(call);
                    Assert.AreEqual($"Unable to lock '{filePath}'.", exception.Message);
                    Assert.IsNotNull(exception.InnerException);
                }
            }
            catch (Exception exception)
            {
                File.Delete(filePath);
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void LockFiles_DirectoryAlreadyDeleted_ThrowsInvalidOperationException()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());
            string filePath = Path.Combine(directoryPath, Path.GetRandomFileName());

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
                    InvalidOperationException exception = Assert.Throws<InvalidOperationException>(call);
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
        public void GivenFileDisposeHelperWithLockedFiles_LockFilesCalledAgain_DoesNotThrowException()
        {
            // Given
            string filePath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());

            try
            {
                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                {
                    fileDisposeHelper.LockFiles();

                    // When
                    TestDelegate call = () => fileDisposeHelper.LockFiles();

                    // Then
                    Assert.DoesNotThrow(call);
                }

                Assert.IsFalse(File.Exists(filePath), $"'{filePath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                File.Delete(filePath);
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
            string filePath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());

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
                File.Delete(filePath);
                Assert.Fail(exception.Message);
            }

            // Assert
            Assert.IsFalse(File.Exists(filePath), $"'{filePath}' should have been deleted.");
        }

        [Test]
        public void Dispose_MultipleFiles_DeletesFiles()
        {
            // Setup
            var filePaths = new[]
            {
                TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName()),
                TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName())
            };

            try
            {
                foreach (var filePath in filePaths)
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
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
                Assert.Fail(exception.Message);
            }

            // Assert
            foreach (var filePath in filePaths)
            {
                Assert.IsFalse(File.Exists(filePath), $"'{filePath}' should have been deleted.");
            }
        }

        [Test]
        public void Dispose_FileAlreadyDeleted_DoesNotThrowException()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(testPath, "willBeRemoved");
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
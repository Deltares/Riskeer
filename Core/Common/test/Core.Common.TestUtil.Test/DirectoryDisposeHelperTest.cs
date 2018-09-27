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
using System.Security.AccessControl;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class DirectoryDisposeHelperTest
    {
        private static readonly string rootFolder = TestHelper.GetScratchPadPath();

        [Test]
        public void Constructor_NullRoot_ThrowsArgumentNullException()
        {
            // Setup
            const string subfolder = "sub folder";

            // Call
            TestDelegate test = () => new DirectoryDisposeHelper(null, subfolder);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("rootFolder", paramName);
        }

        [Test]
        public void Constructor_NullSubfolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new DirectoryDisposeHelper(rootFolder, null);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("subFolders", exception.ParamName);
        }

        [Test]
        public void Constructor_NotExistingFolder_CreatesFolder()
        {
            // Setup
            string subFolder = Path.GetRandomFileName();
            string folderPath = Path.Combine(rootFolder, subFolder);
            bool folderExists;

            // Precondition
            Assert.IsFalse(Directory.Exists(folderPath), $"Precondition failed: Folder '{folderPath}' should not exist");

            // Call
            using (new DirectoryDisposeHelper(rootFolder, subFolder))
            {
                folderExists = Directory.Exists(folderPath);
            }

            // Assert
            Assert.IsTrue(folderExists);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Assert.Fail($"Folder path '{folderPath}' was not removed.");
            }
        }

        [Test]
        public void Constructor_NotExistingFolders_CreatesFolders()
        {
            // Setup
            string subFolder = Path.GetRandomFileName();
            string subSubFolder = Path.GetRandomFileName();
            string folderPath = Path.Combine(rootFolder, subFolder, subSubFolder);
            bool folderExists;

            // Precondition
            Assert.IsFalse(Directory.Exists(folderPath), $"Precondition failed: Folder '{folderPath}' should not exist");

            // Call
            using (new DirectoryDisposeHelper(rootFolder, subFolder, subSubFolder))
            {
                folderExists = Directory.Exists(folderPath);
            }

            // Assert
            Assert.IsTrue(folderExists);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Assert.Fail($"Folder path '{folderPath}' was not removed.");
            }
        }

        [Test]
        public void Constructor_ExistingFolder_DoesNotThrowException()
        {
            // Setup
            string subFolder = Path.GetRandomFileName();
            string folderPath = Path.Combine(rootFolder, subFolder);

            DirectoryDisposeHelper disposeHelper = null;
            try
            {
                Directory.CreateDirectory(folderPath);

                // Call
                TestDelegate test = () => disposeHelper = new DirectoryDisposeHelper(rootFolder, subFolder);

                // Assert
                Assert.DoesNotThrow(test);
                disposeHelper.Dispose();
            }
            finally
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    Assert.Fail($"Folder path '{folderPath}' was not removed.");
                }
            }
        }

        [Test]
        [TestCase("/fo:der/", "valid")]
        [TestCase("valid", "/fo:der/")]
        [TestCase("f*lder", "valid")]
        [TestCase("valid", "f*lder")]
        public void Constructor_InvalidFolderPath_ThrowsArgumentException(string someRootFolder, string subfolder)
        {
            // Call
            TestDelegate test = () => new DirectoryDisposeHelper(someRootFolder, subfolder);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            string subfolder = Path.GetRandomFileName();

            // Call
            TestDelegate test = () =>
            {
                using (var directoryDisposeHelper = new DirectoryDisposeHelper(rootFolder, subfolder))
                {
                    directoryDisposeHelper.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Dispose_FolderAlreadyRemoved_DoesNotThrowException()
        {
            // Setup
            string subfolder = Path.GetRandomFileName();
            string folderPath = Path.Combine(rootFolder, subfolder);

            // Call
            TestDelegate test = () =>
            {
                using (new DirectoryDisposeHelper(rootFolder, subfolder))
                {
                    Directory.Delete(folderPath, true);
                }
            };

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Dispose_FolderInUse_DoesNotThrowException()
        {
            // Setup
            string subfolder = Path.GetRandomFileName();

            // Call
            TestDelegate test = () =>
            {
                using (var directoryDisposeHelper1 = new DirectoryDisposeHelper(rootFolder, subfolder))
                {
                    directoryDisposeHelper1.LockDirectory(FileSystemRights.Write);

                    using (new DirectoryDisposeHelper(rootFolder, subfolder)) {} // Dispose will face the locked directory
                }
            };

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void LockDirectory_ValidPath_LocksDirectory()
        {
            // Setup
            string subfolder = Path.GetRandomFileName();
            string folderPath = Path.Combine(rootFolder, subfolder);

            try
            {
                using (var disposeHelper = new DirectoryDisposeHelper(rootFolder, subfolder))
                {
                    // Call
                    disposeHelper.LockDirectory(FileSystemRights.Write);

                    // Assert
                    Assert.IsFalse(TestHelper.CanWriteInDirectory(folderPath), $"'{folderPath}' should have been locked for writing.");
                }

                Assert.IsFalse(Directory.Exists(folderPath), $"'{folderPath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                RemoveDirectoryAndFail(folderPath, exception);
            }
        }

        [Test]
        public void LockDirectory_RightNotSupported_ThrowsInvalidOperationException()
        {
            // Setup
            string subfolder = Path.GetRandomFileName();
            string folderPath = Path.Combine(rootFolder, subfolder);

            try
            {
                using (var disposeHelper = new DirectoryDisposeHelper(rootFolder, subfolder))
                using (new DirectoryPermissionsRevoker(folderPath, FileSystemRights.Write))
                {
                    // Call
                    TestDelegate call = () => disposeHelper.LockDirectory(FileSystemRights.Synchronize);

                    // Assert
                    var exception = Assert.Throws<InvalidOperationException>(call);
                    Assert.AreEqual($"Unable to lock '{folderPath}'.", exception.Message);
                    Assert.IsNotNull(exception.InnerException);
                }

                Assert.IsFalse(Directory.Exists(folderPath), $"'{folderPath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                RemoveDirectoryAndFail(folderPath, exception);
            }
        }

        [Test]
        public void LockDirectory_DirectoryAlreadyLocked_DoesNotThrowException()
        {
            // Setup
            string subfolder = Path.GetRandomFileName();
            string folderPath = Path.Combine(rootFolder, subfolder);

            try
            {
                using (var disposeHelper = new DirectoryDisposeHelper(rootFolder, subfolder))
                using (new DirectoryPermissionsRevoker(folderPath, FileSystemRights.Write))
                {
                    // Call
                    TestDelegate call = () => disposeHelper.LockDirectory(FileSystemRights.Write);

                    // Assert
                    Assert.DoesNotThrow(call);
                }

                Assert.IsFalse(Directory.Exists(folderPath), $"'{folderPath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                RemoveDirectoryAndFail(folderPath, exception);
            }
        }

        [Test]
        public void UnlockDirectory_DirectoryNotLocked_UnlocksDirectory()
        {
            // Setup
            const string subfolder = nameof(UnlockDirectory_DirectoryNotLocked_UnlocksDirectory);
            string folderPath = Path.Combine(rootFolder, subfolder);

            using (var disposeHelper = new DirectoryDisposeHelper(rootFolder, subfolder))
            {
                // Call
                TestDelegate test = () => disposeHelper.UnlockDirectory();

                // Assert
                string actualMessage = Assert.Throws<InvalidOperationException>(test).Message;
                Assert.AreEqual($"Directory '{folderPath}' is not locked.", actualMessage);
            }

            try
            {
                Assert.IsFalse(Directory.Exists(folderPath), $"'{folderPath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                RemoveDirectoryAndFail(folderPath, exception);
            }
        }

        [Test]
        public void UnlockDirectory_DirectoryLocked_UnlocksDirectory()
        {
            // Setup
            const string subfolder = nameof(UnlockDirectory_DirectoryLocked_UnlocksDirectory);
            string folderPath = Path.Combine(rootFolder, subfolder);

            using (var disposeHelper = new DirectoryDisposeHelper(rootFolder, subfolder))
            {
                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                disposeHelper.UnlockDirectory();

                // Assert
                TestDelegate createFile = () =>
                {
                    using (File.Create(folderPath)) {}
                };
                Assert.Throws<UnauthorizedAccessException>(createFile);
            }

            try
            {
                bool exists = Directory.Exists(folderPath);
                Assert.IsFalse(exists, $"'{folderPath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                RemoveDirectoryAndFail(folderPath, exception);
            }
        }

        [Test]
        public void UnlockDirectory_DirectoryAlreadyUnlocked_DoesNotThrowException()
        {
            // Setup
            const string subfolder = nameof(UnlockDirectory_DirectoryAlreadyUnlocked_DoesNotThrowException);
            string folderPath = Path.Combine(rootFolder, subfolder);

            try
            {
                using (var disposeHelper = new DirectoryDisposeHelper(rootFolder, subfolder))
                {
                    disposeHelper.LockDirectory(FileSystemRights.Write);
                    disposeHelper.UnlockDirectory();

                    // Call
                    TestDelegate call = () => disposeHelper.UnlockDirectory();

                    // Assert
                    Assert.DoesNotThrow(call);
                }

                Assert.IsFalse(Directory.Exists(folderPath), $"'{folderPath}' should have been deleted.");
            }
            catch (Exception exception)
            {
                RemoveDirectoryAndFail(folderPath, exception);
            }
        }

        private static void RemoveDirectoryAndFail(string folderPath, Exception exception)
        {
            try
            {
                Directory.Delete(folderPath);
            }
            catch
            {
                // Ignore
            }

            Assert.Fail(exception.Message, exception.InnerException);
        }
    }
}
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
        [Test]
        public void Constructor_NotExistingFile_DoesNotThrowException()
        {
            // Setup
            string filePath = "doesNotExist.tmp";
            FileDisposeHelper disposeHelper = null;

            // Precondition
            Assert.IsFalse(File.Exists(filePath), string.Format("Precondition failed: File '{0}' should not exist", filePath));

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
            string filePath = "doesExist.tmp";
            FileDisposeHelper disposeHelper = null;

            try
            {
                using (File.Create(filePath)) {}

                // Precondition
                Assert.IsTrue(File.Exists(filePath), string.Format("Precondition failed: File '{0}' should exist", filePath));

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
            string filePath = "willExist.tmp";

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
                "willExist.tmp",
                "alsoWillExist.tmp"
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
            string filePath = Path.Combine("nonExistingPath", "fileThatCannotBeCreated");

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
                Path.Combine("nonExistingPath", "fileThatCannotBeCreated")
            };

            // Call
            TestDelegate test = () => new FileDisposeHelper(filePaths);

            // Assert
            Assert.Throws<ArgumentException>(test);
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
            const string filePath = "doesExist.tmp";

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
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Dispose_MultipleFiles_DeletesFiles()
        {
            // Setup
            var filePaths = new[]
            {
                "doesExist.tmp",
                "alsoDoesExist.tmp"
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
                Assert.IsFalse(File.Exists(filePath));
            }
        }

        [Test]
        public void Dispose_FileAlreadyDeleted_DoesNotThrowException()
        {
            // Setup
            const string directoryPath = "willBeRemoved";
            string filePath = Path.Combine(directoryPath, "doesNotExist.tmp");

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
    }
}
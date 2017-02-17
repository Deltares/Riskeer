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
    public class DirectoryDisposeHelperTest
    {
        private static readonly TestDataPath testPath = TestDataPath.Core.Common.TestUtils;

        [Test]
        public void Constructor_NotExistingFolder_CreatesFolder()
        {
            // Setup
            string folderPath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());
            var folderExists = false;

            // Precondition
            Assert.IsFalse(Directory.Exists(folderPath), $"Precondition failed: Folder '{folderPath}' should not exist");

            // Call
            using (new DirectoryDisposeHelper(folderPath))
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
            string folderPath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());
            DirectoryDisposeHelper disposeHelper = null;

            try
            {
                Directory.CreateDirectory(folderPath);

                // Call
                TestDelegate test = () => disposeHelper = new DirectoryDisposeHelper(folderPath);

                // Assert
                Assert.DoesNotThrow(test);
                disposeHelper.Dispose();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
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
        [TestCase("/fo:der/")]
        [TestCase("f*lder")]
        public void Constructor_InvalidFolderPath_ThrowsArgumentException(string folderPath)
        {
            // Call
            TestDelegate test = () => new DirectoryDisposeHelper(folderPath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            string folderPath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());

            // Call
            TestDelegate test = () =>
            {
                using (var directoryDisposeHelper = new DirectoryDisposeHelper(folderPath))
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
            string folderPath = TestHelper.GetTestDataPath(testPath, Path.GetRandomFileName());

            // Call
            TestDelegate test = () =>
            {
                using (new DirectoryDisposeHelper(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
            };

            // Assert
            Assert.DoesNotThrow(test);
        }
    }
}
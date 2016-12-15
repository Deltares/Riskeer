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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class StorageSqliteCreatorTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles");
        private readonly string tempRingtoetsFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles"), "tempProjectFile.rtd");

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("C:\\folder\\")]
        public void CreateDatabaseStructure_InvalidFilePath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate call = () => StorageSqliteCreator.CreateDatabaseStructure(invalidPath);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void CreateDatabaseStructure_WithNonExistingPath_DoesNotThrowException()
        {
            // Setup
            const string fileName = "DoesNotExist.sqlite";
            var fullPath = Path.GetFullPath(Path.Combine(testDataPath, fileName));

            // Precondition
            Assert.IsFalse(File.Exists(fullPath));

            try
            {
                // Call
                TestDelegate call = () => StorageSqliteCreator.CreateDatabaseStructure(fullPath);

                // Assert
                Assert.DoesNotThrow(call);

                Assert.IsTrue(File.Exists(fullPath));
            }
            finally
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }

        [Test]
        public void CreateDatabaseStructure_WithNonExistingNetworkPath_DoesNotThrowException()
        {
            // Setup
            const string fileName = "DoesNotExist.sqlite";
            var fullPath = Path.GetFullPath(Path.Combine(testDataPath, fileName));
            var uncPath = GetUncPath(fullPath);

            // Precondition
            Assert.IsFalse(File.Exists(fullPath));

            try
            {
                // Call
                TestDelegate call = () => StorageSqliteCreator.CreateDatabaseStructure(uncPath);

                // Assert
                Assert.DoesNotThrow(call);

                Assert.IsTrue(File.Exists(fullPath));
            }
            finally
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }

        [Test]
        public void CreateDatabaseStructure_ValidExistingFile_ThrowsStorageException()
        {
            using (new FileDisposeHelper(tempRingtoetsFile))
            {
                // Call
                TestDelegate call = () => StorageSqliteCreator.CreateDatabaseStructure(tempRingtoetsFile);

                ArgumentException exception;
                using (File.Create(tempRingtoetsFile)) // Locks file
                {
                    exception = Assert.Throws<ArgumentException>(call);
                }

                // Assert
                var expectedMessage = string.Format(@"File '{0}' already exists.", tempRingtoetsFile);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        private static string GetUncPath(string fullPath)
        {
            var root = Path.GetPathRoot(fullPath);
            Assert.IsNotNull(root);

            var relativePath = fullPath.Replace(root, "");
            var drive = root.Remove(1);

            var uncPath = new Uri(Path.Combine(@"\\localhost", drive + "$", relativePath));
            return uncPath.LocalPath;
        }
    }
}
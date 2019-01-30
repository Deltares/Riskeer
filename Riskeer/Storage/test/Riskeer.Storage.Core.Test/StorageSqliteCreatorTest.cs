// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.Storage.Core.Test
{
    [TestFixture]
    public class StorageSqliteCreatorTest
    {
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
            string fullPath = TestHelper.GetScratchPadPath(fileName);

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
            string fullPath = TestHelper.GetScratchPadPath(fileName);
            string uncPath = TestHelper.ToUncPath(fullPath);

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
            string tempRingtoetsFile = TestHelper.GetScratchPadPath(nameof(CreateDatabaseStructure_ValidExistingFile_ThrowsStorageException));
            using (var disposeHelper = new FileDisposeHelper(tempRingtoetsFile))
            {
                disposeHelper.LockFiles();

                // Call
                TestDelegate call = () => StorageSqliteCreator.CreateDatabaseStructure(tempRingtoetsFile);

                // Assert
                var exception = Assert.Throws<ArgumentException>(call);
                string expectedMessage = $@"File '{tempRingtoetsFile}' already exists.";
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }
    }
}
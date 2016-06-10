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
    public class StorageSqliteCreatorTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles");

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(@"C:\folder\")]
        public void CreateDatabaseStructure_InvalidFilePath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate call = () => StorageSqliteCreator.CreateDatabaseStructure(invalidPath);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void CreateDatabaseStructure_WithNonExistingNetworkPath_DoesNotThrowException()
        {
            // Setup
            const string fileName = "DoesNotExist.sqlite";

            var fullPath = Path.GetFullPath(Path.Combine(testDataPath, fileName));
            var uncPath = GetUncPath(fullPath);

            using (new FileDisposeHelper(fullPath))
            {
                // Call
                TestDelegate call = () => StorageSqliteCreator.CreateDatabaseStructure(uncPath);

                // Assert
                Assert.DoesNotThrow(call);
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
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

using System.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Migration.Core.Storage.Test
{
    [TestFixture]
    public class MigrationDatabaseSourceFileTest
    {
        [Test]
        public void SourceFile_ParameteredConstructor_ExptectedProperties()
        {
            // Setup
            string targetFilename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, targetFilename);

            using (new FileDisposeHelper(filePath))
            {
                // Call
                using (var sourceFile = new MigrationDatabaseSourceFile(filePath))
                {
                    // Assert
                    Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(sourceFile);
                    Assert.AreEqual(filePath, sourceFile.Path);
                }
            }
        }

        [Test]
        [TestCase("Demo164.rtd", "4")]
        [TestCase("Demo171.rtd", "17.1")]
        public void GetVersion_ParameteredConstructor_ExptectedProperties(string file, string expectedVersion)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, file);

            // Call
            using (var sourceFile = new MigrationDatabaseSourceFile(filePath))
            {
                // Assert
                Assert.AreEqual(expectedVersion, sourceFile.GetVersion());
            }
        }
    }
}
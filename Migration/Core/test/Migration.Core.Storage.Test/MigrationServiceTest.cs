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

namespace Migration.Core.Storage.Test
{
    [TestFixture]
    public class MigrationServiceTest
    {
        [Test]
        public void Execute_SourceAndTargetLocationAreEqual_ThrowsArgumentExeption()
        {
            // Setup
            const string samePath = "c:\\someValidPath";

            // Call
            TestDelegate call = () => MigrationService.Execute(samePath, samePath);

            // Assert
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual($"{samePath} cannot be the same location as {samePath}", message);
        }

        [Test]
        public void Execute_Migrate4To171_MigratesToNewFile()
        {
            // Setup
            string targetFilename = Path.GetRandomFileName();
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, "Demo164.rtd");
            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, targetFilename);

            using (new FileDisposeHelper(targetFilePath))
            {
                // Call
                MigrationService.Execute(sourceFilePath, targetFilePath);

                // Assert
                ValidateFileVersion(targetFilePath, "17.1");
            }
        }

        private static void ValidateFileVersion(string filePath, string expectedVersion)
        {
            using (var source = new MigrationDatabaseSourceFile(filePath))
            {
                Assert.AreEqual(expectedVersion, source.GetVersion());
            }
        }
    }
}
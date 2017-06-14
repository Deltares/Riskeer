// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Application.Ringtoets.Migration.Core;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.IntegrationTests
{
    [TestFixture]
    public class MigrationIntegrationTest
    {
        [Test]
        [TestCase("Empty valid [Release 16.4].rtd", "17.1")]
        [TestCase("Empty valid [Release 17.1].rtd", "17.2")]
        public void GivenProject_WhenMigratingWithSquareBracketsInPath_DoesNotThrowException(
            string sourceFile, string newVersion)
        {
            // Given
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                               sourceFile);
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            const string name = nameof(GivenProject_WhenMigratingWithSquareBracketsInPath_DoesNotThrowException);
            string targetFilePath = TestHelper.GetScratchPadPath(name);
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(name, sourceFile, ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                // When
                TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Then
                Assert.DoesNotThrow(call);
            }
        }

    }
}
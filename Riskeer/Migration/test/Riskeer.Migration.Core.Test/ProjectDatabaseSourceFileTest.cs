// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.Base.Storage;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.Migration.Core.Test
{
    [TestFixture]
    public class ProjectDatabaseSourceFileTest
    {
        private static readonly TestDataPath testPath = TestDataPath.Riskeer.Migration.Core;

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string fileName = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(testPath, fileName);

            // Call
            TestDelegate test = () =>
            {
                using (new ProjectDatabaseSourceFile(filePath)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual($"Fout bij het lezen van bestand '{filePath}': het bestand bestaat niet.",
                            exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string filePath)
        {
            // Call
            TestDelegate test = () =>
            {
                using (new ProjectDatabaseSourceFile(filePath)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual($"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet leeg of ongedefinieerd zijn.",
                            exception.Message);
        }

        [Test]
        public void Constructor_ValidPath_ExpectedProperties()
        {
            // Setup
            string fileName = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(testPath, fileName);

            // Call
            using (new FileDisposeHelper(filePath))
            using (var file = new ProjectDatabaseSourceFile(filePath))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(file);
            }
        }

        [Test]
        public void GetVersion_EmptyFile_ThrowsStorageValidationException()
        {
            // Setup
            string fileName = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(testPath, fileName);

            using (new FileDisposeHelper(filePath))
            using (var file = new ProjectDatabaseSourceFile(filePath))
            {
                // Call
                TestDelegate call = () => file.GetVersion();

                // Assert
                string message = Assert.Throws<StorageValidationException>(call).Message;
                Assert.AreEqual($"Het bestand '{filePath}' moet een geldig Ringtoets projectbestand zijn.",
                                message);
            }
        }

        [Test]
        public void GetVersion_EmptyRingtoetsDatabaseFile_ReturnsEmpty()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(testPath, "EmptyDatabase.rtd");

            using (var file = new ProjectDatabaseSourceFile(filePath))
            {
                // Call
                string version = file.GetVersion();

                // Assert
                Assert.IsEmpty(version);
            }
        }
    }
}
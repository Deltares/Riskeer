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
    public class MigrationDatabaseTargetFileTest
    {
        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_InvalidPath_ThrowsArgumentException(string filePath)
        {
            // Call
            TestDelegate call = () =>
            {
                using (new MigrationDatabaseTargetFile(filePath)) {}
            };

            // Assert
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual($"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet leeg of ongedefinieerd zijn.", message);
        }

        [Test]
        public void Constructor_FileExistsButNotWritable_ThrowsArgumentException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            using (new FileDisposeHelper(filePath))
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                File.SetAttributes(filePath, attributes | FileAttributes.ReadOnly);

                // Call
                TestDelegate call = () =>
                {
                    using (new MigrationDatabaseTargetFile(filePath)) {}
                };

                // Assert
                string message = Assert.Throws<ArgumentException>(call).Message;
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", message);

                File.SetAttributes(filePath, attributes);
            }
        }
    }
}
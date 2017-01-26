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
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Migration.Scripts.Data.Test
{
    [TestFixture]
    public class VersionedFileTest
    {
        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_InvalidPath_ThrowsArgumentException(string filePath)
        {
            // Call
            TestDelegate call = () => new VersionedFile(filePath);

            // Assert
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual($"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet leeg of ongedefinieerd zijn.", message);
        }

        [Test]
        public void Constructor_ValidfilePath_ExpectedProperties()
        {
            // Setup
            string filePath = "c:\\file.ext";

            // Call
            var versionedFile = new VersionedFile(filePath);

            // Assert
            Assert.AreEqual(filePath, versionedFile.Location);
        }

        [Test]
        public void GetVersion_FileDoesNotExist_ThrowsCriticalFileReadException()
        {
            // Setup
            string file = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, file);

            var sourceFile = new VersionedFile(filePath);

            // Precondition
            Assert.IsFalse(File.Exists(filePath), $"File should not exist at location '{filePath}'");

            // Call
            TestDelegate call = () => sourceFile.GetVersion();

            // Assert
            Assert.Throws<CriticalFileReadException>(call);
        }

        [Test]
        public void GetVersion_FileCannotBeRead_ThrowsCriticalFileReadException()
        {
            // Setup
            string file = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, file);

            var sourceFile = new VersionedFile(filePath);

            using (new FileDisposeHelper(filePath))
            using (File.Create(filePath))
            {
                // Call
                TestDelegate call = () => sourceFile.GetVersion();

                // Assert
                Assert.Throws<CriticalFileReadException>(call);
            }
        }
    }
}
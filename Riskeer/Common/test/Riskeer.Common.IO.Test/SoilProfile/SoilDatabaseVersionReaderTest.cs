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
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Riskeer.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilDatabaseVersionReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(SoilDatabaseVersionReader));

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            TestDelegate test = () =>
            {
                using (new SoilDatabaseVersionReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_InvalidPath_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () =>
            {
                using (new SoilDatabaseVersionReader(fileName)) {}
            };

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            // Call
            using (var reader = new SoilDatabaseVersionReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Constructor_IncorrectSchema_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "incorrectSchema.soil");
            using (var versionReader = new SoilDatabaseVersionReader(filePath))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyVersion();

                // Assert
                string message = $"Fout bij het lezen van bestand '{filePath}':" +
                                 " kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";

                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(message, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(filePath));
        }

        [Test]
        public void Constructor_IncorrectVersion_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "incorrectVersion.soil");
            using (var versionReader = new SoilDatabaseVersionReader(filePath))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyVersion();

                // Assert
                const string requiredVersion = "17.2.0.0";
                string expectedVersionMessage = $"Fout bij het lezen van bestand '{filePath}': " +
                                                "de database heeft niet de vereiste versie informatie. " +
                                                $"Vereiste versie is '{requiredVersion}'.";

                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedVersionMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(filePath));
        }

        [Test]
        public void Constructor_CorrectVersion_DoesNotThrowException()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "correctVersion.soil");
            using (var versionReader = new SoilDatabaseVersionReader(filePath))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyVersion();

                // Assert
                Assert.DoesNotThrow(test);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(filePath));
        }
    }
}
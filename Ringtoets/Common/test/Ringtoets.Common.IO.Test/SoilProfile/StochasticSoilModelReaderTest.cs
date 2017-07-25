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

using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(StochasticSoilModelReader));
        private readonly string constraintsReaderTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(SoilDatabaseConstraintsReader));
        private readonly string versionReaderTestDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(SoilDatabaseVersionReader));

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilModelReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilModelReader(fileName)) {}
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
            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Initialize_IncorrectFormatFileOrInvalidSchema_ThrowsCriticalFileReadException(string dbName)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Initialize_IncorrectVersion_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(versionReaderTestDataPath, "incorrectVersion.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                const string version = "15.0.6.0";
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "De database heeft niet de vereiste versie informatie. " +
                    $"Vereiste versie is '{version}'.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Initialize_InvalidSchemaThatPassesVersionValidation_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(constraintsReaderTestDataPath, "missingStochasticSoilModelTable.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kan geen ondergrondmodellen lezen. Mogelijk bestaat de 'StochasticSoilModel' tabel niet.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Initialize_NonUniqueSoilModelNames_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(constraintsReaderTestDataPath, "nonUniqueSoilModelNames.soil");

            using (var reader = new StochasticSoilModelReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Namen van ondergrondmodellen zijn niet uniek.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}
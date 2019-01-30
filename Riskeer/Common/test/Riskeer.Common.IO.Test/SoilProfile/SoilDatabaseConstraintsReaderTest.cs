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
    public class SoilDatabaseConstraintsReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(SoilDatabaseConstraintsReader));

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            TestDelegate test = () =>
            {
                using (new SoilDatabaseConstraintsReader(testFile)) {}
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
                using (new SoilDatabaseConstraintsReader(fileName)) {}
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
            using (var reader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void VerifyConstraints_MissingStochasticSoilModelTable_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "missingStochasticSoilModelTable.soil");

            using (var versionReader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyConstraints();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kan geen ondergrondmodellen lezen. Mogelijk bestaat de 'StochasticSoilModel' tabel niet.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void VerifyConstraints_MissingStochasticSoilProfileTable_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "missingStochasticSoilProfileTable.soil");

            using (var versionReader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyConstraints();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kan geen ondergrondschematisaties lezen. Mogelijk bestaat de 'StochasticSoilProfile' tabel niet.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void VerifyConstraints_NonUniqueSoilModelNames_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "nonUniqueSoilModelNames.soil");

            using (var versionReader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyConstraints();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Namen van ondergrondmodellen zijn niet uniek.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void VerifyConstraints_MissingStochasticSoilProfileProbability_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "missingStochasticSoilProfileProbability.soil");

            // Call
            using (var versionReader = new SoilDatabaseConstraintsReader(dbFile))
            {
                // Call
                TestDelegate test = () => versionReader.VerifyConstraints();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Er zijn stochastische ondergrondschematisaties zonder geldige kans van voorkomen.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}
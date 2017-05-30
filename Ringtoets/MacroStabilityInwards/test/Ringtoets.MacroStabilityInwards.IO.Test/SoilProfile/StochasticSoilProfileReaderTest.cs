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

using System;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilProfileReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.MacroStabilityInwards.IO, "StochasticSoilProfileReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.soil");

            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilProfileReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Setup
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   fileName, "bestandspad mag niet leeg of ongedefinieerd zijn.");
            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilProfileReader(fileName)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("text.txt")]
        [TestCase("empty.soil")]
        public void Constructor_IncorrectFormatFileOrInvalidSchema_ThrowsCriticalFileReadException(string dbName)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, dbName);
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).
                Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilProfileReader(dbFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ParameteredConstructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            const string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Call
            using (var stochasticSoilModelDatabaseReader = new StochasticSoilProfileReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, stochasticSoilModelDatabaseReader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(stochasticSoilModelDatabaseReader);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Constructor_IncorrectVersion_ThrowsCriticalFileReadException()
        {
            // Setup
            const string version = "15.0.6.0";
            string expectedVersionMessage = $"De database heeft niet de vereiste versie informatie. Vereiste versie is '{version}'.";
            const string dbName = "incorrectversion.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilProfileReader(dbFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedVersionMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Constructor_CorruptDatabase_ThrowsException()
        {
            // Setup
            const string dbName = "corruptStochasticSoilProfile.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            // Call
            TestDelegate test = () =>
            {
                using (new StochasticSoilProfileReader(dbFile)) {}
            };

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_EmptyDatabase_ReturnsFalse()
        {
            // Setup
            const string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilProfileReader = new StochasticSoilProfileReader(dbFile))
            {
                // Call
                bool isPrepared = stochasticSoilProfileReader.HasNext;

                // Assert
                Assert.IsFalse(isPrepared);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void HasNext_CompleteDatabase_ReturnsTrue()
        {
            // Setup
            const string dbName = "complete.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilProfileReader = new StochasticSoilProfileReader(dbFile))
            {
                // Call
                bool hasNext = stochasticSoilProfileReader.HasNext;

                // Assert
                Assert.IsTrue(hasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        [TestCase("invalidStochasticSoilProfile1d.soil")]
        [TestCase("invalidStochasticSoilProfile2d.soil")]
        public void ReadStochasticSoilProfile_DataNotValidToCast_ThrowsStochasticSoilProfileReadException(string dbName)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, dbName);
            const long stochasticProfileId = 1;
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile)
                .Build("De ondergrondschematisatie verwijst naar een ongeldige waarde.");

            using (var stochasticSoilProfileReader = new StochasticSoilProfileReader(dbFile))
            {
                // Call
                TestDelegate test = () => stochasticSoilProfileReader.ReadStochasticSoilProfile(stochasticProfileId);

                // Assert
                var exception = Assert.Throws<StochasticSoilProfileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<FormatException>(exception.InnerException);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilProfile_NoProfileIdsSet_ThrowsStochasticSoilProfileReadException()
        {
            // Setup
            const string dbName = "invalidStochasticSoilProfiles.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            const long stochasticProfileId = 1;
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile)
                .Build("De ondergrondschematisatie verwijst naar een ongeldige waarde.");

            using (var stochasticSoilProfileReader = new StochasticSoilProfileReader(dbFile))
            {
                // Call
                TestDelegate test = () => stochasticSoilProfileReader.ReadStochasticSoilProfile(stochasticProfileId);

                // Assert
                var exception = Assert.Throws<StochasticSoilProfileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        [TestCase(1, 0.15, SoilProfileType.SoilProfile2D, 2)]
        [TestCase(2, 0.15, SoilProfileType.SoilProfile1D, 1)]
        public void ReadStochasticSoilProfile_CompleteDatabase_ReturnsExpectedValues(long profileId, double probability, SoilProfileType soilProfileType, long soilProfileId)
        {
            // Setup
            const string dbName = "complete.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilProfileReader = new StochasticSoilProfileReader(dbFile))
            {
                // Call
                StochasticSoilProfile stochasticSoilProfile = stochasticSoilProfileReader.ReadStochasticSoilProfile(profileId);

                // Assert
                Assert.IsNotNull(stochasticSoilProfile);
                Assert.AreEqual(probability, stochasticSoilProfile.Probability);
                Assert.AreEqual(soilProfileType, stochasticSoilProfile.SoilProfileType);
                Assert.AreEqual(soilProfileId, stochasticSoilProfile.SoilProfileId);
                Assert.IsTrue(stochasticSoilProfileReader.HasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilProfile_SoilModelIdNotInDatabase_ReturnNull()
        {
            // Setup
            const string dbName = "complete.soil";
            string dbFile = Path.Combine(testDataPath, dbName);

            using (var stochasticSoilProfileReader = new StochasticSoilProfileReader(dbFile))
            {
                // Call
                StochasticSoilProfile stochasticSoilProfile = stochasticSoilProfileReader.ReadStochasticSoilProfile(987654321);

                // Assert
                Assert.IsNull(stochasticSoilProfile);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadStochasticSoilProfile_EmptyDatabase_ReturnsNull()
        {
            // Setup
            const string dbName = "emptyschema.soil";
            string dbFile = Path.Combine(testDataPath, dbName);
            const long profileId = 1;

            using (var stochasticSoilProfileReader = new StochasticSoilProfileReader(dbFile))
            {
                // Call
                StochasticSoilProfile stochasticSoilProfile = stochasticSoilProfileReader.ReadStochasticSoilProfile(profileId);

                // Assert
                Assert.IsNull(stochasticSoilProfile);
                Assert.IsFalse(stochasticSoilProfileReader.HasNext);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}
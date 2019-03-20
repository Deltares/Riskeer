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

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Riskeer.HydraRing.IO.Test.HydraulicLocationConfigurationDatabase
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.HydraRing.IO, "HydraulicLocationConfigurationDatabase");

        [Test]
        public void Constructor_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "none.sqlite");

            // Call
            TestDelegate test = () =>
            {
                using (new HydraulicLocationConfigurationDatabaseReader(dbFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "hlcdWithoutScenarioInformation.sqlite");

            // Call
            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(hydraulicBoundaryDatabaseReader);
            }
        }

        [Test]
        [TestCase(18169, 1000, 1801000)]
        [TestCase(6, 1000, 0)]
        public void Read_ValidFileWithoutScenarioInformation_ExpectedValues(int trackId, int hrdLocationId, int expectedLocationId)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "hlcdWithoutScenarioInformation.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = hydraulicBoundaryDatabaseReader.Read(trackId);

                // Assert
                long actualLocationId = readHydraulicLocationConfigurationDatabase.LocationIdMappings.Where(m => m.HrdLocationId == hrdLocationId)
                                                                                  .Select(m => m.HlcdLocationId)
                                                                                  .SingleOrDefault();
                Assert.AreEqual(expectedLocationId, actualLocationId);
                Assert.IsFalse(readHydraulicLocationConfigurationDatabase.UsePreprocessorClosure);
                Assert.IsNull(readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings);
            }
        }

        [Test]
        [TestCase(18169, 1000, 1801000)]
        [TestCase(6, 1000, 0)]
        public void Read_ValidFileWithScenarioInformation_ExpectedValues(int trackId, int hrdLocationId, int expectedLocationId)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "hlcdWithScenarioInformation.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = hydraulicBoundaryDatabaseReader.Read(trackId);

                // Assert
                long actualLocationId = readHydraulicLocationConfigurationDatabase.LocationIdMappings.Where(m => m.HrdLocationId == hrdLocationId)
                                                                                  .Select(m => m.HlcdLocationId)
                                                                                  .SingleOrDefault();
                Assert.AreEqual(expectedLocationId, actualLocationId);
                Assert.IsFalse(readHydraulicLocationConfigurationDatabase.UsePreprocessorClosure);
                IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> readHydraulicLocationConfigurationDatabaseSettings =
                    readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings;
                Assert.AreEqual(2, readHydraulicLocationConfigurationDatabaseSettings.Count());

                CollectionAssert.AreEqual(new[]
                {
                    "ScenarioName WBI2017",
                    "ScenarioName WBI2018"
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.ScenarioName));
                CollectionAssert.AreEqual(new[]
                {
                    2023,
                    2024
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.Year));
                CollectionAssert.AreEqual(new[]
                {
                    "Scope WBI2017",
                    "Scope WBI2018"
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.Scope));
                CollectionAssert.AreEqual(new[]
                {
                    "SeaLevel WBI2017",
                    "SeaLevel WBI2018"
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.SeaLevel));
                CollectionAssert.AreEqual(new[]
                {
                    "RiverDischarge WBI2017",
                    "RiverDischarge WBI2018"
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.RiverDischarge));
                CollectionAssert.AreEqual(new[]
                {
                    "LakeLevel WBI2017",
                    "LakeLevel WBI2018"
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.LakeLevel));
                CollectionAssert.AreEqual(new[]
                {
                    "WindDirection WBI2017",
                    "WindDirection WBI2018"
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.WindDirection));
                CollectionAssert.AreEqual(new[]
                {
                    "WindSpeed WBI2017",
                    "WindSpeed WBI2018"
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.WindSpeed));
                CollectionAssert.AreEqual(new[]
                {
                    "Comment WBI2017",
                    "Comment WBI2018"
                }, readHydraulicLocationConfigurationDatabaseSettings.Select(s => s.Comment));
            }
        }

        [Test]
        [TestCase(3000, true)]
        [TestCase(11, false)]
        public void Read_ValidFileWithUsePreprocessClosureColumn_ExpectedValues(int trackId, bool expectedUsePreprocessorClosure)
        {
            string dbFile = Path.Combine(testDataPath, "hlcdWithPreprocessorClosureColumn.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = hydraulicBoundaryDatabaseReader.Read(trackId);

                // Assert
                Assert.AreEqual(expectedUsePreprocessorClosure, readHydraulicLocationConfigurationDatabase.UsePreprocessorClosure);
            }
        }

        [Test]
        public void Read_InvalidFileWithUsePreprocessClosureColumn_ThrowsLineParseException()
        {
            string dbFile = Path.Combine(testDataPath, "hlcdWithInvalidPreprocessorClosureColumn.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.Read(1000);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                var exception = Assert.Throws<LineParseException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_FileWithScenarioInformationAndMissingColumns_ThrowsCriticalFileReadException()
        {
            // Setup
            const int trackId = 18169;
            string dbFile = Path.Combine(testDataPath, "hlcdWithScenarioInformationMissingColumn.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.Read(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Het bevragen van de database is mislukt.");
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_FileWithScenarioInformationAndInvalidData_ThrowsLineParseException()
        {
            // Setup
            const int trackId = 18169;
            string dbFile = Path.Combine(testDataPath, "hlcdWithScenarioInformationInvalidData.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.Read(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                var exception = Assert.Throws<LineParseException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<ConversionException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_AmbiguousLocations_ReturnsFirstLocationIdAndLogsWarning()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "ambigousLocation.sqlite");
            const int trackId = 11;
            const int hrdLocationId = 1;
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = null;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                Action call = () => readHydraulicLocationConfigurationDatabase = hydraulicBoundaryDatabaseReader.Read(trackId);

                // Assert
                const int expectedLocationId = 1800001;
                const string expectedMessage = "Er zijn meerdere resultaten gevonden, wat niet voor zou mogen komen. Neem contact op met de leverancier. Het eerste resultaat zal worden gebruikt.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
                long actualLocationId = readHydraulicLocationConfigurationDatabase.LocationIdMappings.Where(m => m.HrdLocationId == hrdLocationId)
                                                                                  .Select(m => m.HlcdLocationId)
                                                                                  .Single();
                Assert.AreEqual(expectedLocationId, actualLocationId);
            }
        }

        [Test]
        public void Read_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "corruptschema.sqlite");
            const int trackId = 1;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.Read(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                var exception = Assert.Throws<LineParseException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_EmptyFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "empty.sqlite");
            const int trackId = 1;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.Read(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Het bevragen van de database is mislukt.");
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }
    }
}
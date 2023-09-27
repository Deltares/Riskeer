// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
            string hlcdFilePath = Path.Combine(testDataPath, "none.sqlite");

            // Call
            void Call()
            {
                using (new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath)) {}
            }

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            string expectedMessage = new FileReaderErrorMessageBuilder(hlcdFilePath).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithoutScenarioInformation.sqlite");

            // Call
            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(hydraulicLocationConfigurationDatabaseReader);
            }
        }

        [Test]
        public void Read_ValidFileWithoutScenarioInformation_ExpectedValues()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithoutScenarioInformation.sqlite");

            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Call
                ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = hydraulicLocationConfigurationDatabaseReader.Read();

                // Assert
                AssertReadHydraulicLocations(readHydraulicLocationConfigurationDatabase);
                Assert.IsNull(readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationSettings);
                AssertReadTracks(readHydraulicLocationConfigurationDatabase);
            }
        }

        [Test]
        public void Read_ValidFileWithScenarioInformation_ExpectedValues()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithScenarioInformation.sqlite");

            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Call
                ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = hydraulicLocationConfigurationDatabaseReader.Read();

                // Assert
                AssertReadHydraulicLocations(readHydraulicLocationConfigurationDatabase);
                AssertReadHydraulicLocationConfigurationSettings(readHydraulicLocationConfigurationDatabase);
                AssertReadTracks(readHydraulicLocationConfigurationDatabase);
            }
        }

        [Test]
        [TestCase(3000, true)]
        [TestCase(11, false)]
        public void Read_ValidFileWithUsePreprocessClosureColumn_ExpectedValues(int trackId, bool expectedUsePreprocessorClosure)
        {
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithPreprocessorClosureColumn.sqlite");

            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Call
                ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = hydraulicLocationConfigurationDatabaseReader.Read();

                // Assert
                Assert.AreEqual(expectedUsePreprocessorClosure, readHydraulicLocationConfigurationDatabase.ReadTracks.Single(rt => rt.TrackId == trackId).UsePreprocessorClosure);
            }
        }

        [Test]
        public void Read_InvalidFileWithUsePreprocessClosureColumn_ThrowsLineParseException()
        {
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithInvalidPreprocessorClosureColumn.sqlite");

            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Call
                void Call() => hydraulicLocationConfigurationDatabaseReader.Read();

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(hlcdFilePath).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                var exception = Assert.Throws<LineParseException>(Call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<ConversionException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_FileWithScenarioInformationAndMissingColumns_ThrowsCriticalFileReadException()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithScenarioInformationMissingColumn.sqlite");

            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Call
                void Call() => hydraulicLocationConfigurationDatabaseReader.Read();

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(hlcdFilePath).Build("Het bevragen van de database is mislukt.");
                var exception = Assert.Throws<CriticalFileReadException>(Call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_FileWithScenarioInformationAndInvalidData_ThrowsLineParseException()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "hlcdWithScenarioInformationInvalidData.sqlite");

            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Call
                void Call() => hydraulicLocationConfigurationDatabaseReader.Read();

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(hlcdFilePath).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                var exception = Assert.Throws<LineParseException>(Call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<ConversionException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Call
                void Call() => hydraulicLocationConfigurationDatabaseReader.Read();

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(hlcdFilePath).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                var exception = Assert.Throws<LineParseException>(Call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<ConversionException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_EmptyFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "empty.sqlite");

            using (var hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
            {
                // Call
                void Call() => hydraulicLocationConfigurationDatabaseReader.Read();

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(hlcdFilePath).Build("Het bevragen van de database is mislukt.");
                var exception = Assert.Throws<CriticalFileReadException>(Call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        private static void AssertReadHydraulicLocations(ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase)
        {
            ReadHydraulicLocation[] readHydraulicLocations = readHydraulicLocationConfigurationDatabase.ReadHydraulicLocations.ToArray();
            Assert.AreEqual(36446, readHydraulicLocations.Length);
            Assert.IsNotNull(readHydraulicLocations.SingleOrDefault(rhl => rhl.HlcdLocationId == 1801000
                                                                           && rhl.HrdLocationId == 1000
                                                                           && rhl.TrackId == 18169));
            Assert.IsNotNull(readHydraulicLocations.SingleOrDefault(rhl => rhl.HlcdLocationId == 100008
                                                                           && rhl.HrdLocationId == 8
                                                                           && rhl.TrackId == 1000));
        }

        private static void AssertReadHydraulicLocationConfigurationSettings(ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase)
        {
            ReadHydraulicLocationConfigurationSettings[] readHydraulicLocationConfigurationSettings =
                readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationSettings.ToArray();
            Assert.AreEqual(2, readHydraulicLocationConfigurationSettings.Length);

            CollectionAssert.AreEqual(new[]
            {
                "ScenarioName WBI2017",
                "ScenarioName WBI2018"
            }, readHydraulicLocationConfigurationSettings.Select(s => s.ScenarioName));
            CollectionAssert.AreEqual(new[]
            {
                2023,
                2024
            }, readHydraulicLocationConfigurationSettings.Select(s => s.Year));
            CollectionAssert.AreEqual(new[]
            {
                "Scope WBI2017",
                "Scope WBI2018"
            }, readHydraulicLocationConfigurationSettings.Select(s => s.Scope));
            CollectionAssert.AreEqual(new[]
            {
                "SeaLevel WBI2017",
                "SeaLevel WBI2018"
            }, readHydraulicLocationConfigurationSettings.Select(s => s.SeaLevel));
            CollectionAssert.AreEqual(new[]
            {
                "RiverDischarge WBI2017",
                "RiverDischarge WBI2018"
            }, readHydraulicLocationConfigurationSettings.Select(s => s.RiverDischarge));
            CollectionAssert.AreEqual(new[]
            {
                "LakeLevel WBI2017",
                "LakeLevel WBI2018"
            }, readHydraulicLocationConfigurationSettings.Select(s => s.LakeLevel));
            CollectionAssert.AreEqual(new[]
            {
                "WindDirection WBI2017",
                "WindDirection WBI2018"
            }, readHydraulicLocationConfigurationSettings.Select(s => s.WindDirection));
            CollectionAssert.AreEqual(new[]
            {
                "WindSpeed WBI2017",
                "WindSpeed WBI2018"
            }, readHydraulicLocationConfigurationSettings.Select(s => s.WindSpeed));
            CollectionAssert.AreEqual(new[]
            {
                "Comment WBI2017",
                "Comment WBI2018"
            }, readHydraulicLocationConfigurationSettings.Select(s => s.Comment));
        }

        private static void AssertReadTracks(ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase)
        {
            ReadTrack[] readTracks = readHydraulicLocationConfigurationDatabase.ReadTracks.ToArray();
            Assert.AreEqual(290, readTracks.Length);
            Assert.IsNotNull(readTracks.SingleOrDefault(rt => rt.TrackId == 18169
                                                              && rt.HrdFileName == "WBI2017_Bovenmaas_hoge_keringen_36-5_v02.sqlite"
                                                              && !rt.UsePreprocessorClosure));
            Assert.IsNotNull(readTracks.SingleOrDefault(rt => rt.TrackId == 1000
                                                              && rt.HrdFileName == "WBI2017_Bovenrijn_aslocaties_v02.sqlite"
                                                              && !rt.UsePreprocessorClosure));
        }
    }
}
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
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.HydraRing.IO.Test.HydraulicBoundaryDatabase
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.HydraRing.IO, "HydraulicBoundaryDatabaseReader");

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Assert
                Assert.AreEqual(hydraulicBoundaryDatabaseFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }
        }

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "doesNotExist.sqlite");

            // Call
            TestDelegate test = () =>
            {
                using (new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile)) {}
            };

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': het bestand bestaat niet.";
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FilePathNullOrEmpty_ThrowsCriticalFileReadException(string hydraulicBoundaryDatabaseFilePath)
        {
            // Call
            TestDelegate test = () =>
            {
                using (new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFilePath)) {}
            };

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Read_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "empty.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.Read();

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kon geen locaties verkrijgen van de database.";
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_DatabaseWithoutTrackId_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "emptyGeneral.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.Read();

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void Read_InvalidTrackIdColumn_ThrowsLineParseException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "corruptschema.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.Read();

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_DatabaseSchemaInvalidLocationColumns_ThrowsLineParseException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "corruptLocationSchema.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.Read();

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<ConversionException>(exception.InnerException);
            }
        }

        [Test]
        public void Read_ValidFile_ReturnsReadHydraulicBoundaryDatabase()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                ReadHydraulicBoundaryDatabase readDatabase = reader.Read();

                // Assert
                Assert.AreEqual("Dutch coast South19-11-2015 12:0013", readDatabase.Version);
                Assert.AreEqual(13, readDatabase.TrackId);
                Assert.AreEqual(18, readDatabase.Locations.Count());
                ReadHydraulicBoundaryLocation location = readDatabase.Locations.First();
                Assert.AreEqual(1, location.Id);
                Assert.AreEqual("punt_flw_ 1", location.Name);
                Assert.AreEqual(52697.5, location.CoordinateX);
                Assert.AreEqual(427567.0, location.CoordinateY);
            }
        }

        [Test]
        public void ReadTrackId_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "emptyGeneral.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.ReadTrackId();

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadTrackId_InvalidDatabaseWithoutGeneralTable_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "empty.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.ReadTrackId();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kon geen locaties verkrijgen van de database.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadTrackId_InvalidTrackIdColumn_ThrowsLineParseException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "corruptSchema.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.ReadTrackId();

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadTrackId_ValidFile_ReturnsReadVersion()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                long trackId = reader.ReadTrackId();

                // Assert
                Assert.AreEqual(13, trackId);
            }
        }

        [Test]
        public void ReadVersion_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "emptyGeneral.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.ReadVersion();

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadVersion_InvalidTrackIdColumn_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "empty.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => reader.ReadVersion();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kon geen locaties verkrijgen van de database.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadVersion_ValidFile_ReturnsReadVersion()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                string version = reader.ReadVersion();

                // Assert
                Assert.AreEqual("Dutch coast South19-11-2015 12:0013", version);
            }
        }
    }
}
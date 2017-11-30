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
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Ringtoets.HydraRing.IO.Test.HydraulicLocationConfigurationDatabase
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicLocationConfigurationDatabase");

        [Test]
        public void Constructor_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.sqlite");

            // Call
            TestDelegate test = () =>
            {
                using (new HydraulicLocationConfigurationDatabaseReader(testFile)) {}
            };

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");

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
        public void GetLocationIdsByTrackId_ValidFile_ExpectedValues(int trackId, int hrdLocationId, int expectedLocationId)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                Dictionary<long, long> locationIdDictionary = hydraulicBoundaryDatabaseReader.GetLocationIdsByTrackId(trackId);

                // Assert
                long locationId;
                locationIdDictionary.TryGetValue(hrdLocationId, out locationId);
                Assert.AreEqual(expectedLocationId, locationId);
            }
        }

        [Test]
        public void GetLocationIdsByTrackId_AmbigousLocations_ReturnsFirstAndLogsWarning()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "ambigousLocation.sqlite");
            const int trackId = 18;
            const int hrdLocationId = 1;
            var locationIdDictionary = new Dictionary<long, long>();

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                Action call = () => locationIdDictionary = hydraulicBoundaryDatabaseReader.GetLocationIdsByTrackId(trackId);

                // Assert
                const int expectedLocationId = 1800001;
                const string expectedMessage = "Er zijn meerdere resultaten gevonden, wat niet voor zou mogen komen. Neem contact op met de leverancier. Het eerste resultaat zal worden gebruikt.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
                long locationId;
                locationIdDictionary.TryGetValue(hrdLocationId, out locationId);
                Assert.AreEqual(expectedLocationId, locationId);
            }
        }

        [Test]
        public void GetLocationIdsByTrackId_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "corruptschema.sqlite");
            const int trackId = 1;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.GetLocationIdsByTrackId(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                var exception = Assert.Throws<LineParseException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
        }

        [Test]
        public void GetLocationIdsByTrackId_EmptyFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "empty.sqlite");
            const int trackId = 1;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.GetLocationIdsByTrackId(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Het bevragen van de database is mislukt.");
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        [TestCase(1000, true)]
        [TestCase(11, false)]
        public void GetCanUsePreprocessorByTrackId_ValidFile_ExpectedValues(long trackId, bool expectedUsePreprocessor)
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                bool usePreprocessor = hydraulicBoundaryDatabaseReader.GetCanUsePreprocessorByTrackId(trackId);

                // Assert
                Assert.AreEqual(expectedUsePreprocessor, usePreprocessor);
            }
        }

        [Test]
        public void GetCanUsePreprocessorByTrackId_ValidFileWithoutUsePreprocessorColumn_ReturnFalse()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "noUsePreprocessorColumn.sqlite");
            const int trackId = 1000;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                bool usePreprocessor = hydraulicBoundaryDatabaseReader.GetCanUsePreprocessorByTrackId(trackId);

                // Assert
                Assert.IsFalse(usePreprocessor);
            }
        }

        [Test]
        public void GetCanUsePreprocessorByTrackId_UnknownRegionIdForTrackId_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");
            const int trackId = 1;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.GetCanUsePreprocessorByTrackId(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Het bevragen van de database is mislukt.");
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsNull(exception.InnerException);
            }
        }

        [Test]
        public void GetCanUsePreprocessorByTrackId_EmptyFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "empty.sqlite");
            const int trackId = 1;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.GetCanUsePreprocessorByTrackId(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Het bevragen van de database is mislukt.");
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void GetCanUsePreprocessorByTrackId_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "corruptschema.sqlite");
            const int trackId = 1000;

            using (var hydraulicBoundaryDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.GetCanUsePreprocessorByTrackId(trackId);

                // Assert
                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                var exception = Assert.Throws<LineParseException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<FormatException>(exception.InnerException);
            }
        }
    }
}
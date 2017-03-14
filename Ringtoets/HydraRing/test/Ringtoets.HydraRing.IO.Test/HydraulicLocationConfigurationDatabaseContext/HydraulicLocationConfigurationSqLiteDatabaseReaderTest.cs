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
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabaseContext;
using Ringtoets.HydraRing.IO.Properties;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.HydraRing.IO.Test.HydraulicLocationConfigurationDatabaseContext
{
    [TestFixture]
    public class HydraulicLocationConfigurationSqLiteDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicLocationConfigurationDatabase");

        [Test]
        public void Constructor_ValidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "none.sqlite");
            var expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build(UtilsResources.Error_File_does_not_exist);

            // Call
            TestDelegate test = () => new HydraulicLocationConfigurationSqLiteDatabaseReader(testFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (HydraulicLocationConfigurationSqLiteDatabaseReader hydraulicBoundarySqLiteDatabaseReader = new HydraulicLocationConfigurationSqLiteDatabaseReader(dbFile))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(hydraulicBoundarySqLiteDatabaseReader);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        [TestCase(1, 1, 100001)]
        [TestCase(18, 1000, 1801000)]
        [TestCase(6, 1000, 0)]
        public void GetLocationsIdByTrackId_ValidFile_ExpectedValues(int trackId, int hrdLocationId, int expectedLocationId)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "complete.sqlite");

            using (HydraulicLocationConfigurationSqLiteDatabaseReader hydraulicBoundarySqLiteDatabaseReader = new HydraulicLocationConfigurationSqLiteDatabaseReader(dbFile))
            {
                // Call
                Dictionary<long, long> locationIdDictionary = hydraulicBoundarySqLiteDatabaseReader.GetLocationsIdByTrackId(trackId);

                // Assert
                long locationId;
                locationIdDictionary.TryGetValue(hrdLocationId, out locationId);
                Assert.AreEqual(expectedLocationId, locationId);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetLocationsIdByTrackId_AmbigousLocations_ReturnsFirstAndLogsWarning()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "ambigousLocation.sqlite");
            int trackId = 18;
            int hrdLocationId = 1;
            int expectedLocationId = 1800001;
            string expectedMessage = "Er zijn meerdere resultaten gevonden, wat niet voor zou mogen komen. Neem contact op met de leverancier. Het eerste resultaat zal worden gebruikt.";
            Dictionary<long, long> locationIdDictionary = new Dictionary<long, long>();

            using (HydraulicLocationConfigurationSqLiteDatabaseReader hydraulicBoundarySqLiteDatabaseReader = new HydraulicLocationConfigurationSqLiteDatabaseReader(dbFile))
            {
                // Call
                Action call = () => locationIdDictionary = hydraulicBoundarySqLiteDatabaseReader.GetLocationsIdByTrackId(trackId);

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
                long locationId;
                locationIdDictionary.TryGetValue(hrdLocationId, out locationId);
                Assert.AreEqual(expectedLocationId, locationId);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetLocationsIdByTrackId_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "corruptschema.sqlite");
            var expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
            int trackId = 1;

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            using (HydraulicLocationConfigurationSqLiteDatabaseReader hydraulicBoundarySqLiteDatabaseReader = new HydraulicLocationConfigurationSqLiteDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundarySqLiteDatabaseReader.GetLocationsIdByTrackId(trackId);

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetLocationsIdByTrackId_EmptyFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "empty.sqlite");
            var expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(Resources.HydraulicLocationConfigurationSqLiteDatabaseReader_Critical_Unexpected_Exception);
            int trackId = 1;

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            using (HydraulicLocationConfigurationSqLiteDatabaseReader hydraulicBoundarySqLiteDatabaseReader = new HydraulicLocationConfigurationSqLiteDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundarySqLiteDatabaseReader.GetLocationsIdByTrackId(trackId);

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}
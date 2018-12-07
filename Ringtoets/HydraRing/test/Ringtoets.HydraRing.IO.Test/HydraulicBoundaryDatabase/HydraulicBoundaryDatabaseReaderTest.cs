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

using System;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Ringtoets.HydraRing.IO.Test.HydraulicBoundaryDatabase
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryDatabaseReader");

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Assert
                Assert.AreEqual(hydraulicBoundaryDatabaseFile, hydraulicBoundaryDatabaseReader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(hydraulicBoundaryDatabaseReader);
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
                using (new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile)) { }
            };

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': het bestand bestaat niet.";
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string hydraulicBoundaryDatabaseFile)
        {
            // Call
            TestDelegate test = () =>
            {
                using (new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile)) { }
            };

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Read_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "empty.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.Read();

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
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "emptySchemaGeneral.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.Read();

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

            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.Read();

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
        }
    }
}
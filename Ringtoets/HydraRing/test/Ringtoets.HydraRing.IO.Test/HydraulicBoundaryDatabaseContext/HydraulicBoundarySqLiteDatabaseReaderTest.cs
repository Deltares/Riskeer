﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabaseContext;

namespace Ringtoets.HydraRing.IO.Test.HydraulicBoundaryDatabaseContext
{
    [TestFixture]
    public class HydraulicBoundarySqLiteDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundarySqLiteDatabaseReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "none.sqlite");
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");

            // Call
            TestDelegate test = () => new HydraulicBoundarySqLiteDatabaseReader(testFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Setup
            string expectedMessage = $"Fout bij het lezen van bestand '{fileName}': bestandspad mag niet leeg of ongedefinieerd zijn.";

            // Call
            TestDelegate test = () => new HydraulicBoundarySqLiteDatabaseReader(fileName).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetVersion_InvalidColumns_DoesNotThrowException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "corruptschema.sqlite");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");
            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                var version = "some version";
                // Call
                TestDelegate test = () => version = hydraulicBoundarySqLiteDatabaseReader.GetVersion();

                // Assert
                Assert.DoesNotThrow(test);
                const string expectedVersion = "Namedate7";
                Assert.AreEqual(expectedVersion, version);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetVersion_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "empty.sqlite");
            string expectedException = new FileReaderErrorMessageBuilder(dbFile).Build("Kon geen locaties verkrijgen van de database.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");
            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundarySqLiteDatabaseReader.GetVersion();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedException, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Assert
                Assert.IsFalse(hydraulicBoundarySqLiteDatabaseReader.HasNext);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetLocationCount_ValidFile_ExpectedValues()
        {
            // Setup
            const int expectedNrOfLocations = 18;
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                int nrOfLocations = hydraulicBoundarySqLiteDatabaseReader.GetLocationCount();

                // Assert
                Assert.AreEqual(expectedNrOfLocations, nrOfLocations);
                Assert.IsFalse(hydraulicBoundarySqLiteDatabaseReader.HasNext);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetLocationCount_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "empty.sqlite");
            string expectedException = new FileReaderErrorMessageBuilder(dbFile).Build("Kon geen locaties verkrijgen van de database.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");
            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundarySqLiteDatabaseReader.GetLocationCount();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedException, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetVersion_ValidFile_ExpectedValues()
        {
            // Setup
            const string expectedVersion = "Dutch coast South19-11-2015 12:0013";
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                string version = hydraulicBoundarySqLiteDatabaseReader.GetVersion();

                // Assert
                Assert.AreEqual(expectedVersion, version);
                Assert.IsFalse(hydraulicBoundarySqLiteDatabaseReader.HasNext);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetTrackId_ValidFile_ExpectedValues()
        {
            // Setup
            const long expectedTrackId = 13;
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                long trackId = hydraulicBoundarySqLiteDatabaseReader.GetTrackId();

                // Assert
                Assert.AreEqual(expectedTrackId, trackId);
                Assert.IsFalse(hydraulicBoundarySqLiteDatabaseReader.HasNext);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetTrackId_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "empty.sqlite");
            string expectedException = new FileReaderErrorMessageBuilder(dbFile).Build("Kon geen locaties verkrijgen van de database.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");
            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundarySqLiteDatabaseReader.GetTrackId();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedException, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GetTrackId_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "corruptschema.sqlite");
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundarySqLiteDatabaseReader.GetTrackId();

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadLocation_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "corruptschema.sqlite");
            string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                hydraulicBoundarySqLiteDatabaseReader.PrepareReadLocation();
                TestDelegate test = () => hydraulicBoundarySqLiteDatabaseReader.ReadLocation();

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<ConversionException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadLocation_ValidFileReadOneLocation_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                hydraulicBoundarySqLiteDatabaseReader.PrepareReadLocation();
                ReadHydraulicBoundaryLocation location = hydraulicBoundarySqLiteDatabaseReader.ReadLocation();

                // Assert
                Assert.IsNotNull(location);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadLocation_ValidFilereadAllLocations_ExpectedValues()
        {
            // Setup
            const int nrOfLocations = 18;
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");
            var boundaryLocations = new List<ReadHydraulicBoundaryLocation>();
            CollectionAssert.IsEmpty(boundaryLocations);

            using (var hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Call
                hydraulicBoundarySqLiteDatabaseReader.PrepareReadLocation();
                for (var i = 0; i < nrOfLocations; i++)
                {
                    boundaryLocations.Add(hydraulicBoundarySqLiteDatabaseReader.ReadLocation());
                }

                // Assert
                Assert.IsFalse(hydraulicBoundarySqLiteDatabaseReader.HasNext);
                Assert.IsNull(hydraulicBoundarySqLiteDatabaseReader.ReadLocation());
            }

            CollectionAssert.AllItemsAreInstancesOfType(boundaryLocations, typeof(ReadHydraulicBoundaryLocation));
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ParameteredConstructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptyschema.sqlite");

            // Call
            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, hydraulicBoundaryDatabaseReader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(hydraulicBoundaryDatabaseReader);
            }
        }

        [Test]
        public void Constructor_EmptyDatabase_HasNextFalse()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptyschema.sqlite");

            // Call
            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile))
            {
                // Assert
                Assert.IsFalse(hydraulicBoundaryDatabaseReader.HasNext);
            }
        }

        [Test]
        public void Dispose_AfterConstruction_CorrectlyReleasesFile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            // Call
            new HydraulicBoundarySqLiteDatabaseReader(dbFile).Dispose();

            // Assert
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Dispose_WhenReadLocation_CorrectlyReleasesFile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "complete.sqlite");
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            HydraulicBoundarySqLiteDatabaseReader hydraulicBoundarySqLiteDatabaseReader = null;
            ReadHydraulicBoundaryLocation boundaryLocation;
            try
            {
                hydraulicBoundarySqLiteDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(dbFile);
                hydraulicBoundarySqLiteDatabaseReader.PrepareReadLocation();
                boundaryLocation = hydraulicBoundarySqLiteDatabaseReader.ReadLocation();
            }
            finally
            {
                // Call
                hydraulicBoundarySqLiteDatabaseReader?.Dispose();
            }

            // Assert
            Assert.NotNull(boundaryLocation);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}
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
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Ringtoets.HydraRing.IO.Test.HydraulicBoundaryDatabase
{
    [TestFixture]
    public class OldHydraulicBoundaryDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryDatabaseReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "doesNotExist.sqlite");

            // Call
            TestDelegate test = () =>
            {
                using (new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(hydraulicBoundaryDatabaseFile).Build("Het bestand bestaat niet.");
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
                using (new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = $"Fout bij het lezen van bestand '{hydraulicBoundaryDatabaseFile}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                hydraulicBoundaryDatabaseReader.PrepareReadLocation();

                // Assert
                Assert.AreEqual(hydraulicBoundaryDatabaseFile, hydraulicBoundaryDatabaseReader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(hydraulicBoundaryDatabaseReader);
                Assert.IsTrue(hydraulicBoundaryDatabaseReader.HasNext);
            }
        }

        [Test]
        public void Constructor_EmptyDatabase_HasNextFalse()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "emptyschema.sqlite");

            // Call
            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                hydraulicBoundaryDatabaseReader.PrepareReadLocation();

                // Assert
                Assert.IsFalse(hydraulicBoundaryDatabaseReader.HasNext);
            }
        }

        [Test]
        public void GetVersion_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "empty.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.GetVersion();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                string expectedException = new FileReaderErrorMessageBuilder(hydraulicBoundaryDatabaseFile).Build("Kon geen locaties verkrijgen van de database.");
                Assert.AreEqual(expectedException, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void GetVersion_ValidFile_ExpectedValues()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                string version = hydraulicBoundaryDatabaseReader.GetVersion();

                // Assert
                Assert.AreEqual("Dutch coast South19-11-2015 12:0013", version);
            }
        }

        [Test]
        public void GetTrackId_EmptyDatabase_ThrowsCriticalFileReadException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "empty.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.GetTrackId();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                string expectedException = new FileReaderErrorMessageBuilder(hydraulicBoundaryDatabaseFile).Build("Kon geen locaties verkrijgen van de database.");
                Assert.AreEqual(expectedException, exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void GetTrackId_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "corruptschema.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.GetTrackId();

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                string expectedMessage = new FileReaderErrorMessageBuilder(hydraulicBoundaryDatabaseFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
        }

        [Test]
        public void GetTrackId_ValidFile_ExpectedValues()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                // Call
                long trackId = hydraulicBoundaryDatabaseReader.GetTrackId();

                // Assert
                Assert.AreEqual((long) 13, trackId);
            }
        }

        [Test]
        public void ReadLocation_InvalidColumns_ThrowsLineParseException()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "corruptschema.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                hydraulicBoundaryDatabaseReader.PrepareReadLocation();

                // Call
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.ReadLocation();

                // Assert
                var exception = Assert.Throws<LineParseException>(test);
                string expectedMessage = new FileReaderErrorMessageBuilder(hydraulicBoundaryDatabaseFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<ConversionException>(exception.InnerException);
            }
        }

        [Test]
        public void ReadLocation_ValidFileReadOneLocation_ExpectedValues()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                hydraulicBoundaryDatabaseReader.PrepareReadLocation();

                // Call
                ReadHydraulicBoundaryLocation location = hydraulicBoundaryDatabaseReader.ReadLocation();

                // Assert
                Assert.IsNotNull(location);
                Assert.AreEqual(1, location.Id);
                Assert.AreEqual("punt_flw_ 1", location.Name);
                Assert.AreEqual(52697.5, location.CoordinateX);
                Assert.AreEqual(427567.0, location.CoordinateY);
            }
        }

        [Test]
        public void ReadLocation_ValidFileReadAllLocations_ExpectedValues()
        {
            // Setup
            const int nrOfLocations = 18;
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");
            var boundaryLocations = new List<ReadHydraulicBoundaryLocation>();

            using (var hydraulicBoundaryDatabaseReader = new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile))
            {
                hydraulicBoundaryDatabaseReader.PrepareReadLocation();

                // Call
                for (var i = 0; i < nrOfLocations; i++)
                {
                    boundaryLocations.Add(hydraulicBoundaryDatabaseReader.ReadLocation());
                }

                // Assert
                Assert.IsFalse(hydraulicBoundaryDatabaseReader.HasNext);
                Assert.IsNull(hydraulicBoundaryDatabaseReader.ReadLocation());
            }

            CollectionAssert.AllItemsAreNotNull(boundaryLocations);
        }

        [Test]
        public void Dispose_Always_CorrectlyReleasesFile()
        {
            // Setup
            string hydraulicBoundaryDatabaseFile = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (new OldHydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabaseFile)) {}

            // Assert
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(hydraulicBoundaryDatabaseFile));
        }
    }
}
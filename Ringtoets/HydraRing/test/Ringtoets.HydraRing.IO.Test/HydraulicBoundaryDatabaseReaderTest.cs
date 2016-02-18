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
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO.Properties;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.HydraRing.IO.Test
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "none.sqlite");
            var expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build(UtilsResources.Error_File_does_not_exist);

            // Call
            TestDelegate test = () => new HydraulicBoundaryDatabaseReader(testFile).Dispose();

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
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}", fileName, UtilsResources.Error_Path_must_be_specified);

            // Call
            TestDelegate test = () => new HydraulicBoundaryDatabaseReader(fileName).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);

            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_IncorrectFormatFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "empty.sqlite");
            var expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(Resources.Error_HydraulicBoundaryLocation_read_from_database);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            TestDelegate test = () => new HydraulicBoundaryDatabaseReader(dbFile).Dispose();

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            const string version = "Dutch coast South19-11-2015 12:00";
            const int nrOfLocations = 18;
            var dbFile = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (HydraulicBoundaryDatabaseReader hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(dbFile))
            {
                Assert.AreEqual(version, hydraulicBoundaryDatabaseReader.Version);
                Assert.AreEqual(nrOfLocations, hydraulicBoundaryDatabaseReader.Count);
                Assert.IsTrue(hydraulicBoundaryDatabaseReader.HasNext);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadLocation_InvalidColums_ThrowsCriticalFileReadException()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "corruptschema.sqlite");
            var expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);

            // Precondition
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition: file can be opened for edits.");

            // Call
            using (HydraulicBoundaryDatabaseReader hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(dbFile))
            {
                TestDelegate test = () => hydraulicBoundaryDatabaseReader.ReadLocation();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidCastException>(exception.InnerException);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadLocation_ValidFileReadOneLocation_ExpectedValues()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (HydraulicBoundaryDatabaseReader hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(dbFile))
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryDatabaseReader.ReadLocation();

                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocation>(location);
            }
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadLocation_ValidFilereadAllLocations_ExpectedValues()
        {
            // Setup
            const int nrOfLocations = 18;
            var dbFile = Path.Combine(testDataPath, "complete.sqlite");
            var boundaryLocations = new List<HydraulicBoundaryLocation>();
            CollectionAssert.IsEmpty(boundaryLocations);

            // Call
            using (HydraulicBoundaryDatabaseReader hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(dbFile))
            {
                for (int i = 0; i < nrOfLocations; i++)
                {
                    boundaryLocations.Add(hydraulicBoundaryDatabaseReader.ReadLocation());
                }

                Assert.IsFalse(hydraulicBoundaryDatabaseReader.HasNext);
                Assert.IsNull(hydraulicBoundaryDatabaseReader.ReadLocation());
            }

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(boundaryLocations, typeof(HydraulicBoundaryLocation));
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ParameteredConstructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptyschema.sqlite");

            // Call
            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, hydraulicBoundaryDatabaseReader.Path);
                Assert.IsInstanceOf<DatabaseReaderBase>(hydraulicBoundaryDatabaseReader);
            }
        }

        [Test]
        public void Constructor_EmptyDatabase_HasNextFalse()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "emptyschema.sqlite");

            // Call
            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(dbFile))
            {
                // Assert
                Assert.IsFalse(hydraulicBoundaryDatabaseReader.HasNext);
            }
        }

        [Test]
        public void Dispose_AfterConstruction_CorrectlyReleasesFile()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "complete.sqlite");
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            // Call
            new HydraulicBoundaryDatabaseReader(dbFile).Dispose();

            // Assert
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Dispose_WhenReadLocation_CorrectlyReleasesFile()
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, "complete.sqlite");
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile), "Precondition failed: The file should be writable to begin with.");

            HydraulicBoundaryDatabaseReader hydraulicBoundaryDatabaseReader = null;
            HydraulicBoundaryLocation boundaryLocation;
            try
            {
                hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(dbFile);
                boundaryLocation = hydraulicBoundaryDatabaseReader.ReadLocation();
            }
            finally
            {
                // Call
                if (hydraulicBoundaryDatabaseReader != null)
                {
                    hydraulicBoundaryDatabaseReader.Dispose();
                }
            }

            // Assert
            Assert.NotNull(boundaryLocation);
            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}
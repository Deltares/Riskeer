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

using System.Collections.Generic;
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
    public class HrdFileReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.HydraRing.IO, "HrdFileReader");

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "doesNotExist.sqlite");

            // Call
            void Call()
            {
                using (new HrdFileReader(hrdFilePath)) {}
            }

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hrdFilePath}': het bestand bestaat niet.";
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_PathNullOrEmpty_ThrowsCriticalFileReadException(string hrdFilePath)
        {
            // Call
            void Call()
            {
                using (new HrdFileReader(hrdFilePath)) {}
            }

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{hrdFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_ValidFile_ExpectedValues()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            using (var reader = new HrdFileReader(hrdFilePath))
            {
                // Assert
                Assert.AreEqual(hrdFilePath, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }
        }

        [Test]
        public void ReadHrdLocations_FileWithInvalidDatabaseStructure_ThrowsCriticalFileReadException()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "missingSegmentColumn.sqlite");

            using (var reader = new HrdFileReader(hrdFilePath))
            {
                // Call
                void Call()
                {
                    reader.ReadHrdLocations();
                }

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{hrdFilePath}': kritieke fout opgetreden bij het uitlezen van de structuur van de database.";
                var exception = Assert.Throws<CriticalFileReadException>(Call);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadHrdLocations_FileWithInvalidValues_ThrowsLineParseException()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "invalidHrdLocationIdValue.sqlite");

            using (var reader = new HrdFileReader(hrdFilePath))
            {
                // Call
                void Call()
                {
                    reader.ReadHrdLocations();
                }

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{hrdFilePath}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
                var exception = Assert.Throws<LineParseException>(Call);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ReadHrdLocations_ValidFile_ReturnsExpectedHrdLocations()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "complete.sqlite");

            using (var reader = new HrdFileReader(hrdFilePath))
            {
                // Call
                IEnumerable<ReadHrdLocation> readHrdLocations = reader.ReadHrdLocations().ToArray();

                // Assert
                Assert.AreEqual(164, readHrdLocations.Count());
                AssertReadHrdLocation(readHrdLocations.ElementAt(0), 700001, "10-1", "07_IJsselmeer_selectie_mu2017.sqlite");
                AssertReadHrdLocation(readHrdLocations.ElementAt(41), 1000012, "40-1", "10_Waddenzee_west_selectie_mu2017.sqlite");
                AssertReadHrdLocation(readHrdLocations.ElementAt(121), 11421125, "10-1", "03_Benedenrijn_selectie_mu2017.sqlite");
            }
        }

        private static void AssertReadHrdLocation(ReadHrdLocation readHrdLocation, int expectedHrdLocationId,
                                                  string expectedName, string expectedCoordinateX, string expectedCoordinateY)
        {
            Assert.AreEqual(expectedHrdLocationId, readHrdLocation.HrdLocationId);
            Assert.AreEqual(expectedName, readHrdLocation.Name);
            Assert.AreEqual(expectedCoordinateX, readHrdLocation.CoordinateX);
            Assert.AreEqual(expectedCoordinateY, readHrdLocation.CoordinateY);
        }
    }
}
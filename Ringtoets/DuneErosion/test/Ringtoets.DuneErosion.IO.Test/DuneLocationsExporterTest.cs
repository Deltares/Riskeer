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
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.IO.Test
{
    [TestFixture]
    public class DuneLocationsExporterTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.DuneErosion.IO, "DuneLocationsExporter");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var filePath = Path.Combine(testDataPath, "tets.bnd");

            // Call
            var exporter = new DuneLocationsExporter(Enumerable.Empty<DuneLocation>(), filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_LocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var filePath = Path.Combine(testDataPath, "tets.bnd");

            // Call
            TestDelegate test = () => new DuneLocationsExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocations", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void Constructor_FilePathInvalid_ThrowArgumentException(string filePath)
        {
            // Call
            TestDelegate test = () => new DuneLocationsExporter(Enumerable.Empty<DuneLocation>(), filePath);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            DuneLocation[] duneLocations =
            {
                new DuneLocation(1, string.Empty, new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                 {
                                     CoastalAreaId = 9,
                                     Offset = 9740,
                                     Orientation = 0,
                                     D50 = 1.9583e-4
                                 })
                {
                    Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                                                    {
                                                        WaterLevel = 5.89,
                                                        WaveHeight = 8.54,
                                                        WavePeriod = 14.11,
                                                        TargetProbability = 0,
                                                        TargetReliability = 0,
                                                        CalculatedProbability = 0,
                                                        CalculatedReliability = 0
                                                    })
                },
                new DuneLocation(2, string.Empty, new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                 {
                                     CoastalAreaId = 9,
                                     Offset = 9770.1,
                                     Orientation = 0,
                                     D50 = 1.9583e-4
                                 })
                {
                    Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                                                    {
                                                        WaterLevel = 5.89,
                                                        WaveHeight = 8.53,
                                                        WavePeriod = 14.09,
                                                        TargetProbability = 0,
                                                        TargetReliability = 0,
                                                        CalculatedProbability = 0,
                                                        CalculatedReliability = 0
                                                    })
                }
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.DuneErosion.IO,
                                                              "Export_ValidData_ReturnTrue");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.bnd");

            var exporter = new DuneLocationsExporter(duneLocations, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Assert.AreEqual("Kv\tNr\tRp\tHs\tTp\tTm-1,0\tD50\r\n" +
                                "9\t9740\t5.89\t8.54\t14.11\t*\t0.000196\r\n" +
                                "9\t9770.1\t5.89\t8.53\t14.09\t*\t0.000196\r\n",
                                fileContent);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void Export_InvalidDirectoryRight_LogErrorAndReturnFalse()
        {
            // Setup
            DuneLocation[] duneLocations =
            {
                new DuneLocation(1, string.Empty, new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                 {
                                     CoastalAreaId = 9,
                                     Offset = 9740,
                                     Orientation = 0,
                                     D50 = 1.9583e-4
                                 })
                {
                    Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                                                    {
                                                        WaterLevel = 5.89,
                                                        WaveHeight = 8.54,
                                                        WavePeriod = 14.11,
                                                        TargetProbability = 0,
                                                        TargetReliability = 0,
                                                        CalculatedProbability = 0,
                                                        CalculatedReliability = 0
                                                    })
                },
                new DuneLocation(2, string.Empty, new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                 {
                                     CoastalAreaId = 9,
                                     Offset = 9770,
                                     Orientation = 0,
                                     D50 = 1.9583e-4
                                 })
                {
                    Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                                                    {
                                                        WaterLevel = 5.89,
                                                        WaveHeight = 8.53,
                                                        WavePeriod = 14.09,
                                                        TargetProbability = 0,
                                                        TargetReliability = 0,
                                                        CalculatedProbability = 0,
                                                        CalculatedReliability = 0
                                                    })
                }
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.DuneErosion.IO,
                                                              "Export_InvalidDirectoryRights_LogErrorAndReturnFalse");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.bnd");

            var exporter = new DuneLocationsExporter(duneLocations, filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    bool isExported = true;
                    Action call = () => isExported = exporter.Export();

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. "
                                             + "Er zijn geen hydraulische randvoorwaarden locaties geëxporteerd.";
                    TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
                    Assert.IsFalse(isExported);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}
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
using Ringtoets.DuneErosion.Data.TestUtil;

namespace Ringtoets.DuneErosion.IO.Test
{
    [TestFixture]
    public class DuneLocationsExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var filePath = TestHelper.GetScratchPadPath(Path.Combine(nameof(DuneLocationsExporterTest), "test.bnd"));

            // Call
            var exporter = new DuneLocationsExporter(Enumerable.Empty<DuneLocation>(), filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_LocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneLocationsExporter(null, "IAmValid.bnd");

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
            var locationNoOutput = CreateDuneLocationForExport(9, 9740, 1.9583e-4);

            var locationUncalculatedOutput = CreateDuneLocationForExport(10, 9770.1, 1.9583e-4);
            locationUncalculatedOutput.Output = CreateDuneLocationOutputForExport(double.NaN, double.NaN, double.NaN);

            var locationCalculatedOutput = CreateDuneLocationForExport(11, 9771.34, 1.337e-4);
            locationCalculatedOutput.Output = CreateDuneLocationOutputForExport(5.89, 14.11, 8.53);

            DuneLocation[] duneLocations =
            {
                locationNoOutput,
                locationUncalculatedOutput,
                locationCalculatedOutput
            };

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_ValidData_ReturnTrueAndWritesFile));
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_ValidData_ReturnTrueAndWritesFile)))
            {
                string filePath = Path.Combine(directoryPath, "test.bnd");

                var exporter = new DuneLocationsExporter(duneLocations, filePath);

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Kv\tNr\tRp\tHs\tTp\tTm-1,0\tD50{Environment.NewLine}" +
                                      $"*[-]\t[dam]\t[m+NAP]\t[m]\t[s]\t[s]\t[m]{Environment.NewLine}" +
                                      $"9\t9740\t*\t*\t*\t*\t0.000196{Environment.NewLine}" +
                                      $"10\t9770.1\t*\t*\t*\t*\t0.000196{Environment.NewLine}" +
                                      $"11\t9771.3\t5.89\t8.53\t14.11\t*\t0.000134{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            DuneLocation[] duneLocations =
            {
                new TestDuneLocation
                {
                    Output = null
                },
                new TestDuneLocation
                {
                    Output = new TestDuneLocationOutput()
                }
            };

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse)))
            {
                string filePath = Path.Combine(directoryPath, "test.bnd");
                var exporter = new DuneLocationsExporter(duneLocations, filePath);

                disposeHelper.LockDirectory(FileSystemRights.Write);
                bool isExported = true;

                // Call
                Action call = () => isExported = exporter.Export();

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. "
                                         + "Er zijn geen hydraulische randvoorwaarden locaties geëxporteerd.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
                Assert.IsFalse(isExported);
            }
        }

        private static DuneLocationOutput CreateDuneLocationOutputForExport(double waterLevel, double wavePeriod, double waveHeight)
        {
            return new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
            {
                WaterLevel = waterLevel,
                WavePeriod = wavePeriod,
                WaveHeight = waveHeight
            });
        }

        private static DuneLocation CreateDuneLocationForExport(int coastalAreaId, double offset, double d50)
        {
            return new DuneLocation(0, string.Empty, new Point2D(0.0, 0.0), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = coastalAreaId,
                Offset = offset,
                D50 = d50
            });
        }
    }
}
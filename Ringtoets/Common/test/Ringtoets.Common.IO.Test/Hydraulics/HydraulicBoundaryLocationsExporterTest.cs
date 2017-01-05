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
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Hydraulics;

namespace Ringtoets.Common.IO.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationsExporterTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "test.shp");

            // Call
            var hydraulicBoundaryLocationsExporter = new HydraulicBoundaryLocationsExporter(Enumerable.Empty<HydraulicBoundaryLocation>(), filePath, "Toetspeil", "Golfhoogte");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(hydraulicBoundaryLocationsExporter);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "test.shp");

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(null, filePath, "Toetspeil", "Golfhoogte");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(111.111),
                WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(222.222)
            };

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, null, "Toetspeil", "Golfhoogte");

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void Constructor_DesignWaterLevelNameNull_ThrowArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(111.111),
                WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(222.222)
            };

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "test.shp");

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath, null, "Golfhoogte");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("designWaterLevelName", exception.ParamName);
        }

        [Test]
        public void Constructor_WaveHeightNameNull_ThrowArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(111.111),
                WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(222.222)
            };

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "test.shp");

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath, "Toetspeil", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveHeightName", exception.ParamName);
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(111.111),
                WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(222.222)
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "Export_ValidData_ReturnTrue");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");
            var baseName = "test";

            var exporter = new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath, "Toetspeil", "Golfhoogte");

            bool isExported;
            try
            {
                // Call
                isExported = exporter.Export();

                // Assert
                AssertEssentialShapefileExists(directoryPath, baseName, true);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }

            Assert.IsTrue(isExported);
        }

        [Test]
        public void Export_ValidData_WritesCorrectData()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(111.111),
                WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(222.222)
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "Export_ValidData_ReturnTrue");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");
            var baseName = "test";

            var exporter = new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath, "Toetspeil", "Golfhoogte");

            // Precondition
            AssertEssentialShapefileExists(directoryPath, baseName, false);

            try
            {
                // Call
                exporter.Export();

                // Assert
                AssertEssentialShapefileExists(directoryPath, baseName, true);
                AssertEssentialShapefileMd5Hashes(directoryPath, baseName);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(111.111),
                WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(222.222)
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "Export_InvalidDirectoryRights_LogErrorAndReturnFalse");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");

            var exporter = new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath, "Toetspeil", "Golfhoogte");

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    bool isExported = true;
                    Action call = () => isExported = exporter.Export();

                    // Assert
                    string expectedMessage = string.Format("Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{0}'. " +
                                                           "Er zijn geen hydraulische randvoorwaarden locaties geëxporteerd.", filePath);
                    TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
                    Assert.IsFalse(isExported);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        private static void AssertEssentialShapefileExists(string directoryPath, string baseName, bool shouldExist)
        {
            string pathName = Path.Combine(directoryPath, baseName);
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shp"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shx"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".dbf"));
        }

        private void AssertEssentialShapefileMd5Hashes(string directoryPath, string baseName)
        {
            string refPathName = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO), "PointShapefileMd5");
            string pathName = Path.Combine(directoryPath, baseName);

            AssertBinaryFileContent(refPathName, pathName, ".shp", 100, 28);
            AssertBinaryFileContent(refPathName, pathName, ".shx", 100, 8);
            AssertBinaryFileContent(refPathName, pathName, ".dbf", 32, 441);
        }

        private static void AssertBinaryFileContent(string refPathName, string pathName, string extension, int headerLength, int bodyLength)
        {
            var refContent = File.ReadAllBytes(refPathName + extension);
            var content = File.ReadAllBytes(pathName + extension);
            Assert.AreEqual(headerLength + bodyLength, content.Length);
            Assert.AreEqual(refContent.Skip(headerLength).Take(bodyLength),
                            content.Skip(headerLength).Take(bodyLength));
        }
    }
}
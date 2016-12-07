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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Hydraulics;

namespace Ringtoets.Common.IO.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationsWriterTest
    {
        [Test]
        public void ParameteredConstructor_DesignWaterLevelNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsWriter(null, "bName");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("designWaterLevelName", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_WaveHeightNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsWriter("aName", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveHeightName", exception.ParamName);
        }

        [Test]
        public void WriteHydraulicBoundaryLocations_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("WriteHydraulicBoundaryLocations_NullhydraulicBoundaryLocations_ThrowArgumentNullException",
                                                                      "test.shp"));

            var writer = new HydraulicBoundaryLocationsWriter("aName", "bName");

            // Call
            TestDelegate call = () => writer.WriteHydraulicBoundaryLocations(null, filePath);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void WriteHydraulicBoundaryLocations_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var writer = new HydraulicBoundaryLocationsWriter("aName", "bName");

            // Call
            TestDelegate call = () => writer.WriteHydraulicBoundaryLocations(Enumerable.Empty<HydraulicBoundaryLocation>(), null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void WriteHydraulicBoundaryLocations_ValidData_WritesShapeFile()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevel = (RoundedDouble) 111.111,
                WaveHeight = (RoundedDouble) 222.222
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "WriteHydraulicBoundaryLocations_ValidData_WritesShapeFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");
            var baseName = "test";

            var writer = new HydraulicBoundaryLocationsWriter("Toetspeil", "Golfhoogte");

            // Precondition
            AssertEssentialShapefileExists(directoryPath, baseName, false);

            try
            {
                // Call
                writer.WriteHydraulicBoundaryLocations(new[]
                {
                    hydraulicBoundaryLocation
                }, filePath);

                // Assert
                AssertEssentialShapefileExists(directoryPath, baseName, true);
                AssertEssentialShapefileMd5Hashes(directoryPath, baseName);
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
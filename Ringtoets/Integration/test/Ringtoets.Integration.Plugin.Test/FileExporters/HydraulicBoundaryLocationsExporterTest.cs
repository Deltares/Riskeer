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
using System.Security.AccessControl;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Plugin.FileExporters;

namespace Ringtoets.Integration.Plugin.Test.FileExporters
{
    public class HydraulicBoundaryLocationsExporterTest
    {
        [Test]
        public void ParameteredConstructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevel = (RoundedDouble) 111.111,
                WaveHeight = (RoundedDouble) 222.222
            };

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "test.shp");

            // Call
            var hydraulicBoundaryLocationsExporter = new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(hydraulicBoundaryLocationsExporter);
        }

        [Test]
        public void ParameteredConstructor_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "test.shp");

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_FilePathNull_ThrowArgumentException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevel = (RoundedDouble) 111.111,
                WaveHeight = (RoundedDouble) 222.222
            };

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, null);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void Export_ValidData_ReturnTrue()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevel = (RoundedDouble) 111.111,
                WaveHeight = (RoundedDouble) 222.222
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO,
                                                              "Export_ValidData_ReturnTrue");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");

            var exporter = new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath);

            bool isExported;
            try
            {
                // Call
                isExported = exporter.Export();
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }

            // Assert
            Assert.IsTrue(isExported);
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                DesignWaterLevel = (RoundedDouble) 111.111,
                WaveHeight = (RoundedDouble) 222.222
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO,
                                                              "Export_InvalidDirectoryRights_LogErrorAndReturnFalse");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");

            var exporter = new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    var isExported = exporter.Export();

                    // Assert
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
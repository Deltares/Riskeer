// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsExporterTest
    {
        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsExporter(null, string.Empty, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_OutputMetaDataHeaderNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsExporter(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("outputMetaDataHeader", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsExporter(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), null, string.Empty);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            var exporter = new HydraulicBoundaryLocationCalculationsExporter(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), filePath, string.Empty);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Export_ValidData_ReturnsTrueAndWritesCorrectData()
        {
            // Setup
            const string fileName = "test";

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_ValidData_ReturnsTrueAndWritesCorrectData));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, $"{fileName}.shp");

            var exporter = new HydraulicBoundaryLocationCalculationsExporter(new[]
            {
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2))
            }, filePath, "Waterlevel");

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, false);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(
                    directoryPath, fileName,
                    Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
                                 nameof(HydraulicBoundaryLocationCalculationsExporter)),
                    "ExpectedExport", 28, 8, 628);
                Assert.IsTrue(isExported);
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
            const string fileName = "test";

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, $"{fileName}.shp");

            var exporter = new HydraulicBoundaryLocationCalculationsExporter(new[]
            {
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2))
            }, filePath, "Waterlevel");

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    var isExported = true;
                    void Call() => isExported = exporter.Export();

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. " +
                                             "Er zijn geen hydraulische belastingenlocaties geëxporteerd.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage);
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
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
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsExporterTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsExporter(assessmentSection, null);

            // Assert
            Assert.Throws<ArgumentException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            var exporter = new HydraulicBoundaryLocationCalculationsExporter(assessmentSection, filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
            mocks.VerifyAll();
        }

        [Test]
        public void Export_ValidData_ReturnsTrueAndWritesCorrectData()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            });

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_ValidData_ReturnsTrueAndWritesCorrectData));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.zip");

            var exporter = new HydraulicBoundaryLocationCalculationsExporter(assessmentSection, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                string[] expectedFiles =
                {
                    "Waterstanden bij norm/Waterstanden_30.000.shp",
                    "Waterstanden bij norm/Waterstanden_30.000 (1).shp",
                    "Waterstanden bij doelkans/Waterstanden_10.000.shp",
                    "Waterstanden bij doelkans/Waterstanden_100.000.shp",
                    "Golfhoogten bij doelkans/Golfhoogten_4.000.shp",
                    "Golfhoogten bij doelkans/Golfhoogten_40.000.shp"
                };

                using (ZipArchive zipArchive = ZipFile.OpenRead(filePath))
                {
                    CollectionAssert.IsSubsetOf(expectedFiles, zipArchive.Entries.Select(e => e.FullName));
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void Export_AssessmentSectionWithoutUserDefinedTargetProbabilities_ReturnsTrueAndWritesCorrectData()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            });
            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Clear();
            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Clear();

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_AssessmentSectionWithoutUserDefinedTargetProbabilities_ReturnsTrueAndWritesCorrectData));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.zip");

            var exporter = new HydraulicBoundaryLocationCalculationsExporter(assessmentSection, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                string[] expectedFiles =
                {
                    "Waterstanden bij norm/Waterstanden_30.000.shp",
                    "Waterstanden bij norm/Waterstanden_30.000 (1).shp"
                };

                using (ZipArchive zipArchive = ZipFile.OpenRead(filePath))
                {
                    CollectionAssert.IsSubsetOf(expectedFiles, zipArchive.Entries.Select(e => e.FullName));
                }
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
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            });

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.zip");

            var exporter = new HydraulicBoundaryLocationCalculationsExporter(assessmentSection, filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    var isExported = true;
                    void Call() => isExported = exporter.Export();

                    // Assert
                    string expectedFilePath = Path.Combine(directoryPath, "~temp", "Waterstanden bij norm", "Waterstanden_30.000.shp");
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{expectedFilePath}'. " +
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
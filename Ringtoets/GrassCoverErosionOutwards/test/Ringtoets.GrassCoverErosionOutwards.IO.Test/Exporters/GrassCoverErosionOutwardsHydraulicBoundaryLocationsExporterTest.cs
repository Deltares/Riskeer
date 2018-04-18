﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.IO.Exporters;
using Ringtoets.GrassCoverErosionOutwards.Util.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.IO.Test.Exporters
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporterTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(
                null, assessmentSection, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(
                new GrassCoverErosionOutwardsFailureMechanism(), null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            TestDelegate call = () => new GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(
                new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection, null);

            // Assert
            Assert.Throws<ArgumentException>(call);
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
            var exporter = new GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection, filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
            mocks.VerifyAll();
        }

        [Test]
        public void Export_ValidData_ReturnsTrueAndWritesCorrectData()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            });

            string directoryPath = TestHelper.GetScratchPadPath("Export_ValidData_ReturnsTrueAndWritesCorrectData");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");
            const string baseName = "test";

            var exporter = new GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(failureMechanism, assessmentSection, filePath);

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, baseName, false);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, baseName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(directoryPath,
                                                                 baseName,
                                                                 Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionOutwards.IO),
                                                                              nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter)),
                                                                 "ExpectedExport",
                                                                 28,
                                                                 8,
                                                                 3211);
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
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            });

            string directoryPath = TestHelper.GetScratchPadPath("Export_InvalidDirectoryRights_LogErrorAndReturnFalse");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");

            var exporter = new GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(failureMechanism, assessmentSection, filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    var isExported = true;
                    Action call = () => isExported = exporter.Export();

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. " +
                                             "Er zijn geen hydraulische randvoorwaarden locaties geëxporteerd.";
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
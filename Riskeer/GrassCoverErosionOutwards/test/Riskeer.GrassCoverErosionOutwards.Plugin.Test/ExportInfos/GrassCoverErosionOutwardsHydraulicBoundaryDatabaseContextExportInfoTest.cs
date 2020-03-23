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

using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.IO.Exporters;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContextExportInfoTest
    {
        private ExportInfo exportInfo;

        [SetUp]
        public void Setup()
        {
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                exportInfo = plugin.GetExportInfos()
                                   .Single(ei => ei.DataType == typeof(GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext)
                                                 && ei.Name.Equals("Waterstanden en golfhoogten uit marginale statistiek"));
            }
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual("shp", exportInfo.Extension);
            Assert.IsNotNull(exportInfo.CreateFileExporter);
            Assert.IsNotNull(exportInfo.IsEnabled);
            Assert.AreEqual("Algemeen", exportInfo.Category);
            TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, exportInfo.Image);
            Assert.IsNotNull(exportInfo.GetExportPath);
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection);

            // Call
            IFileExporter fileExporter = exportInfo.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter>(fileExporter);
            mockRepository.VerifyAll();
        }

        [Test]
        public void IsEnabled_HydraulicBoundaryLocationsEmpty_ReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = exportInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mockRepository.VerifyAll();
        }

        [Test]
        public void IsEnabled_HydraulicBoundaryLocationsNotEmpty_ReturnTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(0, "aName", 0, 0)
            });
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = exportInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }
    }
}
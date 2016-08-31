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

using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Plugin;
using RingtoetsCommonIoResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.ExportInfos
{
    [TestFixture]
    public class HydraulicBoundariesGroupContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            HydraulicBoundariesGroupContext context = new HydraulicBoundariesGroupContext(failureMechanism, assessmentSectionMock);
            string filePath = "test";

            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo info = GetInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, filePath);

                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsExporter>(fileExporter);
                mockRepository.VerifyAll();
            }
        }

        [Test]
        public void FileFilter_Always_ReturnsFileFilter()
        {
            // Setup
            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo info = GetInfo(plugin);

                // Call
                string fileFilter = info.FileFilter;

                // Assert
                Assert.AreEqual(RingtoetsCommonIoResources.DataTypeDisplayName_shape_file_filter, fileFilter);
            }
        }

        [Test]
        public void IsEnabled_GrassCoverErosionOutwardsHydraulicBoundaryLocationsEmpty_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            HydraulicBoundariesGroupContext context = new HydraulicBoundariesGroupContext(failureMechanism, assessmentSectionMock);

            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo info = GetInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
        }

        [Test]
        public void IsEnabled_GrassCoverErosionOutwardsHydraulicBoundaryLocationsNotEmpty_ReturnsTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(0, "aName", 0, 0)
                }
            });

            HydraulicBoundariesGroupContext context = new HydraulicBoundariesGroupContext(failureMechanism, assessmentSectionMock);

            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo info = GetInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
        }

        private static ExportInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(HydraulicBoundariesGroupContext));
        }
    }
}
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Hydraulics;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.IO;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.Revetment.Data;
using RingtoetsCommonIoResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.ExportInfos
{
    [TestFixture]
    public class HydraulicBoundariesGroupContextExportInfoTest
    {
        private ExportInfo hydraulicBoundaryLocationsExporterExportInfo;
        private ExportInfo waveConditionsExporterExportInfo;

        [SetUp]
        public void Setup()
        {
            using (GrassCoverErosionOutwardsPlugin plugin = new GrassCoverErosionOutwardsPlugin())
            {
                ExportInfo[] exportInfos = plugin.GetExportInfos().Where(ei => ei.DataType == typeof(HydraulicBoundariesGroupContext)).ToArray();
                hydraulicBoundaryLocationsExporterExportInfo = exportInfos.FirstOrDefault(ei => ei.Name.Equals("Waterstanden en golfhoogtes uit marginale statistiek (*.shp)."));
                waveConditionsExporterExportInfo = exportInfos.FirstOrDefault(ei => ei.Name.Equals("Berekende belastingen bij verschillende waterstanden (*.csv)."));
            }
        }

        [Test]
        public void HydraulicBoundaryLocationsExporterExportInfo_Name_Exists()
        {
            // Assert
            Assert.IsNotNull(hydraulicBoundaryLocationsExporterExportInfo);
        }

        [Test]
        public void HydraulicBoundaryLocationsExporterExportInfo_Category_GeneralCategory()
        {
            // Assert
            Assert.AreEqual("Algemeen", hydraulicBoundaryLocationsExporterExportInfo.Category);
        }

        [Test]
        public void HydraulicBoundaryLocationsExporterExportInfo_Image_ReturnsPointShapefileIcon()
        {
            // Call
            var icon = hydraulicBoundaryLocationsExporterExportInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.PointShapefileIcon, icon);
        }

        [Test]
        public void HydraulicBoundaryLocationsExporterExportInfo_CreateFileExporter_IsInstanceOfHydraulicBoundaryLocationsExporter()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            IFileExporter fileExporter = hydraulicBoundaryLocationsExporterExportInfo.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationsExporter>(fileExporter);
            mockRepository.VerifyAll();
        }

        [Test]
        public void HydraulicBoundaryLocationsExporterExportInfo_IsEnabledHydraulicBoundaryLocationsEmpty_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = hydraulicBoundaryLocationsExporterExportInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        public void HydraulicBoundaryLocationsExporterExportInfo_IsEnabledHydraulicBoundaryLocationsNotEmpty_ReturnsTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(new[] { new HydraulicBoundaryLocation(0, "aName", 0, 0) });

            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = hydraulicBoundaryLocationsExporterExportInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }

        [Test]
        public void HydraulicBoundaryLocationsExporterExportInfo_FileFilter_ShpFileFilter()
        {
            // Call
            string fileFilter = hydraulicBoundaryLocationsExporterExportInfo.FileFilter;

            // Assert
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilter);
        }

        [Test]
        public void WaveConditionsExporterExportInfo_Name_Exists()
        {
            // Assert
            Assert.IsNotNull(waveConditionsExporterExportInfo);
        }

        [Test]
        public void WaveConditionsExporterExportInfo_Category_GeneralCategory()
        {
            // Assert
            Assert.AreEqual("Algemeen", waveConditionsExporterExportInfo.Category);
        }

        [Test]
        public void WaveConditionsExporterExportInfo_Image_ReturnsCsvFileIcon()
        {
            // Call
            var icon = waveConditionsExporterExportInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralOutputIcon, icon);
        }

        [Test]
        public void WaveConditionsExporterExportInfo_IsInstanceOfGrassCoverErosionOutwardsWaveConditionsExporter()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            IFileExporter fileExporter = waveConditionsExporterExportInfo.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsExporter>(fileExporter);
            mockRepository.VerifyAll();
        }

        [Test]
        public void WaveConditionsExporterExportInfo_IsEnabledWaveConditionsCalculationGroupCalculationsEmpty_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = waveConditionsExporterExportInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        public void WaveConditionsExporterExportInfo_IsEnabledCalculationsWithoutOutput_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());

            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = waveConditionsExporterExportInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        public void WaveConditionsExporterExportInfo_IsEnabledCalculationsWithOutput_ReturnsTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            });

            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = waveConditionsExporterExportInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }

        [Test]
        public void WaveConditionsExporterExportInfo_FileFilter_CsvFileFilter()
        {
            // Call
            string fileFilter = waveConditionsExporterExportInfo.FileFilter;

            // Assert
            Assert.AreEqual("Kommagescheiden bestand (*.csv)|*.csv", fileFilter);
        }
    }
}
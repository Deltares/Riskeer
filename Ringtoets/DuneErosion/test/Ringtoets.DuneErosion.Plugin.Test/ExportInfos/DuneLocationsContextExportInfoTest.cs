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
using Core.Common.Gui;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.IO;

namespace Ringtoets.DuneErosion.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class DuneLocationsContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            using (DuneErosionPlugin plugin = new DuneErosionPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<DuneLocationsExporter>(fileExporter);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilter_Always_ReturnFileFilter()
        {
            // Setup
            using (DuneErosionPlugin plugin = new DuneErosionPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                FileFilterGenerator fileFilter = info.FileFilter;

                // Assert
                Assert.AreEqual("MorphAn randvoorwaarden (*.bnd)|*.bnd", fileFilter.Filter);
            }
        }

        [Test]
        public void IsEnabled_NoLocationsWithOutput_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocations.Add(new TestDuneLocation());
            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            using (DuneErosionPlugin plugin = new DuneErosionPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
        }

        [Test]
        public void IsEnabled_LocationsWithOutput_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocations.Add(new TestDuneLocation
                                               {
                                                   Output = new TestDuneLocationOutput()
                                               });
            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            using (DuneErosionPlugin plugin = new DuneErosionPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
        }

        private static ExportInfo GetExportInfo(DuneErosionPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(DuneLocationsContext));
        }
    }
}
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

using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Hydraulics;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                ExportInfo info = GetExportInfo(plugin);

                // Assert
                Assert.IsNotNull(info.CreateFileExporter);
                Assert.IsNotNull(info.IsEnabled);
                Assert.AreEqual("Hydraulische randvoorwaarden", info.Name);
                Assert.AreEqual("Algemeen", info.Category);
                TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, info.Image);
                Assert.IsNotNull(info.FileFilterGenerator);
            }
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 0, 0));

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };
            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);
            const string filePath = "test";

            using (var plugin = new RingtoetsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, filePath);

                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsExporter>(fileExporter);
            }
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnFileFilter()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = info.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void IsEnabled_HydraulicBoundaryDatabaseNotCoupled_ReturnFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
        }

        [Test]
        public void IsEnabled_HydraulicBoundaryDatabaseCoupled_ReturnTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "databaseFile"
                }
            };
            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
        }

        private static ExportInfo GetExportInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(HydraulicBoundaryDatabaseContext));
        }
    }
}
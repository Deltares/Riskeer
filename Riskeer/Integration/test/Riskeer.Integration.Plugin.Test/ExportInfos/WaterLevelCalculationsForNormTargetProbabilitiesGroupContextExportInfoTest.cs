﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Gui.Plugin;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.IO.Exporters;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class WaterLevelCalculationsForNormTargetProbabilitiesGroupContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                ExportInfo info = GetExportInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Name);
                Assert.AreEqual("zip", info.Extension);
                Assert.IsNotNull(info.CreateFileExporter);
                Assert.IsNotNull(info.IsEnabled);
                Assert.AreEqual("Algemeen", info.Category);
                TestHelper.AssertImagesAreEqual(CoreGuiResources.ExportIcon, info.Image);
                Assert.IsNotNull(info.GetExportPath);
            }
        }

        [Test]
        public void Name_Always_ReturnsName()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                string name = info.Name(null);

                // Assert
                Assert.AreEqual("Waterstanden bij norm", name);
            }
        }

        [Test]
        public void CreateFileExporter_WithContext_ReturnFileExporter()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var context = new WaterLevelCalculationsForNormTargetProbabilitiesGroupContext(
                new ObservableList<HydraulicBoundaryLocation>(), assessmentSection);

            const string filePath = "test";

            using (var plugin = new RiskeerPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, filePath);

                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter>(fileExporter);
            }
        }

        [Test]
        public void IsEnabled_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(null);

                // Assert
                Assert.IsTrue(isEnabled);
            }
        }

        private static ExportInfo GetExportInfo(RiskeerPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(WaterLevelCalculationsForNormTargetProbabilitiesGroupContext));
        }
    }
}
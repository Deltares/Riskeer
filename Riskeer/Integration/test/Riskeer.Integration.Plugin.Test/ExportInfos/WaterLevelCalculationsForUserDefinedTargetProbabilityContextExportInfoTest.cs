﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.IO.Exporters;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class WaterLevelCalculationsForUserDefinedTargetProbabilityContextExportInfoTest
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
                Assert.AreEqual("shp", info.Extension);
                Assert.IsNotNull(info.CreateFileExporter);
                Assert.IsNotNull(info.IsEnabled);
                Assert.AreEqual("Algemeen", info.Category);
                TestHelper.AssertImagesAreEqual(CoreGuiResources.ExportIcon, info.Image);
                Assert.IsNotNull(info.GetExportPath);
            }
        }

        [Test]
        public void Name_WithContext_ReturnsName()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1), assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                string name = info.Name(context);

                // Assert
                Assert.AreEqual("Waterstanden (1/10)", name);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateFileExporter_WithContext_ReturnFileExporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1), assessmentSection);

            const string filePath = "test";

            using (var plugin = new RiskeerPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, filePath);

                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter>(fileExporter);
            }

            mocks.VerifyAll();
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
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(WaterLevelCalculationsForUserDefinedTargetProbabilityContext));
        }
    }
}
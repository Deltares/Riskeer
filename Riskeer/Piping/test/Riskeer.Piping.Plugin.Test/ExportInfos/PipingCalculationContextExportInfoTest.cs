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
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.IO.Configurations;
using Riskeer.Piping.Primitives;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class PipingCalculationContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new PipingPlugin())
            {
                // Call
                ExportInfo info = GetExportInfo(plugin);

                // Assert
                Assert.IsNotNull(info.CreateFileExporter);
                Assert.IsNotNull(info.IsEnabled);
                Assert.AreEqual("Riskeer berekeningenconfiguratie", info.Name);
                Assert.AreEqual("Algemeen", info.Category);
                TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, info.Image);
                Assert.IsNotNull(info.FileFilterGenerator);
            }
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                               new CalculationGroup(),
                                                               Enumerable.Empty<PipingSurfaceLine>(),
                                                               Enumerable.Empty<PipingStochasticSoilModel>(),
                                                               new PipingFailureMechanism(),
                                                               assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<PipingCalculationConfigurationExporter>(fileExporter);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnFileFilter()
        {
            // Setup
            using (var plugin = new PipingPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = info.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Riskeer berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void IsEnabled_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                               new CalculationGroup(),
                                                               Enumerable.Empty<PipingSurfaceLine>(),
                                                               Enumerable.Empty<PipingStochasticSoilModel>(),
                                                               new PipingFailureMechanism(),
                                                               assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }

            mocks.VerifyAll();
        }

        private static ExportInfo GetExportInfo(PipingPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(PipingCalculationScenarioContext));
        }
    }
}
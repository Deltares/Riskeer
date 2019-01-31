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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.IO.Configurations;
using Riskeer.MacroStabilityInwards.Primitives;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationGroupContextExportInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
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

            var context = new MacroStabilityInwardsCalculationGroupContext(new CalculationGroup(),
                                                                           null,
                                                                           Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                           Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                           new MacroStabilityInwardsFailureMechanism(),
                                                                           assessmentSection);

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                IFileExporter fileExporter = info.CreateFileExporter(context, "test");

                // Assert
                Assert.IsInstanceOf<MacroStabilityInwardsCalculationConfigurationExporter>(fileExporter);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnFileFilter()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = info.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Riskeer berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void IsEnabled_CalculationGroupNoChildren_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new MacroStabilityInwardsCalculationGroupContext(new CalculationGroup(),
                                                                           null,
                                                                           Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                           Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                           new MacroStabilityInwardsFailureMechanism(),
                                                                           assessmentSection);

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void IsEnabled_CalculationGroupWithChildren_ReturnTrue(bool hasNestedGroup, bool hasCalculation)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            if (hasNestedGroup)
            {
                calculationGroup.Children.Add(new CalculationGroup());
            }

            if (hasCalculation)
            {
                calculationGroup.Children.Add(new MacroStabilityInwardsCalculation());
            }

            var context = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                           null,
                                                                           Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                           Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                           new MacroStabilityInwardsFailureMechanism(),
                                                                           assessmentSection);

            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                ExportInfo info = GetExportInfo(plugin);

                // Call
                bool isEnabled = info.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }

            mocks.VerifyAll();
        }

        private static ExportInfo GetExportInfo(MacroStabilityInwardsPlugin plugin)
        {
            return plugin.GetExportInfos().First(ei => ei.DataType == typeof(MacroStabilityInwardsCalculationGroupContext));
        }
    }
}
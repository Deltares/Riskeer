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
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.HeightStructures.IO.Configurations;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.HeightStructures.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class HeightStructuresCalculationGroupContextExportInfoTest
    {
        private HeightStructuresPlugin plugin;
        private ExportInfo info;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.Replay(gui);
            mocks.Replay(mainWindow);

            plugin = new HeightStructuresPlugin
            {
                Gui = gui
            };

            info = plugin.GetExportInfos().First(ei => ei.DataType == typeof(HeightStructuresCalculationGroupContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(info.CreateFileExporter);
            Assert.IsNotNull(info.IsEnabled);
            Assert.AreEqual("Riskeer berekeningenconfiguratie", info.Name);
            Assert.AreEqual("xml", info.Extension);
            Assert.AreEqual("Algemeen", info.Category);
            TestHelper.AssertImagesAreEqual(CoreCommonGuiResources.ExportIcon, info.Image);
            Assert.IsNotNull(info.GetExportPath);
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new HeightStructuresCalculationGroupContext(new CalculationGroup(),
                                                                      null,
                                                                      new HeightStructuresFailureMechanism(),
                                                                      assessmentSection);

            // Call
            IFileExporter fileExporter = info.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<HeightStructuresCalculationConfigurationExporter>(fileExporter);
        }

        [Test]
        public void IsEnabled_CalculationGroupNoChildren_ReturnFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new HeightStructuresCalculationGroupContext(new CalculationGroup(),
                                                                      null,
                                                                      new HeightStructuresFailureMechanism(),
                                                                      assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void IsEnabled_CalculationGroupWithChildren_ReturnTrue(bool hasNestedGroup, bool hasCalculation)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            if (hasNestedGroup)
            {
                calculationGroup.Children.Add(new CalculationGroup());
            }

            if (hasCalculation)
            {
                calculationGroup.Children.Add(new TestHeightStructuresCalculationScenario());
            }

            var context = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                      null,
                                                                      new HeightStructuresFailureMechanism(),
                                                                      assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }
    }
}
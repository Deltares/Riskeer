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
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.IO.Configurations;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class StabilityPointStructuresCalculationScenarioContextExportInfoTest
    {
        private StabilityPointStructuresPlugin plugin;
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

            plugin = new StabilityPointStructuresPlugin
            {
                Gui = gui
            };

            info = plugin.GetExportInfos().First(ei => ei.DataType == typeof(StabilityPointStructuresCalculationScenarioContext));
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
            TestHelper.AssertImagesAreEqual(CoreGuiResources.ExportIcon, info.Image);
            Assert.IsNotNull(info.GetExportPath);
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new StabilityPointStructuresCalculationScenarioContext(new TestStabilityPointStructuresCalculationScenario(),
                                                                                 new CalculationGroup(),
                                                                                 new StabilityPointStructuresFailureMechanism(),
                                                                                 assessmentSection);

            // Call
            IFileExporter fileExporter = info.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<StabilityPointStructuresCalculationConfigurationExporter>(fileExporter);
        }

        [Test]
        public void IsEnabled_Always_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new StabilityPointStructuresCalculationScenarioContext(new TestStabilityPointStructuresCalculationScenario(),
                                                                                 new CalculationGroup(),
                                                                                 new StabilityPointStructuresFailureMechanism(),
                                                                                 assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }
    }
}
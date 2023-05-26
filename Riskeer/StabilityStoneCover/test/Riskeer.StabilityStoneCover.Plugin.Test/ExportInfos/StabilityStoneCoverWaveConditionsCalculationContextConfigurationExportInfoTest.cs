﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.Contribution;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.IO.Configurations;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Plugin.Test.ExportInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationContextConfigurationExportInfoTest
    {
        private StabilityStoneCoverPlugin plugin;
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

            plugin = new StabilityStoneCoverPlugin
            {
                Gui = gui
            };

            info = plugin.GetExportInfos()
                         .Single(ei => ei.DataType == typeof(StabilityStoneCoverWaveConditionsCalculationContext)
                                       && ei.Name(null).Equals("Riskeer berekeningenconfiguratie"));
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
            Assert.AreEqual("xml", info.Extension);
            Assert.IsNotNull(info.CreateFileExporter);
            Assert.IsNotNull(info.IsEnabled);
            Assert.AreEqual("Algemeen", info.Category);
            TestHelper.AssertImagesAreEqual(CoreGuiResources.ExportIcon, info.Image);
            Assert.IsNotNull(info.GetExportPath);
        }

        [Test]
        public void CreateFileExporter_Always_ReturnFileExporter()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution).Return(new FailureMechanismContribution(0.1, 0.1));
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  new CalculationGroup(),
                                                                                  new StabilityStoneCoverFailureMechanism(),
                                                                                  assessmentSection);

            // Call
            IFileExporter fileExporter = info.CreateFileExporter(context, "test");

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverWaveConditionsCalculationConfigurationExporter>(fileExporter);
        }

        [Test]
        public void IsEnabled_Always_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(new StabilityStoneCoverWaveConditionsCalculation(),
                                                                                  new CalculationGroup(),
                                                                                  new StabilityStoneCoverFailureMechanism(),
                                                                                  assessmentSection);

            // Call
            bool isEnabled = info.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }
    }
}